using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    using System.Data;

    public interface IBusRoute
    {

        // Data retrieval methods
        public DataSet GetBusRoute(string strSiteID);
        public DataSet GetBusRouteFromStop(string strStopID);
        public DataSet GetBusStop(string strBusRouteId);
        public string GetMonthlyPay(string strStopId);
        // Route change request methods
        public string SubmitRouteRequest(string strEmpCode, string strrequest, string strcurrentroute, string strcurrentstop, string strnewroute, string strnewstop, string straddress, string strremark, string strdate, string strpay);

        public DataSet EditRouteChange(string strRequestId);
        public string UpdateRouteRequest(string strRequestId, string strrequest, string strcurrentroute, string strcurrentstop, string strnewroute, string strnewstop, string straddress, string strremark, string strdate, string strpay);

        public DataSet ManageRouteChange(string strEmpCode);
        public DataSet ManageRouteChange1(string strEmpCode);
        public DataSet GetRequestForCancel(string strRequestId);
        public string CancelRouteChangeRequest(string strRequestId, string strCancelRemark);
        public DataSet ADGetBusStop();
        public string UpdateRequestByAdmin(string strRequestId, string stradempcode, string strstatus, string stradminremark);
        public DataSet GetAminEmailId();
        public DataSet GetRouteChngReq(string strEmpCode, string strRequest, string strDateFrom, string strDateTo, string strRoute,
            string strStop, string strStatus, string strKiId, string strSiteID);
        public DataTable EditEvalParam(string evalid);
        public DataTable EditBusCharges(string evalid);
        public DataTable EditStopDetails(string evalid);
        public DataTable EditVendorDetails(string evalid);
        public DataTable GetChargeDetails();
        public DataTable GetCharges(string chargeid);
        public string AddEditBusRoute(string userID, string id, string routecode, string desc, string status, string strSiteID);
        public string AddEditVendorDetails(string userID, string id, string VendorName, string emailid, string contactpersname, string contactno, string status, string strsiteid, string strrkm);
        public string AddEditBusStop(string userID, string routeID, string id, string routecode, string descrip, string charges, string status, string costcode, string strgarage);
        public string AddEditBusRouteCharges(string userID, string id, string codedesc, string mindist, string maxdist, string cost);
        public DataSet GetBusStopList(string strSiteId);
    }


}
