using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels.APPX.ISMS
{
    public class FAQModel
    {
        public List<FaqViewModel> FaqViewModel { get; set; }
        public List<FAQCategorViewModel> Categories { get; set; }
        public string SelectedCategoryId { get; set; }
    }
    public class FaqViewModel
    {
        public string Description { get; set; }
        public string Answer { get; set; }
    }
    public class FAQCategorViewModel
    {
        public int CatId { get; set; }
        public string Category { get; set; }
    }

}
