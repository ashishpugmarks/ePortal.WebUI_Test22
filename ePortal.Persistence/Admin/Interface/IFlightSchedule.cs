using System.Data;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IFlightSchedule
    {
        public int AddFlight(string fsid, string flightname, string attachment, string status, int AddedBy, string strServiceType);
        public DataSet GetFlightDetails();
    }
}

