using ePortal.BusinessLibraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Repositories;
using ePortal.ViewModels;
using ePortal.Application.Contracts;

namespace ePortal.Application.Services
{
    public class TVCService : ITVC
    {
        TVCRepository objTVCRepositry;
        public TVCService()
        {
            objTVCRepositry = new TVCRepository();
        }
        // Content Process Master
        #region Content Process Master
        public IEnumerable<ProcessMstViewModel> BindContentProcess()
        {
            List<long> Processids = new List<long>() { 1 };
            List<ProcessMstViewModel> items = new List<ProcessMstViewModel>();
            items = objTVCRepositry.BindContentProcess().ToList();
            var res = items.Where(p => Processids.Contains(p.PROCESSID)).ToList(); // 2:Road Safety, 3:Press Release, 4:CSR, 5:Motorsport News
            if (res != null)
            {
                items.Clear();
                foreach (var proc in res)
                {
                    items.Add(new ProcessMstViewModel
                    {
                        PROCESSID = proc.PROCESSID,
                        PROCESS_NAME = proc.PROCESS_NAME,
                        DISPLAY_FILE = Convert.ToBase64String(proc.Upload_Banner, 0, proc.Upload_Banner.Length)
                    });
                }
            }
            return items;
        }

        //public ProcessMstViewModel UpdateProcess_Mst(ProcessMstViewModel PVM)
        //{
        //    return objTVCRepositry.UpdateProcess_Mst(PVM);
        //}

        //public ProcessMstViewModel GetEditProcess_MstById(int id)
        //{
        //    return objTVCRepositry.GetEditProcess_MstById(id);
        //}

        //public IEnumerable<ProcessMstViewModel> GetProcess_Mst_List()
        //{
        //    return objTVCRepositry.GetProcess_Mst_List();
        //}

        //public ProcessMstViewModel SaveProcess_Mst(ProcessMstViewModel PVM)
        //{
        //    return objTVCRepositry.SaveProcess_Mst(PVM);
        //}
        #endregion

        // Process Attachment Trn
        #region Process Attachment Trn
        public TVCMasterViewModel SaveProcessAttachment_Trn(TVCMasterViewModel AVM)
        {
            return objTVCRepositry.SaveProcessAttachment_Trn(AVM);
        }

        public TVCMasterViewModel UpdateProcessAttachment_Trn(TVCMasterViewModel AVM)
        {
            return objTVCRepositry.UpdateProcessAttachment_Trn(AVM);
        }

        List<TVCMasterViewModel> ITVC.GetTVCMasterList(SearchViewModel SVM)
        {
            List<TVCMasterViewModel> TVCList = objTVCRepositry.GetTVCMasterList();
            //List<AnnouncementMasterViewModel> iList = new List<AnnouncementMasterViewModel>();

            if (SVM.Status == null && SVM.FromDate == null && SVM.ToDate == null)
            {
                return TVCList;
            }
            else if (SVM.Status == -1)
            {
                TVCList = TVCList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate))).ToList();
            }
            else
            {
                TVCList = TVCList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate)) && x.STATUS == SVM.Status).ToList();
            }
            return TVCList;
        }

        //public TVCMasterViewModel GetEditProcessAttachmentById(int id)
        //{
        //    return objTVCRepositry.GetEditProcessAttachmentById(id);
        //}

        public TVCMasterViewModel GetTVCDetails(int id)
        {
            return objTVCRepositry.GetTVCDetails(id);
        }

        #endregion

        #region Attachment Approval
        //public IEnumerable<TVCApprovalViewModel> GetTVCApprovalAuthorityList()
        //{
        //    return objTVCRepositry.GetITVCApprovalAuthorityList();
        //}

        public TVCApprovalViewModel GetTVCApprovalAuthorityById(int id)
        {
            return objTVCRepositry.GetTVCApprovalAuthorityById(id);
        }

        //public IEnumerable<EmployeeViewModel> BindEmployeeBy_Designation(string[] parametervalue)
        //{
        //    return objTVCRepositry.BindAllEmploye(parametervalue);
        //}

        //public TVCApprovalViewModel SaveHRApproval(TVCApprovalViewModel AAVM)
        //{
        //    return objTVCRepositry.SaveHRApproval(AAVM);
        //}

        public IEnumerable<TVCApprovalViewModel> GetTVCApproval_List(Int64 UserId)
        {
            return objTVCRepositry.GetTVCApprovalList(UserId);
        }
        public TVCApprovalViewModel UpdateStatus(TVCApprovalViewModel AAVM)
        {
            return objTVCRepositry.UpdateStatus(AAVM);
        }
        public FileViewModel GetFileForDownload(Int64 id, string type)
        {
            return objTVCRepositry.GetFileForDownload(id, type);
        }
        public TVCMasterViewModel DeleteDocument(Int64 id, string type)
        {
            return objTVCRepositry.DeleteDocument(id, type);
        }
        #endregion
    }
}
