using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public class AddMenuRequest
    {

        public string MenuID { get; set; }

        public string MenuType { get; set; }
        public string MenuName { get; set; }
        public string MenuTooltip { get; set; }
        public string PageUrl { get; set; }
        public string MenuLevel { get; set; }
        public string Menudisporder { get; set; }
        public string Menudisplay { get; set; }
        public string MenuTargets { get; set; }
        public string Status { get; set; }
        public string DefaultMenu { get; set; }
        public string MenuIcon { get; set; }
        public string IconContentType { get; set; }
        public string IconFileName { get; set; }
        public string Internet { get; set; }
        public string PerentMenu { get; set; }
    }
}
