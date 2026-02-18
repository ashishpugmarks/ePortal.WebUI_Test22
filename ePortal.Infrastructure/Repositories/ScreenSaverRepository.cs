using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class ScreenSaverRepository
    {
        private readonly EPortalDBContext _ScreenSaverDBContext;
        public ScreenSaverRepository(EPortalDBContext dbContext)
        {
            _ScreenSaverDBContext = dbContext;
        }

        public List<ScreenSaverViewModel> GetScreenSaverMasterList()
        {
            List<ScreenSaverViewModel> iList = new List<ScreenSaverViewModel>();

            //List<ScreenSaverViewModel> data = (from m in _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST
            //                                   select new ScreenSaverViewModel
            //                                   {
            //                                       SAVERID = m.SAVERID, // no need to convert here if SAVERID is already of type short/int
            //                                       DEPARTNAME = m.DEPARTNAME,
            //                                       LASTREVISEDDATE = m.LASTREVISEDDATE.ToString(), // Assuming LASTREVISEDDATE is already a DateTime field in your database
            //                                      // STATUS = m.STATUS,// no need to convert here if STATUS is already of type short/int
            //                                      STATUS = (short)(m.STATUS ?? 0),
            //                                   }).ToList();

            // Now, after fetching data from the database, apply conversions
            //foreach (var item in data)
            //{
            //    item.LASTREVISEDDATE = DateTime.Parse(item.LASTREVISEDDATE).ToString("dd/MM/yy");
            //}

            //return data;
            
            var iColl = (from data in _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST
                         select new
                         {
                             data.SAVERID,
                             data.DEPARTNAME,
                             data.LASTREVISEDDATE,
                             data.STATUS
                         }).ToList();

            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {

                    iList.Add(new ScreenSaverViewModel
                    {
                        SAVERID = obj.SAVERID,
                        DEPARTNAME = obj.DEPARTNAME,
                        LASTREVISEDDATE = Convert.ToDateTime(obj.LASTREVISEDDATE).ToString("dd/MM/yy"),
                        STATUS = Convert.ToInt16(obj.STATUS)
                    });

                }
            }
            return iList;
        }

        public Int16 SaveProcessScreenSaver_Trn(ScreenSaverViewModel AVM)
        {
            Int16 retVal = 0;
            using (var transaction = _ScreenSaverDBContext.Database.BeginTransaction())
            {
                try
                {
                    //var MaxId = _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST.Select(x => x.SAVERID).DefaultIfEmpty().Max();
                    var MaxId = _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST.Max(x => x.SAVERID);

                    MaxId++;
                    CM_PROCESSSCREENSAVER_MST CPT = new CM_PROCESSSCREENSAVER_MST();
                    //CPT.SAVERID = AVM.SAVERID;
                    CPT.SAVERID = MaxId;
                    CPT.LASTREVISEDDATE = Convert.ToDateTime(AVM.LASTREVISEDDATE.ToString());
                    CPT.DEPARTNAME = CommonRepository.TextToHtml(AVM.DEPARTNAME);
                    CPT.STATUS = AVM.STATUS;
                    CPT.FILE_NAME = AVM.PoliciesFileName;
                    CPT.FILE_CONTENTTYPE = AVM.PoliciesContentType;
                    CPT.ADDEDON = DateTime.Now;

                    _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST.Add(CPT);
                    _ScreenSaverDBContext.SaveChanges();
                    transaction.Commit();
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public ScreenSaverViewModel GetScreenSaverDetails(long id)
        {
            ScreenSaverViewModel PAM = new ScreenSaverViewModel();
            CM_PROCESSSCREENSAVER_MST CPM = new CM_PROCESSSCREENSAVER_MST();
            CPM = _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST.Where(x => x.SAVERID == id).SingleOrDefault();
            PAM.SAVERID = CPM.SAVERID;
            PAM.STATUS = Convert.ToInt16(CPM.STATUS);
            PAM.DEPARTNAME = CommonRepository.HtmlToText(CPM.DEPARTNAME);
            PAM.LASTREVISEDDATE = Convert.ToDateTime(CPM.LASTREVISEDDATE).ToString("dd/MM/yy");
            PAM.PoliciesFileName = CPM.FILE_NAME;
            PAM.PoliciesContentType = CPM.FILE_CONTENTTYPE;
            return PAM;
            //ScreenSaverViewModel data = (from m in _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST
            //                             where m.SAVERID == id
            //                       select new ScreenSaverViewModel
            //                       {
            //                           SAVERID = m.SAVERID,
            //                           DEPARTNAME = m.DEPARTNAME,
            //                           LASTREVISEDDATE = Convert.ToDateTime(m.LASTREVISEDDATE),
            //                           STATUS = Convert.ToInt16(m.STATUS),
            //                           PoliciesFileName = m.FILE_NAME,
            //                           PoliciesContentType = m.FILE_CONTENTTYPE
            //                        }).FirstOrDefault();
            //                            return data;
        }

        public FileViewModel GetFileForDownload(Int64 id, string type)
        {
            FileViewModel flvm = new FileViewModel();
            CM_PROCESSSCREENSAVER_MST CCPT;
            CCPT = _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST.Where(c => c.SAVERID == id).SingleOrDefault();
            if (CCPT != null)
            {
                if (type == "Policies")
                {
                    flvm.FileName = CCPT.FILE_NAME;
                    flvm.FileContentType = CCPT.FILE_CONTENTTYPE;
                }
            }

            return flvm;
        }

        public IEnumerable<ScreenViewModel> BindScreenSaver()
        {
            IEnumerable<ScreenViewModel> iList;
            iList = (from data in _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST.ToList().OrderBy(o => o.ADDEDON)
                     select new ScreenViewModel
                     {
                         SAVERID = data.SAVERID,
                         DepartmentName = data.DEPARTNAME,
                         LASTREVISEDDATE = data.LASTREVISEDDATE,
                         FILE_NAME = data.FILE_NAME,
                         FILE_CONTENTTYPE = data.FILE_CONTENTTYPE
                     });
            return iList;
        }

        //public short UpdateScreenSaver_Trn(ScreenSaverViewModel PAM)
        //{
        //    Int16 retVal = 0;
        //    using (DbContextTransaction transaction = _ScreenSaverDBContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            CM_PROCESSSCREENSAVER_MST CPT = new CM_PROCESSSCREENSAVER_MST();
        //            CPT = _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST.Find(PAM.SAVERID);
        //            CPT.SAVERID = PAM.SAVERID;
        //            CPT.LASTREVISEDDATE = Convert.ToDateTime(PAM.LASTREVISEDDATE);
        //            CPT.DEPARTNAME = CommonRepository.TextToHtml(PAM.DEPARTNAME);
        //            CPT.STATUS = PAM.STATUS;
        //            //CPT.STATUS = true;

        //            if (PAM.Policies != null)
        //            {
        //                CPT.FILE_NAME = PAM.PoliciesFileName;
        //                CPT.FILE_CONTENTTYPE = PAM.PoliciesContentType;
        //                //CPT.ATTACHMENT1 = PAM.ATTACHMENT1;
        //            }

        //            CPT.MODIFIED_ON = DateTime.Now;
        //            CPT.MODIFIED_BY = PAM.MODIFIED_BY;
        //            _ScreenSaverDBContext.Entry(CPT).State = EntityState.Modified;
        //            _ScreenSaverDBContext.SaveChanges();

        //            transaction.Commit();
        //            retVal = 1;
        //        }
        //        catch (Exception ex)
        //        {
        //            retVal = -1;
        //            transaction.Rollback();
        //        }
        //        return retVal;
        //    }
        //}
        public short UpdateScreenSaver_Trn(ScreenSaverViewModel mst)
        {
            short retVal = 0;
            using (var transaction = _ScreenSaverDBContext.Database.BeginTransaction())
            {
                try
                {
                    int FlagAdd = 0;
                    CM_PROCESSSCREENSAVER_MST CPT = new CM_PROCESSSCREENSAVER_MST();
                    CPT = _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST.Find(mst.SAVERID);
                    CPT.DEPARTNAME = mst.DEPARTNAME;
                    CPT.STATUS = mst.STATUS;
                    CPT.MODIFIED_BY = mst.MODIFIED_BY;
                    //Below Added by Aumento For SR66258-------------------------------
                    if (mst.FILE_NAME != null )
                    {
                    //-----------------------------------------------------------------
                        if (mst.FILE_NAME.FileName != "")
                        {
                            CPT.FILE_NAME = mst.PoliciesFileName;
                            CPT.FILE_CONTENTTYPE = mst.PoliciesContentType;
                        }

                    //Below Added by Aumento For SR66258-------------------------------
                    }
                    //-----------------------------------------------------------------

                    var state = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _ScreenSaverDBContext.Entry(CPT).State = state;


                    _ScreenSaverDBContext.Entry(CPT).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _ScreenSaverDBContext.SaveChanges();
                    transaction.Commit();
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                }
                return retVal;
            }
        }
        public List<ScreenSaverViewModel> ScreenSaver()
        {
            List<ScreenSaverViewModel> iList = new List<ScreenSaverViewModel>();
            var iColl = (from data in _ScreenSaverDBContext.CM_PROCESSSCREENSAVER_MST.Where(d => d.STATUS == 1)
                         select new
                         {
                             data.SAVERID,
                             data.DEPARTNAME,
                             data.LASTREVISEDDATE,
                             data.STATUS,                             
                             data.FILE_NAME
                         }).ToList();

            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {

                    iList.Add(new ScreenSaverViewModel
                    {
                        SAVERID = obj.SAVERID,
                        DEPARTNAME = obj.DEPARTNAME,
                        LASTREVISEDDATE = Convert.ToDateTime(obj.LASTREVISEDDATE).ToString("dd/MM/yy"),
                        STATUS = Convert.ToInt16(obj.STATUS),
                        //Below Added by Aumento For SR66258--------------------------------
                        STATUS1 = Convert.ToInt16(obj.STATUS) == 1 ? "Active" : "Deactive",
                        //------------------------------------------------------------------
                        PoliciesFileName = obj.FILE_NAME
                    });

                }
            }
            return iList;
        }
    }
}
