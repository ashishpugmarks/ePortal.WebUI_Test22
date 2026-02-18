using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;

namespace ePortal.Persistence.TravelBilling.Interface
{
    public interface IcTravelBilling
    {
        public DataTable ChequeRequestApprovalList(long UserId);
    }
}
