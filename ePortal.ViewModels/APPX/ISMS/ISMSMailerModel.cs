using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.ISMS
{
    public class ISMSMailerModel
    {
        public List<GetMaillerDetailsViewModel> GetMaillerDetailsViewModel {  get; set; }
        public List<CategoryViewModel> CategoryViewModel { get; set; }
        public List<MonthCategoryViewModel> MonthCategoryViewModel { get; set; }
        public List<YearCategoryViewModel> YearCategoryViewModel { get; set; }
        public string SelectedYear { get; set; }
        public string SelectedMonth { get; set; }
        public string SelectedCategoryId { get; set; }
    }
    public class GetMaillerDetailsViewModel
    {
        public int MailerId { get; set; }
        public string FileName { get; set; }
        public string MailerDescription { get; set; }
        public DateTime DateValue { get; set; }
        public string MonthYear { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
    }
    public class CategoryViewModel
    {
        public int CatId { get; set; }
        public string CategoryName { get; set; }
    }
    public class MonthCategoryViewModel
    {
        public string MonthYear { get; set; }
    }
    public class YearCategoryViewModel
    {
        public string Year { get; set; }
    }
 
}
