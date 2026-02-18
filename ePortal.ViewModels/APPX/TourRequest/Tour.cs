using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Sockets;
using System.Reflection;

namespace ePortal.ViewModels.APPX.TourRequest
{
    public class Tour
    {
        private string _TourObjective;
        private string _TourFromDate;
        private string _TourTime;
        private string _FromLoc;
        private string _ToLoc;
        //==========Change by aumento on 13012023=====================
        private string _GSINNO;
        //============================================================
        private string _StayingLoc;
        private string _FromLocCode;
        private string _ToLocCode;
        private string _StayingLocCode;
        private string _Mode;
        private string _ModeID;
        private string _TicketClass;
        private string _TicketClassID;
        private string _ModeDetail;
        private string _TicketingBy;
        private string _TicketingByDesc;
        private string _HotelReserv;
        private string _PickDrop;
        private string _SpecialApp;
        private string _SpecialAppDesc;
        private string _Remarks;
        private string _RequestID;
        private string _RequestStatus;
        private string _UserTicketStatus;
        private string _TourPeriod;
        private string _Amount;
        private string _Day;
        private string _IDType;
        private string _IDNo;
        private string _IsGuestHouseBooked;
        private string _GuestHouseCount;
        private string _PreferredLocation; // Added By Kishan Dodiya
        private string _TourTimeTo; // Added By Kishan Dodiya
        private string _FlightDepatureTime; // Added By Kishan Dodiya
        public string TourObjective
        {
            get { return _TourObjective; }
            set { _TourObjective = value; }
        }

        public string TourFromDate
        {
            get { return _TourFromDate; }
            set { _TourFromDate = value; }
        }

        public string TourTime
        {
            get { return _TourTime; }
            set { _TourTime = value; }
        }

        public string FromLoc
        {
            get { return _FromLoc; }
            set { _FromLoc = value; }
        }
        public string ToLoc
        {
            get { return _ToLoc; }
            set { _ToLoc = value; }
        }
        //==========Change by aumento on 13012023=====================
        public string GSINNO
        {
            get { return _GSINNO; }
            set { _GSINNO = value; }
        }
        //============================================================

        public string StayingLoc
        {
            get { return _StayingLoc; }
            set { _StayingLoc = value; }
        }
        public string FromLocCode
        {
            get { return _FromLocCode; }
            set { _FromLocCode = value; }
        }
        public string ToLocCode
        {
            get { return _ToLocCode; }
            set { _ToLocCode = value; }
        }
        public string StayingLocCode
        {
            get { return _StayingLocCode; }
            set { _StayingLocCode = value; }
        }
        public string Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        public string ModeID
        {
            get { return _ModeID; }
            set { _ModeID = value; }
        }

        public string ModeDetail
        {
            get { return _ModeDetail; }
            set { _ModeDetail = value; }
        }

        public string TicketClass
        {
            get { return _TicketClass; }
            set { _TicketClass = value; }
        }

        public string TicketClassID
        {
            get { return _TicketClassID; }
            set { _TicketClassID = value; }
        }
        public string TicketingBy
        {
            get { return _TicketingBy; }
            set { _TicketingBy = value; }
        }

        public string TicketingByDesc
        {
            get { return _TicketingByDesc; }
            set { _TicketingByDesc = value; }
        }

        public string HotelReserv
        {
            get { return _HotelReserv; }
            set { _HotelReserv = value; }
        }

        public string PickDrop
        {
            get { return _PickDrop; }
            set { _PickDrop = value; }
        }

        public string SpecialApp
        {
            get { return _SpecialApp; }
            set { _SpecialApp = value; }
        }
        public string SpecialAppDesc
        {
            get { return _SpecialAppDesc; }
            set { _SpecialAppDesc = value; }
        }
        public string Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }

        public string RequestID
        {
            get { return _RequestID; }
            set { _RequestID = value; }
        }

        public string RequestStatus
        {
            get { return _RequestStatus; }
            set { _RequestStatus = value; }
        }

        public string UserTicketStatus
        {
            get { return _UserTicketStatus; }
            set { _UserTicketStatus = value; }
        }

        public string TourPeriod
        {
            get { return _TourPeriod; }
            set { _TourPeriod = value; }
        }

        public string Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }
        public string Day
        {
            get { return _Day; }
            set { _Day = value; }
        }
        public string IDType
        {
            get { return _IDType; }
            set { _IDType = value; }
        }
        public string IDNo
        {
            get { return _IDNo; }
            set { _IDNo = value; }
        }
        public string IsGuestHouseBooked
        {
            get { return _IsGuestHouseBooked; }
            set { _IsGuestHouseBooked = value; }
        }
        public string GuestHouseCount
        {
            get { return _GuestHouseCount; }
            set { _GuestHouseCount = value; }
        }

        public Tour()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        // Added By Kishan Dodiya
        public string PreferredLocation
        {
            get { return _PreferredLocation; }
            set { _PreferredLocation = value; }
        }

        // Added By Kishan Dodiya
        public string TourTimeTo
        {
            get { return _TourTimeTo; }
            set { _TourTimeTo = value; }
        }
        // Added By Kishan Dodiya
        public string FlightDepatureTime
        {
            get { return _FlightDepatureTime; }
            set { _FlightDepatureTime = value; }
        }
        /// <summary>
        /// CONSTRUCTOR FOR TOUR DETAIL PART
        /// </summary>
        /// <param name="TourObjective"></param>
        /// <param name="TourDate"></param>
        /// <param name="TourTime"></param>
        /// <param name="FromLocCode"></param>
        /// <param name="FromLoc"></param>
        /// <param name="ToLocCode"></param>
        /// <param name="ToLoc"></param> 
        //==========Change by aumento on 13012023=====================   
        /// <param name="GSINNO"></param>
        //============================================================
        /// <param name="StayingLocCode"></param>
        /// <param name="StayingLoc"></param>
        /// <param name="ModeID"></param>
        /// <param name="Mode"></param>
        /// <param name="TicketClassID"></param>
        /// <param name="TicketClass"></param>
        /// <param name="ModeDetail"></param>
        /// <param name="TicketingBy"></param>
        /// <param name="HoteReserv"></param>
        /// <param name="PickDrop"></param>
        /// <param name="SpecialApp"></param>
        /// <param name="Remarks"></param>
        public Tour(string TourObjective, string TourDate, string TourTime,
                        string FromLocCode, string FromLoc,
                        string ToLocCode, string ToLoc,
                        //==========Change by aumento on 13012023===================== 
                        //string GSINNO,
                        //============================================================
                        string StayingLocCode, string StayingLoc,
                        string ModeID, string Mode,
                        string TicketClassID, string TicketClass,
                        string ModeDetail,
                        string TicketingBy, string HoteReserv, string PickDrop,
                        string SpecialApp, string Remarks, string IDType, string IDNo, string RequestDetailID = "", string GSINNO = ""
                        // Added By Kishan Dodiya
                        , string PreferredLocation = "", string TourTimeTo = "", string FlightDepatureTime = ""
                        // End Added
                        )
        {
            _TourObjective = TourObjective;
            _TourFromDate = TourDate;
            _TourTime = TourTime;
            _FromLoc = FromLoc;
            _ToLoc = ToLoc;
            //==========Change by aumento on 13012023===================== 
            //_GSINNO = GSINNO;
            //============================================================
            _StayingLoc = StayingLoc;
            _FromLocCode = FromLocCode;
            _ToLocCode = ToLocCode;
            _StayingLocCode = StayingLocCode;
            _ModeID = ModeID;
            _Mode = Mode;
            _TicketClass = TicketClass;
            _TicketClassID = TicketClassID;
            _ModeDetail = ModeDetail;
            _TicketingBy = TicketingBy;
            if (_TicketingBy == "1")
                _TicketingByDesc = "Admin";
            else
                _TicketingByDesc = "Self";

            _HotelReserv = HoteReserv;
            _PickDrop = PickDrop;
            _SpecialApp = SpecialApp;
            if (_SpecialApp == "1")
                _SpecialAppDesc = "Yes";
            else
                _SpecialAppDesc = "No";
            _Remarks = Remarks;
            _IDType = IDType;
            _IDNo = IDNo;
            _RequestID = RequestDetailID;
            //==========Change by aumento on 13012023===================== 
            _GSINNO = GSINNO;
            _PreferredLocation = PreferredLocation; // Added By Kishan Dodiya
            _TourTimeTo = TourTimeTo; // Added By Kishan Dodiya
            _FlightDepatureTime = FlightDepatureTime; // Added By Kishan Dodiya
                                                      //============================================================
        }

        /// <summary>
        /// CONTRUCTOR FOR TOUR DETAIL PART FOR ADMIN APPROVAL
        /// </summary>
        /// <param name="TourObjective"></param>
        /// <param name="TourDate"></param>
        /// <param name="TourTime"></param>
        /// <param name="FromLocCode"></param>
        /// <param name="FromLoc"></param>
        /// <param name="ToLocCode"></param>
        /// <param name="ToLoc"></param>   
        //==========Change by aumento on 13012023=====================   
        /// <param name="GSINNO"></param>
        //============================================================
        /// <param name="StayingLocCode"></param>
        /// <param name="StayingLoc"></param>
        /// <param name="ModeID"></param>
        /// <param name="Mode"></param>
        /// <param name="TicketClassID"></param>
        /// <param name="TicketClass"></param>
        /// <param name="ModeDetail"></param>
        /// <param name="TicketingBy"></param>
        /// <param name="HoteReserv"></param>
        /// <param name="PickDrop"></param>
        /// <param name="SpecialApp"></param>
        /// <param name="Remarks"></param>
        /// <param name="RequestDetailID"></param>
        /// <param name="RequestStatus"></param>
        /// <param name="RequeststrStatus"></param>
        /// <param name="UserTicketStatas"></param>

        public Tour(string TourObjective, string TourDate, string TourTime,
                        string FromLocCode, string FromLoc,
                        string ToLocCode, string ToLoc,
                        //==========Change by aumento on 13012023===================== 
                        //string GSINNO,
                        //============================================================
                        string StayingLocCode, string StayingLoc,
                        string ModeID, string Mode,
                        string TicketClassID, string TicketClass,
                        string ModeDetail,
                        string TicketingBy, string HoteReserv, string PickDrop,
                        string RequestDetailID, string RequestStatus, string RequeststrStatus, string UserTicketStatas, string IDType, string IDNo, string GuestHouseAvailability,
                        string GuestHouseCnt,
                        //==========Change by aumento on 13012023===================== 
                        string GSINNO,
                        // Added By Kishan Dodiya
                        string PreferredLocation = "",
                        string TourTimeTo = "",
                        string FlightDepatureTime = ""
                        // End Added
                        )
        //============================================================)
        {
            _TourObjective = TourObjective;
            _TourFromDate = TourDate;
            _TourTime = TourTime;
            _FromLoc = FromLoc;
            _ToLoc = ToLoc;
            //==========Change by aumento on 13012023===================== 
            //_GSINNO = GSINNO;
            //============================================================
            _StayingLoc = StayingLoc;
            _FromLocCode = FromLocCode;
            _ToLocCode = ToLocCode;
            _StayingLocCode = StayingLocCode;
            _ModeID = ModeID;
            _Mode = Mode;
            _TicketClass = TicketClass;
            _TicketClassID = TicketClassID;
            _ModeDetail = ModeDetail;
            _TicketingBy = TicketingBy;
            _HotelReserv = HoteReserv;
            _PickDrop = PickDrop;
            _RequestID = RequestDetailID;
            _RequestStatus = RequestStatus;
            _Remarks = RequeststrStatus;
            _UserTicketStatus = UserTicketStatas;
            _IDType = IDType;
            _IDNo = IDNo;
            _IsGuestHouseBooked = GuestHouseAvailability;
            _GuestHouseCount = GuestHouseCnt;
            _PreferredLocation = PreferredLocation;// Added By Kishan Dodiya
            _TourTimeTo = TourTimeTo;// Added By Kishan Dodiya
            _FlightDepatureTime = FlightDepatureTime; // Added By Kishan Dodiya
        }



        //public Tour(string TourObjective, string TourDate, string TourTime,
        //                string FromLocCode, string FromLoc,
        //                string ToLocCode, string ToLoc,
        //                //==========Change by aumento on 13012023===================== 
        //                string GSINNO,
        //                //============================================================
        //                string StayingLocCode, string StayingLoc,
        //                string ModeID, string Mode,
        //                string TicketClassID, string TicketClass,
        //                string ModeDetail,
        //                string TicketingBy, string HoteReserv, string PickDrop,
        //                string RequestDetailID, string RequestStatus, string RequeststrStatus, string UserTicketStatas, string IDType, string IDNo, string GuestHouseAvailability,
        //                string GuestHouseCnt)
        //{
        //    _TourObjective = TourObjective;
        //    _TourFromDate = TourDate;
        //    _TourTime = TourTime;
        //    _FromLoc = FromLoc;
        //    _ToLoc = ToLoc;
        //    //==========Change by aumento on 13012023===================== 
        //    _GSINNO = GSINNO;
        //    //============================================================
        //    _StayingLoc = StayingLoc;
        //    _FromLocCode = FromLocCode;
        //    _ToLocCode = ToLocCode;
        //    _StayingLocCode = StayingLocCode;
        //    _ModeID = ModeID;
        //    _Mode = Mode;
        //    _TicketClass = TicketClass;
        //    _TicketClassID = TicketClassID;
        //    _ModeDetail = ModeDetail;
        //    _TicketingBy = TicketingBy;
        //    _HotelReserv = HoteReserv;
        //    _PickDrop = PickDrop;
        //    _RequestID = RequestDetailID;
        //    _RequestStatus = RequestStatus;
        //    _Remarks = RequeststrStatus;
        //    _UserTicketStatus = UserTicketStatas;
        //    _IDType = IDType;
        //    _IDNo = IDNo;
        //    _IsGuestHouseBooked = GuestHouseAvailability;
        //    _GuestHouseCount = GuestHouseCnt;
        //}



    }

    public class State
    {
        public string STATEMSTID { get; set; }
        public string STATEDESCRIPTION { get; set; }
    }

    public class TourRequestViewModel : TourRequestForm
    {
        [Display(Name = "TourStartDate")]
        public string TourStartDate { get; set; }
        [Display(Name = "TourEndDate")]
        public string TourEndDate { get; set; }
        [Display(Name = "TourObjective")]
        public string TourObjective { get; set; }
        [Display(Name = "DailyObjective")]
        public string DailyObjective { get; set; }
        [Display(Name = "TravelDate")]
        public string TravelDate { get; set; }
        [Display(Name = "TravelTime")]
        public string TravelTime { get; set; }
        [Display(Name = "TravelTimeTo")]
        public string TravelTimeTo { get; set; }
        [Display(Name = "FromLoc")]
        public string FromLoc { get; set; }
        [Display(Name = "ToLoc")]
        public string ToLoc { get; set; }
        [Display(Name = "StayingLoc")]
        public string StayingLoc { get; set; }
        [Display(Name = "Mode")]
        public string Mode { get; set; }
        [Display(Name = "ModeDetail")]
        public string ModeDetail { get; set; }
        [Display(Name = "TicketClass")]
        public string TicketClass { get; set; }
        [Display(Name = "TicketingBy")]
        public string TicketingBy { get; set; }
        [Display(Name = "HotelReserv")]
        public string HotelReserv { get; set; }
        [Display(Name = "PreferredLocation")]
        public string PreferredLocation { get; set; }
        [Display(Name = "SpecialApp")]
        public string SpecialApp { get; set; }
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }
        [Display(Name = "Extension")]
        public string Extension { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "AdvanceRequired")]
        public string AdvanceRequired { get; set; }
        [Display(Name = "ApprovalAuthority")]
        public string ApprovalAuthority { get; set; }
        public List<SelectListItem> OptDayStartList { get; set; }
        public List<SelectListItem> OptDayEndList { get; set; }
        public List<SelectListItem> OptMonthStartList { get; set; }
        public List<SelectListItem> OptMonthEndList { get; set; }
        public List<SelectListItem> OptYearStartList { get; set; }
        public List<SelectListItem> OptYearEndList { get; set; }

        public List<SelectListItem> OptDayList { get; set; }
        public List<SelectListItem> OptMonthList { get; set; }
        public List<SelectListItem> OptYearList { get; set; }

        public string txtMobile { get; set; }
        public string txtExtension { get; set; }
        public string BankAccount { get; set; }
        public string txtEmail { get; set; }
        public string txtObjective { get; set; }
        public string txtAdvRemarks { get; set; }
        public string HFADVREMARKS { get; set; }
        public string lblTourCoordinator { get; set; }
        public string hdnUserId { get; set; }


        public IEnumerable<SelectListItem> cboAdvanceList { get; set; }
        public IEnumerable<SelectListItem> cboAppAuthorityList { get; set; }

        public List<SelectListItem> OptHourList { get; set; }
        public List<SelectListItem> OptHourToList { get; set; }
        public List<SelectListItem> OptMinuteList { get; set; }
        public List<SelectListItem> OptMinuteToList { get; set; }



        public IEnumerable<SelectListItem> cmbFromList { get; set; }

        public IEnumerable<SelectListItem> cmbToList { get; set; }

        public IEnumerable<SelectListItem> cmbStayingList { get; set; }

        public string txtFromCity { get; set; }
        public string txtToCity { get; set; }
        public string txtStayingCity { get; set; }

        public IEnumerable<SelectListItem> cmbModeList { get; set; }
        public IEnumerable<SelectListItem> cmbClassList { get; set; }
        public IEnumerable<SelectListItem> ddlidtypeList { get; set; }

        public string txtidcardno { get; set; }
        public List<SelectListItem> cmbTicketingByList { get; set; }
        public List<SelectListItem> cmbHotelReservList { get; set; }
        public bool chkSpecialApp { get; set; }
        public string txtRemarks { get; set; }
        public string txtMiscAmout { get; set; }
        public string txtMiscRemarks { get; set; }
        public string txtRequiredAmount { get; set; }
        public string txtDailyObjective { get; set; }
        public string txt_GST { get; set; }
        public string TxnNumber { get; set; }
        public string lblTotalCharges { get; set; }
        public string lblStayChargeAPLUS { get; set; }
        public string lblStayChargeA { get; set; }
        public string lblStayChargeB { get; set; }
        public string lblStayChargeC { get; set; }
        public string lblTotalNights { get; set; }
        public string txtModeDetail { get; set; }
        public string preferredLocation { get; set; }
        public IEnumerable<SelectListItem> TourPeriodList { get; set; }
        public IEnumerable<SelectListItem> Cities { get; set; }
        public IEnumerable<SelectListItem> TravelModes { get; set; }
        public IEnumerable<grdPrevList> grdPrevList { get; set; }
        public IEnumerable<gvList> gvList { get; set; }

        public IEnumerable<gvAdvance> gvAdvance { get; set; }
        public IEnumerable<grdprvadv> grdprvadv { get; set; }

        public TourRequestViewModel()
        {

            // Days (1 to 31)
            OptDayList = Enumerable.Range(1, 31)
                .Select(d => new SelectListItem { Text = d.ToString(), Value = d.ToString() })
                .ToList();

            OptDayStartList = OptDayList;
            OptDayEndList = OptDayList;


            OptMonthList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Jan", Value="1"},
                new SelectListItem{Text = "Feb", Value="2"},
                new SelectListItem{Text = "Mar", Value="3"},
                new SelectListItem{Text = "Apr", Value="4"},
                new SelectListItem{Text = "May", Value="5"},
                new SelectListItem{Text = "Jun", Value="6"},
                new SelectListItem{Text = "Jul", Value="7"},
                new SelectListItem{Text = "Aug", Value="8"},
                new SelectListItem{Text = "Sep", Value="9"},
                new SelectListItem{Text = "Oct", Value="10"},
                new SelectListItem{Text = "Nov", Value="11"},
                new SelectListItem{Text = "Dec", Value="12"}
            };

            OptMonthStartList = OptMonthList;
            OptMonthEndList = OptMonthList;

            // Years (current year -1 to current year + 19)
            int currentYear = DateTime.Now.Year;
            OptYearList = Enumerable.Range(currentYear - 1, 21)
                .Select(y => new SelectListItem { Text = y.ToString(), Value = y.ToString() })
                .ToList();

            OptYearStartList = OptYearList;
            OptYearEndList = OptYearList;


            // Hours (0 to 23)
            OptHourList = Enumerable.Range(0, 24)
                .Select(h => new SelectListItem { Text = h.ToString(), Value = h.ToString() })
                .ToList();

            OptHourToList = OptHourList;

            // Minutes (0 to 59)
            OptMinuteList = Enumerable.Range(0, 60)
                .Select(m => new SelectListItem { Text = m.ToString(), Value = m.ToString() })
                .ToList();

            OptMinuteToList = OptMinuteList;

            int currentMonth = DateTime.Now.Month;
            int currentDay = DateTime.Now.Day;

            OptDayStart = currentDay.ToString();
            OptMonthStart = currentMonth.ToString();
            OptYearStart = currentYear.ToString();

            OptDayEnd = currentDay.ToString();
            OptMonthEnd = currentMonth.ToString();
            OptYearEnd = currentYear.ToString();

            OptDay = currentDay.ToString();
            OptMonth = currentMonth.ToString();
            OptYear = currentYear.ToString();


            OptMonthStartList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Jan", Value="1"},
                new SelectListItem{Text = "Feb", Value="2"},
                new SelectListItem{Text = "Mar", Value="3"},
                new SelectListItem{Text = "Apr", Value="4"},
                new SelectListItem{Text = "May", Value="5"},
                new SelectListItem{Text = "Jun", Value="6"},
                new SelectListItem{Text = "Jul", Value="7"},
                new SelectListItem{Text = "Aug", Value="8"},
                new SelectListItem{Text = "Sep", Value="9"},
                new SelectListItem{Text = "Oct", Value="10"},
                new SelectListItem{Text = "Nov", Value="11"},
                new SelectListItem{Text = "Dec", Value="12"}
            };

            OptMonthEndList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Jan", Value="1"},
                new SelectListItem{Text = "Feb", Value="2"},
                new SelectListItem{Text = "Mar", Value="3"},
                new SelectListItem{Text = "Apr", Value="4"},
                new SelectListItem{Text = "May", Value="5"},
                new SelectListItem{Text = "Jun", Value="6"},
                new SelectListItem{Text = "Jul", Value="7"},
                new SelectListItem{Text = "Aug", Value="8"},
                new SelectListItem{Text = "Sep", Value="9"},
                new SelectListItem{Text = "Oct", Value="10"},
                new SelectListItem{Text = "Nov", Value="11"},
                new SelectListItem{Text = "Dec", Value="12"}
            };

            cboAdvanceList = new List<SelectListItem>
            {
                new SelectListItem{Text = "--select--" , Value = string.Empty},
                new SelectListItem{Text = "YES", Value="YES"},
                new SelectListItem{Text = "NO", Value="NO"}
            };

            //cboAppAuthorityList = new List<SelectListItem>();

            cmbTicketingByList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Admin", Value="1"},
                new SelectListItem{Text = "Self", Value="0"}
            };

            cmbHotelReservList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Admin", Value="1"},
                new SelectListItem{Text = "Self", Value="0"}
            };



            ddlidtypeList = new List<SelectListItem>
            {
                new SelectListItem{Text = "--Select--", Value="0"},
                new SelectListItem{Text = "Driving License", Value="1"},
                new SelectListItem{Text = "Passport", Value="2"},
                new SelectListItem{Text = "Pan Card", Value="3"},
                new SelectListItem{Text = "Voter I-Card", Value="4"},
                new SelectListItem{Text = "Unique I- Card", Value="5"},
            };


        }
    }
    public class MovementDetailsViewModel
    {
    }
    public class TourRequestListViewModel
    {
        public bool cboFilter { get; set; }
        public List<TourRequestListModel> TourRequestList { get; set; }
    }

    public class TourRequestForm
    {
        public string OptDayStart { get; set; }
        public string OptDayStartText { get; set; }
        public string OptDayEnd { get; set; }
        public string OptDayEndText { get; set; }
        public string OptMonthStart { get; set; }
        public string OptMonthStartText { get; set; }
        public string OptMonthEnd { get; set; }
        public string OptMonthEndText { get; set; }
        public string OptYearStart { get; set; }
        public string OptYearStartText { get; set; }
        public string OptYearEnd { get; set; }
        public string OptYearEndText { get; set; }
        public string cboAdvance { get; set; }
        public string cboAdvanceText { get; set; }
        public string cboAppAuthority { get; set; }
        public string cboAppAuthorityText { get; set; }


        public string OptHour { get; set; }
        public string OptHourText { get; set; }
        public string OptHourTo { get; set; }
        public string OptHourToText { get; set; }
        public string OptMinute { get; set; }
        public string OptMinuteText { get; set; }
        public string OptMinuteTo { get; set; }
        public string OptMinuteToText { get; set; }

        public string OptDay { get; set; }
        public string OptDayText { get; set; }
        public string OptMonth { get; set; }
        public string OptMonthText { get; set; }
        public string OptYear { get; set; }
        public string OptYearText { get; set; }

        public string HFFROMCITY { get; set; }
        public string HFFROMOTHERCITY { get; set; }

        public string cmbFrom { get; set; }
        public string cmbFromText { get; set; }

        public string cmbTo { get; set; }
        public string cmbToText { get; set; }

        public string cmbStaying { get; set; }
        public string cmbStayingText { get; set; }

        public string cmbMode { get; set; }
        public string cmbModeText { get; set; }
        public string cmbClass { get; set; }
        public string cmbClassText { get; set; }
        public string ddlidtype { get; set; }
        public string ddlidtypeText { get; set; }

        public string cmbTicketingBy { get; set; }
        public string cmbTicketingByText { get; set; }
        public string cmbHotelReserv { get; set; }
        public string cmbHotelReservText { get; set; }

        public string ddltourperiod { get; set; }
        public string ddltourperiodText { get; set; }



    }
    public class grdPrevList
    {
        public string ADTOURREQUESTID { get; set; }
        public string OBJOFJOURNEY { get; set; }
        public DateTime STARTDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public decimal TOTALAMOUNT { get; set; }
    }

    public class gvList
    {
        public string Id { get; set; }
        public string TourObjective { get; set; }
        public string TourFromDate { get; set; }
        public string TourTime { get; set; }
        public string OBJOFJOURNEY { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public string StayingLoc { get; set; }
        public string Mode { get; set; }
        public string TicketClass { get; set; }
        public string ModeDetail { get; set; }
        public string SpecialAppDesc { get; set; }
        public string TicketingByDesc { get; set; }
    }

    public class gvListETR
    {
        public string Id { get; set; }
        public string TourObjective { get; set; }
        public string TourFromDate { get; set; }
        public string TourTime { get; set; }
        public string OBJOFJOURNEY { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public string StayingLoc { get; set; }
        public string Mode { get; set; }
        public string TicketClass { get; set; }
        public string ModeDetail { get; set; }
        public string SpecialAppDesc { get; set; }
        public string TicketingByDesc { get; set; }
    }


    public class gvListCTR
    {
        public string RequestID { get; set; }
        public string RequestStatus { get; set; }
        public string TourFromDate { get; set; }  
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public string StayingLoc { get; set; }
        public string Mode { get; set; }
        public string TicketClass { get; set; }
        public string ModeDetail { get; set; }
        public string TicketingBy { get; set; }
        public string UserTicketStatus { get; set; }
        public string Remarks { get; set; }
    }

    public class gvAdvance
    {
        public string TourPeriod { get; set; }
        public string Amount { get; set; }
        public string Day { get; set; }
    }

    public class grdprvadv
    {
        public string Tourperiod { get; set; }
        public decimal Advance { get; set; }
        public string Exp { get; set; }
        public string Balance { get; set; }
        public string Subdate { get; set; }
        public string Refundbydes { get; set; }
        public string Chqno { get; set; }
        public string Chqdate { get; set; }
        public string Refamount { get; set; }
    }

    public class gvTacList
    {
        public string TRAVELFROMDATE { get; set; }
        public string TRAVELTIME { get; set; }
        public string FRMCITY { get; set; }
        public string TOCITY { get; set; }
        public string TRAVELMODE { get; set; }
    }

    public class TourRequestListModel
    {
        public int ADTOURREQUESTID { get; set; }
        public string ADEMPCODE { get; set; }
        public string EMPNAME { get; set; }
        public DateTime REQUESTDATE { get; set; }
        public DateTime STARTDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public string ISADVANCEREQ { get; set; }
        public string REQUESTSTATUS { get; set; }
    }

    public class CancelTourRequestViewModel
    {
        public string lblRequestID { get; set; }
        public string lblObjective { get; set; }
        public string lblPeriod { get; set; }
        public string lblDays { get; set; }
        public string miscAmount { get; set; }
        public string requiredAmount { get; set; }
        public string TotalAmount { get; set; }
        public string empDesignationId { get; set; }
        public string applicationDate { get; set; }
        public string lblAdvRemarks { get; set; }
        public bool isAdvanceRequired { get; set; }
        public IEnumerable<gvAdvance> gvAdvance { get; set; }
        public IEnumerable<gvListCTR> gvList { get; set; }

        public string lblStayChargeAPLUS { get; set; }
        public string lblAllownceAPLUS { get; set; }
        public string lblStayChargeA { get; set; }
        public string lblAllownceA { get; set; }
        public string lblStayChargeB { get; set; }
        public string lblAllownceB { get; set; }
        public string lblStayChargeC { get; set; }
        public string lblAllownceC { get; set; }

        public string lblTotalNights { get; set; }
        public string lblTotalDays { get; set; }
        public string lblTotalAllowances { get; set; }
        public string lblTotalCharges { get; set; }
        public string ADEMPCODE { get; set; }


        public List<Tour> TourList { get; set; } = new();

    }

    public class CancelButtonViewModel
    {
        public string txtCancellationRemarks { get; set; }
        public string lblObjective { get; set; }
        public string lblPeriod { get; set; }
        public string lblDays { get; set; }
        public string lblRequestID { get; set; }
    }
    public class TourApplicationDetailViewModel
    {
        public bool pnlRecAuth { get; set; }
        public bool pnlSPAuth { get; set; }
        public bool pnlDir2Auth { get; set; }
        public bool pnlFinance { get; set; }
        public bool pnlAdmin { get; set; }

        public string ltlEmpCode { get; set; }
        public string ltlEmpName { get; set; }
        public string ltlObjective { get; set; }
        public string ltlPeriod { get; set; }
        public string ltlDays { get; set; }
        public string ltlRequestDate { get; set; }
        public string ltlRequestStatus { get; set; }
        public string ltlRecAuth { get; set; }
        public string ltlRecEmail { get; set; }
        public string ltlRecStatus { get; set; }
        public string ltlRecRem { get; set; }
        public string ltlRecDate { get; set; }
        public string ltlSPAppName { get; set; }
        public string ltlSPAppEmail { get; set; }
        public string ltlSPAppStatus { get; set; }
        public string ltlSPAppRemarks { get; set; }
        public string ltlSPAppDate { get; set; }
        public string ltlDir2AppName { get; set; }
        public string ltlDir2AppEmail { get; set; }
        public string ltlDir2AppStatus { get; set; }
        public string ltlDir2AppRemarks { get; set; }
        public string ltlDir2AppDate { get; set; }
        public string ltlAppName { get; set; }
        public string ltlAppEmail { get; set; }
        public string ltlAppStatus { get; set; }
        public string ltlAppRemarks { get; set; }
        public string ltlAppDate { get; set; }
        public string ltlFinName { get; set; }
        public string ltlFinEmail { get; set; }
        public string ltlFinRemarks { get; set; }
        public string ltlFinStatus { get; set; }
        public string ltlFinAppDate { get; set; }
        public string ltlTransferAmount { get; set; }
        public string ltlAdminName { get; set; }
        public string ltlAdminEmail { get; set; }
        public string ltlAdminRemarks { get; set; }
        public string ltlAdminStatus { get; set; }
        public string ltlAdminAppDate { get; set; }
    }

    public class TourBookingDetailViewModel
    {
        public IEnumerable<gvDayWiseTourList> gvDayWiseTourList { get; set; }
        public TourBookingDetailViewModel()
        {

        }
    }
    public class gvDayWiseTourList
    {
        public string ADTOURREQDETAILID { get; set; }
        public string TRAVELFROMDATE { get; set; }
        public string TRAVELTIME { get; set; }
        public string FRMCITY { get; set; }
        public string TOCITY { get; set; }
        public string TRAVELMODE { get; set; }
        public string TICKETCLASS { get; set; }
        public string FLIGHTTRAINNO { get; set; }
        public string HOTELRESERVATION { get; set; }
        public string TICKETINGBY { get; set; }
        public string PNRNO { get; set; }
        public string SpecialAppDesc { get; set; }
        public string TICKETNO { get; set; }
        public string TICKETAMOUNT { get; set; }
        public string BOOKEDHOTELNAME { get; set; }
        public string BOOKEDHOTELADDRESS { get; set; }
        public string CHECKINDATE { get; set; }
        public string CHECKINTIME { get; set; }
        public string CHECKOUTDATE { get; set; }
        public string CHECKOUTTIME { get; set; }
        public string PICKUPDROPDETAILS { get; set; }
        public string USERTICKETSTATUS { get; set; }
        public string TICKETSTATUS { get; set; }
        public string ADMINCACELLATIONREMARKS { get; set; }
        public string GSTINNO { get; set; }
    }

    public class TourRequestApprovalViewModel
    {
        public List<Tour> TourList { get; set; } = new();
        public List<TourAdvance> TourAdvacneList { get; set; } = new();
        public string RequestID { get; set; }
        public string EmpCode { get; set; }
        public bool pnlAdvance { get; set; }
        public bool pnlRecomend { get; set; }
        public bool pnlApproval { get; set; }
        public bool pnlRecommendBtns { get; set; }
        public bool pnlApprovalBtns { get; set; }

        public string lblEmpName { get; set; }
        public string txtMobile { get; set; }
        public string txtExtension { get; set; }
        public string txtEmail { get; set; }
        public string txtObjective { get; set; }
        public string txtAdvRemarks { get; set; }
        public string lblMiscAmount { get; set; }
        public string lblRequiredAmount { get; set; }
        public string lblMiscRemarks { get; set; }
        public string lblRecEmpName { get; set; }
        public string lblRecomRemarks { get; set; }
        public string lblAppEmpName { get; set; }
        public string lblAppRemarks { get; set; }
        public string rblSPList { get; set; }
        public string lblSPAppRemarks { get; set; }
        public string lblRequestDate { get; set; }
        public string lblapproxticketamt { get; set; }
        public string lblReturnRemarks { get; set; }
        public bool trReturnVisible { get; set; }
        public string hfAuthType { get; set; }
        public string hfDir2EmpCode { get; set; }
        public string lblAdvAmt { get; set; }
        public bool chkAdvanceChecked { get; set; }
        public bool trAppByVisible { get; set; }
        public bool trAppRemarksVisible { get; set; }
        string stropid { get; set; }
        public string hdnReqoperationid { get; set; }
        public string lbltravelplan { get; set; }
        public string lbltravelactual { get; set; }
        public string lblbalancebudget { get; set; }
        public string lblbudgetconsumed { get; set; }
        public string lblbalancebudgetafttour { get; set; }
        public string hdntotalrequiredamount { get; set; }
        public string lbltotaltouramt { get; set; }
        public string lblStayChargeAPLUS { get; set; }
        public string lblAllownceAPLUS { get; set; }
        public string lblStayChargeA { get; set; }
        public string lblAllownceA { get; set; }
        public string lblStayChargeB { get; set; }
        public string lblAllownceB { get; set; }
        public string lblStayChargeC { get; set; }
        public string lblAllownceC { get; set; }

        public string lblTotalNights { get; set; }
        public string lblTotalDays { get; set; }
        public string lblTotalAllowances { get; set; }
        public string lblTotalCharges { get; set; }
        public string lblTotalAmount { get; set; }
        public bool trSPVisible { get; set; }
        public string appstatus { get; set; }
        public string totalRequiredAmount { get; set; }
        public string intthamount { get; set; }
        public string approxTicketAmount { get; set; }
        public string balanceBudget { get; set; }
        public string budgetError { get; set; }
        public string rblRecomend { get; set; }
        public string txtRecRemarks { get; set; }
        public string rblApprovalStatus { get; set; }
        public string txtAPPRemarks { get; set; }
        public bool errorbudgetVisible { get; set; }
        public string lblbudget { get; set; }
        public bool lblMsgVisible { get; set; }
        public bool pnlMsgVisible { get; set; }
        public string lblsplmag { get; set; }

        public string lblMsg { get; set; }
        public IEnumerable<SelectListItem> cboAppAuthorityList { get; set; }
        public string cboAppAuthority { get; set; }
        public IEnumerable<SelectListItem> cboSPAppAuthorityList { get; set; }
        public string cboSPAppAuthority { get; set; }

        public IEnumerable<grdprvadv> grdprvadv { get; set; }
        public IEnumerable<gvList> gvList { get; set; }

        public IEnumerable<gvAdvance> gvAdvance { get; set; }
        public TourRequestApprovalViewModel()
        {
        }
    }


    public class TourAuthChangeViewModel
    {
        public bool errorpanel { get; set; }
        public bool btnSave { get; set; }

        public bool tr1 { get; set; }
        public bool tr_rec { get; set; }
        public bool tr_RecApp { get; set; }
        public bool tr_App { get; set; }
        public bool tr_AppAuth { get; set; }
        public bool tr_SpApp { get; set; }
        public bool tr_SpAppAuth { get; set; }

        public string lblPageRequestID { get; set; }
        public string lblEmpName { get; set; }
        public string txtMobile { get; set; }
        public string txtExtension { get; set; }
        public string txtObjective { get; set; }
        public string lblDesignation { get; set; }
        public string lblOperation { get; set; }
        public string lblDivision { get; set; }
        public string lblDepartment { get; set; }
        public string lblSection { get; set; }
        public string hfAuthType { get; set; }
        public string lblRecAuth { get; set; }
        public string lblRecDate { get; set; }
        public string lblRecStatus { get; set; }
        public IEnumerable<SelectListItem> cboRecAuthorityList { get; set; }
        public string cboRecAuthority { get; set; }
        public string lblAppAuth { get; set; }
        public string lblAppDate { get; set; }
        public string lblAppStatus { get; set; }
        public IEnumerable<SelectListItem> cboAppAuthorityList { get; set; }
        public string cboAppAuthority { get; set; }
        public string lblSplAppAuth { get; set; }
        public string lblSplAppDate { get; set; }
        public string lblSpAppStatus { get; set; }
        public IEnumerable<SelectListItem> cboSpAppAuthorityList { get; set; }
        public string cboSpAppAuthority { get; set; }

        public string status { get; set; }

        public IEnumerable<gvTacList> gvList { get; set; }
        public TourAuthChangeViewModel()
        {
        }
    }

    public class grdTourScheduleRequestList
    {
        public string TRAVELDATE { get; set; }
        public string DAYNAME { get; set; }
        public string TRAVELTIME { get; set; }
        public string DAYOBJECTIVE { get; set; }
        public string FRMCITY { get; set; }
        public string TOCITY { get; set; }
        public string STCITY { get; set; }
        public string TRAVELMODE { get; set; }
        public string TICKETCLASS { get; set; }
        public string FLIGHTTRAINNO { get; set; }
        public string TICKETINGBY { get; set; }
    }
    public class TourScheduleViewModel
    {
        public IEnumerable<SelectListItem> cboAssociateList { get; set; }
        public string cboAssociate { get; set; }
        public List<SelectListItem> optMonthList { get; set; }
        public string optMonth { get; set; }
        public List<SelectListItem> optYearList { get; set; }
        public string optYear { get; set; } 
        
        public TourScheduleViewModel() { }
    }

    public class TourRequestDuplicateListViewModel
    {
        public TourRequestDuplicateListViewModel() { }
    }

    public class EditTourRequestViewModel
    {
        public List<SelectListItem> optDayStartList { get; set; }
        public string optDayStart { get; set; }
        public List<SelectListItem> optMonthStartList { get; set; }
        public string optMonthStart { get; set; }
        public List<SelectListItem> optYearStartList { get; set; }
        public string optYearStart { get; set; }
        public List<SelectListItem> optDayEndList { get; set; }
        public string optDayEnd { get; set; }
        public List<SelectListItem> optMonthEndList { get; set; }
        public string optMonthEnd { get; set; }
        public List<SelectListItem> optYearEndList { get; set; }
        public string optYearEnd { get; set; }
        public List<SelectListItem> ddltourperiodList { get; set; }
        public string ddltourperiod { get; set; }
        public List<SelectListItem> ddlrefundbyList { get; set; }
        public string ddlrefundby { get; set; }
        public List<SelectListItem> optDayList { get; set; }
        public string optDay { get; set; }
        public List<SelectListItem> optMonthList { get; set; }
        public string optMonth { get; set; }
        public List<SelectListItem> optYearList { get; set; }
        public string optYear { get; set; }
        public List<SelectListItem> optHourList { get; set; }
        public string optHour { get; set; }
        public List<SelectListItem> optMinuteList { get; set; }
        public string optMinute { get; set; }
        public List<SelectListItem> optHourToList { get; set; }
        public string optHourTo { get; set; }
        public List<SelectListItem> optMinuteToList { get; set; }
        public string optMinuteTo { get; set; }
        public IEnumerable<SelectListItem> cmbFromList { get; set; }
        public string cmbFrom { get; set; }
        public IEnumerable<SelectListItem> cmbToList { get; set; }
        public string cmbTo { get; set; }
        public IEnumerable<SelectListItem> cmbStayingList { get; set; }
        public string cmbStaying { get; set; }
        public IEnumerable<SelectListItem> cmbModeList { get; set; }
        public string cmbMode { get; set; }
        public IEnumerable<SelectListItem> cmbClassList { get; set; }
        public string cmbClass { get; set; }
        public List<SelectListItem> ddlidtypeList { get; set; }
        public string ddlidtype { get; set; }
        public List<SelectListItem> cmbTicketingByList { get; set; }
        public string cmbTicketingBy { get; set; }
        public bool cmbTicketingByEnabled { get; set; }
        public List<SelectListItem> cmbHotelReservList { get; set; }
        public string cmbHotelReserv { get; set; }
        public List<SelectListItem> cmbPickDropList { get; set; }
        public string cmbPickDrop { get; set; }
        public string lblBankAccount { get; set; }
        public string txtMobile { get; set; }
        public string txtExtension { get; set; }
        public string txtEmail { get; set; }
        public string txtObjective { get; set; }
        public string txtAdvRemarks { get; set; }
        public bool chkAdvance { get; set; }
        public bool chkAdvanceChecked { get; set; }
        public string lbladvance { get; set; }
        public string lblexp { get; set; }
        public string lblbalance { get; set; }
        public string lblsubdate { get; set; }
        public string lblrefundby { get; set; }
        public string lblchqno { get; set; }
        public string lblrefamount { get; set; }
        public string txtadvance { get; set; }
        public string txtexp { get; set; }
        public string txtbalance { get; set; }
        public string txtsubdate { get; set; }
        public string txtchqno { get; set; }
        public string txtchqdate { get; set; }
        public string txtrefamount { get; set; }
        public string txtDailyObjective { get; set; }
        public string txtFromCity { get; set; }
        public string txtToCity { get; set; }
        public string txtStayingCity { get; set; }
        public string txtModeDetail { get; set; }
        public string txtidcardno { get; set; }
        public string preferredLocation { get; set; }
        public bool chkSpecialApp { get; set; }
        public string TxtRemarks { get; set; }
        public string txtRemarks { get; set; }
        public string txtMiscAmout { get; set; }
        public string txtMiscRemarks { get; set; }
        public string lblTotalAmount { get; set; }
        public string txtRequiredAmount { get; set; }
        public string lblTourPeriod { get; set; }
        public string lblchqdate { get; set; }
        public bool pnlAdvance { get; set; }
        public bool btnSaveAsDraft { get; set; }
        public string lblStayChargeAPLUS { get; set; }
        public string lblAllownceAPLUS { get; set; }
        public string lblStayChargeA { get; set; }
        public string lblAllownceA { get; set; }
        public string lblStayChargeB { get; set; }
        public string lblAllownceB { get; set; }
        public string lblStayChargeC { get; set; }
        public string lblAllownceC { get; set; }

        public string lblTotalNights { get; set; }
        public string lblTotalDays { get; set; }
        public string lblTotalAllowances { get; set; }
        public string lblTotalCharges { get; set; }
        public string lblMiscAmount { get; set; }

        public IEnumerable<gvListETR> gvList { get; set; }
        public IEnumerable<grdprvadv> grdprvadv { get; set; }
        public IEnumerable<gvAdvance> gvAdvance { get; set; }
        public bool tr_train { get; set; }

        public EditTourRequestViewModel()
        {
        }
    }

    public class UpdateTourDetailRequest
    {
        public string RequestID { get; set; }
        public string MobileNo { get; set; }
        public string ExtNo { get; set; }
        public string Email { get; set; }
        public string Objective { get; set; }
        public string AdvRemarks { get; set; }
        public int AdvRequired { get; set; }

        public string APlusNights { get; set; }
        public string ANights { get; set; }
        public string BNights { get; set; }
        public string CNights { get; set; }
        public double StayCharge { get; set; }
        public string APlusDays { get; set; }
        public string ADays { get; set; }
        public string BDays { get; set; }
        public string CDays { get; set; }
        public double DailyAllow { get; set; }
        public double MiscAllow { get; set; }

        public List<Tour> oTourList { get; set; }
        public string MiscRemarks { get; set; }
        public double RequiredAmount { get; set; }
        public List<TourAdvance> oTourListprv { get; set; }
        public string Initiator_Status { get; set; }
        public List<Tour> _deleted { get; set; }
    }

    public class InsertTourDetailRequest
    {
        public string MobileNo { get; set; }
        public string ExtNo { get; set; }
        public string Email { get; set; }
        public string Objective { get; set; }
        public string AdvRemarks { get; set; }
        public int AdvRequired { get; set; }

        public string APlusNights { get; set; }
        public string ANights { get; set; }
        public string BNights { get; set; }
        public string CNights { get; set; }
        public double StayCharge { get; set; }
        public string APlusDays { get; set; }
        public string ADays { get; set; }
        public string BDays { get; set; }
        public string CDays { get; set; }
        public double DailyAllow { get; set; }
        public double MiscAllow { get; set; }
        public string authType { get; set; }
        public string AppAuthCode { get; set; }

        public List<Tour> oTourList { get; set; }
        public string MiscRemarks { get; set; }
        public double RequiredAmount { get; set; }
        public string TourStartDate { get; set; }
        public string TourEndDate { get; set; }
        public List<TourAdvance> oTourListprv { get; set; }
        public string Initiator_Status { get; set; }
        public string AppAuthEmailID { get; set; }
    }


    public class SaveTourRequestApproval
    {
        public string RequestID { get; set; }
        public string EmpCode { get; set; }
        public string authType { get; set; }
        public string appstatus { get; set; }
        public string strStatus { get; set; }
        public string remarks { get; set; }
        public string appAuthCode { get; set; }
        public string spAppAuthCode { get; set; }
        public string intthamount { get; set; }
        public string totalRequiredAmount { get; set; }
        public string approxTicketAmount { get; set; }
        public string balanceBudget { get; set; }
        public string dir2EmpCode { get; set; }
        public string reqOperationId { get; set; }
        public string rblSPList { get; set; }
        public string trSPVisible { get; set; }
    }
}
