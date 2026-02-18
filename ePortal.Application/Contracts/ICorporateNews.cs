using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface ICorporateNews
    {
        //// Content Process Master
        //#region Content Process Master
        //IEnumerable<ProcessMstViewModel> GetProcess_Mst_List();
        IEnumerable<ProcessMstViewModel> BindContentProcess();
        //ProcessMstViewModel SaveProcess_Mst(ProcessMstViewModel PVM);
        //ProcessMstViewModel UpdateProcess_Mst(ProcessMstViewModel PVM);
        //ProcessMstViewModel GetEditProcess_MstById(int id);
        //#endregion

        // Process Attachment Trn
        #region Process Attachment Trn

        List<CorporateNewsMasterViewModel> GetCorporateNewsMasterList(SearchViewModel SVM);
        Int16 SaveProcessAttachment_Trn(CorporateNewsMasterViewModel AVM);
        Int16 UpdateProcessAttachment_Trn(CorporateNewsMasterViewModel AVM);
        //AnnouncementMasterViewModel GetEditProcessAttachmentById(int id);
        CorporateNewsMasterViewModel GetCorporateNewsDetails(int id);
        #endregion

        #region Attachment Approval 
        //IEnumerable<EmployeeViewModel> BindEmployeeBy_Designation(string[] ParameterValue);
        //IEnumerable<CorporateNewsApprovalViewModel> GetCorporateNewsApprovalAuthorityList();
        CorporateNewsApprovalViewModel GetCorporateNewsApprovalAuthorityById(int id);
        //CorporateNewsApprovalViewModel SaveHRApproval(CorporateNewsApprovalViewModel AAVM);
        IEnumerable<CorporateNewsApprovalViewModel> GetCorporateNewsApproval_List(Int64 UserId);
        CorporateNewsApprovalViewModel UpdateStatus(CorporateNewsApprovalViewModel AAVM);
        FileViewModel GetFileForDownload(Int64 id, string type);
        CorporateNewsMasterViewModel DeleteDocument(Int64 id, string type);
        #endregion

        #region Newsletter
        //Added By Bhupesh - NTT for CR-4894
        void AddNewsLetter(NEWSLETTERS newsLetter);
        List<NEWSLETTERS> GetAllNewsLetters();
        NEWSLETTERS GetNewsLetterById(int id);
        void UpdateNewsLetter(NEWSLETTERS newsLetter);
        #endregion
    }
}
