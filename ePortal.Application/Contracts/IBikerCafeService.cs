using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IBikerCafeService
    {
        Task<BC_VALIDATION_ViewModel> GetValidationData();
        Task<List<MealTypeViewModel>> GetMealTypeList(short mealCatID);
        Task<List<MealSlotViewModel>> GetMealSlots();
        Task<List<MealBookingTrnViewModel>> GetAvailabilityMealList(string fromDate, string toDate, long mealType, long loginUser);
        Task<Tuple<short, string>> SaveMealBooking(long AddedBy, List<MealBookingTrnViewModel> model);
        Task<short> CancelBooking(long id, long updatedBy);
        Task<List<MealBookingTrnViewModel>> GetBookedMealList(long mealtype, string fromDate, string toDate, long loginUser);
        //short SaveSubsidizedMealToken(SubsidizedMealTokenViewModel model);


        //-- Guest Meal Booking
        Task<List<GuestMealBookingTrnViewModel>> GetGuestBookedMealList(long mealtype, string fromDate, string toDate, long loginUser);
        Task<List<Meals_AvailabilityViewModel>> GetGuestAvailabilityMealList(string date, long? mealType);
        Task<Tuple<short, string>> SaveGuestMealBooking(long id, GuestMealBookingTrnViewModel model, List<GuestDtlViewModel> guestList);
        Task<short> CancelGuestMealBooking(long id, long updatedBy);

        Task<short> BulkGuestMealApproval(List<GuestMealBookingTrnViewModel> GMBTList);
        Task<List<GuestDtlViewModel>> GuestAutocomplete(string term);
        //GuestMealBookingTrnViewModel GetGuestRequestDtlById(long id);
        Task<GuestMealBookingTrnViewModel> GetGuestRequestDtlById(long id);
        Task<short> GuestMealApproval(GuestMealBookingTrnViewModel model);
        Task<Tuple<long, short>> GetGuestDtlByMno(string mNo, string bookingDate);

        // -- Ala Carte Food --//
        Task<List<AlaCarteHeaderViewModel>> GetAlaCarteRequestList(string fromDate, string toDate, short category, long loginUser);
        Task<List<AlaCarteItemMstViewModel>> GetAlaCarteItemList();
        Task<Tuple<short, string>> SaveAlaCarteOrder(long AddedBy, List<AlaCarteTrnViewModel> model);
        Task<AlaCarteHeaderViewModel> GetAlaCarteDtlById(long id);
        short CancelAlaCarteOrder(long id, long updatedBy);
        List<BcMenuViewModel> GetBCMenu();

        // -- Family Meal Booking -- //
        FamilMealBookingDtlViewModel GetMealByDate(long mealtype, DateTime bookingDate);
        Tuple<short, string> SaveFamilyMealBooking(FamilyMealBookingTrnViewModel model);
        List<FamilyMealBookingTrnViewModel> GetFamilyBookedMealList(long mealtype, string fromDate, string toDate, long loginUser);
        short CancelFamilyMealBooking(long id, long updatedBy);
        List<MealSlotViewModel> GetMealSlotsByBookingDate(string bDate);
        FamilyMealBookingTrnViewModel GetFamilyRequestDtlById(long id);

        // -- Meeting food
        List<MeetingFoodItemMstViewModel> GetMeetingFoodItemList();
        Tuple<short, string> SaveMeetingFoodOrder(long AddedBy, long OPID, string EVENTDATE, string EVENTTIME, string VENUE, string REMARKS, List<MeetingFoodTrnViewModel> model, string Noofguests, string desg);
        List<MeetingFoodHeaderViewModel> GetMeetingFoodRequestList(string fromDate, string toDate, short category, long loginUser);

        MeetingFoodHeaderViewModel GetMeetingFoodDtlById(long id);
        short MeetingFoodApproval(MeetingFoodHeaderViewModel model);
        short CancelMeetingFoodBooking(long id, long updatedBy);
    }
}
