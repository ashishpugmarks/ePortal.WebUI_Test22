using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class UARRepository
    {
        //private ePortalEntities2 _UARDBContext;
        private EPortalDBContext _UARDBContext;
        private SYKI _Syki;
        //private ADORGLEVELHEAD _op;
        public UARRepository(EPortalDBContext objUARDBContext)
        {
            //_UARDBContext = new ePortalEntities2();
            _UARDBContext = objUARDBContext;
            _Syki = _UARDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public List<ADUAR_TRANS> UARRequestList()
        {
            List<ADUAR_TRANS> empdata_ans = new List<ADUAR_TRANS>();
            return empdata_ans;
        }

        public bool GetEmailandName(long Ecode, out string Email, out string Ename)
        {
            var data = _UARDBContext.ADLOGINUSER.FirstOrDefault(x => x.ADEMPCODE == Ecode);

            if (data != null)
            {
                Email = data.EMAILID;
                //Ename = data.FIRSTNAME;
                Ename = data.FIRSTNAME + " " + data.LASTNAME; //SR92683
                return true;
            }
            Email = null;
            Ename = null;
            return false;
        }

        public List<UAR_AccessReviewViewModel> GetEmpNameList(long EmpCode)
        {
            var iList = (from data in _UARDBContext.ADLOGINUSER.Where(m => m.ACTIVE == 1 && m.ADEMPCODE == EmpCode)
                         select new UAR_AccessReviewViewModel
                         {
                             EMPLOYEE = data.FIRSTNAME + " " + data.LASTNAME,
                         }).ToList();

            return iList;
        }

        public List<UAR_ViewModel> ReviewListData()
        {
            List<UAR_ViewModel> data = new List<UAR_ViewModel>();
            return data;
        }

        public string UpdateADUARRequest(long AdEmpCode)
        {
            bool Success = false;

            try
            {
                var i = _UARDBContext.ADUAR_TRANS.Where(c => c.ECODE == AdEmpCode && c.STATUS == 0).SingleOrDefault();
                i.STATUS = 1;
                i.REVIEWED_ON = DateTime.Now; //Added for SR78412
                _UARDBContext.Entry(i).State = EntityState.Modified;
                _UARDBContext.SaveChanges();

                Success = true;

            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
            return Success.ToString(); ;
        }

        public string GetSYKIID()
        {
            try
            {
                var data = _UARDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(y => y.KICODE).FirstOrDefault().ToString();
                return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //SR78412 Changes Start
        public string UpdateADUARRegularRequest(long AdEmpCode)
        {
            bool Success = false;

            try
            {
                var i = _UARDBContext.ADUAR_TRANS.Where(c => c.ECODE == AdEmpCode && c.STATUS == 2).FirstOrDefault();
                i.STATUS = 0;
                i.SELF_REVIEWED_ON = DateTime.Now;
                _UARDBContext.Entry(i).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                _UARDBContext.SaveChanges();

                Success = true;

            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
            return Success.ToString(); ;
        }
        public string SendBackADUARRequest(long AdEmpCode, long REVIEWER)
        {
            bool Success = false;

            try
            {
                var i = _UARDBContext.ADUAR_TRANS.Where(c => c.ECODE == AdEmpCode && c.STATUS == 0 && c.SELECTED_REVIEWER == REVIEWER).SingleOrDefault();

                i.STATUS = 2;
                _UARDBContext.Entry(i).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                _UARDBContext.SaveChanges();

                Success = true;

            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
            return Success.ToString(); ;
        }
        public List<SYKI> GetKICodeList(long userId)
        {
            List<decimal> syki = _UARDBContext.VW_ASSOCIATELVLDETAILS.Where(a => a.ADEMPCODE == userId).Select(x => (decimal)x.SYKI).ToList();
            var iList = (from tmp in syki
                         join b in _UARDBContext.SYKI on tmp equals b.SYKIID
                         select new SYKI
                         {
                             SYKIID = b.SYKIID,
                             KICODE = b.KICODE,
                         }).ToList();
            return iList.OrderByDescending(o => o.SYKIID).ToList();
        }
        //SR78412 Changes End
    }
}
