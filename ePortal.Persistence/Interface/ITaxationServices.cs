using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
    public interface ITaxationServices
    {
        public int AddTAwareness(string taxationid, string series, string attachment, string status, int AddedBy, string strreleasedate);
        public DataSet GetTAwarenessDetails();
        public int AddTCirculars(string taxationid, string series, string attachment, string status, int AddedBy, string strreleasedate);
        public DataSet GetTCircularsDetails();
        public int AddTForms(string taxationid, string series, string attachment, string status, int AddedBy, string strreleasedate);
        public DataSet GetTFormsDetails();

    }
}
