using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface IPresidentDesk
    {
        //List<MenuViewModel> ();
        PresidentDeskViewModel GetPresidentDetail(PresidentDeskSearchModel objSearchModel);
        List<ContentViewModel> GetContent(ContentViewModel objsearch);
        List<EmergencyContViewModel> GetContactList(EmergencyContViewModel objsearch);
        PersonalityQuotesViewModel GetFamousQuotes(PersonalityQuotesViewModel objsearch);
        PresidentDesk_MSTViewModel InsertUpdatePresidentDetail(PresidentDesk_MSTViewModel collection);
        Int16 InsertUpdatePresidentDeskMessage(PresidentDesk_TRNViewModel collection);
        PresidentMsgAPP_TRNViewModel InsertUpdatePresidentMsgApproval(PresidentMsgAPP_TRNViewModel collection);
        FileViewModel GetFileForDownload(Int64 id);
        PresidentDesk_TRNViewModel DeleteDocument(Int64 id);
        long GetApprovalCount(long userid);
        long GetRequestCount(long userid);
    }
}
