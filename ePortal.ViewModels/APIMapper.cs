using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public static class APIMapper
    {
        public const string GetVendorList = "api/PO/get-vendor-list";
        public const string SavePORequest = "api/PO/SavePORequest";
        public const string SavePODetails = "api/PO/SavePODetails";
        public const string SavePOMapping = "api/PO/SavePOMapping";
        public const string SaveAppAuthSeq = "api/PO/SaveAppAuthSeq";
        public const string SavePOAppHis = "api/PO/SavePOAppHis";
        public const string SaveSkipAuth = "api/PO/SaveSkipAuth";
        public const string GetAuthEmpById = "api/PO/GetAuthEmpById";
        public const string GetPORequestById = "api/PO/get-po-detail-by-id?id={0}";
        public const string POApproval = "api/PO/post-po-approval";
        public const string POCancel = "api/PO/POCancel";
        public const string GetAttachmentDetail = "api/PO/GetAttachmentDetail";
        public const string DeleteAttachment = "api/PO/DeleteAttachment";
        public const string GetPRRequestById = "api/PR/get-pr-request-by-id?id={0}";
        public const string GetPRDetailByPOId = "api/PO/get-pr-detail-by-po-id?PoId={0}";
        public const string GetPRStatusByPOId = "api/PO/GetPRStatusByPOId";
        public const string GetPODASHBOARD = "api/PO/GetPODASHBOARD";
        public const string UpdateMailCNT = "api/PO/UpdateMailCNT";
        public const string BindDivision = "api/PO/BindDivision";
        public const string PRApproval = "api/PR/post-pr-approval";
        public const string GetACRRequestById = "api/ACR/get-acr-request-by-id?id={0}";
        public const string ACRApproval = "api/ACR/post-acr-approval";
        public const string getEmployeeDetails = "api/auth/employee-detail?username={0}";
        public const string getActionRecordDetails = "api/Dashboard/getActionRecordDetails?id={0}";
        public const string GetEmpdetailAsync = "api/auth/employee-detail?username={0}";
        public const string VerifyExtAuthOtp = "api/MobileApp/VerifyExtAuthOtp";
        public const string GetGuestRequestDtlById = "api/Canteen/get-guest-request-dtl-by-id?id={0}";
        public const string GetColValue = "api/Canteen/get-col-value";
        public const string GuestMealApproval = "api/Canteen/guest-meal-approval";
        public const string IOM_GetIOMRequestById = "api/IOM/get-iom-request-by-id?id={0}";
        public const string IOM_IOMApproval = "api/IOM/iom-approval";
        public const string IOM_SaveIOMDetails = "api/IOM/save-iom-details";
        public const string IOM_GetAttachmentDetail = "api/IOM/get-attachment-detail";
        public const string SaveACRDetails = "api/ACR/save-acr-details";
        public const string ACR_GetAttachmentDetail = "api/ACR/get-attachment-detail";
        public const string post_mobile_app_log = "api/Auth/post-mobile-app-log";
        public const string get_mobile_app_log = "api/Auth/get-mobile-app-log";
        public const string GetPONextApprovalId = "api/PO/GetPONextApprovalId";
        public const string SaveUserPin = "api/Auth/save-user-pin";
        public const string GetPinCreatedOrNot = "api/Auth/get-pin-created-or-not";
        public const string IOM_GetIOMNextApprovalId = "api/IOM/GetIOMNextApprovalId";

        public static class ICBM
        {
            public const string GetICRequestById = "api/ICIBM/GetICRequestById";
            public const string GetICAssetRequestById = "api/ICIBM/GetICAssetRequestById";
            public const string ICApproval = "api/ICIBM/ICApproval";
            public const string ICAssetApproval = "api/ICIBM/ICAssetApproval";
        }

        public static class WFH
        {
            public const string GetApprovalAuth = "api/WFH/GetApprovalAuth";
            public const string UpdateWFHApproval = "api/WFH/UpdateWFHApproval";
            public const string GetWFHRequestDetail = "api/WFH/GetWFHRequestDetail";
            public const string GetOffDays = "api/WFH/GetOffDays";
            public const string GetParameterValue = "api/WFH/GetParameterValue";
            public const string GetRejectedRequest = "api/WFH/GetRejectedRequest";
        }

        public static class Account
        {
            public const string MaGetVersionInfo = "api/Auth/MaGetVersionInfo";
        }

        public static class Utility
        {
            public const string get_mail_details = "api/Utility/get_mail_details";
            public const string get_request_header = "api/Utility/get_request_header";
            public const string get_user_details = "api/Utility/get_user_details";
            public const string EditUtReqBySctMgr = "api/Utility/EditUtReqBySctMgr";
            public const string EditUtReqByDptMgr = "api/Utility/EditUtReqByDptMgr";
        }

        public static class Taxi
        {
            public const string getDivisionHead = "api/TaxiRequest/getDivisionHead";
            public const string GetEmployeeOfficialDetails = "api/TaxiRequest/GetEmployeeOfficialDetails";
            public const string GetParameterValue = "api/TaxiRequest/GetParameterValue";
            public const string get_AllDivHead = "api/TaxiRequest/get_AllDivHead";
            public const string get_AppStatus = "api/TaxiRequest/get_AppStatus";
            public const string get_request_header = "api/TaxiRequest/get_request_header";
            public const string UpdateTaxiApproval = "api/TaxiRequest/UpdateTaxiApproval";
        }
        public static class Tour
        {
            public const string requestheader = "api/TourRequest/requestheader";
            public const string requestdetail = "api/TourRequest/requestdetail";
            public const string citycategories = "api/TourRequest/citycategories";
            public const string employeeallowancedetail = "api/TourRequest/employeeallowancedetail";
            public const string tourbudget = "api/TourRequest/tourbudget";
            public const string tourbudgethis = "api/TourRequest/tourbudgethis";
            public const string requestpredetail = "api/TourRequest/requestpredetail";
            public const string touradvancedata = "api/TourRequest/touradvancedata";
            public const string appauthorities = "api/TourRequest/appauthorities";
            public const string strGetSpecialAppAuthorityList = "api/TourRequest/GetSpecialAppAuthorityList";
            public const string strGetAppAuthorityList = "api/TourRequest/GetAppAuthorityList";
            public const string strGetAppAuthCode = "api/TourRequest/GetAppAuthCode";
            public const string strUpdateApprovalStatus = "api/TourRequest/UpdateApprovalStatus";
            public const string strInsertTravelBudgetHistory = "api/TourRequest/InsertTravelBudgetHistory";
            public const string tourrequestdetail = "api/TourRequest/requestdetail";
        }
        public static class Announcement
        {
            public const string GetAnnouncementApprovalList = "api/Announcement/GetAnnouncementApprovalList";
            public const string UpdateStatus = "api/Announcement/UpdateStatus";
            public const string GetAnnouncementApprovalAuthorityById = "api/Announcement/GetAnnouncementApprovalAuthorityById";
            public const string GetFileForDownload = "api/Announcement/GetFileForDownload";
        }
        public static class Communication
        {
            public const string GetCommunicationDetails = "api/Announcement/GetCommunicationDetails";
            public const string CommunicationApproval = "api/Announcement/CommunicationApproval";
            public const string SaveCommAttachment = "api/Announcement/SaveCommAttachment";
        }
        public static class ESS
        {
            public const string GetAccessReportAsync = "api/ESS/GetAccessReportAsync";
            public const string GetRealTimeAttendanceAsync = "api/ESS/GetRealTimeAttendanceAsync";
            public const string GetEmployeeDataAsync = "api/ESS/GetEmployeeDataAsync";
            public const string GetAttendanceAsync = "api/ESS/GetAttendanceAsync";
            public const string GetEmployeesAsync = "api/ESS/GetEmployeesAsync";
        }
        public static class BikerCafe
        {
            public const string GetGuestRequestDtlById = "api/BikerCafe/GetGuestRequestDtlById";
            public const string GetValidationData = "api/BikerCafe/GetValidationData";
            public const string GuestMealApproval = "api/BikerCafe/GuestMealApproval";
            public const string GetMealTypeList = "api/BikerCafe/GetMealTypeList";
            public const string GetMealSlots = "api/BikerCafe/GetMealSlots";
            public const string GetAvailabilityMealList = "api/BikerCafe/GetAvailabilityMealList";
            public const string SaveMealBooking = "api/BikerCafe/SaveMealBooking";
            public const string CancelBooking = "api/BikerCafe/CancelBooking";
            public const string GetBookedMealList = "api/BikerCafe/GetBookedMealList";
            public const string GetGuestBookedMealList = "api/BikerCafe/GetGuestBookedMealList";
            public const string GetGuestAvailabilityMealList = "api/BikerCafe/GetGuestAvailabilityMealList";
            public const string SaveGuestMealBooking = "api/BikerCafe/SaveGuestMealBooking";
            public const string CancelGuestMealBooking = "api/BikerCafe/CancelGuestMealBooking";
            public const string BulkGuestMealApproval = "api/BikerCafe/BulkGuestMealApproval";
            public const string GuestAutocomplete = "api/BikerCafe/GuestAutocomplete";
            public const string GetGuestDtlByMno = "api/BikerCafe/GetGuestDtlByMno";
            public const string GetAlaCarteRequestList = "api/BikerCafe/GetAlaCarteRequestList";
            public const string GetAlaCarteItemList = "api/BikerCafe/GetAlaCarteItemList";
            public const string SaveAlaCarteOrder = "api/BikerCafe/SaveAlaCarteOrder";
            public const string GetAlaCarteDtlById = "api/BikerCafe/GetAlaCarteDtlById";
            public const string CancelAlaCarteOrder = "api/BikerCafe/CancelAlaCarteOrder";
            public const string GetBCMenu = "api/BikerCafe/GetBCMenu";
            public const string GetMeetingFoodItemList = "api/BikerCafe/GetMeetingFoodItemList";
            public const string SaveMeetingFoodOrder = "api/BikerCafe/SaveMeetingFoodOrder";
            public const string GetMeetingFoodRequestList = "api/BikerCafe/GetMeetingFoodRequestList";
            public const string GetMeetingFoodDtlById = "api/BikerCafe/GetMeetingFoodDtlById";
            public const string MeetingFoodApproval = "api/BikerCafe/MeetingFoodApproval";
            public const string CancelMeetingFoodBooking = "api/BikerCafe/CancelMeetingFoodBooking";
            public const string SendTokenMail = "api/BikerCafe/SendTokenMail";
            public const string SendSMS = "api/BikerCafe/SendSMS";
        }
        public static class Canteen
        {
            public const string CancelBooking = "api/Canteen/CancelBooking";
            public const string GetAvailabilityMealList = "api/Canteen/GetAvailabilityMealList";
            public const string GetBookingHistory = "api/Canteen/GetBookingHistory";
            public const string GetColValue = "api/Canteen/GetColValue";
            public const string GetMealOptionList = "api/Canteen/GetMealOptionList";
            public const string GetMealTypeList = "api/Canteen/GetMealTypeList";
            public const string SaveMealBooking = "api/Canteen/SaveMealBooking";
        }
    }
}
