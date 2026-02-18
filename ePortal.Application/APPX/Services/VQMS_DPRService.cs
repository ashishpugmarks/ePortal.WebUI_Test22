using ePortal.Application.APPX.Contracts;
using ePortal.Persistence;
using ePortal.Persistence.Interface;
using ePortal.Persistence.VQMS.Interface;
using ePortal.ViewModels.APPX.VQMS_DPR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace ePortal.Application.APPX.Services
{
    public class VQMS_DPRService : IVQMS_DPRService
    {
        static StreamWriter swLog = null;
        private readonly IDPR objDPR;
        private readonly ICommonFunctions objCommon;
        Decimal dcGLTotalInpectionQty = 1.0m;
        public VQMS_DPRService(IDPR dpr, ICommonFunctions comm)
        {
            objDPR = dpr;
            objCommon = comm;
        }

        public async Task<DailyPassRatioRptViewModel> SearchDailyPassRatioRpt(string ddlFactory, string txtDate, string ddlShift)
        {
            //if (string.IsNullOrEmpty(txtDate.Text))
            //if (string.IsNullOrEmpty(txtDate))
            //{
            //    //ScriptManager.RegisterClientScriptBlock(this, GetType(), "al", "alert('Please select date.')", true);
            //    return;
            //}
            DailyPassRatioRptViewModel ReportModel = new();
            string strDate = string.Empty;
            string[] strShifts = { };
            string strLine01 = string.Empty;
            string strLine02 = string.Empty;
            string strLine03 = string.Empty;
            string strLine04 = string.Empty;

            //DPR objDPR = new DPR("3");
            objDPR.InitializeFactory("3");
            switch (ddlFactory)
            {
                case "3":
                    strLine01 = "01";
                    strLine02 = "02";
                    strLine03 = "03";
                    strLine04 = "80";
                    ReportModel.IsChar03Visible = true;
                    ReportModel.IsChar04Visible = true;
                    //objDPR = new DPR("3");
                    objDPR.InitializeFactory("3");
                    break;
                case "6":
                    strLine01 = "04";
                    strLine02 = "05";
                    ReportModel.IsChar03Visible = false;
                    ReportModel.IsChar04Visible = false;
                    //objDPR = new DPR("6");
                    objDPR.InitializeFactory("6");
                    break;
                case "8":
                    strLine01 = "06";
                    strLine02 = "07";
                    ReportModel.IsChar03Visible = false;
                    ReportModel.IsChar04Visible = false;
                    //objDPR = new DPR("8");
                    objDPR.InitializeFactory("8");
                    break;
                case "21":
                    strLine01 = "09";
                    strLine02 = "10";
                    ReportModel.IsChar03Visible = true;
                    ReportModel.IsChar04Visible = false;
                    //objDPR = new DPR("21");
                    objDPR.InitializeFactory("21");
                    break;
            }

            strDate = string.IsNullOrEmpty(txtDate) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtDate;

            #region"LINE 01"

            #region"Pass Ratio - Count"
            //**********************************Commenting for Testing******************
            DataTable dtDPRCnt = objDPR.GetDPRStatusCount(strDate, ddlFactory, ddlShift, strLine01);

            ReportModel.DPRCnt01 = dtDPRCnt.AsEnumerable()
                            .Select(row => new Result
                            {
                                PASS_TYPE = Convert.ToString(row["PASS_TYPE"]) ?? "",
                                CNT = Convert.ToInt32(row["CNT"])
                            })
                            .ToList();

            #endregion

            #region"Pass Ratio - Percentage"
            //**********************************Commenting for Testing******************
            DataTable dtDPRPer = objDPR.GetDPRStatusPercentage(strDate, ddlFactory, ddlShift, strLine01);
            ReportModel.DPRPer01 = dtDPRPer.AsEnumerable()
                            .Select(row => new Result
                            {
                                PASS_TYPE = Convert.ToString(row["PASS_TYPE"]) ?? "",
                                PER = Convert.ToDecimal(row["PER"])
                            })
                            .ToList();
            #endregion

            #region"Sectionwise Defects"
            //**********************************Commenting for Testing******************
            DataTable dtSectionwise = objDPR.GetSectionWiseDefect(strDate, ddlFactory, ddlShift, strLine01);
            ReportModel.Sectionwise01 = dtSectionwise.AsEnumerable()
                            .Select(row => new Result
                            {
                                DEFECTSECTION = Convert.ToString(row["DEFECTSECTION"]) ?? "",
                                CNT = Convert.ToInt32(row["CNT"]),
                                CM = Convert.ToDecimal(row["CM"]),
                            })
                            .ToList();

            #endregion

            #endregion

            #region"LINE 02"

            #region"Pass Ratio - Count"
            DataTable dtDPRCnt02 = objDPR.GetDPRStatusCount(strDate, ddlFactory, ddlShift, strLine02);

            ReportModel.DPRCnt02 = dtDPRCnt02.AsEnumerable()
                            .Select(row => new Result
                            {
                                PASS_TYPE = Convert.ToString(row["PASS_TYPE"]) ?? "",
                                CNT = Convert.ToInt32(row["CNT"])
                            })
                            .ToList();
            #endregion

            #region"Pass Ratio - Percentage"
            DataTable dtDPRPer02 = objDPR.GetDPRStatusPercentage(strDate, ddlFactory, ddlShift, strLine02);
            ReportModel.DPRPer02 = dtDPRPer02.AsEnumerable()
                            .Select(row => new Result
                            {
                                PASS_TYPE = Convert.ToString(row["PASS_TYPE"]) ?? "",
                                PER = Convert.ToDecimal(row["PER"])
                            })
                            .ToList();
            #endregion

            #region"Sectionwise Defects"
            DataTable dtSectionwise02 = objDPR.GetSectionWiseDefect(strDate, ddlFactory, ddlShift, strLine02);

            ReportModel.Sectionwise02 = dtSectionwise02.AsEnumerable()
                            .Select(row => new Result
                            {
                                DEFECTSECTION = Convert.ToString(row["DEFECTSECTION"]) ?? "",
                                CNT = Convert.ToInt32(row["CNT"]),
                                CM = Convert.ToDecimal(row["CM"]),
                            })
                            .ToList();
            #endregion

            #endregion

            #region"LINE 03"

            if (ddlFactory == "3" || ddlFactory == "21")
            {

                #region"Pass Ratio - Count"
                DataTable dtDPRCnt03 = objDPR.GetDPRStatusCount(strDate, ddlFactory, ddlShift, strLine03);
                ReportModel.DPRCnt03 = dtDPRCnt03.AsEnumerable()
                            .Select(row => new Result
                            {
                                PASS_TYPE = Convert.ToString(row["PASS_TYPE"]) ?? "",
                                CNT = Convert.ToInt32(row["CNT"])
                            })
                            .ToList();
                #endregion

                #region"Pass Ratio - Percentage"
                DataTable dtDPRPer03 = objDPR.GetDPRStatusPercentage(strDate, ddlFactory, ddlShift, strLine03);

                ReportModel.DPRPer03 = dtDPRPer03.AsEnumerable()
                            .Select(row => new Result
                            {
                                PASS_TYPE = Convert.ToString(row["PASS_TYPE"]) ?? "",
                                PER = Convert.ToDecimal(row["PER"])
                            })
                            .ToList();

                #endregion

                #region"Sectionwise Defects"
                DataTable dtSectionwise03 = objDPR.GetSectionWiseDefect(strDate, ddlFactory, ddlShift, strLine03);
                ReportModel.Sectionwise03 = dtSectionwise03.AsEnumerable()
                            .Select(row => new Result
                            {
                                DEFECTSECTION = Convert.ToString(row["DEFECTSECTION"]) ?? "",
                                CNT = Convert.ToInt32(row["CNT"]),
                                CM = Convert.ToDecimal(row["CM"]),
                            })
                            .ToList();

                #endregion
            }

            #endregion

            #region"LINE 04"

            if (ddlFactory == "3")
            {

                #region"Pass Ratio - Count"
                DataTable dtDPRCnt04 = objDPR.GetDPRStatusCount(strDate, ddlFactory, ddlShift, strLine04);
                ReportModel.DPRCnt04 = dtDPRCnt04.AsEnumerable()
                            .Select(row => new Result
                            {
                                PASS_TYPE = Convert.ToString(row["PASS_TYPE"]) ?? "",
                                CNT = Convert.ToInt32(row["CNT"])
                            })
                            .ToList();

                #endregion

                #region"Pass Ratio - Percentage"
                DataTable dtDPRPer04 = objDPR.GetDPRStatusPercentage(strDate, ddlFactory, ddlShift, strLine04);
                ReportModel.DPRPer04 = dtDPRPer04.AsEnumerable()
                           .Select(row => new Result
                           {
                               PASS_TYPE = Convert.ToString(row["PASS_TYPE"]) ?? "",
                               PER = Convert.ToDecimal(row["PER"])
                           })
                           .ToList();


                #endregion

                #region"Sectionwise Defects"
                DataTable dtSectionwise04 = objDPR.GetSectionWiseDefect(strDate, ddlFactory, ddlShift, strLine04);
                ReportModel.Sectionwise04 = dtSectionwise04.AsEnumerable()
                            .Select(row => new Result
                            {
                                DEFECTSECTION = Convert.ToString(row["DEFECTSECTION"]) ?? "",
                                CNT = Convert.ToInt32(row["CNT"]),
                                CM = Convert.ToDecimal(row["CM"]),
                            })
                            .ToList();

                #endregion
            }
            #endregion

            return ReportModel;
        }
        public async Task<List<object>> FillChart(string txtdatefrom, string ddlSite)
        {
            var objdt = new DataTable();
            var chartData = new List<object>();
            //DPR objDpr = new DPR(ddlSite.SelectedValue);
            objDPR.InitializeFactory(ddlSite);
            objdt = objDPR.ShopWiseReprot(txtdatefrom, txtdatefrom);
            if (objdt.Rows.Count > 0)
            {
                foreach (DataRow row in objdt.Rows)
                {
                    chartData.Add(new
                    {
                        COUNT = Convert.ToInt32(row[0]),
                        DISC = row[1].ToString(),
                        SUBCODE = row[2].ToString()
                    });
                }
            }
            return chartData;
        }

        public async Task<List<object>> getModelwiseDefect_Report(string strShop, string strDatefrom, string strDateto, string strSiteID)
        {

            //DPR objDpr = new DPR(ddlSite.SelectedValue);
            var objdt = new DataTable();
            var chartData = new List<object>();
            objDPR.InitializeFactory(strSiteID);
            objdt = objDPR.ModelWiseReprot(strShop, strDatefrom, strDateto);
            if (objdt.Rows.Count > 0)
            {
                foreach (DataRow row in objdt.Rows)
                {
                    chartData.Add(new
                    {
                        COUNT = Convert.ToInt32(row[0]),
                        MODELCODE = row[1].ToString(),
                        DISC = row[2].ToString(),
                        SUBCODE = row[3].ToString()
                    });
                }
            }
            return chartData;
        }
        public async Task<List<object>> getdefectwise_Report(string strShop, string strDatefrom, string strModel, string strSiteID)
        {

            //DPR objDpr = new DPR(ddlSite.SelectedValue);
            var objdt = new DataTable();
            var chartData = new List<object>();
            objDPR.InitializeFactory(strSiteID);
            objdt = objDPR.DefectWiseReprot(strShop, strDatefrom, strDatefrom, strModel);
            if (objdt.Rows.Count > 0)
            {
                foreach (DataRow row in objdt.Rows)
                {
                    chartData.Add(new
                    {
                        COUNT = Convert.ToInt32(row[0]),
                        DEFECTDESCRIPTION = row[1].ToString(),
                        MODELCODE = row[2].ToString(),
                        DISC = row[3].ToString(),
                        SUBCODE = row[4].ToString()
                    });
                }
            }
            return chartData;
        }

        public async Task<IEnumerable<SelectListItem>> LoadLineDropdownlist(string ddlSite)
        {
            //DPR objDPR = new DPR(ddlSite.SelectedValue);
            objDPR.InitializeFactory(ddlSite);
            DataTable dt = objDPR.GetLine(ddlSite);
            if (dt == null || dt.Rows.Count == 0) return new List<SelectListItem>();
            return dt.AsSelectList("VQMS_LINE", "VQMS_LINEDES", false, "", "");
        }
        public async Task<IEnumerable<SelectListItem>> LoadModelDropdownlist(string ddlSite)
        {
            //DPR objDpr = new DPR(ddlSite.SelectedValue);
            objDPR.InitializeFactory(ddlSite);
            DataTable dt = objDPR.GetModel(ddlSite);
            if (dt == null || dt.Rows.Count == 0) return new List<SelectListItem>();
            return dt.AsSelectList("MTOMAPCODE", "MTOMAPCODE", false, "", "");
        }
        public async Task<string> GetDocType(string ddlSite)
        {
            var lblDocType = "";
            switch (ddlSite)
            {
                case "3":

                    lblDocType = objCommon.GetParameterValue("DPR1F_FORMATNO"); //"17010-VQ1F-F0-002";
                    break;
                case "6":
                    lblDocType = objCommon.GetParameterValue("DPR2F_FORMATNO");//"07010-VQ2F-F0-005";
                    break;
                case "8":
                    lblDocType = objCommon.GetParameterValue("DPR3F_FORMATNO");//"F1501-VQ3F-F1-13";
                    break;
                case "21":
                    lblDocType = objCommon.GetParameterValue("DPR4F_FORMATNO");//"F1701-VQ4F-F0-0002";
                    break;
                default:
                    lblDocType = "Not Available";
                    break;
            }
            //LoadModelDropdownlist();
            //LoadLineDropdownlist();
            return lblDocType;
        }

        public async Task<ReportHeaderViewModel> LoadReportHeader(string txtDate, string ddlModel, string ddline, string ddlSite)
        {
            ReportHeaderViewModel RptModel = new ReportHeaderViewModel();
            try
            {
                string _date = string.Empty;
                string _model = string.Empty;
                string _line = string.Empty;

                string strModel = string.Empty;
                string strApprovedBy = string.Empty;
                string strVerifiedBy = string.Empty;
                string strPreparedBy = string.Empty;
                string strDate = string.Empty;


                decimal dcProductionQtyShiftA = 0.0m;
                decimal dcInspectionQtyShiftA = 0.0m;
                decimal dcStraightPassQtyGR1ShiftA = 0.0m;
                decimal dcStraightPassRatioGR1ShiftA = 0.0m;
                decimal dcDirectPassQtyGR1ShiftA = 0.0m;
                decimal dcDirectPassRatioGR1ShiftA = 0.0m;
                decimal dcPdiOffGR1ShiftA = 0.0m;

                decimal dcProductionQtyShiftB = 0.0m;
                decimal dcInspectionQtyShiftB = 0.0m;
                decimal dcStraightPassQtyGR2ShiftB = 0.0m;
                decimal dcStraightPassRatioGR2ShiftB = 0.0m;
                decimal dcDirectPassQtyGR2ShiftB = 0.0m;
                decimal dcDirectPassRatioGR2ShiftB = 0.0m;
                decimal dcPdiOffGR2ShiftB = 0.0m;

                decimal dcTotalProductionQty = 0.0m;
                decimal dcTotalInspectionQty = 0.0m;
                decimal dcTotalDPV = 0.0m;
                decimal dcTotalStraightPassQty = 0.0m;
                decimal dcTotalStraightPassRatio = 0.0m;
                decimal dcTotalDirectPassQty = 0.0m;
                decimal dcTotalDirectPassRatio = 0.0m;
                decimal dcTotalPDIOff = 0.0m;
                decimal dcFrameNotEntered = 0.0m;

                _date = string.IsNullOrEmpty(txtDate) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtDate;
                _model = ddlModel;
                _line = ddline;

                objDPR.InitializeFactory(ddlSite);

                DataTable dtProd = objDPR.GetReportHeaderDetailsProd(_date, _model, ddlSite, _line);

                DataTable dtVQ = objDPR.GetReportHeaderDetailsVQ(_date, _model, ddlSite, _line);

                DataTable dtRVQ = objDPR.GetReportHeaderDetailsRVQ(_date, _model, ddlSite, _line);

                DataTable dtSP = objDPR.GetReportHeaderDetailsSP(_date, _model, ddlSite, _line);

                DataTable dtDP = objDPR.GetReportHeaderDetailsDP(_date, _model, ddlSite, _line);

                DataTable dtFN = objDPR.GetReportHeaderDetailsFN(_date, _model, ddlSite, _line);

                if (dtProd.Rows.Count > 0)
                {
                    //DataRow dr = dtProd.Rows[0];
                    dcProductionQtyShiftA = Convert.ToDecimal(dtProd.Rows[0]["PRODUCTION_QTY_A"]);
                    dcProductionQtyShiftB = Convert.ToDecimal(dtProd.Rows[0]["PRODUCTION_QTY_B"]);
                }
                if (dtVQ.Rows.Count > 0)
                {
                    dcInspectionQtyShiftA = Convert.ToDecimal(dtVQ.Rows[0]["VQ_QTY_A"]);
                    dcInspectionQtyShiftB = Convert.ToDecimal(dtVQ.Rows[0]["VQ_QTY_B"]);
                }

                if (dtSP.Rows.Count > 0)
                {
                    dcStraightPassQtyGR1ShiftA = Convert.ToDecimal(dtSP.Rows[0]["CNT_A"]);
                    dcStraightPassQtyGR2ShiftB = Convert.ToDecimal(dtSP.Rows[0]["CNT_B"]);
                }

                if (dtDP.Rows.Count > 0)
                {
                    dcDirectPassQtyGR1ShiftA = Convert.ToDecimal(dtDP.Rows[0]["CNT_A"]);
                    dcDirectPassQtyGR2ShiftB = Convert.ToDecimal(dtDP.Rows[0]["CNT_B"]);
                }

                if (dtRVQ.Rows.Count > 0)
                {
                    dcPdiOffGR1ShiftA = Convert.ToDecimal(dtRVQ.Rows[0]["RVQ_QTY_A"]);
                    dcPdiOffGR2ShiftB = Convert.ToDecimal(dtRVQ.Rows[0]["RVQ_QTY_B"]);
                }

                if (dtFN.Rows.Count > 0)
                {
                    dcFrameNotEntered = Convert.ToDecimal(dtFN.Rows[0]["Not_Entered_Frames"]);
                }

                if (dcFrameNotEntered > 0)
                {
                    RptModel.lblFrameNotEntered = "This report is not complete as defects for " + dcFrameNotEntered + " frames yet to be entered";
                }
                else
                {
                    RptModel.lblFrameNotEntered = "";
                }


                dcTotalPDIOff = dcPdiOffGR1ShiftA + dcPdiOffGR2ShiftB;

                dcTotalProductionQty = dcProductionQtyShiftA + dcProductionQtyShiftB;
                dcTotalInspectionQty = dcInspectionQtyShiftA + dcInspectionQtyShiftB;
                dcGLTotalInpectionQty = dcTotalInspectionQty;
                dcTotalStraightPassQty = dcStraightPassQtyGR1ShiftA + dcStraightPassQtyGR2ShiftB;
                dcTotalDirectPassQty = dcDirectPassQtyGR1ShiftA + dcDirectPassQtyGR2ShiftB;

                if (dcInspectionQtyShiftA > 0)
                    dcStraightPassRatioGR1ShiftA = (dcStraightPassQtyGR1ShiftA / dcInspectionQtyShiftA) * 100;

                if (dcInspectionQtyShiftB > 0)
                    dcStraightPassRatioGR2ShiftB = (dcStraightPassQtyGR2ShiftB / dcInspectionQtyShiftB) * 100;

                if (dcTotalInspectionQty > 0)
                    dcTotalStraightPassRatio = (dcTotalStraightPassQty / dcTotalInspectionQty) * 100;

                if (dcInspectionQtyShiftA > 0)
                    dcDirectPassRatioGR1ShiftA = (dcDirectPassQtyGR1ShiftA / dcInspectionQtyShiftA) * 100;
                if (dcInspectionQtyShiftB > 0)
                    dcDirectPassRatioGR2ShiftB = (dcDirectPassQtyGR2ShiftB / dcInspectionQtyShiftB) * 100;
                if (dcTotalInspectionQty > 0)
                    dcTotalDirectPassRatio = (dcTotalDirectPassQty / dcTotalInspectionQty) * 100;



                RptModel.lblProductQtyShiftA = Math.Round(dcProductionQtyShiftA, 2).ToString();
                RptModel.lblProductionQtyShiftB = Math.Round(dcProductionQtyShiftB, 2).ToString();
                RptModel.lblInpectionQtyShiftA = Math.Round(dcInspectionQtyShiftA, 2).ToString();
                RptModel.lblInspectionQtyShiftB = Math.Round(dcInspectionQtyShiftB, 2).ToString();
                RptModel.lblStraightPassQtyGR1ShiftA = Math.Round(dcStraightPassQtyGR1ShiftA, 2).ToString();
                RptModel.lblStraightPassQtyGR2ShiftB = Math.Round(dcStraightPassQtyGR2ShiftB, 2).ToString();

                RptModel.lblStraightPassRatioGR1ShiftA = Math.Round(dcStraightPassRatioGR1ShiftA, 2).ToString();
                RptModel.lblStraightPassRatioGR2ShiftB = Math.Round(dcStraightPassRatioGR2ShiftB, 2).ToString();

                RptModel.lblDirectPassQtyGR1ShiftA = Math.Round(dcDirectPassQtyGR1ShiftA, 2).ToString();
                RptModel.lblDirectPassQtyGR2ShiftB = Math.Round(dcDirectPassQtyGR2ShiftB, 2).ToString();
                RptModel.lblDirectPassRatioGR1ShiftA = Math.Round(dcDirectPassRatioGR1ShiftA, 2).ToString();
                RptModel.lblDirectPassRatioGR2ShiftB = Math.Round(dcDirectPassRatioGR2ShiftB, 2).ToString();

                RptModel.lblPDIOffGR1ShiftA = Math.Round(dcPdiOffGR1ShiftA, 2).ToString();
                RptModel.lblPDIOFFGR2ShiftB = Math.Round(dcPdiOffGR2ShiftB, 2).ToString();

                RptModel.lblTotalProductionQty = Math.Round(dcTotalProductionQty, 2).ToString();
                RptModel.lblTotalInpectionQty = Math.Round(dcTotalInspectionQty, 2).ToString();
                RptModel.lblTotalStraightPassQty = Math.Round(dcTotalStraightPassQty, 2).ToString();
                RptModel.lblTotalStraightPassRatio = Math.Round(dcTotalStraightPassRatio, 2).ToString();
                RptModel.lblTotalDirectPassQty = Math.Round(dcTotalDirectPassQty, 2).ToString();
                RptModel.lblTotalDirectPassRatio = Math.Round(dcTotalDirectPassRatio, 2).ToString();
                RptModel.lblTotalPDIOff = Math.Round(dcTotalPDIOff, 2).ToString();
                RptModel.lblDate = _date;
                RptModel.lblModel = ddline.ToString() + "-" + _model;
            }
            catch (DivideByZeroException ex)
            {

            }
            return RptModel;
        }

        public async Task<List<SectionWiseGraphRow>> LoadGraphSectionWise(string txtDate, string ddlModel, string ddline, string ddlSite)
        {
            var _date = string.IsNullOrEmpty(txtDate) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtDate;
            var _model = ddlModel;
            var _line = ddline;

            //DPR objDpr = new DPR(ddlSite);
            objDPR.InitializeFactory(ddlSite);
            var dt = objDPR.GetReportDefectShopwise(_date, _model, ddlSite, _line);

            var vm = new List<SectionWiseGraphRow>();

            foreach (DataRow dr in dt.Rows)
            {
                vm.Add(new SectionWiseGraphRow
                {
                    Section = dr["Section"].ToString(),
                    TotalDefects = Convert.ToInt32(dr["TotalDefects"]),
                    CM = Convert.ToDecimal(dr["CM"])
                });
            }

            return vm;
        }

        public async Task<List<CategoryWiseGraphRow>> LoadGraphCategorywise(string txtDate, string ddlModel, string ddline, string ddlSite)
        {
            var _date = string.IsNullOrEmpty(txtDate) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtDate;
            var _model = ddlModel;
            var _line = ddline;

            //DPR objDpr = new DPR(ddlSite);
            objDPR.InitializeFactory(ddlSite);
            var dt = objDPR.GetReportDefectCategorywise(_date, _model, ddlSite, _line);

            var vm = new List<CategoryWiseGraphRow>();

            foreach (DataRow dr in dt.Rows)
            {
                vm.Add(new CategoryWiseGraphRow
                {
                    Category = dr["Category"].ToString(),
                    TotalDefects = Convert.ToInt32(dr["TotalDefects"]),
                    CM = Convert.ToDecimal(dr["CM"])
                });
            }

            return vm;
        }
        public async Task<DefectReportViewModel> LoadDefectDataRows(string txtDate, string ddlSite, string ddline, string ddlModel)
        {
            // Default date
            var _date = string.IsNullOrEmpty(txtDate) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtDate;

            //DPR objDpr = new DPR(site);
            objDPR.InitializeFactory(ddlSite);
            var dtDetails = objDPR.GetReportDefectDetails(_date, ddlModel, ddlSite, ddline);

            var data = new DefectReportViewModel();

            foreach (DataRow dr in dtDetails.Rows)
            {
                var row = new DefectRowViewModel
                {
                    DefectDescription = Convert.ToString(dr["DEFECTDESCRIPTION"]),
                    Category = Convert.ToString(dr["Category"]),
                    Section = Convert.ToString(dr["Section"]),
                    AShift = Convert.ToDecimal(dr["A_SHIFT"]),
                    BShift = Convert.ToDecimal(dr["B_SHIFT"]),
                    Total = Convert.ToDecimal(dr["Total"])
                };

                data.Rows.Add(row);
                data.TotalShiftA += row.AShift;
                data.TotalShiftB += row.BShift;
                data.GrandTotal += row.Total;
            }

            return data;
        }
        public async Task<SectionWiseReportViewModel> LoadSectionWiseData(string txtDate, string ddlModel, string ddlSite, string ddline)
        {
            var _date = string.IsNullOrEmpty(txtDate) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtDate;
            //DPR objDpr = new DPR(ddlSite);
            objDPR.InitializeFactory(ddlSite);
            var dt = objDPR.GetReportDefectShopCnt(_date, ddlModel, ddlSite, ddline);

            var vm = new SectionWiseReportViewModel();
            decimal dcTotalShiftA = 0, dcTotalShiftB = 0, dcTotal = 0;

            foreach (DataRow dr in dt.Rows)
            {
                var row = new SectionWiseRowViewModel
                {
                    Section = dr["Section"].ToString(),
                    AShift = Convert.ToDecimal(dr["A_SHIFT"]),
                    BShift = Convert.ToDecimal(dr["B_SHIFT"]),
                    TotalDefects = Convert.ToDecimal(dr["TotalDefects"]),
                    Dpv = Math.Round((Convert.ToDecimal(dr["TotalDefects"]) / dcGLTotalInpectionQty) * 1000, 2)
                };

                vm.Rows.Add(row);
                dcTotalShiftA += row.AShift;
                dcTotalShiftB += row.BShift;
                dcTotal += row.TotalDefects;
            }

            vm.TotalShiftA = dcTotalShiftA;
            vm.TotalShiftB = dcTotalShiftB;
            vm.GrandTotal = dcTotal;
            vm.TotalDpv = Math.Round(((dcTotal / dcGLTotalInpectionQty) * 1000), 2);
            vm.lblTotalDPV = Math.Round((dcTotal / dcGLTotalInpectionQty), 2).ToString();
            return vm;
        }

        public async Task<CategoryWiseReportViewModel> LoadCategorywiseData(string txtDate, string ddlModel, string ddline, string ddlSite)
        {
            var _date = string.IsNullOrEmpty(txtDate) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtDate;
            //DPR objDpr = new DPR(ddlSite);
            objDPR.InitializeFactory(ddlSite);
            var dt = objDPR.GetReportDefectCategorywiseCnt(_date, ddlModel, ddlSite, ddline);

            var vm = new CategoryWiseReportViewModel();
            decimal dcTotalShiftA = 0, dcTotalShiftB = 0, dcTotal = 0;

            foreach (DataRow dr in dt.Rows)
            {
                var row = new CategoryWiseRowViewModel
                {
                    Category = Convert.ToString(dr["Category"]),
                    AShift = Convert.ToDecimal(dr["A_SHIFT"]),
                    BShift = Convert.ToDecimal(dr["B_SHIFT"]),
                    TotalDefects = Convert.ToDecimal(dr["TotalDefects"])
                };

                vm.Rows.Add(row);
                dcTotalShiftA += row.AShift;
                dcTotalShiftB += row.BShift;
                dcTotal += row.TotalDefects;
            }

            vm.TotalShiftA = Math.Round(dcTotalShiftA, 2);
            vm.TotalShiftB = Math.Round(dcTotalShiftB, 2);
            vm.GrandTotal = Math.Round(dcTotal, 2);

            return vm;
        }

        public async Task<string> GenerateHTMLReport(string txtDate, string ddlModel, string ddlSite, string ddline)
        {
            var str = "";
            try
            {
                decimal dcTotalShiftA = 0.0m;
                decimal dcTotalShiftB = 0.0m;
                decimal dcTotal = 0.0m;

                string _date = string.Empty;
                string _model = string.Empty;
                string _line = string.Empty;


                string strModel = string.Empty;
                string strApprovedBy = string.Empty;
                string strVerifiedBy = string.Empty;
                string strPreparedBy = string.Empty;
                string strDate = string.Empty;


                decimal dcProductionQtyShiftA = 0.0m;
                decimal dcInspectionQtyShiftA = 0.0m;
                decimal dcStraightPassQtyGR1ShiftA = 0.0m;
                decimal dcStraightPassRatioGR1ShiftA = 0.0m;
                decimal dcDirectPassQtyGR1ShiftA = 0.0m;
                decimal dcDirectPassRatioGR1ShiftA = 0.0m;
                decimal dcPdiOffGR1ShiftA = 0.0m;

                decimal dcProductionQtyShiftB = 0.0m;
                decimal dcInspectionQtyShiftB = 0.0m;
                decimal dcStraightPassQtyGR2ShiftB = 0.0m;
                decimal dcStraightPassRatioGR2ShiftB = 0.0m;
                decimal dcDirectPassQtyGR2ShiftB = 0.0m;
                decimal dcDirectPassRatioGR2ShiftB = 0.0m;
                decimal dcPdiOffGR2ShiftB = 0.0m;

                decimal dcTotalProductionQty = 0.0m;
                decimal dcTotalInspectionQty = 0.0m;
                decimal dcTotalDPV = 0.0m;
                decimal dcTotalStraightPassQty = 0.0m;
                decimal dcTotalStraightPassRatio = 0.0m;
                decimal dcTotalDirectPassQty = 0.0m;
                decimal dcTotalDirectPassRatio = 0.0m;
                decimal dcTotalPDIOff = 0.0m;

                string strDocNumber = "";
                switch (ddlSite)
                {
                    case "3":
                        strDocNumber = "F1701-VQ1F-F0-0002";
                        break;
                    case "6":
                        strDocNumber = "S0701-VQ2F-F00-0005";
                        break;
                    case "8":
                        strDocNumber = "F1501-VQ3F-F1-13";
                        break;
                    case "21":
                        strDocNumber = "F1701-VQ4F-F0-0002";
                        //lblDocType.Text = "F1701-VQ4F-F0-0002";
                        break;
                    default:
                        strDocNumber = "Not Available";
                        break;
                }

                _date = string.IsNullOrEmpty(txtDate) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtDate;
                _model = ddlModel;
                _line = ddline;


                //DataTable dtHeadeer = objDpr.GetFinalInspectionHeader(_date, _model, ddlSite.SelectedValue);
                //DPR objDpr = new DPR(ddlSite.SelectedValue);
                objDPR.InitializeFactory(ddlSite);
                DataTable dtProd = objDPR.GetReportHeaderDetailsProd(_date, _model, ddlSite, _line);
                DataTable dtVQ = objDPR.GetReportHeaderDetailsVQ(_date, _model, ddlSite, _line);
                DataTable dtRVQ = objDPR.GetReportHeaderDetailsRVQ(_date, _model, ddlSite, _line);
                DataTable dtSP = objDPR.GetReportHeaderDetailsSP(_date, _model, ddlSite, _line);
                DataTable dtDP = objDPR.GetReportHeaderDetailsDP(_date, _model, ddlSite, _line);

                if (dtProd.Rows.Count > 0)
                {
                    //DataRow dr = dtProd.Rows[0];
                    dcProductionQtyShiftA = Convert.ToDecimal(dtProd.Rows[0]["PRODUCTION_QTY_A"]);
                    dcProductionQtyShiftB = Convert.ToDecimal(dtProd.Rows[0]["PRODUCTION_QTY_B"]);
                }
                if (dtVQ.Rows.Count > 0)
                {
                    dcInspectionQtyShiftA = Convert.ToDecimal(dtVQ.Rows[0]["VQ_QTY_A"]);
                    dcInspectionQtyShiftB = Convert.ToDecimal(dtVQ.Rows[0]["VQ_QTY_B"]);
                }

                if (dtSP.Rows.Count > 0)
                {
                    dcStraightPassQtyGR1ShiftA = Convert.ToDecimal(dtSP.Rows[0]["CNT_A"]);
                    dcStraightPassQtyGR2ShiftB = Convert.ToDecimal(dtSP.Rows[0]["CNT_B"]);
                }

                if (dtDP.Rows.Count > 0)
                {
                    dcDirectPassQtyGR1ShiftA = Convert.ToDecimal(dtDP.Rows[0]["CNT_A"]);
                    dcDirectPassQtyGR2ShiftB = Convert.ToDecimal(dtDP.Rows[0]["CNT_B"]);
                }

                if (dtRVQ.Rows.Count > 0)
                {
                    dcPdiOffGR1ShiftA = Convert.ToDecimal(dtRVQ.Rows[0]["RVQ_QTY_A"]);
                    dcPdiOffGR2ShiftB = Convert.ToDecimal(dtRVQ.Rows[0]["RVQ_QTY_B"]);
                }


                dcTotalPDIOff = dcPdiOffGR1ShiftA + dcPdiOffGR2ShiftB;

                dcTotalProductionQty = dcProductionQtyShiftA + dcProductionQtyShiftB;
                dcTotalInspectionQty = dcInspectionQtyShiftA + dcInspectionQtyShiftB;
                dcGLTotalInpectionQty = dcTotalInspectionQty;
                dcTotalStraightPassQty = dcStraightPassQtyGR1ShiftA + dcStraightPassQtyGR2ShiftB;
                dcTotalDirectPassQty = dcDirectPassQtyGR1ShiftA + dcDirectPassQtyGR2ShiftB;

                if (dcInspectionQtyShiftA > 0)
                    dcStraightPassRatioGR1ShiftA = (dcStraightPassQtyGR1ShiftA / dcInspectionQtyShiftA) * 100;
                if (dcInspectionQtyShiftB > 0)
                    dcStraightPassRatioGR2ShiftB = (dcStraightPassQtyGR2ShiftB / dcInspectionQtyShiftB) * 100;
                if (dcTotalInspectionQty > 0)
                    dcTotalStraightPassRatio = (dcTotalStraightPassQty / dcTotalInspectionQty) * 100;

                if (dcInspectionQtyShiftA > 0)
                    dcDirectPassRatioGR1ShiftA = (dcDirectPassQtyGR1ShiftA / dcInspectionQtyShiftA) * 100;
                if (dcInspectionQtyShiftB > 0)
                    dcDirectPassRatioGR2ShiftB = (dcDirectPassQtyGR2ShiftB / dcInspectionQtyShiftB) * 100;
                if (dcTotalInspectionQty > 0)
                    dcTotalDirectPassRatio = (dcTotalDirectPassQty / dcTotalInspectionQty) * 100;


                //lblProductQtyShiftA.Text = Math.Round(dcProductionQtyShiftA, 2).ToString();
                //lblProductionQtyShiftB.Text = Math.Round(dcProductionQtyShiftB, 2).ToString();
                //lblInpectionQtyShiftA.Text = Math.Round(dcInspectionQtyShiftA, 2).ToString();
                //lblInspectionQtyShiftB.Text = Math.Round(dcInspectionQtyShiftB, 2).ToString();

                //lblStraightPassQtyGR1ShiftA.Text = Math.Round(dcStraightPassQtyGR1ShiftA, 2).ToString();
                //lblStraightPassQtyGR2ShiftB.Text = Math.Round(dcStraightPassQtyGR2ShiftB, 2).ToString();

                //lblStraightPassRatioGR1ShiftA.Text = Math.Round(dcStraightPassRatioGR1ShiftA, 2).ToString();
                //lblStraightPassRatioGR2ShiftB.Text = Math.Round(dcStraightPassRatioGR2ShiftB, 2).ToString();

                //lblDirectPassQtyGR1ShiftA.Text = Math.Round(dcDirectPassQtyGR1ShiftA, 2).ToString();
                //lblDirectPassQtyGR2ShiftB.Text = Math.Round(dcDirectPassQtyGR2ShiftB, 2).ToString();
                //lblDirectPassRatioGR1ShiftA.Text = Math.Round(dcDirectPassRatioGR1ShiftA, 2).ToString();
                //lblDirectPassRatioGR2ShiftB.Text = Math.Round(dcDirectPassRatioGR2ShiftB, 2).ToString();

                //lblPDIOffGR1ShiftA.Text = Math.Round(dcPdiOffGR1ShiftA, 2).ToString();
                //lblPDIOFFGR2ShiftB.Text = Math.Round(dcPdiOffGR2ShiftB, 2).ToString();

                //lblTotalProductionQty.Text = Math.Round(dcTotalProductionQty, 2).ToString();
                //lblTotalInpectionQty.Text = Math.Round(dcTotalInspectionQty, 2).ToString();
                //lblTotalStraightPassQty.Text = Math.Round(dcTotalStraightPassQty, 2).ToString();
                //lblTotalStraightPassRatio.Text = Math.Round(dcTotalStraightPassRatio, 2).ToString();
                //lblTotalDirectPassQty.Text = Math.Round(dcTotalDirectPassQty, 2).ToString();
                //lblTotalDirectPassRatio.Text = Math.Round(dcTotalDirectPassRatio, 2).ToString();
                //lblTotalPDIOff.Text = Math.Round(dcTotalPDIOff, 2).ToString();



                //lblDate.Text = _date;
                //lblModel.Text = _model;
                //}


                decimal dcTotalSec2 = 0.0m;
                DataTable dtShop = new DataTable();
                //dtShop = objDpr.GetDefectShopwiseCountDetails(_date, _model, ddlSite.SelectedValue);
                dtShop = objDPR.GetReportDefectShopCnt(_date, _model, ddlSite, _line);

                foreach (DataRow dr in dtShop.Rows)
                {
                    dcTotalSec2 += Convert.ToDecimal(dr["TotalDefects"]);
                }


                decimal dcTotalSec = 0.0m;
                StringBuilder sb = new StringBuilder();
                sb.Append("<table>");
                sb.Append("<tr>");
                sb.Append("<td align='left' style='border: .5pt solid black;' valign='top' colspan='2'>");
                sb.Append("HMSI");
                sb.Append("</td>");
                sb.Append("<td align='center' colspan='13' style='border: .5pt solid black;' valign='top'>");
                sb.Append("DEFECTIVE RECORD ON FINAL INSPECTION REPORT");
                sb.Append("</td>");
                sb.Append("<td align='left'  style='border: .5pt solid black;' valign='top' colspan='3'>");
                sb.Append(strDocNumber);
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td rowspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center; vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;font-size: 10px; width: 5%'>");
                sb.Append(_model);
                sb.Append("</td>");
                sb.Append("<td rowspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center;vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;font-size: 10px; width: 4%''>");
                sb.Append("PROD. QTY");
                sb.Append("</td>");
                sb.Append("<td rowspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center; vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold; font-size: 10px; width: 4%''>");
                sb.Append("INSP. QTY");
                sb.Append("</td>");
                sb.Append("<td rowspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center;vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;font-size: 10px; width: 3%''>");
                sb.Append("D.P.V");
                sb.Append("</td>");
                sb.Append("<td colspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center;vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;font-size: 10px; width: 7%'>");
                sb.Append("STRAIGHT PASS QTY");
                sb.Append("</td>");
                sb.Append("<td colspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center;vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;");
                sb.Append("font-size: 10px; width: 7%'>");
                sb.Append("STRAIGHT PASS RATIO");
                sb.Append("</td>");
                sb.Append("<td colspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center;vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;font-size: 10px; width: 7%'>");
                sb.Append("DIRECT PASS QTY");
                sb.Append("</td>");
                sb.Append("<td colspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center;vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;font-size: 10px; width: 5%'>");
                sb.Append("DIRECT PASS RATIO");
                sb.Append("</td>");
                sb.Append("<td colspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center;vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;font-size: 10px; width: 5%'>");
                sb.Append("PDI-OFF");
                sb.Append("</td>");
                sb.Append("<td rowspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center;vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;font-size: 10px; width: 5%'>");
                sb.Append("SHIFT<span style='mso-spacerun: yes'>&nbsp; </span>:<span style='mso-spacerun: yes'>&nbsp;</span>A+B");
                sb.Append("</td>");
                sb.Append("<td style='color: black; font-family: Arial, sans-serif; text-align: center; vertical-align: middle;border: .5pt solid windowtext; background: silver; font-weight: bold; font-size: 10px;width: 5%;' colspan='1' rowspan='2'>");
                sb.Append("DATE :");
                sb.Append("</td>");
                sb.Append("<td rowspan='2' style='color: black; font-family: Arial, sans-serif; text-align: center;vertical-align: middle; border: .5pt solid windowtext; background: silver; font-weight: bold;font-size: 10px; width: 5%;' colspan='2'>");
                sb.Append(_date);
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR1");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR2");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR1");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR2");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR1");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR2");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR1");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR2");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR1");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='font-weight: bold; font-size: 10px; font-family: Arial;border: .5pt solid windowtext;'>");
                sb.Append("GR2");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='background-color: Silver; border: .5pt solid windowtext; font-weight: bold;font-size: 10px;'>");
                sb.Append("SHIFT &#39;A&#39;");
                sb.Append("</td>");
                sb.Append("<td style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcProductionQtyShiftA);
                sb.Append("</td>");
                sb.Append("<td style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcInspectionQtyShiftA);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcStraightPassQtyGR1ShiftA);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcStraightPassRatioGR1ShiftA);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcDirectPassQtyGR1ShiftA);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcDirectPassRatioGR1ShiftA);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcPdiOffGR1ShiftA);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td style='font-size: 10px; font-weight: bold; border: .5pt solid windowtext; font-family: Arial;'bgcolor='Silver'>");
                sb.Append("APPROVED BY");
                sb.Append("</td>");
                sb.Append("<td style='font-size: 10px; font-weight: bold; border: .5pt solid windowtext; font-family: Arial;'bgcolor='Silver'>");
                sb.Append("VERIFIED BY");
                sb.Append("</td>");
                sb.Append("<td style='font-size: 10px; font-weight: bold; border: .5pt solid windowtext; font-family: Arial;'bgcolor='Silver' colspan='2'>");
                sb.Append("PREPARED BY");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='background-color: Silver; border: .5pt solid windowtext; font-weight: bold;font-size: 10px; font-family: Arial; border: .5pt solid windowtext;'>");
                sb.Append("SHIFT &#39;B&#39;");
                sb.Append("</td>");
                sb.Append("<td style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcProductionQtyShiftB);
                sb.Append("</td>");
                sb.Append("<td style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcInspectionQtyShiftB);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcStraightPassQtyGR2ShiftB);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcStraightPassRatioGR2ShiftB);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcDirectPassQtyGR2ShiftB);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcDirectPassRatioGR2ShiftB);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcPdiOffGR2ShiftB);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append("<asp:Label runat='server' ID='lblApprovedBy'></asp:Label>");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append("<asp:Label runat='server' ID='lblVerifiedBy'></asp:Label>");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'colspan='2'>");
                sb.Append("<asp:Label runat='server' ID='lblPreparedBy'></asp:Label>");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='background-color: Silver; border: .5pt solid windowtext; font-weight: bold;font-family: Arial; font-size: 10px;'> TOTAL ");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcTotalProductionQty);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(dcTotalInspectionQty);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px; font-family: Arial;'>");
                sb.Append(Math.Round((dcTotalSec2 / dcGLTotalInpectionQty), 2).ToString());
                sb.Append("</td>");
                sb.Append("<td colspan='2' bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px;font-family: Arial;'>");
                sb.Append(dcTotalStraightPassQty);
                sb.Append("</td>");
                sb.Append("<td colspan='2' bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px;font-family: Arial;'>");
                sb.Append(dcTotalStraightPassRatio + " %");
                sb.Append("</td>");
                sb.Append("<td colspan='2' bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px;font-family: Arial;'>");
                sb.Append(dcTotalDirectPassQty);
                sb.Append("</td>");
                sb.Append("<td colspan='2' bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px;font-family: Arial;'>");
                sb.Append(dcTotalDirectPassRatio + " %");
                sb.Append("</td>");
                sb.Append("<td colspan='2' bgcolor='Silver' style='border: .5pt solid windowtext; font-size: 10px;font-family: Arial;'>");
                sb.Append(dcTotalPDIOff);
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td colspan='2' bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("<td bgcolor='Silver' style='border: .5pt solid windowtext;'> &nbsp;");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td colspan='6' valign='top' style='border: .5pt solid windowtext;'>");
                /*Load Defect Rows*/

                //DPR objDPR = new DPR();
                DataTable dtDetails = new DataTable();

                sb.Append("<table width='100%' border='0' cellpadding='0' cellspacing='0' style='border-collapse: collapse;font-size:9px;font-family:Arial;'>");
                sb.Append("<tr>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 15%; font-size: 10px;font-family:Arial;text-align:center;'>DEFECT</td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 15%; font-size: 10px;font-family:Arial;text-align:center;'>CATEGORY</td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>SEC</td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>A</td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>B</td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>TOTAL</td>");
                sb.Append("</tr>");

                dcTotalShiftA = 0.0m;
                dcTotalShiftB = 0.0m;
                dcTotal = 0.0m;

                dtDetails = objDPR.GetReportDefectDetails(_date, _model, ddlSite, _line);

                foreach (DataRow dr in dtDetails.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append("<td style='font-size: 8px;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:left;padding:2px;'>" + Convert.ToString(dr["DEFECTDESCRIPTION"]) + "</td>");
                    sb.Append("<td style='font-size: 8px;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["Category"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["Section"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["A_SHIFT"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["B_SHIFT"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["Total"]) + "</td>");
                    sb.Append("</tr>");
                    dcTotalShiftA += Convert.ToDecimal(dr["A_SHIFT"]);
                    dcTotalShiftB += Convert.ToDecimal(dr["B_SHIFT"]);
                    dcTotal += Convert.ToDecimal(dr["Total"]);
                }

                sb.Append("<tr>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;' colspan='3'>&nbsp;Total&nbsp;</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + dcTotalShiftA.ToString() + "</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + dcTotalShiftB.ToString() + "</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;text-align:center;'>" + dcTotal.ToString() + "</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                /*--------------------------------------------------------------------------------------------*/

                sb.Append("</td>");
                sb.Append("<td colspan='12' valign='top' align='center' style='padding: 10px; padding-left: 35px;border: .5pt solid windowtext;'>            ");
                decimal dcTotalShiftASec = 0.0m;
                decimal dcTotalShiftBSec = 0.0m;
                sb.Append("<table width='60%' border='0' cellpadding='0' cellspacing='0' style='border-collapse: collapse;font-size:9px;font-family:Arial;'>");
                sb.Append("<tr>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 10%; font-size: 10px;font-family:Arial;text-align:center;'>Shop</td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>Shift 'A' </td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>Shift 'B' </td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>Total</td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>D/1000 V</td>");
                sb.Append("</tr>");


                foreach (DataRow dr in dtShop.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;border-left:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["Section"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["A_SHIFT"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["B_SHIFT"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;text-align:center;border-right:solid .5pt #000000;'>" + Convert.ToString(dr["TotalDefects"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;text-align:center;border-right:solid .5pt #000000;'>" + Math.Round((Convert.ToDecimal(dr["TotalDefects"]) / dcGLTotalInpectionQty) * 1000, 2).ToString() + "</td>");
                    sb.Append("</tr>");
                    dcTotalShiftASec += Convert.ToDecimal(dr["A_SHIFT"]);
                    dcTotalShiftBSec += Convert.ToDecimal(dr["B_SHIFT"]);
                    dcTotalSec += Convert.ToDecimal(dr["TotalDefects"]);
                }

                sb.Append("<tr>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;border-left:solid .5pt #000000;text-align:center;'>&nbsp;Total&nbsp;</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + dcTotalShiftASec.ToString() + "</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;'>" + dcTotalShiftBSec.ToString() + "</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;text-align:center;border-right:solid .5pt #000000;'>" + dcTotalSec.ToString() + "</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;text-align:center;border-right:solid .5pt #000000;'>" + Math.Round(((dcTotalSec / dcGLTotalInpectionQty) * 1000), 2).ToString() + "</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                /*-----------------------------------------------------------------------------*/
                sb.Append("</div>");
                sb.Append("<div style='height: 15px;'>&nbsp;</div>");
                sb.Append("<div style='text-align: left; padding-left: 10px;'>");

                dcTotalDPV = Math.Round((dcTotalSec2 / dcGLTotalInpectionQty), 4);

                /*Load Categorywise defects*/
                decimal dcTotalShiftACat = 0.0m;
                decimal dcTotalShiftBCat = 0.0m;
                decimal dcTotalCat = 0.0m;

                DataTable dtcat = new DataTable();

                dtcat = objDPR.GetReportDefectCategorywiseCnt(_date, _model, ddlSite, _line);

                sb.Append("<table width='60%' border='0' cellpadding='0' cellspacing='0' style='border-collapse: collapse;font-size:9px;font-family:Arial;'>");
                sb.Append("<tr>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 7%; font-size: 10px;font-family:Arial;text-align:center;'>Items</td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>Shift 'A' </td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>Shift 'B' </td>");
                sb.Append("<td style='background-color: #FFCC99; border: .5pt solid windowtext; font-weight: bold;width: 5%; font-size: 10px;font-family:Arial;text-align:center;'>Total</td>");
                sb.Append("</tr>");


                foreach (DataRow dr in dtcat.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;border-left:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["Category"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;border-left:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["A_SHIFT"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;border-left:solid .5pt #000000;text-align:center;'>" + Convert.ToString(dr["B_SHIFT"]) + "</td>");
                    sb.Append("<td style='width: 5%; font-size: 8px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;text-align:center;border-left:solid .5pt #000000;'>" + Convert.ToString(dr["TotalDefects"]) + "</td>");
                    sb.Append("</tr>");
                    dcTotalShiftACat += Convert.ToDecimal(dr["A_SHIFT"]);
                    dcTotalShiftBCat += Convert.ToDecimal(dr["B_SHIFT"]);
                    dcTotalCat += Convert.ToDecimal(dr["TotalDefects"]);
                }

                sb.Append("<tr>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;border-left:solid .5pt #000000;text-align:center;'>&nbsp;Total&nbsp;</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;border-left:solid .5pt #000000;text-align:center;'>" + Math.Round(dcTotalShiftACat, 2).ToString() + "</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;border-right:solid .5pt #000000;border-left:solid .5pt #000000;text-align:center;'>" + Math.Round(dcTotalShiftBCat, 2).ToString() + "</td>");
                sb.Append("<td style='width: 5%; font-size: 9px;font-family:Arial;border-bottom:solid .5pt #000000;text-align:center;border-right:solid .5pt #000000;'>" + Math.Round(dcTotalCat, 2).ToString() + "</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                /*------------------------------*/

                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                str = sb.ToString();
            }
            catch (DivideByZeroException ex)
            {
            }
            return str;
        }

        public async Task<string> ExportDefectTrendRpt(string txtDate, string ddlModel, string ddlFactory)
        {
            //if (string.IsNullOrEmpty(txtMonth.Text) || string.IsNullOrEmpty(txtYear.Text))
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, GetType(), "al", "alert('Please select Month & Year.')", true);
            //    return;
            //}

            string strMonth = string.Empty;
            string strYear = string.Empty;
            string strModel = string.Empty;
            string strFactory = string.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //strMonth = txtMonth.Text;
            //strYear = txtYear.Text;
            if (DateTime.TryParse(txtDate, out var parsedDate))
            {
                strMonth = parsedDate.Month.ToString();
                strYear = parsedDate.Year.ToString();

                // backend logic here
            }
            strModel = ddlModel;
            strFactory = ddlFactory;

            DateTime firstDayOfTheMonth = new DateTime(Int32.Parse(strYear), Int32.Parse(strMonth), 1);
            int LoopMaxDays = firstDayOfTheMonth.AddMonths(1).AddDays(-1).Day;

            DataTable dt = new DataTable();
            //DPR objDpr = new DPR(ddlFactory);
           objDPR.InitializeFactory(ddlFactory);
            dt = objDPR.GetDefectTrend(strMonth, strYear, strModel, strFactory);


            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td align='left' style='border: .5pt solid black;' valign='top' colspan='4'>");
            sb.Append(ddlModel + " (Defect Trend)");
            sb.Append("</td>");
            sb.Append("<td align='centre' style='border: .5pt solid black;' valign='top'>Date</td>");

            for (int i = 1; i <= LoopMaxDays; i++)
            {
                sb.Append("<td align='centre' style='border: .5pt solid black;' valign='top' colspan='2'>" + i.ToString() + "</td>");
            }

            sb.Append("</tr>");

            sb.Append("<tr>");
            sb.Append("<td align='left' style='border: .5pt solid black;' valign='top'>#</td>");
            sb.Append("<td align='left' style='border: .5pt solid black;' valign='top'>Defect Code</td>");
            sb.Append("<td align='left' style='border: .5pt solid black;' valign='top'>Defect Description</td>");
            sb.Append("<td align='left' style='border: .5pt solid black;' valign='top'>Defect Category</td>");
            sb.Append("<td align='left' style='border: .5pt solid black;' valign='top'>Defect Section</td>");

            for (int i = 1; i <= LoopMaxDays; i++)
            {
                sb.Append("<td align='left' style='border: .5pt solid black;' valign='top'>A</td>");
                sb.Append("<td align='left' style='border: .5pt solid black;' valign='top'>B</td>");
            }
            sb.Append("</tr>");

            string colNameA = "";
            string colNameB = "";
            string strNum = "";
            string strColIndex = "";
            int rowIndex = 0;
            foreach (DataRow dr in dt.Rows)
            {
                rowIndex += 1;

                sb.Append("<tr>");
                sb.Append("<td  align='left' style='border: .5pt solid black;' valign='top'>" + rowIndex.ToString() + "</td>");
                sb.Append("<td  align='left' style='border: .5pt solid black;' valign='top'>" + dr["DCODE"].ToString() + "</td>");
                sb.Append("<td  align='left' style='border: .5pt solid black;' valign='top'>" + dr["DEFECTDESCRIPTION"].ToString() + "</td>");
                sb.Append("<td  align='left' style='border: .5pt solid black;' valign='top'>" + dr["DISC"].ToString() + "</td>");
                sb.Append("<td  align='left' style='border: .5pt solid black;' valign='top'>" + dr["SUBCODE"].ToString() + "</td>");

                colNameA = "";
                colNameB = "";
                for (int j = 1; j <= LoopMaxDays; j++)
                {
                    strColIndex = "0" + j.ToString();

                    strNum = strColIndex.Substring(strColIndex.Length - 2);  //("0" + j.ToString()).Substring( - 2);
                    colNameA = "A_SHIFT_" + strNum;
                    colNameB = "B_SHIFT_" + strNum;

                    sb.Append("<td  align='left' style='border: .5pt solid black;' valign='top'>" + (String.IsNullOrEmpty(dr[colNameA].ToString()) ? "0" : dr[colNameA].ToString()) + "</td>");
                    sb.Append("<td  align='left' style='border: .5pt solid black;' valign='top'>" + (String.IsNullOrEmpty(dr[colNameB].ToString()) ? "0" : dr[colNameB].ToString()) + "</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            string strHTML = sb.ToString();

            return strHTML;
        }

        //******************************************* Below codes are not used *******************************************


        //protected void LoadShifts(object sender, EventArgs e)
        //{

        //}


        //#region"Function"
        //void PopulateShiftTimings()
        //{
        //    CommonFunctions d = new CommonFunctions();
        //    DataTable dtShift = d.GetShiftTimings(ddlFactory.SelectedValue);
        //    string frTiming = string.Empty;
        //    string toTiming = string.Empty;
        //    string frDate = string.Empty;
        //    string toDate = string.Empty;
        //    frDate = DateTime.ParseExact(txtDate.Text, "dd-MMM-yyyy", null).ToString("yyyyMMdd");

        //    foreach (DataRow dr in dtShift.Rows)
        //    {
        //        frTiming = frDate + "" + dr["START_TIME"].ToString();
        //        if (dr["CODE"].ToString() == "C")
        //        {
        //            toDate = DateTime.ParseExact(txtDate.Text, "dd-MMM-yyyy", null).AddDays(1).ToString("yyyyMMdd");
        //            toTiming = toDate + "" + dr["END_TIME"].ToString();
        //        }
        //        else
        //        {
        //            toDate = DateTime.ParseExact(txtDate.Text, "dd-MMM-yyyy", null).ToString("yyyyMMdd");
        //            toTiming = toDate + "" + dr["END_TIME"].ToString();
        //        }

        //        ListItem l = new ListItem();
        //        l.Text = dr["CODE"].ToString();
        //        l.Value = frTiming + "~" + toTiming;
        //        ddlShift.Items.Add(l);
        //    }
        //    ddlShift.Items.Insert(0, new ListItem("--All--", "0"));
        //}
        //void LoadShift()
        //{
        //    string frTimingA = string.Empty;
        //    string toTimingA = string.Empty;

        //    string frTimingB = string.Empty;
        //    string toTimingB = string.Empty;
        //    ListItem ShiftA = new ListItem();
        //    ListItem ShiftB = new ListItem();
        //    ListItem ShiftAll = new ListItem();
        //    switch (ddlFactory.SelectedValue)
        //    {
        //        case "6":
        //            frTimingA = "060000";
        //            toTimingA = "150000";

        //            frTimingB = "150000";
        //            toTimingB = "055959";

        //            ShiftA = new ListItem("A", frTimingA + "~" + toTimingA);
        //            ShiftB = new ListItem("B", frTimingB + "~" + toTimingB);
        //            ShiftAll = new ListItem("----All----", frTimingA + "~" + toTimingB);
        //            break;
        //        case "3":
        //            frTimingA = "063000";
        //            toTimingA = "150000";

        //            frTimingB = "150000";
        //            toTimingB = "062959";

        //            ShiftA = new ListItem("A", frTimingA + "~" + toTimingA);
        //            ShiftB = new ListItem("B", frTimingB + "~" + toTimingB);
        //            ShiftAll = new ListItem("----All----", frTimingA + "~" + toTimingB);
        //            break;
        //        case "8":
        //            frTimingA = "063000";
        //            toTimingA = "150000";

        //            frTimingB = "150000";
        //            toTimingB = "062959";

        //            ShiftA = new ListItem("A", frTimingA + "~" + toTimingA);
        //            ShiftB = new ListItem("B", frTimingB + "~" + toTimingB);
        //            ShiftAll = new ListItem("----All----", frTimingA + "~" + toTimingB);
        //            break;
        //        case "21":
        //            frTimingA = "063000";
        //            toTimingA = "150000";

        //            frTimingB = "150000";
        //            toTimingB = "062959";

        //            ShiftA = new ListItem("A", frTimingA + "~" + toTimingA);
        //            ShiftB = new ListItem("B", frTimingB + "~" + toTimingB);
        //            ShiftAll = new ListItem("----All----", frTimingA + "~" + toTimingB);
        //            break;
        //    }

        //    ddlShift.Items.Add(ShiftA);
        //    ddlShift.Items.Add(ShiftB);
        //    ddlShift.Items.Insert(0, ShiftAll);
        //}
        //#endregion
    }
}
