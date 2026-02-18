using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class AutoRemindersNoofDaysMaster
    {
        [Key]
        public int SRNO { get; set; }

        public string REMINDER_TYPE { get; set; }

        [Required(ErrorMessage = "Please Enter No of Days")]
        public int NO_OF_DAYS { get; set; }

        //public string CREATED_BY { get; set; }

        //public DateTime? CREATED_DATE { get; set; }

        //public string UPDATED_BY { get; set; }

        //public DateTime? UPDATED_DATE { get; set; }
    }
}
