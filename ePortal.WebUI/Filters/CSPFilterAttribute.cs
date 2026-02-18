using ePortal.Shared.Interface;
using ePortal.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ePortal.WebUI.Filters
{
    public class CSPFilterAttribute : TypeFilterAttribute
    {
        public CSPFilterAttribute(string formActionEndpoints = "", int noOfNonces = 11)
       : base(typeof(CSPAttribute))
        {
            Arguments = new object[]
            {
            new CSPOptions
            {
                FormActionEndpoints = formActionEndpoints,
                NoOfNonces = noOfNonces
            }
            };
        }
    }
}
