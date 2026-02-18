using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IAnnouncementService
    {
        //// Content Process Master
        //#region Content Process Master
        IEnumerable<ProcessMstViewModel> GetProcess_Mst_List();
        IEnumerable<ProcessMstViewModel> BindContentProcess();
        ProcessMstViewModel SaveProcess_Mst(ProcessMstViewModel PVM);
        ProcessMstViewModel UpdateProcess_Mst(ProcessMstViewModel PVM);
        ProcessMstViewModel GetEditProcess_MstById(int id);
        //#endregion

        //// Process Attachment Trn
        //#region Process Attachment Trn
        List<AnnouncementMasterViewModel> GetAnnouncementMasterList(SearchViewModel SVM, Int64 UserId);
        List<AnnouncementMasterViewModel> GetAnnouncementArchiveList(SearchViewModel SVM, Int64 UserId);
        Int16 SaveProcessAttachment_Trn(AnnouncementMasterViewModel AVM);
        Int16 UpdateProcessAttachment_Trn(AnnouncementMasterViewModel AVM);
        //AnnouncementMasterViewModel DeactiveAnnouncement(AnnouncementMasterViewModel AVM);
        short DeactiveAnnouncement(AnnouncementMasterViewModel AVM);
        //AnnouncementMasterViewModel CancelAnnouncement(AnnouncementMasterViewModel AVM);
        short CancelAnnouncement(AnnouncementMasterViewModel AVM);
        AnnouncementMasterViewModel DeleteAnnouncementFile(long id, string type);
        AnnouncementMasterViewModel GetEditProcessAttachmentById(int id);
        AnnouncementMasterViewModel GetAnnouncementDetails(int id);
        IEnumerable<ADFUNCTIONALDESIGNATION> Bind_ADFunctionalDesignation();
        IEnumerable<ADDESIGNATION> Bind_ADDesignation();
        IEnumerable<SYSITE> Bind_SYSite();
        //#endregion

        //#region Attachment Approval 
        ////IEnumerable<EmployeeViewModel> BindEmployeeBy_Designation(string[] ParameterValue);
        String IsEmployeeActive(Int64 Empcode);
        IEnumerable<AnnouncementApprovalViewModel> GetAnnouncementApprovalAuthorityList();
        AnnouncementApprovalViewModel GetAnnouncementApprovalAuthorityById(int id);
        //Task<AnnouncementApprovalViewModel> GetAnnouncementApprovalAuthorityById(int id);
        AnnouncementApprovalViewModel SaveHRApproval(AnnouncementApprovalViewModel AAVM);
        IEnumerable<AnnouncementApprovalViewModel> GetAnnouncementApproval_List(Int64 UserId);
        //Task<IEnumerable<AnnouncementApprovalViewModel>> GetAnnouncementApproval_List(Int64 UserId);

        AnnouncementApprovalViewModel UpdateStatus(AnnouncementApprovalViewModel AAVM);
        //Task<AnnouncementApprovalViewModel> UpdateStatus(AnnouncementApprovalViewModel AAVM);
        FileViewModel GetFileForDownload(Int64 AttachmentId, string Attachment);
        //Task<FileViewModel> GetFileForDownload(Int64 AttachmentId, string Attachment);

        //#endregion

        //#region Communication Proess
        List<CommunicationViewModel> CommunicationRequestList(long userId);
        IEnumerable<CommCategoryViewModel> BindCommCategory();
        CommCategoryViewModel GetCommCategoryById(long id);
        IEnumerable<CommMailTypeViewModel> BindCommMailType();
        Tuple<short, long> SaveCommunicationRequest(CommunicationViewModel model);
        Tuple<short, long> FinalSubmitRequest(CommunicationViewModel model);
        Tuple<short, List<CommunicationDtlViewModel>> SaveCommAttachment(long addedDate, long CommheaderId, List<CommunicationDtlViewModel> modelList);
        List<CommunicationAppSeqViewModel> GetDefaultAuthority(long loginUser, long categoryID, Employee_Details _Employee_Details);
        Tuple<short, List<CommunicationDtlViewModel>> DeleteAttachment(string fileName, string docType, long commHid);
        CommunicationViewModel GetCommunicationDetails(long id);
        List<CommunicationDtlViewModel> GetCommunicationAttachments(long id);
        List<CommunicationViewModel> CommunicationApprovalList(long userId);
        List<CommunicationViewModel> CommunicationApprovalHistory(long userId);
        short CommunicationApproval(CommunicationAppHisViewModel PHVM, Employee_Details emp_dtl, List<CommunicationDtlViewModel> commDetailList);
        Tuple<short, long> UpdateCommunicationRequest(CommunicationViewModel model);

        List<CommunicationViewModel> ArchiveRequestList(int type, CommunicationViewModel model);

        short CancelCommunicationRequest(long id, long userID);
        short DeactivateCommunicationRequest(long id, long userID);

        List<CommunicationViewModel> SelfPendingCommunicationList(long userId);
        short InserViewData(long id, long userID);

        List<CommMailTypeViewModel> GetCommunicationTypeList();
        CommMailTypeViewModel GetCommunicationTypeDtlById(long id);
        short SaveCommunicationType(CommMailTypeViewModel AOVM);

        List<Comm_Op_Div_DepViewModel> BindOperation(long typeId);
        List<Comm_Op_Div_DepViewModel> BindDivision(long typeId, long op_Id);
        List<Comm_Op_Div_DepViewModel> BindDepartment(long typeId, long div_Id);

        List<CommunicationViewModel> ValidateArchiveRequestList(long CommID);
        //#endregion
        //Added by aumento for SR87026
        // Get Communication Report based on month and year
        List<SYKI_DGIT> GetKI();

        //List<CommunicationViewModel> _GetCommunicationReport(string startDate, string endDate, string KI);
        DataTable _GetCommunicationReport(string startDate, string endDate, string KI);
        //Added by aumento for SR87026
    }
}
