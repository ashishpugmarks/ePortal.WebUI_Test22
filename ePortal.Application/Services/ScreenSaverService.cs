using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Services
{
    public class ScreenSaverService : IScreenSaverService
    {
        private readonly ScreenSaverRepository objScreenSaverRepositry;
        private readonly CommonRepository objcommRespository;
        public ScreenSaverService(ScreenSaverRepository saverRepository, CommonRepository commonRepo)
        {
            objScreenSaverRepositry = saverRepository;
            objcommRespository = commonRepo;
        }

        public List<ScreenSaverViewModel> GetScreenSaverMasterList(ScreenViewModel SVM)
        {
            try
            {
                List<ScreenSaverViewModel> ScreenSaverList =
                objScreenSaverRepositry.GetScreenSaverMasterList();

                string DeptName = string.IsNullOrEmpty(SVM.DepartmentName) ? "" : SVM.DepartmentName;
                int? Status_ = SVM.Status == null || SVM.Status == -1 ? (int?)null : Convert.ToInt32(SVM.Status);
                //string LASTREVISEDDATE_ = string.IsNullOrEmpty(SVM.LASTREVISEDDATE.ToString()) ? "" : Convert.ToDateTime(SVM.LASTREVISEDDATE).ToString("dd/mm/yyyy");

                ScreenSaverList = ScreenSaverList
                    .Where(x =>
                        (string.IsNullOrEmpty(DeptName) || x.DEPARTNAME == DeptName) &&
                        /*  (string.IsNullOrEmpty(LASTREVISEDDATE_) || x.LASTREVISEDDATE == LASTREVISEDDATE_) &&*/
                        (!Status_.HasValue || x.STATUS == Status_)
                    ).ToList();

                //if (SVM.Status == null && SVM.LASTREVISEDDATE == null)
                //{
                //    return ScreenSaverList;
                //}
                //else if (SVM.Status == 1)
                //{
                //    ScreenSaverList = ScreenSaverList.Where(x => x.DEPARTNAME == SVM.DepartmentName && x.LASTREVISEDDATE == Convert.ToDateTime(x.LASTREVISEDDATE).ToString() && x.STATUS == SVM.Status).ToList();
                //}
                //else if (SVM.Status == 0)
                //{
                //    ScreenSaverList = ScreenSaverList.Where(x => x.DEPARTNAME == SVM.DepartmentName && x.LASTREVISEDDATE == Convert.ToDateTime(x.LASTREVISEDDATE).ToString() && x.STATUS == SVM.Status).ToList();
                //}

                //else
                //{
                //    ScreenSaverList = ScreenSaverList.Where(x => x.DEPARTNAME == SVM.DepartmentName && x.LASTREVISEDDATE == Convert.ToDateTime(x.LASTREVISEDDATE).ToString() && x.STATUS == SVM.Status).ToList();
                //}

                return ScreenSaverList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public short SaveProcessScreenSaver_Trn(ScreenSaverViewModel AVM)
        {
            return objScreenSaverRepositry.SaveProcessScreenSaver_Trn(AVM);
        }

        public ScreenSaverViewModel GetScreenSaverDetails(long id)
        {
            return objScreenSaverRepositry.GetScreenSaverDetails(id);
        }
        public FileViewModel GetFileForDownload(Int64 id, string type)
        {
            return objScreenSaverRepositry.GetFileForDownload(id, type);
        }

        public IEnumerable<ScreenViewModel> BindScreenSaver()
        {
            List<ScreenViewModel> items = new List<ScreenViewModel>();
            List<long> Processids = new List<long>();
            items = objScreenSaverRepositry.BindScreenSaver().ToList();
            var res = items.Where(p => Processids.Contains(p.SAVERID)).ToList();

            if (res != null)
            {
                items.Clear();
                foreach (var proc in res)
                {
                    items.Add(new ScreenViewModel
                    {
                        SAVERID = proc.SAVERID,
                        DepartmentName = proc.DepartmentName,
                        LASTREVISEDDATE=proc.LASTREVISEDDATE,
                        FILE_NAME= Convert.ToBase64String(proc.Policies, 0, proc.Policies.Length)
                    });
                }
            }
            return items;
        }

        public short UpdateScreenSaver_Trn(ScreenSaverViewModel AVM)
        {
            return objScreenSaverRepositry.UpdateScreenSaver_Trn(AVM);
        }

        public List<ScreenSaverViewModel> ScreenSaver(ScreenViewModel SVM)
        {
            return objScreenSaverRepositry.ScreenSaver();
        }
    }
}
