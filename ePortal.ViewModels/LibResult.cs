using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class LibResult
    {
        public bool hasError { get; set; }
        public string errorMessage { get; set; }

        public object resultObject { get; set; }

        public LibResult()
        {
            this.hasError = false;
            this.errorMessage = "";
        }

        public LibResult(bool error, string ErrorMessage = "")
        {
            this.hasError = error;
            this.errorMessage = ErrorMessage;
        }

        internal object Select(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
