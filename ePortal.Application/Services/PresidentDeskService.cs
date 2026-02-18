using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;
using Microsoft.Extensions.Logging;

namespace ePortal.Application.Services
{
    public class PresidentDeskService : IPresidentDesk
    {
        private readonly PresedentDeskRepository _objPresidentDeskRepositry; 

        public PresidentDeskService(PresedentDeskRepository objPresidentDeskRepositry)
        {
            _objPresidentDeskRepositry = objPresidentDeskRepositry;  
        }

        public long GetApprovalCount(long userid)
        {
            throw new NotImplementedException();
        }

        public List<EmergencyContViewModel> GetContactList(EmergencyContViewModel objsearch)
        {
            throw new NotImplementedException();
        }

        public List<ContentViewModel> GetContent(ContentViewModel objsearch)
        {
            throw new NotImplementedException();
        }

        public PersonalityQuotesViewModel GetFamousQuotes(PersonalityQuotesViewModel objsearch)
        {
            throw new NotImplementedException();
        }

        public PresidentDeskViewModel GetPresidentDetail(PresidentDeskSearchModel objSearchModel)
        {
            var objRet = _objPresidentDeskRepositry.GetPresidentDetail(objSearchModel);
            return objRet;
        }
        public PresidentDesk_MSTViewModel InsertUpdatePresidentDetail(PresidentDesk_MSTViewModel collection)
        {          
                collection = _objPresidentDeskRepositry.InsertUpdatePresidentDetail(collection);
                return collection;           
        }
        public Int16 InsertUpdatePresidentDeskMessage(PresidentDesk_TRNViewModel collection)
        {
            return _objPresidentDeskRepositry.InsertUpdatePresidentDeskMessage(collection);
        }
        public PresidentMsgAPP_TRNViewModel InsertUpdatePresidentMsgApproval(PresidentMsgAPP_TRNViewModel collection)
        {
            return _objPresidentDeskRepositry.InsertUpdatePresidentMsgApproval(collection);
        }
        public FileViewModel GetFileForDownload(Int64 id)
        {
            return _objPresidentDeskRepositry.GetFileForDownload(id);
        }
        public PresidentDesk_TRNViewModel DeleteDocument(Int64 id)
        {
            return _objPresidentDeskRepositry.DeleteDocument(id);
        }
        public long GetRequestCount(long userid)
        {
            throw new NotImplementedException();
        }
    }
}
