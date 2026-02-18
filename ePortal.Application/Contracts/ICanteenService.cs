using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface ICanteenService
    {
        //Canteen Meal Booking
        //List<CSDValidationViewModel> GetValidationList();
        Task<string> GetColValue(long mealTypeId, long mealOpID, string col_name);

        //string GetParmvalue(string PARM_NAME);
        List<SelectListViewModel> GetApprovalAuth(long loginUser);
        Task<List<CanteenMealBookingViewModel>> GetBookingHistory(long mealtype, string fromDate, string toDate, long loginUser);
        Task<List<CSDMealTypeViewModel>> GetMealTypeList(long plantId);
        Task<List<MealOptionMstViewModel>> GetMealOptionList();
        Task<List<CSDMealsAvailabilityViewModel>> GetAvailabilityMealList(string fromDate, string toDate, long mealType, long MEALOPTIONID, long loginUser, long plantId);
        Task<Tuple<short, string>> SaveMealBooking(long AddedBy, CanteenMealBookingViewModel model);
        Task<short> CancelBooking(long id, long updatedBy);

        //-- Guest Meal Booking
        List<CSDGuestMealBookingTrnViewModel> GetGuestBookedMealList(long mealtype, string fromDate, string toDate, long loginUser);
        List<CSDMealsAvailabilityViewModel> GetGuestAvailabilityMealList(string date, long? mealType, long mealOptID, long plantId);
        Tuple<short, string> SaveGuestMealBooking(long id, CSDGuestMealBookingTrnViewModel model, List<GuestDtlViewModel> guestList);
        short CancelGuestMealBooking(long id, long updatedBy);
        Tuple<long, short> GetGuestDtlByMno(string mobileNo, string bookingDate, int mealOption);
        List<GuestDtlViewModel> GuestAutocomplete(string term);
        Task<CSDGuestMealBookingTrnViewModel> GetGuestRequestDtlById(long _ReqId);
        Task<short> GuestMealApproval(CSDGuestMealBookingTrnViewModel model);
        short BulkGuestMealApproval(List<CSDGuestMealBookingTrnViewModel> GMBTList);

        ////BakeryItem

        List<CSDBakeryItemViewModel> BakeryItem();

        #region MealFeedback
        Task<List<CanteenMealBookingViewModel>> GetBookingFeedbackHistory(long mealtype, string fromDate, string toDate, long loginUser, long plantId);
        Task<List<CanteenMealBookingViewModel>> GetAllBookingFeedbackLst(long mealtype, string fromDate, string toDate, long loginUser, long plantId, long mealRating);
        List<MealBookingExcelVM> GetMealFeedbackExcelData(long mealtype, string fromDate, string toDate, long plantId, long mealRating);
        short BookingFeedback(CanteenMealBookingViewModel model);
        List<FeedbackPointViewModel> GetBookingFeedback(long bookingId, long mealType, long userId);
        #endregion
    }
}
