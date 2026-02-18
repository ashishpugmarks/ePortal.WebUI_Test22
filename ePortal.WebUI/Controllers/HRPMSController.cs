using Microsoft.AspNetCore.Mvc;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.DomainClasses;
using System.Diagnostics.Contracts;
using ePortal.Persistence.Admin.Interface;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.Shared;
using System.Text;
using ePortal.WebUI.Filters;


namespace ePortal.WebUI.Controllers
{

    [CSPFilter]
    public class HRPMSController : Controller
    {
        private readonly IHRPMS _pms;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HRPMSController> _logger;
        private readonly IConfiguration _configuration;
        private const int DefaultPageSize = 15;
        public HRPMSController(IHRPMS hrpmsService, ISessionService sessionService, ILogger<HRPMSController> logger, IConfiguration configuration)
        {
            _pms = hrpmsService;
            _sessionService = sessionService;
            _logger = logger;
            _configuration = configuration;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public IActionResult ManagePMS()
        {
            var model = new PMSViweModel();
            LoadDropdowns(model);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ManagePMS([FromBody] PMSViweModel model, int evalPage = 0, int revPage = 0)
        {
            // Re-populate dropdowns (select lists)
            //LoadDropdowns(model);

            // Validate required SYKI
            if (string.IsNullOrWhiteSpace(model.SYKI))
            {
                ModelState.AddModelError(nameof(model.SYKI), "SYKI is mandatory field");
                return View(model);
            }

            // Get user id from session (like WebForms)
            var ecode = _sessionService.Get<string>("userID");
            if (string.IsNullOrWhiteSpace(ecode))
            {
                // user not logged in - redirect to login
                return RedirectToAction("Login", "Account"); // update action/controller as appropriate
            }
            

            // call your data method
            DataSet ds = _pms.GETAMANAGEPMS(
                model.SYKI, ecode,
                model.Operation, model.Division,
                model.Department, model.Section
            );

            var result = new PmsResultModel();

            // Self
            if (ds != null && ds.Tables.Count > 0)
            {
                result.SelfTable = ds.Tables[0];
                result.EvaluatorPaging.PageSize = DefaultPageSize;
                result.EvaluatorPaging.TotalItems = result.SelfTable.Rows.Count;
                result.EvaluatorPaging.PageIndex = Math.Max(0, evalPage);
            }
            else
            {
                result.SelfTable = new DataTable();
            }

            // Evaluator
            if (ds != null && ds.Tables.Count > 1)
            {
                result.EvaluatorTable = ds.Tables[1];
                result.EvaluatorPaging.PageSize = DefaultPageSize;
                result.EvaluatorPaging.TotalItems = result.EvaluatorTable.Rows.Count;
                result.EvaluatorPaging.PageIndex = Math.Max(0, evalPage);
            }
            else
            {
                result.EvaluatorTable = new DataTable();
            }

            // Reviewer
            if (ds != null && ds.Tables.Count > 2)
            {
                result.ReviewerTable = ds.Tables[2];
                result.ReviewerPaging.PageSize = DefaultPageSize;
                result.ReviewerPaging.TotalItems = result.ReviewerTable.Rows.Count;
                result.ReviewerPaging.PageIndex = Math.Max(0, revPage);
                result.SYKI = model.SYKI;
            }
            else
            {
                result.ReviewerTable = new DataTable();
            }

            //ViewBag.Result = result;

            //return View(model);
            return PartialView("_cmnPMSView", result);
        }

        // -- helper to build dropdowns from GETKIOPERDIVDEPTSEC --
        private void LoadDropdowns(PMSViweModel model)
        {
            // call service - pass currently selected filters so the stored procedure returns filtered lists similarly to original.
            DataSet ds = _pms.GETKIOPERDIVDEPTSEC(model.Operation ?? "", model.Division ?? "", model.Department ?? "");

            // Table 0 - KI (SYKI)
            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = ds.Tables[0];
                model.KiList = dt.AsEnumerable()
                    .Where(r => r.Field<object>("SYKIID") != null &&
                                r.Field<decimal>("SYKIID") > 17m )
                    .OrderBy(r => r.Field<decimal>("SYKIID"))
                    .Select(r => new SelectListItem
                    {
                        Value = r["SYKIID"].ToString(),
                        Text = r["KICODE"].ToString()
                    }).ToList();

                model.KiList.Insert(0, new SelectListItem { Value = "", Text = "-Select-" });
            }

            // Table 1 - Operation
            if (ds != null && ds.Tables.Count > 1)
            {
                var dt1 = ds.Tables[1];
                model.OperationList = dt1.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["ADVPID"].ToString(),
                        Text = r["DESCRIP"].ToString()
                    }).ToList();

                model.OperationList.Insert(0, new SelectListItem { Value = "", Text = "--select--" });
            }

            // Table 2 - Division
            if (ds != null && ds.Tables.Count > 2)
            {
                var dt2 = ds.Tables[2];
                model.DivisionList = dt2.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["ADDIVISIONID"].ToString(),
                        Text = r["DESCRIP"].ToString()
                    }).ToList();

                model.DivisionList.Insert(0, new SelectListItem { Value = "", Text = "--select--" });
            }

            // Table 3 - Department
            if (ds != null && ds.Tables.Count > 3)
            {
                var dt3 = ds.Tables[3];
                model.DepartmentList = dt3.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["addepartmentid"].ToString(),
                        Text = r["deptname"].ToString()
                    }).ToList();

                model.DepartmentList.Insert(0, new SelectListItem { Value = "", Text = "--select--" });
            }

            // Table 4 - Section
            if (ds != null && ds.Tables.Count > 4)
            {
                var dt4 = ds.Tables[4];
                model.SectionList = dt4.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r["ADSECTIONID"].ToString(),
                        Text = r["SECTIONNAME"].ToString()
                    }).ToList();

                model.SectionList.Insert(0, new SelectListItem { Value = "", Text = "--select--" });
            }
        }

        // Optional: Action to open child pages in new windows if desired
        public IActionResult ViewGoalSetting(string pmsId)
        {
            // pmsId is encrypted or plain depending on your encryption usage.
            // Implement logic to open page or return view.
            // For now redirect to existing legacy url if needed:
            return Redirect($"/GoalSettingForm?PMSID={pmsId}");
        }

        [HttpPost]
        public async Task<ActionResult> RedirectGoalSetting([FromBody] GoalInputModel model)
        {
            if (!string.IsNullOrEmpty(model.SYKI))
            {
                if (string.IsNullOrEmpty(model.TWOWAYCONFIRM))
                {
                    return Json(new
                    {
                        redirect = Url.Action("GoalSettingForm", "HRPMS",
                                   new { SYKI = Encryption.Encrypt(model.SYKI) })
                    });
                }
                else
                {
                    return Json(new
                    {
                        popup = Url.Action("ViewGoalSettingForm", "HRPMS",
                               new { PMSID = Encryption.Encrypt(model.PMSID) })
                    });
                }
            }
            return BadRequest();
        }

        // ---------- FIRST HALF SELF ----------
        //[HttpPost("firsthalf/redirect")]
        [HttpPost]
        public IActionResult RedirectFirstHalf([FromBody] FirstHalfInputModel model)
        {
            if (!string.IsNullOrEmpty(model.PMSID))
            {
                if (model.FHSTATUS == "5" || string.IsNullOrEmpty(model.FHCOMMENT))
                {
                    return Json(new
                    {
                        redirect = Url.Action("FirstHalfForm", "HRPMS",
                                   new { PMSID = Encryption.Encrypt(model.PMSID) })
                    });
                }
                else
                {
                    return Json(new
                    {
                        popup = Url.Action("ViewFirstHalfForm", "HRPMS",
                               new { PMSID = Encryption.Encrypt(model.PMSID) })
                    });
                }
            }
            return BadRequest();
        }
        //public IActionResult GSEvaluator(string PID) => RedirectToAction("Index", "GSEvaluatorForm");
       // public IActionResult FHEvaluator(string PMSID) => RedirectToAction("Index", "FHEvaluatorForm");
        //public IActionResult SHEvaluator(string PID) => RedirectToAction("Index", "SHEvaluatorForm");

        [HttpPost]
        public IActionResult GSEvaluator([FromBody] EvaluatorModel model)
        {
            if (!string.IsNullOrEmpty(model.PMSID))
            {
                    return Json(new
                    {
                        redirect = Url.Action("GSEvaluatorForm", "HRPMS",
                                  new { pid = Encryption.Encrypt(model.PMSID) })
                    });
                
            }
            return BadRequest();
        }
        [HttpPost]
        public IActionResult FHEvaluator([FromBody] EvaluatorModel model)
        {
            if (!string.IsNullOrEmpty(model.PMSID))
            {
                
                    return Json(new
                    {
                        popup = Url.Action("ViewFirstHalfEvaluatorVM", "HRPMS",
                              new { pmsId = Encryption.Encrypt(model.PMSID) })
                    });
                
            }
            return BadRequest();
        }
         
        [HttpPost]
        public IActionResult SHEvaluator([FromBody] EvaluatorModel model)
        {
            if (!string.IsNullOrEmpty(model.PMSID))
            {

                return Json(new
                {
                    popup = Url.Action("ViewSecondHalfEvaluatorForm", "HRPMS",
                          new { PMSID = Encryption.Encrypt(model.PMSID) })
                });

            }
            return BadRequest();
        }

        // ---------- SECOND HALF SELF ----------
        //[HttpPost("secondhalf/redirect")]
        [HttpPost]
        public IActionResult RedirectSecondHalf([FromBody] SecondHalfInputModel model)
        {
            if (!string.IsNullOrEmpty(model.PMSID))
            {
                if (model.SHSTATUS == "12" || string.IsNullOrEmpty(model.SHCOMMENT))
                {
                    return Json(new
                    {
                        redirect = Url.Action("SecondHalfForm", "HRPMS",
                                  new { PMSID = Encryption.Encrypt(model.PMSID) })
                    });
                }
                else
                {
                    return Json(new
                    {
                        popup = Url.Action("ViewSecondHalfForm", "HRPMS",
                              new { PMSID = Encryption.Encrypt(model.PMSID) })
                    });
                }
            }
            return BadRequest();
        }

        // Note: HRPMS is your existing class used in WebForms. If moved to a service, inject that instead.
        
        [HttpGet]
        public IActionResult ViewGoalSettingForm(string PMSID)
        {
            if (string.IsNullOrEmpty(PMSID))
            {
                return BadRequest("Missing PMSID");
            }

            // decrypt & url decode - mirror original
            string hrpmsId;
            try
            {
                // UrlDecode then Decrypt as original did (original: Server.UrlDecode(Encryption.Decrypt(Request.QueryString["PMSID"])))
                hrpmsId = System.Net.WebUtility.UrlDecode(Encryption.Decrypt(PMSID));
            }
            catch (Exception ex)
            {
                // adapt error handling
                return BadRequest("Invalid PMSID");
            }

            DataSet ds = _pms.GETGOALSETTINGDETAIL(hrpmsId);

            var model = MapDataSetToViewModel(ds, hrpmsId);

            return View("ViewGoalSettingForm", model);
        }

        private GoalSettingViewModel MapDataSetToViewModel(DataSet ds, string hrpmsId)
        {
            var model = new GoalSettingViewModel();

            // Table 0 => header details
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                model.DateOfJoining = r["DATEOFJOINING"]?.ToString();
                model.Employee = r["EMPNAME"]?.ToString();
                model.Operation = r["OPERATION"]?.ToString();
                model.Division = r["DIVISION"]?.ToString();
                model.Department = r["DEPARTMENT"]?.ToString();
                model.Section = r["SECTION"]?.ToString();
                model.Evaluator = r["EVALUATOR"]?.ToString();
                model.Reviewer = r["REVIEWER"]?.ToString();
            }

            // Table 1 => role
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                model.Role = ds.Tables[1].Rows[0]["ROLE"]?.ToString();
            }

            // Table 2 => activities (ACTIVITYTYPE 1 = main, 2 = other)
            if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                var dtAct = ds.Tables[2];
                // main activities (ACTIVITYTYPE = 1)
                var dvMain = new DataView(dtAct) { RowFilter = "ACTIVITYTYPE=1" };
                int totalWeight = 0;
                for (int i = 0; i < dvMain.Count; i++)
                {
                    var row = dvMain[i].Row;
                    var av = new ActivityViewModel
                    {
                        SNo = i + 1,
                        Activity = row["ACTIVITY"]?.ToString(),
                        ControlItem = row["CONTROLITEM"]?.ToString(),
                        Target = row["TARGET"]?.ToString(),
                        Weightage = int.TryParse(row["WEIGHTAGE"]?.ToString(), out var w) ? w : 0
                    };
                    totalWeight += av.Weightage;
                    model.Activities.Add(av);
                }
                model.ActivitiesTotalWeightage = totalWeight;

                // other activities (ACTIVITYTYPE = 2)
                var dvOther = new DataView(dtAct) { RowFilter = "ACTIVITYTYPE=2" };
                for (int i = 0; i < dvOther.Count; i++)
                {
                    var row = dvOther[i].Row;
                    var av = new ActivityViewModel
                    {
                        SNo = i + 1,
                        Activity = row["ACTIVITY"]?.ToString(),
                        ControlItem = row["CONTROLITEM"]?.ToString(),
                        Target = row["TARGET"]?.ToString(),
                        Weightage = int.TryParse(row["WEIGHTAGE"]?.ToString(), out var w) ? w : 0
                    };
                    model.OtherActivities.Add(av);
                }

                if (dvOther.Count > 0)
                {
                    // original code used dv_OtherActivity[0]["WEIGHTAGE"] for label; replicate:
                    model.OtherActivitiesWeightageText = "Total Weightage = " + dvOther[0]["WEIGHTAGE"]?.ToString() + "%";
                }
            }
            else
            {
                // PERFASSTBL.Visible = false; in MVC the view will hide if Activities empty
            }

            // Table 3 => competency headers
            if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
            {
                var dtHeader = ds.Tables[3];
                foreach (DataRow r in dtHeader.Rows)
                {
                    var header = new CompetencyHeaderViewModel
                    {
                        HeadDescription = r["HEADDESCRIPTION"]?.ToString(),
                        HeadId = r["HEADID"]?.ToString()
                    };

                    // In original code, RepHeader_ItemDataBound fetched levels via pms.GETCOMPENLEVELWITHSCORE(HRPMSID, headerId)
                    DataSet dsLevels = _pms.GETCOMPENLEVELWITHSCORE(hrpmsId, header.HeadId);
                    if (dsLevels != null && dsLevels.Tables.Count > 0 && dsLevels.Tables[0].Rows.Count > 0)
                    {
                        var dtLvl = dsLevels.Tables[0];
                        for (int i = 0; i < dtLvl.Rows.Count; i++)
                        {
                            var lvlRow = dtLvl.Rows[i];
                            decimal slsHEvalScore = 0, slsHRevScore = 0;
                            try
                            {
                                // original used Encryption.Decrypt on the encrypted values; replicate if they are encrypted
                                var encEval = lvlRow["SLSHEVALSCORE"]?.ToString();
                                var encRev = lvlRow["SLSHREVSCORE"]?.ToString();

                                if (!string.IsNullOrEmpty(encEval))
                                {
                                    var dec = Encryption.Decrypt(encEval);
                                    slsHEvalScore = decimal.TryParse(dec, out var tmp) ? tmp : 0;
                                }
                                if (!string.IsNullOrEmpty(encRev))
                                {
                                    var dec2 = Encryption.Decrypt(encRev);
                                    slsHRevScore = decimal.TryParse(dec2, out var tmp2) ? tmp2 : 0;
                                }
                            }
                            catch
                            {
                                // fallback: try raw numeric parse
                                decimal.TryParse(lvlRow["SLSHEVALSCORE"]?.ToString(), out slsHEvalScore);
                                decimal.TryParse(lvlRow["SLSHREVSCORE"]?.ToString(), out slsHRevScore);
                            }

                            // score calculation matches original Math.Round((((SLSHEVALSCORE *60)/100) + ((SLSHREVSCORE *40)/100)),0)
                            decimal score = Math.Round(((slsHEvalScore * 60m) / 100m) + ((slsHRevScore * 40m) / 100m), 0, MidpointRounding.AwayFromZero);

                            header.Levels.Add(new CompetencyLevelViewModel
                            {
                                SNo = i + 1,
                                LevelDescription = lvlRow["LEVELDESCRIPTION"]?.ToString(),
                                Score = score
                            });
                        }
                    }

                    model.CompetencyHeaders.Add(header);
                }
            }
            else
            {
                // competenciestbl.Visible = false;
            }

            // Table 5 => two-way comments (note: original referenced ds.Tables[5])
            if (ds.Tables.Count > 5 && ds.Tables[5].Rows.Count > 0)
            {
                var r = ds.Tables[5].Rows[0];
                var eval = r["EVALCOMMENT"]?.ToString();
                var rev = r["REVCOMMENT"]?.ToString();
                var ass = r["ASSCOMMENT"]?.ToString();

                if (!string.IsNullOrEmpty(eval)) model.EvalComment = eval;
                if (!string.IsNullOrEmpty(rev)) model.RevComment = rev;
                if (!string.IsNullOrEmpty(ass)) model.AssocComment = ass;
            }
            else
            {
                // twowaycommtable.Visible = false;
            }

            return model;
        }
       // [HttpGet]
        [HttpGet]
        public IActionResult ViewFirstHalfForm(string PMSID)
        {
            // Session check (Page_Load equivalent)
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");

            var model = new ViewFirstHalfViewModel();
            PopulateFirstHalfDetails(model, PMSID);

            return View("ViewFirstHalfForm", model);
        }

        private void PopulateFirstHalfDetails(ViewFirstHalfViewModel model, string PMSID)
        {
            string decryptedPmsId = 
                //Encryption.Decrypt(System.Net.WebUtility.UrlDecode(PMSID));
            System.Net.WebUtility.UrlDecode(Encryption.Decrypt(PMSID));

            DataSet ds = _pms.GETGOALSETTINGDETAIL(decryptedPmsId);

            // -------- Associate Details --------
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow r = ds.Tables[0].Rows[0];
                model.EmployeeName = r["EMPNAME"].ToString();
                model.DateOfJoining = r["DATEOFJOINING"].ToString();
                model.Operation = r["OPERATION"].ToString();
                model.Division = r["DIVISION"].ToString();
                model.Department = r["DEPARTMENT"].ToString();
                model.Section = r["SECTION"].ToString();
                model.Evaluator = r["EVALUATOR"].ToString();
                model.Reviewer = r["REVIEWER"].ToString();
            }

            // -------- Role --------
            if (ds.Tables[1].Rows.Count > 0)
                model.Role = ds.Tables[1].Rows[0]["ROLE"].ToString();

            // -------- PART A --------
            if (ds.Tables[2].Rows.Count > 0)
            {
                var dvAct = new DataView(ds.Tables[2]) { RowFilter = "ACTIVITYTYPE=1" };
                model.Activities = dvAct.ToTable();
                model.ActivityTotalWeight = model.Activities
                    .AsEnumerable()
                    .Sum(r => Convert.ToInt32(r["WEIGHTAGE"]));

                var dvOther = new DataView(ds.Tables[2]) { RowFilter = "ACTIVITYTYPE=2" };
                model.OtherActivities = dvOther.ToTable();
                model.OtherActivityWeight = dvOther.Count > 0
                    ? Convert.ToInt32(dvOther[0]["WEIGHTAGE"]).ToString()
                    : "0";
            }

            // -------- PART B (ItemDataBound replacement) --------
            model.CompetencyData = new List<CompetencyLevelVM>();

            if (ds.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow header in ds.Tables[3].Rows)
                {
                    DataSet dsLevel = _pms.GETCOMPENLEVELWITHSCORE(
                        decryptedPmsId,
                        header["HEADID"].ToString()
                    );
                    DataTable levelTable = dsLevel.Tables.Count > 0
                    ? dsLevel.Tables[0]
                    : new DataTable();

                    if (!levelTable.Columns.Contains("FINAL_SCORE"))
                    {
                        levelTable.Columns.Add("FINAL_SCORE", typeof(decimal));
                    }

                    foreach (DataRow row in levelTable.Rows)
                    {
                        decimal evalScore = 0;
                        decimal revScore = 0;

                        if (row["SLSHEVALSCORE"] != DBNull.Value)
                        {
                            evalScore = Convert.ToDecimal(
                                Encryption.Decrypt(row["SLSHEVALSCORE"].ToString())
                            );
                        }

                        if (row["SLSHREVSCORE"] != DBNull.Value)
                        {
                            revScore = Convert.ToDecimal(
                                Encryption.Decrypt(row["SLSHREVSCORE"].ToString())
                            );
                        }

                        decimal finalScore =
                            ((evalScore * 60) / 100) +
                            ((revScore * 40) / 100);

                        row["FINAL_SCORE"] = Math.Round(
                            finalScore,
                            0,
                            MidpointRounding.AwayFromZero
                        );
                    }


                    model.CompetencyData.Add(new CompetencyLevelVM
                    {
                        HeaderName = header["HEADDESCRIPTION"].ToString(),
                        Levels = levelTable
                    });
                    //model.CompetencyHeaders.Add(new CompetencyLevelVM
                    //{
                    //    HeaderName = header["HEADDESCRIPTION"].ToString(),
                    //    Levels = dsLevel.Tables[0]
                    //});
                }
            }

            // -------- Comments --------
            if (ds.Tables[5].Rows.Count > 0)
            {
                DataRow r = ds.Tables[5].Rows[0];
                model.PartBASComment = r["FHPARTBASSCOMMENT"].ToString();
                model.EvalFeedback = r["FHEVALFEEDBACK"].ToString();
                model.RevFeedback = r["FHREVFEEDBACK"].ToString();
                model.AssocFeedback = r["FHASSCOMMENT"].ToString();
            }
        }

        protected string GetDecryptedString(string value)
        {
            return Encryption.Decrypt(value);
        }

        // Used inside Razor for nested repeater replacement
        public DataTable GetCompetencyLevels(string pmsId, string headerId)
        {
            DataSet ds = _pms.GETCOMPENLEVELWITHSCORE(pmsId, headerId);
            return ds.Tables[0];
        }

        [HttpGet]
        public IActionResult ViewSecondHalfForm(string PMSID)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new ViewSecondHalfFormViewModel();

            string decryptedPmsId = Encryption.Decrypt(Uri.UnescapeDataString(PMSID));
            DataSet ds = _pms.GETGOALSETTINGDETAIL(decryptedPmsId);

            /* ===================== TABLE 0 : EMPLOYEE ===================== */
            if (ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                model.EmployeeName = r["EMPNAME"].ToString();
                model.DateOfJoining = r["DATEOFJOINING"].ToString();
                model.Operation = r["OPERATION"].ToString();
                model.Division = r["DIVISION"].ToString();
                model.Department = r["DEPARTMENT"].ToString();
                model.Section = r["SECTION"].ToString();
                model.Evaluator = r["EVALUATOR"].ToString();
                model.Reviewer = r["REVIEWER"].ToString();
            }

            /* ===================== TABLE 1 : ROLE ===================== */
            if (ds.Tables[1].Rows.Count > 0)
            {
                model.Role = ds.Tables[1].Rows[0]["ROLE"].ToString();
            }

            /* ===================== TABLE 2 : ACTIVITIES ===================== */
            if (ds.Tables[2].Rows.Count > 0)
            {
                var activityTable = ds.Tables[2];

                var dvActivity = new DataView(activityTable) { RowFilter = "ACTIVITYTYPE=1" };
                foreach (DataRowView row in dvActivity)
                {
                    model.Activities.Add(new ActivitySecondHalfViewModel
                    {
                        Activity = row["ACTIVITY"].ToString(),
                        ControlItem = row["CONTROLITEM"].ToString(),
                        Target = row["TARGET"].ToString(),
                        Weightage = Convert.ToInt32(row["WEIGHTAGE"]),
                        FirstHalfResult = row["MYRESULT"].ToString(),
                        SecondHalfResult = row["FYRESULT"].ToString()
                    });

                    model.ActivityTotalWeight += Convert.ToInt32(row["WEIGHTAGE"]);
                }

                var dvOther = new DataView(activityTable) { RowFilter = "ACTIVITYTYPE=2" };
                foreach (DataRowView row in dvOther)
                {
                    model.OtherActivities.Add(new ActivitySecondHalfViewModel
                    {
                        Activity = row["ACTIVITY"].ToString(),
                        ControlItem = row["CONTROLITEM"].ToString(),
                        Target = row["TARGET"].ToString(),
                        FirstHalfResult = row["MYRESULT"].ToString(),
                        SecondHalfResult = row["FYRESULT"].ToString()
                    });
                }
            }

            /* ===================== TABLE 3 : COMPETENCIES ===================== */
            if (ds.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow header in ds.Tables[3].Rows)
                {
                    var headerVm = new CompetencyHeaderSndHalfViewModel
                    {
                        HeaderId = header["HEADID"].ToString(),
                        Description = header["HEADDESCRIPTION"].ToString()
                    };

                    DataSet dsLevel = _pms.GETCOMPENLEVELWITHSCORE(decryptedPmsId, headerVm.HeaderId);
                    if (dsLevel.Tables.Count > 0)
                    {
                        foreach (DataRow lvl in dsLevel.Tables[0].Rows)
                        {
                            decimal eval = Convert.ToDecimal(Encryption.Decrypt(lvl["SLSHEVALSCORE"].ToString()));
                            decimal rev = Convert.ToDecimal(Encryption.Decrypt(lvl["SLSHREVSCORE"].ToString()));

                            headerVm.Levels.Add(new CompetencyLevelSndHalViewModel
                            {
                                Description = lvl["LEVELDESCRIPTION"].ToString(),
                                FinalScore = Math.Round(((eval * 60) / 100) + ((rev * 40) / 100), 0,
                                    MidpointRounding.AwayFromZero)
                            });
                        }
                    }

                    model.CompetencyHeaders.Add(headerVm);
                }
            }

            /* ===================== TABLE 5 : COMMENTS ===================== */
            if (ds.Tables[5].Rows.Count > 0)
            {
                var r = ds.Tables[5].Rows[0];
                model.FirstHalfAssociateComment = r["FHPARTBASSCOMMENT"].ToString();
                model.SecondHalfAssociateComment = r["SHPARTBASSCOMMENT"].ToString();
                model.EvaluatorComment = r["SHEVALFEEDBACKCOMMENT"].ToString();
                model.ReviewerComment = r["SHREVFEEDBACKCOMMENT"].ToString();
                model.AssociateComment = r["SHASSOCIATECOMMENT"].ToString();
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult GSEvaluatorForm(string pid)
        {
            if (string.IsNullOrEmpty(pid))
            {
                return BadRequest("Missing PMSID");
            }

            // decrypt & url decode - mirror original
            //string hrpmsId;
            //try
            //{
            //    // UrlDecode then Decrypt as original did (original: Server.UrlDecode(Encryption.Decrypt(Request.QueryString["PMSID"])))
            //    hrpmsId = System.Net.WebUtility.UrlDecode(Encryption.Decrypt(pid));
            //    pid = hrpmsId;
            //}
            //catch (Exception ex)
            //{
            //    // adapt error handling
            //    return BadRequest("Invalid PMSID");
            //}

            var ecode = _sessionService.Get<string>("userID");
            if (string.IsNullOrWhiteSpace(ecode))
            {
                return RedirectToAction("Index", "Login"); 
            }



            string pmsId = Encryption.Decrypt(pid);
            var vm = new GSEvaluatorViewModel { PMSID = pid };

            DataSet ds = _pms.GETGOALSETTINGDETAIL(pmsId);

            // ----- TABLE 0 : ASSOCIATE -----
            if (ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                vm.Associate = r["EMPNAME"].ToString();
                vm.DateOfJoining = r["DATEOFJOINING"].ToString();
                vm.Department = r["DEPARTMENT"].ToString();
                vm.Section = r["SECTION"].ToString();
                vm.Evaluator = r["EVALUATOR"].ToString();
                vm.Reviewer = r["REVIEWER"].ToString();
                vm.Operation = r["OPERATION"].ToString();
                vm.Division = r["DIVISION"].ToString();
                vm.AssociateEmail = r["EMAILID"].ToString();

                if (r["EVALUATORECODE"].ToString() != _sessionService.Get<string>("userID"))
                    return RedirectToAction("ManagePMS");
            }

            // ----- TABLE 1 : ROLE / STATUS -----
            if (ds.Tables[1].Rows.Count > 0)
            {
                vm.Role = ds.Tables[1].Rows[0]["ROLE"].ToString();
                vm.ShowSendBack = ds.Tables[1].Rows[0]["SENDBACKFLAG"].ToString() != "1";
            }

            // ----- TABLE 2 : ACTIVITIES -----
            if (ds.Tables[2].Rows.Count > 0)
            {
                var dvAct = new DataView(ds.Tables[2]) { RowFilter = "ACTIVITYTYPE=1" };
                vm.Activities = dvAct.ToTable();

                foreach (DataRow a in vm.Activities.Rows)
                    vm.TotalActivityWeight += Convert.ToInt32(a["WEIGHTAGE"]);

                var dvOther = new DataView(ds.Tables[2]) { RowFilter = "ACTIVITYTYPE=2" };
                vm.OtherActivities = dvOther.ToTable();
                if (vm.OtherActivities.Rows.Count > 0)
                    vm.OtherActivityWeight = vm.OtherActivities.Rows[0]["WEIGHTAGE"] + "%";
            }

            // ----- TABLE 3 : COMPETENCIES -----
            if (ds.Tables[3].Rows.Count > 0)
                vm.CompetencyHeaders = ds.Tables[3];

            // ----- TABLE 5 : COMMENTS -----
            if (ds.Tables[5].Rows.Count > 0)
            {
                vm.EvalComment = ds.Tables[5].Rows[0]["EVALCOMMENT"].ToString();
                if (!string.IsNullOrEmpty(vm.EvalComment))
                    vm.ShowButtons = false;
            }

            HttpContext.Session.SetString("ASSEMAIL", vm.AssociateEmail);
            return View("GSEvaluatorForm", vm);
        }

        // ---------------- SEND TO REVIEWER ----------------
        [HttpPost]
        public IActionResult SendToReviewer(GSEvaluatorViewModel model)
        {
            string pmsId = Encryption.Decrypt(model.PMSID);
            string ecode = _sessionService.Get<string>("userID");
            if (string.IsNullOrWhiteSpace(ecode))
            {
                ecode = "";
            }
            string result = _pms.GOALSETTINGSTATUS_UPDATE(pmsId, "2", model.EvalComment, ecode, "");
            var res = result.Split('#');

            if (res[0] == "1")
                return RedirectToAction("ManagePMS");

            TempData["Error"] = res[1];
            return RedirectToAction("GSEvaluatorForm", new { pid = model.PMSID });
        }

        // ---------------- SEND BACK ----------------
        [HttpPost]
        public IActionResult SendBack(GSEvaluatorViewModel model)
        {
            string pmsId = Encryption.Decrypt(model.PMSID);
            string ecode = _sessionService.Get<string>("userID");
            if (string.IsNullOrWhiteSpace(ecode))
            {
                ecode = "";
            }

            string result = _pms.GOALSETTINGSTATUS_UPDATE(pmsId, "1", model.EvalComment, ecode, "");
            var res = result.Split('#');

            if (res[0] == "1")
                return RedirectToAction("ManagePMS");

            TempData["Error"] = res[1];
            return RedirectToAction("GSEvaluatorForm", new { pid = model.PMSID });
        }

        public IActionResult Cancel()
        {
            return RedirectToAction("ManagePMS");
        }
        [HttpGet]
        public IActionResult FHEvaluatorForm(string pid)
        {
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");

            if (string.IsNullOrEmpty(pid))
                return RedirectToAction("ManagePMS");

            string hrpmsId;
            try
            {
                hrpmsId = Encryption.Decrypt((pid));
            }
            catch
            {
                return BadRequest("Invalid PMSID");
            }

            DataSet ds = _pms.GETGOALSETTINGDETAIL(hrpmsId);

            var vm = new FHEvaluatorViewModel
            {
                EncPmsId = pid,
                AssociateTable = ds.Tables[0],
                RoleTable = ds.Tables[1],
                ActivityTable = ds.Tables[2],
                CompetencyTable = ds.Tables[3],
                CommentTable = ds.Tables.Count > 5 ? ds.Tables[5] : null
            };

            return View("FHEvaluatorForm", vm);
        }

        // =====================================================
        // TWO WAY COMMUNICATION SUBMIT (AJAX ONLY)
        // =====================================================
        [HttpPost]
        public IActionResult SubmitFHTwoWay([FromBody] TwoWayInput input)
        {
            if (input == null)
                return BadRequest("Invalid request");

            if (string.IsNullOrWhiteSpace(input.EncPmsId))
                return BadRequest("PMSID missing");

            if (string.IsNullOrWhiteSpace(input.EvalFeedback))
                return BadRequest("Feedback is mandatory");

            if (!input.ConfirmFlag)
                return BadRequest("Confirmation required");

            string hrpmsId = Encryption.Decrypt(input.EncPmsId);
            string ecode = _sessionService.Get<string>("userID") ?? "";

            string result = _pms.FIRSTHALFEVALSUBMIT(
                hrpmsId,
                "3",            // status
                "", "",          // part A / B comments
                ecode,
                "", "",          // xml
                "0",             // flag auth
                "", "", "", "",  // scores
                input.EvalFeedback,
                ""
            );

            var res = result.Split('#');
            if (res[0] != "1")
                return BadRequest(res[1]);

            return Ok(new
            {
                redirect = Url.Action("ManagePMS")
            });
        }

        // =====================================================
        // CANCEL
        // =====================================================
        [HttpGet]
        public IActionResult CancelFH()
        {
            return RedirectToAction("ManagePMS");
        }
        [HttpPost]
        public IActionResult ReviewerPMS([FromBody] ReviewerPmsRequest req)
        {
            if (string.IsNullOrEmpty(req.SYKI))
            {
                return Json(new { success = false, message = "SYKI is mandatory" });
            }

            string ecode = _sessionService.Get<string>("userID") ?? "";

            DataSet ds = _pms.GETAMANAGEPMS(
                req.SYKI,
                ecode,
                req.Operation,
                req.Division,
                req.Department,
                req.Section
            );

            DataTable reviewerTable = ds.Tables.Count > 2 ? ds.Tables[2] : new DataTable();

            int totalRows = reviewerTable.Rows.Count;
            //int totalPages = (int)Math.Ceiling(totalRows / (double)req.PageSize);

            var model = new PmsResultModel
            {
                ReviewerTable = reviewerTable,
                ReviewerPaging = new PagingModel
                {
                    PageIndex = req.RevPage,
                    PageSize = req.PageSize,
                    TotalItems = totalRows
                }
            };

            return PartialView("_ReviewerPmsGrid", model);
        }

        [HttpGet]
        public IActionResult SHEvaluatorForm(string PID)
        {
            if (_sessionService.Get<string>("userID") == null)
                return Redirect("/Login");

            if (string.IsNullOrEmpty(PID))
                return Redirect("/ManagePMS");

            string HRPMSID = Encryption.Decrypt(PID);
            var model = LoadFirstHalfDetails(HRPMSID);

            return View("SHEvaluatorForm", model);
        }

        // =========================================================
        // GET FIRST HALF DETAILS (GETFIRSTHALFDETAILS)
        // =========================================================
        private SHEvaluatorViewModel LoadFirstHalfDetails(string HRPMSID)
        {
            var model = new SHEvaluatorViewModel();
            DataSet ds = _pms.GETGOALSETTINGDETAIL(HRPMSID);

            // ---------------- ASSOCIATE DETAILS ----------------
            if (ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];

                model.HRPMSID = HRPMSID;
                model.Associate = r["EMPNAME"].ToString();
                model.DOJ = r["DATEOFJOINING"].ToString();
                model.Department = r["DEPARTMENT"].ToString();
                model.Section = r["SECTION"].ToString();
                model.Evaluator = r["EVALUATOR"].ToString();
                model.Reviewer = r["REVIEWER"].ToString();
                model.Operation = r["OPERATION"].ToString();
                model.Division = r["DIVISION"].ToString();

                string evalECode = r["EVALUATORECODE"].ToString();
                string sessionUser = _sessionService.Get<string>("userID");

                if (evalECode != sessionUser)
                    throw new UnauthorizedAccessException();

                DateTime start = Convert.ToDateTime(r["EVALSECONDHALFSTART"]);
                DateTime end = Convert.ToDateTime(r["EVALSECONDHALFEND"]);

                if (DateTime.Now.Date < start.Date || DateTime.Now.Date > end.Date)
                    throw new Exception("Evaluation period expired");

                HttpContext.Session.SetString("ASSEMAIL", r["EMAILID"].ToString());
            }

            // ---------------- ROLE + STATUS ----------------
            if (ds.Tables[1].Rows.Count > 0)
            {
                var r = ds.Tables[1].Rows[0];
                if (r["FYRSTATUS"].ToString() != "13")
                    throw new Exception("Invalid evaluation status");

                model.RoleDescription = r["ROLE"].ToString();
               // model.FirstHalfEvalScore = Encryption.Decrypt(r["FHEVALSCORE"].ToString());
            }

            // ---------------- ACTIVITIES ----------------
            model.Activities = new();
            foreach (DataRow r in ds.Tables[2].Select("ACTIVITYTYPE=1"))
            {
                model.Activities.Add(new ActivityVM
                {
                    GoalId = Convert.ToInt32(r["GOALID"].ToString()),
                    Activity = r["ACTIVITY"].ToString(),
                    ControlItem = r["CONTROLITEM"].ToString(),
                    Target = r["TARGET"].ToString(),
                    Weightage = Convert.ToInt32(r["WEIGHTAGE"])
                });
            }

            // ---------------- COMPETENCIES ----------------
            model.Competencies = new();
            foreach (DataRow header in ds.Tables[3].Rows)
            {
                var h = new CompetencyHeaderVM
                {
                    HeaderId = Convert.ToInt32(header["HEADERID"].ToString()),
                    Header = header["HEADER"].ToString(),
                    Levels = new()
                };

                DataSet lvlds = _pms.GETCOMPENLEVELWITHSCORE(HRPMSID, h.HeaderId.ToString());
                foreach (DataRow lvl in lvlds.Tables[0].Rows)
                {
                    h.Levels.Add(new CompetencyLevelVM_SH
                    {
                        CompetencyId = Convert.ToInt32( lvl["COMPETENCYID"].ToString()),
                        Description = lvl["DESCRIPTION"].ToString()
                    });
                }

                model.Competencies.Add(h);
            }

            return model;
        }

        // =========================================================
        // SAVE PART A
        // =========================================================
        [HttpPost]
        public IActionResult SavePartA([FromBody] SavePartARequest model)
        {
            string ECODE = _sessionService.Get<string>("userID");
            StringBuilder xml = new("<ACT>");

            foreach (var a in model.Activities)
            {
                xml.Append("<ACTLIST>");
                xml.Append($"<SC>{Encryption.Encrypt(a.Score)}</SC>");
                xml.Append("<TYP>1</TYP>");
                xml.Append($"<GID>{a.GoalId}</GID>");
                xml.Append("</ACTLIST>");
            }

            xml.Append("</ACT>");

            string result = _pms.SECONDHALFEVALSUBMIT(
                model.PID, "1",
                Encryption.Encrypt(model.OtherActivityScore),
                model.PartAEvalComment,
                xml.ToString(),
                "", "", "", "", "", "", "",
                ECODE, ""
            );

            var s = result.Split('#');
            return Json(new { success = s[0] == "1", message = s[1] });
        }

        // =========================================================
        // SAVE PART B
        // =========================================================
        [HttpPost]
        public IActionResult SavePartB([FromBody] SavePartBRequest model)
        {
            string ECODE = _sessionService.Get<string>("userID");
            StringBuilder xml = new("<COMP>");

            foreach (var c in model.Competencies)
            {
                xml.Append("<COMPLIST>");
                xml.Append($"<SC>{Encryption.Encrypt(c.Score)}</SC>");
                xml.Append($"<CID>{c.CompetencyId}</CID>");
                xml.Append("</COMPLIST>");
            }

            xml.Append("</COMP>");

            string result = _pms.SECONDHALFEVALSUBMIT(
                model.PID, "2", "", "", "",
                xml.ToString(),
                model.PartBEvalComment,
                "", "",
                Encryption.Encrypt(model.FinalEvalScore),
                Encryption.Encrypt(model.FinalRating),
                Encryption.Encrypt(model.FinalSHScore),
                ECODE,
                Encryption.Encrypt(model.FinalReviewerScore)
            );

            var s = result.Split('#');
            return Json(new { success = s[0] == "1", message = s[1] });
        }

        // =========================================================
        // SEND TO REVIEWER
        // =========================================================
        [HttpPost]
        public IActionResult SendToReviewer([FromBody] SendToReviewerRequest model)
        {
            string ECODE = _sessionService.Get<string>("userID");
            string FLAGAUTH = model.IsEvaluatorReviewerSame ? "1" : "0";

            string result = _pms.SECONDHALFEVALSUBMIT(
                model.PID, "3", "", "", "", "",
                "", model.EvalFeedbackComment,
                FLAGAUTH, "", "", "",
                ECODE, ""
            );

            if (FLAGAUTH == "1")
                SendMailToAssociate();

            var s = result.Split('#');
            return Json(new { success = s[0] == "1", redirect = "/ManagePMS" });
        }

        // =========================================================
        // VIEW FORM POPUP
        // =========================================================
        [HttpGet]
        public IActionResult ViewForm(string PID)
        {
            string enc = Encryption.Encrypt(PID);
            return Redirect($"/ViewSecondHalfEvaluatorForm?PMSID={enc}");
        }

        // =========================================================
        // MAIL
        // =========================================================
        private void SendMailToAssociate()
        {
            string email = HttpContext.Session.GetString("ASSEMAIL");
            if (string.IsNullOrEmpty(email)) return;

            commanEmail mail = new commanEmail
            {
                MailFrom = "portal.admin@honda.hmsi.in",
                MailTo = email,
                MailSubject = "Fill Two Way Communication",
                MailBody = "Second Half evaluation completed."
            };
            mail.Send();
        }
        [HttpGet]
        public IActionResult ViewFirstHalfEvaluatorVM(string pmsId)
        {
            if (string.IsNullOrEmpty(pmsId))
                return RedirectToAction("Index", "ManagePMS");

            string hrpmsId = Encryption.Decrypt(pmsId);
            DataSet ds = _pms.GETGOALSETTINGDETAIL(hrpmsId);

            var vm = new ViewFirstHalfEvaluatorVM();

            // ===== Associate Details =====
            var r0 = ds.Tables[0].Rows[0];
            vm.Associate = r0["EMPNAME"].ToString();
            vm.DateOfJoining = r0["DATEOFJOINING"].ToString();
            vm.Operation = r0["OPERATION"].ToString();
            vm.Division = r0["DIVISION"].ToString();
            vm.Department = r0["DEPARTMENT"].ToString();
            vm.Section = r0["SECTION"].ToString();
            vm.Evaluator = r0["EVALUATOR"].ToString();
            vm.Reviewer = r0["REVIEWER"].ToString();

            string partAWt = r0["PARTAWEIGHTAGE"].ToString();
            string partBWt = r0["PARTBWEIGHTAGE"].ToString();

            // ===== Role =====
            vm.Role = ds.Tables[1].Rows[0]["ROLE"].ToString();
            vm.OtherActivityEvalScore =
                Convert.ToInt32(Encryption.Decrypt(ds.Tables[1].Rows[0]["FHEVALSCORE"].ToString()));

            // ===== Activities =====
            var dvAct = new DataView(ds.Tables[2]) { RowFilter = "ACTIVITYTYPE=1" };
            foreach (DataRowView r in dvAct)
            {
                vm.Activities.Add(new ActivityVM_View
                {
                    Activity = r["ACTIVITY"].ToString(),
                    ControlItem = r["CONTROLITEM"].ToString(),
                    Target = r["TARGET"].ToString(),
                    Weightage = Convert.ToInt32(r["WEIGHTAGE"]),
                    MyResult = r["MYRESULT"].ToString(),
                    EvalScore = Convert.ToInt32(Encryption.Decrypt(r["MYEVLTSCORE"].ToString()))
                });
            }
            vm.ActivityWeightage = vm.Activities.Sum(x => x.Weightage);

            var dvOther = new DataView(ds.Tables[2]) { RowFilter = "ACTIVITYTYPE=2" };
            foreach (DataRowView r in dvOther)
            {
                vm.OtherActivities.Add(new ActivityVM_View
                {
                    Activity = r["ACTIVITY"].ToString(),
                    ControlItem = r["CONTROLITEM"].ToString(),
                    Target = r["TARGET"].ToString(),
                    MyResult = r["MYRESULT"].ToString(),
                    Weightage = Convert.ToInt32(r["WEIGHTAGE"])
                });
            }
            vm.OtherActivityWeightage = dvOther.Count > 0 ? Convert.ToInt32(dvOther[0]["WEIGHTAGE"]) : 0;

            // ===== Competencies =====
            foreach (DataRow h in ds.Tables[3].Rows)
            {
                var header = new CompetencyHeaderVM_View
                {
                    HeaderId = h["HEADID"].ToString(),
                    HeaderDescription = h["HEADDESCRIPTION"].ToString()
                };

                var cds = _pms.GETCOMPENLEVELWITHSCORE(hrpmsId, header.HeaderId);
                foreach (DataRow l in cds.Tables[0].Rows)
                {
                    header.Levels.Add(new CompetencyLevelVM_View
                    {
                        LevelDescription = l["LEVELDESCRIPTION"].ToString(),
                        SkillLevel = Math.Round(
                            ((Convert.ToDecimal(Encryption.Decrypt(l["SLSHEVALSCORE"].ToString())) * 60) +
                             (Convert.ToDecimal(Encryption.Decrypt(l["SLSHREVSCORE"].ToString())) * 40)) / 100,
                            0, MidpointRounding.AwayFromZero),
                        EvalScore = Convert.ToInt32(Encryption.Decrypt(l["MYEVLTSCORE"].ToString()))
                    });
                }
                vm.Competencies.Add(header);
            }

            // ===== Comments =====
            var c = ds.Tables[5].Rows[0];
            vm.PartAEvalComment = c["FHPARTAEVALCOMMENT"].ToString();
            vm.PartBAssComment = c["FHPARTBASSCOMMENT"].ToString();
            vm.PartBEvalComment = c["FHPARTBEVALCOMMENT"].ToString();
            vm.TwoWayEvalComment = c["FHEVALFEEDBACK"].ToString();
            vm.TwoWayAssocComment = c["FHASSCOMMENT"].ToString();

            // ===== Score Calculation (100% same) =====
            decimal partAScore = vm.Activities.Sum(a => a.Weightage * a.EvalScore / 100M);
            partAScore += vm.OtherActivityWeightage * vm.OtherActivityEvalScore / 100M;

            //  decimal partBScore = vm.Competencies.SelectMany(x => x.Levels).Average(x => x.EvalScore);
            var partBLevels = vm.Competencies.SelectMany(x => x.Levels).ToList();

            decimal partBScore = partBLevels.Any()
                ? partBLevels.Average(x => (decimal)x.EvalScore)
                : 0m;

            vm.Formula =
                $"({partAWt}% of PART A = {(partAScore * Convert.ToInt32(partAWt)) / 100}) + " +
                $"({partBWt}% of PART B = {(partBScore * Convert.ToInt32(partBWt)) / 100}) =";

            vm.TotalScore =
                ((partAScore * Convert.ToInt32(partAWt)) / 100) +
                ((partBScore * Convert.ToInt32(partBWt)) / 100);

            return View(vm);
        }
        [HttpGet]
        public IActionResult ViewSecondHalfEvaluatorForm(string PMSID)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //    return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(PMSID))
                return RedirectToAction("ManagePMS");

            string hrpmsId = Encryption.Decrypt(PMSID);

            DataSet ds = _pms.GETGOALSETTINGDETAIL(hrpmsId);

            var vm = new ViewSecondHalfEvaluatorVM();

            /* ===== TABLE 0 ===== */
            if (ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                vm.DateOfJoining = r["DATEOFJOINING"].ToString();
                vm.EmployeeName = r["EMPNAME"].ToString();
                vm.Department = r["DEPARTMENT"].ToString();
                vm.Section = r["SECTION"].ToString();
                vm.Evaluator = r["EVALUATOR"].ToString();
                vm.Reviewer = r["REVIEWER"].ToString();
                vm.Operation = r["OPERATION"].ToString();
                vm.Division = r["DIVISION"].ToString();
                vm.PartAWeightage = r["PARTAWEIGHTAGE"].ToString();
                vm.PartBWeightage = r["PARTBWEIGHTAGE"].ToString();

                vm.FirstHalfFormula =
                    $"60% of (({vm.PartAWeightage}% of PART A) + ({vm.PartBWeightage}% of PART B)) =";
            }

            /* ===== TABLE 1 ===== */
            if (ds.Tables[1].Rows.Count > 0)
            {
                var r = ds.Tables[1].Rows[0];
                vm.Role = r["ROLE"].ToString();
                vm.FirstHalfTotalScore = Encryption.Decrypt(r["FHEVALTTLSCORE"].ToString());
                vm.SecondHalfTotalScore = Encryption.Decrypt(r["SHEVALTTLSCORE"].ToString());
                vm.FirstHalfScore = Encryption.Decrypt(r["FHEVALSCORE"].ToString());
                vm.SecondHalfScore = Encryption.Decrypt(r["SHEVALSCORE"].ToString());
            }

            /* ===== TABLE 2 (ACTIVITIES) ===== */
            vm.Activities = ds.Tables[2].Select("ACTIVITYTYPE=1");
            vm.OtherActivities = ds.Tables[2].Select("ACTIVITYTYPE=2");

            if (vm.Activities.Length > 0)
                vm.PartAActivityWeightage = vm.Activities.Sum(x => Convert.ToInt32(x["WEIGHTAGE"]));

            if (vm.OtherActivities.Length > 0)
                vm.OtherActivityWeightage = vm.OtherActivities[0]["WEIGHTAGE"].ToString();

            /* ===== TABLE 3 (HEADERS) ===== */
            vm.Headers = ds.Tables[3];

            /* ===== TABLE 5 (COMMENTS) ===== */
            if (ds.Tables[5].Rows.Count > 0)
            {
                var r = ds.Tables[5].Rows[0];
                vm.FHPartAComment = r["FHPARTAEVALCOMMENT"].ToString();
                vm.SHPartAComment = r["SHPARTAEVALCOMMENT"].ToString();
                vm.FHPartBComment = r["FHPARTBASSCOMMENT"].ToString();
                vm.SHPartBComment = r["SHPARTBASSCOMMENT"].ToString();
                vm.SHPartBEvalComment = r["SHPARTBEVALCOMMENT"].ToString();
                vm.FHFeedback = r["FHEVALFEEDBACK"].ToString();
                vm.SHFeedback = r["SHEVALFEEDBACKCOMMENT"].ToString();
                vm.AssociateComment = r["SHASSOCIATECOMMENT"].ToString();
            }

            /* ===== SCORE CALCULATION ===== */
            decimal partAScore = 0;
            foreach (var r in vm.Activities)
            {
                partAScore +=
                    (Convert.ToDecimal(r["WEIGHTAGE"]) *
                     Convert.ToDecimal(Encryption.Decrypt(r["FYEVLTSCORE"].ToString()))) / 100;
            }

            partAScore +=
                (Convert.ToDecimal(vm.OtherActivityWeightage) *
                 Convert.ToDecimal(vm.SecondHalfScore)) / 100;

            decimal partBScore = 0;
            foreach (DataRow r in ds.Tables[4].Rows)
                partBScore += Convert.ToDecimal(Encryption.Decrypt(r["FYEVLTSCORE"].ToString()));

            partBScore /= ds.Tables[4].Rows.Count;

            vm.SecondHalfFormula =
                $"60% of (({vm.PartAWeightage}% of PART A = {(partAScore * Convert.ToInt32(vm.PartAWeightage)) / 100}) " +
                $"+ ({vm.PartBWeightage}% of PART B = {(partBScore * Convert.ToInt32(vm.PartBWeightage)) / 100})) =";

            return View(vm);
        }

        /* ===== AJAX / PARTIAL ===== */
        public PartialViewResult CompetencyLevels(string pmsId, string headerId)
        {
            var ds = _pms.GETCOMPENLEVELWITHSCORE(pmsId, headerId);
            return PartialView("_CompetencyLevels", ds.Tables[0]);
        }
        public IActionResult ViewFirstHalfReviewerForm(string pmsid)
        {
            //if (string.IsNullOrEmpty(_sessionService.Get<string>("userID")))
            //    return RedirectToAction("Login", "Account");

            string hrpmsId = Encryption.Decrypt(pmsid);
            var model = new ViewFirstHalfReviewerVM();

            DataSet ds = _pms.GETGOALSETTINGDETAIL(hrpmsId);

            // Associate Details
            if (ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                model.Associate = r["EMPNAME"].ToString();
                model.DateOfJoining = r["DATEOFJOINING"].ToString();
                model.Department = r["DEPARTMENT"].ToString();
                model.Section = r["SECTION"].ToString();
                model.Evaluator = r["EVALUATOR"].ToString();
                model.Reviewer = r["REVIEWER"].ToString();
                model.Operation = r["OPERATION"].ToString();
                model.Division = r["DIVISION"].ToString();
            }

            // Role & Scores
            if (ds.Tables[1].Rows.Count > 0)
            {
                var r = ds.Tables[1].Rows[0];
                model.Role = r["ROLE"].ToString();
                model.EvalScore = Encryption.Decrypt(r["FHEVALTTLSCORE"].ToString());
                model.RevScore = Encryption.Decrypt(r["FHREVTTLSCORE"].ToString());
                model.EvalOtherActScore = Encryption.Decrypt(r["FHEVALSCORE"].ToString());
                model.RevOtherActScore = Encryption.Decrypt(r["FHREVSCORE"].ToString());
                model.FinalScore = Encryption.Decrypt(r["FHFINALSCORE"].ToString());
            }

            // Activities
            var dvAct = new DataView(ds.Tables[2]) { RowFilter = "ACTIVITYTYPE=1" };
            model.ActivityWeightage = dvAct.ToTable().AsEnumerable().Sum(x => Convert.ToInt32(x["WEIGHTAGE"]));

            foreach (DataRowView a in dvAct)
            {
                model.Activities.Add(new ActivityVM_FHR
                {
                    Activity = a["ACTIVITY"].ToString(),
                    ControlItem = a["CONTROLITEM"].ToString(),
                    Target = a["TARGET"].ToString(),
                    Weightage = Convert.ToInt32(a["WEIGHTAGE"]),
                    MyResult = a["MYRESULT"].ToString(),
                    EvalScore = Encryption.Decrypt(a["MYEVLTSCORE"].ToString()),
                    RevScore = Encryption.Decrypt(a["MYRVWRSCORE"].ToString())
                });
            }

            // Competencies
            foreach (DataRow h in ds.Tables[3].Rows)
            {
                var header = new CompetencyHeaderVM_FHR
                {
                    HeaderDescription = h["HEADDESCRIPTION"].ToString()
                };

                DataSet lvlDs = _pms.GETCOMPENLEVELWITHSCORE(hrpmsId, h["HEADID"].ToString());
                foreach (DataRow l in lvlDs.Tables[0].Rows)
                {
                    header.Levels.Add(new CompetencyLevelVM_FHR
                    {
                        LevelDescription = l["LEVELDESCRIPTION"].ToString(),
                        SkillLevel = Encryption.Decrypt(l["SLSHEVALSCORE"].ToString()),
                        EvalScore = Encryption.Decrypt(l["MYEVLTSCORE"].ToString()),
                        RevScore = Encryption.Decrypt(l["MYRVWRSCORE"].ToString())
                    });
                }
                model.Competencies.Add(header);
            }

            // Comments
            if (ds.Tables[5].Rows.Count > 0)
            {
                var r = ds.Tables[5].Rows[0];
                model.PartAEvalComment = r["FHPARTAEVALCOMMENT"].ToString();
                model.PartBAssComment = r["FHPARTBASSCOMMENT"].ToString();
                model.PartBEvalComment = r["FHPARTBEVALCOMMENT"].ToString();
                model.EvalTwoWayComment = r["FHEVALFEEDBACK"].ToString();
                model.RevTwoWayComment = r["FHREVFEEDBACK"].ToString();
                model.AssocTwoWayComment = r["FHASSCOMMENT"].ToString();
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult ViewFirstHalfReviewer([FromBody] EvaluatorModel model)
        {
            if (!string.IsNullOrEmpty(model.PMSID))
            {

                return Json(new
                {
                    popup = Url.Action("ViewFirstHalfReviewerForm", "HRPMS",
                          new { pmsid = Encryption.Encrypt(model.PMSID) })
                });

            }
            return BadRequest();
        }
        [HttpPost]
        public IActionResult ViewSecondHalfReviewer([FromBody] EvaluatorModel model)
        {
            if (!string.IsNullOrEmpty(model.PMSID))
            {

                return Json(new
                {
                    popup = Url.Action("ViewSecondHalfReviewerForm", "HRPMS",
                          new { PMSID = Encryption.Encrypt(model.PMSID) })
                });

            }
            return BadRequest();
        }

        public IActionResult ViewSecondHalfReviewerForm(string PMSID)
        {
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(PMSID))
                return RedirectToAction("ManagePMS");

            string hrpmsId = Encryption.Decrypt(PMSID);
            var model = LoadReviewerData(hrpmsId);

            return View(model);
        }

        private ViewSecondHalfReviewerVM LoadReviewerData(string hrpmsId)
        {
            var vm = new ViewSecondHalfReviewerVM();
            DataSet ds = _pms.GETGOALSETTINGDETAIL(hrpmsId);

            // ================= TABLE 0 : BASIC DETAILS =================
            if (ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                vm.Associate = r["EMPNAME"].ToString();
                vm.DateOfJoining = r["dateofjoining"].ToString();
                vm.Department = r["DEPARTMENT"].ToString();
                vm.Section = r["SECTION"].ToString();
                vm.Evaluator = r["EVALUATOR"].ToString();
                vm.Reviewer = r["REVIEWER"].ToString();
                vm.Operation = r["OPERATION"].ToString();
                vm.Division = r["DIVISION"].ToString();

                vm.PartAWeightage = Convert.ToInt32(r["PARTAWEIGHTAGE"]);
                vm.PartBWeightage = Convert.ToInt32(r["PARTBWEIGHTAGE"]);

                vm.FHEvalFormula = $"60% of (({vm.PartAWeightage}% of PART A) + ({vm.PartBWeightage}% of PART B)) =";
                vm.FHRevFormula = $"40% of (({vm.PartAWeightage}% of PART A) + ({vm.PartBWeightage}% of PART B)) =";
                vm.SHEvalFormula = vm.FHEvalFormula;
            }

            // ================= TABLE 1 : SCORES =================
            if (ds.Tables[1].Rows.Count > 0)
            {
                var r = ds.Tables[1].Rows[0];
                vm.Role = r["ROLE"].ToString();

                vm.FHEvalScore = Encryption.Decrypt(r["FHEVALSCORE"].ToString());
                vm.FHRevScore = Encryption.Decrypt(r["FHREVSCORE"].ToString());
                vm.SHEvalScore = Encryption.Decrypt(r["SHEVALSCORE"].ToString());
                vm.SHRevScore = Encryption.Decrypt(r["SHREVSCORE"].ToString());

                vm.FHEvalTotal = Encryption.Decrypt(r["FHEVALTTLSCORE"].ToString());
                vm.FHRevTotal = Encryption.Decrypt(r["FHREVTTLSCORE"].ToString());
                vm.SHTotal = Encryption.Decrypt(r["SHFINALSCORE"].ToString());
            }

            // ================= TABLE 2 : ACTIVITIES =================
            if (ds.Tables[2].Rows.Count > 0)
            {
                var dvA = new DataView(ds.Tables[2]) { RowFilter = "ACTIVITYTYPE=1" };
                var dvO = new DataView(ds.Tables[2]) { RowFilter = "ACTIVITYTYPE=2" };

                vm.Activities = dvA.ToTable();
                vm.OtherActivities = dvO.ToTable();
            }

            // ================= TABLE 3 : COMPETENCIES =================
            if (ds.Tables[3].Rows.Count > 0)
                vm.CompetencyHeaders = ds.Tables[3];

            // ================= TABLE 5 : COMMENTS =================
            if (ds.Tables[5].Rows.Count > 0)
            {
                var r = ds.Tables[5].Rows[0];
                vm.FHEvalComment = r["FHEVALFEEDBACK"].ToString();
                vm.FHRevComment = r["FHREVFEEDBACK"].ToString();
                vm.SHEvalComment = r["SHEVALFEEDBACKCOMMENT"].ToString();
                vm.SHRevComment = r["SHREVFEEDBACKCOMMENT"].ToString();
                vm.AssociateComment = r["SHASSOCIATECOMMENT"].ToString();
            }

            return vm;
        }
        [HttpPost]
        public IActionResult GSReviewer([FromBody] EvaluatorModel model)
        {
            if (!string.IsNullOrEmpty(model.PMSID))
            {

                return Json(new
                {
                    redirect = Url.Action("GSReviewerForm", "HRPMS",
                                   new { pid = Encryption.Encrypt(model.PMSID) })
                });

            }
            return BadRequest();
        }

        public IActionResult GSReviewerForm(string pid)
        {
            if (string.IsNullOrEmpty(pid))
                return RedirectToAction("ManagePMS", "HRPMS");

            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");


            string pmsId = Encryption.Decrypt(pid);
            DataSet ds = _pms.GETGOALSETTINGDETAIL(pmsId);

            GSReviewerViewModel vm = new();

            if (ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                vm.AssociateName = r["EMPNAME"].ToString();
                vm.DateOfJoining = r["DATEOFJOINING"].ToString();
                vm.Department = r["DEPARTMENT"].ToString();
                vm.Section = r["SECTION"].ToString();
                vm.Evaluator = r["EVALUATOR"].ToString();
                vm.Reviewer = r["REVIEWER"].ToString();
                vm.ReviewerECode = r["REVIEWERECODE"].ToString();
                vm.Operation = r["OPERATION"].ToString();
                vm.Division = r["DIVISION"].ToString();
                vm.AssociateEmail = r["EMAILID"].ToString();
            }

            if (vm.ReviewerECode != _sessionService.Get<string>("userID"))
                return RedirectToAction("Index", "ManagePMS");

            if (ds.Tables[1].Rows.Count > 0)
            {
                vm.Role = ds.Tables[1].Rows[0]["ROLE"].ToString();
                vm.GSStatus = ds.Tables[1].Rows[0]["GSSTATUS"].ToString();
            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    var act = new ActivityVM_GSRF
                    {
                        Activity = dr["ACTIVITY"].ToString(),
                        ControlItem = dr["CONTROLITEM"].ToString(),
                        Target = dr["TARGET"].ToString(),
                        Weightage = Convert.ToInt32(dr["WEIGHTAGE"])
                    };

                    if (dr["ACTIVITYTYPE"].ToString() == "1")
                        vm.Activities.Add(act);
                    else
                        vm.OtherActivities.Add(act);
                }
            }

            if (ds.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow hdr in ds.Tables[3].Rows)
                {
                    var header = new CompetencyHeaderVM_GSRF
                    {
                        HeaderDescription = hdr["HEADDESCRIPTION"].ToString()
                    };

                    DataSet lvl = _pms.GETCOMPENLEVELWITHSCORE(pmsId, hdr["HEADID"].ToString());

                    foreach (DataRow l in lvl.Tables[0].Rows)
                    {
                        decimal eval = Convert.ToDecimal(Encryption.Decrypt(l["SLSHEVALSCORE"].ToString()));
                        decimal rev = Convert.ToDecimal(Encryption.Decrypt(l["SLSHREVSCORE"].ToString()));

                        header.Levels.Add(new CompetencyLevelVM_GSRF
                        {
                            LevelDescription = l["LEVELDESCRIPTION"].ToString(),
                            SkillLevel = Math.Round((eval * 0.6m) + (rev * 0.4m), 0)
                        });
                    }

                    vm.Competencies.Add(header);
                }
            }

            if (ds.Tables[5].Rows.Count > 0)
            {
                vm.EvaluatorComment = ds.Tables[5].Rows[0]["EVALCOMMENT"].ToString();
                vm.ReviewerComment = ds.Tables[5].Rows[0]["REVCOMMENT"].ToString();
                vm.IsEditable = vm.GSStatus == "3";
            }

            vm.PMSID = pid;
            return View(vm);
        }

        // AJAX GET – NO POST
        public IActionResult Submit(string pid, string comment)
        {
            string pmsId = Encryption.Decrypt(pid);
            string ecode = _sessionService.Get<string>("userID");

            var result = _pms.GOALSETTINGSTATUS_UPDATE(pmsId, "3", comment, ecode, "");
            return Json(new { success = result.StartsWith("1") });
        }
    }
      
    }
    