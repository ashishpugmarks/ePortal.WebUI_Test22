using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Interface;
using ePortal.ViewModels.APPX;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Services
{
    public class EssServices : IEssServices
    {
        private readonly IEportalESS _objasset;

        public EssServices(IEportalESS objasset)
        {
            _objasset = objasset;
        }
        public void GET_EMPACCSTATEMENT_BALANCE()
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            DataView DV = new DataView();
            dt = null;
            int CurYear = DateTime.Now.Year;
            int Curmonth = DateTime.Now.Month;
            int curfmont = 1;
            if (Curmonth == 4)
                curfmont = 01;
            if (Curmonth == 5)
                curfmont = 02;
            if (Curmonth == 6)
                curfmont = 03;
            if (Curmonth == 7)
                curfmont = 04;
            if (Curmonth == 8)
                curfmont = 05;
            if (Curmonth == 9)
                curfmont = 06;
            if (Curmonth == 10)
                curfmont = 07;
            if (Curmonth == 11)
                curfmont = 08;
            if (Curmonth == 12)
                curfmont = 09;
            if (Curmonth == 1)
                curfmont = 10;
            if (Curmonth == 2)
                curfmont = 11;
            if (Curmonth == 3)
                curfmont = 12;
            //Int32 strecode =   Convert.ToInt32(Session["userID"].ToString());
            //string strecode1 = strecode.ToString("0000000000");
            //EportalESS objessaddress = new EportalESS();
            ////*************Retreiving address detail; of employee from SAP
            //dt = objessaddress.GetEmployeeACStatement(strecode1, ddlcategory.SelectedValue.ToString(), ddlmonthfrom.SelectedValue.ToString(), ddlmonthto.SelectedValue.ToString(), ddlfiscal.SelectedValue.ToString());


            dt.DefaultView.RowFilter = "PARTICULAR='Closing Balance'";
            dt1 = dt.DefaultView.ToTable();            
        }

        public DataTable GET_EMPACCSTATEMENT_DATATABLE(string userId, string category, string monthFrom, string monthTo, string fiscalYear)
        {
            DataTable dt = new DataTable();
            //DataTable dt1 = new DataTable();
            dt = null;
            Int32 strecode = Convert.ToInt32(userId);
            string strecode1 = strecode.ToString("0000000000");

            //EportalESS objessaddress = new EportalESS();
            //*************Retreiving address detail; of employee from SAP
            //dt = objessaddress.GetEmployeeACStatement(strecode1, ddlcategory.SelectedValue.ToString(), ddlmonthfrom.SelectedValue.ToString(), ddlmonthto.SelectedValue.ToString(), ddlfiscal.SelectedValue.ToString());
           // dt = _objasset.GetEmployeeACStatement(strecode1, category, monthFrom, monthTo, fiscalYear);
            //***************End of address detail
            //objDtl.Zfii_Emp_Acc_Statement(ddlcategory.SelectedValue.ToString(), strecode1, ddlmonthfrom.SelectedValue.ToString(), ddlmonthto.SelectedValue.ToString(), ddlfiscal.SelectedValue.ToString(), out Return0, ref empacc);
            // objDtl.Zfii_Emp_Acc_Statement("U", "0000009844", "05", "05", "2016", out Return0, ref empacc);

            DataTable dt_empacc = new DataTable();
            dt_empacc.Columns.Add("PARTICULAR");
            dt_empacc.Columns.Add("PSTNG_DATE");
            dt_empacc.Columns.Add("ALLOC_NMBR");
            dt_empacc.Columns.Add("DEBIT");
            dt_empacc.Columns.Add("CREDIT");
            dt_empacc.Columns.Add("CUMULATIVE_BALANCE");
            dt_empacc.Columns.Add("ITEM_TEXT");
            dt_empacc.Columns.Add("STATUS");
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                DataRow drrow;
                drrow = dt_empacc.NewRow();
                drrow[0] = dt.Rows[i][0].ToString();
                drrow[1] = dt.Rows[i][1].ToString().Substring(8, 2) + "." + dt.Rows[i][1].ToString().Substring(5, 2) + "." + dt.Rows[i][1].ToString().Substring(0, 4);
                drrow[2] = dt.Rows[i][2].ToString();
                drrow[3] = decimal.Round(Convert.ToDecimal(dt.Rows[i][3].ToString()), 2).ToString("N", new CultureInfo("en-US"));
                drrow[4] = decimal.Round(Convert.ToDecimal(dt.Rows[i][4].ToString()), 2).ToString("N", new CultureInfo("en-US"));
                drrow[5] = decimal.Round(Convert.ToDecimal(dt.Rows[i][5].ToString()), 2).ToString("N", new CultureInfo("en-US"));
                drrow[6] = dt.Rows[i][6].ToString();
                drrow[7] = dt.Rows[i][7].ToString();
                if (dt.Rows[i][0].ToString() == "Closing Balance")
                {
                    drrow[1] = "";
                    drrow[2] = "";
                    drrow[6] = "";
                    drrow[7] = "";
                }
                if (dt.Rows[i][0].ToString() == "Opening Balance")
                {
                    drrow[1] = "";
                    drrow[2] = "";
                    drrow[6] = "";
                    drrow[7] = "";
                }
                dt_empacc.Rows.Add(drrow);
            }

            return dt_empacc;
        }

        public async Task<List<EssViewModel.EmpAccStatementDto>> GET_EMPACCSTATEMENT_DATATABLE1(string userId, string category, string monthFrom, string monthTo, string fiscalYear)
        {
            string formattedUserId = Convert.ToInt32(userId).ToString("0000000000");
            DataTable dt = new DataTable();
            //var essService = new EportalESS();
            dt = await _objasset.GetEmployeeACStatement(formattedUserId, category, monthFrom, monthTo, fiscalYear);

           
            var result = new List<EssViewModel.EmpAccStatementDto>();


            foreach (DataRow row in dt.Rows)
            {
                var particular = row[0]?.ToString();

                var dto = new EssViewModel.EmpAccStatementDto
                {
                    Particular = particular,
                    PostingDate = FormatDate(row[1]?.ToString()),
                    AllocationNumber = row[2]?.ToString(),
                    Debit = FormatCurrency(row[3]?.ToString()),
                    Credit = FormatCurrency(row[4]?.ToString()),
                    CumulativeBalance = FormatCurrency(row[5]?.ToString()),
                    ItemText = row[6]?.ToString(),
                    Status = row[7]?.ToString()
                };

                if (particular == "Closing Balance" || particular == "Opening Balance")
                {
                    dto.PostingDate = "";
                    dto.AllocationNumber = "";
                    dto.ItemText = "";
                    dto.Status = "";
                }

                result.Add(dto);
            }

            return result;
        }

        private string FormatDate(string rawDate)
        {
            if (!string.IsNullOrEmpty(rawDate) && rawDate.Length >= 10)
            {
                return $"{rawDate.Substring(8, 2)}.{rawDate.Substring(5, 2)}.{rawDate.Substring(0, 4)}";
            }
            return "";
        }

        private string FormatCurrency(string value)
        {
            if (decimal.TryParse(value, out decimal amount))
            {
                return decimal.Round(amount, 2).ToString("N", new CultureInfo("en-US"));
            }
            return "0.00";
        }
    }
}
