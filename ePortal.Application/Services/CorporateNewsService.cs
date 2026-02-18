using System.Data;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class CorporateNewsService : ICorporateNews
    {
        CorporateNewsRepository _objCorporateNewsRepositry;
        private readonly CommonRepository _objcommRespository;

        public CorporateNewsService(CorporateNewsRepository objCorporateNewsRepositry, CommonRepository objcommRespository)
        {
            _objCorporateNewsRepositry = objCorporateNewsRepositry;
            _objcommRespository = objcommRespository;
        }

        #region Content Process Master
        public IEnumerable<ProcessMstViewModel> BindContentProcess()
        {
            string Processids1 = _objcommRespository.GetParameterValue("CORPORATENEWS_PRCID");
            string[] values = Processids1.Split(',');
            List<long> Processids = new List<long>();
            for (int i = 0; i < values.Length; i++)
            {
                long lngval = Convert.ToInt32(values[i].Trim().ToString().Split('~')[1]);
                Processids.Add(lngval);
            }
            //List<long> Processids = new List<long>() { 2, 3, 4, 5, 7 };
            List<ProcessMstViewModel> items = new List<ProcessMstViewModel>();
            items = _objCorporateNewsRepositry.BindContentProcess().ToList();
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
        //public string GetParameterValue(string strParmaName)
        //{
        //    return objcommRespository.GetParameterValue(strParmaName);
        //}

        //public ProcessMstViewModel UpdateProcess_Mst(ProcessMstViewModel PVM)
        //{
        //    return objCorporateNewsRepositry.UpdateProcess_Mst(PVM);
        //}

        //public ProcessMstViewModel GetEditProcess_MstById(int id)
        //{
        //    return objCorporateNewsRepositry.GetEditProcess_MstById(id);
        //}

        //public IEnumerable<ProcessMstViewModel> GetProcess_Mst_List()
        //{
        //    return objCorporateNewsRepositry.GetProcess_Mst_List();
        //}

        //public ProcessMstViewModel SaveProcess_Mst(ProcessMstViewModel PVM)
        //{
        //    return objCorporateNewsRepositry.SaveProcess_Mst(PVM);
        //}
        #endregion

        // Process Attachment Trn
        #region Process Attachment Trn
        public Int16 SaveProcessAttachment_Trn(CorporateNewsMasterViewModel AVM)
        {
            return _objCorporateNewsRepositry.SaveProcessAttachment_Trn(AVM);
        }

        public Int16 UpdateProcessAttachment_Trn(CorporateNewsMasterViewModel AVM)
        {
            return _objCorporateNewsRepositry.UpdateProcessAttachment_Trn(AVM);
        }

        List<CorporateNewsMasterViewModel> ICorporateNews.GetCorporateNewsMasterList(SearchViewModel SVM)
        {
            List<CorporateNewsMasterViewModel> CorporateNewsList = _objCorporateNewsRepositry.GetCorporateNewsMasterList();
            //List<AnnouncementMasterViewModel> iList = new List<AnnouncementMasterViewModel>();

            if (SVM.Status == null && SVM.FromDate == null && SVM.ToDate == null)
            {
                return CorporateNewsList;
            }
            else if (SVM.Status == -1)
            {
                CorporateNewsList = CorporateNewsList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate))).ToList();
            }
            else if (SVM.Status == 4)
            {
                CorporateNewsList = CorporateNewsList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate)) && DateTime.Now > Convert.ToDateTime(x.END_DATE) && x.STATUS == 1).ToList();
            }
            else
            {
                CorporateNewsList = CorporateNewsList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate)) && x.STATUS == SVM.Status && DateTime.Now < Convert.ToDateTime(x.END_DATE)).ToList();
            }
            return CorporateNewsList;
        }

        //public CorporateNewsMasterViewModel GetEditProcessAttachmentById(int id)
        //{
        //    return objCorporateNewsRepositry.GetEditProcessAttachmentById(id);
        //}

        public CorporateNewsMasterViewModel GetCorporateNewsDetails(int id)
        {
            return _objCorporateNewsRepositry.GetCorporateNewsDetails(id);
        }

        #endregion

        #region Attachment Approval
        //public IEnumerable<CorporateNewsApprovalViewModel> GetCorporateNewsApprovalAuthorityList()
        //{
        //    return objCorporateNewsRepositry.GetICorporateNewsApprovalAuthorityList();
        //}

        public CorporateNewsApprovalViewModel GetCorporateNewsApprovalAuthorityById(int id)
        {
            return _objCorporateNewsRepositry.GetCorporateNewsApprovalAuthorityById(id);
        }

        //public IEnumerable<EmployeeViewModel> BindEmployeeBy_Designation(string[] parametervalue)
        //{
        //    return objCorporateNewsRepositry.BindAllEmploye(parametervalue);
        //}

        //public CorporateNewsApprovalViewModel SaveHRApproval(CorporateNewsApprovalViewModel AAVM)
        //{
        //    return objCorporateNewsRepositry.SaveHRApproval(AAVM);
        //}

        public IEnumerable<CorporateNewsApprovalViewModel> GetCorporateNewsApproval_List(Int64 UserId)
        {
            return _objCorporateNewsRepositry.GetCorporateNewsApprovalList(UserId);
        }
        public CorporateNewsApprovalViewModel UpdateStatus(CorporateNewsApprovalViewModel AAVM)
        {
            return _objCorporateNewsRepositry.UpdateStatus(AAVM);
        }
        public FileViewModel GetFileForDownload(Int64 id, string type)
        {
            return _objCorporateNewsRepositry.GetFileForDownload(id, type);
        }
        public CorporateNewsMasterViewModel DeleteDocument(Int64 id, string type)
        {
            return _objCorporateNewsRepositry.DeleteDocument(id, type);
        }
        #endregion

        #region Newsletters 
        //Added By Bhupesh - NTT for CR-4894
        public void AddNewsLetter(NEWSLETTERS newsLetter)
        {
            _objCorporateNewsRepositry.AddNewsLetter(newsLetter);
        }
        public List<NEWSLETTERS> GetAllNewsLetters()
        {
            return _objCorporateNewsRepositry.GetAllNewsLetters();
        }
        public NEWSLETTERS GetNewsLetterById(int id)
        {
            return _objCorporateNewsRepositry.GetNewsLetterById(id);
        }
        public void UpdateNewsLetter(NEWSLETTERS newsLetter)
        {
            _objCorporateNewsRepositry.UpdateNewsLetter(newsLetter);
        }
        #endregion
    }
}
