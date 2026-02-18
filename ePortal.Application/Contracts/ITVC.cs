using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface ITVC
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
        List<TVCMasterViewModel> GetTVCMasterList(SearchViewModel SVM);
        TVCMasterViewModel SaveProcessAttachment_Trn(TVCMasterViewModel AVM);
        TVCMasterViewModel UpdateProcessAttachment_Trn(TVCMasterViewModel AVM);
        //AnnouncementMasterViewModel GetEditProcessAttachmentById(int id);
        TVCMasterViewModel GetTVCDetails(int id);
        #endregion

        #region Attachment Approval 
        //IEnumerable<EmployeeViewModel> BindEmployeeBy_Designation(string[] ParameterValue);
        //IEnumerable<TVCApprovalViewModel> GetTVCApprovalAuthorityList();
        TVCApprovalViewModel GetTVCApprovalAuthorityById(int id);
        //TVCApprovalViewModel SaveHRApproval(TVCApprovalViewModel AAVM);
        IEnumerable<TVCApprovalViewModel> GetTVCApproval_List(Int64 UserId);
        TVCApprovalViewModel UpdateStatus(TVCApprovalViewModel AAVM);
        FileViewModel GetFileForDownload(Int64 id, string type);
        TVCMasterViewModel DeleteDocument(Int64 id, string type);
        #endregion
    }
}
