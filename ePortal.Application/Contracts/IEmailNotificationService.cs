using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface IEmailNotificationService
    {
        bool SendItemRejectedNotification(LostAndFoundViewModel item, string userEmail, string userName);
        bool SendItemApprovedNotification(LostAndFoundViewModel item, string userEmail, string userName);
        bool SendItemClaimedNotification(LostAndFoundViewModel item, string userEmail, string userName);
        bool SendItemSentBackNotification(LostAndFoundViewModel item, string userEmail, string userName);
    }
}
