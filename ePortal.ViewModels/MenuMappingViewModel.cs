using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class MenuMappingViewModel
    {
        public long MappingId { get; set; }
        public long MenuId { get; set; }
        public string MenuName { get; set; }
        public string ParamVal { get; set; }
        public string Operator { get; set; }
        public string Status { get; set; }
        public long Created_By { get; set; }
        public System.DateTime Created_Date { get; set; }
        public Nullable<long> Modified_By { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
        public int MenuparamId { get; set; }
        public string MenuParamName { get; set; }
        public List<MenuParamdata> paramlist { get; set; }

        public string Discription { get; set; }
    }

    public class MenuParamdata
    {
        public long value { get; set; }
        public string text { get; set; }
    }
}
