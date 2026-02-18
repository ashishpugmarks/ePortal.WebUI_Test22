


using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class CreativeMasterService : ICreativeMaster
    {
        CreativeMasterRepository objCreativeMasterRepositry;
        CommonRepository objcommRespository;
        public CreativeMasterService(CreativeMasterRepository creativeMasterRepositry, CommonRepository commRespository)
        {
            objCreativeMasterRepositry = creativeMasterRepositry;
            objcommRespository = commRespository;
        }
        // Content Process Master
        //#region Content Process Master
        public IEnumerable<ProcessMstViewModel> BindContentProcess()
        {
            string Processids1 = objcommRespository.GetParameterValue("CORPORATENEWS_PRCID");
            string[] values = Processids1.Split(',');
            List<long> Processids = new List<long>();
            for (int i = 0; i < values.Length; i++)
            {
                long lngval = Convert.ToInt32(values[i].Trim().ToString().Split('~')[1]);
                Processids.Add(lngval);
            }
            //List<long> Processids = new List<long>() { 2, 3, 4, 5, 7 };
            List<ProcessMstViewModel> items = new List<ProcessMstViewModel>();
            items = objCreativeMasterRepositry.BindContentProcess().ToList();
            //var res = items.Where(p => Processids.Contains(p.PROCESSID)).ToList(); // 2:Road Safety, 3:Press Release, 4:CSR, 5:Motorsport News
            //var res = items; 
            //if (res != null)
            //{
            //    items.Clear();
            //    foreach (var proc in res)
            //    {
            //        items.Add(new ProcessMstViewModel
            //        {
            //            PROCESSID = proc.PROCESSID,
            //            PROCESS_NAME = proc.PROCESS_NAME,
            //            DISPLAY_FILE = Convert.ToBase64String(proc.Upload_Banner, 0, proc.Upload_Banner.Length)
            //        });
            //    }
            //}
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
        //#endregion

        // Process Attachment Trn
        //#region Process Attachment Trn
        public Int16 SaveProcessAttachment_Trn(CreativeMasterViewModel AVM)
        {
            return objCreativeMasterRepositry.SaveProcessAttachment_Trn(AVM);
        }

        public Int16 UpdateProcessAttachment_Trn(CreativeMasterViewModel AVM)
        {
            return objCreativeMasterRepositry.UpdateProcessAttachment_Trn(AVM);
        }

        List<CreativeMasterViewModel> ICreativeMaster.GetCorporateNewsMasterList(SearchViewModel SVM)
        {
            List<CreativeMasterViewModel> CorporateNewsList = objCreativeMasterRepositry.GetCorporateNewsMasterList();
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
                CorporateNewsList = CorporateNewsList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate)) && DateTime.Now > Convert.ToDateTime(x.END_DATE) && x.STATUS==1).ToList();
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

        public CreativeMasterViewModel GetCorporateNewsDetails(int id)
        {
            return objCreativeMasterRepositry.GetCorporateNewsDetails(id);
        }

        public CreativeMasterViewModel GetCreativeMSTRequestById(long id)
        {
            return objCreativeMasterRepositry.GetCreativeMSTRequestById(id);
        }

        public List<Employee_Details> PortalAutocompleteSuggestions(string term)
        {
            return objCreativeMasterRepositry.PortalAutocompleteSuggestions(term);
        }
        public IEnumerable<Employee_Details> BindAppAuth1()
        {
            return objCreativeMasterRepositry.BindAppAuth1().ToList();
        }

        
        public short CreativeMasterAppr(CM_Processattachmentappmapping_TrnViewModel PHVM, Employee_Details emp_dtl)
        {
            short retVal = objCreativeMasterRepositry.CreativeMasterAppr(PHVM, emp_dtl);
            if (retVal == 1)
            {
                //SendMailByApprovalAuthority(IHVM.IOMID, IHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }

        public CreativeMasterViewModel DeleteDocument(Int64 id, string type)
        {
            return objCreativeMasterRepositry.DeleteDocument(id, type);
        }

        //public IEnumerable<Employee_Details> BindAppAuth1()
        //{
        //    List<Employee_Details> items = new List<Employee_Details>();
        //    items = objCreativeMasterRepositry.BindAppAuth1().ToList();
        //}


        //#endregion

        //#region Attachment Approval
        //public IEnumerable<CorporateNewsApprovalViewModel> GetCorporateNewsApprovalAuthorityList()
        //{
        //    return objCorporateNewsRepositry.GetICorporateNewsApprovalAuthorityList();
        //}

        //public CorporateNewsApprovalViewModel GetCorporateNewsApprovalAuthorityById(int id)
        //{
        //    return objCorporateNewsRepositry.GetCorporateNewsApprovalAuthorityById(id);
        //}

        //public IEnumerable<EmployeeViewModel> BindEmployeeBy_Designation(string[] parametervalue)
        //{
        //    return objCorporateNewsRepositry.BindAllEmploye(parametervalue);
        //}

        //public CorporateNewsApprovalViewModel SaveHRApproval(CorporateNewsApprovalViewModel AAVM)
        //{
        //    return objCorporateNewsRepositry.SaveHRApproval(AAVM);
        //}

        //public IEnumerable<CorporateNewsApprovalViewModel> GetCorporateNewsApproval_List(Int64 UserId)
        //{
        //    return objCorporateNewsRepositry.GetCorporateNewsApprovalList(UserId);
        //}
        //public CorporateNewsApprovalViewModel UpdateStatus(CorporateNewsApprovalViewModel AAVM)
        //{
        //    return objCorporateNewsRepositry.UpdateStatus(AAVM);
        //}
        //public FileViewModel GetFileForDownload(Int64 id, string type)
        //{
        //    return objCorporateNewsRepositry.GetFileForDownload(id, type);
        //}
        //public CorporateNewsMasterViewModel DeleteDocument(Int64 id, string type)
        //{
        //    return objCorporateNewsRepositry.DeleteDocument(id, type);
        //}
        //#endregion
    }
}
