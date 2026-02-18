using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class SISMenuViewModel
    {
        public double Proc_id { get; set; }
        public string decription { get; set; }

        public string Document_type { get; set; }

        public Int32 Parent_ID { get; set; }

        public Int32 Active { get; set; }

        public DateTime? Dateadded { get; set; }
        public DateTime? Datelstmode { get; set; }

        public Int32 Addedby { get; set; }
        public Int32? Modifiedby { get; set; }

        public string doccode { get; set; }

        public string Filename { get; set; }
        public string Menu_url { get; set; }

        public Int32? MenuLevel { get; set; }

        public Int32? Menu_Target { get; set; }

    }
    public class ISMSMENUVM
    {
        public Int64 ISMSID { get; set; }
        public string ISMS_DES { get; set; }
        public string MENU_TOOLTIP { get; set; }
        public string FILE_NAME { get; set; }
        public Int64 MENU_PARENT_ID { get; set; }
        public string Active { get; set; }
        public Int32 DISPLAY { get; set; }
        public DateTime? ADDED_DATE { get; set; }
        public DateTime? MODIFIDATE { get; set; }
        public Int32 Addedby { get; set; }
        public Int32? MODIFIBY { get; set; }
        public string doccode { get; set; }
        public string Filename { get; set; }
        public string MENU_URL { get; set; }
        public string MENU_TARGET { get; set; }
        public string MENUICON_CONTENT_TYPE { get; set; }
        public string MENUICON_FILENAME { get; set; }
        public Int32 MENU_DISPLAY_ORDER { get; set; }
        public Int32 INTERNET { get; set; }
        public string MENU_CONTROLLER { get; set; }
        public Int32 MENU_DISPLAY { get; set; }
        public Int32 ISDEFAULTMENU { get; set; }
        public int MENU_LEVEL { get; set; }
        public string status { get; set; }
        public string DISPLAYstatus { get; set; }
        public Int32 LVL { get; set; }
        public List<ISMSMENUVM> SubMenuItems { get; set; }

    }

    public class ISMSSUBMENUVM
    {
        public double Proc_id { get; set; }
        public string decription { get; set; }
        public string Document_type { get; set; }
        public Int32 Parent_ID { get; set; }
        public Int32 Active { get; set; }
        public DateTime? Dateadded { get; set; }
        public DateTime? Datelstmode { get; set; }
        public Int32 Addedby { get; set; }
        public Int32? Modifiedby { get; set; }
        public string doccode { get; set; }
        public string Filename { get; set; }
        public string Menu_url { get; set; }
        public Int32? MenuLevel { get; set; }
        public Int32? Menu_Target { get; set; }
    }
    /*Menu Class*/
    public class SISMenuVM
    {
        public double Proc_id { get; set; }
        public string decription { get; set; }
    }
    public class ISMSANNOUNANCEVM
    {
        public double ANNOUCE_ID { get; set; }
        public string DESCRIPTION { get; set; }

        public string DETAILS { get; set; }

        public Int32 Parent_ID { get; set; }

        public Int32 ACTIVE { get; set; }

        public DateTime? Dateadded { get; set; }
        public DateTime? Datelstmode { get; set; }

        public Int32 Addedby { get; set; }
        public Int32? Modifiedby { get; set; }

        public long ISMSID { get; set; }
        public string MENU_URL { get; set; }

        public string YEARS { get; set; }

        public string MONTHS { get; set; }
        public string MODIFIDATE { get; set; }
        public byte[] BANNER { get; set; }
        public string BANNER_CONTENTTYPE { get; set; }
        public string BANNER_NAME { get; set; }
        public List<ISMSANNOUNANCEVM> ISMSANNOUNANCEVMLISTItems { get; set; }

    }

    public class MenuISMSViewModel
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
        public List<MenuISMSViewModel> SubMenuItems { get; set; }
        public byte[] MenuIcon { get; set; }
        public string MenuContentType { get; set; }

    }
    public class MenuISMSVM
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
        public List<MenuISMSVM> ISMSSubMenuItems { get; set; }
        public byte[] MenuIcon { get; set; }
        public string MenuContentType { get; set; }
        public string DocumentType { get; set; }
        public int SubMenuLevel { get; set; }
        //public List<ISMSInFoSubMenuVM> ISMSSubInfoMenuItems { get; set; }
    }
    public class SIS_ISMS
    {
        public Int64 ISMSID { get; set; }
        public string ISMS_DES { get; set; }
        public string MENU_TOOLTIP { get; set; }
        public string FILE_NAME { get; set; }
        public Int64 MENU_PARENT_ID { get; set; }
        public string Active { get; set; }
        public Int32 DISPLAY { get; set; }
        public string doccode { get; set; }
        public string Filename { get; set; }
        public string MENU_URL { get; set; }
        public string MENU_TARGET { get; set; }
        public string MENUICON_CONTENT_TYPE { get; set; }
        public string MENUICON_FILENAME { get; set; }
        public Int32 MENU_DISPLAY_ORDER { get; set; }
        public Int32 INTERNET { get; set; }
        public string MENU_CONTROLLER { get; set; }
        public Int32 MENU_DISPLAY { get; set; }
        public Int32 ISDEFAULTMENU { get; set; }
        public int MENU_LEVEL { get; set; }
        public string status { get; set; }
    }
    public class ISMS_Info_Security
    {
        public Int64 Proc_id { get; set; }
        public string decription { get; set; }
        public string Document_type { get; set; }
        public Int64 Parent_ID { get; set; }
        public string Active { get; set; }
        public Int32 Addedby { get; set; }
        public Int32? Modifiedby { get; set; }
        public string doccode { get; set; }
        public string Filename { get; set; }
        public string MENU_URL { get; set; }
        public string MENU_TARGET { get; set; }
        public int MENU_LEVEL { get; set; }
    }
}
