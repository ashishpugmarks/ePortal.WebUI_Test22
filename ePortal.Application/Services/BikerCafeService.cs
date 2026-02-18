using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Web.WebPages;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.DataExchange.BikerCafe;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ePortal.Application.Services
{
    public class BikerCafeService : IBikerCafeService
    {
        private readonly BikerCafeRepository _bcRepo;
        Tuple<short, long> _retVal_tuple;
        private readonly HttpClient _httpClient;
        private readonly IApiClient _apiClient;
        private readonly IConfiguration _configuration;
        private readonly ISessionService _session;
        private readonly ILogger<BikerCafeService> _logger;


        public BikerCafeService(BikerCafeRepository bcRepo, HttpClient httpClient, IApiClient apiClient, IConfiguration configuration, ISessionService _session, ILogger<BikerCafeService> logger)
        {
            _bcRepo = bcRepo;
            _retVal_tuple = new Tuple<short, long>(0, 0);
            _httpClient = httpClient;
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<BC_VALIDATION_ViewModel> GetValidationData()
        {
            return _bcRepo.GetValidationData();           

            //try
            //{



            //    //string endPoint = APIMapper.BikerCafe.GetValidationData;
            //    //return await _apiClient.GetAsync<GuestMealBookingTrnViewModel>(endPoint);

            //    var response = await _apiClient.PostAsync<BC_VALIDATION_ViewModel>(APIMapper.BikerCafe.GetValidationData, new
            //    {

            //    });

            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    // return new BC_VALIDATION_ViewModel();
            //    _logger.LogError("Error in GetValidationData:" + ex.Message);
            //    throw;
            //}
        }
        public async Task<List<MealTypeViewModel>> GetMealTypeList(short mealCatID)
        {
            return _bcRepo.GetMealTypeList(mealCatID);

            //try
            //{
            //    var response = await _apiClient.PostAsync<List<MealTypeViewModel>>(APIMapper.BikerCafe.GetMealTypeList, new
            //    {
            //        mealCatID
            //    });

            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    //return new List<MealTypeViewModel>();
            //    _logger.LogError("Error in GetMealTypeList:" + ex.Message);
            //    throw;
            //}
        }

        public async Task<List<MealSlotViewModel>> GetMealSlots()
        {
            return _bcRepo.GetMealSlots();


            //try
            //{
            //    var response = await _apiClient.GetAsync<List<MealSlotViewModel>>(APIMapper.BikerCafe.GetMealSlots);

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return new List<MealSlotViewModel>();
            //}
        }

        public async Task<List<MealBookingTrnViewModel>> GetAvailabilityMealList(string fromDate, string toDate, long mealType, long loginUser)
        {
            return _bcRepo.GetAvailabilityMealList(fromDate, toDate, mealType, loginUser);

            //try
            //{
            //    var models = new 
            //    {
            //        fromDate = fromDate,
            //        toDate = toDate,
            //        mealType = mealType,
            //        loginUser = loginUser
            //    };


            //    var response = await _apiClient.PostAsync<List<MealBookingTrnViewModel>>(APIMapper.BikerCafe.GetAvailabilityMealList, new { models });

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return new List<MealBookingTrnViewModel>();
            //}
        }

        public async Task<Tuple<short, string>> SaveMealBooking(long AddedBy, List<MealBookingTrnViewModel> modelList)
        {
            return _bcRepo.SaveMealBooking(AddedBy, modelList);


            //    return new Tuple<short, string>(0, "Failed");
            //}
            //catch (Exception ex)
            //{
            //    return new Tuple<short, string>(0, "Exception occurred");
            //}

            //try
            //{
            //    var models = new
            //    {
            //        AddedBy = AddedBy,
            //        request = modelList
            //    };


            //    var response = await _apiClient.PostAsync<Tuple<short, string>>(APIMapper.BikerCafe.SaveMealBooking, new { models });

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return new Tuple<short, string>(0, "Failed");
            //}

        }

        public async Task<List<MealBookingTrnViewModel>> GetBookedMealList(long mealtype, string fromDate, string toDate, long loginUser)
        {
            return _bcRepo.GetBookedMealList(mealtype, fromDate, toDate, loginUser);


            //try
            //{
            //    var models = new
            //    {
            //        mealType = Convert.ToInt64(mealtype),
            //        fromDate = fromDate,
            //        toDate = toDate,
            //        loginUser = loginUser
            //    };

            //    var response = await _apiClient.PostAsync<List<MealBookingTrnViewModel>>(APIMapper.BikerCafe.GetBookedMealList, new { models });

            //    return response;

            //}
            //catch (Exception ex)
            //{
            //    return new List<MealBookingTrnViewModel>();
            //}
        }

        public async Task<short> CancelBooking(long id, long updatedBy)
        {
            return _bcRepo.CancelBooking(id, updatedBy);


            //try
            //{
            //    var models = new
            //    {
            //        id = id,
            //        updatedBy = updatedBy
            //    };

            //    var response = await _apiClient.PostAsync<short>(APIMapper.BikerCafe.CancelBooking, new { models });
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}
        }

        //public short SaveSubsidizedMealToken(SubsidizedMealTokenViewModel model)
        //{
        //    return _bcRepo.SaveSubsidizedMealToken(model);
        //}


        //-- Guest Meal Booking
        public async Task<List<GuestMealBookingTrnViewModel>> GetGuestBookedMealList(long mealtype, string fromDate, string toDate, long loginUser)
        {
            return _bcRepo.GetGuestBookedMealList(mealtype, fromDate, toDate, loginUser);


            //try
            //{
            //    var models = new
            //    {
            //        mealType = Convert.ToInt64(mealtype),
            //        fromDate = fromDate,
            //        toDate = toDate,
            //        loginUser = loginUser
            //    };
            //    var response = await _apiClient.PostAsync<List<GuestMealBookingTrnViewModel>>(APIMapper.BikerCafe.GetGuestBookedMealList, new { models });
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return new List<GuestMealBookingTrnViewModel>();
            //}
        }



        public async Task<List<Meals_AvailabilityViewModel>> GetGuestAvailabilityMealList(string date, long? mealType)
        {
            return _bcRepo.GetGuestAvailabilityMealList(date, mealType);

            //try
            //{
            //    var models = new
            //    {
            //        date = date,
            //        mealType = mealType
            //    };

            //    var response = await _apiClient.PostAsync<List<Meals_AvailabilityViewModel>>(APIMapper.BikerCafe.GetGuestAvailabilityMealList, new { models });
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return new List<Meals_AvailabilityViewModel>();
            //}
        }
        public async Task<Tuple<short, string>> SaveGuestMealBooking(long id, GuestMealBookingTrnViewModel model, List<GuestDtlViewModel> guestList)
        {
            Tuple<short, string> _newTuple = new Tuple<short, string>(0, "");
            Tuple<short, string, long> _tuple = _bcRepo.SaveGuestMealBooking(id, model, guestList);
            if (_tuple.Item1 == 1 && _tuple.Item3 > 0 && model.PAYMENT_TYPE == 2)
            {
                SendMailByRequestor(_tuple.Item3);
            }
            return _newTuple = new Tuple<short, string>(_tuple.Item1, _tuple.Item2); ;


            //try
            //{
            //    var models = new 
            //    {
            //        id = id,
            //        model = model,
            //        guestList = guestList
            //    };

            //    Tuple<short, string> _newTuple = new Tuple<short, string>(0, "");
            //    Tuple<short, string, long> _tuple = await _apiClient.PostAsync<Tuple<short, string, long>>(APIMapper.BikerCafe.SaveGuestMealBooking, new { models });

            //    if (_tuple.Item1 == 1 && _tuple.Item3 > 0 && model.PAYMENT_TYPE == 2)
            //    {
            //        SendMailByRequestor(_tuple.Item3);
            //    }
            //    return _newTuple = new Tuple<short, string>(_tuple.Item1, _tuple.Item2); ;
            //}
            //catch (Exception ex)
            //{
            //    return new Tuple<short, string>(0, "Failed");
            //}
        }
        public async Task<short> CancelGuestMealBooking(long id, long updatedBy)
        {
            return _bcRepo.CancelGuestMealBooking(id, updatedBy);


            //try
            //{
            //    var models = new
            //    {
            //        id = id,
            //        updatedBy = updatedBy
            //    };

            //    var response = await _apiClient.PostAsync<short>(APIMapper.BikerCafe.CancelGuestMealBooking, new { models });
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}
        }

        public async Task<short> BulkGuestMealApproval(List<GuestMealBookingTrnViewModel> GMBTList)
        {
            short retVal = 0;
            BC_VALIDATION_ViewModel validModel = await GetValidationData();
            foreach (GuestMealBookingTrnViewModel GMBT in GMBTList)
            {
                short IsValidDateTime = 1;
                if (validModel != null)
                {
                    DateTime currentDate = DateTime.Now;
                    string bookingTime = validModel.GUEST_BOOKING_TIME;
                    int validDay = Convert.ToInt32(validModel.GUEST_BOOKING_DAY);
                    DateTime ValidDateTime = DateTime.ParseExact(GMBT.strBOOKED_DATE + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-validDay);
                    if (currentDate > ValidDateTime)
                    {
                        IsValidDateTime = 0;
                    }
                }
                if (IsValidDateTime == 1)
                {
                    retVal = _bcRepo.GuestMealApproval(GMBT);
                    if (retVal == 1)
                    {
                        SendMailByApprovalAuthority(GMBT.BC_GUEST_BOOKINGID, GMBT.APPROVE_STATUS);
                    }
                }
                else
                {
                    retVal = 2;
                }
            }
            return retVal;
        }

        public async Task<List<GuestDtlViewModel>> GuestAutocomplete(string term)
        {
            return _bcRepo.GuestAutocomplete(term);


            //try
            //{
            //    var response = await _apiClient.PostAsync<List<GuestDtlViewModel>>(APIMapper.BikerCafe.GuestAutocomplete, new { term });
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return new List<GuestDtlViewModel>();
            //}
        }

        public async Task<GuestMealBookingTrnViewModel> GetGuestRequestDtlById(long id)
        {
            return _bcRepo.GetGuestRequestDtlById(id);


            //try
            //{
            //    var response = await _apiClient.PostAsync<GuestMealBookingTrnViewModel>(APIMapper.BikerCafe.GetGuestRequestDtlById, new
            //    {
            //        Id = id
            //    });


            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return new GuestMealBookingTrnViewModel();
            //}

        }

        public async Task<short> GuestMealApproval(GuestMealBookingTrnViewModel model)
        {
            short retVal = _bcRepo.GuestMealApproval(model);
            if (retVal == 1)
            {
                SendMailByApprovalAuthority(model.BC_GUEST_BOOKINGID, model.APPROVE_STATUS);
            }
            return retVal;


            //try
            //{
            //    var response = await _apiClient.PostAsync<short>(APIMapper.BikerCafe.GuestMealApproval, new
            //    {
            //        model
            //    });

            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}
        }

        public async Task<Tuple<long, short>> GetGuestDtlByMno(string mNo, string bookingDate)
        {
            return _bcRepo.GetGuestDtlByMno(mNo, bookingDate);


            //try
            //{
            //    var models = new
            //    {
            //        mNo = mNo,
            //        bookingDate = bookingDate
            //    };

            //    var response = await _apiClient.PostAsync<Tuple<long, short>>(APIMapper.BikerCafe.GetGuestDtlByMno, new { models });
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return new Tuple<long, short>(0, 0);
            //}

        }

        public short SendMailByRequestor(long reqId)
        {
            short retVal = 0;
            try
            {
                GuestMealBookingTrnViewModel GMBT = _bcRepo.GetGuestRequestDtlById(reqId);
                if (GMBT != null)
                {
                    if (GMBT.APPROVER_ECODE > 0 && !string.IsNullOrEmpty(GMBT.APPROVER_EMAIL))
                    {
                        EmailCore sendMail = new EmailCore();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = GMBT.APPROVER_EMAIL;

                        string struid = (GMBT.APPROVER_ECODE).ToString();
                        string strid = reqId.ToString();
                        string paymenyType = GMBT.PAYMENT_TYPE == 1 ? "Self Paid" : "Company Paid";
                        int noOfGuest = GMBT.GuestDtlList.Count();
                        decimal totalAmt = GMBT.GuestDtlList.Sum(s => s.MEAL_PRICE);

                        string strSubject = "Guest Meal Booking Approval, Requestor - " + GMBT.Emp_Detail._EName + "(" + GMBT.Emp_Detail._ECode + ")";
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Guest Meal Booking Request from " + GMBT.Emp_Detail._EName + " - Emp Code (" + GMBT.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + GMBT.APPROVER_NAME + " San ,</br></br>" + GMBT.Emp_Detail._EName + " San has raised a Guest Meal Booking Approval Request in Employee Portal. Below are the details :</br></td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Booking Date: </td><td width=389 valign=top>" + GMBT.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Payment Type: </td><td width=389 valign=top>" + paymenyType + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>No. of Guest: </td><td width=389 valign=top>" + noOfGuest + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Total Amount: </td><td width=389 valign=top>" + Convert.ToDecimal(totalAmt) + "</td></tr>" +
                                          //Guest meal booking change
                                          "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?wid=" + struid + "&C=BikerCafe&A=GuestMealApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                         //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                            retVal = -1;
                        }
                        finally
                        {
                            retVal = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public short SendMailByApprovalAuthority(long reqId, short approvalStatus)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Rejected" : "";

                GuestMealBookingTrnViewModel GMBT = _bcRepo.GetGuestRequestDtlById(reqId);
                #region Send mail for requestor
                if (!string.IsNullOrEmpty(GMBT.Emp_Detail._EmailId))
                {
                    EmailCore sendMail = new EmailCore();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                        sendMail.MailTo = GMBT.Emp_Detail._EmailId;

                    string paymentType = GMBT.PAYMENT_TYPE == 1 ? "Self Paid" : "Company Paid";
                    int noOfGuest = GMBT.GuestDtlList.Count();
                    decimal totalAmt = GMBT.GuestDtlList.Sum(s => s.MEAL_PRICE);

                    string strSubject = "Guest Meal Booking Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Guest Meal Booking Approval For " + GMBT.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + "</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Guest Meal Booking request has been <b>" + RequestStatus + "</b> by " + GMBT.APPROVER_NAME + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>Booking Date: </td><td width=389 valign=top>" + GMBT.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + "</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>Payment Type: </td><td width=389 valign=top>" + paymentType + "</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>No. of Guest: </td><td width=389 valign=top>" + noOfGuest + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Total Amount: </td><td width=389 valign=top>" + Convert.ToDecimal(totalAmt) + "</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the detail.</td></tr>" +
                                     //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to View the detail.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    finally
                    {
                        retVal = 1;
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        // -- Ala Carte Food --//
        public async Task<List<AlaCarteHeaderViewModel>> GetAlaCarteRequestList(string fromDate, string toDate, short category, long loginUser)
        {
            return _bcRepo.GetAlaCarteRequestList(fromDate, toDate, category, loginUser);

            //try
            //{
            //    var models = new
            //    {
            //        fromDate = fromDate,  //.ToString("dd-MMM-yyyy"),
            //        toDate = toDate,  //.ToString("dd-MMM-yyyy"),
            //        category = category,
            //        loginUser = loginUser
            //    };

            //    var response = await _apiClient.PostAsync<List<AlaCarteHeaderViewModel>>(APIMapper.BikerCafe.GetAlaCarteRequestList, new { models });
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return new List<AlaCarteHeaderViewModel>();
            //}
        }

        public async Task<List<AlaCarteItemMstViewModel>> GetAlaCarteItemList()
        {
            return _bcRepo.GetAlaCarteItemList();


            //try
            //{

            //    var response = await _apiClient.GetAsync<List<AlaCarteItemMstViewModel>>(APIMapper.BikerCafe.GetAlaCarteItemList);
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return new List<AlaCarteItemMstViewModel>();
            //}
        }

        public async Task<Tuple<short, string>> SaveAlaCarteOrder(long AddedBy, List<AlaCarteTrnViewModel> model)
        {
            return _bcRepo.SaveAlaCarteOrder(AddedBy, model);


            //var models = new
            //{
            //    AddedBy = AddedBy,
            //    model = model
            //};


            //try
            //{

            //    var response = await _apiClient.PostAsync<Tuple<short, string>>(APIMapper.BikerCafe.SaveAlaCarteOrder, new { models });
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return new Tuple<short, string>(0, "Failed");
            //}
        }

        public async Task<AlaCarteHeaderViewModel> GetAlaCarteDtlById(long id)
        {
            return _bcRepo.GetAlaCarteDtlById(id);

            //try
            //{

            //    var response = await _apiClient.GetAsync<AlaCarteHeaderViewModel>($"{APIMapper.BikerCafe.GetAlaCarteDtlById}/{id}");
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    return new AlaCarteHeaderViewModel();
            //}
        }
        public short CancelAlaCarteOrder(long id, long updatedBy)
        {
            return _bcRepo.CancelAlaCarteOrder(id, updatedBy);
        }

        public List<BcMenuViewModel> GetBCMenu()
        {
            return _bcRepo.GetBCMenu();
        }

        // -- Family Meal Booking --//
        public FamilMealBookingDtlViewModel GetMealByDate(long mealtype, DateTime bookingDate)
        {
            return _bcRepo.GetMealByDate(mealtype, bookingDate);
        }

        public Tuple<short, string> SaveFamilyMealBooking(FamilyMealBookingTrnViewModel model)
        {
            return _bcRepo.SaveFamilyMealBooking(model);
        }

        public List<FamilyMealBookingTrnViewModel> GetFamilyBookedMealList(long mealtype, string fromDate, string toDate, long loginUser)
        {
            return _bcRepo.GetFamilyBookedMealList(mealtype, fromDate, toDate, loginUser);
        }

        public short CancelFamilyMealBooking(long id, long updatedBy)
        {
            return _bcRepo.CancelFamilyMealBooking(id, updatedBy);
        }

        public List<MealSlotViewModel> GetMealSlotsByBookingDate(string bDate)
        {
            return _bcRepo.GetMealSlotsByBookingDate(bDate);
        }

        public FamilyMealBookingTrnViewModel GetFamilyRequestDtlById(long id)
        {
            return _bcRepo.GetFamilyRequestDtlById(id);
        }

        //---Meeting Food-------------//
        public List<MeetingFoodItemMstViewModel> GetMeetingFoodItemList()
        {
            return _bcRepo.GetMeetingFoodItemList();
        }

        public Tuple<short, string> SaveMeetingFoodOrder(long AddedBy, long OPID, string EVENTDATE, string EVENTTIME, string VENUE, string REMARKS, List<MeetingFoodTrnViewModel> model, string noofGuests, string desg)
        {
            return _bcRepo.SaveMeetingFoodOrder(AddedBy, OPID, EVENTDATE, EVENTTIME, VENUE, REMARKS, model, noofGuests, desg);
        }

        public List<MeetingFoodHeaderViewModel> GetMeetingFoodRequestList(string fromDate, string toDate, short category, long loginUser)
        {
            return _bcRepo.GetMeetingFoodRequestList(fromDate, toDate, category, loginUser);
        }
        public MeetingFoodHeaderViewModel GetMeetingFoodDtlById(long id)
        {
            return _bcRepo.GetMeetingFoodDtlById(id);
        }

        public short MeetingFoodApproval(MeetingFoodHeaderViewModel model)
        {
            short retVal = _bcRepo.MeetingFoodApproval(model);
            //if (retVal == 1)
            //{
            //    SendMailByApprovalAuthority(model.BC_GUEST_BOOKINGID, model.APPROVE_STATUS);
            //}
            return retVal;
        }

        public short CancelMeetingFoodBooking(long id, long updatedBy)
        {
            return _bcRepo.CancelMeetingFoodOrder(id, updatedBy);
        }
    }
}
