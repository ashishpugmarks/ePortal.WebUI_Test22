using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class SecurityLocationMapUpdateDto
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int EmpCode { get; set; }
        public int IsActive { get; set; }
    }
}
