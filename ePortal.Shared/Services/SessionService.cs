using System.Text;
using System.Text.Json;
using ePortal.Shared.Interface;
using Microsoft.AspNetCore.Http;




namespace ePortal.Shared.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //public void Set<T>(string key, T value)
        //{
        //    //var session = _httpContextAccessor.HttpContext?.Session;
        //    //if (session != null)
        //    //{
        //    //    string json = JsonSerializer.Serialize(value);
        //    //    session.SetString(key, json);
        //    //}

        //    var json = System.Text.Json.JsonSerializer.Serialize(value);
        //    _httpContextAccessor.HttpContext?.Session.SetString(key, json);
        //}

        //public T? Get<T>(string key)
        //{
        //    var session = _httpContextAccessor.HttpContext?.Session;
        //    var json = session?.GetString(key);
        //    //return json == null ? default : JsonSerializer.Deserialize<T>(json);
        //    return  string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        //}

        public void Set<T>(string key, T value)
        {
            if (value is string stringValue)
            {
                _httpContextAccessor.HttpContext?.Session.SetString(key, stringValue);
            }
            else
            {
                var json = JsonSerializer.Serialize(value);
                _httpContextAccessor.HttpContext?.Session.SetString(key, json);
            }
        }

        public T? Get<T>(string key)
        {
            var storedValue = _httpContextAccessor.HttpContext?.Session.GetString(key);
            if (storedValue == null)
                return default;

            if (typeof(T) == typeof(string))
            {
                return (T)(object)storedValue;
            }

            return JsonSerializer.Deserialize<T>(storedValue);
        }


        public void Remove(string key)
        {
            _httpContextAccessor.HttpContext?.Session?.Remove(key);
        }

        public void Clear()
        {
            _httpContextAccessor.HttpContext?.Session?.Clear(); //Clear all session keys.
        }
    }
}
