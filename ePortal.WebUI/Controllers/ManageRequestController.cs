using ePortal.Application.Contracts;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Data;
using System.Net;



namespace ePortal.WebUI.Controllers
{
    public class ManageRequestController : Controller
    {
        IAnnouncementService _AnnouncementService;
        private readonly ISessionService _sessionService;
        //IWFHService _WFHService;
        //IAssetDisposalService _AssetService;
        IBikerCafeService _BikerCafeService;
        //ICanteenService _CanteenService;
        IPORequest objPO;
        public ManageRequestController(IAnnouncementService AnnouncementService, IBikerCafeService BikerCafeService, ISessionService objsessionService, IPORequest _IPORequest)
        {
            _AnnouncementService = AnnouncementService;
            //_WFHService = WFHService;
            //_AssetService = AssetService;
            _BikerCafeService = BikerCafeService;
            //_CanteenService = CanteenService;
            _sessionService = objsessionService;
            objPO = _IPORequest;
        }
        [HttpGet]
        public ActionResult ManageRequest()
        {
            return View("~/Views/ManageRequest/ManageRequest.cshtml");
        }

        //[HttpGet]
        //public ActionResult RetrieveLeave()
        //{
        //    return Json(FetchLeave().ToList());
        //}
        //public List<LeaveApproval> FetchLeave()
        //{
        //    List<LeaveApproval> LeaveApplist = new List<LeaveApproval>();
        //    LeaveApps objLeave = new LeaveApps();
        //    DataTable dtLeaves = objLeave.ManageLeaveApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtLeaves.Rows.Count == 0)
        //    {
        //        //div_leave.Visible = false;
        //    }
        //    else
        //    {
        //        //div_leave.Visible = true;
        //        LeaveApproval leave = null;
        //        foreach (DataRow dr in dtLeaves.Rows)
        //        {
        //            leave = new LeaveApproval();
        //            leave.TransactionId = dr["ADTRANSACTIONID"].ToString();
        //            leave.EmpCode = dr["adempcode"].ToString();
        //            leave.EmpName = dr["empname"].ToString();
        //            leave.FromDate = dr["fromdate"].ToString();
        //            leave.ToDate = dr["todate"].ToString();
        //            leave.LeaveDays = dr["NOOFDAYS"].ToString();
        //            leave.LeaveType = dr["LEAVETYPEDETAIL"].ToString();
        //            leave.AppliedDate = dr["dateapplied"].ToString();
        //            LeaveApplist.Add(leave);
        //        }
        //    }
        //    return LeaveApplist;
        //}

        //[HttpGet]
        //public ActionResult RetrieveTransfer()
        //{
        //    return Json(FetchTransfer().ToList());
        //}
        //public List<TransferApproval> FetchTransfer()
        //{
        //    List<TransferApproval> TransferApplist = new List<TransferApproval>();
        //    CommonFunctions objtransfer = new CommonFunctions();
        //    DataTable dttransfer = objtransfer.ManageTransferApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dttransfer.Rows.Count == 0)
        //    {
        //        //div_leave.Visible = false;
        //    }
        //    else
        //    {
        //        //div_leave.Visible = true;
        //        TransferApproval Transfer = null;
        //        foreach (DataRow dr in dttransfer.Rows)
        //        {
        //            Transfer = new TransferApproval();
        //            Transfer.Process = "Transfer Process";
        //            Transfer.Count = dr["batch_no"].ToString();
        //            Transfer.Status = dr["status"].ToString();
        //            ////-- ------------Start------ - Aumento - chabges - 29-03-2024 :: SR68584--------------///
        //            //Transfer.Urlpath = "/trprocess/redirecttotmpl";

        //            Transfer.Urlpath = "/trprocess/RedirectToTMPlForApproval?BatchNo=" + Transfer.Count + "&Status=" + Transfer.Status;
        //            ////-- ------------End------ - Aumento - chabges - 29-03-2024 :: SR68584---------------///
        //            TransferApplist.Add(Transfer);
        //        }
        //    }
        //    return TransferApplist;
        //}

        //[HttpGet]
        //public ActionResult RetrieveLookSee()
        //{
        //    return Json(FetchLookSee().ToList());
        //}
        //public List<LookseeApproval> FetchLookSee()
        //{
        //    List<LookseeApproval> TransferApplist = new List<LookseeApproval>();
        //    CommonFunctions objtransfer = new CommonFunctions();
        //    DataTable dttransfer = objtransfer.ManageLookSeeApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dttransfer.Rows.Count == 0)
        //    {
        //        //div_leave.Visible = false;
        //    }
        //    else
        //    {
        //        //div_leave.Visible = true;
        //        LookseeApproval Transfer = null;
        //        foreach (DataRow dr in dttransfer.Rows)
        //        {
        //            Transfer = new LookseeApproval();
        //            Transfer.Process = "Look & See Process";
        //            Transfer.Associate = dr["empname"].ToString();
        //            //Transfer.Fromdate = dr["batch_no"].ToString();
        //            //Transfer.Todate = dr["batch_no"].ToString();
        //            Transfer.Status = dr["Status"].ToString();
        //            Transfer.Urlpath = "/trprocess/redirecttotmpl";
        //            TransferApplist.Add(Transfer);
        //        }
        //    }
        //    return TransferApplist;
        //}
        //[HttpGet]
        //public ActionResult RetrieveLookSeeSTL()
        //{
        //    return Json(FetchLookSeeSTL().ToList());
        //}
        //public List<LookseeApproval> FetchLookSeeSTL()
        //{
        //    List<LookseeApproval> TransferApplist = new List<LookseeApproval>();
        //    CommonFunctions objtransfer = new CommonFunctions();
        //    DataTable dttransfer = objtransfer.ManageLookSeeSTLApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dttransfer.Rows.Count == 0)
        //    {
        //        //div_leave.Visible = false;
        //    }
        //    else
        //    {
        //        //div_leave.Visible = true;
        //        LookseeApproval Transfer = null;
        //        foreach (DataRow dr in dttransfer.Rows)
        //        {
        //            Transfer = new LookseeApproval();
        //            Transfer.Process = "Look & See Settlement";
        //            Transfer.Associate = dr["empname"].ToString();
        //            //Transfer.Fromdate = dr["batch_no"].ToString();
        //            //Transfer.Todate = dr["batch_no"].ToString();
        //            Transfer.Status = dr["Status"].ToString();
        //            Transfer.Urlpath = "/trprocess/redirecttotmpl";
        //            TransferApplist.Add(Transfer);
        //        }
        //    }
        //    return TransferApplist;
        //}

        //[HttpGet]
        //public ActionResult RetrieveRelocation()
        //{
        //    return Json(FetchRelocation().ToList());
        //}
        //public List<LookseeApproval> FetchRelocation()
        //{
        //    List<LookseeApproval> TransferApplist = new List<LookseeApproval>();
        //    CommonFunctions objtransfer = new CommonFunctions();
        //    DataTable dttransfer = objtransfer.ManageRelocationApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dttransfer.Rows.Count == 0)
        //    {
        //        //div_leave.Visible = false;
        //    }
        //    else
        //    {
        //        //div_leave.Visible = true;
        //        LookseeApproval Transfer = null;
        //        foreach (DataRow dr in dttransfer.Rows)
        //        {
        //            Transfer = new LookseeApproval();
        //            Transfer.Process = "Relocation";
        //            Transfer.Associate = dr["empname"].ToString();
        //            //Transfer.Fromdate = dr["batch_no"].ToString();
        //            //Transfer.Todate = dr["batch_no"].ToString();
        //            Transfer.Status = dr["Status"].ToString();
        //            Transfer.Urlpath = "/trprocess/redirecttotmpl";
        //            TransferApplist.Add(Transfer);
        //        }
        //    }
        //    return TransferApplist;
        //}
        //[HttpGet]
        //public ActionResult RetrieveRelocationSTL()
        //{
        //    return Json(FetchRelocationSTL().ToList());
        //}
        //public List<LookseeApproval> FetchRelocationSTL()
        //{
        //    List<LookseeApproval> TransferApplist = new List<LookseeApproval>();
        //    CommonFunctions objtransfer = new CommonFunctions();
        //    DataTable dttransfer = objtransfer.ManageRelocationSTLApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dttransfer.Rows.Count == 0)
        //    {
        //        //div_leave.Visible = false;
        //    }
        //    else
        //    {
        //        //div_leave.Visible = true;
        //        LookseeApproval Transfer = null;
        //        foreach (DataRow dr in dttransfer.Rows)
        //        {
        //            Transfer = new LookseeApproval();
        //            Transfer.Process = "Relocation Settlement";
        //            Transfer.Associate = dr["empname"].ToString();
        //            //Transfer.Fromdate = dr["batch_no"].ToString();
        //            //Transfer.Todate = dr["batch_no"].ToString();
        //            Transfer.Status = dr["Status"].ToString();
        //            Transfer.Urlpath = "/trprocess/redirecttotmpl";
        //            TransferApplist.Add(Transfer);
        //        }
        //    }
        //    return TransferApplist;
        //}

        //[HttpPost]
        //public ActionResult RetrieveLeave(List<LeaveApprovalbulk> AppList)
        //{
        //    short retVal = 0;
        //    LeaveApps objleave = new LeaveApps();
        //    try
        //    {
        //        Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];

        //        if (Session["UserId"] == null)
        //        {
        //            retVal = -2;
        //            return Json(retVal);
        //        }
        //        foreach (LeaveApprovalbulk _WRA in AppList)
        //        {
        //            objleave.Leavebulkapproval(_WRA.TransactionId, _WRA.EmpCode, _Employee_Details.Employee_Code, _Employee_Details._EName);
        //        }
        //        retVal = 1;

        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return Json(retVal);
        //}

        //public ActionResult RetrieveSES()
        //{
        //    return Json(FetchSES().ToList());
        //}
        //public List<SESRquestapproval> FetchSES()
        //{

        //    List<SESRquestapproval> ACRApplist = new List<SESRquestapproval>();
        //    PORequest objPO = new PORequest();
        //    DataTable dtACR = objPO.ManageSESApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtACR.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {

        //        SESRquestapproval ACR = null;
        //        foreach (DataRow dr in dtACR.Rows)
        //        {
        //            ACR = new SESRquestapproval();
        //            string strDisable = string.Empty;
        //            ACR.APPTransactionId = (dr["SMHEADERID"].ToString()).ToString();
        //            ACR.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["SMHEADERID"].ToString())).ToString();
        //            ACR.EmpCode = dr["adempcode"].ToString();
        //            ACR.EmpName = dr["empname"].ToString();
        //            ACR.SESNo = dr["SES_MRN_NO"].ToString();
        //            ACR.INVAMT = dr["INVAMOUNT"].ToString();
        //            ACR.PONO = dr["PONUMBER"].ToString();
        //            ACR.Supplier_Code = dr["SUPPLIER_CODE"].ToString();
        //            ACR.Supplier_Name = dr["SUPPLIER_NAME"].ToString();
        //            ACR.AppliedDate = dr["REQDATE"].ToString();
        //            ACR.Status = dr["REQSTATUS"].ToString();
        //            ACR.SYSITEIDNAME = dr["SYSITEIDNAME"].ToString();
        //            ACRApplist.Add(ACR);
        //        }
        //    }
        //    return ACRApplist;
        //}
        //[HttpGet]
        //public ActionResult RetrieveWFH()
        //{
        //    return Json(FetchWFH().ToList());
        //}
        //public List<WFHRquestapproval> FetchWFH()
        //{

        //    List<WFHRquestapproval> WGHApplist = new List<WFHRquestapproval>();
        //    OfficialOD objOOD = new OfficialOD();
        //    DataTable dtWFH = objOOD.ManageWFHApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtWFH.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {

        //        WFHRquestapproval WFH = null;
        //        foreach (DataRow dr in dtWFH.Rows)
        //        {
        //            WFH = new WFHRquestapproval();
        //            string strDisable = string.Empty;
        //            WFH.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["asrwfhid"].ToString())).ToString();
        //            WFH.EmpCode = dr["adempcode"].ToString();
        //            WFH.EmpName = dr["empname"].ToString();
        //            WFH.REQUESTTYPE = dr["ReqTYPEDETAIL"].ToString();
        //            WFH.DURATION = dr["WFHDATE"].ToString();
        //            WFH.STARTTIME = dr["starttime"].ToString();
        //            WFH.ENDTIME = dr["endtime"].ToString();
        //            WFH.Status = dr["Status"].ToString();
        //            WFH.AppliedDate = dr["DateApplied"].ToString();
        //            WFH.APPLYFOR = dr["applyfordes"].ToString();
        //            WGHApplist.Add(WFH);
        //        }
        //    }
        //    return WGHApplist;
        //}

        //[HttpGet]
        //public ActionResult RetrievePO()
        //{
        //    return Json(FetchPO().ToList());
        //}
        //public List<PORquestapproval> FetchPO()
        //{

        //    List<PORquestapproval> POApplist = new List<PORquestapproval>();
        //    PORequest objPO = new PORequest();
        //    DataTable dtPO = objPO.ManagePOApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtPO.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {

        //        PORquestapproval PO = null;
        //        foreach (DataRow dr in dtPO.Rows)
        //        {
        //            PO = new PORquestapproval();
        //            string strDisable = string.Empty;
        //            PO.APPTransactionId = (dr["POHEADERID"].ToString()).ToString();
        //            PO.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["POHEADERID"].ToString())).ToString();
        //            PO.EmpCode = dr["adempcode"].ToString();
        //            PO.EmpName = dr["empname"].ToString();
        //            PO.PONo = dr["PONO"].ToString();
        //            PO.VendorCode = dr["VENDORID"].ToString();
        //            PO.VendorName = dr["VENDORNAME"].ToString();
        //            PO.AppliedDate = dr["REQDATE"].ToString();
        //            PO.Status = dr["REQSTATUS"].ToString();
        //            PO.HighUrgency = dr["HIGHURGENCY"].ToString();
        //            POApplist.Add(PO);
        //        }
        //    }
        //    return POApplist;
        //}
        //public ActionResult RetrievePR()
        //{
        //    return Json(FetchPR().ToList());
        //}
        //public List<PRRquestapproval> FetchPR()
        //{

        //    List<PRRquestapproval> PRApplist = new List<PRRquestapproval>();
        //    PORequest objPO = new PORequest();
        //    DataTable dtPR = objPO.ManagePRApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtPR.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {

        //        PRRquestapproval PR = null;
        //        foreach (DataRow dr in dtPR.Rows)
        //        {
        //            PR = new PRRquestapproval();
        //            string strDisable = string.Empty;
        //            PR.APPTransactionId = (dr["PRHEADERID"].ToString()).ToString();
        //            PR.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["PRHEADERID"].ToString())).ToString();
        //            PR.EmpCode = dr["adempcode"].ToString();
        //            PR.EmpName = dr["empname"].ToString();
        //            PR.PRNo = dr["INDENT_NO"].ToString();
        //            PR.PRDATE = dr["INDENT_DATE"].ToString();
        //            PR.PRAMT = dr["INDENT_AMOUNT"].ToString();
        //            PR.AppliedDate = dr["REQDATE"].ToString();
        //            PR.Status = dr["REQSTATUS"].ToString();
        //            PR.ProcessStatus = dr["PROCESS_STATUS"].ToString(); // added by Aumento as on 24072024
        //            PRApplist.Add(PR);
        //        }
        //    }
        //    return PRApplist;
        //}

        //public ActionResult RetrieveIOMAPP_DOC()
        //{
        //    return Json(FetchIOMAPP_DOC().ToList());
        //}
        //public List<DGIT_IOMRquestapproval> FetchIOMAPP_DOC()
        //{

        //    List<DGIT_IOMRquestapproval> IOMApplist = new List<DGIT_IOMRquestapproval>();
        //    PORequest objPO = new PORequest();
        //    DataTable dtPR = objPO.ManageIOMApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtPR.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {

        //        DGIT_IOMRquestapproval IOM = null;
        //        foreach (DataRow dr in dtPR.Rows)
        //        {
        //            IOM = new DGIT_IOMRquestapproval();
        //            string strDisable = string.Empty;
        //            IOM.APPTransactionId = (dr["IOMHEADERID"].ToString()).ToString();
        //            IOM.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["IOMHEADERID"].ToString())).ToString();
        //            IOM.EmpCode = dr["adempcode"].ToString();
        //            IOM.EmpName = dr["empname"].ToString();
        //            IOM.IOM_DESC = (dr["IOM_DESC"].ToString().Length < 200 ? dr["IOM_DESC"].ToString() : dr["IOM_DESC"].ToString().Substring(0, 200));//dr["IOM_DESC"].ToString();
        //            IOM.AppliedDate = dr["REQDATE"].ToString();
        //            IOM.Status = dr["REQSTATUS"].ToString();
        //            IOM.IsHighlighted = dr["ISHIGHLIGHTED"].ToString();
        //            IOM.CATDESC = dr["CATDESC"].ToString();
        //            IOM.ProcessStatus = dr["PROCESS_STATUS"].ToString(); //Aumento 12072024
        //            IOMApplist.Add(IOM);
        //        }
        //    }
        //    return IOMApplist;
        //}
        
          [HttpGet]
        public ActionResult RetrieveACR()
        {
            return Json(FetchACR().ToList());
        }
        public List<ACRRquestapproval> FetchACR()
        {

            List<ACRRquestapproval> ACRApplist = new List<ACRRquestapproval>();
            
            DataTable dtACR = objPO.ManageACRApproval(_sessionService.Get<string>("userID").ToString()).Tables[0];

            if (dtACR.Rows.Count == 0)
            {

            }
            else
            {

                ACRRquestapproval ACR = null;
                foreach (DataRow dr in dtACR.Rows)
                {
                    ACR = new ACRRquestapproval();
                    string strDisable = string.Empty;
                    ACR.APPTransactionId = dr["ACRHEADERID"].ToString().ToString();
                    ACR.TransactionId = WebUtility.UrlEncode(Encryption.Encrypt(dr["ACRHEADERID"].ToString())).ToString();
                    ACR.EmpCode = dr["adempcode"].ToString();
                    ACR.EmpName = dr["empname"].ToString();
                    ACR.ACRNo = dr["ACRNO"].ToString();
                    ACR.ACRAMT = dr["AMOUNT"].ToString();
                    ACR.PONO = dr["PO_NUMBER"].ToString();
                    ACR.Supplier_Code = dr["SUPPLIER_CODE"].ToString();
                    ACR.Supplier_Name = dr["SUPPLIER_NAME"].ToString();
                    ACR.AppliedDate = dr["REQDATE"].ToString();
                    ACR.Status = dr["REQSTATUS"].ToString();
                    ACRApplist.Add(ACR);
                }
            }
            return ACRApplist;
        }
        public ActionResult RetrieveACRRequest()
        {
            return Json(FetchACRRequest().ToList());
        }
        public List<ACRRquest> FetchACRRequest()
        {
            List<ACRRquest> Reqlist = new List<ACRRquest>();
            
            DataTable dt = objPO.ManageACRRequest(_sessionService.Get<string>("userID").ToString()).Tables[0];

            if (dt.Rows.Count != 0)
            {
                //div_leave.Visible = true;
                ACRRquest ACR = null;
                foreach (DataRow dr in dt.Rows)
                {
                    string strDisable = string.Empty;
                    ACR = new ACRRquest();
                    ACR.TransactionId = WebUtility.UrlEncode(Encryption.Encrypt(dr["ACRHEADERID"].ToString())).ToString();
                    //ACR.EmpCode = dr["adempcode"].ToString();
                    //ACR.EmpName = dr["empname"].ToString();
                    ACR.ACRNo = dr["ACRNO"].ToString();
                    ACR.ACRAMT = dr["AMOUNT"].ToString();
                    ACR.PONO = dr["PO_NUMBER"].ToString();
                    ACR.Supplier_Code = dr["SUPPLIER_CODE"].ToString();
                    ACR.Supplier_Name = dr["SUPPLIER_NAME"].ToString();
                    ACR.AppliedDate = dr["REQDATE"].ToString();
                    ACR.Status = dr["REQSTATUS"].ToString();
                    ACR.Reqname = dr["REQECODE"].ToString();
                    ACR.ProcessStatus = dr["PROCESS_STATUS"].ToString();
                    ACR.ACRattachment = dr["FILENAME"].ToString();

                    if (ACR.ProcessStatus == "0")
                    {
                        ACR.IsEdit = "1";
                    }
                    else
                    {
                        ACR.IsEdit = "0";
                    }

                    if (ACR.ProcessStatus == "0")
                    {
                        ACR.IsCancel = "1";
                    }
                    else
                    {
                        ACR.IsCancel = "0";
                    }

                    Reqlist.Add(ACR);
                }
            }
            return Reqlist;
        }
        //[HttpPost]
        //public ActionResult RetrieveWFH(List<WFHRquestapproval> AppList)
        //{
        //    short retVal = 0;
        //    try
        //    {
        //        if (Session["UserId"] == null)
        //        {
        //            retVal = -2;
        //            return Json(retVal);
        //        }
        //        List<ASRWFH_APPROVALHIS_ViewModel> AAVMList = new List<ASRWFH_APPROVALHIS_ViewModel>();
        //        foreach (WFHRquestapproval _WRA in AppList)
        //        {
        //            AAVMList.Add(new ASRWFH_APPROVALHIS_ViewModel
        //            {
        //                ASRWFHID = Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(_WRA.TransactionId))),
        //                APPADEMPCODE = Convert.ToInt64(Session["UserId"]),
        //                APPAPPROVEDDATE = DateTime.Now,
        //                APPREMARKS = "Ok",
        //                ISAPPAPPROVED = 1,
        //                RECADEMPCODE = Convert.ToInt64(Session["UserId"]),
        //                RECAPPROVEDDATE = DateTime.Now,
        //                RECREMARKS = "Ok",
        //                ISRECAPPROVED = 1,
        //            });
        //        }
        //        Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
        //        retVal = _WFHService.UpdateWFHApprovalList(AAVMList, _Employee_Details);
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return Json(retVal);
        //}



        //[HttpGet]
        //public ActionResult RetrieveLTA()
        //{
        //    return Json(FetchLTA().ToList());
        //}
        //public List<LTAApproval> FetchLTA()
        //{
        //    List<LTAApproval> Applist = new List<LTAApproval>();
        //    LTA objdb = new LTA();
        //    DataTable dt = objdb.GetPendingLTAApproval(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        LTAApproval lta = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            lta = new LTAApproval();
        //            lta.TransactionId = dr["ADLTATRANSID"].ToString();
        //            lta.EmpCode = dr["adempcode"].ToString();
        //            lta.EmpName = dr["empname"].ToString();
        //            lta.AppliedYear = dr["APPLIED_YEAR"].ToString();
        //            lta.AppliedDate = dr["dateapplied"].ToString();
        //            Applist.Add(lta);
        //        }
        //    }
        //    return Applist;
        //}

        //[HttpGet]
        //public ActionResult RetrieveUIM()
        //{
        //    return Json(FetchUIM().ToList());
        //}
        //public List<UIMApproval> FetchUIM()
        //{
        //    List<UIMApproval> Applist = new List<UIMApproval>();
        //    cITServices objdb = new cITServices();
        //    DataTable dt = objdb.GetManagereqpendingDetails(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        UIMApproval UIM = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            UIM = new UIMApproval();
        //            UIM.TransactionId = dr["SERVICEREQUESTID"].ToString();
        //            UIM.AuthTransactionId = dr["APPAUTHTRANSID"].ToString();
        //            UIM.RequestedBy = dr["REQUESTEREMP"].ToString();
        //            UIM.RequestedFor = dr["REQUESTEDFOR"].ToString();
        //            UIM.Service = dr["SERVICE"].ToString();
        //            UIM.ServiceModule = dr["SERVICEMODULE"].ToString();
        //            UIM.AppliedDate = dr["ADDDATE"].ToString();
        //            Applist.Add(UIM);
        //        }
        //    }
        //    return Applist;
        //}

        //[HttpGet]
        //public ActionResult RetrievePTC()
        //{
        //    return Json(FetchPTC().ToList());
        //}
        //public List<PTCApproval> FetchPTC()
        //{
        //    List<PTCApproval> Applist = new List<PTCApproval>();
        //    ASRAttendanceCorrection objdb = new ASRAttendanceCorrection();
        //    DataTable dt_unsorted = objdb.ManageATTCORR_PenAppList(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //    DataView DV = dt_unsorted.DefaultView;
        //    DV.Sort = "Dateadded ASC";
        //    DataTable dt = DV.ToTable();
        //    if (dt.Rows.Count > 0)
        //    {
        //        PTCApproval PTC = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            PTC = new PTCApproval();
        //            PTC.TransactionId = dr["ATTCORRECTIONID"].ToString();
        //            PTC.EmpCode = dr["ADEMPCODE"].ToString();
        //            PTC.EmpName = dr["EMPNAME"].ToString();
        //            PTC.Shift = dr["shift"].ToString();
        //            PTC.Date = dr["attcorrectiondate"].ToString();
        //            PTC.PunchIn = dr["TIMEIN"].ToString();
        //            PTC.PunchOut = dr["TIMEOUT"].ToString();
        //            PTC.AppliedDate = dr["Dateadded"].ToString();
        //            Applist.Add(PTC);
        //        }
        //    }
        //    return Applist;
        //}

        //[HttpGet]
        //public ActionResult RetrieveVC()
        //{
        //    return Json(FetchVC().ToList());
        //}
        //public List<VCApproval> FetchVC()
        //{
        //    List<VCApproval> Applist = new List<VCApproval>();
        //    Visiting objdb = new Visiting();
        //    DataTable dt = objdb.ManageVisitingCard(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        VCApproval VC = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            VC = new VCApproval();
        //            VC.TransactionId = dr["advisitingcardrequestid"].ToString();
        //            VC.EmpCode = dr["empcode"].ToString();
        //            VC.EmpName = dr["empname"].ToString();
        //            VC.Quantity = dr["quantity"].ToString();
        //            VC.Status = dr["Status"].ToString();
        //            VC.AppliedDate = dr["dateapplied"].ToString();
        //            Applist.Add(VC);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveTaxi()
        //{
        //    return Json(FetchTaxi().ToList());
        //}
        //public List<TaxiApproval> FetchTaxi()
        //{
        //    List<TaxiApproval> Applist = new List<TaxiApproval>();
        //    TaxiRequest objdb = new TaxiRequest();
        //    DataTable dt = objdb.PendingVehicleRequest(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        TaxiApproval Taxi = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            Taxi = new TaxiApproval();
        //            Taxi.TransactionId = dr["advehiclerequestid"].ToString();
        //            Taxi.EmpCode = dr["empcode"].ToString();
        //            Taxi.EmpName = dr["empname"].ToString();
        //            Taxi.FromDate = dr["dateoftravelfrom"].ToString();
        //            Taxi.ToDate = dr["dateoftravelto"].ToString();
        //            Taxi.Status = dr["Status"].ToString();
        //            Taxi.AppliedDate = dr["dateapplied"].ToString();
        //            Applist.Add(Taxi);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveOverstay()
        //{
        //    return Json(FetchOverstay().ToList());
        //}

        ////---SR48564 -- Multiple OT cancel - Change Start
        //[HttpPost]
        //public ActionResult RetrieveOverstay(List<OSApproval> AppList)
        //{
        //    short retVal = 0;
        //    Overstayhours objdb = new Overstayhours();
        //    try
        //    {
        //        Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
        //        if (Session["UserId"] == null)
        //        {
        //            retVal = -2;
        //            return Json(retVal);
        //        }
        //        foreach (OSApproval _WRA in AppList)
        //        {
        //            DataSet empdt = objdb.EditOverstayCardApproval(_WRA.TransactionId, _WRA.EmpCode);
        //            objdb.UpdateOSApproval(_WRA.EmpCode, _WRA.TransactionId, "1", "Bulk Approve", empdt.Tables[0].Rows[0]["currentshift"].ToString());
        //        }
        //        retVal = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return Json(retVal);
        //}
        ////---SR48564 -- Multiple OT cancel - Change End

        //public List<OSApproval> FetchOverstay()
        //{
        //    List<OSApproval> Applist = new List<OSApproval>();
        //    Overstayhours objdb = new Overstayhours();
        //    DataTable dt = objdb.ManageOverstayCard(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        OSApproval OS = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            OS = new OSApproval();
        //            OS.TransactionId = dr["ASROVERSTAYTRANID"].ToString();
        //            OS.EmpCode = dr["adempcode"].ToString();
        //            OS.EmpName = dr["empname"].ToString();
        //            OS.OverstayHours = dr["hours"].ToString();
        //            OS.OverstayDate = dr["overstaydate"].ToString();
        //            OS.CurrentShift = dr["currentshift"].ToString();
        //            OS.NewShift = dr["newshift"].ToString();
        //            OS.Status = dr["Status"].ToString();
        //            OS.AppliedDate = dr["dateapplied"].ToString();
        //            //---Overstay pre. Visability start
        //            OS.currOTHrs = dr["currMonthHrs"].ToString();
        //            OS.firstOTHrs = dr["firstMonthHrs"].ToString();
        //            OS.secondOTHrs = dr["secondMonthHrs"].ToString();
        //            OS.thirdOTHrs = dr["thirdMonthHrs"].ToString();
        //            //---Overstay pre. Visability end

        //            Applist.Add(OS);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveShift()
        //{
        //    return Json(FetchShift().ToList());
        //}
        //public List<SCApproval> FetchShift()
        //{
        //    List<SCApproval> Applist = new List<SCApproval>();
        //    Shiftchg objdb = new Shiftchg();
        //    DataTable dt = objdb.GetShiftReqDetails(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        SCApproval SC = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            SC = new SCApproval();
        //            SC.TransactionId = dr["asrshiftchangetranid"].ToString();
        //            SC.EmpCode = dr["empcode"].ToString();
        //            SC.EmpName = dr["empname"].ToString();
        //            SC.CurrentShift = dr["Cshift"].ToString();
        //            SC.NewShift = dr["Nshift"].ToString();
        //            SC.Status = dr["Status"].ToString();
        //            SC.AppliedDate = dr["dateapplied"].ToString();
        //            Applist.Add(SC);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveUTReq()
        //{
        //    return Json(FetchUTReq().ToList());
        //}
        //public List<UTApproval> FetchUTReq()
        //{
        //    List<UTApproval> Applist = new List<UTApproval>();
        //    UtilityDesk objdb = new UtilityDesk();
        //    DataTable dt = objdb.ApprovalOfUtReq(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        UTApproval UT = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            UT = new UTApproval();
        //            UT.TransactionId = dr["ID"].ToString();
        //            UT.EmpCode = dr["EMPCODE"].ToString();
        //            UT.EmpName = dr["EMPNAME"].ToString();
        //            UT.Catalog = dr["CATALOG"].ToString();
        //            UT.Classification = dr["CLASSIFICATION"].ToString();
        //            UT.AppliedDate = dr["LOGDATE"].ToString();
        //            Applist.Add(UT);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrievePGPReq()
        //{
        //    return Json(FetchPGPReq().ToList());
        //}
        //public List<PGPApproval> FetchPGPReq()
        //{
        //    List<PGPApproval> Applist = new List<PGPApproval>();
        //    OD objdb = new OD();
        //    DataTable dt = objdb.ApprovalOfODReq(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        PGPApproval PGP = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            PGP = new PGPApproval();
        //            PGP.TransactionId = dr["ASROUTDUTYID"].ToString();
        //            PGP.EmpCode = dr["EMPCODE"].ToString();
        //            PGP.EmpName = dr["EMPNAME"].ToString();
        //            PGP.ODType = dr["odtype"].ToString();
        //            PGP.Status = dr["status"].ToString();
        //            PGP.AppliedDate = dr["DATEADDED"].ToString();
        //            Applist.Add(PGP);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveOOD()
        //{
        //    return Json(FetchOOD().ToList());
        //}
        //public List<OODApproval> FetchOOD()
        //{
        //    List<OODApproval> Applist = new List<OODApproval>();
        //    OD objdb = new OD();
        //    DataTable dt = objdb.ApprovalOfOfficialODReq(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        OODApproval OOD = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            OOD = new OODApproval();
        //            OOD.TransactionId = dr["ASOFFODID"].ToString();
        //            OOD.EmpCode = dr["EMPCODE"].ToString();
        //            OOD.EmpName = dr["EMPNAME"].ToString();
        //            OOD.StartDate = dr["STARTDATE"].ToString();
        //            OOD.EndDate = dr["ENDDATE"].ToString();
        //            OOD.Status = dr["status"].ToString();
        //            OOD.AppliedDate = dr["ADDEDDATE"].ToString();
        //            Applist.Add(OOD);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveHWR()
        //{
        //    return Json(FetchHWR().ToList());
        //}
        //public List<HWRApproval> FetchHWR()
        //{
        //    List<HWRApproval> Applist = new List<HWRApproval>();
        //    HolidayWorking objdb = new HolidayWorking();
        //    DataTable dt = objdb.ApprovalOfIOMReq(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        HWRApproval HWR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            HWR = new HWRApproval();
        //            HWR.TransactionId = dr["REQUESTID"].ToString();
        //            HWR.EmpName = dr["EMPNAME"].ToString();
        //            HWR.StartDate = dr["STARTDATE"].ToString();
        //            HWR.EndDate = dr["ENDDATE"].ToString();
        //            HWR.AppliedDate = dr["APPDATE"].ToString();
        //            Applist.Add(HWR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveSRT()
        //{
        //    return Json(FetchSRT().ToList());
        //}
        //public List<SRTApproval> FetchSRT()
        //{
        //    List<SRTApproval> Applist = new List<SRTApproval>();
        //    SafetyRiding objdb = new SafetyRiding();
        //    DataTable dt = objdb.ApprovalOfSRReq(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        SRTApproval SRT = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            SRT = new SRTApproval();
        //            SRT.TransactionId = dr["SAFETYRIDINGREQUESTID"].ToString();
        //            SRT.EmpCode = dr["empcode"].ToString();
        //            SRT.EmpName = dr["EMPNAME"].ToString();
        //            SRT.RegistrationFor = dr["Registration"].ToString();
        //            SRT.Status = dr["Status"].ToString();
        //            SRT.AppliedDate = dr["DateAdded"].ToString();
        //            Applist.Add(SRT);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveTourReq()
        //{
        //    return Json(FetchTourReq().ToList());
        //}
        //public List<TourReqApproval> FetchTourReq()
        //{
        //    List<TourReqApproval> Applist = new List<TourReqApproval>();
        //    cTourQueries objdb = new cTourQueries();
        //    DataTable dt = objdb.GetPendingApprovalList(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        TourReqApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new TourReqApproval();
        //            TR.TransactionId = dr["ADTOURREQUESTID"].ToString();
        //            TR.EmpCode = dr["ADEMPCODE"].ToString();
        //            TR.EmpName = dr["EMPNAME"].ToString();
        //            TR.TourPeriod = dr["PERIOD"].ToString();
        //            TR.Days = dr["DAYS"].ToString();
        //            TR.AppliedDate = dr["ADDEDDATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveTourBill()
        //{
        //    return Json(FetchTourBill().ToList());
        //}
        //public List<TourBillApproval> FetchTourBill()
        //{
        //    List<TourBillApproval> Applist = new List<TourBillApproval>();
        //    cTourSettlement objdb = new cTourSettlement();
        //    DataTable dt = objdb.GetPendingApplist(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //    DataView dv = new DataView(dt);
        //    dv.RowFilter = "APPSTATUS in (0,3) AND LVL IN (1,2)";
        //    if (dv.Count > 0)
        //    {
        //        TourBillApproval TR = null;
        //        foreach (DataRow dr in dv.ToTable().Rows)
        //        {
        //            TR = new TourBillApproval();
        //            TR.TransactionId = dr["SETTLEMENTID"].ToString();
        //            TR.EmpCode = dr["ADEMPCODE"].ToString();
        //            TR.EmpName = dr["EMPNAME"].ToString();
        //            TR.TourPeriod = dr["TOUR_PERIOD"].ToString();
        //            TR.AdvanceIssued = dr["ADVANCE"].ToString();
        //            TR.ClaimAmt = dr["SETTDAYTOTAL"].ToString();
        //            TR.AppliedDate = dr["APL_DATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveTrngResch()
        //{
        //    return Json(FetchTrngResch().ToList());
        //}
        //public List<TrngReschApproval> FetchTrngResch()
        //{
        //    List<TrngReschApproval> Applist = new List<TrngReschApproval>();
        //    HrIntrnlTraining objdb = new HrIntrnlTraining();
        //    DataTable dt = objdb.GetPendingReScheduleRequest(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        TrngReschApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new TrngReschApproval();
        //            TR.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["REQUESTID"].ToString()));
        //            TR.EmpCode = dr["ADEMPCODE"].ToString();
        //            TR.EmpName = dr["EMP_NAME"].ToString();
        //            TR.Training = dr["TRAINING"].ToString();
        //            TR.TrainingPeriod = dr["TRAINING_DATE"].ToString();
        //            TR.RescheduleNo = dr["TOTAL"].ToString();
        //            TR.AppliedDate = dr["APPLICATION_DATE"].ToString();
        //            TR.BatchId = dr["HRTRAININGTRANSACTIONID"].ToString();
        //            TR.AppAuth = dr["APPAUTH"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveTrngEval()
        //{
        //    return Json(FetchTrngEval().ToList());
        //}
        //public List<TrngEvalApproval> FetchTrngEval()
        //{
        //    List<TrngEvalApproval> Applist = new List<TrngEvalApproval>();
        //    TrainingCalendar objdb = new TrainingCalendar();
        //    DataTable dt = objdb.GetEvaluationList(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //    DataView dv = new DataView(dt);
        //    dv.RowFilter = "ISEVALUATIONFILLED='0'";
        //    if (dv.Count > 0)
        //    {
        //        TrngEvalApproval TR = null;
        //        foreach (DataRow dr in dv.ToTable().Rows)
        //        {
        //            TR = new TrngEvalApproval();
        //            TR.TransactionId = dr["TRANSID"].ToString();
        //            TR.EmpCode = dr["EMPCODE"].ToString();
        //            TR.EmpName = dr["ENAME"].ToString();
        //            TR.Training = dr["TRAINING"].ToString();
        //            TR.TrainingPeriod = dr["TrainingPeriod"].ToString();
        //            TR.TrainingTime = dr["TrainingTime"].ToString();
        //            TR.Venue = dr["TRAININGVENUE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveStationary()
        //{
        //    return Json(FetchStationary().ToList());
        //}
        //public List<StationaryApproval> FetchStationary()
        //{
        //    List<StationaryApproval> Applist = new List<StationaryApproval>();
        //    Stationary objdb = new Stationary();
        //    DataTable dt = objdb.GetPendingApprovalList(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        StationaryApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new StationaryApproval();
        //            TR.TransactionId = dr["ADSTYREQUESTID"].ToString();
        //            TR.EmpCode = dr["ADEMPCODE"].ToString();
        //            TR.EmpName = dr["EMPNAME"].ToString();
        //            TR.AppliedDate = dr["ADDEDDATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveQMSDoc()
        //{
        //    return Json(FetchQMSDoc().ToList());
        //}
        //public List<QMSDocApproval> FetchQMSDoc()
        //{
        //    List<QMSDocApproval> Applist = new List<QMSDocApproval>();
        //    QMS objdb = new QMS();
        //    DataTable dt = objdb.GetPendingApprovalList(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        QMSDocApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new QMSDocApproval();
        //            TR.TransactionId = dr["ISODOCID"].ToString();
        //            TR.EmpCode = dr["REQEMPCODE"].ToString();
        //            TR.EmpName = dr["EMPNAME"].ToString();
        //            TR.ReqType = dr["CHANGETYPE"].ToString();
        //            TR.DocNo = dr["DOCNO"].ToString();
        //            TR.DocTitle = dr["DOCTITLE"].ToString();
        //            TR.Status = dr["STATUSDESCRIPTION"].ToString();
        //            TR.AppliedDate = dr["DATEADDED"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveEmpConf()
        //{
        //    return Json(FetchEmpConf().ToList());
        //}
        //public List<EmpConfApproval> FetchEmpConf()
        //{
        //    List<EmpConfApproval> Applist = new List<EmpConfApproval>();
        //    HRConfirmationReview objdb = new HRConfirmationReview();
        //    DataTable dt = objdb.GetConfirmationpending(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        EmpConfApproval TR = null;
        //        //ViewModels.Employee_Details empDetails = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new EmpConfApproval();
        //            TR.EmpCode = dr["ADEMPCODE"].ToString();
        //            TR.EmpName = dr["ENAME"].ToString();
        //            TR.JoiningDate = dr["REGDATE"].ToString();
        //            TR.ConfirmationDate = dr["DOC"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveITAsset()
        //{
        //    return Json(FetchITAsset().ToList());
        //}
        //public List<ITAssetApproval> FetchITAsset()
        //{
        //    List<ITAssetApproval> Applist = new List<ITAssetApproval>();
        //    NewAsset objdb = new NewAsset();
        //    DataTable dt = objdb.GETNEWITASSETPENDINGAPPROVAL(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        ITAssetApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new ITAssetApproval();
        //            TR.TransactionId = dr["ASSETREQUESTID"].ToString();
        //            TR.EmpName = dr["EMPLOYEENAME"].ToString();
        //            TR.RequestType = dr["REQUESTTYPE"].ToString();
        //            TR.Status = dr["PROCESSSTATUSNAME"].ToString();
        //            TR.AppliedDate = dr["ADDEDDATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveITAssetTrf()
        //{
        //    return Json(FetchITAssetTrf().ToList());
        //}
        //public List<ITAssetTrfApproval> FetchITAssetTrf()
        //{
        //    List<ITAssetTrfApproval> Applist = new List<ITAssetTrfApproval>();
        //    AssetTransfer objdb = new AssetTransfer();
        //    DataTable dt = objdb.GETASSETTRANSFERAPPROVALREQ(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        ITAssetTrfApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new ITAssetTrfApproval();
        //            TR.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["ASSETTRANSFERREQUESTID"].ToString()));
        //            TR.EmpCode = dr["EMPLOYEEID"].ToString();
        //            TR.EmpName = dr["ENAME"].ToString();
        //            TR.Status = dr["REQSTATUS"].ToString();
        //            TR.AppliedDate = dr["REQUESTDATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveITConsumable()
        //{
        //    return Json(FetchITConsumable().ToList());
        //}
        //public List<ITConsumableApproval> FetchITConsumable()
        //{
        //    List<ITConsumableApproval> Applist = new List<ITConsumableApproval>();
        //    ConsumableIssuance objdb = new ConsumableIssuance();
        //    DataTable dt = objdb.GETCONSUMABLEAPPROVALREQ(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        ITConsumableApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new ITConsumableApproval();
        //            TR.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["CONSUMABLEREQUESTID"].ToString()));
        //            TR.EmpCode = dr["EMPLOYEEID"].ToString();
        //            TR.EmpName = dr["ENAME"].ToString();
        //            TR.Status = dr["REQSTATUS"].ToString();
        //            TR.AppliedDate = dr["REQUESTDATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveITBreakage()
        //{
        //    return Json(FetchITBreakage().ToList());
        //}
        //public List<ITBreakageApproval> FetchITBreakage()
        //{
        //    List<ITBreakageApproval> Applist = new List<ITBreakageApproval>();
        //    HardwareBreakage objdb = new HardwareBreakage();
        //    DataTable dt = objdb.GETHARDWAREAPPROVALREQ(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        ITBreakageApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new ITBreakageApproval();
        //            TR.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["HARDWAREREQUESTID"].ToString()));
        //            TR.EmpCode = dr["EMPLOYEEID"].ToString();
        //            TR.EmpName = dr["ENAME"].ToString();
        //            TR.Status = dr["REQSTATUS"].ToString();
        //            TR.AppliedDate = dr["REQUESTDATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveVendorApp()
        //{
        //    return Json(FetchVendorApp().ToList());
        //}
        //public List<VendorApproval> FetchVendorApp()
        //{
        //    List<VendorApproval> Applist = new List<VendorApproval>();
        //    VendorMaster objdb = new VendorMaster();
        //    DataTable dt = objdb.GETVENDORAPPROVALGREQUEST(System.Web.HttpContext.Current.Session["UserId"].ToString(), "0", "", "", "").Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        VendorApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new VendorApproval();
        //            TR.TransactionId = dr["VENDORHEADERID"].ToString();
        //            TR.Employee = dr["EMPLOYEE"].ToString();
        //            TR.RequestType = dr["REQUESTTYPEDESC"].ToString();
        //            TR.AccountGroup = dr["VENDORACCOUNTGRP"].ToString();
        //            TR.Vendor = dr["VENDORNAME"].ToString();
        //            TR.Status = dr["HISTORYSTATUS"].ToString();
        //            TR.AppliedDate = dr["REQUESTDATE"].ToString();
        //            TR.EncryptedId = Server.UrlEncode(Encryption.Encrypt(dr["VENDORHEADERID"].ToString()));
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveMaterialCrtApp()
        //{
        //    return Json(FetchMaterialCrtApp().ToList());
        //}
        //public List<MaterialCrtApproval> FetchMaterialCrtApp()
        //{
        //    List<MaterialCrtApproval> Applist = new List<MaterialCrtApproval>();
        //    cMasterQueries objdb = new cMasterQueries();
        //    DataTable dt = objdb.GetMMApprovalCreationRequestList(Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"])).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        MaterialCrtApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new MaterialCrtApproval();
        //            TR.TransactionId = dr["MMHEADERDETAILID"].ToString();
        //            TR.ECode = dr["REQUESTERID"].ToString();
        //            TR.EName = dr["REQUESTORNAME"].ToString();
        //            TR.Status = dr["STATUS"].ToString();
        //            TR.AppliedDate = dr["REQUESTDATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveMaterialExtApp()
        //{
        //    return Json(FetchMaterialExtApp().ToList());
        //}
        //public List<MaterialExtApproval> FetchMaterialExtApp()
        //{
        //    List<MaterialExtApproval> Applist = new List<MaterialExtApproval>();
        //    cMasterQueries objdb = new cMasterQueries();
        //    DataTable dt = objdb.GetMMApprovalExtendRequestList(Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"])).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        MaterialExtApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new MaterialExtApproval();
        //            TR.TransactionId = dr["MMHEADERDETAILID"].ToString();
        //            TR.ECode = dr["REQUESTERID"].ToString();
        //            TR.EName = dr["REQUESTORNAME"].ToString();
        //            TR.Status = dr["STATUS"].ToString();
        //            TR.AppliedDate = dr["REQUESTDATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveVendorBlockApp()
        //{
        //    return Json(FetchVendorBlockApp().ToList());
        //}
        //public List<VendorBlockApproval> FetchVendorBlockApp()
        //{
        //    List<VendorBlockApproval> Applist = new List<VendorBlockApproval>();
        //    VendorMaster objdb = new VendorMaster();
        //    DataTable dt = objdb.GETVENDORBLOCAPPROVALGREQUEST(System.Web.HttpContext.Current.Session["UserId"].ToString(), "0", "", "", "").Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        VendorBlockApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            if (dr["PSTATUSID"].ToString() == "2")
        //            {
        //                TR = new VendorBlockApproval();
        //                TR.TransactionId = dr["VENDORBLOCKHEADERID"].ToString();
        //                TR.EncryptedId = Server.UrlEncode(Encryption.Encrypt(dr["VENDORBLOCKHEADERID"].ToString()));
        //                TR.Employee = dr["EMPLOYEE"].ToString();
        //                TR.RequestType = dr["REQUESTTYPEDESC"].ToString();
        //                TR.RequestCategory = dr["REQUESTCATEDESC"].ToString();
        //                TR.Status = dr["HISTORYSTATUS"].ToString();
        //                TR.AppliedDate = dr["REQUESTDATE"].ToString();
        //                Applist.Add(TR);
        //            }
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveJDApp()
        //{
        //    return Json(FetchJDApp().ToList());
        //}
        //public List<JDApproval> FetchJDApp()
        //{
        //    List<JDApproval> Applist = new List<JDApproval>();
        //    Recruitment objdb = new Recruitment();
        //    DataTable dt = objdb.Getpendingappreqlist(System.Web.HttpContext.Current.Session["UserId"].ToString(), "0").Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        JDApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new JDApproval();
        //            TR.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["JDID"].ToString()));
        //            TR.InitatedBy = dr["REQNAME"].ToString();
        //            TR.Position = dr["POSDESC"].ToString();
        //            TR.Status = dr["JDAPPSTATUS"].ToString();
        //            TR.AppliedDate = dr["REQDATE"].ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveMRApp()
        //{
        //    return Json(FetchMRApp().ToList());
        //}
        //public List<MRApproval> FetchMRApp()
        //{
        //    List<MRApproval> Applist = new List<MRApproval>();
        //    Recruitment objdb = new Recruitment();
        //    DataTable dt = objdb.GetMrPendingApprovalByEmpId(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        MRApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new MRApproval();
        //            TR.TransactionId = dr["MRID"].ToString();
        //            TR.InitatedBy = dr["REQNAME"].ToString();
        //            TR.Position = dr["POSDESC"].ToString();
        //            TR.Status = dr["VIEWSTATUS"].ToString();
        //            TR.AppliedDate = dr["MRADDEDDATE"].ToString();

        //            if (dr["STATUS"].ToString() == "5" || dr["STATUS"].ToString() == "6")
        //            {
        //                TR.URL = "Recruitment/MrJdEvalApprovalForm.aspx?MRID=" + Server.UrlEncode(Encryption.Encrypt(dr["MRID"].ToString())).ToString();
        //            }
        //            else
        //            {
        //                TR.URL = "Recruitment/MRApprovalForm.aspx?MRID=" + Server.UrlEncode(Encryption.Encrypt(dr["MRID"].ToString())).ToString();
        //            }

        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveResigApp()
        //{
        //    return Json(FetchResigApp().ToList());
        //}
        //public List<ResigApproval> FetchResigApp()
        //{
        //    List<ResigApproval> Applist = new List<ResigApproval>();
        //    Separation objdb = new Separation();
        //    DataTable dt = objdb.HRSPpendingApproval(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        ResigApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new ResigApproval();
        //            TR.TransactionId = dr["RESIGNATIONID"].ToString();
        //            TR.ECode = dr["ADEMPCODE"].ToString();
        //            TR.EName = dr["EMPNAME"].ToString();
        //            TR.RelievingDate = dr["RELIEVING_DATE_SELF"].ToString();
        //            TR.Status = dr["status"].ToString();
        //            TR.AppliedDate = dr["RESIGNED_DATE"].ToString();

        //            TR.URL = "Separation/ApprovalForm.aspx?RESIGID=" + Server.UrlEncode(Encryption.Encrypt(dr["RESIGNATIONID"].ToString())).ToString();

        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}
        //[HttpGet]
        //public ActionResult GetAnnouncementForApproval()
        //{
        //    List<AnnouncementApproval> iList = new List<AnnouncementApproval>();
        //    try
        //    {
        //        if (Session["UserId"] == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        IEnumerable<AnnouncementApprovalViewModel> AAVMList = _AnnouncementService.GetAnnouncementApproval_List(Convert.ToInt64(Session["UserId"]));
        //        if (AAVMList.Count() > 0)
        //        {
        //            foreach (AnnouncementApprovalViewModel _obj in AAVMList)
        //            {
        //                iList.Add(new AnnouncementApproval
        //                {
        //                    TransactionId = _obj.ATTACHMENTID.ToString(),
        //                    EmpCode = _obj.INITIATED_BY.ToString(),
        //                    EmpName = _obj.INITIATED_User.ToString().Split('(')[0].Trim(),
        //                    Subject = _obj.AnnouncementTrn.SUBJECT,
        //                    Status = _obj.STATUS == 2 || _obj.AnnouncementTrn.STATUS == 3 ? "Pending" : "Approved",
        //                    AppliedDate = _obj.INITIATED_DATE.ToString("dd-MMM-yyyy")
        //                });
        //            }
        //        }
        //        return Json(iList);
        //    }
        //    catch (Exception ex)
        //    {
        //        iList = new List<AnnouncementApproval>();
        //        return Json(iList);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public ActionResult GetAssetDisposalForApproval()
        //{
        //    List<AssetDisposalApproval> iList = new List<AssetDisposalApproval>();
        //    try
        //    {
        //        if (Session["UserId"] == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
        //        IEnumerable<AssetDisposalViewModel> AAVMList = _AssetService.GetApprovalListByUser(_Employee_Details);
        //        if (AAVMList.Count() > 0)
        //        {
        //            foreach (AssetDisposalViewModel _obj in AAVMList)
        //            {
        //                iList.Add(new AssetDisposalApproval
        //                {
        //                    TransactionId = _obj.DISPOSALHEADERID.ToString(),// Server.UrlEncode(Encryption.Encrypt(_obj.DISPOSALHEADERID.ToString())).ToString(), ////_obj.DISPOSALHEADERID.ToString(),
        //                    EmpCode = _obj.ADEMPCODE.ToString(),
        //                    EmpName = _obj.EMP_NAME.ToString(),
        //                    RequestType = _obj.ASSETTYPE,
        //                    Status = _obj.PROCESSSTATUS,
        //                    AssetCount = _obj.AssetCount.ToString(),
        //                    AppliedDate = _obj.DATEADDED.ToString("dd-MMM-yyyy")
        //                });
        //            }
        //        }
        //        return Json(iList);
        //    }
        //    catch (Exception ex)
        //    {
        //        iList = new List<AssetDisposalApproval>();
        //        return Json(iList);
        //    }
        //}
        //[HttpGet]
        //public ActionResult RetrieveVR()
        //{
        //    return Json(FetchVR().ToList());
        //}
        //public List<VRApproval> FetchVR()
        //{
        //    List<VRApproval> Applist = new List<VRApproval>();
        //    VMS objdb = new VMS();
        //    DataTable dt = objdb.ManageVisitorRequest(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dt.Rows.Count > 0)
        //    {
        //        VRApproval VR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            VR = new VRApproval();
        //            VR.TransactionId = dr["visitreqid"].ToString();
        //            VR.EmpCode = dr["adempcode"].ToString();
        //            VR.EmpName = dr["empname"].ToString();
        //            VR.AppointmentDate = dr["appointment_date"].ToString();
        //            VR.AppointmentTime = dr["appointment_time"].ToString();
        //            VR.Status = dr["Status"].ToString();
        //            VR.AppliedDate = dr["dateapplied"].ToString();
        //            Applist.Add(VR);
        //        }
        //    }
        //    return Applist;
        //}

        //public ActionResult RetrieveIOMApp()
        //{
        //    return Json(FetchIOMApp().ToList());
        //}
        //public List<IOMApproval> FetchIOMApp()
        //{
        //    List<IOMApproval> Applist = new List<IOMApproval>();
        //    IOM objdb = new IOM();
        //    DataTable dt = objdb.GET_MANAGEIOMDETAILS(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    if (dt.Rows.Count > 0)
        //    {
        //        IOMApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new IOMApproval();
        //            TR.TransactionId = dr["IOMID"].ToString();
        //            TR.Agreement = dr["AGREEMENTTYPEDESC"].ToString();
        //            TR.Contract = dr["CONTRACTTYPEDESC"].ToString();
        //            TR.EffectiveDate = dr["EFFECTIVEDATE"].ToString();
        //            TR.ExpiryDate = dr["EXPIRYDATE"].ToString();
        //            TR.Vendor = dr["VENDORNAME"].ToString();
        //            TR.RequestedBy = dr["REQUESTDBY"].ToString();
        //            TR.Status = dr["STATUSNAME"].ToString();
        //            TR.URL = "IOM/IOMApprovalForm.aspx?IOMID=" + Server.UrlEncode(Encryption.Encrypt(dr["IOMID"].ToString())).ToString() + "&DESIGN=" + Server.UrlEncode(Encryption.Encrypt(dr["EMPDESIGNATION"].ToString())).ToString();
        //            Applist.Add(TR);
        //        }
        //    }
        //    return Applist;
        //}
        //// Meeting Food
        //[HttpGet]
        //public ActionResult RetrieveMeetingFoodBooking()
        //{
        //    return Json(FetchMeetingFoodBooking().ToList());
        //}
        //public List<GuestMealApproval> FetchMeetingFoodBooking()
        //{
        //    List<GuestMealApproval> GMAlist = new List<GuestMealApproval>();
        //    BikerCafe objBK = new BikerCafe();
        //    DataTable dtGMA = objBK.ManageMeetingFoodApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtGMA.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {
        //        GuestMealApproval GMA = null;
        //        foreach (DataRow dr in dtGMA.Rows)
        //        {
        //            GMA = new GuestMealApproval();
        //            string strDisable = string.Empty;
        //            GMA.TransactionId = dr["BC_MEETINGFOOD_BOOKINGID"].ToString(); //Server.UrlEncode(Encryption.Encrypt(dr["BC_GUEST_BOOKINGID"].ToString())).ToString();
        //            GMA.EmpCode = dr["ADEMPCODE"].ToString();
        //            GMA.EmpName = dr["EMPNAME"].ToString();
        //            GMA.BookingDate = dr["BOOKING_DATE"].ToString();
        //            GMA.Status = dr["BOOKING_STATUS"].ToString();
        //            GMA.AppliedDate = dr["ADDEDDATE"].ToString();
        //            GMA.GuestCount = dr["ITEMCOUNT"].ToString();
        //            GMA.TotalAmt = dr["TOTALAMT"].ToString();
        //            GMAlist.Add(GMA);
        //        }
        //    }
        //    return GMAlist;
        //}

        //[HttpGet]
        //public ActionResult RetrieveGuestMealBooking()
        //{
        //    return Json(FetchGuestMealBooking().ToList());
        //}
        //public List<GuestMealApproval> FetchGuestMealBooking()
        //{
        //    List<GuestMealApproval> GMAlist = new List<GuestMealApproval>();
        //    BikerCafe objBK = new BikerCafe();
        //    DataTable dtGMA = objBK.ManageGuestMealApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtGMA.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {
        //        GuestMealApproval GMA = null;
        //        foreach (DataRow dr in dtGMA.Rows)
        //        {
        //            GMA = new GuestMealApproval();
        //            string strDisable = string.Empty;
        //            GMA.TransactionId = dr["BC_GUEST_BOOKINGID"].ToString(); //Server.UrlEncode(Encryption.Encrypt(dr["BC_GUEST_BOOKINGID"].ToString())).ToString();
        //            GMA.EmpCode = dr["ADEMPCODE"].ToString();
        //            GMA.EmpName = dr["EMPNAME"].ToString();
        //            GMA.MealType = dr["MEAL_TYPE_DESC"].ToString();
        //            GMA.Meal = dr["MEAL_NAME"].ToString();
        //            GMA.BookingDate = dr["BOOKING_DATE"].ToString();
        //            GMA.Status = dr["BOOKING_STATUS"].ToString();
        //            GMA.AppliedDate = dr["ADDEDDATE"].ToString();
        //            GMA.GuestCount = dr["GUESTCOUNT"].ToString();
        //            GMA.TotalAmt = dr["TOTALAMT"].ToString();
        //            GMAlist.Add(GMA);
        //        }
        //    }
        //    return GMAlist;
        //}

        //[HttpPost]
        //public ActionResult RetrieveGuestMealBooking(List<GuestMealApproval> AppList)
        //{
        //    short retVal = 0;
        //    try
        //    {
        //        if (Session["UserId"] == null)
        //        {
        //            retVal = -2;
        //            return Json(retVal);
        //        }
        //        List<GuestMealBookingTrnViewModel> GMBTList = new List<GuestMealBookingTrnViewModel>();
        //        foreach (GuestMealApproval _GMA in AppList)
        //        {
        //            GMBTList.Add(new GuestMealBookingTrnViewModel
        //            {
        //                BC_GUEST_BOOKINGID = Convert.ToInt64(_GMA.TransactionId), //Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(_GMA.TransactionId))),
        //                strBOOKED_DATE = _GMA.BookingDate,
        //                APPROVER_ECODE = Convert.ToInt64(Session["UserId"]),
        //                APPROVE_REMARK = "",
        //                APPROVE_STATUS = 1,
        //                APPROVE_DATE = DateTime.Now,
        //                LSTMODIFIEDBY = Convert.ToInt64(Session["UserId"]),
        //                LSTMODDATE = DateTime.Now,
        //            });
        //        }
        //        retVal = _BikerCafeService.BulkGuestMealApproval(GMBTList);
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return Json(retVal);
        //}

        ////// Canteen Service
        //[HttpGet]
        //public ActionResult RetrieveCanteenGuestMealBooking()
        //{
        //    return Json(FetchCanteenGuestMealBooking().ToList());
        //}
        //public List<GuestMealApproval> FetchCanteenGuestMealBooking()
        //{
        //    List<GuestMealApproval> GMAlist = new List<GuestMealApproval>();
        //    CanteenMealService objBK = new CanteenMealService();
        //    DataTable dtGMA = objBK.ManageGuestMealApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtGMA.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {
        //        GuestMealApproval GMA = null;
        //        foreach (DataRow dr in dtGMA.Rows)
        //        {
        //            GMA = new GuestMealApproval();
        //            string strDisable = string.Empty;
        //            GMA.TransactionId = dr["CSD_GUEST_BOOKINGID"].ToString(); //Server.UrlEncode(Encryption.Encrypt(dr["BC_GUEST_BOOKINGID"].ToString())).ToString();
        //            GMA.EmpCode = dr["ADEMPCODE"].ToString();
        //            GMA.EmpName = dr["EMPNAME"].ToString();
        //            GMA.MealType = dr["MEAL_TYPE_DESC"].ToString();
        //            GMA.Meal = dr["MEAL_NAME"].ToString();
        //            GMA.BookingDate = dr["BOOKING_DATE"].ToString();
        //            GMA.Status = dr["BOOKING_STATUS"].ToString();
        //            GMA.AppliedDate = dr["ADDEDDATE"].ToString();
        //            GMA.GuestCount = dr["GUESTCOUNT"].ToString();
        //            GMA.TotalAmt = dr["TOTALAMT"].ToString();
        //            GMAlist.Add(GMA);
        //        }
        //    }
        //    return GMAlist;
        //}

        //[HttpPost]
        //public ActionResult RetrieveCanteenGuestMealBooking(List<GuestMealApproval> AppList)
        //{
        //    short retVal = 0;
        //    try
        //    {
        //        if (Session["UserId"] == null)
        //        {
        //            retVal = -2;
        //            return Json(retVal);
        //        }
        //        List<CSDGuestMealBookingTrnViewModel> GMBTList = new List<CSDGuestMealBookingTrnViewModel>();
        //        foreach (GuestMealApproval _GMA in AppList)
        //        {
        //            GMBTList.Add(new CSDGuestMealBookingTrnViewModel
        //            {
        //                CSD_GUEST_BOOKINGID = Convert.ToInt64(_GMA.TransactionId), //Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(_GMA.TransactionId))),
        //                strBOOKED_DATE = _GMA.BookingDate,
        //                APPROVER_ECODE = Convert.ToInt64(Session["UserId"]),
        //                APPROVE_REMARK = "",
        //                APPROVE_STATUS = 1,
        //                APPROVE_DATE = DateTime.Now,
        //                LSTMODIFIEDBY = Convert.ToInt64(Session["UserId"]),
        //                LSTMODDATE = DateTime.Now,
        //            });
        //        }
        //        retVal = _CanteenService.BulkGuestMealApproval(GMBTList);
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return Json(retVal);
        //}

        ////--------------------------------------------------------------------------------------------------------------------------------------------------------
        ////-----SR39197 IT GRC Changes Start(Aumento)-------------------------------------------------------------------------------------------------------------------------------------------------------
        ////-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //[HttpGet]
        //public JsonResult GetUARListData()
        //{

        //    List<UAR_AccessReviewViewModel> UARList = new List<UAR_AccessReviewViewModel>();
        //    // string UARList = string.Empty;
        //    UARService _UARService = new UARService();
        //    int logincode = int.Parse(Session["Userid"].ToString());
        //    UARList = _UARService.GetUARList(logincode, "0", "", "");
        //    System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    string sJSON = oSerializer.Serialize(UARList);
        //    return Json(sJSON);
        //}
        ////--------------------------------------------------------------------------------------------------------------------------------------------------------
        ////-----SR39197 IT GRC Changes End(Aumento)-------------------------------------------------------------------------------------------------------------------------------------------------------
        ////-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //[HttpGet]
        //public ActionResult RetrieveCommunication()
        //{
        //    return Json(FetchCommunicationRequest().ToList());
        //}
        //public List<CommunicationRquestApproval> FetchCommunicationRequest()
        //{

        //    List<CommunicationRquestApproval> CommApplist = new List<CommunicationRquestApproval>();
        //    var dtPO = _AnnouncementService.CommunicationApprovalList(Convert.ToInt64(System.Web.HttpContext.Current.Session["UserId"].ToString()));
        //    if (dtPO.Count > 0)
        //    {
        //        CommunicationRquestApproval CRA = null;
        //        foreach (var dr in dtPO)
        //        {
        //            CRA = new CommunicationRquestApproval();
        //            string strDisable = string.Empty;
        //            CRA.APPTransactionId = dr.COMMUNICATIONID.ToString();
        //            CRA.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr.COMMUNICATIONID.ToString())).ToString();
        //            CRA.EmpCode = dr.CREATED_BY.ToString();
        //            CRA.EmpName = dr.CREATED_BY_NAME.ToString();
        //            CRA.Subject = dr.SUBJECT.ToString();
        //            CRA.Category = dr.CATEGORY.ToString();
        //            CRA.RequestType = dr.REQUEST_TYPE.ToString();
        //            CRA.AppliedDate = dr.CREATED_DATE.ToString();
        //            CRA.Status = dr.STATUS.ToString();
        //            CRA.ProcessStatus = dr.PROCESS_STATUS.ToString();
        //            CommApplist.Add(CRA);
        //        }
        //    }
        //    return CommApplist;
        //}

        //public ActionResult RetrieveClearanceApp() //Clearance Approval Process
        //{
        //    List<ClearanceApproval> Applist = new List<ClearanceApproval>();
        //    Separation objdb = new Separation();
        //    DataTable dt = objdb.GET_MANAGEDPTCLRFORMDETAILS(System.Web.HttpContext.Current.Session["UserId"].ToString(), "0");

        //    if (dt.Rows.Count > 0)
        //    {
        //        ClearanceApproval TR = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TR = new ClearanceApproval();
        //            TR.TransactionId = dr["RESIGNATIONID"].ToString();
        //            TR.ECode = dr["ADEMPCODE"].ToString();
        //            TR.EName = dr["EMPNAME"].ToString();
        //            TR.RelievingDate = dr["RELIEVING_DATE_AUTH"].ToString();
        //            TR.Status = dr["DPTCLSTATUS"].ToString();

        //            //TR.URL = "Separation/ApprovalForm.aspx?RESIGID=" + Server.UrlEncode(Encryption.Encrypt(dr["RESIGNATIONID"].ToString())).ToString();
        //            TR.URL = "Separation/ManageResignationRequest.aspx";

        //            Applist.Add(TR);
        //        }
        //    }


        //    return Json(Applist);
        //}

        ////Below added by aumento for Creative Master=============================
        //public ActionResult RetrieveCRMPP_Detail()
        //{
        //    return Json(FetchCRMPP_Detail().ToList());
        //}
        //public List<CreativeMasterApproval> FetchCRMPP_Detail()
        //{

        //    List<CreativeMasterApproval> CreativeMasApplist = new List<CreativeMasterApproval>();
        //    PORequest objPO = new PORequest();
        //    DataTable dtPR = objPO.ManageCMAApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtPR.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {
        //        CreativeMasterApproval CRM = null;
        //        foreach (DataRow dr in dtPR.Rows)
        //        {
        //            CRM = new CreativeMasterApproval();
        //            string strDisable = string.Empty;
        //            CRM.MAPPINGATTACHMENTID = dr["MAPPINGATTACHMENTID"].ToString();
        //            CRM.ATTACHMENTID = dr["ATTACHMENTID"].ToString();
        //            CRM.INITIATEDBY = dr["INITIATED_BY"].ToString();
        //            CRM.INITIATEDDATE = dr["INITIATED_DATE"].ToString();
        //            CRM.SUBJECT = dr["SUBJECT"].ToString();
        //            CRM.STATUS = dr["STATUS"].ToString();
        //            CreativeMasApplist.Add(CRM);
        //        }
        //    }
        //    return CreativeMasApplist;
        //}
        ////=================Creative Master Change End=========================================


        ////Below added by aumento for Calendar Master=============================
        //public ActionResult RetrieveCalMasAPP_Detail()
        //{
        //    return Json(FetchCalMasAPP_Detail().ToList());
        //}

        //public List<CalenderMasterApproval> FetchCalMasAPP_Detail()
        //{
        //    List<CalenderMasterApproval> CalenderMasApplist = new List<CalenderMasterApproval>();
        //    PORequest objPO = new PORequest();
        //    DataTable dtPR = objPO.ManageCalMasApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtPR.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {
        //        CalenderMasterApproval CRM = null;
        //        foreach (DataRow dr in dtPR.Rows)
        //        {
        //            CRM = new CalenderMasterApproval();
        //            string strDisable = string.Empty;
        //            CRM.CALENDERMAPPINGID = dr["CALENDERMAPPINGID"].ToString();
        //            CRM.SRNO = dr["Srno"].ToString();
        //            CRM.FINANCIALYEAR = dr["Financialyear"].ToString();
        //            CRM.CALENDER = dr["Calender"].ToString();
        //            CRM.LOCATION = dr["Location"].ToString();
        //            CRM.INITIATEDBY = dr["CREATED_BY"].ToString();
        //            CRM.STATUS = dr["Status"].ToString();
        //            CalenderMasApplist.Add(CRM);
        //        }
        //    }
        //    return CalenderMasApplist;
        //}
        ////==========================================================

        ////Below added by aumento for ISMS Master=============================
        //public ActionResult RetrieveISMSMasAPP_Detail()
        //{
        //    return Json(FetchISMSMasAPP_Detail().ToList());
        //}
        //public List<ISMSMasterApproval> FetchISMSMasAPP_Detail()
        //{
        //    List<ISMSMasterApproval> ISMSMasApplist = new List<ISMSMasterApproval>();
        //    PORequest objPO = new PORequest();
        //    DataTable dtPR = objPO.ManageISMSMasApproval(System.Web.HttpContext.Current.Session["UserId"].ToString()).Tables[0];

        //    if (dtPR.Rows.Count == 0)
        //    {

        //    }
        //    else
        //    {
        //        ISMSMasterApproval CRM = null;
        //        foreach (DataRow dr in dtPR.Rows)
        //        {
        //            CRM = new ISMSMasterApproval();
        //            string strDisable = string.Empty;
        //            CRM.ISMSMAPPINGID = dr["ISMSMAPPINGID"].ToString();
        //            CRM.SRNO = dr["SRNO"].ToString();
        //            CRM.ISMS_UPLOAD = dr["ISMS_UPLOAD"].ToString();
        //            CRM.INITIATEDBY = dr["CREATED_BY"].ToString();
        //            CRM.STATUS = dr["Status"].ToString();

        //            ISMSMasApplist.Add(CRM);
        //        }
        //    }
        //    return ISMSMasApplist;
        //}
        ////==========================================================
        ///// <summary>
        ///// Customer Request for Approval
        ///// </summary>
        ///// <returns></returns>

        //[HttpGet]
        //public ActionResult CustomerApprovalReq()
        //{
        //    return Json(FetchCMRequest().ToList());
        //}
        //public List<CustomerApproval> FetchCMRequest()
        //{
        //    List<CustomerApproval> cmr = new List<CustomerApproval>();
        //    CustomerMaster cm = new CustomerMaster();
        //    DataSet ds = cm.GetCutomerRequest(Convert.ToInt64(System.Web.HttpContext.Current.Session["UserId"].ToString()), "CMAPPROVAL");
        //    DataTable dt = null;

        //    if (ds.Tables[0] != null)
        //    {
        //        dt = ds.Tables[0];
        //        CustomerApproval cmrs = null;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            string strDisable = string.Empty;
        //            cmrs = new CustomerApproval();
        //            cmrs.GEN_REQUEST_NO = dr["GEN_REQUEST_NO"].ToString();
        //            cmrs.CUSTOMER_CODE = dr["CUSTOMER_CODE"].ToString();
        //            cmrs.CUST_ACC_TYPE = dr["CUST_ACC_TYPE"].ToString();
        //            cmrs.DETAIL_STATUS = dr["DETAIL_STATUS"].ToString();
        //            cmrs.NAME1 = dr["NAME1"].ToString();
        //            cmrs.REQUESTDATE = dr["REQUESTDATE"].ToString();
        //            cmrs.REQUESTERID = dr["ENAME"].ToString();
        //            cmrs.REQUEST_TYPE = dr["REQUEST_TYPE"].ToString();
        //            cmrs.STATUS = dr["STATUS"].ToString();
        //            cmr.Add(cmrs);
        //        }
        //    }
        //    return cmr;
        //}
    }
}

public class IOMApproval
{
    public string TransactionId { get; set; }
    public string EncryptedId { get; set; }
    public string Agreement { get; set; }
    public string Contract { get; set; }
    public string EffectiveDate { get; set; }
    public string ExpiryDate { get; set; }
    public string Vendor { get; set; }
    public string Status { get; set; }
    public string RequestedBy { get; set; }
    public string Designation { get; set; }
    public string URL { get; set; }
}

public class ResigApproval
{
    public string TransactionId { get; set; }
    public string ECode { get; set; }
    public string EName { get; set; }
    public string RelievingDate { get; set; }
    public string Status { get; set; }
    public string URL { get; set; }
    public string AppliedDate { get; set; }
}

//Clearance Approval Process
public class ClearanceApproval
{
    public string TransactionId { get; set; }
    public string ECode { get; set; }
    public string EName { get; set; }
    public string RelievingDate { get; set; }
    public string Status { get; set; }
    public string URL { get; set; }
    //public string AppliedDate { get; set; }
}

public class MRApproval
{
    public string TransactionId { get; set; }
    public string InitatedBy { get; set; }
    public string Position { get; set; }
    public string Status { get; set; }
    public string URL { get; set; }
    public string AppliedDate { get; set; }
}

public class JDApproval
{
    public string TransactionId { get; set; }
    public string InitatedBy { get; set; }
    public string Position { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class VendorBlockApproval
{
    public string TransactionId { get; set; }
    public string EncryptedId { get; set; }
    public string Employee { get; set; }
    public string RequestType { get; set; }
    public string RequestCategory { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class MaterialExtApproval
{
    public string TransactionId { get; set; }
    public string ECode { get; set; }
    public string EName { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class MaterialCrtApproval
{
    public string TransactionId { get; set; }
    public string ECode { get; set; }
    public string EName { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class VendorApproval
{
    public string TransactionId { get; set; }
    public string EncryptedId { get; set; }
    public string Employee { get; set; }
    public string RequestType { get; set; }
    public string AccountGroup { get; set; }
    public string Vendor { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class LeaveApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string LeaveDays { get; set; }
    public string LeaveType { get; set; }
    public string AppliedDate { get; set; }
}
public class TransferApproval
{
    public string Process { get; set; }
    public string Count { get; set; }
    public string Status { get; set; }
    public string Urlpath { get; set; }

}

public class LookseeApproval
{
    public string Process { get; set; }
    public string Associate { get; set; }
    public string Fromdate { get; set; }
    public string Todate { get; set; }
    public string Status { get; set; }
    public string Urlpath { get; set; }

}
public class LeaveApprovalbulk
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string APPCode { get; set; }
    public string APPname { get; set; }

}
public class LTAApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string AppliedYear { get; set; }
    public string AppliedDate { get; set; }

}

public class UIMApproval
{
    public string TransactionId { get; set; }
    public string AuthTransactionId { get; set; }
    public string RequestedBy { get; set; }
    public string RequestedFor { get; set; }
    public string Service { get; set; }
    public string ServiceModule { get; set; }
    public string AppliedDate { get; set; }
}

public class PTCApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string Date { get; set; }
    public string Shift { get; set; }
    public string PunchIn { get; set; }
    public string PunchOut { get; set; }
    public string AppliedDate { get; set; }
}

public class VCApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string Quantity { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class TaxiApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class OSApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string OverstayHours { get; set; }
    public string OverstayDate { get; set; }
    public string CurrentShift { get; set; }
    public string NewShift { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }

    //Overstay pre.Visability start
    public string currOTHrs { get; set; }
    public string firstOTHrs { get; set; }
    public string secondOTHrs { get; set; }
    public string thirdOTHrs { get; set; }
    //Overstay pre.Visability end
}

public class SCApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string CurrentShift { get; set; }
    public string NewShift { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class UTApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string Catalog { get; set; }
    public string Classification { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class PGPApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string ODType { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class OODApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class HWRApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }

}

public class SRTApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string RegistrationFor { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class TourReqApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TourPeriod { get; set; }
    public string Days { get; set; }
    public string AppliedDate { get; set; }
}

public class TourBillApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TourPeriod { get; set; }
    public string AdvanceIssued { get; set; }
    public string ClaimAmt { get; set; }
    public string AppliedDate { get; set; }
}

public class TrngReschApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string Training { get; set; }
    public string TrainingPeriod { get; set; }
    public string RescheduleNo { get; set; }
    public string AppliedDate { get; set; }
    public string BatchId { get; set; }
    public string AppAuth { get; set; }
}

public class TrngEvalApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string Training { get; set; }
    public string TrainingPeriod { get; set; }
    public string TrainingTime { get; set; }
    public string Venue { get; set; }
}

public class StationaryApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string AppliedDate { get; set; }
}

public class QMSDocApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string ReqType { get; set; }
    public string DocNo { get; set; }
    public string DocTitle { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class EmpConfApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string JoiningDate { get; set; }
    public string ConfirmationDate { get; set; }
}

public class ITAssetApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string RequestType { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class ITAssetTrfApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class ITConsumableApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class ITBreakageApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}
public class AnnouncementApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string Subject { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class AssetDisposalApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string RequestType { get; set; }
    public string AssetCount { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class WFHRquestapproval
{
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TransactionId { get; set; }
    public string REQUESTTYPE { get; set; }
    public string APPLYFOR { get; set; }
    public string DURATION { get; set; }
    public string STARTDATE { get; set; }
    public string ENDDATE { get; set; }
    public string STARTTIME { get; set; }
    public string ENDTIME { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
    public string IsEdit { get; set; }
    public string IsCancel { get; set; }
}

public class GuestMealApproval
{
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TransactionId { get; set; }
    public string GuestCount { get; set; }
    public string TotalAmt { get; set; }
    public string MealType { get; set; }
    public string Meal { get; set; }
    public string BookingDate { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
    public string IsEdit { get; set; }
    public string IsCancel { get; set; }
}


public class PORquestapproval
{
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TransactionId { get; set; }
    public string APPTransactionId { get; set; }
    public string PONo { get; set; }
    public string Reqname { get; set; }
    public string VendorCode { get; set; }
    public string VendorName { get; set; }
    public string Status { get; set; }
    public string ProcessStatus { get; set; }
    public string AppliedDate { get; set; }
    public string IsEdit { get; set; }
    public string IsCancel { get; set; }
    public string HighUrgency { get; set; }
}
public class PRRquestapproval
{
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TransactionId { get; set; }
    public string APPTransactionId { get; set; }
    public string PRNo { get; set; }
    public string Reqname { get; set; }
    public string PRDATE { get; set; }
    public string PRAMT { get; set; }
    public string Status { get; set; }
    public string ProcessStatus { get; set; }
    public string AppliedDate { get; set; }
    public string IsEdit { get; set; }
    public string IsCancel { get; set; }
}
public class DGIT_IOMRquestapproval
{
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TransactionId { get; set; }
    public string APPTransactionId { get; set; }
    public string IOM_DESC { get; set; }
    public string Reqname { get; set; }
    public string Status { get; set; }
    public string ProcessStatus { get; set; }
    public string AppliedDate { get; set; }
    public string IsEdit { get; set; }
    public string IsCancel { get; set; }
    public string IsHighlighted { get; set; }
    public string CATDESC { get; set; }
}
public class VRApproval
{
    public string TransactionId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string AppointmentDate { get; set; }
    public string AppointmentTime { get; set; }
    public string Status { get; set; }
    public string AppliedDate { get; set; }
}

public class ACRRquestapproval
{
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TransactionId { get; set; }
    public string APPTransactionId { get; set; }
    public string ACRNo { get; set; }
    public string Reqname { get; set; }
    public string ACRDATE { get; set; }
    public string ACRAMT { get; set; }
    public string Supplier_Name { get; set; }
    public string Supplier_Code { get; set; }
    public string PONO { get; set; }

    public string Status { get; set; }
    public string ProcessStatus { get; set; }
    public string AppliedDate { get; set; }
    public string IsEdit { get; set; }
    public string IsCancel { get; set; }
}
public class SESRquestapproval
{
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TransactionId { get; set; }
    public string APPTransactionId { get; set; }
    public string SESNo { get; set; }
    public string Reqname { get; set; }
    public string INVDATE { get; set; }
    public string INVAMT { get; set; }
    public string Supplier_Name { get; set; }
    public string Supplier_Code { get; set; }
    public string PONO { get; set; }

    public string Status { get; set; }
    public string ProcessStatus { get; set; }
    public string AppliedDate { get; set; }
    public string IsEdit { get; set; }
    public string IsCancel { get; set; }
    public string SYSITEIDNAME { get; set; }

}

public class CommunicationRquestApproval
{
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TransactionId { get; set; }
    public string APPTransactionId { get; set; }
    public string Reqname { get; set; }
    public string Subject { get; set; }
    public string Category { get; set; }
    public string RequestType { get; set; }
    public string Status { get; set; }
    public string ProcessStatus { get; set; }
    public string AppliedDate { get; set; }
    public string IsEdit { get; set; }
    public string IsCancel { get; set; }
}

//Creative Master change start
public class CreativeMasterApproval
{
    public string MAPPINGATTACHMENTID { get; set; }
    public string ATTACHMENTID { get; set; }
    public string INITIATEDBY { get; set; }
    public string INITIATEDDATE { get; set; }
    public string SUBJECT { get; set; }
    public string STATUS { get; set; }
}
//Creative Master change end

//Calendar Master Change start
public class CalenderMasterApproval
{
    public string CALENDERMAPPINGID { get; set; }
    public string SRNO { get; set; }
    public string FINANCIALYEAR { get; set; }
    public string CALENDER { get; set; }
    public string LOCATION { get; set; }
    public string INITIATEDBY { get; set; }
    public string STATUS { get; set; }
}
//Calendar Master Change start

//ISMS Master start
public class ISMSMasterApproval
{

    public string ISMSMAPPINGID { get; set; }
    public string SRNO { get; set; }
    public string ISMS_UPLOAD { get; set; }
    public string INITIATEDBY { get; set; }
    public string STATUS { get; set; }
}
//ISMS Master end

//Customer Approval Request 
public class CustomerApproval
{
    public string GEN_REQUEST_NO { get; set; }
    public string REQUEST_TYPE { get; set; }
    public string REQUESTERID { get; set; }
    public string REQUESTDATE { get; set; }
    public string CUSTOMER_CODE { get; set; }
    public string CUST_ACC_TYPE { get; set; }
    public string NAME1 { get; set; }
    public string STATUS { get; set; }
    public string DETAIL_STATUS { get; set; }

}
public class ACRRquest
{
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public string TransactionId { get; set; }
    public string APPTransactionId { get; set; }
    public string ACRNo { get; set; }
    public string Reqname { get; set; }
    public string ACRDATE { get; set; }
    public string ACRAMT { get; set; }
    public string Supplier_Name { get; set; }
    public string Supplier_Code { get; set; }
    public string PONO { get; set; }

    public string Status { get; set; }
    public string ProcessStatus { get; set; }
    public string AppliedDate { get; set; }
    public string IsEdit { get; set; }
    public string IsCancel { get; set; }
    public string ACRattachment { get; set; }
}



