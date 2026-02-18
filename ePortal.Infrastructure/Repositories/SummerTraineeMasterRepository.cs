//using ePortal.DataModel;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Infrastructure.Repositories
{
    public class SummerTraineeMasterRepository
    {
        private EPortalDBContext _STMDBContext;
        private SYKI _Syki;
        public SummerTraineeMasterRepository(EPortalDBContext objEPortalDBContext)
        {
            _STMDBContext = objEPortalDBContext;
            _Syki = _STMDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public List<SummerTraineeViewModel> oplist(long obj)
        {
            List<SummerTraineeViewModel> result = (from data in _STMDBContext.DGIT_TR_OP_LIST
                                                   join data1 in _STMDBContext.ADORGLEVEL on data.OPID equals data1.ADORGLEVELID
                                                   where (obj == 0 ?  true : data.OPID == obj)
                                                   && data1.ACTIVE==1 && data.STATUS==1
                                                   select new SummerTraineeViewModel
                                                   {
                                                       ID=(long)data.ID,
                                                       OPID = (long)data.OPID,
                                                       OperationName = data1.LEVELDESCRIP
                                                   }
                                            ).ToList();
            return result;
        }

        public short Delete(long opID)
        {
            short retval = 0;
            try
            {
                DGIT_TR_OP_LIST DPAH = _STMDBContext.DGIT_TR_OP_LIST.Where(x => x.OPID == opID && x.STATUS==1).FirstOrDefault();
                int FlagAdd = 0;
                if (DPAH == null)
                {
                    retval = 0;
                    return retval;
                }
                else
                {
                    DPAH.STATUS = 0;
                    //_STMDBContext.Entry(DPAH).State = EntityState.Modified;
                    _STMDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _STMDBContext.SaveChanges();
                    retval = 1;
                    return retval;
                }
            }
            catch(Exception ex)
            {
                retval = -1;
                return retval;
            }
        }

        /*public short Add(long obj,string empcode)
        {
            short retval = 0;
            try
            {
                DGIT_TR_OP_LIST DPAH = _STMDBContext.DGIT_TR_OP_LIST.Where(x => x.OPID == obj && x.STATUS==1).FirstOrDefault();
                int FlagAdd = 0;
                if(DPAH == null)
                {
                    DPAH = new DGIT_TR_OP_LIST();
                    DPAH.ID = _STMDBContext.DGIT_TR_OP_LIST.Max(x => x.ID) + 1;
                    DPAH.OPID = obj;
                    DPAH.STATUS = 1;
                    DPAH.COUNT = 2;
                    DPAH.ADDEDBY = empcode;
                    DPAH.ADDEDDATE = DateTime.Now;

                    _STMDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _STMDBContext.SaveChanges();
                    retval = 1;
                    return retval;
                }
                else
                {
                    retval = 2;
                    return retval;
                }
            }
            catch(Exception ex)
            {
                retval = -1;
                return retval;
            }
        }*/
        public short Add(long obj, string empcode)
        {
            short retval = 0;
            try
            {
                var DPAH = _STMDBContext.DGIT_TR_OP_LIST
                    .FirstOrDefault(x => x.OPID == obj && x.STATUS == 1);

                if (DPAH == null)
                {
                    DPAH = new DGIT_TR_OP_LIST
                    {
                        ID = (_STMDBContext.DGIT_TR_OP_LIST.Max(x => (int?)x.ID) ?? 0) + 1,
                        OPID = obj,
                        STATUS = 1,
                        COUNT = 2,
                        ADDEDBY = empcode,
                        ADDEDDATE = DateTime.Now
                    };

                    _STMDBContext.DGIT_TR_OP_LIST.Add(DPAH); // Correct way to add
                    _STMDBContext.SaveChanges();
                    retval = 1;
                }
                else
                {
                    retval = 2;
                }
            }
            catch (Exception ex)
            {
                // Optional: log ex.Message
                retval = -1;
            }

            return retval;
        }
    }
}
