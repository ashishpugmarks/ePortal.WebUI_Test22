using Microsoft.AspNetCore.Mvc;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.DomainClasses;
using System.Diagnostics.Contracts;


namespace ePortal.WebUI.Controllers
{
    public class ChroniclesController : Controller
    {
        private readonly IChroniclesService _chroniclesService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<ChroniclesController> _logger;
        private readonly IConfiguration _configuration;
        public ChroniclesController(IChroniclesService chroniclesService, ISessionService sessionService, ILogger<ChroniclesController> logger, IConfiguration configuration)
        {
            _chroniclesService = chroniclesService;
            _sessionService = sessionService;
            _logger = logger;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> GetChroniclesData()
        {
            List<ChroniclesViewModel> iList = await _chroniclesService.GetChroniclesData();
            return View("Chronicles", iList);
        }
        public IActionResult ChronicleVolumes()
        {
            return View("ChronicleVolumes");
        }
        [HttpGet]
        public async Task<ActionResult> AddNewVolume(int? id)
        {
            if (id.HasValue)
            {
                List<ChroniclesViewModel> iList = await _chroniclesService.GetChronicleVolumeDataById(id.Value);
                if (iList == null || !iList.Any())
                    return NotFound();
                var record = iList.First();
                var model = new ChroniclesViewModel
                {
                    HRC_ROWID = record.HRC_ROWID,
                    HRC_TITLE = record.HRC_TITLE,
                    HRC_DESCRIPTION = record.HRC_DESCRIPTION,
                    HRC_VALID_FROM = record.HRC_VALID_FROM,
                    HRC_VALID_TILL = record.HRC_VALID_TILL,
                    HRC_STATUS = record.HRC_STATUS,
                    HRC_DOC_NAME = record.HRC_DOC_NAME,
                    HRC_DOC_TYPE = record.HRC_DOC_TYPE,
                    HRC_CREATED_BY = record.HRC_CREATED_BY
                };
                return View("AddNewVolume",model); 
            }
            else
            {
              return View("AddNewVolume");
            }
        }

        [HttpGet]
        public async Task<IActionResult> get_ChronicleVolumeData()
        {
            var data = await _chroniclesService.GetChronicleVolumeData();
            return Ok(data);
        }
    
        [HttpPost]
        public async Task<IActionResult> AddChroniclesData(ChroniclesViewAddModel obj)
        {    
           var result = await _chroniclesService.AddChroniclesData(obj);
            if (result != null) 
            {
                TempData["SwalMessage"] = "Data saved successfully";
                TempData["SwalType"] = "success";
                return View("AddNewVolume");
            }
            else
            {
                TempData["SwalMessage"] = "Failed to save data";
                TempData["SwalType"] = "error";
                return View("AddNewVolume");
            }
        }
        public async Task<ActionResult> ViewChronicle(string docname)
        {
            var extension = Path.GetExtension(docname)?.ToLower();
            if (extension == ".png" || extension == ".jpg")
            {
                ViewBag.ImageType = "IMAGE";
            }
            else if (extension == ".mp4")
            {
                ViewBag.ImageType = "VIDEO";
            }
            ViewBag.Filename = docname;
            return View("ViewChronicle");        
        }
    }
}
