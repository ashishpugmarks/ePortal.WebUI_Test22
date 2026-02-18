using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IBikerCafe_DAL
    {
        DataTable GetMealTypeList();
        DataTable GetMealMstList(string MealType, string strstatus);
        DataTable GetMealAllocationList(string MealType, string Status, string FromDate, string TillDate);
        string InsertMealAllocation(long ID, long MealTypeId, string MappingDate, long MealId, short status, long addedby);
        DataTable GetMealdatewiseList(string ecode, string FromDate, string TillDate, string MealType, string reporttype);
        string MealType_Set(string strCategoryID, string strDescription, string strReader, string strStatus, string strEmpcode);
        string UpdateAvailabilityStatus(long ID, short status, long updatedBy);
        string AddMeal_Set(string strMealID, string strMealTypeID, string strSMealName, string strSMealDesc, string mealPrice, string mealPhotoName, byte[] mealPhoto, string strStatus, string strUserID);
        string SaveSubsidizedMealToken(long ID, string TOKEN_CODE, long EMPLOYEE_ID, string TOKEN_DATE, short STATUS, long ADDEDBY);
        DataTable SubsidizedMealTokenList(string TOKEN_CODE, long EMPLOYEE_ID, string STATUS, string FromDate, string TillDate);
        DataTable OffdayList(string FromDate, string TillDate, string strmealtype);
        string UpdateSubsidizedAmt(Int64 TRNID, Int64 SubsidizedTokenID, Int64 EmpCode, string strTotalAmt, string strSubsidizedAmt, string strPayableAmt, string strStatus, string strUserID);
        DataTable GetSubsidizedDataByOtp(string otp, string tokenDate);
        DataTable GetSubsidizedReport(string ecode, string FromDate, string TillDate);
        DataTable GetMealBookingData(string MealType, string FromDate, string TillDate, long loginUser, int type, int mealStatus);
        DataTable GetValidationData();
        string InsertMealBooking(long ID, long MealTypeId, string bookingDate, short status, long addedby, int validBookingCount, long slotID);
        string BookingCancellation(long ID, short status, long updatedBy);
        DataSet ManageGuestMealApproval(string strSupEmpCode);
        string UpdateMealTakenStatus(Int64 TRNID, Int64 GuestDTLID, string strStatus, string strUserID);
        DataTable GetGuestMealBookingByOtp(string otp, string bookingDate);
        DataTable GetGuestMealReqList(string ecode, string FromDate, string TillDate, string MealType);
        DataTable GetMealCountList(string FromDate, string TillDate, string MealType);
        DataTable GetMealReqCostList(string ecode, string FromDate, string TillDate, string MealType);
        string UpdateValidationMst(long BC_ValidationId, string strUserID, string MealUpdatePeriod, string MealChangeTime, string MealBookingDuration, string MealBookingTime,
              string MealCancellationTime, string MealBookingDay, string IndQty, string JapQty, string AlaCarteSubsidyPercentage,
              string AlaCarteMaxSubsidy, string AlaCarteBookingTime, string GuestBookingTime, string GuestCancellationTime,
              string GuestBookingDay, string SelfAppDesg, string JapBookingTime, string JapCancellationTime, string JapBookingDay, string FmyVisitEndDate, string FmyVisitQty,
              string AlaOTPAcceptTime);
        DataTable BindDesignation();
        DataTable GetHRIDMappingData(string id, string ecode, string status);
        string InsertHRCodeMapping(string ecode, string hridCode, string saviorCode, string status, string addedby, string mappingId);
        DataTable GetMealBookingByAdmin(string MealType, string FromDate, string TillDate, long loginUser, int type, int mealStatus);
        string InsertMealBookingByAdmin(long ID, long empCode, long MealTypeId, string bookingDate, short status, long requestBy, int validBookingCount, Int64 slotID);
        string BookingCancellationByAdmin(long ID, short status, long updatedBy);
        DataTable GetMealSlot(string MealType);
        DataTable GetAlaCarteOrderByAdmin(string FromDate, string TillDate, string ecode, int status);
        DataSet GetOrderDetailById(long id);
        string UpdateOrderStatus(long hdrId, long trnId, Int16 item_status, Int16 order_status, string addedby);
        DataTable GetAlacarteTypeList();
        DataTable GetAlacarteMstList(string MealType, string strstatus);
        string AddAlaCarte_Set(string strMealID, string strMealTypeID, string strSMealName, string strSMealPreTime, string mealPrice, string mealPhotoName, byte[] mealPhoto, string strTakeway, string strDinein, string strStatus, string strUserID);
        DataTable GetFamilyMealCountList(string FromDate, string TillDate, string MealType);
        DataTable GetFamilyMealBookingByOtp(string otp, string bookingDate);
        DataTable GetFamilyReqdatewiseList(string FromDate, string TillDate, string reporttype);
        DataTable SlotDateList(string FromDate, string TillDate, string strmealtype);
        string InsertUpdateSlotAllocation(long slotTypeID, string mappingDate, short status, long addedby);
        DataTable GetMeetingfoodMstList(string MealType, string strstatus);
        string AddMeetingfood_Set(string strMealID, string strMealTypeID, string strSMealName, string mealPrice, string mealPhotoName, byte[] mealPhoto, string strStatus, string strUserID);
        DataSet ManageMeetingFoodApproval(string strSupEmpCode);
        DataTable GetMeetingFoodByAdmin(string FromDate, string TillDate, string ecode, int status);
        DataSet GetMeetingfoodOrderDetailById(long id);
        string SUBMITADMINMEETINGFOOD(string HDID, string strempid, string strRemarks, string strstatus);
        DataTable GetMeetingFoodByCafe(string FromDate, string TillDate, string ecode, int status);
        DataSet GetMeetingfoodOrdercafeById(long id);
        string SUBMITCAFEMEETINGFOOD(string HDID, string strempid, string strRemarks, string strstatus, string strXml, string STRATTACHMENT);
        DataTable GetMeetingFoodReqList(string ecode, string FromDate, string TillDate);
    }
}
