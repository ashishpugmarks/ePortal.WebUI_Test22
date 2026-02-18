using ePortal.DomainClasses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ePortal.ViewModels
{
    public class NewsLetterViewModel  //Added By Bhupesh - NTT for CR-4894
    {
        public int? Id { get; set; } 
        [Required]
        public string Description { get; set; }
        [Required]
        public IFormFile Document { get; set; }
        [Required]
        public string Status { get; set; }

        public List<NEWSLETTERS>? NewsLetters { get; set; } 
    }
}
