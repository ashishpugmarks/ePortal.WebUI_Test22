using System.Data;
using System.Reflection.Emit;
using DocumentFormat.OpenXml.Spreadsheet;
using ePortal.Persistence.Admin.Interface;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.HCG;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class HCGController : Controller
    {
        private readonly ILogger<HomeController> _logger;        
        private readonly ISessionService _sessionService;
        private readonly IHCG objhcg;
        public HCGController(IHCG _objhcg) {
            objhcg = _objhcg;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> HCGknowledge()
        {
            HcgListViewModel vwMdl = new HcgListViewModel();
            DataSet objds = new DataSet();
            objds = objhcg.gethcgDetails();
            vwMdl.HcgItems = objds.Tables[0].AsEnumerable().Select(r => new HcgItem
            {
                HcgType = Convert.ToInt32(r["HCGTYPE"].ToString()),
                Series = r["HCGSERIES"].ToString(),
                ReleaseDate = r["RELEASEDATE"].ToString(),
                Attachment = r["ATTECHMENT"].ToString()                
            }).ToList();

            return View(vwMdl);
        }

        //protected void grdhcg_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.EmptyDataRow)
        //    {
        //        object dataitem = e.Row.DataItem;
        //        if (dataitem != null)
        //        {
        //            ((HyperLink)e.Row.FindControl("hlseries")).Text = Convert.ToString(DataBinder.Eval(dataitem, "HCGSERIES"));
        //            ((Label)e.Row.FindControl("lblreleasedate")).Text = String.Format("{0:dd MMM yyyy }", Convert.ToDateTime(DataBinder.Eval(dataitem, "RELEASEDATE").ToString()));
        //            ((HyperLink)e.Row.FindControl("hlseries")).NavigateUrl = "javascript:WindowSettings_WithoutMenu('../../Uploads/HCGknowledge/" + Convert.ToString(DataBinder.Eval(dataitem, "ATTECHMENT")) + "','test')";
        //        }
        //    }
        //}



       
    }
}
