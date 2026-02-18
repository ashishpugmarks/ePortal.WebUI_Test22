using ClosedXML.Excel;
using ePortal.DomainClasses;
using ePortal.Persistence.Admin.Services;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.ViewModels;
using System.Data;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;
using ePortal.Application.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.Persistence.Admin.Interface;
using ePortal.WebUI.Filters;
using System.Collections;
using ePortal.Persistence;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using ePortal.WebUI.Helpers;
using Org.BouncyCastle.Ocsp;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class SurveyConfigurationController : Controller
    {
        private readonly ISurveyService _ISurveyService;
        private readonly ICommonFunctions _cmmnFnctn;
        private readonly ISessionService _sessionService;
        private readonly ILogger<SurveyConfigurationController> _logger;
        private readonly IConfiguration _settings;
        private readonly IInformationSecurity objInfoSec;
        private readonly ITraining objTraining;
        private readonly IEmpUserDetails objAddDetail;

        private readonly ICommonFunctions _objCommon;
        private readonly IDepartment _oDept;
        private readonly IDivision _oDiv;
        private readonly IPMS _objPms;
        private readonly ExcelExportHelper _excelHelper;




        public SurveyConfigurationController(ISessionService sessionService, ILogger<SurveyConfigurationController> logger, ISurveyService ISurveyService, ICommonFunctions cmmnFnctn, IConfiguration settings, IInformationSecurity _objInfoSec, ITraining _objTraining,
            IEmpUserDetails _objAddDetail, ICommonFunctions objCommon, IDepartment oDept, IDivision oDiv, IPMS objPms)
        {
            _sessionService = sessionService;
            _logger = logger;
            _ISurveyService = ISurveyService;
            _cmmnFnctn = cmmnFnctn;
            _settings = settings;
            objInfoSec = _objInfoSec;
            objTraining = _objTraining;
            objAddDetail = _objAddDetail;
            _objCommon = objCommon;
            _oDept = oDept;
            _oDiv = oDiv;
            _objPms = objPms;
            _excelHelper = new ExcelExportHelper();
        }


        public ActionResult SurveyList()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            //sa CR6927
            List<PR_Div_Dep_SecViewModel> _List = new List<PR_Div_Dep_SecViewModel>();
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            _List = _ISurveyService.GetEmpListForSurvey();
            _List.Insert(0, new PR_Div_Dep_SecViewModel { Value = 0, Text = "--select--" });
            ViewBag.EmpList = new SelectList(_List, "Value", "Text");
            return View(_ISurveyService.ServerList().OrderByDescending(x => x.HRSECURITYSURVEYID).ToList());
            //ea CR6927 
        }

        public ActionResult SurveyAutoFillList()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            return View(_ISurveyService.FetchHRSurveyAutoFillList().OrderBy(x => x.AUTOFILLID).ToList());
        }

        //sa CR6927
        [HttpPost]
        public IActionResult RemoveQuestion([FromBody] QuestionViewModel Quest)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

          
                List<QuestionViewModel> Quslist = new List<QuestionViewModel>();
                Quest.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                var IsDeleted = _ISurveyService.RemoveQuestion(Quest.QusId);
                if (IsDeleted == 1)
                {
                    Quslist = JsonConvert.DeserializeObject<List<QuestionViewModel>>(TempData["Question"]?.ToString());     //(List<QuestionViewModel>)TempData["Question"];
                    Quslist = Quslist == null ? new List<QuestionViewModel>() : Quslist;
                    int index = Quslist.FindIndex(x => x.QusId == Quest.QusId);
                    if (index != -1)
                    {
                        var v_question = Quslist.FirstOrDefault(r => r.QusId == Quest.QusId);
                        Quslist.Remove(v_question);
                    }
                    else
                    {
                        Quslist.Add(Quest);
                    }
                }
                TempData["Question"] = JsonConvert.SerializeObject(Quslist);
                TempData.Keep("Question");
                Quslist.ForEach(x =>
                {
                    x.isChildPresent = Quslist.Any(q => q.SUBQUESTIONPARENTID == x.QusId) ? 1 : 0;
                });

                return Json(Quslist);
        }


        [HttpPost]
        public ActionResult SurveyList([FromBody] SearchSurveyViewModel SI)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            var list = _ISurveyService.GetSurveyList(SI).OrderByDescending(x => x.HRSECURITYSURVEYID).ToList();
            return PartialView("_viewSurveyList", list);
        }
        //ea CR6927

        [HttpPost]
        public JsonResult UpdateSurveyAutoFill(int autoFillId, int status)
        {
            try
            {
                string result = _ISurveyService.UpdateHRSurveyAutoFillList(autoFillId, modifiedBy: Convert.ToInt32(_sessionService.Get<string>("userID")), status: status);
                string[] strStatusRes = result.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strStatusRes[0]);
                string errMsg = Convert.ToString(strStatusRes[1]);
                if (errResult == "1")
                {
                    return Json(new { success = true, message = "Update successful" });
                }
                else
                {
                    return Json(new { success = false, message = "Error: " + errMsg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetHrSurveyEmployeeDetails()
        {
            try
            {
                var employeeDetails = _ISurveyService.GetHrSurveyEmployeeDetails();
                if (employeeDetails == null || !employeeDetails.Any())
                {
                    return Json(new { success = true, error = "No employee data found." });
                }
                return Json(new { success = true, employeeDetails });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "An error occurred while fetching the data.", exception = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AddUserHrSurveyAutoFill(int employeeId, int status)
        {
            try
            {
                string result = _ISurveyService.AddUserHrSurveyAutoFill(employeeId, createdBy: Convert.ToInt32(_sessionService.Get<string>("userID")), modifiedBy: Convert.ToInt32(_sessionService.Get<string>("userID")), status: status);

                string[] strStatusRes = result.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strStatusRes[0]);
                string errMsg = Convert.ToString(strStatusRes[1]);
                if (errResult == "1")
                {
                    return Json(new { success = true, message = errMsg, errResult });
                }
                else if (errResult == "2")
                {
                    return Json(new { success = true, message = errMsg, errResult });
                }
                else
                {
                    return Json(new { success = false, message = "Error: " + errMsg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult CreateSurvey(int? id)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            SurveyViewModel vwmain = new SurveyViewModel();
            //IEnumerable<ADFUNCTIONALDESIGNATION> FnDesItems = _ISurveyService.Bind_ADFunctionalDesignation();// Comment by Aumento on 29-05-2024 :: SR71845
            IEnumerable<ADDESIGNATION> FnDesItems = _ISurveyService.Bind_ADDesignation();// Updated by Aumento on 29-05-2024 :: SR71845
            IEnumerable<SYSITE> SYSiteItems = _ISurveyService.Bind_SYSite();
            IEnumerable<OperationViewModel> OperationData = _ISurveyService.BindOperation(1);
            IEnumerable<DivisionViewModel> DivisionData = _ISurveyService.BindDivision(2, 0);
            //ViewBag.FunctionalDesignation = new MultiSelectList(FnDesItems, "ADFUNCTIONALDESIGNATIONID", "DESCRIP");// Comment by Aumento on 29-05-2024 :: SR71845
            ViewBag.Designation = new MultiSelectList(FnDesItems, "ADDESIGNATIONID", "DESCRIP");// Updated by Aumento on 29-05-2024 :: SR71845
            ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            ViewBag.DIVISION = new MultiSelectList(DivisionData, "ADDIVISIONID", "DESCRIP");
            ViewBag.OPERATION = new MultiSelectList(OperationData, "OPERATIONID", "OPERATION");
            TempData.Remove("Question");


            //CommonFunctions fc = new CommonFunctions();

            string paramvalue = _cmmnFnctn.GetParameterValue("HCGSURVEYAPPECODE");
            string strecode = _sessionService.Get<string>("userID").ToString();
            if (paramvalue.Split(',').Contains(strecode))
                vwmain.ISHCGAUTHECODE = true;
            else
                vwmain.ISHCGAUTHECODE = false;


            if (id == null)
            {
                var autoFillRecord = _ISurveyService.GetHRSurveyAutoFillList(strecode);
                if (autoFillRecord)
                {
                    ViewBag.IsActive = true;
                }
                else
                {
                    ViewBag.IsActive = false;
                }
            }

            vwmain.HRSURVEYSTARTDATE_STRING = DateTime.Today.AddDays(1).ToString("dd-MMM-yyyy");
                vwmain.HRSURVEYENDDATE_STRING = DateTime.Today.AddDays(1).ToString("dd-MMM-yyyy");

            return View(vwmain);
        }

        //add new & edit question
        [HttpGet]
        public ActionResult AddQuestion(int? id, Boolean Ret, int? parentId)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            QuestionViewModel QuestionList = new QuestionViewModel();
            if (id != null)
            {
                var autoFillRecord = _ISurveyService.GetHRSurveyAutoFillList(_sessionService.Get<string>("userID"));
                if (autoFillRecord)
                {
                    ViewBag.IsActive = true;
                }
                else
                {
                    ViewBag.IsActive = false;
                }
            }
            if (id > 0)
            {
                
                if (TempData["Question"] != null)
                {
                    //List<QuestionViewModel> Ilist = (List<QuestionViewModel>)TempData["Question"];
                    List<QuestionViewModel> Ilist = JsonConvert.DeserializeObject<List<QuestionViewModel>>(TempData["Question"]?.ToString());
                    TempData.Keep("Question");

                    var _obj = Ilist.Where(r => r.QusId == id).FirstOrDefault();
                    if (_obj != null)
                    {
                        QuestionList.MappingList = _obj.MappingList;
                        QuestionList.QUSDESCRIPTION = _obj.QUSDESCRIPTION;
                        QuestionList.QUSDESCRIPTIONHINDI = _obj.QUSDESCRIPTIONHINDI;
                        QuestionList.QUSDESCRIPTIONKANNADA = _obj.QUSDESCRIPTIONKANNADA;
                        QuestionList.QUSDESCRIPTIONGUJARATI = _obj.QUSDESCRIPTIONGUJARATI;
                        QuestionList.QUSDESCRIPTIONJAPANESE = _obj.QUSDESCRIPTIONJAPANESE;
                        foreach (var item in QuestionList.MappingList)
                        {
                            QuestionList.ISCORRECT = item.ISCORRECT;
                            QuestionList.Status = item.Status;
                        }
                        QuestionList.ISHCGSURVEY = Ret;
                        QuestionList.QusId = _obj.QusId;

                        QuestionList.OPTIONTYPE = _obj.OPTIONTYPE;
                        QuestionList.SUBQUESTIONPARENTID = _obj.SUBQUESTIONPARENTID;
                        QuestionList.ISSUBQUESTION = _obj.ISSUBQUESTION;
                        QuestionList.TRIGGEROPTIONIDS = _obj.TRIGGEROPTIONIDS;
                    }
                    if (parentId > 0)
                    {
                        var _parentobj = Ilist.Where(r => r.QusId == parentId).FirstOrDefault();
                        if (_parentobj != null)
                        {
                            QuestionList.ParentMappingList = _parentobj.MappingList;
                            QuestionList.PARENTQUSDESCRIPTION = _parentobj.QUSDESCRIPTION;
                            QuestionList.SUBQUESTIONPARENTID = _parentobj.QusId;
                            QuestionList.ISSUBQUESTION = 1;
                        }
                    }
                    //return PartialView("_createOrEditanswer", QuestionList);
                }
                else
                {
                    return PartialView("_createOrEditanswer", _ISurveyService.GetAllAnswer(id));
                }
            }
            else
            {
                QuestionList.MappingList = new List<AnswerViewModel>();
                QuestionList.ISHCGSURVEY = Ret;
                QuestionList.OPTIONTYPE = 1;
                QuestionList.ISSUBQUESTION = 0;
            }
            return PartialView("_createOrEditanswer", QuestionList);
        }

        [HttpGet]
        public ActionResult AddSubQuestion(int? id, Boolean Ret, int? parentId)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            QuestionViewModel QuestionList = new QuestionViewModel();
            if (id != null)
            {
                var autoFillRecord = _ISurveyService.GetHRSurveyAutoFillList(_sessionService.Get<string>("userID"));
                if (autoFillRecord)
                {
                    ViewBag.IsActive = true;
                }
                else
                {
                    ViewBag.IsActive = false;
                }
            }
            //TempData.Keep();

            //List<QuestionViewModel> Ilist = (List<QuestionViewModel>)TempData["Question"];
            List<QuestionViewModel> Ilist = JsonConvert.DeserializeObject<List<QuestionViewModel>>(TempData["Question"]?.ToString());
            TempData.Keep("Question"); 


            var _obj = Ilist.Where(r => r.QusId == parentId).FirstOrDefault();
            if (_obj != null)
            {
                QuestionList.ParentMappingList = _obj.MappingList;
                QuestionList.PARENTQUSDESCRIPTION = _obj.QUSDESCRIPTION;
                QuestionList.ISHCGSURVEY = Ret;
                QuestionList.SUBQUESTIONPARENTID = _obj.QusId;
                QuestionList.ISSUBQUESTION = 1;
                QuestionList.OPTIONTYPE = 1;
                QuestionList.TRIGGEROPTIONIDS = _obj.TRIGGEROPTIONIDS;
            }

            return PartialView("_createOrEditanswer", QuestionList);
        }


        //edit question popup
        [HttpPost]
        //public ActionResult AddQuestion([FromBody] QuestionViewModel Quest, int? id, int? parentId)
        public IActionResult AddQuestion([FromBody] QuestionViewModel_VM Ques1)
        {
             List<QuestionViewModel> Quslist = new List<QuestionViewModel>();
            if (Ques1 == null) return View();

            QuestionViewModel Quest = new QuestionViewModel();

            Quest.QusId = Ques1.QusId;
            Quest.QUSDESCRIPTION = Ques1.QUSDESCRIPTION;
            Quest.QUSDESCRIPTIONHINDI = Ques1.QUSDESCRIPTIONHINDI;
            Quest.QUSDESCRIPTIONKANNADA = Ques1.QUSDESCRIPTIONKANNADA;
            Quest.QUSDESCRIPTIONGUJARATI = Ques1.QUSDESCRIPTIONGUJARATI;
            Quest.QUSDESCRIPTIONJAPANESE = Ques1.QUSDESCRIPTIONJAPANESE;
            Quest.OPTIONTYPE = Convert.ToInt16(Ques1.OPTIONTYPE);
            Quest.MappingList = Ques1.MappingList;
            Quest.SUBQUESTIONPARENTID = Ques1.SUBQUESTIONPARENTID;
            Quest.ISSUBQUESTION = Convert.ToInt16(Ques1.ISSUBQUESTION);
            Quest.TRIGGEROPTIONIDS = Ques1.TRIGGEROPTIONIDS;
            Quest.HRSECURITYSURVEYID = Ques1.HRSECURITYSURVEYID;


            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
           

            //code changes start
            //if (TempData["Question"] != null)
            //{
            //    Quslist = (List<QuestionViewModel>)TempData["Question"];
            //    int index = Quslist.FindIndex(x => x.QusId == Quest.QusId);

            //    if (index != -1)
            //    {
            //        Quslist[index] = Quest;
            //    }
            //    else
            //    {
            //        Quslist.Add(Quest);
            //    }
            //}
            //else
            //{
            //    Quslist.Add(Quest);
            //}
            Quest.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
            Quest = _ISurveyService.AddQuestionDetails(Quest);
            if (Quest != null)
            {
                //Quslist = (List<QuestionViewModel>)TempData["Question"];
                Quslist = TempData["Question"]==null?null:JsonConvert.DeserializeObject<List<QuestionViewModel>>(TempData["Question"].ToString());

                Quslist = Quslist == null ? new List<QuestionViewModel>() : Quslist;
                int index = Quslist.FindIndex(x => x.QusId == Quest.QusId);
                if (index != -1)
                {
                    Quslist[index] = Quest;
                }
                else
                {
                    Quslist.Add(Quest);
                }

                //int questionIdChange = Quslist.FindIndex(x => x.QusId == Quest.QusId);

                //if (questionIdCahnge != -1)
                //{
                //    Quslist[questionIdCahnge].QusId = retVal;
                //}

            }

            //if (Quest.ISSUBQUESTION)
            //{
            //    Quest.SUBQUESTIONPARENTID = parentId;
            //    Quest.SelectedParentOptionIds = Quest.SelectedParentOptionIds ?? new List<long>();
            //}
            TempData["Question"] = JsonConvert.SerializeObject(Quslist);
            TempData.Keep("Question");
            //code changes end
            Quslist.ForEach(x =>
            {
                x.isChildPresent = Quslist.Any(q => q.SUBQUESTIONPARENTID == x.QusId) ? 1 : 0;
            });
            return Json(Quslist);
        }

        [HttpPost]
        public ActionResult ModifyQuestion(QuestionViewModel Quest, int id)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            List<QuestionViewModel> Quslist = new List<QuestionViewModel>();
            if (TempData["Question"] != null)
            {
                //Quslist = (List<QuestionViewModel>)TempData["Question"];
                Quslist = JsonConvert.DeserializeObject<List<QuestionViewModel>>(TempData["Question"]?.ToString());
            }
            Quslist.First(r => r.QusId == id);
            TempData["Question"] = JsonConvert.SerializeObject(Quslist);
            TempData.Keep("Question");

            return Json(Quslist);
        }

        [HttpPost]
        public ActionResult CreateSurvey([FromBody] SurveyViewModel SV)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            List<QuestionViewModel> Quslist = new List<QuestionViewModel>();
            if (TempData["Question"] != null)
            {
                //Quslist = (List<QuestionViewModel>)TempData["Question"];
                Quslist = JsonConvert.DeserializeObject<List<QuestionViewModel>>(TempData["Question"]?.ToString());
            }
            long retVal = 0;
            SV.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
            SV.Questionlist = Quslist;

            //Survey enhancement start - Check correct answer for Result based survey
            bool IsCorrect = true;
            if (SV.HRSURVEYMODULETYPE == "RBS")
            {
                if (SV.Questionlist != null)
                {
                    foreach (var Q_item in Quslist)
                    {
                        if (Q_item.MappingList.Where(x => x.ISCORRECT == 1).Count() < 1)
                        {
                            IsCorrect = false;
                        }
                    }
                }
            }

            if (IsCorrect == false)
            {
                retVal = -1;
            }
            else
            {
                retVal = _ISurveyService.CreateSurvey(SV);
            }
            //retVal = _ISurveyService.CreateSurvey(SV);
            //Survey enhancement end

            return Json(retVal);
        }

        [HttpGet]
        public IActionResult EditSurvey(int? id)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            //IEnumerable<ADFUNCTIONALDESIGNATION> FnDesItem = _ISurveyService.Bind_ADFunctionalDesignation();   // Comment by Aumento on 29-05-2024 :: SR71845
            IEnumerable<ADDESIGNATION> FnDesItem = _ISurveyService.Bind_ADDesignation();// Updated by Aumento on 29-05-2024 :: SR71845
            IEnumerable<SYSITE> SYSiteItem = _ISurveyService.Bind_SYSite();
            IEnumerable<OperationViewModel> OperationData = _ISurveyService.BindOperation(1);
            IEnumerable<DivisionViewModel> DivisionData = _ISurveyService.BindDivision(2, 0);
            //ViewBag.FunctionalDesignation = new MultiSelectList(FnDesItems, "ADFUNCTIONALDESIGNATIONID", "DESCRIP");// Comment by Aumento on 29-05-2024 :: SR71845
            ViewBag.Designation = new SelectList(FnDesItem, "ADDESIGNATIONID", "DESCRIP");// Updated by Aumento on 29-05-2024 :: SR71845
            ViewBag.SYSITE = new SelectList(SYSiteItem, "SYSITEID", "DESCRIP");
            ViewBag.DIVISION = new MultiSelectList(DivisionData, "ADDIVISIONID", "DESCRIP");
            ViewBag.OPERATION = new MultiSelectList(OperationData, "OPERATIONID", "OPERATION");
            var _result = _ISurveyService.GetEditById(id);

            if (_result != null)
            {
                _result.HRSURVEYSTARTDATE_STRING =
                    _result.HRSURVEYSTARTDATE.HasValue
                        ? _result.HRSURVEYSTARTDATE.Value.ToString("dd-MMM-yyyy")
                        : string.Empty;

                _result.HRSURVEYENDDATE_STRING =
                    _result.HRSURVEYENDDATE.HasValue
                        ? _result.HRSURVEYENDDATE.Value.ToString("dd-MMM-yyyy")
                        : string.Empty;
            }

            _result.Questionlist.ForEach(x =>
            {
                x.isChildPresent = _result.Questionlist.Any(q => q.SUBQUESTIONPARENTID == x.QusId) ? 1 : 0;
            });

            TempData["Question"] = JsonConvert.SerializeObject(_result.Questionlist.ToList());
            TempData.Peek("Question").ToString();

            if (id != null)
            {
                var autoFillRecord = _ISurveyService.GetHRSurveyAutoFillList(_sessionService.Get<string>("userID"));
                if (autoFillRecord)
                {
                    ViewBag.IsActive = true;
                }
                else
                {
                    ViewBag.IsActive = false;
                }
            }
            return View(_result);
        }

        
        [HttpPost]
        //public ActionResult EditSurvey([FromBody] SurveyViewModel SV, [FromRoute] int? id)
        public IActionResult EditSurvey(SurveyViewModel SV, int? id)
        {
            short retVal = 0;

            try
            {                
                if (SV == null)
                {
                    return Json(retVal);
                }


                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                List<QuestionViewModel> Quslist = new List<QuestionViewModel>();
                if (TempData["Question"] != null)
                {
                    //Quslist = (List<QuestionViewModel>)TempData["Question"];
                    Quslist = JsonConvert.DeserializeObject<List<QuestionViewModel>>(TempData["Question"]?.ToString());
                }

                SV.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                SV.Questionlist = Quslist;

                //Survey enhancement start - Check correct answer for Result based survey
                bool IsCorrect = true;
                if (SV.HRSURVEYMODULETYPE == "RBS")
                {
                    if (SV.Questionlist != null)
                    {
                        foreach (var Q_item in Quslist)
                        {
                            if (Q_item.MappingList.Where(x => x.ISCORRECT == 1).Count() < 1)
                            {
                                IsCorrect = false;
                            }
                        }
                    }
                }

                if (IsCorrect == false)
                {
                    retVal = -1;
                }
                else
                {
                    retVal = _ISurveyService.EditSurvey(SV, id);
                }
                //retVal = _ISurveyService.EditSurvey(SV, id);
                //Survey enhancement end
            }
            catch (Exception ex)
            {
                _logger.LogError("In Exception " + ex.Message);
                return Json(retVal);
            }

            return Json(retVal);
        }

        [HttpGet]
        public ActionResult SurveyReTest(int id)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            //IEnumerable<ADFUNCTIONALDESIGNATION> FnDesItems = _ISurveyService.Bind_ADFunctionalDesignation();  // Comment by Aumento on 29-05-2024 :: SR71845
            IEnumerable<ADDESIGNATION> FnDesItems = _ISurveyService.Bind_ADDesignation();// Updated by Aumento on 29-05-2024 :: SR71845
            IEnumerable<SYSITE> SYSiteItems = _ISurveyService.Bind_SYSite();
            IEnumerable<OperationViewModel> OperationData = _ISurveyService.BindOperation(1);
            IEnumerable<DivisionViewModel> DivisionData = _ISurveyService.BindDivision(2, 0);
            //ViewBag.FunctionalDesignation = new MultiSelectList(FnDesItems, "ADFUNCTIONALDESIGNATIONID", "DESCRIP");// Comment by Aumento on 29-05-2024 :: SR71845
            ViewBag.Designation = new MultiSelectList(FnDesItems, "ADDESIGNATIONID", "DESCRIP");// Updated by Aumento on 29-05-2024 :: SR71845
            ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            ViewBag.DIVISION = new MultiSelectList(DivisionData, "ADDIVISIONID", "DESCRIP");
            ViewBag.OPERATION = new MultiSelectList(OperationData, "OPERATIONID", "OPERATION");
            var _res = _ISurveyService.GetDetail(id);

            _res.Questionlist.ForEach(x =>
            {
                x.isChildPresent = _res.Questionlist.Any(q => q.SUBQUESTIONPARENTID == x.QusId) ? 1 : 0;
            });

            TempData["Question"] = JsonConvert.SerializeObject(_res.Questionlist.ToList());
            TempData.Keep("Question");
            //TempData.Peek("Question").ToString();

            if (_res != null)
            {
                _res.HRSURVEYSTARTDATE_STRING =
                    _res.HRSURVEYSTARTDATE.HasValue
                        ? _res.HRSURVEYSTARTDATE.Value.ToString("dd-MMM-yyyy")
                        : string.Empty;

                _res.HRSURVEYENDDATE_STRING =
                    _res.HRSURVEYENDDATE.HasValue
                        ? _res.HRSURVEYENDDATE.Value.ToString("dd-MMM-yyyy")
                        : string.Empty;
            }


            if (id != null)
            {
                var autoFillRecord = _ISurveyService.GetHRSurveyAutoFillList(_sessionService.Get<string>("userID"));
                if (autoFillRecord)
                {
                    ViewBag.IsActive = true;
                }
                else
                {
                    ViewBag.IsActive = false;
                }
            }
            return View("SurveyReTest", _res);
        }

        [HttpPost]
        public ActionResult SurveyReTest(SurveyViewModel SV, int id)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            short retVal = 0;
            SV.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
            long userid = SV.ADDEDBY;
            SV.HRSECURITYSURVEYID = id;
            //List<QuestionViewModel> Quslist = new List<QuestionViewModel>();
            //if (TempData["Question"] != null)
            //{
            //    Quslist = (List<QuestionViewModel>)TempData["Question"];
            //}
            //SV.Questionlist = Quslist;
            retVal = _ISurveyService.AddPercentage(SV, id);
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult SurveyRetestFilterTempData(string selectedQuestion)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            var selectedQuestions = JsonConvert.DeserializeObject<List<QuestionViewModel>>(selectedQuestion);

            List<QuestionViewModel> Quslist = new List<QuestionViewModel>();
            if (TempData["Question"] != null)
            {
                //Quslist = (List<QuestionViewModel>)TempData["Question"];
                Quslist = JsonConvert.DeserializeObject<List<QuestionViewModel>>(TempData["Question"]?.ToString());
            }

            var filteredQuestions = Quslist.Where(q => selectedQuestions.Any(sq => sq.QusId == q.QusId)).ToList();

            TempData["Question"] = JsonConvert.SerializeObject(filteredQuestions);
            TempData.Keep("Question");
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> TextDataAutofill(string text, string targetLang)
        {
            try
            {
                var langTrans = _settings["GeneralSettings:langTrans"].ToString(); // ConfigurationManager.AppSettings["langTrans"].ToString();

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                string encodedText = HttpUtility.UrlEncode(text);
                var response = await client.GetStringAsync(langTrans + targetLang + "&q=" + encodedText);

                // Check if the response body contains data
                if (!string.IsNullOrEmpty(response))
                {
                    //var responseData = await response.Content.ReadAsStringAsync();

                    return Json(new { translatedText = response });
                }
                else
                {
                    return Json(new { error = "Translation failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in TextDataAutofill(). Message:" + Convert.ToString(ex.Message));
                return Json(new { error = "Translation failed" });
            }            
        }

        [HttpGet]
        public ActionResult UserDashboard()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            var GetQuestionResult = _ISurveyService.UserDashboard();
            ViewBag.SurveyName = GetQuestionResult.Select(r => r.SurveyName).FirstOrDefault();
            return View(GetQuestionResult);
        }
        //Added By Aumento as On 10062024 Start
        [HttpGet]
        public ActionResult InformationSecuritySurvey()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            var model = new InformationSecuritySurveyViewModel();
            //InformationSecurity objInfoSec = new InformationSecurity();
            //Training objTraining = new Training();
            DataTable oDs = new DataTable();
            DataTable dts = new DataTable();
            DataTable dtsn = new DataTable();
            Employee_Details Emp = new Employee_Details();
            var questions = new List<Question>();
            int SurveyNumber = 0;
            dtsn = objInfoSec.GetActiveSurveyWithTM(Convert.ToInt32(_sessionService.Get<string>("userID")));
            if (dtsn.Rows.Count > 0)
            {
                SurveyNumber = Convert.ToInt16(dtsn.Rows[0]["HRSECURITYSURVEYID"].ToString());
            }
            Boolean isQuizFilled = false;
            isQuizFilled = objInfoSec.IsQuizCompleted(_sessionService.Get<string>("userID"), SurveyNumber);
            if (isQuizFilled == true)
            {
                return RedirectToAction("Home", "Home");
                //Response.Redirect("../../Home/Home", false);
                //Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                string SurveyType = "";
                dts = objInfoSec.GetPartSurveyList(SurveyNumber.ToString());
                if (dts.Rows.Count > 0)
                {
                    //lblSurveyHeading.Text = dts.Rows[0]["HRSURVEYDESC"].ToString();
                    model.SurveyHeading = dts.Rows[0]["HRSURVEYDESC"].ToString();
                    model.SurveyHeadingHindi = dts.Rows[0]["HRSURVEYDESCHINDI"].ToString();
                    model.SurveyHeadingKannada = dts.Rows[0]["HRSURVEYDESCKANNADA"].ToString();
                    model.SurveyHeadingGujarati = dts.Rows[0]["HRSURVEYDESCGUJARATI"].ToString();
                    model.SurveyHeadingJapanese = dts.Rows[0]["HRSURVEYDESCJAPANESE"].ToString();
                    //lblSurveydesc.Text = dts.Rows[0]["HRSURVEYHEADER"].ToString();
                    model.SurveyDescription = dts.Rows[0]["HRSURVEYHEADER"].ToString();
                    model.SurveyDescriptionHindi = dts.Rows[0]["HRSURVEYHEADERHINDI"].ToString();
                    model.SurveyDescriptionKannada = dts.Rows[0]["HRSURVEYHEADERKANNADA"].ToString();
                    model.SurveyDescriptionGujarati = dts.Rows[0]["HRSURVEYHEADERGUJARATI"].ToString();
                    model.SurveyDescriptionJapanese = dts.Rows[0]["HRSURVEYHEADERJAPANESE"].ToString();
                    //hdemailstatus.Value = dts.Rows[0]["SURVEYRESULTMAIL"].ToString();
                    model.SURVEYRESULTMAIL = dts.Rows[0]["SURVEYRESULTMAIL"].ToString();
                    //hdemailack.Value = dts.Rows[0]["SURVEYACKMAIL"].ToString();
                    model.SURVEYACKMAIL = dts.Rows[0]["SURVEYACKMAIL"].ToString();
                    //hdremarksstatus.Value = dts.Rows[0]["FEEDBACKSTATUS"].ToString();
                    model.FEEDBACKSTATUS = dts.Rows[0]["FEEDBACKSTATUS"].ToString();
                    //hdisremekmand.Value = dts.Rows[0]["ISREMARKSMANDATORY"].ToString();
                    model.ISREMARKSMANDATORY = dts.Rows[0]["ISREMARKSMANDATORY"].ToString();
                    SurveyType = dts.Rows[0]["SURVEYTYPE"].ToString();
                    model.SURVEYTYPE = dts.Rows[0]["SURVEYTYPE"].ToString();
                    model.SurveyNumber = SurveyNumber.ToString();
                    model.ISFAMILYDECREQ = (dts.Rows[0]["ISHCGSURVEY"].ToString() == "1" ? true : false);
                    //if (model.FEEDBACKSTATUS == "1")
                    //{
                    //    //tr_remarks.Visible = true;
                    //    if (model.ISREMARKSMANDATORY == "1")
                    //    {
                    //        // spn_mandatory.Visible = true;
                    //    }
                    //    else
                    //    {
                    //        // spn_mandatory.Visible = false;
                    //    }
                    //}
                }
                if (SurveyType == "2")
                {
                    oDs = objInfoSec.GetInformationSecurityQuiz_QuestionFixed(SurveyNumber, Convert.ToInt64(_sessionService.Get<string>("userID")));
                }
                else
                {
                    oDs = objInfoSec.GetInformationSecurityQuiz_Question(SurveyNumber);
                }

                foreach (DataRow row in oDs.Rows)
                {
                    var question = new Question
                    {
                        Id = Convert.ToInt32(row["HRSECURITYQUSLISTID"]),
                        Description = row["QUSDESCRIPTION"].ToString(),
                        DescriptionHindi = row["QUSDESCRIPTIONHINDI"].ToString(),
                        DescriptionKannada = row["QUSDESCRIPTIONKANNADA"].ToString(),
                        DescriptionGujarati = row["QUSDESCRIPTIONGUJARATI"].ToString(),
                        DescriptionJapanese = row["QUSDESCRIPTIONJAPANESE"].ToString(),
                        OptionType = row["OPTIONTYPE"].ToString(),
                        IsSubQuestion = row["ISSUBQUESTION"] != DBNull.Value ? Convert.ToInt16(row["ISSUBQUESTION"]) : 0,
                        SubQuestionParentId = row["SUBQUESTIONPARENTID"] != DBNull.Value ? Convert.ToInt32(row["SUBQUESTIONPARENTID"]) : 0,
                        TriggerOptionIds = row["TRIGGEROPTIONIDS"].ToString(),
                        Options = GetOptionsFromDataTable(Convert.ToInt32(row["HRSECURITYQUSLISTID"]))
                    };
                    questions.Add(question);
                }
                int skipcount = objInfoSec.GetSurveyskipCount(SurveyNumber, _sessionService.Get<string>("userID"));
                model.skipcount = skipcount.ToString();
            }

            model.Questions = new List<Question>();


            questions.Where(Q => Q.IsSubQuestion == 0).ToList().ForEach(X =>
            {
                Question Que = questions.Where(q => X.Id == q.SubQuestionParentId).FirstOrDefault();
                if (Que != null)
                {
                    X.TriggerOptionIds = Que.TriggerOptionIds.ToString();
                    X.SubQuestionParentId = Que.Id;
                    Que.IsVisible = "hidden ml-30";
                    Que.TriggerOptionIds = "";
                    Que.SubQuestionParentId = 0;
                }
                model.Questions.Add(X);
                if (Que != null)
                {
                    model.Questions.Add(Que);
                }
            });


            return View(model);
        }
        private List<AnsOptions> GetOptionsFromDataTable(int QuestionID)
        {
            var AnsOptions = new List<AnsOptions>();
            DataTable ds;
            //InformationSecurity objInfoSec = new InformationSecurity();
            ds = objInfoSec.GetInformationSecurityQuiz_Answer(QuestionID);
            foreach (DataRow row in ds.Rows)
            {
                var Ans = new AnsOptions
                {
                    ANSDESCRIPTION = row["ANSDESCRIPTION"].ToString(),
                    ANSDESCRIPTIONHINDI = row["ANSDESCRIPTIONHINDI"].ToString(),
                    ANSDESCRIPTIONKANNADA = row["ANSDESCRIPTIONKANNADA"].ToString(),
                    ANSDESCRIPTIONGUJARATI = row["ANSDESCRIPTIONGUJARATI"].ToString(),
                    ANSDESCRIPTIONJAPANESE = row["ANSDESCRIPTIONJAPANESE"].ToString(),
                    HRSECURITYANSLISTID = row["HRSECURITYANSLISTID"].ToString(),
                    ISFAMILYDECREQ = (row["ISFAMILYDEC_REQ"].ToString() == "1" ? true : false)
                };
                AnsOptions.Add(Ans);
            }
            return AnsOptions;
        }
        [HttpPost]
        public ActionResult InformationSecuritySurvey(InformationSecuritySurveyViewModel model, string remark, int skipcount)
        {
            try
            {
                // Changed by TTl on 02-July-2025 against CR6641 - START
                model.SurveyHeading = WebUtility.UrlDecode(model.SurveyHeading);
                model.SurveyHeadingHindi = WebUtility.UrlDecode(model.SurveyHeadingHindi);
                model.SurveyHeadingKannada = WebUtility.UrlDecode(model.SurveyHeadingKannada);
                model.SurveyHeadingGujarati = WebUtility.UrlDecode(model.SurveyHeadingGujarati);
                model.SurveyHeadingJapanese = WebUtility.UrlDecode(model.SurveyHeadingJapanese);

                model.SurveyDescription = WebUtility.UrlDecode(model.SurveyDescription);
                model.SurveyDescriptionHindi = WebUtility.UrlDecode(model.SurveyDescriptionHindi);
                model.SurveyDescriptionKannada = WebUtility.UrlDecode(model.SurveyDescriptionKannada);
                model.SurveyDescriptionGujarati = WebUtility.UrlDecode(model.SurveyDescriptionGujarati);
                model.SurveyDescriptionJapanese = WebUtility.UrlDecode(model.SurveyDescriptionJapanese);
                // Changed by TTl on 02-July-2025 against CR6641 - END
                //InformationSecurity objInfoSec = new InformationSecurity();
                string userID = _sessionService.Get<string>("userID");
                string strremark = string.Empty;
                foreach (var question in model.Questions)
                {
                    //if (string.IsNullOrEmpty(question.SelectedOption))
                    //{
                    //    ModelState.AddModelError("", "Please fill the answer of all questions");
                    //    return View("InformationSecuritySurvey", model);
                    //}

                    //when none of the answers are filled
                    if (question.IsSubQuestion == 0 && string.IsNullOrEmpty(question.SelectedOption) && question.SelectedOptions == null)
                    {
                        ModelState.AddModelError("CustomError", "Please fill the answer of all questions");
                        return View("InformationSecuritySurvey", model);
                    }

                    //option type = radio + parent question + IsVisible = null
                    if (question.OptionType == "1" && question.IsSubQuestion == 0 && question.IsVisible == null)
                    {
                        if (string.IsNullOrEmpty(question.SelectedOption))
                        {
                            ModelState.AddModelError("CustomError", "Please fill the answer of all questions");
                            return View("InformationSecuritySurvey", model);
                        }
                    }
                    //option type = checkbox + parent question + isvisible = null
                    else if (question.OptionType == "2" && question.IsSubQuestion == 0 && question.IsVisible == "null")
                    {
                        if (question.SelectedOptions == null)
                        {
                            ModelState.AddModelError("CustomError", "Please fill the answer of all questions");
                            return View("InformationSecuritySurvey", model);
                        }
                    }
                    //option type = radio + sub question + isVisible = ml-30
                    else if (question.OptionType == "1" && question.IsSubQuestion == 1 && question.IsVisible == "ml-30")
                    {
                        if (string.IsNullOrEmpty(question.SelectedOption))
                        {
                            ModelState.AddModelError("CustomError", "Please fill the answer of all questions");
                            return View("InformationSecuritySurvey", model);
                        }
                    }
                    //option type = checkboc + sub question + isvisible = ml-30
                    else if (question.OptionType == "2" && question.IsSubQuestion == 1 && question.IsVisible == "ml-30")
                    {
                        if (question.SelectedOptions == null)
                        {
                            ModelState.AddModelError("CustomError", "Please fill the answer of all questions");
                            return View("InformationSecuritySurvey", model);
                        }
                    }
                }
                if (model.FEEDBACKSTATUS == "1")
                {
                    if (model.ISREMARKSMANDATORY == "1")
                    {
                        if (string.IsNullOrEmpty(remark.Trim()))
                        {
                            ModelState.AddModelError("CustomError", "Remarks is mandatory field");
                            return View("InformationSecuritySurvey", model);
                        }
                    }
                }
                foreach (var question in model.Questions)
                {
                    // Save the answer
                    if (question.OptionType == "1")  // Radio button
                    {
                        string errMsg = objInfoSec.SaveQuizData(userID, question.Id.ToString(), question.SelectedOption, strremark);
                        if (!string.IsNullOrEmpty(errMsg))
                        {
                            ModelState.AddModelError("CustomError", errMsg);
                            return View("InformationSecuritySurvey", model);
                        }
                    }
                    else if (question.OptionType == "2")  // Checkbox
                    {
                        //start - for multiple entry in database
                        if (question.SelectedOptions != null)
                        {
                            foreach (var selectedOption in question.SelectedOptions)
                            {
                                // Save each selected checkbox answer
                                string errMsg = objInfoSec.SaveQuizData(userID, question.Id.ToString(), selectedOption, strremark);
                                if (!string.IsNullOrEmpty(errMsg))
                                {
                                    ModelState.AddModelError("CustomError", errMsg);
                                    return View("InformationSecuritySurvey", model);
                                }
                            }
                        }
                        //end - for multiple entry in database

                        //start - comma separated value in hrsecurityquiz table column hrsecurityanslistid
                        //string selectedMultipleOptions = string.Join(",", question.SelectedOptions);

                        //string errMsg = objInfoSec.SaveQuizData(userID, question.Id.ToString(), selectedMultipleOptions, strremark);
                        //if (!string.IsNullOrEmpty(errMsg))
                        //{
                        //    ModelState.AddModelError("", errMsg);
                        //    return View("InformationSecuritySurvey", model);
                        //}
                        //end
                    }
                }

                int SurveyNumber = 0;
                /* Code Commented By Aumento as on 05072024
                DataTable dtsn = new DataTable();
                dtsn = objInfoSec.GetActiveSurvey();
                if (dtsn.Rows.Count > 0)
                {
                    SurveyNumber = Convert.ToInt16(dtsn.Rows[0]["HRSECURITYSURVEYID"].ToString());
                }
                */
                SurveyNumber = Convert.ToInt32(model.SurveyNumber); // Added By Aumento as on 05072024
                int empcode = Convert.ToInt32(userID);

                if (!string.IsNullOrEmpty(remark))
                {
                    string errMsg = objInfoSec.SaveSurveyremarks(userID, SurveyNumber.ToString(), remark);
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        ModelState.AddModelError("CustomError", errMsg);
                        return View("InformationSecuritySurvey", model);
                    }
                }

                if (model.SURVEYRESULTMAIL == "1")
                {
                    //Sending survey email...
                    DataTable oDs = new DataTable();
                    oDs = objInfoSec.CheckInformationSecurityQuiz_Answer(empcode, SurveyNumber);
                    string errMsg = sendmail(oDs);
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        ModelState.AddModelError("CustomError", errMsg);
                        return View("InformationSecuritySurvey", model);
                    }
                }
                if (model.SURVEYACKMAIL == "1")
                {
                    //Sending survey acknowledgement email
                    string errMsg = sendmailack(model);
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        ModelState.AddModelError("CustomError", errMsg);
                        return View("InformationSecuritySurvey", model);
                    }
                }
                return RedirectToAction("Home", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("CustomError", "An error occurred while processing your request.");
                //return View("InformationSecuritySurvey", model);

                this.sendErrormail(ex.Message); // //Added by Aumento as on 19-06-2024 
                return RedirectToAction("InformationSecuritySurvey");
            }


            return View("InformationSecuritySurvey", model);
        }

        protected string sendmail(DataTable oDs)
        {
            String RetrunResult = "";
            try
            {
                int correctanswer = 0;
                int answer = 0;
                string result;
                int iscorrect;
                int qasno = 0;
                Employee_Details Emp = new Employee_Details();
                //Training objTraining = new Training();
                // create a string type variable to generate dynamic table
                string dynTable = "";
                int a = 1;

                // start with table tag with following attributes
                dynTable = "<table style=\"border-left:1px solid #C1DAD7;\" cellspacing=\"0\" cellpadding=\"2\" border=\"1\">";

                dynTable += "<tr><td width=10% style=\"background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\" align=\"center\" >Ques.No.</td>" +
                            "<td width=90% style=\"background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\"><center>Ques. Description</center></td>";
                foreach (DataRow dr in oDs.Rows)
                {
                    qasno++;
                    iscorrect = Convert.ToInt32(dr["ISCORRECT"]);
                    if (iscorrect == 1)
                    {
                        correctanswer++;
                        answer++;
                    }
                    else
                    {
                        dynTable += "<tr>";
                        for (int tCols = 0; tCols <= 1; tCols++)
                        {
                            if (tCols == 0)
                                dynTable += "<td style=\"background-color:#F5FAFA; border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;text-align:middle;font-size:10px;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\" valign=\"top\" align=\"center\" >";
                            else
                                dynTable += "<td style=\"background-color:#F5FAFA; border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;text-align:left;font-size:10px;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\" valign=\"top\">";
                            if (tCols.ToString() == "0")
                            {
                                dynTable += qasno.ToString();
                            }
                            else if (tCols.ToString() == "1")
                            {
                                dynTable += dr["qusdescription"].ToString() + "<br/>";
                                if (dr["qusdescription"].ToString() != "")
                                {
                                    string[] arr = dr["AllAns"].ToString().Split('~');
                                    int cnt = 0;
                                    foreach (string itm in arr)
                                    {
                                        if (itm == dr["IncorrectAns"].ToString())
                                            dynTable += "<input type='radio' id='rd" + cnt + "' disabled checked><label for='rd" + cnt + "' style='color: red;'> " + itm + " </label>&nbsp;&nbsp;&nbsp;";
                                        else
                                            dynTable += "<input type='radio' id='rd" + cnt + "' disabled><label for='rd" + cnt + "'> " + itm + " </label>&nbsp;&nbsp;&nbsp;";
                                        cnt++;
                                    }
                                    dynTable += "<br>Correct Answer: <label style='color: green;'>" + dr["CorrectAns"].ToString() + " </label>";
                                }
                            }
                            dynTable += "</td>";
                        }
                        dynTable += "</tr>";
                    }
                }
                dynTable += "</table>";

                correctanswer = (correctanswer * 100) / oDs.Rows.Count;
                if (correctanswer < 50)
                {
                    result = "Poor";
                }
                else if (90 > correctanswer && correctanswer >= 50)
                {
                    result = "Satisfactory";
                }
                else
                {
                    result = "Excellent";
                }

                Emp = _sessionService.Get<Employee_Details>("Employee");  //(Employee_Details)Session["Employee"];

                string strauthemailid = Emp.EMail_Id;
                string strauthname = Emp.Employee_Name;

                DataTable objdt = new DataTable();
                string deptheadEmailID = string.Empty;
                objdt = objTraining.SPROC_AUTHORITY_GET(_sessionService.Get<string>("userID"));
                if (objdt.Rows.Count != 0)
                {
                    deptheadEmailID = objdt.Rows[0]["emailid"].ToString();
                }

                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {


                    sendMail.MailTo = (strauthemailid != "" ? Check_N_Filter_Email(strauthemailid) : strauthemailid);
                    if (result == "Poor" && (!string.IsNullOrEmpty(deptheadEmailID)))
                    {
                        sendMail.MailCc = Check_N_Filter_Email(deptheadEmailID);
                    }
                }
                string strSubject = "Result of Survey";
                string strBody;
                string r = string.Empty;
                string s;
                s = "<span>" + answer + "</span>";
                if (result == "Poor")
                {
                    r = "<Label style='font-size:18px;font-weight:bold;color:Red;font-family:Calibri;'><center>Your Awareness Level is Poor.</center></Label>" +
                        "<Label style='font-size:18px;font-weight:bold;color:Red;font-family:Calibri;'><center>You will be required to undertake the awareness session</b></center></Label>" +
                        "<Label style='font-size:18px;font-weight:bold;color:Red;font-family:Calibri;'><center>which will be intimated to you separately .</center></Label>";
                }
                else if (result == "Satisfactory")
                {
                    r = "<Label style='font-size:18px;font-weight:bold;color:Green;font-family:Calibri;'><center>Your Awareness Level is Satisfactory.</center></Label>" +
                        "<Label style='font-size:18px;font-weight:bold;color:Red;font-family:Calibri;'><center>You will be required to undertake the awareness session</b></center></Label>" +
                        "<Label style='font-size:18px;font-weight:bold;color:Red;font-family:Calibri;'><center>which will be intimated to you separately .</center></Label>";

                }
                else if (result == "Excellent")
                {
                    if (correctanswer < 100)
                    {
                        r = "<Label style='font-size:18px;font-weight:bold;color:Green;font-family:Calibri;'><center>Congratulation!! Your Awareness Level is Excellent.</center></Label>" +
                            "<Label style='font-size:18px;font-weight:bold;color:Red;font-family:Calibri;'><center>You will be required to undertake the awareness session</b></center></Label>" +
                            "<Label style='font-size:18px;font-weight:bold;color:Red;font-family:Calibri;'><center>which will be intimated to you separately .</center></Label>";
                    }
                    else
                    {
                        r = "<Label style='font-size:18px;font-weight:bold;color:Green;font-family:Calibri;'><center>Congratulation!! Your Awareness Level is Excellent.</center></Label>";

                    }
                }

                strBody = "<div style='width:650px;border:2px skyblue solid;font-family:Calibri;'><div style='width:670px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Survey Result</b></div>"
                                    + "<table cellpadding=0 cellspacing=0 border=0 width=650px style='Margin:10px 10px 0px 10px;text-align:justify;' >"
                                    + "<tr><td colspan=2><b>Dear " + _sessionService.Get<string>("userName") + " San,</b><br/><br/></td></tr>"
                                    + "<tr><td colspan=2>Thank you for attempting awareness Questionnaire on E-Portal.<br/><br/></td></tr>"
                                    + "<tr><td ><center><b>Result:</b></center></td></tr>"
                                    + "<tr><td ><center><b>" + r + "</b><br/></center></td></tr>"
                                    + "<tr><td colspan=2 align='center'><div style='background-color:skyblue;width:65%;padding:6px;font-size:16px;border:1px solid black;'>"
                                    + "<center><table style='font-size:0.8em;margin-top:5px;'>"
                                    + "<tr><td align='left' style='font-size:18px;'><b>Your Awareness Level Score</b></td><td style='padding:0px 40px 0px 40px;'>:</td><td style='font-size:18px;'><b>" + s + "</b></td></tr>"
                                    + "</table></center></div><br/></td></tr>"
                                    + "<tr><td style='font-size:0.8em;'><center><b>Criteria:</b> Excellent(90% & above),Satisfactory(50%>90%),Poor(below 50%)</center></td></tr>";
                if (correctanswer < 100)
                {
                    strBody = strBody + "<tr><td colspan=2 align='center'><br>Following Questions were attempted WRONGLY :- </td></tr>"
                    + "<tr><td ><center><b>" + dynTable + "</b><br/></center></td></tr>";
                }

                strBody = strBody + "<tr><td colspan=2>&nbsp;</td></tr>"
                + "<tr><td colspan=2><b>Best Regards</b></td></tr>"
                + "<tr><td colspan=2>Team - EPortal<br/><br/></td></tr>"
                + "<tr><td colspan=2><b>Note: It is a system generated email, please do not reply.<b><br/><br/></td></tr>"
                + "</table></div>";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                if (Check_N_Filter_Email(strauthemailid) != "")
                {
                    sendMail.Send();
                }

            }
            catch (Exception ex)
            {
                RetrunResult = "Error in Sending survey email";
            }
            return RetrunResult;
        }
        protected string sendmailack(InformationSecuritySurveyViewModel model)
        {
            String RetrunResult = "";
            try
            {
                Employee_Details Emp = new Employee_Details();
                Emp = _sessionService.Get<Employee_Details>("Employee"); //(Employee_Details)Session["Employee"];

                string strauthemailid = Emp.EMail_Id;
                string strauthname = Emp.Employee_Name;

                if (!string.IsNullOrEmpty(strauthemailid) && (Check_N_Filter_Email(strauthemailid) != ""))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";

                    sendMail.MailTo = Check_N_Filter_Email(strauthemailid);


                    string strSubject = "Acknowledgement of Survey";
                    string strBody;

                    strBody = "<div style='width:650px;border:2px skyblue solid;font-family:Calibri;'><div style='width:670px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Survey Result</b></div>"
                                        + "<table cellpadding=0 cellspacing=0 border=0 width=650px style='Margin:10px 10px 0px 10px;text-align:justify;' >"
                                        + "<tr><td colspan=2><b>Dear " + _sessionService.Get<string>("userName") + " San,</b><br/><br/></td></tr>"
                                        //+ "<tr><td colspan=2>Thank you for acknowledging and submitting the " + lblSurveyHeading.Text + ".<br/><br/></td></tr>";
                                        + "<tr><td colspan=2>Thank you for acknowledging and submitting the " + model.SurveyHeading + ".<br/><br/></td></tr>";
                    strBody = strBody + "<tr><td colspan=2>&nbsp;</td></tr>"
        + "<tr><td colspan=2><b>Best Regards</b></td></tr>"
        + "<tr><td colspan=2>Team - EPortal<br/><br/></td></tr>"
        + "<tr><td colspan=2><b>Note: It is a system generated email, please do not reply.<b><br/><br/></td></tr>"
        + "</table></div>";


                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;

                    sendMail.Send();
                }
            }
            catch (Exception ex)
            {
                RetrunResult = "Error in Sending survey acknowledgement email";
            }
            return RetrunResult;
        }
        [HttpPost]
        public ActionResult Skip(int skipcount, int SurveyNumber)
        {
            String ErrorMessage, Status;
            ErrorMessage = "";
            Status = "true";
            try
            {
                //InformationSecurity objInfoSec = new InformationSecurity();
                DataTable dtsn = new DataTable();
                string userID = _sessionService.Get<string>("userID");
                string errMsg = string.Empty;
                //int SurveyNumber = 0;
                //dtsn = objInfoSec.GetActiveSurvey();
                //if (dtsn.Rows.Count > 0)
                //{
                //    SurveyNumber = Convert.ToInt16(dtsn.Rows[0]["HRSECURITYSURVEYID"].ToString());
                //}
                int newSkipCount = skipcount + 1;
                errMsg = objInfoSec.Surveyskip(userID, SurveyNumber.ToString(), newSkipCount.ToString());
                if (!string.IsNullOrEmpty(errMsg))
                {
                    //ModelState.AddModelError("", errMsg);
                    //return RedirectToAction("InformationSecuritySurvey");
                    ErrorMessage = errMsg;
                    Status = "false";
                }
                //return RedirectToAction("Home", "Home");               
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Status = "false";
                //ModelState.AddModelError("", "An error occurred while processing your request.");
                this.sendErrormail(ex.Message); // //Added by Aumento as on 19-06-2024 
                //return RedirectToAction("InformationSecuritySurvey");               
            }
            return Json(new { Status = Status, Message = ErrorMessage });
        }
        //Added By Aumento as On 10062024 End




        //Added by Aumento as on 19-06-2024 
        protected string sendErrormail(string ErrorMSg)
        {
            String RetrunResult = "";
            try
            {
                //InformationSecurity objInfoSec = new InformationSecurity();
                string strauthemailid = string.Empty;

                strauthemailid = objInfoSec.GetSurveyErrorEmailId();

                commanEmail sendMail = new commanEmail();

                sendMail.MailFrom = "portal.admin@honda.hmsi.in";

                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                    sendMail.MailTo = (strauthemailid != "" ? Check_N_Filter_Email(strauthemailid) : strauthemailid);

                string strSubject = "Error of Survey";

                string strBody = @"<div style='width:650px; border:2px solid skyblue; font-family: Calibri;'>
                                    <div style='width: 640px; background-color: skyblue; text-align: center; padding: 5px;'>
                                        <b>Survey Error</b>
                                    </div>
                                    <p style='padding-left:7px;'>Dear Sir/Madam,</p>
                                    <p style='padding-left:7px;'>We encountered the following error while processing the request for the :<br/> ""<b>{EMPLOYEE_CODE}</b>""</p>
                                    <div style='border: 0px solid skyblue; padding: 10px; margin-top: 10px;'>
                                        <table style='width: 100%; border-collapse: collapse;'>
                                            <tr>
                                                <th style='border: 1px solid skyblue; padding: 8px; text-align: left;'>Error</th>
                                            </tr>
                                            <tr>
                                                <td style='border: 1px solid skyblue; padding: 8px; text-align: left;'>{ERRORMSG}</td>
                                            </tr>
                                        </table>
                                    </div>
                                   </div>";

                strBody = strBody.Replace("{EMPLOYEE_CODE}", (_sessionService.Get<string>("userID").ToString() + "-" + _sessionService.Get<string>("userName"))).Replace("{ERRORMSG}", ErrorMSg);

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
            catch (Exception ex)
            {
                RetrunResult = "Error in Sending survey email";
            }
            return RetrunResult;
        }
        //Added by Aumento as on 19-06-2024 
        public string Check_N_Filter_Email(string emailIds)
        {
            StringBuilder _FilteredEmail = new StringBuilder();
            string[] EmailIds = emailIds.Split(',');
            foreach (string Email in EmailIds)
            {
                if (Email.Contains("@honda2wheelersindia.com") || Email.Contains("@honda.hmsi.in"))
                {
                    _FilteredEmail.Append(Email);
                    _FilteredEmail.Append(",");
                }
            }
            return _FilteredEmail.ToString().Remove(_FilteredEmail.Length - 1, 1);
        }
        //Export Survey Report in Excel - Added by Bhupesh
        public ActionResult ExportSurveyReport(string strSurveyHeaderID)
        {
            try
            {
                //InformationSecurity objInfoSec = new InformationSecurity();
                DataTable dt = new DataTable();
                dt = objInfoSec.GetQuesRpt(strSurveyHeaderID);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Survey Report");
                    //Adding column names to the Excel file.
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = dt.Columns[i].ColumnName;
                    }
                    //Adding rows to the Excel file.
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            worksheet.Cell(i + 2, j + 1).Value = dt.Rows[i][j].ToString();
                        }
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        string fileName = "SurveyReport_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        //START : Added by Aumento : : SR74173
        [HttpGet]
        public JsonResult CheckFamilyDeclaration(string Surveyid)
        {

            string sJSON = string.Empty;
            object ResponseData;
            JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            List<FamilyDeclaration_Survery> oList = new List<FamilyDeclaration_Survery>();
            try
            {
                string str_Ecode = _sessionService.Get<string>("userID");
                // int surveyId = int.Parse(Surveyid);
                //EmpUserDetails objAddDetail = new EmpUserDetails();
                DataTable dt = objAddDetail.GetEmployeeDeclaration(str_Ecode);

                foreach (DataRow dr in dt.Rows)
                {
                    FamilyDeclaration_Survery obj = new FamilyDeclaration_Survery
                    {
                        FamilyName = dr["ASSOCIATENAME"].ToString(),
                        RelationShip = dr["ASSOCIATEREL"].ToString(),
                        addedby = dr["ADEMPCODE"].ToString(),
                        ISPastorCurrent = dr["ISPASTORCURR"].ToString(),
                        FAMILYASSOCIATE_ID = string.IsNullOrEmpty(dr["FAMILYASSOCIATE_ID"].ToString()) ? 0 : Convert.ToInt64(dr["FAMILYASSOCIATE_ID"].ToString())
                    };
                    oList.Add(obj);
                }
                ResponseData = new { result = false, errorMessage = "", data = oList };

            }
            catch (Exception ex)
            {
                ResponseData = new { result = false, errorMessage = ex.Message, data = oList };
            }

            sJSON = JsonConvert.SerializeObject(ResponseData, settings);

            return Json(sJSON);
        }

        [HttpPost]
        public JsonResult AddFamilyMember(FamilyDeclaration_Survery temp)
        {
            string sJSON = string.Empty;
            object ResponseData;
            JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            List<FamilyDeclaration_Survery> oList = new List<FamilyDeclaration_Survery>();

            try
            {

                temp.addedby = _sessionService.Get<string>("userID");
                //EmpUserDetails objAddDetail = new EmpUserDetails();
                Tuple<int, string> ret = objAddDetail.UpdateFamilyDeclaration(temp.addedby, temp.FamilyName.Trim(), temp.RelationShip.Trim(), temp.addedby, Convert.ToInt32(temp.ISPastorCurrent));
                DataTable dt = objAddDetail.GetEmployeeDeclaration(temp.addedby);

                foreach (DataRow dr in dt.Rows)
                {
                    FamilyDeclaration_Survery obj = new FamilyDeclaration_Survery
                    {
                        FamilyName = dr["ASSOCIATENAME"].ToString(),
                        RelationShip = dr["ASSOCIATEREL"].ToString(),
                        addedby = dr["ADEMPCODE"].ToString(),
                        ISPastorCurrent = dr["ISPASTORCURR"].ToString(),
                        FAMILYASSOCIATE_ID = string.IsNullOrEmpty(dr["FAMILYASSOCIATE_ID"].ToString()) ? 0 : Convert.ToInt64(dr["FAMILYASSOCIATE_ID"].ToString())
                    };
                    oList.Add(obj);
                }

                ResponseData = new { result = false, errorMessage = "", data = oList };

            }
            catch (Exception ex)
            {
                ResponseData = new { result = false, errorMessage = ex.Message, data = oList };
            }

            sJSON = JsonConvert.SerializeObject(ResponseData, settings);
            return Json(sJSON);
        }

        [HttpDelete]
        public JsonResult DeleteFamily(string ecode, string HeaderId)
        {
            string sJSON = string.Empty;
            object ResponseData;
            JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            List<FamilyDeclaration_Survery> oList = new List<FamilyDeclaration_Survery>();

            try
            {

                var addedby = _sessionService.Get<string>("userID");
                //EmpUserDetails objAddDetail = new EmpUserDetails();
                var ret = objAddDetail.DeleteFamilyDeclaration(ecode, HeaderId);

                DataTable dt = objAddDetail.GetEmployeeDeclaration(addedby);

                foreach (DataRow dr in dt.Rows)
                {
                    FamilyDeclaration_Survery obj = new FamilyDeclaration_Survery
                    {
                        FamilyName = dr["ASSOCIATENAME"].ToString(),
                        RelationShip = dr["ASSOCIATEREL"].ToString(),
                        addedby = dr["ADEMPCODE"].ToString(),
                        ISPastorCurrent = dr["ISPASTORCURR"].ToString(),
                        FAMILYASSOCIATE_ID = string.IsNullOrEmpty(dr["FAMILYASSOCIATE_ID"].ToString()) ? 0 : Convert.ToInt64(dr["FAMILYASSOCIATE_ID"].ToString())
                    };
                    oList.Add(obj);
                }

                ResponseData = new { result = false, errorMessage = "", data = oList };

            }
            catch (Exception ex)
            {
                ResponseData = new { result = false, errorMessage = ex.Message, data = oList };
            }

            sJSON = JsonConvert.SerializeObject(ResponseData, settings);
            return Json(sJSON);
        }
        //END : Added by Aumento : : SR74173

        [HttpGet]
        public ActionResult SurveyPreview(int id)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            var model = new InformationSecuritySurveyViewModel();
            //InformationSecurity objInfoSec = new InformationSecurity();
            //Training objTraining = new Training();
            DataTable oDs = new DataTable();
            DataTable dts = new DataTable();
            DataTable dtsn = new DataTable();
            Employee_Details Emp = new Employee_Details();
            var questions = new List<Question>();
            string SurveyType = "";
            dts = objInfoSec.GetPartSurveyList(id.ToString());
            if (dts.Rows.Count > 0)
            {
                //lblSurveyHeading.Text = dts.Rows[0]["HRSURVEYDESC"].ToString();
                model.SurveyHeading = dts.Rows[0]["HRSURVEYDESC"].ToString();
                model.SurveyHeadingHindi = dts.Rows[0]["HRSURVEYDESCHINDI"].ToString();
                model.SurveyHeadingKannada = dts.Rows[0]["HRSURVEYDESCKANNADA"].ToString();
                model.SurveyHeadingGujarati = dts.Rows[0]["HRSURVEYDESCGUJARATI"].ToString();
                model.SurveyHeadingJapanese = dts.Rows[0]["HRSURVEYDESCJAPANESE"].ToString();
                //lblSurveydesc.Text = dts.Rows[0]["HRSURVEYHEADER"].ToString();
                model.SurveyDescription = dts.Rows[0]["HRSURVEYHEADER"].ToString();
                model.SurveyDescriptionHindi = dts.Rows[0]["HRSURVEYHEADERHINDI"].ToString();
                model.SurveyDescriptionKannada = dts.Rows[0]["HRSURVEYHEADERKANNADA"].ToString();
                model.SurveyDescriptionGujarati = dts.Rows[0]["HRSURVEYHEADERGUJARATI"].ToString();
                model.SurveyDescriptionJapanese = dts.Rows[0]["HRSURVEYHEADERJAPANESE"].ToString();
                //hdemailstatus.Value = dts.Rows[0]["SURVEYRESULTMAIL"].ToString();
                model.SURVEYRESULTMAIL = dts.Rows[0]["SURVEYRESULTMAIL"].ToString();
                //hdemailack.Value = dts.Rows[0]["SURVEYACKMAIL"].ToString();
                model.SURVEYACKMAIL = dts.Rows[0]["SURVEYACKMAIL"].ToString();
                //hdremarksstatus.Value = dts.Rows[0]["FEEDBACKSTATUS"].ToString();
                model.FEEDBACKSTATUS = dts.Rows[0]["FEEDBACKSTATUS"].ToString();
                //hdisremekmand.Value = dts.Rows[0]["ISREMARKSMANDATORY"].ToString();
                model.ISREMARKSMANDATORY = dts.Rows[0]["ISREMARKSMANDATORY"].ToString();
                SurveyType = dts.Rows[0]["SURVEYTYPE"].ToString();
                model.SURVEYTYPE = dts.Rows[0]["SURVEYTYPE"].ToString();
                model.SurveyNumber = id.ToString();
                model.ISFAMILYDECREQ = (dts.Rows[0]["ISHCGSURVEY"].ToString() == "1" ? true : false);
            }

            if (SurveyType == "2")
            {
                oDs = objInfoSec.GetInformationSecurityQuiz_QuestionFixed(id, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            else
            {
                oDs = objInfoSec.GetInformationSecurityQuiz_Question(id);
            }
            foreach (DataRow row in oDs.Rows)
            {
                var question = new Question
                {
                    Id = Convert.ToInt32(row["HRSECURITYQUSLISTID"]),
                    Description = row["QUSDESCRIPTION"].ToString(),
                    DescriptionHindi = row["QUSDESCRIPTIONHINDI"].ToString(),
                    DescriptionKannada = row["QUSDESCRIPTIONKANNADA"].ToString(),
                    DescriptionGujarati = row["QUSDESCRIPTIONGUJARATI"].ToString(),
                    DescriptionJapanese = row["QUSDESCRIPTIONJAPANESE"].ToString(),
                    OptionType = row["OPTIONTYPE"].ToString(),
                    IsSubQuestion = row["ISSUBQUESTION"] != DBNull.Value ? Convert.ToInt16(row["ISSUBQUESTION"]) : 0,
                    SubQuestionParentId = row["SUBQUESTIONPARENTID"] != DBNull.Value ? Convert.ToInt32(row["SUBQUESTIONPARENTID"]) : 0,
                    TriggerOptionIds = row["TRIGGEROPTIONIDS"].ToString(),
                    Options = GetOptionsFromDataTable(Convert.ToInt32(row["HRSECURITYQUSLISTID"]))
                };
                questions.Add(question);
            }

            //model.Questions = questions;

            model.Questions = new List<Question>();

            questions.Where(Q => Q.IsSubQuestion == 0).ToList().ForEach(X =>
            {
                Question Que = questions.Where(q => X.Id == q.SubQuestionParentId).FirstOrDefault();
                if (Que != null)
                {
                    X.TriggerOptionIds = Que.TriggerOptionIds.ToString();
                    X.SubQuestionParentId = Que.Id;
                    Que.IsVisible = "hidden ml-30";
                    Que.TriggerOptionIds = "";
                    Que.SubQuestionParentId = 0;
                }
                model.Questions.Add(X);
                if (Que != null)
                {
                    model.Questions.Add(Que);
                }
            });

            return View(model);
        }


        [HttpPost]
        public ActionResult BindDivisionByOperationId([FromBody] CommunicationViewModel model)
        {
            List<DivisionViewModel> FinalDivList = new List<DivisionViewModel>();
            if (model!=null && model.OPERATION != null)
            {
                foreach (long id in model.OPERATION)
                {
                    IEnumerable<DivisionViewModel> divList = _ISurveyService.BindDivision(2, Convert.ToInt64(id));
                    if (divList != null)
                    {
                        FinalDivList.AddRange(divList);
                    }
                }
            }
            return Json(FinalDivList);
        }


        [HttpGet]
        public ActionResult InitiateRetest(int id)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            var _res = _ISurveyService.DuplicateSurvey(id);
            if (_res > id)
            {
                return Json(new { success = true, data = _res });
            }
            else
            {
                return Json(new { success = false });
            }

        }

        
        [HttpGet]
        public IActionResult InfoSecuReport()
        {
            var model = new InfoSecuReportViewModel
            {
                Operations = _objPms.GetOperaton().AsSelectList("ADVPID", "DESCRIP",true,"All Operation","0"),
                Divisions = _oDiv.GetFilterDivision(0).AsSelectList_DS("ADDIVISIONID", "DESCRIP",true, "All Division", "0"), //new List<SelectListItem>(),
                Departments = _oDept.GetFilterDepartment(0, 0).AsSelectList_DS("addepartmentid","deptname", true, "All Department", "0"), //new List<SelectListItem>(),
                Sections = _objPms.GetSect("0", "0", "0").AsSelectList("ADSECTIONID", "DESCRIP", true, "All Section", "0"), //new List<SelectListItem>(),
                Surveys = objInfoSec.GetSurveyList(_sessionService.Get<string>("userID")).AsSelectList("HRSECURITYSURVEYID", "HRSURVEYTITAL", true, "All Survey", "0"),
                Designations = _objPms.GetFilterDesignation().AsSelectList("ID", "DESIGNATION", true, "All Designation", "0"),
            };

            return View(model);
        }



        //[HttpGet]
        //public JsonResult GetOperaton()
        //{
        //    objdt = objPms.GetFilterDesignation();
        //    return Json(sections);
        //}

        //[HttpGet]
        //public JsonResult GetDesignation()
        //{
        //    objdt = objPms.GetFilterDesignation();
        //    return Json(sections);
        //}

        //[HttpGet]
        //public JsonResult GetSurveyList()
        //{
        //    //objdt = objInfoSec.GetSurveyList(Session["userid"].ToString());
        //    var divisions = GetDropdownData("sp_GetDivisionsByOperation", new SqlParameter("@OperationId", operationId));
        //    return Json(divisions);
        //}

        [HttpGet]
        public JsonResult GetDivisions(int operationId)
        {
            //oDs = oDiv.GetFilterDivision(operationID);
            var divisions = _oDiv.GetFilterDivision(operationId).AsSelectList_DS("ADDIVISIONID", "DESCRIP", true, "All Division", "0");
            return Json(divisions);
        }

        [HttpGet]
        public JsonResult GetDepartments(int operationID, int divisionId)
        {
            //oDs = oDept.GetFilterDepartment(OperationID, divisionID);
            var departments = _oDept.GetFilterDepartment(operationID, divisionId).AsSelectList_DS("addepartmentid", "deptname", true, "All Department", "0");
            return Json(departments);
        }

        [HttpGet]
        public JsonResult GetSections(int operationID, int divisionId,int departmentId)
        {
            //objPms.GetSect(strOperationId, strDivisionId, strDepartmentId);
            var sections = _objPms.GetSect(Convert.ToString(operationID), Convert.ToString(divisionId), Convert.ToString(departmentId)).AsSelectList("ADSECTIONID", "DESCRIP", true, "All Section", "0");
            return Json(sections);
        }



        [HttpPost]
        public IActionResult InfoSecuReport(InfoSecuReportViewModel model)
        {
            string designationIds = model.DesignationIds
              .Where(id => id != 0)
              .Any()
              ? string.Join(",", model.DesignationIds.Where(id => id.ToString() != "0"))
              : _objCommon.GetParameterValue("HR_INFORSECURITY_DESIGID");

            if (designationIds == "")
            {
                designationIds = _objCommon.GetParameterValue("HR_INFORSECURITY_DESIGID");
            }

            //objInfoSec.GetInfoSecSurveyRpt(strDesignationId, strDivisionId, strDepartmentId, strSectionId, strOperationId,
            //                                strAssoEmpCode, strAssoEmpName, strSurveyNumber);

            //objInfoSec.GetInfoSecurityOvelAllResult(strDesignationId, strDivisionId, strDepartmentId, strSectionId, strOperationId,
            //                                strAssoEmpCode, strAssoEmpName, strSurveyNumber);

            DataTable SummaryList = objInfoSec.GetInfoSecSurveyRpt(
               designationIds,
               model.DivisionId.ToString(),
               model.DepartmentId.ToString(),
               model.SectionId.ToString(),
               model.OperationId.ToString(),
               model.EmpCode,
               model.EmpName,
               model.SurveyId.ToString()
           );

            DataTable DetailsList = objInfoSec.GetInfoSecurityOvelAllResult(
                designationIds,
                model.DivisionId.ToString(),
                model.DepartmentId.ToString(),
                model.SectionId.ToString(),
                model.OperationId.ToString(),
                model.EmpCode,
                model.EmpName,
                model.SurveyId.ToString()
            );

            model.SummaryList = SummaryList.AsEnumerable().Select(r => new SummaryItem
            {
                Ecode = r["ECODE"].ToString(),
                Name = r["EMPLOYEENAME"].ToString(),
                Score = r["SCORE"].ToString(),
                AVGPER = r["AVGPER"].ToString()
            }).ToList();


            model.DetailList = DetailsList.AsEnumerable().Select(r => new DetailItem
            {
                Level = r["LEVEL1"].ToString(),
                TotalAssociate = r["Total"].ToString()
            }).ToList();


            //var data = _infoSecService.GetSurveyData(filter);
            return PartialView("_searchSurvey", model);
        }


        [HttpPost]
        public IActionResult ExportToExcel(InfoSecuReportViewModel filter)
        {
            // TODO: Replace with your actual logic that fetches filtered data
            DataTable dt = GetFilteredData(filter);

            //if (dt == null || dt.Rows.Count == 0)
            //    return BadRequest("No records found for export.");

            byte[] fileBytes = _excelHelper.ExportToExcel(dt, "Info Security Report");

            string fileName = $"InfoSecurityReport_{DateTime.Now:ddMMyyyy_HHmmss}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        // Mock for demonstration – you will replace this with your DB logic
        private DataTable GetFilteredData(InfoSecuReportViewModel model)
        {

            //foreach (int i in lstDesignation.GetSelectedIndices())
            //{
            //    if (i.ToString() != "0")
            //    {
            //        strDesignationId = strDesignationId + "," + lstDesignation.Items[i].Value;
            //        strDesignationId = strDesignationId.Trim(',').ToString();
            //    }
            //}
            //if (strDesignationId == "")
            //{
            //    strDesignationId = objCommon.GetParameterValue("HR_INFORSECURITY_DESIGID");
            //}

            //DataTable dt = new DataTable();
            //dt = objInfoSec.GetInfoSecSurveyRpt(strDesignationId, strDivisionId, strDepartmentId, strSectionId, strOperationId,
            //                                    strAssoEmpCode, strAssoEmpName, strSurveyNumber);



            string designationIds = model.DesignationIds
              .Where(id => id != 0)
              .Any()
              ? string.Join(",", model.DesignationIds.Where(id => id.ToString() != "0"))
              : _objCommon.GetParameterValue("HR_INFORSECURITY_DESIGID");

            if (designationIds == "")
            {
                designationIds = _objCommon.GetParameterValue("HR_INFORSECURITY_DESIGID");
            }

            DataTable SummaryList = objInfoSec.GetInfoSecSurveyRpt(
               designationIds,
               model.DivisionId.ToString(),
               model.DepartmentId.ToString(),
               model.SectionId.ToString(),
               model.OperationId.ToString(),
               model.EmpCode,
               model.EmpName,
               model.SurveyId.ToString()
           );

            //model.SummaryList = SummaryList.AsEnumerable().Select(r => new SummaryItem
            //{
            //    Ecode = r["ECODE"].ToString(),
            //    Name = r["EMPLOYEENAME"].ToString(),
            //    Score = r["SCORE"].ToString(),
            //    AVGPER = r["AVGPER"].ToString()
            //}).ToList();


            //var dt = new DataTable();
            //dt.Columns.Add("SrNo");
            //dt.Columns.Add("UserName");
            //dt.Columns.Add("Department");
            //dt.Columns.Add("RequestDate");

            //dt.Rows.Add("1", "Dalbir", "IT", DateTime.Now.ToString("dd-MM-yyyy"));
            //dt.Rows.Add("2", "Gurveer", "Admin", DateTime.Now.ToString("dd-MM-yyyy"));

            return SummaryList;
        }



        //[HttpPost]
        //public IActionResult InfoSecuReport(InfoSecuReportViewModel model)
        //{
        //    // Get search results



        //    //string designationIds1 = model.DesignationIds.Any()
        //    //    ? string.Join(",", model.DesignationIds)
        //    //    : _objCommon.GetParameterValue("HR_INFORSECURITY_DESIGID");


        //    string designationIds = model.DesignationIds
        //        .Where(id => id != 0)
        //        .Any()
        //        ? string.Join(",", model.DesignationIds.Where(id => id.ToString() != "0"))
        //        : _objCommon.GetParameterValue("HR_INFORSECURITY_DESIGID");

        //    if (designationIds == "")
        //    {
        //        designationIds = _objCommon.GetParameterValue("HR_INFORSECURITY_DESIGID");
        //    }

        //    //objInfoSec.GetInfoSecSurveyRpt(strDesignationId, strDivisionId, strDepartmentId, strSectionId, strOperationId,
        //    //                                strAssoEmpCode, strAssoEmpName, strSurveyNumber);

        //    //objInfoSec.GetInfoSecurityOvelAllResult(strDesignationId, strDivisionId, strDepartmentId, strSectionId, strOperationId,
        //    //                                strAssoEmpCode, strAssoEmpName, strSurveyNumber);

        //     DataTable SummaryList = objInfoSec.GetInfoSecSurveyRpt(
        //        designationIds,
        //        model.DivisionId.ToString(),
        //        model.DepartmentId.ToString(),
        //        model.SectionId.ToString(),
        //        model.OperationId.ToString(),
        //        model.EmpCode,
        //        model.EmpName,
        //        model.SurveyId.ToString()
        //    );

        //    DataTable DetailsList = objInfoSec.GetInfoSecurityOvelAllResult(
        //        designationIds,
        //        model.DivisionId.ToString(),
        //        model.DepartmentId.ToString(),
        //        model.SectionId.ToString(),
        //        model.OperationId.ToString(),
        //        model.EmpCode,
        //        model.EmpName,
        //        model.SurveyId.ToString()
        //    );

        //    model.SummaryList = SummaryList.AsEnumerable().Select(r => new SummaryItem
        //    {
        //        Ecode = r["ECODE"].ToString(),
        //        Name = r["EMPLOYEENAME"].ToString(),
        //        Score = r["SCORE"].ToString(),
        //        AVGPER = r["AVGPER"].ToString()
        //    }).ToList();


        //    model.DetailList = DetailsList.AsEnumerable().Select(r => new DetailItem
        //    {
        //        Level = r["LEVEL1"].ToString(),
        //        TotalAssociate = r["Total"].ToString()
        //    }).ToList();            


        //    // Rebind dropdowns after postback

        //    model.Operations = _objPms.GetOperaton().AsSelectList("ADVPID", "DESCRIP", true, "All Operation", "0");
        //    model.Divisions = _oDiv.GetFilterDivision(model.OperationId).AsSelectList_DS("ADDIVISIONID", "DESCRIP", true, "All Division", "0"); //new List<SelectListItem>(),
        //    model.Departments = _oDept.GetFilterDepartment(model.OperationId, model.DivisionId).AsSelectList_DS("addepartmentid", "deptname", true, "All Department", "0"); //new List<SelectListItem>(),
        //    model.Sections = _objPms.GetSect(model.OperationId.ToString(), model.DivisionId.ToString(), model.DepartmentId.ToString()).AsSelectList("ADSECTIONID", "DESCRIP", true, "All Section", "0"); //new List<SelectListItem>(),
        //    model.Surveys = objInfoSec.GetSurveyList(_sessionService.Get<string>("userID")).AsSelectList("HRSECURITYSURVEYID", "HRSURVEYTITAL", true, "All Survey", "0");
        //    model.Designations = _objPms.GetFilterDesignation().AsSelectList("ID", "DESIGNATION", true, "All Designation", "0");

        //    return View(model);
        //}


        [HttpGet]
        public JsonResult GetSurveyResult(string ecode, string surveyId)
        {
            // Fetch data from DB based on ID
            var dt = objInfoSec.GetSurveyResult(surveyId, ecode);
            //string associateName = oTourQueries.GetEmpName(ecode);
            //string ResultInPer = objInfoSec.GetSurveyResultInPercentage(surveyId, ecode).ToString() + "%";


            var list = dt.AsEnumerable().Select(row => new SurveyQuestion_Result
            {
                ROWNUM = Convert.ToInt32(row["ROWNUM"]),
                QUSDESCRIPTION = row["QUSDESCRIPTION"].ToString(),
                SCORE = row["SCORE"].ToString()
            }).ToList();

            return Json(list);
        }


        //get
        //PendingConfirmations


    }




    //START : Added by Aumento : : SR74173
    public class FamilyDeclaration_Survery
    {
        public string FamilyName { get; set; }
        public string RelationShip { get; set; }
        public string addedby { get; set; }
        public string ISPastorCurrent { get; set; }
        public long FAMILYASSOCIATE_ID { get; set; }
    }
    //END : Added by Aumento : : SR74173



}