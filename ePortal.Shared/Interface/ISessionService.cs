using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ePortal.Shared.Interface
{
    public interface ISessionService
    {
        void Set<T>(string key, T value);
        T? Get<T>(string key);
        void Remove(string key);
        void Clear();
    }
}
