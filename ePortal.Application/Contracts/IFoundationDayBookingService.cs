using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IFoundationDayBookingService
    {

        // -- Family Meal Booking -- //
        FD_VALIDATION_ViewModel GetValidationData(long siteid);
        Tuple<short, string> SaveFamilyMealBooking(FoundationDayBookingTrnViewModel model);
        List<FoundationDayBookingTrnViewModel> GetFamilyBookedMealList(long mealtype, string fromDate, string toDate, long loginUser);
        short CancelBooking(long id, long updatedBy);
        FoundationDayBookingTrnViewModel GetRequestDtlById(long id);
        SearchFDREPORT_ViewModel FDReport(SearchFDREPORT_ViewModel vm);

    }
}
