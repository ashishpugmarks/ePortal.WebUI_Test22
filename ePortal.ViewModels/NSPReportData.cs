using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
   public class NSPReportData
    {


        public int RequestRefNo { get; set; }
        public string RequestDate { get; set; }
        public string Category { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorPANNo { get; set; }
        public string VendorAddress { get; set; }
        public string CostCentre { get; set; }
        public string Budgeted { get; set; }
        public string Location { get; set; }
        public string PurposeofExpenses { get; set; }
        public string SBIForexRateDate { get; set; }

        public string TotalAmount { get; set; }
        public Nullable<decimal> TDSDeduction { get; set; }
        public Nullable<decimal> AdvancePaid_Adjustments { get; set; }
        public string NetPayableAmount { get; set; }
        public string FY_RequestTranType { get; set; }

        public NSPReportData_Initiate_DT Initiate_DT { get; set; }

        public List<NSPReportData_Invoice_DT> Invoice_DT { get; set; }

        public List<NSPReportData_Approval_DT> Approval_DT { get; set; }
    }


    public class NSPReportData_Invoice_DT
    {
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }

        public string Exrate { get; set; }
        public string AmountinINR { get; set; }
    }
    public class NSPReportData_Initiate_DT
    {
        public string IntiatorName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Operation { get; set; }
        public string Dt_Location { get; set; }
    }
    public class NSPReportData_Approval_DT
    {
        public string App_Title { get; set; }
        public string Designation { get; set; }
        public string App_Name { get; set; }
        public string App_Date { get; set; }

    }
}
