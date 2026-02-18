using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Vml;
using ePortal.Persistence.Admin.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Org.BouncyCastle.Ocsp;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using Spire.Pdf;
using ePortal.ViewModels.APPX.Separation;
using ePortal.ViewModels;
using System.Reflection.Emit;
using DocumentFormat.OpenXml.Office2016.Drawing.Charts;
using DocumentFormat.OpenXml.Office.CustomUI;
using ePortal.WebUI.Filters;
using System.Security.AccessControl;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace ePortal.WebUI.Controllers
{    
    [CSPFilter]
    [SessionTimeout]
    public class SeparationController : Controller
    {
        public enum ExportFormat : int { CSV = 1, Excel = 2 }; // Export format enumeration	
        private readonly ISeparation _Separation;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        public SeparationController(ISeparation SeparationService, ISessionService objSessionService, ILogger<HomeController> logger)
        {
            _Separation = SeparationService;
            _sessionService = objSessionService;
            _logger = logger;
        }

        //-------------MyresignationRequest-------------------
        [HttpGet]
        public ActionResult MYResignationRequest()
        {
            var modelResigation = myresignationprocess();
            return View("MYResignationRequest", modelResigation);
        }
        private MyResignationRequestModel myresignationprocess()
        {
            string strempcode = _sessionService.Get<string>("userID");
            ViewBag.UserID = strempcode;
            string strPhotoPath = serverpath.getPhotoPath();
            string strPhotoServer = string.Empty;
            string strImagePath = string.Empty;
            string strapptype = string.Empty;
            strPhotoServer = serverpath.getServerPath();
            int E = 0;
            int W = 0;
            var modelSeparation = new MyResignationRequestModel();
            var modelResignationStatus = new ResignationStatusViewModel();
            var modelHRSPResigDetail = new HRSPResigDetailViewModel();
            DataTable dt = _Separation.Get_Resignationprocess(strempcode);
            DataTable dtflow = _Separation.Get_RESIGPROCESSFLOW(strempcode);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strappstatus = dt.Rows[i]["APPSTATUS"].ToString();
                string strordrno = dt.Rows[i]["orderno"].ToString();
                string strprostatus = dt.Rows[i]["APPEDIT"].ToString();
                string strhrstatus = dt.Rows[i]["HRHEAD_STATUS"].ToString();
                strapptype = dt.Rows[i]["resig_app"].ToString();
                //hd_Resign.Value = dt.Rows[i]["resignationid"].ToString();
                modelResignationStatus.hd_Resign = dt.Rows[i]["resignationid"].ToString();
                if (!string.IsNullOrEmpty(strappstatus))
                {
                    E = E + Convert.ToInt32(strappstatus);
                }

                if (strordrno == "2")
                {
                    if (strappstatus == "0")
                    {
                        modelResignationStatus.lbldptstatus = "Pending";
                        if (Convert.ToUInt32(strprostatus) >= 1)
                            modelResignationStatus.lbldptstatusColor = "red";
                        else
                            modelResignationStatus.lbldptstatusColor = "gray";
                    }
                    if (strappstatus == "1")
                    {
                        modelResignationStatus.lbldptstatus = "Approved";
                        modelResignationStatus.lbldptstatusColor = "green";
                    }
                    if (strappstatus == "2")
                    {
                        modelResignationStatus.lbldptstatus = "Hold";
                        modelResignationStatus.lbldptstatusColor = "orange";
                    }
                }
                if (strordrno == "3")
                {
                    if (strappstatus == "0")
                    {
                        modelResignationStatus.lbldivstatus = "Pending";
                        if (Convert.ToUInt32(strprostatus) >= 2)
                            modelResignationStatus.lbldivstatusColor = "red";
                        else
                            modelResignationStatus.lbldivstatusColor = "gray";
                    }

                    if (strappstatus == "1")
                    {
                        modelResignationStatus.lbldivstatus = "Approved";
                        modelResignationStatus.lbldivstatusColor = "green";
                    }
                    if (strappstatus == "2")
                    {
                        modelResignationStatus.lbldivstatus = "Hold";
                        modelResignationStatus.lbldivstatusColor = "orange";
                    }
                }
                if (strordrno == "4")
                {

                    if (strappstatus == "0")
                    {
                        modelResignationStatus.lblopstatus = "Pending"; //=3
                        if (Convert.ToUInt32(strprostatus) >= 3)
                            modelResignationStatus.lblopstatusColor = "red";
                        else
                            modelResignationStatus.lblopstatusColor = "gray";
                    }

                    if (strappstatus == "1")
                    {
                        modelResignationStatus.lblopstatus = "Approved";
                        modelResignationStatus.lblopstatusColor = "green";
                    }
                    if (strappstatus == "2")
                    {
                        modelResignationStatus.lblopstatus = "Hold";
                        modelResignationStatus.lblopstatusColor = "orange";
                    }
                    if (!string.IsNullOrEmpty(strappstatus))
                    {
                        W = W + Convert.ToInt32(strappstatus);
                    }

                }
                if (strordrno == "9")
                {

                    if (strappstatus == "0")
                    {
                        modelResignationStatus.lbldirstatus = "Pending"; //=3
                        if (Convert.ToUInt32(strprostatus) >= 3)
                            modelResignationStatus.lbldirstatusColor = "red";
                        else
                            modelResignationStatus.lbldirstatusColor = "gray";
                    }

                    if (strappstatus == "1")
                    {
                        modelResignationStatus.lbldirstatus = "Approved";
                        modelResignationStatus.lbldirstatusColor = "green";
                    }
                    if (strappstatus == "2")
                    {
                        modelResignationStatus.lbldirstatus = "Hold";
                        modelResignationStatus.lbldirstatusColor = "orange";
                    }
                    if (!string.IsNullOrEmpty(strappstatus))
                    {
                        W = W + Convert.ToInt32(strappstatus);
                    }

                }
                if (strordrno == "5")
                {
                    if (strappstatus == "0")
                    {
                        modelResignationStatus.lblhrstatus = "Pending";
                        if (Convert.ToUInt32(strprostatus) >= 4)
                            modelResignationStatus.lblhrstatusColor = "red";
                        else
                            modelResignationStatus.lblhrstatusColor = "gray";
                    }
                    if (strappstatus == "1")
                    {
                        modelResignationStatus.lblhrstatus = "Approved";
                        modelResignationStatus.lblhrstatusColor = "green";
                    }
                    if (strappstatus == "2")
                    {
                        modelResignationStatus.lblhrstatus = "Hold";
                        modelResignationStatus.lblhrstatusColor = "orange";
                    }
                    //separation change
                    bool isBlueCaller = false; //Company casual, staff, line associate etc
                    DataTable dtEmp = new DataTable();
                    string strresigid = dt.Rows[i]["resignationid"].ToString();
                    dtEmp = _Separation.HRSPResigDetail(strempcode, strresigid);
                    if (dtEmp.Rows.Count > 0)
                    {
                        //lbl_desg.Text = dt.Rows[0]["DESCRIP"].ToString();
                        DataTable dtDesg = _Separation.GET_SYPARAMETERS_PARAMVALUE("DESG_STAFFHOMEPAGE_VALIDATION");
                        if (dtDesg.Rows.Count > 0)
                        {
                            string[] desg = dtDesg.Rows[0]["PARAMVALUE"].ToString().Split(',');
                            for (int i1 = 0; i1 <= desg.Length - 1; i1++)
                            {
                                if (desg[i1].ToString() == dtEmp.Rows[0]["ADDESIGNATIONID"].ToString())
                                {
                                    isBlueCaller = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (isBlueCaller == true)
                    {
                        modelHRSPResigDetail.lblhrname = dt.Rows[i]["APPNAME"].ToString().Replace("HR", "IR");
                        modelHRSPResigDetail.lblhrdesg = dt.Rows[i]["DESIG"].ToString().Replace("HR", "IR");
                        modelHRSPResigDetail.ShowExitInterview = !string.IsNullOrEmpty(modelHRSPResigDetail.exitinterview);


                    }
                    else
                    {
                        modelHRSPResigDetail.lblhrname = dt.Rows[i]["APPNAME"].ToString();
                        modelHRSPResigDetail.lblhrdesg = dt.Rows[i]["DESIG"].ToString();
                    }
                    //separation change
                    if (dt.Rows[i]["APPECODE"].ToString() != "0")
                    {

                        if (System.IO.File.Exists(strPhotoPath + dt.Rows[i]["APPECODE"].ToString() + "." + "jpg"))
                        {
                            modelHRSPResigDetail.uimghrUrl = @"" + strPhotoServer + "Uploads/Photographs/" + dt.Rows[i]["APPECODE"].ToString() + "." + "jpg";
                            modelHRSPResigDetail.Showuimghr = true;
                        }
                        else
                        {
                            modelHRSPResigDetail.Showuimghr = false;
                        }
                    }
                    else
                    {
                        modelHRSPResigDetail.Showuimghr = false;
                    }

                }
                if (strordrno == "6")
                {
                    if (strappstatus == "0")
                    {
                        modelHRSPResigDetail.lblacptstaus = "Pending";
                        if (Convert.ToUInt32(strprostatus) >= 5)
                            modelHRSPResigDetail.lblacptstausColor = "red";
                        else
                            modelHRSPResigDetail.lblacptstausColor = "gray";
                    }
                    if (strappstatus == "1")
                    {
                        modelHRSPResigDetail.lblacptstaus = "Approved";
                        modelHRSPResigDetail.lblacptstausColor = "green";
                    }
                    if (strappstatus == "2")
                    {
                        modelHRSPResigDetail.lblacptstaus = "Hold";
                        modelHRSPResigDetail.lblacptstausColor = "orange";
                    }
                    //separation change
                    bool isBlueCaller = false; //Company casual, staff, line associate etc
                    DataTable dtEmp = new DataTable();
                    string strresigid = dt.Rows[i]["resignationid"].ToString();
                    dtEmp = _Separation.HRSPResigDetail(strempcode, strresigid);
                    if (dtEmp.Rows.Count > 0)
                    {
                        DataTable dtDesg = _Separation.GET_SYPARAMETERS_PARAMVALUE("DESG_STAFFHOMEPAGE_VALIDATION");
                        if (dtDesg.Rows.Count > 0)
                        {
                            string[] desg = dtDesg.Rows[0]["PARAMVALUE"].ToString().Split(',');
                            for (int i1 = 0; i1 <= desg.Length - 1; i1++)
                            {
                                if (desg[i1].ToString() == dtEmp.Rows[0]["ADDESIGNATIONID"].ToString())
                                {
                                    isBlueCaller = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (isBlueCaller == true)
                    {
                        modelHRSPResigDetail.lblacptname = dt.Rows[i]["APPNAME"].ToString().Replace("HR", "IR");
                        modelHRSPResigDetail.lblacptdesg = dt.Rows[i]["DESIG"].ToString().Replace("HR", "IR");
                        modelHRSPResigDetail.ShowExitInterview = !string.IsNullOrEmpty(modelHRSPResigDetail.exitinterview);

                    }
                    else
                    {
                        modelHRSPResigDetail.lblacptname = dt.Rows[i]["APPNAME"].ToString();
                        modelHRSPResigDetail.lblacptdesg = dt.Rows[i]["DESIG"].ToString();
                    }
                    //separation change
                    if (dt.Rows[i]["APPECODE"].ToString() != "0")
                    {

                        if (System.IO.File.Exists(strPhotoPath + dt.Rows[i]["APPECODE"].ToString() + "." + "jpg"))
                        {
                            modelHRSPResigDetail.uimgacptUrl = @"" + strPhotoServer + "Uploads/Photographs/" + dt.Rows[i]["APPECODE"].ToString() + "." + "jpg";
                            modelHRSPResigDetail.Showuimgacpt = true;
                        }
                        else
                        {
                            modelHRSPResigDetail.Showuimgacpt = false;
                        }
                    }
                    else
                    {
                        modelHRSPResigDetail.Showuimgacpt = false;
                    }

                }
            }
            if (E > 0)
            {
                modelHRSPResigDetail.ShowLinkEdit = false;
            }
            if (W > 0)
            {
                modelHRSPResigDetail.ShowAnchWithdraw = false;
            }

            //Visible true after open withdraw by HR (Managewithdrawresignation.aspx)
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["WITHDRAW"].ToString() == "1")
                {
                    modelHRSPResigDetail.ShowAnchWithdraw = true;
                }
            }
            ///////////////////////////////////////////////////////////////////////////

            string strascode = string.Empty;
            string strasname = string.Empty;
            string strasDS = string.Empty;
            string strdptcode = string.Empty;
            string strdptname = string.Empty;
            string strdptDS = string.Empty;
            string strdivcode = string.Empty;
            string strdivname = string.Empty;
            string strdivDS = string.Empty;
            string stropcode = string.Empty;
            string stropname = string.Empty;
            string stropDS = string.Empty;
            string strdircode = string.Empty;
            string strdirname = string.Empty;
            string strdirDS = string.Empty;
            string strdirpos = string.Empty;
            string sttrpos = string.Empty;

            modelHRSPResigDetail.pnldpt = true;
            modelHRSPResigDetail.pnldiv = true;
            modelHRSPResigDetail.pnldir = true;
            modelHRSPResigDetail.pnlop = true;
            if (dtflow.Rows.Count > 0)
            {
                strascode = dtflow.Rows[0]["ADEMPCODE"].ToString();
                strasname = dtflow.Rows[0]["EMPNAME"].ToString();
                strasDS = dtflow.Rows[0]["ASDESG"].ToString();
                strdptcode = dtflow.Rows[0]["DEPARTMENTHEADID"].ToString();
                strdptname = dtflow.Rows[0]["DPTHEADNAME"].ToString();
                strdptDS = dtflow.Rows[0]["DTDESG"].ToString();
                strdivcode = dtflow.Rows[0]["DIVISIONHEADID"].ToString();
                strdivname = dtflow.Rows[0]["DIVHEADNAME"].ToString();
                strdivDS = dtflow.Rows[0]["DVDESG"].ToString();
                stropcode = dtflow.Rows[0]["VPHEADID"].ToString();
                stropname = dtflow.Rows[0]["OPHEADNAME"].ToString();
                stropDS = dtflow.Rows[0]["OPDESG"].ToString();
                sttrpos = dtflow.Rows[0]["POS"].ToString();
                strdircode = dtflow.Rows[0]["DIRID"].ToString();
                strdirname = dtflow.Rows[0]["DIRNAME"].ToString();
                strdirDS = dtflow.Rows[0]["DIRDESG"].ToString();
            }
            if (!string.IsNullOrEmpty(strascode))
            {
                modelHRSPResigDetail.lblassociatename = strasname;
                modelHRSPResigDetail.lblassociatedesg = strasDS;
                if (System.IO.File.Exists(strPhotoPath + strascode + "." + "jpg"))
                {
                    modelHRSPResigDetail.uimgassociateUrl = @"" + strPhotoServer + "Uploads/Photographs/" + strascode + "." + "jpg";
                    modelHRSPResigDetail.Showuimgassociate = true;
                }
                else
                {
                    modelHRSPResigDetail.Showuimgassociate = false;
                }
            }
            if (string.IsNullOrEmpty(strdptcode))
            {
                modelHRSPResigDetail.pnldpt = false;

            }
            else
            {
                modelHRSPResigDetail.lbldptname = strdptname;
                modelHRSPResigDetail.lbldptdesg = strdptDS;

                if (System.IO.File.Exists(strPhotoPath + strdptcode + "." + "jpg"))
                {
                    modelHRSPResigDetail.uimgdptUrl = @"" + strPhotoServer + "Uploads/Photographs/" + strdptcode + "." + "jpg";
                    modelHRSPResigDetail.Showuimgdpt = true;
                }
                else
                {
                    modelHRSPResigDetail.Showuimgdpt = false;
                }

            }
            if (string.IsNullOrEmpty(strdivcode))
            {
                modelHRSPResigDetail.pnldiv = false;
            }
            else
            {
                modelHRSPResigDetail.lbldivname = strdivname;
                modelHRSPResigDetail.lbldivdesg = strdivDS;
                if (System.IO.File.Exists(strPhotoPath + strdivcode + "." + "jpg"))
                {
                    modelHRSPResigDetail.uimgdivUrl = @"" + strPhotoServer + "Uploads/Photographs/" + strdivcode + "." + "jpg";
                    modelHRSPResigDetail.Showuimgdiv = true;
                }
                else
                {
                    modelHRSPResigDetail.Showuimgdiv = false;
                }

            }
            if (string.IsNullOrEmpty(stropcode))
            {
                modelHRSPResigDetail.pnlop = false;
            }
            else
            {
                modelHRSPResigDetail.lblopname = stropname;
                modelHRSPResigDetail.lblopdesg = stropDS;

                if (System.IO.File.Exists(strPhotoPath + stropcode + "." + "jpg"))
                {
                    modelHRSPResigDetail.uimgopUrl = @"" + strPhotoServer + "Uploads/Photographs/" + stropcode + "." + "jpg";
                    modelHRSPResigDetail.Showuimgop = true;
                }
                else
                {
                    modelHRSPResigDetail.Showuimgop = false;
                }

            }

            if (sttrpos == "DPT")
            {
                modelHRSPResigDetail.pnldpt = false;
                modelHRSPResigDetail.pnldir = false;
            }
            if (sttrpos == "DIV")
            {
                modelHRSPResigDetail.pnldpt = false;
                modelHRSPResigDetail.pnldiv = false;
                modelHRSPResigDetail.pnldir = false;
            }
            if (sttrpos == "OP")
            {
                modelHRSPResigDetail.pnldpt = false;
                modelHRSPResigDetail.pnldiv = false;
                modelHRSPResigDetail.pnlop = false;
                modelHRSPResigDetail.pnldir = true;
                modelHRSPResigDetail.lbldirname = strdirname;
                modelHRSPResigDetail.lbldirdesg = strdirDS;

                if (System.IO.File.Exists(strPhotoPath + strdircode + "." + "jpg"))
                {
                    modelHRSPResigDetail.uimgdirUrl = @"" + strPhotoServer + "Uploads/Photographs/" + strdircode + "." + "jpg";
                    modelHRSPResigDetail.Showuimgdir = true;
                }
                else
                {
                    modelHRSPResigDetail.Showuimgdir = false;
                }
            }
            if (strdirDS == "Director" && (string.IsNullOrEmpty(sttrpos) || sttrpos == "OTH"))
            {

                modelHRSPResigDetail.pnldpt = false;
                modelHRSPResigDetail.pnldiv = false;
                modelHRSPResigDetail.pnlop = false;
                modelHRSPResigDetail.pnldir = false;
            }
            if (dt.Rows.Count > 0)
            {
                modelHRSPResigDetail.Appdetailsdiv = true;
                modelHRSPResigDetail.nodata = false;

                var modelAssociate = GET_ASSOCIATEHEADER();
                modelSeparation.AssociateHeaderData = modelAssociate;

                var modelheaderclearance = Get_headerclearance();
                modelSeparation.HeaderClearanceData = modelheaderclearance;

                var modelfillexitintQuest = fillexitinterviewquestionaire();
                modelSeparation.InterviewQuestionsData = modelfillexitintQuest;
            }
            else
            {
                modelHRSPResigDetail.Appdetailsdiv = false;
                modelHRSPResigDetail.nodata = true;
            }

            if (strapptype == "2")
            {
                modelHRSPResigDetail.Resig_Dev = false;
            }
            else
            {
                modelHRSPResigDetail.Resig_Dev = true;
            }

            modelSeparation.ResignationStatusModel = modelResignationStatus;
            modelSeparation.HRSPResigDetailModel = modelHRSPResigDetail;

            return modelSeparation;
        }
        private AssociateHeaderViewModel GET_ASSOCIATEHEADER()
        {
            string strempcode = _sessionService.Get<string>("userID");
            DataTable ADT = _Separation.HRSPAcceptanceDetail(strempcode, "");
            var modelAssociateHeader = new AssociateHeaderViewModel();
            if (ADT.Rows.Count > 0)
            {
                if (ADT.Rows[0]["dptstatus"].ToString() == "0")
                {
                    modelAssociateHeader.anchpendingass = true;
                }
                else
                {
                    modelAssociateHeader.anchapproveass = true;
                }

                if (ADT.Rows[0]["RESIG_STATUS"].ToString() == "2")
                {
                    modelAssociateHeader.ALLPROCESSSTATUSDIV = true;
                }
                else
                {
                    modelAssociateHeader.ALLPROCESSSTATUSDIV = false;
                }
            }
            return modelAssociateHeader;
        }
        private HeaderClearanceViewModel Get_headerclearance()
        {
            string strempcode = _sessionService.Get<string>("userID");
            DataTable dt = _Separation.Get_HEADERCLEARANCE(strempcode);
            var modelHeaderClearance = new HeaderClearanceViewModel();
            if (dt.Rows.Count > 0)
            {
                //modelHeaderClearance.rep_headerclr = dt;
                foreach (DataRow row in dt.Rows)
                {
                    modelHeaderClearance.rep_headerclr.Add(new ClearanceHeaderItem
                    {
                        DESCRIPTION = row["DESCRIPTION"].ToString(),
                        pending = Convert.ToInt32(row["pending"]),
                        complete = Convert.ToInt32(row["complete"]),
                        STATUS = row["STATUS"].ToString(),
                        RESIGNATIONID = row["RESIGNATIONID"].ToString(),
                        CLEARENCEHEADERID = row["CLEARENCEHEADERID"].ToString()
                    });
                }
                modelHeaderClearance.clrprocessdiv = true;
                fillformI(modelHeaderClearance);
            }
            else
            {
                modelHeaderClearance.clrprocessdiv = false;
                modelHeaderClearance.formidiv = false;
            }
            return modelHeaderClearance;
        }
        private ExitInterviewQuestionnaireViewModel fillexitinterviewquestionaire()
        {
            var modelExitQuestionnaire = new ExitInterviewQuestionnaireViewModel();
            DataTable dt = new DataTable();
            dt = _Separation.Get_Exitinterviewquestionaire(_sessionService.Get<string>("userID"));
            if (dt.Rows.Count > 0)
            {
                modelExitQuestionnaire.namelbl = dt.Rows[0]["EMPNAME"].ToString();
                modelExitQuestionnaire.forminamelbl = dt.Rows[0]["EMPNAME"].ToString();
                string RESIGNID = dt.Rows[0]["RESIGNATIONID"].ToString();
                modelExitQuestionnaire.hd_resignid = RESIGNID;
                TempData["hd_resignid"] = RESIGNID;
                string status = dt.Rows[0]["STATUS"].ToString(); //0 PENDING AT USER HAND //1 PENDING AT HR HAND //COMPLETED
                if (status == "")
                {
                    modelExitQuestionnaire.anchorpendingatuser = true;
                    modelExitQuestionnaire.anchorcompleted = false;
                    modelExitQuestionnaire.HRDIV = false;
                    modelExitQuestionnaire.aninterview = true;
                    modelExitQuestionnaire.linkanswerview = false;
                    modelExitQuestionnaire.exitinterques_div = true;
                }
                else if (status == "1")
                {
                    modelExitQuestionnaire.anchorpendingatuser = false;
                    modelExitQuestionnaire.HRDIV = true;
                    modelExitQuestionnaire.anchpendinghr = true;
                    modelExitQuestionnaire.anchcompleted = false;
                    modelExitQuestionnaire.aninterview = false;
                    modelExitQuestionnaire.linkanswerview = true;
                    modelExitQuestionnaire.exitinterques_div = false;
                }
                else if (status == "2")
                {
                    modelExitQuestionnaire.anchpendinghr = false;
                    modelExitQuestionnaire.anchcompleted = true;
                    modelExitQuestionnaire.anchorpendingatuser = false;
                    modelExitQuestionnaire.HRDIV = true;
                    modelExitQuestionnaire.aninterview = false;
                    modelExitQuestionnaire.linkanswerview = true;
                    modelExitQuestionnaire.exitinterques_div = false;
                }
                modelExitQuestionnaire.exitinterview = true;
                modelExitQuestionnaire.exitinterques_div = true;
            }
            else
            {
                modelExitQuestionnaire.exitinterview = false;
                modelExitQuestionnaire.exitinterques_div = false;
            }

            return modelExitQuestionnaire;

        }
        private void fillformI(HeaderClearanceViewModel model)
        {
            DataTable dt = new DataTable();
            dt = _Separation.HRFornIDetail(_sessionService.Get<string>("userID"), "");
            if (dt.Rows.Count > 0)
            {
                model.forminamelbl = dt.Rows[0]["EMPNAME"].ToString();
                //string RESIGNID = hd_resignid.Value;
                //string RESIGNID = TempData["hd_resignid"].ToString();
                string status = dt.Rows[0]["FISTATUS"].ToString(); //0 PENDING AT USER HAND //1 PENDING AT HR HAND //COMPLETED
                if (status == "")
                {
                    model.formidiv = true;
                    model.a1 = true;
                    model.a2 = false;
                    model.lnkformi = true;
                    model.lnkformiview = false;
                }
                else
                {
                    model.formidiv = true;
                    model.a1 = false;
                    model.a2 = true;
                    model.lnkformi = false;
                    model.lnkformiview = true;
                }
            }
            else
            {
                model.formidiv = false;
            }
        }

        [HttpGet]
        public ActionResult linkexitinterview_Click(string resignId)
        {
            HttpContext.Session.SetString("ResigID", resignId);
            return RedirectToAction("ExitInterviewQuestionnaire");
        }

        [HttpGet]
        public ActionResult linkfromi_Click(string resignId)
        {
            HttpContext.Session.SetString("ResigID", resignId);
            return RedirectToAction("FormI");
        }

        [HttpGet]
        public ActionResult anchwithdraw_onclick(string resignId)
        {
            HttpContext.Session.SetString("ResigID", resignId);
            return RedirectToAction("CancellationResignation");
        }

        [HttpGet]
        public ActionResult linkedit_onclick(string resignId)
        {
            HttpContext.Session.SetString("RESIGNID", resignId);
            return RedirectToAction("EditAssociateResignationForm");
        }
        //--------------Set Session----------------------------------
        [HttpPost]
        public void SetResignSession(string resignId)
        {
            HttpContext.Session.SetString("ResigID", resignId);
        }

        //-------------ViewResignationDetails-------------------

        [HttpGet]
        public ActionResult ViewResignationDetails(string resignId)
        {
          
            var viewModel = new ViewResignationDetailsModel();

            string strresigid = _sessionService.Get<string>("ResigID");
            string struserid = _sessionService.Get<string>("userID");

            if (struserid != null && strresigid != null)
            {
                viewModel.EmployeeDetails = FillEmpDetails(struserid, strresigid);
                viewModel.ApprovalDetails = FillApprovalDetails(struserid, strresigid);
            }
            return View("ViewResignationDetails", viewModel);
        }
        private ViewEmpDetailsModel FillEmpDetails(string struserid, string strresigid)
        {
            ViewEmpDetailsModel modelempInfo = new ViewEmpDetailsModel();
            DataTable dt = new DataTable();
            dt = _Separation.HRSPResigDetail(struserid, strresigid);
            if (dt.Rows.Count > 0)
            {
                modelempInfo.lbl_assname = dt.Rows[0]["EMPNAME"].ToString();
                modelempInfo.lbl_assecode = dt.Rows[0]["ADEMPCODE"].ToString();
                modelempInfo.lbl_joindate = dt.Rows[0]["REGDATE"].ToString();
                modelempInfo.lbl_desg = dt.Rows[0]["DESCRIP"].ToString();
                modelempInfo.lbl_fundesg = dt.Rows[0]["FNDESIG"].ToString();
                modelempInfo.lbl_phone = dt.Rows[0]["TMOBILE"].ToString();
                modelempInfo.lbl_emailid = dt.Rows[0]["EMAILID"].ToString();
                modelempInfo.lbl_op = dt.Rows[0]["OPERATION"].ToString();
                modelempInfo.lbl_div = dt.Rows[0]["DIVISION"].ToString();
                modelempInfo.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                modelempInfo.lbl_sec = dt.Rows[0]["SECTION"].ToString();
                modelempInfo.lbl_site = dt.Rows[0]["sitedesc"].ToString();
                modelempInfo.lbl_applydate = dt.Rows[0]["RESIGNED_DATE"].ToString();
                modelempInfo.lbl_reldate = dt.Rows[0]["RELIEVING_DATE_SELF"].ToString();
                modelempInfo.lbl_deptreldate = dt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();
                modelempInfo.lbl_subject = dt.Rows[0]["RESIG_SUBJECT"].ToString();
                modelempInfo.lbl_reason = dt.Rows[0]["RESIG_REASON"].ToString();
                modelempInfo.lblPrintedby = dt.Rows[0]["EMPNAME"].ToString() + "[" + struserid + "]";
            }
            return modelempInfo;
        }
        private List<ApprovalDetailModel> FillApprovalDetails(string struserid, string strresigid)
        {
            DataTable dt = new DataTable();
            dt = _Separation.HRSPApprovalDetail(struserid, strresigid);
            var approvalDetails = new List<ApprovalDetailModel>();
            if (dt.Rows.Count > 0)
            {
                approvalDetails = dt.AsEnumerable().Select(row => new ApprovalDetailModel
                {
                    APPNAME = row["APPNAME"].ToString(),
                    APPECODE = row["APPECODE"].ToString(),
                    DESIG = row["DESIG"].ToString(),
                    STATUS = row["STATUS"].ToString(),
                    APPSUBMITEDATE = row["APPSUBMITEDATE"].ToString(),
                    APPREMARK = row["APPREMARK"].ToString()
                }).ToList();
            }
            return approvalDetails;
        }

        //-----------ViewClearanceDetails-----------------------
        [HttpGet]
        public ActionResult ViewClearanceDetails()
        {
            var viewModel = new ViewClearanceDetailsModel();
            viewModel.ClearanceDetailsViewModel = fillassociatedetails();
            viewModel.ResignationDetailViewModel = GET_RESIGNATION_FULLFINAL_DETAILS();

            return View("ViewClearanceDetails", viewModel);
        }
        private ClearanceDetailsViewModel fillassociatedetails()
        {
            DataTable dt = new DataTable();
            string strresigid = Request.Query["RESIGNID"].ToString();
            string ecode = _sessionService.Get<string>("userID");
            ClearanceDetailsViewModel modelClrDtls = new ClearanceDetailsViewModel();
            dt = _Separation.HRSPResigDetail(ecode, strresigid);
            if (dt.Rows.Count > 0)
            {
                modelClrDtls.lbl_assname = dt.Rows[0]["EMPNAME"].ToString();
                modelClrDtls.lbl_assecode = dt.Rows[0]["ADEMPCODE"].ToString();
                modelClrDtls.lbl_joindate = dt.Rows[0]["REGDATE"].ToString();
                modelClrDtls.lbl_desg = dt.Rows[0]["DESCRIP"].ToString();
                modelClrDtls.lbl_fundesg = dt.Rows[0]["FNDESIG"].ToString();
                modelClrDtls.lbl_phone = dt.Rows[0]["TMOBILE"].ToString();
                modelClrDtls.lbl_emailid = dt.Rows[0]["EMAILID"].ToString();
                modelClrDtls.lbl_op = dt.Rows[0]["OPERATION"].ToString();
                modelClrDtls.lbl_div = dt.Rows[0]["DIVISION"].ToString();
                modelClrDtls.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                modelClrDtls.lbl_sec = dt.Rows[0]["SECTION"].ToString();
                modelClrDtls.lbl_site = dt.Rows[0]["sitedesc"].ToString();
                modelClrDtls.lbl_applydate = dt.Rows[0]["RESIGNED_DATE"].ToString();
                modelClrDtls.lbl_reldate = dt.Rows[0]["RELIEVING_DATE_SELF"].ToString();
                modelClrDtls.lbl_deptreldate = dt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();
                modelClrDtls.lbl_subject = dt.Rows[0]["RESIG_SUBJECT"].ToString();
                modelClrDtls.lbl_reason = dt.Rows[0]["RESIG_REASON"].ToString();
            }
            return modelClrDtls;
        }
        protected List<ResignationDetailViewModel> GET_RESIGNATION_FULLFINAL_DETAILS()
        {
            string RESIGNID = Request.Query["RESIGNID"].ToString();
            string CLEARANCEHEADERID = Request.Query["CLRHEADERID"].ToString();
            DataSet ds = _Separation.FULLRESIGNDETAIL_GET(RESIGNID, "", CLEARANCEHEADERID);

            var headerDetails = ds.Tables[0].AsEnumerable().Select(row => new ResignationDetailViewModel
            {
                Description = row["DESCRIPTION"].ToString(),
                Remarks = row["remarks"].ToString(),
                Ename = row["Ename"].ToString(),
                SubmitBy = row["SUBMITBY"].ToString(),
                SubmittedDate = row["submittedate"].ToString(),
                Status = row["status"].ToString(),
                SubHeaderDetails = ds.Tables[1].AsEnumerable()
                .Where(sub => sub["CLEARENCEHEADERID"].ToString() == row["CLEARENCEHEADERID"].ToString())
                .Select(sub => new SubHeaderDetail
                {
                    Description = sub["DESCRIPTION"].ToString(),
                    Amount = sub["Amount"].ToString(),
                    Remarks = sub["REMARKS"].ToString(),
                    SubmitBy = sub["SUBMITBY"].ToString(),
                    SubmittedDate = sub["SUBMITTEDATE"].ToString(),
                    Status = sub["status"].ToString(),
                    AttachDocument = sub["ATTACHDOCUMENT"].ToString()
                }).ToList()
            }).ToList();

            return headerDetails;
        }

        //----------ExitInterviewAnswers--------------------------
        [HttpGet]
        public ActionResult ExitInterviewAnswers()
        {
            var ResigID = Request.Query["id"].ToString();

            var modelIntQuest = getinterviewquestion();
            var modelIntAnsw = getanswerdetails(ResigID);
            var modelEmpDtls = getempdetails();

            ExitInterviewAnswersModel model = new ExitInterviewAnswersModel
            {
                ExitInterviewQuestionModel = modelIntQuest,
                ExitInterviewAnswerViewModel = modelIntAnsw,
                EmpDetailsModel = modelEmpDtls
            };
            return View("ExitInterviewAnswers", model);
        }
        private List<ExitInterviewQuestionModel> getinterviewquestion()
        {
            DataSet ds = _Separation.GET_EXITINTERVIEW_QUESTIONNAIRE("1", "1", "1", "");

            DataTable questionTable = ds.Tables[0];
            DataTable optionTable = ds.Tables[1];

            var sections = questionTable.AsEnumerable()
                .GroupBy(row => row["SECTIONID"].ToString())
                .Select(group =>
                {
                    var sectionId = group.Key;
                    var sectionTitle = optionTable.AsEnumerable()
                        .Where(r => r["SECTIONID"].ToString() == sectionId)
                        .Select(r => r["SECTIONDESC"].ToString())
                        .FirstOrDefault() ?? "Unknown Section";

                    return new ExitInterviewQuestionModel
                    {
                        SectionId = sectionId,
                        SectionTitle = sectionTitle,
                        Questions = group.Select(q => new InterviewQuestionModel
                        {
                            ExitInterviewQusId = q["exitinerviewqusid"].ToString(),
                            QusType = q["QUSTYPE"].ToString(),
                            SectionId = sectionId,
                            QusDesc = q["qusdesc"].ToString(),
                            Options = optionTable.AsEnumerable()
                                .Where(opt => opt["exitinerviewqusid"].ToString() == q["exitinerviewqusid"].ToString())
                                .Select(opt => new OptionModel
                                {
                                    ExitInterviewOptionId = opt["EXITINERVIEWOPTIONID"].ToString(),
                                    OptionDesc = opt["optiondesc"].ToString()
                                }).ToList()
                        }).ToList()
                    };
                }).ToList();


            return sections;
        }
        public List<ExitInterviewAnswerViewModel> getanswerdetails(string ResigID)
        {
            DataSet ds = _Separation.Get_GET_INTERVIEWANSWER(ResigID);
            var answers = new List<ExitInterviewAnswerViewModel>();

            var questionTable = ds.Tables[0];
            var answerTable = ds.Tables[1];

            var groupedSections = questionTable.AsEnumerable()
                .GroupBy(q => new
                {
                    SectionId = q["SECTIONID"].ToString(),
                    SectionTitle = q.Table.Columns.Contains("SECTIONTITLE") ? q["SECTIONTITLE"].ToString() : ""
                });

            foreach (var sectionGroup in groupedSections)
            {
                var sectionModel = new ExitInterviewAnswerViewModel
                {
                    SectionId = sectionGroup.Key.SectionId,
                    SectionTitle = sectionGroup.Key.SectionTitle
                };

                foreach (var row in sectionGroup)
                {
                    var questionId = row["EXITINERVIEWQUSID"].ToString();
                    var questionType = row["QUSTYPE"].ToString();

                    var filteredAnswers = answerTable.AsEnumerable()
                        .Where(a => a["EXITINERVIEWQUSID"].ToString() == questionId)
                        .ToList();

                    var answerModel = new InterviewAnswerModel
                    {
                        ExitInterviewQusId = questionId,
                        QusType = questionType
                    };

                    if (questionType == "2") // Radio
                    {
                        answerModel.SelectedOption = filteredAnswers.FirstOrDefault()?["Optionans"].ToString();
                    }
                    else if (questionType == "3") // Checklist
                    {
                        answerModel.SelectedOptions = filteredAnswers
                            .Where(a => !string.IsNullOrEmpty(a["Optionans"].ToString()))
                            .Select(a => a["Optionans"].ToString())
                            .ToList();
                    }
                    else // Text
                    {
                        answerModel.TextAns = filteredAnswers.FirstOrDefault()?["Textans"].ToString();
                    }

                    sectionModel.Questions.Add(answerModel);
                }

                answers.Add(sectionModel);
            }

            return answers;
        }
        private EmpDetailsModel getempdetails()
        {
            EmpDetailsModel modelEmpDtls = new EmpDetailsModel();
            DataTable dt = _Separation.HRDDetail(_sessionService.Get<string>("userID"));
            if (dt.Rows.Count > 0)
            {
                modelEmpDtls.lbl_name = dt.Rows[0]["ENAME"].ToString();
                modelEmpDtls.lbl_ecode = dt.Rows[0]["ECODE"].ToString();
                modelEmpDtls.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                modelEmpDtls.lbl_designation = dt.Rows[0]["DESIGNATION"].ToString();
                modelEmpDtls.lbl_since = dt.Rows[0]["doj"].ToString();
                modelEmpDtls.lbl_loc = dt.Rows[0]["sitedesc"].ToString();
                modelEmpDtls.lbl_supdesign = dt.Rows[0]["FNDESIG"].ToString();
                modelEmpDtls.lbl_supname = dt.Rows[0]["SUPNAME"].ToString() + "[" + dt.Rows[0]["SUPERVISOREMPCODE"].ToString() + "]";
                modelEmpDtls.lbl_doj = dt.Rows[0]["DOJ"].ToString();
                modelEmpDtls.lbl_lastdateemp = dt.Rows[0]["relieving_date"].ToString();
                modelEmpDtls.lbl_EXINTDATE = dt.Rows[0]["EXITINTERVIEWDATE"].ToString();
                modelEmpDtls.lbl_age = dt.Rows[0]["AGE"].ToString();
                modelEmpDtls.lbl_qualification = dt.Rows[0]["QUALIFICATION"].ToString();
            }
            return modelEmpDtls;
        }

        //-----------ViewFormI-----------------------------
        [HttpGet]
        public ActionResult ViewFormI()
        {
            if (!string.IsNullOrEmpty(_sessionService.Get<string>("ResigID")))
            {
                string strresigid = _sessionService.Get<string>("ResigID");
                var modelempdtls = getemployeedetails(strresigid);

                ViewFormIModel model = new ViewFormIModel
                {
                    EmployeeViewFormIModel = modelempdtls
                };

                return View("ViewFormI", model);
            }
            else
            {
                return RedirectToAction("ManageResignationRequest", "Separation");
            }
        }
        private EmployeeViewFormIModel getemployeedetails(string strresigid)
        {
            try
            {
                EmployeeViewFormIModel model = new EmployeeViewFormIModel();
                if (strresigid != string.Empty)
                {
                    DataTable dt = _Separation.GetFormIDetail(strresigid);
                    if (dt.Rows.Count > 0)
                    {
                        model.lbl_resignationdate = dt.Rows[0]["RELIEVING_DATE"].ToString();
                        model.lbl_assname = dt.Rows[0]["EMPNAME"].ToString();
                        model.lbl_assecode = dt.Rows[0]["ADEMPCODE"].ToString();
                        model.lbl_daddress = dt.Rows[0]["ADDRESS"].ToString();
                        model.lbl_org = dt.Rows[0]["ORG"].ToString();
                        model.lbl_ecodedesg = dt.Rows[0]["ADEMPCODE"].ToString() + "/" + dt.Rows[0]["DESCRIP"].ToString();
                        model.lbl_joindate = dt.Rows[0]["REGDATE"].ToString();
                        model.lbl_Relivingdate = dt.Rows[0]["RELIEVING_DATE"].ToString();
                        model.lbl_serviceperiod = dt.Rows[0]["yrs"].ToString() + " " + "Year" + " " + dt.Rows[0]["mnths"].ToString() + " " + "Month" + " " + dt.Rows[0]["dys"].ToString() + " " + "Days";
                        model.lbl_basicamt = dt.Rows[0]["BASICSALARYAMT"].ToString();
                        model.lbl_gratuityamt = dt.Rows[0]["gratuityamt"].ToString();
                        model.lblemailid = dt.Rows[0]["EMAILID"].ToString();
                        model.lbl_namesing = dt.Rows[0]["EMPNAME"].ToString();
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //------------ViewHistory----------------------
        [HttpGet]
        public ActionResult ViewHistory()
        {

            Disable_History(Response);
            var modelAsso = fillassociatedetail();
            var modelHistory = GetHistory();
            var modelApprDtls = FillApprovalDetails();

            ViewHistoryModel model = new ViewHistoryModel
            {
                AssociatedetailsViewModel = modelAsso,
                ViewHistoryParentModel = modelHistory,
                ApprovalDetailsModel = modelApprDtls,
            };

            return View("ViewHistory", model);
        }
        public void Disable_History(HttpResponse response)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }
        private AssociatedetailsViewModel fillassociatedetail()
        {
            AssociatedetailsViewModel modelReg = new AssociatedetailsViewModel();
            DataTable dt = new DataTable();
            string strresigid = Request.Query["ResignID"].ToString();
            string ecode = Request.Query["ECODE"].ToString();
            dt = _Separation.HRSPResigDetail(ecode, strresigid);
            if (dt.Rows.Count > 0)
            {
                modelReg.lbl_assname = dt.Rows[0]["EMPNAME"].ToString();
                modelReg.lbl_assecode = dt.Rows[0]["ADEMPCODE"].ToString();
                modelReg.lbl_joindate = dt.Rows[0]["REGDATE"].ToString();
                modelReg.lbl_desg = dt.Rows[0]["DESCRIP"].ToString();
                modelReg.lbl_fundesg = dt.Rows[0]["FNDESIG"].ToString();
                modelReg.lbl_phone = dt.Rows[0]["TMOBILE"].ToString();
                modelReg.lbl_emailid = dt.Rows[0]["EMAILID"].ToString();
                modelReg.lbl_op = dt.Rows[0]["OPERATION"].ToString();
                modelReg.lbl_div = dt.Rows[0]["DIVISION"].ToString();
                modelReg.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                modelReg.lbl_sec = dt.Rows[0]["SECTION"].ToString();
                modelReg.lbl_site = dt.Rows[0]["sitedesc"].ToString();
                modelReg.lbl_applydate = dt.Rows[0]["RESIGNED_DATE"].ToString();
                modelReg.lbl_reldate = dt.Rows[0]["RELIEVING_DATE_SELF"].ToString();
                modelReg.lbl_deptreldate = dt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();
                modelReg.lbl_subject = dt.Rows[0]["RESIG_SUBJECT"].ToString();
                modelReg.lbl_reason = dt.Rows[0]["RESIG_REASON"].ToString();
                modelReg.RESIG_PROCESSSTATUS = dt.Rows[0]["RESIG_PROCESSSTATUS"].ToString();
            }
            return modelReg;
        }
        private ViewHistoryParentModel GetHistory()
        {
            ViewHistoryParentModel model = new ViewHistoryParentModel();
            string Historyname = Request.Query["History"].ToString();
            if (Historyname == "resignapp")
            {
            }
            else if (Historyname == "pendclrform")
            {
                model.ShowPendClrForm = true;
                model.PendingClearanceFormViewModel = Get_pendclrform();
            }
            else if (Historyname == "manageclrheader")
            {
                model.ShowManageClrHeader = true;
                model.ManageClearanceHeaderViewModel = Get_manageclrheader();
            }
            else if (Historyname == "deptclrform")
            {
                model.ShowDeptClrForm = true;
                model.DeptClearanceFormViewModel = get_deptclrform();
            }
            else if (Historyname == "deptclrform1")
            {
                model.ShowDeptClrForm = true;
                model.DeptClearanceFormViewModel = get_deptclrformmyresign();
            }
            return model;
        }
        private List<PendingClearanceFormViewModel> Get_pendclrform()
        {
            DataTable odt = _Separation.GET_SUBHEADERCLRDETAILS(_sessionService.Get<string>("userID"), Request.Query["ResignID"].ToString());
            var details = new List<PendingClearanceFormViewModel>();
            foreach (DataRow row in odt.Rows)
            {
                details.Add(new PendingClearanceFormViewModel
                {
                    ClearanceHeaderId = Convert.ToInt32(row["CLEARENCEHEADERID"]),
                    ClearDetailId = Convert.ToInt32(row["CLEARDETAILID"]),
                    //Amount = Convert.ToDecimal(row["AMOUNT"]),
                    Amount = row["AMOUNT"] != DBNull.Value
                    ? Convert.ToDecimal(row["AMOUNT"])
                    : 0m,
                    Remarks = row["REMARKS"].ToString(),
                    Status = row["STATUS"].ToString(),
                    Description = row["DESCRIPTION"].ToString(),
                    AttachDocument = row["ATTACHDOCUMENT"].ToString()
                });
            }
            return details;
        }
        private List<ManageClearanceHeaderViewModel> Get_manageclrheader()
        {
            string resignId = Request.Query["ResignID"].ToString();
            string ecode = _sessionService.Get<string>("userID");
            DataSet ds = _Separation.FULLRESIGNDETAIL_GET(resignId, ecode, "");

            var list = new List<ManageClearanceHeaderViewModel>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var subHeader = new ManageClearanceHeaderViewModel
                {
                    //ClearanceSubHeaderId = Convert.ToInt32(row["CLEARENCESUBHEADERID"]),
                    ClearanceHeaderId = Convert.ToInt32(row["CLEARENCEHEADERID"]),
                    //SubHeaderDescription = row["DESCRIPTION"].ToString(),
                    //Amount = Convert.ToDecimal(row["AMOUNT"]),
                    Remarks = row["REMARKS"].ToString(),
                    //Status = row["STATUS"].ToString(),
                    //ESubmitBy = row["ESUBMITBY"].ToString(),
                    //SubmitBy = row["SUBMITBY"].ToString(),
                    //SubmittedDate = row["SUBMITTEDATE"].ToString(),
                    //SubmittedDate1 = row["SUBMITTEDATE1"].ToString(),
                    //ClearDetailId = Convert.ToInt32(row["CLEARDETAILID"]),
                    //ResignationId = row["RESIGNATIONID"].ToString(),
                    //ClearanceHeaderId1 = row["CLEARENCEHEADERID_1"].ToString(),
                    //HeaderDesc = row["HEADERDECS"].ToString(),
                    //HeadCode = row["HEADCODE"].ToString(),
                    //HsubDate = row["HSUBDATE"].ToString(),
                    //HRRemarks = row["HREMARK"].ToString(),
                    //ClearanceHeaderId12 = Convert.ToInt32(row["CLEARENCEHEADERID_2"]),
                    //HsStatus = row["HSTATUS"].ToString(),
                    //AttachDocument = row["ATTACHDOCUMENT"].ToString(),
                    HeaderDetailsChildModel = new List<ManageClearanceHeaderChildViewModel>()
                };

                DataSet childDs = _Separation.FULLRESIGNDETAIL_GET(resignId, "", subHeader.ClearanceHeaderId.ToString());

                foreach (DataRow childRow in childDs.Tables[1].Rows)
                {
                    subHeader.HeaderDetailsChildModel.Add(new ManageClearanceHeaderChildViewModel
                    {
                        Description = childRow["DESCRIPTION"].ToString(),
                        Amount = Convert.ToDecimal(childRow["AMOUNT"]),
                        Remarks = childRow["REMARKS"].ToString(),
                        SubmitBy = childRow["SUBMITBY"].ToString(),
                        SubmitDate = childRow["SUBMITTEDATE"].ToString()
                    });
                }

                list.Add(subHeader);
            }

            return list;
        }
        private DeptClearanceFormViewModel get_deptclrform()
        {
            DeptClearanceFormViewModel modelDptClr = new DeptClearanceFormViewModel();
            DataTable odt = _Separation.Get_DeptclrviewHistoryDetails(_sessionService.Get<string>("userID"), Request.Query["ResignID"].ToString());
            if (odt.Rows.Count > 0)
            {
                modelDptClr.lbl_confdoc = odt.Rows[0]["CONFIDENTIAL_DOC"].ToString();
                modelDptClr.lbl_otherdoc = odt.Rows[0]["OTHER_DOC"].ToString();
                modelDptClr.lbl_book = odt.Rows[0]["LIBRARY_BOOK"].ToString();
                modelDptClr.lbl_itasset = odt.Rows[0]["ITASSETS"].ToString();
                modelDptClr.lbl_camera = odt.Rows[0]["CAMERA"].ToString();
                modelDptClr.lbl_key = odt.Rows[0]["KEY"].ToString();
                modelDptClr.lbl_travel = odt.Rows[0]["TRAVEL_BILL"].ToString();
            }
            return modelDptClr;
        }
        private DeptClearanceFormViewModel get_deptclrformmyresign()
        {
            DeptClearanceFormViewModel modelDptClrResig = new DeptClearanceFormViewModel();
            DataTable odt = _Separation.Get_DeptclrviewHistoryDetails("", Request.Query["ResignID"].ToString());
            if (odt.Rows.Count > 0)
            {
                modelDptClrResig.lbl_confdoc = odt.Rows[0]["CONFIDENTIAL_DOC"].ToString();
                modelDptClrResig.lbl_otherdoc = odt.Rows[0]["OTHER_DOC"].ToString();
                modelDptClrResig.lbl_book = odt.Rows[0]["LIBRARY_BOOK"].ToString();
                modelDptClrResig.lbl_itasset = odt.Rows[0]["ITASSETS"].ToString();
                modelDptClrResig.lbl_camera = odt.Rows[0]["CAMERA"].ToString();
                modelDptClrResig.lbl_key = odt.Rows[0]["KEY"].ToString();
                modelDptClrResig.lbl_travel = odt.Rows[0]["TRAVEL_BILL"].ToString();
            }
            return modelDptClrResig;
        }
        private List<ApprovalDetailsModel> FillApprovalDetails()
        {

            DataTable dt = new DataTable();
            DataView objdtview = new DataView();
            DataSet ds = new DataSet();
            string strresigid = Request.Query["ResignID"].ToString();
            dt = _Separation.HRSPApprovalDetail(_sessionService.Get<string>("userID"), strresigid);
            dt.DefaultView.RowFilter = "APPSTATUS=1 OR APPSTATUS=2";
            objdtview = dt.DefaultView;
            ds.Tables.Add(objdtview.ToTable());
            var list = new List<ApprovalDetailsModel>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new ApprovalDetailsModel
                {
                    AppName = row["APPNAME"].ToString(),
                    AppECode = row["APPECODE"].ToString(),
                    AppRemark = row["APPREMARK"].ToString(),
                    AppStatus = Convert.ToInt32(row["APPSTATUS"]),
                    Status = row["STATUS"].ToString(),
                    AppSubmittedDate = row["APPSUBMITEDATE"].ToString(),
                    Designation = row["DESIG"].ToString(),
                    OrderNo = Convert.ToInt32(row["ORDERNO"])
                });
            }
            return list;
        }

        //--------------ExitInterviewQuestionnaire--------------
        [HttpGet]
        public ActionResult ExitInterviewQuestionnaire()
        {

            var modelIntQuest = getexitinterviewquestion();
            var modelEmpDtls = getexitintempdetails();


            ExitInterviewQuestionnaireModel model = new ExitInterviewQuestionnaireModel
            {
                ExitIntQuestModel = modelIntQuest,
                EmpDetailsIntQuestModel = modelEmpDtls
            };
            return View("ExitInterviewQuestionnaire", model);
        }
        private List<ExitIntQuestModel> getexitinterviewquestion()
        {
            DataSet ds = _Separation.GET_EXITINTERVIEW_QUESTIONNAIRE("1", "1", "1", "");

            DataTable questionTable = ds.Tables[0];
            DataTable optionTable = ds.Tables[1];

            var sections = questionTable.AsEnumerable()
                .GroupBy(row => row["SECTIONID"].ToString())
                .Select(group =>
                {
                    var sectionId = group.Key;
                    var sectionTitle = optionTable.AsEnumerable()
                        .Where(r => r["SECTIONID"].ToString() == sectionId)
                        .Select(r => r["SECTIONDESC"].ToString())
                        .FirstOrDefault() ?? "Unknown Section";

                    return new ExitIntQuestModel
                    {
                        SectionId = sectionId,
                        SectionTitle = sectionTitle,
                        Questions = group.Select(q => new IntQuestModel
                        {
                            ExitInterviewQusId = q["exitinerviewqusid"].ToString(),
                            QusType = q["QUSTYPE"].ToString(),
                            SectionId = sectionId,
                            QusDesc = q["qusdesc"].ToString(),
                            Options = optionTable.AsEnumerable()
                                .Where(opt => opt["exitinerviewqusid"].ToString() == q["exitinerviewqusid"].ToString())
                                .Select(opt => new OptionIntQuestModel
                                {
                                    ExitInterviewOptionId = opt["EXITINERVIEWOPTIONID"].ToString(),
                                    OptionDesc = opt["optiondesc"].ToString()
                                }).ToList()
                        }).ToList()
                    };
                }).ToList();


            return sections;
        }
        private EmpDetailsIntQuestModel getexitintempdetails()
        {
            EmpDetailsIntQuestModel modelEmpDtls = new EmpDetailsIntQuestModel();
            DataTable dt = _Separation.HRDDetail(_sessionService.Get<string>("userID"));
            if (dt.Rows.Count > 0)
            {
                modelEmpDtls.lbl_name = dt.Rows[0]["ENAME"].ToString();
                modelEmpDtls.lbl_ecode = dt.Rows[0]["ECODE"].ToString();
                modelEmpDtls.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                modelEmpDtls.lbl_designation = dt.Rows[0]["DESIGNATION"].ToString();
                modelEmpDtls.lbl_since = dt.Rows[0]["doj"].ToString();
                modelEmpDtls.lbl_loc = dt.Rows[0]["sitedesc"].ToString();
                modelEmpDtls.lbl_supdesign = dt.Rows[0]["FNDESIG"].ToString();
                modelEmpDtls.lbl_supname = dt.Rows[0]["SUPNAME"].ToString() + "[" + dt.Rows[0]["SUPERVISOREMPCODE"].ToString() + "]";
                modelEmpDtls.lbl_doj = dt.Rows[0]["DOJ"].ToString();
                modelEmpDtls.lbl_lastdateemp = dt.Rows[0]["relieving_date"].ToString();
                modelEmpDtls.lbl_EXINTDATE = dt.Rows[0]["EXITINTERVIEWDATE"].ToString();
                modelEmpDtls.lbl_age = dt.Rows[0]["AGE"].ToString();
                modelEmpDtls.lbl_qualification = dt.Rows[0]["QUALIFICATION"].ToString();
            }
            return modelEmpDtls;
        }
        [HttpPost]
        public ActionResult insertinterviewanswer(ExitInterviewQuestionnaireModel model, string qualification)
        {
            try
            {
                if (model.ExitIntAnsModel == null || !model.ExitIntAnsModel.Any())
                {
                    return Json(new { success = false, message = "No answers received" });
                }

                var xml = new System.Text.StringBuilder();
                xml.Append("<INTERVIEW>");
                foreach (var section in model.ExitIntAnsModel)
                {
                    foreach (var q in section.Questions)
                    {
                        xml.Append("<I>");
                        xml.Append("<Q>" + q.ExitInterviewQusId + "</Q>");
                        xml.Append("<O>" + (q.SelectedOption ?? string.Join(",", q.SelectedOptions)) + "</O>");
                        xml.Append("<T>" + (q.TextAns ?? "") + "</T>");
                        xml.Append("<QT>" + q.QusType + "</QT>");
                        xml.Append("</I>");
                    }
                }
                xml.Append("</INTERVIEW>");

                string interviewXml = xml.ToString();
                string submitBy = _sessionService.Get<string>("userID").ToString();
                //string qualification = qualification;

                string msg = _Separation.INSERT_INTERVIEW_ANSWER(_sessionService.Get<string>("ResigID"), submitBy, interviewXml, qualification);
                var cmdarg = msg.Split('#');

                if (cmdarg[0] == "1")
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = cmdarg[1] });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //--------------------FormI--------------------------------
        [HttpGet]
        public ActionResult FormI()
        {

            ViewBag.hdn_REGID_ECODE = _sessionService.Get<string>("ResigID");

            var empDtls = getemployeedetails();
            if (empDtls == null)
            {
                return RedirectToAction("ManageResignationRequest", "Separation");
            }
            else
            {
                FormIModel model = new FormIModel
                {
                    FormIEmpDtlsViewModel = empDtls
                };

                return View("FormI", model);
            }

        }
        private FormIEmpDtlsViewModel getemployeedetails()
        {
            try
            {
                var Userid = _sessionService.Get<string>("userID").ToString();
                var regId = _sessionService.Get<string>("ResigID");
                if (!string.IsNullOrEmpty(regId))
                {
                    FormIEmpDtlsViewModel modelEmpDtls = new FormIEmpDtlsViewModel();
                    DataTable dt = _Separation.HRFornIDetail(Userid, regId);
                    if (dt.Rows.Count > 0)
                    {
                        modelEmpDtls.lbl_resignationdate = dt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();
                        modelEmpDtls.lbl_assname = dt.Rows[0]["EMPNAME"].ToString();
                        modelEmpDtls.lbl_assecode = dt.Rows[0]["ADEMPCODE"].ToString();
                        modelEmpDtls.lbl_daddress = dt.Rows[0]["ADDRESS"].ToString();
                        modelEmpDtls.lbl_org = dt.Rows[0]["ORG"].ToString();
                        modelEmpDtls.lbl_ecodedesg = dt.Rows[0]["ADEMPCODE"].ToString() + "/" + dt.Rows[0]["DESCRIP"].ToString();
                        modelEmpDtls.lbl_joindate = dt.Rows[0]["REGDATE"].ToString();
                        modelEmpDtls.lbl_Relivingdate = dt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();
                        modelEmpDtls.lbl_serviceperiod = dt.Rows[0]["yrs"].ToString() + " Year " + dt.Rows[0]["mnths"].ToString() + " Month " + dt.Rows[0]["dys"].ToString() + " Days";
                        modelEmpDtls.lbl_basicamt = dt.Rows[0]["BASICSALARYAMT"].ToString();
                        modelEmpDtls.lbl_gratuityamt = dt.Rows[0]["gratuityamt"].ToString();
                    }
                    return modelEmpDtls;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult SubmitFormI(FormISubmitViewModel model)
        {
            string errResult = string.Empty;
            string errMsg = string.Empty;

            if (string.IsNullOrEmpty(model.EmailId))
            {
                return Json(new { success = false, message = "Email ID is mandatory field" });
            }

            if (!model.Agree)
            {
                return Json(new { success = false, message = "Terms and Conditions is mandatory field" });
            }

            string strErrMsg = _Separation.INSERT_FORMI(
                "",
                model.ResigID,
                model.BasicAmt,
                model.GratuityAmt,
                "1",
                _sessionService.Get<string>("userID").ToString(),
                model.EmailId,
                model.RelievingDate
            );

            string[] ResigStatus = strErrMsg.Split(new Char[] { '#' });
            errResult = Convert.ToString(ResigStatus[0]);
            errMsg = Convert.ToString(ResigStatus[1]);


            if (errResult == "1")
            {
                return Json(new { success = true, redirectUrl = Url.Action("MyResignationRequest", "Separation") });
            }
            else
            {
                return Json(new { success = false, message = errMsg });
            }
        }

        //----------CancellationResignation-----------------
        [HttpGet]
        public ActionResult CancellationResignation()
        {

            if (_sessionService.Get<string>("userID") != null && _sessionService.Get<string>("ResigID") != null)
            {
                ViewBag.ResigID = _sessionService.Get<string>("ResigID");
                var fillEmpDtls = FillEmpDetails();

                CancellationResignationModel model = new CancellationResignationModel
                {
                    CanResigFillEmpDtlsViewModel = fillEmpDtls
                };

                return View("CancellationResignation", model);
            }
            else
            {
                return RedirectToAction("ManageResignationRequest", "Separation");
            }
        }
        private CanResigFillEmpDtlsViewModel FillEmpDetails()
        {
            CanResigFillEmpDtlsViewModel model = new CanResigFillEmpDtlsViewModel();
            DataTable dt = new DataTable();
            string strresigid = _sessionService.Get<string>("ResigID");
            string strdptrelievingdat = string.Empty;
            string strprocessstatus = string.Empty;
            dt = _Separation.HRSPResigDetail("", strresigid);
            if (dt.Rows.Count > 0)
            {
                model.lbl_assname = dt.Rows[0]["EMPNAME"].ToString();
                model.lbl_assecode = dt.Rows[0]["ADEMPCODE"].ToString();
                model.lbl_joindate = dt.Rows[0]["REGDATE"].ToString();
                model.lbl_reldate = dt.Rows[0]["RELIEVING_DATE_SELF"].ToString();
                model.lbl_subject = dt.Rows[0]["RESIG_SUBJECT"].ToString();
                model.lbl_reason = dt.Rows[0]["RESIG_REASON"].ToString();

                strdptrelievingdat = dt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();
                strprocessstatus = dt.Rows[0]["RESIG_PROCESSSTATUS"].ToString();
                model.lbl_desg = dt.Rows[0]["DESCRIP"].ToString();
                model.lbl_fundesg = dt.Rows[0]["FNDESIG"].ToString();
                model.lbl_op = dt.Rows[0]["OPERATION"].ToString();
                model.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                model.lbl_div = dt.Rows[0]["DIVISION"].ToString();
                model.lbl_sec = dt.Rows[0]["SECTION"].ToString();
                model.lbl_site = dt.Rows[0]["sitedesc"].ToString();
            }
            return model;
        }
        [HttpPost]
        public JsonResult SubmitCancellationResignation(string Rmks, string ResigID)
        {
            string strErrMsg = string.Empty;
            string errResult = string.Empty;
            string errMsg = string.Empty;
            string RESIGNID = ResigID;
            string remarks = Rmks;

            if (string.IsNullOrEmpty(remarks))
            {
                return Json(new { success = false, message = "Enter Remarks" });
            }

            strErrMsg = _Separation.CANCELLATION_RESIGNATION(RESIGNID, remarks);
            string[] MSG = strErrMsg.Split(new Char[] { '#' });
            errResult = Convert.ToString(MSG[0]);
            errMsg = Convert.ToString(MSG[1]);

            if (errResult == "1")
            {
                SENDMAILONAUHTORITY(RESIGNID);
                //---------------------------------------------------------------------------------------------------------
                //Changes Start(Aumento) on 27-Oct-2023--------------------------------------
                //---------------------------------------------------------------------------------------------------------
                SendMailCancellation(RESIGNID);
                //---------------------------------------------------------------------------------------------------------
                //Changes End(Aumento) on 27-Oct-2023--------------------------------------
                //---------------------------------------------------------------------------------------------------------
                return Json(new { success = true, redirectUrl = Url.Action("MyResignationRequest", "Separation") });
            }
            else
            {
                return Json(new { success = false, message = errMsg });
            }
        }
        private void SENDMAILONAUHTORITY(string RESIGNID)
        {
            string TOMAIL = string.Empty;
            string CCEMAIL = "";
            string ENAME = string.Empty;
            string DHEMAIL = string.Empty;
            string DIVHEMAIL = string.Empty;
            string OHEMAIL = string.Empty;
            DataSet DS = _Separation.APPROVALAUTHDETAIL(RESIGNID);
            if (DS.Tables[0].Rows.Count > 0)
            {
                CCEMAIL = DS.Tables[0].Rows[0]["EMPEMAIL"].ToString();
                ENAME = DS.Tables[0].Rows[0]["ENAME"].ToString();
                DHEMAIL = DS.Tables[0].Rows[0]["DHEMAIL"].ToString();
                DIVHEMAIL = DS.Tables[0].Rows[0]["DIVHEMAIL"].ToString();
                OHEMAIL = DS.Tables[0].Rows[0]["OHEMAIL"].ToString();
                if (DHEMAIL != "")
                {
                    TOMAIL = DHEMAIL;
                }
                if (DIVHEMAIL != "")
                {
                    TOMAIL = TOMAIL + "," + DIVHEMAIL;
                }
                if (OHEMAIL != "")
                {
                    TOMAIL = TOMAIL + "," + OHEMAIL;
                }

                if (DS.Tables[1].Rows.Count > 0)
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                    sendMail.MailTo = TOMAIL;
                    sendMail.MailCc = CCEMAIL;
                    string strSubject = "withdrawal Resignation";
                    string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;line-height:26px;background-color:skyblue;'><b>&nbsp;&nbsp;Resignation Withdrawal</b></div>"
                                    + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                    + "<tr><td colspan=2><br/>&nbsp;&nbsp;Dear San,<br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                    + "<tr><td colspan=2><div style='text-align:justify;margin-left:10px;margin-right:10px;'>" + ENAME + " San has withdrawn his/her resignation,details are given below<div></td></tr><tr><td colspan=2>&nbsp;</td></tr></table>";


                    strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:10px;margin-right:10px;font-family:arial;' cellspacing='0' cellpadding='2' border='1'>"
                   + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Associate Name</td>"
                   + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + DS.Tables[1].Rows[0]["ENAME"].ToString() + "</td>"
                   + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Associate Ecode</td>"
                   + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + DS.Tables[1].Rows[0]["ADEMPCODE"].ToString() + "</td></tr>"
                   + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Relieving Date</td>"
                   + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + DS.Tables[1].Rows[0]["RELIEVING_DATE_AUTH"].ToString() + "</td>"
                   + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Withdraw Date</td>"
                   + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + DS.Tables[1].Rows[0]["WITHDRAW_DATE"].ToString() + "</td></tr>"
                   + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Resignation Reason</td>"
                   + "<td width='520' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:justify;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;line-height:15px;padding:5px;' colspan='3'>" + DS.Tables[1].Rows[0]["RESIG_REASON"].ToString() + "</td></tr>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Withdraw Remarks</td>"
                   + "<td width='520' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:justify;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;line-height:15px;padding:5px;' colspan='3'>" + DS.Tables[1].Rows[0]["WITHDRAW_REMARKS"].ToString() + "</td></tr></table><br/>";
                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><b>&nbsp;&nbsp;Thank you<br/><br/>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team E-Separation<br/><br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</tr></table></td></tr><tr><td></td></tr><tr> " +
                    "</tr></table></div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
            }
        }
        private void SendMailCancellation(string RESIGNID)
        {
            string strcode = _sessionService.Get<string>("userID");
            string GETEMAILTYPE = "Withdraw Resignation";
            string strresigid = RESIGNID;

            DataTable rdt = _Separation.HRSPResigDetail("", strresigid);
            if (rdt.Rows.Count > 0)
            {
                if (rdt.Rows[0]["HRHEAD_ECODE"].ToString() != "" && rdt.Rows[0]["HRHEAD_STATUS"].ToString() != "0")//---[Code Added by Aumento on 15-Dec-2023]
                {
                    string NAME = string.Empty,
                     ECODE = string.Empty,
                     DESIGNATION = string.Empty,
                     DEPT = string.Empty,
                     DATEOFRESIGN = string.Empty,
                     RECBYHR = string.Empty,
                     WITHDRAW_DATE = string.Empty,
                     OPNAME = string.Empty,
                     DIVNAME = string.Empty,
                     SECNAME = string.Empty,
                     FUNDESIGNATION = string.Empty;


                    NAME = rdt.Rows[0]["EMPNAME"].ToString();
                    ECODE = rdt.Rows[0]["ADEMPCODE"].ToString();
                    DESIGNATION = rdt.Rows[0]["descrip"].ToString();
                    FUNDESIGNATION = rdt.Rows[0]["FUNCTIONALDESIGNATION"].ToString();
                    OPNAME = rdt.Rows[0]["OPERATION"].ToString();
                    DIVNAME = rdt.Rows[0]["DIVISION"].ToString();
                    DEPT = rdt.Rows[0]["DEPARTMENT"].ToString();
                    SECNAME = rdt.Rows[0]["SECTION"].ToString();
                    DATEOFRESIGN = rdt.Rows[0]["RESIGNED_DATE"].ToString();
                    RECBYHR = rdt.Rows[0]["HRHEAD_SUBMITDATE"].ToString();
                    WITHDRAW_DATE = rdt.Rows[0]["WITHDRAW_DATE"].ToString();


                    DataTable dt = _Separation.GET_DEPRTMENTAPPEMAILIDS("", GETEMAILTYPE, strcode);

                    if (dt.Rows.Count > 0)
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                        sendMail.MailTo = Check_N_Filter_Email(dt.Rows[0]["EMAIL"].ToString());
                        sendMail.MailCc = Check_N_Filter_Email(dt.Rows[1]["EMAIL"].ToString());
                        string strSubject = "Resignation Withdraw on " + DateTime.Now.ToString("D");

                        string strBody = " <div style='height: 30px; width: 700px'><div style=' background-color: skyblue;padding-top: 5px'><b>&nbsp;&nbsp;Withdrawal Resignation</b></div><div style='border: 3px solid skyblue; width: 700px'>"
                                       + "<table cellpadding=0 cellspacing=0 border=0  style='width: 100%;font-size: 14px ;'>" +
                                  "<tr>" +
                                    "<td colspan=2  height=35></td>" +
                                  "</tr>" +
                                  "<tr><td colspan=2>&nbsp;&nbsp;Dear All,<br/></td></tr>" +
                                  "<tr><td colspan=2>&nbsp;&nbsp;Please find the detail of the associate who has withdrawn his/her resignation today.</td></tr></table><br/><br/>" +
                                  "<table style='font-size: 14px;' >" +
                                  "<tr><td style='width: 25%;' ><b>&nbsp;&nbsp;Name:-</b></td><td style='width: 25%;'>" + NAME + "</td>" +
                                  "<td style='width: 25%;'><b>&nbsp;&nbsp;Employee code:-</b></td><td style='width: 25%;'>" + ECODE + "</td></tr>" +
                                  "<tr><td style='width: 25%;'><b>&nbsp;&nbsp;Operation:-</b></td><td style='width: 25%;'>" + OPNAME + "</td>" +
                                  "<td style='width: 25%;'><b>&nbsp;&nbsp;Division:-</b></td><td style='width: 25%;'>" + DIVNAME + "</td></tr>" +
                                  "<tr><td style='width: 25%;'><b>&nbsp;&nbsp;Department:-</b></td><td style='width: 25%;'>" + DEPT + "</td>" +
                                  "<td style='width: 25%;'><b>&nbsp;&nbsp;Section:-</b></td><td style='width: 25%;'>" + SECNAME + "</td></tr>" +
                                 "<tr><td style='width: 25%;'><b>&nbsp;&nbsp;Designation:-</b></td><td style='width: 25%;'>" + DESIGNATION + "</td>" +
                                 "<td style='width: 25%;'><b>&nbsp;&nbsp;Func. Designation:-</b></td><td style='width: 25%;'>" + FUNDESIGNATION + "</td></tr>" +
                                  "<tr><td style='width: 25%;'><b>&nbsp;&nbsp;Resignation Date:-</b></td><td style='width: 25%;'>" + DATEOFRESIGN + "</td>" +
                                  "<td style='width: 25%;'><b>&nbsp;&nbsp;Approved Date(HR):-</b></td><td style='width: 25%;'>" + RECBYHR + "</td></tr>" +
                                  "<tr><td ><b>&nbsp;&nbsp;Withdraw Date:-</b></td><td colspan=2>" + WITHDRAW_DATE + "</td></tr></table>" +
                                 "<br/><br/><p><b> &nbsp; &nbsp; Thank You</b></p><br/> " +
                                  "<div style='font-size: 12px ;font-family:Arial'><b>&nbsp;&nbsp;Best Regards</b><br/>" +
                                  "&nbsp;&nbsp;Team E-Separation<br/>" +
                                  "<b>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</b></div>" +
                                  "</div></div>";
                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        sendMail.Send();
                    }
                }
            }
        }
        public string Check_N_Filter_Email(string emailIds)
        {
            StringBuilder _FilteredEmail = new StringBuilder();
            string[] EmailIds = emailIds.Split(',');
            foreach (string Email in EmailIds)
            {
                if (Email.Contains("@honda.hmsi.in"))
                {
                    _FilteredEmail.Append(Email);
                    _FilteredEmail.Append(",");
                }
            }
            return _FilteredEmail.ToString().Remove(_FilteredEmail.Length - 1, 1);
        }

        //-------------EditAssociateResignationForm----------------
        [HttpGet]
        public ActionResult EditAssociateResignationForm()
        {

            ViewBag.ResigID = _sessionService.Get<string>("RESIGNID");
            var modelPopDateTime = PopulateDateTime();
            var modelEmpDtls = FillEmpDetailsEditAssoc();
            var modelAPPAUTHDetails = FillAPPAUTHDetails();
            var modelResigData = Get_Resignation_data(ViewBag.ResigID);

            EditAssociateResignationFormModel model = new EditAssociateResignationFormModel
            {
                DateSelectionViewModel = modelPopDateTime,
                EmpDtlsViewModel = modelEmpDtls,
                APPAUTHDetailsViewModel = modelAPPAUTHDetails,
                ResignationDataViewModel = modelResigData
            };

            return View("EditAssociateResignationForm", model);
        }
        public DateSelectionViewModel PopulateDateTime()
        {
            var model = new DateSelectionViewModel();

            // Days 1–31
            model.Days = Enumerable.Range(1, 31)
                .Select(d => new SelectListItem { Value = d.ToString(), Text = d.ToString() })
                .ToList();

            // Months 1–12
            model.Months = Enumerable.Range(1, 12)
                .Select(m => new SelectListItem
                {
                    Value = m.ToString(),
                    Text = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(m)
                })
                .ToList();

            // Years (current year -1 to +1)
            int currentYr = DateTime.Now.Year;
            model.Years = Enumerable.Range(currentYr - 1, 3)
                .Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() })
                .ToList();

            // Preselect current date
            model.SelectedDay = DateTime.Now.Day;
            model.SelectedMonth = DateTime.Now.Month;
            model.SelectedYear = DateTime.Now.Year;

            return model;
        }
        private EmpDtlsViewModel FillEmpDetailsEditAssoc()
        {
            EmpDtlsViewModel model = new EmpDtlsViewModel();
            DataTable dt = new DataTable();
            dt = _Separation.HRDDetail(_sessionService.Get<string>("userID"));
            if (dt.Rows.Count > 0)
            {
                model.lbl_assname = dt.Rows[0]["ENAME"].ToString();
                model.lbl_assecode = dt.Rows[0]["ECODE"].ToString();
                model.lbl_joindate = dt.Rows[0]["DOJ"].ToString();
                model.lbl_desg = dt.Rows[0]["DESIGNATION"].ToString();
                model.lbl_fundesg = dt.Rows[0]["FNDESIG"].ToString();
                model.lbl_op = dt.Rows[0]["OPERATION"].ToString();
                model.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                model.lbl_div = dt.Rows[0]["DIVISION"].ToString();
                model.lbl_sec = dt.Rows[0]["SECTION"].ToString();
                model.lbl_site = dt.Rows[0]["sitedesc"].ToString();

            }
            return model;
        }
        private APPAUTHDetailsViewModel FillAPPAUTHDetails()
        {
            APPAUTHDetailsViewModel model = new APPAUTHDetailsViewModel();
            DataTable dt = new DataTable();

            dt = _Separation.HRSPAPPROVAL(_sessionService.Get<string>("userID"), "0");
            if (dt.Rows.Count > 0)
            {
                model.lbl_appauth = dt.Rows[0]["empname"].ToString() + " [" + dt.Rows[0]["adempcode"].ToString() + "]";
                model.hdauthlevel = dt.Rows[0]["AUTHLEVEL"].ToString();
                model.hdauthcode = dt.Rows[0]["adempcode"].ToString();
            }
            return model;
        }
        private ResignationDataViewModel Get_Resignation_data(string RESIGNID)
        {
            ResignationDataViewModel model = new ResignationDataViewModel();
            DataTable dt = _Separation.Get_Resignation_data(RESIGNID);
            if (dt.Rows.Count > 0)
            {
                model.txt_subject = dt.Rows[0]["resig_subject"].ToString();
                model.txt_body = dt.Rows[0]["resig_reason"].ToString();
                model.txt_latestaddress = dt.Rows[0]["LATESTADDRESS"].ToString();
                string RELDATE = dt.Rows[0]["RELIEVING_DATE_SELF"].ToString();
                //string[] RDATE = RELDATE.Split(new Char[] { '/' });
                DateTime dt1 = DateTime.ParseExact(RELDATE, "dd-MM-yyyy HH:mm:ss", null);
                model.SelectedOptDay = dt1.Day.ToString();
                model.SelectedOptMonth = dt1.Month.ToString();
                model.SelectedOptYear = dt1.Year.ToString();
            }
            return model;
        }
        [HttpPost]
        public JsonResult SubmitEditAssociateResignationForm(SubmitEditAssocResigModel model)
        {
            string strErrMsg = string.Empty;
            string errResult = string.Empty;
            string errMsg = string.Empty;
            string strDay;
            string strMonth;
            string strYear;
            int vdatefrom = 0;
            int vdateto = 0;
            string vmonth = "";
            int DRVSTATUS = 0;
            int Cday = Convert.ToInt32(System.DateTime.Now.Day);
            int Cmonth = Convert.ToInt32(System.DateTime.Now.Month);
            int Cyear = Convert.ToInt32(System.DateTime.Now.Year);
            //DateTime cdate = Convert.ToDateTime(Cmonth + "/" + Cday + "/" + Cyear);
            DateTime cdate = new DateTime(Cyear, Cmonth, Cday);

            string strDate = model.SelectedMonth + "/" + model.SelectedDay + "/" + model.SelectedYear;
            DataTable vdt = _Separation.Get_DateRangeValidation(model.SelectedMonth.ToString());

            if (vdt.Rows.Count > 0)
            {
                vmonth = vdt.Rows[0]["MONTH"].ToString();
                vdatefrom = Convert.ToInt32(vdt.Rows[0]["DATERANGEFROM"]);
                vdateto = Convert.ToInt32(vdt.Rows[0]["DATERANGETO"]);
                DRVSTATUS = Convert.ToInt32(vdt.Rows[0]["STATUS"]);
            }
            //DateTime uselectdate = DateTime.Parse(strDate);
            DateTime uselectdate = DateTime.ParseExact(
                    strDate,
                    "M/d/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture
            );

            if (string.IsNullOrEmpty(model.Subject))
            {
                return Json(new { success = false, message = "Subject is mandatory field", focusId = "txtup", showErrorPanel = true });
            }
            else if (string.IsNullOrEmpty(model.Body))
            {
                return Json(new { success = false, message = "Body is mandatory field", focusId = "txtup" });
            }

            else if (uselectdate < cdate.AddDays(2))
            {
                return Json(new { success = false, message = "Relieving date should  not be less than current date+2", focusId = "txtup", showErrorPanel = true });
            }
            else if (model.hdauthcode == string.Empty)
            {
                return Json(new { success = false, message = "Approving authority is  mandatory field", focusId = "txtup", showErrorPanel = true });
            }
            else if (model.AgreeTerms == false)
            {
                return Json(new { success = false, message = "Please check I agree to Terms and Condition", focusId = "txtup", showErrorPanel = true });
            }
            else if (DRVSTATUS == 1)
            {
                if (Convert.ToInt32(model.SelectedDay) <= vdateto && Convert.ToInt32(model.SelectedDay) >= vdatefrom)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Relieving date should not be between {vdatefrom} {vmonth} to {vdateto} {vmonth}",
                        focusId = "txtup",
                        showErrorPanel = true
                    });
                }
                else
                {
                    var result = Updation(model);
                    if (result.success)
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("MyResignationRequest", "Separation") });
                    }
                    else
                    {
                        return Json(new { success = false, message = "", showErrorPanel = false });
                    }
                }
            }
            else
            {
                var result = Updation(model);
                if (result.success)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("MyResignationRequest", "Separation") });
                }
                else
                {
                    return Json(new { success = false, message = "", showErrorPanel = false });
                }
            }
        }
        private (bool success, string message) Updation(SubmitEditAssocResigModel model)
        {
            string strErrMsg = string.Empty;
            string errResult = string.Empty;
            string errMsg = string.Empty;
            string strDay;
            string strMonth;
            string strYear;
            //errorpanel.Visible = false;
            strDay = model.SelectedDay;
            strMonth = model.SelectedMonthText;
            strYear = model.SelectedYear;
            string releDate = strDay + "-" + strMonth + "-" + strYear;

            string RESIGNID = model.ResigID.ToString();
            strErrMsg = _Separation.UPDATE_RESIGNATION_DETAILS(RESIGNID, model.Subject, model.Body, releDate, model.latestaddress);
            string[] ResigStatus = strErrMsg.Split(new Char[] { '#' });
            errResult = Convert.ToString(ResigStatus[0]);
            errMsg = Convert.ToString(ResigStatus[1]);

            if (errResult == "1")
            {
                ViewBag.ResigID = null;
                ViewData["ResigID"] = null;
                return (true, "Resignation updated successfully");
            }
            return (false, errMsg);
        }

        //----------------------ManageResignationRequest-----------------------------
        [HttpGet]
        //public ActionResult ManageResignationRequest()//Commneted for : SR107848
        public ActionResult ManageResignationRequest(string txtSearch = "", int ddlPageSize = 10, int page = 1)//Added for : SR107848
        {
            //Start : SR107848
            if (HttpContext.Session.GetString("IsBackNavigation") == "true")
            {
                txtSearch = HttpContext.Session.GetString("SearchText") ?? txtSearch;
                ddlPageSize = HttpContext.Session.GetInt32("PageSize") ?? ddlPageSize;
                page = HttpContext.Session.GetInt32("CurrentPage") ?? page;
                HttpContext.Session.Remove("IsBackNavigation");
            }

            HttpContext.Session.SetString("SearchText", txtSearch ?? "");
            HttpContext.Session.SetInt32("PageSize", ddlPageSize);
            HttpContext.Session.SetInt32("CurrentPage", page);            
            var modelClearance = BindClearanceGrid( string.IsNullOrWhiteSpace(txtSearch) ? null : txtSearch, ddlPageSize);
            //var modelClearance = BindClearanceGrid();
            //End : SR107848
            var modelPendingApprv = FillPendingApproval();
            var modelClearanceHeader = GET_CLEARANCEHEADER_DETAILS();
            var modelDptClearance = GET_DPTCLEARANCE_DETAILS();
            var modelHideShowClearance = GET_HIDESHOW_CLEARANCELIST();
            ManageResignationRequestModel model = new ManageResignationRequestModel
            {
                ClearanceFormViewModel = modelClearance,
                PendingApprovalViewModel = modelPendingApprv,
                ClearHeaderViewModel = modelClearanceHeader,
                DeptClearanceViewModel = modelDptClearance,
                HideShowClearanceViewModel = modelHideShowClearance,
            };

            //Start : SR107848
            ViewBag.SearchText = txtSearch;
            ViewBag.PageSize = ddlPageSize;
            ViewBag.CurrentPage = page;
            //End : SR107848
            return View("ManageResignationRequest", model);
        }


        //Start : SR107848
        //private List<ClearanceFormViewModel> BindClearanceGrid()//Commneted for : SR107848
        private List<ClearanceFormViewModel> BindClearanceGrid(string searchText, int pageSize)//Added for : SR107848
        {
            //var clearanceList = new List<ClearanceFormViewModel>(); //Commneted for : SR107848
            var list = new List<ClearanceFormViewModel>(); //Added for : SR107848

            DataTable dt = _Separation.GET_MANAGECLRFORMDETAILS(_sessionService.Get<string>("userID"), "0");

            //Start : SR107848
            //foreach (DataRow row in dt.Rows)
            //{
            //    clearanceList.Add(new ClearanceFormViewModel
            //    {
            //        ADEMPCODE = row["ADEMPCODE"].ToString(),
            //        EMPNAME = row["EMPNAME"].ToString(),
            //        RELIEVING_DATE_AUTH = row["RELIEVING_DATE_AUTH"].ToString(),
            //        RESIGNED_DATE = row["RESIGNED_DATE"].ToString(),
            //        CLSTATUS = row["CLSTATUS"].ToString(),
            //        RESIGNATIONID = row["RESIGNATIONID"].ToString()
            //    });
            //}

            // return clearanceList;
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ClearanceFormViewModel
                {
                    ADEMPCODE = row["ADEMPCODE"].ToString(),
                    EMPNAME = row["EMPNAME"].ToString(),
                    RELIEVING_DATE_AUTH = row["RELIEVING_DATE_AUTH"].ToString(),
                    RESIGNED_DATE = row["RESIGNED_DATE"].ToString(),
                    CLSTATUS = row["CLSTATUS"].ToString(),
                    RESIGNATIONID = row["RESIGNATIONID"].ToString()
                });
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();

                list = list.Where(x =>
                    (
                        $"{x.ADEMPCODE} " +
                        $"{x.EMPNAME} " +
                        $"{x.RELIEVING_DATE_AUTH} " +
                        $"{x.RESIGNED_DATE} " +
                        $"{x.CLSTATUS} " +
                        $"{x.RESIGNATIONID}"
                    ).ToLower().Contains(searchText)
                ).ToList();
            }
           
            //return clearanceList;
            return list;
            //Start : SR107848
        }
        private List<PendingApprovalViewModel> FillPendingApproval()
        {
            var pendingApprvList = new List<PendingApprovalViewModel>();
            DataTable dt = _Separation.HRSPpendingApproval(_sessionService.Get<string>("userID"));

            foreach (DataRow row in dt.Rows)
            {
                pendingApprvList.Add(new PendingApprovalViewModel
                {
                    ADEMPCODE = row["ADEMPCODE"].ToString(),
                    EMPNAME = row["EMPNAME"].ToString(),
                    RELIEVING_DATE_SELF = row["RELIEVING_DATE_SELF"].ToString(),
                    RESIGNED_DATE = row["RESIGNED_DATE"].ToString(),
                    STATUS = row["STATUS"].ToString(),
                    RESIGNATIONID = row["RESIGNATIONID"].ToString()
                });
            }

            return pendingApprvList;

        }
        private List<ClearHeaderViewModel> GET_CLEARANCEHEADER_DETAILS()
        {
            var pendingApprvList = new List<ClearHeaderViewModel>();
            string strecode = _sessionService.Get<string>("userID");
            DataTable dt = _Separation.GET_MANAGECLEARANCEHEADERDETAILS(strecode);

            foreach (DataRow row in dt.Rows)
            {
                pendingApprvList.Add(new ClearHeaderViewModel
                {
                    Adempcode = row["Adempcode"].ToString(),
                    EMPNAME = row["EMPNAME"].ToString(),
                    RELIEVING_DATE_AUTH = row["RELIEVING_DATE_AUTH"].ToString(),
                    Resigned_Date = row["Resigned_Date"].ToString(),
                    status = row["status"].ToString(),
                    Resignationid = row["Resignationid"].ToString()
                });
            }

            return pendingApprvList;

        }
        private List<DeptClearanceViewModel> GET_DPTCLEARANCE_DETAILS()
        {
            var deptClearanceList = new List<DeptClearanceViewModel>();
            DataTable dt = _Separation.GET_MANAGEDPTCLRFORMDETAILS(_sessionService.Get<string>("userID"), "0");

            foreach (DataRow row in dt.Rows)
            {
                deptClearanceList.Add(new DeptClearanceViewModel
                {
                    ADEMPCODE = row["ADEMPCODE"].ToString(),
                    EMPNAME = row["EMPNAME"].ToString(),
                    RELIEVING_DATE_AUTH = row["RELIEVING_DATE_AUTH"].ToString(),
                    RESIGNED_DATE = row["RESIGNED_DATE"].ToString(),
                    DPTCLSTATUS = row["DPTCLSTATUS"].ToString(),
                    RESIGNATIONID = row["RESIGNATIONID"].ToString()
                });
            }

            return deptClearanceList;

        }
        private HideShowClearanceViewModel GET_HIDESHOW_CLEARANCELIST()
        {
            HideShowClearanceViewModel model = new HideShowClearanceViewModel();
            DataTable dt = new DataTable();
            string headerreq = _Separation.Get_SHOWCLRHEADREQUEST(_sessionService.Get<string>("userID"));
            if (headerreq == "1")
            {
                model.ShowHeaderList = true;
            }
            else
            {
                model.ShowHeaderList = false;
            }
            string subheaderreq = _Separation.Get_SHOWCLRSUBHEADREQUEST(_sessionService.Get<string>("userID"));
            if (subheaderreq == "1")
            {
                model.ShowSubHeaderList = true;
            }
            else
            {
                model.ShowSubHeaderList = false;
            }
            return model;
        }
        public IActionResult lnk_pendresignapp_Click()
        {
            HttpContext.Session.SetString("Resignationhistory", "pendresignapp");
            return RedirectToAction("ManageResignationHistory", "Separation");
        }
        public IActionResult ApprovePendingApproval(string resignationId)
        {
            HttpContext.Session.SetString("ResigID", resignationId);
            return RedirectToAction("ApprovalForm", "Separation");
        }
        public IActionResult lnk_pendclrform_Click()
        {
            HttpContext.Session.SetString("Resignationhistory", "pendclrform");
            return RedirectToAction("ManageResignationHistory", "Separation");
        }
        //public IActionResult EditRegPendClearForm(string resignationId, string empCode)
        public IActionResult EditRegPendClearForm(string resignationId, string empCode, int returnPage = 1) //Added (returnPage) : SR107848
        {
            //Start : SR107848
            // _sessionService.Set<string>("PendClearForm", resignationId + "&" + empCode);//Existing code that is chanhged for SR107848
            HttpContext.Session.SetString("PendClearForm", resignationId +"&" + empCode);
            HttpContext.Session.SetInt32("CurrentPage", returnPage);
            HttpContext.Session.SetString("IsBackNavigation", "true");
            //End : SR107848
            return RedirectToAction("ClearanceFullFinalform");
        }
        public IActionResult lnk_manageclrheader_Click()
        {
            _sessionService.Set<string>("Resignationhistory", "manageclrheader");
            return RedirectToAction("ManageResignationHistory", "Separation");
        }
        public IActionResult EditClearHeaderForm(string id)
        {
            _sessionService.Set<string>("RESIGNID", id);
            return RedirectToAction("ClearanceFullFinalHeader", "Separation");
        }
        public IActionResult lnk_deptclrform_Click()
        {
            _sessionService.Set<string>("Resignationhistory", "deptclrform");
            return RedirectToAction("ManageResignationHistory", "Separation");
        }
        public IActionResult EditDeptClearance(string id, string code)
        {
            _sessionService.Set<string>("REGID_&_ECODE", id + "&" + code);
            return RedirectToAction("SetDepartmentClearance", "Separation");
        }
       //Start : New Method For SR107848
        [HttpPost]
        public IActionResult GetAllData()
        {
            string userId = HttpContext.Session.GetString("userID");
            string search = HttpContext.Session.GetString("SearchText") ?? "";

            DataTable dt = _Separation.GET_EXPORTCLRFORMDETAIL(userId, "0");

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Replace("'", "''");

                string filter = $@"
                    Convert(ADEMPCODE, 'System.String') LIKE '%{search}%'
                    OR EMPNAME LIKE '%{search}%'
                    OR Convert(RESIGNATIONID, 'System.String') LIKE '%{search}%'
                    OR RESIG_SUBJECT LIKE '%{search}%'
                    OR RESIG_REASON LIKE '%{search}%'
                    OR RESIGNATION_STATUS LIKE '%{search}%'
                    OR REQUEST_STATUS LIKE '%{search}%'
                    OR RELIEVING_DATE_SELF LIKE '%{search}%'
                    OR RELIEVING_DATE_AUTH LIKE '%{search}%'
                    OR RESIGNED_DATE LIKE '%{search}%'
                ";


                DataView dv = dt.DefaultView;
                dv.RowFilter = filter;
                dt = dv.ToTable();
            }
            var rows = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                var row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row[col.ColumnName] = dr[col] == DBNull.Value ? "" : dr[col];
                }
                rows.Add(row);
            }
            return Json(rows);
        }
        //End :New Method For SR107848

        //-----------ManageResignationHistory----------------------
        [HttpGet]
        public ActionResult ManageResignationHistory()
        {

            if (_sessionService.Get<string>("Resignationhistory") == null)
            {
                return RedirectToAction("ManageResignationRequest");
            }

            var modelHistory = gethistory();
            return View("ManageResignationHistory", modelHistory);
        }
        private ManageResignationHistoryModel gethistory()
        {
            ManageResignationHistoryModel modelMraghistory = new ManageResignationHistoryModel();
            string hd_sessionhist = _sessionService.Get<string>("Resignationhistory");

            if (hd_sessionhist == "pendresignapp")
            {
                ViewBag.tbl_resigapp = true;
                var model = Get_pendresignappHistory();
                modelMraghistory = new ManageResignationHistoryModel
                {
                    PendingApprovalHistoryViewModel = model
                };

            }
            else if (hd_sessionhist == "pendclrform")
            {
                ViewBag.tbl_compclr = true;
                var model = Get_pendclearanceformHistory();
                modelMraghistory = new ManageResignationHistoryModel
                {
                    ClearanceFormHistoryViewModel = model
                };
            }
            else if (hd_sessionhist == "manageclrheader")
            {
                ViewBag.tbl_clrheader = true;
                var model = Get_manageclearanceheaderHistory();
                modelMraghistory = new ManageResignationHistoryModel
                {
                    ClearHeaderHistoryViewModel = model
                };
            }
            else if (hd_sessionhist == "deptclrform")
            {
                ViewBag.tbl_completedclr = true;
                var model = Get_deptclearanceformHistory();
                modelMraghistory = new ManageResignationHistoryModel
                {
                    DeptClearanceHistoryViewModel = model
                };
            }
            return modelMraghistory;
        }
        private List<PendingApprovalHistoryViewModel> Get_pendresignappHistory()
        {
            var pendingApprvList = new List<PendingApprovalHistoryViewModel>();
            DataTable dt = _Separation.Get_ResgApprovalHistory(_sessionService.Get<string>("userID"));

            foreach (DataRow row in dt.Rows)
            {
                pendingApprvList.Add(new PendingApprovalHistoryViewModel
                {
                    ADEMPCODE = row["ADEMPCODE"].ToString(),
                    EMPNAME = row["EMPNAME"].ToString(),
                    RELIEVING_DATE_SELF = row["RELIEVING_DATE_SELF"].ToString(),
                    RESIGNED_DATE = row["RESIGNED_DATE"].ToString(),
                    RESIGNATIONID = row["RESIGNATIONID"].ToString()
                });
            }

            return pendingApprvList;

        }
        private List<ClearanceFormHistoryViewModel> Get_pendclearanceformHistory()
        {
            var clearanceList = new List<ClearanceFormHistoryViewModel>();
            DataTable dt = _Separation.GET_ResgCLRFORMHistory(_sessionService.Get<string>("userID"), "1");

            foreach (DataRow row in dt.Rows)
            {
                clearanceList.Add(new ClearanceFormHistoryViewModel
                {
                    ADEMPCODE = row["ADEMPCODE"].ToString(),
                    EMPNAME = row["EMPNAME"].ToString(),
                    RELIEVING_DATE_AUTH = row["RELIEVING_DATE_AUTH"].ToString(),
                    RESIGNED_DATE = row["RESIGNED_DATE"].ToString(),
                    //STATUS = row["Status"].ToString(),
                    RESIGNATIONID = row["RESIGNATIONID"].ToString(),
                    //REMARKS = row["REMARKS"].ToString(),
                });
            }

            return clearanceList;

        }
        private List<ClearHeaderHistoryViewModel> Get_manageclearanceheaderHistory()
        {
            var pendingApprvList = new List<ClearHeaderHistoryViewModel>();
            DataTable dt = _Separation.GET_ResgCLRHEADEHistory(_sessionService.Get<string>("userID"));

            foreach (DataRow row in dt.Rows)
            {
                pendingApprvList.Add(new ClearHeaderHistoryViewModel
                {
                    Adempcode = row["Adempcode"].ToString(),
                    EMPNAME = row["EMPNAME"].ToString(),
                    RELIEVING_DATE_AUTH = row["RELIEVING_DATE_AUTH"].ToString(),
                    Resigned_Date = row["Resigned_Date"].ToString(),
                    status = row["status"].ToString(),
                    Resignationid = row["Resignationid"].ToString()
                });
            }

            return pendingApprvList;

        }
        private List<DeptClearanceHistoryViewModel> Get_deptclearanceformHistory()
        {
            var deptClearanceList = new List<DeptClearanceHistoryViewModel>();
            DataTable dt = _Separation.GET_rESGDPTCLRFORMHistory(_sessionService.Get<string>("userID"), "1");

            foreach (DataRow row in dt.Rows)
            {
                deptClearanceList.Add(new DeptClearanceHistoryViewModel
                {
                    ADEMPCODE = row["ADEMPCODE"].ToString(),
                    EMPNAME = row["EMPNAME"].ToString(),
                    RELIEVING_DATE_AUTH = row["RELIEVING_DATE_AUTH"].ToString(),
                    RESIGNED_DATE = row["RESIGNED_DATE"].ToString(),
                    DPTCLSTATUS = row["DPTCLSTATUS"].ToString(),
                    RESIGNATIONID = row["RESIGNATIONID"].ToString()
                });
            }

            return deptClearanceList;

        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            // Example DataTable (replace with your own data fetch)
            DataTable dt = _Separation.ExcelExport(_sessionService.Get<string>("userID"), "1");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // Load DataTable into worksheet
                worksheet.Cell(1, 1).InsertTable(dt);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    var fileName = $"ManageRegistrationHistory_{DateTime.Now:yyyyMMdd}.xlsx";
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }

        //--------------ApprovalForm-----------------------------
        [HttpGet]
        public ActionResult ApprovalForm()
        {

            if (_sessionService.Get<string>("ResigID") == null)
            {
                return RedirectToAction("ManageResignationRequest");
            }
            else
            {
                ViewBag.ResigID = _sessionService.Get<string>("ResigID");
                var modelPopDateTime = PopulateDateTimeApprvForm();
                var modelEmpDtls = FillEmpDetails(_sessionService.Get<string>("ResigID"));
                var ApprovalDtls = FillApprovalDetails(_sessionService.Get<string>("ResigID"));

                ApprovalFormModel model = new ApprovalFormModel
                {
                    ApprvFrmDateTimeViewModel = modelPopDateTime,
                    ApprvFormEmpDetailsViewModel = modelEmpDtls,
                    ApprvFormApprovalDtlsViewModel = ApprovalDtls
                };

                return View("ApprovalForm", model);
            }
        }
        public ApprvFrmDateTimeViewModel PopulateDateTimeApprvForm()
        {
            var model = new ApprvFrmDateTimeViewModel();

            model.Days = Enumerable.Range(1, 31)
                .Select(d => new SelectListItem { Value = d.ToString(), Text = d.ToString() })
                .ToList();


            model.Months = Enumerable.Range(1, 12)
                .Select(m => new SelectListItem
                {
                    Value = m.ToString(),
                    Text = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(m)
                })
                .ToList();

            int currentYr = DateTime.Now.Year;
            model.Years = Enumerable.Range(currentYr - 1, 22) 
                          .Select(y => new SelectListItem
                          {
                              Value = y.ToString(),
                              Text = y.ToString()
                          }).ToList();

            return model;
        }
        private ApprvFormEmpDetailsViewModel FillEmpDetails(string strresigid)
        {
            var model = new ApprvFormEmpDetailsViewModel();
            DataTable dt = _Separation.HRSPResigDetail("", strresigid);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];

                model.lbl_assname = row["EMPNAME"].ToString();
                model.lbl_assecode = row["ADEMPCODE"].ToString();
                model.lbl_joindate = row["REGDATE"].ToString();
                model.lbl_desg = row["DESCRIP"].ToString();
                model.lbl_fundesg = row["FNDESIG"].ToString();
                model.lbl_phone = row["TMOBILE"].ToString();
                model.lbl_emailid = row["EMAILID"].ToString();
                model.lbl_op = row["OPERATION"].ToString();
                model.lbl_div = row["DIVISION"].ToString();
                model.lbl_dept = row["DEPARTMENT"].ToString();
                model.lbl_sec = row["SECTION"].ToString();
                model.lbl_site = row["sitedesc"].ToString();
                model.lbl_applydate = row["RESIGNED_DATE"].ToString();
                model.lbl_reldate = row["RELIEVING_DATE_SELF"].ToString();
                model.lbl_deptreldate = row["RELIEVING_DATE_AUTH"].ToString();
                model.lbl_subject = row["RESIG_SUBJECT"].ToString();
                model.lbl_reason = row["RESIG_REASON"].ToString();
                model.lblResignType = row["RESIGNTYPE"].ToString();

                string strdptrelievingdat = row["RELIEVING_DATE_AUTH"].ToString();
                string strprocessstatus = row["RESIG_PROCESSSTATUS"].ToString();
                string deptreldate = row["DPT_RELIEVING_DATE"].ToString();
                string strrel = row["RELIEVING_DATE"].ToString();

                string[] reldate = strrel.Split(new Char[] { '-' });

                if (!string.IsNullOrEmpty(deptreldate))
                {
                    string[] strdeptreldate = deptreldate.Split('-');
                    model.SelectedoptDay = Convert.ToInt32(strdeptreldate[0]).ToString();
                    model.SelectedoptMonth = Convert.ToInt32(strdeptreldate[1]).ToString();
                    model.SelectedoptYear = strdeptreldate[2];
                }
                else
                {
                    model.SelectedoptDay = Convert.ToInt32(reldate[0]).ToString();
                    model.SelectedoptMonth = Convert.ToInt32(reldate[1]).ToString();
                    model.SelectedoptYear = reldate[2];
                }

                model.ApprvFormFillAPPAUTHDetails= FillAPPAUTHDetails(model.lbl_assecode, strprocessstatus, strresigid);

                model.RelievingDate = strdptrelievingdat;
            }

            return model;
        }
        private ApprvFormFillAPPAUTHDetails FillAPPAUTHDetails(string struser, string strprocessstatus,string ResigID)
        {
            ApprvFormFillAPPAUTHDetails model = new ApprvFormFillAPPAUTHDetails();
            DataTable dt = new DataTable();

            dt = _Separation.HRSPAPPROVAL(struser, strprocessstatus);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["empname"].ToString() == "HR")
                {
                    //lbl_appauth.Text = dt.Rows[0]["empname"].ToString();
                    //separation change
                    bool isBlueCaller = false; //Company casual, staff, line associate etc
                    DataTable dtEmp = new DataTable();
                    string strresigid = ResigID;
                    dtEmp = _Separation.HRSPResigDetail(struser, strresigid);
                    if (dtEmp.Rows.Count > 0)
                    {
                        DataTable dtDesg = _Separation.GET_SYPARAMETERS_PARAMVALUE("DESG_STAFFHOMEPAGE_VALIDATION");
                        if (dtDesg.Rows.Count > 0)
                        {
                            string[] desg = dtDesg.Rows[0]["PARAMVALUE"].ToString().Split(',');
                            for (int i1 = 0; i1 <= desg.Length - 1; i1++)
                            {
                                if (desg[i1].ToString() == dtEmp.Rows[0]["ADDESIGNATIONID"].ToString())
                                {
                                    isBlueCaller = true;
                                    break;
                                }
                            }
                        }
                    }
                    model.lbl_appauth = isBlueCaller == true ? dt.Rows[0]["empname"].ToString().Replace("HR", "IR") : dt.Rows[0]["empname"].ToString();
                    //separation change
                }
                else
                {
                    model.lbl_appauth = dt.Rows[0]["empname"].ToString() + " [" + dt.Rows[0]["adempcode"].ToString() + "]";
                }
                model.hdauthlevel = dt.Rows[0]["AUTHLEVEL"].ToString();
                model.hdauthcode = dt.Rows[0]["adempcode"].ToString();
                model.hdauthemailid = dt.Rows[0]["emailid"].ToString();
                model.hdauthname = dt.Rows[0]["empname"].ToString();
            }
            return model;
        }
        private List<ApprvFormApprovalDtlsViewModel> FillApprovalDetails(string strresigid)
        {
            var approvals = new List<ApprvFormApprovalDtlsViewModel>();
            DataTable dt = new DataTable();
            DataView objdtview = new DataView();
            DataSet ds = new DataSet();
            dt = _Separation.HRSPApprovalDetail(_sessionService.Get<string>("userID"), strresigid);
            dt.DefaultView.RowFilter = "APPSTATUS=1 OR APPSTATUS=2";
            objdtview = dt.DefaultView;
            ds.Tables.Add(objdtview.ToTable());
           
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                approvals.Add(new ApprvFormApprovalDtlsViewModel
                {
                    APPNAME = row["APPNAME"].ToString(),
                    APPECODE = row["APPECODE"].ToString(),
                    DESIG = row["DESIG"].ToString(),
                    STATUS = row["STATUS"].ToString(),
                    APPSUBMITEDATE = row["APPSUBMITEDATE"].ToString(),
                    APPREMARK = row["APPREMARK"].ToString()
                });
            }
            return approvals;
        }
        [HttpPost]
        public JsonResult SubmitApprovalForm(SubmitApprovalFormViewModel model)
        {
            string strErrMsg = string.Empty;
            string errResult = string.Empty;
            string errMsg = string.Empty;
            string releDate = string.Empty;
            string strDay;
            string strMonth;
            string strYear;
            int vdatefrom = 0;
            int vdateto = 0;
            string vmonth = "";
            int DRVSTATUS = 0;
            int iMonthNo = 0;

            strDay = model.SelectedDay;
            strMonth = model.SelectedMonthText;
            strYear = model.SelectedYear;
            releDate = strDay + "/" + strMonth + "/" + strYear;
            iMonthNo = Convert.ToDateTime(releDate).Month;

            DataTable vdt = _Separation.Get_DateRangeValidation(iMonthNo.ToString());
            if (vdt.Rows.Count > 0)
            {
                vmonth = vdt.Rows[0]["MONTH"].ToString();
                vdatefrom = Convert.ToInt32(vdt.Rows[0]["DATERANGEFROM"]);
                vdateto = Convert.ToInt32(vdt.Rows[0]["DATERANGETO"]);
                DRVSTATUS = Convert.ToInt32(vdt.Rows[0]["STATUS"]);
            }

            DateTime authselectdate = DateTime.Parse(releDate);
            // DateTime ureledate = DateTime.Parse(lbl_reldate.Text);
            int Cday = Convert.ToInt32(System.DateTime.Now.Day);
            int Cmonth = Convert.ToInt32(System.DateTime.Now.Month);
            int Cyear = Convert.ToInt32(System.DateTime.Now.Year);
            //DateTime cdate = Convert.ToDateTime(Cmonth + "-" + Cday + "-" + Cyear);
            DateTime cdate = new DateTime(Cyear, Cmonth, Cday);

            //if (authselectdate < cdate.AddDays(2)) //Separation change
            if (authselectdate < cdate.AddDays(2) && model.ResignType == "0") //Separation change
            {
                return Json(new { success = false, message = "Relieving date should  not be less than current date+2", focusId = "txtup", showErrorPanel = true });
            }
            //Separation change start
            else if (authselectdate < cdate.AddDays(-60) && model.ResignType == "1")
            {
                return Json(new { success = false, message = "Relieving date should not be less than 60 days", focusId = "txtup", showErrorPanel = true });
            }
            //Separation change end
            else if (string.IsNullOrEmpty(model.Remarks))
            {
                return Json(new { success = false, message = "Remark  is mandatory field", focusId = "txtup", showErrorPanel = true });
            }
            else if (DRVSTATUS == 1)
            {
                if (Convert.ToInt32(model.SelectedDay) <= vdateto && Convert.ToInt32(model.SelectedDay) >= vdatefrom)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Relieving date should not be between {vdatefrom} {vmonth} to {vdateto} {vmonth}",
                        focusId = "txtup",
                        showErrorPanel = true
                    });
                }
                else
                {
                    var modelAppResigned= ApprovedResigned(model,releDate);
                    if (modelAppResigned.success == false)
                    {
                        return Json(new { success = false, message = modelAppResigned.message, focusId = "txtup", showErrorPanel = true });
                    }
                    else
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("ManageResignationRequest", "Separation"), showErrorPanel = false });
                    }
                }
            }
            else
            {
                var modelAppResigned = ApprovedResigned(model,releDate);
                if (modelAppResigned.success == false)
                {
                    return Json(new { success = false, message = modelAppResigned.message, focusId = "txtup", showErrorPanel = true });
                }
                else
                {
                    return Json(new { success = true, redirectUrl = Url.Action("ManageResignationRequest", "Separation"), showErrorPanel = false });
                }
            }
        }
        private (bool success, string message) ApprovedResigned(SubmitApprovalFormViewModel model,string releDate)
        {
            string strErrMsg = string.Empty;
            string errResult = string.Empty;
            string errMsg = string.Empty;
            strErrMsg = _Separation.UpdateApproval_Set(model.ResigID, releDate,model.RadiobtnAppAtatus.ToString(), model.Remarks, model.AuthCode, model.AuthLevel, _sessionService.Get<string>("userID"));
            string[] ResigStatus = strErrMsg.Split(new Char[] { '#' });
            errResult = Convert.ToString(ResigStatus[0]);
            errMsg = Convert.ToString(ResigStatus[1]);

            if (errResult == "1")
            {
                if (model.AppAuth != "HR" && model.AppAuth != "IR") //separation change
                {
                    mailsend(model);
                }
                else if (model.AppAuth == "HR")
                {
                    SendMailHR(model);
                }
                //separation change
                else if (model.AppAuth == "IR")
                {
                    SendMailIR(model);
                }
                //separation change
                return (true, "ManageResignationRequest");
            }
            else
            {
                return (false, errMsg);
            }
        }
        public void SendMailHR(SubmitApprovalFormViewModel model)
        {
            string userName = _sessionService.Get<string>("userName");
            string EmailsId = string.Empty;
            DataTable dtmail = new DataTable();
            dtmail = _Separation.Get_HRMailIdResignation(model.ResigID);
            foreach (DataRow dr in dtmail.Rows)
            {
                EmailsId = EmailsId + dr["EMAILID"].ToString() + ",";
            }
            string FinalEmailID = EmailsId.Remove(EmailsId.Length - 1, 1);

            if (!string.IsNullOrEmpty(FinalEmailID))
            {
                if (model.RadiobtnAppAtatus == "1")
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                    sendMail.MailTo = FinalEmailID;
                    string strSubject = "Pending for the HR Approval";
                    string strBody = "<div style='width: 700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width: 700px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Resignation Approval</b></div>"
                        + "<table cellpadding=0 cellspacing=0 border=0 width='700px' >"
                        + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear HR Head,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                        + "<tr><td colspan=2>&nbsp;&nbsp;Resignation Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr><tr><td colspan=2>&nbsp;&nbsp;Details as follows :-</td></tr></table><br/>"
                        + "<b>&nbsp;&nbsp;Associate Name:-</b> " + model.AssName + "[" + model.AsseCode + "]<br/>"
                        + "<b>&nbsp;&nbsp;Relieving Date:-</b> " + model.RelDate + "<br/>"
                        + "<b>&nbsp;&nbsp;Reason:-</b> " + model.Reason;
                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 width='700px'><tr><td><br/> &nbsp;&nbsp;Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                    "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team E-Separation<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</td></tr></table></div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
                else if (model.RadiobtnAppAtatus == "2")
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                    sendMail.MailTo = model.Emailid;
                    sendMail.MailCc = FinalEmailID;
                    string strSubject = " Resignation is kept on hold by " + userName;
                    string strBody = "<div style='width: 700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width: 700px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Resignation Approval</b></div>"
                        + "<table cellpadding=0 cellspacing=0 border=0 width='700px' >"
                        + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + model.AssName + " San,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                        + "<tr><td colspan=2>&nbsp;&nbsp;Your resignation request is kept on hold by approving authority due to following reason.</td></tr></table><br/>"
                        + "<b>&nbsp;&nbsp;Reason:-</b> " + model.Remarks + "<br/>";

                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 width='700px'><tr><td><br/>&nbsp;&nbsp;Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for further details." +
                    "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team E-Separation<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</tr></table></td> " +
                    "</tr></table></div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
            }
        }
        //Separation change
        public void SendMailIR(SubmitApprovalFormViewModel model)
        {
            string userName = _sessionService.Get<string>("userName");
            string EmailsId = string.Empty;
            DataTable dtmail = new DataTable();
            dtmail = _Separation.Get_IRMailIdResignation(model.ResigID);
            foreach (DataRow dr in dtmail.Rows)
            {
                EmailsId = EmailsId + dr["EMAILID"].ToString() + ",";
            }
            string FinalEmailID = EmailsId.Remove(EmailsId.Length - 1, 1);

            if (!string.IsNullOrEmpty(FinalEmailID))
            {
                if (model.RadiobtnAppAtatus == "1")
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                    sendMail.MailTo = FinalEmailID;
                    string strSubject = "Pending for the IR Approval";
                    string strBody = "<div style='width: 700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width: 700px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Resignation Approval</b></div>"
                        + "<table cellpadding=0 cellspacing=0 border=0 width='700px' >"
                        + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear IR Head,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                        + "<tr><td colspan=2>&nbsp;&nbsp;Resignation Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr><tr><td colspan=2>&nbsp;&nbsp;Details as follows :-</td></tr></table><br/>"
                        + "<b>&nbsp;&nbsp;Associate Name:-</b> " + model.AssName + "[" + model.AsseCode + "]<br/>"
                        + "<b>&nbsp;&nbsp;Relieving Date:-</b> " + model.RelDate + "<br/>"
                        + "<b>&nbsp;&nbsp;Reason:-</b> " + model.Reason;
                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 width='700px'><tr><td><br/> &nbsp;&nbsp;Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                    "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team E-Separation<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</td></tr></table></div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
                else if (model.RadiobtnAppAtatus == "2")
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                    sendMail.MailTo = model.Emailid;
                    sendMail.MailCc = FinalEmailID;
                    string strSubject = " Resignation is kept on hold by " + userName;
                    string strBody = "<div style='width: 700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width: 700px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Resignation Approval</b></div>"
                        + "<table cellpadding=0 cellspacing=0 border=0 width='700px' >"
                        + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + model.AssName + " San,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                        + "<tr><td colspan=2>&nbsp;&nbsp;Your resignation request is kept on hold by approving authority due to following reason.</td></tr></table><br/>"
                        + "<b>&nbsp;&nbsp;Reason:-</b> " + model.Remarks + "<br/>";

                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 width='700px'><tr><td><br/>&nbsp;&nbsp;Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for further details." +
                    "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team E-Separation<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</tr></table></td> " +
                    "</tr></table></div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
            }
        }
        //private void SendMailApproval()
        //{
        //    string strcode = Session["userid"].ToString();
        //    string GETEMAILTYPE = "APPROVELETTER";
        //    string strresigid = Session["ResigID"].ToString();
        //    DataTable dt = objsp.GET_DEPRTMENTAPPEMAILIDS("", GETEMAILTYPE, strcode);

        //    if (dt.Rows.Count > 0)
        //    {

        //        string NAME = string.Empty,
        //               ECODE = string.Empty,
        //               DESIGNATION = string.Empty,
        //               DEPT = string.Empty,
        //               DATEOFRESIGN = string.Empty,
        //               RECBYHR = string.Empty,
        //               APPDATEOFREL = string.Empty,
        //               OPNAME = string.Empty,
        //               DIVNAME = string.Empty,
        //               SECNAME = string.Empty,
        //               FUNDESIGNATION = string.Empty;


        //        DataTable rdt = objsp.HRSPResigDetail("", strresigid);
        //        if (rdt.Rows.Count > 0)
        //        {
        //            NAME = rdt.Rows[0]["EMPNAME"].ToString();
        //            ECODE = rdt.Rows[0]["ADEMPCODE"].ToString();
        //            DESIGNATION = rdt.Rows[0]["descrip"].ToString();
        //            FUNDESIGNATION = rdt.Rows[0]["FUNCTIONALDESIGNATION"].ToString();
        //            OPNAME = rdt.Rows[0]["OPERATION"].ToString();
        //            DIVNAME = rdt.Rows[0]["DIVISION"].ToString();
        //            DEPT = rdt.Rows[0]["DEPARTMENT"].ToString();
        //            SECNAME = rdt.Rows[0]["SECTION"].ToString();
        //            DATEOFRESIGN = rdt.Rows[0]["RESIGNED_DATE"].ToString();
        //            RECBYHR = rdt.Rows[0]["HRHEAD_SUBMITDATE"].ToString();
        //            APPDATEOFREL = rdt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();
        //        }

        //        commanEmail sendMail = new commanEmail();
        //        sendMail.MailFrom = "E-Separation@honda.hmsi.in";
        //        sendMail.MailTo = Check_N_Filter_Email(dt.Rows[0]["EMAIL"].ToString());
        //        sendMail.MailCc = Check_N_Filter_Email(dt.Rows[1]["EMAIL"].ToString());
        //        string strSubject = "Resignation Received on " + DateTime.Now.ToString("D");

        //        string strBody = " <div style='height: 30px; width: 700px'><div style=' background-color: skyblue;padding-top: 5px'><b>&nbsp;&nbsp;Resignation Details</b></div><div style='border: 3px solid skyblue; width: 700px'>"
        //                       + "<table cellpadding=0 cellspacing=0 border=0  style='width: 100%;font-size: 14px ;'>" +
        //                  "<tr>" +
        //                    "<td colspan=2  height=35></td>" +
        //                  "</tr>" +
        //                  "<tr><td colspan=2>&nbsp;&nbsp;Dear All,<br/></td></tr>" +
        //                  "<tr><td colspan=2>&nbsp;&nbsp;Please find the detail of the resignation received today for your kind information.</td></tr></table><br/><br/>" +
        //                  "<table style='font-size: 14px;' >" +
        //                  "<tr><td style='width: 25%;' ><b>&nbsp;&nbsp;Name:-</b></td><td style='width: 25%;'>" + NAME + "</td>" +
        //                  "<td style='width: 25%;'><b>&nbsp;&nbsp;Employee code:-</b></td><td style='width: 25%;'>" + ECODE + "</td></tr>" +
        //                  "<tr><td style='width: 25%;'><b>&nbsp;&nbsp;Operation:-</b></td><td style='width: 25%;'>" + OPNAME + "</td>" +
        //                  "<td style='width: 25%;'><b>&nbsp;&nbsp;Division:-</b></td><td style='width: 25%;'>" + DIVNAME + "</td></tr>" +
        //                  "<tr><td style='width: 25%;'><b>&nbsp;&nbsp;Department:-</b></td><td style='width: 25%;'>" + DEPT + "</td>" +
        //                  "<td style='width: 25%;'><b>&nbsp;&nbsp;Section:-</b></td><td style='width: 25%;'>" + SECNAME + "</td></tr>" +
        //                 "<tr><td style='width: 25%;'><b>&nbsp;&nbsp;Designation:-</b></td><td style='width: 25%;'>" + DESIGNATION + "</td>" +
        //                 "<td style='width: 25%;'><b>&nbsp;&nbsp;Func. Designation:-</b></td><td style='width: 25%;'>" + FUNDESIGNATION + "</td></tr>" +
        //                  "<tr><td style='width: 25%;'><b>&nbsp;&nbsp;Resignation Date:-</b></td><td style='width: 25%;'>" + DATEOFRESIGN + "</td>" +
        //                  "<td style='width: 25%;'><b>&nbsp;&nbsp;Approved Date(HR):-</b></td><td style='width: 25%;'>" + RECBYHR + "</td></tr>" +
        //                  "<tr><td ><b>&nbsp;&nbsp;Final Relieving Date:-</b></td><td colspan=2>" + APPDATEOFREL + "</td></tr>" +
        //                   "<tr><td colspan=2 ><b>&nbsp;&nbsp;<br/>&nbsp;&nbsp;Thank You<br/><br/></td><td>&nbsp;</td></tr>" +
        //                   "<tr><td><b>&nbsp;&nbsp;Best Regards</td><td>&nbsp;</td></tr>" +
        //                   "<tr><td>&nbsp;&nbsp;Team E-Separation</td><td>&nbsp;</td></tr>" +
        //                   "<tr><td><b>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</td><td>&nbsp;</td></tr>" +
        //                  "</table>";
        //        sendMail.MailSubject = strSubject;
        //        sendMail.MailBody = strBody;
        //        sendMail.Send();

        //    }
        //}

        //public string Check_N_Filter_Email(string emailIds)
        //{
        //    StringBuilder _FilteredEmail = new StringBuilder();
        //    string[] EmailIds = emailIds.Split(',');
        //    foreach (string Email in EmailIds)
        //    {
        //        if (Email.Contains("@honda.hmsi.in"))
        //        {
        //            _FilteredEmail.Append(Email);
        //            _FilteredEmail.Append(",");
        //        }
        //    }
        //    return _FilteredEmail.ToString().Remove(_FilteredEmail.Length - 1, 1);
        //}

       
        //separation change
        private void mailsend(SubmitApprovalFormViewModel model)
        {
            string userName = _sessionService.Get<string>("userName");
            string EmailsId = string.Empty;
            DataTable dtmail = new DataTable();
            dtmail = _Separation.Get_HRMailIdResignation(model.ResigID);
            foreach (DataRow dr in dtmail.Rows)
            {
                EmailsId = EmailsId + dr["EMAILID"].ToString() + ",";
            }
            string FinalEmailID = EmailsId.Remove(EmailsId.Length - 1, 1);

            if (model.RadiobtnAppAtatus == "1")
            {
                if (!string.IsNullOrEmpty(model.AuthMailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                    sendMail.MailTo = model.AuthMailId;
                    string strSubject = "Pending for the Resignation Approval";
                    string strBody = "<div style='width: 700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width: 700px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Resignation Approval</b></div>"
                        + "<table cellpadding=0 cellspacing=0 border=0 width='700px' >"
                        + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + model.AuthName + " San,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                        + "<tr><td colspan=2>&nbsp;&nbsp;Resignation Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr><tr><td colspan=2>&nbsp;&nbsp;Details as follows :-</td></tr></table><br/>"
                        + "<b>&nbsp;&nbsp;Associate Name:-</b> " + model.AssName + "[" + model.AsseCode + "]<br/>"
                        + "<b>&nbsp;&nbsp;Relieving Date:-</b> " + model.RelDate + "<br/>"
                        + "<b>&nbsp;&nbsp;Reason:-</b> " + model.Reason;

                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 width='700px'><tr><td><br/>&nbsp;&nbsp;Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                    "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team E-Separation<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</tr></table></td> " +
                    "</tr></table></div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }

            }
            else if (model.RadiobtnAppAtatus == "2")
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                sendMail.MailTo = model.Emailid;
                sendMail.MailCc = model.AuthMailId + "," + FinalEmailID;
                string strSubject = " Resignation is kept on hold by " + userName;
                string strBody = "<div style='width: 700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width: 700px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Resignation Approval</b></div>"
                    + "<table cellpadding=0 cellspacing=0 border=0 width='700px' >"
                    + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + model.AssName + " San,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                    + "<tr><td colspan=2>&nbsp;&nbsp;Your resignation request is kept on hold by approving authority due to following reason.</td></tr></table><br/>"
                    + "<b>&nbsp;&nbsp;Reason:-</b> " + model.Remarks + "<br/>";

                strBody = strBody +
                "<table cellpadding=0 cellspacing=0 border=0 width='700px'><tr><td><br/>&nbsp;&nbsp;Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for further details." +
                "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team E-Separation<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                "</tr></table></td> " +
                "</tr></table></div> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
        }

        //--------------------ClearanceFullFinalform-------------------------------
        [HttpGet]
        public ActionResult ClearanceFullFinalform()
        {

            if (_sessionService.Get<string>("PendClearForm") != null)
            {
                string regID = _sessionService.Get<string>("PendClearForm").ToString().Split('&')[0];
                string Ecode = _sessionService.Get<string>("PendClearForm").ToString().Split('&')[1];

                var modelEmpDtls = getemployeedetails(regID, Ecode);
                var modelResigClrDtls = getresignclearancedetails(regID);

                ClearanceFullFinalformModel model = new ClearanceFullFinalformModel
                {
                    ClearFullFinalEmpDtlsViewModel = modelEmpDtls,
                    ClearanceDetailViewModel = modelResigClrDtls
                };

                return View("ClearanceFullFinalform", model);
            }
            else
            {
                return RedirectToAction("ManageResignationRequest", "Separation");
            }


        }
        private ClearFullFinalEmpDtlsViewModel getemployeedetails(string regID, string Ecode)
        {
            ClearFullFinalEmpDtlsViewModel model = new ClearFullFinalEmpDtlsViewModel();
            DataTable odt = _Separation.HRSPResigDetail(Ecode, regID);
            if (odt.Rows.Count > 0)
            {
                model.lbl_assname = odt.Rows[0]["EMPNAME"].ToString();
                model.lbl_assecode = odt.Rows[0]["ADEMPCODE"].ToString();
                model.lbl_joindate = odt.Rows[0]["REGDATE"].ToString();
                model.lbl_desg = odt.Rows[0]["DESCRIP"].ToString();
                model.lbl_fundesg = odt.Rows[0]["FNDESIG"].ToString();
                model.lbl_phone = odt.Rows[0]["TMOBILE"].ToString();
                model.lbl_emailid = odt.Rows[0]["EMAILID"].ToString();
                model.lbl_op = odt.Rows[0]["OPERATION"].ToString();
                model.lbl_div = odt.Rows[0]["DIVISION"].ToString();
                model.lbl_dept = odt.Rows[0]["DEPARTMENT"].ToString();
                model.lbl_sec = odt.Rows[0]["SECTION"].ToString();
                model.lbl_site = odt.Rows[0]["sitedesc"].ToString();
                model.lbl_applydate = odt.Rows[0]["RESIGNED_DATE"].ToString();
                model.lbl_reldate = odt.Rows[0]["RELIEVING_DATE_SELF"].ToString();
                model.lbl_deptreldate = odt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();

                model.lbl_basicsal = odt.Rows[0]["BASICSALARYAMT"].ToString();
            }
            return model;
        }
        private List<ClearanceDetailViewModel> getresignclearancedetails(string regID)
        {
            List<ClearanceDetailViewModel> details = new List<ClearanceDetailViewModel>();
            DataTable odt = _Separation.GET_SUBHEADERCLRDETAILS(_sessionService.Get<string>("userID"), regID);

            foreach (DataRow row in odt.Rows)
            {
                details.Add(new ClearanceDetailViewModel
                {
                    CLEARENCEHEADERID =row["CLEARENCEHEADERID"].ToString(),
                    DESCRIPTION = row["DESCRIPTION"].ToString(),
                    AMOUNT = row["AMOUNT"].ToString(),
                    REMARKS = row["REMARKS"].ToString(),
                    STATUS = Convert.ToInt32(row["STATUS"]),
                    ATTACHDOCUMENT = row["ATTACHDOCUMENT"].ToString()
                });
            }

            return details;
        }
        [HttpPost]
        public JsonResult SubmitDetails(List<ClearanceDetailViewModel> model, string lbl_emailid, string lbl_assname)
        {
            string headerdesc = string.Empty;
            string attachdoc = string.Empty;
            string fullMpath = string.Empty;
            var xmlList = new StringBuilder();
            //var headerDesc = new StringBuilder();
            int j = 0;
            string filePath = serverpath.getFileUploadPath() + "Separation";

            foreach (var item in model)
            {
                if (string.IsNullOrEmpty(item.REMARKS))
                {
                    return Json(new { success = false, message = "Remarks column is mandatory", showErrorPanel = true });
                }

                if (item.FileUpload != null && item.FileUpload.Length > 0)
                {
                    var ext = System.IO.Path.GetExtension(item.FileUpload.FileName).ToLower();
                    if (ext != ".pdf" && ext != ".docx" && ext != ".doc")
                    {
                        return Json(new { success = false, message = "Cannot upload agreement file because the file format or extension is invalid please select a valid file (doc or pdf)');", showErrorPanel = true });
                    }
                    if (item.FileUpload.Length >= 20480000)
                    {
                        return Json(new { success = false, message = "Unable to upload,file exceeds maximum limit", showErrorPanel = true });
                    }
                }
            }

            xmlList.Append("<HEADER>");

            foreach (var item in model)
            {
                string attachDoc = string.Empty;

                if (item.STATUS == 0)
                {
                    headerdesc = headerdesc + "<tr><td>&nbsp;<b>" + item.DESCRIPTION + " : </b> " + item.REMARKS + "</td></tr>";
                    j++;
                }

                if (item.FileUpload != null && item.STATUS == 1)
                {
                    var fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{item.FileUpload.FileName}";
                    var fullPath = System.IO.Path.Combine(filePath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        item.FileUpload.CopyTo(stream);
                    }
                    attachDoc = fileName;
                }
                else
                {
                    attachDoc = item.ATTACHDOCUMENT;
                }

                xmlList.Append("<SUBHEADER>");
                xmlList.Append($"<SUBHEADERID>{item.CLEARENCEHEADERID}</SUBHEADERID>");
                xmlList.Append($"<AMOUNT>{item.AMOUNT}</AMOUNT>");
                xmlList.Append($"<REMARKS>{System.Net.WebUtility.HtmlEncode(item.REMARKS)}</REMARKS>");
                xmlList.Append($"<STATUS>{item.STATUS}</STATUS>");
                xmlList.Append($"<DOC>{attachDoc}</DOC>");
                xmlList.Append("</SUBHEADER>");
            }

            xmlList.Append("</HEADER>");

            string regID = _sessionService.Get<string>("PendClearForm").ToString().Split('&')[0];
            string Ecode = _sessionService.Get<string>("PendClearForm").ToString().Split('&')[1];

            string SUBHEADERLIST = Convert.ToString(xmlList);
            string SUBMITBY = _sessionService.Get<string>("userID");
            string ecode = Ecode;
            string ResigID = regID;
            string msg = _Separation.INSERT_CLRANCESUBHEADERDETAILS(SUBMITBY, ResigID, SUBHEADERLIST);
            string[] cmdarg = msg.ToString().Split(new char[] { '#' });
            if (cmdarg[0].ToString() == "1")
            {
                if (j > 0)
                {
                    string a = headerdesc.Remove(headerdesc.Length - 1, 1);
                    mailsend(a, lbl_emailid, lbl_assname);
                }
                return Json(new { success = true, redirectUrl = Url.Action("ManageResignationRequest", "Separation"), showErrorPanel = false });
            }
            else
            {
                return Json(new { success = false, Message = cmdarg[1].ToString(), showErrorPanel = true });
            }
        }
        private void mailsend(string headerdesc, string lbl_emailid, string lbl_assname)
        {
            if (!string.IsNullOrEmpty(lbl_emailid))
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                sendMail.MailTo = lbl_emailid;
                string userName = _sessionService.Get<string>("userName");
                string strSubject = "Clearance form is pending";
                string strBody = "<div style='width: 700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width: 700px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Resignation Clearance Detail</b></div>"
                    + "<table cellpadding=0 cellspacing=0 border=0 width='700px' >"
                    + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + lbl_assname + " San,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                    + "<tr><td colspan=2>&nbsp;&nbsp;Your Clearance form is pending due to some document requirement.Details are given below:</td></tr><tr><td colspan=2>&nbsp;&nbsp;<table>" + headerdesc + "</table></td></tr></table><br/>"
                    + "<tr><td colspan=2>&nbsp;&nbsp;Kindly Contact " + userName + "San<br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>";

                strBody = strBody +
                "<table cellpadding=0 cellspacing=0 border=0 width='700px'>" +
                "<tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp; Team E-Separation<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                "</tr></table></td> " +
                "</tr></table></div> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
        }

        //----------------ClearanceFullFinalHeader-----------------------------------------
        [HttpGet]
        public ActionResult ClearanceFullFinalHeader()
        {
            
            var modelEmpDtls = getemployeedtls();
            var modelHeader = getsubgeaderdetails();

            ClearanceFullFinalHeaderModel model = new ClearanceFullFinalHeaderModel
            {
                ClrFullFinalHeaderEmpDtlsViewModel = modelEmpDtls,
                ClrFullFinalHeaderViewModel = modelHeader
            };

            return View("ClearanceFullFinalHeader", model);
        }
        private ClrFullFinalHeaderEmpDtlsViewModel getemployeedtls()
        {
            ClrFullFinalHeaderEmpDtlsViewModel model = new ClrFullFinalHeaderEmpDtlsViewModel();
            DataTable dt = new DataTable();
            string strresigid = _sessionService.Get<string>("RESIGNID");
            dt = _Separation.HRSPResigDetail("", strresigid);
            if (dt.Rows.Count > 0)
            {
                model.lbl_assname = dt.Rows[0]["EMPNAME"].ToString();
                model.lbl_assecode = dt.Rows[0]["ADEMPCODE"].ToString();
                model.lbl_joindate = dt.Rows[0]["REGDATE"].ToString();
                model.lbl_desg = dt.Rows[0]["DESCRIP"].ToString();
                model.lbl_fundesg = dt.Rows[0]["FNDESIG"].ToString();
                model.lbl_phone = dt.Rows[0]["TMOBILE"].ToString();
                model.lbl_emailid = dt.Rows[0]["EMAILID"].ToString();
                model.lbl_op = dt.Rows[0]["OPERATION"].ToString();
                model.lbl_div = dt.Rows[0]["DIVISION"].ToString();
                model.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                model.lbl_sec = dt.Rows[0]["SECTION"].ToString();
                model.lbl_site = dt.Rows[0]["sitedesc"].ToString();
                model.lbl_applydate = dt.Rows[0]["RESIGNED_DATE"].ToString();
                model.lbl_reldate = dt.Rows[0]["RELIEVING_DATE_SELF"].ToString();
                model.lbl_deptreldate = dt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();
                model.lbl_subject = dt.Rows[0]["RESIG_SUBJECT"].ToString();
                model.lbl_reason = dt.Rows[0]["RESIG_REASON"].ToString();
            }
            return model;
        }
        private List<ClrFullFinalHeaderViewModel> getsubgeaderdetails()
        {
            string RESIGNID = _sessionService.Get<string>("RESIGNID");
            string ecode = _sessionService.Get<string>("userID");
            DataSet ds = _Separation.FULLRESIGNDETAIL_GET(RESIGNID, ecode, "");

            var headers = ds.Tables[0].AsEnumerable().Select(h =>
            {
                var detailDs = _Separation.FULLRESIGNDETAIL_GET(RESIGNID, "", h["CLEARENCEHEADERID"].ToString());

                var subHeaders = detailDs.Tables[1].AsEnumerable().Select(s => new ClrFullFinalSubHeaderViewModel
                {
                    DESCRIPTION = s["DESCRIPTION"].ToString(),
                    Amount = string.IsNullOrEmpty(s["Amount"].ToString()) ? 0 : Convert.ToDecimal(s["Amount"]),
                    REMARKS = s["REMARKS"].ToString(),
                    SUBMITBY = s["SUBMITBY"].ToString(),
                    SUBMITTEDATE = s["SUBMITTEDATE"].ToString(),
                    STATUS = s["STATUS"].ToString(),
                    ATTACHDOCUMENT = s["ATTACHDOCUMENT"].ToString()
                }).ToList();

                return new ClrFullFinalHeaderViewModel
                {
                    CLEARENCEHEADERID = Convert.ToInt32(h["CLEARENCEHEADERID"]),
                    DESCRIPTION = h["DESCRIPTION"].ToString(),
                    CLEAHEADRDETAILID = Convert.ToInt32(h["CLEAHEADRDETAILID"]),
                    SubHeaderDetails = subHeaders,
                    CanSubmit = !subHeaders.Any(s => s.STATUS == "0"),
                    status = h.Table.Columns.Contains("STATUS") ? h["STATUS"].ToString() : "",
                    remarks = h.Table.Columns.Contains("REMARKS") ? h["REMARKS"].ToString() : ""
                };
            }).ToList();

            return headers;
        }
        [HttpPost]
        public JsonResult SubmitClrFullFinalHeader(string detailId, string remarks)
        {
            if (string.IsNullOrEmpty(remarks))
            {
                return Json(new { success = false, message = "Enter Remarks", showErrorPanel = false });
            }
            else
            {
                string REMARKS = remarks.Trim();
                string detailid = detailId;
                string msg = _Separation.UPDATE_SUBHEADERREMARKS(REMARKS, detailid, "1");
                string[] cmdarg = msg.ToString().Split(new char[] { '#' });
                if (cmdarg[0].ToString() == "1")
                {
                    return Json(new { success = true, message = "Successfully Submitted", showErrorPanel = false });
                }
                else
                {
                    return Json(new { success = false, message = cmdarg[1].ToString(), showErrorPanel = false });
                }
            }
        }

        //---------------SetDepartmentClearance------------------------
        [HttpGet]
        public ActionResult SetDepartmentClearance()
        {

            if (_sessionService.Get<string>("REGID_&_ECODE") != null)
            {
                ViewBag.hdn_REGID_ECODE = _sessionService.Get<string>("REGID_&_ECODE");
                HttpContext.Session.SetString("REGID_&_ECODE", string.Empty);
            }
            
            var modelEmpDtls= getempdtls();
            if (modelEmpDtls == null)
            {
                return RedirectToAction("ManageResignationRequest", "Separation");
            }
            else
            {
                SetDepartmentClearanceModel model = new SetDepartmentClearanceModel
                {
                    SetDeptClrEmpDtlsViewModel = modelEmpDtls,
                };

                return View("SetDepartmentClearance", model);
            }
        }
        private SetDeptClrEmpDtlsViewModel getempdtls()
        {
            try
            {
                string hdn_REGID_ECODE = ViewBag.hdn_REGID_ECODE;
                if (hdn_REGID_ECODE != string.Empty)
                {
                    SetDeptClrEmpDtlsViewModel model = new SetDeptClrEmpDtlsViewModel();
                    DataTable dt = _Separation.HRSPResigDetail(hdn_REGID_ECODE.ToString().Split('&')[1]/*Ecode*/, hdn_REGID_ECODE.ToString().Split('&')[0]);
                    if (dt.Rows.Count > 0)
                    {
                        model.lbl_assname = dt.Rows[0]["EMPNAME"].ToString();
                        model.lbl_assecode = dt.Rows[0]["ADEMPCODE"].ToString();
                        model.lbl_joindate = dt.Rows[0]["REGDATE"].ToString();
                        model.lbl_desg = dt.Rows[0]["DESCRIP"].ToString();
                        model.lbl_fundesg = dt.Rows[0]["FNDESIG"].ToString();
                        model.lbl_phone = dt.Rows[0]["TMOBILE"].ToString();
                        model.lbl_emailid = dt.Rows[0]["EMAILID"].ToString();
                        model.lbl_op = dt.Rows[0]["OPERATION"].ToString();
                        model.lbl_div = dt.Rows[0]["DIVISION"].ToString();
                        model.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                        model.lbl_sec = dt.Rows[0]["SECTION"].ToString();
                        model.lbl_site = dt.Rows[0]["sitedesc"].ToString();
                        model.lbl_applydate = dt.Rows[0]["RESIGNED_DATE"].ToString();
                        model.lbl_reldate = dt.Rows[0]["RELIEVING_DATE_SELF"].ToString();
                        model.lbl_deptreldate = dt.Rows[0]["RELIEVING_DATE_AUTH"].ToString();
                        model.lbl_basicsal = dt.Rows[0]["BASICSALARYAMT"].ToString();
                    }
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        public JsonResult SubmitSetDeptClearance(SubmitSetDeptClrEmpDtlsViewModel model)
        {
            string CONFIDENTIAL_DOC = model.ConfDoc;
            string OTHER_DOC = model.OtherDoc;
            string LIBRARY_BOOK = model.LibraryBook;
            string ITASSETS = model.ItAssest;
            string CAMERA = model.Camere;
            string KEY = model.Key;
            string TRAVEL_BILL = model.TravelBill;

            if (string.IsNullOrEmpty(CONFIDENTIAL_DOC))
            {
                return Json(new { success = false, message = "Enter Confidential Documents & Material Remarks"});
            }
            else if (string.IsNullOrEmpty(OTHER_DOC))
            {
                return Json(new { success = false, message = "Enter Other Documents and Material Remarks" });
            }
            else if (string.IsNullOrEmpty(LIBRARY_BOOK))
            {
                return Json(new { success = false, message = "Enter Books/Library Remarks" });
            }
            else if (string.IsNullOrEmpty(ITASSETS))
            {
                return Json(new { success = false, message = "Enter Laptop, CDS, Pen Drives Remarks" });
            }
            else if (string.IsNullOrEmpty(CAMERA))
            {
                return Json(new { success = false, message = "Enter Camera Remarks" });
            }
            else if (string.IsNullOrEmpty(KEY))
            {
                return Json(new { success = false, message = "Enter Key(Drawer/File/Cabinet) Remarks'" });
            }
            else if (string.IsNullOrEmpty(TRAVEL_BILL))
            {
                return Json(new { success = false, message = "Enter Travel Bills Remarks" });
            }
            else
            {
                //Sourabh
                string SUBMITBY = _sessionService.Get<string>("userID");
                string ECODE = model.Regid_Ecode.ToString().Split('&')[1].ToString();
                string ResigID = model.Regid_Ecode.ToString().Split('&')[0].ToString();
                string AMOUNT = model.BasicSal;
                string msg = _Separation.INSERT_DEPT_CLEARANCE(CONFIDENTIAL_DOC, OTHER_DOC, LIBRARY_BOOK, ITASSETS, CAMERA, KEY, TRAVEL_BILL, SUBMITBY, ECODE, ResigID);
                string[] cmdarg = msg.ToString().Split(new char[] { '#' });
                if (cmdarg[0].ToString() == "1")
                {
                    return Json(new { success = true, redirectUrl = Url.Action("ManageResignationRequest", "Separation"), message = "" });
                }
                else
                {
                    return Json(new { success = false, message = cmdarg[1].ToString() });
                }
            }
        }

        //-----------------AssociateResignationForm----------------------
        [HttpGet]
        public ActionResult AssociateResignationForm()
        {

            var modelPopDateTime = PopulateDateTimeAssRegFrom();
            var modelEmpDtls = FillEmpDtlsEditAssocAssRegFrom();
            var modelAPPAUTHDetails = FillAPPAUTHDtlsAssRegFrom();

            AssociateResignationFormModel model = new AssociateResignationFormModel
            {
                AssRegFromDateSelectionViewModel = modelPopDateTime,
                AssRegFromEmpDtlsViewModel = modelEmpDtls,
                AssRegFromAPPAUTHDetailsViewModel = modelAPPAUTHDetails
            };

            return View("AssociateResignationForm", model);
        }
        public AssRegFromDateSelectionViewModel PopulateDateTimeAssRegFrom()
        {
            var model = new AssRegFromDateSelectionViewModel();

            // Days 1–31
            model.Days = Enumerable.Range(1, 31)
                .Select(d => new SelectListItem { Value = d.ToString(), Text = d.ToString() })
                .ToList();

            // Months 1–12
            model.Months = Enumerable.Range(1, 12)
                .Select(m => new SelectListItem
                {
                    Value = m.ToString(),
                    Text = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(m)
                })
                .ToList();

            // Years (current year -1 to +1)
            int currentYr = DateTime.Now.Year;
            model.Years = Enumerable.Range(currentYr - 1, 3)
                .Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() })
                .ToList();

            // Preselect current date
            model.SelectedDay = DateTime.Now.Day;
            model.SelectedMonth = DateTime.Now.Month;
            model.SelectedYear = DateTime.Now.Year;

            return model;
        }
        private AssRegFromEmpDtlsViewModel FillEmpDtlsEditAssocAssRegFrom()
        {
            AssRegFromEmpDtlsViewModel model = new AssRegFromEmpDtlsViewModel();
            DataTable dt = new DataTable();
            dt = _Separation.HRDDetail(_sessionService.Get<string>("userID"));
            if (dt.Rows.Count > 0)
            {
                model.lbl_assname = dt.Rows[0]["ENAME"].ToString();
                model.lbl_assecode = dt.Rows[0]["ECODE"].ToString();
                model.lbl_joindate = dt.Rows[0]["DOJ"].ToString();
                model.lbl_desg = dt.Rows[0]["DESIGNATION"].ToString();
                model.lbl_fundesg = dt.Rows[0]["FNDESIG"].ToString();
                model.lbl_op = dt.Rows[0]["OPERATION"].ToString();
                model.lbl_dept = dt.Rows[0]["DEPARTMENT"].ToString();
                model.lbl_div = dt.Rows[0]["DIVISION"].ToString();
                model.lbl_sec = dt.Rows[0]["SECTION"].ToString();
                model.lbl_site = dt.Rows[0]["sitedesc"].ToString();

            }
            return model;
        }
        private AssRegFromAPPAUTHDetailsViewModel FillAPPAUTHDtlsAssRegFrom()
        {
            AssRegFromAPPAUTHDetailsViewModel model = new AssRegFromAPPAUTHDetailsViewModel();
            DataTable dt = new DataTable();
            dt = _Separation.HRSPAPPROVAL(_sessionService.Get<string>("userID"), "0");

            string strpoc = string.Empty;
            string strdpthead = string.Empty;
            string strdptname = string.Empty;
            string strdivhead = string.Empty;
            string strdivname = string.Empty;
            string strophead = string.Empty;
            string stropname = string.Empty;

            if (dt.Rows.Count > 0)
            {
                strpoc = dt.Rows[0]["AUTHLEVEL"].ToString();
                if (dt.Rows[0]["empname"].ToString() == "HR")
                {
                    model.lbl_appauth = dt.Rows[0]["empname"].ToString();
                }
                else
                {
                    model.lbl_appauth = dt.Rows[0]["empname"].ToString() + " [" + dt.Rows[0]["adempcode"].ToString() + "]";
                }

                model.hdauthlevel = dt.Rows[0]["AUTHLEVEL"].ToString();
                model.hdauthcode = dt.Rows[0]["adempcode"].ToString();
                model.hdauthemailid = dt.Rows[0]["emailid"].ToString();
                model.hdauthname = dt.Rows[0]["empname"].ToString();
            }
            return model;
        }

        [HttpPost]
        public JsonResult SubmitAssociateResignationForm(AssRegFromSubmitEditAssocResigModel model)
        {
            string strErrMsg = string.Empty;
            string errResult = string.Empty;
            string errMsg = string.Empty;
            string strDay;
            string strMonth;
            string strYear;
            int vdatefrom = 0;
            int vdateto = 0;
            string vmonth = "";
            int DRVSTATUS = 0;
            int Cday = Convert.ToInt32(System.DateTime.Now.Day);
            int Cmonth = Convert.ToInt32(System.DateTime.Now.Month);
            int Cyear = Convert.ToInt32(System.DateTime.Now.Year);
            //DateTime cdate = Convert.ToDateTime(Cmonth + "/" + Cday + "/" + Cyear);
            DateTime cdate = new DateTime(Cyear, Cmonth, Cday);

            string strDate = model.SelectedMonth + "/" + model.SelectedDay + "/" + model.SelectedYear;
            DataTable vdt = _Separation.Get_DateRangeValidation(model.SelectedMonth.ToString());

            if (vdt.Rows.Count > 0)
            {
                vmonth = vdt.Rows[0]["MONTH"].ToString();
                vdatefrom = Convert.ToInt32(vdt.Rows[0]["DATERANGEFROM"]);
                vdateto = Convert.ToInt32(vdt.Rows[0]["DATERANGETO"]);
                DRVSTATUS = Convert.ToInt32(vdt.Rows[0]["STATUS"]);
            }
            // DateTime uselectdate = DateTime.ParseExact(strDate, "M/d/yyyy", null);
            DateTime uselectdate = DateTime.ParseExact(
                    strDate,
                    "M/d/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture
            );

            if (string.IsNullOrEmpty(model.Subject))
            {
                return Json(new { success = false, message = "Subject is mandatory field", focusId = "txtup", showErrorPanel = true });
            }
            else if (string.IsNullOrEmpty(model.Body))
            {
                return Json(new { success = false, message = "Body is mandatory field", focusId = "txtup" });
            }
            else if (string.IsNullOrEmpty(model.latestaddress))
            {
                return Json(new { success = false, message = "Latest address is mandatory field", focusId = "txtup" });
            }

            else if (uselectdate < cdate.AddDays(2))
            {
                return Json(new { success = false, message = "Relieving date should  not be less than current date+2", focusId = "txtup", showErrorPanel = true });
            }
            else if (model.hdauthcode == string.Empty)
            {
                return Json(new { success = false, message = "Approving authority is  mandatory field", focusId = "txtup", showErrorPanel = true });
            }
            else if (model.AgreeTerms == false)
            {
                return Json(new { success = false, message = "Terms and Conditions is  mandatory field", focusId = "txtup", showErrorPanel = true });
            }
            else if (DRVSTATUS == 1)
            {
                if (Convert.ToInt32(model.SelectedDay) <= vdateto && Convert.ToInt32(model.SelectedDay) >= vdatefrom)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Relieving date should not be between {vdatefrom} {vmonth} to {vdateto} {vmonth}",
                        focusId = "txtup",
                        showErrorPanel = true
                    });
                }
                else
                {
                    var result = Insert_Resigned(model);
                    if (result.success)
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("MyResignationRequest", "Separation") });
                    }
                    else
                    {
                        return Json(new { success = false, message = result.message, showErrorPanel = true });
                    }
                }
            }
            else
            {
                var result = Insert_Resigned(model);
                if (result.success)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("MyResignationRequest", "Separation") });
                }
                else
                {
                    return Json(new { success = false, message = result.message, showErrorPanel = true });
                }
            }
        }
        private (bool success, string message) Insert_Resigned(AssRegFromSubmitEditAssocResigModel model)
        {
            string strErrMsg = string.Empty;
            string errResult = string.Empty;
            string errMsg = string.Empty;
            string strDay;
            string strMonth;
            string strYear;
            //errorpanel.Visible = false;
            strDay = model.SelectedDay;
            strMonth = model.SelectedMonthText;
            strYear = model.SelectedYear;
            string releDate = strDay + "-" + strMonth + "-" + strYear;

            strErrMsg = _Separation.EMPRESIGREQ_Set(_sessionService.Get<string>("userID"), model.Subject, model.Body, releDate, model.hdauthcode,model.hdauthlevel, _sessionService.Get<string>("userID"),model.latestaddress);
            string[] ResigStatus = strErrMsg.Split(new Char[] { '#' });
            errResult = Convert.ToString(ResigStatus[0]);
            errMsg = Convert.ToString(ResigStatus[1]);

            if (errResult == "1")
            {
                if (model.lbl_appauth == "HR")
                {
                    SendMailHR(model);
                }
                else
                {
                    mailsend(model);
                }

                return (true, "Resignation inserted successfully");
            }
            return (false, errMsg);
        }
        private void mailsend(AssRegFromSubmitEditAssocResigModel model)
        {
            if (!string.IsNullOrEmpty(model.hdauthemailid))
            {
                try
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                    sendMail.MailTo = model.hdauthemailid;
                    string strSubject = "Pending for the Resignation Approval";
                    string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Resignation Request</b></div>"
                                    + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                    + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + model.hdauthname + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                    + "<tr><td colspan=2>&nbsp;&nbsp;Resignation Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr><tr><td>&nbsp;&nbsp;Details as follows :- </td></tr><tr>&nbsp;</tr></table><br/>"
                                    + "<b>&nbsp;&nbsp;Associate Name:-</b> " + model.lbl_assname + "[" + model.lbl_assecode + "]<br/>"
                                    + "<b>&nbsp;&nbsp;Relieving Date:-</b> " + DateTime.Now.ToShortDateString() + "<br/>"
                                    + "<b>&nbsp;&nbsp;Reason:-</b> " + model.Body;
                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                    "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team E-Separation<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</tr></table></td></tr><tr><td></td></tr><tr> " +
                    "</tr></table></div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
                catch
                {

                }
            }
        }
        public void SendMailHR(AssRegFromSubmitEditAssocResigModel model)
        {
            string EmailsId = string.Empty;
            DataTable dtmail = new DataTable();
            dtmail = _Separation.Get_RESIGNOPHEADHRSENDMAIL(_sessionService.Get<string>("userID"));//ResignationOPHEADsendHRmail
            foreach (DataRow dr in dtmail.Rows)
            {
                EmailsId = EmailsId + dr["EMAILID"].ToString() + ",";
            }
            string FinalEmailID = EmailsId.Remove(EmailsId.Length - 1, 1);

            if (!string.IsNullOrEmpty(FinalEmailID))
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "E-Separation@honda.hmsi.in";
                sendMail.MailTo = FinalEmailID;
                string strSubject = "Pending for the HR Approval";
                string strBody = "<div style='width: 700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width: 700px;height:25px;background-color:skyblue;'><b>&nbsp;&nbsp;Resignation Approval</b></div>"
                    + "<table cellpadding=0 cellspacing=0 border=0 width='700px' >"
                    + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear HR Head,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                    + "<tr><td colspan=2>&nbsp;&nbsp;Resignation Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr><tr><td colspan=2>&nbsp;&nbsp;Details as follows :-</td></tr></table><br/>"
                    + "<b>&nbsp;&nbsp;Associate Name:-</b> " + model.lbl_assname + "[" + model.lbl_assecode + "]<br/>"
                    + "<b>&nbsp;&nbsp;Reason:-</b> " + model.Body;
                strBody = strBody +
                "<table cellpadding=0 cellspacing=0 border=0 width='700px'><tr><td><br/> &nbsp;&nbsp;Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td> &nbsp;&nbsp;Team E-Seperation<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                "</td></tr></table></div> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
        }

        //-----------------ClearanceFormPrint-----------------------------

        public ActionResult clrformbtn_click(string id)
        {
            HttpContext.Session.SetString("REG_ID", id);
            return RedirectToAction("ClearanceFormPrint", "Separation");
        }
        [HttpGet]
        public async Task<ActionResult> ClearanceFormPrint()
        {

            string UserID = _sessionService.Get<string>("userID");

            string ResigID = _sessionService.Get<string>("REG_ID");

            DataTable odt = _Separation.HRSPAcceptanceDetail("", ResigID);
            DataSet cds = _Separation.FULLRESIGNDETAIL_GET(ResigID, "", "");

            //DataRow nDr;
            //SeparationLetter ds_SeparationLetter = new SeparationLetter();
            var modelShow = new ShowClearanceFormPrintViewModel
            {
                lta = false,
                housrent = false,
                othrdedct = false,
                mdcllon = false,
                medreimb = false,
                Tr33 = false,
                Tr34 = false,
                TR52 = false,
                TR53 = false,
                TR70 = false,
                TR71 = false,
                TR72 = false,
                TR73 = false,
                TR74 = false,
                TR75 = false,
                TR76 = false,
                homlon = false,
                susptrvl = false,
                prdtlon = false,
                TRSecurity = false,
                Tr20 = false,
                Tr32 = false,
                Tr35 = false,
                TR51 = false,
                TR54 = false,
                TR77 = false,
                TR78 = false,
                TR79 = false,
                TR80 = false,
                TR81 = false,
                TR82 = false,
                TR91 = false,
                TR92 = false,
                TR93 = false,
                TR94 = false,
                Tr28 = false,
                Tr29 = false,
                Tr1 = false,
                Tr2 = false,
                Tr3 = false,
                Tr4 = false,
                Tr5 = false,
                Tr6 = false,
                TR50 = false,
                TR55 = false,
                Tr56 = false,
                TR83 = false,
                TR84 = false,
                TR85 = false,
                TR86 = false,
                TR87 = false,
                TR88 = false,
                TR89 = false,
                Tr21 = false,
                Tr22 = false,
                Tr23 = false,
                Tr24 = false,
                Tr25 = false,
                Tr26 = false,
                Tr27 = false,
                Tr31 = false,
                Tr30 = false,
                TR58 = false,
                TR57 = false,
                TR67 = false,
                TR68 = false,
                TR69 = false,
                Tr7 = false,
                Tr8 = false,
                Tr9 = false,
                Tr10 = false,
                Tr11 = false,
                Tr12 = false,
                TRHR = false,
                TRASR = false,
                TRLEGL = false,
                TRADMIN = false,
                TRMEDICAL = false,
                Tr38 = false,
                Tr36 = false,
                Tr39 = false,
                Tr40 = false,
                TR59 = false,
                TR60 = false,
                TR61 = false,
                TR62 = false,
                Tr13 = false,
                Tr14 = false,
                Tr15 = false,
                Tr16 = false,
                Tr17 = false,
                Tr18 = false,
                Tr19 = false,
                trsis = false,
                traccount = false,
                Amounttab = false,
                TR41 = false,
                TR42 = false,
                TR43 = false,
                TR44 = false,
                TR45 = false,
                TR46 = false,
                TR63 = false,
                TR64 = false,
                TR65 = false,
                TR66 = false,
            };

            HRSPAcceptanceDetailViewModel modelHrps = new HRSPAcceptanceDetailViewModel();
            foreach (DataRow dr in odt.Rows)
            {
                modelHrps = new HRSPAcceptanceDetailViewModel
                {
                    lblEmpName = Convert.ToString(dr["EMPNAME"]),
                    lblsignature = Convert.ToString(dr["EMPNAME"]),
                    lblEmpcode = Convert.ToString(dr["ADEMPCODE"]),
                    lblDesignation = Convert.ToString(dr["EMPDESIG"]),
                    lblOperation = Convert.ToString(dr["OPERATION"]),
                    lblresigndt = Convert.ToString(dr["RESIGNED_DATE"]),
                    lblrelevdt = Convert.ToString(dr["RELIEVING_DATE_AUTH"]),
                    lbldate = Convert.ToString(dr["ISSUE_DATE"]),
                    lbljoindt = Convert.ToString(dr["REGDATE"]),
                    lblsalry = Convert.ToString(dr["BASICSALARYAMT"]),
                    lblremark = Convert.ToString(dr["CONFIDENTIAL_DOC"]),
                    lblsubmitby = Convert.ToString(dr["ASSDPTHEADNAME"]),
                    lblsubmitdt = Convert.ToString(dr["ADSUBMITDATE"]),
                    Label4 = Convert.ToString(dr["OTHER_DOC"]),
                    Label8 = Convert.ToString(dr["ITASSETS"]),
                    Label12 = Convert.ToString(dr["CAMERA"]),
                    Label16 = Convert.ToString(dr["KEY"]),
                    Label20 = Convert.ToString(dr["TRAVEL_BILL"]),
                    Label347 = Convert.ToString(dr["LATESTADDRESS"])
                };
            }

            // --------SIS----
            SISViewModel modelSIS = new SISViewModel();
             SISClearanceViewModel SISClrns  = new SISClearanceViewModel();
             SISAcBlockSAPViewModel SISAcBlk  = new SISAcBlockSAPViewModel();
             SISAppPcpackViewModel SISAppPc= new SISAppPcpackViewModel();
             SISNetworkingViewModel SISNetw = new SISNetworkingViewModel();
             SISUserSupportViewModel SISUsrSpt  = new SISUserSupportViewModel();
        //--------ANSR----
        ANSRViewModel modelANSR = new ANSRViewModel();
            ANSRProductLoanViewModel modelANSRProd = new ANSRProductLoanViewModel();
            PersonalLoanViewModel modelANSRPerLn = new PersonalLoanViewModel();
            NPSViewModel modelANSRNPS = new NPSViewModel();
            GratuityViewModel modelANSRGrat = new GratuityViewModel();
            HomeLoanIntSubsidyViewModel modelANSRHmLoan = new HomeLoanIntSubsidyViewModel();
            LTAViewModel modelANSRLTA = new LTAViewModel();
            LTARecUABSViewModel modelANSRLATRec = new LTARecUABSViewModel();
            EarnLeaveViewModel modelANSREarn = new EarnLeaveViewModel();
            CasualLeaveViewModel modelANSRCasuLv = new CasualLeaveViewModel();
            SickLeaveViewModel modelANSRSickLv = new SickLeaveViewModel();
            COffViewModel modelANSRCoff = new COffViewModel();
            AttndIdCardViewModel modelANSRAttnId = new AttndIdCardViewModel();
            AnyOtherViewModel modelANSRAnyOth = new AnyOtherViewModel();
            //--------Security----
            SecurityViewModel modelSecurity = new SecurityViewModel();
             LaptoAuthViewModel modelSecurityLaptop = new LaptoAuthViewModel();
            CarParkStickerViewModel modelSecurityCarPark = new CarParkStickerViewModel();
            //--------Administration----
            AdministrationViewModel modelAdmnstion = new AdministrationViewModel();
             TeaSnackDedViewModel modelAdmnstionTea = new TeaSnackDedViewModel();
             CanteenViewModel modelAdmnstionCan = new CanteenViewModel();
             TelephoneViewModel modelAdmnstionTel = new TelephoneViewModel();
             TravelBookingViewModel modelAdmnstionTrav = new TravelBookingViewModel();
             UniformViewModel modelAdmnstionUni = new UniformViewModel();
             TransportDedViewModel modelAdmnstionTrans = new TransportDedViewModel();
             CarPatrolViewModel modelAdmnstionCar = new CarPatrolViewModel();
        //-------Medical-----------
        MedicalViewModel modelMedcl = new MedicalViewModel();
            MedInsuPremViewModel modelMedclInsP = new MedInsuPremViewModel();
            DelNmMedInsuPolcyViewModel modelMedclDelNmIns = new DelNmMedInsuPolcyViewModel();
            //-------SecLegal-----------
            SectryLeglViewModel modelSeclegl = new SectryLeglViewModel();
            BondFTraningViewModel modelSecBFT = new BondFTraningViewModel();
            BondFTrainingViewModel modelSecBFTng = new BondFTrainingViewModel();
            GratuitySecLegViewModel modelSecGraSL = new GratuitySecLegViewModel();
            SuperannuationViewModel modelSecSupr = new SuperannuationViewModel();
            //------HR----------
            HRViewModel modelHR = new HRViewModel();
            ResForSeprViewModel modelHRResSep = new ResForSeprViewModel();
            ExitIntvViewModel modelHRExitInt = new ExitIntvViewModel();
            AnyOthViewModel modelHRAnyOth = new AnyOthViewModel();
            RelocationDotViewModel modelHRRelDot = new RelocationDotViewModel();
            RelocationViewModel modelHRRel = new RelocationViewModel();
            NoticeViewModel modelHRNot= new NoticeViewModel();
            NoticeAppointLtrViewModel modelHRNotApp= new NoticeAppointLtrViewModel();
            NoticePayDayViewModel modelHRNotPDay=new NoticePayDayViewModel();
            //------Account----------
            AccountViewModel modelAccount = new AccountViewModel();
            ProductLoanViewModel modelAcProdloan = new ProductLoanViewModel();
            MedReiumbViewModel modalAcMedRem = new MedReiumbViewModel();
            MedicalLoanViewModel modelAcMedLoan = new MedicalLoanViewModel();
            LTAccountViewModel modelAcLTA = new LTAccountViewModel();
            HouseRentViewModel modelAcHouseRent = new HouseRentViewModel();
            HomeLoanIntViewModel modelACHomeLoan = new HomeLoanIntViewModel();
            OthDed80ViewModel modelAcOthDed = new OthDed80ViewModel();
            SuspenseViewModel modelAcSusp = new SuspenseViewModel();

            foreach (DataRow dr in cds.Tables[1].Rows)
            {
                string footerstring = Convert.ToString(dr["DESCRIPTION"]) ?? string.Empty;
                string headString = Convert.ToString(dr["HEADERDECS"]) ?? string.Empty;
                if (headString == "SIS")
                {
                    modelSIS = new SISViewModel
                    {
                        Label29 = Convert.ToString(dr["HEADCODE"]) ?? string.Empty,
                        Label30 = Convert.ToString(dr["HSUBDATE"]) ?? string.Empty,
                        Label28 = Convert.ToString(dr["HREMARK"]) ?? string.Empty
                    };

                    if (footerstring == "SIS Clearance")
                    {
                        modelShow.TR42 = true;
                        SISClrns.Label211 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISClrns.Label23 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISClrns.Label24 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        SISClrns.Label25 = Convert.ToString(dr["SUBMITBY"]) ?? string.Empty;
                        SISClrns.Label26 = Convert.ToString(dr["SUBMITTEDATE1"]) ?? string.Empty;
                        if (SISClrns.Label211 != "0" && SISClrns.Label211 != "")
                        {
                            modelShow.trsis = true; modelShow.Amounttab = true;
                            SISClrns.Label212 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        }
                    }
                    if (footerstring == "Account blocked in SAP")
                    {
                        modelShow.TR43 = true;
                        SISAcBlk.Label335 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISAcBlk.Label317 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISAcBlk.Label318 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        SISAcBlk.Label319 = Convert.ToString(dr["SUBMITBY"]) ?? string.Empty;
                        SISAcBlk.Label320 = Convert.ToString(dr["SUBMITTEDATE1"]) ?? string.Empty;
                        if (SISAcBlk.Label317 != "0" && SISAcBlk.Label317 != "")
                        {
                            modelShow.trsis = true; modelShow.Amounttab = true; modelShow.Tr38 = true;
                            SISAcBlk.Label336 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        }
                    }
                    if (footerstring == "Application (Pcpack, MRO,SAP,CATIA/GCC etc.)")
                    {
                        modelShow.TR44 = true;
                        SISAppPc.Label337 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISAppPc.Label321 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISAppPc.Label322 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        SISAppPc.Label323 = Convert.ToString(dr["SUBMITBY"]) ?? string.Empty;
                        SISAppPc.Label324 = Convert.ToString(dr["SUBMITTEDATE1"]) ?? string.Empty;
                        if (SISAppPc.Label321 != "0" && SISAppPc.Label321 != "")
                        {
                            modelShow.trsis = true; modelShow.Amounttab = true; modelShow.Tr39 = true;
                            SISAppPc.Label338 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        }
                    }
                    if (footerstring == "Networking & Server (Lotus & LAN ID etc)")
                    {
                        modelShow.TR45 = true;
                        SISNetw.Label333 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISNetw.Label325 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISNetw.Label326 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        SISNetw.Label327 = Convert.ToString(dr["SUBMITBY"]) ?? string.Empty;
                        SISNetw.Label328 = Convert.ToString(dr["SUBMITTEDATE1"]) ?? string.Empty;
                        if (SISNetw.Label325 != "0" && SISNetw.Label325 != "")
                        {
                            modelShow.trsis = true; modelShow.Amounttab = true; modelShow.Tr36 = true;
                            SISNetw.Label334 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        }
                    }
                    if (footerstring == "User Support( Floppies / CD / Laptop etc.)")
                    {
                        modelShow.TR46 = true;
                        SISUsrSpt.Label339 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISUsrSpt.Label329 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        SISUsrSpt.Label330 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        SISUsrSpt.Label331 = Convert.ToString(dr["SUBMITBY"]) ?? string.Empty;
                        SISUsrSpt.Label329 = Convert.ToString(dr["SUBMITTEDATE1"]) ?? string.Empty;
                        if (SISUsrSpt.Label329 != "0" && SISUsrSpt.Label329 != "")
                        {
                            modelShow.trsis = true; modelShow.Amounttab = true; modelShow.Tr40 = true;
                            SISUsrSpt.Label340 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        }
                    }
                }
                if (headString == "A&SR")
                {
                    modelANSR = new ANSRViewModel
                    {
                        Label129 = Convert.ToString(dr["HEADCODE"]),
                        Label130 = Convert.ToString(dr["HSUBDATE"]),
                        Label128 = Convert.ToString(dr["HREMARK"])
                    };

                    if (footerstring == "Product Loan")
                    {
                        modelShow.TR70 = true;
                        modelANSRProd.Label237 = Convert.ToString(dr["Amount"]);
                        modelANSRProd.Label99 = Convert.ToString(dr["Amount"]);
                        modelANSRProd.Label100 = Convert.ToString(dr["REMARKS"]);
                        modelANSRProd.Label101 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRProd.Label102 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRProd.Label237 != "0" && modelANSRProd.Label237 != "")
                        {
                            modelShow.Tr7 = true; modelShow.TRASR = true;
                            modelANSRProd.Label238 = Convert.ToString(dr["REMARKS"]);
                            modelShow.Amounttab = true;

                        }
                    }
                    if (footerstring == "Personal Loan")
                    {
                        modelShow.TR71 = true;
                        modelANSRPerLn.Label239 = Convert.ToString(dr["Amount"]);
                        modelANSRPerLn.Label103 = Convert.ToString(dr["Amount"]);
                        modelANSRPerLn.Label104 = Convert.ToString(dr["REMARKS"]);
                        modelANSRPerLn.Label105 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRPerLn.Label106 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRPerLn.Label239 != "0" && modelANSRPerLn.Label239 != "")
                        {
                            modelShow.Tr8 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRPerLn.Label240 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "NPS")
                    {
                        modelShow.TR72 = true;
                        modelANSRNPS.Label241 = Convert.ToString(dr["Amount"]);
                        modelANSRNPS.Label107 = Convert.ToString(dr["Amount"]);
                        modelANSRNPS.Label108 = Convert.ToString(dr["REMARKS"]);
                        modelANSRNPS.Label109 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRNPS.Label110 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRNPS.Label241 != "0" && modelANSRNPS.Label241 != "")
                        {
                            modelShow.Tr9 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRNPS.Label242 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Gratuity")
                    {
                        modelShow.TR73 = true;
                        modelANSRGrat.Label111 = Convert.ToString(dr["Amount"]);
                        modelANSRGrat.Label243 = Convert.ToString(dr["Amount"]);
                        modelANSRGrat.Label112 = Convert.ToString(dr["REMARKS"]);
                        modelANSRGrat.Label113 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRGrat.Label114 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRGrat.Label243 != "0" && modelANSRGrat.Label243 != "")
                        {
                            modelShow.Tr10 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRGrat.Label216 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Home Loan Interest Subsidy")
                    {
                        modelShow.TR74 = true;
                        modelANSRHmLoan.Label115 = Convert.ToString(dr["Amount"]);
                        modelANSRHmLoan.Label245 = Convert.ToString(dr["Amount"]);
                        modelANSRHmLoan.Label116 = Convert.ToString(dr["REMARKS"]);
                        modelANSRHmLoan.Label117 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRHmLoan.Label118 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRHmLoan.Label245 != "0" && modelANSRHmLoan.Label245 != "")
                        {
                            modelShow.Tr11 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRHmLoan.Label246 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "LTA")
                    {
                        modelShow.TR75 = true;
                        modelANSRLTA.Label119 = Convert.ToString(dr["Amount"]);
                        modelANSRLTA.Label259 = Convert.ToString(dr["Amount"]);
                        modelANSRLTA.Label120 = Convert.ToString(dr["REMARKS"]);
                        modelANSRLTA.Label121 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRLTA.Label122 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRLTA.Label259 != "0" && modelANSRLTA.Label259 != "")
                        {
                            modelShow.Tr18 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRLTA.Label260 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "LTA recovery due to UABS")
                    {
                        modelShow.TR76 = true;
                        modelANSRLATRec.Label123 = Convert.ToString(dr["Amount"]);
                        modelANSRLATRec.Label261 = Convert.ToString(dr["Amount"]);
                        modelANSRLATRec.Label124 = Convert.ToString(dr["REMARKS"]);
                        modelANSRLATRec.Label125 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRLATRec.Label126 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRLATRec.Label261 != "0" && modelANSRLATRec.Label261 != "")
                        {
                            modelShow.Tr19 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRLATRec.Label262 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Earned Leave")
                    {
                        modelShow.TR77 = true;
                        modelANSREarn.Label131 = Convert.ToString(dr["Amount"]);
                        modelANSREarn.Label247 = Convert.ToString(dr["Amount"]);
                        modelANSREarn.Label132 = Convert.ToString(dr["REMARKS"]);
                        modelANSREarn.Label133 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSREarn.Label134 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSREarn.Label247 != "0" && modelANSREarn.Label247 != "")
                        {
                            modelShow.Tr12 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSREarn.Label248 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Casual Leave")
                    {
                        modelShow.TR78 = true;
                        modelANSRCasuLv.Label135 = Convert.ToString(dr["Amount"]);
                        modelANSRCasuLv.Label249 = Convert.ToString(dr["Amount"]);
                        modelANSRCasuLv.Label136 = Convert.ToString(dr["REMARKS"]);
                        modelANSRCasuLv.Label137 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRCasuLv.Label138 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRCasuLv.Label249 != "0" && modelANSRCasuLv.Label249 != "")
                        {
                            modelShow.Tr13 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRCasuLv.Label250 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Sick Leave")
                    {
                        modelShow.TR79 = true;
                        modelANSRSickLv.Label139 = Convert.ToString(dr["Amount"]);
                        modelANSRSickLv.Label251 = Convert.ToString(dr["Amount"]);
                        modelANSRSickLv.Label140 = Convert.ToString(dr["REMARKS"]);
                        modelANSRSickLv.Label141 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRSickLv.Label142 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRSickLv.Label251 != "0" && modelANSRSickLv.Label251 != "")
                        {
                            modelShow.Tr14 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRSickLv.Label252 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "C-Off")
                    {
                        modelShow.TR80 = true;
                        modelANSRCoff.Label143 = Convert.ToString(dr["Amount"]);
                        modelANSRCoff.Label253 = Convert.ToString(dr["Amount"]);
                        modelANSRCoff.Label144 = Convert.ToString(dr["REMARKS"]);
                        modelANSRCoff.Label145 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRCoff.Label146 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRCoff.Label253 != "0" && modelANSRCoff.Label253 != "")
                        {
                            modelShow.Tr15 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRCoff.Label254 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Attendence/ID Card")
                    {
                        modelShow.TR81 = true;
                        modelANSRAttnId.Label147 = Convert.ToString(dr["Amount"]);
                        modelANSRAttnId.Label255 = Convert.ToString(dr["Amount"]);
                        modelANSRAttnId.Label148 = Convert.ToString(dr["REMARKS"]);
                        modelANSRAttnId.Label149 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRAttnId.Label150 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRAttnId.Label255 != "0" && modelANSRAttnId.Label255 != "")
                        {
                            modelShow.Tr16 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRAttnId.Label256 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Any Other")
                    {
                        modelShow.TR82 = true;
                        modelANSRAnyOth.Label151 = Convert.ToString(dr["Amount"]);
                        modelANSRAnyOth.Label257 = Convert.ToString(dr["Amount"]);
                        modelANSRAnyOth.Label152 = Convert.ToString(dr["REMARKS"]);
                        modelANSRAnyOth.Label153 = Convert.ToString(dr["SUBMITBY"]);
                        modelANSRAnyOth.Label154 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelANSRAnyOth.Label257 != "0" && modelANSRAnyOth.Label257 != "")
                        {
                            modelShow.Tr17 = true; modelShow.Amounttab = true; modelShow.TRASR = true;
                            modelANSRAnyOth.Label258 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                }
                if (headString == "ADMINISTRATION")
                {
                    modelAdmnstion = new AdministrationViewModel
                    {
                        Label185 = Convert.ToString(dr["HEADCODE"]),
                        Label186 = Convert.ToString(dr["HSUBDATE"]),
                        Label184 = Convert.ToString(dr["HREMARK"])
                    };

                    if (footerstring == "Tea & Snacks Deduction")
                    {
                        modelShow.TR85 = true;
                        modelAdmnstionTea.Label265 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionTea.Label163 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionTea.Label164 = Convert.ToString(dr["REMARKS"]);
                        modelAdmnstionTea.Label165 = Convert.ToString(dr["SUBMITBY"]);
                        modelAdmnstionTea.Label166 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAdmnstionTea.Label265 != "0" && modelAdmnstionTea.Label265 != "")
                        {
                            modelShow.TRADMIN = true;
                            modelShow.Tr21 = true;
                            modelShow.Amounttab = true;
                            modelAdmnstionTea.Label266 = Convert.ToString(dr["REMARKS"]);

                        }
                    }
                    if (footerstring == "Canteen (Lunch / Dinner)")
                    {
                        modelShow.TR86 = true;
                        modelAdmnstionCan.Label267 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionCan.Label167 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionCan.Label168 = Convert.ToString(dr["REMARKS"]);
                        modelAdmnstionCan.Label169 = Convert.ToString(dr["SUBMITBY"]);
                        modelAdmnstionCan.Label170 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAdmnstionCan.Label267 != "0" && modelAdmnstionCan.Label267 != "")
                        {
                            modelShow.TRADMIN = true; modelShow.Tr22 = true; modelShow.Amounttab = true;
                            modelAdmnstionCan.Label268 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Telephone/Mobile Deduction")
                    {
                        modelShow.TR83 = true;
                        modelAdmnstionTel.Label155 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        modelAdmnstionTel.Label269 = Convert.ToString(dr["Amount"]) ?? string.Empty;
                        modelAdmnstionTel.Label156 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        modelAdmnstionTel.Label157 = Convert.ToString(dr["SUBMITBY"]) ?? string.Empty;
                        modelAdmnstionTel.Label158 = Convert.ToString(dr["SUBMITTEDATE1"]) ?? string.Empty;
                        if (modelAdmnstionTel.Label269 != "0" && modelAdmnstionTel.Label269 != "")
                        {
                            modelShow.TRADMIN = true; modelShow.Tr23 = true; modelShow.Amounttab = true;
                            modelAdmnstionTel.Label270 = Convert.ToString(dr["REMARKS"]) ?? string.Empty;
                        }
                    }
                    if (footerstring == "Travel Booking")
                    {
                        modelShow.TR84 = true;
                        modelAdmnstionTrav.Label159 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionTrav.Label271 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionTrav.Label160 = Convert.ToString(dr["REMARKS"]);
                        modelAdmnstionTrav.Label161 = Convert.ToString(dr["SUBMITBY"]);
                        modelAdmnstionTrav.Label162 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAdmnstionTrav.Label271 != "0" && modelAdmnstionTrav.Label271 != "")
                        {
                            modelShow.TRADMIN = true; modelShow.Tr24 = true; modelShow.Amounttab = true;
                            modelAdmnstionTrav.Label272 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Uniform & Locker Keys")
                    {
                        modelShow.TR87 = true;
                        modelAdmnstionUni.Label171 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionUni.Label273 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionUni.Label172 = Convert.ToString(dr["REMARKS"]);
                        modelAdmnstionUni.Label173 = Convert.ToString(dr["SUBMITBY"]);
                        modelAdmnstionUni.Label174 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAdmnstionUni.Label273 != "0" && modelAdmnstionUni.Label273 != "")
                        {
                            modelShow.TRADMIN = true; modelShow.Tr25 = true; modelShow.Amounttab = true;
                            modelAdmnstionUni.Label274 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Transport Deduction/TAG")
                    {
                        modelShow.TR88 = true;
                        modelAdmnstionTrans.Label175 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionTrans.Label275 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionTrans.Label176 = Convert.ToString(dr["REMARKS"]);
                        modelAdmnstionTrans.Label177 = Convert.ToString(dr["SUBMITBY"]);
                        modelAdmnstionTrans.Label178 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAdmnstionTrans.Label275 != "0" && modelAdmnstionTrans.Label275 != "")
                        {
                            modelShow.TRADMIN = true; modelShow.Tr26 = true; modelShow.Amounttab = true;
                            modelAdmnstionTrans.Label276 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Car/Petrol Slips")
                    {
                        modelShow.TR89 = true;
                        modelAdmnstionCar.Label179 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionCar.Label277 = Convert.ToString(dr["Amount"]);
                        modelAdmnstionCar.Label180 = Convert.ToString(dr["REMARKS"]);
                        modelAdmnstionCar.Label181 = Convert.ToString(dr["SUBMITBY"]);
                        modelAdmnstionCar.Label182 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAdmnstionCar.Label277 != "0" && modelAdmnstionCar.Label277 != "")
                        {
                            modelShow.TRADMIN = true; modelShow.Tr27 = true; modelShow.Amounttab = true;
                            modelAdmnstionCar.Label278 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                }
                if (headString == "SECURITY")
                {
                    modelSecurity = new SecurityViewModel
                    {
                        Label193 = Convert.ToString(dr["HEADCODE"]),
                        Label194 = Convert.ToString(dr["HSUBDATE"]),
                        Label192 = Convert.ToString(dr["HREMARK"])
                    };

                    if (footerstring == "Laptop Authorization Card")
                    {
                        modelShow.TR91 = true;
                        modelSecurityLaptop.Label188 = Convert.ToString(dr["REMARKS"]);
                        modelSecurityLaptop.Label189 = Convert.ToString(dr["SUBMITBY"]);
                        modelSecurityLaptop.Label190 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        modelSecurityLaptop.Label187 = Convert.ToString(dr["Amount"]);
                        modelSecurityLaptop.Label279 = Convert.ToString(dr["Amount"]);
                        if (modelSecurityLaptop.Label279 != "0" && modelSecurityLaptop.Label279 != "")
                        {
                            modelShow.TRSecurity = true; modelShow.Tr28 = true; modelShow.Amounttab = true;
                            modelSecurityLaptop.Label280 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Car Parking Sticker")
                    {
                        modelShow.TR92 = true;
                        modelSecurityCarPark.Label195 = Convert.ToString(dr["Amount"]);
                        modelSecurityCarPark.Label281 = Convert.ToString(dr["Amount"]);
                        modelSecurityCarPark.Label196 = Convert.ToString(dr["REMARKS"]);
                        modelSecurityCarPark.Label197 = Convert.ToString(dr["SUBMITBY"]);
                        modelSecurityCarPark.Label198 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelSecurityCarPark.Label281 != "0" && modelSecurityCarPark.Label281 != "")
                        {
                            modelShow.TRSecurity = true; modelShow.Tr29 = true; modelShow.Amounttab = true;
                            modelSecurityCarPark.Label282 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                }
                if (headString == "MEDICAL")
                {
                    modelMedcl = new MedicalViewModel
                    {
                        Label209 = Convert.ToString(dr["HEADCODE"]),
                        Label210 = Convert.ToString(dr["HSUBDATE"]),
                        Label208 = Convert.ToString(dr["HREMARK"])
                    };

                    if (footerstring == "Mediclaim Insu. Prem.")
                    {
                        modelShow.TR93 = true;
                        modelMedclInsP.Label199 = Convert.ToString(dr["Amount"]);
                        modelMedclInsP.Label283 = Convert.ToString(dr["Amount"]);
                        modelMedclInsP.Label200 = Convert.ToString(dr["REMARKS"]);
                        modelMedclInsP.Label201 = Convert.ToString(dr["SUBMITBY"]);
                        modelMedclInsP.Label202 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelMedclInsP.Label283 != "0" && modelMedclInsP.Label283 != "")
                        {
                            modelShow.TRMEDICAL = true; modelShow.Tr30 = true; modelShow.Amounttab = true;
                            modelMedclInsP.Label280 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Deletion of name from med. Insu. Policy")
                    {
                        modelShow.TR94 = true;
                        modelMedclDelNmIns.Label203 = Convert.ToString(dr["Amount"]);
                        modelMedclDelNmIns.Label285 = Convert.ToString(dr["Amount"]);
                        modelMedclDelNmIns.Label204 = Convert.ToString(dr["REMARKS"]);
                        modelMedclDelNmIns.Label205 = Convert.ToString(dr["SUBMITBY"]);
                        modelMedclDelNmIns.Label206 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelMedclDelNmIns.Label285 != "0" && modelMedclDelNmIns.Label285 != "")
                        {
                            modelShow.TRMEDICAL = true; modelShow.Tr31 = true; modelShow.Amounttab = true;
                            modelMedclDelNmIns.Label282 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                }
                if (headString == "SECRETARIAL & LEGAL")
                {
                    modelSeclegl = new SectryLeglViewModel
                    {
                        Label97 = Convert.ToString(dr["HEADCODE"]),
                        Label98 = Convert.ToString(dr["HSUBDATE"]),
                        Label96 = Convert.ToString(dr["HREMARK"])
                    };

                    if (footerstring == "Bond (Foreign Traning)")
                    {
                        modelShow.TR55 = true;
                        modelSecBFT.Label345 = Convert.ToString(dr["Amount"]);
                        modelSecBFT.Label341 = Convert.ToString(dr["Amount"]);
                        modelSecBFT.Label342 = Convert.ToString(dr["REMARKS"]);
                        modelSecBFT.Label343 = Convert.ToString(dr["SUBMITBY"]);
                        modelSecBFT.Label344 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelSecBFT.Label341 != "0" && modelSecBFT.Label341 != "")
                        {
                            modelShow.TRLEGL = true; modelShow.Tr56 = true;
                            modelSecBFT.Label346 = Convert.ToString(dr["REMARKS"]);
                            modelShow.Amounttab = true;
                        }
                    }
                    if (footerstring == "Bond (Foreign Training)")
                    {
                        modelShow.TR52 = true;
                        modelSecBFTng.Label91 = Convert.ToString(dr["Amount"]);
                        modelSecBFTng.Label263 = Convert.ToString(dr["Amount"]);
                        modelSecBFTng.Label92 = Convert.ToString(dr["REMARKS"]);
                        modelSecBFTng.Label93 = Convert.ToString(dr["SUBMITBY"]);
                        modelSecBFTng.Label94 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelSecBFTng.Label263 != "0" && modelSecBFTng.Label263 != "")
                        {
                            modelShow.TRLEGL = true; modelShow.Tr33 = true;
                            modelSecBFTng.Label264 = Convert.ToString(dr["REMARKS"]);
                            modelShow.Amounttab = true;
                        }
                    }
                    if (footerstring == "Gratuity")
                    {
                        modelShow.TR53 = true;
                        modelSecGraSL.Label313 = Convert.ToString(dr["Amount"]);
                        modelSecGraSL.Label305 = Convert.ToString(dr["Amount"]);
                        modelSecGraSL.Label306 = Convert.ToString(dr["REMARKS"]);
                        modelSecGraSL.Label307 = Convert.ToString(dr["SUBMITBY"]);
                        modelSecGraSL.Label308 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelSecGraSL.Label305 != "0" && modelSecGraSL.Label305 != "")
                        {
                            modelShow.TRLEGL = true; modelShow.Tr34 = true;
                            modelSecGraSL.Label314 = Convert.ToString(dr["REMARKS"]);
                            modelShow.Amounttab = true;
                        }
                    }
                    if (footerstring == "Superannuation")
                    {
                        modelShow.TR54 = true;
                        modelSecSupr.Label315 = Convert.ToString(dr["Amount"]);
                        modelSecSupr.Label309 = Convert.ToString(dr["Amount"]);
                        modelSecSupr.Label310 = Convert.ToString(dr["REMARKS"]);
                        modelSecSupr.Label311 = Convert.ToString(dr["SUBMITBY"]);
                        modelSecSupr.Label312 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelSecSupr.Label315 != "0" && modelSecSupr.Label315 != "")
                        {
                            modelShow.TRLEGL = true; modelShow.Tr35 = true;
                            modelSecSupr.Label316 = Convert.ToString(dr["REMARKS"]);
                            modelShow.Amounttab = true;
                        }
                    }
                }
                if (headString == "HR")
                {
                    modelHR = new HRViewModel
                    {
                        Label57 = Convert.ToString(dr["HEADCODE"]),
                        Label58 = Convert.ToString(dr["HSUBDATE"]),
                        Label56 = Convert.ToString(dr["HREMARK"])
                    };

                    if (footerstring == "Reason for Seperation")
                    {
                        modelShow.TR60 = true;
                        modelHRResSep.Label219 = Convert.ToString(dr["Amount"]);
                        modelHRResSep.Label35 = Convert.ToString(dr["Amount"]);
                        modelHRResSep.Label36 = Convert.ToString(dr["REMARKS"]);
                        modelHRResSep.Label37 = Convert.ToString(dr["SUBMITBY"]);
                        modelHRResSep.Label38 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelHRResSep.Label219 != "0" && modelHRResSep.Label219 != "")
                        {
                            modelShow.TRHR = true;
                            modelShow.Tr2 = true;
                            modelShow.Amounttab = true;
                            modelHRResSep.Label220 = Convert.ToString(dr["REMARKS"]);

                        }
                    }
                    if (footerstring == "Exit Interview")
                    {
                        modelShow.TR61 = true;
                        modelHRExitInt.Label223 = Convert.ToString(dr["Amount"]);
                        modelHRExitInt.Label39 = Convert.ToString(dr["Amount"]);
                        modelHRExitInt.Label40 = Convert.ToString(dr["REMARKS"]);
                        modelHRExitInt.Label41 = Convert.ToString(dr["SUBMITBY"]);
                        modelHRExitInt.Label42 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelHRExitInt.Label223 != "0" && modelHRExitInt.Label223 != "")
                        {
                            modelShow.TRHR = true; modelShow.Tr3 = true; modelShow.Amounttab = true;
                            modelHRExitInt.Label224 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Any Other")
                    {
                        modelShow.TR62 = true;
                        modelHRAnyOth.Label43 = Convert.ToString(dr["Amount"]);
                        modelHRAnyOth.Label227 = Convert.ToString(dr["Amount"]);
                        modelHRAnyOth.Label44 = Convert.ToString(dr["REMARKS"]);
                        modelHRAnyOth.Label45 = Convert.ToString(dr["SUBMITBY"]);
                        modelHRAnyOth.Label46 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelHRAnyOth.Label227 != "0" && modelHRAnyOth.Label227 != "")
                        {
                            modelShow.TRHR = true; modelShow.Tr4 = true; modelShow.Amounttab = true;
                            modelHRAnyOth.Label228 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Relocation Expenses.")
                    {
                        modelShow.TR41 = true;
                        modelHRRelDot.Label31 = Convert.ToString(dr["Amount"]);
                        modelHRRelDot.Label215 = Convert.ToString(dr["Amount"]);
                        modelHRRelDot.Label32 = Convert.ToString(dr["REMARKS"]);
                        modelHRRelDot.Label33 = Convert.ToString(dr["SUBMITBY"]);
                        modelHRRelDot.Label34 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelHRRelDot.Label215 != "0" && modelHRRelDot.Label215 != "")
                        {

                            modelShow.TRHR = true; modelShow.Tr1 = true; modelShow.Amounttab = true;
                            modelHRRelDot.Label216 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Relocation Expenses")
                    {
                        modelShow.TR59 = true;
                        modelHRRel.Label303 = Convert.ToString(dr["Amount"]);
                        modelHRRel.Label299 = Convert.ToString(dr["Amount"]);
                        modelHRRel.Label300 = Convert.ToString(dr["REMARKS"]);
                        modelHRRel.Label301 = Convert.ToString(dr["SUBMITBY"]);
                        modelHRRel.Label302 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelHRRel.Label299 != "0" && modelHRRel.Label299 != "")
                        {
                            modelShow.TRHR = true; modelShow.Tr32 = true; modelShow.Amounttab = true;
                            modelHRRel.Label304 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Notice Period")
                    {
                        modelShow.TR50 = true;
                        modelHRNot.Label47 = Convert.ToString(dr["Amount"]);
                        modelHRNot.Label231 = Convert.ToString(dr["Amount"]);
                        modelHRNot.Label48 = Convert.ToString(dr["REMARKS"]);
                        modelHRNot.Label49 = Convert.ToString(dr["SUBMITBY"]);
                        modelHRNot.Label50 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelHRNot.Label231 != "0" && modelHRNot.Label231 != "")
                        {
                            modelShow.TRHR = true; modelShow.Tr5 = true; modelShow.Amounttab = true;
                            modelHRNot.Label232 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Notice Period (as per appointment letter)")
                    {
                        modelShow.TR57 = true;
                        modelHRNotApp.Label297 = Convert.ToString(dr["Amount"]);
                        modelHRNotApp.Label293 = Convert.ToString(dr["Amount"]);
                        modelHRNotApp.Label294 = Convert.ToString(dr["REMARKS"]);
                        modelHRNotApp.Label295 = Convert.ToString(dr["SUBMITBY"]);
                        modelHRNotApp.Label296 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelHRNotApp.Label293 != "0" && modelHRNotApp.Label293 != "")
                        {
                            modelShow.TRHR = true; modelShow.Tr20 = true; modelShow.Amounttab = true;
                            modelHRNotApp.Label298 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Notice Payment Days (if relieving preponed)")
                    {
                        modelShow.TR58 = true;
                        modelHRNotPDay.Label51 = Convert.ToString(dr["Amount"]);
                        modelHRNotPDay.Label235 = Convert.ToString(dr["Amount"]);
                        modelHRNotPDay.Label52 = Convert.ToString(dr["REMARKS"]);
                        modelHRNotPDay.Label53 = Convert.ToString(dr["SUBMITBY"]);
                        modelHRNotPDay.Label54 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelHRNotPDay.Label235 != "0" && modelHRNotPDay.Label235 != "")
                        {
                            modelShow.TRHR = true; modelShow.Tr6 = true; modelShow.Amounttab = true;
                            modelHRNotPDay.Label236 = Convert.ToString(dr["REMARKS"]);
                        }
                    }

                }
                if (headString == "ACCOUNTS")
                {
                    // Account.Visible = false;
                    modelAccount = new AccountViewModel
                    {
                        Label85 = Convert.ToString(dr["HEADCODE"]),
                        Label86 = Convert.ToString(dr["HSUBDATE"]),
                        Label84 = Convert.ToString(dr["HREMARK"])
                    };


                    if (footerstring == "Med .Reiumb")
                    {
                        modelShow.TR51 = true;
                        modalAcMedRem.Label291 = Convert.ToString(dr["Amount"]);
                        modalAcMedRem.Label287 = Convert.ToString(dr["Amount"]);
                        modalAcMedRem.Label288 = Convert.ToString(dr["REMARKS"]);
                        modalAcMedRem.Label289 = Convert.ToString(dr["SUBMITBY"]);
                        modalAcMedRem.Label290 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modalAcMedRem.Label287 != "0" && modalAcMedRem.Label287 != null)
                        {
                            modelShow.traccount = true;
                            modelShow.medreimb = true;
                            modelShow.Amounttab = true;
                            modalAcMedRem.Label292 = Convert.ToString(dr["REMARKS"]);

                        }
                    }
                    if (footerstring == "Product Loan")
                    {
                        modelAcProdloan =new ProductLoanViewModel();
                        {
                            modelShow.TR63 = true;
                            modelAcProdloan.Label1 = Convert.ToString(dr["Amount"]);
                            modelAcProdloan.Label59 = Convert.ToString(dr["Amount"]);
                            modelAcProdloan.Label60 = Convert.ToString(dr["REMARKS"]);
                            modelAcProdloan.Label61 = Convert.ToString(dr["SUBMITBY"]);
                            modelAcProdloan.Label62 = Convert.ToString(dr["SUBMITTEDATE1"]);
                            if (modelAcProdloan.Label1 != "0" && modelAcProdloan.Label1 != null)
                            {
                                modelShow.traccount = true;
                                modelShow.prdtlon = true;
                                modelShow.Amounttab = true;
                                modelAcProdloan.Label2 = Convert.ToString(dr["REMARKS"]);

                            }
                        }
                    }
                    if (footerstring == "Medical Loan")
                    {
                        modelShow.TR64 = true;
                        modelAcMedLoan.Label63 = Convert.ToString(dr["Amount"]);
                        modelAcMedLoan.Label213 = Convert.ToString(dr["Amount"]);
                        modelAcMedLoan.Label64 = Convert.ToString(dr["REMARKS"]);
                        modelAcMedLoan.Label65 = Convert.ToString(dr["SUBMITBY"]);
                        modelAcMedLoan.Label66 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAcMedLoan.Label213 != "0" && modelAcMedLoan.Label213 != "")
                        {
                            modelShow.traccount = true;
                            modelShow.mdcllon = true; modelShow.Amounttab = true;
                            modelAcMedLoan.Label214 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "LTA")
                    {
                        modelShow.TR65 = true;
                        modelAcLTA.Label67 = Convert.ToString(dr["Amount"]);
                        modelAcLTA.Label217 = Convert.ToString(dr["Amount"]);
                        modelAcLTA.Label68 = Convert.ToString(dr["REMARKS"]);
                        modelAcLTA.Label69 = Convert.ToString(dr["SUBMITBY"]);
                        modelAcLTA.Label70 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAcLTA.Label217 != "0" && modelAcLTA.Label217 != "")
                        {
                            modelShow.traccount = true;
                            modelShow.lta = true; modelShow.Amounttab = true;
                            modelAcLTA.Label218 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "House Rent Paid")
                    {
                        modelShow.TR66 = true;
                        modelAcHouseRent.Label71 = Convert.ToString(dr["Amount"]);
                        modelAcHouseRent.Label221 = Convert.ToString(dr["Amount"]);
                        modelAcHouseRent.Label72 = Convert.ToString(dr["REMARKS"]);
                        modelAcHouseRent.Label73 = Convert.ToString(dr["SUBMITBY"]);
                        modelAcHouseRent.Label74 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAcHouseRent.Label221 != "0" && modelAcHouseRent.Label221 != "")
                        {
                            modelShow.traccount = true;
                            modelShow.housrent = true; modelShow.Amounttab = true;
                            modelAcHouseRent.Label222 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Home Loan Interest")
                    {
                        modelShow.TR67 = true;
                        modelACHomeLoan.Label75 = Convert.ToString(dr["Amount"]);
                        modelACHomeLoan.Label225 = Convert.ToString(dr["Amount"]);
                        modelACHomeLoan.Label76 = Convert.ToString(dr["REMARKS"]);
                        modelACHomeLoan.Label77 = Convert.ToString(dr["SUBMITBY"]);
                        modelACHomeLoan.Label78 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelACHomeLoan.Label225 != "0" && modelACHomeLoan.Label225 != "")
                        {
                            modelShow.traccount = true;
                            modelShow.homlon = true; modelShow.Amounttab = true;
                            modelACHomeLoan.Label226 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Other Deduction (80C to 80U)")
                    {
                        modelShow.TR68 = true;
                        modelAcOthDed.Label79 = Convert.ToString(dr["Amount"]);
                        modelAcOthDed.Label229 = Convert.ToString(dr["Amount"]);
                        modelAcOthDed.Label80 = Convert.ToString(dr["REMARKS"]);
                        modelAcOthDed.Label81 = Convert.ToString(dr["SUBMITBY"]);
                        modelAcOthDed.Label82 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAcOthDed.Label229 != "0" && modelAcOthDed.Label229 != "")
                        {
                            modelShow.traccount = true;
                            modelShow.othrdedct = true; modelShow.Amounttab = true;
                            modelAcOthDed.Label230 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                    if (footerstring == "Suspense/Travel Bills")
                    {
                        modelShow.TR69 = true;
                        modelAcSusp.Label87 = Convert.ToString(dr["Amount"]);
                        modelAcSusp.Label233 = Convert.ToString(dr["Amount"]);
                        modelAcSusp.Label88 = Convert.ToString(dr["REMARKS"]);
                        modelAcSusp.Label89 = Convert.ToString(dr["SUBMITBY"]);
                        modelAcSusp.Label90 = Convert.ToString(dr["SUBMITTEDATE1"]);
                        if (modelAcSusp.Label233 != "0" && modelAcSusp.Label233 != "")
                        {
                            modelShow.traccount = true;
                            modelShow.susptrvl = true; modelShow.Amounttab = true;
                            modelAcSusp.Label234 = Convert.ToString(dr["REMARKS"]);
                        }
                    }
                }
               
            }

            var model = new ClearanceFormPrintModel
            {
                ShowClearanceFormPrintViewModel = modelShow,
                HRSPAcceptanceDetailViewModel = modelHrps,

                SISViewModel = modelSIS,
                SISClearanceViewModel= SISClrns,
                SISAcBlockSAPViewModel= SISAcBlk,
                SISAppPcpackViewModel =SISAppPc,
                SISNetworkingViewModel =SISNetw,
                SISUserSupportViewModel= SISUsrSpt,

                ANSRViewModel = modelANSR,
                ANSRProductLoanViewModel= modelANSRProd,
                PersonalLoanViewModel= modelANSRPerLn,
                NPSViewModel =modelANSRNPS,
                GratuityViewModel =modelANSRGrat ,
                HomeLoanIntSubsidyViewModel= modelANSRHmLoan,
                LTAViewModel= modelANSRLTA,
                LTARecUABSViewModel= modelANSRLATRec,
                EarnLeaveViewModel= modelANSREarn,
                CasualLeaveViewModel= modelANSRCasuLv,
                SickLeaveViewModel= modelANSRSickLv,
                COffViewModel =modelANSRCoff,
                AttndIdCardViewModel= modelANSRAttnId ,
                AnyOtherViewModel= modelANSRAnyOth,

                AdministrationViewModel = modelAdmnstion,
                TeaSnackDedViewModel =modelAdmnstionTea,
                CanteenViewModel =modelAdmnstionCan,
                TelephoneViewModel= modelAdmnstionTel,
                TravelBookingViewModel= modelAdmnstionTrav,
                UniformViewModel =modelAdmnstionUni,
                TransportDedViewModel =modelAdmnstionTrans,
                CarPatrolViewModel =modelAdmnstionCar,

                SecurityViewModel = modelSecurity,
                LaptoAuthViewModel =modelSecurityLaptop,
                CarParkStickerViewModel =modelSecurityCarPark,

                MedicalViewModel = modelMedcl,
                MedInsuPremViewModel =modelMedclInsP,
                DelNmMedInsuPolcyViewModel= modelMedclDelNmIns,


                SectryLeglViewModel = modelSeclegl,
                BondFTraningViewModel =modelSecBFT,
                BondFTrainingViewModel= modelSecBFTng,
                GratuitySecLegViewModel= modelSecGraSL,
                SuperannuationViewModel= modelSecSupr,

                HRViewModel = modelHR,
                ResForSeprViewModel =modelHRResSep,
                ExitIntvViewModel =modelHRExitInt,
                AnyOthViewModel =modelHRAnyOth,
                RelocationDotViewModel =modelHRRelDot,
                RelocationViewModel= modelHRRel,
                NoticeViewModel =modelHRNot,
                NoticeAppointLtrViewModel= modelHRNotApp,
                NoticePayDayViewModel= modelHRNotPDay,

                AccountViewModel = modelAccount,
                ProductLoanViewModel= modelAcProdloan,
                MedReiumbViewModel =modalAcMedRem,
                MedicalLoanViewModel= modelAcMedLoan,
                LTAccountViewModel =modelAcLTA,
                HouseRentViewModel =modelAcHouseRent,
                HomeLoanIntViewModel= modelACHomeLoan,
                OthDed80ViewModel= modelAcOthDed ,
                SuspenseViewModel= modelAcSusp
            };

            //return View("CleranceDetail", model);

            string strHtml = await RenderViewToString(this, "CleranceDetail", model);

            using (var memStream = new MemoryStream())
            using (var docWorkingDocument = new iTextSharp.text.Document(PageSize.A4, 20, 20, 20, 5))
            {
                try
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(docWorkingDocument, memStream);

                    using (var srdDocToString = new StringReader(strHtml))
                    {
                        docWorkingDocument.Open();
                        XMLWorkerHelper.GetInstance().ParseXHtml(pdfWriter, docWorkingDocument, srdDocToString);
                        docWorkingDocument.Close();
                    }

                    byte[] pdfBytes = memStream.ToArray();

                    return File(pdfBytes, "application/pdf");
                }
                catch (Exception ex)
                {
                    return Content("Error generating PDF: " + ex.Message);
                }
            }
        }

        public static async Task<string> RenderViewToString(Controller controller, string viewName, object model)
        {
            var serviceProvider = controller.HttpContext.RequestServices;
            var razorViewEngine = serviceProvider.GetService<IRazorViewEngine>();
            var tempDataProvider = serviceProvider.GetService<ITempDataProvider>();

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = razorViewEngine.FindView(controller.ControllerContext, viewName, false);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} not found.");
                }

                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return sw.GetStringBuilder().ToString();
            }
        }

    }
}
