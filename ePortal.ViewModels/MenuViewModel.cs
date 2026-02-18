using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class MenuViewModel
    {
        public Int64 MenuId { get; set; }
        public Int64 MenuParentId { get; set; }
        public int MenuLevel { get; set; }
        public string Title { get; set; }
        public string ToolTip { get; set; }
        public string URL { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public bool isMenuAccessibleToUser { get; set; }
        public string MenuTarget { get; set; }
        public List<MenuViewModel> SubMenuItems { get; set; }
        public byte[] MenuIcon { get; set; }
        public string MenuContentType { get; set; }
        public int MenuOrder { get; set; }
        //------------------START-[Code Added by Aumento on 14-Jan-2024]------------------------------
        public string Status { get; set; }
        public string MenuType { get; set; }
        public int IsDefaultMenu { get; set; }
        public string MenuIconFileName { get; set; }
        public int Internet { get; set; }
        public string MenuDisaplay { get; set; }

        public string ModifiedBy { get; set; }
        public string ParentMenu_text { get; set; }
        public DateTime Modified_Date { get; set; }
        //------------------END-[Code Added by Aumento on 14-Jan-2024]--------------------------------

    }

    public class ParentMenuObject
    {
        public string UserId { get; set; } //check datatype
        public List<MenuViewModel> menuObject { get; set; }
    }
    public class ExtPortalViewModel
    {
        public long EXTPORTALID { get; set; }
        public string PORTALNAME { get; set; }
        public string PORTAL_DESC { get; set; }
        public string PORTAL_URL { get; set; }
        public byte[] PORTAL_ICON { get; set; }
        public string PORTALICON_CONTENT_TYPE { get; set; }
        public string PORTALICON_FILENAME { get; set; }
        public short STATUS { get; set; }
        public long CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<long> DISPLAY_ORDER { get; set; }
    }
}
