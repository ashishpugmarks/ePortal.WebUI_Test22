using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;

namespace ePortal.Infrastructure.Repositories
{
    public class VpfCreateRepositories
    {
        private readonly EPortalDBContext _pdDBContext;

        public VpfCreateRepositories(EPortalDBContext pdDBContext)
        {
            _pdDBContext = pdDBContext;
        }

        public VPF_CreateDetailViewModel SaveVPF_CreateDetail(VPF_CreateDetailViewModel VPFCModel)
        {
            List<VPF_CreateDetailViewModel> vpfDetails = new List<VPF_CreateDetailViewModel>();
            List<VPF_ENTRYDETAILS> vpdetails;
            DateTime Dt = Convert.ToDateTime(VPFCModel.DATEADDED);

            //long Ti = 2;

            //var DataExists = _pdDBContext.VPF_ENTRYDETAILS.Any(x => x.APPROVESTATUS == 2 && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE && x.STATUS != 0);
            var DataExists = _pdDBContext.VPF_ENTRYDETAILS.FirstOrDefault(x => x.APPROVESTATUS == 2 && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE && x.STATUS != 0) != null ? true:false;
            if (DataExists)
            {
                VPFCModel.ErrorMsg = "Request already in process";
                return VPFCModel;
            }
            if (VPFCModel.REQUESTTYPE == 1 && DataExists == false)
            {

                var DataExists1 = _pdDBContext.VPF_ENTRYDETAILS.Any(x => x.APPROVESTATUS == 1 && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE && Dt.Year == x.KI && x.STATUS != 0 && x.REQUESTTYPE == 1);

                //var DataExists2 = _pdDBContext.VPF_ENTRYDETAILS.Any(x => Dt.Year == x.KI && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE);

                if (DataExists1)
                {
                    VPFCModel.ErrorMsg = "Already Entered Request for this Year and Approved.";
                    return VPFCModel;
                }
                //else if (DataExists2)
                //{
                //    VPFCModel.ErrorMsg = "Already Entered Request for this Year.";
                //    return VPFCModel;
                //}
                else
                {
                    //

                    VPF_ENTRYDETAILS pVPF = new VPF_ENTRYDETAILS();
                    pVPF.EMPLOYEECODE = VPFCModel.EMPLOYEECODE;
                    pVPF.VPFCONTRIBUTION = VPFCModel.VPFCONTRIBUTION;
                    pVPF.DATEADDED = VPFCModel.DATEADDED;
                    pVPF.STATUS = VPFCModel.STATUS;
                    pVPF.EFFECTIVEDATE = DateTime.ParseExact(VPFCModel.EFFECTIVEDATE, "dd-MMM-yyyy", null);
                    pVPF.REQUESTTYPE = VPFCModel.REQUESTTYPE;
                    pVPF.STATUS = 1;
                    pVPF.APPROVESTATUS = 2;                                         // 2 is for Pending at IR for Approval

                    pVPF.KI = Dt.Year;

                    _pdDBContext.VPF_ENTRYDETAILS.Add(pVPF);
                    _pdDBContext.SaveChanges();

                    return VPFCModel;
                }
            }


            if (VPFCModel.REQUESTTYPE == 3)
            {

                //var DataExists2 = _pdDBContext.VPF_ENTRYDETAILS.Any(x => Dt.Year == x.KI && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE);
                //if (!DataExists2)
                //{
                //    VPFCModel.ErrorMsg = "You can't Stop because data doesn't Exist of This year. First Create VPF";
                //    return VPFCModel;
                //}
                //var DataExists3 = _pdDBContext.VPF_ENTRYDETAILS.Any(x => Dt.Year == x.KI && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE && x.REQUESTTYPE == 3 && x.APPROVESTATUS == 1 && x.STATUS != 0);
                var DataExists3 = _pdDBContext.VPF_ENTRYDETAILS.Any(x => Dt.Year == x.KI && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE && x.REQUESTTYPE == 3 && x.APPROVESTATUS == 1 && x.STATUS != 0) != null ? true : false;
                if (DataExists3)
                {
                    VPFCModel.ErrorMsg = "Request Already Stoped";
                    return VPFCModel;
                }
                else
                {
                    VPF_ENTRYDETAILS pVPF = new VPF_ENTRYDETAILS();
                    pVPF.EMPLOYEECODE = VPFCModel.EMPLOYEECODE;
                    pVPF.VPFCONTRIBUTION = 0;//VPFCModel.VPFCONTRIBUTION;
                    pVPF.DATEADDED = VPFCModel.DATEADDED;
                    pVPF.STATUS = VPFCModel.STATUS;
                    pVPF.EFFECTIVEDATE = DateTime.ParseExact(VPFCModel.EFFECTIVEDATE, "dd-MMM-yyyy", null);
                    pVPF.REQUESTTYPE = VPFCModel.REQUESTTYPE;
                    pVPF.STATUS = 1;
                    pVPF.APPROVESTATUS = 2;                                         // 2 is for Pending at IR for Approval

                    pVPF.KI = Dt.Year;

                    _pdDBContext.VPF_ENTRYDETAILS.Add(pVPF);
                    _pdDBContext.SaveChanges();

                    return VPFCModel;
                }
            }



            if (VPFCModel.REQUESTTYPE == 2)
            {

                //var DataExists2 = _pdDBContext.VPF_ENTRYDETAILS.Any(x => Dt.Year == x.KI && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE && x.APPROVESTATUS == 1);
                //if (!DataExists2)
                //{
                //    VPFCModel.ErrorMsg = "You can't Change because data doesn't Exist of This year. First Create VPF";
                //    return VPFCModel;
                //}

                //var DataExists4 = _pdDBContext.VPF_ENTRYDETAILS.Any(x => Dt.Year == x.KI && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE && x.REQUESTTYPE == 2 && x.APPROVESTATUS == 1 && x.STATUS != 0);
                var DataExists4 = _pdDBContext.VPF_ENTRYDETAILS.Any(x => Dt.Year == x.KI && x.EMPLOYEECODE == VPFCModel.EMPLOYEECODE && x.REQUESTTYPE == 2 && x.APPROVESTATUS == 1 && x.STATUS != 0) != null ? true : false;
                if (DataExists4)
                {
                    VPFCModel.ErrorMsg = "change of percentage in VPF deduction will be done only once in a year";
                    return VPFCModel;
                }
                else
                {
                    VPF_ENTRYDETAILS pVPF = new VPF_ENTRYDETAILS();
                    pVPF.EMPLOYEECODE = VPFCModel.EMPLOYEECODE;
                    pVPF.VPFCONTRIBUTION = VPFCModel.VPFCONTRIBUTION;
                    pVPF.DATEADDED = VPFCModel.DATEADDED;
                    pVPF.STATUS = VPFCModel.STATUS;
                    pVPF.EFFECTIVEDATE = DateTime.ParseExact(VPFCModel.EFFECTIVEDATE, "dd-MMM-yyyy", null);
                    pVPF.REQUESTTYPE = VPFCModel.REQUESTTYPE;
                    pVPF.STATUS = 1;
                    pVPF.APPROVESTATUS = 2;                                         // 2 is for Pending at IR for Approval

                    pVPF.KI = Dt.Year;

                    _pdDBContext.VPF_ENTRYDETAILS.Add(pVPF);
                    _pdDBContext.SaveChanges();

                    return VPFCModel;
                }

            }
            //else
            //{

            //    if (DataExists)
            //    {
            //        VPFCModel.ErrorMsg = "Already Pending For Approval";
            //        return VPFCModel;
            //    }
            //    else if (DataExists1)
            //    {
            //        VPFCModel.ErrorMsg = "Already Entered Request for this Year and Approved.";
            //        return VPFCModel;
            //    }
            //    else if (DataExists2)
            //    {
            //        VPFCModel.ErrorMsg = "Already Entered Request for this Year.";
            //        return VPFCModel;
            //    }
            //    else
            //    {
            //        //

            //        VPF_ENTRYDETAILS pVPF = new VPF_ENTRYDETAILS();
            //        pVPF.EMPLOYEECODE = VPFCModel.EMPLOYEECODE;
            //        pVPF.VPFCONTRIBUTION = VPFCModel.VPFCONTRIBUTION;
            //        pVPF.DATEADDED = VPFCModel.DATEADDED;
            //        pVPF.STATUS = VPFCModel.STATUS;
            //        pVPF.EFFECTIVEDATE = VPFCModel.EFFECTIVEDATE;
            //        pVPF.REQUESTTYPE = VPFCModel.REQUESTTYPE;
            //        pVPF.STATUS = 1;
            //        pVPF.APPROVESTATUS = 2;                                         // 2 is for Pending at IR for Approval

            //        pVPF.KI = Dt.Year;

            //        _pdDBContext.VPF_ENTRYDETAILS.Add(pVPF);
            //        _pdDBContext.SaveChanges();

            //        return VPFCModel;
            //    }
            //}
            return VPFCModel;
        }

        public VPF_CreateDetailViewModel CancelVPF_Detail(VPF_CreateDetailViewModel VPFCModel)
        {
            VPF_ENTRYDETAILS pVPF = new VPF_ENTRYDETAILS();
            var VPFDetail = _pdDBContext.VPF_ENTRYDETAILS.Where(v => v.ID == VPFCModel.ID).FirstOrDefault();

            if (VPFDetail != null)
            {
                VPFDetail.STATUS = 0;
                VPFDetail.MODIFIEDBY = VPFCModel.MODIFIEDBY;
                VPFDetail.MODIFIEDDATE = VPFCModel.MODIFIEDDATE;
                VPFDetail.CANCELREMARKS = VPFCModel.CANCELREMARKS;
                _pdDBContext.SaveChanges();
            }
            return VPFCModel;


        }

        public VPF_CreateDetailViewModel UpdateVPF_Detail(VPF_CreateDetailViewModel VPFCModel)
        {
            VPF_ENTRYDETAILS pVPF = new VPF_ENTRYDETAILS();
            var VPFDetail = _pdDBContext.VPF_ENTRYDETAILS.Where(v => v.ID == VPFCModel.ID).FirstOrDefault();

            if (VPFDetail != null)
            {
                //VPFDetail.EMPLOYEECODE = 4875;
                VPFDetail.VPFCONTRIBUTION = VPFCModel.VPFCONTRIBUTION;
                VPFDetail.MODIFIEDBY = VPFCModel.MODIFIEDBY;
                VPFDetail.MODIFIEDDATE = VPFCModel.MODIFIEDDATE;
                //VPFDetail.STATUS = VPFCModel.STATUS;
                VPFDetail.EFFECTIVEDATE = DateTime.ParseExact(VPFCModel.EFFECTIVEDATE, "dd-MMM-yyyy", null);
                VPFDetail.REQUESTTYPE = VPFCModel.REQUESTTYPE;

                _pdDBContext.SaveChanges();
            }
            return VPFCModel;


        }

        public VPF_CreateDetailViewModel ApproveVPF_Detail(VPF_CreateDetailViewModel VPFCModel)
        {
            VPF_ENTRYDETAILS pVPF = new VPF_ENTRYDETAILS();
            var VPFDetail = _pdDBContext.VPF_ENTRYDETAILS.Where(v => v.ID == VPFCModel.ID).FirstOrDefault();

            if (VPFDetail != null)
            {
                //VPFDetail.APPROVALAUTHID = 
                VPFDetail.APPROVESTATUS = VPFCModel.APPROVESTATUS;
                VPFDetail.APPROVALAUTHID = VPFCModel.APPROVALAUTHID;
                VPFDetail.APPROVALDATE = VPFCModel.APPROVALDATE;
                //VPFDetail.STATUS = VPFCModel.STATUS;
                VPFDetail.APPROVALREMARKS = VPFCModel.APPROVALREMARKS;
                VPFDetail.APPROVESTATUS = VPFCModel.APPROVESTATUS;

                _pdDBContext.SaveChanges();
            }
            return VPFCModel;
        }

        public List<VPF_CreateDetailViewModel> IRVPFDetail()
        {

            List<VPF_CreateDetailViewModel> vpfDetails = new List<VPF_CreateDetailViewModel>();
            //List<VPF_ENTRYDETAILS> vpdetails;
            //int EmpId = ;
            //vpdetails = _pdDBContext.VPF_ENTRYDETAILS.Where(x => x.STATUS == 1).ToList();
            //vpdetails = _pdDBContext.VPF_ENTRYDETAILS.Where(v => v.ID == id).FirstOrDefault();

            var q = (from pd in _pdDBContext.VPF_ENTRYDETAILS
                     join od in _pdDBContext.ADLOGINUSER on pd.EMPLOYEECODE equals od.ADEMPCODE
                     where pd.STATUS == 1
                     //into tempdsg
                     //from dsg in tempdsg.DefaultIfEmpty()
                     select new
                     {
                         pd.CANCELREMARKS,
                         pd.APPROVALAUTHID,
                         pd.APPROVALDATE,
                         pd.APPROVALREMARKS,
                         pd.APPROVESTATUS,
                         pd.DATEADDED,
                         pd.REQUESTTYPE,
                         pd.VPFCONTRIBUTION,
                         pd.ID,
                         pd.STATUS,
                         pd.EFFECTIVEDATE,
                         pd.EMPLOYEECODE,
                         od.FIRSTNAME,
                         od.LASTNAME
                     }).ToList();


            //var partialResult = (from c in _pdDBContext.VPF_ENTRYDETAILS
            //                     join o in _pdDBContext.ADLOGINUSER

            //                     on c.EMPLOYEECODE equals o.ADEMPCODE
            //                     select new
            //                     {
            //                         c.ADDEDBY,
            //                         c.APPROVALAUTHID,
            //                         c.APPROVALDATE,
            //                         c.APPROVALREMARKS,
            //                         c.APPROVESTATUS,
            //                         c.DATEADDED,
            //                         c.EFFECTIVEDATE,
            //                         c.EMPLOYEECODE,
            //                         o.FIRSTNAME
            //                     });

            //var finalResult = from c in db.Customers
            //                  orderby c.name
            //                  select new
            //                  {
            //                      name = c.name,
            //                      list = (from r in partialResult where c.name == r.name select r.order_total).ToList()

            //                  };


            //var dd = (from lu in _pdDBContext.VPF_ENTRYDETAILS
            //          join emp in _pdDBContext.ADLOGINUSER on lu.EMPLOYEECODE equals emp.ADEMPCODE into tempemp
            //          from empw in tempemp.DefaultIfEmpty()
            //          where((d => d.ADEMPCODE == userid))
            //          select new
            //          {
            //              lu.

            //          }

            //                                  ).ToList();

            foreach (var VPFD in q)
            {
                VPF_CreateDetailViewModel tt = new VPF_CreateDetailViewModel();

                tt.EMPLOYEECODE = VPFD.EMPLOYEECODE;
                tt.APPROVALAUTHID = VPFD.APPROVALAUTHID;
                tt.APPROVALDATE = VPFD.APPROVALDATE;
                tt.EFFECTIVEDATE = string.Format("{0:dd-MMM-yyyy}", VPFD.EFFECTIVEDATE);
                tt.REQUESTTYPE = VPFD.REQUESTTYPE;
                tt.VPFCONTRIBUTION = VPFD.VPFCONTRIBUTION;
                tt.FIRSTNAME = VPFD.FIRSTNAME;
                tt.LASTNAME = VPFD.LASTNAME;
                tt.ID = VPFD.ID;
                tt.CANCELREMARKS = VPFD.CANCELREMARKS;
                tt.STATUS = VPFD.STATUS;
                tt.APPROVALREMARKS = VPFD.APPROVALREMARKS;
                tt.APPROVESTATUS = VPFD.APPROVESTATUS;
                vpfDetails.Add(tt);

            }
            return vpfDetails;
        }

        public List<VPF_CreateDetailViewModel> GetVPFDetail(long id)
        {

            List<VPF_CreateDetailViewModel> vpfDetails = new List<VPF_CreateDetailViewModel>();
            List<VPF_ENTRYDETAILS> vpdetails;
            vpdetails = _pdDBContext.VPF_ENTRYDETAILS.Where(v => v.EMPLOYEECODE == id).ToList();
            //vpdetails = _pdDBContext.VPF_ENTRYDETAILS.ToList();
            foreach (var VPFD in vpdetails)
            {
                VPF_CreateDetailViewModel tt = new VPF_CreateDetailViewModel();
                tt.EMPLOYEECODE = VPFD.EMPLOYEECODE;
                tt.APPROVALAUTHID = VPFD.APPROVALAUTHID;
                tt.APPROVALDATE = VPFD.APPROVALDATE;
                tt.EFFECTIVEDATE = string.Format("{0:dd-MMM-yyyy}", VPFD.EFFECTIVEDATE);
                tt.REQUESTTYPE = VPFD.REQUESTTYPE;
                tt.VPFCONTRIBUTION = VPFD.VPFCONTRIBUTION;
                tt.ID = VPFD.ID;
                tt.CANCELREMARKS = VPFD.CANCELREMARKS;
                tt.STATUS = VPFD.STATUS;
                tt.APPROVALREMARKS = VPFD.APPROVALREMARKS;
                tt.APPROVESTATUS = VPFD.APPROVESTATUS;
                vpfDetails.Add(tt);

            }
            return vpfDetails;
        }

        //public VPF_CreateDetailViewModel CheckVPF_Detail(long id)
        //{
        //    VPF_ENTRYDETAILS vpdetails;
        //    vpdetails = _pdDBContext.VPF_ENTRYDETAILS.Where(v => (v.APPROVESTATUS == 2) ).FirstOrDefault();

        //    List<VPF_CreateDetailViewModel> vpfDetails = new List<VPF_CreateDetailViewModel>();
        //    //List<VPF_ENTRYDETAILS> vpdetails;
        //    //int EmpId = ;
        //    //vpdetails = _pdDBContext.VPF_ENTRYDETAILS.Where(x => x.STATUS == 1).ToList();

        //    VPF_CreateDetailViewModel tt = new VPF_CreateDetailViewModel();
        //    tt.APPROVALAUTHID = vpdetails.APPROVALAUTHID;
        //    tt.APPROVALDATE = vpdetails.APPROVALDATE;
        //    tt.EFFECTIVEDATE = vpdetails.EFFECTIVEDATE;
        //    tt.REQUESTTYPE = vpdetails.REQUESTTYPE;
        //    tt.VPFCONTRIBUTION = vpdetails.VPFCONTRIBUTION;
        //    //tt.ID = VPFD.ID;                  
        //    return tt;
        //}

        public VPF_CreateDetailViewModel Get_Data_By_UserId(long id)
        {
            VPF_ENTRYDETAILS vpdetails;
            VPF_CreateDetailViewModel tt = new VPF_CreateDetailViewModel();

            DateTime Dt = DateTime.Now;
            vpdetails = _pdDBContext.VPF_ENTRYDETAILS.Where(v => v.EMPLOYEECODE == id && v.KI == Dt.Year).OrderByDescending(k => k.ID).FirstOrDefault();
            if (vpdetails == null)
            {
                return tt;
            }
            else
            {
                tt.APPROVALAUTHID = vpdetails.APPROVALAUTHID;
                tt.APPROVALDATE = vpdetails.APPROVALDATE;
                tt.EFFECTIVEDATE = string.Format("{0:dd-MMM-yyyy}", vpdetails.EFFECTIVEDATE);
                tt.REQUESTTYPE = vpdetails.REQUESTTYPE;
                tt.VPFCONTRIBUTION = vpdetails.VPFCONTRIBUTION;
                //tt.ID = VPFD.ID;                  
                return tt;
            }
        }

        public VPF_CreateDetailViewModel EditVPF_Detail(long id)
        {
            VPF_ENTRYDETAILS vpdetails;
            vpdetails = _pdDBContext.VPF_ENTRYDETAILS.Where(v => v.ID == id).FirstOrDefault();
            VPF_CreateDetailViewModel tt = new VPF_CreateDetailViewModel();
            tt.EMPLOYEECODE = vpdetails.EMPLOYEECODE;
            tt.APPROVALAUTHID = vpdetails.APPROVALAUTHID;
            tt.APPROVALDATE = vpdetails.APPROVALDATE;
            tt.EFFECTIVEDATE = string.Format("{0:dd-MMM-yyyy}", vpdetails.EFFECTIVEDATE);
            tt.REQUESTTYPE = vpdetails.REQUESTTYPE;
            tt.VPFCONTRIBUTION = vpdetails.VPFCONTRIBUTION;
            //tt.ID = VPFD.ID;                  
            return tt;
        }
        public VPF_CreateDetailViewModel Edit_ApproveVPF_Detail(long id)
        {
            VPF_ENTRYDETAILS vpdetails;
            vpdetails = _pdDBContext.VPF_ENTRYDETAILS.Where(v => v.ID == id).FirstOrDefault();

            VPF_CreateDetailViewModel tt = new VPF_CreateDetailViewModel();
            tt.APPROVALAUTHID = vpdetails.APPROVALAUTHID;
            tt.APPROVALDATE = vpdetails.APPROVALDATE;
            tt.EFFECTIVEDATE = string.Format("{0:dd-MMM-yyyy}", vpdetails.EFFECTIVEDATE);
            tt.REQUESTTYPE = vpdetails.REQUESTTYPE;
            tt.VPFCONTRIBUTION = vpdetails.VPFCONTRIBUTION;
            //tt.ID = VPFD.ID;                  
            return tt;
        }
    }
}
