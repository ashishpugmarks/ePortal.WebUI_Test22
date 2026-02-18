using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.HCG
{
    public class HcgListViewModel
    {

        public IReadOnlyList<HcgItem> HcgItems { get; set; } = new List<HcgItem>();
        public int Page { get; set; }
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }


    public class HcgItem
    {
        public int HcgType { get; set; }          // maps to HCGTYPE
        public string Series { get; set; }        // maps to HCGSERIES
        public string ReleaseDate { get; set; }// maps to RELEASEDATE
        public string Attachment { get; set; }    // maps to ATTECHMENT
    }

}
