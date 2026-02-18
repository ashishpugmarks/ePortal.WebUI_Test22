

using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class FoundationDayBookingService : IFoundationDayBookingService
    {
        FoundationDayBookingRepository _bcRepo;
        Tuple<short, long> _retVal_tuple;

        public FoundationDayBookingService(FoundationDayBookingRepository obkFoundationDayBookingRepository)
        {
            _bcRepo = obkFoundationDayBookingRepository;
            _retVal_tuple = new Tuple<short, long>(0, 0);
        }
        // -- Family Meal Booking --//
        public FD_VALIDATION_ViewModel GetValidationData(long siteid)
        {
            return _bcRepo.GetValidationData(siteid);
        }
        
        public Tuple<short, string> SaveFamilyMealBooking(FoundationDayBookingTrnViewModel model)
        {
            return _bcRepo.SaveFamilyMealBooking(model);
        }

        public List<FoundationDayBookingTrnViewModel> GetFamilyBookedMealList(long mealtype, string fromDate, string toDate, long loginUser)
        {
            return _bcRepo.GetFamilyBookedMealList(mealtype, fromDate, toDate, loginUser);
        }

        public short CancelBooking(long id, long updatedBy)
        {
            return _bcRepo.CancelBooking(id, updatedBy);
        }
        
        public FoundationDayBookingTrnViewModel GetRequestDtlById(long id)
        {
            return _bcRepo.GetRequestDtlById(id);
        }
        public SearchFDREPORT_ViewModel FDReport(SearchFDREPORT_ViewModel vm)
        {
            return _bcRepo.FDReport(vm);
        }
    }
}
