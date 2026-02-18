using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Infrastructure.Repositories
{
    public class ThirdpartyEmployeeRepository
    {
        private EPortalDBContext _DB;
        private decimal SYKI_;
        public ThirdpartyEmployeeRepository(EPortalDBContext objEPortalDBContext)
        {
            _DB = objEPortalDBContext;
            SYKI_ = _DB.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
        }



        public List<ThirdpartyEmployeeViewModel> GetThirdpartyEmployeeList(int LogonCode)
        {
            List<ThirdpartyEmployeeViewModel> ilist = new List<ThirdpartyEmployeeViewModel>();
            List<long> EmpCodeList = new List<long>();

            var LoginObj = _DB.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.ADEMPCODE == LogonCode && x.SYKI == SYKI_).FirstOrDefault();
            if (LoginObj != null)
            {
                if (LoginObj.ADFUNCTIONALDESIGNATIONID == 4)
                {
                    EmpCodeList = _DB.VW_ASSOCIATELVLDETAILS.Where(f => f.ADFUNCTIONALDESIGNATIONID == 3 && f.SYKI == SYKI_ && f.OPERATIONID == LoginObj.OPERATIONID).Select(x => x.ADEMPCODE).ToList();

                }
                if (LoginObj.ADFUNCTIONALDESIGNATIONID == 3)
                {

                    EmpCodeList = _DB.VW_ASSOCIATELVLDETAILS.Where(f => f.ADFUNCTIONALDESIGNATIONID == 2 && f.SYKI == SYKI_ && f.DIVISIONID == LoginObj.DIVISIONID).Select(x => x.ADEMPCODE).ToList();

                }

                EmpCodeList.Add(LogonCode);
            }

            ilist = (from t in _DB.TP_EMP_DETAIL
                     join j in _DB.VW_ASSOCIATELVLDETAILS on t.OWNERSHIP equals j.ADEMPCODE
                     where j.ACTIVE == 1 && j.SYKI == SYKI_
                     && (t.CREATEDBY == LogonCode || EmpCodeList.Contains(t.OWNERSHIP))
                     select new ThirdpartyEmployeeViewModel
                     {
                         ID = t.TPID,
                         Ecode = t.EMPCODE,
                         AssociateName = t.ASSOCIATENAME,
                         Designation = t.DESIGNATION,
                         CompanyName = t.COMPANYNAME,
                         AssociateCompanyId = t.ASSCOMPANYID,
                         ProjectManagerName = t.PROJECTMANAGERNAME,
                         ProjectManagerMailID = t.PROJECTMANAGERMAILID,
                         AccoountManagerName = t.ACCOUNTMANAGERNAME,
                         AccountManagerMailID = t.ACCOUNTMANAGERMAILID,
                         Operation = j.OPERATION,
                         Division = j.DIVISION,
                         Department = j.DEPARTMENT,
                         Section = j.SECTION,
                         Ownership = t.OWNERSHIP,
                         Is_Deboarded = t.IS_DEBOARDED
                     }
            ).ToList();

            return ilist.OrderBy(X => X.Ecode).ToList();
        }




        #region  AutoSuggestions

        public dynamic Getprojectmanagermailid(string projectManagerName, string term)
        {
            dynamic Res;

            try
            {
                var elist = (from data in _DB.TP_EMP_DETAIL
                             where (data.PROJECTMANAGERMAILID.ToString().ToUpper().Contains(term.ToUpper()) && data.PROJECTMANAGERNAME == projectManagerName)
                             select new
                             { data.PROJECTMANAGERMAILID }
                        ).Distinct().ToList();

                Res = elist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Res;
        }

        public dynamic GetAccountManagermailid(string AccountManagerName, string term)
        {
            dynamic Res;

            try
            {
                var elist = (from data in _DB.TP_EMP_DETAIL
                             where (data.ACCOUNTMANAGERMAILID.ToString().ToUpper().Contains(term.ToUpper()) && data.ACCOUNTMANAGERNAME == AccountManagerName)
                             select new
                             { data.ACCOUNTMANAGERMAILID }
                        ).Distinct().ToList();

                Res = elist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Res;
        }

        public dynamic GetCompanyName(string term)
        {
            dynamic Res;

            try
            {
                var elist = (from data in _DB.TP_EMP_DETAIL
                             where (data.COMPANYNAME.ToString().ToUpper().Contains(term.ToUpper()))
                             select new
                             { data.COMPANYNAME }
                        ).Distinct().ToList();

                Res = elist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Res;
        }

        public dynamic GetprojectmanagerName(string companyname, string projectmangername)
        {
            dynamic Res;

            try
            {
                var elist = (from data in _DB.TP_EMP_DETAIL
                             where (data.COMPANYNAME == companyname && data.PROJECTMANAGERNAME.ToString().ToUpper().Contains(projectmangername.ToUpper()))
                             select new
                             { data.PROJECTMANAGERNAME }
                        ).Distinct().ToList();

                Res = elist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Res;
        }

        public dynamic GetAccountmanagerName(string companyname, string Accountmanager)
        {
            dynamic Res;

            try
            {
                var elist = (from data in _DB.TP_EMP_DETAIL
                             where (data.COMPANYNAME == companyname && data.ACCOUNTMANAGERNAME.ToString().ToUpper().Contains(Accountmanager.ToUpper()))
                             select new
                             { data.ACCOUNTMANAGERNAME }
                        ).Distinct().ToList();

                Res = elist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Res;
        }

        public object GetEcode()
        {
            object res;

            long maxEcode = 0;
            try
            {
                var count = _DB.TP_EMP_DETAIL.Count().ToString();
                if (count == "0")
                {
                    maxEcode = 99000901;
                }
                else
                {
                    maxEcode = Int32.Parse(_DB.TP_EMP_DETAIL.Max(i => i.EMPCODE).ToString()) + 1;
                }
                res = maxEcode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        public LibResult GetOwnership(string term, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {

                var adFunDesi = _DB.VW_ASSOCIATELVLDETAILS.Where(f => f.ADEMPCODE == LoginCode && f.SYKI == SYKI_).Select(f => f.ADFUNCTIONALDESIGNATIONID).FirstOrDefault();

                if (adFunDesi == 4)
                {

                    var OperationID = _DB.VW_ASSOCIATELVLDETAILS.Where(f => f.ADEMPCODE == LoginCode && f.SYKI == SYKI_).Select(f => f.OPERATIONID).FirstOrDefault();

                    var data = (from k in _DB.VW_ASSOCIATELVLDETAILS
                                join v in _DB.ADEMPLOYEE on k.ADEMPCODE equals v.ADEMPCODE
                                where k.OPERATIONID == OperationID && (k.ADFUNCTIONALDESIGNATIONID == 3 || k.ADFUNCTIONALDESIGNATIONID == 2) && k.SYKI == SYKI_ && v.ACTIVE == 1 &&

                                 (v.ADEMPCODE.ToString().ToUpper().Contains(term.ToUpper())
                         || v.FIRSTNAME.ToUpper().Contains(term.ToUpper())
                         || v.LASTNAME.ToUpper().Contains(term.ToUpper()))

                                select new
                                { name = v.FIRSTNAME + " " + v.LASTNAME, ecode = v.ADEMPCODE }

                               ).ToList();

                    res.resultObject = data;
                }
                else if (adFunDesi == 3)
                {

                    var divisionId = _DB.VW_ASSOCIATELVLDETAILS.Where(f => f.ADEMPCODE == LoginCode && f.SYKI == SYKI_).Select(f => f.DIVISIONID)
                        .FirstOrDefault();

                    var data = (from k in _DB.VW_ASSOCIATELVLDETAILS
                                join v in _DB.ADEMPLOYEE on k.ADEMPCODE equals v.ADEMPCODE
                                where k.DIVISIONID == divisionId && k.ADFUNCTIONALDESIGNATIONID == 2 && k.SYKI == SYKI_ && v.ACTIVE == 1
                                &&

                                 (v.ADEMPCODE.ToString().ToUpper().Contains(term.ToUpper())
                         || v.FIRSTNAME.ToUpper().Contains(term.ToUpper())
                         || v.LASTNAME.ToUpper().Contains(term.ToUpper()))

                                select new
                                { name = v.FIRSTNAME + " " + v.LASTNAME, ecode = v.ADEMPCODE }

                               ).ToList();

                    res.resultObject = data;
                }
                else if (adFunDesi == 2)
                {

                    var departmentId = _DB.VW_ASSOCIATELVLDETAILS
                        .Where(f => f.ADEMPCODE == LoginCode && f.SYKI == SYKI_)
                        .Select(f => f.DEPARTMENTID)
                        .FirstOrDefault();

                    var data = (from k in _DB.VW_ASSOCIATELVLDETAILS
                                join v in _DB.ADEMPLOYEE on k.ADEMPCODE equals v.ADEMPCODE
                                where k.DEPARTMENTID == departmentId && k.ADFUNCTIONALDESIGNATIONID == 2 && k.SYKI == SYKI_ && v.ACTIVE == 1
                                &&

                                 (v.ADEMPCODE.ToString().ToUpper().Contains(term.ToUpper())
                         || v.FIRSTNAME.ToUpper().Contains(term.ToUpper())
                         || v.LASTNAME.ToUpper().Contains(term.ToUpper()))

                                select new
                                { name = v.FIRSTNAME + " " + v.LASTNAME, ecode = v.ADEMPCODE }

                               ).ToList();

                    res.resultObject = data;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return res;
        }

        public List<SelectOwnShipData> GetOwnershipList(int LoginCode)
        {
            List<SelectOwnShipData> res = new List<SelectOwnShipData>();
            try
            {
                var LogonEmp = (from k in _DB.VW_ASSOCIATELVLDETAILS
                                join v in _DB.ADEMPLOYEE on k.ADEMPCODE equals v.ADEMPCODE
                                where k.ADEMPCODE == LoginCode && k.SYKI == SYKI_ && v.ACTIVE == 1
                                select new SelectOwnShipData
                                { Text = v.FIRSTNAME + " " + v.LASTNAME, Value = v.ADEMPCODE }
                             ).ToList();


                var adFunDesi = _DB.VW_ASSOCIATELVLDETAILS.Where(f => f.ADEMPCODE == LoginCode && f.SYKI == SYKI_).Select(f => f.ADFUNCTIONALDESIGNATIONID).FirstOrDefault();

                if (adFunDesi == 4)
                {

                    var OperationID = _DB.VW_ASSOCIATELVLDETAILS.Where(f => f.ADEMPCODE == LoginCode && f.SYKI == SYKI_).Select(f => f.OPERATIONID).FirstOrDefault();

                    var data = (from k in _DB.VW_ASSOCIATELVLDETAILS
                                join v in _DB.ADEMPLOYEE on k.ADEMPCODE equals v.ADEMPCODE

                                where k.OPERATIONID == OperationID && (k.ADFUNCTIONALDESIGNATIONID == 3 || k.ADFUNCTIONALDESIGNATIONID == 2) && k.SYKI == SYKI_ && v.ACTIVE == 1

                                select new SelectOwnShipData
                                { Text = v.FIRSTNAME + " " + v.LASTNAME, Value = v.ADEMPCODE }
                               ).ToList();


                    res = data;
                }
                else if (adFunDesi == 3)
                {

                    var divisionId = _DB.VW_ASSOCIATELVLDETAILS.Where(f => f.ADEMPCODE == LoginCode && f.SYKI == SYKI_).Select(f => f.DIVISIONID)
                        .FirstOrDefault();

                    var data = (from k in _DB.VW_ASSOCIATELVLDETAILS
                                join v in _DB.ADEMPLOYEE on k.ADEMPCODE equals v.ADEMPCODE
                                where k.DIVISIONID == divisionId && k.ADFUNCTIONALDESIGNATIONID == 2 && k.SYKI == SYKI_ && v.ACTIVE == 1

                                select new SelectOwnShipData
                                { Text = v.FIRSTNAME + " " + v.LASTNAME, Value = v.ADEMPCODE }
                               ).ToList();

                    res = data;
                }
                else if (adFunDesi == 2)
                {

                    var departmentId = _DB.VW_ASSOCIATELVLDETAILS
                        .Where(f => f.ADEMPCODE == LoginCode && f.SYKI == SYKI_)
                        .Select(f => f.DEPARTMENTID)
                        .FirstOrDefault();

                    var data = (from k in _DB.VW_ASSOCIATELVLDETAILS
                                join v in _DB.ADEMPLOYEE on k.ADEMPCODE equals v.ADEMPCODE
                                where k.DEPARTMENTID == departmentId && k.ADFUNCTIONALDESIGNATIONID == 2 && k.SYKI == SYKI_ && v.ACTIVE == 1

                                select new SelectOwnShipData
                                { Text = v.FIRSTNAME + " " + v.LASTNAME, Value = v.ADEMPCODE }
                               ).ToList();

                    res = data;
                }


                res.AddRange(LogonEmp);

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return res;
        }

        #endregion

        public short AddThirdpartyEmployeeData(ThirdpartyEmployeeViewModel model_, string userID)
        {
            short retVal = 0;
            int FlagAdd = 0;
            int maxId = 0;

            using (var transaction = _DB.Database.BeginTransaction())
            {
                try
                {
                    var ctg = _DB.TP_EMP_DETAIL.Where(x => x.COMPANYNAME == model_.CompanyName && x.ASSCOMPANYID == model_.AssociateCompanyId).ToList();
                    if (ctg.Count() == 0)
                    {
                        var count = _DB.TP_EMP_DETAIL.Count().ToString();
                        if (count == "0")
                        {
                            maxId = 1;
                        }
                        else
                        {
                            maxId = Int32.Parse(_DB.TP_EMP_DETAIL.Max(i => i.TPID).ToString()) + 1;
                        }
                        FlagAdd = 1;


                        if (model_.Ecode != 0)
                        {

                            TP_EMP_DETAIL MST = new TP_EMP_DETAIL();

                            MST.TPID = maxId;
                            MST.EMPCODE = model_.Ecode??0;
                            MST.ASSOCIATENAME = model_.AssociateName;
                            MST.DESIGNATION = model_.Designation;
                            MST.COMPANYNAME = model_.CompanyName;
                            MST.ASSCOMPANYID = model_.AssociateCompanyId;
                            MST.PROJECTMANAGERNAME = model_.ProjectManagerName;
                            MST.PROJECTMANAGERMAILID = model_.ProjectManagerMailID;
                            MST.ACCOUNTMANAGERNAME = model_.AccoountManagerName;
                            MST.ACCOUNTMANAGERMAILID = model_.AccountManagerMailID;
                            MST.CREATEDBY = Int32.Parse(userID.ToString());
                            MST.CREATEDDATE = DateTime.Now;
                            MST.MODIFIEDBY = Int32.Parse(userID.ToString());
                            MST.MODIFIEDDATE = model_.Modified_Date;
                            MST.OWNERSHIP = model_.Ownership?? 0;
                            MST.IS_DEBOARDED = 0;
                            _DB.Entry(MST).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                            _DB.SaveChanges();

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

        // Added by TTL SR94104 - CR6022
        //public List<TP_EMP_DETAIL> BulkUploadEmployeeDetails(List<ThirdpartyEmpUploadModel> employeeDetails, string userID)
        //{
        //    try
        //    {
        //        // Get current max TPID
        //        var maxId = _DB.TP_EMP_DETAIL.Any()
        //                    ? _DB.TP_EMP_DETAIL.Max(i => i.TPID) + 1
        //                    : 1;

        //        //Get current max EMPCODE
        //        var maxEmpCode = _DB.TP_EMP_DETAIL.Any()
        //                    ? _DB.TP_EMP_DETAIL.Max(i => i.EMPCODE) + 1
        //                    : 1;

        //        // Map ViewModel to Entity and assign TPID sequentially
        //        var entities = employeeDetails.Select((e, index) => new TP_EMP_DETAIL
        //        {
        //            TPID = maxId + index,  // Assign incremental TPID
        //            EMPCODE = maxEmpCode + index,
        //            ASSOCIATENAME = e.AssociateName,
        //            DESIGNATION = e.Designation,
        //            COMPANYNAME = e.CompanyName,
        //            PROJECTMANAGERNAME = e.ProjectManagerName,
        //            PROJECTMANAGERMAILID = e.ProjectManagerMailID,
        //            ACCOUNTMANAGERNAME = e.AccoountManagerName,
        //            ACCOUNTMANAGERMAILID = e.AccountManagerMailID,
        //            ASSCOMPANYID = e.AssociateCompanyId,
        //            CREATEDBY = Int32.Parse(userID.ToString()),
        //            CREATEDDATE = DateTime.Now,
        //            MODIFIEDBY = Int32.Parse(userID.ToString()),
        //            MODIFIEDDATE = e.Modified_Date,
        //            OWNERSHIP = e.Ownership,
        //            IS_DEBOARDED = 0
        //        }).ToList();

        //        _DB.TP_EMP_DETAIL.AddRange(entities);
        //        _DB.SaveChangesAsync();

        //        return entities;  // Return updated list with TPID assigned
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<TP_EMP_DETAIL>();
        //    }
        //}

        public List<ThirdpartyEmpUploadModel> BulkUploadEmployeeDetails(List<ThirdpartyEmpUploadModel> employeeDetails, string userID)
        {
            try
            {
                // Get current max TPID
                var maxId = _DB.TP_EMP_DETAIL.Any() ? _DB.TP_EMP_DETAIL.Max(i => i.TPID) + 1 : 1;

                // Get current max EMPCODE
                var maxEmpCode = _DB.TP_EMP_DETAIL.Any() ? _DB.TP_EMP_DETAIL.Max(i => i.EMPCODE) + 1 : 1;

                // Map ViewModel to Entity and assign TPID sequentially
                var entities = employeeDetails.Select((e, index) => new TP_EMP_DETAIL
                {
                    TPID = maxId + index, // Assign incremental TPID
                    EMPCODE = maxEmpCode + index,
                    ASSOCIATENAME = e.AssociateName,
                    DESIGNATION = e.Designation,
                    COMPANYNAME = e.CompanyName,
                    PROJECTMANAGERNAME = e.ProjectManagerName,
                    PROJECTMANAGERMAILID = e.ProjectManagerMailID,
                    ACCOUNTMANAGERNAME = e.AccoountManagerName,
                    ACCOUNTMANAGERMAILID = e.AccountManagerMailID,
                    ASSCOMPANYID = e.AssociateCompanyId,
                    CREATEDBY = Int32.Parse(userID.ToString()),
                    CREATEDDATE = DateTime.Now,
                    MODIFIEDBY = Int32.Parse(userID.ToString()),
                    MODIFIEDDATE = e.Modified_Date,
                    OWNERSHIP = e.Ownership ?? 0,
                    IS_DEBOARDED = 0
                }).ToList();

                _DB.TP_EMP_DETAIL.AddRange(entities);
                _DB.SaveChangesAsync();

                // Join TP_EMP_DETAIL with ADEMPLOYEE table to get ownership details
                var result = from emp in entities
                             join owner in _DB.ADEMPLOYEE
                             on emp.OWNERSHIP equals owner.ADEMPCODE
                             where owner.ACTIVE == 1
                             select new ThirdpartyEmpUploadModel
                             {
                                 Ecode = emp.EMPCODE,
                                 AssociateName = emp.ASSOCIATENAME,
                                 Designation = emp.DESIGNATION,
                                 CompanyName = emp.COMPANYNAME,
                                 ProjectManagerName = emp.PROJECTMANAGERNAME,
                                 ProjectManagerMailID = emp.PROJECTMANAGERMAILID,
                                 AccoountManagerName = emp.ACCOUNTMANAGERNAME,
                                 AccountManagerMailID = emp.ACCOUNTMANAGERMAILID,
                                 AssociateCompanyId = emp.ASSCOMPANYID,
                                 Modified_Date = emp.MODIFIEDDATE,
                                 OwnerEmpCode = owner.ADEMPCODE.ToString(),
                                 OwnerEmpName = $"{owner.FIRSTNAME} {owner.LASTNAME}"
                             };

                return result.ToList();
            }
            catch (Exception ex)
            {
                return new List<ThirdpartyEmpUploadModel>();
            }
        }


        public long GetExistingEmpCode(string associateCompanyID, string companyName)
        {
            long empCode = _DB.TP_EMP_DETAIL
                            .Where(e => e.ASSCOMPANYID == associateCompanyID && e.COMPANYNAME == companyName)
                            .Select(e => e.EMPCODE).FirstOrDefault();
            return empCode;
        }

        public string GetValidEmployee(string eCode)
        {
            try
            {
                long empCode = Convert.ToInt64(eCode);
                var result = _DB.VW_ASSOCIATELVLDETAILS
                            .Join(_DB.ADEMPLOYEE,
                                  k => k.ADEMPCODE,
                                  v => v.ADEMPCODE,
                                  (k, v) => new { k, v })
                            .Where(x => x.k.ADEMPCODE == empCode && x.k.SYKI == SYKI_ && x.v.ACTIVE == 1)
                            .Select(x => x.v.FIRSTNAME + " " + x.v.LASTNAME).FirstOrDefault();

                return result == null ? "" : result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        // End by TTL SR94104 - CR6022

        public short EditThirdpartyempData(ThirdpartyEmployeeViewModel model_, string userID)
        {
            short retVal = 0;
            int FlagAdd = 2;

            using (var transaction = _DB.Database.BeginTransaction())
            {
                try
                {
                    var MST = _DB.TP_EMP_DETAIL.Where(x => x.EMPCODE == model_.Ecode).FirstOrDefault();
                    if (MST != null)
                    {
                        var ctg = _DB.TP_EMP_DETAIL.Where(x => x.COMPANYNAME == model_.CompanyName && x.ASSCOMPANYID == model_.AssociateCompanyId).ToList();

                        if (ctg.Count > 1)
                        {
                            return 0;
                        }
                        else
                        {


                            MST.ASSOCIATENAME = model_.AssociateName;
                            MST.DESIGNATION = model_.Designation;
                            MST.COMPANYNAME = model_.CompanyName;
                            MST.ASSCOMPANYID = model_.AssociateCompanyId;
                            MST.PROJECTMANAGERNAME = model_.ProjectManagerName;
                            MST.PROJECTMANAGERMAILID = model_.ProjectManagerMailID;
                            MST.ACCOUNTMANAGERNAME = model_.AccoountManagerName;
                            MST.ACCOUNTMANAGERMAILID = model_.AccountManagerMailID;
                            MST.MODIFIEDBY = Int32.Parse(userID.ToString());
                            MST.MODIFIEDDATE = DateTime.Now;


                            _DB.Entry(MST).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                            _DB.SaveChanges();

                            transaction.Commit();

                            retVal = 1;
                        }
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
        public short DeleteThirdpartyEmp(long ecode)
        {
            short retVal = 0;

            using (var transaction = _DB.Database.BeginTransaction())
            {
                try
                {
                    var MST = _DB.TP_EMP_DETAIL.Where(x => x.EMPCODE == ecode).FirstOrDefault();

                    if (MST != null)
                    {

                        _DB.TP_EMP_DETAIL.Remove(MST);
                        _DB.SaveChanges();
                        transaction.Commit();

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

        public short DeboardEmployee(long ecode, int LoginCode)
        {
            short retVal = 0;

            using (var transaction = _DB.Database.BeginTransaction())
            {
                try
                {
                    var MST = _DB.TP_EMP_DETAIL.Where(x => x.EMPCODE == ecode).FirstOrDefault();

                    if (MST != null)
                    {
                        MST.IS_DEBOARDED = 1;
                        MST.MODIFIEDBY = LoginCode;
                        MST.MODIFIEDDATE = DateTime.Now;
                        _DB.Entry(MST).State = EntityState.Modified;
                        _DB.SaveChanges();
                        transaction.Commit();

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


    }
}
