using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;

using System.Text.Json;
using ePortal.Shared.Services;
using ePortal.DomainClasses;
using ePortal.WebUI.Filters;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class MenuProcessRoleController : Controller
    {
        //private System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        private string sJSON = String.Empty;

        private readonly IMenuMappingMasterService _IMenuMappingMaster;
        private readonly IMenuMasterServices _IMenuMaster;
        private readonly IProcessMasterService _IProcessMaster;
        private readonly IRoleMappingMasterServices _IRoleMappingMaster;
        private readonly IRoleMasterServices _IRoleMaster;
        private readonly IRoleUserMappingMasterService _IRoleUserMappingMaster;

        private readonly ISessionService _SessionService;

        // GET: MenuMappingMaster

        public MenuProcessRoleController(
            IMenuMappingMasterService objIMenuMappingMaster,
            IMenuMasterServices objIMenuMaster,
            IProcessMasterService objIProcessMaster,
            IRoleMappingMasterServices objIRoleMappingMaster,
            IRoleMasterServices objRoleMaster,
            IRoleUserMappingMasterService objRoleUserMappingMaster,

            ISessionService objsessionService
            )

        {
            _IMenuMappingMaster = objIMenuMappingMaster;
            _IMenuMaster = objIMenuMaster;
            _IProcessMaster = objIProcessMaster;
            _IRoleMappingMaster = objIRoleMappingMaster;
            _IRoleMaster = objRoleMaster;
            _IRoleUserMappingMaster = objRoleUserMappingMaster;

            _SessionService = objsessionService;
        }

        #region  MENU MAPPING MASTER

        // GET: MenuMappingMaster
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MenuMappingList()
        {
            return View();
        }
        public IActionResult AddMapping()
        {
            ViewBag.DropDownList = _IMenuMappingMaster.GetADMENUPARAM_MSTLIst();
            return View();
        }

        [HttpPost]
        public IActionResult AddMapping(string MenuId, string Operator, string Status, string Id, string MenuParamId)
        {
            short retVal = -1;
            try
            {
                string? UserID = _SessionService.Get<string>("userID");
                var Model_ = new MenuMappingViewModel()
                {
                    MenuId = Convert.ToInt64(MenuId),
                    Operator = Operator,
                    Status = Status,
                    ParamVal = Id,
                    MenuparamId = Convert.ToInt16(MenuParamId)
                };

                retVal = _IMenuMappingMaster.AddMenuMappingData(Model_, UserID);
                //return Json(new { res = retVal, error = "" }, JsonRequestBehavior.AllowGet);

                return new JsonResult(new { res = retVal, error = "" });

            }
            catch (Exception ex)
            {

                //return Json(new { res = retVal, error = ex.Message }, JsonRequestBehavior.AllowGet);

                return new JsonResult(new { res = retVal, error = "" });

            }

        }

        public IActionResult EditMapping(int id)
        {
            MenuMappingViewModel menumaster = _IMenuMappingMaster.GetMappingList().Where(x => x.MappingId == id).FirstOrDefault();
            ViewBag.DropDownList = _IMenuMappingMaster.GetADMENUPARAM_MSTLIst();
            menumaster.paramlist = _IMenuMappingMaster.GetMappingDataforGrid(Convert.ToInt64(id)).ToList();
            return View(menumaster);
        }

        [HttpPost]
        public IActionResult EditMapping(string MapId, string MenuId, string Operator, string Status, string Id, string MenuParamId)
        {
            short retVal = -1;
            try
            {
                string UserID = _SessionService.Get<string>("userID");
                var Model_ = new MenuMappingViewModel()
                {
                    MenuId = Convert.ToInt64(MenuId),
                    Operator = Operator,
                    Status = Status,
                    ParamVal = Id,
                    MenuparamId = Convert.ToInt16(MenuParamId)
                };

                retVal = _IMenuMappingMaster.EditMenuMappingData(MapId, Model_, UserID);
                //return Json(new { res = retVal, error = "" }, JsonRequestBehavior.AllowGet);

                return new JsonResult(new { res = retVal, error = "" });

            }
            catch (Exception ex)
            {

                //return Json(new { res = retVal, error = ex.Message }, JsonRequestBehavior.AllowGet);

                return new JsonResult(new { res = retVal, error = ex.Message });

            }
        }

        [HttpGet]
        public JsonResult GetMenuMappingList()
        {
            var ilist = _IMenuMappingMaster.GetMappingList();
            //sJSON = oSerializer.Serialize(ilist);
            string sJSON = JsonSerializer.Serialize(ilist);
            return new JsonResult(sJSON);
        }

        //public ActionResult AutocompleteMenuName(string term)
        //{

        //    var suggestions = repo.GETEmployee(term);
        //    return Json(suggestions, JsonRequestBehavior.AllowGet);
        //}


        public IActionResult GetSuggesionValue(string term, int ParamType)
        {

            var suggestions = _IMenuMappingMaster.GetSuggesionValue(term, ParamType);
            return new JsonResult(suggestions);
        }

        public IActionResult AutocompleteMenuName(string term)
        {

            var suggestions = _IMenuMappingMaster.GetMenuName(term);
            return new JsonResult(suggestions);
        }



        #endregion

        #region  MENU MASTER

        public ActionResult MenuList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddMenu()
        {
            ViewBag.ParentMenu = _IMenuMaster.GetMenuList("ddl").ToList();
            MenuViewModel _model = new MenuViewModel();
            return View();
        }

        [HttpPost]
        public ActionResult AddMenu([FromBody] AddMenuRequest request)
        {
            short retVal = -1;
            try
            {
                byte[] Icon = !string.IsNullOrEmpty(request.MenuIcon) ? Convert.FromBase64String(request.MenuIcon) : null;
                string UserID = _SessionService.Get<string>("userID");

                var model = new MenuViewModel()
                {
                    MenuType = request.MenuType,
                    Title = request.MenuName,
                    ToolTip = request.MenuTooltip,
                    URL = request.PageUrl,
                    MenuLevel = Convert.ToInt16(request.MenuLevel),
                    MenuOrder = Convert.ToInt16(request.Menudisporder),
                    MenuDisaplay = request.Menudisplay,
                    MenuTarget = request.MenuTargets,
                    Status = request.Status,
                    IsDefaultMenu = Convert.ToInt16(request.DefaultMenu),
                    MenuIcon = Icon,
                    MenuContentType = request.IconContentType,
                    MenuIconFileName = request.IconFileName,
                    Internet = Convert.ToInt16(request.Internet),
                    MenuParentId = string.IsNullOrEmpty(request.PerentMenu) ? 0 : (int)Convert.ToInt64(request.PerentMenu),
                };

                retVal = _IMenuMaster.AddMenuData(model, UserID);
                return Json(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { res = retVal, error = ex.Message });
            }
        }


        /*[HttpPost]
        public ActionResult AddMenu(string MenuType, string MenuName, string MenuTooltip, string PageUrl, string MenuLevel, string Menudisporder, string Menudisplay, string MenuTargets, string Status, string DefaultMenu, string MenuIcon, string IconContentType, string IconFileName, string Internet, string PerentMenu)
        {
            short retVal = -1;
            //return Json(new { res = retVal, error = "" }, JsonRequestBehavior.AllowGet);
            try
            {
                //byte[] Icon = (MenuIcon != "") ? Convert.FromBase64String(MenuIcon) : null;
                byte[] Icon = !string.IsNullOrEmpty(MenuIcon) ? Convert.FromBase64String(MenuIcon) : null;
                string UserID = _SessionService.Get<string>("userID");

                if (PerentMenu == "")
                {
                    PerentMenu = null;
                }

                var Model_ = new MenuViewModel()
                {
                    MenuType = MenuType,
                    Title = MenuName,
                    ToolTip = MenuTooltip,
                    URL = PageUrl,
                    MenuLevel = Convert.ToInt16(MenuLevel),
                    MenuOrder = Convert.ToInt16(Menudisporder),
                    MenuDisaplay = Menudisplay,
                    MenuTarget = MenuTargets,
                    Status = Status,
                    IsDefaultMenu = Convert.ToInt16(DefaultMenu),
                    MenuIcon = Icon,
                    MenuContentType = IconContentType,
                    MenuIconFileName = IconFileName,
                    Internet = Convert.ToInt16(Internet),
                    //MenuParentId = Convert.ToInt64(PerentMenu),

                    MenuParentId = string.IsNullOrEmpty(PerentMenu) ? 0 : (int)Convert.ToInt64(PerentMenu),


                };

                retVal = _IMenuMaster.AddMenuData(Model_, UserID);
                return new JsonResult(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { res = retVal, error = ex.Message });
            }

        }*/
        [HttpGet]
        public async Task<ActionResult> EditMenu(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("MenuList");
            }
            MenuViewModel menumaster = _IMenuMaster.GetMenuData().Where(x => x.MenuId == id).FirstOrDefault();
            var pid = menumaster.MenuParentId;
            menumaster.ParentMenu_text = _IMenuMaster.GetParentName(pid).Select(s => s.ParentMenu_text).FirstOrDefault();
            ViewBag.ParentMenu = _IMenuMaster.GetMenuList("ddl").ToList();
            return View(menumaster);
        }

        [HttpPost]
        public ActionResult EditMenu([FromBody] AddMenuRequest request)
        {
            short retVal = -1;
            try
            {
                byte[] Icon = !string.IsNullOrEmpty(request.MenuIcon) ? Convert.FromBase64String(request.MenuIcon) : null;
                string? UserID = _SessionService.Get<string>("userID");

                var model = new MenuViewModel()
                {
                    MenuId = Convert.ToInt16(request.MenuID),
                    MenuType = request.MenuType,
                    Title = request.MenuName,
                    ToolTip = request.MenuTooltip,
                    URL = request.PageUrl,
                    MenuLevel = Convert.ToInt16(request.MenuLevel),
                    MenuOrder = Convert.ToInt16(request.Menudisporder),
                    MenuDisaplay = request.Menudisplay,
                    MenuTarget = request.MenuTargets,
                    Status = request.Status,
                    IsDefaultMenu = Convert.ToInt16(request.DefaultMenu),
                    MenuIcon = Icon,
                    MenuContentType = request.IconContentType,
                    MenuIconFileName = request.IconFileName,
                    Internet = Convert.ToInt16(request.Internet),
                    MenuParentId = string.IsNullOrEmpty(request.PerentMenu) ? 0 : Convert.ToInt64(request.PerentMenu)
                };

                retVal = _IMenuMaster.EditMenuData(request.MenuID, model, UserID);

                return Json(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { res = retVal, error = ex.Message });
            }
        }


        /*[HttpPost]
        public ActionResult EditMenu(string MenuID, string MenuType, string MenuName, string MenuTooltip, string PageUrl, string MenuLevel, string Menudisporder, string Menudisplay, string MenuTargets, string Status, string DefaultMenu, string MenuIcon, string IconContentType, string IconFileName, string Internet, string PerentMenu)
        {
            short retVal = -1;
            try
            {
                byte[] Icon = (MenuIcon != "") ? Convert.FromBase64String(MenuIcon) : null;
                string? UserID = _SessionService.Get<string>("userID");

                if (PerentMenu == "" || PerentMenu == "null")
                {
                    PerentMenu = null;
                }


                var Model_ = new MenuViewModel()
                {
                    MenuType = MenuType,
                    Title = MenuName,
                    ToolTip = MenuTooltip,
                    URL = PageUrl,
                    MenuLevel = Convert.ToInt16(MenuLevel),
                    MenuOrder = Convert.ToInt16(Menudisporder),
                    MenuDisaplay = Menudisplay,
                    MenuTarget = MenuTargets,
                    Status = Status,
                    IsDefaultMenu = Convert.ToInt16(DefaultMenu),
                    MenuIcon = Icon,
                    MenuContentType = IconContentType,
                    MenuIconFileName = IconFileName,
                    Internet = Convert.ToInt16(Internet),
                    MenuParentId = Convert.ToInt64(PerentMenu)

                };

                retVal = _IMenuMaster.EditMenuData(MenuID, Model_, UserID);

                return new JsonResult(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { res = retVal, error = ex.Message });
            }

        }*/

        [HttpGet]
        public JsonResult GetMenuList()
        {
            // List<ADMENU_MST> ilist = new List<ADMENU_MST>();
            var ilist = _IMenuMaster.GetMenuList("grid");
            string sJSON = JsonSerializer.Serialize(ilist);
            return new JsonResult(sJSON);
        }


        [HttpGet]
        public JsonResult ParentMenu(decimal MenuType, decimal PerentMenu, decimal MenuLevel)
        {

            int LoginCode = int.Parse(_SessionService.Get<string>("userID"));
            int Max = _IMenuMaster.ParentMenu(MenuType, PerentMenu, MenuLevel, LoginCode);
            string sJSON = JsonSerializer.Serialize(Max);
            return new JsonResult(sJSON);

        }

        #endregion

        #region PROCESS MASTER

        public ActionResult ProcessList()
        {

            return View();
        }

        [HttpGet]
        public JsonResult GetProcessList()
        {
            var ilist = _IProcessMaster.GetMenuList();
            sJSON = JsonSerializer.Serialize(ilist);
            return new JsonResult(sJSON);
        }

        public ActionResult AddProcess()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProcess(string ProcessName, string status)
        {
            short retVal = -1;
            try
            {
                string? UserID = _SessionService.Get<string>("userID");
                retVal = _IProcessMaster.AddProcess(ProcessName, status, UserID);
                return new JsonResult(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { res = retVal, error = ex.Message });
            }
        }

        public ActionResult EditProcess(int PROCESS_ID)
        {

            Decimal PROCESS_ID1 = decimal.Parse(PROCESS_ID.ToString());
            ProcessMasterViewModel ProcessMst = _IProcessMaster.GetMenuList().Where(x => x.PROCESS_ID == PROCESS_ID1).FirstOrDefault();
            return View(ProcessMst);
        }

        [HttpPost]
        public ActionResult EditProcess(string ProcessID, string ProcessName, string status)
        {
            short retVal = -1;
            try
            {

                string? UserID = _SessionService.Get<string>("userID");
                retVal = _IProcessMaster.EditProcess(ProcessID, ProcessName, status, UserID);
                return new JsonResult(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { res = retVal, error = ex.Message });
            }
        }

        #endregion

        #region ROLE MAPPING MASTER

        public ActionResult IndexRoleMapping()
        {
            return View();
        }

        public ActionResult RoleMappingList()

        {
            return View();
        }

        public ActionResult AutocompleteMenuID(string term)
        {
            var suggestions = _IRoleMappingMaster.GetMenuId(term);
            return new JsonResult(suggestions);
        }

        //public JsonResult AutocompleteMenuID(string term)
        //{
        //    var suggestions = _IRoleMappingMaster.GetMenuId(term);

        //    var formattedSuggestions = ((IEnumerable<dynamic>)suggestions).Select(s => new
        //    {
        //        label = s.name,
        //        value = s.mid
        //    });

        //    return Json(formattedSuggestions);
        //}



        //[HttpGet]
        //public IActionResult AutocompleteMenuID(string term)
        //{
        //    var suggestions = _IRoleMappingMaster.GetMenuId(term);

        //    // Cast dynamic to IEnumerable<dynamic> to use LINQ
        //    var result = ((IEnumerable<dynamic>)suggestions).Select(s => new
        //    {
        //        mid = s.mid,
        //        name = s.name
        //    });

        //    return Ok(result);
        //}



        [HttpGet]
        public JsonResult GetRoleMappingList()
        {
            // List<ADMENU_MST> ilist = new List<ADMENU_MST>();
            var ilist = _IRoleMappingMaster.GetRoleMappingList();
            sJSON = JsonSerializer.Serialize(ilist);
            return new JsonResult(sJSON);
        }

        public ActionResult AddRoleMapping()
        {
            ViewBag.Role_Name = _IRoleMappingMaster.GetRoleList().ToList();
            RoleMappingViewModel _model = new RoleMappingViewModel();
            return View();
        }

        [HttpPost]
        public ActionResult AddRoleMapping(string MenuName, string RoleName, string Status)
        {
            short retVal = -1;
            try
            {

                string? UserID = _SessionService.Get<string>("userID");
                var Model_ = new RoleMappingViewModel()
                {
                    MenuId = Convert.ToInt64(MenuName),
                    RoleId = Convert.ToInt64(RoleName),
                    Status = Status
                };

                retVal = _IRoleMappingMaster.AddRoleMappingData(Model_, UserID);
                return new JsonResult(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { res = retVal, error = ex.Message });
            }
        }

        public ActionResult EditRoleMapping(int id)
        {
            RoleMappingViewModel RoleMappingmaster = _IRoleMappingMaster.GetRoleMappingList().Where(x => x.MappingId == id).FirstOrDefault();
            ViewBag.Role_Name = _IRoleMappingMaster.GetRoleList().ToList();
            return View(RoleMappingmaster);
        }

        [HttpPost]
        public ActionResult EditRoleMapping(string MappingId, string MenuName, string RoleName, string Status)
        {
            short retVal = -1;
            try
            {

                string userID = _SessionService.Get<string>("userID");
                var Model_ = new RoleMappingViewModel()
                {
                    MenuId = Convert.ToInt64(MenuName),
                    RoleId = Convert.ToInt64(RoleName),
                    Status = Status
                };

                retVal = _IRoleMappingMaster.EditRoleMappingData(MappingId, Model_, userID);
                return new JsonResult(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { res = retVal, error = ex.Message });
            }

        }

        public JsonResult AutocomplitName(long menuid)
        {
            var data = _IRoleMappingMaster.AutocomplitName(menuid);
            sJSON = JsonSerializer.Serialize(data);
            return new JsonResult(sJSON);
        }

        #endregion

        #region ROLE MASTER

        public ActionResult IndexRoleMaster()
        {
            return View();
        }
        public ActionResult RoleList()
        {
            return View();
        }

        public ActionResult AddRole()
        {
            ViewBag.process_name = _IRoleMaster.GetProNameList().ToList();
            RoleViewModel _model = new RoleViewModel();
            return View();
        }

        [HttpGet]
        public JsonResult GetRoleList()
        {
            var ilist = _IRoleMaster.GetRoleList();
            sJSON = JsonSerializer.Serialize(ilist);
            return new JsonResult(sJSON);
        }

        public ActionResult AutocompleteEmpID(string term)
        {
            var suggestions = _IRoleMaster.GetEmpID(term);
            return new JsonResult(suggestions);
        }

        [HttpPost]
        public ActionResult AddRole(string RoleName, string ProcessName, string RoleBPO, string Status)
        {
            short retVal = -1;
            try
            {

                string? UserID = _SessionService.Get<string>("userID");
                var Model_ = new RoleViewModel()
                {
                    RoleName = RoleName,
                    ProcessId = Convert.ToInt64(ProcessName),
                    RoleBPO = Convert.ToInt64(RoleBPO),
                    Status = Status
                };

                retVal = _IRoleMaster.AddRoleData(Model_, UserID);
                return new JsonResult(new { res = retVal, error = "" });

            }
            catch (Exception ex)
            {

                return new JsonResult(new { res = retVal, error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult EditRole(int id)
        {
            RoleViewModel menumaster = _IRoleMaster.GetRoleList().Where(x => x.RoleId == id).FirstOrDefault();
            ViewBag.process_name = _IRoleMaster.GetProNameList().ToList();
            return View(menumaster);
        }

        [HttpPost]
        public ActionResult EditRole(string RoleId, string RoleName, string ProcessName, string RoleBPO, string Status)
        {
            short retVal = -1;
            try
            {

                string? UserID = _SessionService.Get<string>("userID");
                var Model_ = new RoleViewModel()
                {
                    RoleName = RoleName,
                    ProcessId = Convert.ToInt64(ProcessName),
                    RoleBPO = Convert.ToInt64(RoleBPO),
                    Status = Status
                };

                retVal = _IRoleMaster.EditRoleData(RoleId, Model_, UserID);
                return new JsonResult(new { res = retVal, error = "" });

            }
            catch (Exception ex)
            {

                return new JsonResult(new { res = retVal, error = ex.Message });
            }
        }

        public JsonResult AutocomplitNameRoleMaster(long Ecode)
        {
            var data = _IRoleMaster.AutocomplitName(Ecode);
            sJSON = JsonSerializer.Serialize(data);
            return new JsonResult(sJSON);
        }

        #endregion

        #region ROLE USER MAPPING MASTER

        public ActionResult IndexRoleUserMapping()
        {
            return View();
        }


        public ActionResult RoleUserMappingList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetRoleUserMappingList()
        {
            var ilist = _IRoleUserMappingMaster.GetRoleUserMppingList();
            sJSON = JsonSerializer.Serialize(ilist);
            return new JsonResult(sJSON);
        }

        public ActionResult AddRoleUserMapping()
        {
            ViewBag.Role_Name = _IRoleUserMappingMaster.GetRoleNameList().ToList();
            RoleUserMappingViewModel _model = new RoleUserMappingViewModel();
            return View();
        }
        [HttpPost]
        public ActionResult AddRoleUserMapping(string RoleName, string UserName, string Status)
        {
            short retVal = -1;
            try
            {
                string? UserID = _SessionService.Get<string>("userID");
                var Model_ = new RoleUserMappingViewModel()
                {
                    RoleId = Convert.ToInt64(RoleName),
                    UserId = Convert.ToInt64(UserName),
                    Status = Status
                };

                retVal = _IRoleUserMappingMaster.AddRoleUserMappingData(Model_, UserID);
                return Json(new { res = retVal, error = "" });

            }
            catch (Exception ex)
            {

                return new JsonResult(new { res = retVal, error = ex.Message });
            }
        }

        public ActionResult EditRoleUserMapping(int id)
        {
            RoleUserMappingViewModel menumaster = _IRoleUserMappingMaster.GetRoleUserMppingList().Where(x => x.MappingId == id).FirstOrDefault();
            ViewBag.Role_Name = _IRoleUserMappingMaster.GetRoleNameList();
            return View(menumaster);
        }

        [HttpPost]
        public ActionResult EditRoleUserMapping(string MappingId, string RoleName, string UserName, string Status)
        {
            short retVal = -1;
            try
            {
                string? UserID = _SessionService.Get<string>("userID");
                var Model_ = new RoleUserMappingViewModel()
                {
                    MappingId = Convert.ToInt64(MappingId),
                    RoleId = Convert.ToInt64(RoleName),
                    UserId = Convert.ToInt64(UserName),
                    Status = Status
                };

                retVal = _IRoleUserMappingMaster.EditRoleUserMappingData(MappingId, Model_, UserID);
                return new JsonResult(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { res = retVal, error = ex.Message });
            }
        }

        #endregion

    }
}
