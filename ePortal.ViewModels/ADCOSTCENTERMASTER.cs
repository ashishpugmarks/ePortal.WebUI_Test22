using System;

namespace ePortal.ViewModels
{
    public class ADCOSTCENTERMASTER
    {
        public int ADCOSTCENTERID { get; set; }

        public int ADCOSTCENTERCODE { get; set; }

        public string ADCOSTCENTERNAME { get; set; }

        public string ADSYSITE { get; set; }

        public string ADDIVISION { get; set; }

        public DateTime ADDEDDATE { get; set; }

        public int ADDEDBY { get; set; }

        public DateTime DATELSTMOD { get; set; }

        public int MODIFIEDBY { get; set; }
    }

    public class ADCOSTCENTERMASTER_LOG
    {
        public int ADCOSTCENTERID { get; set; }

        public int ADCOSTCENTERCODE { get; set; }

        public string ADCOSTCENTERNAME { get; set; }

        public string ADSYSITE { get; set; }

        public string ADDIVISION { get; set; }

        public string LOG_MODE { get; set; }

        public string ADDEDDATE { get; set; }

        public string ADDEDBY { get; set; }

        public string DATELSTMOD { get; set; }

        public string MODIFIEDBY { get; set; }
    }
}
