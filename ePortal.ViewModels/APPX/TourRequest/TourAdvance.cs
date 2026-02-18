namespace ePortal.ViewModels.APPX.TourRequest
{
    public class TourAdvance //: Tour
    {
        private string _Tourperiod;
        private string _Advance;
        private string _Exp;
        private string _Balance;
        private string _Subdate;
        private string _Refundby;
        private string _Refundbydes;
        private string _Chqno;
        private string _Chqdate;
        private string _Refamount;



        public string Tourperiod
        {
            get { return _Tourperiod; }
            set { _Tourperiod = value; }
        }

        public string Advance
        {
            get { return _Advance; }
            set { _Advance = value; }
        }

        public string Exp
        {
            get { return _Exp; }
            set { _Exp = value; }
        }

        public string Balance
        {
            get { return _Balance; }
            set { _Balance = value; }
        }

        public string Subdate
        {
            get { return _Subdate; }
            set { _Subdate = value; }
        }
        public string Refundby
        {
            get { return _Refundby; }
            set { _Refundby = value; }
        }
        public string Refundbydes
        {
            get { return _Refundbydes; }
            set { _Refundbydes = value; }
        }

        public string Chqno
        {
            get { return _Chqno; }
            set { _Chqno = value; }
        }
        public string Chqdate
        {
            get { return _Chqdate; }
            set { _Chqdate = value; }
        }
        public string Refamount
        {
            get { return _Refamount; }
            set { _Refamount = value; }
        }

        public TourAdvance()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        /// CONSTRUCTOR FOR TOUR PREVIOUS DETAIL 
        /// </summary>
        /// <param name="Tourdate"></param>
        /// <param name="Advance"></param>
        /// <param name="Exp"></param>
        /// <param name="Balance"></param>
        /// <param name="Subdate"></param>
        /// <param name="Refundby"></param>
        /// <param name="Chqno"></param>
        /// <param name="Chqdate"></param>
        /// <param name="Refamount"></param>

        public TourAdvance(string Tourperiod, string Advance, string Exp,
                        string Balance, string Subdate,
                        string Refundby, string Chqno,
                        string Chqdate, string Refamount)
        {
            _Tourperiod = Tourperiod;
            _Advance = Advance;
            _Exp = Exp;
            _Balance = Balance;
            _Subdate = Subdate;
            _Refundby = Refundby;
            _Chqno = Chqno;
            _Chqdate = Chqdate;
            _Refamount = Refamount;

            if (_Refundby == "1")
                _Refundbydes = "Cheque";
            else
                if (_Refundby == "2")
                _Refundbydes = "Cash";
            else
                _Refundbydes = "";


        }



    }
}

