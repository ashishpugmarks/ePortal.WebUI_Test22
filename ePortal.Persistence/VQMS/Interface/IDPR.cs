using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.VQMS.Interface
{
    public interface IDPR
    {
        DataTable DefectWiseReprot(string V_SHOP, string V_DATEFROM, string V_DATETO, string V_MODEL);
        DataTable GetDefectTrend(string Month, string Year, string Model, string Factory);
        DataTable GetDPRStatusCount(string Date, string SiteID, string Shift, string Line);
        DataTable GetDPRStatusPercentage(string Date, string SiteID, string Shift, string Line);
        DataTable GetLine(string SitedID);
        List<string> GetMisReportUser();
        DataTable GetModel(string SitedID);
        DataTable GetReportDefectCategorywise(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportDefectCategorywiseCnt(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportDefectDetails(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportDefectShopCnt(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportDefectShopwise(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportHeaderDetails(string strDate, string strModel, string strSiteID);
        DataTable GetReportHeaderDetailsDP(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportHeaderDetailsFN(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportHeaderDetailsProd(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportHeaderDetailsRVQ(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportHeaderDetailsSP(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetReportHeaderDetailsVQ(string strDate, string strModel, string strSiteID, string strline);
        DataTable GetSectionWiseDefect(string Date, string SiteID, string Shift, string Line);
        void InitializeFactory(string Factory);
        DataTable ModelWiseReprot(string V_SHOP, string V_DATEFROM, string V_DATETO);
        DataTable ShopWiseReprot(string V_DATEFROM, string V_DATETO);
    }
}
