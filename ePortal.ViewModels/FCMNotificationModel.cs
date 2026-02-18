using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class FCMNotificationModel
    {
        public string userId { get; set; }
        public string? accessToken { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string? ttlSeconds { get; set; }
        public string? messageSubTitle { get; set; }
        public string? badgeCount { get; set; }
        public string soundName { get; set; } = "default";
        public string? channedID { get; set; }
        public string? loggedBy { get; set; }
        public string? response { get; set; }
    }
}