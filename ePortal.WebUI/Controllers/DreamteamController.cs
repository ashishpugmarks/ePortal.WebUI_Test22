using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class DreamteamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult DTVol34()
        {
            return View();
        }

        public IActionResult DTVol35()
        {
            return View();
        }

        public IActionResult DTVol36()
        {
            return View();
        }

        public IActionResult DTVol38()
        {
            return View();
        }

        public IActionResult DTVol40()
        {
            return View();
        }

        public IActionResult DTVol44()
        {
            return View();
        }

        public IActionResult DTVol45()
        {
            return View();
        }

        public IActionResult DTVol47()
        {
            return View();
        }
        public IActionResult DTVol48()
        {
            return View();
        }
        public IActionResult DTVol49()
        {
            return View();
        }
        public IActionResult DTVol50()
        {
            return View();
        }
        public IActionResult DTVol51()
        {
            return View();
        }
        public IActionResult DTVol52()
        {
            return View();
        }
        public IActionResult DTVol53()
        {
            return View();
        }
        public IActionResult DTVol54()
        {
            return View();
        }
        public IActionResult DTVol55()
        {
            return View();
        }
        public IActionResult DTVol56()
        {
            return View();
        }
        

        public IActionResult DTVol57()
        {
            return View();
        }

        public IActionResult DTVol58()
        {
            return View();
        }

        public IActionResult DTVol59()
        {
            return View();
        }

        public IActionResult DTVol60()
        {
            return View();
        }

        public IActionResult DTVol61()
        {
            return View();
        }

        public IActionResult DTVol62()
        {
            return View();
        }

        public IActionResult DTVol63()
        {
            return View();
        }

        public IActionResult DTVol64()
        {
            return View();
        }

        public IActionResult DTVol65()
        {
            return View();
        }


        public IActionResult DTVol66()
        {
            return View();
        }

        public IActionResult DTVol67()
        {
            return View();
        }

        public IActionResult DTVol68()
        {
            return View();
        }

        public IActionResult DTVol69()
        {
            return View();
        }

        public IActionResult DTVol70()
        {
            return View();
        }

        public IActionResult DTVol82()
        {
            return View();
        }

        public IActionResult DTVol72()
        {
            return View();
        }

        public IActionResult DTVol73()
        {
            return View();
        }

        public IActionResult DTVol74()
        {
            return View();
        }

        public IActionResult DTVol75()
        {
            return View();
        }

        public IActionResult DTVol76()
        {
            return View();
        }

        public IActionResult DTVol77()
        {
            return View();
        }

        public IActionResult DTVol78()
        {
            return View();
        }

        public IActionResult DTVol79()
        {
            return View();
        }

        public IActionResult DTVol80()
        {
            return View();
        }
            
        public IActionResult DreamTeamArchive()
        {
            return View();
        }

        public IActionResult ShowImages(string pageHeading, int imageCount, string folderName = "default")
        {
            var model = new DreamTeamViewModel
            {
                PageHeading = pageHeading,                
                ImageCount = imageCount,
                FolderName = folderName
            };

            return View(model);
        }


        



    }
}
