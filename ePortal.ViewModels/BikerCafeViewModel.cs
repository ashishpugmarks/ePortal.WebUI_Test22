using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public partial class MealTypeViewModel
    {
        public long BC_MEALTYPEID { get; set; }
        public string MEAL_TYPE_DESC { get; set; }
        public long READER_CODE { get; set; }
        public decimal MEAL_PRICE { get; set; }
        public bool STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }
    }
    public class BCMstMealViewModel
    {
        public long BC_MEALID { get; set; }

        [DisplayName("Meal Type")]
        public long MEALTYPEID { get; set; }
        public virtual MealTypeViewModel MealTypeModel { get; set; }

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

    public class Meals_AvailabilityViewModel
    {
        public long BC_MEALS_AVAILABILITY_ID { get; set; }

        public long MEALS_ID { get; set; }
        public BCMstMealViewModel MealMst_MODEL { get; set; }

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
        public short BOOKING_STATUS { get; set; }
        [DisplayName("Slot")]
        public long SLOTMSTID { get; set; }
    }


    public class MealBookingTrnViewModel
    {
        [DisplayName("SNo")]
        public long BC_MEALSBOOKING_ID { get; set; }

        public long MEALS_AVAILABILITY_ID { get; set; }

        [DisplayName("Meal Type")]
        public long MEALTYPEID { get; set; }
        public virtual MealTypeViewModel MealType { get; set; }
        public virtual BCMstMealViewModel MealMst { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Booking Date")]
        public DateTime MEAL_BOOKED_DATE { get; set; }
        public string strBOOKED_DATE { get; set; }

        [DisplayName("Booking From")]
        public string strBookingFrom { get; set; }

        [DisplayName("Booking To")]
        public string strBookingTo { get; set; }

        [DisplayName("Status")]
        public short MEAL_STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }

        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
        public virtual Meals_AvailabilityViewModel MealAvailabilityModel { get; set; }
        public bool IsEnableCancel { get; set; }

        [DisplayName("Slot")]
        public long SLOTMSTID { get; set; }
        public string SLOT_TIME { get; set; }
        public bool IsSelectedForBooking { get; set; }
    }

    public class BC_VALIDATION_ViewModel
    {
        public long BC_MST_VALIDATION_ID { get; set; }
        public string MEAL_UPDATE_PERIOD { get; set; }
        public string MEAL_CHANGE_TIME { get; set; }
        public string MEAL_BOOKING_DURATION { get; set; }
        public string MEAL_BOOKING_TIME { get; set; }
        public string MEAL_CANCELLATION_TIME { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }
        public Nullable<long> INDIANMEAL_QTY { get; set; }
        public Nullable<long> JAPANESEMEAL_QTY { get; set; }
        public string MEAL_BOOKING_DAY { get; set; }
        public Int16? ALACARTESUBSIDY { get; set; }
        public decimal? ALACARTESUBSIDY_MAX { get; set; }
        public string ALACARTE_BOOKING_TIME { get; set; }
        public string GUEST_BOOKING_TIME { get; set; }
        public string GUEST_CANCELLATION_TIME { get; set; }
        public string GUEST_BOOKING_DAY { get; set; }
        public string GUEST_BOOKING_SELFAPP { get; set; }
        public string JAPANESEMEAL_BOOKING_DAY { get; set; }
        public string JAPANESEMEAL_BOOKING_TIME { get; set; }
        public string JAPANESEMEAL_CANCELLATION_TIME { get; set; }
        public string FMY_MEAL_BOOKING_DAY { get; set; }
        public string FMY_MEAL_BOOKING_TIME { get; set; }
        public string FMY_MEAL_CANCELLATION_TIME { get; set; }
        public long FMY_VISIT_MEMBER_QTY { get; set; }
        public short FMY_VISIT_MAX_MEMBER_ALLOW { get; set; }
        public long FMY_VISIT_FOLLOW_SITE_CALENDER { get; set; }
        public DateTime? FMY_VISIT_TILL_DATE { get; set; }
    }

    public class MealSlotViewModel
    {
        public long BCSLOTMSTID { get; set; }
        public string SLOT_DESC { get; set; }
        public string SLOT_TIME { get; set; }
        public short CAPACITY { get; set; }
        public long MEALTYPEID { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }

    }

    public class SubsidizedMealTokenViewModel
    {
        [DisplayName("SNo")]
        public long BC_SUBSIDIZED_MEAL_TOKEN_ID { get; set; }
        public long EMPLOYEE_ID { get; set; }

        [DisplayName("OTP")]
        public string TOKEN_CODE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Subsidized Date")]
        public DateTime TOKEN_DATE { get; set; }
        public string strTOKEN_DATE { get; set; }

        [DisplayName("Total Meal Amount")]
        public decimal MEAL_AMOUNT { get; set; }

        [DisplayName("Subsidized Amount")]
        public decimal SUBSIDIZED_AMOUNT { get; set; }

        [DisplayName("Payable Amount")]
        public decimal PAYABLE_AMOUNT { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }

    }

    public class GuestMealBookingTrnViewModel
    {
        [DisplayName("SNo")]
        public long BC_GUEST_BOOKINGID { get; set; }

        public long MEALS_AVAILABILITY_ID { get; set; }

        [DisplayName("Meal Type")]
        public long MEALTYPEID { get; set; }
        public decimal MEAL_PRICE { get; set; }
        public virtual MealTypeViewModel MealType { get; set; }
        public virtual BCMstMealViewModel MealMst { get; set; }

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
        public List<Meals_AvailabilityViewModel> MealAvailabilityList { get; set; }
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

        [DisplayName("Slot")]
        public long SLOTMSTID { get; set; }
        public string SLOT_TIME { get; set; }

        public List<GuestDtlViewModel> GUEST_LIST { get; set; }

    }

    public class GuestDtlViewModel
    {
        [DisplayName("Sno")]
        public long BC_GUESTID { get; set; }
        [DisplayName("Guest Name")]
        public string GUEST_NAME { get; set; }
        [DisplayName("Guest Mobile")]
        public long MOBILE_NUMBER { get; set; }
        [DisplayName("Guest Company")]
        public string COMPANY { get; set; }
        [DisplayName("Status")]
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }
        [DisplayName("OTP")]
        public string TOKEN_CODE { get; set; }

        [DisplayName("Meal Price")]
        public decimal MEAL_PRICE { get; set; }

        [DisplayName("Admin Status")]
        public short? ADMIN_STATUS { get; set; }

    }
       
    public class FamilyMealBookingTrnViewModel_VM
    {   
        public string strBOOKED_DATE { get; set; }

        [DisplayName("Status")]
        public short MEAL_STATUS { get; set; }        
        public List<FamilMealBookingDtlViewModel> MEALBOOKING_DTL { get; set; }

        [DisplayName("Slot")]
        public long SLOTID { get; set; }
    }
      

    public class FamilyMealBookingTrnViewModel
    {
        [DisplayName("SNo")]
        public long BC_FAMILY_BOOKINGID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Booking Date")]
        public System.DateTime MEAL_BOOKED_DATE { get; set; }
        public string strBOOKED_DATE { get; set; }

        [DisplayName("Status")]
        public short MEAL_STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> LSTMODIFIEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }

        [DisplayName("Slot")]
        public long SLOTID { get; set; }
        public string SLOT_TIME { get; set; }

        [DisplayName("OTP")]
        public string TOKEN_CODE { get; set; }

        [DisplayName("Meal Price")]
        public decimal MEAL_PRICE { get; set; }
        public Nullable<short> ADMIN_STAUS { get; set; }
        public Nullable<System.DateTime> ADMIN_DATE { get; set; }
        public Nullable<long> ADMIN_ECODE { get; set; }
        public bool IsEnableCancel { get; set; }
        public List<FamilMealBookingDtlViewModel> MEALBOOKING_DTL { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }

    }
    public class FamilMealBookingDtlViewModel
    {
        [DisplayName("SNo")]
        public long BC_FAMILYMEALBOOKING_DTLID { get; set; }
        public long BC_FAMILY_BOOKINGID { get; set; }

        [DisplayName("Member Name")]
        public string MEMBER_NAME { get; set; }

        [DisplayName("Relation Type")]
        public string RELATION_TYPE { get; set; }

        [DisplayName("Meal Type")]
        public long MEALTYPEID { get; set; }
        public string MEALTYPE { get; set; }

        [DisplayName("Meal")]
        public long MEALS_AVAILABILITY_ID { get; set; }
        public virtual BCMstMealViewModel MealMst { get; set; }

        public Nullable<short> STATUS { get; set; }
    }

    // -- Ala Carte Food -- //
    public class AlaCarteItemMstViewModel
    {
        [DisplayName("SNo")]
        public long BC_FOODMSTID { get; set; }

        [DisplayName("Category")]
        public short ITEM_CATEGORY { get; set; }
        [DisplayName("Item Name")]
        public string ITEM_NAME { get; set; }

        [DisplayName("Price")]
        public decimal ITEM_PRICE { get; set; }

        [DisplayName("Preparation Time")]
        public short LEAD_TIME { get; set; }

        [DisplayName("Take Way")]
        public bool TAKE_WAY { get; set; }

        [DisplayName("Dine In")]
        public bool DINE_IN { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
        public byte[] ITEM_PHOTO { get; set; }
        public string ITEM_PHOTO_NAME { get; set; }
    }

    public class AlaCarteHeaderViewModel
    {
        [DisplayName("SNo")]
        public long ALACARTEFOODHDR_ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Order Date")]
        public DateTime ORDER_DATE { get; set; }
        public string strORDER_DATE { get; set; }

        [DisplayName("Status")]
        public short ORDER_STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm tt}"), DisplayName("Accepted Date")]
        public DateTime? ACCEPTED_DATE { get; set; }
        public bool IsEnableCancel { get; set; }
        public List<AlaCarteTrnViewModel> TrnModel { get; set; }
        public List<AlaCarteItemMstViewModel> ItemModelList { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
    }

    public class AlaCarteTrnViewModel
    {
        [DisplayName("SNo")]
        public long ALACARTEFOODTRN_ID { get; set; }
        public long ALACARTEFOODHDR_ID { get; set; }
        public long ITEMID { get; set; }

        [DisplayName("Qty")]
        public short ITEM_QTY { get; set; }

        [DisplayName("Price")]
        public decimal ITEM_PRICE { get; set; }

        [DisplayName("Delivery Type")]
        public short DELIVERY_TYPE { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Item Status")]
        public short ITEM_STATUS { get; set; }

        public virtual AlaCarteItemMstViewModel ItemModel { get; set; }

    }

    public class BcMenuViewModel
    {
        public long BCMENUID { get; set; }
        public string MENU_NAME { get; set; }
        public string MENU_DESC { get; set; }
        public string MENU_URL { get; set; }
        public byte[] MENU_ICON { get; set; }
        public string MENUICON_CONTENT_TYPE { get; set; }
        public string MENUICON_FILENAME { get; set; }
        public short STATUS { get; set; }
        public long CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<long> DISPLAY_ORDER { get; set; }
    }

    //----------Meeting food--//
    public class MeetingFoodItemMstViewModel
    {
        [DisplayName("SNo")]
        public long BC_FOODMSTID { get; set; }

        [DisplayName("Category")]
        public short ITEM_CATEGORY { get; set; }
        [DisplayName("Item Name")]
        public string ITEM_NAME { get; set; }

        [DisplayName("Price")]
        public decimal ITEM_PRICE { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
        public byte[] ITEM_PHOTO { get; set; }
        public string ITEM_PHOTO_NAME { get; set; }
    }

    public class MeetingFoodHeaderViewModel
    {
        [DisplayName("SNo")]
        public long MEETINGFOODFOODHDR_ID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Event Date")]
        public DateTime EVENT_DATE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Order Date")]
        public DateTime ORDER_DATE { get; set; }
        public string strORDER_DATE { get; set; }

        [DisplayName("Event Time")]
        public string EVENTTIME { get; set; }

        [DisplayName("Venue")]
        public string VENUE { get; set; }
        [DisplayName("Guest Details")]
        public string REMARKS { get; set; }
        [DisplayName("No. Of Pax")]
        public string NoOfGuests { get; set; }

        [DisplayName("Status")]
        public short ORDER_STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public DateTime? LSTMODDATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm tt}"), DisplayName("Accepted Date")]
        public DateTime? ACCEPTED_DATE { get; set; }
        public bool IsEnableCancel { get; set; }
        public List<MeetingFoodTrnViewModel> TrnModel { get; set; }
        public List<MeetingFoodItemMstViewModel> ItemModelList { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
        public Nullable<long> OHAPPROVAL { get; set; }
        public Nullable<System.DateTime> OHAPPROVALDATE { get; set; }
        public Nullable<long> ADMINAPPROVAL { get; set; }
        public Nullable<System.DateTime> ADMINAPPDATE { get; set; }
        [DisplayName("Remarks")]
        public string OHREMARKS { get; set; }
        [DisplayName("Remarks")]
        public string ADMINREMARKS { get; set; }
        [DisplayName("Status")]
        public Nullable<short> OHSTATUS { get; set; }
        public Nullable<short> ADMINSTATUS { get; set; }
        public List<MeetingFoodItemMstViewModel> ItemModellList { get; set; }

    }

    public class MeetingFoodTrnViewModel
    {
        [DisplayName("SNo")]
        public long MEETINGFOODFOODTRN_ID { get; set; }
        public long MEETINGFOODFOODHDR_ID { get; set; }
        public long ITEMID { get; set; }

        [DisplayName("Item Name")]
        public string ITEM_NAME { get; set; }

        [DisplayName("Qty")]
        public short ITEM_QTY { get; set; }

        [DisplayName("Price")]
        public decimal ITEM_PRICE { get; set; }
        public decimal TOTAL_PRICE { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Item Status")]
        public short ITEM_STATUS { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
        public virtual MeetingFoodItemMstViewModel ItemModel { get; set; }
        public List<MeetingFoodItemMstViewModel> ItemModellList { get; set; }
        public virtual BCMstMealViewModel MealMst { get; set; }

    }

}
