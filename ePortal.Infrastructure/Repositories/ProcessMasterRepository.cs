using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;

namespace ePortal.Infrastructure.Repositories
{
    public class ProcessMasterRepository
    {
        private EPortalDBContext _ProcessMasterDBContext;
        private SYKI _Syki;

        public ProcessMasterRepository(EPortalDBContext objProcessMasterDBContext)
        {
            _ProcessMasterDBContext = objProcessMasterDBContext;
            _Syki = _ProcessMasterDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public List<ProcessMasterViewModel> GetMenuList()
        {
            List<ProcessMasterViewModel> ilist = new List<ProcessMasterViewModel>();

            var data = (from t in _ProcessMasterDBContext.ADPROCESS_MST
                        where t.STATUS == 1
                        select new
                        {
                            t.PROCESS_ID,
                            t.PROCESS_NAME,
                            t.STATUS

                        }
            ).ToList();

            foreach (var itm in data)
            {
                ilist.Add(new ProcessMasterViewModel
                {
                    PROCESS_ID = itm.PROCESS_ID,
                    PROCESS_NAME = itm.PROCESS_NAME,
                    STATUS = itm.STATUS,
                    Status1 = (itm.STATUS == 1) ? "Active" : "Inactive",
                });
            }

            return ilist;


        }

        public short AddProcess(string ProcessName, string status, string UserID)
        {
            short retVal = 0;
            int FlagAdd = 0;
            int max = 0;

            using (var transaction = _ProcessMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    var ctg = _ProcessMasterDBContext.ADPROCESS_MST.Where(x => x.PROCESS_NAME == ProcessName).ToList();
                    if (ctg.Count() == 0)
                    {
                        var count = _ProcessMasterDBContext.ADPROCESS_MST.Count().ToString();
                        if (count == "0")
                        {
                            max = 1;
                        }
                        else
                        {

                            max = Int32.Parse(_ProcessMasterDBContext.ADPROCESS_MST.Max(i => i.PROCESS_ID).ToString()) + 1;

                        }
                        FlagAdd = 1;

                        if (ProcessName != "" && status != "")
                        {

                            ADPROCESS_MST IT = new ADPROCESS_MST();

                            IT.PROCESS_ID = max;
                            IT.PROCESS_NAME = ProcessName;
                            IT.CREATED_BY = Int32.Parse(UserID.ToString());
                            IT.CREATED_DATE = DateTime.Now;
                            IT.STATUS = Int16.Parse(status.ToString());



                            // _ITDIncomeTaxDeclarationDBContext.ITDDOCTYPEMASTERSet.Add(IT);

                            //_POADBContext.Entry(PSM).State = EntityState.Added;
                            _ProcessMasterDBContext.Entry(IT).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ProcessMasterDBContext.SaveChanges();
                            //_POADBContext.Entry(PSM).State = EntityState.Detached;


                            transaction.Commit();
                        }
                        retVal = 1;
                    }
                    else
                    {
                        retVal = 0;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                    throw ex;
                }

                return retVal;
            }
        }

        public short EditProcess(string ProcessID, string ProcessName, string status, string UserID)
        {
            short retVal = 0;
            int FlagAdd = 0;
            //int max = 0;

            using (var transaction = _ProcessMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    Decimal ProcessID1 = decimal.Parse(ProcessID.ToString());
                    var ctg = _ProcessMasterDBContext.ADPROCESS_MST.Where(x => x.PROCESS_ID == ProcessID1).FirstOrDefault();

                    FlagAdd = 0;

                    ctg.PROCESS_ID = ProcessID1;
                    ctg.PROCESS_NAME = ProcessName;
                    ctg.CREATED_BY = Int32.Parse(UserID.ToString());
                    ctg.CREATED_DATE = DateTime.Now;
                    ctg.STATUS = Int16.Parse(status.ToString());

                    _ProcessMasterDBContext.Entry(ctg).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ProcessMasterDBContext.SaveChanges();

                    transaction.Commit();
                    retVal = 1;

                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                    throw ex;
                }

                return retVal;
            }
        }

    }
}
