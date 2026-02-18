using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ePortal.Application.Contracts
{
    public interface ITokenProvider
    {
        Task<string> GetTokenAsync();
        Task<string> GetOrCreateTokenAsync();
    }
}
