using ePortal.Persistence;
using ePortal.Persistence.Interface;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static ePortal.ViewModels.TaxationViewModel;

namespace ePortal.WebUI.Controllers
{
    public class TaxationController : Controller
    {

        private readonly ITaxationServices _taxationService;
        public TaxationController(ITaxationServices taxationService)
        {
            _taxationService = taxationService;
        }
        public IActionResult Index()
        {
            return View();
        }


        // Equivalent of FillTaxationGrid()
        public IActionResult TaxationCirculars()
        {
            var TCircularsDt = _taxationService.GetTCircularsDetails();

            var TCircularsLists = TCircularsDt.Tables[0].AsEnumerable()
             .Select(row => new TaxationViewModel
             {
                    Series = row.Field<string>("TAXATIONCSDES"),
                    ReleaseDate = row.Field<DateTime>("RELEASEDATE"),
                    ATTECHMENT = row.Field<string>("ATTECHMENT")

             })
             .ToList();
            return View("TaxationCirculars", TCircularsLists);
        }
        public IActionResult Taxationknowledgeseries()
        {
            var TAwarenessDt = _taxationService.GetTAwarenessDetails();

            var TAwarenessLists = TAwarenessDt.Tables[0].AsEnumerable()
             .Select(row => new TaxationViewModel
             {
                 Series = row.Field<string>("TAXATIONASDES"),
                 ReleaseDate = row.Field<DateTime>("RELEASEDATE"),
                 ATTECHMENT = row.Field<string>("ATTECHMENT")

             })
             .ToList();
            return View("Taxationknowledgeseries", TAwarenessLists);
        }
        public IActionResult TaxationRelatedForm()
        {
            var TFormsDt = _taxationService.GetTFormsDetails();

            var TFormsList = TFormsDt.Tables[0].AsEnumerable()
             .Select(row => new TaxationViewModel
             {
                 Series = row.Field<string>("TAXATIONFSDES"),
                 ReleaseDate = row.Field<DateTime>("RELEASEDATE"),
                 ATTECHMENT = row.Field<string>("ATTECHMENT")

             })
             .ToList();
            return View("TaxationRelatedForm", TFormsList);
        }
    }
}
