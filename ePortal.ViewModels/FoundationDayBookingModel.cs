using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    
    public class FoundationDayBookingTrnViewModel
    {
        [DisplayName("SNo")]
        public long BC_FAMILY_BOOKINGID { get; set; }
      
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Foundation Day Date")]
        public System.DateTime MEAL_BOOKED_DATE { get; set; }
        public string strBOOKED_DATE { get; set; }

        [DisplayName("Status")]
        public short MEAL_STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Request Date")]
        public System.DateTime ADDEDDATE { get; set; }
        public long? LSTMODIFIEDBY { get; set; }
        public System.DateTime? LSTMODDATE { get; set; }
        
        public short? ADMIN_STAUS { get; set; }
        [DisplayName("TRANSPORT TYPE")]
        public long? TRANSPORT_TYPE { get; set; }
        public long? VEHICLE_TYPE { get; set; }
        public System.DateTime? ADMIN_DATE { get; set; }
        public long? ADMIN_ECODE { get; set; }
        public bool IsEnableCancel { get; set; }
        public List<FoundationDayBookingDtlViewModel> MEALBOOKING_DTL { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
        public long lngbookingid { get; set; }
        public DateTime cancellationtime { get; set; }
        public decimal? WHATSAPPNUMBER { get; set; }// #Upgrade Aumento :: CR7504
        public string? PERSONALEMAIL { get; set; }// #Upgrade Aumento :: CR7504

    }
    public class FoundationDayBookingDtlViewModel
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

        public short? STATUS { get; set; }
    }

    public class FD_VALIDATION_ViewModel
    {
        public long BC_MST_VALIDATION_ID { get; set; }
        public DateTime VISIT_CANCEL_DATE { get; set; }
        public DateTime VISIT_START_DATE { get; set; }
        public DateTime VISIT_END_DATE { get; set; }
        public DateTime VISIT_BOOKING_DATE { get; set; }
        public long SITE_ID { get; set; }

    }

    public class SearchFDREPORT_ViewModel
    {
        [DisplayName("Request From Date")]
        public string Startdate { get; set; }

        [DisplayName("Request End Date")]
        public string ENDDATE { get; set; }
        
        [DisplayName("Ecode")]
        public long ecode { get; set; }

        [DisplayName("Status")]
        public short ReqStatus { get; set; }
        public List<FDREPORT_ViewModel> SearchResult { get; set; }
    }
    public class FDREPORT_ViewModel
    {
        [DisplayName("SNo")]
        public long BC_FAMILYMEALBOOKING_DTLID { get; set; }
        public long BC_FAMILY_BOOKINGID { get; set; }

        [DisplayName("Member Name")]
        public string MEMBER_NAME { get; set; }

        [DisplayName("Relation Type")]
        public string RELATION_TYPE { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Request Date")]
        public DateTime RequestDate { get; set; }

        [DisplayName("Requestor Ecode")]
        public string ReqEcode { get; set; }

        [DisplayName("Requestor Name")]
        public string Req_NAME { get; set; }

        [DisplayName("Foundation Day Date")]
        public DateTime BookingDate { get; set; }

        [DisplayName("Mobile No.")]
        public string MobileNo { get; set; }
        [DisplayName("Site")]
        public string SiteDesc { get; set; }

        [DisplayName("Transport type")]
        public long? TRANSPORT_TYPE { get; set; }

        [DisplayName("Vehicle Type")]
        public long? VEHICLE_TYPE { get; set; }
        //  #Upgrade Aumento Start :: CR7504
        [DisplayName("Whatsapp No.")]
        public Nullable<long> WHATSAPPNUMBER { get; set; }
        [DisplayName("Personal Email")]
        public string? PERSONALEMAIL { get; set; }
        //  #Upgrade Aumento End :: CR7504
    }
}
