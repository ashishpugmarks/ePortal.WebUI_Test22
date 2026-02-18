using System.Globalization;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class RiskAssessmentController : Controller
    {
        private readonly IRiskAssessmentService _objRiskAssessmentService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        public RiskAssessmentController(IRiskAssessmentService objRiskAssessmentService, ISessionService sessionService, ILogger<HomeController> logger)
        {
            _objRiskAssessmentService = objRiskAssessmentService;
            _sessionService = sessionService;
            _logger = logger;
        }

        public IActionResult RiskPendingForApprovalForDivHead()
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objRiskAssessmentService.GetRiskPendingForApprovalForDivHead(userid);
            return Json(result);
        }

        public IActionResult RiskPendingForApprovalForOPHead()
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objRiskAssessmentService.Get_Risk_Details_For_OpHeadORDivHead(userid);
            return Json(result);
        }
        public IActionResult GetSelfRiskPending()
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objRiskAssessmentService.GetSelfRiskPending(userid);
            return Json(result);
        }

        public IActionResult GetSelfRiskRequest()
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objRiskAssessmentService.GetSelfRiskRequest(userid);
            return Json(result);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RiskAssessmentPeriodSettingITGRC()
        {
            try
            {
                TempData["PageHead"] = "Risk Assessment - Period Setting for Current Ki";
                List<SYKI> iList = new List<SYKI>();

                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }

                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

                List<ADORGLEVEL> _opList = _objRiskAssessmentService.GetOrgLevelList((long)1);
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");

                var PeriodSettingList = _objRiskAssessmentService.GetRiskAssessmentPeriodSettingITGRCList();
                ViewBag.PeriodSettingList = PeriodSettingList;
                ViewBag.CurrentSYKIData = iList.Select(x => x.KICODE).FirstOrDefault();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in RiskAssessmentPeriodSettingITGRC. Message: " + ex.Message.ToString());
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RiskAssessmentPeriodSettingITGRC(RiskAssessmentPeriodForITGRCMMemberNominationListVM model, int? id)
        {
            try
            {
                RiskAssessmentForITGRCMemberNominationVM PeriodSetting = new RiskAssessmentForITGRCMemberNominationVM();
                var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
                model.CreatedBy = userid;
                CultureInfo culture = new CultureInfo("en-GB");
                if (id == 0 || id == null)
                {
                    PeriodSetting = _objRiskAssessmentService.InsertUpdateRiskAssessmentPeriodITGRCSettingDetail(model);
                }


                if (PeriodSetting.Status == 1)
                {
                    TempData["msg"] = PeriodSetting.Msg;

                    var sendmailist = _objRiskAssessmentService.GetISSCMemberList((long)model.SYKIID, 0, 0);

                    foreach (var item in sendmailist)
                    {
                        SendMailTo_ISSC_Member_ToFill_Risk_Details(item.Empname, item.Emailid, Convert.ToDateTime(model.StartDate, culture), Convert.ToDateTime(model.EndDate, culture));
                    }
                }

                else
                {
                    TempData["msg"] = PeriodSetting.Msg;
                }
                return RedirectToAction("RiskAssessmentPeriodSettingITGRC");
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in RiskAssessmentPeriodSettingITGRC. Message: " + ex.Message);
                return RedirectToAction("RiskAssessmentPeriodSettingITGRC");
            }
        }

        [HttpPost]
        public JsonResult BulkUpdate(string selectIDs)
        {
            BulkUpdateRiskPeriodForITGRCMMemberNominationVM BulkUpdate = new BulkUpdateRiskPeriodForITGRCMMemberNominationVM();
            BulkUpdate.lstPeriodForITGRC = new List<RiskAssessmentPeriodForITGRCMMemberNominationListVM>();
            BulkUpdate.PeriodForITGRC = new RiskAssessmentPeriodForITGRCMMemberNominationListVM();
            try
            {
                string[] ids = selectIDs.Split(',');
                BulkUpdate.PeriodForITGRC.SelectedIds = selectIDs;
                if (ids.Length > 0)
                {
                    foreach (var item in ids)
                    {
                        var newLst = _objRiskAssessmentService.GetRisk_Assessment_period_ITGRC_List_WithID(Convert.ToInt16(item));
                        if (newLst.Count > 0 && newLst != null)
                        {
                            foreach (var items in newLst)
                            {
                                RiskAssessmentPeriodForITGRCMMemberNominationListVM objModel = new RiskAssessmentPeriodForITGRCMMemberNominationListVM();
                                objModel.SYKI = items.SYKI;
                                objModel.OperationName = items.OperationName;
                                objModel.DivisionName = items.DivisionName;
                                DateTime sdate = Convert.ToDateTime(items.StartDate);
                                DateTime edate = Convert.ToDateTime(items.EndDate);
                                objModel.StartDate = sdate.ToString("dd/MM/yyyy");
                                objModel.EndDate = edate.ToString("dd/MM/yyyy");
                                //objModel.StartDate = items.StartDate;
                                //objModel.EndDate = items.EndDate;
                                BulkUpdate.lstPeriodForITGRC.Add(objModel);
                                BulkUpdate.PeriodForITGRC.SYKI = items.SYKI;
                            }

                        }
                    }
                }

                return Json(BulkUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in BulkUpdate. Message: " + ex.Message);
            }
            return Json(BulkUpdate);
        }

        [HttpPost]
        public ActionResult BulkUpdatePeriodITGRCSettingNew(BulkUpdatePeriod model)
        {
            try
            {
                var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
                CultureInfo culture = new CultureInfo("en-GB");
                string[] ids = model.SelectedIds.Split(',');
                if (ids.Length > 0)
                {
                    foreach (var item in ids)
                    {
                        BulkUpdatePeriod bulkObj = new BulkUpdatePeriod();
                        bulkObj.CreatedBy = userid;
                        bulkObj.ID = Convert.ToDecimal(item);
                        //bulkObj.StartDate = Convert.ToDateTime(model.StartDate).ToString("dd/MM/yyyy");
                        //bulkObj.EndDate = Convert.ToDateTime(model.EndDate).ToString("dd/MM/yyyy");
                        bulkObj.StartDate = model.StartDate == null ? Convert.ToDateTime(model.StartDate).ToString("dd/MM/yyyy") : DateTime.ParseExact(model.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                        bulkObj.EndDate = model.EndDate == null ? Convert.ToDateTime(model.EndDate).ToString("dd/MM/yyyy") : DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");

                        var result = _objRiskAssessmentService.BulkUpdateITGRCNomination(bulkObj);

                    }
                    TempData["msg"] = "Data has been update succesufully.";
                    List<SYKI> iList = new List<SYKI>();
                    var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                    foreach (var item in kiData._SYKIList)
                    {
                        iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                    }
                    var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                    ViewBag.CurrKey = current_ki;
                    try
                    {
                        var sendmailist = _objRiskAssessmentService.GetISSCMemberListForBulkUpdate(model.SelectedIds, (long)current_ki);
                        if (sendmailist.Count > 0)
                        {
                            foreach (var item in sendmailist)
                            {
                                SendMailTo_ISSC_Member_ToFill_Risk_Details(item.Empname, item.Emailid, Convert.ToDateTime(item.StartDate, culture), Convert.ToDateTime(item.EndDate, culture));
                            }
                        }
                        else
                        {
                            return RedirectToAction("RiskAssessmentPeriodSettingITGRC");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Exception in BulkUpdatePeriodITGRCSettingNew - GetISSCMemberListForBulkUpdate. Message: " + ex.Message);
                        return RedirectToAction("RiskAssessmentPeriodSettingITGRC");
                    }


                }
                return RedirectToAction("RiskAssessmentPeriodSettingITGRC");
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in BulkUpdatePeriodITGRCSettingNew. Message: " + ex.Message.ToString());
                return RedirectToAction("RiskAssessmentPeriodSettingITGRC");
            }

        }

        [HttpGet]
        public ActionResult RiskRegister(int? Id)
        {
            try
            {
                TempData["PageHead"] = "Information Risk Assessment Updation Form";

                List<SYKI> iList = new List<SYKI>();
                List<RiskCategoryVM> categoryList = new List<RiskCategoryVM>();
                List<RiskFrequencyVM> freqList = new List<RiskFrequencyVM>();
                List<RiskFrequencyImapectMapVM> freqImpectList = new List<RiskFrequencyImapectMapVM>();
                List<RiskTreatmentLevelVM> riskTRTList = new List<RiskTreatmentLevelVM>();
                List<RiskImpactVM> riskImpactList = new List<RiskImpactVM>();
                List<RiskStatementVM> statementList = new List<RiskStatementVM>();
                List<Risk_Current_Meaasure_LevelVM> riskMeasureList = new List<Risk_Current_Meaasure_LevelVM>();
                List<PotentialRiskVM> potentialList = new List<PotentialRiskVM>();
                List<RiskLeadVM> leadList = new List<RiskLeadVM>();
                List<Degree_LevelVM> DegLVLList = new List<Degree_LevelVM>();
                List<PrimaryAssestsVM> primaryAssetList = new List<PrimaryAssestsVM>();
                List<ImpactTypeVM> impactTypeList = new List<ImpactTypeVM>();
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var result = _objRiskAssessmentService.Get_Division_For_ISSC_Member(userId);

                ViewBag.OHeadName = result.OperatingHead_Name;
                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                ViewBag.CurrKey = current_ki;
                ViewBag.ChkRiskData = _objRiskAssessmentService.CheckLast_year_RiskData(userId, (long)current_ki);
                ViewBag.OldISSC_MemberCode = _objRiskAssessmentService.GetOldISSC_MemberByCurrentISSCMemCode((long)current_ki, userId);

                int nominationcheck = _objRiskAssessmentService.Check_Issc_Member_Nomination((long)current_ki, userId);

                if (nominationcheck == 0)
                    TempData["msg"] = "Only Information Security Steering Committee Member is Authorized to update Risk Assessment.";


                if (nominationcheck == 0)
                    return RedirectToAction("RiskDashboard");

                primaryAssetList = _objRiskAssessmentService.GetPrimaryAssetByUserId(userId, (int)current_ki);
                impactTypeList = _objRiskAssessmentService.GetAllImpactType();
                ViewBag.impactTypeList = new SelectList(impactTypeList, "ImpactTypeId", "ImpactType");
                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
                //ViewBag.PrimaryId = new SelectList(pasd1, "ID", "PrimaryAsset");
                ViewBag.primaryAssetList = new SelectList(primaryAssetList, "AssestId", "Assets");
                categoryList = _objRiskAssessmentService.GetAllRiskCategory();
                freqList = _objRiskAssessmentService.GetAllFrequency();
                //riskImpactList = _objRiskAssessmentService.GetAllImpact();
                ViewBag.riskImpactList = new SelectList(riskImpactList, "RiskLevelid", "DegreeOfImpact");
                ViewBag.categoryList = new SelectList(categoryList, "RiskId", "Category");
                ViewBag.freqList = new SelectList(freqList, "FrequencyLevelId", "DegreeOfFrequency");
                ViewBag.freqImpectList = new SelectList(freqImpectList, "Id", "Result");
                ViewBag.riskTRTList = new SelectList(riskTRTList, "TreatmentId", "Treatment");
                ViewBag.statementList = new SelectList(statementList, "StatementId", "RiskStatement");
                riskMeasureList = _objRiskAssessmentService.GetAllCurrentMeasureLevel();
                ViewBag.riskMeasureList = new SelectList(riskMeasureList, "CurrentMeasureLevelId", "RiskLevel");
                ViewBag.potentialList = new SelectList(potentialList, "PotentialId", "PotentialRisk");
                leadList = _objRiskAssessmentService.GetRiskLeadByPotential();
                ViewBag.leadList = new SelectList(leadList, "RiskLeadId", "RiskLead");
                ViewBag.DegLVLList = new SelectList(DegLVLList, "DigreeId", "Digree");


                RiskAssessmentVM PeriodSetting = new RiskAssessmentVM();
                PeriodSetting.SkyId = iList.Select(x => x.SYKIID).FirstOrDefault();

                ViewBag.RiskAssessment_details_List = _objRiskAssessmentService.Get_Risk_Assessment(userId, (long)current_ki).ToList();
                ViewBag.EnddateCheck = _objRiskAssessmentService.ISSC_Member_RiskDetails_EndDate_Check(userId, (long)current_ki);
                ViewBag.Final_Submit_Check = _objRiskAssessmentService.ISSC_Member_AssetDetails_FinalSubmit_Check(userId, (long)current_ki);
                var lastki = current_ki - 1;
                ViewBag.Remarks = _objRiskAssessmentService.GetRemrksForISSCMember((long)current_ki, userId);
                ViewBag.DRemarks = _objRiskAssessmentService.GetDeficencyRemrks((long)current_ki, userId);

                ViewBag.OrgnizationMapping_check = _objRiskAssessmentService.Check_OrganizationMapping((long)current_ki, userId);

                //if (Id != null)
                //{
                //    PeriodSetting  = _objRiskAssessmentService.Get_Edit_Risk_Details(Id);
                //    return View(PeriodSetting);
                //}
                return View(PeriodSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in RiskRegister. Message: " + ex.Message.ToString());
                return View();
            }
        }

        [HttpGet]
        public ActionResult EditRiskRegister(int Id)
        {
            try
            {
                TempData["PageHead"] = "Information Risk Assessment Updation Form";

                List<SYKI> iList = new List<SYKI>();
                List<RiskCategoryVM> categoryList = new List<RiskCategoryVM>();
                List<RiskFrequencyVM> freqList = new List<RiskFrequencyVM>();
                List<RiskFrequencyImapectMapVM> freqImpectList = new List<RiskFrequencyImapectMapVM>();
                List<RiskTreatmentLevelVM> riskTRTList = new List<RiskTreatmentLevelVM>();
                List<RiskImpactVM> riskImpactList = new List<RiskImpactVM>();
                List<RiskStatementVM> statementList = new List<RiskStatementVM>();
                List<Risk_Current_Meaasure_LevelVM> riskMeasureList = new List<Risk_Current_Meaasure_LevelVM>();
                List<PotentialRiskVM> potentialList = new List<PotentialRiskVM>();
                List<RiskLeadVM> leadList = new List<RiskLeadVM>();
                List<Degree_LevelVM> DegLVLList = new List<Degree_LevelVM>();
                List<Degree_LevelVM> DegLVLList1 = new List<Degree_LevelVM>();
                List<Degree_LevelVM> DegLVLList2 = new List<Degree_LevelVM>();
                List<PrimaryAssestsVM> primaryAssetList = new List<PrimaryAssestsVM>();
                List<ImpactTypeVM> impactTypeList = new List<ImpactTypeVM>();
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var result = _objRiskAssessmentService.Get_Division_For_ISSC_Member(userId);

                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                primaryAssetList = _objRiskAssessmentService.GetPrimaryAssetByUserId(userId, (int)current_ki);
                RiskAssessmentVM PeriodSetting = new RiskAssessmentVM();
                PeriodSetting.SkyId = iList.Select(x => x.SYKIID).FirstOrDefault();
                PeriodSetting = _objRiskAssessmentService.Get_Edit_Risk_Details(Id);
                impactTypeList = _objRiskAssessmentService.GetAllImpactType();
                ViewBag.impactTypeList = new SelectList(impactTypeList, "ImpactTypeId", "ImpactType");
                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
                ViewBag.primaryAssetList = new SelectList(primaryAssetList, "AssestId", "Assets");
                categoryList = _objRiskAssessmentService.GetAllRiskCategory();
                freqList = _objRiskAssessmentService.GetAllFrequency();
                riskImpactList = _objRiskAssessmentService.GetImpactByImpactTypeId(PeriodSetting.RiskImactTypeId);
                ViewBag.riskImpactList = new SelectList(riskImpactList, "RiskLevelid", "DegreeOfImpact");
                ViewBag.categoryList = new SelectList(categoryList, "RiskId", "Category");
                ViewBag.freqList = new SelectList(freqList, "FrequencyLevelId", "DegreeOfFrequency");
                ViewBag.freqImpectList = new SelectList(freqImpectList, "Id", "Result");
                riskTRTList = _objRiskAssessmentService.GetRiskTreatmentBy(PeriodSetting.Current_Degree_Risk_Lebel, PeriodSetting.Frequency, PeriodSetting.Imapct);
                ViewBag.riskTRTList = new SelectList(riskTRTList, "TreatmentId", "Treatment");
                ViewBag.statementList = new SelectList(statementList, "StatementId", "RiskStatement");
                riskMeasureList = _objRiskAssessmentService.GetAllCurrentMeasureLevel();
                ViewBag.riskMeasureList = new SelectList(riskMeasureList, "CurrentMeasureLevelId", "RiskLevel");
                potentialList = _objRiskAssessmentService.GetPotentialRiskByRiskCategoryId(PeriodSetting.Risk_CategoryId);
                ViewBag.potentialList = new SelectList(potentialList, "PotentialId", "PotentialRisk");
                leadList = _objRiskAssessmentService.GetRiskLeadByPotential();
                ViewBag.leadList = new SelectList(leadList, "RiskLeadId", "RiskLead");
                DegLVLList = _objRiskAssessmentService.GetAllDegreeLevelByFrequencyIdAndImpactId(PeriodSetting.Ident_Frequency, PeriodSetting.Ident_Imapct);
                DegLVLList1 = _objRiskAssessmentService.GetAllDegreeLevelByFrequencyIdAndImpactId(PeriodSetting.Frequency, PeriodSetting.Imapct);
                DegLVLList2 = _objRiskAssessmentService.GetAllDegreeLevelByFrequencyIdAndImpactId(PeriodSetting.Frequency_RRA, PeriodSetting.Imapct_RRA);
                ViewBag.DegLVLList = new SelectList(DegLVLList, "DegreeId", "Degree");
                ViewBag.DegLVLList1 = new SelectList(DegLVLList1, "DegreeId", "Degree");
                ViewBag.DegLVLList2 = new SelectList(DegLVLList2, "DegreeId", "Degree");
                return View(PeriodSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in EditRiskRegister. Message: " + ex.Message.ToString());
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditRiskRegister(RiskAssessmentVM riskAssessmentVM)
        {
            List<SYKI> iList = new List<SYKI>();
            List<RiskCategoryVM> categoryList = new List<RiskCategoryVM>();
            List<RiskFrequencyVM> freqList = new List<RiskFrequencyVM>();
            List<RiskFrequencyImapectMapVM> freqImpectList = new List<RiskFrequencyImapectMapVM>();
            List<RiskTreatmentLevelVM> riskTRTList = new List<RiskTreatmentLevelVM>();
            List<RiskImpactVM> riskImpactList = new List<RiskImpactVM>();
            List<RiskStatementVM> statementList = new List<RiskStatementVM>();
            List<Risk_Current_Meaasure_LevelVM> riskMeasureList = new List<Risk_Current_Meaasure_LevelVM>();
            List<PotentialRiskVM> potentialList = new List<PotentialRiskVM>();
            List<RiskLeadVM> leadList = new List<RiskLeadVM>();
            List<Degree_LevelVM> DegLVLList = new List<Degree_LevelVM>();
            List<PrimaryAssestsVM> primaryAssetList = new List<PrimaryAssestsVM>();
            List<ImpactTypeVM> impactTypeList = new List<ImpactTypeVM>();

            try
            {
                TempData["PageHead"] = "Information Risk Assessment Updation Form";

                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                riskAssessmentVM.SkyId = current_ki;
                riskAssessmentVM.CreatedBy = userId;
                var result = _objRiskAssessmentService.Get_Division_For_ISSC_Member(userId);
                riskAssessmentVM.Risk_Owner = result.OperatingHead_Name;
                var response = _objRiskAssessmentService.InsertORUpdateRiskRegister(riskAssessmentVM);
                TempData["msg"] = response;
                return RedirectToAction("RiskRegister");

            }
            catch (Exception ex)
            {
                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                primaryAssetList = _objRiskAssessmentService.GetPrimaryAssetByUserId(userId, (int)current_ki);
                impactTypeList = _objRiskAssessmentService.GetAllImpactType();
                ViewBag.impactTypeList = new SelectList(impactTypeList, "ImpactTypeId", "ImpactType");
                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
                ViewBag.primaryAssetList = new SelectList(primaryAssetList, "AssestId", "Assets");
                categoryList = _objRiskAssessmentService.GetAllRiskCategory();
                //freqList = _objRiskAssessmentService.GetAllFrequency();
                riskImpactList = _objRiskAssessmentService.GetAllImpact();
                ViewBag.riskImpactList = new SelectList(riskImpactList, "RiskLevelid", "DegreeOfImpact");
                ViewBag.categoryList = new SelectList(categoryList, "RiskId", "Category");
                ViewBag.freqList = new SelectList(freqList, "FrequencyLevelId", "DegreeOfFrequency");
                ViewBag.freqImpectList = new SelectList(freqImpectList, "Id", "Result");
                ViewBag.riskTRTList = new SelectList(riskTRTList, "TreatmentId", "Treatment");
                ViewBag.statementList = new SelectList(statementList, "StatementId", "RiskStatement");
                riskMeasureList = _objRiskAssessmentService.GetAllCurrentMeasureLevel();
                ViewBag.riskMeasureList = new SelectList(riskMeasureList, "CurrentMeasureLevelId", "RiskLevel");
                ViewBag.potentialList = new SelectList(potentialList, "PotentialId", "PotentialRisk");
                leadList = _objRiskAssessmentService.GetRiskLeadByPotential();
                ViewBag.leadList = new SelectList(leadList, "RiskLeadId", "RiskLead");
                ViewBag.DegLVLList = new SelectList(DegLVLList, "DigreeId", "Digree");
                return RedirectToAction("EditRiskRegister");
            }
            return View("RiskRegister");
        }

        [HttpGet]
        public ActionResult DeleteRiskRegister(int Id)
        {
            int UID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objRiskAssessmentService.Delete_RiskDetails(Id, UID);
            TempData["msg"] = result;
            return RedirectToAction("RiskRegister");
        }

        [HttpGet]
        public ActionResult RiskCategory()
        {
            ViewBag.RiskCategories = _objRiskAssessmentService.GetAllRiskCategories();
            TempData["PageHead"] = "Risk Category";
            RiskCategoryCreVM riskCategoryCreVM = new RiskCategoryCreVM();
            return View(riskCategoryCreVM);
        }

        [HttpGet]
        public ActionResult PotentialRisk()
        {
            List<RiskCategoryVM> RiskCategories = new List<RiskCategoryVM>();
            RiskCategories = _objRiskAssessmentService.GetAllRiskCategory();
            ViewBag.RiskCategories = new SelectList(RiskCategories, "RiskId", "Category");
            ViewBag.potentialRisk = _objRiskAssessmentService.GetAllPotentialRisk();
            TempData["PageHead"] = "Potential Risk";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PotentialRisk(RiskStatementCreVM RiskStatementCreVM)
        {
            if (ModelState.IsValid)
            {
                var result = _objRiskAssessmentService.InsertORUpdatePotentialRisk(RiskStatementCreVM);
                if (result == 0)
                {
                    TempData["msg"] = "Potential risk updated successfully.";
                }
                else
                {
                    TempData["msg"] = "Potential risk saved successfully.";
                }
                List<RiskCategoryVM> RiskCategories = new List<RiskCategoryVM>();
                RiskCategories = _objRiskAssessmentService.GetAllRiskCategory();
                ViewBag.RiskCategories = new SelectList(RiskCategories, "RiskId", "Category");
            }
            return RedirectToAction("PotentialRisk");

        }

        [HttpGet]
        public ActionResult EditPotentialRisk(long id)
        {
            var result = _objRiskAssessmentService.EditPotentialRisk(id);
            List<RiskCategoryVM> RiskCategories = new List<RiskCategoryVM>();
            RiskCategories = _objRiskAssessmentService.GetAllRiskCategory();
            ViewBag.RiskCategories = new SelectList(RiskCategories, "RiskId", "Category");
            TempData["PageHead"] = "Potential Risk";
            return View(result);
        }

        public ActionResult DeletePotentialRisk(long id)
        {
            var result = _objRiskAssessmentService.DeletePotentialRisk(id);
            TempData["msg"] = result;
            return RedirectToAction("PotentialRisk");
        }

        [HttpGet]
        public ActionResult EditRiskCategory(long id)
        {
            var result = _objRiskAssessmentService.GetRiskCategoryById(id);
            TempData["PageHead"] = "Risk Category";
            return View(result);
        }

        public ActionResult DeleteRiskCategory(long id)
        {
            var result = _objRiskAssessmentService.DeleteRiskCategory(id);
            TempData["msg"] = result;
            return RedirectToAction("RiskCategory");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult RiskCategory(RiskCategoryModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Save logic
        //        return RedirectToAction("Index");
        //    }
        //    return View(model);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RiskCategory(RiskCategoryCreVM RiskCategoryCreVM)
        {
            if (ModelState.IsValid)
            {
                var result = _objRiskAssessmentService.InsertORUpdateRiskCategory(RiskCategoryCreVM);
                if (result == 0)
                {
                    TempData["msg"] = "Risk category updated successfully.";
                }
                else
                {
                    TempData["msg"] = "Risk category saved successfully.";
                }
                ViewBag.RiskCategories = _objRiskAssessmentService.GetAllRiskCategories();
                RiskCategoryCreVM riskCategoryCreVM = new RiskCategoryCreVM();
            }
            return RedirectToAction("RiskCategory");
        }

        [HttpPost]
        public JsonResult SaveAsDraftkRegister(string skyid)
        {
            int UID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objRiskAssessmentService.SaveAsDraftkRegister(Convert.ToInt32(skyid), UID);
            //TempData["msg"] = result;
            return Json(result);
        }

        [HttpPost]
        public ActionResult RiskRegister(RiskAssessmentVM riskAssessmentVM)
        {
            List<SYKI> iList = new List<SYKI>();
            List<RiskCategoryVM> categoryList = new List<RiskCategoryVM>();
            List<RiskFrequencyVM> freqList = new List<RiskFrequencyVM>();
            List<RiskFrequencyImapectMapVM> freqImpectList = new List<RiskFrequencyImapectMapVM>();
            List<RiskTreatmentLevelVM> riskTRTList = new List<RiskTreatmentLevelVM>();
            List<RiskImpactVM> riskImpactList = new List<RiskImpactVM>();
            List<RiskStatementVM> statementList = new List<RiskStatementVM>();
            List<Risk_Current_Meaasure_LevelVM> riskMeasureList = new List<Risk_Current_Meaasure_LevelVM>();
            List<PotentialRiskVM> potentialList = new List<PotentialRiskVM>();
            List<RiskLeadVM> leadList = new List<RiskLeadVM>();
            List<Degree_LevelVM> DegLVLList = new List<Degree_LevelVM>();
            List<PrimaryAssestsVM> primaryAssetList = new List<PrimaryAssestsVM>();
            List<ImpactTypeVM> impactTypeList = new List<ImpactTypeVM>();

            try
            {
                TempData["PageHead"] = "Information Risk Assessment Updation Form";

                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                riskAssessmentVM.SkyId = current_ki;
                riskAssessmentVM.CreatedBy = userId;
                var result = _objRiskAssessmentService.Get_Division_For_ISSC_Member(userId);
                riskAssessmentVM.Risk_Owner = result.OperatingHead_Name;
                var response = _objRiskAssessmentService.InsertORUpdateRiskRegister(riskAssessmentVM);
                TempData["msg"] = response;
                return Redirect("../RiskAssessment/RiskRegister");

            }
            catch (Exception ex)
            {
                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                primaryAssetList = _objRiskAssessmentService.GetPrimaryAssetByUserId(userId, (int)current_ki);
                impactTypeList = _objRiskAssessmentService.GetAllImpactType();
                ViewBag.impactTypeList = new SelectList(impactTypeList, "ImpactTypeId", "ImpactType");
                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
                ViewBag.primaryAssetList = new SelectList(primaryAssetList, "AssestId", "Assets");
                categoryList = _objRiskAssessmentService.GetAllRiskCategory();
                //freqList = _objRiskAssessmentService.GetAllFrequency();
                riskImpactList = _objRiskAssessmentService.GetAllImpact();
                ViewBag.riskImpactList = new SelectList(riskImpactList, "RiskLevelid", "DegreeOfImpact");
                ViewBag.categoryList = new SelectList(categoryList, "RiskId", "Category");
                ViewBag.freqList = new SelectList(freqList, "FrequencyLevelId", "DegreeOfFrequency");
                ViewBag.freqImpectList = new SelectList(freqImpectList, "Id", "Result");
                ViewBag.riskTRTList = new SelectList(riskTRTList, "TreatmentId", "Treatment");
                ViewBag.statementList = new SelectList(statementList, "StatementId", "RiskStatement");
                riskMeasureList = _objRiskAssessmentService.GetAllCurrentMeasureLevel();
                ViewBag.riskMeasureList = new SelectList(riskMeasureList, "CurrentMeasureLevelId", "RiskLevel");
                ViewBag.potentialList = new SelectList(potentialList, "PotentialId", "PotentialRisk");
                leadList = _objRiskAssessmentService.GetRiskLeadByPotential();
                ViewBag.leadList = new SelectList(leadList, "RiskLeadId", "RiskLead");
                ViewBag.DegLVLList = new SelectList(DegLVLList, "DigreeId", "Digree");

            }
            return View("RiskRegister");
        }


        public IActionResult GetPotentialRiskByRiskCategoryId(int categoryId)
        {
            var result = _objRiskAssessmentService.GetPotentialRiskByRiskCategoryId(categoryId);
            return Json(result);
        }

        public IActionResult GetImpactByImpactTypeId(int impTypeId)
        {
            var result = _objRiskAssessmentService.GetImpactByImpactTypeId(impTypeId);
            return Json(result);
        }

        public IActionResult GetImpactByImpactId(int impID)
        {
            var result = _objRiskAssessmentService.GetImpactByImpactId(impID);
            return Json(result);
        }

        public IActionResult GetFrequencyByFrequencyId(int freqId)
        {
            var result = _objRiskAssessmentService.GetFrequencyByFrequencyId(freqId);
            return Json(result);
        }

        //public ActionResult GetAllTRTLevelByPotentialId(int potentialId)
        //{
        //    var result = _objRiskAssessmentService.GetRiskTreatmentBy(potentialId);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetRiskLeadByPotentialRiskId(int potentialid)
        //{
        //    var result = _objRiskAssessmentService.GetRiskLeadByPotentialRiskId(potentialid);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public IActionResult GetAllDegreeLevelByFrequencyIdAndImpactId(int categoryId, int impactId)
        {
            var result = _objRiskAssessmentService.GetAllDegreeLevelByFrequencyIdAndImpactId(categoryId, impactId);
            return Json(result);
        }

        public IActionResult GetRiskTreatmentBy(int levelId, int freqId, int impactId)
        {
            var result = _objRiskAssessmentService.GetRiskTreatmentBy(levelId, freqId, impactId);
            return Json(result);
        }



        public ActionResult LastYear_Ki_AssetDetails()
        {
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            var result = _objRiskAssessmentService.Last_year_Asset_Detail_Only_Non_CommonAsset(userId, (long)current_ki);
            TempData["ResultMsg"] = result;
            return RedirectToAction("RiskRegister");
        }

        [HttpGet]
        public ActionResult RiskDashboard()
        {
            TempData["PageHead"] = "Manage Information Risk Assessment";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objRiskAssessmentService.Get_Division_For_ISSC_Member(userId);
            ViewBag.Division = result.DivisionName;
            ViewBag.ISSC = result.ISSCMember;
            ViewBag.ISSC_MemberCode = userId;

            ViewBag.ISSCStatus = result.ISSC_Member_Submit_Status;
            ViewBag.ISSCFinalDate = result.ISSC_Member_Submit_Date;

            ViewBag.Divisionstatus = result.DivisionHead_Submit_Status;
            ViewBag.DivisionFinalDate = result.DivisionHead_Submit_Date;

            ViewBag.OperatingStatus = result.OperatingHead_Submit_Status;
            ViewBag.OperatingFinalDate = result.OperatingHead__Submit_Date;
            ViewBag.DHeadName = result.DivisionHead_Name;
            ViewBag.OHeadName = result.OperatingHead_Name;

            DateTime? dt = result.EndDate;
            ViewBag.endate = String.Format("{0:dd/MM/yyyy}", dt);

            DateTime end_dates = Convert.ToDateTime(result.EndDate);
            int nodaysleft = end_dates.Subtract(DateTime.Now.Date).Days;
            if (result.ISSCMember != null && result.OperatingHead_Submit_Status == null)
                ViewBag.nodaysleft = nodaysleft + " Days";
            else
                ViewBag.nodaysleft = "0 Days";

            List<SYKI> iList1 = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objRiskAssessmentService.GetAssetRegistrationSYKIListForDashboard();
            foreach (var item in kiData._SYKIList)
            {
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            ViewBag.Lastki_details = iList1;
            return View();
        }

        [HttpGet]
        public ActionResult RiskRemarkByDivisionHead(int ISSC_Code)
        {
            TempData["PageHead"] = "Information Security Risk updation";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            ViewBag.ISSCMEM = ISSC_Code;
            ViewBag.EnddateCheck = _objRiskAssessmentService.ISSC_Member_RiskDetails_EndDate_Check(ISSC_Code, (long)current_ki);
            ViewBag.Final_Submit_Check = _objRiskAssessmentService.Operating_Head_AssetDetails_FinalSubmit_Check(userId, (long)current_ki);

            var result = _objRiskAssessmentService.Get_ISSC_Member_Risk_For_DivisionHead(ISSC_Code, (long)current_ki);
            ViewBag.Riskdetails = result;
            ViewBag.Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            ViewBag.High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            ViewBag.Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            ViewBag.Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            ViewBag.VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            CommonRiskAssessmentVM asset_deatils = new CommonRiskAssessmentVM();
            asset_deatils.CreatedBy = ISSC_Code;
            asset_deatils.SKYID = (long)current_ki;
            ViewBag.Remarks = _objRiskAssessmentService.GetRemrks((long)current_ki, ISSC_Code);
            return View(asset_deatils);
        }



        public ActionResult OldRiskRegisterData()
        {
            TempData["PageHead"] = "Information Risk Register updation Form";

            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            long OldISSCMemID = _objRiskAssessmentService.GetOldISSC_MemberByCurrentISSCMemCode((long)current_ki, userId);
            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

            List<Asset_Year> year = new List<Asset_Year>();


            CommonRiskAssessmentVM PeriodSetting = new CommonRiskAssessmentVM();
            ViewBag.EnddateCheck = _objRiskAssessmentService.ISSC_Member_RiskDetails_EndDate_Check((int)OldISSCMemID, (long)current_ki);
            ViewBag.Final_Submit_Check = _objRiskAssessmentService.ISSC_Member_AssetDetails_FinalSubmit_Check((int)OldISSCMemID, (long)current_ki);
            var lastki = current_ki - 1;
            ViewBag.Last_Ki_Asset_details_List = _objRiskAssessmentService.Get_Last_Ki_Asset_Details(OldISSCMemID, (long)lastki);
            ViewBag.Remarks = _objRiskAssessmentService.GetRemrksForISSCMember((long)current_ki, (int)OldISSCMemID);
            ViewBag.DRemarks = _objRiskAssessmentService.GetDeficencyRemrks((long)current_ki, (int)OldISSCMemID);
            ViewBag.OrgnizationMapping_check = _objRiskAssessmentService.Check_OrganizationMapping((long)current_ki, (int)OldISSCMemID);
            return View(PeriodSetting);
        }

        [HttpGet]
        public ActionResult RiskSameDivisionAndOperationApproval()
        {
            TempData["PageHead"] = "Division Head and Operating Head Name Updation";
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

            List<ADORGLEVEL> _opList = _objRiskAssessmentService.GetOrgLevelList((long)1);
            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.CurrentSYKIData = iList.Select(x => x.KICODE).FirstOrDefault();
            return View();
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult RiskCategory(RiskCategoryCreVM RiskCategoryCreVM)
        //{
        //    if (ModelState.IsValid)
        //    {

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RiskSameDivisionAndOperationApproval(SameDivisionAndOprationApprovalVM model)
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            model.CreatedBy = userid;
            var msg = _objRiskAssessmentService.RiskOldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(model);
            TempData["msg"] = msg;
            return RedirectToAction("RiskSameDivisionAndOperationApproval");
        }

        public IActionResult BindDivisionByOperationId(long id)
        {
            List<SearchParameterList> divList = _objRiskAssessmentService.BindDivision(id);
            return Json(divList);
        }
        public IActionResult GetOperationIdsByLoginUserId(long id)
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            List<SearchParameterList> divList = _objRiskAssessmentService.BindDivision(id);
            var divisions = _objRiskAssessmentService.GetDivisionsIdsByLoginUserId(userid);
            divList = divList.Where(x => divisions.Contains(x.DIVISIONID)).ToList();
            return Json(divList);
        }
        public ActionResult BindDivisionWithOldOperationHeadEmpCodeByOperationId(long id, long? sYKIID)
        {
            AssetRegisterOperationHeadData assetRegisterOperationHeadData = new AssetRegisterOperationHeadData();
            assetRegisterOperationHeadData.divList = _objRiskAssessmentService.BindDivision(id);
            assetRegisterOperationHeadData.OperatingHeadEmpCode = _objRiskAssessmentService.BindDivisionWithOldOperationHeadEmpCodeByOperationId(id, sYKIID);
            return Json(assetRegisterOperationHeadData);
        }

        public ActionResult BindOldDivisionHeadEmpCodeByDivisionId(long id, long? sYKIID)
        {
            AssetRegisterOperationHeadData assetRegisterOperationHeadData = new AssetRegisterOperationHeadData();
            assetRegisterOperationHeadData.DivisionHeadEmpCode = _objRiskAssessmentService.BindOldDivisionHeadEmpCodeByDivisionId(id, sYKIID);
            return Json(assetRegisterOperationHeadData);
        }

        [HttpPost]
        public ActionResult BulkUpdateAssetApplicability(AssetApplicabilityUpdateVM model, string SelectedIds)
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            CultureInfo culture = new CultureInfo("en-GB");
            model.CreatedBy = userid;
            model.SelectedIds = SelectedIds.Split(',').Select(x => Convert.ToDecimal(x)).ToList();
            var result = _objRiskAssessmentService.BulkUpdateAssetApplicability(model);
            TempData["msg"] = result == 1 ? "Data has been updated succesufully." : "There is some problem, please try again later.";
            return RedirectToAction("AssetRegister");
        }

        public ActionResult FinalSubmit_RiskDetails()
        {
            try
            {
                CommonRiskAssessmentVM risk = new CommonRiskAssessmentVM();
                List<CommonRiskAssessmentVM> get_common_asset_details = new List<CommonRiskAssessmentVM>();
                List<SYKI> iList = new List<SYKI>();
                // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                int UID = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var divison_head = _objRiskAssessmentService.Get_DivisionHead_((long)current_ki, UID);
                List<int> lst = new List<int>();
                lst = _objRiskAssessmentService.GetPrimaryAssetByUserId(UID, (int)current_ki).Select(x => (int)x.AssestId).ToList();
                var value = _objRiskAssessmentService.CheckAllPrimaryRiskCreatedOrNotByUserId(lst, (int)current_ki, UID);
                if (value == "0")
                {
                    risk.SKYID = (long)current_ki;
                    risk.CreatedBy = UID;
                    risk.Division_Head_EmpCode = divison_head.DivisionEmpCode;
                    var result = _objRiskAssessmentService.FinalSubmit_RiskDetails(risk);
                    TempData["msg"] = result;

                    // Send EMail to Division Head            
                    SendMailTo_DivisionHead_After_Issc_Member(divison_head.Empname, divison_head.Emailid, kiData._SYKIList.Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName);
                    return RedirectToAction("RiskDashboard");
                }
                else
                {
                    TempData["msg"] = value;
                    return RedirectToAction("RiskRegister");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in FinalSubmit_RiskDetails. Message: " + ex.Message.ToString());
                return RedirectToAction("RiskRegister");
            }

        }

        [HttpGet]
        public ActionResult RiskDashboardForDivisionHead()
        {
            TempData["PageHead"] = "Information Risk Assessment Approval";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            List<Get_Division_ISSC_Member> lstDivision_ISSC_Member = new List<Get_Division_ISSC_Member>();
            List<Asset_Dvision_Deatils_VM> lstDivision_Details = new List<Asset_Dvision_Deatils_VM>();
            var oldUserId = _objRiskAssessmentService.GetAssetUserDeailsForDivisionHeadByLoginUserID(userId, (long)current_ki);

            if (oldUserId != null)
            {
                lstDivision_ISSC_Member = _objRiskAssessmentService.Get_ISSC_Members_Detail_For_DivisionHead(oldUserId);
                var lstDivision_ISSC_Member_1 = _objRiskAssessmentService.Get_ISSC_Members_Detail_For_DivisionHead(userId);
                if (lstDivision_ISSC_Member_1.Any())
                {
                    if (lstDivision_ISSC_Member.Any())
                        lstDivision_ISSC_Member.AddRange(lstDivision_ISSC_Member_1);
                    else
                    {
                        lstDivision_ISSC_Member = lstDivision_ISSC_Member_1;
                    }
                }
                lstDivision_Details = _objRiskAssessmentService.Get_ISSC_Members_Division_Details_For_DivisionHead(oldUserId);
                var lstDivision_Details_1 = _objRiskAssessmentService.Get_ISSC_Members_Division_Details_For_DivisionHead(userId);
                if (lstDivision_Details_1.Any())
                {
                    if (lstDivision_Details.Any())
                        lstDivision_Details.AddRange(lstDivision_Details_1);
                    else
                    {
                        lstDivision_Details = lstDivision_Details_1;
                    }
                }

                ViewBag.AssetsDetailList = lstDivision_ISSC_Member;
                ViewBag.DivisionList = lstDivision_Details;
            }
            else
            {
                ViewBag.AssetsDetailList = _objRiskAssessmentService.Get_ISSC_Members_Detail_For_DivisionHead(userId);
                ViewBag.DivisionList = _objRiskAssessmentService.Get_ISSC_Members_Division_Details_For_DivisionHead(userId);
            }

            return View();
        }

        [HttpGet]
        public ActionResult RiskDetailsApproveByDivisionHead(int ISSC_Code)
        {
            TempData["PageHead"] = "Information Rsik Assessment Approval";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            ViewBag.EnddateCheck = _objRiskAssessmentService.ISSC_Member_RiskDetails_EndDate_Check(ISSC_Code, (long)current_ki);
            ViewBag.Final_Submit_Check = _objRiskAssessmentService.Division_Head_AssetDetails_FinalSubmit_Check(userId, (long)current_ki, ISSC_Code);

            var result = _objRiskAssessmentService.Get_ISSC_Member_Risk_For_DivisionHead(ISSC_Code, (long)current_ki);
            ViewBag.Riskdetails = result;
            ViewBag.Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            ViewBag.High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            ViewBag.Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            ViewBag.Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            ViewBag.VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            CommonRiskAssessmentVM risk_deatils = new CommonRiskAssessmentVM();
            risk_deatils.ISSC_MEM_EmpCode = ISSC_Code;
            risk_deatils.SKYID = (long)current_ki;
            return View(risk_deatils);
        }

        [HttpPost]
        public IActionResult RiskDetailsApproveByDivisionHead(CommonRiskAssessmentVM risk_deatils)
        {
            ////swLog = System.IO.File.AppendText(strLogPath);
            ////swLog = System.IO.File.CreateText(strLogPath);

            //try
            //{
            //    //swLog.WriteLine("First step ");
            //    int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            //    risk_deatils.Division_Head_EmpCode = userId;
            //    List<SYKI> iList = new List<SYKI>();
            //    CultureInfo culture = new CultureInfo("en-GB");
            //    var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            //    foreach (var item in kiData._SYKIList)
            //    {
            //        iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            //    }

            //    var divison_head = _objRiskAssessmentService.Get_DivisionHead_((long)risk_deatils.SKYID, (long)risk_deatils.ISSC_MEM_EmpCode);
            //    //swLog.WriteLine("second step ");
            //    if (Request.Form["btnFinalSubmit"] != null)
            //    {
            //        var ResponseResult = _objRiskAssessmentService.Risk_Approve_By_DivisionHead(risk_deatils);
            //        TempData["ResultMsg"] = ResponseResult;
            //        // Send EMail tp  Operating Head  
            //        var result = _objRiskAssessmentService.Send_Email_To_ISSC_OperatingHead(risk_deatils).Select(x => x).FirstOrDefault();
            //        if (result != null)
            //        {
            //            //swLog.WriteLine("Third step ");

            //            SendMailTo_Operatinghead_After_DivisionHead(result.EmpName, result.EmpEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName);
            //            //swLog.WriteLine("Forth step ");
            //            //Send EMail to  ISSC Member 
            //            SendMailTo_ISSCMember_After_DivisionHeadApprove(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname);
            //            //swLog.WriteLine("Fifth step ");
            //        }
            //        //swLog.WriteLine("Six step ");
            //        return RedirectToAction("RiskDashboardForDivisionHead");
            //    }
            //    else if (Request.Form["btnsendback"] != null)
            //    {
            //        var ResponseResult = _objRiskAssessmentService.Risk_SendBack_By_DivisionHead(risk_deatils);
            //        TempData["ResultMsg"] = ResponseResult;
            //        var s = _objRiskAssessmentService.GetStartDate_EndDate_ForAsset_Register(risk_deatils);
            //        // Send EMail to ISSC Member
            //        var result = _objRiskAssessmentService.Send_Email_To_ISSC_OperatingHead_FroSendBack(risk_deatils);
            //        if (result != null)
            //        {
            //            //swLog.WriteLine("Seven step ");
            //            SendMailTo_ISSCMember_After_DivisionHeadSendBack(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname, Convert.ToDateTime(s.StartDate, culture), Convert.ToDateTime(s.EndDate, culture));
            //        }
            //        //swLog.WriteLine("Eight step ");
            //        return RedirectToAction("RiskDashboardForDivisionHead");

            //    }
            //    //swLog.Close();
            //}
            //catch (Exception ex)
            //{
            //    //TempData["ResultMsg"] = ex.Message;
            //    //string strLogFileName = ConfigurationSettings.AppSettings["LogFileName"];
            //    //string _LogFileName = strLogFileName + "_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            //    //swLog = File.CreateText(strLogPath + _LogFileName);

            //    //swLog.WriteLine("Approve by Div Head : " + ex.InnerException.ToString());
            //    //swLog.Close();
            //    return RedirectToAction("RiskDashboardForDivisionHead");
            //}
            return RedirectToAction("RiskDashboardForDivisionHead");
        }

        [HttpGet]
        public ActionResult RiskAssessmentSubmitStatus()
        {
            SearchParameterList paramsList = new SearchParameterList();
            try
            {
                TempData["PageHead"] = "Risk Assessment Submit Status";
                List<SYKI> iList1 = new List<SYKI>();
                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                var curki = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
                ViewBag.curki = curki;
                ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
                ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(); ;
                ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
                ViewBag.OpList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");

                var ViewToList = _objRiskAssessmentService.GetRiskAssessmentSunmitStatus((long)curki, null, null);

                ViewBag.ViewToList = ViewToList;

            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in RiskAssessmentSubmitStatus. Message: " + ex.Message.ToString());
            }
            return View();

        }

        [HttpPost]
        public IActionResult RiskAssessmentSubmitStatus(long? SYKIDatanew, long? OPERATIONID, long? DIVISIONID)
        {
            TempData["PageHead"] = "Risk Assessment Submit Status";
            SearchParameterList paramsList = new SearchParameterList();
            try
            {
                //if (Request.Form["btnSubmit"] != null)
                if (Request.HasFormContentType && Request.Form.ContainsKey("btnSubmit"))
                {
                    List<SYKI> iList1 = new List<SYKI>();
                    var kiData = _objRiskAssessmentService.GetRiskRegistrationSYKIListForViewTeam();
                    foreach (var item in kiData._SYKIList)
                    {
                        iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                    }
                    ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
                    ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
                    ITDivisionApprovalgetSearchFilterData(new SearchParameterList());

                    List<ADORGLEVEL> _opList = _objRiskAssessmentService.BindOperation(SYKIDatanew);
                    ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                    List<SearchParameterList> divList = _objRiskAssessmentService.BindDivision(OPERATIONID);
                    ViewBag.DivisionList = new SelectList(divList, "DIVISIONID", "DIVISION", DIVISIONID);

                    var ViewToList = _objRiskAssessmentService.GetRiskAssessmentSunmitStatus(SYKIDatanew, OPERATIONID, DIVISIONID);
                    ViewBag.ViewToList = ViewToList;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RiskAssessmentSubmitStatus. Message:" + ex.Message.ToString());
            }

            return View();

        }


        private void SendMailTo_Operatinghead_After_DivisionHead(string Emp, string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName)
        {
            try
            {
                //swLog.WriteLine("To Mail : " + Emp_Emailid);

                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
                sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
                string strSubject = "Risk Assessment Approval Request " + ski;
                string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                     "<tr style='background-color:skyblue;'>Risk Assessment Approval  " + ski + "</tr>" +
                                     "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                     "<tr><td><b>Dear " + Emp + " " + "San," + "</b></td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>This is to inform you that ISO27001 -Risk Assessment has been submitted for " + ski + ".</td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>Division Name - " + DivisionName + "</td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>ISSC Member Name -  " + ISSC_MemberName + " " + "San" + "</td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                     "<tr><td><br><br></td></tr>" +
                                     "<tr><td>Best Regards</td></tr>" +
                                     "<tr><td>Team ISMS</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                     "</table> </td> </tr> </table>";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                //swLog.Close();

                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in SendMailTo_Operatinghead_After_DivisionHead. Message: " + ex.Message.ToString());
                //swLog.WriteLine("Send mail exception : " + ex.InnerException.ToString());
                //swLog.Close();
            }
        }

        private void SendMailTo_ISSCMember_After_DivisionHeadApprove(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Risk Assessment Approval Request " + ski;
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                "<tr style='background-color:skyblue;'>Risk Assessment Approval " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                 "<tr><td><b>Dear " + ISSC_MemberName + " " + "San," + "</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Risk Assessment has been approved by  " + ApproverName + " " + "San" + " for " + ski + ".</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for details.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void SendMailTo_ISSCMember_After_DivisionHeadSendBack(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName, DateTime StateDate, DateTime EndDate)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Risk Assessment send back";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                  "<tr style='background-color:skyblue;'>Risk Assessment send back  " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                 "<tr><td><b>Dear " + ISSC_MemberName + " " + "San," + "</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Risk Assessment has been send back by  " + ApproverName + " " + "San" + " for " + ski + ".</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>Kindly update Risk Assessment as window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                                "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }


        [HttpGet]
        public ActionResult LastYearKiRiskDetails(int SkyiId, int? DivId)
        {
            TempData["PageHead"] = "Last Year Information Risk Assessment";
            long? userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            ViewBag.Riskdetails = _objRiskAssessmentService.Get_Last_Ki_Asset_DetailsNew(userid, SkyiId, DivId);
            return View();
        }

        private void SendMailTo_DivisionHead_After_Issc_Member(string Emp, string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Risk Assessment Approval Request " + ski;
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                  "<tr style='background-color:skyblue;'>Risk Assessment Approval " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                 "<tr><td><b>Dear " + Emp + " " + "San," + "</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Risk Assessment has been submitted for " + ski + ".</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>Division Name - " + DivisionName + "</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>ISSC Member Name -  " + ISSC_MemberName + " " + "San" + "</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void SendMailTo_ISSC_Member_ToFill_Risk_Details(string Emp, string Emp_Emailid, DateTime StateDate, DateTime EndDate)
        {
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            var ski = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid;// user mail id
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Risk Assessment Updation";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                               "<tr style='background-color:skyblue;'>Risk Assessment Updation " + ski + "</tr>" +
                               "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                               "<tr><td><b>Dear " + Emp + " " + "San," + "</b></td></tr>" +
                               "<tr><td><br></td></tr>" +
                               "<tr><td valign=top>This is to inform you that you are eligible for ISO27001 -Risk Assessment Updation for  " + ski + ".</td></tr>" +
                               "<tr><td><br></td></tr>" +
                               "<tr><td valign=top>Risk Assessment updation window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                               "<tr><td><br></td></tr>" +
                               "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> and ensure timely completion of activity.</td></tr>" +
                               "<tr><td><br><br></td></tr>" +
                               "<tr><td>Best Regards</td></tr>" +
                               "<tr><td>Team ISMS</td></tr>" +
                               "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                               "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        public ActionResult RiskReportDashboardForDivisionHead()
        {
            TempData["PageHead"] = "View Dashboard";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            EmpDetails emp = _objRiskAssessmentService.GetEmp_Details_Ki_Wise(userId);
            List<DivisionWise_Risk_Count> discount = new List<DivisionWise_Risk_Count>();

            List<SYKI> iList1 = new List<SYKI>();
            var kiData = _objRiskAssessmentService.GetRiskRegistrationSYKIListForViewTeam();
            foreach (var item in kiData._SYKIList)
            {
                //if (item.ACTIVE == 1)
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
            ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
            decimal current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            List<ADORGLEVEL> opList = _objRiskAssessmentService.BindOperation((long)current_ki);
            var opertaions = _objRiskAssessmentService.GetOperationIdsByLoginUserId(userId);
            opList = opList.Where(x => opertaions.Contains(x.ADORGLEVELID)).ToList();
            ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
            ViewBag.OpList = new SelectList(opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "DIVISIONID", "DIVISION");

            var result = _objRiskAssessmentService.Get_Risk_Details_Report_Operation_Division_Wise(userId);
            ViewBag.Riskdetails = result;
            ViewBag.Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            ViewBag.High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            ViewBag.Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            ViewBag.Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            ViewBag.VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            ViewBag.Assetdetails = result;
            var resultcount = _objRiskAssessmentService.Get_Risk_Report_Count_DivisionWise(userId);
            for (int i = 0; i < resultcount.Count; i++)
            {
                DivisionWise_Risk_Count d = new DivisionWise_Risk_Count();
                d.DivisionName = resultcount[i].DivisionName;
                d.TSCount = resultcount[i].TSCount;
                d.TACount = resultcount[i].TACount;
                d.TBCount = resultcount[i].TBCount;
                d.TCCount = resultcount[i].TCCount;
                d.TDCount = resultcount[i].TDCount;
                discount.Add(d);
            }
            ViewBag.divcount = discount;
            return View();
        }

        public ActionResult RiskReportDashboardForOperatingHead()
        {
            TempData["PageHead"] = "View Dashboard";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            EmpDetails emp = _objRiskAssessmentService.GetEmp_Details_Ki_Wise(userId);
            List<DivisionWise_Risk_Count> discount = new List<DivisionWise_Risk_Count>();

            List<SYKI> iList1 = new List<SYKI>();
            var kiData = _objRiskAssessmentService.GetRiskRegistrationSYKIListForViewTeam();
            foreach (var item in kiData._SYKIList)
            {
                //if (item.ACTIVE == 1)
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
            ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
            decimal current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            List<ADORGLEVEL> opList = _objRiskAssessmentService.BindOperation((long)current_ki);
            var opertaions = _objRiskAssessmentService.GetOperationIdsByLoginUserId(userId);
            opList = opList.Where(x => opertaions.Contains(x.ADORGLEVELID)).ToList();
            ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
            ViewBag.OpList = new SelectList(opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "DIVISIONID", "DIVISION");

            var result = _objRiskAssessmentService.Get_Risk_Details_Report_OperationHead_Division_Wise(userId);
            ViewBag.Riskdetails = result;
            ViewBag.Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            ViewBag.High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            ViewBag.Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            ViewBag.Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            ViewBag.VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            ViewBag.Assetdetails = result;
            var resultcount = _objRiskAssessmentService.Get_Risk_Report_Count_OperationHead_DivisionWise(userId);
            for (int i = 0; i < resultcount.Count; i++)
            {
                DivisionWise_Risk_Count d = new DivisionWise_Risk_Count();
                d.DivisionName = resultcount[i].DivisionName;
                d.TSCount = resultcount[i].TSCount;
                d.TACount = resultcount[i].TACount;
                d.TBCount = resultcount[i].TBCount;
                d.TCCount = resultcount[i].TCCount;
                d.TDCount = resultcount[i].TDCount;
                discount.Add(d);
            }
            ViewBag.divcount = discount;
            return View();
        }
        public ActionResult RiskAdminDashboard()
        {
            TempData["PageHead"] = "View Dashboard";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            EmpDetails emp = _objRiskAssessmentService.GetEmp_Details_Ki_Wise(userId);
            List<DivisionWise_Risk_Count> discount = new List<DivisionWise_Risk_Count>();

            List<SYKI> iList1 = new List<SYKI>();
            var kiData = _objRiskAssessmentService.GetRiskRegistrationSYKIListForViewTeam();
            foreach (var item in kiData._SYKIList)
            {
                //if (item.ACTIVE == 1)
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
            ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
            decimal current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            List<ADORGLEVEL> opList = _objRiskAssessmentService.BindOperation((long)current_ki);
            ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
            ViewBag.OpList = new SelectList(opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "DIVISIONID", "DIVISION");

            var result = _objRiskAssessmentService.Get_Risk_Details_Operation_Division_Wise((long)current_ki, 0, 0);
            ViewBag.Riskdetails = result;
            ViewBag.Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            ViewBag.High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            ViewBag.Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            ViewBag.Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            ViewBag.VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            ViewBag.Assetdetails = result;
            var resultcount = _objRiskAssessmentService.Get_Risk_Count_DivisionWise((long)current_ki, 0, 0);
            for (int i = 0; i < resultcount.Count; i++)
            {
                DivisionWise_Risk_Count d = new DivisionWise_Risk_Count();
                d.DivisionName = resultcount[i].DivisionName;
                d.TSCount = resultcount[i].TSCount;
                d.TACount = resultcount[i].TACount;
                d.TBCount = resultcount[i].TBCount;
                d.TCCount = resultcount[i].TCCount;
                d.TDCount = resultcount[i].TDCount;
                discount.Add(d);
            }
            ViewBag.divcount = discount;
            return View();
        }

        [HttpPost]
        public ActionResult RiskSearch(long SYKIDatanew, decimal? OPERATIONID, decimal? DIVISIONID)
        {
            TempData["PageHead"] = "Risk Assessment Report";
            var result = _objRiskAssessmentService.Get_Risk_Details_Operation_Division_Wise(SYKIDatanew, OPERATIONID == null ? 0 : OPERATIONID, DIVISIONID == null ? 0 : DIVISIONID);
            ViewBag.Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            ViewBag.High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            ViewBag.Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            ViewBag.Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            ViewBag.VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            ViewBag.Riskdetails = result;
            ViewBag.SYKIDatanew = SYKIDatanew;
            ViewBag.OPERATIONID = OPERATIONID;
            ViewBag.DIVISIONID = DIVISIONID;
            return View();
        }

        public ActionResult BindChart1(long? SYKIID)
        {
            List<ADORGLEVEL> divList = _objRiskAssessmentService.BindOperation(SYKIID);
            var result = _objRiskAssessmentService.Get_Risk_Details_Operation_Division_Wise((long)SYKIID, 0, 0);

            int Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            int High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            int Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            int Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            int VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            string s = Extreme + "," + High + "," + Medium + "," + Low + "," + VeryLow;

            return Json(s);
        }

        public ActionResult BindChart2(long? SYKIID)
        {
            var resultcount = _objRiskAssessmentService.Get_Risk_Count_DivisionWise((long)SYKIID, 0, 0);
            List<DivisionWise_Risk_Count> discount = new List<DivisionWise_Risk_Count>();

            for (int i = 0; i < resultcount.Count; i++)
            {
                DivisionWise_Risk_Count d = new DivisionWise_Risk_Count();
                d.DivisionName = resultcount[i].DivisionName;
                d.TSCount = resultcount[i].TSCount;
                d.TACount = resultcount[i].TACount;
                d.TBCount = resultcount[i].TBCount;
                d.TCCount = resultcount[i].TCCount;
                d.TDCount = resultcount[i].TDCount;
                discount.Add(d);
            }
            return Json(discount);
        }

        public ActionResult BindDivisionHeadChart1(long? SYKIID)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            List<ADORGLEVEL> divList = _objRiskAssessmentService.BindOperation(SYKIID);
            var result = _objRiskAssessmentService.Get_Risk_Details_Report_Operation_Division_Wise(userId);

            int Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            int High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            int Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            int Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            int VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            string s = Extreme + "," + High + "," + Medium + "," + Low + "," + VeryLow;

            return Json(s);
        }

        public ActionResult BindDivisionHeadChart2()
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var resultcount = _objRiskAssessmentService.Get_Risk_Report_Count_DivisionWise(userId);
            List<DivisionWise_Risk_Count> discount = new List<DivisionWise_Risk_Count>();

            for (int i = 0; i < resultcount.Count; i++)
            {
                DivisionWise_Risk_Count d = new DivisionWise_Risk_Count();
                d.DivisionName = resultcount[i].DivisionName;
                d.TSCount = resultcount[i].TSCount;
                d.TACount = resultcount[i].TACount;
                d.TBCount = resultcount[i].TBCount;
                d.TCCount = resultcount[i].TCCount;
                d.TDCount = resultcount[i].TDCount;
                discount.Add(d);
            }
            return Json(discount);
        }


        public ActionResult BindOperationHeadChart1(long? SYKIID)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            List<ADORGLEVEL> divList = _objRiskAssessmentService.BindOperation(SYKIID);
            var result = _objRiskAssessmentService.Get_Risk_Details_Report_OperationHead_Division_Wise(userId);

            int Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            int High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            int Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            int Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            int VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            string s = Extreme + "," + High + "," + Medium + "," + Low + "," + VeryLow;

            return Json(s);
        }

        public ActionResult BindOperationHeadChart2()
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var resultcount = _objRiskAssessmentService.Get_Risk_Report_Count_OperationHead_DivisionWise(userId);
            List<DivisionWise_Risk_Count> discount = new List<DivisionWise_Risk_Count>();

            for (int i = 0; i < resultcount.Count; i++)
            {
                DivisionWise_Risk_Count d = new DivisionWise_Risk_Count();
                d.DivisionName = resultcount[i].DivisionName;
                d.TSCount = resultcount[i].TSCount;
                d.TACount = resultcount[i].TACount;
                d.TBCount = resultcount[i].TBCount;
                d.TCCount = resultcount[i].TCCount;
                d.TDCount = resultcount[i].TDCount;
                discount.Add(d);
            }
            return Json(discount);
        }
        public ActionResult BindOperationId(long? SYKIID)
        {
            List<ADORGLEVEL> divList = _objRiskAssessmentService.BindOperation(SYKIID);
            return Json(divList);
        }
        private SearchParameterList ITDivisionApprovalgetSearchFilterData(SearchParameterList paramsList)
        {
            List<SearchParameterList> list = new List<SearchParameterList>();
            SearchParameterList data = new SearchParameterList();
            List<SearchParameterList> kiData = new List<SearchParameterList>();

            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            kiData.Add(new SearchParameterList() { SYKIID = 0, SYKI = "Select" });

            var kiData1 = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            var kii = kiData1._SYKIList.ToList().OrderBy(x => x.SYKIID);
            foreach (var tblKiCode in kii)
            {
                kiData.Add(new SearchParameterList() { SYKIID = tblKiCode.SYKIID, SYKI = tblKiCode.KICODE });
            }
            var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? 0 : paramsList.SYKIID;
            ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI", selectedKi);
            data.OPERATIONID = paramsList.OPERATIONID;
            data.DIVISIONID = paramsList.DIVISIONID;

            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            list.Add(data);
            ViewBag.List = list;
            return data;
        }

        [HttpGet]
        public ActionResult DeficiencyRiskRegister()
        {
            TempData["PageHead"] = "ITGRC Team Feedback";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            decimal current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            List<ADORGLEVEL> opList = _objRiskAssessmentService.BindOperation((long)current_ki);
            List<RiskCategoryVM> categoryList = new List<RiskCategoryVM>();

            ViewBag.OpList = new SelectList(opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");

            ViewBag.PassetList = new SelectList(string.Empty, "Id", "Text");
            categoryList = _objRiskAssessmentService.GetAllRiskCategory().ToList();
            ViewBag.categoryList = new SelectList(categoryList, "RiskId", "Category");

            return View();
        }

        [HttpPost]
        public ActionResult DeficiencyRiskRegister(Risk_DifiencyReport didiency)
        {

            //if (!ModelState.IsValid)
            //{
            //    return View(didiency);
            //}

            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            didiency.createdby = userId;
            _objRiskAssessmentService.InsertRiskDifiency(didiency);

            List<SYKI> iList = new List<SYKI>();
            CultureInfo culture = new CultureInfo("en-GB");
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            long currentki = Convert.ToInt64(kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault());

            // Send EMail to  ISSC Member and Division Head  
            //var result = _objRiskAssessmentService.Send_Email_To_ISSC_Member_DivisionHead(didiency);
            // Send EMail to ISSC Member
            var result = _objRiskAssessmentService.Send_Email_To_ISSC_Member_DivisionHead(didiency);
            if (result != null)
            {
                CommonRiskAssessmentVM risk_detail = new CommonRiskAssessmentVM();
                risk_detail.CreatedBy = result.Select(x => x.EmpCode).FirstOrDefault();
                risk_detail.SKYID = currentki;
                var s = _objRiskAssessmentService.GetStartDate_EndDate_ForRisk_Register(risk_detail);
                var divison_head = _objRiskAssessmentService.Get_OperatingHead_(currentki, result.Select(x => x.EmpCode).FirstOrDefault(), userId);
                // var divison_head = _objRiskAssessmentService.Get_OperatingHead_Old(currentki, result.Select(x => x.EmpCode).FirstOrDefault());

                SendMailTo_ISSCMember_After_DifiencyReportSendBack(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.ISSC_Member_Name, Convert.ToDateTime(s.StartDate, culture), Convert.ToDateTime(s.EndDate, culture));
                TempData["msg"] = "Deficiency feedback submitted sucessfully.";
            }
            return RedirectToAction("DeficiencyRiskRegister");
        }

        public ActionResult BindDivisionByRiskCategory()
        {
            List<RiskCategoryVM> divList = _objRiskAssessmentService.GetAllRiskCategory().ToList();
            return Json(divList);
        }
        public ActionResult BindDivisionByPrimaryAsset(long id)
        {
            var divList = _objRiskAssessmentService.BindPrimaryAsset(id).Distinct().ToList();
            return Json(divList);
        }


        [HttpGet]
        public ActionResult RiskDetailsApproveByOperatingHead(int ISSC_Code)
        {
            TempData["PageHead"] = "Information Risk Register Approval";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            ViewBag.EnddateCheck = _objRiskAssessmentService.ISSC_Member_RiskDetails_EndDate_Check(ISSC_Code, (long)current_ki);
            ViewBag.Final_Submit_Check = _objRiskAssessmentService.Operating_Head_RiskDetails_FinalSubmit_Check(ISSC_Code, (long)current_ki);

            var result = _objRiskAssessmentService.Get_ISSC_Member_Risk_For_DivisionHead(ISSC_Code, (long)current_ki);
            ViewBag.Assetdetails = result;

            ViewBag.Extreme = result.Where(x => x.Current_Risk_Level_RA == "S").Count();
            ViewBag.High = result.Where(x => x.Current_Risk_Level_RA == "A").Count();
            ViewBag.Medium = result.Where(x => x.Current_Risk_Level_RA == "B").Count();
            ViewBag.Low = result.Where(x => x.Current_Risk_Level_RA == "C").Count();
            ViewBag.VeryLow = result.Where(x => x.Current_Risk_Level_RA == "D").Count();
            CommonRiskAssessmentVM asset_deatils = new CommonRiskAssessmentVM();
            asset_deatils.CreatedBy = ISSC_Code;
            asset_deatils.SKYID = (long)current_ki;
            //ViewBag.Remarks = _objRiskAssessmentService.GetRemrksForOPHead((long)current_ki, ISSC_Code);
            return View(asset_deatils);
        }
        [HttpPost]
        public IActionResult RiskDetailsApproveByOperatingHead(CommonRiskAssessmentVM risk_deatils)
        {
            //int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            //risk_deatils.Operating_Head_EmpCode = userId;
            //List<SYKI> iList = new List<SYKI>();
            //CultureInfo culture = new CultureInfo("en-GB");
            //var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            //foreach (var item in kiData._SYKIList)
            //{
            //    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            //}
            //if (userId > 0)
            //{
            //    var divison_head = _objRiskAssessmentService.Get_OperatingHead_((long)risk_deatils.SKYID, risk_deatils.CreatedBy, userId);

            //    if (Request.Form["btnFinalSubmit"] != null)
            //    {
            //        var ResponseResult = _objRiskAssessmentService.Risk_Approve_By_OperatingHead(risk_deatils);
            //        TempData["ResultMsg"] = ResponseResult;
            //        // Send EMail to  ISSC Member and Operating Head  
            //        // var result // Send EMail to  Operating Head  
            //        var result = _objRiskAssessmentService.Get_DivisionHead_((long)risk_deatils.SKYID, risk_deatils.CreatedBy);
            //        try
            //        {
            //            SendMailTo_DivisionHead_After_Operatinghead(result.Empname, result.Emailid, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname);
            //        }
            //        catch (Exception ex)
            //        {
            //            TempData["ResultMsg"] = ex.Message;
            //        }
            //        // Send EMail to  ISSC Member 

            //        SendMailTo_ISSCMember_AfterOperatingHeadApprove(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname);

            //    }
            //    else if (Request.Form["btnsendback"] != null)
            //    {
            //        var ResponseResult = _objRiskAssessmentService.Risk_SendBack_By_OperatingHead(risk_deatils);
            //        TempData["ResultMsg"] = ResponseResult;
            //        // Send EMail to ISSC Member
            //        // var result = _objRiskAssessmentService.Send_Email_To_ISSC_(risk_deatils);
            //        var s = _objRiskAssessmentService.GetStartDate_EndDate_ForRisk_Register(risk_deatils);
            //        // Send EMail to ISSC Member
            //        var result = _objRiskAssessmentService.Send_Email_To_ISSC_OperatingHead_FroSendBack(risk_deatils);

            //        SendMailTo_ISSCMember_After_OperatingHeadSendBack(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname, Convert.ToDateTime(s.StartDate, culture), Convert.ToDateTime(s.EndDate, culture));

            //    }
            //    //else if (Request.Form["btnReject"] != null)
            //    //{
            //    //    var ResponseResult = _objRiskAssessmentService.Risk_Reject_By_OperatingHead(risk_deatils);
            //    //    TempData["ResultMsg"] = ResponseResult;
            //    //}
            //}
            return RedirectToAction("RiskAssessmentDashboardForOperatingHead");
        }

        [HttpGet]
        public ActionResult Risk_Assessment_Data_for_Operating_Head()
        {
            ViewBag.ISSCM = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            ViewBag.CurrKey = current_ki;
            var result = _objRiskAssessmentService.OPeratingHeads_DeatiledReport();
            return View(result);

        }

        [HttpGet]
        public ActionResult RiskAssessmentDashboardForOperatingHead()
        {
            TempData["PageHead"] = "Information Risk Assessment Approval";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var kiData_ = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            var current_ki = kiData_._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            var oldUserId = _objRiskAssessmentService.GetRiskUserDeailsForOPHeadByLoginUserID(userId, (long)current_ki);
            List<Get_RiskDivision_ISSC_Member> lstDivision_ISSC_Member = new List<Get_RiskDivision_ISSC_Member>();
            if (oldUserId != null)
            {
                lstDivision_ISSC_Member = _objRiskAssessmentService.Get_ISSC_Members_Risk_Details_For_OpearingHead(oldUserId);
                var lst_Division_ISSC_Member_1 = _objRiskAssessmentService.Get_ISSC_Members_Risk_Details_For_OpearingHead(userId);
                if (lst_Division_ISSC_Member_1.Any())
                {
                    if (lstDivision_ISSC_Member.Any())
                        lstDivision_ISSC_Member.AddRange(lst_Division_ISSC_Member_1);
                    else
                        lstDivision_ISSC_Member = lst_Division_ISSC_Member_1;
                }
            }
            else
            {
                lstDivision_ISSC_Member = _objRiskAssessmentService.Get_ISSC_Members_Risk_Details_For_OpearingHead(userId);
            }

            List<SYKI> iList1 = new List<SYKI>();

            foreach (var item in kiData_._SYKIList)
            {
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            DateTime? dt = lstDivision_ISSC_Member.Select(x => x.EndDate).FirstOrDefault();
            ViewBag.endate = String.Format("{0:dd/MM/yyyy}", dt);
            ViewBag.ISSC_MemberData = lstDivision_ISSC_Member;

            DateTime end_dates = Convert.ToDateTime(lstDivision_ISSC_Member.Select(x => x.EndDate).FirstOrDefault());
            int nodaysleft = end_dates.Subtract(DateTime.Now.Date).Days;

            int Op_Head_Appoval_Count = lstDivision_ISSC_Member.Where(x => x.OperatingHead_Submit_Status == 1).Count();
            int total_list_count = lstDivision_ISSC_Member.Count();

            if (Op_Head_Appoval_Count != total_list_count)
                ViewBag.nodaysleft = nodaysleft + " Days";
            else
                ViewBag.nodaysleft = "0 Days";

            List<RiskAssessmentSYKIListForOH> last_year = new List<RiskAssessmentSYKIListForOH>();

            if (oldUserId != null)
            {
                RiskAssessmentSYKIViewModelOH assetRegistrationSYKIViewModel = new RiskAssessmentSYKIViewModelOH();
                assetRegistrationSYKIViewModel = _objRiskAssessmentService.GetRiskAssessmentSYKIListFor_OperatingHeadDeails_Dashboard((long)current_ki, (int)oldUserId);
                var assetRegistrationSYKIViewModel_1 = _objRiskAssessmentService.GetRiskAssessmentSYKIListFor_OperatingHeadDeails_Dashboard((long)current_ki, userId);

                if (assetRegistrationSYKIViewModel_1._SYKIList.Any())
                {
                    if (assetRegistrationSYKIViewModel._SYKIList.Any())
                        assetRegistrationSYKIViewModel._SYKIList.AddRange(assetRegistrationSYKIViewModel_1._SYKIList);
                    else
                    {
                        assetRegistrationSYKIViewModel._SYKIList = assetRegistrationSYKIViewModel_1._SYKIList;
                    }
                }

                foreach (var item in assetRegistrationSYKIViewModel._SYKIList)
                {
                    last_year.Add(new RiskAssessmentSYKIListForOH { KICODE = item.KICODE, SYKIID = item.SYKIID, DIVISIONNAMEN = item.DIVISIONNAMEN, ADEMPCODE = item.ADEMPCODE, OperationName = item.OperationName, OPID = item.OPID });
                }
                ViewBag.Lastki_details = last_year;
            }
            else
            {
                var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIListFor_OperatingHeadDeails_Dashboard((long)current_ki, userId);

                foreach (var item in kiData._SYKIList)
                {
                    last_year.Add(new RiskAssessmentSYKIListForOH { KICODE = item.KICODE, SYKIID = item.SYKIID, DIVISIONNAMEN = item.DIVISIONNAMEN, ADEMPCODE = item.ADEMPCODE, OperationName = item.OperationName, OPID = item.OPID });
                }
                ViewBag.Lastki_details = last_year;
            }
            return View();
        }
        private void SendMailTo_ISSCMember_AfterOperatingHeadApprove(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Risk Assessment Approval Request " + ski;
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                 "<tr style='background-color:skyblue;'>Risk Assessment Approval " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +

                                 "<tr><td><b>Dear " + ISSC_MemberName + " " + "San," + "</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Risk Assessment has been approved by  " + ApproverName + " " + "San" + " for " + ski + ".</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for details.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }
        private void SendMailTo_DivisionHead_After_Operatinghead(string Emp, string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = " Risk Assessment Approval Request " + ski;
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                 "<tr style='background-color:skyblue;'>Risk Assessment Approval " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +

                                 "<tr><td><b>Dear " + Emp + " " + "San," + "</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Risk Assessment has been approved by  " + ApproverName + " " + "San" + " for " + ski + ".</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for details.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }
        private void SendMailTo_ISSCMember_After_OperatingHeadSendBack(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName, DateTime StateDate, DateTime EndDate)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Risk Assessment send back";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                "<tr style='background-color:skyblue;'> " + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +

                                 "<tr><td><b>Dear " + ISSC_MemberName + " " + "San," + "</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Risk Assessment has been send back by  " + ApproverName + " " + "San" + ".</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>Kindly update Risk Assessment as window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                                "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for updation process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void SendMailTo_ISSCMember_After_DifiencyReportSendBack(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName, DateTime StateDate, DateTime EndDate)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Risk Assessment send back (Deficiency Report)";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                "<tr style='background-color:skyblue;'>Risk Assessment send back " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                 "<tr><td><b>Dear " + ISSC_MemberName + " " + "San," + "</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Risk Assessment has been send back by ISMS team with deficiency report.</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>Kindly update Risk Assessment as window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                                "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for updation process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        //Mailer_For_ISSC_Nomination works Start here
        public ActionResult RiskAssessmentReminderMailers(int? id)
        {
            TempData["PageHead"] = "Risk Assessment Reminder Mailers";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objRiskAssessmentService.GetRiskAssessmentSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
            ViewBag.CurrentSYKIData = DateTime.Now.Date;
            var currentki = iList.Select(x => x.SYKIID).FirstOrDefault();
            var MaileReminderlist = _objRiskAssessmentService.GetReminderDetailsForISSCMember_Nomination();
            ViewBag.MaileReminderlist = MaileReminderlist;
            RiskMailerReminderVM MaileReminder = _objRiskAssessmentService.GetStart_Dt_End_Dt_ISSC_Nomination((long)currentki);
            if (id > 0)
            {
                MaileReminder.ReminerDate = _objRiskAssessmentService.RiskGetReminderDate((long)id);
                MaileReminder.ReminderId = id;
            }
            return View(MaileReminder);
        }

        [HttpPost]
        public ActionResult RiskAssessmentReminderMailers(RiskMailerReminderVM MaileReminder)
        {
            MaileReminder.ReminderFor = 1;
            var result = _objRiskAssessmentService.InsertISSC_Reminder(MaileReminder);
            if (MaileReminder.ReminderId > 0)
                return Redirect("../../RiskAssessment/RiskAssessmentReminderMailers");
            else
                return RedirectToAction("RiskAssessmentReminderMailers");
        }
        //Mailer_For_ISSC_Nomination works End here
    }
}
