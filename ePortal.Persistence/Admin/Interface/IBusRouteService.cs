using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IBusRouteService
    {
        IEnumerable<SiteModel> SelectSysite();
        // DataSet SelectSysite();
        IEnumerable<BusStopModel> GetBusStopsListAsync(string siteId);
        IEnumerable<BusRouteModel> GetCurrentRouteAsync(string strStopID);
        IEnumerable<BusStopModel> GetNewStopAsync(string siteId);
        // String GetMonthlyPay(string strStopId);
        DataSet GetBusRouteFromStop1(string strStopID);
       // DataSet GetRouteChangeRequestData(string strRequestId);
        DataSet GetBusRoute(string strSiteID);
        DataSet GetBusStop(string strBusRouteId);
        String SubmitRouteRequest(string strEmpCode, string strrequest, string strcurrentroute, string strcurrentstop, string strnewroute, string strnewstop, string straddress, string strremark, string strdate, string strpay);
        DataSet EditRouteChange(int strRequestId);
        String UpdateRouteRequest(string strRequestId, string strrequest, string strcurrentroute, string strcurrentstop, string strnewroute, string strnewstop, string straddress, string strremark, string strdate, string strpay);
        DataSet ManageRouteChange(string strEmpCode);
        DataSet GetRequestForCancel(string strRequestId);
        string CancelRouteChangeRequest(string strRequestId, string strCancelRemark);
        String UpdateRequestByAdmin(string strRequestId, string stradempcode, string strstatus, string stradminremark);
        DataSet GetAdminEmailId();
        DataSet GetRouteChngReq(string strEmpCode, string strRequest, string strDateFrom, string strDateTo, string strRoute,
        string strStop, string strStatus, string strKiId, string strSiteID);
        DataTable EditEvalParam(string evalid);
        DataTable EditBusCharges(string evalid);
        DataTable EditStopDetails(string evalid);
        DataTable EditVendorDetails(string evalid);
        DataTable GetChargeDetails();
        DataTable GetCharges(string chargeid);
        String AddEditBusRoute(string userID, string id, string routecode, string desc, string status, string strSiteID);
        String AddEditVendorDetails(string userID, string id, string VendorName, string emailid, string contactpersname, string contactno, string status, string strsiteid, string strrkm);

        String AddEditBusRouteCharges(string userID, string id, string codedesc, string mindist, string maxdist, string cost);

        String AddEditBusStop(string userID, string routeID, string id, string routecode, string descrip, string charges, string status, string costcode, string strgarage);
        DataSet GetBusStopList(string strSiteId);
        IEnumerable<BusRouteModel>? GetBusRouteFromStop(string stopId);
        String GetMonthlyPayFromDatabase(int strStopId);
        RouteChangeRequest GetRouteChangeRequest(long requestId);
        bool SentMailforSubmit(string strEmpCode, string userName, string strRequest, string strDate, string strAddress);
        bool SentMailforCancel(string strEmpCode, string userName, string strRequest, string strRequestId, string strDate, string strAddress,string Remarks);
        bool SentMailforUpdate(string strEmpCode, string userName, string strRequest, string strRequestId, string strDate, string strAddress);
    }
}
