using ePortal.DomainClasses;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ePortal.Persistence.Admin.Interface
{
    public interface ITexiRequest
    {
        public List<SelectListItem> ManageVehicleRequest(string strEmpCode);
        public DataSet ManageVehicleRequest1(string strEmpCode);
        public string AddVechilerequest(string strEmpCode, string strExtNo, string strPhoneNo, string strDateFrom,
                                        string strDateTo, string strReportingPlace, string strReportingTime, string strPurposeVisit,
                                        string strPlaceVisit, string strShift, string strNoofPerson, string strRemarks, string strAppCode, string strsiteid);
        public DataSet PendingVehicleRequest(string strSupEmpCode);
        public DataSet EditTaxiApproval(string Requestid, string Ecode);
        public string UpdateTaxiApproval(string strSupervisorEmpCode, string strID, string strApprovalStatus, string strRemarks,
            string strsupsupempcode, string stroperation);
        public string UpdateAdminTaxiApproval(string strAdminEmpCode, string strID, string strApprovalStatus, string strRemarks,
            string strTaxiNo, string strVendorName, string strAmt, string strtaxitype);
        public DataSet AdminVehicleRequest(string Type);
        public VehicleReleased VehicleRequestById(string strTransID, string Did);
        public DataSet get_AllTaxiRequest(string Type);
        public string UpdateTaxiRequest(string strShift, string strExtNo, string strPhoneNo, string strFrmdate,
      string strTodate, string strReportingPlace, string strReportingTime, string strPurposeVisit,
      string strPlaceVisit, string strNoofPerson, string strRemarks, string strAppAuth, string strID);
        public string CancelTaxiRequest(string strID, string strCancelRequest);
        public DataSet GetTaxiRecord(string struserid, string strfromdate, string strtodate);
        public int CheckStatus(string struserid);
        public DataSet getDivisionHead(string strrequesterempcode);
        public DataSet get_AllDivHead(string strrequesterempcode);
        public DataTable GetVendor();
        public string AdminsideAddVechilerequest(string strEmpCode, string strExtNo, string strPhoneNo, string strDateFrom,
            string strDateTo, string strReportingPlace, string strReportingTime, string strPurposeVisit,
            string strPlaceVisit, string strShift, string strNoofPerson, string strRemarks, string strsite, string strappcode);
        public DataSet VehicleApprovalHistory(string strEmpCode);
        public string isValidForTaxiBooking(string userID);
        public DataTable get_TaxiApprovalAuthList(string strEmpcode);
        public string AddVechiledetail(string strvehiclerequestid, string strvehicleno, string strmeterreading, string strdrivername,
                                         string strdriverno, string strAppCode);
        public string UpdateVechiledetail(string strvehiclerequestid, string strvehicleno, string strmeterreading, string strdrivername,
                                           string strdriverno, string strAppCode, string strreleasedby, string strapprovedby, string strremark, string slipno);
        public string UpdateVechileReleased(string strvehiclerequestid, string strkmused, string strreleaseat, string strvisiteplace, string strtaxicondition, string strremarks);
        public DataTable get_TaxiRequestGateList(string strEmpcode, int strsite, string strsysdate);
        public DataTable get_TaxiRelease(string strEmpcode, string strsite, string strreqid, string strstatus, string strfromdate, string strtodate);
        public DataTable Get_TaxiusesReport(string strEmpcode, string strsite, string strfromdate, string strtodate, string strvenderid);
        public DataTable Get_TaxiConditionReport(string strsite, string strfromdate, string strtodate, string strvenderid);
        public DataTable Get_TaxiAdminApprovalList();
        public DataTable Get_TaxiTypeList(string strsiteid, string strtaxitypeid);
        public string AddUpdateTaxiType(string stryaxitypeid, string strtaxitype, string strrate4_40, string strrate8_80,
                                           string strrate_perkm, string strrate_perhr, string strrate_night, string strsite,
                                           string strstatus, string straddedby);
        public Tuple<string, string> SaveTaxiRequest(TaxiRequestRequest obj, string UserName, string UserId);
        public VehicleRequestModel VehicleRequestDetails(string strTransCode);
        public TaxiRequestViewModel CancelRequestDetails(string strRequestCode);
        public Tuple<string, string> SubmitCancelTaxiRequest(TaxiRequestRequest objModel, string userId, string userName);
        public TaxiRequestViewModel EditRequestDetails(string strRequestCode);
        public Tuple<string, string> EditTaxiRequest(TaxiRequestRequest obj, string UserName, string UserId);
        public Tuple<string, string> TaxiApproval(TaxiRequestRequest obj, string UserName, string UserId);
    }
}
