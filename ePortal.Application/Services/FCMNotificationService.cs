using ePortal.Application.Contracts;
using ePortal.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Application.Services
{
    public class FCMNotificationService : IFCMNotificationContract
    {
        readonly HttpClient _httpClient;
        readonly IConfiguration _configuration;
        public FCMNotificationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<FCMNotificationModel>> SendFCMNotification(List<FCMNotificationModel> obj)
        {
            foreach (var item in obj)
            {
                item.accessToken = _configuration["FCMSettings:FCMToken"].ToString();
            }

            var json = JsonSerializer.Serialize(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("account/SendFCMNotification", content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<FCMNotificationModel>>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}