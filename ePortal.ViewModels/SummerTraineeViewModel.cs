using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class SummerTraineeViewModel
    {
       public long ID { get; set; }
       public long OPID { get; set; }
       public string OperationName { get; set; }
       

    }

    public class AddUserRequest
    {
        public long Obj { get; set; }
    }
}
