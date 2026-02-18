using AspNetCoreGeneratedDocument;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Admin.Services;
using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.Finance;
using ePortal.WebUI.Filters;
using ePortal.WebUI.Helpers;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Org.BouncyCastle.Tls;
using System.Collections;
using System.Data;
using System.Reflection.Emit;
using System.Web.Helpers;
using System.Web.WebPages;




namespace ePortal.WebUI.Controllers
{
    [SessionTimeout]
    [CSPFilter]
    public class FinanceController : Controller
    {

        private readonly ITaxDeclaration objtax;
        private readonly IPMS _objIPMS;
        private readonly ICommonFunctions _CommonFunctions;
        private readonly ISessionService _sessionService;
        private readonly Employee_Details LoginEmpDetails;
        private readonly IEportalESS objess;
        private readonly ISearchEmp _objSearchEmp;
        public FinanceController(ITaxDeclaration TaxDeclaration, ICommonFunctions CommonFunctions, ISessionService sessionService, IPMS objIPMS, IEportalESS _objess, ISearchEmp objSearchEmp) //IPRService Added by Aumento
        {
            objtax = TaxDeclaration;
            _objIPMS = objIPMS;
            _CommonFunctions = CommonFunctions;
            _sessionService = sessionService;
            LoginEmpDetails = sessionService.Get<Employee_Details>("Employee");
            objess = _objess;
            _objSearchEmp = objSearchEmp;


        }
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            FinanceViewModel data = new FinanceViewModel();
            DataTable dt = objtax.GetPreviousYearDtl(_sessionService.Get<string>("userID"));
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.FinancePrvYearDetail = JsonConvert.DeserializeObject<List<FinancePreviousYearDetail>>(json);


            }

            dt = objtax.GetCurrKIStatus(_sessionService.Get<string>("userID"));

            if (dt.Rows.Count > 0)
            {



                data.PERIOD = dt.Rows[0]["PERIOD"].ToString();
                if (dt.Rows[0]["PROJ_STATUS"].ToString() == "0")
                {
                    data.CurrProjImg = "~/images/system-users.png";
                    data.CurrProjLink = "EmpTax_InvDec?KIID=" + dt.Rows[0]["SYKIID"].ToString() + "&Type=1";
                }
                if (dt.Rows[0]["PROJ_STATUS"].ToString() == "1")
                {

                    data.CurrProjImg = "~/images/applications.png";
                    data.CurrProjLink = "EmpTax_InvDec?KIID=" + dt.Rows[0]["SYKIID"].ToString() + "&Type=1";
                }
                if (dt.Rows[0]["PROJ_STATUS"].ToString() == "2")
                {

                    data.CurrProjImg = "~/images/checkmark-korganizer.png";
                    if (dt.Rows[0]["TXDCLTYPE"].ToString() == "0")
                    {
                        data.CurrProjLink = "ViewProjEmpTax_InvDec?KIID=" + dt.Rows[0]["SYKIID"].ToString() + "";
                    }
                }
                if (dt.Rows[0]["PROJ_STATUS"].ToString() == "3")
                {
                    data.CurrProjImg = "~/images/notapplicable20.png";
                    data.CurrProjLink = "#";
                }

                if (dt.Rows[0]["ACT_STATUS"].ToString() == "0")
                {
                    data.CurrAccImg = "~/images/system-users.png";
                    data.CurrAccLink = "EmpTax_InvDec?KIID=" + dt.Rows[0]["SYKIID"].ToString() + "&Type=2";
                }
                if (dt.Rows[0]["ACT_STATUS"].ToString() == "1")
                {
                    data.CurrAccImg = "~/images/applications.png";
                    data.CurrAccLink = "EmpTax_InvDec?KIID=" + dt.Rows[0]["SYKIID"].ToString() + "&Type=2";
                }
                if (dt.Rows[0]["ACT_STATUS"].ToString() == "2")
                {
                    data.CurrAccImg = "~/images/checkmark-korganizer.png";
                    if (dt.Rows[0]["TXDCLTYPE"].ToString() == "0")
                    {
                        data.CurrAccLink = "ViewEmpTax_InvDec?KIID=" + dt.Rows[0]["SYKIID"].ToString() + "";

                    }
                }
                if (dt.Rows[0]["ACT_STATUS"].ToString() == "3")
                {
                    data.CurrAccImg = "~/images/notapplicable20.png";
                    data.CurrAccLink = "#";
                }
                data.TAXREGIM = dt.Rows[0]["TAXREGIME"].ToString();

            }
            return View("Dashboard", data);
        }


        [HttpGet]
        public async Task<IActionResult> ViewEmpTax_InvDec(string KIID)
        {

            EMPTAX_INVDEC data = new EMPTAX_INVDEC();
            data.KIIID = Convert.ToInt32(double.Parse(KIID)).ToString();
            DataTable dt = objtax.GetEmpRent_Invdetail_N(_sessionService.Get<string>("userID"), data.KIIID);
            List<Rent_Detail> _lstobjrent = new List<Rent_Detail>();
            if (dt.Rows.Count > 0)
            {
                int AnnualRent = 0;
                data.MOBILENO = dt.Rows[0]["TMOBILE"].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    DateTime fdate = DateTime.ParseExact(dt.Rows[i]["RENT_FROM"].ToString(), "d-MMM-yyyy", null);
                    DateTime tdate = DateTime.ParseExact(dt.Rows[i]["RENT_TO"].ToString(), "d-MMM-yyyy", null);

                    int months =
                            ((tdate.Year - fdate.Year) * 12)
                            + (tdate.Month - fdate.Month)
                            + 1;
                    int monthlyperiodammount = months * int.Parse(dt.Rows[i]["RENT_AMOUNT"].ToString());
                    AnnualRent = AnnualRent + monthlyperiodammount;
                }
                data.AnnualRent = AnnualRent.ToString();


            }
            else
            {
                // txtresaddress = ds.Tables[0].Rows[0]["TADDRESS1"].ToString() + " " + ds.Tables[0].Rows[0]["TADDRESS2"].ToString() + " " + ds.Tables[0].Rows[0]["TCITY"].ToString() + " " + ds.Tables[0].Rows[0]["TSTATE"].ToString();
                data.MOBILENO = LoginEmpDetails.MobileNo;

            }


            data.KII = _objIPMS.GetKiList().Select("SYKIID=" + data.KIIID).CopyToDataTable().AsSelectList("SYKIID", "KICODE", false);

            data.PANNUMBER = LoginEmpDetails.PancardNo;

            var Fy = FillKI(KIID, new EmpTaxInvDecReportModel());
            
            data.FINANCIALYEAR = Fy.ltlfinyear;
            dt = objtax.GetEmp584_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "584");
            DataView dv = dt.DefaultView;
            dv.RowFilter = "SAPSUBTYPE=1";
            if (dv.ToTable().Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dv.ToTable());
                data.Taxinv584 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv584)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);


                }
            }

            dv = dt.DefaultView;
            dv.RowFilter = "SAPSUBTYPE=2";
            if (dv.ToTable().Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dv.ToTable());
                data.Taxinv584Sub2 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv584Sub2)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);


                }
                var sum = data.Taxinv584Sub2.Where(x => !string.IsNullOrEmpty(x.PROJ_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.PROJ_AMOUNT)));
                data.total584sub2 = sum.ToString();


            }

            dt = objtax.GetEmpTax_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "585");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv585 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv585)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);


                }
                var sum = data.Taxinv585.Where(x => !string.IsNullOrEmpty(x.PROJ_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.PROJ_AMOUNT)));
                data.total585_proj = sum.ToString();
                sum = data.Taxinv585.Where(x => !string.IsNullOrEmpty(x.ACT_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.ACT_AMOUNT)));
                data.total585_act = sum.ToString();
            }

            dt = objtax.GetEmpTax_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "586");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv586 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv586)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);
                }
                var sum = data.Taxinv586.Where(x => !string.IsNullOrEmpty(x.PROJ_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.PROJ_AMOUNT)));
                data.total586_proj = sum.ToString();
                sum = data.Taxinv586.Where(x => !string.IsNullOrEmpty(x.ACT_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.ACT_AMOUNT)));
                data.total586_act = sum.ToString();
            }

            dt = objtax.GetEmp584_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "580");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv580 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv580)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);
                }
            }

            dt = objtax.GetEmp584_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "582");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv582 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv582)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);
                }
            }

            //txtprojamttotal_OnTextChanged(sender, e);
            //txtactualamttotal_OnTextChanged(sender, e);
            //txtprojamt_OnTextChanged(sender, e);
            //txt80Dprojamt_OnTextChanged(sender, e);

            return View("ViewEmpTax_InvDec", data);
        }

        [HttpGet]
        public async Task<IActionResult> ViewProjEmpTax_InvDec(string KIID)
        {

            EMPTAX_INVDEC data = new EMPTAX_INVDEC();
            data.KIIID = Convert.ToInt32(double.Parse(KIID)).ToString();
            DataTable dt = objtax.GetEmpRent_Invdetail_N(_sessionService.Get<string>("userID"), data.KIIID);
            List<Rent_Detail> _lstobjrent = new List<Rent_Detail>();
            if (dt.Rows.Count > 0)
            {
                int AnnualRent = 0;
                data.MOBILENO = dt.Rows[0]["TMOBILE"].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    DateTime fdate = DateTime.ParseExact(dt.Rows[i]["RENT_FROM"].ToString(), "d-MMM-yyyy", null);
                    DateTime tdate = DateTime.ParseExact(dt.Rows[i]["RENT_TO"].ToString(), "d-MMM-yyyy", null);

                    int months =
                            ((tdate.Year - fdate.Year) * 12)
                            + (tdate.Month - fdate.Month)
                            + 1;
                    int monthlyperiodammount = months * int.Parse(dt.Rows[i]["RENT_AMOUNT"].ToString());
                    AnnualRent = AnnualRent + monthlyperiodammount;
                }
                data.AnnualRent = AnnualRent.ToString();


            }
            else
            {
                // txtresaddress = ds.Tables[0].Rows[0]["TADDRESS1"].ToString() + " " + ds.Tables[0].Rows[0]["TADDRESS2"].ToString() + " " + ds.Tables[0].Rows[0]["TCITY"].ToString() + " " + ds.Tables[0].Rows[0]["TSTATE"].ToString();
                data.MOBILENO = LoginEmpDetails.MobileNo;

            }


            data.KII = _objIPMS.GetKiList().Select("SYKIID=" + data.KIIID).CopyToDataTable().AsSelectList("SYKIID", "KICODE", false);

            data.PANNUMBER = LoginEmpDetails.PancardNo;
            var Fy = FillKI(KIID, new EmpTaxInvDecReportModel());

            data.FINANCIALYEAR = Fy.ltlfinyear;

            dt = objtax.GetEmp584_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "584");
            DataView dv = dt.DefaultView;
            dv.RowFilter = "SAPSUBTYPE=1";
            if (dv.ToTable().Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dv.ToTable());
                data.Taxinv584 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv584)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);


                }
            }

            dv = dt.DefaultView;
            dv.RowFilter = "SAPSUBTYPE=2";
            if (dv.ToTable().Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dv.ToTable());
                data.Taxinv584Sub2 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv584Sub2)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);


                }
                var sum = data.Taxinv584Sub2.Where(x => !string.IsNullOrEmpty(x.PROJ_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.PROJ_AMOUNT)));
                data.total584sub2 = sum.ToString();


            }

            dt = objtax.GetEmpTax_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "585");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv585 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv585)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);


                }
                var sum = data.Taxinv585.Where(x => !string.IsNullOrEmpty(x.PROJ_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.PROJ_AMOUNT)));
                data.total585_proj = sum.ToString();
                sum = data.Taxinv585.Where(x => !string.IsNullOrEmpty(x.ACT_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.ACT_AMOUNT)));
                data.total585_act = sum.ToString();
            }

            dt = objtax.GetEmpTax_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "586");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv586 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv586)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);
                }
                var sum = data.Taxinv586.Where(x => !string.IsNullOrEmpty(x.PROJ_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.PROJ_AMOUNT)));
                data.total586_proj = sum.ToString();
                sum = data.Taxinv586.Where(x => !string.IsNullOrEmpty(x.ACT_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.ACT_AMOUNT)));
                data.total586_act = sum.ToString();
            }



            return View("ViewProjEmpTax_InvDec", data);
        }
        [HttpGet]
        public async Task<IActionResult> EmpTax_InvDec(string KIID, int Type)
        {

            EMPTAX_INVDEC data = new EMPTAX_INVDEC();
            data.KIIID = Convert.ToInt32(double.Parse(KIID)).ToString();
            data.PANNUMBER = LoginEmpDetails.PancardNo;
            if (data.PANNUMBER == "")
            {
                //txtpancard.ReadOnly = true;
                data.Status = "Kindly contact to Kawalpreet Singh San - Finance Deptt. (Extn. No. - 2833)";
                //tr_ErrorRow.Visible = true;
            }

            data.ProjStatus = Type.ToString();


            DataTable dt = objtax.GetEmpUserPeriodDetail(_sessionService.Get<string>("userID"), "1", KIID);
            if (dt.Rows.Count > 0)
            {
                data.FromDate = dt.Rows[0]["START_DATE"].ToString();
                data.ToDate = dt.Rows[0]["END_DATE"].ToString();
                data.ISEDITABLE = dt.Rows[0]["ISEDITABLE"].ToString();
                data.ISEDITABLEACTUAL = dt.Rows[0]["ISEDITABLE_ACTUAL"].ToString();
                data.LTACOUNT = dt.Rows[0]["LTACOUNT"].ToString();
                if (dt.Rows[0]["TXDCLTYPE"].ToString() == "0")
                {
                    data.TaxType = "old";

                }

                if (dt.Rows[0]["TXDCLTYPE"].ToString() == "1")
                {
                    data.TaxType = "new";

                }

                if (string.IsNullOrEmpty(dt.Rows[0]["TXDCLTYPE"].ToString()))
                {
                    data.TaxType = "-";

                }
            }
            else
            {
                data.FromDate = "";
                data.ToDate = "";
                data.ISEDITABLE = "0";
                data.ISEDITABLEACTUAL = "0";

            }
            if (data.FromDate == "" || data.ToDate == "")
            {
                data.Status = "Your time period is not set to fill Projected Investment Declaration.Please contact to Finance Department.";
                data.Taxinv584 = new List<TaxInvoiceDetails>();

            }
            DateTime strStartdate = DateTime.ParseExact(data.FromDate, "dd-MMM-yyyy", null);
            DateTime strEnddate = DateTime.ParseExact(data.ToDate, "dd-MMM-yyyy", null);
            DateTime strtodaydate = DateTime.Today;
            var Fy = FillKI(KIID, new EmpTaxInvDecReportModel());
            Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
            if (Convert.ToDateTime(emp.DOJ) <= Convert.ToDateTime("01-APR-" + Fy.ltlfinyear.Substring(0, 4)))
            {
                data.B12Enabled = false;

                data.B12Msg = "Applicable only if date of joining is greater than 01-APR-" + Fy.ltlfinyear.Substring(0, 4) + ".";
            }
            data.FINANCIALYEAR = Fy.ltlfinyear;
            if (data.ISEDITABLEACTUAL == "0" || !(strtodaydate.Date >= strStartdate.Date && strtodaydate.Date <= strEnddate.Date))
            {
                data.Status = "You have already submit your actual investment detail.";
            }
            if (!(strtodaydate.Date >= strStartdate.Date && strtodaydate.Date <= strEnddate.Date))
            {
                data.Status = "Time period to fill Projected/Actual Investment declaration has expired. Please Contact to Finance department.";

            }
            if (data.ISEDITABLE.Trim() == "0" && data.ISEDITABLEACTUAL.Trim() == "0")
            {

                data.Status = "Your time period has expire to fill Projected/Investment declaration.Please Contact to Finance department.";
            }
            else
            {
                data.print = false;
            }

            #region "Getting rent detail data"
            // ADDED BY N.A
            dt = objtax.GetEmpRent_Invdetail_N(_sessionService.Get<string>("userID"), KIID);
            List<HRADetails> _lstobjrent = new List<HRADetails>();
            if (dt.Rows.Count > 0)
            {
                int AnnualRent = 0;
                data.MOBILENO = dt.Rows[0]["TMOBILE"].ToString();
                //ddlhraclaim.SelectedValue = dt.Rows[0]["HRACLAIM"].ToString();


                data.HRClaim = dt.Rows[0]["HRACLAIM"].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HRADetails objrent = new HRADetails();
                    objrent._strFINEMPRENTDTLID = Convert.ToInt32(dt.Rows[i]["FINEMPRENTDTLID"].ToString());
                    objrent.strgridindex = i;
                    objrent.SLNO = (i + 1);
                    objrent.FinPeriodStartDate = dt.Rows[i]["RENT_FROM"].ToString();
                    objrent.FinPeriodEndDate = dt.Rows[i]["RENT_TO"].ToString();

                    //ddlacctype.SelectedValue = dt.Rows[0]["ACC_TYPE"].ToString();
                    objrent.CityCategory = dt.Rows[i]["CITY_CAT"].ToString();
                    objrent.MonthlyRentAmount = dt.Rows[i]["RENT_AMOUNT"].ToString();
                    objrent.PresentResidentialAddress = dt.Rows[i]["TADDRESS"].ToString();
                    objrent.ISLANPAN = dt.Rows[i]["ISLANPAN"].ToString();
                    objrent.EmployeeSO = dt.Rows[i]["EMPSO"].ToString();
                    objrent.TotalRentAmount = MonthAmountcalculate(objrent.MonthlyRentAmount, DateTime.Parse(objrent.FinPeriodStartDate), DateTime.Parse(objrent.FinPeriodEndDate)).ToString();
                    objrent.MultipleLandlord = false;
                    if (dt.Rows[i]["LANDLORDNAME2"].ToString() != "")
                    {
                        objrent.MultipleLandlord = true;
                    }

                    objrent.LandLord1 = new LandlordDeclarationViewModel
                    {
                        LandlordName = dt.Rows[i]["LANDLORDNAME"].ToString(),
                        LandlordPAN = dt.Rows[i]["LANDLORDPAN"].ToString(),
                        LandlordResHNo = dt.Rows[i]["LLHNO"].ToString(),
                        LandlordResGNo = dt.Rows[i]["LLGNO"].ToString(),
                        LandlordResVillage = dt.Rows[i]["LLVILLAGE"].ToString(),
                        LandlordResCity = dt.Rows[i]["LLCITY"].ToString(),
                        LandlordResPincode = dt.Rows[i]["LLPINECODE"].ToString(),
                        LandlordSO = dt.Rows[i]["LANDLORDSO"].ToString(),
                        LandlordMobileNo = dt.Rows[i]["LANLORDMNO"].ToString(),

                    };
                    objrent.LandLord2 = new LandlordDeclarationViewModel
                    {
                        LandlordName = dt.Rows[i]["LANDLORDNAME2"].ToString(),
                        LandlordPAN = dt.Rows[i]["LANDLORDPAN2"].ToString(),
                        LandlordResHNo = dt.Rows[i]["LLHNO2"].ToString(),
                        LandlordResGNo = dt.Rows[i]["LLGNO2"].ToString(),
                        LandlordResVillage = dt.Rows[i]["LLVILLAGE2"].ToString(),
                        LandlordResCity = dt.Rows[i]["LLCITY2"].ToString(),
                        LandlordResPincode = dt.Rows[i]["LLPINECODE2"].ToString(),
                        LandlordSO = dt.Rows[i]["LANDLORDSO2"].ToString(),
                        LandlordMobileNo = dt.Rows[i]["LANLORDMNO2"].ToString()
                    };
                    objrent.LandLord3 = new LandlordDeclarationViewModel
                    {
                        LandlordName = dt.Rows[i]["LANDLORDNAME3"].ToString(),
                        LandlordPAN = dt.Rows[i]["LANDLORDPAN3"].ToString(),
                        LandlordResHNo = dt.Rows[i]["LLHNO3"].ToString(),
                        LandlordResGNo = dt.Rows[i]["LLGNO3"].ToString(),
                        LandlordResVillage = dt.Rows[i]["LLVILLAGE3"].ToString(),
                        LandlordResCity = dt.Rows[i]["LLCITY3"].ToString(),
                        LandlordResPincode = dt.Rows[i]["LLPINECODE3"].ToString(),
                        LandlordSO = dt.Rows[i]["LANDLORDSO3"].ToString(),
                        LandlordMobileNo = dt.Rows[i]["LANLORDMNO3"].ToString()
                    };
                    objrent.LandLord4 = new LandlordDeclarationViewModel
                    {
                        LandlordName = dt.Rows[i]["LANDLORDNAME4"].ToString(),
                        LandlordPAN = dt.Rows[i]["LANDLORDPAN4"].ToString(),
                        LandlordResHNo = dt.Rows[i]["LANDLORDPAN4"].ToString(),
                        LandlordResGNo = dt.Rows[i]["LLGNO4"].ToString(),
                        LandlordResVillage = dt.Rows[i]["LLVILLAGE4"].ToString(),
                        LandlordResCity = dt.Rows[i]["LLVILLAGE4"].ToString(),
                        LandlordResPincode = dt.Rows[i]["LLPINECODE4"].ToString(),
                        LandlordSO = dt.Rows[i]["LANDLORDSO4"].ToString(),
                        LandlordMobileNo = dt.Rows[i]["LANLORDMNO4"].ToString()
                    };

                    _lstobjrent.Add(objrent);

                }

                data.AnnualRent = _lstobjrent.Count() > 0 ? _lstobjrent.Sum(x => Int64.Parse(x.TotalRentAmount)).ToString() : "0";
                data.txtrentamount1 = data.AnnualRent;


            }
            else
            {
                data.MOBILENO = LoginEmpDetails.MobileNo;
            }
            //HttpContext.Session.SetObject<List<HRADetails>>("HRAList", new List<HRADetails>());
            //HttpContext.Session.SetObject<List<HRADetails>>("HRAList", _lstobjrent);
            data.HRADetaillist = new HRADetailList();
            data.HRADetaillist.hRADetails = new HRADetails();
            data.HRADetaillist.hRAList = _lstobjrent;

            #endregion



            data.KII = _objIPMS.GetKiList().Select("SYKIID=" + data.KIIID).CopyToDataTable().AsSelectList("SYKIID", "KICODE", false);




            dt = objtax.GetEmp584_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "584");
            DataView dv = dt.DefaultView;
            dv.RowFilter = "SAPSUBTYPE=1";
            if (dv.ToTable().Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dv.ToTable());
                data.Taxinv584 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv584)
                {
                    itm.ACT_AMOUNT = itm.PROJ_AMOUNT;
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);


                }
            }

            dv = dt.DefaultView;
            dv.RowFilter = "SAPSUBTYPE=2";
            if (dv.ToTable().Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dv.ToTable());
                data.Taxinv584Sub2 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv584Sub2)
                {
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);
                    itm.ACT_AMOUNT = itm.PROJ_AMOUNT;

                }
                var sum = data.Taxinv584Sub2.Where(x => !string.IsNullOrEmpty(x.PROJ_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.PROJ_AMOUNT)));
                data.total584sub2 = sum.ToString();


            }

            dt = objtax.GetEmpTax_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "585");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv585 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv585)
                {

                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);


                }
                var sum = data.Taxinv585.Where(x => !string.IsNullOrEmpty(x.PROJ_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.PROJ_AMOUNT)));
                data.total585_proj = sum.ToString();
                sum = data.Taxinv585.Where(x => !string.IsNullOrEmpty(x.ACT_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.ACT_AMOUNT)));
                data.total585_act = sum.ToString();
            }

            dt = objtax.GetEmpTax_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "586");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv586 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv586)
                {
                    //itm.ACT_AMOUNT = itm.PROJ_AMOUNT;
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);
                }
                var sum = data.Taxinv586.Where(x => !string.IsNullOrEmpty(x.PROJ_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.PROJ_AMOUNT)));
                data.total586_proj = sum.ToString();
                sum = data.Taxinv586.Where(x => !string.IsNullOrEmpty(x.ACT_AMOUNT)).Sum(x => Convert.ToInt32(double.Parse(x.ACT_AMOUNT)));
                data.total586_act = sum.ToString();
            }
            _sessionService.Set("KIID", data.KIIID);
            dt = objtax.GetEmp584_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "580");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv580 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv580)
                {
                    itm.ACT_AMOUNT = itm.PROJ_AMOUNT;
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);
                }
            }

            dt = objtax.GetEmp584_Invdetail("", _sessionService.Get<string>("userID"), "1", data.KIIID, "582");
            if (dt.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt);
                data.Taxinv582 = JsonConvert.DeserializeObject<List<TaxInvoiceDetails>>(json);
                foreach (var itm in data.Taxinv582)
                {
                    itm.ACT_AMOUNT = itm.PROJ_AMOUNT;
                    itm.OptionId = Convert.ToInt32(double.Parse(itm.TAXHEADID)).ToString();
                    itm.OptionList = objtax.GetTaxhead_Optdetail("", itm.OptionId, "1").AsSelectList("TAXHEADOPTID", "OPTDESC", false);
                }
            }

            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(data));
            return View("EmpTax_InvDec", data);
        }

        [HttpPost]
        public async Task<IActionResult> GetHRADetails(string KIID, string AnnualAmount)
        {
            var Json = _sessionService.Get<string>("EMPTAX_INVDEC");
            var HRASession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(Json);
            HRADetailList data = new HRADetailList();
            if (HRASession.HRADetaillist != null)
            {
                data = HRASession.HRADetaillist;
            }
            else
            {
                data.hRADetails = new HRADetails();

                data.hRADetails.TotalRentAmount = AnnualAmount;
            }
            BindDropDown(data.hRADetails);
            return PartialView("_HRADetails", data);
        }

        [HttpPost]
        public async Task<IActionResult> AddHRADetails([FromBody] HRADetails obj)
        {
            var HraVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var HRASession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(HraVal);
            HRADetailList data = new HRADetailList();
            if (HRASession.HRADetaillist != null)
            {
                data = HRASession.HRADetaillist;
            }
            else
            {
                data.hRADetails = new HRADetails();
            }
            data.hRADetails.TotalRentAmount = obj.TotalRentAmount;

            data.hRADetails.ActionType = obj.ActionType;

            BindDropDown(data.hRADetails);

            int totalrentamount = 0;
            Tuple<string, string> retVal_tuple;
            if (obj.FinPeriodStartDate.Trim() == "" || obj.FinPeriodEndDate.Trim() == "")
            {

                return Json(new Tuple<string, string>("error", "Please enter From Date and To Date."));
            }

            if (obj.MonthlyRentAmount.Trim() == "")
            {

                return Json(new Tuple<string, string>("error", "Please enter monthly rent amount."));



            }
            if (obj.PresentResidentialAddress.Trim() == "")
            {

                return Json(new Tuple<string, string>("error", "Please enter residential address."));


            }
            if (Convert.ToInt32(obj.MonthlyRentAmount) <= 8333 && obj.CheckBox0 == false)
            {

                return Json(new Tuple<string, string>("error", "Terms check box is mandatory field."));


            }
            if (Convert.ToInt32(obj.MonthlyRentAmount) >= 8334 && Convert.ToInt32(obj.MonthlyRentAmount) <= 20000 && obj.CheckBox1 == false)
            {

                return Json(new Tuple<string, string>("error", "Terms check box is mandatory field."));


            }
            if (Convert.ToInt32(obj.MonthlyRentAmount) >= 20001 && Convert.ToInt32(obj.MonthlyRentAmount) <= 50000 && obj.CheckBox2 == false)
            {

                return Json(new Tuple<string, string>("error", "Terms check box is mandatory field.."));


            }
            if (Convert.ToInt32(obj.MonthlyRentAmount) > 50000 && obj.CheckBox3 == false)
            {

                return Json(new Tuple<string, string>("error", "Terms check box is mandatory field."));


            }

            DateTime fromdate = DateTime.ParseExact(obj.FinPeriodStartDate.Trim(), "d-MMM-yyyy", null);
            DateTime todate = DateTime.ParseExact(obj.FinPeriodEndDate.Trim(), "d-MMM-yyyy", null);
            todate = todate.AddMonths(1).AddDays(-1);
            if (fromdate >= todate)
            {

                return Json(new Tuple<string, string>("error", "From Date must be greater than To Date"));


            }
            else
            {
                // checking finance period
                if (obj.FinPeriodStartDate.Trim() == "" || obj.FinPeriodEndDate.Trim() == "")
                {
                    //FillKI();
                }
                DateTime finPeriodstartDate = DateTime.ParseExact(obj.FinPeriodStartDate.Trim(), "d-MMM-yyyy", null);
                DateTime finPeriodEndDate = DateTime.ParseExact(obj.FinPeriodEndDate.Trim(), "d-MMM-yyyy", null);
                finPeriodEndDate = finPeriodEndDate.AddMonths(1).AddDays(-1);
                obj.FinPeriodEndDate = finPeriodEndDate.ToString("d-MMM-yyyy");
                if (fromdate < finPeriodstartDate || todate > finPeriodEndDate)
                {

                    return Json(new Tuple<string, string>("error", "Please check financial year period"));


                }
                totalrentamount = MonthAmountcalculate(obj.MonthlyRentAmount, fromdate, todate);
            }

            if (Convert.ToInt32(obj.TotalRentAmount) > 100000)
            {

                if (obj.LandLord1.LandlordName.Trim() == "")
                {

                    return Json(new Tuple<string, string>("error", "Please enter Landlord name in Landlord detail"));


                }
                if (obj.LandLord1.LandlordPAN.Trim() == "" || obj.LandLord1.LandlordPAN.Length != 10)
                {

                    return Json(new Tuple<string, string>("error", "Please enter valid Pancard number of Landlord"));



                }
                if (obj.LandLord1.LandlordName.Trim() != "" & obj.LandLord1.LandlordSO.Trim() == "" || obj.LandLord1.LandlordResHNo.Trim() == "" || obj.LandLord1.LandlordResGNo.Trim() == "" || obj.LandLord1.LandlordResVillage.Trim() == "" || obj.LandLord1.LandlordResCity.Trim() == "" || obj.LandLord1.LandlordResPincode.Trim() == "" || obj.LandLord1.LandlordMobileNo.Trim() == "")
                {

                    return Json(new Tuple<string, string>("error", "All fields of Landlord detail are mandatory"));


                }
                if (obj.LandLord1.LandlordMobileNo.Trim() == "" || obj.LandLord1.LandlordMobileNo.Length != 10)
                {

                    return Json(new Tuple<string, string>("error", "Invalid Mobile no in Landlord detail"));


                }
                if (obj.LandLord1.LandlordResPincode.Trim() == "" || obj.LandLord1.LandlordResPincode.Length != 6)
                {

                    return Json(new Tuple<string, string>("error", "Invalid Pin code in Landlord detail"));


                }
            }


            List<HRADetails> _listRentDetail = new List<HRADetails>();
            if (HRASession.HRADetaillist.hRAList == null)
            {

                HRADetails objRentDetail = new HRADetails();
                //   objRentDetail._strFINEMPRENTDTLID = 1;
                objRentDetail.SLNO = 1;
                objRentDetail.strgridindex = 1;
                objRentDetail.FinPeriodStartDate = obj.FinPeriodStartDate;
                objRentDetail.FinPeriodEndDate = obj.FinPeriodEndDate;
                objRentDetail.TotalRentAmount = obj.TotalRentAmount;
                objRentDetail.MultipleLandlord = obj.MultipleLandlord;
                objRentDetail.CityCategory = obj.CityCategory;
                objRentDetail.EmployeeSO = obj.EmployeeSO;
                objRentDetail.PresentResidentialAddress = obj.PresentResidentialAddress;
                objRentDetail.TotalRentAmount = MonthAmountcalculate(objRentDetail.MonthlyRentAmount, DateTime.Parse(objRentDetail.FinPeriodStartDate), DateTime.Parse(objRentDetail.FinPeriodEndDate)).ToString();

                objRentDetail.LandLord1 = new LandlordDeclarationViewModel
                {
                    LandlordName = obj.LandLord1.LandlordName,
                    LandlordPAN = obj.LandLord1.LandlordPAN,
                    // ResA = txtpraddress,


                    LandlordSO = obj.LandLord1.LandlordSO,
                    LandlordResHNo = obj.LandLord1.LandlordResHNo,

                    LandlordResGNo = obj.LandLord1.LandlordResGNo,

                    LandlordResVillage = obj.LandLord1.LandlordResVillage.Trim(),
                    LandlordResCity = obj.LandLord1.LandlordResCity.Trim(),
                    LandlordResPincode = obj.LandLord1.LandlordResPincode.Trim(),
                    LandlordMobileNo = obj.LandLord1.LandlordMobileNo.Trim()
                };
                objRentDetail.LandLord2 = new LandlordDeclarationViewModel
                {
                    LandlordName = obj.LandLord2.LandlordName,
                    LandlordPAN = obj.LandLord2.LandlordPAN,
                    // ResA = txtpraddress,


                    LandlordSO = obj.LandLord2.LandlordSO,
                    LandlordResHNo = obj.LandLord2.LandlordResHNo,

                    LandlordResGNo = obj.LandLord2.LandlordResGNo,

                    LandlordResVillage = obj.LandLord2.LandlordResVillage.Trim(),
                    LandlordResCity = obj.LandLord2.LandlordResCity.Trim(),
                    LandlordResPincode = obj.LandLord2.LandlordResPincode.Trim(),
                    LandlordMobileNo = obj.LandLord2.LandlordMobileNo.Trim()
                };
                objRentDetail.LandLord3 = new LandlordDeclarationViewModel
                {
                    LandlordName = obj.LandLord3.LandlordName,
                    LandlordPAN = obj.LandLord3.LandlordPAN,
                    // ResA = txtpraddress,


                    LandlordSO = obj.LandLord3.LandlordSO,
                    LandlordResHNo = obj.LandLord3.LandlordResHNo,

                    LandlordResGNo = obj.LandLord3.LandlordResGNo,

                    LandlordResVillage = obj.LandLord3.LandlordResVillage.Trim(),
                    LandlordResCity = obj.LandLord3.LandlordResCity.Trim(),
                    LandlordResPincode = obj.LandLord3.LandlordResPincode.Trim(),
                    LandlordMobileNo = obj.LandLord3.LandlordMobileNo.Trim()
                };
                objRentDetail.LandLord4 = new LandlordDeclarationViewModel
                {
                    LandlordName = obj.LandLord4.LandlordName,
                    LandlordPAN = obj.LandLord4.LandlordPAN,
                    // ResA = txtpraddress,


                    LandlordSO = obj.LandLord4.LandlordSO,
                    LandlordResHNo = obj.LandLord4.LandlordResHNo,

                    LandlordResGNo = obj.LandLord4.LandlordResGNo,

                    LandlordResVillage = obj.LandLord4.LandlordResVillage.Trim(),
                    LandlordResCity = obj.LandLord4.LandlordResCity.Trim(),
                    LandlordResPincode = obj.LandLord4.LandlordResPincode.Trim(),
                    LandlordMobileNo = obj.LandLord4.LandlordMobileNo.Trim()
                };

                if (totalrentamount > Convert.ToInt32(obj.TotalRentAmount))
                {

                    return Json(new Tuple<string, string>("error", "Total Monthly rent amount is greater than Annual rent amount " + obj.TotalRentAmount));


                }
                _listRentDetail.Add(objRentDetail);

                data.hRAList = _listRentDetail;
                HRASession.HRADetaillist = data;
                _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(HRASession));
                return PartialView("_HRADetails", data);

                //return Json(new Tuple<string, string>("success", "House Rent detail\\'s are added successfully"));


            }
            else
            {
                _listRentDetail = HRASession.HRADetaillist.hRAList;
                if (_listRentDetail.Count > 0)
                {
                    if (data.hRADetails.ActionType != "UPDATE")
                    {
                        var existcount = (from a in _listRentDetail
                                          where ((fromdate >= Convert.ToDateTime(a.FinPeriodStartDate) && fromdate <= Convert.ToDateTime(a.FinPeriodEndDate))
                                             || (todate >= Convert.ToDateTime(a.FinPeriodStartDate) && todate <= Convert.ToDateTime(a.FinPeriodEndDate)))
                                             || (fromdate <= Convert.ToDateTime(a.FinPeriodStartDate) && todate >= Convert.ToDateTime(a.FinPeriodEndDate))
                                          select a).Count();

                        if (existcount == 0)
                        {
                            int maxid = 0;
                            var maxindexdata = _listRentDetail.OrderByDescending(x => x.strgridindex).FirstOrDefault();
                            if (maxindexdata != null)
                            {
                                maxid = Convert.ToInt32(maxindexdata.strgridindex);
                            }
                            else
                            {
                                maxid = 0;
                            }
                            int sumofRent = _listRentDetail.Sum(x => int.Parse(x.MonthlyRentAmount));
                            sumofRent = sumofRent + totalrentamount;
                            if (sumofRent > Convert.ToInt32(obj.TotalRentAmount))
                            {

                                return Json(new Tuple<string, string>("error", "Total Monthly rent amount is greater than Annual rent amount " + Convert.ToInt32(obj.TotalRentAmount)));


                            }
                            obj.TotalRentAmount = totalrentamount.ToString();
                            //obj.EmployeeSO = txtempso.Trim();
                            HRADetails objRentDetail = new HRADetails();
                            //             objRentDetail._strFINEMPRENTDTLID = (_listRentDetail.Count) + 1;
                            objRentDetail.strgridindex = maxid + 1;
                            objRentDetail.SLNO = (_listRentDetail.Count) + 1;
                            objRentDetail.FinPeriodStartDate = obj.FinPeriodStartDate;
                            objRentDetail.FinPeriodEndDate = obj.FinPeriodEndDate;
                            objRentDetail.MonthlyRentAmount = obj.MonthlyRentAmount;
                            objRentDetail.CityCategory = obj.CityCategory;
                            objRentDetail.TotalRentAmount = obj.TotalRentAmount;
                            objRentDetail.MultipleLandlord = obj.MultipleLandlord;
                            objRentDetail.EmployeeSO = obj.EmployeeSO;
                            objRentDetail.PresentResidentialAddress = obj.PresentResidentialAddress;
                            objRentDetail.TotalRentAmount = MonthAmountcalculate(objRentDetail.MonthlyRentAmount, DateTime.Parse(objRentDetail.FinPeriodStartDate), DateTime.Parse(objRentDetail.FinPeriodEndDate)).ToString();

                            objRentDetail.LandLord1 = new LandlordDeclarationViewModel
                            {
                                LandlordName = obj.LandLord1.LandlordName,
                                LandlordPAN = obj.LandLord1.LandlordPAN,
                                // ResA = txtpraddress,


                                LandlordSO = obj.LandLord1.LandlordSO,
                                LandlordResHNo = obj.LandLord1.LandlordResHNo,

                                LandlordResGNo = obj.LandLord1.LandlordResGNo,

                                LandlordResVillage = obj.LandLord1.LandlordResVillage.Trim(),
                                LandlordResCity = obj.LandLord1.LandlordResCity.Trim(),
                                LandlordResPincode = obj.LandLord1.LandlordResPincode.Trim(),
                                LandlordMobileNo = obj.LandLord1.LandlordMobileNo.Trim()
                            };
                            objRentDetail.LandLord2 = new LandlordDeclarationViewModel
                            {
                                LandlordName = obj.LandLord2.LandlordName,
                                LandlordPAN = obj.LandLord2.LandlordPAN,
                                // ResA = txtpraddress,


                                LandlordSO = obj.LandLord2.LandlordSO,
                                LandlordResHNo = obj.LandLord2.LandlordResHNo,

                                LandlordResGNo = obj.LandLord2.LandlordResGNo,

                                LandlordResVillage = obj.LandLord2.LandlordResVillage.Trim(),
                                LandlordResCity = obj.LandLord2.LandlordResCity.Trim(),
                                LandlordResPincode = obj.LandLord2.LandlordResPincode.Trim(),
                                LandlordMobileNo = obj.LandLord2.LandlordMobileNo.Trim()
                            };
                            objRentDetail.LandLord3 = new LandlordDeclarationViewModel
                            {
                                LandlordName = obj.LandLord3.LandlordName,
                                LandlordPAN = obj.LandLord3.LandlordPAN,
                                // ResA = txtpraddress,


                                LandlordSO = obj.LandLord3.LandlordSO,
                                LandlordResHNo = obj.LandLord3.LandlordResHNo,

                                LandlordResGNo = obj.LandLord3.LandlordResGNo,

                                LandlordResVillage = obj.LandLord3.LandlordResVillage.Trim(),
                                LandlordResCity = obj.LandLord3.LandlordResCity.Trim(),
                                LandlordResPincode = obj.LandLord3.LandlordResPincode.Trim(),
                                LandlordMobileNo = obj.LandLord3.LandlordMobileNo.Trim()
                            };
                            objRentDetail.LandLord4 = new LandlordDeclarationViewModel
                            {
                                LandlordName = obj.LandLord4.LandlordName,
                                LandlordPAN = obj.LandLord4.LandlordPAN,
                                // ResA = txtpraddress,


                                LandlordSO = obj.LandLord4.LandlordSO,
                                LandlordResHNo = obj.LandLord4.LandlordResHNo,

                                LandlordResGNo = obj.LandLord4.LandlordResGNo,

                                LandlordResVillage = obj.LandLord4.LandlordResVillage.Trim(),
                                LandlordResCity = obj.LandLord4.LandlordResCity.Trim(),
                                LandlordResPincode = obj.LandLord4.LandlordResPincode.Trim(),
                                LandlordMobileNo = obj.LandLord4.LandlordMobileNo.Trim()
                            };
                            _listRentDetail.Add(objRentDetail);

                            data.hRAList = _listRentDetail;
                            data.hRADetails.ActionType = "";
                            HRASession.HRADetaillist = data;
                            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(HRASession));
                            return PartialView("_HRADetails", data);
                        }
                        else
                        {
                            return Json(new Tuple<string, string>("error", "HRA detail\\'s already filled for selected period."));
                        }

                    }
                    else
                    {
                        // for  edit case
                        // index id start 1


                        var existcount = (from a in _listRentDetail
                                          where a.SLNO != obj.SLNO && (((fromdate >= Convert.ToDateTime(a.FinPeriodStartDate) && fromdate <= Convert.ToDateTime(a.FinPeriodEndDate))
                                             || (todate >= Convert.ToDateTime(a.FinPeriodStartDate) && todate <= Convert.ToDateTime(a.FinPeriodEndDate)))
                                             || (fromdate <= Convert.ToDateTime(a.FinPeriodStartDate) && todate >= Convert.ToDateTime(a.FinPeriodEndDate)))
                                          select a).Count();
                        if (existcount == 0)
                        {
                            var _list = _listRentDetail.Single(x => x.SLNO == obj.SLNO);
                            _listRentDetail.Remove(_list);
                            if (_listRentDetail.Count > 0)
                            {
                                int sumofRent = _listRentDetail.Sum(x => int.Parse(x.MonthlyRentAmount));
                                sumofRent = sumofRent + totalrentamount;
                                if (sumofRent > Convert.ToInt32(obj.TotalRentAmount))
                                {

                                    return Json(new Tuple<string, string>("error", "Total Monthly rent amount is greater than Annual rent amount " + Convert.ToInt32(obj.TotalRentAmount)));


                                }
                            }
                            else
                            {
                                if (totalrentamount > Convert.ToInt32(obj.TotalRentAmount))
                                {

                                    return Json(new Tuple<string, string>("error", "Total Monthly rent amount is greater than Annual rent amount " + Convert.ToInt32(obj.TotalRentAmount)));


                                }
                            }
                            HRADetails objRentDetail = new HRADetails();
                            objRentDetail._strFINEMPRENTDTLID = obj._strFINEMPRENTDTLID;
                            objRentDetail.SLNO = obj.SLNO;
                            objRentDetail.strgridindex = obj.strgridindex;
                            objRentDetail.FinPeriodStartDate = obj.FinPeriodStartDate;
                            objRentDetail.FinPeriodEndDate = obj.FinPeriodEndDate;
                            objRentDetail.MonthlyRentAmount = obj.MonthlyRentAmount;
                            objRentDetail.CityCategory = obj.CityCategory;
                            objRentDetail.TotalRentAmount = obj.TotalRentAmount;
                            objRentDetail.MultipleLandlord = obj.MultipleLandlord;
                            objRentDetail.EmployeeSO = obj.EmployeeSO;
                            objRentDetail.PresentResidentialAddress = obj.PresentResidentialAddress;
                            objRentDetail.TotalRentAmount = MonthAmountcalculate(objRentDetail.MonthlyRentAmount, DateTime.Parse(objRentDetail.FinPeriodStartDate), DateTime.Parse(objRentDetail.FinPeriodEndDate)).ToString();

                            objRentDetail.LandLord1 = new LandlordDeclarationViewModel
                            {
                                LandlordName = obj.LandLord1.LandlordName,
                                LandlordPAN = obj.LandLord1.LandlordPAN,
                                // ResA = txtpraddress,


                                LandlordSO = obj.LandLord1.LandlordSO,
                                LandlordResHNo = obj.LandLord1.LandlordResHNo,

                                LandlordResGNo = obj.LandLord1.LandlordResGNo,

                                LandlordResVillage = obj.LandLord1.LandlordResVillage.Trim(),
                                LandlordResCity = obj.LandLord1.LandlordResCity.Trim(),
                                LandlordResPincode = obj.LandLord1.LandlordResPincode.Trim(),
                                LandlordMobileNo = obj.LandLord1.LandlordMobileNo.Trim()
                            };
                            objRentDetail.LandLord2 = new LandlordDeclarationViewModel
                            {
                                LandlordName = obj.LandLord2.LandlordName,
                                LandlordPAN = obj.LandLord2.LandlordPAN,
                                // ResA = txtpraddress,


                                LandlordSO = obj.LandLord2.LandlordSO,
                                LandlordResHNo = obj.LandLord2.LandlordResHNo,

                                LandlordResGNo = obj.LandLord2.LandlordResGNo,

                                LandlordResVillage = obj.LandLord2.LandlordResVillage.Trim(),
                                LandlordResCity = obj.LandLord2.LandlordResCity.Trim(),
                                LandlordResPincode = obj.LandLord2.LandlordResPincode.Trim(),
                                LandlordMobileNo = obj.LandLord2.LandlordMobileNo.Trim()
                            };
                            objRentDetail.LandLord3 = new LandlordDeclarationViewModel
                            {
                                LandlordName = obj.LandLord3.LandlordName,
                                LandlordPAN = obj.LandLord3.LandlordPAN,
                                // ResA = txtpraddress,


                                LandlordSO = obj.LandLord3.LandlordSO,
                                LandlordResHNo = obj.LandLord3.LandlordResHNo,

                                LandlordResGNo = obj.LandLord3.LandlordResGNo,

                                LandlordResVillage = obj.LandLord3.LandlordResVillage.Trim(),
                                LandlordResCity = obj.LandLord3.LandlordResCity.Trim(),
                                LandlordResPincode = obj.LandLord3.LandlordResPincode.Trim(),
                                LandlordMobileNo = obj.LandLord3.LandlordMobileNo.Trim()
                            };
                            objRentDetail.LandLord4 = new LandlordDeclarationViewModel
                            {
                                LandlordName = obj.LandLord4.LandlordName,
                                LandlordPAN = obj.LandLord4.LandlordPAN,
                                // ResA = txtpraddress,


                                LandlordSO = obj.LandLord4.LandlordSO,
                                LandlordResHNo = obj.LandLord4.LandlordResHNo,

                                LandlordResGNo = obj.LandLord4.LandlordResGNo,

                                LandlordResVillage = obj.LandLord4.LandlordResVillage.Trim(),
                                LandlordResCity = obj.LandLord4.LandlordResCity.Trim(),
                                LandlordResPincode = obj.LandLord4.LandlordResPincode.Trim(),
                                LandlordMobileNo = obj.LandLord4.LandlordMobileNo.Trim()
                            };

                            _listRentDetail.Add(objRentDetail);
                            _listRentDetail = _listRentDetail.OrderBy(x => x.strgridindex).ToList();

                            data.hRADetails.ActionType = "";
                            data.hRAList = _listRentDetail;
                            HRASession.HRADetaillist = data;
                            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(HRASession));
                            return PartialView("_HRADetails", data);

                        }
                        else
                        {

                            return Json(new Tuple<string, string>("error", "Please check From Date and To Date."));


                        }

                    }
                }
                else
                {
                    HRADetails objRentDetail = new HRADetails();
                    //objRentDetail._strFINEMPRENTDTLID = (_listRentDetail.Count) + 1; ;
                    objRentDetail.strgridindex = 1;
                    objRentDetail.SLNO = 1;
                    objRentDetail.FinPeriodStartDate = obj.FinPeriodStartDate;
                    objRentDetail.FinPeriodEndDate = obj.FinPeriodEndDate;
                    objRentDetail.MonthlyRentAmount = obj.MonthlyRentAmount;
                    objRentDetail.CityCategory = obj.CityCategory;
                    objRentDetail.TotalRentAmount = obj.TotalRentAmount;
                    objRentDetail.MultipleLandlord = obj.MultipleLandlord;
                    objRentDetail.EmployeeSO = obj.EmployeeSO;
                    objRentDetail.PresentResidentialAddress = obj.PresentResidentialAddress;
                    objRentDetail.TotalRentAmount = MonthAmountcalculate(objRentDetail.MonthlyRentAmount, DateTime.Parse(objRentDetail.FinPeriodStartDate), DateTime.Parse(objRentDetail.FinPeriodEndDate)).ToString();

                    objRentDetail.LandLord1 = new LandlordDeclarationViewModel
                    {
                        LandlordName = obj.LandLord1.LandlordName,
                        LandlordPAN = obj.LandLord1.LandlordPAN,
                        LandlordSO = obj.LandLord1.LandlordSO,
                        LandlordResHNo = obj.LandLord1.LandlordResHNo,

                        LandlordResGNo = obj.LandLord1.LandlordResGNo,

                        LandlordResVillage = obj.LandLord1.LandlordResVillage.Trim(),
                        LandlordResCity = obj.LandLord1.LandlordResCity.Trim(),
                        LandlordResPincode = obj.LandLord1.LandlordResPincode.Trim(),
                        LandlordMobileNo = obj.LandLord1.LandlordMobileNo.Trim()
                    };
                    objRentDetail.LandLord2 = new LandlordDeclarationViewModel
                    {
                        LandlordName = obj.LandLord2.LandlordName,
                        LandlordPAN = obj.LandLord2.LandlordPAN,
                        // ResA = txtpraddress,


                        LandlordSO = obj.LandLord2.LandlordSO,
                        LandlordResHNo = obj.LandLord2.LandlordResHNo,

                        LandlordResGNo = obj.LandLord2.LandlordResGNo,

                        LandlordResVillage = obj.LandLord2.LandlordResVillage.Trim(),
                        LandlordResCity = obj.LandLord2.LandlordResCity.Trim(),
                        LandlordResPincode = obj.LandLord2.LandlordResPincode.Trim(),
                        LandlordMobileNo = obj.LandLord2.LandlordMobileNo.Trim()
                    };
                    objRentDetail.LandLord3 = new LandlordDeclarationViewModel
                    {
                        LandlordName = obj.LandLord3.LandlordName,
                        LandlordPAN = obj.LandLord3.LandlordPAN,
                        // ResA = txtpraddress,


                        LandlordSO = obj.LandLord3.LandlordSO,
                        LandlordResHNo = obj.LandLord3.LandlordResHNo,

                        LandlordResGNo = obj.LandLord3.LandlordResGNo,

                        LandlordResVillage = obj.LandLord3.LandlordResVillage.Trim(),
                        LandlordResCity = obj.LandLord3.LandlordResCity.Trim(),
                        LandlordResPincode = obj.LandLord3.LandlordResPincode.Trim(),
                        LandlordMobileNo = obj.LandLord3.LandlordMobileNo.Trim()
                    };
                    objRentDetail.LandLord4 = new LandlordDeclarationViewModel
                    {
                        LandlordName = obj.LandLord4.LandlordName,
                        LandlordPAN = obj.LandLord4.LandlordPAN,
                        // ResA = txtpraddress,


                        LandlordSO = obj.LandLord4.LandlordSO,
                        LandlordResHNo = obj.LandLord4.LandlordResHNo,

                        LandlordResGNo = obj.LandLord4.LandlordResGNo,

                        LandlordResVillage = obj.LandLord4.LandlordResVillage.Trim(),
                        LandlordResCity = obj.LandLord4.LandlordResCity.Trim(),
                        LandlordResPincode = obj.LandLord4.LandlordResPincode.Trim(),
                        LandlordMobileNo = obj.LandLord4.LandlordMobileNo.Trim()
                    };
                    if (totalrentamount > Convert.ToInt32(obj.TotalRentAmount))
                    {

                        return Json(new Tuple<string, string>("error", "Annual rent amount exceeded"));


                    }
                    _listRentDetail.Add(objRentDetail);

                    data.hRAList = _listRentDetail;
                    data.hRADetails.ActionType = "";
                    HRASession.HRADetaillist = data;
                    _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(HRASession));
                    return PartialView("_HRADetails", data);

                }
            }


        }

        [HttpPost]
        public async Task<IActionResult> EditHRADetails(string KIID, int Id)
        {
            var HraVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var HRASession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(HraVal);
            HRADetailList data = new HRADetailList();
            if (HRASession.HRADetaillist != null)
            {
                data = HRASession.HRADetaillist;
            }

            data.hRADetails = data.hRAList.Where(x => x.SLNO == Id).FirstOrDefault() ?? new HRADetails();

            BindDropDown(data.hRADetails);
            data.hRADetails.PeriodFromMonth = DateTime.Parse(data.hRADetails.FinPeriodStartDate).Month;
            data.hRADetails.PeriodToMonth = DateTime.Parse(data.hRADetails.FinPeriodEndDate).Month;
            // data.hRAList = prvList;
            data.hRADetails.ActionType = "UPDATE";
            data.hRADetails.SLNO = Id;
            if (data.hRADetails.LandLord2.LandlordName != "")
            {
                data.hRADetails.MultipleLandlord = true;
            }

            return PartialView("_HRADetails", data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHRADetails(string KIID, int Id)
        {
            var HraVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var HRASession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(HraVal);
            HRADetailList data = new HRADetailList();
            if (HRASession.HRADetaillist != null)
            {
                data = HRASession.HRADetaillist;
            }

            data.hRAList = data.hRAList.Where(x => x.SLNO != Id).ToList();

            BindDropDown(data.hRADetails);

            HRASession.HRADetaillist = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(HRASession));
            return PartialView("_HRADetails", data);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitHRADetails(string AnnualRent)
        {
            Tuple<string, string> retVal_tuple;
            var HraVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var HRASession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(HraVal);
            if (HRASession.HRADetaillist.hRAList == null)
            {
                return Json(new Tuple<string, string>("error", "Please add HRA Details"));
            }
            var TotalRean = HRASession.HRADetaillist.hRAList.Sum(x => int.Parse(x.MonthlyRentAmount));
            if (TotalRean > int.Parse(AnnualRent))
            {
                return Json(new Tuple<string, string>("error", "Annual Rent is less then monthly rent amount"));

            }
            HRASession.txtrentamount1 = AnnualRent;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(HRASession));
            return Json(new Tuple<string, string>("success", ""));
        }
        [HttpPost]
        public async Task<IActionResult> ViewReceiptDetails(string Head, string TaxId)
        {



            if (Head.ToUpper() == "INTEREST PAID ON HOME LOAN (U/S 80EE)")
            {
                var EEBVal = _sessionService.Get<string>("EMPTAX_INVDEC");
                var EEBSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(EEBVal);
                string strKiId = _objIPMS.GetKIId();
                EEDetailsModels data = new EEDetailsModels();
                data.TaxId = TaxId;
                var EEDetails = HttpContext.Session.GetObject<EEDetailsModels>(TaxId);
                if (EEBSession.EE != null)
                {
                    data = EEBSession.EE;
                }

                else
                {
                    DataTable DT = new DataTable();
                    DT = objtax.Get80EEDTL(_sessionService.Get<string>("userID"), strKiId);
                    if (DT.Rows.Count > 0)
                    {
                        data.Loansanctiondate = DT.Rows[0]["LSDATTE"].ToString();
                        data.Loansanctionamount = DT.Rows[0]["LSAMOUNT"].ToString();
                        data.Vofhouse = DT.Rows[0]["TVRHP"].ToString();
                        data.Ownerofoterhhouse = DT.Rows[0]["ISEMPOOTHHOUSE"].ToString();
                        data.Ownerofproperty = DT.Rows[0]["OWNEROFPROPERTY"].ToString();
                        data.Popunderconstr = DT.Rows[0]["ISPUC"].ToString();
                        data.Compofconstdate = DT.Rows[0]["DATEOFCOMP"].ToString();
                        data.Amtclaimed24b = DT.Rows[0]["IAC24B"].ToString();
                        data.Perofshareproperty = DT.Rows[0]["PSP"].ToString();
                        data.Perofshareintax = DT.Rows[0]["PSTE"].ToString();
                        data.Declamount = DT.Rows[0]["DECLAREAMOUNT"].ToString();
                        //txt_amount_OnTextChanged(sender, e);
                    }
                    if (data.Perofshareintax != "" && data.Declamount != "")
                    {
                        data.EEamount = ((Convert.ToInt64(data.Declamount) * Convert.ToInt32(data.Perofshareintax)) / 100).ToString();
                        if (Convert.ToInt64(data.EEamount) > 50000)
                        {
                            data.EEamount = "50000";
                        }
                    }
                    EEBSession.EE = data;

                    _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(EEBSession));
                }
                return PartialView("_EEDetails", data);
                //mpeEditEE.Show();
                //ScriptManager.RegisterClientScriptBlock(Page, GetType(), "alt", "javascript:fnOpen_585('Emp80EE_Form.aspx?id=" + hdntaxid.Value + "'," + gr.RowIndex + ");", true);
            }
            else if (Head.ToUpper() == "INTEREST PAID ON ELECTRIC VEHICLE  (U/S 80EEB)")
            {
                var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
                var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
                var ReciptsList = ReciptsSession.ReceiptIndexViewModel?.Where(x => x.TaxId == TaxId).FirstOrDefault();
                ReceiptIndexViewModel data = new ReceiptIndexViewModel();
                var ReceiptDetails = HttpContext.Session.GetObject<ReceiptIndexViewModel>(TaxId);
                if (ReciptsList != null)
                {
                    data = ReciptsList;
                }
                else
                {
                    if (ReciptsSession.ReceiptIndexViewModel == null)
                    {
                        ReciptsSession.ReceiptIndexViewModel = new List<ReceiptIndexViewModel>();
                    }
                    data = TDS585(TaxId);
                    data.TaxId = TaxId;
                    data.NewReceipt = new ReceiptDtl_586();
                    data.NewReceipt._TaxHeadID = TaxId;
                    ReciptsSession.ReceiptIndexViewModel.Add(data);
                    _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
                }
                return PartialView("_EEBDetails", data);
            }
            else if (Head.Equals("Life Insurance Premium", StringComparison.OrdinalIgnoreCase))
            {
                var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
                var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
                var ReciptsList = ReciptsSession.ReceiptIndexViewModel?.Where(x => x.TaxId == TaxId).FirstOrDefault();
                ReceiptIndexViewModel data = new ReceiptIndexViewModel();

                if (ReciptsList != null)
                {
                    data = ReciptsList;
                }
                else
                {
                    if (ReciptsSession.ReceiptIndexViewModel == null)
                    {
                        ReciptsSession.ReceiptIndexViewModel = new List<ReceiptIndexViewModel>();
                    }
                    data = TDS585_LIC(TaxId);
                    data.TaxId = TaxId;
                    data.NewReceipt = new ReceiptDtl_586();
                    data.NewReceipt._TaxHeadID = TaxId;


                }
                var msg = UpdateCheckReceiptAndSum(data.Receipts);
                data.Total = msg.Item2[0];
                ReciptsSession.ReceiptIndexViewModel.Add(data);
                _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
                return PartialView("_LICDetails", data);

            }
            else if (Head.Equals("Unit Linked Insurance Policy (ULIP)", StringComparison.OrdinalIgnoreCase))
            {
                var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
                var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
                var ReciptsList = ReciptsSession.ReceiptIndexViewModel?.Where(x => x.TaxId == TaxId).FirstOrDefault();
                ReceiptIndexViewModel data = new ReceiptIndexViewModel();
                var ReceiptDetails = HttpContext.Session.GetObject<ReceiptIndexViewModel>(TaxId);
                if (ReciptsList != null)
                {
                    data = ReciptsList;
                }
                else
                {
                    if (ReciptsSession.ReceiptIndexViewModel == null)
                    {
                        ReciptsSession.ReceiptIndexViewModel = new List<ReceiptIndexViewModel>();
                    }
                    data = TDS585_LIC(TaxId);
                    data.TaxId = TaxId;
                    data.NewReceipt = new ReceiptDtl_586();
                    data.NewReceipt._TaxHeadID = TaxId;


                }
                var msg = UpdateCheckReceiptAndSumULIP(data.Receipts);
                data.Total = msg.Item2[0];
                ReciptsSession.ReceiptIndexViewModel.Add(data);
                _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
                return PartialView("_LICDetails", data);
            }
            else
            {
                var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
                var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
                var ReciptsList = ReciptsSession.ReceiptIndexViewModel?.Where(x => x.TaxId == TaxId).FirstOrDefault();
                ReceiptIndexViewModel data = new ReceiptIndexViewModel();
                var ReceiptDetails = HttpContext.Session.GetObject<ReceiptIndexViewModel>(TaxId);
                if (ReciptsList != null)
                {
                    data = ReciptsList;
                }
                else
                {
                    if (ReciptsSession.ReceiptIndexViewModel == null)
                    {
                        ReciptsSession.ReceiptIndexViewModel = new List<ReceiptIndexViewModel>();
                    }
                    data = TDS585(TaxId);
                    data.TaxId = TaxId;
                    data.NewReceipt = new ReceiptDtl_586();
                    data.NewReceipt._TaxHeadID = TaxId;
                    ReciptsSession.ReceiptIndexViewModel.Add(data);
                    _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
                }
                return PartialView("_ReceiptsDetails", data);
            }





        }
        [HttpPost]
        public async Task<IActionResult> AddReceiptDetails(string TaxId, int Id, string ReceiptNo, string Date, string Amount)
        {
            var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
            var ReciptsList = ReciptsSession.ReceiptIndexViewModel.Where(x => x.TaxId == TaxId).FirstOrDefault();
            ReceiptIndexViewModel data = new ReceiptIndexViewModel();
            if (ReciptsList != null)
            {
                data = ReciptsList;
            }
            if (ReceiptNo == "" || Date == "" || Amount == "")
            {
                return Json(new Tuple<string, string>("error", "Please fill the mandatory fields"));

            }
            DataTable dt = new DataTable();

            var strKiId = _objIPMS.GetKIId();
            dt = objtax.GetEmpRent_Invdetail(_sessionService.Get<string>("userID"), strKiId);
            if (dt.Rows.Count > 0)
            {
                string strfromdate = dt.Rows[0]["RENT_FROM"].ToString();
                DateTime strStartdate = DateTime.ParseExact(strfromdate, "dd-MMM-yyyy", null);
                DateTime strrecptdate = DateTime.ParseExact(Date, "dd-MMM-yyyy", null);
                DateTime strtodaydate = DateTime.Today;
                if (!(strtodaydate.Date >= strrecptdate.Date && strrecptdate.Date >= strStartdate.Date))
                {
                    string strmsg = "Receipt date should not be grater than current date and less than " + strStartdate;
                    return Json(new Tuple<string, string>("error", strmsg));
                }
            }
            if (Id != 0)
            {
                data.Receipts = data.Receipts.Where(x => x.SLNO != Id).ToList();
                ReceiptDtl_586 obj = new ReceiptDtl_586();
                obj.SLNO = Id;
                obj._ReceiptNo = ReceiptNo;
                obj._PrmDate = Date;
                obj.Amount = Amount;
                obj._TaxHeadID = TaxId.ToString();
                data.Receipts.Insert((Id - 1), obj);
            }
            else
            {
                ReceiptDtl_586 obj = new ReceiptDtl_586();
                obj.SLNO = (data.Receipts.Count() + 1);
                obj._ReceiptNo = ReceiptNo;
                obj._PrmDate = Date;
                obj.Amount = Amount;
                obj._TaxHeadID = TaxId.ToString();
                data.Receipts.Add(obj);
            }
            ReciptsList = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
            HttpContext.Session.SetObject<ReceiptIndexViewModel>(TaxId, data);
            return PartialView("_ReceiptsDetails", data);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteReceiptDetails(int Id, string TaxId)
        {
            var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
            var ReciptsList = ReciptsSession.ReceiptIndexViewModel.Where(x => x.TaxId == TaxId).FirstOrDefault();
            ReceiptIndexViewModel data = new ReceiptIndexViewModel();
            if (ReciptsList != null)
            {
                data = ReciptsList;
            }
            data.Receipts = data.Receipts.Where(x => x.SLNO != Id).ToList();
            ReciptsList = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
            return PartialView("_ReceiptsDetails", data);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitReceiptDetails(string TaxId, string TaxHead)
        {
            var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
            var ReciptsList = ReciptsSession.ReceiptIndexViewModel.Where(x => x.TaxId == TaxId).FirstOrDefault();
            ReceiptIndexViewModel data = new ReceiptIndexViewModel();
            if (ReciptsList != null)
            {
                data = ReciptsList;
            }
            Int64 total = data.Receipts.Sum(x => Int64.Parse(x.Amount));
            if (TaxHead == "INTEREST PAID ON ELECTRIC VEHICLE  (U/S 80EEB)")
            {
                if (total > 150000)
                {
                    total = 150000;
                }
            }
            ReciptsSession = UpdateProjAmount(ReciptsSession, TaxId, total.ToString());
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
            return Json(total);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitLicDetails(string TaxId, string TaxHead, string Amt)
        {
            var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);

            ReciptsSession = UpdateProjAmount(ReciptsSession, TaxId, Amt.ToString());
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
            return Json(Amt);
        }
        [HttpPost]
        public async Task<IActionResult> AddEEBDetails(string TaxId, int Id, string ReceiptNo, string Date, string Amount)
        {
            var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
            var ReciptsList = ReciptsSession.ReceiptIndexViewModel.Where(x => x.TaxId == TaxId).FirstOrDefault();
            ReceiptIndexViewModel data = new ReceiptIndexViewModel();
            if (ReciptsList != null)
            {
                data = ReciptsList;
            }
            if (ReceiptNo == "" || Date == "" || Amount == "")
            {
                return Json(new Tuple<string, string>("error", "Please fill the mandatory fields"));

            }
            if (ReceiptNo == "0")
            {
                return Json(new Tuple<string, string>("error", "Deductions benefits only for Electric Vehicle"));

            }
            if (Convert.ToInt32(Amount) > 150000)
            {
                return Json(new Tuple<string, string>("error", "Interest paid amount should not exceed 150000/-."));

            }

            string strfromdate = "01-Apr-2019";
            string strtodate = "31-Mar-2023";
            DateTime strStartdate = DateTime.ParseExact(strfromdate, "dd-MMM-yyyy", null);
            DateTime strenddate = DateTime.ParseExact(strtodate, "dd-MMM-yyyy", null);
            DateTime strrecptdate = DateTime.ParseExact(Date, "dd-MMM-yyyy", null);
            DateTime strtodaydate = DateTime.Today;
            if (!(strtodaydate.Date >= strrecptdate.Date && strrecptdate.Date >= strStartdate.Date && strrecptdate.Date <= strenddate.Date))
            {
                string strmsg = "Date of loan sanctioned should not be grater than current date and less than " + strStartdate;
                return Json(new Tuple<string, string>("error", strmsg));

            }
            if (!(strrecptdate.Date <= strenddate.Date))
            {
                string strmsg = "Date of loan sanctioned should not be grater than " + strenddate;
                return Json(new Tuple<string, string>("error", strmsg));

            }

            if (Id != 0)
            {
                data.Receipts = data.Receipts.Where(x => x.SLNO != Id).ToList();
                ReceiptDtl_586 obj = new ReceiptDtl_586();
                obj.SLNO = Id;
                obj._ReceiptNo = ReceiptNo;
                obj._PrmDate = Date;
                obj.Amount = Amount;
                obj._TaxHeadID = TaxId.ToString();
                data.Receipts.Insert((Id - 1), obj);
            }
            else
            {
                ReceiptDtl_586 obj = new ReceiptDtl_586();
                obj.SLNO = (data.Receipts.Count() + 1);
                obj._ReceiptNo = ReceiptNo;
                obj._PrmDate = Date;
                obj.Amount = Amount;
                obj._TaxHeadID = TaxId.ToString();
                data.Receipts.Add(obj);
            }

            ReciptsList = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
            return PartialView("_EEBDetails", data);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteEEBDetails(int Id, string TaxId)
        {
            var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
            var ReciptsList = ReciptsSession.ReceiptIndexViewModel.Where(x => x.TaxId == TaxId).FirstOrDefault();
            ReceiptIndexViewModel data = new ReceiptIndexViewModel();
            if (ReciptsList != null)
            {
                data = ReciptsList;
            }
            data.Receipts = data.Receipts.Where(x => x.SLNO != Id).ToList();
            ReciptsList = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
            return PartialView("_EEBDetails", data);
        }

        [HttpPost]
        public async Task<IActionResult> AddLICDetails(string TaxHead, string TaxId, int Id, string Policyno, string PrmDate, string SumAssured, string ReceiptNo, string Date, string Amount)
        {
            var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
            var ReciptsList = ReciptsSession.ReceiptIndexViewModel.Where(x => x.TaxId == TaxId).FirstOrDefault();
            ReceiptIndexViewModel data = new ReceiptIndexViewModel();
            if (ReciptsList != null)
            {
                data = ReciptsList;
            }


            if (SumAssured == "" || Policyno == "" || ReceiptNo == "" || Date == "" || PrmDate == "" || Amount == "")
            {
                return Json(new Tuple<string, string>("error", "Please fill the mandatory fields."));

            }
            DataTable dt = new DataTable();
            var strKiId = _objIPMS.GetKIId();
            dt = objtax.GetEmpRent_Invdetail(_sessionService.Get<string>("userID"), strKiId);
            if (dt.Rows.Count > 0)
            {
                string strfromdate = dt.Rows[0]["RENT_FROM"].ToString();
                DateTime strStartdate = DateTime.ParseExact(strfromdate, "dd-MMM-yyyy", null);
                DateTime strrecptdate = DateTime.ParseExact(Date, "dd-MMM-yyyy", null);
                DateTime strtodaydate = DateTime.Today;
                if (!(strtodaydate.Date >= strrecptdate.Date && strrecptdate.Date >= strStartdate.Date))
                {
                    string strmsg = "Receipt date should not be greater than current date and less than " + strStartdate;
                    return Json(new Tuple<string, string>("error", strmsg));
                }
            }



            if (Id != 0)
            {
                data.Receipts = data.Receipts.Where(x => x.SLNO != Id).ToList();
                ReceiptDtl_586 obj = new ReceiptDtl_586();
                obj.SLNO = Id;
                obj._ReceiptNo = ReceiptNo;
                obj._PrmDate = Date;
                obj.Amount = Amount;
                obj._PolicyNo = Policyno;
                obj._Date = PrmDate;
                obj._SumAssured = SumAssured;
                obj._TaxHeadID = TaxId.ToString();
                data.Receipts.Insert((Id - 1), obj);
            }
            else
            {
                ReceiptDtl_586 obj = new ReceiptDtl_586();
                obj.SLNO = (data.Receipts.Count() + 1);
                obj._ReceiptNo = ReceiptNo;
                obj._PrmDate = Date;
                obj.Amount = Amount;
                obj._PolicyNo = Policyno;
                obj._Date = PrmDate;
                obj._SumAssured = SumAssured;
                obj._TaxHeadID = TaxId.ToString();
                data.Receipts.Add(obj);
            }
            var Msg = TaxHead.Equals("Life Insurance Premium", StringComparison.OrdinalIgnoreCase) ? UpdateCheckReceiptAndSum(data.Receipts) : UpdateCheckReceiptAndSumULIP(data.Receipts);
            if (Msg.Item1)
            {
                data.Total = Msg.Item2[0];


            }
            else if (Msg.Item2[1] == "SA")
            {

                return Json(new Tuple<string, string>("error", "Sum Assured amount should be same for same policy"));

            }
            else if (Msg.Item2[1] == "PD")
            {
                return Json(new Tuple<string, string>("error", "For same policy different date of issuance is not valid"));

            }
            else if (Msg.Item2[1] == "RD")
            {
                return Json(new Tuple<string, string>("error", "Under same policy same receipt number is not valid."));

            }
            else if (Msg.Item2[1] == "SA")
            {
                return Json(new Tuple<string, string>("error", "Sum Assured amount should be same for same policy"));
            }
            else if (Msg.Item2[1] == "PD")
            {
                return Json(new Tuple<string, string>("error", "For same policy different date of issuance is not valid."));
            }
            else if (Msg.Item2[1] == "RD")
            {
                return Json(new Tuple<string, string>("error", "Under same policy same receipt number is not valid."));
            }
            ReciptsList = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
            return PartialView("_LICDetails", data);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteLICDetails(int Id, string TaxId)
        {
            var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
            var ReciptsList = ReciptsSession.ReceiptIndexViewModel.Where(x => x.TaxId == TaxId).FirstOrDefault();
            ReceiptIndexViewModel data = new ReceiptIndexViewModel();
            if (ReciptsList != null)
            {
                data = ReciptsList;
            }
            data.Receipts = data.Receipts.Where(x => x.SLNO != Id).ToList();
            var Msg = UpdateCheckReceiptAndSum(data.Receipts);
            if (Msg.Item1)
            {
                data.Total = Msg.Item2[0];


            }
            ReciptsList = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
            return PartialView("_LICDetails", data);
        }
        [HttpGet]
        public async Task<IActionResult> GetLoanDetails(string ki, string TaxId)
        {
            var LoanVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LoanSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LoanVal);

            var model = new LoanApplicationViewModel();

            if (LoanSession.LoanApplicationViewModel != null)
            {
                model = LoanSession.LoanApplicationViewModel;
            }
            else
            {
                if (LoanSession.ProjStatus == "2")
                {
                    var dt = objtax.GetLOANDTL(_sessionService.Get<string>("userID"), ki);
                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        model.IsUnderCons = row["ISUC"]?.ToString();
                        model.IsEmployeeOwner = row["OTHHOUSEOWNER"]?.ToString();
                        model.ID = row["FIN_TAXLOANDTLID"]?.ToString();
                        model.ConsDate = row["DATEOFCOMPLETION"]?.ToString();
                        model.LoanTaken = row["DATELOANTKN"]?.ToString();
                        model.PropertyOwner = row["ISOWNER"]?.ToString();
                        model.ShareInProperty = row["SHAREPER"].ToString();
                        model.CurrInterest = row["CURRINTREST"]?.ToString();
                        model.PreYear1 = row["YEAR1INTEREST"]?.ToString();
                        model.PreYear2 = row["YEAR2INTEREST"]?.ToString();
                        model.PreYear3 = row["YEAR3INTEREST"]?.ToString();
                        model.PreYear4 = row["YEAR4INTEREST"]?.ToString();
                        model.PreYear5 = row["YEAR5INTEREST"]?.ToString();
                        model.ShareInTaxExemption = row["TAXEXEMPSHAREPER"].ToString();

                        model.IsJointLoan = row["ISJOINTLOAN"]?.ToString();

                        model.RelationshipId = row["RELATION"]?.ToString();

                        model.HouseAddress = row["ADDRESSOFHLT"]?.ToString();
                        model.SelfOccupancyFrom = row["SELFOCCUPFROM"]?.ToString();
                        model.SelfOccupancyTo = row["SELFOCCUPTO"]?.ToString();
                        model.FinalTotalAmount = row["LOAN_AMOUNT"]?.ToString();
                        model.LoanProvider = new LoanProviderModel
                        {
                            LoanAmount = row["LOAN_AMOUNT"]?.ToString(),
                            LoanProvider = row["LOAD_PROVIDER"]?.ToString(),
                            AddressLoanProvider = row["LOANPROVIDER_ADDRESS"]?.ToString(),
                            PanLoanProvider = row["LOANPROVIDER_PAN"]?.ToString()
                        };
                        model.LoanAmount = row["LOAN_AMOUNT"]?.ToString();
                        model.LoanProviderName = row["LOAD_PROVIDER"]?.ToString();
                        model.AddressLoanProvider = row["LOANPROVIDER_ADDRESS"]?.ToString();
                        model.PanLoanProvider = row["LOANPROVIDER_PAN"]?.ToString();
                        model.AnnualRent = row["ANNUAL_RENT"]?.ToString();
                        model.MunicipalTax = row["MUNICIPAL_TAX"]?.ToString();
                        model.ValueOfHouse = row["VALUEOFHOUSE"]?.ToString();

                        // Load house loan providers
                        var houseDt = objtax.GetHOUSELOANDTL(row["FIN_TAXLOANDTLID"].ToString());
                        model.LoanProviders = new List<LoanProviderModel>();
                        foreach (DataRow hRow in houseDt.Rows)
                        {
                            model.LoanProviders.Add(new LoanProviderModel
                            {
                                SLNO = (model.LoanProviders.Count + 1).ToString(),
                                Id = hRow["HOUSELOANDTLID"]?.ToString(),
                                LoanAmount = hRow["LoanAmount"]?.ToString(),
                                LoanProvider = hRow["LoanProvider"]?.ToString(),
                                AddressLoanProvider = hRow["addressLoanProvider"]?.ToString(),
                                PanLoanProvider = hRow["panLoanProvider"]?.ToString()
                            });
                        }
                    }
                    var years = objtax.GetFINANCIALYEAR();

                    model.CurrentYear = years.Rows[0]["FINANCIALYEAR"].ToString();
                    model.Year1 = years.Rows[1]["FINANCIALYEAR"].ToString();
                    model.Year2 = years.Rows[2]["FINANCIALYEAR"].ToString();
                    model.Year3 = years.Rows[3]["FINANCIALYEAR"].ToString();
                    model.Year4 = years.Rows[4]["FINANCIALYEAR"].ToString();
                    model.Year5 = years.Rows[5]["FINANCIALYEAR"].ToString();
                }
                else
                {
                   var  DT = objtax.GetLOANDTL_proj(_sessionService.Get<string>("userID"), ki);
                    if (DT.Rows.Count > 0)
                    {
                        model.ProjIntrest = DT.Rows[0]["INTERESTAMOUNT"].ToString();
                        model.LoanAmount = DT.Rows[0]["LOAN_AMOUNT"].ToString();
                        model.LoanProviderName = DT.Rows[0]["LOAN_PROVIDER"].ToString();
                        model.AddressLoanProvider = DT.Rows[0]["LOAN_PROVIDER_ADDRESS"].ToString();
                        model.PanLoanProvider = DT.Rows[0]["LOAN_PROVIDER_PAN"].ToString();
                        model.ProjLoanTaken = DT.Rows[0]["DATELOANTKN"].ToString();
                        model.ProjValueofHouse = DT.Rows[0]["VALUEOFHOUSE"].ToString();

                    }
                }
            }

            // Financial years
           
            model.TaxId = TaxId;
            LoanSession.LoanApplicationViewModel = model;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LoanSession));
            return DateOfLoanChanged(LoanSession.LoanApplicationViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AddLoanProvider([FromBody] LoanApplicationViewModel obj)
        {
            var LoanVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LoanSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LoanVal);
            var LoanList = obj;
            LoanList.Year1 = LoanSession.LoanApplicationViewModel.Year1;
            LoanList.Year2 = LoanSession.LoanApplicationViewModel.Year2;
            LoanList.Year3 = LoanSession.LoanApplicationViewModel.Year3;
            LoanList.Year4 = LoanSession.LoanApplicationViewModel.Year4;
            LoanList.Year5 = LoanSession.LoanApplicationViewModel.Year5;
            if (LoanSession.LoanApplicationViewModel != null)
            {
                LoanList.LoanProviders = LoanSession.LoanApplicationViewModel.LoanProviders;
            }
            

            if (obj.LoanProvider.LoanAmount == "" || obj.LoanProvider.LoanProvider == "" || obj.LoanProvider.AddressLoanProvider == "" || obj.LoanProvider.PanLoanProvider == "")
            {
                return Json(new Tuple<string, string>("error", "Loan Amount,Provider Name,Address and Pancard no. are mandatory fields."));

            }
            if (obj.LoanProvider.PanLoanProvider != "" && obj.LoanProvider.PanLoanProvider.Length != 10)
            {

                return Json(new Tuple<string, string>("error", "Please enter valid Pancard no."));

            }



            if (string.IsNullOrEmpty(obj.LoanProvider.SLNO))
            {

                if (LoanList.LoanProviders?.Count() == 4)
                {
                    return Json(new Tuple<string, string>("error", "Maximum 4 Record You can add in list."));

                }
                obj.LoanProvider.SLNO = ((LoanList.LoanProviders?.Count() ?? 0) + 1).ToString();
                if (LoanList.LoanProviders == null)
                {
                    LoanList.LoanProviders = new List<LoanProviderModel>();
                }

                LoanList.LoanProviders?.Add(obj.LoanProvider);
            }
            else
            {
                if (LoanList.LoanProviders != null)
                {
                    
                        int idIndex = LoanList.LoanProviders.FindIndex(x => x.SLNO == obj.LoanProvider.SLNO);
                        LoanList.LoanProviders = LoanList.LoanProviders.Where(x => x.SLNO != obj.LoanProvider.SLNO).ToList();
                        LoanList.LoanProviders.Insert(idIndex, obj.LoanProvider);
                   
                }
                else
                {
                    LoanList.LoanProviders.Add(obj.LoanProvider);
                }
                // ViewState["EditRow"] = null;

            }
            LoanSession.LoanApplicationViewModel = LoanList;
            LoanList.LoanProvider = new LoanProviderModel();
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LoanSession));
            return DateOfLoanChanged(LoanSession.LoanApplicationViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteLoanDetails(string Id)
        {
            var LoanVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LoanSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LoanVal);

            var LoanList = new LoanApplicationViewModel();

            if (LoanSession.LoanApplicationViewModel != null)
            {
                LoanList = LoanSession.LoanApplicationViewModel;
            }

            LoanList.LoanProviders = LoanList.LoanProviders.Where(x => x.SLNO != Id).ToList();
            LoanSession.LoanApplicationViewModel = LoanList;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LoanSession));
            return DateOfLoanChanged(LoanSession.LoanApplicationViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProjLoanDetails(string TaxId)
        {
            var LoanVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LoanSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LoanVal);

            LoanSession.LoanApplicationViewModel = new LoanApplicationViewModel();

            LoanSession = UpdateProjAmount(LoanSession, decimal.Parse(TaxId).ToString("0"), "0");
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LoanSession));
            return Json("0");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitLoanDetails([FromBody] LoanApplicationViewModel obj)
        {
            var LoanVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LoanSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LoanVal);

            var LoanList = obj;
            LoanList.PnlValidatedVisible = LoanSession.LoanApplicationViewModel.PnlValidatedVisible;
            LoanList.PnlStopVisible = LoanSession.LoanApplicationViewModel.PnlStopVisible;

            if (LoanSession.LoanApplicationViewModel != null)
            {
                obj.LoanProviders = LoanSession.LoanApplicationViewModel.LoanProviders;
            }
           

            if (obj.IsUnderCons == "")
            {
                return Json(new Tuple<string, string>("error", "Please select property under construction option"));

            }
            else
            {
                if (obj.IsUnderCons == "0" && obj.PropertyOwner == "-1")
                {
                    return Json(new Tuple<string, string>("error", "Please select owner of the property option"));

                }
                if (obj.IsUnderCons == "0" && (obj.ConsDate == "" || obj.LoanTaken == ""))
                {
                    return Json(new Tuple<string, string>("error", "Date of Completion of Construction and Date on which loan was taken are mandatory fields."));

                }
                if (obj.IsUnderCons == "0" && (obj.ValueOfHouse == ""))
                {
                    return Json(new Tuple<string, string>("error", "Value of house are mandatory fields."));

                }
                if (obj.PropertyOwner == "2" && obj.IsJointLoan == "")
                {
                    return Json(new Tuple<string, string>("error", "In case of joint property ,Is it a Joint Loan field is mandatory."));

                }
                if (obj.PropertyOwner == "2" && obj.IsJointLoan == "1" && (obj.ShareInProperty == "" || obj.ShareInTaxExemption == "" || obj.RelationshipId == "-1"))
                {
                    return Json(new Tuple<string, string>("error", "In case of joint loan :\\nRelationship\\nPercentage of share in property\\nPercentage of share in tax exemption \\nare mandatory fields."));

                }
                if (obj.PropertyOwner == "2" && obj.IsJointLoan == "1" && obj.ShareInTaxExemption != "" && Convert.ToInt64(obj.ShareInTaxExemption) > 100)
                {
                    return Json(new Tuple<string, string>("error", "Percentage of share in tax exemption should not be greater than 100%."));

                }
                if (obj.PropertyOwner == "2" && obj.IsJointLoan == "1" && obj.ShareInProperty != "" && Convert.ToInt64(obj.ShareInProperty) > 100)
                {
                    return Json(new Tuple<string, string>("error", "Percentage of share in property should not be greater than 100%."));

                }
                if (obj.IsUnderCons == "0" && (obj.SelfOccupancyFrom == "" || obj.SelfOccupancyTo == ""))
                {
                    return Json(new Tuple<string, string>("error", "Period of full self-occupancy is mendatory"));

                }
                if (obj.IsUnderCons == "0" && obj.HouseAddress == "")
                {
                    return Json(new Tuple<string, string>("error", "Address Of house for which Home loan Taken is mendatory"));

                }
                if (obj.IsUnderCons == "0")
                {
                    if (DateTime.ParseExact(obj.ConsDate, "dd-MMM-yyyy", null) > DateTime.ParseExact(DateTime.Now.Date.ToString("dd-MMM-yyyy"), "dd-MMM-yyyy", null))
                    {
                        return Json(new Tuple<string, string>("error", "Date of Completion of Construction is not grater than current date"));

                    }
                    if (DateTime.ParseExact(obj.LoanTaken, "dd-MMM-yyyy", null) > DateTime.ParseExact(DateTime.Now.Date.ToString("dd-MMM-yyyy"), "dd-MMM-yyyy", null))
                    {
                        return Json(new Tuple<string, string>("error", "Date on which loan was taken is not grater than current date"));

                    }
                    //IF NO Loan DETAIL IS FILLED
                    if ((obj.LoanProviders == null || obj.LoanProviders?.Count() == 0) && obj.PnlValidatedVisible)
                    {
                        return Json(new Tuple<string, string>("error", "Please Fill at least one Loan details."));


                    }

                }

                var TotalLoan = string.IsNullOrEmpty( obj.Total)?0: Convert.ToInt32(obj.Total);
                if (TotalLoan > 200000)
                {
                    if (Convert.ToInt32(obj.ValueOfHouse) <= 4500000 && Convert.ToInt32(obj.IsEmployeeOwner) == 0)
                    {
                        if ((DateTime.ParseExact(obj.LoanTaken, "dd-MMM-yyyy", null).Date > DateTime.ParseExact("01-Apr-2019", "dd-MMM-yyyy", null).Date) && (DateTime.ParseExact(obj.LoanTaken, "dd-MMM-yyyy", null).Date > DateTime.ParseExact("31-Mar-2021", "dd-MMM-yyyy", null).Date))
                        {
                            if (TotalLoan > 350000)
                            {
                                TotalLoan = 350000;
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                            TotalLoan = 200000;
                        }
                    }
                    else
                    {
                        TotalLoan = 200000;
                    }
                }
                else
                {

                }
                LoanList.FinalTotalAmount = TotalLoan.ToString();
                LoanSession.LoanApplicationViewModel = LoanList;
                LoanSession = UpdateProjAmount(LoanSession, decimal.Parse(obj.TaxId).ToString("0"), TotalLoan.ToString());
                _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LoanSession));

                return Json(TotalLoan);
            }


        }
        [HttpPost]
        public async Task<IActionResult> SubmitProjectedLoanDetails([FromBody] LoanApplicationViewModel obj)
        {
            var LoanVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LoanSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LoanVal);

            var LoanList = obj;


            if (obj.ProjLoanTaken == "")
            {
                return Json(new Tuple<string, string>("error", "Date on which loan was taken are mandatory fields."));

            }
            if (obj.ProjValueofHouse == "")
            {
                return Json(new Tuple<string, string>("error", "Value of house are mandatory fields."));

            }
            if (obj.ProjIntrest == "")
            {
                return Json(new Tuple<string, string>("error", "Date on which loan was taken are mandatory fields."));

            }

            if (obj.LoanAmount == "")
            {
                return Json(new Tuple<string, string>("error", "Loan Amount are mandatory fields."));

            }
            if (obj.LoanProviderName == "")
            {
                return Json(new Tuple<string, string>("error", "Lender Name are mandatory fields."));

            }
            if (obj.AddressLoanProvider == "")
            {
                return Json(new Tuple<string, string>("error", "Lender address are mandatory fields."));

            }
            if (obj.PanLoanProvider == "")
            {
                return Json(new Tuple<string, string>("error", "Lender pan are mandatory fields."));

            }
            if ((DateTime.ParseExact(obj.ProjLoanTaken, "dd-MMM-yyyy", null).Date > DateTime.ParseExact("01-Apr-2019", "dd-MMM-yyyy", null).Date) && (DateTime.ParseExact(obj.ProjLoanTaken, "dd-MMM-yyyy", null).Date < DateTime.ParseExact("31-Mar-2021", "dd-MMM-yyyy", null).Date) && Convert.ToInt32(obj.ProjValueofHouse) <= 4500000)
            {
                if (Convert.ToInt32(obj.ProjIntrest) > 350000)
                {
                    return Json(new Tuple<string, string>("error", "Interest amount should be maximum 350000"));
                    
                }
            }
            else
            {
                if (Convert.ToInt32(obj.ProjIntrest) > 200000)
                {
                    
                }
            }

            LoanSession.LoanApplicationViewModel = LoanList;
                LoanSession = UpdateProjAmount(LoanSession, decimal.Parse(obj.TaxId).ToString("0"), obj.ProjIntrest.ToString());
                _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LoanSession));

                return Json(obj.ProjIntrest);
            


        }

        [HttpPost]
        public IActionResult DateOfLoanChanged([FromBody] LoanApplicationViewModel obj)
        {
            var LoanVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LoanSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LoanVal);
            var LoanList = LoanSession.LoanApplicationViewModel;
            LoanList.ConsDate = obj.ConsDate;
            LoanList.LoanTaken = obj.LoanTaken;
            if (string.IsNullOrEmpty(obj.ConsDate) || string.IsNullOrEmpty(obj.LoanTaken))
            {

                LoanSession.LoanApplicationViewModel = LoanList;

                _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LoanSession));

                return PartialView("_LoanDetails", LoanList);

            }

            

            DateTime loanDate = DateTime.Parse(obj.LoanTaken);
            DateTime conEndDate = DateTime.Parse(obj.ConsDate);

            // --- Panel visibility logic (maps 1:1 from WebForms) ---
            // If loan month >= April, add 6 years; else add 5 years
            DateTime dt = loanDate.Month >= 4 ? loanDate.AddYears(6) : loanDate.AddYears(5);
            DateTime endOfLoanTakenFY = new DateTime(dt.Year, 3, 31);

            bool stop = conEndDate > endOfLoanTakenFY;

            LoanList.PnlStopVisible = stop;
            LoanList.PnlValidatedVisible = !stop;


            // --- Financial year start from loan date ---
            int financialYearStart = loanDate.Month >= 4 ? loanDate.Year : loanDate.Year - 1;

            // Helper: parse leading year (YYYY) from labels like "2021-22"
            static int YearStart(string label)
            {
                if (string.IsNullOrWhiteSpace(label)) return 0;
                var first = label.Split('-')[0];
                return int.TryParse(first, out var y) ? y : 0;
            }

            int y1 = YearStart(LoanList.Year1);
            int y2 = YearStart(LoanList.Year2);
            int y3 = YearStart(LoanList.Year3);
            int y4 = YearStart(LoanList.Year4);
            int y5 = YearStart(LoanList.Year5);
            int cmpConstructionYear = conEndDate.Month >= 4 ? conEndDate.Year : conEndDate.Year - 1;

            LoanList.PreYear1Enabled = !(y1 >= financialYearStart && y1 <= cmpConstructionYear);
            if (LoanList.PreYear1Enabled)
            {
                LoanList.PreYear1 = string.Empty;
            }
            LoanList.PreYear2Enabled = !(y2 >= financialYearStart && y2 <= cmpConstructionYear);
            if (LoanList.PreYear2Enabled)
            {
                LoanList.PreYear2 = string.Empty;

            }
            LoanList.PreYear3Enabled = !(y3 >= financialYearStart && y3 <= cmpConstructionYear);
            if (LoanList.PreYear3Enabled)
            {
                LoanList.PreYear3 = string.Empty;
            }
            LoanList.PreYear4Enabled = !(y4 >= financialYearStart && y4 <= cmpConstructionYear);
            if (LoanList.PreYear4Enabled)
            {
                LoanList.PreYear4 = string.Empty;
            }
            LoanList.PreYear5Enabled = !(y5 >= financialYearStart && y5 <= cmpConstructionYear);
            if (LoanList.PreYear5Enabled)
            {
                LoanList.PreYear5 = string.Empty;
            }

            LoanSession.LoanApplicationViewModel = LoanList;

            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LoanSession));

            return PartialView("_LoanDetails", LoanList);

        }



            [HttpPost]
        public async Task<IActionResult> GetLTADetails()
        {
            //Session["LTA_FAMILY_LISTVALID"] = ViewState["LTA_FAMILY_LIST"];
            //Session["LTADEC"] = ViewState["LTADEC"];
            var LTAVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LTASession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LTAVal);
            ReceiptDtl_LTAMain objLTA = new ReceiptDtl_LTAMain();
            if (LTASession.LTA != null)
            {
                objLTA = LTASession.LTA;
            }
            else
            {
                objLTA = await LTA();
            }
            LTASession.LTA = objLTA;
            objLTA.TotEligableAmt = objLTA.objLTAdtlcolls?.Where(X => !string.IsNullOrEmpty(X.Amount)).Sum(X => Int64.Parse(X.Amount)).ToString();
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LTASession));
            return PartialView("_LTAForm", objLTA);
            //ScriptManager.RegisterClientScriptBlock(Page, GetType(), "alt", "javascript:fOpen_LTADec('EmpTaxLTADec_Form.aspx');", true);

        }
        [HttpPost]
        public IActionResult GetFareEligibity(string TravalType, string lblname, string TravelMode, string NormalTrainFare, string Amount, int Index)
        {
            var LTAVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LTASession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LTAVal);
            ReceiptDtl_LTAMain objLTA = LTASession.LTA;

            if (TravalType == "1")
            {

                if (TravelMode == "1")
                {
                    if (!string.IsNullOrEmpty(NormalTrainFare))
                    {
                        string relation = objLTA.objLTAdtlcolls[Index].Relation;
                        
                            var lblGender = objLTA.objLTAdtlcolls[Index].Gender;
                            var txtage = objLTA.objLTAdtlcolls[Index].Age;
                            var txtfare = objLTA.objLTAdtlcolls[Index].TrainFare;
                           
                                string gender = string.Empty;
                                if (lblGender == "1") { gender = "MALE"; } else { gender = "FEMALE"; }
                                var dt = objtax.GetTrainFareEligPerAge(gender, txtage, NormalTrainFare);
                                if (dt.Rows.Count > 0)
                                {
                                    double amt = Convert.ToDouble(dt.Rows[0]["EligableFare"].ToString().Trim());
                            Int64 fare = (Int64)amt;
                            Int64 eligAmt = (Int64)amt;
                                    if (Amount!= "" && eligAmt>0)
                                    {
                                        eligAmt = Convert.ToInt64(Amount) > eligAmt ? eligAmt : Convert.ToInt64(Amount);

                                    }
                                    // result = dt.Rows[0]["EligableFare"].ToString().Trim();
                                    return Json(new Tuple<string, string>(fare.ToString(), eligAmt.ToString()));

                                
                           
                            

                        }
                        else
                        {

                        }
                    }


                } }
            return Json(new Tuple<string, string>("Success", "0"));
        }
        [HttpGet]
      
        public async Task<IActionResult> EmpTaxInv_DecReport(string SYKIID)
        {
            EmpTaxInvDecReportModel data = new EmpTaxInvDecReportModel();



            #region "Deduction U/S 80C to 80U"
            data = FillKI(SYKIID, data);
            string str_KIID;
            //if (SYKIID != null || SYKIID != "")
            if (!String.IsNullOrEmpty(SYKIID))
                str_KIID = SYKIID;
            else
                str_KIID = _objIPMS.GetKIId();
            Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
            //ltlname = "";//emp.Employee_Name;
            //DataTable obj_Add = objtax.GetEmpUsrPrdDtl(emp.Employee_Code.ToString());
            // string str_Address = obj_Add.Rows[0]["EMP_ADDRESS"].ToString();

            string format = "dd/MM/yyyy";
            string str_DOJ = string.Empty;

            data.ltl_Ecode = emp.Employee_Code.ToString();
            data.ltl_Name = emp.Employee_Name.ToString();
            data.ltl_Designation = emp.Designation.ToString();
            str_DOJ = emp.DOJ.ToString();
            DateTime time = Convert.ToDateTime(str_DOJ);
            data.ltl_DOJ = time.ToString(format);
            if (!string.IsNullOrEmpty(emp.Department_Desc))
            {
                data.ltl_Op_Dep = emp.Operation_Desc.ToString() + " / " + emp.Department_Desc.ToString();
            }
            else
            {
                data.ltl_Op_Dep = emp.Operation_Desc.ToString();
            }
            //txt_ContactNo = emp.MobileNo.ToString();
            data.ltl_PAN = emp.PancardNo.ToString();
            //txt_Address = str_Address;
            int int_TaxLIC = 0;
            int int_TaxULIP = 0;

            string strki = str_KIID;//objPms.GetKIId();
            int int_AllTotal = 0;
            DataTable dtamount = objtax.GetEmpTax_Invdetail("", _sessionService.Get<string>("userID"), "1", strki, "586");
            foreach (DataRow dr in dtamount.Rows)
            {
                if (dr["TAXHEAD"].ToString().Equals("Life Insurance Premium", StringComparison.OrdinalIgnoreCase))
                {
                    int_TaxLIC = Convert.ToInt32(dr["ACT_AMOUNT"]);
                }
            }
            foreach (DataRow dr in dtamount.Rows)
            {
                if (dr["TAXHEAD"].ToString().Equals("Unit Linked Insurance Policy (ULIP)", StringComparison.OrdinalIgnoreCase))
                {
                    int_TaxULIP = Convert.ToInt32(dr["ACT_AMOUNT"]);
                }
            }
            DataTable dttaxhead = objtax.GetTaxheaddetail("", "", "1", "586,585", strki);
            DataTable dttaxrecLIC = objtax.Get585_586RecDtl_V1(emp.Employee_Code, strki, "1", "586,585", "");
            var dttaxrecdtl = objtax.Get585_586RecDtl(emp.Employee_Code, strki, "1", "586,585", "");
            data.ltl586detail = "<table width='99%' style=\"border: solid 1px #6B6B6B; margin-top:5px;\">";
            for (Int16 i = 0; i < dttaxhead.Rows.Count; i += 3)
            {
                DataRow dr;

                #region "TAX_HEAD"
                data.ltl586detail = data.ltl586detail + "<tr style=\"background-color:silver;font-size:12px; height: 20px;\">";
                data.ltl586detail += "<td width='33%' style=\"border: solid 1px #6B6B6B;\">";
                if (i < dttaxhead.Rows.Count)
                {
                    dr = dttaxhead.Rows[i];
                    if (!dr["TAXHEAD"].ToString().Equals("Life Insurance Premium", StringComparison.OrdinalIgnoreCase))
                    {
                        data.ltl586detail += dr["TAXHEAD"].ToString();
                    }
                    else
                    {
                        data.ltl586detail += "";
                    }
                }
                data.ltl586detail += "</td><td width='33%' style=\"border: solid 1px #6B6B6B;\">";
                if ((i + 1) < dttaxhead.Rows.Count)
                {
                    dr = dttaxhead.Rows[i + 1];
                    if (!dr["TAXHEAD"].ToString().Equals("Life Insurance Premium", StringComparison.OrdinalIgnoreCase))
                    {
                        data.ltl586detail += dr["TAXHEAD"].ToString();
                    }
                    else
                    {
                        data.ltl586detail += "";
                    }
                }
                data.ltl586detail += "</td><td width='33%' style=\"border: solid 1px #6B6B6B;\">";
                if (i + 2 < dttaxhead.Rows.Count)
                {
                    dr = dttaxhead.Rows[i + 2];
                    if (!dr["TAXHEAD"].ToString().Equals("Unit Linked Insurance Policy (ULIP)", StringComparison.OrdinalIgnoreCase))
                    {
                        data.ltl586detail += dr["TAXHEAD"].ToString();
                    }
                    else
                    {
                        data.ltl586detail += "";
                    }
                }
                data.ltl586detail += "</td></tr>";
                #endregion
                data.ltl586detail = data.ltl586detail + "<tr style=\"background-color:white;font-size:12px;\">";
                #region  FirstColumn
                // First Tax Head Table Column
                data.ltl586detail += "<td width='33%' valign='top' style=\"border: solid 1px #6B6B6B;\">";
                if (i < dttaxhead.Rows.Count)
                {
                    int int_TaxHeadTotal1 = 0;

                    dr = dttaxhead.Rows[i];
                    DataView dv = dttaxrecdtl.DefaultView;
                    dv.RowFilter = "TAXHEADID=" + dr["TAXHEADID"].ToString();
                    data.ltl586detail += "<table cellspacing='0' cellpadding='1' width='100%'>";
                    data.ltl586detail += "<tr style=\"background-color:#F0F0F0;font-size:12px; height: 18px;\">";
                    data.ltl586detail += "<td width='33%' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Receipt No.</td>";
                    data.ltl586detail += "<td width='33%' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Date</td>";
                    data.ltl586detail += "<td width='33%' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Amount</td>";
                    data.ltl586detail += "</tr>";
                    if (dv.ToTable().Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in dv.ToTable().Rows)
                        {
                            data.ltl586detail += "<tr style=\"height:18px;\">";
                            data.ltl586detail += "<td style=\"border-bottom: solid 1px #6B6B6B;\">" + dr1["RECEIPT_NO"].ToString() + "</td>";
                            data.ltl586detail += "<td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">" + dr1["RECEIPT_DATE"].ToString() + "</td>";
                            data.ltl586detail += "<td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">" + dr1["RECEIPT_AMOUNT"].ToString() + "</td>";
                            data.ltl586detail += "</tr>";

                            int_TaxHeadTotal1 = int_TaxHeadTotal1 + Convert.ToInt32(dr1["RECEIPT_AMOUNT"].ToString());
                            int_AllTotal = int_AllTotal + Convert.ToInt32(dr1["RECEIPT_AMOUNT"].ToString());
                        }
                    }
                    else
                        data.ltl586detail += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>";
                    data.ltl586detail += "<tr><td style=\"border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" colspan='2' align='right'>Total&nbsp;</td>";
                    data.ltl586detail += "<td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" align='left'>&nbsp;" + int_TaxHeadTotal1.ToString() + "</td></tr>";
                    data.ltl586detail += "</table>";
                }
                data.ltl586detail += "</td>";
                #endregion
                #region  SecondColumn
                // Second Tax Head Table Column
                data.ltl586detail += "<td width='33%' valign='top' style=\"border: solid 1px #6B6B6B;\">";
                if ((i + 1) < dttaxhead.Rows.Count)
                {
                    int int_TaxHeadTotal2 = 0;

                    dr = dttaxhead.Rows[i + 1];
                    DataView dv = dttaxrecdtl.DefaultView;
                    dv.RowFilter = "TAXHEADID=" + dr["TAXHEADID"].ToString();
                    if (!dr["TAXHEAD"].ToString().Equals("Life Insurance Premium", StringComparison.OrdinalIgnoreCase))
                    {


                        data.ltl586detail += "<table cellspacing='0' cellpadding='1' width='100%'>";
                        data.ltl586detail += "<tr style=\"background-color:#F0F0F0;font-size:12px; height: 18px;\">";
                        data.ltl586detail += "<td width='33%' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Receipt No.</td>";
                        data.ltl586detail += "<td width='33%' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Date</td>";
                        data.ltl586detail += "<td width='33%' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Amount</td>";
                        data.ltl586detail += "</tr>";
                        if (dv.ToTable().Rows.Count > 0)
                        {
                            //data.ltl586detail += "<tr style=\"background-color:white;font-size:10px;\"><td width='33%'>&nbsp;Receipt No.</td><td width='33%'>&nbsp;Date</td><td width='33%'>&nbsp;Amount</td></tr>";
                            foreach (DataRow dr1 in dv.ToTable().Rows)
                            {
                                data.ltl586detail += "<tr style=\"height:18px;\">";
                                data.ltl586detail += "<td style=\"border-bottom: solid 1px #6B6B6B;\">" + dr1["RECEIPT_NO"].ToString() + "</td>";
                                data.ltl586detail += "<td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">" + dr1["RECEIPT_DATE"].ToString() + "</td>";
                                data.ltl586detail += "<td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">" + dr1["RECEIPT_AMOUNT"].ToString() + "</td>";
                                data.ltl586detail += "</tr>";

                                int_TaxHeadTotal2 = int_TaxHeadTotal2 + Convert.ToInt32(dr1["RECEIPT_AMOUNT"].ToString());
                                int_AllTotal = int_AllTotal + Convert.ToInt32(dr1["RECEIPT_AMOUNT"].ToString());
                            }
                        }
                        else
                            data.ltl586detail += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>";
                        data.ltl586detail += "<tr><td style=\"border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" align='right' colspan='2'>Total&nbsp;</td>";
                        data.ltl586detail += "<td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" align='left'>&nbsp;" + int_TaxHeadTotal2 + "</td></tr>";
                        data.ltl586detail += "</table>";

                    }
                    else
                    {
                        data.ltl586detail += "<table cellspacing='0' cellpadding='1' width='100%'>";
                        data.ltl586detail += "<tr style=\"background-color:;font-size:12px; height: 18px;\">";
                        data.ltl586detail += "<td width='33%'>&nbsp;</td></tr></table>";
                    }
                }
                data.ltl586detail += "</td>";
                #endregion
                #region Third
                // Third Head Table Column
                data.ltl586detail += "<td width='33%' valign='top' style=\"border: solid 1px #6B6B6B;\">";
                if (i + 2 < dttaxhead.Rows.Count)
                {
                    int int_TaxHeadTotal3 = 0;

                    dr = dttaxhead.Rows[i + 2];
                    DataView dv = dttaxrecdtl.DefaultView;
                    dv.RowFilter = "TAXHEADID=" + dr["TAXHEADID"].ToString();
                    if (!dr["TAXHEAD"].ToString().Equals("Unit Linked Insurance Policy (ULIP)", StringComparison.OrdinalIgnoreCase))
                    {
                        data.ltl586detail += "<table cellspacing='0' cellpadding='1' width='100%'>";
                        data.ltl586detail += "<tr style=\"background-color:#F0F0F0;font-size:12px; height: 18px;\">";
                        data.ltl586detail += "<td width='33%' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Receipt No.</td>";
                        data.ltl586detail += "<td width='33%' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Date</td>";
                        data.ltl586detail += "<td width='33%' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Amount</td>";
                        data.ltl586detail += "</tr>";
                        if (dv.ToTable().Rows.Count > 0)
                        {
                            //data.ltl586detail += "<tr style=\"background-color:white;font-size:10px;\"><td width='33%'>&nbsp;Receipt No.</td><td width='33%'>&nbsp;Date</td><td width='33%'>&nbsp;Amount</td></tr>";
                            foreach (DataRow dr1 in dv.ToTable().Rows)
                            {
                                data.ltl586detail += "<tr style=\"height:18px;\">";
                                data.ltl586detail += "<td style=\"border-bottom: solid 1px #6B6B6B;\">" + dr1["RECEIPT_NO"].ToString() + "</td>";
                                data.ltl586detail += "<td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">" + dr1["RECEIPT_DATE"].ToString() + "</td>";
                                data.ltl586detail += "<td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">" + dr1["RECEIPT_AMOUNT"].ToString() + "</td>";
                                data.ltl586detail += "</tr>";

                                int_TaxHeadTotal3 = int_TaxHeadTotal3 + Convert.ToInt32(dr1["RECEIPT_AMOUNT"].ToString());
                                int_AllTotal = int_AllTotal + Convert.ToInt32(dr1["RECEIPT_AMOUNT"].ToString());
                            }
                        }
                        else

                            data.ltl586detail += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>";
                        data.ltl586detail += "<tr><td style=\"border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" colspan='2' align='right'>Total&nbsp;</td>";
                        data.ltl586detail += "<td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" align='left'>&nbsp;" + int_TaxHeadTotal3 + "</td></tr>";
                        data.ltl586detail += "</table>";
                    }
                    else
                    {
                        data.ltl586detail += "<table cellspacing='0' cellpadding='1' width='100%'>";
                        data.ltl586detail += "<tr style=\"background-color:;font-size:12px; height: 18px;\">";
                        data.ltl586detail += "<td width='33%'>&nbsp;</td></tr></table>";
                    }
                }
                data.ltl586detail += "</td></tr>";

                #endregion
            }
            // ----------------------LIC

            data.ltr586LICdetails += "<tr style=\"background-color:silver; height: 20px;\"><td colspan='3' style=\"border: solid 1px #6B6B6B;\">Life Insurance Premium</td></tr>";
            data.ltr586LICdetails += "<tr style=\"background-color:white;font-size:12px;\"><td colspan='3'>";
            data.ltr586LICdetails += "<table cellspacing='0' cellpadding='1' width='100%'>";
            data.ltr586LICdetails += "<tr style=\"background-color:#F0F0F0;font-size:12px; height: 18px;\">";
            data.ltr586LICdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Policy No.</td>";
            data.ltr586LICdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Assured Amount</td>";
            data.ltr586LICdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Policy Date</td>";
            data.ltr586LICdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Receipt No.</td>";
            data.ltr586LICdetails += "<td width='' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Date</td>";
            data.ltr586LICdetails += "<td width='' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Amount</td>";
            data.ltr586LICdetails += "</tr>";
            foreach (DataRow dr in dttaxrecLIC.Rows)
            {
                if (dr["TAXHEAD"].ToString().Equals("Life Insurance Premium", StringComparison.OrdinalIgnoreCase))
                {
                    data.ltr586LICdetails += "<tr style=\"background-color:White;font-size:12px; height: 18px;\">";
                    data.ltr586LICdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["POLICYNO"].ToString() + "</td>";
                    data.ltr586LICdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["TOTAL_SUM_ASSURED"].ToString() + "</td>";
                    data.ltr586LICdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["POLICY_DATE"].ToString() + "</td>";
                    data.ltr586LICdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["RECEIPT_NO"].ToString() + "</td>";
                    data.ltr586LICdetails += "<td width='' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["RECEIPT_DATE"].ToString() + "</td>";
                    data.ltr586LICdetails += "<td width='' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["RECEIPT_AMOUNT"].ToString() + "</td>";
                    data.ltr586LICdetails += "</td></tr>";
                }

            }
            data.ltr586LICdetails += "<tr><td style=\"border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" colspan='5' align='right'> Total Eligible for Tax Benefit &nbsp;</td><td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" align='center'>&nbsp;" + int_TaxLIC + "</td></tr>";
            data.ltr586LICdetails += "</table></tr>";
            ////----------------------

            //data.ltr586LICdetails += "<tr style=\"background-color:silver; height: 20px;\"><td colspan='2' align='right' style=\"border: solid 1px #6B6B6B;\">Grand Total&nbsp;</td><td align='left' style=\"border: solid 1px #6B6B6B;\">&nbsp;" + (int_AllTotal + int_TaxLIC) + "</td></tr>";
            //data.ltr586LICdetails += "</table>";

            // ----------------------ULIP

            data.ltr586ULIPdetails += "<tr style=\"background-color:silver; height: 20px;\"><td colspan='3' style=\"border: solid 1px #6B6B6B;\">Unit Linked Insurance Policy (ULIP)</td></tr>";
            data.ltr586ULIPdetails += "<tr style=\"background-color:white;font-size:12px;\"><td colspan='3'>";
            data.ltr586ULIPdetails += "<table cellspacing='0' cellpadding='1' width='100%'>";
            data.ltr586ULIPdetails += "<tr style=\"background-color:#F0F0F0;font-size:12px; height: 18px;\">";
            data.ltr586ULIPdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Policy No.</td>";
            data.ltr586ULIPdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Assured Amount</td>";
            data.ltr586ULIPdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Policy Date</td>";
            data.ltr586ULIPdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;Receipt No.</td>";
            data.ltr586ULIPdetails += "<td width='' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Date</td>";
            data.ltr586ULIPdetails += "<td width='' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;Amount</td>";
            data.ltr586ULIPdetails += "</tr>";
            foreach (DataRow dr in dttaxrecLIC.Rows)
            {
                if (dr["TAXHEAD"].ToString().Equals("Unit Linked Insurance Policy (ULIP)", StringComparison.OrdinalIgnoreCase))
                {
                    data.ltr586ULIPdetails += "<tr style=\"background-color:White;font-size:12px; height: 18px;\">";
                    data.ltr586ULIPdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["POLICYNO"].ToString() + "</td>";
                    data.ltr586ULIPdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["TOTAL_SUM_ASSURED"].ToString() + "</td>";
                    data.ltr586ULIPdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["POLICY_DATE"].ToString() + "</td>";
                    data.ltr586ULIPdetails += "<td width='' style=\"border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["RECEIPT_NO"].ToString() + "</td>";
                    data.ltr586ULIPdetails += "<td width='' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["RECEIPT_DATE"].ToString() + "</td>";
                    data.ltr586ULIPdetails += "<td width='' style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;\">&nbsp;" + dr["RECEIPT_AMOUNT"].ToString() + "</td>";
                    data.ltr586ULIPdetails += "</td></tr>";
                }

            }
            data.ltr586ULIPdetails += "<tr><td style=\"border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" colspan='5' align='right'> Total Eligible for Tax Benefit &nbsp;</td><td style=\"border-left: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B; border-top: solid 1px #6B6B6B;\" align='center'>&nbsp;" + int_TaxULIP + "</td></tr>";
            data.ltr586ULIPdetails += "</table></tr>";
            ////----------------------

            data.ltr586ULIPdetails += "<tr style=\"background-color:silver; height: 20px;\"><td colspan='2' align='right' style=\"border: solid 1px #6B6B6B;\">Grand Total&nbsp;</td><td align='left' style=\"border: solid 1px #6B6B6B;\">&nbsp;" + (int_AllTotal + int_TaxLIC + int_TaxULIP) + "</td></tr>";
            data.ltr586ULIPdetails += "</table>";
            #endregion

            #region "House Rent"
            DataTable dt_House = objtax.GetEmpRent_Invdetail(emp.Employee_Code.ToString(), str_KIID);
            if (dt_House.Rows.Count > 0)
            {
                //ltl_HouseRent = dt_House.Rows[0]["RENT_AMOUNT"].ToString();
                //if (dt_House.Rows[0]["CITY_CAT"].ToString() == "0")
                //    ltl_RentedLocation = "Non Metro";
                //else
                //    ltl_RentedLocation = "Metro";
                //ltl_RentPeriod = dt_House.Rows[0]["RENT_FROM"].ToString() + " - " + dt_House.Rows[0]["RENT_TO"].ToString();
                var json = JsonConvert.SerializeObject(dt_House);

                data.REPHOUSERENT = JsonConvert.DeserializeObject<List<RENTDetails>>(json);

                data.txt_Address = dt_House.Rows[0]["SAPaddress"].ToString();
                data.ltlsite = dt_House.Rows[0]["SITE"].ToString();
                data.txt_ContactNo = dt_House.Rows[0]["TMOBILE"].ToString();
                data.strlname = dt_House.Rows[0]["LANDLORDNAME"].ToString();
            }
            #endregion

            #region "LTA"
            ReceiptDtl_LTAMain objLTA = new ReceiptDtl_LTAMain();
            objLTA.objLTAdtlcolls = new List<ReceiptDtl_LTACollection>();
            int int_LTATotal = 0;
            double taxi_amount_sum = 0;
            double taxi_trainfare_sum = 0;
            DataSet ds = objtax.GetLTADtl(emp.Employee_Code.ToString(), str_KIID, "1");
            DataView dtv = ds.Tables[1].DefaultView;
            dtv.RowFilter = "ISTRAVEL=1";
            if (ds.Tables[0].Rows.Count > 0)
            {
                data.ltl_LTAAmount = ds.Tables[0].Rows[0]["LTATAKEN"].ToString();
                data.ltl_LTAPlace = ds.Tables[0].Rows[0]["PLACEOFVISIT"].ToString();
                data.ltl_LTAFromDate = ds.Tables[0].Rows[0]["JOURNEYFROM_DT"].ToString();
                data.ltl_LTAToDate = ds.Tables[0].Rows[0]["JOURNEYTO_DT"].ToString();

                foreach (DataRow dr in dtv.ToTable().Rows)
                {
                    ReceiptDtl_LTACollection objcoll = new ReceiptDtl_LTACollection();
                    objcoll.Age = dr["AGE"].ToString();
                    objcoll.Amount = dr["AMOUNT"].ToString();
                    objcoll.ID = dr["FIN_TAXLTAJOURDTLID"].ToString();
                    objcoll.MOT = dr["MOT"].ToString();

                    objcoll.Name = dr["NAMEOFPERSON"].ToString();
                    objcoll.Relation = dr["RELATION"].ToString();
                    //if (dr["RELATION"].ToString() == "1")
                    //    objcoll._Relation = "Self";
                    //else if (dr["RELATION"].ToString() == "2")
                    //    objcoll._Relation = "Spouse";
                    //else if (dr["RELATION"].ToString() == "3")
                    //    objcoll._Relation = "Child";
                    //else if (dr["RELATION"].ToString() == "4")
                    //    objcoll._Relation = "Parent";

                    objcoll.TktBillNo = dr["BILLNO"].ToString();
                    objcoll.TrainFare = dr["TRAINFARE"].ToString();
                    objcoll.ElilableAmt = dr["ISELIGIBLEAMT"].ToString();
                    if (objcoll.MOT == "1")
                    {
                        string nfare = ds.Tables[0].Rows[0]["NORMALTRAINFARE"].ToString().Trim();
                        if (!string.IsNullOrEmpty(nfare))
                        {
                            if (ds.Tables[0].Rows[0]["NORMALTRAINFARE"].ToString() != "0")
                            {

                                if (!string.IsNullOrEmpty(objcoll.TrainFare.ToString()))
                                {
                                    taxi_trainfare_sum = taxi_trainfare_sum + Convert.ToDouble(objcoll.TrainFare.ToString());
                                }
                                if (!string.IsNullOrEmpty(objcoll.Amount.ToString()))
                                {
                                    taxi_amount_sum = taxi_amount_sum + Convert.ToDouble(objcoll.Amount.ToString());
                                }

                            }

                        }
                    }
                    string strMOT = objcoll.MOT;
                    if (strMOT == "1")
                        objcoll.MOT = "Taxi";
                    if (strMOT == "2")
                        objcoll.MOT = "Air";
                    if (strMOT == "3")
                        objcoll.MOT = "Bus";
                    if (strMOT == "4")
                        objcoll.MOT = "Train";

                    objLTA.objLTAdtlcolls.Add(objcoll);
                    int_LTATotal = int_LTATotal + Convert.ToInt32(objcoll.ElilableAmt.ToString());
                }
                data.ltl_LTATotal = int_LTATotal.ToString();
                data.LTA = objLTA.objLTAdtlcolls;

                if (data.LTA.Count > 0)
                {
                    int sykiid = Convert.ToInt32(ds.Tables[0].Rows[0]["SYKIID"].ToString());
                    if (sykiid >= 19)
                    {
                        string nfare = ds.Tables[0].Rows[0]["NORMALTRAINFARE"].ToString().Trim();
                        if (!string.IsNullOrEmpty(nfare))
                        {
                            if (ds.Tables[0].Rows[0]["NORMALTRAINFARE"].ToString() != "0")
                            {
                                foreach (ReceiptDtl_LTACollection gr in data.LTA)
                                {
                                    string mot = gr.MOT.Trim();
                                    if (mot.ToUpper() == "TAXI")
                                    {
                                        string relation = gr.Relation.Trim();
                                        if (relation.ToUpper() != "SELF")
                                        {
                                            gr.ElilableAmt = "";
                                        }
                                        else
                                        {
                                            gr.ElilableAmt = taxi_trainfare_sum > taxi_amount_sum ? taxi_amount_sum.ToString() : taxi_trainfare_sum.ToString();
                                            data.ltl_LTATotal = taxi_trainfare_sum > taxi_amount_sum ? taxi_amount_sum.ToString() : taxi_trainfare_sum.ToString();
                                        }
                                    }


                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region LTC
            ///// ADDED BY VISHAL on 07-Jan-2021 /////
            DataSet dsLTC = objtax.GetLTCDtl(_sessionService.Get<string>("userID"), str_KIID, "1");
            if (dsLTC.Tables[0].Rows.Count > 0)
            {
                List<LTAModel> LtcList = new List<LTAModel>();
                decimal tot_amt = 0;
                foreach (DataRow dr in dsLTC.Tables[0].Rows)
                {
                    LTAModel objLTC = new LTAModel();
                    objLTC.SID = dr["FIN_TAXLTCDTLID"].ToString();
                    objLTC.SellerName = dr["SELLERNAME"].ToString();
                    objLTC.GSTNo = dr["GSTNUMBER"].ToString();
                    objLTC.InvoiceDate = dr["INVOICEDATE"].ToString();
                    objLTC.InvoiceNo = dr["INVOICENO"].ToString();
                    objLTC.Goods_Service = dr["PARTICULARSGOODS"].ToString();
                    objLTC.InvoiceAMT = Convert.ToDecimal(dr["GOODSAMOUNT"].ToString());
                    objLTC.GSTRate = Convert.ToInt32(dr["GSTRATE"].ToString());
                    objLTC.GSTAmount = Convert.ToDecimal(dr["GSTAMOUNT"].ToString());
                    objLTC.TotalAmount = Convert.ToDecimal(dr["TOTALINVOICEAMOUNT"].ToString());

                    LtcList.Add(objLTC);
                    tot_amt = tot_amt + Convert.ToDecimal(dr["TOTALINVOICEAMOUNT"].ToString());
                }
                data.LTA1 = LtcList;

                data.ltltotexm = Convert.ToInt32((tot_amt / 3)).ToString();
            }
            if (dsLTC.Tables[1].Rows.Count > 0)
            {
                List<LTAFamilyModel> LtcmLList = new List<LTAFamilyModel>();
                decimal tot_exmpamt = 0;
                foreach (DataRow dr in dsLTC.Tables[1].Rows)
                {
                    LTAFamilyModel objcoll = new LTAFamilyModel();
                    objcoll.FID = dr["FIN_TAXLTCMEMBERDTLID"].ToString();
                    objcoll.Age = dr["AGE"].ToString();
                    objcoll.Personname = dr["NAMEOFPERSON"].ToString();
                    objcoll.Relationship = dr["RELATION"].ToString();
                    objcoll.Max_Eligible = dr["ISELIGIBLEAMT"].ToString();
                    objcoll.ClaimExp = data.ltlfinyear;
                    if (dr["ISCLAIM"].ToString() == "1")
                    {
                        objcoll.ClaimExp = "Yes";
                    }
                    else if (dr["ISCLAIM"].ToString() == "0")
                    {
                        objcoll.ClaimExp = "No";
                    }
                    objcoll.ExemptionAmount = dr["EXEM_AMOUNT"].ToString();

                    LtcmLList.Add(objcoll);
                    tot_exmpamt = tot_exmpamt + Convert.ToDecimal(dr["EXEM_AMOUNT"].ToString());
                }

                data.LTA2 = LtcmLList;

                data.ltlexmamt = tot_exmpamt.ToString();
            }

            ///// END /////
            #endregion

            #region "Interest on Housing Loan"
            DataTable DT_LoanDtl = objtax.GetLOANDTL(_sessionService.Get<string>("userID"), str_KIID);
            if (DT_LoanDtl.Rows.Count > 0)
            {
                // Date of Completion of Construction
                data.ltl_DC = DT_LoanDtl.Rows[0]["DATEOFCOMPLETION"].ToString();

                //Date on which Loan was taken
                data.ltl_LoanDate = DT_LoanDtl.Rows[0]["DATELOANTKN"].ToString();

                // Relationship in case of Joint Loan
                if (DT_LoanDtl.Rows[0]["RELATION"].ToString() == "1")
                    data.ltl_RJL = "Spouse";
                else if (DT_LoanDtl.Rows[0]["RELATION"].ToString() == "2")
                    data.ltl_RJL = "Parent";
                else if (DT_LoanDtl.Rows[0]["RELATION"].ToString() == "3")
                    data.ltl_RJL = "Brother";
                else if (DT_LoanDtl.Rows[0]["RELATION"].ToString() == "4")
                    data.ltl_RJL = "Other";

                // Owner of the property
                if (DT_LoanDtl.Rows[0]["ISOWNER"].ToString() == "1")
                    data.ltl_PropertyOwner = "Self";
                else if (DT_LoanDtl.Rows[0]["ISOWNER"].ToString() == "2")
                    data.ltl_PropertyOwner = "Joint";
                else if (DT_LoanDtl.Rows[0]["ISOWNER"].ToString() == "3")
                    data.ltl_PropertyOwner = "Other";

                // Percentage of share in tax exemption
                data.ltl_Taxexemption = DT_LoanDtl.Rows[0]["TAXEXEMPSHAREPER"].ToString();

                // Percentage of share in property
                data.ltl_PSP = DT_LoanDtl.Rows[0]["SHAREPER"].ToString();

                // Is is a Joint Loan
                if (DT_LoanDtl.Rows[0]["ISJOINTLOAN"].ToString() == "1")
                    data.ltl_JL = "Yes";
                else if (DT_LoanDtl.Rows[0]["ISJOINTLOAN"].ToString() == "0")
                    data.ltl_JL = "No";

                // Is Property Under Construction
                if (DT_LoanDtl.Rows[0]["ISUC"].ToString() == "1")
                    data.ltl_UC = "Yes";
                else if (DT_LoanDtl.Rows[0]["ISUC"].ToString() == "0")
                    data.ltl_UC = "No";

                data.ltl_CurrentInterest = DT_LoanDtl.Rows[0]["CURRINTREST"].ToString();

                data.ltl_PreYear1 = DT_LoanDtl.Rows[0]["YEAR1INTEREST"].ToString();

                data.ltl_PreYear2 = DT_LoanDtl.Rows[0]["YEAR2INTEREST"].ToString();

                data.ltl_PreYear3 = DT_LoanDtl.Rows[0]["YEAR3INTEREST"].ToString();
                data.ltl_PreYear4 = DT_LoanDtl.Rows[0]["YEAR4INTEREST"].ToString();
                data.ltl_PreYear5 = DT_LoanDtl.Rows[0]["YEAR5INTEREST"].ToString();


                //newly added
                data.ltrloanAmount = DT_LoanDtl.Rows[0]["LOAN_AMOUNT"].ToString();
                data.ltrloanProvider = DT_LoanDtl.Rows[0]["LOAD_PROVIDER"].ToString();

                //string FIN_TAXLOANDTLID = DT_LoanDtl.Rows[0]["FIN_TAXLOANDTLID"].ToString();
                //DataTable DTHouse = objtax.GetHOUSELOANDTL(FIN_TAXLOANDTLID);
                //if (DTHouse.Rows.Count > 0)
                //{
                //    reptHouseLoan.DataSource = DTHouse;
                //    reptHouseLoan.DataBind();
                //}

                data.ltrannualRent = DT_LoanDtl.Rows[0]["ANNUAL_RENT"].ToString();
                data.ltrmunicipalTax = DT_LoanDtl.Rows[0]["MUNICIPAL_TAX"].ToString();

                DataTable dt = new DataTable();
                dt = objtax.GetFINANCIALYEAR();
                data.ltl_CurrentYear = dt.Rows[0]["FINANCIALYEAR"].ToString();
                data.ltl_year1 = dt.Rows[1]["FINANCIALYEAR"].ToString();
                data.ltl_year2 = dt.Rows[2]["FINANCIALYEAR"].ToString();
                data.ltl_year3 = dt.Rows[3]["FINANCIALYEAR"].ToString();
                data.ltl_year4 = dt.Rows[4]["FINANCIALYEAR"].ToString();
                data.ltl_year5 = dt.Rows[5]["FINANCIALYEAR"].ToString();
                Int64 currintr, TotalYear1, TotalYear2, TotalYear3, TotalYear4, TotalYear5;
                currintr = data.ltl_CurrentInterest != "" ? (Convert.ToInt64(data.ltl_CurrentInterest)) : 0;
                TotalYear1 = data.ltl_PreYear1 != "" ? (Convert.ToInt64(data.ltl_PreYear1) / 5) : 0;
                TotalYear2 = data.ltl_PreYear2 != "" ? (Convert.ToInt64(data.ltl_PreYear2) / 5) : 0;
                TotalYear3 = data.ltl_PreYear3 != "" ? (Convert.ToInt64(data.ltl_PreYear3) / 5) : 0;
                TotalYear4 = data.ltl_PreYear4 != "" ? (Convert.ToInt64(data.ltl_PreYear4) / 5) : 0;
                TotalYear5 = data.ltl_PreYear5 != "" ? (Convert.ToInt64(data.ltl_PreYear5) / 5) : 0;
                data.ltl_TotalCurrentInterest = data.ltl_CurrentInterest;
                data.ltl_TotalYear1 = TotalYear1.ToString();
                data.ltl_TotalYear2 = TotalYear2.ToString();
                data.ltl_TotalYear3 = TotalYear3.ToString();
                data.ltl_TotalYear4 = TotalYear4.ToString();
                data.ltl_TotalYear5 = TotalYear5.ToString();
                data.ltl_totalHouseLoan = (currintr + TotalYear1 + TotalYear2 + TotalYear3 + TotalYear4 + TotalYear5).ToString();
                data.ltl_SelfOccupFromDate = DT_LoanDtl.Rows[0]["SELFOCCUPFROM"].ToString();
                data.ltl_SelfOccupToDate = DT_LoanDtl.Rows[0]["SELFOCCUPTO"].ToString();
                data.ltl_HouseAddressLoan = DT_LoanDtl.Rows[0]["ADDRESSOFHLT"].ToString();
                if (DT_LoanDtl.Rows[0]["ISJOINTLOAN"].ToString() == "1" && DT_LoanDtl.Rows[0]["TAXEXEMPSHAREPER"].ToString() != "")
                {
                    data.ltltotded24B = Convert.ToInt64((Convert.ToInt64(data.ltl_totalHouseLoan) * Convert.ToInt64(data.ltl_Taxexemption)) / 100).ToString();
                }
                else
                {
                    data.ltltotded24B = Convert.ToInt64(Convert.ToInt64(data.ltl_totalHouseLoan == "" ? "0" : Convert.ToInt64(data.ltl_totalHouseLoan))).ToString();
                }
                if (Convert.ToInt32(data.ltltotded24B) > 200000)
                    data.ltltotded24B = "200000";
            }
            #endregion

            #region "Interest on Housing Loan U/S 80EE "
            DataTable DT_80EEDtl = objtax.Get80EEDTL(_sessionService.Get<string>("userID"), str_KIID);
            if (DT_80EEDtl.Rows.Count > 0)
            {
                data.ltl_loansanctiondate = DT_80EEDtl.Rows[0]["LSDATTE"].ToString();
                data.ltl_loansanctioned = DT_80EEDtl.Rows[0]["LSAMOUNT"].ToString();
                data.ltl_vofhouse = DT_80EEDtl.Rows[0]["TVRHP"].ToString();
                if (DT_80EEDtl.Rows[0]["ISEMPOOTHHOUSE"].ToString() == "1")
                    data.ltl_ownerofoterhhouse = "Yes";
                else if (DT_80EEDtl.Rows[0]["ISEMPOOTHHOUSE"].ToString() == "0")
                    data.ltl_ownerofoterhhouse = "No";

                if (DT_80EEDtl.Rows[0]["OWNEROFPROPERTY"].ToString() == "1")
                    data.ltl_ownerofproperty = "Self";
                else if (DT_80EEDtl.Rows[0]["OWNEROFPROPERTY"].ToString() == "0")
                    data.ltl_ownerofproperty = "Joint";

                if (DT_80EEDtl.Rows[0]["ISPUC"].ToString() == "1")
                    data.ltl_popunderconstr = "Yes";
                else if (DT_80EEDtl.Rows[0]["ISPUC"].ToString() == "0")
                    data.ltl_popunderconstr = "No";

                data.ltl_compofconstdate = DT_80EEDtl.Rows[0]["DATEOFCOMP"].ToString();

                if (DT_80EEDtl.Rows[0]["IAC24B"].ToString() == "1")
                    data.ltl_amtclaimed24b = "Yes";
                else if (DT_80EEDtl.Rows[0]["IAC24B"].ToString() == "0")
                    data.ltl_amtclaimed24b = "No";

                data.ltl_perofshareproperty = DT_80EEDtl.Rows[0]["PSP"].ToString();
                data.ltl_perofshareintax = DT_80EEDtl.Rows[0]["PSTE"].ToString();
                data.ltl_Declamount = DT_80EEDtl.Rows[0]["DECLAREAMOUNT"].ToString();
                DataView dv = new DataView(dttaxrecdtl);
                dv.RowFilter = "TAXHEADID=282";
                if (dv.ToTable().Rows.Count == 1)
                {
                    foreach (DataRow drhome in dv.ToTable().Rows)
                    {
                        data.ltr_Dedunction80EE = drhome["RECEIPT_AMOUNT"].ToString();
                    }
                }
            }
            #endregion

            #region "Expenses incurred in performance of official duties"
            data.ltl_AttireAllowAmount = "30000";
            data.ltl_StandardDeduction = "50000";
            #endregion

            #region "Income from Other Sources"
            DataTable dt1 = objtax.GetEmp584_Invdetail("", _sessionService.Get<string>("userID"), "1", str_KIID, "584");
            DataView dv1 = dt1.DefaultView;
            dv1.RowFilter = "SAPSUBTYPE=2";
            data.ltl584sub2 = "<table width='99%' style='margin-top: 3px; border: solid 1px #6B6B6B; font-family: Calibri;font-size: 13px; color: Black;'>" +
                            "<tr style='background-color: #F0F0F0; font-weight: bold;'>" +
                            "<td style='border-right: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;'>Nature of Income</td>" +
                            "<td style='border-right: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;'>Date</td>" +
                            "<td style='border-bottom: solid 1px #6B6B6B;'>Amount</td></tr>";
            Int64 total = 0;
            foreach (DataRow dr in dv1.ToTable().Rows)
            {
                data.ltl584sub2 += "<tr>" +
                            "<td style='border-right: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;'>" + dr["TAXHEAD"].ToString() + "</td>" +
                            "<td style='border-right: solid 1px #6B6B6B; border-bottom: solid 1px #6B6B6B;'>&nbsp;</td>" +
                            "<td style='border-bottom: solid 1px #6B6B6B;'>" + dr["PROJ_AMOUNT"].ToString() + "</td></tr>";
                total += dr["PROJ_AMOUNT"].ToString() != "" ? Convert.ToInt64(dr["PROJ_AMOUNT"].ToString()) : 0;
            }
            data.ltl584sub2 += "<tr><td colspan='2' align='right' style='border-right: solid 1px #6B6B6B;'>Total</td>" +
                                   "<td>" + total.ToString() + "</td></tr></table>";
            #endregion
            if (data.strlname != "")
            {
                data.reportType = "EmpRent_NewFormPrint?SYKIID=" + SYKIID;
                //ScriptManager.RegisterClientScriptBlock(Page, GetType(), "alt", "javascript:openPopupwithScrol('EmpRent_NewFormPrint.aspx','700','350');", true);

            }
            if (time.Date > DateTime.ParseExact("01-APR-" + data.ltlfinyear.Substring(0, 4), "dd-MMM-yyyy", null))
            {
                data.B12Report = "EmpTaxInv_12BReport?SYKIID=" + SYKIID;
                //    //ScriptManager.RegisterClientScriptBlock(Page, GetType(), "alt1", "javascript:openPopupwithScrol1('EmpTaxInv_12BReport.aspx','900','550');", true);
            }

            data.BB12Report= "EmpTaxInv_12BBReport?SYKIID=" + SYKIID;
            //ScriptManager.RegisterClientScriptBlock(this.Page, typeof(string), "print", "window.print();", true);
            return View("EmpTaxInv_DecReport", data);

        }
        [HttpGet]
        public async Task<IActionResult> EmpTaxInv_12BReport(string SYKIID)
        {
            EmpTaxInvDecReportModel data = new EmpTaxInvDecReportModel();
            string strki = _objIPMS.GetKIId();

            data = FillKI(SYKIID, data);
            //Employee_Details emp = (Employee_Details)Session["Employee"];
            DataTable dt = objtax.Get12BDtl(_sessionService.Get<string>("userID"), strki, "1");
            if (dt.Rows.Count > 0)
            {
                data.ltlname = dt.Rows[0]["EMPNAME"].ToString();//emp.Employee_Name;
                data.ltlpan = dt.Rows[0]["PANCARDNO"].ToString(); ;//emp.PancardNo;

                data.grd122Bview = "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\">";

                foreach (DataRow dr in dt.Rows)
                {
                    data.grd122Bview += "<tr style='border: solid 1px #050304;'>";
                    data.grd122Bview += " <td style='border-right: solid 1px #050304;' width='10%'>";
                    data.grd122Bview += dr["EMPLOYER_NAME"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px #050304;' width='10%'>";
                    data.grd122Bview += dr["EMPLOYER_ADD"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px #050304;' width=7%'>";
                    data.grd122Bview += dr["TAN"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px #050304;' width='7%'>";
                    data.grd122Bview += dr["PAN"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px #050304;' width='8%'>";
                    data.grd122Bview += "From : " + dr["PERIOD_FROM"].ToString() + "To :" + dr["PERIOD_TO"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px #050304;' width='10%'>";
                    data.grd122Bview += dr["TOTAL_GROSS"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px #050304;' width='10%'>";
                    data.grd122Bview += dr["TOTAL_PERK"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px #050304;' width='13%'>";
                    data.grd122Bview += dr["TOTAL_EXEMP"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px #050304;' width='7%'>";
                    data.grd122Bview += dr["TOTAL_DED"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px #050304;' width='8%'>";
                    data.grd122Bview += dr["TAXABLEINCOME"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "<td style='border-right: solid 1px white;' width='10%'>";
                    data.grd122Bview += dr["TOTALTAXAMT"].ToString();
                    data.grd122Bview += "</td>";
                    data.grd122Bview += "</tr>";


                    data.ltladdress = dr["EMPADDRESS"].ToString();
                    data.ltlresstatus = dr["RES_STATUS"].ToString();

                }
            }
            return View("EmpTaxInv_12BReport", data);


        }

        [HttpGet]
        public async Task<IActionResult> EmpRent_NewFormPrint(string KIID)
        {
            List<EmpRent_NewFormPrintModel> data = new List<EmpRent_NewFormPrintModel>();
            string strKI = _objIPMS.GetKIId();
            DataTable dt = objtax.GetEmpRent_Invdetail_N(_sessionService.Get<string>("userID"), strKI);
            var json = JsonConvert.SerializeObject(dt);
            data = JsonConvert.DeserializeObject<List<EmpRent_NewFormPrintModel>>(json);
            return View("EmpRent_NewFormPrint", data);

        }

        [HttpPost]
        public async Task<IActionResult> SubmitEE([FromBody] EEDetailsModels objM)
        {  // string strdate = "01" + "-" + "Apr" + "-" + (Convert.ToInt32(DateTime.Now.Year) - 1).ToString();
            var EEBVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var EEBSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(EEBVal);
            string strstartdate = "01-Apr-2016";
            string strenddate = "31-Mar-2017";
            DateTime lsstartdate = DateTime.ParseExact(strstartdate, "dd-MMM-yyyy", null);
            DateTime lsenddate = DateTime.ParseExact(strenddate, "dd-MMM-yyyy", null);
            if (objM.Loansanctiondate == "")
            {
                return Json(new Tuple<string, string>("error", "Please fill Date of Loan Sanctioned"));


            }
            if (objM.Loansanctionamount == "")
            {
                return Json(new Tuple<string, string>("error", "Please fill Amount of Loan Sanctioned"));


            }
            if (objM.Vofhouse == "")
            {
                return Json(new Tuple<string, string>("error", "Please fill Total Value of Residential House Purchased"));


            }
            if (objM.Ownerofoterhhouse == "")
            {
                return Json(new Tuple<string, string>("error", "Please select Is employee does have owner of any other house"));


            }
            if (objM.Ownerofproperty == "")
            {
                return Json(new Tuple<string, string>("error", "Please select Is Owner of the property"));


            }
            if (objM.Popunderconstr == "")
            {
                return Json(new Tuple<string, string>("error", "Please select Is Property Under Construction"));


            }
            if (objM.Popunderconstr == "0")
            {
                if (objM.Compofconstdate == "")
                {
                    return Json(new Tuple<string, string>("error", "Please fill Date of Completion of Construction"));


                }
            }
            if (objM.Amtclaimed24b == "")
            {
                return Json(new Tuple<string, string>("error", "Please select Whether Same interset amount claimed U/S 24B"));

            }
            if (objM.Perofshareproperty == "")
            {
                return Json(new Tuple<string, string>("error", "Please fill Percentage of Share in Property "));

            }
            if (objM.Perofshareintax == "")
            {
                return Json(new Tuple<string, string>("error", "Please fill Percentage of Share in Tax exemption "));

            }
            if (objM.Declamount == "")
            {
                return Json(new Tuple<string, string>("error", "Please fill Amount of Interest Paid"));


            }
            if (DateTime.ParseExact(objM.Loansanctiondate, "dd-MMM-yyyy", null) < lsstartdate || DateTime.ParseExact(objM.Loansanctiondate, "dd-MMM-yyyy", null) > lsenddate)
            {
                return Json(new Tuple<string, string>("error", "You are not eligble to get benfit U/S 80EE"));

            }
            if (DateTime.ParseExact(objM.Loansanctiondate, "dd-MMM-yyyy", null) > DateTime.ParseExact(DateTime.Now.Date.ToString("dd-MMM-yyyy"), "dd-MMM-yyyy", null))
            {
                return Json(new Tuple<string, string>("error", "Date of Loan Sanctioned is not greater than current date"));

            }
            if (Convert.ToInt64(objM.Loansanctionamount) > 3500000)
            {
                return Json(new Tuple<string, string>("error", "You are not eligble to get benfit U/S 80EE"));

            }
            if (Convert.ToInt64(objM.Vofhouse) > 5000000)
            {
                return Json(new Tuple<string, string>("error", "You are not eligble to get benfit U/S 80EE"));

            }
            if (objM.Ownerofoterhhouse == "1")
            {
                return Json(new Tuple<string, string>("error", "You are not eligble to get benfit U/S 80EE"));

            }
            if (objM.Amtclaimed24b == "1")
            {
                return Json(new Tuple<string, string>("error", "You are not eligble to get benfit U/S 80EE"));

            }

            EEBSession.EE = objM;
            UpdateProjAmount(EEBSession, objM.TaxId, objM.EEamount);
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(EEBSession));
           
            return Json(objM.EEamount);
            //ScriptManager.RegisterClientScriptBlock(Page, GetType(), "alt", "window.returnValue ='1';window.close();", true);
            //hdn585_ValueChanged(sender, e);
            //mpeEditEE.Hide();
        }
        [HttpPost]
        public async Task<IActionResult> SubmitLTA([FromBody] ReceiptDtl_LTAMain obj)
        {

            var LTAVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var LTASession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(LTAVal);


            if (obj.MOTHeader != "1")
            {
                obj.NormalTrainFare = "";

            }


            foreach (var gr in obj.objLTAdtlcolls)
            {
                ReceiptDtl_LTACollection objcoll = new ReceiptDtl_LTACollection();


                if (gr.IsTravel == "")
                {
                    return Json(new Tuple<string, string>("error", "Is Travel is mandatory fields."));
                }
                if (objcoll.IsTravel == "1")
                {
                    if (objcoll.MOT == "1")
                    {
                        if (objcoll.Age == "" || objcoll.Amount == "" || objcoll.MOT == "" || objcoll.Name == "" || objcoll.Relation == "" || objcoll.TktBillNo == "")
                        {
                            if ((objcoll.Relation).ToUpper() == "SELF")
                            {
                                return Json(new Tuple<string, string>("error", "Bill No and Amount are mandatory fields."));
                            }
                        }
                    }

                    else
                    {
                        if (objcoll.Age == "" || objcoll.Amount == "" || objcoll.MOT == "" || objcoll.Name == "" || objcoll.Relation == "" || objcoll.TktBillNo == "")
                        {

                            return Json(new Tuple<string, string>("error", "Bill No and Amount are mandatory fields."));

                        }
                    }

                    if ((objcoll.MOT == "1" || objcoll.MOT == "3") && objcoll.TrainFare == "")
                    {
                        if (objcoll.MOT == "1" && (objcoll.Relation).ToUpper() == "SELF")
                        {
                            return Json(new Tuple<string, string>("error", "In case of bus and taxi train fare is mandatory."));

                        }
                        else if (objcoll.MOT == "3" && objcoll.TrainFare == "")
                        {
                            return Json(new Tuple<string, string>("error", "In case of bus and taxi train fare is mandatory."));

                        }
                    }
                }

            }

            if (obj.JFrom != "" && obj.JTo != "" && obj.Placeofvisit != "" && obj.objLTAdtlcolls.Count() == 0)
            {
                return Json(new Tuple<string, string>("error", "Please add  at least one journey details."));

            }
            var TotEligableAmt = obj.objLTAdtlcolls.Sum(x => int.Parse(x.ElilableAmt));
            obj.TotEligableAmt = TotEligableAmt.ToString();
            var objReturn = new
            {
                TotEligableAmount = TotEligableAmt,
                StartDate = int.Parse(DateTime.Parse(obj.JFrom).ToString("ddMMyyyy")),
                EndDate = int.Parse(DateTime.Parse(obj.JTo).ToString("ddMMyyyy")),
                Trip = obj.TripNo
            };
            var taxDetails = LTASession.Taxinv582.Where(x => x.TAXHEAD.ToUpper() == "EXEMPTION AMOUNT").FirstOrDefault();
            UpdateProjAmount(LTASession, Decimal.Parse(taxDetails.TAXHEADID).ToString("0"), TotEligableAmt.ToString());
            taxDetails = LTASession.Taxinv582.Where(x => x.TAXHEAD.ToUpper() == "JOURNEY START DATE").FirstOrDefault();
            UpdateProjAmount(LTASession, Decimal.Parse(taxDetails.TAXHEADID).ToString("0"), DateTime.Parse(obj.JFrom).ToString("ddMMyyyy"));
            taxDetails = LTASession.Taxinv582.Where(x => x.TAXHEAD.ToUpper() == "JOURNEY END DATE").FirstOrDefault();
            UpdateProjAmount(LTASession, Decimal.Parse(taxDetails.TAXHEADID).ToString("0"), DateTime.Parse(obj.JTo).ToString("ddMMyyyy"));
            taxDetails = LTASession.Taxinv582.Where(x => x.TAXHEAD.ToUpper() == ("Trip/Exemption Claimed Between 1-Jan-2022 to 31-Dec-2025").ToUpper()).FirstOrDefault();
            UpdateProjAmount(LTASession, Decimal.Parse(taxDetails.TAXHEADID).ToString("0"), obj.TripNo.ToString());

            LTASession.LTA = obj;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(LTASession));
            return Json(new Tuple<string, object>("success", objReturn)); // ScriptManager.RegisterClientScriptBlock(Page, GetType(), "alt", "window.returnValue ='1';window.close();", true);

        }

        [HttpPost]
        public async Task<IActionResult> ViewB12Details(string Head, string TaxId)
        {
            var B12Val = _sessionService.Get<string>("EMPTAX_INVDEC");
            var B12Session = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(B12Val);

            EmployerB12ViewModel data = new EmployerB12ViewModel();
            if (B12Session.B12 != null)
            {
                data = B12Session.B12;
            }
            else
            {
                data = B12Form();
            }
            B12Session.B12 = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(B12Session));
            return PartialView("_B12Details", data);

        }
        [HttpPost]
        public async Task<IActionResult> AddB12Details([FromBody] EmployerB12ViewModel obj)
        {
            var B12Val = _sessionService.Get<string>("EMPTAX_INVDEC");
            var B12Session = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(B12Val);
            EmployerB12ViewModel data = new EmployerB12ViewModel();
            if (B12Session.B12 != null)
            {
                data = B12Session.B12;
            }
            if (obj.Address.Trim() == "" || obj.B12.EmployerName == "" || obj.B12.EmployerPan == "" || obj.B12.FromDate == "" || obj.B12.ToDate == "" || obj.B12.EmployerTan == "" || obj.B12.TotalGross == "" || obj.B12.Perk == "" || obj.B12.TotalExempt == "" || obj.B12.Deductions == "" || obj.B12.TotalAmtTax == "")
            {
                Json(new Tuple<string, string>("error", "All fields are mandatory"));

            }
            if (obj.B12.EmployerPan.Length != 10 || obj.B12.EmployerTan.Length != 10)
            {
                return Json(new Tuple<string, string>("error", "PAN & TAN No. must be 10 character."));

            }
            obj.B12.TaxableIncome = (
     (Int64.Parse(obj.B12.TotalGross ?? "0") + Int64.Parse(obj.B12.Perk ?? "0"))
     - (Int64.Parse(obj.B12.TotalExempt ?? "0") + Int64.Parse(obj.B12.Deductions ?? "0"))
 ).ToString();
            if (!string.IsNullOrEmpty(obj.B12.SLNO))
            {
                data.B12List = data.B12List.Where(x => x.SLNO != obj.B12.SLNO).ToList();

                data.B12List.Insert((int.Parse(obj.B12.SLNO) - 1), obj.B12);
            }
            else
            {

                obj.B12.SLNO = (data.B12List.Count() + 1).ToString();

                data.B12List.Add(obj.B12);
            }
            data.Address = obj.Address;
            B12Session.B12 = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(B12Session));
            return PartialView("_B12Details", data);

        }

        [HttpPost]
        public async Task<IActionResult> SubmitB12Details()
        {
            var B12Val = _sessionService.Get<string>("EMPTAX_INVDEC");
            var B12Session = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(B12Val);
            EmployerB12ViewModel data = new EmployerB12ViewModel();
            if (B12Session.B12 != null)
            {
                data = B12Session.B12;
            }

            if (data != null)
            {
                var total = new
                {
                    TotalGross = data.B12List.Sum(x => Int64.Parse(x.TotalGross)),
                    Perk = data.B12List.Sum(x => Int64.Parse(x.Perk)),
                    TotalExempt = data.B12List.Sum(x => Int64.Parse(x.TotalExempt)),
                    Deductions = data.B12List.Sum(x => Int64.Parse(x.Deductions)),
                    TotalAmtTax = data.B12List.Sum(x => Int64.Parse(x.TotalAmtTax))
                };
                var taxDetails = B12Session.Taxinv580.Where(x => x.TAXHEAD.ToUpper() == "SALARY AS PER PROVISIONS US 17(1)").FirstOrDefault();
                UpdateProjAmount(B12Session, Decimal.Parse(taxDetails.TAXHEADID).ToString("0"), total.TotalGross.ToString());
                taxDetails = B12Session.Taxinv580.Where(x => x.TAXHEAD.ToUpper() == "VALUE OF PERQUISITE  US 17 (2)").FirstOrDefault();
                UpdateProjAmount(B12Session, Decimal.Parse(taxDetails.TAXHEADID).ToString("0"), total.Perk.ToString());
                taxDetails = B12Session.Taxinv580.Where(x => x.TAXHEAD.ToUpper() == "EXEMPTION US 10").FirstOrDefault();
                UpdateProjAmount(B12Session, Decimal.Parse(taxDetails.TAXHEADID).ToString("0"), total.TotalExempt.ToString());
                taxDetails = B12Session.Taxinv580.Where(x => x.TAXHEAD.ToUpper() == "PROVIDEND FUND").FirstOrDefault();
                UpdateProjAmount(B12Session, Decimal.Parse(taxDetails.TAXHEADID).ToString("0"), total.Deductions.ToString());
                taxDetails = B12Session.Taxinv580.Where(x => x.TAXHEAD.ToUpper() == "INCOME TAX DEDUCTED").FirstOrDefault();
                UpdateProjAmount(B12Session, Decimal.Parse(taxDetails.TAXHEADID).ToString("0"), total.TotalAmtTax.ToString());
                _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(B12Session));
                return Json(new Tuple<string, object>("success", total));
            }
            return Json(new Tuple<string, object>("error", 0));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteB12Details(int Id)
        {
            var B12Val = _sessionService.Get<string>("EMPTAX_INVDEC");
            var B12Session = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(B12Val);
            EmployerB12ViewModel data = new EmployerB12ViewModel();
            if (B12Session.B12 != null)
            {
                data = B12Session.B12;
            }
            data.B12List = data.B12List.Where(x => x.SLNO != Id.ToString()).ToList();
            B12Session.B12 = data;
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(B12Session));
            return PartialView("_B12Details", data);
        }

        [HttpPost]
        public async Task<IActionResult> TaxSaveAsDraft(string HRClaim, string Type)
        {
            var MasterVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var MasterSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(MasterVal);
            MasterSession.HRClaim = HRClaim;
            if (string.IsNullOrEmpty(HRClaim))
            {
                return Json(new Tuple<string, string>("error", "Please select HR claim Yes/No"));

            }
            if (MasterSession.FromDate == "" || MasterSession.ToDate == "")
            {
                return Json(new Tuple<string, string>("error", "Your time period is not set to fill Projected Investment Declaration.Please contact to Finance Department."));

            }
            DateTime strStartdate = DateTime.ParseExact(MasterSession.FromDate, "dd-MMM-yyyy", null);
            DateTime strEnddate = DateTime.ParseExact(MasterSession.ToDate, "dd-MMM-yyyy", null);
            DateTime strtodaydate = DateTime.Today;
            if (!(strtodaydate.Date >= strStartdate.Date && strtodaydate.Date <= strEnddate.Date))
            {
                return Json(new Tuple<string, string>("error", "Your time period has expire to fill Projected/Investment declaration.Please Contact to Finance department."));

            }
            if (MasterSession.ISEDITABLEACTUAL.ToString().Trim() == "0")
            {
                return Json(new Tuple<string, string>("error", "Not Editable"));

            }

            #region "Section 80C and 80D detail of associate"
            string[,] invdetail = new string[MasterSession.Taxinv586.Count() + MasterSession.Taxinv585.Count(), 4];
            //Creating the array for section 80c Declaraton
            for (int i = 0; i < MasterSession.Taxinv586.Count(); i++)
            {


                invdetail[i, 0] = MasterSession.Taxinv586[i].TAXHEADID;// hdn.Value;//Tax Head Id
                invdetail[i, 1] = MasterSession.Taxinv586[i].OptionId;//Option value
                invdetail[i, 2] = MasterSession.Taxinv586[i].PROJ_AMOUNT;//Projection amount
                invdetail[i, 3] = MasterSession.Taxinv586[i].ACT_AMOUNT;//Actual amount
            }
            int rowno = MasterSession.Taxinv586.Count;
            //Creating the array for section 80c Declaraton
            for (int i = 0; i < MasterSession.Taxinv585.Count; i++)
            {
                invdetail[rowno + i, 0] = MasterSession.Taxinv585[i].TAXHEADID;// hdn.Value;//Tax Head Id
                invdetail[rowno + i, 1] = MasterSession.Taxinv585[i].OptionId;//Option value
                invdetail[rowno + i, 2] = MasterSession.Taxinv585[i].PROJ_AMOUNT;//Projection amount
                invdetail[rowno + i, 3] = MasterSession.Taxinv585[i].ACT_AMOUNT;//Actual amount
            }
            #endregion

            #region "Section 584,582,580 detail of associate"
            string[,] inv584detail = new string[MasterSession.Taxinv584.Count + MasterSession.Taxinv580.Count + MasterSession.Taxinv582.Count + MasterSession.Taxinv584Sub2.Count, 4];
            for (int i = 0; i < MasterSession.Taxinv584.Count; i++)
            {
                var AMOUNT = MasterSession.ProjStatus == "1" ? MasterSession.Taxinv584[i].PROJ_AMOUNT : MasterSession.Taxinv584[i].ACT_AMOUNT;
                if (AMOUNT != "" && Convert.ToDouble(MasterSession.Taxinv584[i].MAXLIMIT) > 0 && Convert.ToDouble(AMOUNT) > Convert.ToDouble(MasterSession.Taxinv584[i].MAXLIMIT))
                {
                    return Json(new Tuple<string, string>("error", "Maximum deduction for interest on home loan U / S 24B is permissble only upto Rs. 2, 00, 000 / -."));

                }
                inv584detail[i, 0] = MasterSession.Taxinv584[i].TAXHEADID;// hdn.Value;//Tax Head Id
                inv584detail[i, 1] = MasterSession.Taxinv584[i].OptionId;//Option value
                inv584detail[i, 2] = AMOUNT;//Projection amount
            }
            rowno = MasterSession.Taxinv584.Count;
            for (int i = 0; i < MasterSession.Taxinv582.Count; i++)
            {

                inv584detail[rowno + i, 0] = MasterSession.Taxinv582[i].TAXHEADID;// hdn.Value;//Tax Head Id
                inv584detail[rowno + i, 1] = MasterSession.Taxinv582[i].OptionId;//Option value
                inv584detail[rowno + i, 2] = MasterSession.Taxinv582[i].ACT_AMOUNT;//Projection amount
            }
            rowno += MasterSession.Taxinv582.Count;
            for (int i = 0; i < MasterSession.Taxinv580.Count; i++)
            {

                inv584detail[rowno + i, 0] = MasterSession.Taxinv580[i].TAXHEADID;// hdn.Value;//Tax Head Id
                inv584detail[rowno + i, 1] = MasterSession.Taxinv580[i].OptionId;//Option value
                inv584detail[rowno + i, 2] = MasterSession.Taxinv580[i].ACT_AMOUNT;//Projection amount
            }
            rowno += MasterSession.Taxinv580.Count;
            for (int i = 0; i < MasterSession.Taxinv584Sub2.Count; i++)
            {

                inv584detail[rowno + i, 0] = MasterSession.Taxinv584Sub2[i].TAXHEADID;// hdn.Value;//Tax Head Id
                inv584detail[rowno + i, 1] = MasterSession.Taxinv584Sub2[i].OptionId;//Option value
                inv584detail[rowno + i, 2] = MasterSession.Taxinv584Sub2[i].PROJ_AMOUNT;//Projection amount

            }
            #endregion
            string strtaxregime = "0";
             
            try
            {
                List<HRADetails> _listRentDetail = new List<HRADetails>();
                if (MasterSession.HRADetaillist.hRAList != null)
                {
                    _listRentDetail = MasterSession.HRADetaillist.hRAList;
                }
                

                //objuser.UpdateUserPancardno(Session["userID"].ToString(), txtpancard.Text.Trim(), txtmobileno.Text);
                string strRentamt = (MasterSession.AnnualRent == "" || MasterSession.AnnualRent == "0" ? "0" : MasterSession.AnnualRent);
                string strIsexep = (MasterSession.AnnualRent == "" || MasterSession.AnnualRent == "0" ? "" : "X");//(chktaxexmp.Checked == true ? "X" : "")
                                                                                                                  //objtax.UpdateEmpInvDetail(Session["userID"].ToString(), ddlki.SelectedValue, txtfromdate.Text, txttodate.Text, ddlacctype.SelectedValue, ddlcitycat.SelectedValue, strIsexep, strRentamt, txtresaddress.Text, txtmobileno.Text, (Rent_Detail)ViewState["RENT"], inv584detail, invdetail, "1", "0", (ArrayList)ViewState["Receipt_586"], grdtaxinv586, (ArrayList)ViewState["Receipt_585"], grdtaxinv585, (ReceiptDtl_LTAMain)ViewState["LTADEC"], (ReceiptDtl_12BMain)ViewState["12BDtlList"], (LoanDetail)ViewState["Loan"], (Emp80EE_Detail)ViewState["Emp80EE_585"]);
                objtax.UpdateTaxEmpInvDetail(_sessionService.Get<string>("userID"), MasterSession.KIIID, MasterSession.FromDate, MasterSession.ToDate, "", "", strIsexep, MasterSession.HRClaim.ToString(), strRentamt, MasterSession.ResidentialAddress, MasterSession.MOBILENO, _listRentDetail, inv584detail, invdetail, "1", "0", MasterSession.ReceiptIndexViewModel, MasterSession.LTA, MasterSession.B12, MasterSession.LoanApplicationViewModel, MasterSession.EE, MasterSession.LoanApplicationViewModel, MasterSession.LoanApplicationViewModel, _sessionService.Get<string>("userID"), MasterSession.ProjStatus);
                // UpdateEmpInvDetailTemp(Session["userID"].ToString(), ddlki.SelectedValue, txtfromdate.Text, txttodate.Text, ddlacctype.SelectedValue, ddlcitycat.SelectedValue, strIsexep, strRentamt, txtresaddress.Text, txtmobileno.Text, _listRentDetail, inv584detail, invdetail, "1", "0", (ArrayList)ViewState["Receipt_586"], grdtaxinv586, (ArrayList)ViewState["Receipt_585"], grdtaxinv585, (ReceiptDtl_LTAMain)ViewState["LTADEC"], (ReceiptDtl_12BMain)ViewState["12BDtlList"], (LoanDetail)ViewState["Loan"], (Emp80EE_Detail)ViewState["Emp80EE_585"], (ArrayList)ViewState["HLLIST"]);
                if (MasterSession.ProjStatus == "1")
                {
                    objtax.Update_TaxPeriodProjected(strtaxregime, _sessionService.Get<string>("userID"), MasterSession.KIIID);
                }
                else
                {
                    objtax.Update_TaxPeriod(strtaxregime, _sessionService.Get<string>("userID"), MasterSession.KIIID);
                }


                return Json(new Tuple<string, string>("success", "Information Saved As Draft"));

            }
            catch (Exception ex)
            {

                return Json(new Tuple<string, string>("error", "Updation failed"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> TaxSubmit(string HRClaim, string Type)
        {
            var MasterVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var MasterSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(MasterVal);
            MasterSession.HRClaim = HRClaim;
            MasterSession.TaxType = Type;
            if (string.IsNullOrEmpty(HRClaim))
            {
                return Json(new Tuple<string, string>("error", "Please select HR claim Yes/No"));

            }
            
            if (MasterSession.FromDate == "" || MasterSession.ToDate == "")
            {
                return Json(new Tuple<string, string>("error", "Your time period is not set to fill Projected Investment Declaration.Please contact to Finance Department."));

            }
            DateTime strStartdate = DateTime.ParseExact(MasterSession.FromDate, "dd-MMM-yyyy", null);
            DateTime strEnddate = DateTime.ParseExact(MasterSession.ToDate, "dd-MMM-yyyy", null);
            DateTime strtodaydate = DateTime.Today;
            if (!(strtodaydate.Date >= strStartdate.Date && strtodaydate.Date <= strEnddate.Date))
            {
                return Json(new Tuple<string, string>("error", "Your time period has expire to fill Projected/Investment declaration.Please Contact to Finance department."));

            }
            if (MasterSession.ISEDITABLEACTUAL.ToString().Trim() == "0")
            {
                return Json(new Tuple<string, string>("error", "Not Editable"));

            }

            #region "Section 80C and 80D detail of associate"
            string[,] invdetail = new string[MasterSession.Taxinv586.Count() + MasterSession.Taxinv585.Count(), 4];
            //Creating the array for section 80c Declaraton
            for (int i = 0; i < MasterSession.Taxinv586.Count(); i++)
            {


                invdetail[i, 0] = MasterSession.Taxinv586[i].TAXHEADID;// hdn.Value;//Tax Head Id
                invdetail[i, 1] = MasterSession.Taxinv586[i].OptionId;//Option value
                invdetail[i, 2] = MasterSession.Taxinv586[i].PROJ_AMOUNT;//Projection amount
                invdetail[i, 3] = MasterSession.Taxinv586[i].ACT_AMOUNT;//Actual amount
            }
            int rowno = MasterSession.Taxinv586.Count;
            //Creating the array for section 80c Declaraton
            for (int i = 0; i < MasterSession.Taxinv585.Count; i++)
            {
                invdetail[rowno + i, 0] = MasterSession.Taxinv585[i].TAXHEADID;// hdn.Value;//Tax Head Id
                invdetail[rowno + i, 1] = MasterSession.Taxinv585[i].OptionId;//Option value
                invdetail[rowno + i, 2] = MasterSession.Taxinv585[i].PROJ_AMOUNT;//Projection amount
                invdetail[rowno + i, 3] = MasterSession.Taxinv585[i].ACT_AMOUNT;//Actual amount
            }
            #endregion

            #region "Section 584,582,580 detail of associate"
            string[,] inv584detail = new string[MasterSession.Taxinv584.Count + MasterSession.Taxinv580.Count + MasterSession.Taxinv582.Count + MasterSession.Taxinv584Sub2.Count, 4];
            for (int i = 0; i < MasterSession.Taxinv584.Count; i++)
            {

                if (MasterSession.Taxinv584[i].PROJ_AMOUNT != "" && Convert.ToDouble(MasterSession.Taxinv584[i].MAXLIMIT) > 0 && Convert.ToDouble(MasterSession.Taxinv584[i].PROJ_AMOUNT) > Convert.ToDouble(MasterSession.Taxinv584[i].MAXLIMIT))
                {
                    return Json(new Tuple<string, string>("error", "Maximum deduction for interest on home loan U / S 24B is permissble only upto Rs. 2, 00, 000 / -."));

                }
                inv584detail[i, 0] = MasterSession.Taxinv584[i].TAXHEADID;// hdn.Value;//Tax Head Id
                inv584detail[i, 1] = MasterSession.Taxinv584[i].OptionId;//Option value
                inv584detail[i, 2] = MasterSession.Taxinv584[i].PROJ_AMOUNT;//Projection amount
            }
            rowno = MasterSession.Taxinv584.Count;
            for (int i = 0; i < MasterSession.Taxinv582.Count; i++)
            {

                inv584detail[rowno + i, 0] = MasterSession.Taxinv582[i].TAXHEADID;// hdn.Value;//Tax Head Id
                inv584detail[rowno + i, 1] = MasterSession.Taxinv582[i].OptionId;//Option value
                inv584detail[rowno + i, 2] = MasterSession.Taxinv582[i].PROJ_AMOUNT;//Projection amount
            }
            rowno += MasterSession.Taxinv582.Count;
            for (int i = 0; i < MasterSession.Taxinv580.Count; i++)
            {

                inv584detail[rowno + i, 0] = MasterSession.Taxinv580[i].TAXHEADID;// hdn.Value;//Tax Head Id
                inv584detail[rowno + i, 1] = MasterSession.Taxinv580[i].OptionId;//Option value
                inv584detail[rowno + i, 2] = MasterSession.Taxinv580[i].PROJ_AMOUNT;//Projection amount
            }
            rowno += MasterSession.Taxinv580.Count;
            for (int i = 0; i < MasterSession.Taxinv584Sub2.Count; i++)
            {

                inv584detail[rowno + i, 0] = MasterSession.Taxinv584Sub2[i].TAXHEADID;// hdn.Value;//Tax Head Id
                inv584detail[rowno + i, 1] = MasterSession.Taxinv584Sub2[i].OptionId;//Option value
                inv584detail[rowno + i, 2] = MasterSession.Taxinv584Sub2[i].PROJ_AMOUNT;//Projection amount

            }
            #endregion
            string strtaxregime = "0";
           
            try
            {
                List<HRADetails> _listRentDetail = new List<HRADetails>();
                if (MasterSession.HRADetaillist.hRAList != null)
                {
                    _listRentDetail = MasterSession.HRADetaillist.hRAList;
                }
                

                //objuser.UpdateUserPancardno(Session["userID"].ToString(), txtpancard.Text.Trim(), txtmobileno.Text);
                string strRentamt = (MasterSession.AnnualRent == "" || MasterSession.AnnualRent == "0" ? "0" : MasterSession.AnnualRent);
                string strIsexep = (MasterSession.AnnualRent == "" || MasterSession.AnnualRent == "0" ? "" : "X");//(chktaxexmp.Checked == true ? "X" : "")
                                                                                                                  //objtax.UpdateEmpInvDetail(Session["userID"].ToString(), ddlki.SelectedValue, txtfromdate.Text, txttodate.Text, ddlacctype.SelectedValue, ddlcitycat.SelectedValue, strIsexep, strRentamt, txtresaddress.Text, txtmobileno.Text, (Rent_Detail)ViewState["RENT"], inv584detail, invdetail, "1", "0", (ArrayList)ViewState["Receipt_586"], grdtaxinv586, (ArrayList)ViewState["Receipt_585"], grdtaxinv585, (ReceiptDtl_LTAMain)ViewState["LTADEC"], (ReceiptDtl_12BMain)ViewState["12BDtlList"], (LoanDetail)ViewState["Loan"], (Emp80EE_Detail)ViewState["Emp80EE_585"]);
                objtax.UpdateTaxEmpInvDetail(_sessionService.Get<string>("userID"), MasterSession.KIIID, MasterSession.FromDate, MasterSession.ToDate, "", "", strIsexep, MasterSession.HRClaim.ToString(), strRentamt, MasterSession.ResidentialAddress, MasterSession.MOBILENO, _listRentDetail, inv584detail, invdetail, "1", "0", MasterSession.ReceiptIndexViewModel, MasterSession.LTA, MasterSession.B12, MasterSession.LoanApplicationViewModel, MasterSession.EE, MasterSession.LoanApplicationViewModel, MasterSession.LoanApplicationViewModel, _sessionService.Get<string>("userID"), MasterSession.ProjStatus);
                // UpdateEmpInvDetailTemp(Session["userID"].ToString(), ddlki.SelectedValue, txtfromdate.Text, txttodate.Text, ddlacctype.SelectedValue, ddlcitycat.SelectedValue, strIsexep, strRentamt, txtresaddress.Text, txtmobileno.Text, _listRentDetail, inv584detail, invdetail, "1", "0", (ArrayList)ViewState["Receipt_586"], grdtaxinv586, (ArrayList)ViewState["Receipt_585"], grdtaxinv585, (ReceiptDtl_LTAMain)ViewState["LTADEC"], (ReceiptDtl_12BMain)ViewState["12BDtlList"], (LoanDetail)ViewState["Loan"], (Emp80EE_Detail)ViewState["Emp80EE_585"], (ArrayList)ViewState["HLLIST"]);
                if (MasterSession.ProjStatus == "1")
                {
                    objtax.Update_TaxFinalPeriodProjected(strtaxregime, _sessionService.Get<string>("userID"), MasterSession.KIIID);
                }

                else
                {
                    objtax.Update_TaxFinalPeriod(strtaxregime, _sessionService.Get<string>("userID"), MasterSession.KIIID);
                }
                return Json(new Tuple<string, string>("success", "Income Tax Declaration Form Submitted Successfully."));

            }
            catch (Exception ex)
            {

                return Json(new Tuple<string, string>("error", "Updation failed"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangeProjAmount(string Val, string TaxId)
        {
            var ReciptsVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var ReciptsSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(ReciptsVal);
            UpdateProjAmount(ReciptsSession, TaxId, Val);
            _sessionService.Set("EMPTAX_INVDEC", JsonConvert.SerializeObject(ReciptsSession));
            return Json(null);
        }
        [HttpGet]
        public async Task<IActionResult> EmpTaxInv_12BBReport(string SYKIID)
        {
            var data = new EmpTaxInvDec12BBReportModel();
            var objdt = _objIPMS.GetKiList();
            string strKiId = string.Empty;

            if (!String.IsNullOrEmpty(SYKIID))
                strKiId = SYKIID;
            else
                strKiId = _objIPMS.GetKIId();


            data.lblname = LoginEmpDetails.Employee_Name.ToString();
            data.lblpannumber = LoginEmpDetails.PancardNo.ToString();
            data.litdate = System.DateTime.Now.ToString("dd-MMM-yyyy");
            data.litdesignation = LoginEmpDetails.Designation.ToString();
            data.litfulldate = LoginEmpDetails.Employee_Name.ToString();

            objdt.DefaultView.RowFilter = "SYKIID=" + strKiId;
            string financialyear = objdt.DefaultView.ToTable().Rows[0]["FINANCIALYEAR"].ToString();
            data.lblfinancialyear = financialyear;

            #region "House Rent"
            DataTable dt_House = objtax.GetEmpRent_Invdetail(LoginEmpDetails.Employee_Code.ToString(), strKiId);
            if (dt_House.Rows.Count > 0)
            {
                data.lbladdress = dt_House.Rows[0]["SAPaddress"].ToString();
                data.ltlldevidance = "Evidence attached";
                var json = JsonConvert.SerializeObject(dt_House);
                data.HouseItem = JsonConvert.DeserializeObject<List<HouseItem>>(json);

                //ltlrentamount.Text = dt_House.Rows[0]["RENT_AMOUNT"].ToString();
                //ltlldname.Text = dt_House.Rows[0]["LANDLORDNAME"].ToString();
                //ltlldpan.Text = dt_House.Rows[0]["LANDLORDPAN"].ToString();
                //ltlldaddress.Text = dt_House.Rows[0]["LLHNO"].ToString()+" "+dt_House.Rows[0]["LLGNO"].ToString()+" "+dt_House.Rows[0]["LLGNO"].ToString()+" "+dt_House.Rows[0]["LLVILLAGE"].ToString()+" "+dt_House.Rows[0]["LLCITY"].ToString();


            }
            #endregion
            #region "LTA"
            ReceiptDtl_LTAMain objLTA = new ReceiptDtl_LTAMain();

            int int_LTATotal = 0;
            DataSet ds = objtax.GetLTADtl(LoginEmpDetails.Employee_Code.ToString(), strKiId, "1");
            DataView dtv = ds.Tables[1].DefaultView;
            dtv.RowFilter = "ISTRAVEL=1";

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dtv.ToTable().Rows)
                {
                    int_LTATotal = int_LTATotal + Convert.ToInt32(dr["ISELIGIBLEAMT"].ToString());
                }
                data.ltl_LTAAmount = int_LTATotal.ToString();
                data.ltlltaevidance = "Evidence attached";

            }
            #endregion

            #region "Interest on Housing Loan"
            DataTable DT_LoanDtl = objtax.GetLOANDTL(LoginEmpDetails.Employee_Code.ToString(), strKiId);
            if (DT_LoanDtl.Rows.Count > 0)
            {
                string FIN_TAXLOANDTLID = DT_LoanDtl.Rows[0]["FIN_TAXLOANDTLID"].ToString();
                DataTable DTHouse = objtax.GetHOUSELOANDTL(FIN_TAXLOANDTLID);
                if (DTHouse.Rows.Count > 0)
                {
                    var json = JsonConvert.SerializeObject(DTHouse);
                    data.REPHOUSELOAN = JsonConvert.DeserializeObject<IList<HouseLoanItem>>(json);


                    data.ltlhlevidance = "Evidence attached";
                    //ltlhllname.Text = DT_LoanDtl.Rows[0]["LOAD_PROVIDER"].ToString();
                    //ltlhlladress.Text = DT_LoanDtl.Rows[0]["LOANPROVIDER_ADDRESS"].ToString();
                    //ltlhllpan.Text = DT_LoanDtl.Rows[0]["LOANPROVIDER_PAN"].ToString();
                    //---------------------------
                    Int64 currintr, TotalYear1, TotalYear2, TotalYear3;
                    string ltl_CurrentInterest = DT_LoanDtl.Rows[0]["CURRINTREST"].ToString();
                    string ltl_PreYear1 = DT_LoanDtl.Rows[0]["YEAR1INTEREST"].ToString();
                    string ltl_PreYear2 = DT_LoanDtl.Rows[0]["YEAR2INTEREST"].ToString();
                    string ltl_PreYear3 = DT_LoanDtl.Rows[0]["YEAR3INTEREST"].ToString();
                    currintr = ltl_CurrentInterest != "" ? (Convert.ToInt64(ltl_CurrentInterest)) : 0;
                    TotalYear1 = ltl_PreYear1 != "" ? (Convert.ToInt64(ltl_PreYear1) / 5) : 0;
                    TotalYear2 = ltl_PreYear2 != "" ? (Convert.ToInt64(ltl_PreYear2) / 5) : 0;
                    TotalYear3 = ltl_PreYear3 != "" ? (Convert.ToInt64(ltl_PreYear3) / 5) : 0;
                    string ltl_totalHouseLoan = (currintr + TotalYear1 + TotalYear2 + TotalYear3).ToString();
                    // Percentage of share in tax exemption-----
                    string ltl_Taxexemption = DT_LoanDtl.Rows[0]["TAXEXEMPSHAREPER"].ToString();


                    if (DT_LoanDtl.Rows[0]["ISJOINTLOAN"].ToString() == "1" && DT_LoanDtl.Rows[0]["TAXEXEMPSHAREPER"].ToString() != "")
                    {
                        data.ltlhlpayable = Convert.ToInt64((Convert.ToInt64(ltl_totalHouseLoan) * Convert.ToInt64(ltl_Taxexemption)) / 100).ToString();
                    }
                    else
                    {
                        data.ltlhlpayable = Convert.ToInt64(Convert.ToInt64(ltl_totalHouseLoan == "" ? "0" : ltl_totalHouseLoan)).ToString();
                    }
                    if (Convert.ToInt32(data.ltlhlpayable) > 200000)
                        data.ltlhlpayable = "200000";
                }
            }
                #endregion

                DataTable dt = objtax.GetEmpTax_Invdetail("", LoginEmpDetails.Employee_Code.ToString(), "1", strKiId, "586");
                if (dt.Rows.Count > 0)
                {
                var json = JsonConvert.SerializeObject(dt);
                data.Rep_section80c = JsonConvert.DeserializeObject<List<Section80Item>>(json);
             
                    data.ltlsec80cevidance = "Evidence attached";
                }
                DataTable dt1 = objtax.GetEmpTax_Invdetail("", LoginEmpDetails.Employee_Code.ToString(), "1", strKiId, "585");
                if (dt1.Rows.Count > 0)
                {
                var json = JsonConvert.SerializeObject(dt1);
                data.Rep_sectionother = JsonConvert.DeserializeObject<List<Section80Item>>(json);
                
                }
            return View("EmpTaxInv_12BBReport", data);
        }
        public async Task<IActionResult> TaxSubmitNew()
        {
            var MasterVal = _sessionService.Get<string>("EMPTAX_INVDEC");
            var MasterSession = JsonConvert.DeserializeObject<EMPTAX_INVDEC>(MasterVal);
            string strtaxregime = string.Empty;
            DateTime strtodaydate = DateTime.Today;

            strtaxregime = "1";

            try
            {
                if (MasterSession.ProjStatus == "1")
                {
                    objtax.Update_TaxFinalPeriodProjected(strtaxregime, _sessionService.Get<string>("userID"), MasterSession.KIIID);
                }
                else
                {
                    objtax.Update_TaxFinalPeriod(strtaxregime, _sessionService.Get<string>("userID"), MasterSession.KIIID);
                }

                    return Json(new Tuple<string, string>("success", "Income Tax Declaration Form Submitted Successfully."));
            }
            catch (Exception ex)
            {
                return Json(new Tuple<string, string>("error", "Updation failed"));
            }
        }
        public EMPTAX_INVDEC UpdateProjAmount(EMPTAX_INVDEC model, string taxHeadId, string projAmount)
        {
            var allInvoices = Enumerable.Empty<TaxInvoiceDetails>()
                .Concat(model.Taxinv580 ?? Enumerable.Empty<TaxInvoiceDetails>())
                .Concat(model.Taxinv582 ?? Enumerable.Empty<TaxInvoiceDetails>())
                .Concat(model.Taxinv584 ?? Enumerable.Empty<TaxInvoiceDetails>())
                .Concat(model.Taxinv584Sub2 ?? Enumerable.Empty<TaxInvoiceDetails>())
                .Concat(model.Taxinv585 ?? Enumerable.Empty<TaxInvoiceDetails>())
                .Concat(model.Taxinv586 ?? Enumerable.Empty<TaxInvoiceDetails>());
            var invoice = allInvoices.FirstOrDefault(x => Convert.ToDecimal(x.TAXHEADID).ToString("0") == taxHeadId);
            if (invoice != null)
            {
                if (model.ProjStatus == "1")
                {
                    invoice.PROJ_AMOUNT = projAmount;
                }
                else
                {

                    invoice.ACT_AMOUNT = projAmount;
                }
            }
            return model;
        }

        public JsonResult Calculate(string ddlmot, string txtamount, string txtfare, string txtNormalTrainFare, string gender, string age, bool isTravelled)
        {
            string fareValue = "0";
            string eligibleAmount = "";

            // Case: Train (ddlmot == "1")
            if (ddlmot == "1")
            {
                if (!string.IsNullOrEmpty(txtamount) && !string.IsNullOrEmpty(txtNormalTrainFare))
                {
                    // Call your existing eligibility logic
                    fareValue = getfareeligibility(ddlmot, gender, age, txtNormalTrainFare);
                    eligibleAmount = fareValue; // Eligible = train fare
                }
            }
            else
            {
                // Case: Other modes
                if (!string.IsNullOrEmpty(txtamount) && !string.IsNullOrEmpty(txtfare))
                {
                    // Eligible = min(amount, fare)
                    eligibleAmount = (Convert.ToInt64(txtamount) > Convert.ToInt64(txtfare)) ? txtfare : txtamount;
                    fareValue = txtfare;
                }
            }

            return Json(new
            {
                txtfare = fareValue,
                ltleliamount = eligibleAmount
            });
        }
        private EmpTaxInvDecReportModel FillKI(string KIID, EmpTaxInvDecReportModel obj)
        {
            System.Data.DataTable objdt = new System.Data.DataTable();
            string strKiId = KIID;
            objdt = _objIPMS.GetKiList();


            objdt.DefaultView.RowFilter = "SYKIID=" + strKiId;
            string financialyear = objdt.DefaultView.ToTable().Rows[0]["FINANCIALYEAR"].ToString();
            obj.FinStartDate = "01-APR-" + financialyear.Substring(0, 4);
            obj.FinEndDate = "31-MAR-" + financialyear.Substring(5, 4);
            obj.ltlfinyear = financialyear;
            obj.ltlfinyearstart = /*"31-MAR-" +*/ financialyear.Substring(5, 4);
            obj.ltl_LTAYear = financialyear;
            obj.ltl_DeclPeriod = "01-APR-" + financialyear.Substring(0, 4) + " To " + "31-MAR-" + financialyear.Substring(5, 4);
            return obj;
        }
        private ReceiptIndexViewModel TDS585(string str586id)
        {
            ReceiptIndexViewModel data = new ReceiptIndexViewModel();
            var stridTDS585 = str586id;
            var dt = objtax.GetTaxheaddetail(str586id, "", "", "", "");
            if (dt.Rows.Count > 0)
            {
                data.TaxHeadName = dt.Rows[0]["TAXHEAD"].ToString();

            }
            var strKiId = _objIPMS.GetKIId();

            dt = objtax.Get586_585ReceiptDetail(_sessionService.Get<string>("userID"), str586id, strKiId, "1");
            List<ReceiptDtl_586> o = new List<ReceiptDtl_586>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ReceiptDtl_586 ob = new ReceiptDtl_586();
                ob.SLNO = (i + 1);
                ob._PrmDate = dt.Rows[i]["RECEIPT_DATE"].ToString();
                ob._ID = dt.Rows[i]["ID"].ToString();
                ob._ReceiptNo = dt.Rows[i]["RECEIPT_NO"].ToString();
                ob._TaxHeadID = dt.Rows[i]["TAXHEADID"].ToString();
                ob.Amount = dt.Rows[i]["RECEIPT_AMOUNT"].ToString();
                o.Add(ob);
            }
            data.Receipts = o;


            return data;

        }
        private ReceiptIndexViewModel TDS585_LIC(string str586id)
        {
            ReceiptIndexViewModel data = new ReceiptIndexViewModel();
            var stridTDS585 = str586id;
            var dt = objtax.GetTaxheaddetail(str586id, "", "", "", "");
            if (dt.Rows.Count > 0)
            {
                data.TaxHeadName = dt.Rows[0]["TAXHEAD"].ToString();

            }
            var strKiId = _objIPMS.GetKIId();

            dt = objtax.Get586_585ReceiptDetail_V1(_sessionService.Get<string>("userID"), str586id, strKiId, "1");
            List<ReceiptDtl_586> o = new List<ReceiptDtl_586>();
            var yetTotal = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ReceiptDtl_586 ob = new ReceiptDtl_586();
                ob.SLNO = (i + 1);
                ob._Date = dt.Rows[i]["POLICY_DATE"].ToString();
                ob._ID = dt.Rows[i]["TID"].ToString();
                ob._PolicyNo = dt.Rows[i]["POLICYNO"].ToString();
                ob._ReceiptNo = dt.Rows[i]["RECEIPT_NO"].ToString();
                ob._TaxHeadID = dt.Rows[i]["TAXHEADID"].ToString();
                ob._SumAssured = dt.Rows[i]["TOTAL_SUM_ASSURED"].ToString();
                ob._PrmDate = dt.Rows[i]["RECEIPT_DATE"].ToString();
                ob.Amount = dt.Rows[i]["RECEIPT_AMOUNT"].ToString();
                //yetTotal += Convert.ToInt32(dt.Rows[i]["RECEIPT_AMOUNT"].ToString());
                o.Add(ob);
            }
            data.Total = o.Where(x => string.IsNullOrEmpty(x.Amount) == false).Sum(x => Convert.ToInt32(x.Amount)).ToString();
            data.Receipts = o;


            return data;

        }

        private int MonthAmountcalculate(string MonthlyAmt, DateTime fromdate, DateTime todate)
        {
            int NoOfMonth = 0;
            int monthyrent = Convert.ToInt32(MonthlyAmt);

            int fromYear = fromdate.Year;
            int toYear = todate.Year;
            int frommonth = fromdate.Month;
            int tomonth = todate.Month;
            int fromday = fromdate.Day;
            int today = todate.Day;
            if (fromYear == toYear && frommonth == tomonth)
            {
                NoOfMonth = 1;
            }
            else if (fromYear == toYear && frommonth < tomonth)
            {
                NoOfMonth = (tomonth - frommonth) + 1;
            }
            else if (fromYear < toYear)
            {
                int NoOfFromYearmonth = (12 - frommonth) + 1;
                int NoOfToYearmonth = tomonth;
                NoOfMonth = NoOfFromYearmonth + NoOfToYearmonth;
            }

            return monthyrent * NoOfMonth;
        }
        private void BindDropDown(HRADetails obj)
        {
            EmpTaxInvDecReportModel objTax = FillKI(_sessionService.Get<string>("KIID"), new EmpTaxInvDecReportModel());
            DateTime finPeriodstartDate = DateTime.ParseExact(objTax.FinStartDate.Trim(), "d-MMM-yyyy", null);
            DateTime finPeriodEndDate = DateTime.ParseExact(objTax.FinEndDate.Trim(), "d-MMM-yyyy", null);
            // checking finance period

            Dictionary<string, string> objdic = new Dictionary<string, string>();
            string[] month = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            for (int i = 4; i <= 12; i++)
            {
                objdic.Add(i.ToString(), month[i - 1] + "-" + finPeriodstartDate.Year);

            }
            for (int i = 1; i <= 3; i++)
            {
                objdic.Add(i.ToString(), month[i - 1] + "-" + finPeriodEndDate.Year);

            }


            obj.FromMonthOptions = objdic.Select(x => new SelectListItem
            {
                Value = x.Key,
                Text = x.Value
            });
            obj.ToMonthOptions = objdic.Select(x => new SelectListItem
            {
                Value = x.Key,
                Text = x.Value
            });






        }
        private async Task<ReceiptDtl_LTAMain> LTA()
        {

            var strKiId = _objIPMS.GetKIId();
            var objdt = _objIPMS.GetKiList();
            objdt.DefaultView.RowFilter = "SYKIID=" + strKiId;
            string financialyear = objdt.DefaultView.ToTable().Rows[0]["FINANCIALYEAR"].ToString();
            ReceiptDtl_LTAMain objLTA = new ReceiptDtl_LTAMain();


            objLTA.objLTAdtlcolls = new List<ReceiptDtl_LTACollection>();
            DataSet ds = objtax.GetLTADtl(_sessionService.Get<string>("userID"), strKiId, "1");
            if (ds.Tables[0].Rows.Count > 0)
            {
                objLTA.ID = ds.Tables[0].Rows[0]["FIN_TAXLTADTLID"].ToString();
                objLTA.JFrom = ds.Tables[0].Rows[0]["JOURNEYFROM_DT"].ToString();
                objLTA.JTo = ds.Tables[0].Rows[0]["JOURNEYTO_DT"].ToString();
                objLTA.KI = ds.Tables[0].Rows[0]["SYKIID"].ToString();
                objLTA.LTAAmount = ds.Tables[0].Rows[0]["LTATAKEN"].ToString();
                objLTA.Placeofvisit = ds.Tables[0].Rows[0]["PLACEOFVISIT"].ToString();
                objLTA.TripNo = ds.Tables[0].Rows[0]["TRIPNO"].ToString();
                objLTA.NormalTrainFare = ds.Tables[0].Rows[0]["NORMALTRAINFARE"].ToString();
                objLTA.Year = financialyear;

                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    ReceiptDtl_LTACollection objcoll = new ReceiptDtl_LTACollection();
                    objcoll.Age = dr["AGE"].ToString();
                    objcoll.Amount = dr["AMOUNT"].ToString();
                    objcoll.ID = dr["FIN_TAXLTAJOURDTLID"].ToString();
                    objcoll.MOT = dr["MOT"].ToString();
                    objcoll.Name = dr["NAMEOFPERSON"].ToString();
                    objcoll.Relation = dr["RELATION"].ToString();
                    objcoll.TktBillNo = dr["BILLNO"].ToString();
                    objcoll.TrainFare = dr["TRAINFARE"].ToString();
                    objcoll.IsTravel = dr["istravel"].ToString();
                    objcoll.ElilableAmt = dr["ISELIGIBLEAMT"].ToString();
                    objLTA.objLTAdtlcolls.Add(objcoll);
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    objLTA.MOTHeader = ds.Tables[1].Rows[0]["MOT"].ToString();
                    if (ds.Tables[1].Rows[0]["MOT"].ToString() != "1")
                    {
                        objLTA.NormalTrainFare = "";
                    }

                }


            }
            else
            {
                //SAP Connection Object
                //**************** Getting family datial Of employee From SAP

                DataTable dt = new DataTable();
                dt = await objess.GetFamilylist(_sessionService.Get<string>("userID"), "");
                // End of Family Detail
                ReceiptDtl_LTACollection o1 = new ReceiptDtl_LTACollection();
                o1.Name = _sessionService.Get<string>("userName");
                o1.Relation = "Self";
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                TimeSpan ts1 = DateTime.Now.Date.Subtract(DateTime.ParseExact(emp.DOB, "dd-MMM-yyyy", null));
                o1.Age = (ts1.Days / 365).ToString();
                if (emp.Gender.ToUpper() == "M")
                {
                    o1.Gender = "1";
                }
                else
                {
                    o1.Gender = "2";
                }
                objLTA.objLTAdtlcolls.Add(o1);
                //*********** Date Formatting 
                foreach (DataRow dr in dt.Rows)
                {
                    string strName = dr["Firstname"].ToString() + " " + dr["Lastname"].ToString();
                    ReceiptDtl_LTACollection o = new ReceiptDtl_LTACollection();
                    o.Name = strName;
                    o.Relation = dr["nameofmembertype"].ToString();
                    TimeSpan ts = DateTime.Now.Date.Subtract(DateTime.ParseExact(dr["dateofbirth"].ToString(), "yyyy-MM-dd", null));
                    o.Age = (ts.Days / 365).ToString();
                    o.Gender = dr["Gender"].ToString();


                    objLTA.objLTAdtlcolls.Add(o);
                    //string vaildfromdate, vaildenddate;
                    //vaildfromdate = dr[5].ToString();
                    //vaildfromdate = vaildfromdate.Substring(6, 2) + "." + vaildfromdate.Substring(4, 2) + "." + vaildfromdate.Substring(0, 4);
                    //vaildenddate = dr[4].ToString();
                    //vaildenddate = vaildenddate.Substring(6, 2) + "." + vaildenddate.Substring(4, 2) + "." + vaildenddate.Substring(0, 4);

                    //if(DateTime.ParseExact(vaildfromdate,"yyyyMMdd",null) 
                }
                DataTable dt1 = objtax.GetEmpUserPeriodDetail(_sessionService.Get<string>("userID"), "1", strKiId);
                if (dt1.Rows.Count > 0)
                {
                    objLTA.TripNo = (Convert.ToInt16(dt1.Rows[0]["LTACOUNT"].ToString()) + 1).ToString();

                }
            }




            
            HttpContext.Session.SetObject<ReceiptDtl_LTAMain>("LTADEC", objLTA);
            return objLTA;
        }

        private string getfareeligibility(string Typ, string gender, string age, string normaltrainfare)
        {
            DataTable dt = new DataTable();
            string result = string.Empty;
            if (Typ == "1")
            {
                dt = objtax.GetTrainFareEligPerAge(gender, age, normaltrainfare);
                if (dt.Rows.Count > 0)
                {
                    double amt = Convert.ToDouble(dt.Rows[0]["EligableFare"].ToString().Trim());
                    Int64 eligAmt = (Int64)amt;
                    // result = dt.Rows[0]["EligableFare"].ToString().Trim();
                    result = eligAmt.ToString();

                }
            }
            return result;

        }
        private Tuple<bool, string[]> UpdateCheckReceiptAndSum(List<ReceiptDtl_586> arr_listLIC)
        {
            var hdnCalclic = "0";
            var Error = "";
            bool check = false;
            try
            {
                if (arr_listLIC != null)
                {
                    List<ReceiptDtl_586> List = new List<ReceiptDtl_586>();
                    foreach (ReceiptDtl_586 recipt in arr_listLIC)
                    {
                        List.Add(recipt);
                    }
                    var Policy = (from ReceiptDtl_586 res in List.Distinct() select res._PolicyNo).ToList();
                    var t = (from p in Policy.Distinct(StringComparer.CurrentCultureIgnoreCase) select p).ToList();

                    if (t.Count > 0)
                    {
                        if (t.Count > 0)
                        {
                            for (int i = 0; i < t.Count; i++)
                            {
                                Int64 amount = 0;
                                var sum = ((from ReceiptDtl_586 res in List where res._PolicyNo == t[i] select res.Amount).ToList());

                                var sumAssurAmount = ((from ReceiptDtl_586 res in List where res._PolicyNo == t[i] select res._SumAssured).ToList());
                                var CheckSumAssured = (from s in sumAssurAmount.Distinct() select s).ToList();

                                var policydate = ((from ReceiptDtl_586 res in List where res._PolicyNo == t[i] select res._Date).ToList());
                                var checkPolicyDate = (from pd in policydate.Distinct(StringComparer.CurrentCultureIgnoreCase) select pd).ToList();

                                var ReceiptContainer = (from ReceiptDtl_586 receipt in List where receipt._PolicyNo == t[i] select receipt._ReceiptNo).ToList();
                                var checkReciept = (from r in ReceiptContainer.Distinct(StringComparer.CurrentCultureIgnoreCase) select r).ToList();

                                if (CheckSumAssured.Count != 1)
                                {
                                    Error = "SA";
                                }
                                else if (checkPolicyDate.Count != 1)
                                {
                                    Error = "PD";
                                }
                                else if (ReceiptContainer.Count != checkReciept.Count)
                                {
                                    Error = "RD";
                                }
                                if (Error == string.Empty)
                                {
                                    for (int j = 0; j < sum.Count; j++)
                                    {
                                        amount = amount + Convert.ToInt64(sum[j]);
                                    }
                                    check = true;
                                }
                                else
                                {
                                    check = false;
                                }
                                if (check == false)
                                {
                                    break;
                                }
                                else
                                {
                                    string PubAmount = string.Empty;
                                    if (Convert.ToDateTime(policydate[0]) < (new DateTime(2012, 4, 1)) && check == true)
                                    {
                                        PubAmount = Convert.ToString(Math.Ceiling(.2 * Convert.ToInt32(sumAssurAmount[0])));
                                    }
                                    else if (Convert.ToDateTime(policydate[0]) >= (new DateTime(2012, 4, 1)) && check == true)
                                    {
                                        PubAmount = Convert.ToString(Math.Ceiling(.1 * Convert.ToInt32(sumAssurAmount[0])));
                                    }
                                    if (Convert.ToInt32(PubAmount) > Convert.ToInt32(amount))
                                    {
                                        PubAmount = Convert.ToString(amount);
                                    }
                                    hdnCalclic = Convert.ToString(Convert.ToInt32(hdnCalclic) + Convert.ToInt32(PubAmount));

                                }
                            }
                        }
                        else
                        {
                            check = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new Tuple<bool, string[]>(check, new string[2] { hdnCalclic, Error });
        }

        public Tuple<bool, string[]> UpdateCheckReceiptAndSumULIP(List<ReceiptDtl_586> arr_listULIP)
        {
            var hdnCalcULIP = "0";
            var Error = "";
            bool check = false;
            try
            {
                if (arr_listULIP != null)
                {
                    List<ReceiptDtl_586> List = new List<ReceiptDtl_586>();
                    foreach (ReceiptDtl_586 recipt in arr_listULIP)
                    {
                        List.Add(recipt);
                    }
                    var Policy = (from ReceiptDtl_586 res in List.Distinct() select res._PolicyNo).ToList();
                    var t = (from p in Policy.Distinct(StringComparer.CurrentCultureIgnoreCase) select p).ToList();

                    if (t.Count > 0)
                    {
                        if (t.Count > 0)
                        {
                            for (int i = 0; i < t.Count; i++)
                            {
                                Int64 amount = 0;
                                var sum = ((from ReceiptDtl_586 res in List where res._PolicyNo == t[i] select res.Amount).ToList());

                                var sumAssurAmount = ((from ReceiptDtl_586 res in List where res._PolicyNo == t[i] select res._SumAssured).ToList());
                                var CheckSumAssured = (from s in sumAssurAmount.Distinct() select s).ToList();

                                var policydate = ((from ReceiptDtl_586 res in List where res._PolicyNo == t[i] select res._Date).ToList());
                                var checkPolicyDate = (from pd in policydate.Distinct(StringComparer.CurrentCultureIgnoreCase) select pd).ToList();

                                var ReceiptContainer = (from ReceiptDtl_586 receipt in List where receipt._PolicyNo == t[i] select receipt._ReceiptNo).ToList();
                                var checkReciept = (from r in ReceiptContainer.Distinct(StringComparer.CurrentCultureIgnoreCase) select r).ToList();

                                if (CheckSumAssured.Count != 1)
                                {
                                    Error = "SA";
                                }
                                else if (checkPolicyDate.Count != 1)
                                {
                                    Error = "PD";
                                }
                                else if (ReceiptContainer.Count != checkReciept.Count)
                                {
                                    Error = "RD";
                                }
                                if (Error == string.Empty)
                                {
                                    for (int j = 0; j < sum.Count; j++)
                                    {
                                        amount = amount + Convert.ToInt64(sum[j]);
                                    }
                                    check = true;
                                }
                                else
                                {
                                    check = false;
                                }
                                if (check == false)
                                {
                                    break;
                                }
                                else
                                {
                                    string PubAmount = string.Empty;
                                    if (Convert.ToDateTime(policydate[0]) < (new DateTime(2012, 4, 1)) && check == true)
                                    {
                                        PubAmount = Convert.ToString(Math.Ceiling(.2 * Convert.ToInt32(sumAssurAmount[0])));
                                    }
                                    else if (Convert.ToDateTime(policydate[0]) >= (new DateTime(2012, 4, 1)) && check == true)
                                    {
                                        PubAmount = Convert.ToString(Math.Ceiling(.1 * Convert.ToInt32(sumAssurAmount[0])));
                                    }
                                    if (Convert.ToInt32(PubAmount) > Convert.ToInt32(amount))
                                    {
                                        PubAmount = Convert.ToString(amount);
                                    }
                                    hdnCalcULIP = Convert.ToString(Convert.ToInt32(hdnCalcULIP) + Convert.ToInt32(PubAmount));

                                }
                            }
                        }
                        else
                        {
                            check = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new Tuple<bool, string[]>(check, new string[2] { hdnCalcULIP, Error });
        }

        private EmployerB12ViewModel B12Form()
        {
            EmployerB12ViewModel data = new EmployerB12ViewModel();
            try
            {
                data.Name = LoginEmpDetails.Employee_Name;
                data.PAN = LoginEmpDetails.PancardNo;
                data.Resident = "Resident";

                data.B12List = new List<EmployerB12ListDetail>();

                DataTable dt = objtax.Get12BDtl(LoginEmpDetails.Employee_Code, _objIPMS.GetKIId(), "1");
                if (dt.Rows.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow dr in dt.Rows)
                    {
                        EmployerB12ListDetail o = new EmployerB12ListDetail();
                        data.Address = dr["EMPADDRESS"].ToString();
                        data.Resident = dr["RES_STATUS"].ToString();

                        o.EmployerAddress = dr["EMPLOYER_ADD"].ToString();
                        o.ID = dr["FIN_TAX12BDTLID"].ToString();
                        o.SLNO = i.ToString();
                        o.EmployerName = dr["EMPLOYER_NAME"].ToString();
                        o.EmployerPan = dr["PAN"].ToString();
                        o.FromDate = dr["PERIOD_FROM"].ToString();
                        o.ToDate = dr["PERIOD_TO"].ToString();
                        o.EmployerTan = dr["TAN"].ToString();
                        o.TotalAmtTax = dr["TOTALTAXAMT"].ToString();
                        o.Deductions = dr["TOTAL_DED"].ToString();
                        o.TotalExempt = dr["TOTAL_EXEMP"].ToString();
                        o.TotalGross = dr["TOTAL_GROSS"].ToString();
                        o.Perk = dr["TOTAL_PERK"].ToString();
                        data.B12List.Add(o);
                        i++;
                    }
                }
                else
                {
                    DataSet ds = _objSearchEmp.OfficialDetail(_sessionService.Get<string>("userID"));
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        data.Address = ds.Tables[0].Rows[0]["TADDRESS1"].ToString() + " " + ds.Tables[0].Rows[0]["TADDRESS2"].ToString() + " " + ds.Tables[0].Rows[0]["TCITY"].ToString() + " " + ds.Tables[0].Rows[0]["TSTATE"].ToString();
                    }

                }

            }catch(Exception ex)
            {

            }
            
           
            return data;

        }

    }
}

