using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface IHomePageService
    {
        List<MenuViewModel> GetMenu(int internet);
        PresidentDeskViewModel GetPresidentDetail(PresidentDeskViewModel objsearch);
        List<ContentViewModel> GetContent(ContentViewModel objsearch);
        List<ContentViewModel> GetContent();
        List<ContentViewModel> GetCommunicationContent();
        List<EmergencyContViewModel> GetContactList(EmergencyContViewModel objsearch);
        PersonalityQuotesViewModel GetFamousQuotes(PersonalityQuotesViewModel objsearch);
        long GetApprovalCount(long userid);
        long GetRequestCount(long userid);
        List<PeopleSerchViewModel> GetAssociateDetails(string KI, string OperationID, string DivisionID, string DepartmentID, string SectionID, string EmpCode, string FirstName, string LastName, string Designation, string EMailID, string BloodGroup);

        List<PresidentDesk_TRNViewModel> GetPresidentContent(List<PresidentDesk_TRNViewModel> objsearch);
        List<PresidentDesk_TRNViewModel> GetPresidentContentDwn(List<PresidentDesk_TRNViewModel> objsearch);
        Task CreateUserMenuLog(string Ecode, string URL_IN);
        List<MenuViewModel> GetQuickLinkMenu(int internet);
        MenuViewModel GetMenuURL(MenuViewModel objMenu);
        Task<string> GetMandatorytoFilledPage(decimal Ecode, decimal Usertype, Int16 ISOnlyMandatory, decimal ISSSOLOGIN); //--90/30 days password policy
        Task<string> ISAUTH_ACCESS(string Ecode, string URL);
        List<ExtPortalViewModel> GetExtPortalList();
    }
}
