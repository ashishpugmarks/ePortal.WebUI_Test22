using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;

namespace ePortal.ViewModels
{
    public class CSDMealTypeViewModel
    {
        public long CSD_MEALTYPEID { get; set; }
        public string MEAL_TYPE_DESC { get; set; }
        public long MEAL_QTY { get; set; }
        public decimal? MEAL_PRICE { get; set; }
        public bool STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }
        public long CSD_CANTEENID { get; set; }
        public string CANTEEN_NAME { get; set; }
        public long PLANTID { get; set; }

    }

    public class CSDMealMstViewModel
    {
        public long CSD_MEALID { get; set; }

        [DisplayName("Meal Type")]
        public long MEALTYPEID { get; set; }
        public virtual CSDMealTypeViewModel MealTypeModel { get; set; }

        [DisplayName("Meal Name")]
        public string MEAL_NAME { get; set; }

        [DisplayName("Menu")]
        public string MEAL_DESC { get; set; }

        [DisplayName("Meal Price")]
        public decimal MEAL_PRICE { get; set; }

        [DisplayName("Status")]
        public bool STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }

        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }

        [DisplayName("Meal Image")]
        public byte[] MEAL_PHOTO { get; set; }
        public string MEAL_PHOTO_NAME { get; set; }
    }

    public class CSDMealsAvailabilityViewModel
    {
        public long CSD_MEAL_AVAILABILITY_ID { get; set; }
        public long MEALS_ID { get; set; }
        public CSDMealMstViewModel MealMst_MODEL { get; set; }
        public List<CSDCanteenMealMapViewModel> MealCanMapList { get; set; }

        [DisplayName("Meal Option")]
        public long MEALOPTIONID { get; set; }
        public string MEALOPTION { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Meal Date")]
        public DateTime MEAL_MAPPING_DATE { get; set; }

        [DisplayName("Status")]
        public bool STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
    }

    public class CSDCanteenMealMapViewModel
    {
        public long CSD_CMTMAPID { get; set; }
        public long MEALTYPE_ID { get; set; }
        public long CANTEENID { get; set; }
        public string CANTEEN_NAME { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }

    }

    public class CanteenMealBookingViewModel
    {
        [DisplayName("SNo")]
        public long CSD_MEALSBOOKING_ID { get; set; }
        public long BookingId { get; set; }

        public long MEALS_AVAILABILITY_ID { get; set; }

        [DisplayName("Meal Type")]
        public long MEALTYPEID { get; set; }
        public long MEAL_QTY { get; set; }
        public virtual CSDMealTypeViewModel MealType { get; set; }
        public virtual CSDMealMstViewModel MealMst { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Booking Date")]
        public DateTime MEAL_BOOKED_DATE { get; set; }
        public string strBOOKED_DATE { get; set; }

        [DisplayName("Meal Option")]
        public long MEALOPTIONID { get; set; }
        public string MEALOPTION { get; set; }

        [DisplayName("Booking From")]
        public string strBookingFrom { get; set; }

        [DisplayName("Booking To")]
        public string strBookingTo { get; set; }

        [DisplayName("Status")]
        public short MEAL_STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }
        public string EMAILID { get; set; }
        public string FULLNAME { get; set; }
        [DisplayName("Feedback Rating")]
        public long? FEEDBACK_RATING { get; set; }
        [DisplayName("Feedback Remark")]
        public string? FEEDBACK_REMARK { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
        public virtual CSDMealsAvailabilityViewModel MealAvailabilityModel { get; set; }
        public bool IsEnableCancel { get; set; }
        public List<FeedbackPointViewModel> FeedbackPoints { get; set; }

        public byte[] EvidenceFile { get; set; }
        public string EvidenceFileName { get; set; }
        public string EvidenceFileType { get; set; }
        public string EvidenceFilePath { get; set; }
    }

    public class CanteenMstViewModel
    {
        public long CSD_CANTEENID { get; set; }
        [DisplayName("Canteen")]
        public string CANTEEN_NAME { get; set; }
        public long PLANTID { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }
    }

    public class CSDGuestMealBookingTrnViewModel
    {
        [DisplayName("SNo")]
        public long CSD_GUEST_BOOKINGID { get; set; }

        public long MEALS_AVAILABILITY_ID { get; set; }

        [DisplayName("Meal Type")]
        public long MEALTYPEID { get; set; }
        public decimal MEAL_PRICE { get; set; }
        public long MEAL_QTY { get; set; }
        public virtual CSDMealTypeViewModel MealType { get; set; }
        public virtual CSDMealMstViewModel MealMst { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Booking Date")]
        public DateTime MEAL_BOOKED_DATE { get; set; }
        public string strBOOKED_DATE { get; set; }

        [DisplayName("Status")]
        public short MEAL_STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
        public List<CSDMealsAvailabilityViewModel> MealAvailabilityList { get; set; }
        public bool IsEnableCancel { get; set; }
        public List<GuestDtlViewModel> GuestDtlList { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }

        [DisplayName("Payment Type")]
        public short PAYMENT_TYPE { get; set; }
        public string APPROVER_EMAIL { get; set; }
        public string APPROVER_FNAME { get; set; }
        public string APPROVER_LNAME { get; set; }
        [DisplayName("Approval Authority")]
        public string APPROVER_NAME
        {
            get { return APPROVER_FNAME + " " + APPROVER_LNAME; }
        }
        public long APPROVER_ECODE { get; set; }
        [DisplayName("Status")]
        public short APPROVE_STATUS { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm:ss}"), DisplayName("Approval Date")]
        public Nullable<System.DateTime> APPROVE_DATE { get; set; }
        [DisplayName("Remark")]
        public string APPROVE_REMARK { get; set; }

        [DisplayName("Visit Purpose")]
        public string VISIT_PURPOSE { get; set; }

        [DisplayName("Meal Option")]
        public long MEALOPTIONID { get; set; }
        public string MEALOPTION { get; set; }

        public List<GuestDtlViewModel> GUEST_LIST { get; set; }
    }

    public class CSDValidationViewModel
    {
        public long CSD_VALIDMST_ID { get; set; }
        public string PARM_NAME { get; set; }
        public string PARM_VALUE { get; set; }
        public string DESCRIPTION { get; set; }
        public string PARM_INPUT_TYPE { get; set; }
        public short STATUS { get; set; }
    }

    public class CSDBakeryItemViewModel
    {
        [DisplayName("SNo")]
        public long CSD_ITEMID { get; set; }
        [DisplayName("Item Name")]
        public string ITEM_NAME { get; set; }
        [DisplayName("Item Description")]
        public string ITEM_DESC { get; set; }
        [DisplayName("Price")]
        public decimal ITEM_PRICE { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }
    }

    public class MealOptionMstViewModel
    {
        public long CSDMEAL_OPTIONID { get; set; }
        [DisplayName("Meal Option")]
        public string MEAL_OPTION { get; set; }
        [DisplayName("Status")]
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDON { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDON { get; set; }

    }

    public class CSD_MealTimeMapViewModel
    {
        public long CSD_TIMEMAPID { get; set; }
        public long MEALTYPEID { get; set; }
        public long MEAL_OPTIONID { get; set; }
        public short MEAL_BOOKING_DURATION { get; set; }
        public short BOOKING_DAY { get; set; }
        public short CANCELLATION_DAY { get; set; }
        public string BOOKING_TIME { get; set; }
        public string CANCELLATION_TIME { get; set; }
        public short GUEST_BOOKING_DAY { get; set; }
        public short GUEST_CANCELLATION_DAY { get; set; }
        public string GUEST_BOOKING_TIME { get; set; }
        public string GUEST_CANCELLATION_TIME { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }

    }

    #region MealFeedback
    public class MealFeedbackMailViewModel
    {
        public long CSD_MEALSBOOKING_ID { get; set; }
        public long MEAL_AVAILABILITY_ID { get; set; }
        public long MEALTYPEID { get; set; }
        public System.DateTime MEAL_BOOKED_DATE { get; set; }
        public short MEAL_STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
        public long MEALOPTIONID { get; set; }
        public string? FEEDBACK_REMARK { get; set; }
        public long? FEEDBACK_RATING { get; set; }

        public long ADEMPCODE { get; set; }
        public string SALUTATION { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string FULLNAME { get; set; }
        public string EMAILID { get; set; }
    }

    public class MealBookingFeedbackVM
    {
        public CSD_MEALBOOKING_TRN Booking { get; set; }
        public string MealTypeDesc { get; set; }
    }

    public class FeedbackPointViewModel
    {
        public int FeedbackPointId { get; set; }
        public string PointName { get; set; }
        public int? Rating { get; set; }
        public string Remark { get; set; }
        public string? EvidenceImagePath { get; set; }
    }

    public class MealBookingExcelVM
    {
        [DisplayName("SNo")]
        public long CSD_MEALSBOOKING_ID { get; set; }
        public long BookingId { get; set; }

        public long MEALS_AVAILABILITY_ID { get; set; }

        [DisplayName("Meal Type")]
        public long MEALTYPEID { get; set; }
        public long MEAL_QTY { get; set; }
        public virtual CSDMealTypeViewModel MealType { get; set; }
        public virtual CSDMealMstViewModel MealMst { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Booking Date")]
        public DateTime MEAL_BOOKED_DATE { get; set; }
        public string strBOOKED_DATE { get; set; }
        public string EmpName { get; set; }

        [DisplayName("Meal Option")]
        public long MEALOPTIONID { get; set; }
        public string MEALOPTION { get; set; }

        [DisplayName("Booking From")]
        public string strBookingFrom { get; set; }

        [DisplayName("Booking To")]
        public string strBookingTo { get; set; }

        [DisplayName("Status")]
        public short MEAL_STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }
        public string EMAILID { get; set; }
        public string FULLNAME { get; set; }
        [DisplayName("Feedback Rating")]
        public long? FEEDBACK_RATING { get; set; }
        [DisplayName("Feedback Remark")]
        public string? FEEDBACK_REMARK { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
        public virtual CSDMealsAvailabilityViewModel MealAvailabilityModel { get; set; }
        public bool IsEnableCancel { get; set; }
        public List<FeedbackPointViewModel> FeedbackPoints { get; set; }

        public byte[] EvidenceFile { get; set; }
        public string EvidenceFileName { get; set; }
        public string EvidenceFileType { get; set; }
        public string EvidenceFilePath { get; set; }
    }
    #endregion

}