using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IFCMNotificationContract
    {
        Task<List<FCMNotificationModel>> SendFCMNotification(List<FCMNotificationModel> obj);
    }
}