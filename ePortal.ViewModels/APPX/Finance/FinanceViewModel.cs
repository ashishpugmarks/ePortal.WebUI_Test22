using ePortal.DomainClasses;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Finance
{
    public class FinanceViewModel
    {
        public string PERIOD { get; set; } = "Not Set";

        public List<FinancePreviousYearDetail> FinancePrvYearDetail { get; set; }
        public string CurrProjImg { get; set; }
        public string CurrProjLink { get; set; }
        public string CurrAccImg { get; set; }
        public string CurrAccLink { get; set; }
        public string TAXREGIM { get; set; }
        public string ProStatus { get; set; }

    }
    public class FinancePreviousYearDetail
    {
        public string FINANCIALYEAR { get; set; }
        public string TXDCLTYPE { get; set; }
        public string TAXREGIME { get; set; }
        public string SYKIID { get; set; }
    }
    public class EMPTAX_INVDEC
    {
        public string FINANCIALYEAR { get; set; }
        public string ProjStatus { get; set; }
        public bool print { get; set; } = true;
        public bool B12Enabled { get; set; } = true;
        public string B12Msg { get; set; }
        public IEnumerable<SelectListItem>? KII { get; set; }
        public string KIIID { get; set; }
        public string PANNUMBER { get; set; }
        public string MOBILENO { get; set; }
        public string txtrentamount1 { get; set; }
        // Period
        public string FromDate { get; set; }  // use DateTime if you prefer
        public string ToDate { get; set; }
        public string? HRClaim { get; set; }
        // Accommodation Type
        public int? SelectedAccType { get; set; }
        public IEnumerable<SelectListItem> AccTypeList { get; set; }
        //Tax Type
        public string TaxType { get; set; }
        // HRA Tax Exempt
        public bool IsTaxExempt { get; set; }

        // Monthly Rent
        public string MonthlyRentAmount { get; set; }

        // Residential Address
        public string ResidentialAddress { get; set; }

        // City Category
        public int? SelectedCityCategory { get; set; }
        public IEnumerable<SelectListItem> CityCategoryList { get; set; }

        // Annual House Rent Paid
        public string? AnnualRent { get; set; }
        public string? Status { get; set; }
        public string? MSG { get; set; }
        public string? ISEDITABLE { get; set; }
        public string? ISEDITABLEACTUAL { get; set; }
        public string? LTACOUNT { get; set; }

        public List<TaxInvoiceDetails> Taxinv580 { get; set; }
        public List<TaxInvoiceDetails> Taxinv582 { get; set; }

        public List<TaxInvoiceDetails> Taxinv584 { get; set; }
        public List<TaxInvoiceDetails> Taxinv584Sub2 { get; set; }
        public string total584sub2 { get; set; }
        public List<TaxInvoiceDetails> Taxinv585 { get; set; }
        public string total585_proj { get; set; }
        public string total585_act { get; set; }
        public List<TaxInvoiceDetails> Taxinv586 { get; set; }
        public string total586_proj { get; set; }
        public string total586_act { get; set; }
        public List<ReceiptIndexViewModel> ReceiptIndexViewModel { get; set; }
        public HRADetailList HRADetaillist { get; set; }
        public LoanApplicationViewModel LoanApplicationViewModel { get; set; }

        public ReceiptDtl_LTAMain LTA { get; set; }
        public EEDetailsModels EE { get; set; }
        public EmployerB12ViewModel B12 { get; set; }

    }

    public class TaxInvoiceDetails
    {

        public string TAXHEADID { get; set; }
        public string TAXHEAD { get; set; }
        public string TAXHEADABBR { get; set; }
        public string DESCRIPTION { get; set; }
        public string MAXLIMIT { get; set; }
        public string ISMANDATORY { get; set; }
        public string STATUS { get; set; }
        public string ACTIVE { get; set; }
        public string EMPTAXINVDECID { get; set; }
        public string PROJ_AMOUNT { get; set; }
        public string ISEDITABLE { get; set; }
        public string ISEDITABLEACTUAL { get; set; }
        public string SYKIID { get; set; }
        public string TAXHEADOPTID { get; set; }
        public string ACT_AMOUNT { get; set; }
        public string ADEMPCODE { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string OptionId { get; set; }
        public IEnumerable<SelectListItem>? OptionList { get; set; }

    }
    public class HRADetailList
    {
        public HRADetails hRADetails { get; set; }
        public List<HRADetails> hRAList { get; set; }

    }
    public class HRADetails
    {
        public string ActionType { get; set; }
        public int? strgridindex { get; set; }
        public int? SLNO { get; set; }
        public int? _strFINEMPRENTDTLID { get; set; }
        public string? TotalRentAmount { get; set; }
        public IEnumerable<SelectListItem> StartMonthList { get; set; }
        public IEnumerable<SelectListItem> EndMonthList { get; set; }
        public string? FinPeriodStartDate { get; set; }
        public string? FinPeriodEndDate { get; set; }
        public string ISLANPAN { get; set; }

        // Period From / To (month numbers 1..12)
        [Required(ErrorMessage = "Please select Period From month.")]
        public int? PeriodFromMonth { get; set; }

        [Required(ErrorMessage = "Please select Period To month.")]
        public int? PeriodToMonth { get; set; }

        public IEnumerable<SelectListItem> FromMonthOptions { get; set; }
        public IEnumerable<SelectListItem> ToMonthOptions { get; set; }
        // Monthly House Rent
        [Required(ErrorMessage = "Monthly rent is required.")]
        [Range(0, 99999999, ErrorMessage = "Monthly rent must be a non-negative number up to 8 digits.")]
        public string? MonthlyRentAmount { get; set; }

        // Present Residential Address
        [Required(ErrorMessage = "Present residential address is required.")]
        [MaxLength(250)]
        public string PresentResidentialAddress { get; set; }

        // City category
        [Required(ErrorMessage = "Please select Location of Rented House.")]
        public string? CityCategory { get; set; }

        // Employee S/O D/O
        [Required(ErrorMessage = "Please enter Employee S/O D/O.")]
        [MaxLength(100)]
        public string EmployeeSO { get; set; }

        [Display(Name = "Check in case of multiple landlord on same address")]
        public bool MultipleLandlord { get; set; }
        public bool CheckBox0 { get; set; }
        public bool CheckBox1 { get; set; }
        public bool CheckBox2 { get; set; }
        public bool CheckBox3 { get; set; }
        public LandlordDeclarationViewModel LandLord1 { get; set; }
        public LandlordDeclarationViewModel LandLord2 { get; set; }
        public LandlordDeclarationViewModel LandLord3 { get; set; }
        public LandlordDeclarationViewModel LandLord4 { get; set; }
    }

    public class LandlordDeclarationViewModel
    {
        // Controls the visibility of the panel (maps from Panel.Visible)
        public bool ShowLandlordPanel { get; set; } = false;

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Landlord name is required.")]
        [MaxLength(100)]
        public string LandlordName { get; set; } = string.Empty;

        [Display(Name = "PAN")]
        [Required(ErrorMessage = "PAN is required.")]
        [RegularExpression(@"[A-Za-z]{5}\d{4}[A-Za-z]{1}", ErrorMessage = "Invalid PAN format.")]
        [MaxLength(10)]
        public string LandlordPAN { get; set; } = string.Empty;

        [Display(Name = "S/O D/O")]
        [Required(ErrorMessage = "S/O D/O is required.")]
        [MaxLength(100)]
        public string LandlordSO { get; set; } = string.Empty;

        [Display(Name = "Flat No./Building/ Village")]
        [Required(ErrorMessage = "Address (Flat/Building/Village) is required.")]
        [MaxLength(100)]
        public string LandlordResHNo { get; set; } = string.Empty;

        [Display(Name = "Road/Street/Lane/Post Office")]
        [Required(ErrorMessage = "Road/Street/Lane/Post Office is required.")]
        [MaxLength(100)]
        public string LandlordResGNo { get; set; } = string.Empty;

        [Display(Name = "Area/Locality/Sub-Division")]
        [Required(ErrorMessage = "Area/Locality/Sub-Division is required.")]
        [MaxLength(100)]
        public string LandlordResVillage { get; set; } = string.Empty;

        [Display(Name = "Town/City/District")]
        [Required(ErrorMessage = "Town/City/District is required.")]
        [MaxLength(100)]
        public string LandlordResCity { get; set; } = string.Empty;

        [Display(Name = "Pincode")]
        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits.")]
        public string LandlordResPincode { get; set; } = string.Empty;

        [Display(Name = "Mobile No")]
        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string LandlordMobileNo { get; set; } = string.Empty;

        // Checkbox: multiple landlord at the same address


    }


    public class ReceiptIndexViewModel
    {
        public string? TaxHeadName { get; set; }  // replaces lbltaxheadnameTDS585
        public string? TaxId { get; set; }
        public List<ReceiptDtl_586>? Receipts { get; set; }

        // Inline "Add New" form model
        public ReceiptDtl_586? NewReceipt { get; set; }
        public string? Total { get; set; }
    }


    public class LoanApplicationViewModel
    {
        public string ID { get; set; } = string.Empty;
        public string IsUnderCons { get; set; } = string.Empty;
        public string IsEmployeeOwner { get; set; } = string.Empty;

        public string LoanTaken { get; set; } = string.Empty;
        public string ConsDate { get; set; } = string.Empty;
        public string ValueOfHouse { get; set; } = string.Empty;
        public string AnnualRent { get; set; } = string.Empty;
        public string MunicipalTax { get; set; } = string.Empty;
        public string PropertyOwner { get; set; } = string.Empty;

        public string CurrentYear { get; set; } = string.Empty;
        public string CurrInterest { get; set; } = string.Empty;
        public string TotalCurrInterest { get; set; } = string.Empty;
        public string Year1 { get; set; } = string.Empty;
        public string PreYear1 { get; set; } = string.Empty;
        public string TotalYear1 { get; set; } = string.Empty;
        public string Year2 { get; set; } = string.Empty;
        public string PreYear2 { get; set; } = string.Empty;
        public string TotalYear2 { get; set; } = string.Empty;
        public string Year3 { get; set; } = string.Empty;
        public string PreYear3 { get; set; } = string.Empty;
        public string TotalYear3 { get; set; } = string.Empty;

        public string Year4 { get; set; } = string.Empty;
        public string PreYear4 { get; set; } = string.Empty;
        public string TotalYear4 { get; set; } = string.Empty;

        public string Year5 { get; set; } = string.Empty;
        public string PreYear5 { get; set; } = string.Empty;
        public string TotalYear5 { get; set; } = string.Empty;
        public string IsJointLoan { get; set; } = string.Empty;

        public string? RelationshipId { get; set; } = string.Empty;// Spouse, Parent, Brother, Other

        public string? ShareInProperty { get; set; } = string.Empty;

        public string? ShareInTaxExemption { get; set; } = string.Empty;

        public string FinalTotalAmount { get; set; } = string.Empty;

        public string? SelfOccupancyFrom { get; set; } = string.Empty;

        public string? SelfOccupancyTo { get; set; } = string.Empty;

        public string HouseAddress { get; set; } = string.Empty;

        public string TaxId { get; set; } = string.Empty;
        public LoanProviderModel LoanProvider { get; set; }
        public List<LoanProviderModel> LoanProviders { get; set; }

        public string Total { get; set; } = string.Empty;
        public bool PreYear1Enabled { get; set; } = true;
        public bool PreYear2Enabled { get; set; } = true;
        public bool PreYear3Enabled { get; set; } = true;
        public bool PreYear4Enabled { get; set; } = true;
        public bool PreYear5Enabled { get; set; } = true;
        public bool PnlStopVisible { get; set; } = false;
        public bool PnlValidatedVisible { get; set; } = true;
        public string ProjLoanTaken { get; set; }
        public string ProjValueofHouse { get; set; }
        public string ProjIntrest { get; set; }
        
        public string LoanAmount { get; set; }
        public string LoanProviderName { get; set; }
        public string AddressLoanProvider { get; set; }
        public string PanLoanProvider { get; set; }

    }
    public class LoanProviderModel
    {
        public string Id { get; set; }
        public string SLNO { get; set; }
        public string LoanAmount { get; set; }
        public string LoanProvider { get; set; }
        public string AddressLoanProvider { get; set; }
        public string PanLoanProvider { get; set; }
    }
    public class ReceiptDtl_586
    {
        public int SLNO { get; set; }
        public string? _ID { get; set; }
        public string? _TaxHeadID { get; set; }
        public string? _PolicyNo { get; set; }
        public string? _Date { get; set; }
        public string? _SumAssured { get; set; }
        public string? _ReceiptNo { get; set; }
        public string? _PrmDate { get; set; }
        public string? Amount { get; set; }
        public string _PublishAmount { get; set; }
    }

    public class EmployerB12ViewModel
    {
        public string Name { get; set; }
        public string PAN { get; set; }
        public string Address { get; set; }
        public string Resident { get; set; }
        public EmployerB12ListDetail B12 { get; set; }
        public List<EmployerB12ListDetail> B12List { get; set; }
    }

    public class EmployerB12ListDetail
    {
        public string ID { get; set; }
        public string SLNO { get; set; }
        public string EmployerName { get; set; }
        public string EmployerAddress { get; set; }
        public string ResStatus { get; set; }

        public string EmployerTan { get; set; }
        public string EmployerPan { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? TotalGross { get; set; }
        public string? Perk { get; set; }
        public string? TotalExempt { get; set; }
        public string? Deductions { get; set; }
        public string? TaxableIncome { get; set; }
        public string? TotalAmtTax { get; set; }
    }
    [Serializable]
    public class ReceiptDtl_12BCollection
    {
        string strID = string.Empty;
        string strEmployerName = string.Empty;
        string strAddress = string.Empty;
        string strTAN = string.Empty;
        string strPAN = string.Empty;
        string strPeriodFrom = string.Empty;
        string strPeriodTo = string.Empty;
        string strTotGross = string.Empty;
        string strTotPerk = string.Empty;
        string strTotExemp = string.Empty;
        string strTotDed = string.Empty;
        string strTaxableInc = string.Empty;
        string strTotalAmtTax = string.Empty;
        string strRemark = string.Empty;

        public ReceiptDtl_12BCollection()
        { }

        public string _ID
        {
            get { return strID; }
            set { strID = value; }
        }
        public string _Name
        {
            get { return strEmployerName; }
            set { strEmployerName = value; }
        }
        public string _Address
        {
            get { return strAddress; }
            set { strAddress = value; }
        }
        public string _TAN
        {
            get { return strTAN; }
            set { strTAN = value; }
        }
        public string _PAN
        {
            get { return strPAN; }
            set { strPAN = value; }
        }
        public string _PeriodFrom
        {
            get { return strPeriodFrom; }
            set { strPeriodFrom = value; }
        }
        public string _PeriodTo
        {
            get { return strPeriodTo; }
            set { strPeriodTo = value; }
        }
        public string _TotalGross
        {
            get { return strTotGross; }
            set { strTotGross = value; }
        }
        public string _TotalPerk
        {
            get { return strTotPerk; }
            set { strTotPerk = value; }
        }
        public string _TotalExemption
        {
            get { return strTotExemp; }
            set { strTotExemp = value; }
        }
        public string _TotalDeduction
        {
            get { return strTotDed; }
            set { strTotDed = value; }
        }
        public string _TaxableIncome
        {
            get
            {
                strTaxableInc = (Convert.ToInt32(strTotGross) + Convert.ToInt64(strTotPerk) - Convert.ToInt64(strTotExemp) - Convert.ToInt64(strTotDed)).ToString();
                return strTaxableInc;

            }
        }
        public string _TotalAmtTax
        {
            get { return strTotalAmtTax; }
            set { strTotalAmtTax = value; }
        }
        public string _Remark
        {
            get { return strRemark; }
            set { strRemark = value; }
        }
    }
    [Serializable]
    public class ReceiptDtl_12BMain
    {
        string strEmpAddress = string.Empty;
        string strEmpPAN = string.Empty;
        string strEmpResstaus = string.Empty;
        List<ReceiptDtl_12BCollection> obj12bdtlcoll;
        public ReceiptDtl_12BMain()
        { }

        public string _EmpAddress
        {
            get { return strEmpAddress; }
            set { strEmpAddress = value; }
        }
        public string _EmpPAN
        {
            get { return strEmpPAN; }
            set { strEmpPAN = value; }
        }
        public string _EmpResstaus
        {
            get { return strEmpResstaus; }
            set { strEmpResstaus = value; }
        }
        public List<ReceiptDtl_12BCollection> _12Bdtlcollection
        {
            get { return obj12bdtlcoll; }
            set { obj12bdtlcoll = value; }
        }
        public string _TotalGross
        {
            get
            {
                Int64 Totgross = 0;
                if (obj12bdtlcoll != null && obj12bdtlcoll.Count > 0)
                {
                    foreach (ReceiptDtl_12BCollection o in obj12bdtlcoll)
                    {
                        Totgross += Convert.ToInt64(o._TotalGross);
                    }
                }
                return Totgross.ToString();
            }
        }
        public string _TotalPerk
        {
            get
            {
                Int64 TotPerk = 0;
                if (obj12bdtlcoll != null && obj12bdtlcoll.Count > 0)
                {
                    foreach (ReceiptDtl_12BCollection o in obj12bdtlcoll)
                    {
                        TotPerk += Convert.ToInt64(o._TotalPerk);
                    }
                }
                return TotPerk.ToString();
            }
        }
        public string _TotalExemption
        {
            get
            {
                Int64 TotExemp = 0;
                if (obj12bdtlcoll != null && obj12bdtlcoll.Count > 0)
                {
                    foreach (ReceiptDtl_12BCollection o in obj12bdtlcoll)
                    {
                        TotExemp += Convert.ToInt64(o._TotalExemption);
                    }
                }
                return TotExemp.ToString();
            }
        }
        public string _TotalDeduction
        {
            get
            {
                Int64 TotDed = 0;
                if (obj12bdtlcoll != null && obj12bdtlcoll.Count > 0)
                {
                    foreach (ReceiptDtl_12BCollection o in obj12bdtlcoll)
                    {
                        TotDed += Convert.ToInt64(o._TotalDeduction);
                    }
                }
                return TotDed.ToString();
            }
        }
        public string _TotalAmtTax
        {
            get
            {
                Int64 TotAmtTax = 0;
                if (obj12bdtlcoll != null && obj12bdtlcoll.Count > 0)
                {
                    foreach (ReceiptDtl_12BCollection o in obj12bdtlcoll)
                    {
                        TotAmtTax += Convert.ToInt64(o._TotalAmtTax);
                    }
                }
                return TotAmtTax.ToString();
            }
        }
    }
    public class ReceiptDtl_LTACollection
    {

        public string ID { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Relation { get; set; } = string.Empty;

        public string Age { get; set; } = string.Empty;

        public string MOT { get; set; } = string.Empty;

        public string TktBillNo { get; set; } = string.Empty;

        public string Amount { get; set; } = string.Empty;

        public string TrainFare { get; set; } = string.Empty;

        public string ElilableAmt { get; set; } = string.Empty;

        public string IsTravel { get; set; } = string.Empty;


        public string Gender { get; set; } = string.Empty;

    }
    public class EEDetailsModels
    {
        public string ID { get; set; }
        public string TaxId { get; set; }
        public string Loansanctiondate { get; set; }
        public string Loansanctionamount { get; set; }
        public string Vofhouse { get; set; }
        public string Ownerofoterhhouse { get; set; }
        public string Ownerofproperty { get; set; }
        public string Popunderconstr { get; set; }
        public string Compofconstdate { get; set; }
        public string Amtclaimed24b { get; set; }
        public string Perofshareproperty { get; set; }

        public string Perofshareintax { get; set; }
        public string Declamount { get; set; }
        public string EEamount { get; set; }

    }

    public class ReceiptDtl_LTAMain
    {

        public List<ReceiptDtl_LTACollection> objLTAdtlcolls { get; set; }



        public string ID { get; set; } = string.Empty;

        public string Year { get; set; } = string.Empty;

        public string KI { get; set; } = string.Empty;

        public string LTAAmount { get; set; } = string.Empty;

        public string Placeofvisit { get; set; } = string.Empty;

        public string JFrom { get; set; } = string.Empty;

        //property to get date in dMyyyy format
        public string _JFromInSAPFormat { get; set; } = string.Empty;

        public string JTo { get; set; } = string.Empty;

        public string TripNo { get; set; } = string.Empty;

        public string JTOInSAPFormat { get; set; } = string.Empty;



        public string TotEligableAmt { get; set; } = string.Empty;


        public string MOTHeader { get; set; } = string.Empty;

        public string NormalTrainFare { get; set; } = string.Empty;


    }

    public class EmpTaxInvDecReportModel
    {
        public string ltl_Ecode { get; set; }
        public string ltl_LTAYear { get; set; }
        public string ltl_LTA_YEAR { get; set; }
        public string ltl_Name { get; set; }
        public string ltl_Designation { get; set; }
        public string ltl_DOJ { get; set; }
        public string ltl_Op_Dep { get; set; }
        public string ltl_PAN { get; set; }
        public string ltl586detail { get; set; }
        public string ltr586LICdetails { get; set; }
        public string ltr586ULIPdetails { get; set; }
        public string ltl_LTAAmount { get; set; }
        public string ltl_LTAPlace { get; set; }
        public string ltl_LTAFromDate { get; set; }
        public string ltl_LTAToDate { get; set; }
        public string ltl_LTATotal { get; set; }
        public List<RENTDetails> REPHOUSERENT { get; set; }
        public string txt_Address { get; set; }
        public string ltlsite { get; set; }
        public string txt_ContactNo { get; set; }
        public string strlname { get; set; }
        public List<ReceiptDtl_LTACollection> LTA { get; set; }
        public List<LTAModel> LTA1 { get; set; }
        public string ltltotexm { get; set; }
        public string ltlexmamt { get; set; }

        public List<LTAFamilyModel> LTA2 { get; set; }
        public string ltl_DC { get; set; }
        public string ltl_LoanDate { get; set; }
        public string ltl_RJL { get; set; }

        public string ltl_PropertyOwner { get; set; }
        public string ltl_Taxexemption { get; set; }
        public string ltl_PSP { get; set; }
        public string ltl_JL { get; set; }
        public string ltl_UC { get; set; }
        public string ltl_CurrentInterest { get; set; }
        public string ltl_PreYear1 { get; set; }
        public string ltl_PreYear2 { get; set; }
        public string ltl_PreYear3 { get; set; }
        public string ltl_PreYear4 { get; set; }
        public string ltl_PreYear5 { get; set; }
        public string ltrloanAmount { get; set; }
        public string ltrloanProvider { get; set; }
        public string ltrannualRent { get; set; }
        public string ltrmunicipalTax { get; set; }
        public string ltl_CurrentYear { get; set; }
        public string ltl_year1 { get; set; }
        public string ltl_year2 { get; set; }
        public string ltl_year3 { get; set; }
        public string ltl_year4 { get; set; }
        public string ltl_year5 { get; set; }
        public string ltl_TotalCurrentInterest { get; set; }
        public string ltl_TotalYear1 { get; set; }
        public string ltl_TotalYear2 { get; set; }
        public string ltl_TotalYear3 { get; set; }
        public string ltl_TotalYear4 { get; set; }
        public string ltl_TotalYear5 { get; set; }
        public string ltl_totalHouseLoan { get; set; }
        public string ltl_SelfOccupFromDate { get; set; }
        public string ltl_SelfOccupToDate { get; set; }
        public string ltl_HouseAddressLoan { get; set; }
        public string ltltotded24B { get; set; }
        public string ltl_loansanctiondate { get; set; }
        public string ltl_loansanctioned { get; set; }
        public string ltl_vofhouse { get; set; }
        public string ltl_ownerofoterhhouse { get; set; }
        public string ltl_ownerofproperty { get; set; }
        public string ltl_popunderconstr { get; set; }

        public string ltl_compofconstdate { get; set; }
        public string ltl_amtclaimed24b { get; set; }
        public string ltl_perofshareproperty { get; set; }
        public string ltl_perofshareintax { get; set; }
        public string ltl_Declamount { get; set; }
        public string ltr_Dedunction80EE { get; set; }
        public string ltl_AttireAllowAmount { get; set; }
        public string ltl_StandardDeduction { get; set; }
        public string ltl584sub2 { get; set; }
        public string ltlfinyear { get; set; }
        public string ltl_DeclPeriod { get; set; }
        public string ltlname { get; set; }
        public string ltlpan { get; set; }
        public string grd122Bview { get; set; }
        public string ltladdress { get; set; }
        public string ltlresstatus { get; set; }
        public string ltlfinyearstart { get; set; }
        public string reportType { get; set; } = string.Empty;
        public string BB12Report { get; set; } = string.Empty;
        public string B12Report { get; set; } = string.Empty;

        public string FinStartDate { get; set; } = string.Empty;
        public string FinEndDate { get; set; } = string.Empty;


    }
    public class EmpRent_NewFormPrintModel
    {
        public string FINEMPRENTDTLID { get; set; }
        public string ADEMPCODE { get; set; }
        public string SYKIID { get; set; }
        public string RENT_TO { get; set; }
        public string ACC_TYPE { get; set; }
        public string CITY_CAT { get; set; }
        public string ISTAXEXEMPTED { get; set; }
        public string HRACLAIM { get; set; }
        public string RENT_AMOUNT { get; set; }
        public string RENT_FROM { get; set; }
        public string TADDRESS { get; set; }
        public string TZIPCODE { get; set; }
        public string TMOBILE { get; set; }
        public string EMPSO { get; set; }
        public string LANDLORDNAME { get; set; }
        public string LANDLORDSO { get; set; }
        public string LANDLORDRESOF { get; set; }
        public string LLHNO { get; set; }
        public string LLGNO { get; set; }
        public string LLVILLAGE { get; set; }
        public string LLCITY { get; set; }
        public string LLPINECODE { get; set; }
        public string LANDLORDPAN { get; set; }
        public string ISLANPAN { get; set; }
        public string LANLORDMNO { get; set; }

        public string LANDLORDNAME2 { get; set; }
        public string LANDLORDSO2 { get; set; }
        public string LLHNO2 { get; set; }
        public string LLGNO2 { get; set; }
        public string LLVILLAGE2 { get; set; }
        public string LLCITY2 { get; set; }
        public string LLPINECODE2 { get; set; }
        public string LANDLORDPAN2 { get; set; }
        public string LANLORDMNO2 { get; set; }

        public string LANDLORDNAME3 { get; set; }
        public string LANDLORDSO3 { get; set; }
        public string LLHNO3 { get; set; }
        public string LLGNO3 { get; set; }
        public string LLVILLAGE3 { get; set; }
        public string LLCITY3 { get; set; }
        public string LLPINECODE3 { get; set; }
        public string LANDLORDPAN3 { get; set; }
        public string LANLORDMNO3 { get; set; }

        public string LANDLORDNAME4 { get; set; }
        public string LANDLORDSO4 { get; set; }
        public string LLHNO4 { get; set; }
        public string LLGNO4 { get; set; }
        public string LLVILLAGE4 { get; set; }
        public string LLCITY4 { get; set; }
        public string LLPINECODE4 { get; set; }
        public string LANDLORDPAN4 { get; set; }
        public string LANLORDMNO4 { get; set; }

        public string SITE { get; set; }
    }


    public class RENTDetails
    {
        public string RENT_AMOUNT { get; set; }
        public string citytype { get; set; }
        public string RENT_FROM { get; set; }
        public string RENT_TO { get; set; }
    }
    public class LoanDetail
    {
        public string _LoanAmount { get; set; }
        public string _LoanProvider { get; set; }
        public string _addressLoanProvider { get; set; }
        public string _panLoanProvider { get; set; }
        public string _AnnualRent { get; set; }
        public string _MunicipalTax { get; set; }

        public string _ID { get; set; }
        public string _IsUC { get; set; }
        public string _PropertyOwn { get; set; }
        public string _DateOfCons { get; set; }
        public string _DateOfLoantkn { get; set; }

        public string _CurrYear { get; set; }
        public string _CurrInterest { get; set; }

        public string _Year1 { get; set; }
        public string _Year1Interest { get; set; }
        public string _Year2 { get; set; }
        public string _Year2Interest { get; set; }
        public string _Year3 { get; set; }
        public string _Year3Interest { get; set; }
        public string _Year4 { get; set; }
        public string _Year4Interest { get; set; }
        public string _Year5 { get; set; }
        public string _Year5Interest { get; set; }

        public string _ShareInTaxExemp { get; set; }
        public string _IsJointLoan { get; set; }
        public string _JointRelation { get; set; }
        public string _SharePer { get; set; }

        public string _Selfoccupfrom { get; set; }
        public string _Selfoccupto { get; set; }
        public string _Addresshlt { get; set; }
        public string _Total { get; set; }
        public string _ValueOfHouse { get; set; }
        public string _OwnerOfOtherHouse { get; set; }
        public string _strownerofother { get; set; }
        public string _valueofhouse { get; set; }
    }

    [Serializable]
    public class PLoanDetail
    {
        string strID = string.Empty;
        string strIsUC = string.Empty;
        string strPropertyOwn = string.Empty;
        string InterestAmount = string.Empty;
        string loanAmount = string.Empty;
        string loanProvider = string.Empty;
        string addressloanProvider = string.Empty;
        string panloanProvider = string.Empty;
        string strDateOfLoantkn = string.Empty;
        string valueofhouse = string.Empty;



        public PLoanDetail()
        { }
        public string _InterestAmount
        {
            get { return InterestAmount; }
            set { InterestAmount = value; }
        }
        public string _LoanAmount
        {
            get { return loanAmount; }
            set { loanAmount = value; }
        }
        public string _LoanProvider
        {
            get { return loanProvider; }
            set { loanProvider = value; }
        }
        public string _addressLoanProvider
        {
            get { return addressloanProvider; }
            set { addressloanProvider = value; }
        }
        public string _panLoanProvider
        {
            get { return panloanProvider; }
            set { panloanProvider = value; }
        }
        public string _DateOfLoantkn
        {
            get { return strDateOfLoantkn; }
            set { strDateOfLoantkn = value; }
        }
        public string _valueofhouse
        {
            get { return valueofhouse; }
            set { valueofhouse = value; }
        }
        public string _ID
        {
            get { return strID; }
            set { strID = value; }
        }
        public string _IsUC
        {
            get { return strIsUC; }
            set { strIsUC = value; }
        }
        public string _PropertyOwn
        {
            get { return strPropertyOwn; }
            set { strPropertyOwn = value; }
        }

    }
    [Serializable]
    public class Rent_Detail
    {
        string strRentAmt = string.Empty;
        string strEmpso = string.Empty;
        string strLandlordName = string.Empty;
        string strLandlordso = string.Empty;
        // string strLandlordResof = string.Empty;
        string strLandlordResHno = string.Empty;
        string strLandlordResGno = string.Empty;
        string strLandlordResVillage = string.Empty;
        string strLandlordResCity = string.Empty;
        string strLandlordResPincode = string.Empty;
        string strLandlordPAN = string.Empty;
        string strEmpAddress = string.Empty;
        string strIsPANcard = string.Empty;
        string strLandlordmno = string.Empty;
        //---- Added by N.A
        string strMonthlyRentAmt = string.Empty;
        string strPeriodfromdate = string.Empty;
        string strPeriodTodate = string.Empty;
        string strResAddress = string.Empty;
        string strCityCat = string.Empty;

        string strLandlordName2 = string.Empty;
        string strLandlordso2 = string.Empty;
        string strLandlordResHno2 = string.Empty;
        string strLandlordResGno2 = string.Empty;
        string strLandlordResVillage2 = string.Empty;
        string strLandlordResCity2 = string.Empty;
        string strLandlordResPincode2 = string.Empty;
        string strLandlordmno2 = string.Empty;
        string strLandlordPAN2 = string.Empty;

        string strLandlordName3 = string.Empty;
        string strLandlordso3 = string.Empty;
        string strLandlordResHno3 = string.Empty;
        string strLandlordResGno3 = string.Empty;
        string strLandlordResVillage3 = string.Empty;
        string strLandlordResCity3 = string.Empty;
        string strLandlordResPincode3 = string.Empty;
        string strLandlordmno3 = string.Empty;
        string strLandlordPAN3 = string.Empty;

        string strLandlordName4 = string.Empty;
        string strLandlordso4 = string.Empty;
        string strLandlordResHno4 = string.Empty;
        string strLandlordResGno4 = string.Empty;
        string strLandlordResVillage4 = string.Empty;
        string strLandlordResCity4 = string.Empty;
        string strLandlordResPincode4 = string.Empty;
        string strLandlordmno4 = string.Empty;
        string strLandlordPAN4 = string.Empty;

        int strFINEMPRENTDTLID = 0;
        int strRentSum = 0;
        int strgridindex = 0;
        // List<HRA_Detail> objHRADetail;
        //---- end-------------
        public Rent_Detail()
        { }
        public string _RentAmount
        {
            get { return strRentAmt; }
            set { strRentAmt = value; }
        }
        public string _EmpSO
        {
            get { return strEmpso; }
            set { strEmpso = value; }
        }
        public string _LandlordName
        {
            get { return strLandlordName; }
            set { strLandlordName = value; }
        }
        public string _LandlordSO
        {
            get { return strLandlordso; }
            set { strLandlordso = value; }
        }
        public string _LandlordResHno
        {
            get { return strLandlordResHno; }
            set { strLandlordResHno = value; }
        }
        public string _LandlordResGno
        {
            get { return strLandlordResGno; }
            set { strLandlordResGno = value; }
        }
        public string _LandlordResVillage
        {
            get { return strLandlordResVillage; }
            set { strLandlordResVillage = value; }
        }
        public string _LandlordResCity
        {
            get { return strLandlordResCity; }
            set { strLandlordResCity = value; }
        }
        public string _LandlordResPincode
        {
            get { return strLandlordResPincode; }
            set { strLandlordResPincode = value; }
        }
        public string _Landlordmno
        {
            get { return strLandlordmno; }
            set { strLandlordmno = value; }
        }
        public string _LandlordPAN
        {
            get { return strLandlordPAN; }
            set { strLandlordPAN = value; }
        }

        public string _EmpAddress
        {
            get { return strEmpAddress; }
            set { strEmpAddress = value; }
        }
        public string _ISPANCARD
        {
            get { return strIsPANcard; }
            set { strIsPANcard = value; }
        }


        public string _MonthlyRentAmt
        {
            get { return strMonthlyRentAmt; }
            set { strMonthlyRentAmt = value; }
        }
        public string _Periodfromdate
        {
            get { return strPeriodfromdate; }
            set { strPeriodfromdate = value; }
        }
        //property to get date in dMyyyy format
        public string _PeriodfromdateFormat
        {
            get
            {
                if (strPeriodfromdate != "")
                {
                    return DateTime.ParseExact(strPeriodfromdate, "d-MMM-yyyy", null).ToString();
                }
                else
                    return "";

            }

        }
        public string _PeriodTodate
        {
            get { return strPeriodTodate; }
            set { strPeriodTodate = value; }
        }

        //property to get date in dMyyyy format
        public string _PeriodTodateFormat
        {
            get
            {
                if (strPeriodTodate != "")
                {
                    return DateTime.ParseExact(strPeriodTodate, "d-MMM-yyyy", null).ToString();
                }
                else
                    return "";

            }

        }
        public string _ResAddress
        {
            get { return strResAddress; }
            set { strResAddress = value; }
        }
        public string _CityCat
        {
            get { return strCityCat; }
            set { strCityCat = value; }
        }

        public string _LandlordName2
        {
            get { return strLandlordName2; }
            set { strLandlordName2 = value; }
        }
        public string _LandlordSO2
        {
            get { return strLandlordso2; }
            set { strLandlordso2 = value; }
        }
        public string _LandlordResHno2
        {
            get { return strLandlordResHno2; }
            set { strLandlordResHno2 = value; }
        }
        public string _LandlordResGno2
        {
            get { return strLandlordResGno2; }
            set { strLandlordResGno2 = value; }
        }
        public string _LandlordResVillage2
        {
            get { return strLandlordResVillage2; }
            set { strLandlordResVillage2 = value; }
        }
        public string _LandlordResCity2
        {
            get { return strLandlordResCity2; }
            set { strLandlordResCity2 = value; }
        }
        public string _LandlordResPincode2
        {
            get { return strLandlordResPincode2; }
            set { strLandlordResPincode2 = value; }
        }
        public string _Landlordmno2
        {
            get { return strLandlordmno2; }
            set { strLandlordmno2 = value; }
        }
        public string _LandlordPAN2
        {
            get { return strLandlordPAN2; }
            set { strLandlordPAN2 = value; }
        }

        public string _LandlordName3
        {
            get { return strLandlordName3; }
            set { strLandlordName3 = value; }
        }
        public string _LandlordSO3
        {
            get { return strLandlordso3; }
            set { strLandlordso3 = value; }
        }
        public string _LandlordResHno3
        {
            get { return strLandlordResHno3; }
            set { strLandlordResHno3 = value; }
        }
        public string _LandlordResGno3
        {
            get { return strLandlordResGno3; }
            set { strLandlordResGno3 = value; }
        }
        public string _LandlordResVillage3
        {
            get { return strLandlordResVillage3; }
            set { strLandlordResVillage3 = value; }
        }
        public string _LandlordResCity3
        {
            get { return strLandlordResCity3; }
            set { strLandlordResCity3 = value; }
        }
        public string _LandlordResPincode3
        {
            get { return strLandlordResPincode3; }
            set { strLandlordResPincode3 = value; }
        }
        public string _Landlordmno3
        {
            get { return strLandlordmno3; }
            set { strLandlordmno3 = value; }
        }
        public string _LandlordPAN3
        {
            get { return strLandlordPAN3; }
            set { strLandlordPAN3 = value; }
        }

        public string _LandlordName4
        {
            get { return strLandlordName4; }
            set { strLandlordName4 = value; }
        }
        public string _LandlordSO4
        {
            get { return strLandlordso4; }
            set { strLandlordso4 = value; }
        }
        public string _LandlordResHno4
        {
            get { return strLandlordResHno4; }
            set { strLandlordResHno4 = value; }
        }
        public string _LandlordResGno4
        {
            get { return strLandlordResGno4; }
            set { strLandlordResGno4 = value; }
        }
        public string _LandlordResVillage4
        {
            get { return strLandlordResVillage4; }
            set { strLandlordResVillage4 = value; }
        }
        public string _LandlordResCity4
        {
            get { return strLandlordResCity4; }
            set { strLandlordResCity4 = value; }
        }
        public string _LandlordResPincode4
        {
            get { return strLandlordResPincode4; }
            set { strLandlordResPincode4 = value; }
        }
        public string _Landlordmno4
        {
            get { return strLandlordmno4; }
            set { strLandlordmno4 = value; }
        }
        public string _LandlordPAN4
        {
            get { return strLandlordPAN4; }
            set { strLandlordPAN4 = value; }
        }

        public int _strgridindex
        {
            get { return strgridindex; }
            set { strgridindex = value; }
        }

        public int _strRentSum
        {
            get { return strRentSum; }
            set { strRentSum = value; }
        }
        public int _strFINEMPRENTDTLID
        {
            get { return strFINEMPRENTDTLID; }
            set { strFINEMPRENTDTLID = value; }
        }



    }

    [Serializable]
    public class Emp80EE_Detail
    {
        string strID = string.Empty;
        string strTaxHeadID = string.Empty;
        string strDateofLS = string.Empty;
        string strAmtofLS = string.Empty;
        string strTVRHP = string.Empty;
        string strOofothhouse = string.Empty;
        string strOofproperty = string.Empty;
        string strPunderconst = string.Empty;
        string strDateofcomp = string.Empty;
        string strInterset24B = string.Empty;
        string strPofsp = string.Empty;
        string strPofste = string.Empty;
        string strDeclAmount = string.Empty;
        string strAmount = string.Empty;

        public Emp80EE_Detail()
        { }

        public string _ID
        {
            get { return strID; }
            set { strID = value; }
        }
        public string _TaxHeadID
        {
            get { return strTaxHeadID; }
            set { strTaxHeadID = value; }
        }
        public string _DateofLS
        {
            get { return strDateofLS; }
            set { strDateofLS = value; }
        }
        public string _AmtofLS
        {
            get { return strAmtofLS; }
            set { strAmtofLS = value; }
        }
        public string _TVRHP
        {
            get { return strTVRHP; }
            set { strTVRHP = value; }
        }
        public string _Oofothhouse
        {
            get { return strOofothhouse; }
            set { strOofothhouse = value; }
        }
        public string _Oofproperty
        {
            get { return strOofproperty; }
            set { strOofproperty = value; }
        }
        public string _Punderconst
        {
            get { return strPunderconst; }
            set { strPunderconst = value; }
        }
        public string _Dateofcomp
        {
            get { return strDateofcomp; }
            set { strDateofcomp = value; }
        }
        public string _Interset24B
        {
            get { return strInterset24B; }
            set { strInterset24B = value; }
        }
        public string _Pofsp
        {
            get { return strPofsp; }
            set { strPofsp = value; }
        }
        public string _Pofste
        {
            get { return strPofste; }
            set { strPofste = value; }
        }
        public string _DeclAmount
        {
            get { return strDeclAmount; }
            set { strDeclAmount = value; }
        }
        public string Amount
        {
            get { return strAmount; }
            set { strAmount = value; }
        }

    }
    public class Emp80EEA_Detail
    {
        string strID = string.Empty;
        string strTaxHeadID = string.Empty;
        string strDateofLS = string.Empty;
        string strTVRHP = string.Empty;
        string strOofothhouse = string.Empty;
        string strOofproperty = string.Empty;
        string strInterset24B = string.Empty;
        string strPofsp = string.Empty;
        string strPofste = string.Empty;
        string strDeclAmount = string.Empty;
        string strAmount = string.Empty;

        public Emp80EEA_Detail()
        { }

        public string _ID
        {
            get { return strID; }
            set { strID = value; }
        }
        public string _TaxHeadID
        {
            get { return strTaxHeadID; }
            set { strTaxHeadID = value; }
        }
        public string _DateofLS
        {
            get { return strDateofLS; }
            set { strDateofLS = value; }
        }
        public string _TVRHP
        {
            get { return strTVRHP; }
            set { strTVRHP = value; }
        }
        public string _Oofothhouse
        {
            get { return strOofothhouse; }
            set { strOofothhouse = value; }
        }
        public string _Oofproperty
        {
            get { return strOofproperty; }
            set { strOofproperty = value; }
        }
        public string _Interset24B
        {
            get { return strInterset24B; }
            set { strInterset24B = value; }
        }
        public string _Pofsp
        {
            get { return strPofsp; }
            set { strPofsp = value; }
        }
        public string _Pofste
        {
            get { return strPofste; }
            set { strPofste = value; }
        }
        public string _DeclAmount
        {
            get { return strDeclAmount; }
            set { strDeclAmount = value; }
        }
        public string Amount
        {
            get { return strAmount; }
            set { strAmount = value; }
        }

    }

    [Serializable]
    public class HouseRent_Detail
    {

        string AmountOfLoan = string.Empty;
        string nameOfLoanProvider = string.Empty;
        string AddressOfLoanProvide = string.Empty;
        string PanOfLoanProvider = string.Empty;
        public HouseRent_Detail()
        { }
        public string LoanAmount
        {
            get { return AmountOfLoan; }
            set { AmountOfLoan = value; }
        }
        public string LoanProvider
        {
            get { return nameOfLoanProvider; }
            set { nameOfLoanProvider = value; }
        }

        public string addressLoanProvider
        {
            get { return AddressOfLoanProvide; }
            set { AddressOfLoanProvide = value; }
        }
        public string panLoanProvider
        {
            get { return PanOfLoanProvider; }
            set { PanOfLoanProvider = value; }
        }


    }

    public class LTAModel
    {
        public string SID { get; set; }
        public string SellerName { get; set; }
        public string GSTNo { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public decimal InvoiceAMT { get; set; }
        public string Goods_Service { get; set; }
        public int GSTRate { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }

    }
    [Serializable]
    public class LTAFamilyModel
    {
        public string FID { get; set; }
        public string Personname { get; set; }
        public string Relationship { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string ClaimExp { get; set; }
        public string Max_Eligible { get; set; }
        public string ExemptionAmount { get; set; }
        public decimal TotalAmount { get; set; }

    }

    public enum CityCategory
    {
        NonMetro = 0,
        Metro = 1
    }

    public class EmpTaxInvDec12BBReportModel
    {
        // ===== Header =====
        [Display(Name = "Name of the employee:")]
        public string lblname { get; set; }

        [Display(Name = "Address of the employee:")]
        public string lbladdress { get; set; }

        [Display(Name = "PAN of the employee:")]
        public string lblpannumber { get; set; }

        [Display(Name = "Financial year:")]
        public string lblfinancialyear { get; set; }

        // ===== 1. House Rent Allowance (HRA) =====
        // Shown as: @Model.RENT_FROM - @Model.RENT_TO
        [Display(Name = "(i) Rent paid for period (From)")]
        public string RENT_FROM { get; set; }

        [Display(Name = "(i) Rent paid for period (To)")]
        public string RENT_TO { get; set; }

        [Display(Name = "(ii) Rent paid to the landlord")]
        public decimal? RENT_AMOUNT { get; set; }

        [Display(Name = "(iii) Name of the landlord")]
        public string LANDLORDNAME { get; set; }

        // NOTE: In your cshtml, "(iv) Address of the landlord" displays LANDLORDPAN.
        // Keeping exact property name to avoid changing the view.
        [Display(Name = "(iv) Address of the landlord")]
        public string LANDLORDPAN { get; set; }

        // "(v) Permanent Account Number of the landlord" line prints: LLHNO LLGNO LLVILLAGE LLCITY
        // Again, keeping property names as used by the view.
        [Display(Name = "(v) Permanent Account Number of the landlord (House No)")]
        public string LLHNO { get; set; }

        [Display(Name = "(v) Permanent Account Number of the landlord (Gali No)")]
        public string LLGNO { get; set; }

        [Display(Name = "(v) Permanent Account Number of the landlord (Village)")]
        public string LLVILLAGE { get; set; }

        [Display(Name = "(v) Permanent Account Number of the landlord (City)")]
        public string LLCITY { get; set; }

        [Display(Name = "Evidence / particulars")]
        public string ltlldevidance { get; set; }

        // ===== 2. Leave Travel Concession (LTA) =====
        [Display(Name = "Leave travel concessions or assistance")]
        public string? ltl_LTAAmount { get; set; }

        [Display(Name = "Evidence / particulars")]
        public string ltlltaevidance { get; set; }

        // ===== 3. Deduction of interest on borrowing (House Loan) =====
        // Your view shows a literal for payable/paid; keeping a model property for it:
        [Display(Name = "(i) Interest payable/paid to the lender")]
        public string ltlhlpayable { get; set; } // use decimal? if you want numeric formatting

        // @foreach(var item in @Model.REPHOUSELOAN) { ... }
        public IEnumerable<HouseLoanItem> REPHOUSELOAN { get; set; } = new List<HouseLoanItem>();

        [Display(Name = "Evidence / particulars")]
        public string ltlhlevidance { get; set; }

        // ===== 4. Chapter VI-A Sections =====
        // @Model.Rep_section80c[i].TAXHEAD / ACT_AMOUNT
        public List<Section80Item> Rep_section80c { get; set; } = new List<Section80Item>();
        public List<HouseItem> HouseItem { get; set; } = new List<HouseItem>();

        // @Model.Rep_sectionother[i].TAXHEAD / ACT_AMOUNT
        public List<Section80Item> Rep_sectionother { get; set; } = new List<Section80Item>();

        [Display(Name = "Evidence / particulars")]
        public string ltlsec80cevidance { get; set; }

        // ===== Verification =====
        public string litverificationname { get; set; }
        public string litfathername { get; set; }
        public string litplace { get; set; }

        // Shown as plain string in the view; keep as string to avoid format issues
        public string litdate { get; set; }

        public string litdesignation { get; set; }
        public string litfulldate { get; set; }
    }

    // ===== Supporting Items =====

    public class HouseLoanItem
    {
        // Used in the view:
        // @item.LoanProvider, @item.addressLoanProvider, @item.panLoanProvider
        [Display(Name = "(ii) Name of the lender")]
        public string LoanProvider { get; set; }

        [Display(Name = "(iii) Address of the lender")]
        public string addressLoanProvider { get; set; }

        [Display(Name = "(iv) Permanent Account Number of the lender")]
        public string panLoanProvider { get; set; }
    }
    public class HouseItem
    {
        public string RENT_FROM { get; set; }
        public string RENT_TO { get; set; }
        public string RENT_AMOUNT { get; set; }
        public string LANDLORDNAME { get; set; }
        public string LANDLORDPAN { get; set; }
        public string LLHNO { get; set; }
        public string LLGNO { get; set; }
        public string LLVILLAGE { get; set; }
        public string LLCITY { get; set; }
    }

    public class Section80Item
    {
        // Used as: TAXHEAD and ACT_AMOUNT
        [Display(Name = "Nature of claim")]
        public string TAXHEAD { get; set; }

        [Display(Name = "Amount (Rs.)")]
        public string ACT_AMOUNT { get; set; }
    }
}




