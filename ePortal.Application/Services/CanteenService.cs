using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using Newtonsoft.Json;
using System.Net.Http;
using ePortal.DomainClasses;

namespace ePortal.Application.Services
{
    public class CanteenService : ICanteenService
    {
        CanteenRepository _csdRepo;
        Tuple<short, long> _retVal_tuple;
        private readonly IApiClient _apiClient;

        public CanteenService(CanteenRepository csdRepo, IApiClient apiClient)
        {
            _csdRepo = csdRepo;
            _retVal_tuple = new Tuple<short, long>(0, 0);
            _apiClient = apiClient;
        }


        //public List<CSDValidationViewModel> GetValidationList()
        //{
        //    return _csdRepo.GetValidationList();
        //}

        public async Task<string> GetColValue(long mealTypeId, long mealOpID, string col_name)
        {
            return _csdRepo.GetColValue(mealTypeId, mealOpID, col_name);

            //Error
            // in mobile app its a GET call
            //try
            //{
            //    string PARM_VALUE = string.Empty;

            //    var models = new
            //    {
            //        mealTypeId = mealTypeId,
            //        mealOpID = mealOpID,
            //        col_name = col_name
            //    };

            //    var response = await _apiClient.PostAsync<List<CSD_MealTimeMapViewModel>>(APIMapper.GetColValue,  models );

            //    var result = response.Where(w => (models.mealTypeId == 0 ? 1 == 1 : w.MEALTYPEID == models.mealTypeId) && (models.mealOpID == 0 ? 1 == 1 : w.MEAL_OPTIONID == models.mealOpID)).FirstOrDefault();
            //    if (result != null)
            //    {
            //        PARM_VALUE = result.GetType().GetProperties().Where(a => a.Name == models.col_name).Select(p => p.GetValue(result, null)).FirstOrDefault().ToString();
            //    }

            //    return PARM_VALUE;

            //}
            //catch (Exception ex)
            //{
            //    return string.Empty;
            //}
        }

        //public string GetParmvalue(string PARM_NAME)
        //{
        //    return _csdRepo.GetParmvalue(PARM_NAME);
        //}

        public List<SelectListViewModel> GetApprovalAuth(long loginUser)
        {
            return _csdRepo.GetApprovalAuth(loginUser);
        }

        public async Task<List<CSDMealTypeViewModel>> GetMealTypeList(long plantId)
        {
            return _csdRepo.GetMealTypeList(plantId);

            //try
            //{

            //    var response = await _apiClient.PostAsync<List<CSDMealTypeViewModel>>(string.Format(APIMapper.Canteen.GetMealTypeList), plantId);

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }

        public async Task<List<CSDMealsAvailabilityViewModel>> GetAvailabilityMealList(string fromDate, string toDate, long mealType, long mealoptionID, long loginUser, long plantId)
        {
            return _csdRepo.GetAvailabilityMealList(fromDate, toDate, mealType, mealoptionID, loginUser, plantId);


            //Error
            //GetAvailabilityMealList is Get in API code

            //try
            //{               

            //    var response = await _apiClient.PostAsync<List<CSDMealsAvailabilityViewModel>>(string.Format(APIMapper.Canteen.GetAvailabilityMealList), new { fromDate, toDate, mealType, mealoptionID, loginUser, plantId });

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }

        public async Task<List<CanteenMealBookingViewModel>> GetBookingHistory(long mealtype, string fromDate, string toDate, long loginUser)
        {
            return _csdRepo.GetBookingHistory(mealtype, fromDate, toDate, loginUser);


            //Error
            //GetBookingHistory in mobile its a GET call

            //try
            //{

            //    var response = await _apiClient.PostAsync<List<CanteenMealBookingViewModel>>(string.Format(APIMapper.Canteen.GetBookingHistory), new { mealtype, fromDate, toDate, loginUser });

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }

        public async Task<Tuple<short, string>> SaveMealBooking(long AddedBy, CanteenMealBookingViewModel model)
        {
            return _csdRepo.SaveMealBooking(AddedBy, model);


            //try
            //{

            //    var response = await _apiClient.PostAsync<Tuple<short, string>>(string.Format(APIMapper.Canteen.SaveMealBooking), new { model, AddedBy });

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }

        public async Task<short> CancelBooking(long id, long updatedBy)
        {
            return _csdRepo.CancelBooking(id, updatedBy);


            //try
            //{
            //    var response = await _apiClient.PostAsync<short>(string.Format(APIMapper.Canteen.CancelBooking), new { id, updatedBy });

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}
        }

        //// Guest Meal Boonking
        public List<CSDGuestMealBookingTrnViewModel> GetGuestBookedMealList(long mealtype, string fromDate, string toDate, long loginUser)
        {
            return _csdRepo.GetGuestBookedMealList(mealtype, fromDate, toDate, loginUser);
        }

        public List<CSDMealsAvailabilityViewModel> GetGuestAvailabilityMealList(string date, long? mealType, long mealopID, long plantId)
        {
            return _csdRepo.GetGuestAvailabilityMealList(date, mealType, mealopID, plantId);
        }

        public Tuple<short, string> SaveGuestMealBooking(long id, CSDGuestMealBookingTrnViewModel model, List<GuestDtlViewModel> guestList)
        {
            Tuple<short, string> _newTuple = new Tuple<short, string>(0, "");
            Tuple<short, string, long> _tuple = _csdRepo.SaveGuestMealBooking(id, model, guestList);
            if (_tuple.Item1 == 1 && _tuple.Item3 > 0 && model.PAYMENT_TYPE == 2)
            {
                // SendMailByRequestor(_tuple.Item3);
            }
            return _newTuple = new Tuple<short, string>(_tuple.Item1, _tuple.Item2); ;
        }

        public short CancelGuestMealBooking(long id, long updatedBy)
        {
            return _csdRepo.CancelGuestMealBooking(id, updatedBy);
        }

        public Tuple<long, short> GetGuestDtlByMno(string mobileNo, string bookingDate, int mealOption)
        {
            return _csdRepo.GetGuestDtlByMno(mobileNo, bookingDate, mealOption);
        }

        public List<GuestDtlViewModel> GuestAutocomplete(string term)
        {
            return _csdRepo.GuestAutocomplete(term);
        }

        public async Task<CSDGuestMealBookingTrnViewModel> GetGuestRequestDtlById(long _ReqId)
        {
            return _csdRepo.GetGuestRequestDtlById(_ReqId);

            //try
            //{
            //    var response = await _apiClient.GetAsync<CSDGuestMealBookingTrnViewModel>(string.Format(APIMapper.GetGuestRequestDtlById, _ReqId));

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return new CSDGuestMealBookingTrnViewModel();
            //}
        }

        public async Task<short> GuestMealApproval(CSDGuestMealBookingTrnViewModel model)
        {
            return _csdRepo.GuestMealApproval(model);

            //try
            //{
            //    var response = await _apiClient.PostAsync<short>(APIMapper.GuestMealApproval, model);

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}
        }

        public short BulkGuestMealApproval(List<CSDGuestMealBookingTrnViewModel> GMBTList)
        {
            short retVal = 0;
            //string GUEST_BOOKING_DAY = _csdRepo.GetParmvalue("GUEST_BOOKING_ALLOW_DAYS");
            //string GUEST_BOOKING_TIME = _csdRepo.GetParmvalue("GUEST_BOOKING_TIME");
            foreach (CSDGuestMealBookingTrnViewModel GMBT in GMBTList)
            {
                short IsValidDateTime = 1;
                //if (string.IsNullOrEmpty(GUEST_BOOKING_TIME) || string.IsNullOrEmpty(GUEST_BOOKING_DAY))
                //{
                //    DateTime currentDate = DateTime.Now;
                //    int validDay = Convert.ToInt32(GUEST_BOOKING_DAY);
                //    DateTime ValidDateTime = DateTime.ParseExact(GMBT.strBOOKED_DATE + " " + GUEST_BOOKING_TIME, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-validDay);
                //    if (currentDate > ValidDateTime)
                //    {
                //        IsValidDateTime = 0;
                //    }
                //}
                if (IsValidDateTime == 1)
                {
                    retVal = _csdRepo.GuestMealApproval(GMBT);
                    //if (retVal == 1)
                    //{
                    //    SendMailByApprovalAuthority(GMBT.BC_GUEST_BOOKINGID, GMBT.APPROVE_STATUS);
                    //}
                }
                else
                {
                    retVal = 2;
                }
            }
            return retVal;
        }


        //// BakeryItem
        public List<CSDBakeryItemViewModel> BakeryItem()
        {
            return _csdRepo.BakeryItem();
        }

        public async Task<List<MealOptionMstViewModel>> GetMealOptionList()
        {
            return _csdRepo.GetMealOption();

            //try
            //{
            //    var response = await _apiClient.GetAsync<List<MealOptionMstViewModel>>(APIMapper.Canteen.GetMealOptionList);

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }

        #region MealFeedback

        public short BookingFeedback(CanteenMealBookingViewModel model)
        {
            return _csdRepo.BookingFeedback(model);
        }

        public List<FeedbackPointViewModel> GetBookingFeedback(long bookingId, long mealType, long userId)
        {
            return _csdRepo.GetBookingFeedback(bookingId, mealType, userId);
        }

        public async Task<List<CanteenMealBookingViewModel>> GetBookingFeedbackHistory(long mealtype, string fromDate, string toDate, long loginUser, long plantId)
        {
            return _csdRepo.GetBookingFeedbackHistory(mealtype, fromDate, toDate, loginUser, plantId);
        }

        public async Task<List<CanteenMealBookingViewModel>> GetAllBookingFeedbackLst(long mealtype, string fromDate, string toDate, long loginUser, long plantId, long mealRating)
        {
            return _csdRepo.GetAllBookingFeedbackLst(mealtype, fromDate, toDate, loginUser, plantId, mealRating);
        }

        public List<MealBookingExcelVM> GetMealFeedbackExcelData(long mealtype, string fromDate, string toDate, long plantId, long mealRating)
        {
            return _csdRepo.GetMealFeedbackExcelData(mealtype, fromDate, toDate, plantId, mealRating);
        }

        #endregion
    }
}