using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    class OperatingHeadViewModel
    {
    }
    public class OperatingHeadSYKIViewModel
    {
        public List<OperatingHeadSYKIList> _SYKIList { get; set; }
        public OperatingHeadSYKIViewModel()
        {
            _SYKIList = new List<OperatingHeadSYKIList>();
        }
    }
    public class OperatingHeadSearchModel
    {
        public long? ISSCMemberNominationID { get; set; }
        public long ADEMPCODE { get; set; }
        public string SYKI { get; set; }
        public decimal SYKIID { get; set; }
        public long? OPERATIONID { get; set; }
        public long? DIVHDHDID { get; set; }
        public long ADDEDBY { get; set; }

    }

    public class OperatingHeadAddNominationVM
    {
        public List<long?> NominationIDs { get; set; }
        public decimal SYKIID { get; set; }
        public long OperationID { get; set; }
        public List<long> DIVISIONIDs { get; set; }
        public List<long> ISSCMEMBERs { get; set; }
        public int Status { get; set; }
        public int SaveStatus { get; set; }
        public string SaveMsg { get; set; }
        public long ADDEDBY { get; set; }

    }
    public class OperatingHeadAddNomination1VM
    {
        public String SYKIID { get; set; }
        public String OperationID { get; set; }
        //public List<long> DIVISIONID { get; set; }
        //public List<long> ISSCMEMBER { get; set; }
        //public int Status { get; set; }
        //public int SaveStatus { get; set; }
        //public string SaveMsg { get; set; }
    }
    public class OperatingHeadSearchModel1
    {
     //   public string PERIODSETTINGID { get; set; }
        public long? DIVISIONID { get; set; }
        public string DIVISION { get; set; }
        public List<empName> ISSCMEMBER { get; set; }
        public string OLDISSCMEMBER { get; set; }

        public long? Current_Ki_Empcode { get; set; }

    }
    public class empName
    {
        public long empcode { get; set; }
        public string Empname { get; set; }

    }

    public class OperatingHead
    {
        public string Emailid { get; set; }
        public string Empname { get; set; }

    }
    public class OperatingHeadSYKIList
    {
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public decimal ACTIVE { get; set; }
    }


   
}
