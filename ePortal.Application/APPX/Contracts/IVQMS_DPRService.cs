using ePortal.ViewModels.APPX.VQMS_DPR;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.APPX.Contracts
{
    public interface IVQMS_DPRService
    {
        Task<string> ExportDefectTrendRpt(string txtDate, string ddlModel, string ddlFactory);
        Task<List<object>> FillChart(string txtdatefrom, string ddlSite);
        Task<string> GenerateHTMLReport(string txtDate, string ddlModel, string ddlSite, string ddline);
        Task<List<object>> getdefectwise_Report(string strShop, string strDatefrom, string strModel, string strSiteID);
        Task<string> GetDocType(string ddlSite);
        Task<List<object>> getModelwiseDefect_Report(string strShop, string strDatefrom, string strDateto, string strSiteID);
        Task<CategoryWiseReportViewModel> LoadCategorywiseData(string txtDate, string ddlModel, string ddline, string ddlSite);
        Task<DefectReportViewModel> LoadDefectDataRows(string txtDate, string ddlSite, string ddline, string ddlModel);
        Task<List<CategoryWiseGraphRow>> LoadGraphCategorywise(string txtDate, string ddlModel, string ddline, string ddlSite);
        Task<List<SectionWiseGraphRow>> LoadGraphSectionWise(string txtDate, string ddlModel, string ddline, string ddlSite);
        Task<IEnumerable<SelectListItem>> LoadLineDropdownlist(string ddlSite);
        Task<IEnumerable<SelectListItem>> LoadModelDropdownlist(string ddlSite);
        Task<ReportHeaderViewModel> LoadReportHeader(string txtDate, string ddlModel, string ddline, string ddlSite);
        Task<SectionWiseReportViewModel> LoadSectionWiseData(string txtDate, string ddlModel, string ddline, string ddlSite);
        Task<DailyPassRatioRptViewModel> SearchDailyPassRatioRpt(string ddlFactory, string txtDate, string ddlShift);
    }
}
