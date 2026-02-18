using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ePortal.ViewModels
{
    public class FormAHeaderViewModel
    {

        [DisplayName("SNo")]
        public long FORMAHEADERID { get; set; }
        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("From Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime FROMDATE { get; set; }
        public string strFROMDATE { get; set; }

        [DisplayName("To Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime TODATE { get; set; }
        public string strTODATE { get; set; }

        [DisplayName("Sign. Authority")]
        public long SIGN_AUTHORITY { get; set; }
        public string SIGN_AUTHORITY_NAME { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayName("Added Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime DATEADDED { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DATELSTMOD { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<short> PRINT_FORMA { get; set; }
        public virtual FormADetailViewModel FormADetail { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
        public List<FormACaldayViewModel> Cal_Day_Detail { get; set; }

    }

    public class FormADetailViewModel
    {
        public long FORMADTLID { get; set; }
        public long FORMAHEADERID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DATE_OF_NOTICE { get; set; }
        public string strDATE_OF_NOTICE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DATE_OF_DISCHARGE { get; set; }
        public string strDATE_OF_DISCHARGE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DATE_OF_PREGNANCY { get; set; }
        public string strDATE_OF_PREGNANCY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DATE_OF_CHILD_BIRTH { get; set; }
        public string strDATE_OF_CHILD_BIRTH { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DATE_OF_DMD { get; set; }
        public string strDATE_OF_DMD { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DATE_OF_ILLNESS { get; set; }
        public string strDATE_OF_ILLNESS { get; set; }
        public string AMT_MATERNITY_BENEFIT { get; set; }
        public string AMT_SUBSEQUENT { get; set; }
        public string AMT_BONUS { get; set; }
        public string AMT_WAGES { get; set; }
        public string GRANTED_LEAVE_PERIOD { get; set; }
        public string NOMINEE_NAME { get; set; }
        public string DEATH_DATE { get; set; }
        public string MATERNITY_BENEFIT_PAYMENT { get; set; }
        public string INSPECTOR_REMARK { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime DATEADDED { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DATELSTMOD { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
    }

    public class FormACaldayViewModel
    {
        public string Month { get; set; }
        public string ND_Employed { get; set; }
        public string ND_LaidOff { get; set; }
        public string NDN_Employed { get; set; }
    }
}

