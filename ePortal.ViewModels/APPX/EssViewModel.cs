using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ePortal.DomainClasses;
using Microsoft.AspNetCore.Http;
using System.Net;
namespace ePortal.ViewModels.APPX
{
    public class EssViewModel
    {

        public class TempFileModel
        {
            public string FileContents { get; set; }
            public string ContentType { get; set; }
            public string FileDownloadName { get; set; }
        }

        public class EmployeeAccountStatementViewModel
        {
            public string? FiscalYear { get; set; }
            public string? MonthFrom { get; set; }
            public string? MonthTo { get; set; }
            public string? Category { get; set; }
            public List<SelectListItem> FiscalYears { get; set; }
            public List<SelectListItem> Months { get; set; }
            public List<SelectListItem> Categories { get; set; }
            public bool ShowTravel { get; set; }
            public List<TravelRecord> TravelRecords { get; set; } = new();
            public bool HasTravelData => TravelRecords != null && TravelRecords.Any();
            //public List<TravelRecords> TravelList { get; set; }
            public List<TravelRecord> TicketList { get; set; } = new();
            public List<TravelRecord> ProductLoanList { get; set; } = new();
            public List<TravelRecord> MedicalLoanList { get; set; }
            public List<TravelRecord> ImprestList { get; set; } = new();
            public List<TravelRecord> SalaryAdvanceList { get; set; } = new();
            public int TravelRecordCount { get; set; }

        }

        [Serializable]
        public class TravelRecord
        {

            [DisplayName("STATUS")]
            public string? STATUS { get; set; }

            [DisplayName("PSTNG_DATE")]
            public string? PSTNG_DATE { get; set; }

            [DisplayName("ALLOC_NMBR")]
            public string? ALLOC_NMBR { get; set; }

            [DisplayName("DEBIT")]
            public string? DEBIT { get; set; }

            [DisplayName("CREDIT")]
            public string? CREDIT { get; set; }

            [DisplayName("CUMULATIVE_BALANCE")]
            public string? CUMULATIVE_BALANCE { get; set; }

            [DisplayName("PARTICULAR")]
            public string? PARTICULAR { get; set; }

            [DisplayName("ITEM_TEXT")]
            public string? ITEM_TEXT { get; set; }

        }
        public string FullName { get; set; }
        public string PersonalNumber { get; set; }
        public string SelectedAddressType { get; set; }
        public List<SelectListItem> AddressTypes { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Permanent residence" },
            new SelectListItem { Value = "2", Text = "Temporary residence" },
            new SelectListItem { Value = "5", Text = "Mailing Address" }
        }; 
        public bool ShowPersonalNumber { get; set; } = false;
        public List<AddressRecords> Addresses { get; set; } = new List<AddressRecords>();
        public class AddressRecords
        {
            public int Id { get; set; }
            public DateTime ValidBegin { get; set; }
            public DateTime? ValidEnd { get; set; }
            public string StreetAndHouseNo { get; set; }
            public string PostalCodeCity { get; set; }
            public string City { get; set; }
        }
        //Ended Address

        // Start Addressdetails view model
        public class AddressdetailsViewModel
        {
            public string FullName { get; set; }
            public string PersonalNumber { get; set; }
            public string Co { get; set; }
            public DateTime? ValidFrom { get; set; }
            public DateTime? ValidTo { get; set; }
            public string StreetNo { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public string District { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string TelephoneNo { get; set; }
            public string Subtype { get; set; }
            public string StreetAndHouseNo { get; set; }
        }

        // Ended Addressdetails view model

        /// <summary>
        /// Start AnnexureDetail view model
        /// </summary>

        //public class AnnexureDetailViewModel
        //{
            public string EmpCode { get; set; }
            public string EmpName { get; set; }
            public string Operation { get; set; }
            public string Division { get; set; }
            public string Department { get; set; }
            public string Designation { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string EffectiveDateText { get; set; }
            public string Description { get; set; }
            public decimal TotalSumA { get; set; }
            public decimal TotalPerquisitesB { get; set; }
            public decimal TotalSumC { get; set; }
            public string Amount { get; set; }
            public decimal TotalAmount { get; set; }
            public List<TFIComponentsItemsViewModel> TFIComponentsItems { get; set; }
            public List<PerquisitesItemsViewModel> PerquisitesItems { get; set; }
            public List<CompanyItemsViewModel> CompanyItems { get; set; }
        //}

        public class TFIComponentsItemsViewModel
        { 
            public string Description { get; set; } 
            public decimal Amount { get; set; }
        }
        public class PerquisitesItemsViewModel
        {
            public string Description { get; set; }
            public decimal Amount { get; set; }
        }
        public class CompanyItemsViewModel
        {
            public string Description { get; set; }
            public decimal Amount { get; set; }
        }
        /// <summary>
        /// Ended AnnexureDetail view model
        /// </summary>

        /// BankInformation View Model

        public List<BankInfoModel> BankInfo { get; set; } = new List<BankInfoModel>();
        public class BankInfoModel
        {
            public int Id { get; set; }
            public DateTime ValidBegin { get; set; }
            public DateTime? ValidEnd { get; set; }
            public string Payee { get; set; }
            public string BankName { get; set; }
            public string AccountNo { get; set; }
            public string Country { get; set; }
        }

        public class BankDetailsViewModel
        {
            public string FullName { get; set; }
            public string PersonnelNumber { get; set; }
            public string Payee { get; set; }
            public string City { get; set; }
            public string BankCountry { get; set; }
            public string PostalCode { get; set; }
            public string BankKey { get; set; }
            public string BankName { get; set; }
            public string AccountNo { get; set; }
            public string PaymentMode { get; set; }
            public string Method { get; set; }
            public string Currency { get; set; }
            public string PayeePostalCodeCity { get; set; }
            public string PayeePostalCodeCountry { get; set; }
            public int PaymentMethod { get; set; }
            public int PaymentMethodName { get; set; }
            public string PayeeCity { get; set; }
        }
             
        public class EmpAccStatementDto
        {
            public string Particular { get; set; }
            public string PostingDate { get; set; }
            public string AllocationNumber { get; set; }
            public string Debit { get; set; }
            public string Credit { get; set; }
            public string CumulativeBalance { get; set; }
            public string ItemText { get; set; }
            public string Status { get; set; }
        }

        // Form16detailsModel
        public decimal? SYKIID { get; set; }
        public string? FINANCIALYEAR { get; set; }
        //public IEnumerable<Form16details> dt_form16 { get; set; }
        public List<Form16details> dt_form16 { get; set; } = new();

        [Serializable]
        public class Form16details
        {

            [DisplayName("FILEPATH")]
            public string? FILEPATH { get; set; }

            [DisplayName("FILENAME")]
            public string? FILENAME { get; set; }

        }
        public string? FrameHtml { get; set; }

        /// <summary>
        /// SalaryAnnexureViewModel 
        /// </summary>
        //public class SalaryAnnexureViewModel
        //
       
        public int rowIndex { get; set; }
        public string KiId { get; set; }
        public List<SelectListItem> KiOptions { get; set; }
        public List<SalaryRow> SalaryRows { get; set; }
        public List<SalaryRow> SalaryData { get; set; }
        public List<SalaryRow> TFIRows { get; set; }
        public List<SalaryRow> PERKRows { get; set; }
        public List<SalaryRow> VARIRows { get; set; }
        //}

        public class SalaryRow
        {
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string ActionDesc { get; set; }
            public decimal TfiAmt { get; set; }
            public string BreakupType { get; set; }
        }
        public string SelectedMonthOption { get; set; }       
        
        public class RemunerationRequest
        {
            public int Seqno { get; set; }
            public DateTime PaymentDate { get; set; }            
            //public string Head { get; set; }      // Head
            //public string EmployeeName { get; set; }      // dr[0]
            //public string EmpCode { get; set; }      // dr[1]
            //public string Department { get; set; }        // dr[2]
            //public string Designation { get; set; }       // dr[3]
            //public string DOJ { get; set; }               // dr[4]
            //public string PAN { get; set; }               // dr[5]  
            //public string TaxRegime { get; set; }          // dr[6]
            //public string BankAccount { get; set; }       // dr[7]
            //public string PFNumber { get; set; }          // dr[8]
            //public string UAN { get; set; }               // dr[9]
            //public string ESICNumber { get; set; }        // dr[10]
            //public string Location { get; set; }          // dr[11]
            //public string Grade { get; set; }             // dr[12]
            //public string PayPeriod { get; set; }         // dr[13]
            //public string BasicSalary { get; set; }       // dr[14]
            //public string HRA { get; set; }               // dr[15]
            //public string Conveyance { get; set; }        // dr[16]
            //public string OtherAllowance { get; set; }    // dr[17]
            //public string GrossEarnings { get; set; }     // dr[18]
            //public string Deductions { get; set; }        // dr[19]
            //public string NetPay { get; set; }            // dr[20]
            //public string EmployerContribution { get; set; } // dr[21]
            //public string Remarks { get; set; }           // dr[22]
        }

        // RealtimeAccess_PunchReport model             
        public class RemunerationRecord
        {
            public int SequenceNumber { get; set; }
            public DateTime FpBegin { get; set; }
            public DateTime FpEnd { get; set; }
            public DateTime PayDate { get; set; }
            public string PayTypeText { get; set; }
        }       
        public class PunchReportViewModel
        {
            [Required(ErrorMessage = "From date is required")]
            [RegularExpression(@"[0-9]{2}/[0-1][0-9]/[0-9]{4}",
            ErrorMessage = "Please enter date in dd/mm/yyyy format, eg. 01/01/1800")]
            public string txtfrom { get; set; }

            [Required(ErrorMessage = "To date is required")]
            [RegularExpression(@"[0-9]{2}/[0-1][0-9]/[0-9]{4}",
                ErrorMessage = "Please enter date in dd/mm/yyyy format, eg. 01/01/1800")]
            public string txtto { get; set; }
            public string SelectedOption { get; set; } // "Period", "Current", "Previous"
            public List<RemunerationRecord> RemunerationResults { get; set; } = new();
            // Paging info
            public int PageIndex { get; set; }
            public int PageSize { get; set; } = 16;
            public int TotalCount { get; set; }
            public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
            public string FullName { get; set; }
            public string PersonnelNumber { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
            public DateTime FromDate { get; set; }
            public string ReportType { get; set; }
            public long cmbemp { get; set; }
            public List<SelectListItem> EmployeeList { get; set; } 
            public long TotalValue { get; set; }
            public long TotalAssociates { get; set; }
            public long AssociatePunch { get; set; }
            public bool ShowSummary { get; set; }
            public string NoteMessage { get; set; }

            public List<PunchReportViewModel> PunchReporRecords { get; set; }
            public string EMPNAME { get; set; }
            public string ADEMPCODE { get; set; }
            public DateTime? PUNCHINDATETIME { get; set; }
            public string EXPECTED_HALFDAYTIME { get; set; }
            public string PUNCHOUTDATETIME { get; set; }
            public string InOut { get; set; }
            public string LOCATION { get; set; }
            public bool ShowHalfDayColumn { get; set; }
            public bool ShowFullDayColumn { get; set; }
        }

        // HLISubsidyStatement View Model

        public class HLISubsidyStatementViewModel
        {

            public string rpOption { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string ErrorMessage { get; set; }
            public string Message { get; set; }

            [Required(ErrorMessage = "From date is required")]
            [RegularExpression(@"[0-9]{2}/[0-1][0-9]/[0-9]{4}",
             ErrorMessage = "Please enter date in dd/mm/yyyy format, eg. 01/01/1800")]
            public string txtfrom { get; set; } = "01/01/1800";

            [Required(ErrorMessage = "To date is required")]
            [RegularExpression(@"[0-9]{2}/[0-1][0-9]/[0-9]{4}",
            ErrorMessage = "Please enter date in dd/mm/yyyy format, eg. 01/01/1800")]
            public string txtto { get; set; }
            public List<HLISubsidyRecord> HLISubsidyRecords { get; set; }
        }
        public class HLISubsidyRecord
        {
            public string KONTRH { get; set; }
            public string NAME { get; set; }
            public string BU_SORT1 { get; set; }
            public string ZUOND { get; set; }
            public string LOANPROPADD { get; set; }
            public int REFER { get; set; }
            public DateTime DVALUT { get; set; }
            public string DVALUT1 { get; set; }
            public decimal PKOND { get; set; }
            public decimal BBASIS { get; set; }
            public decimal MI { get; set; }
            public decimal MP { get; set; }
            public decimal EMI { get; set; }
            public decimal PEB { get; set; }
            public decimal INT_SUB { get; set; }
        }
        public class familymemberRecord
        {
            public DateTime ValidBegin { get; set; }
            public DateTime ValidEnd { get; set; }
            public string FirstName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string MemberType { get; set; }
            public string SubType { get; set; }
            public int Objectid { get; set; }
        }
        public List<PreviousEmployerRecord> PreviousEmployers { get; set; }
        public class PreviousEmployerRecord
        {
            public DateTime ValidBegin { get; set; }
            public DateTime ValidEnd { get; set; }
            public string EmployerName { get; set; }
            public string City { get; set; }
            public string JobName { get; set; }
            public string IndustryName { get; set; }
        }

        /// <summary>
        /// RealtimeAttendanceReport View Model
        /// </summary>
        public class RealtimeAttendanceRecord
        {
            public string ADEmpCode { get; set; }
            public string EmpName { get; set; }
            public DateTime PunchDate { get; set; }
            public string PunchIn { get; set; }
            public string Location { get; set; }
        }
        public class RealtimeAttendanceViewModel
        {
            public string FullName { get; set; }
            public string PersonnelNumber { get; set; }
            public string FromDate { get; set; }
            public List<SelectListItem> EmployeeList { get; set; }
            public string TotalValue { get; set; }
            public int TotalAssociates { get; set; }
            public int AssociatePunch { get; set; }
            public bool ShowTotals { get; set; }
            public List<RealtimeAttendanceRecord> RealtimeAttendanceRecords { get; set; }
        }
        /// <summary>
        /// TaxworkingNew
        /// </summary>
        ///           
        public class TaxworkingNewViewModel
        {
            public string AssetYear { get; set; }
            public string TaxDate { get; set; }
            public string AfterSalaryMonth { get; set; }
            public string EmpCode { get; set; }
            public string EmpName { get; set; }
            public string PanNumber { get; set; }
            public string JoiningDate { get; set; }
            public string Operation { get; set; }
            //public string Department { get; set; }
            public string Designation { get; set; }
            //public string TaxRate { get; set; }
            public string FinancialYear { get; set; }
           
            //public string EmployeeCode { get; set; }
            //public string EmployeeName { get; set; }
            
            //public string Department { get; set; }
            ////public string Designation { get; set; }
            ////public DateTime JoiningDate { get; set; }
            public string TaxRegime { get; set; }
            //public string rincome_head { get; set; }
            //public decimal rincome_head_April { get; set; }

        
            public TaxworkingNewViewModel()
            {
                RegularIncomeTotal = new RegularIncomeViewModel();
                IrregularIncomeTotal = new IrregularIncomeViewModel();
                CEarningHead = new List<CEarningHeadViewModel>();                
                DeductionRecords = new List<DeductionRecordsViewModel>();
                DeductionTotalRecords = new List<DeductionRecordsViewModel>();
                DIncomeHead = new List<DEarningHeadViewModel>();
                DIncomeTotal = new DEarningHeadViewModel();
                TotalIncomeRecords = new List<TotalSalaryHeadViewModel>();
                PerkRecords = new List<PerkRecord>();
                //Deductions80C = new List<Deduction80CItem>();
                //Deductions80D = new List<Deduction80DItem>();
            }
            public List<RegularIncomeViewModel> RegularIncome { get; set; }
            public RegularIncomeViewModel RegularIncomeTotal { get; set; }     
            public List<IrregularIncomeViewModel> IrregularIncome { get; set; }
            public IrregularIncomeViewModel IrregularIncomeTotal { get; set; }
            public List<CEarningHeadViewModel> CEarningHead { get; set; }
            public List<DEarningHeadViewModel> DIncomeHead { get; set; }
            public DEarningHeadViewModel DIncomeTotal { get; set; }
            public List<TotalSalaryHeadViewModel> TotalIncomeRecords { get; set; }
            public List<DeductionRecordsViewModel> DeductionRecords { get; set; }
            public List<DeductionRecordsViewModel> DeductionTotalRecords { get; set; }
            public List<PerkRecord> PerkRecords { get; set; }
            //public List<Deduction80CItem> Deductions80C { get; set; }
            public List<Deduction80DItem> Deductions80D { get; set; }
            public List<DeductionHead> DeductionHeads { get; set; } = new();
            public DeductionHead DeductionTotal { get; set; }
            public List<Deduction80C> Deduction80C { get; set; } = new();
            public List<Deduction80D> Deduction80D { get; set; } = new();

            public string NoticePay { get; set; }
            public string TotalPCS { get; set; }
            public string TotalHMSISalary { get; set; }
            public string PreviousEmploymentSalary { get; set; }
            public string GrossSalary { get; set; }
            public string RentDetails { get; set; }
            public string RentLocation { get; set; }
            public string RentAmount { get; set; }
            public string HRAReceived { get; set; }
            public string RentBasic { get; set; }
            public string RentMetro { get; set; }
            public string FinalHRA { get; set; }
            public string UniformAllowance { get; set; }
            public string LTAAmount { get; set; }
            public string LeaveEncashment { get; set; }            
            public string TotalAnyOtherExemption { get; set; }
            public string TotalExemption { get; set; }
            public string EducationAllowance { get; set; }
            public string AttireAllowance { get; set; }
            public string TotalAnyExemption { get; set; }
            public string TotalExemptionSum { get; set; }
            public string TotalSalary { get; set; }

            public string StandardDeduction { get; set; }
            public string ProfessionalTaxDeduction { get; set; }
            public string EntertainmentAllowance { get; set; }
            public string TotalDeduction16 { get; set; }
            public string IncomeChargeable { get; set; }
            public string HouseProperty { get; set; }
            public string IncomeOtherSources { get; set; }
            public string TotalOtherIncome { get; set; }
            public string GrossTotal { get; set; }

            public string EmployeePF { get; set; }
            public string EmployerNPS { get; set; }
            public string TotalDeductionChapter6A { get; set; }
            public string TaxableIncome { get; set; }
            public string OneTimePayment { get; set; }
            public string TaxThereon { get; set; }
            public string TaxLiability { get; set; }
            public string Surcharge { get; set; }
            public string HealthCess { get; set; }
            public string TaxWithSurcharge { get; set; }

            public string TaxDedPE { get; set; }
            public string TaxDedOtherSources { get; set; }
            public string TaxDedHMSI { get; set; }
            public string YTDTax { get; set; }
            public string BalanceTax { get; set; }
            public string TaxCurrentMonth { get; set; }
            public string TaxLiabilityOneTime { get; set; }
            public string TaxProjectedMonth { get; set; }
            public decimal TotalPerks { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal Total { get; set; }
            //public decimal IrregularIncomeApril { get; set; }
            //public decimal IrregularIncomeMay { get; set; }
            //public decimal IrregularIncomeJune { get; set; }
            //public decimal IrregularIncomeJuly { get; set; }
            //public decimal IrregularIncomeAugust { get; set; }
            //public decimal IrregularIncomeSeptember { get; set; }
            //public decimal IrregularIncomeOctober { get; set; }
            //public decimal IrregularIncomeNovember { get; set; }
            //public decimal IrregularIncomeDecember { get; set; }
            //public decimal IrregularIncomeJanuary { get; set; }
            //public decimal IrregularIncomeFebruary { get; set; }
            //public decimal IrregularIncomeMarch { get; set; }
            //public decimal IrregularIncomeTotal { get; set; }
           
        }
        public class RegularIncomeViewModel
        {
            public string Pay_Head { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal Total { get; set; }
            public string dr_earn_head_0 { get; set; }
            public decimal dr_earn_head_1 { get; set; }
            public decimal dr_earn_head_2 { get; set; }
            public decimal dr_earn_head_3 { get; set; }
            public decimal dr_earn_head_4 { get; set; }
            public decimal dr_earn_head_5 { get; set; }
            public decimal dr_earn_head_6 { get; set; }
            public decimal dr_earn_head_7 { get; set; }
            public decimal dr_earn_head_8 { get; set; }
            public decimal dr_earn_head_9 { get; set; }
            public decimal dr_earn_head_10 { get; set; }
            public decimal dr_earn_head_11 { get; set; }
            public decimal dr_earn_head_12 { get; set; }
            public decimal dr_earn_head_13 { get; set; }
            public string rincome_head { get; set; }
            public decimal rincome_head_April { get; set; }
            public decimal rincome_head_May { get; set; }
            public decimal rincome_head_June { get; set; }
            public decimal rincome_head_July { get; set; }
            public decimal rincome_head_August { get; set; }
            public decimal rincome_head_September { get; set; }
            public decimal rincome_head_October { get; set; }
            public decimal rincome_head_November { get; set; }
            public decimal rincome_head_December { get; set; }
            public decimal rincome_head_January { get; set; }
            public decimal rincome_head_February { get; set; }
            public decimal rincome_head_March { get; set; }
            public decimal rincome_head_Total { get; set; }
        }
        public class IrregularIncomeViewModel
        {
            public string Pay_Head { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal Total { get; set; }
            public string irincome_head { get; set; }
            public decimal irincome_head_April { get; set; }
            public decimal irincome_head_May { get; set; }
            public decimal irincome_head_June { get; set; }
            public decimal irincome_head_July { get; set; }
            public decimal irincome_head_August { get; set; }
            public decimal irincome_head_September { get; set; }
            public decimal irincome_head_October { get; set; }
            public decimal irincome_head_November { get; set; }
            public decimal irincome_head_December { get; set; }
            public decimal irincome_head_January { get; set; }
            public decimal irincome_head_February { get; set; }
            public decimal irincome_head_March { get; set; }
            public decimal irincome_head_Total { get; set; }
            //public decimal total_salary { get; set; }
        }
        public class CEarningHeadViewModel
        {
            public string Pay_Head { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal Total { get; set; }
        }
        public class DEarningHeadViewModel
        {
            public string Pay_Head { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal Total { get; set; }
            public string dincome_head { get; set; }
            public decimal dincome_head_April { get; set; }
            public decimal dincome_head_May { get; set; }
            public decimal dincome_head_June { get; set; }
            public decimal dincome_head_July { get; set; }
            public decimal dincome_head_August { get; set; }
            public decimal dincome_head_September { get; set; }
            public decimal dincome_head_October { get; set; }
            public decimal dincome_head_November { get; set; }
            public decimal dincome_head_December { get; set; }
            public decimal dincome_head_January { get; set; }
            public decimal dincome_head_February { get; set; }
            public decimal dincome_head_March { get; set; }
            public decimal dincome_head_Total { get; set; }
        }
        public class TotalSalaryHeadViewModel
        {
            public string Pay_Head { get; set; }
            public string Name { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal Total { get; set; }
        }
        public class DeductionRecordsViewModel
        {
            public string Pay_Head { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal Total { get; set; }
            public decimal Header { get; set; }
            public decimal Proposed { get; set; }
            public decimal Actual { get; set; }
        }
        public class DeductionHead
        {
            public string PayHead { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal tot_dedu { get; set; }
        }
        public class Deduction80C
        {
            public string Header { get; set; }
            public decimal Proposed { get; set; }
            public decimal Actual { get; set; }
        }
        public class Deduction80D
        {
            //public string Name { get; set; }
            //public string Amount { get; set; }
            //public string Description { get; set; }
            public string Header { get; set; }
            public decimal Proposed { get; set; }
            public decimal Actual { get; set; }
        }
        //public class TaxReportViewModel
        //{
        //    public string TaxDate { get; set; }
        //    public string AssessmentMonth { get; set; }
        //    public string AssetYear { get; set; }
        //    public string EmpCode { get; set; }
        //    public string EmpName { get; set; }
        //    public string PAN { get; set; }
        //    public string JoiningDate { get; set; }
        //    public string Operation { get; set; }
        //    public string Designation { get; set; }
        //    public string TaxRate { get; set; }
        //    public string FinancialYear { get; set; }
        //}      
        
        //public class IncomeViewModel
        //{
        //    public List<IncomeRecord> IncomeRecords { get; set; }
        //}
        public class IncomeRowViewModel
        {
            public string IncomeHead { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal Total { get; set; }
        }

        //public class IncomeRecord
        //{
        //    public string Pay_Head { get; set; }
        //    public decimal April { get; set; }
        //    public decimal May { get; set; }
        //    public decimal June { get; set; }
        //    public decimal July { get; set; }
        //    public decimal August { get; set; }
        //    public decimal September { get; set; }
        //    public decimal October { get; set; }
        //    public decimal November { get; set; }
        //    public decimal December { get; set; }
        //    public decimal January { get; set; }
        //    public decimal February { get; set; }
        //    public decimal March { get; set; }
        //    public decimal Total { get; set; }
        //}

        //public class IncomeViewModel
        //{
        //    public List<IncomeRecord> IncomeRecords { get; set; }

        //    // Irregular income row
        //    public string IrregularIncomeHead { get; set; }
        //    public decimal IrregularIncomeApril { get; set; }
        //    public decimal IrregularIncomeMay { get; set; }
        //    public decimal IrregularIncomeJune { get; set; }
        //    public decimal IrregularIncomeJuly { get; set; }
        //    public decimal IrregularIncomeAugust { get; set; }
        //    public decimal IrregularIncomeSeptember { get; set; }
        //    public decimal IrregularIncomeOctober { get; set; }
        //    public decimal IrregularIncomeNovember { get; set; }
        //    public decimal IrregularIncomeDecember { get; set; }
        //    public decimal IrregularIncomeJanuary { get; set; }
        //    public decimal IrregularIncomeFebruary { get; set; }
        //    public decimal IrregularIncomeMarch { get; set; }
        //    public decimal IrregularIncomeTotal { get; set; }
        //}

        public class PerkRecord
        {
            public string Pay_Head { get; set; }
            public decimal April { get; set; }
            public decimal May { get; set; }
            public decimal June { get; set; }
            public decimal July { get; set; }
            public decimal August { get; set; }
            public decimal September { get; set; }
            public decimal October { get; set; }
            public decimal November { get; set; }
            public decimal December { get; set; }
            public decimal January { get; set; }
            public decimal February { get; set; }
            public decimal March { get; set; }
            public decimal Total { get; set; }
        }
        public class SalaryViewModel
        {
            public List<PerkRecord> PerkRecords { get; set; }
            public decimal TotalPerks { get; set; }
            public decimal NoticePay { get; set; }
        }
        //public class SalaryDetailsViewModel
        //{
            public decimal TotalPCS { get; set; }
            public decimal TotalHMSISalary { get; set; }
            public decimal PreviousEmploymentSalary { get; set; }
            public decimal GrossSalary { get; set; }
            public string RentDetails { get; set; }
            public string RentLocation { get; set; }
            public decimal RentAmount { get; set; }
            public decimal HRAReceived { get; set; }
            public decimal RentBasic { get; set; }
            public decimal RentMetro { get; set; }
            public decimal FinalHRA { get; set; }
            public decimal UniformAllowance { get; set; }
            public decimal LTAAmount { get; set; }
            public decimal LeaveEncashment { get; set; }
            public decimal TotalExemption { get; set; }
            public decimal EducationAllowance { get; set; }
            public decimal AttireAllowance { get; set; }
            public decimal TotalAnyOtherExemption { get; set; }
        //}
        public class Deduction80CItem
        {
            public string Header { get; set; }
            public decimal Proposed { get; set; }
            public decimal Actual { get; set; }
        }
        public class SalaryComputationViewModel
        {
            public decimal TotalExemption { get; set; }
            public decimal TotalSalary { get; set; }
            public decimal StandardDeduction { get; set; }
            public decimal ProfessionalTaxDeduction { get; set; }
            public decimal EntertainmentAllowance { get; set; }
            public decimal TotalDeduction16 { get; set; }
            public decimal IncomeChargeable { get; set; }
            public decimal HousePropertyIncome { get; set; }
            public decimal OtherSourcesIncome { get; set; }
            public decimal TotalOtherIncome { get; set; }
            public decimal GrossTotalIncome { get; set; }
            public decimal EmployeePF { get; set; }
            public decimal EmployerNPS { get; set; }
        }
        public List<Deduction80CItem> Deductions80C { get; set; }
        public class Deduction80DItem
        {
            public string Header { get; set; }
            public decimal Proposed { get; set; }
            public decimal Actual { get; set; }
        }
        public class TaxComputationViewModel
        {
            public List<Deduction80DItem> Deductions80D { get; set; }
            public decimal TotalDeductionChapter6A { get; set; }
            public decimal TaxableIncome { get; set; }
            public decimal OneTimePayment { get; set; }
        }
    // OverstayReport view model       
        public class OverstayReportViewModel
        {
            public string FullName { get; set; }
            public string PersonnelNumber { get; set; }
            public string cmbmonth { get; set; }
            public int cmbyear { get; set; }
            public string cmbemp { get; set; }
            public string ReportOption { get; set; }

            public DateTime CurrDate { get; set; }
            public DateTime ValidationDate { get; set; }
            public string Message { get; set; }
            public SelectList MonthList { get; set; }
            //public List<SelectListItem> MonthList { get; set; } = new List<SelectListItem>();
            public List<SelectListItem> YearsList { get; set; } = new List<SelectListItem>();
            public List<SelectListItem> EmpList { get; set; } = new List<SelectListItem>();
            public List<OverstayReportRecords> OverstayRecords { get; set; }           
        }
        public class OverstayReportRecords
        {
            public string Emp_Ldate { get; set; }
            public string Emp_In_Time { get; set; }
            public string Emp_Out_Time { get; set; }
            public string Emp_Shift { get; set; }
            public string Emp_App_Overstay { get; set; }
            public string Emp_Act_Overstay { get; set; }
            public string Emp_Prst_Remark { get; set; }
            public string Emp_Uabs_Remark { get; set; }
        }
        // Ended
        // EmployeeBlockProcess View Model 
        public class EmployeeBlockProcessModel
            {
                public string SelectedOption { get; set; }
                public string EmpCode { get; set; }
                public string BlockReason { get; set; }
                public bool ShowEmployeePanel { get; set; }
                public bool ShowDownloadPanel { get; set; }
                public string? InvalidDataMessage { get; set; }
                //public List<EmpBlockProcessRecords> EmpBlockProcess { get; set; }
                public List<EmpBlockProcessRecords> EmpBlockProcess { get; set; }
                public IFormFile FileUploadExcel { get; set; }              
            }
        public class EmpBlockProcessRecords
        {
            public string ADEMPCODE { get; set; }
            public DateTime CREATED_ON { get; set; }
            public string BSTATUS { get; set; }
            public string Reason { get; set; }
        }

        // EmployeeUnblockProcess View Model 
        public class EmpUnBlockProcessModel
        {
                public string SelectedOption { get; set; }
                public string EmpCode { get; set; }
                public string UnBlockReason { get; set; }
                public bool ShowEmployeePanel { get; set; }
                public bool ShowDownloadPanel { get; set; }
                public string InvalidDataMessage { get; set; }
                public IFormFile FileUploadExcel { get; set; }
                public List<EmpUnBlockProcessRecords> EmpUnBlockProcess { get; set; }                
                public class EmpUnBlockProcessRecords
                {
                    public string ADEMPCODE { get; set; }

                    [DisplayName("Unblock Date")]
                    public string CREATED_ON { get; set; }
                    public string UNBSTATUS { get; set; }
                    public string Reason { get; set; }
                }
            }
        }
        public class PayslipViewModel
        {
            public string? CompanyName { get; set; }
            public string? CompanyAddress { get; set; }
            public string? PayMonth { get; set; }
            public string? EmployeeName { get; set; }
            public string? EmployeeCode { get; set; }
            public string? Designation { get; set; }
            public string? Department { get; set; }
            public string? DateOfJoining { get; set; }
            public int PayDays { get; set; }
            public decimal Basic { get; set; }
            public decimal HRA { get; set; }
            public decimal Conveyance { get; set; }
            public decimal OtherAllowance { get; set; }
            public decimal PF { get; set; }
            public decimal ESI { get; set; }
            public decimal ProfessionalTax { get; set; }
            public decimal OtherDeduction { get; set; }
            public decimal TotalEarnings { get; set; }
            public decimal TotalDeductions { get; set; }
            public decimal NetPay { get; set; }
            public string? NetPayInWords { get; set; }
        }

    public class SalarySlipData
    {
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string Department { get; set; }
        public string MonthYear { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
    }
}
