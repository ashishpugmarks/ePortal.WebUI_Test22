using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web;
using System;
using System.ComponentModel.DataAnnotations;
using ePortal.DomainClasses;
using ePortal.ViewModels;


namespace ePortal.ViewModels
{
    public class LocalConveyanceRequestViewModel
    {
        public int SRNO { get; set; }
        public string EMPCODE { get; set; }
        public string EMPNAME { get; set; }
        public string ASOFFODID { get; set; }
        public string DATEOFEXPENDITURE { get; set; }
        public string PURPOSE { get; set; }
        public string FROMCITY { get; set; }
        public string TOCITY { get; set; }
        public string FROMCITYID { get; set; }
        public string TOCITYID { get; set; }
        public string TIMEIN { get; set; }
        public string TIMEOUT { get; set; }
        public string NOOFHOURS { get; set; }
        public string MODEOFTRAVEL { get; set; }
        public string KMCOVERED { get; set; }
        public string RATEPERKM { get; set; }
        public string TRAVELREIMBURSEMENT { get; set; }
        public decimal TOLLTAXMISCAMOUNT { get; set; }
        public decimal TOTAL { get; set; }
        public string DT { get; set; }
        public string FROMCITY_OTHER { get; set; }
        public string TOCITY_OTHER { get; set; }
        public string MODEOFTRAVEL_OTHER { get; set; }
        public string MEAL_REFRE_ALLW_CLAIM { get; set; }
        public string REFRESHMENTMEALALLOWANCE { get; set; }
        public string FUEL_ALLW_CLAIM { get; set; }
        public string FUEL_REIMBURSMENT { get; set; }
        public string DINNER_ALLW_CLAIM { get; set; }
        public string DINNER_ALLW_AMOUNT { get; set; }
        public string LUNCH_ALLW_CLAIM { get; set; }
        public string LUNCH_ALLW_AMOUNT { get; set; }
        public string TOURSTARTFROMHOMEKM { get; set; }

        public string FromCityName { get; set; }
        public string ToCityName { get; set; }


    }

   


}

