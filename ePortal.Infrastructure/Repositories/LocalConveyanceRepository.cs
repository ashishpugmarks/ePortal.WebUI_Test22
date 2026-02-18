using System.Data;
using System.Dynamic;
using System.Text.RegularExpressions;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Infrastructure.Repositories;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using Spire.Xls;

namespace ePortal.Infrastructure.Repositories
{
    public class LocalConveyanceRepository
    {

        public static class DefaultValues
        {
            public const int MaxSrnoHead = 1;
            public const int MaxLcTranId = 2001;
            public const int MaxSrnoDt = 1;
        }


        private LCModelDBContext _DB;
        private decimal _Syki;
        private readonly IEportalESS _iEportalESS;
        private readonly CommonRepository _cmr;
        private readonly ICommonFunctions _iCmmnFunction;
        


        public LocalConveyanceRepository(LCModelDBContext DB, IConfiguration configuration, IEportalESS iEportalESS, CommonRepository cmr, ICommonFunctions iCmmnFunction)
        {
            _DB = DB;
            _iEportalESS = iEportalESS;
            _cmr = cmr;
            _Syki = _DB.SYKI_LC.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            _iCmmnFunction = iCmmnFunction;
        }



        #region local Coveyance Master

        #region  LC_ZTABLE MASTER

        public List<LC_ZTABLE> GetLC_ZTABLEList()
        {
            List<LC_ZTABLE> ilist = new List<LC_ZTABLE>();

            ilist = _DB.LC_ZTABLE.ToList();


            return ilist;


        }

        public LibResult InsertDataToLC_ZTABLE(decimal FourWheelerRate, decimal TwoWheelerRate, DateTime FromDate, DateTime ToDate, int LoginCode, long SiteID) //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026
        {
            LibResult res = new LibResult();
            try
            {

                //using (var db = new LCEntities())
                //using (var db = _DB)
                //{

                var SiteDescript = _DB.SYSITE_LC.Where(x => x.SYSITEID == SiteID).Select(x => x.DESCRIP).FirstOrDefault().ToString(); // Added by Aumento as on 01082026

                var Exists = _DB.LC_ZTABLE.Where(x => x.SYSITEID == SiteID // Added by Aumento as on 01082026
                                                    && x.FOURWHEELER_RATE == FourWheelerRate
                                                    && x.TWOWHEELER_RATE == TwoWheelerRate
                                                    && x.FROMDATE == FromDate
                                                    && x.TODATE == ToDate
                                                   ).Count();


                if (Exists > 0)
                {
                    res.hasError = true;
                    res.errorMessage = "Record Already Exists";
                }
                else
                {
                    //var Srno = db.LC_ZTABLE.Select(x => x.SRNO).DefaultIfEmpty(0).Max();


                    var Srno = _DB.LC_ZTABLE
                     .Select(x => x.SRNO)
                     .AsEnumerable() // Switch to client-side
                        .DefaultIfEmpty(0)
                     .Max();



                    LC_ZTABLE crmst = new LC_ZTABLE();

                    crmst.SRNO = Srno + 1;
                    crmst.SYSITENAME = SiteDescript; // Added by Aumento as on 01082026
                    crmst.SYSITEID = SiteID;         // Added by Aumento as on 01082026
                    crmst.STATE = "";                // Added by Aumento as on 01082026
                    crmst.STATEID = 0;               // Added by Aumento as on 01082026
                    crmst.FOURWHEELER_RATE = FourWheelerRate;
                    crmst.TWOWHEELER_RATE = TwoWheelerRate;
                    crmst.FROMDATE = FromDate;
                    crmst.TODATE = ToDate;

                    crmst.ADDEDBY = LoginCode;
                    crmst.ADDEDON = DateTime.Now;
                    crmst.ACTIVE = 1; // SR107643 :: LC ENHANCEMENT ::  AUMENTO

                    _DB.Entry(crmst).State = EntityState.Added;
                    _DB.SaveChanges();
                    var flag = InsertLC_ZTABLE_LOG(crmst.FOURWHEELER_RATE, crmst.TWOWHEELER_RATE, crmst.FROMDATE, crmst.TODATE, int.Parse(crmst.ADDEDBY.ToString()), crmst.ADDEDON, null, null, crmst.SYSITEID, crmst.SYSITENAME, "ADD", int.Parse(crmst.ACTIVE.ToString()), crmst.SRNO);// SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    res.hasError = false;
                    res.errorMessage = "Record Inserted successfully";

                }

                //}
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public LC_ZTABLE GetEditDataforLC_ZTABLE(int id)
        {
            //long SRNo = Convert.ToInt64(id);
            return _DB.LC_ZTABLE.Find(Convert.ToInt64(id));
        }

        public bool GetDeleteDataforLC_ZTABLE(int id, int loginCode)// {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {
            bool res = false;
            LC_ZTABLE CrROle = _DB.LC_ZTABLE.Find(Convert.ToInt64(id));
            // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
            //_DB.LC_ZTABLE.Remove(CrROle);
            CrROle.UPDATEDBY = Convert.ToInt64(loginCode);
            CrROle.UPDATEDON = DateTime.Now;
            CrROle.ACTIVE = CrROle.ACTIVE == 0 ? (short)1 : (short)0;
            _DB.Entry(CrROle).State = EntityState.Modified;
            _DB.SaveChanges();
            var flag = InsertLC_ZTABLE_LOG(CrROle.FOURWHEELER_RATE, CrROle.TWOWHEELER_RATE, CrROle.FROMDATE, CrROle.TODATE, int.Parse(CrROle.ADDEDBY.ToString()), CrROle.ADDEDON, int.Parse(CrROle.UPDATEDBY.ToString()), CrROle.UPDATEDON, CrROle.SYSITEID, CrROle.SYSITENAME, "UPDATED", int.Parse(CrROle.ACTIVE.ToString()), CrROle.SRNO);
            // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
            res = true;
            return res;
        }

        public LibResult EditDataForLC_ZTABLE(int SrNo, decimal FourWheelerRate, decimal TwoWheelerRate, DateTime FromDate, DateTime ToDate, int LoginCode, long SiteID) //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026
        {
            LibResult Res = new LibResult();
            try
            {

                //using (var db = new LCEntities())
                //using (var db = _DB)
                //{
                var SiteDescript = _DB.SYSITE_LC.Where(x => x.SYSITEID == SiteID).Select(x => x.DESCRIP).FirstOrDefault().ToString(); // Added by Aumento as on 01082026

                var Exists = _DB.LC_ZTABLE.Where(x => x.SYSITEID == SiteID // Added by Aumento as on 01082026
                                                    && x.FOURWHEELER_RATE == FourWheelerRate
                                                    && x.TWOWHEELER_RATE == TwoWheelerRate
                                                    && x.FROMDATE == FromDate
                                                    && x.TODATE == ToDate
                                                   ).Count();


                if (Exists > 1)
                {
                    Res.hasError = true;
                    Res.errorMessage = "Record Already Exists Please check and Update Another Employee";
                }
                else
                {


                    LC_ZTABLE crmst = _DB.LC_ZTABLE.Find(Convert.ToInt64(SrNo));

                    crmst.SYSITENAME = SiteDescript; // Added by Aumento as on 01082026
                    crmst.SYSITEID = SiteID;         // Added by Aumento as on 01082026
                    crmst.STATE = "";                // Added by Aumento as on 01082026
                    crmst.STATEID = 0;               // Added by Aumento as on 01082026
                    crmst.FOURWHEELER_RATE = FourWheelerRate;
                    crmst.TWOWHEELER_RATE = TwoWheelerRate;
                    crmst.FROMDATE = FromDate;
                    crmst.TODATE = ToDate;

                    crmst.UPDATEDBY = LoginCode;
                    crmst.UPDATEDON = DateTime.Now;


                    _DB.Entry(crmst).State = EntityState.Modified;
                    _DB.SaveChanges();
                    var flag = InsertLC_ZTABLE_LOG(FourWheelerRate, TwoWheelerRate, FromDate, ToDate, int.Parse(crmst.ADDEDBY.ToString()), DateTime.Parse(crmst.ADDEDON.ToString()), int.Parse(crmst.UPDATEDBY.ToString()), DateTime.Parse(crmst.UPDATEDON.ToString()), crmst.SYSITEID, crmst.SYSITENAME, "UPDATED", int.Parse(crmst.ACTIVE.ToString()), crmst.SRNO);// SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    Res.hasError = false;
                    Res.errorMessage = "Record Updated successfully";

                }
                //}
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.ToString();

            }
            return Res;
        }

        public List<object> GetSatelist()
        {
            List<object> cityList = new List<object>();
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = serverpath.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_OFFOUTDUTYTRANS.SPROC_LC_GETSTATE";
                    objCmd.Parameters.Add("CUR_OUTDUTY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;

                    OracleDataReader reader = objCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var city = new
                        {
                            PSTATE = reader["PSTATE"]

                        };

                        cityList.Add(city);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }

            return cityList;
        }
        #endregion

        #region LC_EXCEPTION MASTER C TABLE
        public List<LC_EXCEPTION_C_TABLE> GetLC_EXCEPTIONList()
        {
            List<LC_EXCEPTION_C_TABLE> ilist = new List<LC_EXCEPTION_C_TABLE>();

            ilist = _DB.LC_EXCEPTION_C_TABLE.ToList();


            return ilist;


        }

        public LibResult InsertDataIntoLC_EXCEPTION(int EmpCode, decimal FuelReimbursementPerLtr, DateTime FromDate, DateTime ToDate, int LoginCode, string Remarks)
        {
            LibResult res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //using (var db = _DB)
                //{
                var Exists = _DB.LC_EXCEPTION_C_TABLE.Where(x => x.EMPCODE == EmpCode
                                                    && x.FUEL_REIMBUR_PER_LTR == FuelReimbursementPerLtr
                                                    && x.FROMDATE == FromDate
                                                    && x.TODATE == ToDate
                                                   ).Count();


                if (Exists > 0)
                {
                    res.hasError = true;
                    res.errorMessage = "Record Already Exists";
                }
                else
                {
                    var Srno = _DB.LC_EXCEPTION_C_TABLE.Select(x => x.SRNO).AsEnumerable().DefaultIfEmpty(0).Max();


                    var EmpName = (from r in _DB.ADEMPLOYEE_LC where r.ACTIVE == 1 && r.ADEMPCODE == EmpCode select r.FIRSTNAME + " " + r.LASTNAME).FirstOrDefault().ToString();
                    LC_EXCEPTION_C_TABLE crmst = new LC_EXCEPTION_C_TABLE();

                    crmst.SRNO = Srno + 1;
                    crmst.EMPCODE = EmpCode;
                    crmst.EMPNAME = EmpName;
                    crmst.FUEL_REIMBUR_PER_LTR = FuelReimbursementPerLtr;
                    crmst.FROMDATE = FromDate;
                    crmst.TODATE = ToDate;
                    crmst.REMARKS = Remarks;
                    crmst.ADDEDBY = LoginCode;
                    crmst.ADDEDON = DateTime.Now;
                    crmst.ACTIVE = 1; // SR107643 :: LC ENHANCEMENT ::  AUMENTO

                    _DB.Entry(crmst).State = EntityState.Added;
                    _DB.SaveChanges();
                    bool flag = InsertLC_EXCEPTION_C_TABLE_LOG(crmst.EMPCODE, crmst.EMPNAME, crmst.FUEL_REIMBUR_PER_LTR, crmst.FROMDATE, crmst.TODATE, crmst.REMARKS, crmst.ADDEDBY, crmst.ADDEDON, crmst.UPDATEDBY, crmst.UPDATEDON, int.Parse(crmst.ACTIVE.ToString()), "ADD", crmst.SRNO); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    res.hasError = false;
                    res.errorMessage = "Record Inserted successfully";

                }

                //}
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public LC_EXCEPTION_C_TABLE GetEditDataForLC_EXCEPTION(int id)
        {
            return _DB.LC_EXCEPTION_C_TABLE.Find(Convert.ToInt64(id));
        }

        public bool GetDeleteDataForLC_EXCEPTION(int id, long loginCode) 	// {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {
            bool res = false;
            LC_EXCEPTION_C_TABLE CrROle = _DB.LC_EXCEPTION_C_TABLE.Find(Convert.ToInt64(id));
            // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
            //_DB.LC_EXCEPTION_C_TABLE.Remove(CrROle);
            CrROle.UPDATEDBY = Convert.ToInt64(loginCode);
            CrROle.UPDATEDON = DateTime.Now;
            CrROle.ACTIVE = CrROle.ACTIVE == 0 ? (short)1 : (short)0;
            _DB.Entry(CrROle).State = EntityState.Modified;
            _DB.SaveChanges();
            bool flag = InsertLC_EXCEPTION_C_TABLE_LOG(CrROle.EMPCODE, CrROle.EMPNAME, CrROle.FUEL_REIMBUR_PER_LTR, CrROle.FROMDATE, CrROle.TODATE, CrROle.REMARKS, CrROle.ADDEDBY, CrROle.ADDEDON, CrROle.UPDATEDBY, CrROle.UPDATEDON, int.Parse(CrROle.ACTIVE.ToString()), "UPDATE", CrROle.SRNO);
            // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
            res = true;
            return res;
        }

        public LibResult EditDataForLC_EXCEPTION(int SrNo, int EmpCode, decimal FuelReimbursementPerLtr, DateTime FromDate, DateTime ToDate, int LoginCode, string Remarks)
        {
            LibResult Res = new LibResult();
            try
            {

                //using (var db = new LCEntities())
                //using (var db = _DB)
                //{

                var Exists = _DB.LC_EXCEPTION_C_TABLE.Where(x => x.EMPCODE == EmpCode
                                                    && x.FUEL_REIMBUR_PER_LTR == FuelReimbursementPerLtr
                                                    && x.FROMDATE == FromDate
                                                    && x.TODATE == ToDate
                                                   ).Count();
                if (Exists > 1)
                {
                    Res.hasError = true;
                    Res.errorMessage = "Record Already Exists Please check and Update Another Employee";
                }
                else
                {

                    var EmpName = (from r in _DB.ADEMPLOYEE_LC where r.ACTIVE == 1 && r.ADEMPCODE == EmpCode select r.FIRSTNAME + " " + r.LASTNAME).FirstOrDefault().ToString();
                    LC_EXCEPTION_C_TABLE crmst = _DB.LC_EXCEPTION_C_TABLE.Find(Convert.ToInt64(SrNo));
                    crmst.EMPCODE = EmpCode;
                    crmst.EMPNAME = EmpName;
                    crmst.FUEL_REIMBUR_PER_LTR = FuelReimbursementPerLtr;
                    crmst.FROMDATE = FromDate;
                    crmst.TODATE = ToDate;
                    crmst.REMARKS = Remarks;
                    crmst.UPDATEDBY = LoginCode;
                    crmst.UPDATEDON = DateTime.Now;


                    _DB.Entry(crmst).State = EntityState.Modified;
                    _DB.SaveChanges();
                    bool flag = InsertLC_EXCEPTION_C_TABLE_LOG(crmst.EMPCODE, crmst.EMPNAME, crmst.FUEL_REIMBUR_PER_LTR, crmst.FROMDATE, crmst.TODATE, crmst.REMARKS, crmst.ADDEDBY, crmst.ADDEDON, crmst.UPDATEDBY, crmst.UPDATEDON, int.Parse(crmst.ACTIVE.ToString()), "UPDATE", crmst.SRNO); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    Res.hasError = false;
                    Res.errorMessage = "Record Updated successfully";

                }
                //}
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.ToString();

            }
            return Res;
        }

        public LibResult GETEmployee(string Key)
        {
            LibResult Res = new LibResult();

            try
            {


                var elist = (from data in _DB.ADEMPLOYEE_LC
                             where (data.ADEMPCODE.ToString().ToUpper().Contains(Key.ToUpper())
                             || data.FIRSTNAME.ToUpper().Contains(Key.ToUpper())
                             || data.LASTNAME.ToUpper().Contains(Key.ToUpper()))
                             && data.ACTIVE == 1
                             select new
                             { name = data.FIRSTNAME + " " + data.LASTNAME, ecode = data.ADEMPCODE }
                        ).ToList();

                Res.resultObject = elist;



            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Res;
        }

        #endregion


        #region LC_ReimbursementMaster B TABLE
        public List<LC_REIMBURSEMENT_B_TABLE> GetLC_ReimbursementMasterList()
        {
            List<LC_REIMBURSEMENT_B_TABLE> ilist = new List<LC_REIMBURSEMENT_B_TABLE>();

            ilist = _DB.LC_REIMBURSEMENT_B_TABLE.ToList();


            return ilist;


        }

        public LibResult InsertDataIntoLC_ReimbursementMaster(int DesignationID, string VehicleEntitlement, decimal Fule, int Refresh, int Meal, DateTime FromDate, DateTime ToDate, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //using (var db = _DB)
                //{
                var Exists = _DB.LC_REIMBURSEMENT_B_TABLE.Where(x => x.DESIGNATIONID == DesignationID
                                                    && x.VEHICLE_ENTITLEMENT == VehicleEntitlement
                                                    && x.FUEL_REIMBUR_PER_LTR == Fule
                                                    && x.REFRESH_ALLOW_PER_DAY == Refresh
                                                    && x.MEAL_ALLOWANCE == Meal
                                                    && x.FROMDATE == FromDate
                                                    && x.TODATE == ToDate
                                                   ).Count();


                if (Exists > 0)
                {
                    res.hasError = true;
                    res.errorMessage = "Record Already Exists";
                }
                else
                {
                    var Srno = _DB.LC_REIMBURSEMENT_B_TABLE.Select(x => x.SRNO).AsEnumerable().DefaultIfEmpty(0).Max();
                    var DesignationDescript = (from r in _DB.ADDESIGNATION_LC where r.ADDESIGNATIONID == DesignationID && r.ACTIVE == 1 select r.DESCRIP).FirstOrDefault().ToString();
                    LC_REIMBURSEMENT_B_TABLE crmst = new LC_REIMBURSEMENT_B_TABLE();

                    crmst.SRNO = Srno + 1;
                    crmst.DESIGNATIONID = DesignationID;
                    crmst.DESIGNATION = DesignationDescript;
                    crmst.VEHICLE_ENTITLEMENT = VehicleEntitlement;
                    crmst.FUEL_REIMBUR_PER_LTR = Fule;
                    crmst.REFRESH_ALLOW_PER_DAY = Refresh;
                    crmst.MEAL_ALLOWANCE = Meal;
                    crmst.FROMDATE = FromDate;
                    crmst.TODATE = ToDate;

                    crmst.ADDEDBY = LoginCode;
                    crmst.ADDEDON = DateTime.Now;
                    crmst.ACTIVE = 1; // SR107643 :: LC ENHANCEMENT ::  AUMENTO

                    _DB.Entry(crmst).State = EntityState.Added;
                    _DB.SaveChanges();
                    var flag = InsertLC_REIMBURSEMENT_B_TABLE_LOG(crmst.DESIGNATIONID, crmst.DESIGNATION, crmst.VEHICLE_ENTITLEMENT, crmst.FUEL_REIMBUR_PER_LTR, crmst.REFRESH_ALLOW_PER_DAY, crmst.MEAL_ALLOWANCE, crmst.FROMDATE, crmst.TODATE, crmst.ADDEDBY, crmst.ADDEDON, crmst.UPDATEDBY, crmst.UPDATEDON, int.Parse(crmst.ACTIVE.ToString()), "ADD", crmst.SRNO); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    res.hasError = false;
                    res.errorMessage = "Record Inserted successfully";

                }

                //}
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public LC_REIMBURSEMENT_B_TABLE GetEditDataForLC_ReimbursementMaster(int id)
        {
            return _DB.LC_REIMBURSEMENT_B_TABLE.Find(Convert.ToInt64(id));
        }
        public bool GetDeleteDataForLC_ReimbursementMaster(int id, int loginCode) // {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {
            bool res = false;
            LC_REIMBURSEMENT_B_TABLE CrROle = _DB.LC_REIMBURSEMENT_B_TABLE.Find(Convert.ToInt64(id));
            // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
            //_DB.LC_REIMBURSEMENT_B_TABLE.Remove(CrROle);
            CrROle.UPDATEDBY = Convert.ToInt64(loginCode);
            CrROle.UPDATEDON = DateTime.Now;
            CrROle.ACTIVE = CrROle.ACTIVE == 0 ? (short)1 : (short)0;
            _DB.Entry(CrROle).State = EntityState.Modified;
            var flag = InsertLC_REIMBURSEMENT_B_TABLE_LOG(CrROle.DESIGNATIONID, CrROle.DESIGNATION, CrROle.VEHICLE_ENTITLEMENT, CrROle.FUEL_REIMBUR_PER_LTR, CrROle.REFRESH_ALLOW_PER_DAY, CrROle.MEAL_ALLOWANCE, CrROle.FROMDATE, CrROle.TODATE, CrROle.ADDEDBY, CrROle.ADDEDON, CrROle.UPDATEDBY, CrROle.UPDATEDON, int.Parse(CrROle.ACTIVE.ToString()), "UPDATE", CrROle.SRNO);
            // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
            _DB.SaveChanges();
            res = true;
            return res;
        }

        public LibResult EditDataForLC_ReimbursementMaster(int SrNo, int DesignationID, string VehicleEntitlement, decimal Fule, int Refresh, int Meal, DateTime FromDate, DateTime ToDate, int LoginCode)
        {
            LibResult Res = new LibResult();
            try
            {

                //using (var db = new LCEntities())
                //using (var db = _DB)
                //{

                var Exists = _DB.LC_REIMBURSEMENT_B_TABLE.Where(x => x.DESIGNATIONID == DesignationID
                                                    && x.VEHICLE_ENTITLEMENT == VehicleEntitlement
                                                    && x.FUEL_REIMBUR_PER_LTR == Fule
                                                    && x.REFRESH_ALLOW_PER_DAY == Refresh
                                                    && x.MEAL_ALLOWANCE == Meal
                                                    && x.FROMDATE == FromDate
                                                    && x.TODATE == ToDate
                                                   ).Count();


                if (Exists > 1)
                {
                    Res.hasError = true;
                    Res.errorMessage = "Record Already Exists Please check and Update Another Employee";
                }
                else
                {

                    var DesignationDescript = (from r in _DB.ADDESIGNATION_LC where r.ADDESIGNATIONID == DesignationID && r.ACTIVE == 1 select r.DESCRIP).FirstOrDefault().ToString();

                    LC_REIMBURSEMENT_B_TABLE crmst = _DB.LC_REIMBURSEMENT_B_TABLE.Find(Convert.ToInt64(SrNo));

                    crmst.DESIGNATIONID = DesignationID;
                    crmst.DESIGNATION = DesignationDescript;
                    crmst.VEHICLE_ENTITLEMENT = VehicleEntitlement;
                    crmst.FUEL_REIMBUR_PER_LTR = Fule;
                    crmst.REFRESH_ALLOW_PER_DAY = Refresh;
                    crmst.MEAL_ALLOWANCE = Meal;
                    crmst.FROMDATE = FromDate;
                    crmst.TODATE = ToDate;

                    crmst.UPDATEDBY = LoginCode;
                    crmst.UPDATEDON = DateTime.Now;


                    _DB.Entry(crmst).State = EntityState.Modified;
                    _DB.SaveChanges();
                    var flag = InsertLC_REIMBURSEMENT_B_TABLE_LOG(crmst.DESIGNATIONID, crmst.DESIGNATION, crmst.VEHICLE_ENTITLEMENT, crmst.FUEL_REIMBUR_PER_LTR, crmst.REFRESH_ALLOW_PER_DAY, crmst.MEAL_ALLOWANCE, crmst.FROMDATE, crmst.TODATE, crmst.ADDEDBY, crmst.ADDEDON, crmst.UPDATEDBY, crmst.UPDATEDON, int.Parse(crmst.ACTIVE.ToString()), "UPDATE", crmst.SRNO); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    Res.hasError = false;
                    Res.errorMessage = "Record Updated successfully";

                }
                //}
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.ToString();

            }
            return Res;
        }

        public LibResult GetDesignationlist()
        {
            LibResult Res = new LibResult();
            try
            {

                //using (var db = new LCEntities())
                //using (var db = _DB)
                //{
                var Data = (from r in _DB.ADDESIGNATION_LC
                            where r.ACTIVE == 1
                            select new
                            {
                                Text = r.DESCRIP,
                                Value = r.ADDESIGNATIONID
                            }).Distinct()
                              .Where(item => !string.IsNullOrEmpty(item.Text))
                              .ToList();


                Res.resultObject = Data;

                //}
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.ToString();

            }
            return Res;
        }
        #endregion

        #endregion

        #region  Local Conveyance Request functions

        // Commented by Aumento as on 21-06-2024
        //public DataTable GetODDATA(string strEmpCode, string FromDate, string ToDate)
        //{
        //    DataTable Dt = new DataTable();
        //    ConnectionString objCnStr = new ConnectionString();
        //    string strCn = objCnStr.getConnectingString();



        //    using (OracleConnection objCn = new OracleConnection())
        //    {
        //        objCn.ConnectionString = strCn;
        //        try
        //        {

        //            objCn.Open();
        //            OracleCommand objCmd = new OracleCommand();
        //            objCmd.Connection = objCn;
        //            objCmd.CommandText = "PKG_OFFOUTDUTYTRANS.SPROC_OFFODAPPROVALDATA";

        //            objCmd.Parameters.Add("CUR_OUTDUTY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
        //            objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
        //            objCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = ToDate;
        //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            objCmd.BindByName = true;
        //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
        //            objAdr.Fill(Dt);


        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            if (objCn != null)
        //            {
        //                objCn.Close();
        //            }


        //        }

        //        return Dt;
        //    }
        //}
        // Commented by Aumento as on 21-06-2024

        // New Function Added by Aumento as on 21-06-2024
        public async Task<List<LC_DETAIL_TEMP>> GetODDATA(string strEmpCode, string FromDate)
        {

            DataTable Dt = new DataTable();
            //EportalESS objessaddress = new EportalESS();
            List<LC_DETAIL_TEMP> LCIlist = new List<LC_DETAIL_TEMP>();

            try
            {
                string FromDateYYYYMM = FromDate.Replace("-", "");
                FromDateYYYYMM += "01";

                Dt = await _iEportalESS.GetOverstayReport(strEmpCode, FromDateYYYYMM, "x");
                long ADEMPCODE_ = long.Parse(strEmpCode);
                string ADEMPCODE_String = ADEMPCODE_.ToString();
                string codeString = ADEMPCODE_String.Length > 5 ? ADEMPCODE_String.Substring(ADEMPCODE_String.Length - 5) : ADEMPCODE_String;
                var empobj = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == ADEMPCODE_ && x.ACTIVE == 1).Select(x => new { x.FIRSTNAME, x.LASTNAME }).FirstOrDefault();
                var EmpName = (empobj.FIRSTNAME + " " + empobj.LASTNAME);


                foreach (var row in Dt.AsEnumerable())
                {
                    var dateofexpendure = DateTime.Parse(row["EMP_LDATE"].ToString());
                    var Year = dateofexpendure.Year.ToString().Substring(dateofexpendure.Year.ToString().Length - 2);

                    if (row["EMP_UABS_REMARK"].ToString() == "Out Duty Local" && dateofexpendure <= DateTime.Now.Date)
                    //if (dateofexpendure <= DateTime.Now.Date)  //uncomment for testing only
                    {
                        LC_DETAIL_TEMP LCitem = new LC_DETAIL_TEMP();

                        var MaxASOFFODID = long.Parse(codeString + Year + dateofexpendure.Month.ToString() + dateofexpendure.Day.ToString());

                        LCitem.ASOFFODID = MaxASOFFODID;
                        LCitem.EMPCODE = ADEMPCODE_;
                        LCitem.EMPNAME = EmpName;
                        LCitem.DATEOFEXPENDITURE = dateofexpendure;
                        LCitem.PURPOSE = "";
                        LCitem.FROMCITY = "";
                        LCitem.TOCITY = "";
                        LCitem.FROMCITYID = 99999;
                        LCitem.TOCITYID = 99999;
                        LCitem.TIMEIN = "00:00";
                        LCitem.TIMEOUT = "00:00";
                        LCitem.NOOFHOURS = 0;
                        //LCitem.TIMEIN = $"{(row["EMP_IN_TIME"].ToString()).Substring(0, 2)}:{(row["EMP_IN_TIME"].ToString()).Substring(2, 2)}";
                        //LCitem.TIMEOUT = $"{(row["EMP_OUT_TIME"].ToString()).Substring(0, 2)}:{(row["EMP_OUT_TIME"].ToString()).Substring(2, 2)}";
                        //TimeSpan timeIn = TimeSpan.Parse(row["EMP_IN_TIME"].ToString());
                        //TimeSpan timeOut = TimeSpan.Parse(row["EMP_OUT_TIME"].ToString());
                        //TimeSpan timeDifference = timeOut - timeIn;
                        //string timeDifferenceString = $"{timeDifference.Hours}.{timeDifference.Minutes:D2}";
                        //LCitem.NOOFHOURS = decimal.Parse(timeDifferenceString);///
                        LCitem.KMCOVERED = 0;
                        LCitem.MODEOFTRAVEL = "";
                        LCitem.RATEPERKM = 0;
                        LCitem.TRAVELREIMBURSEMENT = 0;
                        LCitem.TOLLTAXMISCAMOUNT = 0;
                        LCitem.TOTAL = 0;
                        LCitem.DT = "";
                        LCitem.TEMPID = 0;
                        LCitem.REFRESHMENTMEALALLOWANCE = 0;
                        LCitem.FUEL_REIMBURSMENT = 0;
                        LCitem.MODEOFTRAVEL_OTHER = "";
                        LCitem.FROMCITY_OTHER = "";
                        LCitem.TOCITY_OTHER = "";
                        LCitem.ISSUBMITTED = 0;
                        LCitem.STATUS = "Pending";

                        LCIlist.Add(LCitem);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return LCIlist.OrderBy(l => l.DATEOFEXPENDITURE).ToList();
        }
        // New Function Added by Aumento as on 21-06-2024


        public LC_DETAIL_TEMP GetODDATAFromLC_DETAIL(long? EmpCode, DateTime? DateOfExpenditure, long? ASOFFODID)
        {
            LC_DETAIL_TEMP Res = new LC_DETAIL_TEMP();


            try
            {
                Res = (from i in _DB.LC_DETAIL_TEMP
                       where i.EMPCODE == EmpCode
                            && i.ASOFFODID == ASOFFODID
                            && i.DATEOFEXPENDITURE == DateOfExpenditure
                       // && (i.ISSUBMITTED == 0 || i.ISSUBMITTED == 3)

                       select i).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Res;

        }

        public List<LC_DETAIL_TEMP> GetODListDATAFromLC_DETAIL(string FromDateYYYYMM, int? EmpCode, int Status) // Remove Todate perameter by Aumento as on 21-06-2024 // {Status added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {
            List<LC_DETAIL_TEMP> ilist = new List<LC_DETAIL_TEMP>();

            try
            {

                // Added by Aumento as on 21-06-2024
                string FromDate = FromDateYYYYMM + "-01";
                DateTime FromDate_ = DateTime.Parse(FromDate);
                int month = FromDate_.Month;
                int year = FromDate_.Year;
                int lastDayOfMonth = DateTime.DaysInMonth(year, month);
                string ToDate = $"{FromDateYYYYMM}-{lastDayOfMonth.ToString("00")}";
                DateTime ToDate_ = DateTime.Parse(ToDate);

                // Added by Aumento as on 21-06-2024
                var data = (from i in _DB.LC_DETAIL_TEMP
                            where i.DATEOFEXPENDITURE >= FromDate_
            && i.DATEOFEXPENDITURE <= ToDate_ && i.EMPCODE == EmpCode
            //&& (i.ISSUBMITTED == 1 || i.ISSUBMITTED == 2 || i.ISSUBMITTED == 4)
            && (Status == 1 ? new List<long> { 1, 2 }.Contains(i.ISSUBMITTED.Value) : i.ISSUBMITTED == Status) //  SR107643 :: LC ENHANCEMENT ::  AUMENTO
                            select i).ToList();

                foreach (var k in data)
                {
                    k.STATUS = getStatus(k);

                    ilist.Add(k);

                }




            }
            catch (Exception ex)
            {

                ilist = null;
            }

            return ilist.OrderBy(l => l.DATEOFEXPENDITURE).ToList();

        }

        public LibResult GetEmployeeDATA(int EmpCode)

        {
            LibResult res = new LibResult();
            try
            {

                var query_ = (from a in _DB.VW_ASSOCIATELVLDETAILS_LC
                              join b in _DB.ADEMPLOYEE_LC on a.ADEMPCODE equals b.ADEMPCODE
                              where a.SYKI == _Syki && a.ADEMPCODE == EmpCode // && a.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
                              select new
                              {
                                  a.ADEMPCODE,
                                  EmpName = b.FIRSTNAME + " " + b.LASTNAME,
                                  ADDESIGNATION = (from k in _DB.ADDESIGNATION_LC
                                                   where k.ACTIVE == 1 && k.ADDESIGNATIONID == a.ADDESIGNATIONID
                                                   select k.DESCRIP).FirstOrDefault(),
                                  CurrentDate = DateTime.Now,
                                  a.DIVISION,
                                  a.DEPARTMENT,
                                  a.SECTION,
                                  b.TMOBILE
                              }).AsEnumerable()
                .Select(x => new
                {
                    x.ADEMPCODE,
                    x.EmpName,
                    ADDESIGNATION = x.ADDESIGNATION != null ? x.ADDESIGNATION.ToString() : null,
                    CurrentDate = x.CurrentDate.Date,
                    x.DIVISION,
                    x.DEPARTMENT,
                    x.SECTION,
                    x.TMOBILE
                }).ToList();




                //var query = (from a in _DB.VW_ASSOCIATELVLDETAILS
                //             join b in _DB.ADEMPLOYEE_LC on a.ADEMPCODE equals b.ADEMPCODE
                //             where a.SYKI == _Syki && a.ADEMPCODE == EmpCode && a.ACTIVE == 1
                //             select new
                //             {
                //                 a.ADEMPCODE,
                //                 EmpName = b.FIRSTNAME + " " + b.LASTNAME,
                //                 a.FUNCTIONALDESIGNATION,
                //                 CurrentDate = DateTime.Now,
                //                 a.DIVISION,
                //                 a.DEPARTMENT,
                //                 a.SECTION,
                //                 b.TMOBILE
                //             }).AsEnumerable()
                //             .Select(x => new
                //             {
                //                 x.ADEMPCODE,
                //                 x.EmpName,
                //                 x.FUNCTIONALDESIGNATION,
                //                 CurrentDate = x.CurrentDate.Date,
                //                 x.DIVISION,
                //                 x.DEPARTMENT,
                //                 x.SECTION,
                //                 x.TMOBILE
                //             }).ToList();

                res.resultObject = query_;

            }
            catch (Exception ex)
            {
                res.errorMessage = ex.Message;
                res.hasError = true;
            }


            return res;
        }


        public List<object> CityList()
        {
            List<object> cityList = new List<object>();
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = serverpath.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_OFFOUTDUTYTRANS.SPROC_LC_GETCITY";
                    objCmd.Parameters.Add("CUR_OUTDUTY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;

                    OracleDataReader reader = objCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var city = new
                        {
                            SYCITYID = reader["SYCITYID"],
                            DESCRIP = reader["DESCRIP"]
                        };

                        cityList.Add(city);
                    }

                    var city_ = new
                    {
                        SYCITYID = 0,
                        DESCRIP = "Other"
                    };

                    cityList.Add(city_);


                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }

            return cityList;
        }


        public LibResult SaveDetails(LC_DETAIL_TEMP i, int logincode, string Mode) // {Mode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {
            LibResult res = new LibResult();
            //string logFilePath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/LCODD/errorLog.txt");
            //StreamWriter swMailLog = null;
            try
            {
                //if (File.Exists(logFilePath))
                //{
                //    swMailLog = new StreamWriter(logFilePath, true);
                //}
                //else
                //{
                //    swMailLog = File.CreateText(logFilePath);
                //}
                //swMailLog.WriteLine("--============================================================");
                //swMailLog.WriteLine("SaveDetail Step1: " + DateTime.Now.ToString());

                LC_DETAIL_TEMP exist = _DB.LC_DETAIL_TEMP.FirstOrDefault(m =>
                       m.ASOFFODID == i.ASOFFODID &&
                       m.EMPCODE == i.EMPCODE &&
                       m.DATEOFEXPENDITURE == i.DATEOFEXPENDITURE);

                // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                LC_DETAIL_TEMP existCheck = _DB.LC_DETAIL_TEMP.FirstOrDefault(m =>
                      m.EMPCODE == i.EMPCODE &&
                      m.DATEOFEXPENDITURE == i.DATEOFEXPENDITURE);

                if (Mode == "Add" && existCheck != null)
                {
                    res.hasError = true;
                    res.errorMessage = "Request Already exist for this Date of expenditire";
                    return res;
                }
                // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

                decimal tollTaxBillTotal = 0;
                decimal otherTotal = 0;
                //swMailLog.WriteLine("SaveDetail Step2: " + DateTime.Now.ToString());
                decimal otherChargeTotal = 0; // Added by Aumento :: SR107643 :: LC Enhancement :: Additional Change

                if (i.DT != "[{}]")
                {

                    dynamic DT_DATA = Newtonsoft.Json.JsonConvert.DeserializeObject(i.DT);
                    foreach (var item in DT_DATA)
                    {

                        string docType = item.DocType.ToString().Trim();
                        decimal amount = !string.IsNullOrEmpty(item.Amount?.ToString()) ? decimal.Parse(item.Amount.ToString().Trim()) : 0;

                        if (docType == "Toll Tax Bill")
                        {
                            tollTaxBillTotal += amount;
                        }
                        // START :: Added by Aumento :: SR107643 :: LC Enhancement :: Additional Change
                        else if (docType == "Other Charges")
                        {
                            otherChargeTotal += amount;
                        }
                        // END :: Added by Aumento :: SR107643 :: LC Enhancement :: Additional Change
                        else
                        {
                            otherTotal += amount;
                        }
                    }

                }
                //swMailLog.WriteLine("SaveDetail Step3: " + DateTime.Now.ToString());

                decimal fuelReimbursement, mealAllowanceAmt, refreshAllowance, twoWheelRatePerKM, fourWheelRatePerKM, fuelReimbursementFromException;
                GetAllowanceAndRate_FUEL(logincode, out fuelReimbursement, out mealAllowanceAmt, out refreshAllowance, out twoWheelRatePerKM, out fourWheelRatePerKM, out fuelReimbursementFromException);

                decimal fuelRate = (fuelReimbursementFromException != 0) ? fuelReimbursementFromException : fuelReimbursement;
                bool hasFuelReimbursement = (fuelRate != 0);

                //swMailLog.WriteLine("SaveDetail Step4: " + DateTime.Now.ToString());

                if (exist != null)
                {
                    _DB.Entry(exist).Reload();
                    exist.UPDATEDON = DateTime.Now;
                    exist.UPDATEDBY = logincode;
                    exist.DT = i.DT;
                    exist.FROMCITY = i.FROMCITY;
                    exist.FROMCITYID = i.FROMCITYID;
                    exist.TOCITY = i.TOCITY;
                    exist.TOCITYID = i.TOCITYID;
                    exist.TIMEIN = i.TIMEIN;
                    exist.TIMEOUT = i.TIMEOUT;
                    exist.MODEOFTRAVEL = i.MODEOFTRAVEL;
                    exist.KMCOVERED = i.KMCOVERED;
                    exist.NOOFHOURS = i.NOOFHOURS;
                    //exist.TOLLTAXMISCAMOUNT = i.TOLLTAXMISCAMOUNT;
                    exist.TOLLTAXMISCAMOUNT = tollTaxBillTotal;
                    //exist.REFRESHMENTMEALALLOWANCE = i.MEAL_REFRE_ALLW_CLAIM == "YES" ? i.REFRESHMENTMEALALLOWANCE : 0;
                    exist.REFRESHMENTMEALALLOWANCE = (i.MEAL_REFRE_ALLW_CLAIM == "YES" ? i.REFRESHMENTMEALALLOWANCE : 0) + otherChargeTotal; // Added by Aumento :: SR107643 :: LC Enhancement :: Additional Change

                    exist.FROMCITY_OTHER = i.FROMCITY_OTHER;
                    exist.TOCITY_OTHER = i.TOCITY_OTHER;
                    exist.MODEOFTRAVEL_OTHER = i.MODEOFTRAVEL_OTHER;
                    exist.MEAL_REFRE_ALLW_CLAIM = i.MEAL_REFRE_ALLW_CLAIM;
                    exist.LUNCH_ALLW_CLAIM = i.LUNCH_ALLW_CLAIM;
                    exist.LUNCH_ALLW_AMOUNT = i.LUNCH_ALLW_AMOUNT;
                    exist.DINNER_ALLW_AMOUNT = i.DINNER_ALLW_AMOUNT;
                    exist.DINNER_ALLW_CLAIM = i.DINNER_ALLW_CLAIM;
                    exist.FUEL_ALLW_CLAIM = i.FUEL_ALLW_CLAIM;
                    exist.TOURSTARTFROMHOMEKM = i.TOURSTARTFROMHOMEKM;
                    exist.PURPOSE = i.PURPOSE;// Added by Aumento as on 26-06-2024 
                    if (i.MODEOFTRAVEL == "Self Car" || i.MODEOFTRAVEL == "Self Bike/Scooter")
                    {
                        if (hasFuelReimbursement)
                        {
                            exist.TRAVELREIMBURSEMENT = otherTotal;
                            exist.FUEL_REIMBURSMENT = (i.FUEL_ALLW_CLAIM == "YES" && (i.KMCOVERED - i.TOURSTARTFROMHOMEKM) > 0) ? ((i.KMCOVERED - i.TOURSTARTFROMHOMEKM) / fuelRate) : 0;
                            exist.RATEPERKM = fuelRate; //Added by Aumento as on 01082030
                        }
                        else
                        {
                            var rate_ = i.MODEOFTRAVEL == "Self Bike/Scooter" ? twoWheelRatePerKM : fourWheelRatePerKM;
                            exist.TRAVELREIMBURSEMENT = (i.KMCOVERED * rate_) + otherTotal;
                            exist.RATEPERKM = rate_;
                            exist.FUEL_REIMBURSMENT = 0;
                        }
                    }
                    else
                    {
                        exist.TRAVELREIMBURSEMENT = otherTotal;
                        exist.FUEL_REIMBURSMENT = 0;
                        exist.RATEPERKM = 0;
                    }
                    //swMailLog.WriteLine("SaveDetail Step5: " + DateTime.Now.ToString());

                    _DB.Entry(exist).State = EntityState.Modified;
                    _DB.SaveChanges();

                }
                else
                {
                    var MaxSrno = _DB.LC_DETAIL_TEMP.Select(x => x.SRNO).AsEnumerable().DefaultIfEmpty(1).Max();
                    MaxSrno++;

                    i.TOLLTAXMISCAMOUNT = tollTaxBillTotal;
                    if (i.MODEOFTRAVEL == "Self Car" || i.MODEOFTRAVEL == "Self Bike/Scooter")
                    {
                        if (hasFuelReimbursement)
                        {
                            i.TRAVELREIMBURSEMENT = otherTotal;
                            i.FUEL_REIMBURSMENT = (i.FUEL_ALLW_CLAIM == "YES" && (i.KMCOVERED - i.TOURSTARTFROMHOMEKM) > 0) ? ((i.KMCOVERED - i.TOURSTARTFROMHOMEKM) / fuelRate) : 0;
                        }
                        else
                        {
                            var rate_ = i.MODEOFTRAVEL == "Self Bike/Scooter" ? twoWheelRatePerKM : fourWheelRatePerKM;
                            i.TRAVELREIMBURSEMENT = (i.KMCOVERED * rate_) + otherTotal;
                            i.RATEPERKM = rate_;
                            i.FUEL_REIMBURSMENT = 0;
                        }
                    }
                    else
                    {
                        i.TRAVELREIMBURSEMENT = otherTotal;
                        i.FUEL_REIMBURSMENT = 0;
                        i.RATEPERKM = 0;

                    }

                    //i.REFRESHMENTMEALALLOWANCE = _REFRESHMENTMEALALLOWANCE;                    
                    i.REFRESHMENTMEALALLOWANCE = (i.MEAL_REFRE_ALLW_CLAIM == "YES" ? i.REFRESHMENTMEALALLOWANCE : 0) + otherChargeTotal; // Added by T09-10-2025
                    i.LUNCH_ALLW_CLAIM = i.LUNCH_ALLW_CLAIM;
                    i.LUNCH_ALLW_AMOUNT = i.LUNCH_ALLW_AMOUNT;
                    i.DINNER_ALLW_AMOUNT = i.DINNER_ALLW_AMOUNT;
                    i.DINNER_ALLW_CLAIM = i.DINNER_ALLW_CLAIM;
                    i.FUEL_ALLW_CLAIM = i.FUEL_ALLW_CLAIM;
                    i.TOURSTARTFROMHOMEKM = i.TOURSTARTFROMHOMEKM;


                    i.SRNO = MaxSrno;
                    i.ADDEDON = DateTime.Now;
                    i.ADDEDBY = logincode;
                    i.ISSUBMITTED = 0;

                    _DB.Entry(i).State = EntityState.Added;
                    _DB.SaveChanges();
                }


                res.hasError = false;

                //swMailLog.WriteLine("SaveDetail Task end: " + DateTime.Now.ToString());
                //swMailLog.WriteLine("--============================================================");

                //swMailLog.Close();
                //swMailLog.Dispose();
            }
            catch (Exception ex)
            {
                res.hasError = true;
                LogException(ex);
                res.errorMessage = ex.Message.ToString();
            }

            return res;
        }



        //private decimal CalculateRefreshmentMealAllowance(decimal? noOfHours, bool lunchFlag, bool dinnerFlag, decimal refreshAllowance, decimal mealAllowanceAmt)
        //{
        //    if (noOfHours > 4)
        //    {
        //        decimal totalAllowance = refreshAllowance;
        //        if (lunchFlag)
        //            totalAllowance += mealAllowanceAmt;
        //        if (dinnerFlag)
        //            totalAllowance += mealAllowanceAmt;
        //        return totalAllowance;
        //    }
        //    else if (noOfHours > 2 && !lunchFlag && !dinnerFlag)
        //    {
        //        return refreshAllowance;
        //    }
        //    else if (noOfHours > 2 && (lunchFlag || dinnerFlag))
        //    {
        //        return mealAllowanceAmt;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        public LibResult Getallowcheck(int logincode, TimeSpan INTime, TimeSpan OUTTime, decimal NOOFHOURS, string MODEOFTRAVEL, int KMCOVERED)
        {
            LibResult res = new LibResult();
            try
            {

                decimal fuelReimbursement, mealAllowanceAmt, refreshAllowance, twoWheelRatePerKM, fourWheelRatePerKM, fuelReimbursementFromException;
                GetAllowanceAndRate_FUEL(logincode, out fuelReimbursement, out mealAllowanceAmt, out refreshAllowance, out twoWheelRatePerKM, out fourWheelRatePerKM, out fuelReimbursementFromException);
                decimal fuelRate = (fuelReimbursementFromException != 0) ? fuelReimbursementFromException : fuelReimbursement;
                bool hasFuelReimbursement = fuelRate != 0 ? true : false;
                decimal fuelReimbursementCalculated = 0;

                Tuple<TimeSpan, TimeSpan, TimeSpan, TimeSpan> LDTime = GetLunchDinnerTime();
                TimeSpan LunchStartTime = LDTime.Item1;
                TimeSpan LunchEndTime = LDTime.Item2;
                TimeSpan DinnerStartTime = LDTime.Item3;
                TimeSpan DinnerEndTime = LDTime.Item4;

                bool IsRefreshmentAllowanceApplicable = false;
                bool IsLunchApplicable_ = false;
                bool IsDinnerApplicable_ = false;


                if (NOOFHOURS > 0) //if (NOOFHOURS > 2)  // commented as on 20-05-2024
                {

                    //if ((INTime.TotalHours <= LunchStartTime.TotalHours && OUTTime.TotalHours >= LunchEndTime.TotalHours)
                    //    || (INTime.TotalHours >= LunchStartTime.TotalHours && INTime.TotalHours <= LunchEndTime.TotalHours)
                    //    || (OUTTime.TotalHours >= LunchStartTime.TotalHours && OUTTime.TotalHours <= LunchEndTime.TotalHours)
                    //    )
                    //{
                    //    IsLunchApplicable_ = true;
                    //}  // commented as on 17-05-2024

                    IsLunchApplicable_ = true;

                    //if ((INTime.TotalHours <= DinnerStartTime.TotalHours && OUTTime.TotalHours >= DinnerEndTime.TotalHours)
                    //   || (INTime.TotalHours >= DinnerStartTime.TotalHours && INTime.TotalHours <= DinnerEndTime.TotalHours)
                    //   || (OUTTime.TotalHours >= DinnerStartTime.TotalHours && OUTTime.TotalHours <= DinnerEndTime.TotalHours)
                    //   )

                    //{
                    //    IsDinnerApplicable_ = true;
                    //} // commented as on 17-05-2024

                    IsDinnerApplicable_ = true;

                    //if ((IsLunchApplicable_ || IsDinnerApplicable_) && NOOFHOURS > 5)
                    //{
                    //    IsRefreshmentAllowanceApplicable = true;
                    //}
                    //else if (!IsLunchApplicable_ && !IsDinnerApplicable_)
                    //{
                    //    IsRefreshmentAllowanceApplicable = true;
                    //} // commented as on 20-05-2024


                    if (NOOFHOURS > 2)
                    {
                        IsRefreshmentAllowanceApplicable = true;
                    }
                }


                if (MODEOFTRAVEL == "Self Car" || MODEOFTRAVEL == "Self Bike/Scooter")
                {
                    if (hasFuelReimbursement == true)
                    {
                        fuelReimbursementCalculated = (KMCOVERED / fuelRate);

                    }

                }


                var data = new
                {

                    IsRefreshAllwance = IsRefreshmentAllowanceApplicable,
                    IsDinnerApplicable = IsDinnerApplicable_,
                    IsLunchApplicable = IsLunchApplicable_,
                    FuelReimburAmount = fuelReimbursementCalculated,
                    RefreshAmount = refreshAllowance,
                    MealAmount = mealAllowanceAmt

                };

                res.resultObject = data;

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return res;
        }
        public string getStatus(LC_DETAIL_TEMP i)
        {
            string status = "Pending";

            try
            {
                var data = _DB.LC_HEAD.FirstOrDefault(v => v.EMPCODE == i.EMPCODE && v.LCTRANNO == i.TEMPID);

                if (data != null)
                {
                    // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    if (data.RECOMM_STATUS == "Pending")
                    {
                        status = "Pending at Recommendation " + (data.RECOMM_USER != null ? data.RECOMM_USER.ToString() : "");
                    }
                    else if (data.RECOMM_STATUS == "SendBack")
                    {
                        status = "Sent Back from Recommendation " + (data.RECOMM_USER != null ? data.RECOMM_USER.ToString() : "");
                    }
                    else if (data.RECOMM_STATUS == "Rejected")
                    {
                        status = "Rejected by Recommendation " + (data.RECOMM_USER != null ? data.RECOMM_USER.ToString() : "");
                    }
                    else if (data.APPROVER_STATUS == "Pending")
                    {
                        status = "Pending at Approval " + (data.APPROVER_USER != null ? data.APPROVER_USER.ToString() : "");
                    }
                    else if (data.APPROVER_STATUS == "SendBack")
                    {
                        status = "Sent Back from Approval " + (data.APPROVER_USER != null ? data.APPROVER_USER.ToString() : "");
                    }
                    else if (data.APPROVER_STATUS == "Rejected")
                    {
                        status = "Rejected by Approval " + (data.APPROVER_USER != null ? data.APPROVER_USER.ToString() : "");
                    }
                    else if (data.ADMIN_STATUS != null && data.ADMIN_STATUS == "Pending")
                    {
                        status = "Pending at Admin " + (data.ADMIN_USER != null ? data.ADMIN_USER.ToString() : "");
                    }
                    else if (data.ADMIN_STATUS != null && data.ADMIN_STATUS == "SendBack")
                    {
                        status = "Sent Back from Admin " + (data.ADMIN_USER != null ? data.ADMIN_USER.ToString() : "");
                    }
                    else if (data.ADMIN_STATUS != null && data.ADMIN_STATUS == "Rejected")
                    {
                        status = "Rejected by Admin " + (data.ADMIN_USER != null ? data.ADMIN_USER.ToString() : "");
                    }
                    else if (data.FINANCE_STATUS == "Pending")
                    {
                        status = "Pending at Finance " + (data.FINANCE_USER != null ? data.FINANCE_USER.ToString() : "");
                    }
                    else if (data.FINANCE_STATUS == "SendBack")
                    {
                        status = "Sent Back from Finance " + (data.FINANCE_USER != null ? data.FINANCE_USER.ToString() : "");
                    }
                    else if (data.FINANCE_STATUS == "Rejected")
                    {
                        status = "Rejected by Finance " + (data.FINANCE_USER != null ? data.FINANCE_USER.ToString() : "");
                    }
                    else if (data.ADMIN_STATUS != null && data.ADMIN_STATUS == "Pending" && data.FINANCE_STATUS == "Pending")
                    {
                        status = "Pending at Admin " + (data.ADMIN_USER != null ? data.ADMIN_USER.ToString() : "") + " and Finance " + (data.FINANCE_USER != null ? data.FINANCE_USER.ToString() : "");
                    }
                    else if (data.ADMIN_STATUS != null && data.ADMIN_STATUS == "Approved" && data.FINANCE_STATUS == "Approved")
                    {
                        status = "Approved by Finanace " + (data.FINANCE_USER != null ? data.FINANCE_USER.ToString() : "");
                    }
                    else if (data.ADMIN_STATUS == null && data.FINANCE_STATUS == "Approved")
                    {
                        status = "Approved by Finanace " + (data.FINANCE_USER != null ? data.FINANCE_USER.ToString() : "");
                    }
                    // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    else
                    {
                        status = "";
                    }
                }

            }
            catch (Exception ex)
            {
                status = "";
            }

            return status;
        }

        public LibResult SubmitData(List<LC_DETAIL_TEMP> DtData, LC_HEAD HeaderData, int logincode, Employee_Details LoginEmpDT)
        {
            LibResult res = new LibResult();
            List<LC_DETAIL> dtList = new List<LC_DETAIL>();
            string Purpose = string.Empty;
            int tempID = 0;
            decimal TOTAL_MEAL_AMOUNT = 0;
            decimal TOTAL_CONV_AMOUNT = 0;

            try
            {
                if (DtData != null)
                {
                    Purpose = DtData[0].PURPOSE.ToString();
                    tempID = (int)DtData[0].TEMPID == null ? 0 : (int)DtData[0].TEMPID;

                }

                foreach (var item in DtData)
                {
                    var DINNER_ALLW_AMOUNT_ = item.DINNER_ALLW_AMOUNT == null ? "0" : item.DINNER_ALLW_AMOUNT.ToString();
                    var TOLLTAXMISCAMOUNT_ = item.TOLLTAXMISCAMOUNT == null ? "0" : item.TOLLTAXMISCAMOUNT.ToString();
                    var LUNCH_ALLW_AMOUNT_ = item.LUNCH_ALLW_AMOUNT == null ? "0" : item.LUNCH_ALLW_AMOUNT.ToString();
                    var REFRESHMENTMEALALLOWANCE_ = item.REFRESHMENTMEALALLOWANCE == null ? "0" : item.REFRESHMENTMEALALLOWANCE.ToString();
                    var TRAVELREIMBURSEMENT_ = item.TRAVELREIMBURSEMENT == null ? "0" : item.TRAVELREIMBURSEMENT.ToString();

                    TOTAL_MEAL_AMOUNT = TOTAL_MEAL_AMOUNT + decimal.Parse(DINNER_ALLW_AMOUNT_) + decimal.Parse(LUNCH_ALLW_AMOUNT_) + decimal.Parse(REFRESHMENTMEALALLOWANCE_);

                    TOTAL_CONV_AMOUNT = TOTAL_CONV_AMOUNT + decimal.Parse(TOLLTAXMISCAMOUNT_) + decimal.Parse(TRAVELREIMBURSEMENT_);
                }

                HeaderData.TOTAL_MEAL_AMOUNT = TOTAL_MEAL_AMOUNT;
                HeaderData.TOTAL_CONV_AMOUNT = TOTAL_CONV_AMOUNT;

                LC_HEAD lcHead = CreateLCHead(HeaderData, logincode, Purpose, tempID, LoginEmpDT);


                if (lcHead != null)
                {
                    foreach (LC_DETAIL_TEMP item in DtData)
                    {
                        LC_DETAIL lcDetail = CreateLCDetail(item, lcHead, logincode);
                        dtList.Add(lcDetail);
                    }

                }
                res.resultObject = dtList;
                res.errorMessage = "Request submitted successfully with Request No: " + lcHead.LCTRANNO.ToString();
                res.hasError = false;
            }
            catch (Exception ex)
            {

                res.hasError = true;
                LogException(ex);
                res.errorMessage = ex.Message.ToString();
            }


            return res;
        }



        private LC_HEAD CreateLCHead(LC_HEAD HeaderData, int loginCode, string Purpose, int TEMPID, Employee_Details LoginEmpDT)
        {
            LC_HEAD lcHead = null;
            try
            {
                //using (var _db = new LCEntities())
                //using (var _db = _DB)
                //{
                lcHead = _DB.LC_HEAD.FirstOrDefault(x => x.LCTRANNO == TEMPID);

                if (lcHead == null)
                {
                    lcHead = CreateLCHead_DT(HeaderData, loginCode, Purpose, TEMPID, LoginEmpDT);
                    var flag = InsertLC_HEAD_LOG(lcHead, "Insert");   //  SR107643 :: LC ENHANCEMENT ::  AUMENTO
                }
                else
                {
                    lcHead = UpdateLCHead_DT(lcHead, HeaderData, loginCode, Purpose, TEMPID, LoginEmpDT);
                    var flag = InsertLC_HEAD_LOG(lcHead, "Update");  //  SR107643 :: LC ENHANCEMENT ::  AUMENTO
                }
                //}
            }
            catch (DbUpdateException ex)
            {

                lcHead = null;
            }
            catch (Exception ex)
            {

                lcHead = null;
            }

            return lcHead;
        }

        private LC_HEAD CreateLCHead_DT(LC_HEAD headerData, int loginCode, string purpose, int tempId, Employee_Details LoginEmpDT)
        {
            LC_HEAD lcHead = new LC_HEAD();
            try
            {
                //using (var db = new LCEntities())
                //{                  

                var MAXSRNO = _DB.LC_HEAD.Select(x => x.SRNO).AsEnumerable().DefaultIfEmpty(DefaultValues.MaxSrnoHead).Max() + 1;
                var MAXLCTRANNO = _DB.LC_HEAD.Select(x => x.LCTRANNO).AsEnumerable().DefaultIfEmpty(DefaultValues.MaxLcTranId).Max() + 1;

                lcHead.SRNO = MAXSRNO;
                    lcHead.LCTRANNO = MAXLCTRANNO;
                    lcHead.LCTRANDATE = headerData.LCTRANDATE;
                    lcHead.EMPCODE = headerData.EMPCODE;
                    lcHead.EMPNAME = headerData.EMPNAME;
                    lcHead.PURPOSE = purpose;
                    lcHead.DESIGNATION = headerData.DESIGNATION;
                    lcHead.TOTAL_AMOUNT = headerData.TOTAL_AMOUNT;
                    lcHead.TOTAL_AMT_IN_WORD = headerData.TOTAL_AMT_IN_WORD;
                    lcHead.REQUIRED_AMOUNT = headerData.TOTAL_AMOUNT;
                    lcHead.TOTAL_KMCOVERED = headerData.TOTAL_KMCOVERED;
                    lcHead.TOTAL_NOOFHOURS = headerData.TOTAL_NOOFHOURS;
                    lcHead.FUAL = headerData.FUAL;
                    lcHead.TOTAL_CONV_AMOUNT = headerData.TOTAL_CONV_AMOUNT;
                    lcHead.TOTAL_MEAL_AMOUNT = headerData.TOTAL_MEAL_AMOUNT;
                    lcHead.PPPLANT = headerData.PPPLANT;

                    // var sectionIdCheck = db.VW_ASSOCIATELVLDETAILS.FirstOrDefault(x => x.ADEMPCODE == loginCode && x.SYKI == db.SYKI.Where(s => s.ACTIVE == 1).Select(s => s.SYKIID).FirstOrDefault() && x.ACTIVE == 1);

                    //CommonRepository comprepo = new CommonRepository();

                    UserApprovalAuthority UserAppAuth = _cmr.CheckApprovalAuthority(loginCode);

                    string strDesginations = _DB.SYPARAMETERS_LC.Where(x => x.PARAMNAME == "LOCAL_CONVEYANCE_REQUEST_SELF_APPROVAL").Select(x => x.PARAMVALUE).FirstOrDefault().ToString();
                    var PPPLANT_ = (headerData.PPPLANT == null ? ((from k in _DB.VW_ASSOCIATELVLDETAILS_LC where k.SYKI == _Syki && k.ADEMPCODE == loginCode && k.ACTIVE == 1 select k.SYPLANTID).FirstOrDefault()) : headerData.PPPLANT);
                    string[] _desgIds = strDesginations.Split(',');
                    decimal ZeroFeul = 0;
                    int statusflag = 1;
                    if (_desgIds.Contains(LoginEmpDT._DesigId.ToString()))
                    {

                        lcHead.RECOMM_USER = loginCode;
                        lcHead.RECOMM_STATUS = "Approved";
                        lcHead.RECOMM_REMARKS = "Self Approved";
                        lcHead.RECOMM_DATETIME = DateTime.Now;
                        lcHead.APPROVER_USER = loginCode;
                        lcHead.APPROVER_STATUS = "Approved";
                        lcHead.APPROVER_REMARKS = "Self Approved";
                        lcHead.APPROVER_DATETIME = DateTime.Now;

                        if (lcHead.FUAL != ZeroFeul)
                        {
                            lcHead.ADMIN_USER = GetAdminUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                            lcHead.ADMIN_STATUS = "Pending";
                            ADEMPLOYEE_LC ARequester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                            ADEMPLOYEE_LC ANextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.ADMIN_USER && x.ACTIVE == 1).FirstOrDefault();
                            SendMailToNextAuthority(ARequester_, ANextApproval_, 3); // Admion = 3
                        }

                        lcHead.FINANCE_USER = GetFinanceUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                        lcHead.FINANCE_STATUS = "Pending";

                        ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                        ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.FINANCE_USER && x.ACTIVE == 1).FirstOrDefault();
                        SendMailToNextAuthority(Requester_, NextApproval_, 4); // Finance = 4


                    }
                    else
                    {
                        if ((UserAppAuth.SectionManager != null && UserAppAuth.SectionManager != 0) && UserAppAuth.SectionManager != loginCode)
                        {
                            //  var RecoUserID = GetApprovalUserID(loginCode, LoginEmpDT);

                            lcHead.RECOMM_USER = int.Parse(UserAppAuth.SectionManager.ToString());
                            lcHead.RECOMM_STATUS = "Pending";
                            ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                            ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.RECOMM_USER && x.ACTIVE == 1).FirstOrDefault();
                            SendMailToNextAuthority(Requester_, NextApproval_, 1); // Recommandation = 1

                            if ((UserAppAuth.DepartmentManager != null && UserAppAuth.DepartmentManager != 0) && UserAppAuth.DepartmentManager != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.DepartmentManager.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.DivisionHead != null && UserAppAuth.DivisionHead != 0) && UserAppAuth.DivisionHead != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.DivisionHead.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.OperationHead != null && UserAppAuth.OperationHead != 0) && UserAppAuth.OperationHead != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.OperationHead.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.Director.ToString());
                            }

                            lcHead.APPROVER_REMARKS = "";
                            lcHead.ADMIN_REMARKS = "";
                            lcHead.FINANCE_REMARKS = "";
                        }
                        else
                        {

                            lcHead.RECOMM_USER = loginCode;
                            lcHead.RECOMM_STATUS = "Approved";
                            lcHead.RECOMM_REMARKS = "Self Approved";
                            lcHead.RECOMM_DATETIME = DateTime.Now;
                            // lcHead.APPROVER_USER = GetApprovalUserID(loginCode, LoginEmpDT);
                            if ((UserAppAuth.DepartmentManager != null && UserAppAuth.DepartmentManager != 0) && UserAppAuth.DepartmentManager != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.DepartmentManager.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.DivisionHead != null && UserAppAuth.DivisionHead != 0) && UserAppAuth.DivisionHead != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.DivisionHead.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.OperationHead != null && UserAppAuth.OperationHead != 0) && UserAppAuth.OperationHead != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.OperationHead.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.Director != null && UserAppAuth.Director != 0) && UserAppAuth.Director != loginCode) // SR107643 :: LC ENHANCEMENT ::  AUMENTO
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.Director.ToString());
                            }
                            // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                            else
                            {
                                lcHead.APPROVER_USER = loginCode;
                            }
                            // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                            statusflag = 2;
                            lcHead.APPROVER_STATUS = "Pending";
                            lcHead.APPROVER_REMARKS = "";
                            lcHead.ADMIN_REMARKS = "";
                            lcHead.FINANCE_REMARKS = "";
                            // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                            Boolean isSendEmail = true;
                            //string[] DirDesigList = { "8", "27", "28", "29", "31", "34" };
                            string[] DirDesigList = _DB.SYPARAMETERS_LC.Where(v => v.PARAMNAME == "LOCAL_CONVEYANCE_OH_DESIGNATION").Select(v => v.PARAMVALUE).FirstOrDefault()?.Split(',') ?? new string[0];
                            string OHFunDesig = _DB.SYPARAMETERS_LC.Where(v => v.PARAMNAME == "LOCAL_CONVEYANCE_OH_FUN_DESIGNATION").Select(v => v.PARAMVALUE).FirstOrDefault();

                            if (LoginEmpDT.Functional_Designation_Id == OHFunDesig || DirDesigList.Contains(LoginEmpDT.Designation_Id))
                            {
                                if (LoginEmpDT.Functional_Designation_Id == OHFunDesig) //For OH
                                {
                                    LC_OH_DIRAPPROVALFLOW_MASTER obj = _DB.LC_OH_DIRAPPROVALFLOW_MASTER.Where(x => x.REQUESTOR_DESG == "OH").FirstOrDefault();

                                    if (obj.APPROVAL_DESG == "AUTO")
                                    {
                                        isSendEmail = false;
                                        lcHead.APPROVER_USER = loginCode;
                                        lcHead.APPROVER_STATUS = "Approved";
                                        lcHead.APPROVER_REMARKS = "Self Approve";


                                        if (lcHead.FUAL != ZeroFeul)
                                        {
                                            lcHead.ADMIN_USER = GetAdminUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                                            lcHead.ADMIN_STATUS = "Pending";
                                            ADEMPLOYEE_LC ARequester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                                            ADEMPLOYEE_LC ANextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.ADMIN_USER && x.ACTIVE == 1).FirstOrDefault();
                                            SendMailToNextAuthority(ARequester_, ANextApproval_, 3); // Admion = 3
                                        }

                                        lcHead.FINANCE_USER = GetFinanceUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                                        lcHead.FINANCE_STATUS = "Pending";

                                        ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                                        ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.FINANCE_USER && x.ACTIVE == 1).FirstOrDefault();
                                        SendMailToNextAuthority(Requester_, NextApproval_, 4); // Finance = 4
                                    }
                                    else
                                    {
                                        lcHead.APPROVER_USER = int.Parse(UserAppAuth.Director.ToString()); ;
                                        lcHead.APPROVER_STATUS = "Pending";
                                        lcHead.APPROVER_REMARKS = "";
                                    }
                                }

                                if (DirDesigList.Contains(LoginEmpDT.Designation_Id)) //For DIRECTOR
                                {
                                    LC_OH_DIRAPPROVALFLOW_MASTER obj = _DB.LC_OH_DIRAPPROVALFLOW_MASTER.Where(x => x.REQUESTOR_DESG == "DIRECTOR").FirstOrDefault();

                                    if (obj.APPROVAL_DESG == "AUTO")
                                    {
                                        isSendEmail = false;
                                        lcHead.APPROVER_USER = loginCode;
                                        lcHead.APPROVER_STATUS = "Approved";
                                        lcHead.APPROVER_REMARKS = "Self Approve";

                                        if (lcHead.FUAL != ZeroFeul)
                                        {
                                            lcHead.ADMIN_USER = GetAdminUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                                            lcHead.ADMIN_STATUS = "Pending";
                                            ADEMPLOYEE_LC ARequester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                                            ADEMPLOYEE_LC ANextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.ADMIN_USER && x.ACTIVE == 1).FirstOrDefault();
                                            SendMailToNextAuthority(ARequester_, ANextApproval_, 3); // Admion = 3
                                        }

                                        lcHead.FINANCE_USER = GetFinanceUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                                        lcHead.FINANCE_STATUS = "Pending";

                                        ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                                        ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.FINANCE_USER && x.ACTIVE == 1).FirstOrDefault();
                                        SendMailToNextAuthority(Requester_, NextApproval_, 4); // Finance = 4

                                    }
                                    else
                                    {


                                        string PresidentDesig = _DB.SYPARAMETERS_LC.Where(v => v.PARAMNAME == "LOCAL_CONVEYANCE_PRESIDENT").Select(v => v.PARAMVALUE).FirstOrDefault();
                                        long AdDesignationValue = Convert.ToInt32(PresidentDesig);

                                        //var President_obj = db.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADDESIGNATIONID == 12 && x.SYKI == _Syki && x.ACTIVE == 1).FirstOrDefault();
                                        var President_obj = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADDESIGNATIONID == AdDesignationValue && x.SYKI == _Syki && x.ACTIVE == 1).FirstOrDefault();

                                        if (President_obj != null)
                                        {
                                            lcHead.APPROVER_USER = President_obj.ADEMPCODE;
                                            lcHead.APPROVER_STATUS = "Pending";
                                            lcHead.APPROVER_REMARKS = "";
                                        }
                                        else
                                        {
                                            isSendEmail = false;
                                            lcHead.APPROVER_USER = loginCode;
                                            lcHead.APPROVER_STATUS = "Approved";
                                            lcHead.APPROVER_REMARKS = "Self Approve";

                                            if (lcHead.FUAL != ZeroFeul)
                                            {
                                                lcHead.ADMIN_USER = GetAdminUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                                                lcHead.ADMIN_STATUS = "Pending";
                                                ADEMPLOYEE_LC ARequester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                                                ADEMPLOYEE_LC ANextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.ADMIN_USER && x.ACTIVE == 1).FirstOrDefault();
                                                SendMailToNextAuthority(ARequester_, ANextApproval_, 3); // Admion = 3
                                            }

                                            lcHead.FINANCE_USER = GetFinanceUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                                            lcHead.FINANCE_STATUS = "Pending";

                                            ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                                            ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.FINANCE_USER && x.ACTIVE == 1).FirstOrDefault();
                                            SendMailToNextAuthority(Requester_, NextApproval_, 4); // Finance = 4

                                        }
                                    }
                                }
                            }

                            bool isSendToReco = false;
                            if (lcHead.FUAL != ZeroFeul)
                            {
                                if ((UserAppAuth.DivisionHead != null && UserAppAuth.DivisionHead != 0) && UserAppAuth.DivisionHead != loginCode)
                                {
                                    lcHead.RECOMM_USER = int.Parse(UserAppAuth.DivisionHead.ToString()); ;
                                    lcHead.RECOMM_STATUS = "Pending";
                                    lcHead.RECOMM_REMARKS = "";
                                    lcHead.RECOMM_DATETIME = null;

                                    if ((UserAppAuth.OperationHead != null && UserAppAuth.OperationHead != 0) && UserAppAuth.OperationHead != loginCode)
                                    {
                                        lcHead.APPROVER_USER = int.Parse(UserAppAuth.OperationHead.ToString());
                                        lcHead.APPROVER_STATUS = "";
                                        lcHead.APPROVER_REMARKS = "";
                                    }
                                    isSendToReco = true;
                                }
                            }

                            if (isSendEmail == true)
                            {
                                ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                                ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == (isSendToReco ? lcHead.RECOMM_USER : lcHead.APPROVER_USER) && x.ACTIVE == 1).FirstOrDefault();
                                int mailTyoe = isSendToReco ? 1 : 2;
                                SendMailToNextAuthority(Requester_, NextApproval_, mailTyoe);  // Approval/Recomm
                            }
                            // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                        }

                    }


                    lcHead.ADDEDBY = loginCode;
                    lcHead.ADDEDON = DateTime.Now;
                _DB.Entry(lcHead).State = EntityState.Added;
                _DB.SaveChanges();
                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lcHead;
        }

        private LC_HEAD UpdateLCHead_DT(LC_HEAD lcHead, LC_HEAD headerData, int loginCode, string purpose, int tempId, Employee_Details LoginEmpDT)
        {
            try
            {
                //using (var db = new LCEntities())
                //{
                    lcHead.TOTAL_AMOUNT = headerData.TOTAL_AMOUNT;
                    lcHead.TOTAL_AMT_IN_WORD = headerData.TOTAL_AMT_IN_WORD;
                    lcHead.REQUIRED_AMOUNT = headerData.TOTAL_AMOUNT;
                    lcHead.TOTAL_KMCOVERED = headerData.TOTAL_KMCOVERED;
                    lcHead.TOTAL_NOOFHOURS = headerData.TOTAL_NOOFHOURS;
                    lcHead.FUAL = headerData.FUAL;
                    lcHead.TOTAL_CONV_AMOUNT = headerData.TOTAL_CONV_AMOUNT;
                    lcHead.TOTAL_MEAL_AMOUNT = headerData.TOTAL_MEAL_AMOUNT;
                    lcHead.PURPOSE = purpose; //  Added by Aumento as on 26-06-2024 
                    lcHead.PPPLANT = headerData.PPPLANT;

                    /// var sectionIdCheck = db.VW_ASSOCIATELVLDETAILS.FirstOrDefault(x => x.ADEMPCODE == loginCode && x.SYKI == db.SYKI.Where(s => s.ACTIVE == 1).Select(s => s.SYKIID).FirstOrDefault() && x.ACTIVE == 1);

                    //CommonRepository comprepo = new CommonRepository();

                    UserApprovalAuthority UserAppAuth = _cmr.CheckApprovalAuthority(loginCode);

                    string strDesginations = _DB.SYPARAMETERS_LC.Where(x => x.PARAMNAME == "LOCAL_CONVEYANCE_REQUEST_SELF_APPROVAL").Select(x => x.PARAMVALUE).FirstOrDefault().ToString();
                    var PPPLANT_ = (headerData.PPPLANT == null ? ((from k in _DB.VW_ASSOCIATELVLDETAILS_LC where k.SYKI == _Syki && k.ADEMPCODE == loginCode && k.ACTIVE == 1 select k.SYPLANTID).FirstOrDefault()) : headerData.PPPLANT);
                    string[] _desgIds = strDesginations.Split(',');
                    decimal ZeroFeul = 0;
                    int statusflag = 1;
                    if (_desgIds.Contains(LoginEmpDT._DesigId.ToString()))
                    {

                        lcHead.RECOMM_USER = loginCode;
                        lcHead.RECOMM_STATUS = "Approved";
                        lcHead.RECOMM_REMARKS = "Self Approved";
                        lcHead.RECOMM_DATETIME = DateTime.Now;
                        lcHead.APPROVER_USER = loginCode;
                        lcHead.APPROVER_STATUS = "Approved";
                        lcHead.APPROVER_REMARKS = "Self Approved";
                        lcHead.APPROVER_DATETIME = DateTime.Now;

                        if (lcHead.FUAL != ZeroFeul)
                        {
                            lcHead.ADMIN_USER = GetAdminUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                            lcHead.ADMIN_STATUS = "Pending";
                            ADEMPLOYEE_LC ARequester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                            ADEMPLOYEE_LC ANextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.ADMIN_USER && x.ACTIVE == 1).FirstOrDefault();
                            SendMailToNextAuthority(ARequester_, ANextApproval_, 3); // Admion = 3
                        }

                        lcHead.FINANCE_USER = GetFinanceUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                        lcHead.FINANCE_STATUS = "Pending";

                        ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                        ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.FINANCE_USER && x.ACTIVE == 1).FirstOrDefault();
                        SendMailToNextAuthority(Requester_, NextApproval_, 4); // Finance = 4



                    }
                    else
                    {
                        if ((UserAppAuth.SectionManager != null && UserAppAuth.SectionManager != 0) && UserAppAuth.SectionManager != loginCode)
                        {
                            //  var RecoUserID = GetApprovalUserID(loginCode, LoginEmpDT);

                            lcHead.RECOMM_USER = int.Parse(UserAppAuth.SectionManager.ToString());
                            lcHead.RECOMM_STATUS = "Pending";
                            lcHead.RECOMM_REMARKS = "";
                            ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                            ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.RECOMM_USER && x.ACTIVE == 1).FirstOrDefault();
                            SendMailToNextAuthority(Requester_, NextApproval_, 1); // Recommandation = 1


                            if ((UserAppAuth.DepartmentManager != null && UserAppAuth.DepartmentManager != 0) && UserAppAuth.DepartmentManager != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.DepartmentManager.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.DivisionHead != null && UserAppAuth.DivisionHead != 0) && UserAppAuth.DivisionHead != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.DivisionHead.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.OperationHead != null && UserAppAuth.OperationHead != 0) && UserAppAuth.OperationHead != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.OperationHead.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.Director.ToString());
                            }

                            lcHead.APPROVER_REMARKS = "";
                            lcHead.ADMIN_REMARKS = "";
                            lcHead.FINANCE_REMARKS = "";
                        }
                        else
                        {

                            lcHead.RECOMM_USER = loginCode;
                            lcHead.RECOMM_STATUS = "Approved";
                            lcHead.RECOMM_REMARKS = "Self Approved";
                            lcHead.RECOMM_DATETIME = DateTime.Now;
                            // lcHead.APPROVER_USER = GetApprovalUserID(loginCode, LoginEmpDT);
                            if ((UserAppAuth.DepartmentManager != null && UserAppAuth.DepartmentManager != 0) && UserAppAuth.DepartmentManager != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.DepartmentManager.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.DivisionHead != null && UserAppAuth.DivisionHead != 0) && UserAppAuth.DivisionHead != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.DivisionHead.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.OperationHead != null && UserAppAuth.OperationHead != 0) && UserAppAuth.OperationHead != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.OperationHead.ToString());  //GetApprovalUserID(UserAppAuth.SectionManager, LoginEmpDT);
                            }
                            else if ((UserAppAuth.Director != null && UserAppAuth.Director != 0) && UserAppAuth.Director != loginCode)
                            {
                                lcHead.APPROVER_USER = int.Parse(UserAppAuth.Director.ToString());
                            }
                            else
                            {
                                lcHead.APPROVER_USER = loginCode;
                            }

                            lcHead.APPROVER_STATUS = "Pending";
                            lcHead.APPROVER_REMARKS = "";
                            // lcHead.ADMIN_REMARKS = ""; // SR107643 :: LC ENHANCEMENT ::  AUMENTO
                            lcHead.FINANCE_REMARKS = "";
                            // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                            //--=====================================================
                            Boolean isSendEmail = true;

                            //string[] DirDesigList = { "8", "27", "28", "29", "31", "34" };
                            string[] DirDesigList = _DB.SYPARAMETERS_LC.Where(v => v.PARAMNAME == "LOCAL_CONVEYANCE_OH_DESIGNATION").Select(v => v.PARAMVALUE).FirstOrDefault()?.Split(',') ?? new string[0];

                            string OHFunDesig = _DB.SYPARAMETERS_LC.Where(v => v.PARAMNAME == "LOCAL_CONVEYANCE_OH_FUN_DESIGNATION").Select(v => v.PARAMVALUE).FirstOrDefault();

                            if (LoginEmpDT.Functional_Designation_Id == OHFunDesig || DirDesigList.Contains(LoginEmpDT.Designation_Id))
                            {
                                if (LoginEmpDT.Functional_Designation_Id == OHFunDesig) //For OH
                                {

                                    LC_OH_DIRAPPROVALFLOW_MASTER obj = _DB.LC_OH_DIRAPPROVALFLOW_MASTER.Where(x => x.REQUESTOR_DESG == "OH").FirstOrDefault();

                                    if (obj.APPROVAL_DESG == "AUTO")
                                    {
                                        isSendEmail = false;
                                        lcHead.APPROVER_USER = loginCode;
                                        lcHead.APPROVER_STATUS = "Approved";
                                        lcHead.APPROVER_REMARKS = "Self Approve";
                                    }
                                    else
                                    {
                                        lcHead.APPROVER_USER = int.Parse(UserAppAuth.Director.ToString()); ;
                                        lcHead.APPROVER_STATUS = "Pending";
                                        lcHead.APPROVER_REMARKS = "";
                                    }
                                }

                                if (DirDesigList.Contains(LoginEmpDT.Designation_Id)) //For DIRECTOR
                                {
                                    LC_OH_DIRAPPROVALFLOW_MASTER obj = _DB.LC_OH_DIRAPPROVALFLOW_MASTER.Where(x => x.REQUESTOR_DESG == "DIRECTOR").FirstOrDefault();

                                    if (obj.APPROVAL_DESG == "AUTO")
                                    {
                                        isSendEmail = false;
                                        lcHead.APPROVER_USER = loginCode;
                                        lcHead.APPROVER_STATUS = "Approved";
                                        lcHead.APPROVER_REMARKS = "Self Approve";
                                    }
                                    else
                                    {
                                        string PresidentDesig = _DB.SYPARAMETERS_LC.Where(v => v.PARAMNAME == "LOCAL_CONVEYANCE_PRESIDENT").Select(v => v.PARAMVALUE).FirstOrDefault();
                                        long AdDesignationValue = Convert.ToInt32(PresidentDesig);
                                        // var President_obj = db.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADDESIGNATIONID == 12 && x.SYKI == _Syki && x.ACTIVE == 1).FirstOrDefault();
                                        var President_obj = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADDESIGNATIONID == AdDesignationValue && x.SYKI == _Syki && x.ACTIVE == 1).FirstOrDefault();
                                        if (President_obj != null)
                                        {
                                            lcHead.APPROVER_USER = President_obj.ADEMPCODE;
                                            lcHead.APPROVER_STATUS = "Pending";
                                            lcHead.APPROVER_REMARKS = "";
                                        }
                                        else
                                        {
                                            isSendEmail = false;
                                            lcHead.APPROVER_USER = loginCode;
                                            lcHead.APPROVER_STATUS = "Approved";
                                            lcHead.APPROVER_REMARKS = "Self Approve";
                                        }
                                    }
                                }
                            }

                            bool isSendToReco = false;
                            if (lcHead.FUAL != ZeroFeul)
                            {
                                if ((UserAppAuth.DivisionHead != null && UserAppAuth.DivisionHead != 0) && UserAppAuth.DivisionHead != loginCode)
                                {
                                    lcHead.RECOMM_USER = int.Parse(UserAppAuth.DivisionHead.ToString()); ;
                                    lcHead.RECOMM_STATUS = "Pending";
                                    lcHead.RECOMM_REMARKS = "";
                                    lcHead.RECOMM_DATETIME = null;

                                    if ((UserAppAuth.OperationHead != null && UserAppAuth.OperationHead != 0) && UserAppAuth.OperationHead != loginCode)
                                    {
                                        lcHead.APPROVER_USER = int.Parse(UserAppAuth.OperationHead.ToString());
                                        lcHead.APPROVER_STATUS = "";
                                        lcHead.APPROVER_REMARKS = "";
                                    }
                                    isSendToReco = true;
                                }
                            }
                            //--=====================================================

                            if (isSendEmail == true)
                            {
                                ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == lcHead.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                                ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == (isSendToReco ? lcHead.RECOMM_USER : lcHead.APPROVER_USER) && x.ACTIVE == 1).FirstOrDefault();
                                int mailTyoe = isSendToReco ? 1 : 2;
                                SendMailToNextAuthority(Requester_, NextApproval_, mailTyoe);  // Approval/Recomm
                            }

                        }
                    }



                    lcHead.UPDATEDBY = loginCode;
                    lcHead.UPDATEDON = DateTime.Now;
                _DB.Entry(lcHead).State = EntityState.Modified;
                // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                _DB.SaveChanges();
                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return lcHead;
        }



        private LC_DETAIL CreateLCDetail(LC_DETAIL_TEMP detailTemp_, LC_HEAD lcHead, int loginCode)
{
    LC_DETAIL lcDetail = null;



    try
    {
        //using (var _db = new LCEntities())
        //using (var _db = _DB)
        //{
        LC_DETAIL_TEMP detailTemp = _DB.LC_DETAIL_TEMP.FirstOrDefault(x => x.ASOFFODID == detailTemp_.ASOFFODID && x.DATEOFEXPENDITURE == detailTemp_.DATEOFEXPENDITURE && x.EMPCODE == detailTemp_.EMPCODE);

        lcDetail = _DB.LC_DETAIL.FirstOrDefault(x => x.ASOFFODID == detailTemp.ASOFFODID && x.DATEOFEXPENDITURE == detailTemp.DATEOFEXPENDITURE && x.EMPCODE == detailTemp.EMPCODE);

        if (lcDetail != null)
        {
            lcDetail = UpdateExistingLCDetail(lcDetail, detailTemp, loginCode);
            var flag = InsertLC_Detail_LOG(new List<LC_DETAIL> { lcDetail }, "Update");// SR107643 :: LC ENHANCEMENT ::  AUMENTO
        }
        else
        {
            lcDetail = AddNewLCDetail(detailTemp, lcHead, loginCode);
            var flag = InsertLC_Detail_LOG(new List<LC_DETAIL> { lcDetail }, "Insert"); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
        }
        //}
    }
    catch (DbUpdateException ex)
    {

        lcDetail = null;
    }
    catch (Exception ex)
    {

        lcDetail = null;
    }

    return lcDetail;
}

private LC_DETAIL UpdateExistingLCDetail(LC_DETAIL lcDetail, LC_DETAIL_TEMP detailTemp, int loginCode)
{
    try
    {
        //using (var db = new LCEntities())
        //using (var db = _DB)
        //{
        lcDetail.FROMCITY = detailTemp.FROMCITY;
        lcDetail.TOCITY = detailTemp.TOCITY;
        lcDetail.FROMCITYID = detailTemp.FROMCITYID;
        lcDetail.TOCITYID = detailTemp.TOCITYID;
        lcDetail.TIMEIN = detailTemp.TIMEIN;
        lcDetail.TIMEOUT = detailTemp.TIMEOUT;
        lcDetail.NOOFHOURS = detailTemp.NOOFHOURS;
        lcDetail.MODEOFTRAVEL = detailTemp.MODEOFTRAVEL;
        lcDetail.KMCOVERED = detailTemp.KMCOVERED;
        lcDetail.RATEPERKM = detailTemp.RATEPERKM;
        lcDetail.TRAVELREIMBURSEMENT = detailTemp.TRAVELREIMBURSEMENT;
        lcDetail.REFRESHMENTMEALALLOWANCE = detailTemp.REFRESHMENTMEALALLOWANCE;
        lcDetail.TOLLTAXMISCAMOUNT = detailTemp.TOLLTAXMISCAMOUNT;
        lcDetail.TOTAL = detailTemp.TOTAL;
        lcDetail.DT = detailTemp.DT;
        lcDetail.FUEL_REIMBURSMENT = detailTemp.FUEL_REIMBURSMENT;
        lcDetail.UPDATEDON = DateTime.Now;
        lcDetail.UPDATEDBY = loginCode;
        lcDetail.FROMCITY_OTHER = detailTemp.FROMCITY_OTHER;
        lcDetail.TOCITY_OTHER = detailTemp.TOCITY_OTHER;
        lcDetail.MODEOFTRAVEL_OTHER = detailTemp.MODEOFTRAVEL_OTHER;
        lcDetail.MEAL_REFRE_ALLW_CLAIM = detailTemp.MEAL_REFRE_ALLW_CLAIM;
        //lcDetail.STATUS = "Pending at approval";
        lcDetail.LUNCH_ALLW_CLAIM = detailTemp.LUNCH_ALLW_CLAIM;
        lcDetail.LUNCH_ALLW_AMOUNT = detailTemp.LUNCH_ALLW_AMOUNT;
        lcDetail.DINNER_ALLW_AMOUNT = detailTemp.DINNER_ALLW_AMOUNT;
        lcDetail.DINNER_ALLW_CLAIM = detailTemp.DINNER_ALLW_CLAIM;
        lcDetail.FUEL_ALLW_CLAIM = detailTemp.FUEL_ALLW_CLAIM;
        lcDetail.TOURSTARTFROMHOMEKM = detailTemp.TOURSTARTFROMHOMEKM;
        lcDetail.PURPOSE = detailTemp.PURPOSE; // Added by Aumento as on 26-06-2024 
        _DB.Entry(lcDetail).State = EntityState.Modified;

        _DB.SaveChanges();
        //}
    }
    catch (Exception ex)
    {

        throw ex;
    }

    return lcDetail;
}
private LC_DETAIL AddNewLCDetail(LC_DETAIL_TEMP detailTemp, LC_HEAD lcHead, int loginCode)
{
    LC_DETAIL lcDetail = null;
    try
    {
        //using (var db = new LCEntities())
        //using (var db = _DB)
        //{
        var maxSrnoDt = _DB.LC_DETAIL.Select(x => x.SRNO).AsEnumerable().DefaultIfEmpty(DefaultValues.MaxSrnoDt).Max() + 1;

        lcDetail = new LC_DETAIL
        {
            SRNO = maxSrnoDt,
            EMPCODE = detailTemp.EMPCODE,
            EMPNAME = detailTemp.EMPNAME,
            ASOFFODID = detailTemp.ASOFFODID,
            LCTRANDATE = lcHead.LCTRANDATE,
            LCTRANNO = lcHead.LCTRANNO,
            DATEOFEXPENDITURE = detailTemp.DATEOFEXPENDITURE,
            PURPOSE = detailTemp.PURPOSE,
            FROMCITY = detailTemp.FROMCITY,
            TOCITY = detailTemp.TOCITY,
            FROMCITYID = detailTemp.FROMCITYID,
            TOCITYID = detailTemp.TOCITYID,
            TIMEIN = detailTemp.TIMEIN,
            TIMEOUT = detailTemp.TIMEOUT,
            NOOFHOURS = detailTemp.NOOFHOURS,
            MODEOFTRAVEL = detailTemp.MODEOFTRAVEL,
            KMCOVERED = detailTemp.KMCOVERED,
            RATEPERKM = detailTemp.RATEPERKM,
            TRAVELREIMBURSEMENT = detailTemp.TRAVELREIMBURSEMENT,
            REFRESHMENTMEALALLOWANCE = detailTemp.REFRESHMENTMEALALLOWANCE,
            TOLLTAXMISCAMOUNT = detailTemp.TOLLTAXMISCAMOUNT,
            TOTAL = detailTemp.TOTAL,
            DT = detailTemp.DT,
            FUEL_REIMBURSMENT = detailTemp.FUEL_REIMBURSMENT,
            ADDEDON = DateTime.Now,
            ADDEDBY = loginCode,
            FROMCITY_OTHER = detailTemp.FROMCITY_OTHER,
            TOCITY_OTHER = detailTemp.TOCITY_OTHER,
            FUEL_ALLW_CLAIM = detailTemp.FUEL_ALLW_CLAIM == null ? "NO" : detailTemp.FUEL_ALLW_CLAIM.ToString(),
            MODEOFTRAVEL_OTHER = detailTemp.MODEOFTRAVEL_OTHER,
            MEAL_REFRE_ALLW_CLAIM = detailTemp.MEAL_REFRE_ALLW_CLAIM,
            LUNCH_ALLW_CLAIM = detailTemp.LUNCH_ALLW_CLAIM,
            LUNCH_ALLW_AMOUNT = detailTemp.LUNCH_ALLW_AMOUNT,
            DINNER_ALLW_AMOUNT = detailTemp.DINNER_ALLW_AMOUNT,
            DINNER_ALLW_CLAIM = detailTemp.DINNER_ALLW_CLAIM,
            TOURSTARTFROMHOMEKM = detailTemp.TOURSTARTFROMHOMEKM,
            // STATUS = "Pending at approval",
        };

        _DB.Entry(lcDetail).State = EntityState.Added;
        _DB.SaveChanges();
        // }


    }
    catch (Exception ex)
    {

        throw ex;
    }

    return lcDetail;
}


public string GetStateByEmpcode(int? EmpCode)
{
    string state = string.Empty;

    //ConnectionString objCnStr = new ConnectionString();
    string strCn = serverpath.getConnectingString();

    using (OracleConnection objCn = new OracleConnection())
    {
        objCn.ConnectionString = strCn;
        try
        {
            objCn.Open();
            OracleCommand objCmd = new OracleCommand();
            objCmd.Connection = objCn;
            objCmd.CommandText = "PKG_OFFOUTDUTYTRANS.SPROC_LC_GETSTATE_BY_EMPCODE";
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;

            // Input parameter
            objCmd.Parameters.Add("P_EMPCODE", OracleDbType.Int32).Value = EmpCode;

            // Output parameter
            OracleParameter outputParam = new OracleParameter("P_STATE", OracleDbType.Varchar2);
            outputParam.Direction = ParameterDirection.Output;
            outputParam.Size = 255; // Adjust the size according to your needs
            objCmd.Parameters.Add(outputParam);

            objCmd.ExecuteNonQuery();

            // Check for DBNull before converting to string
            state = objCmd.Parameters["P_STATE"].Value != DBNull.Value
                ? objCmd.Parameters["P_STATE"].Value.ToString()
                : string.Empty;
        }
        catch (Exception ex)
        {
            // Handle the exception appropriately (e.g., log it)
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        finally
        {
            if (objCn != null && objCn.State == ConnectionState.Open)
            {
                objCn.Close();
            }
        }
    }

    return state;
}

public bool UpdateLCDetailTemp(LC_DETAIL dt)
{
    bool res = false;
    try
    {


        //using (var _db = new LCEntities())
        //using (var _db = _DB)
        //{
        LC_DETAIL_TEMP exist = _DB.LC_DETAIL_TEMP.Where(x => x.ASOFFODID == dt.ASOFFODID && x.EMPCODE == dt.EMPCODE && x.DATEOFEXPENDITURE == dt.DATEOFEXPENDITURE).FirstOrDefault(); // Added DATEOFEXPENDITURE field in Condtion on 01-05-2024 
        _DB.Entry(exist).Reload();
        exist.ISSUBMITTED = 1;
        exist.TEMPID = dt.LCTRANNO;
        exist.UPDATEDBY = dt.EMPCODE;
        exist.UPDATEDON = DateTime.Now;
        _DB.Entry(exist).State = EntityState.Modified;
        _DB.SaveChanges();
        res = true;
        //}

    }
    catch (Exception ex)
    {
        res = false;

    }
    return res;
}

public Tuple<string, string> GetDateRange()
{
    DateTime currentDate = DateTime.Now;
    DateTime endDate = currentDate.Date;

    string strparam = "LOCAL_CONVEYANCE_REQUEST_ALLOW_DAYS";

    var Count = (from v in _DB.SYPARAMETERS_LC where v.PARAMNAME == strparam select v.PARAMVALUE).FirstOrDefault();

    int intcount = int.Parse(Count);

    DateTime startDate = endDate.AddDays(-intcount);

    return Tuple.Create(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
}

// New Function Added by Aumento as on 21-06-2024
public int GetMonthRangeCount()
{
    int Count = 0;

    string strparam = "LOCAL_CONVEYANCE_REQUEST_ALLOW_MONTHS";

    var CountData = (from v in _DB.SYPARAMETERS_LC where v.PARAMNAME == strparam select v.PARAMVALUE).FirstOrDefault();

    Count = int.Parse(CountData);

    return Count;
}
// New Function Added by Aumento as on 21-06-2024

public Tuple<TimeSpan, TimeSpan, TimeSpan, TimeSpan> GetLunchDinnerTime()
{
    string Lunch = "LOCAL_CONVEYANCE_REQUEST_LUNCH_TIME";
    string Dinner = "LOCAL_CONVEYANCE_REQUEST_DINNER_TIME";

    var LunchDinnerTime = (from v in _DB.SYPARAMETERS_LC
                           where v.PARAMNAME == Lunch || v.PARAMNAME == Dinner
                           select v.PARAMVALUE).ToList();

    // Assuming the format of LunchDinnerTime is "HH:mm,HH:mm"
    var times = LunchDinnerTime.SelectMany(time => time.Split(',')).Select(time => time.Split(':').Select(int.Parse).ToArray()).ToArray();

    TimeSpan LunchStartTime = new TimeSpan(times[0][0], times[0][1], 0);
    TimeSpan LunchEndTime = new TimeSpan(times[1][0], times[1][1], 0);
    TimeSpan DinnerStartTime = new TimeSpan(times[2][0], times[2][1], 0);
    TimeSpan DinnerEndTime = new TimeSpan(times[3][0], times[3][1], 0);

    return Tuple.Create(LunchStartTime, LunchEndTime, DinnerStartTime, DinnerEndTime);
}


public void GetAllowanceAndRate_FUEL(int? empCode, out decimal fuelReimbursement, out decimal mealAllowance, out decimal refreshAllowance, out decimal twoWheelRatePerKM, out decimal fourWheelRatePerKM, out decimal fuelReimbursementFromException)
{
    fuelReimbursement = 0;
    mealAllowance = 0;
    refreshAllowance = 0;
    twoWheelRatePerKM = 0;
    fourWheelRatePerKM = 0;
    fuelReimbursementFromException = 0;

    var state = GetStateByEmpcode(empCode);

    var LoginSiteId = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == empCode && x.SYKI == _Syki).Select(x => x.SYSITEID).FirstOrDefault(); //Added by Aumento as on 01082029

    // from Z table 
    //var data = (from r in _DB.LC_ZTABLE where r.STATE == state select r).FirstOrDefault();  //Added by Aumento as on 01082029
    //var data = (from r in _DB.LC_ZTABLE where r.SYSITEID == LoginSiteId select r).FirstOrDefault(); //Added by Aumento as on 01082029 // SR107643 :: LC ENHANCEMENT ::  AUMENTO
    var data = (from r in _DB.LC_ZTABLE where r.SYSITEID == LoginSiteId && r.ACTIVE == 1 select r).FirstOrDefault(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO

    if (data != null)
    {
        twoWheelRatePerKM = decimal.Parse(data.TWOWHEELER_RATE.ToString());
        fourWheelRatePerKM = decimal.Parse(data.FOURWHEELER_RATE.ToString());
    }

    // from exception table 
    //var exceptionFound = (from i in _DB.LC_EXCEPTION_C_TABLE where i.EMPCODE == empCode select i).FirstOrDefault(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
    var exceptionFound = (from i in _DB.LC_EXCEPTION_C_TABLE where i.EMPCODE == empCode && i.ACTIVE == 1 select i).FirstOrDefault(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO


    if (exceptionFound != null)
    {
        fuelReimbursementFromException = decimal.Parse(exceptionFound.FUEL_REIMBUR_PER_LTR.ToString());
    }


    // from B table 
    //var designationIds = _DB.VW_ASSOCIATELVLDETAILS.Where(i => i.ACTIVE == 1  && i.SYKI == _Syki.SYKIID && i.ADEMPCODE == empCode) .Select(i => i.ADDESIGNATIONID).ToList();
    //var adfunID = designationIds.FirstOrDefault(id => new string[] { "19", "16", "33", "3", "7", "13", "2", "11" }.Contains(id.ToString()));

    //if (adfunID != null)
    //{

    var adfunID = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(i => i.ACTIVE == 1 && i.SYKI == _Syki && i.ADEMPCODE == empCode).Select(i => i.ADDESIGNATIONID).FirstOrDefault();
    //var data_ = (from r in _DB.LC_REIMBURSEMENT_B_TABLE where r.DESIGNATIONID == adfunID select r).FirstOrDefault(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO

    var data_ = (from r in _DB.LC_REIMBURSEMENT_B_TABLE where r.DESIGNATIONID == adfunID && r.ACTIVE == 1 select r).FirstOrDefault();// SR107643 :: LC ENHANCEMENT ::  AUMENTO
    if (data_ != null)
    {
        fuelReimbursement = decimal.Parse(data_.FUEL_REIMBUR_PER_LTR.ToString());
        mealAllowance = decimal.Parse(data_.MEAL_ALLOWANCE.ToString());
        refreshAllowance = decimal.Parse(data_.REFRESH_ALLOW_PER_DAY.ToString());
    }

    // }
}


public LocalConveyanceRequestViewModel GetReqData(DateTime ExDate, int LoginEmp)
{
    LocalConveyanceRequestViewModel res = new LocalConveyanceRequestViewModel();

    try
    {
        var data = _DB.LC_DETAIL_TEMP
                          .FirstOrDefault(x => x.DATEOFEXPENDITURE == ExDate && x.ADDEDBY == LoginEmp);

        if (data != null)
        {
            res.SRNO = int.Parse(data.SRNO.ToString());
            res.EMPCODE = data.EMPCODE.ToString();
            res.EMPNAME = data.EMPNAME;
            res.ASOFFODID = data.ASOFFODID.ToString(); // Ensure ASOFFODID is string in your view model if necessary
            res.DATEOFEXPENDITURE = data.DATEOFEXPENDITURE.ToString(); // Convert DateTime to string as needed

            res.PURPOSE = data.PURPOSE;
            res.FROMCITY = data.FROMCITY;
            res.TOCITY = data.TOCITY;
            res.FROMCITYID = data.FROMCITYID.ToString(); // Ensure FROMCITYID is string in your view model if necessary
            res.TOCITYID = data.TOCITYID.ToString(); // Ensure TOCITYID is string in your view model if necessary

            res.TIMEIN = data.TIMEIN;
            res.TIMEOUT = data.TIMEOUT;
            res.NOOFHOURS = data.NOOFHOURS.ToString(); // Ensure NOOFHOURS is int in your view model if necessary
            res.MODEOFTRAVEL = data.MODEOFTRAVEL;
            res.KMCOVERED = data.KMCOVERED.ToString(); // Ensure KMCOVERED is string in your view model if necessary
            res.RATEPERKM = data.RATEPERKM.ToString(); // Ensure RATEPERKM is string in your view model if necessary
            res.TRAVELREIMBURSEMENT = data.TRAVELREIMBURSEMENT.ToString(); // Ensure TRAVELREIMBURSEMENT is string in your view model if necessary
            res.TOLLTAXMISCAMOUNT = decimal.Parse(data.TOLLTAXMISCAMOUNT.ToString()); // Ensure TOLLTAXMISCAMOUNT is correctly mapped                 
            res.DT = data.DT;
            res.FROMCITY_OTHER = data.FROMCITY_OTHER;
            res.TOCITY_OTHER = data.TOCITY_OTHER;
            res.MODEOFTRAVEL_OTHER = data.MODEOFTRAVEL_OTHER;
            res.MEAL_REFRE_ALLW_CLAIM = data.MEAL_REFRE_ALLW_CLAIM;
            res.REFRESHMENTMEALALLOWANCE = data.REFRESHMENTMEALALLOWANCE.ToString(); // Ensure REFRESHMENTMEALALLOWANCE is string in your view model if necessary
            res.FUEL_ALLW_CLAIM = data.FUEL_ALLW_CLAIM;
            res.FUEL_REIMBURSMENT = data.FUEL_REIMBURSMENT.ToString(); // Ensure FUEL_REIMBURSMENT is string in your view model if necessary
            res.DINNER_ALLW_CLAIM = data.DINNER_ALLW_CLAIM;
            res.DINNER_ALLW_AMOUNT = data.DINNER_ALLW_AMOUNT.ToString(); // Ensure DINNER_ALLW_AMOUNT is string in your view model if necessary
            res.LUNCH_ALLW_CLAIM = data.LUNCH_ALLW_CLAIM;
            res.LUNCH_ALLW_AMOUNT = data.LUNCH_ALLW_AMOUNT.ToString(); // Ensure LUNCH_ALLW_AMOUNT is string in your view model if necessary
            res.TOURSTARTFROMHOMEKM = data.TOURSTARTFROMHOMEKM.ToString(); // Ensure TOURSTARTFROMHOMEKM is string in your view model if necessary
        }
    }
    catch (Exception ex)
    {

        res = null;
    }

    return res;
}



#endregion


#region Local Conveyance Recommandation Functions


public List<LC_HEAD> GetListDataForReco(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
{

    List<LC_HEAD> ilist = new List<LC_HEAD>();
    try
    {

        ilist = _DB.LC_HEAD.Where(x => x.RECOMM_USER == LoginCode
                                                    && x.RECOMM_STATUS == Status
                                                    && x.LCTRANDATE >= FromDate && x.LCTRANDATE <= ToDate
                                                    && (Ecode == 0 || x.EMPCODE == Ecode)).ToList();

    }
    catch (Exception ex)
    {
        throw ex;
    }
    return ilist;
}

public LibResult GetRequestDataForReco(int LCTRANNO, int EMPCODE)
{
    LibResult Res = new LibResult();


    try
    {
        var HeaderData = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO).FirstOrDefault();
        var Empdata = GetEmployeeDATA(EMPCODE);
        var DtData = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).OrderBy(x => x.DATEOFEXPENDITURE).ToList(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO

        var DATA = new
        {
            HeaderData = HeaderData,
            DtData = DtData,
            Empdata = Empdata
        };

        Res.resultObject = DATA;
        Res.hasError = false;

    }
    catch (Exception ex)
    {
        Res.hasError = true;
        throw ex;
    }


    return Res;

}

public LibResult ApproveRejectReco(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode)
{
    LibResult Res = new LibResult();
    // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
    using (var transaction = _DB.Database.BeginTransaction())
    {

        try
        {
            int statusflag = 1;
            LC_HEAD hd = new LC_HEAD();
            hd = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE).FirstOrDefault();
            if (hd == null)
            {
                Res.hasError = true;
                Res.errorMessage = "Record not found.";
                return Res;
            }
            _DB.Entry(hd).Reload();
            if (Response == "Approved")
            {
                // hd.APPROVER_USER = GetApprovalUserID(loginCode,LoginEmpDT);
                hd.APPROVER_STATUS = "Pending";
                statusflag = 2;
                ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.APPROVER_USER && x.ACTIVE == 1).FirstOrDefault();

                SendMailToNextAuthority(Requester_, NextApproval_, 2); // Approval = 2 
            }
            else if (Response == "SendBack")
            {
                statusflag = 3;

            }
            else
            {
                statusflag = 4;
            }

            hd.RECOMM_REMARKS = Remark;
            hd.RECOMM_STATUS = Response;
            hd.RECOMM_DATETIME = DateTime.Now;
            hd.UPDATEDBY = loginCode;
            hd.UPDATEDON = DateTime.Now;
            _DB.Entry(hd).State = EntityState.Modified;
            _DB.SaveChanges();

            var flag = InsertLC_HEAD_LOG(hd, "Update"); //  LC ENHANCEMENT ::  AUMENTO

            Res.hasError = false;
            Res.errorMessage = "Response submitted successfully";

            ADEMPLOYEE_LC Requester = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
            ADEMPLOYEE_LC NextApproval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.APPROVER_USER && x.ACTIVE == 1).FirstOrDefault();
            ADEMPLOYEE_LC Curr_Approval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();

            SendMailToRequester(Requester, Curr_Approval, statusflag, 1, Remark); // recommandation = 1





            List<LC_DETAIL> dtlist = _DB.LC_DETAIL.Where(x => x.LCTRANNO == hd.LCTRANNO).ToList();

            foreach (var item in dtlist)
            {
                // LC_DETAIL_TEMP exist = dbi.LC_DETAIL_TEMP.Where(x => x.ASOFFODID == item.ASOFFODID && x.DATEOFEXPENDITURE == item.DATEOFEXPENDITURE).FirstOrDefault();
                LC_DETAIL_TEMP exist = _DB.LC_DETAIL_TEMP.Where(x => x.TEMPID == item.LCTRANNO && x.ASOFFODID == item.ASOFFODID).FirstOrDefault();
                // dbi.Entry(exist).Reload();

                if (exist != null)
                {
                    exist.ISSUBMITTED = statusflag;
                    exist.UPDATEDON = DateTime.Now;
                    _DB.Entry(exist).State = EntityState.Modified;
                }
                //exist.ISSUBMITTED = statusflag;
                //exist.UPDATEDON = DateTime.Now;
                //_DB.Entry(exist).State = System.Data.Entity.EntityState.Modified;
            }
            _DB.SaveChanges();




            _DB.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Res.hasError = true;
            Res.errorMessage = "Error: " + ex.Message;
        }
    }
    // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
    return Res;

}

#endregion

#region Local Conveyance Approval Functions


public List<LC_HEAD> GetListDataForApproval(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
{

    List<LC_HEAD> ilist = new List<LC_HEAD>();
    try
    {

        ilist = _DB.LC_HEAD.Where(x => x.APPROVER_USER == LoginCode
                                                    && x.APPROVER_STATUS == Status
                                                    && x.LCTRANDATE >= FromDate && x.LCTRANDATE <= ToDate
                                                    && (Ecode == 0 || x.EMPCODE == Ecode)).ToList();

    }
    catch (Exception ex)
    {
        throw ex;
    }
    return ilist;
}

public LibResult GetRequestDataForApproval(int LCTRANNO, int EMPCODE)
{
    LibResult Res = new LibResult();


    try
    {
        var HeaderData = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO).FirstOrDefault();
        var Empdata = GetEmployeeDATA(EMPCODE);
        var DtData = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).OrderBy(x => x.DATEOFEXPENDITURE).ToList(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO

        var DATA = new
        {
            HeaderData = HeaderData,
            DtData = DtData,
            Empdata = Empdata
        };

        Res.resultObject = DATA;
        Res.hasError = false;

    }
    catch (Exception ex)
    {
        Res.hasError = true;
        throw ex;
    }


    return Res;

}

public LibResult ApproveReject(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode)
{
    LibResult Res = new LibResult();
    // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
    using (var transaction = _DB.Database.BeginTransaction())
    {
        try
        {
            decimal ZeroFeul = 0;

            int statusflag = 1;

            LC_HEAD hd = new LC_HEAD();

            try
            {
                hd = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE).FirstOrDefault();

                if (hd == null)
                {
                    Res.hasError = true;
                    Res.errorMessage = "Record not found.";
                    return Res;
                }
                _DB.Entry(hd).Reload();


                hd.APPROVER_REMARKS = Remark;
                hd.APPROVER_STATUS = Response;
                hd.APPROVER_DATETIME = DateTime.Now;
                var PPPLANT_ = (hd.PPPLANT == null ? ((from k in _DB.VW_ASSOCIATELVLDETAILS_LC where k.SYKI == _Syki && k.ADEMPCODE == loginCode && k.ACTIVE == 1 select k.SYPLANTID).FirstOrDefault()) : hd.PPPLANT);
                var Admin_User = GetAdminUserID(loginCode, int.Parse(PPPLANT_.ToString())); //  LC ENHANCEMENT ::  AUMENTO

                if (Response == "Approved")
                {// START :: LC ENHANCEMENT ::  AUMENTO
                    if (hd.FUAL != ZeroFeul && hd.ADMIN_USER != Admin_User && hd.ADMIN_STATUS != "Approved")
                    {
                        //hd.ADMIN_USER = GetAdminUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                        hd.ADMIN_USER = Admin_User;
                        hd.ADMIN_STATUS = "Pending";
                        // END :: LC ENHANCEMENT ::  AUMENTO

                        ADEMPLOYEE_LC Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                        ADEMPLOYEE_LC NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.ADMIN_USER && x.ACTIVE == 1).FirstOrDefault();
                        ADEMPLOYEE_LC Curr_Approval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();
                        SendMailToNextAuthority(Requester_, NextApproval_, 3); // Admin = 3
                    }

                    hd.FINANCE_USER = GetFinanceUserID(loginCode, int.Parse(PPPLANT_.ToString()));
                    hd.FINANCE_STATUS = "Pending";
                    statusflag = 2;

                    ADEMPLOYEE_LC _Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                    ADEMPLOYEE_LC _NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.FINANCE_USER && x.ACTIVE == 1).FirstOrDefault();
                    SendMailToNextAuthority(_Requester_, _NextApproval_, 4); //  // Finance = 4


                }
                else if (Response == "SendBack")
                {

                    statusflag = 3;
                }
                else
                {
                    statusflag = 4;
                }


                hd.UPDATEDBY = loginCode;
                hd.UPDATEDON = DateTime.Now;
                _DB.Entry(hd).State = EntityState.Modified;
                _DB.SaveChanges();
                var flag = InsertLC_HEAD_LOG(hd, "Update"); // LC ENHANCEMENT ::  AUMENTO
                Res.hasError = false;
                Res.errorMessage = "Response submitted successfully";

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                Res.hasError = true;
                Res.errorMessage = "Error: " + ex.Message;
            }

            ADEMPLOYEE_LC Requester = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
            ADEMPLOYEE_LC Curr_Approval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();

            SendMailToRequester(Requester, Curr_Approval, statusflag, 2, Remark); // Approval = 2



            List<LC_DETAIL> dtlist = _DB.LC_DETAIL.Where(x => x.LCTRANNO == hd.LCTRANNO).ToList();

            foreach (var item in dtlist)
            {
                //LC_DETAIL_TEMP exist = dbi.LC_DETAIL_TEMP.Where(x => x.ASOFFODID == item.ASOFFODID && x.DATEOFEXPENDITURE == item.DATEOFEXPENDITURE).FirstOrDefault();
                LC_DETAIL_TEMP exist = _DB.LC_DETAIL_TEMP.Where(x => x.TEMPID == item.LCTRANNO && x.ASOFFODID == item.ASOFFODID).FirstOrDefault();

                // _DB.Entry(exist).Reload();

                if (exist != null)
                {
                    exist.ISSUBMITTED = statusflag;
                    exist.UPDATEDON = DateTime.Now;
                    _DB.Entry(exist).State = EntityState.Modified;
                }
                //exist.ISSUBMITTED = statusflag;
                //    exist.UPDATEDON = DateTime.Now;
                //    _DB.Entry(exist).State = System.Data.Entity.EntityState.Modified;
            }




            _DB.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Res.hasError = true;
            Res.errorMessage = "Error: " + ex.Message;
        }
    }
    // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
    return Res;

}


#endregion

#region Local Conveyance Admin Functions


public List<LC_HEAD> GetListDataForAdmin(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
{

    List<LC_HEAD> ilist = new List<LC_HEAD>();
    try
    {


        ilist = _DB.LC_HEAD.Where(x => x.ADMIN_USER == LoginCode
                                              && x.ADMIN_STATUS == Status
                                                 && x.LCTRANDATE >= FromDate && x.LCTRANDATE <= ToDate
                                                && (Ecode == 0 || x.EMPCODE == Ecode)).ToList();



    }
    catch (Exception ex)
    {
        throw ex;
    }
    return ilist;
}

public LibResult GetRequestDataForAdmin(int LCTRANNO, int EMPCODE)
{
    LibResult Res = new LibResult();


    try
    {
        var HeaderData = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO).FirstOrDefault();
        var Empdata = GetEmployeeDATA(EMPCODE);
        var DtData = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).ToList();

        var DATA = new
        {
            HeaderData = HeaderData,
            DtData = DtData,
            Empdata = Empdata
        };

        Res.resultObject = DATA;
        Res.hasError = false;

    }
    catch (Exception ex)
    {
        Res.hasError = true;
        throw ex;
    }


    return Res;

}

    public LibResult AdminSubmit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode, string Fuel, string doc, string approvetype) // {doc,approvetype added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
{
    LibResult Res = new LibResult();


    try
    {
        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        //CommonRepository comprepo = new CommonRepository();
        UserApprovalAuthority UserAppAuth = _cmr.CheckApprovalAuthority(loginCode);
        decimal AdminApprovedFuel = decimal.Parse(Fuel);

        //using (var _db = new LCEntities())
        //using (var _db = _DB)
        //{
        LC_HEAD hd = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE).FirstOrDefault();
        _DB.Entry(hd).Reload();
        hd.ADMIN_REMARKS = Remark;
        hd.ADMIN_STATUS = Response;
        hd.ADMIN_DATETIME = DateTime.Now;
        hd.ADMINFUEL_DOC = doc;
        hd.ADMIN_APPROVAL_TYPE = approvetype;
        hd.APPROVED_FUEL = AdminApprovedFuel;
        hd.UPDATEDBY = loginCode;
        hd.UPDATEDON = DateTime.Now;

        //if(AdminApprovedFuel > 0)
        //{
        //    hd.ADMINFUEL_DH_USER = UserAppAuth.DivisionHead;
        //    hd.ADMINFUEL_DH_REMARKS = "";
        //    hd.ADMINFUEL_DH_STATUS = "Pending";
        //    hd.ADMINFUEL_DH_DATETIME = null;                       
        //}
        if (AdminApprovedFuel > 0 && ((approvetype == "2") || (approvetype == "3")))
        {
            hd.ADMINFUEL_DH_USER = UserAppAuth.DivisionHead;
            hd.ADMINFUEL_DH_REMARKS = "";
            hd.ADMINFUEL_DH_STATUS = "Pending";
            hd.ADMINFUEL_DH_DATETIME = null;
        }
        if (AdminApprovedFuel > 0 && approvetype == "1")
        {
            hd.ADMIN_REMARKS = Remark;
            hd.ADMIN_STATUS = "Approved";
            hd.ADMIN_DATETIME = DateTime.Now;
        }
        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

        int statusflag = 2;

        if (Response == "Approved")
        {

            statusflag = 2;

        }
        else if (Response == "SendBack")
        {

            statusflag = 3;
        }
        else
        {
            statusflag = 4;
        }

        //using (var dbi = new LCEntities())
        //using (var dbi = _DB)
        //{

        List<LC_DETAIL> dtlist = _DB.LC_DETAIL.Where(x => x.LCTRANNO == hd.LCTRANNO).ToList();

        foreach (var item in dtlist)
        {
            LC_DETAIL_TEMP exist = _DB.LC_DETAIL_TEMP.Where(x => x.ASOFFODID == item.ASOFFODID && x.DATEOFEXPENDITURE == item.DATEOFEXPENDITURE).FirstOrDefault();

            _DB.Entry(exist).Reload();
            exist.ISSUBMITTED = statusflag;
            exist.UPDATEDON = DateTime.Now;
            _DB.Entry(exist).State = EntityState.Modified;
        }
        _DB.SaveChanges();

        //};



        _DB.Entry(hd).State = EntityState.Modified;
        _DB.SaveChanges();
        var flag = InsertLC_HEAD_LOG(hd, "Update");  //   SR107643 :: LC ENHANCEMENT ::  AUMENTO
        Res.hasError = false;
        Res.errorMessage = "Response submitted successfully";
        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        if (statusflag == 2 && AdminApprovedFuel > 0 && approvetype != "1")
        {
            ADEMPLOYEE_LC _Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
            ADEMPLOYEE_LC _NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.ADMINFUEL_DH_USER && x.ACTIVE == 1).FirstOrDefault();
            SendMailToNextAuthority(_Requester_, _NextApproval_, 5); //  // DH = 5
        }
        else
        {
            ADEMPLOYEE_LC Requester = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
            ADEMPLOYEE_LC Curr_Approval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();
            SendMailToRequester(Requester, Curr_Approval, statusflag, 3, Remark); // Admin = 3
        }

        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
    }
    catch (Exception ex)
    {
        Res.hasError = true;
        Res.errorMessage = ex.Message.ToString();
        throw ex;
    }


    return Res;

}

public LibResult AdminSaveDetail(decimal FUEL_Reimburse, int EMPCODE, int ASOFFODID, DateTime Date, int loginCode)
{
    LibResult Res = new LibResult();


    try
    {

        //using (var _db = new LCEntities())
        //using (var _db = _DB)
        //{
        LC_DETAIL dt = _DB.LC_DETAIL.Where(x => x.ASOFFODID == ASOFFODID && x.EMPCODE == EMPCODE && x.DATEOFEXPENDITURE == Date).FirstOrDefault();
        _DB.Entry(dt).Reload();
        dt.FUEL_REIMBURSMENT = FUEL_Reimburse;
        dt.UPDATEDBY = loginCode;
        dt.UPDATEDON = DateTime.Now;
        _DB.Entry(dt).State = EntityState.Modified;
        _DB.SaveChanges();
        Res.hasError = false;
        Res.errorMessage = "Saved";
        //}



    }
    catch (Exception ex)
    {
        Res.hasError = true;
        Res.errorMessage = ex.Message.ToString();
        throw ex;
    }


    return Res;

}

#endregion

// START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
#region Local Conveyance DH Functions
public List<LC_HEAD> GetListDataForAdminDH(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
{
    List<LC_HEAD> ilist = new List<LC_HEAD>();
    try
    {

        ilist = _DB.LC_HEAD.Where(x => x.ADMINFUEL_DH_USER == LoginCode
                                              && x.ADMINFUEL_DH_STATUS == Status
                                                 && x.LCTRANDATE >= FromDate && x.LCTRANDATE <= ToDate
                                                && (Ecode == 0 || x.EMPCODE == Ecode)).ToList();

    }
    catch (Exception ex)
    {
        throw ex;
    }
    return ilist;
}

public LibResult GetRequestDataForAdminDH(int LCTRANNO, int EMPCODE)
{
    LibResult Res = new LibResult();
    try
    {
        var HeaderData = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO).FirstOrDefault();
        var Empdata = GetEmployeeDATA(EMPCODE);
        var DtData = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).OrderBy(x => x.DATEOFEXPENDITURE).ToList();


        var DATA = new
        {
            HeaderData = HeaderData,
            DtData = DtData,
            Empdata = Empdata
        };

        Res.resultObject = DATA;
        Res.hasError = false;

    }
    catch (Exception ex)
    {
        Res.hasError = true;
        throw ex;
    }
    return Res;
}

public LibResult AdminDH_Submit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode)
{
    LibResult Res = new LibResult();


    try
    {
        //CommonRepository comprepo = new CommonRepository();
        UserApprovalAuthority UserAppAuth = _cmr.CheckApprovalAuthority(loginCode);
       
            LC_HEAD hd = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE).FirstOrDefault();
                _DB.Entry(hd).Reload();
            hd.ADMINFUEL_DH_REMARKS = Remark;
            hd.ADMINFUEL_DH_STATUS = Response;
            hd.ADMINFUEL_DH_DATETIME = DateTime.Now;

            hd.UPDATEDBY = loginCode;
            hd.UPDATEDON = DateTime.Now;


            int statusflag = 2;

            if (Response == "Approved")
            {
                if (hd.ADMIN_APPROVAL_TYPE == "2")
                {
                    hd.ADMINFUEL_DH_REMARKS = Remark;
                    hd.ADMINFUEL_DH_STATUS = "Approved";
                    hd.ADMINFUEL_DH_DATETIME = DateTime.Now;
                }
                else
                {

                    hd.ADMINFUEL_OH_USER = UserAppAuth.OperationHead;
                    hd.ADMINFUEL_OH_REMARKS = "";
                    hd.ADMINFUEL_OH_STATUS = "Pending";
                    hd.ADMINFUEL_OH_DATETIME = null;
                }
                statusflag = 2;
            }
            else if (Response == "SendBack")
            {
                hd.ADMIN_STATUS = "Pending";
                hd.ADMIN_REMARKS = "";
                hd.ADMIN_DATETIME = null;
                statusflag = 3;
            }
            else
            {
                statusflag = 4;
            }

                //using (var dbi = new LCEntities())
                //{

                //    List<LC_DETAIL> dtlist = dbi.LC_DETAIL.Where(x => x.LCTRANNO == hd.LCTRANNO).ToList();

                //    foreach (var item in dtlist)
                //    {
                //        LC_DETAIL_TEMP exist = dbi.LC_DETAIL_TEMP.Where(x => x.ASOFFODID == item.ASOFFODID && x.DATEOFEXPENDITURE == item.DATEOFEXPENDITURE).FirstOrDefault();

                //        dbi.Entry(exist).Reload();
                //        exist.ISSUBMITTED = statusflag;
                //        exist.UPDATEDON = DateTime.Now;
                //        dbi.Entry(exist).State = System.Data.Entity.EntityState.Modified;
                //    }
                //    dbi.SaveChanges();

                //};



                _DB.Entry(hd).State = EntityState.Modified;
                _DB.SaveChanges();
            var flag = InsertLC_HEAD_LOG(hd, "Update");
            Res.hasError = false;
            Res.errorMessage = "Response submitted successfully";

            if (statusflag == 2 && hd.ADMIN_APPROVAL_TYPE == "3")
            {
                ADEMPLOYEE_LC _Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                ADEMPLOYEE_LC _NextApproval_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.ADMINFUEL_OH_USER && x.ACTIVE == 1).FirstOrDefault();
                SendMailToNextAuthority(_Requester_, _NextApproval_, 6); //  // OH = 6
            }
            else if (statusflag == 3)
            {
                ADEMPLOYEE_LC admin = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.ADMIN_USER && x.ACTIVE == 1).FirstOrDefault();
                ADEMPLOYEE_LC CurrApproval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();
                ADEMPLOYEE_LC _Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                SendMailToRequester(admin, CurrApproval, statusflag, 5, Remark, null, _Requester_); // DH = 5
            }
            else
            {
                ADEMPLOYEE_LC Requester = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                ADEMPLOYEE_LC Curr_Approval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();
                SendMailToRequester(Requester, Curr_Approval, statusflag, 5, Remark); // DH = 5
            }
            //ADEMPLOYEE_LC Requester = _db.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
            //ADEMPLOYEE_LC Curr_Approval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();
            //SendMailToRequester(Requester, Curr_Approval, statusflag, 5, Remark); // DH = 5

        
    }
    catch (Exception ex)
    {
        Res.hasError = true;
        Res.errorMessage = ex.Message.ToString();
        throw ex;
    }


    return Res;

}


#endregion

#region Local Conveyance Admin OH Functions
public List<LC_HEAD> GetListDataForAdminOH(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
{

    List<LC_HEAD> ilist = new List<LC_HEAD>();
    try
    {


        ilist = _DB.LC_HEAD.Where(x => x.ADMINFUEL_OH_USER == LoginCode
                                              && x.ADMINFUEL_OH_STATUS == Status
                                                 && x.LCTRANDATE >= FromDate && x.LCTRANDATE <= ToDate
                                                && (Ecode == 0 || x.EMPCODE == Ecode)).ToList();



    }
    catch (Exception ex)
    {
        throw ex;
    }
    return ilist;
}

public LibResult GetRequestDataForAdminOH(int LCTRANNO, int EMPCODE)
{
    LibResult Res = new LibResult();
    try
    {
        var HeaderData = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO).FirstOrDefault();
        var Empdata = GetEmployeeDATA(EMPCODE);
        var DtData = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).OrderBy(x => x.DATEOFEXPENDITURE).ToList();

        var DATA = new
        {
            HeaderData = HeaderData,
            DtData = DtData,
            Empdata = Empdata
        };

        Res.resultObject = DATA;
        Res.hasError = false;

    }
    catch (Exception ex)
    {
        Res.hasError = true;
        throw ex;
    }
    return Res;
}

public LibResult AdminOH_Submit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode)
{
    LibResult Res = new LibResult();


    try
    {
        //CommonRepository comprepo = new CommonRepository();

        UserApprovalAuthority UserAppAuth = _cmr.CheckApprovalAuthority(loginCode);



        //using (var _db = new LCEntities())
        //{
            LC_HEAD hd = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE).FirstOrDefault();
                _DB.Entry(hd).Reload();
            hd.ADMINFUEL_OH_REMARKS = Remark;
            hd.ADMINFUEL_OH_STATUS = Response;
            hd.ADMINFUEL_OH_DATETIME = DateTime.Now;


            hd.UPDATEDBY = loginCode;
            hd.UPDATEDON = DateTime.Now;

            int statusflag = 2;

            if (Response == "Approved")
            {

                statusflag = 2;

            }
            else if (Response == "SendBack")
            {
                hd.ADMIN_STATUS = "Pending";
                hd.ADMIN_REMARKS = "";
                hd.ADMIN_DATETIME = null;
                hd.ADMINFUEL_DH_REMARKS = "";
                hd.ADMINFUEL_DH_STATUS = "";
                hd.ADMINFUEL_DH_DATETIME = null;
                statusflag = 3;
            }
            else
            {
                statusflag = 4;
            }




                _DB.Entry(hd).State = EntityState.Modified;
                _DB.SaveChanges();
            var flag = InsertLC_HEAD_LOG(hd, "Update");
            Res.hasError = false;
            Res.errorMessage = "Response submitted successfully";

            if (statusflag == 2)
            {

                string Mailcc = "";
                ADEMPLOYEE_LC Requester = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                ADEMPLOYEE_LC FuelAdmin = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.ADMIN_USER && x.ACTIVE == 1).FirstOrDefault();
                ADEMPLOYEE_LC Curr_Approval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();

                if (FuelAdmin != null)
                {
                    Mailcc = FuelAdmin.EMAILID.ToString().Trim();
                }
                SendMailToRequester(Requester, Curr_Approval, statusflag, 6, Remark, Mailcc); // OH = 6
            }
            else if (statusflag == 3)
            {
                ADEMPLOYEE_LC admin = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.ADMIN_USER && x.ACTIVE == 1).FirstOrDefault();
                ADEMPLOYEE_LC Curr_Approval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();
                ADEMPLOYEE_LC _Requester_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
                SendMailToRequester(admin, Curr_Approval, statusflag, 6, Remark, null, _Requester_); // OH = 6
            }
       // }
    }
    catch (Exception ex)
    {
        Res.hasError = true;
        Res.errorMessage = ex.Message.ToString();
        throw ex;
    }

    return Res;

}

#endregion
// END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

#region  Local Conveyance Finance Functions



public List<LC_HEAD> GetListDataForFinance(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
{

    List<LC_HEAD> ilist = new List<LC_HEAD>();
    try
    {

        // Commented by Aumento as on 05-07-2024
        //ilist = _DB.LC_HEAD.Where(x => x.FINANCE_USER == LoginCode
        //                                            && x.FINANCE_STATUS == Status
        //                                            && x.LCTRANDATE >= FromDate && x.LCTRANDATE <= ToDate
        //                                            && (Ecode == 0 || x.EMPCODE == Ecode)).ToList();
        // Updated by Aumento as on 05-07-2024

        bool IsExist = true;

        var PlantID = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == LoginCode && x.SYKI == _Syki && x.ACTIVE == 1).Select(x => x.SYPLANTID).FirstOrDefault();

        //IsExist = _DB.LC_ROLEMASTER.Any(x => x.PLANTID == PlantID && x.EMPCODE == LoginCode && x.ROLE == "FINANCE");

        IsExist = _DB.LC_ROLEMASTER.Count(x => x.PLANTID == PlantID && x.EMPCODE == LoginCode && x.ROLE == "FINANCE") > 0;

        ilist = _DB.LC_HEAD.Where(x => x.FINANCE_STATUS == Status && x.PPPLANT == PlantID && IsExist == true
                                                   && x.LCTRANDATE >= FromDate && x.LCTRANDATE <= ToDate
                                                   && (Ecode == 0 || x.EMPCODE == Ecode)).ToList();

    }
    catch (Exception ex)
    {
        throw ex;
    }
    return ilist;
}

public LibResult GetRequestDataForFinance(int LCTRANNO, int EMPCODE)
{
    LibResult Res = new LibResult();


    try
    {
        var HeaderData = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO).FirstOrDefault();
        var Empdata = GetEmployeeDATA(EMPCODE);
        var DtData = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).OrderBy(x => x.DATEOFEXPENDITURE).ToList(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO

        var DATA = new
        {
            HeaderData = HeaderData,
            DtData = DtData,
            Empdata = Empdata
        };

        Res.resultObject = DATA;
        Res.hasError = false;

    }
    catch (Exception ex)
    {
        Res.hasError = true;
        throw ex;
    }


    return Res;

}


//public async Task<LibResult> FinanceSubmit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode, string RequiredAmount, DateTime PostingDate, string ConvAmount, string MealAmout, List<LC_DETAIL> Fin_DT_Data)
//{
//    LibResult Res = new LibResult();
//    try
//    {
//        //using (var dbi = new LCEntities())
//        //using (var dbi = _DB)
//        //{
//        foreach (var item in Fin_DT_Data)
//        {
//            LC_DETAIL obj = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO && x.DATEOFEXPENDITURE == item.DATEOFEXPENDITURE).FirstOrDefault();

//            if (obj != null)  // // chnages on 03-05-2024
//            {
//                obj.FIN_CONV_AMOUNT = item.FIN_CONV_AMOUNT;
//                obj.FIN_MEAL_REFRE_AMOUNT = item.FIN_MEAL_REFRE_AMOUNT;
//                _DB.Entry(obj).State = EntityState.Modified;
//            }

//        }
//        _DB.SaveChanges();
//        //}
//        //;



//        //using (var _db = new LCEntities())
//        //using (var _db = _DB)
//        //{
//        LibResult resRfc = new LibResult();
//        LibResult GenPdf = new LibResult();

//        LC_HEAD hd = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE).FirstOrDefault();

//        decimal _ConvAmount = decimal.Parse(ConvAmount);
//        decimal _MealAmout = decimal.Parse(MealAmout);
//        hd.TOTAL_CONV_AMOUNT = _ConvAmount;
//        hd.TOTAL_MEAL_AMOUNT = _MealAmout;


//        //VW_ASSOCIATELVLDETAILS_LC vw = _db.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.SYKI == _Syki && x.ACTIVE == 1).FirstOrDefault(); // Commented by Aumento :: SR81736 – CR-5140
//        VW_ASSOCIATELVLDETAILS_LC vw = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.SYKI == _Syki).FirstOrDefault(); // Added by Aumento :: SR81736 – CR-5140

//        var hd1 = (from data in _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE)
//                   join addes in _DB.ADDESIGNATION_LC on vw.ADDESIGNATIONID equals addes.ADDESIGNATIONID
//                   join vw_se in _DB.VW_ASSOCIATELVLDETAILS_LC on vw.SECTIONID equals vw_se.SECTIONID
//                   join vw_di in _DB.VW_ASSOCIATELVLDETAILS_LC on vw.DIVISIONID equals vw_di.DIVISIONID
//                   join vw_dep in _DB.VW_ASSOCIATELVLDETAILS_LC on vw.DEPARTMENTID equals vw_dep.DEPARTMENTID
//                   join vw_sysite in _DB.SYSITE_LC on vw.SYSITEID equals vw_sysite.SYSITEID
//                   join ade in _DB.ADEMPLOYEE_LC on vw.ADEMPCODE equals ade.ADEMPCODE
//                   //  where (ade.ACTIVE == 1) // Commented by Aumento :: SR81736 – CR-5140
//                   select new
//                   {
//                       addes.DESCRIP,
//                       vw.SECTION,
//                       vw_di.DIVISION,
//                       vw_dep.DEPARTMENT,
//                       ade.TPHONE,
//                       vw_se.SYPLANTID,
//                       vw_se.OPERATIONID,
//                       vw_sysite.SITEADDRESS
//                   }).FirstOrDefault();

//        _DB.Entry(hd).Reload();

//        var RequestorData = (from t in _DB.ADEMPLOYEE_LC
//                             join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
//                             where t.ADEMPCODE == hd.EMPCODE
//                             // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
//                             && v.SYKI == _Syki
//                             select new
//                             {
//                                 Name = t.FIRSTNAME + " " + t.LASTNAME + "",
//                                 ADEMPCODE = t.ADEMPCODE
//                             }).FirstOrDefault();


//        var RecoHead = (from t in _DB.ADEMPLOYEE_LC
//                        join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
//                        where t.ADEMPCODE == hd.RECOMM_USER
//                        // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
//                        && v.SYKI == _Syki
//                        select new
//                        {
//                            Name = t.FIRSTNAME + " " + t.LASTNAME + "",
//                            ADEMPCODE = t.ADEMPCODE
//                        }).FirstOrDefault();


//        var ApprovalHead = (from t in _DB.ADEMPLOYEE_LC
//                            join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
//                            where t.ADEMPCODE == hd.APPROVER_USER
//                            // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
//                            && v.SYKI == _Syki
//                            select new
//                            {
//                                Name = t.FIRSTNAME + " " + t.LASTNAME + "",
//                                ADEMPCODE = t.ADEMPCODE
//                            }).FirstOrDefault();


//        var Finance = (from t in _DB.ADEMPLOYEE_LC
//                       join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
//                       where t.ADEMPCODE == loginCode
//                       // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
//                       && v.SYKI == _Syki
//                       select new
//                       {
//                           Name = t.FIRSTNAME + " " + t.LASTNAME + "",
//                           ADEMPCODE = t.ADEMPCODE
//                       }).FirstOrDefault();

//        List<LC_DETAIL> DataList = new List<LC_DETAIL>();
//        DataList = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).OrderBy(l => l.DATEOFEXPENDITURE).ToList();


//        if (Response == "Approved" && (_ConvAmount + _MealAmout) != 0)
//        {
//            resRfc = await Local_Conveyance_PostDataToSAP(hd, DataList, loginCode, PostingDate, ConvAmount, MealAmout);
//        }

//        if (resRfc.hasError == false && Response == "Approved")
//        {

//            hd.FINANCE_REMARKS = Remark;
//            hd.FINANCE_STATUS = Response;
//            hd.FINANCE_USER = loginCode;
//            hd.FINANCE_DATETIME = DateTime.Now;
//            hd.REQUIRED_AMOUNT = decimal.Parse(RequiredAmount);
//            hd.UPDATEDBY = loginCode;
//            hd.UPDATEDON = DateTime.Now;
//            hd.POSTINGDATE = PostingDate;
//            hd.SAP_DOC_NO = resRfc.errorMessage.ToString();
//            hd.TOTAL_CONV_AMOUNT = _ConvAmount;
//            hd.TOTAL_MEAL_AMOUNT = _MealAmout;
//            GenPdf = GeneratePdf(hd, DataList, loginCode, RequiredAmount, PostingDate, hd1, RecoHead, ApprovalHead, Finance, RequestorData);
//            if (GenPdf.hasError == false)
//            {
//                hd.PDF_NAME = GenPdf.errorMessage.ToString();
//            }

//        }

//        else if ((resRfc.hasError == true) && Response == "Approved")
//        {
//            Res.errorMessage = "SAP Error : " + resRfc.errorMessage;
//            return Res;
//        }

//        else
//        {
//            hd.FINANCE_REMARKS = Remark;
//            hd.FINANCE_STATUS = Response;
//            hd.FINANCE_DATETIME = DateTime.Now;
//            hd.FINANCE_USER = loginCode;
//            hd.REQUIRED_AMOUNT = decimal.Parse(RequiredAmount);
//            hd.UPDATEDBY = loginCode;
//            hd.UPDATEDON = DateTime.Now;
//            //hd.POSTINGDATE = PostingDate;
//            hd.TOTAL_CONV_AMOUNT = _ConvAmount;
//            hd.TOTAL_MEAL_AMOUNT = _MealAmout;
//        }



//        int statusflag = 2;

//        if (Response == "Approved")
//        {
//            statusflag = 2;

//        }
//        else if (Response == "SendBack")
//        {

//            statusflag = 3;

//        }
//        else
//        {
//            statusflag = 4;
//        }

//        //using (var dbi = new LCEntities())
//        //using (var dbi = _DB)
//        //{

//        List<LC_DETAIL> dtlist = _DB.LC_DETAIL.Where(x => x.LCTRANNO == hd.LCTRANNO).ToList();

//        foreach (var item in dtlist)
//        {
//            LC_DETAIL_TEMP exist = _DB.LC_DETAIL_TEMP.Where(x => x.ASOFFODID == item.ASOFFODID && x.DATEOFEXPENDITURE == item.DATEOFEXPENDITURE).FirstOrDefault();

//            _DB.Entry(exist).Reload();
//            exist.ISSUBMITTED = statusflag;
//            exist.UPDATEDON = DateTime.Now;
//            _DB.Entry(exist).State = EntityState.Modified;
//        }
//        _DB.SaveChanges();

//        //}
//        //;



//        _DB.Entry(hd).State = EntityState.Modified;
//        _DB.SaveChanges();
//        Res.hasError = false;

//        if (Response == "Approved" && resRfc.hasError == false && (_ConvAmount + _MealAmout) != 0)
//        {
//            Res.errorMessage = "Response submitted successfully with " + resRfc.errorMessage.ToString();
//        }
//        else
//        {
//            Res.errorMessage = "Response submitted successfully";
//        }

//        ADEMPLOYEE_LC Requester = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
//        ADEMPLOYEE_LC CurrApproval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();
//        SendMailToRequester(Requester, CurrApproval, statusflag, 4, Remark); // Finance = 4

//        //}



//    }
//    catch (Exception ex)
//    {
//        Res.hasError = true;
//        Res.errorMessage = ex.Message.ToString();
//        LogException(ex);
//        throw ex;
//    }


//    return Res;

//}

public async Task<LibResult> FinanceSubmit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode, string RequiredAmount, DateTime PostingDate, string ConvAmount, string MealAmout, List<LC_DETAIL> Fin_DT_Data)
{
    LibResult Res = new LibResult();
    // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
    using (var transaction = _DB.Database.BeginTransaction())
    {
        try
        {
            foreach (var item in Fin_DT_Data)
            {
                LC_DETAIL obj = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO && x.DATEOFEXPENDITURE == item.DATEOFEXPENDITURE).FirstOrDefault();

                if (obj != null)
                {
                    obj.FIN_CONV_AMOUNT = item.FIN_CONV_AMOUNT;
                    obj.FIN_MEAL_REFRE_AMOUNT = item.FIN_MEAL_REFRE_AMOUNT;
                    _DB.Entry(obj).State = EntityState.Modified;
                }
            }

            _DB.SaveChanges();




            LibResult resRfc = new LibResult();
            LibResult GenPdf = new LibResult();

            LC_HEAD hd = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE).FirstOrDefault();

            decimal _ConvAmount = decimal.Parse(ConvAmount);
            decimal _MealAmout = decimal.Parse(MealAmout);
            hd.TOTAL_CONV_AMOUNT = _ConvAmount;
            hd.TOTAL_MEAL_AMOUNT = _MealAmout;
            _DB.SaveChanges();


            List<LC_DETAIL> DataList = new List<LC_DETAIL>();
            DataList = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).OrderBy(l => l.DATEOFEXPENDITURE).ToList();


            if (Response == "Approved" && (_ConvAmount + _MealAmout) != 0)
            {
                resRfc = await Local_Conveyance_PostDataToSAP(hd, DataList, loginCode, PostingDate, ConvAmount, MealAmout);
            }

            if (resRfc.hasError == false && Response == "Approved")
            {

                hd.FINANCE_REMARKS = Remark;
                hd.FINANCE_STATUS = Response;
                hd.FINANCE_USER = loginCode;
                hd.FINANCE_DATETIME = DateTime.Now;
                hd.REQUIRED_AMOUNT = decimal.Parse(RequiredAmount);
                hd.UPDATEDBY = loginCode;
                hd.UPDATEDON = DateTime.Now;
                hd.POSTINGDATE = PostingDate;
                hd.SAP_DOC_NO = resRfc.errorMessage.ToString();
                hd.TOTAL_CONV_AMOUNT = _ConvAmount;
                hd.TOTAL_MEAL_AMOUNT = _MealAmout;
                _DB.SaveChanges();
                //GenPdf = GeneratePdf(hd, DataList, loginCode, RequiredAmount, PostingDate, hd1, RecoHead, ApprovalHead, Finance, RequestorData);
                //GenPdf = ViewPDF((int)hd.LCTRANNO, (int)hd.EMPCODE, loginCode);
                //if (GenPdf.hasError == false)
                //{
                //    hd.PDF_NAME = GenPdf.errorMessage.ToString();
                //}

            }

            else if ((resRfc.hasError == true) && Response == "Approved")
            {
                Res.errorMessage = "SAP Error : " + resRfc.errorMessage;
                return Res;
            }

            else
            {
                hd.FINANCE_REMARKS = Remark;
                hd.FINANCE_STATUS = Response;
                hd.FINANCE_DATETIME = DateTime.Now;
                hd.FINANCE_USER = loginCode;
                hd.REQUIRED_AMOUNT = decimal.Parse(RequiredAmount);
                hd.UPDATEDBY = loginCode;
                hd.UPDATEDON = DateTime.Now;
                //hd.POSTINGDATE = PostingDate;
                hd.TOTAL_CONV_AMOUNT = _ConvAmount;
                hd.TOTAL_MEAL_AMOUNT = _MealAmout;
            }



            int statusflag = 2;

            if (Response == "Approved")
            {
                statusflag = 2;

            }
            else if (Response == "SendBack")
            {

                statusflag = 3;

            }
            else
            {
                statusflag = 4;
            }


            List<LC_DETAIL> dtlist = _DB.LC_DETAIL.Where(x => x.LCTRANNO == hd.LCTRANNO).ToList();

            foreach (var item in dtlist)
            {
                LC_DETAIL_TEMP exist = _DB.LC_DETAIL_TEMP.Where(x => x.ASOFFODID == item.ASOFFODID && x.DATEOFEXPENDITURE == item.DATEOFEXPENDITURE).FirstOrDefault();

                _DB.Entry(exist).Reload();
                exist.ISSUBMITTED = statusflag;
                exist.UPDATEDON = DateTime.Now;
                _DB.Entry(exist).State = EntityState.Modified;
            }
            _DB.SaveChanges();
            var flagDT = InsertLC_Detail_LOG(dtlist, "Update");




            _DB.Entry(hd).State = EntityState.Modified;
            _DB.SaveChanges();
            var flag = InsertLC_HEAD_LOG(hd, "Update");
            Res.hasError = false;

            if (Response == "Approved" && resRfc.hasError == false && (_ConvAmount + _MealAmout) != 0)
            {
                Res.errorMessage = "Response submitted successfully with " + resRfc.errorMessage.ToString();
            }
            else
            {
                Res.errorMessage = "Response submitted successfully";
            }

            ADEMPLOYEE_LC Requester = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.ACTIVE == 1).FirstOrDefault();
            ADEMPLOYEE_LC CurrApproval = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginCode && x.ACTIVE == 1).FirstOrDefault();
            SendMailToRequester(Requester, CurrApproval, statusflag, 4, Remark); // Finance = 4

            _DB.SaveChanges();
            transaction.Commit();
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
            transaction.Rollback();
            Res.hasError = true;
            Res.errorMessage = ex.Message.ToString();
        }

        return Res;
    }
    // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
}



//public LibResult ViewPDF(int LCTRANNO, int EMPCODE, int loginCode)
//{
//    LibResult Res = new LibResult();
//    try
//    {

//        //using (var _db = new LCEntities())
//        //using (var _db = _DB)
//        //{
//        LibResult GenPdf = new LibResult();



//        LC_HEAD hd = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE).FirstOrDefault();

//        DateTime PostingDate = DateTime.Now;

//        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

//        //VW_ASSOCIATELVLDETAILS_LC vw = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.SYKI == _Syki && x.ACTIVE == 1).FirstOrDefault(); // Commented by Aumento :: SR81736 – CR-5140
//        VW_ASSOCIATELVLDETAILS_LC vw = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.SYKI == _Syki).FirstOrDefault(); // Added by Aumento :: SR81736 – CR-5140

//        var hd1 = (from data in _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE)
//                       //join vw in _db.VW_ASSOCIATELVLDETAILS on data.EMPCODE equals Int32.Parse(vw.ADEMPCODE.ToString())                          
//                   join addes in _DB.ADDESIGNATION_LC on vw.ADDESIGNATIONID equals addes.ADDESIGNATIONID
//                   join vw_se in _DB.VW_ASSOCIATELVLDETAILS_LC on vw.SECTIONID equals vw_se.SECTIONID
//                   join vw_di in _DB.VW_ASSOCIATELVLDETAILS_LC on vw.DIVISIONID equals vw_di.DIVISIONID
//                   join vw_dep in _DB.VW_ASSOCIATELVLDETAILS_LC on vw.DEPARTMENTID equals vw_dep.DEPARTMENTID
//                   join vw_sysite in _DB.SYSITE_LC on vw.SYSITEID equals vw_sysite.SYSITEID
//                   join ade in _DB.ADEMPLOYEE_LC on vw.ADEMPCODE equals ade.ADEMPCODE
//                   // where (ade.ACTIVE == 1) // Added by Aumento :: SR81736 – CR-5140
//                   select new
//                   {
//                       addes.DESCRIP,
//                       vw.SECTION,
//                       vw_di.DIVISION,
//                       vw_dep.DEPARTMENT,
//                       ade.TPHONE,
//                       vw_se.SYPLANTID,
//                       vw_se.OPERATIONID,
//                       vw_sysite.SITEADDRESS
//                   }).FirstOrDefault();

//        _DB.Entry(hd).Reload();
//        //_DB.Entry(hd).Reload();


//        var RequestorData = (from t in _DB.ADEMPLOYEE_LC
//                             join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
//                             where t.ADEMPCODE == hd.EMPCODE && v.SYKI == _Syki   // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
//                             select new
//                             {
//                                 Name = t.FIRSTNAME + " " + t.LASTNAME + "",
//                                 ADEMPCODE = t.ADEMPCODE
//                             }).FirstOrDefault();





//        var RecoHead = (from t in _DB.ADEMPLOYEE_LC
//                        join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
//                        where t.ADEMPCODE == hd.RECOMM_USER && v.SYKI == _Syki   // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
//                        select new
//                        {
//                            Name = t.FIRSTNAME + " " + t.LASTNAME + "",
//                            ADEMPCODE = t.ADEMPCODE
//                        }).FirstOrDefault();


//        var ApprovalHead = (from t in _DB.ADEMPLOYEE_LC
//                            join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
//                            where t.ADEMPCODE == hd.APPROVER_USER && v.SYKI == _Syki   // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
//                            select new
//                            {
//                                Name = t.FIRSTNAME + " " + t.LASTNAME + "",
//                                ADEMPCODE = t.ADEMPCODE
//                            }).FirstOrDefault();




//        var Finance = (from t in _DB.ADEMPLOYEE_LC
//                       join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
//                       where t.ADEMPCODE == hd.FINANCE_USER && v.SYKI == _Syki   // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140

//                       select new
//                       {
//                           Name = t.FIRSTNAME + " " + t.LASTNAME + "",
//                           ADEMPCODE = t.ADEMPCODE
//                       }).FirstOrDefault();


//        List<LC_DETAIL> dtlist = new List<LC_DETAIL>();
//        dtlist = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).OrderBy(l => l.DATEOFEXPENDITURE).ToList();
//        GenPdf = GeneratePdf(hd, dtlist, loginCode, hd.REQUIRED_AMOUNT.ToString(), PostingDate, hd1, RecoHead, ApprovalHead, Finance, RequestorData);



//        Res = GenPdf;


//        //}



//    }
//    catch (Exception ex)
//    {
//        Res.hasError = true;
//        Res.errorMessage = ex.Message.ToString();

//        throw ex;
//    }


//    return Res;

//}


public LibResult ViewPDF(int LCTRANNO, int EMPCODE, int loginCode)
{
    LibResult Res = new LibResult();
    try
    {

        //using (var _db = new LCEntities())
        //{
            LibResult GenPdf = new LibResult();



            LC_HEAD hd = _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE).FirstOrDefault();

            DateTime PostingDate = DateTime.Now;
            // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

            //VW_ASSOCIATELVLDETAILS_LC vw = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.SYKI == _Syki && x.ACTIVE == 1).FirstOrDefault(); // Commented by Aumento :: SR81736 – CR-5140
            VW_ASSOCIATELVLDETAILS_LC vw = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == hd.EMPCODE && x.SYKI == _Syki).FirstOrDefault(); // Added by Aumento :: SR81736 – CR-5140

            var hd1 = (from data in _DB.LC_HEAD.Where(x => x.LCTRANNO == LCTRANNO && x.EMPCODE == EMPCODE)
                       join addes in _DB.ADDESIGNATION_LC on vw.ADDESIGNATIONID equals addes.ADDESIGNATIONID
                       join vw_se in _DB.VW_ASSOCIATELVLDETAILS_LC on vw.SECTIONID equals vw_se.SECTIONID
                       join vw_di in _DB.VW_ASSOCIATELVLDETAILS_LC on vw.DIVISIONID equals vw_di.DIVISIONID
                       join vw_dep in _DB.VW_ASSOCIATELVLDETAILS_LC on vw.DEPARTMENTID equals vw_dep.DEPARTMENTID
                       join vw_sysite in _DB.SYSITE_LC on vw.SYSITEID equals vw_sysite.SYSITEID
                       join ade in _DB.ADEMPLOYEE_LC on vw.ADEMPCODE equals ade.ADEMPCODE
                       //  where (ade.ACTIVE == 1) // Commented by Aumento :: SR81736 – CR-5140
                       select new
                       {
                           addes.DESCRIP,
                           vw.SECTION,
                           vw_di.DIVISION,
                           vw_dep.DEPARTMENT,
                           ade.TPHONE,
                           vw_se.SYPLANTID,
                           vw_se.OPERATIONID,
                           vw_sysite.SITEADDRESS
                       }).FirstOrDefault();

            var RequestorData = (from t in _DB.ADEMPLOYEE_LC
                                 join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
                                 where t.ADEMPCODE == hd.EMPCODE
                                 // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
                                 && v.SYKI == _Syki
                                 select new
                                 {
                                     Name = t.FIRSTNAME + " " + t.LASTNAME + "",
                                     ADEMPCODE = t.ADEMPCODE
                                 }).FirstOrDefault();


            var RecoHead = (from t in _DB.ADEMPLOYEE_LC
                            join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
                            where t.ADEMPCODE == hd.RECOMM_USER
                            // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
                            && v.SYKI == _Syki
                            select new
                            {
                                Name = t.FIRSTNAME + " " + t.LASTNAME + "",
                                ADEMPCODE = t.ADEMPCODE
                            }).FirstOrDefault();


            var ApprovalHead = (from t in _DB.ADEMPLOYEE_LC
                                join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
                                where t.ADEMPCODE == hd.APPROVER_USER
                                // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
                                && v.SYKI == _Syki
                                select new
                                {
                                    Name = t.FIRSTNAME + " " + t.LASTNAME + "",
                                    ADEMPCODE = t.ADEMPCODE
                                }).FirstOrDefault();


            var Finance = (from t in _DB.ADEMPLOYEE_LC
                           join v in _DB.VW_ASSOCIATELVLDETAILS_LC on t.ADEMPCODE equals v.ADEMPCODE
                           where t.ADEMPCODE == loginCode
                           // && v.ACTIVE == 1 && t.ACTIVE == 1 // Commented by Aumento :: SR81736 – CR-5140
                           && v.SYKI == _Syki
                           select new
                           {
                               Name = t.FIRSTNAME + " " + t.LASTNAME + "",
                               ADEMPCODE = t.ADEMPCODE
                           }).FirstOrDefault();


            List<LC_DETAIL> dtlist = new List<LC_DETAIL>();
            dtlist = _DB.LC_DETAIL.Where(x => x.LCTRANNO == LCTRANNO).OrderBy(l => l.DATEOFEXPENDITURE).ToList();

            GenPdf = GeneratePdf(hd, dtlist, loginCode, hd.REQUIRED_AMOUNT.ToString(), PostingDate, hd1, RecoHead, ApprovalHead, Finance, RequestorData);
            if (GenPdf.hasError == false)
            {
                hd.PDF_NAME = GenPdf.errorMessage;
                    _DB.Entry(hd).State = EntityState.Modified;
                _DB.SaveChanges();

            }
            // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
            Res = GenPdf;


       // }



    }
    catch (Exception ex)
    {
        Res.hasError = true;
        Res.errorMessage = ex.Message.ToString();

        throw ex;
    }


    return Res;

}



#endregion

#region Sent to SAP
public async Task<LibResult> Local_Conveyance_PostDataToSAP(LC_HEAD hd, List<LC_DETAIL> dtlist, int loginCode, DateTime PostingDate, string ConvAmount, string MealAmout)
{
    LibResult res = new LibResult();

    try
    {
        string SuccessMsg = string.Empty;

        //EportalESS es = new EportalESS();

        string strFromDate = string.Empty;
        string strTodate = string.Empty;

        string formattedDocDate = FormatDate(hd.LCTRANDATE);
        string formattedPostingDate = FormatDate(PostingDate);
        string DocNo = hd.LCTRANNO.ToString();
        string EmpCode = hd.EMPCODE.ToString();
        if (dtlist.Count > 0)
        {

            DateTime FromDate_ = DateTime.Parse(dtlist[0].DATEOFEXPENDITURE.ToString());
            DateTime ToDate_ = DateTime.Parse(dtlist[dtlist.Count - 1].DATEOFEXPENDITURE.ToString());
            strFromDate = FormatDateddmmyy(FromDate_);
            strTodate = FormatDateddmmyy(ToDate_);
        }

        SuccessMsg = await _iEportalESS.Local_Conveyance_PostDataToSAP(EmpCode, formattedDocDate, DocNo, formattedPostingDate, ConvAmount, MealAmout, strFromDate, strTodate);

        string[] Status = SuccessMsg.Split(new Char[] { '#' });
        string errResult = Convert.ToString(Status[0]);
        string DocMsg = Convert.ToString(Status[1]);
        string DocumnetNo = RemoveParts(DocMsg);
        if (errResult == "S" && DocumnetNo != "ERR")
        {

            res.hasError = false;
            res.errorMessage = DocumnetNo;

        }
        else
        {
            res.hasError = true;
            res.errorMessage = DocMsg;

        }
    }
    catch (Exception ex)
    {
        LogException(ex);
        throw;
    }

    return res;
}

public LibResult GeneratePdf(LC_HEAD hd, List<LC_DETAIL> dtlist, int loginCode, string RequiredAmount, DateTime PostingDate, dynamic hd1, dynamic RecoData, dynamic ApproData, dynamic Finance, dynamic ClaimantData)
{
    LibResult res = new LibResult();

    try
    {
        int OPERATIONID_ = hd1.OPERATIONID == null ? 0 : int.Parse(hd1.OPERATIONID.ToString());
        //string COSTCENTER = _DB.FINTBS_COST_CENTER_MST.Where(x => x.OPERATIONID == OPERATIONID_ && x.STATEID == 698).Select(x => x.COSTCENTER).FirstOrDefault();
        string COSTCENTER = null;


        //string path = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/LCODD/TEMPLATE/");

        //local
        //string path = Path.Combine(serverpath.getContentRootPath(),"UPLOADS", "LCODD", "TEMPLATE");

        //server
        string path = Path.Combine(serverpath.getFileUploadPath(), "LCODD", "TEMPLATE");

        Directory.CreateDirectory(path); // Ensure folder exists


        string timestamp = DateTime.Now.ToString("yyMMddHHmmss");
        var excelTemplatePath = Path.Combine(path, "FINANCEPDF.xlsx");
        var tempExcelPath = Path.Combine(path, $"{hd.LCTRANNO}_{hd.EMPCODE}_{timestamp}_file.xlsx");
        var filename = $"{hd.LCTRANNO}_{hd.EMPCODE}_{timestamp}file.pdf";
        //
        var pdfPath = Path.Combine(path, "../PDF/", filename);
        //var pdfPath = Path.Combine(path, "PDF", filename);

        File.Copy(excelTemplatePath, tempExcelPath, true);
        var workbook = new Workbook();

        workbook.LoadFromFile(tempExcelPath);
        var worksheet = workbook.Worksheets[0];


        var Address = worksheet.FindAllString("[Address]", true, true);
        foreach (var range in Address)
        {
            range.Text = hd1.SITEADDRESS == null ? "" : hd1.SITEADDRESS.ToString();
        }


        var CostCentre = worksheet.FindAllString("[Cost Centre]", true, true);
        foreach (var range in CostCentre)
        {
            range.Text = COSTCENTER == null ? "" : COSTCENTER.ToString();
        }

        //// SAP posting no
        var SAPDOCNo = worksheet.FindAllString("[SAPDOCNO]", true, true);
        foreach (var range in SAPDOCNo)
        {
            range.Text = hd.SAP_DOC_NO == null ? "" : hd.SAP_DOC_NO.ToString();
        }
        var POSTINGDATE = worksheet.FindAllString("[POSTINGDATE]", true, true);
        foreach (var range in POSTINGDATE)
        {
            range.NumberFormat = "dd-mmm-yyyy";
            range.Text = hd.POSTINGDATE == null ? "" : ((DateTime)hd.POSTINGDATE).ToString("dd-MMM-yyyy").ToLower();
        }


        var nameranges = worksheet.FindAllString("[NAME]", true, true);
        foreach (var range in nameranges)
        {
            range.Text = hd.EMPNAME;
        }
        var Ecoderanges = worksheet.FindAllString("[ECODE]", true, true);
        foreach (var range in Ecoderanges)
        {
            range.Text = hd.EMPCODE.ToString();
        }
        var Designation = worksheet.FindAllString("[DESIGNATION]", true, true);
        foreach (var range in Designation)
        {
            range.Text = hd1.DESCRIP != null ? hd1.DESCRIP.ToString() : "";
        }
        var Department = worksheet.FindAllString("[DEPT]", true, true);
        foreach (var range in Department)
        {
            range.Text = hd1.DEPARTMENT != null ? hd1.DEPARTMENT.ToString() : "";
        }

        var Section = worksheet.FindAllString("[SECTION]", true, true);
        foreach (var range in Section)
        {
            range.Text = hd1.SECTION != null ? hd1.SECTION.ToString() : "";
        }

        var Division = worksheet.FindAllString("[DIVISION]", true, true);
        foreach (var range in Division)
        {
            range.Text = hd1.DIVISION != null ? hd1.DIVISION.ToString() : "";
        }

        var DATE = worksheet.FindAllString("[DATE]", true, true);
        foreach (var range in DATE)
        {
            range.NumberFormat = "dd-mmm-yyyy";
            range.Text = ((DateTime)hd.LCTRANDATE).ToString("dd-MMM-yyyy").ToLower();
        }
        var MOBILENO = worksheet.FindAllString("[MOBILENO]", true, true);
        foreach (var range in MOBILENO)
        {
            range.Text = hd1.TPHONE != null ? hd1.TPHONE.ToString() : "";
        }
        var TOTAL_IN_WORDS = worksheet.FindAllString("[TOTAL_IN_WORDS]", true, true);
        foreach (var range in TOTAL_IN_WORDS)
        {
            range.Text = hd.TOTAL_AMT_IN_WORD.ToString();
        }

        // Claimant

        var Claimant_HEAD = worksheet.FindAllString("[Claimant_HEAD]", true, true);
        foreach (var range in Claimant_HEAD)
        {

            range.Text = ClaimantData != null ? (ClaimantData.ADEMPCODE.ToString() + " - " + ClaimantData.Name.ToString()) : "";
        }

        var Claimant_RequestDate = worksheet.FindAllString("[Claimant_RequestDate]", true, true);
        foreach (var range in Claimant_RequestDate)
        {
            range.Text = hd.LCTRANDATE != null ? "Request Date : " + ((DateTime)hd.LCTRANDATE).ToString("dd-MMM-yyyy").ToLower().ToString() : "";
        }
        /// Recommandation
        if (RecoData.ADEMPCODE.ToString() != ClaimantData.ADEMPCODE.ToString())
        {
            var Reco_HEAD = worksheet.FindAllString("[Reco_HEAD]", true, true);
            foreach (var range in Reco_HEAD)
            {
                range.Text = RecoData != null ? (RecoData.ADEMPCODE.ToString() + " - " + RecoData.Name.ToString()) : "";
            }
            var Reco_ApproveDate = worksheet.FindAllString("[Reco_ApproveDate]", true, true);
            foreach (var range in Reco_ApproveDate)
            {
                range.Text = hd.RECOMM_DATETIME != null ? "Approve Date : " + ((DateTime)hd.RECOMM_DATETIME).ToString("dd-MMM-yyyy").ToLower().ToString() : "";
            }
        }
        else
        {
            var Reco_HEAD = worksheet.FindAllString("[Reco_HEAD]", true, true);
            foreach (var range in Reco_HEAD)
            {
                range.Text = "";
            }
            var Reco_ApproveDate = worksheet.FindAllString("[Reco_ApproveDate]", true, true);
            foreach (var range in Reco_ApproveDate)
            {
                range.Text = "";
            }
        }



        /// aprroval 

        var Approval_HEAD = worksheet.FindAllString("[Approval_HEAD]", true, true);
        foreach (var range in Approval_HEAD)
        {
            range.Text = ApproData != null ? (ApproData.ADEMPCODE.ToString() + " - " + ApproData.Name.ToString()) : "";
        }
        var Approval_ApproveDate = worksheet.FindAllString("[Approval_ApproveDate]", true, true);
        foreach (var range in Approval_ApproveDate)
        {
            range.Text = hd.APPROVER_DATETIME != null ? "Approve Date : " + ((DateTime)hd.APPROVER_DATETIME).ToString("dd-MMM-yyyy").ToLower().ToString() : "";
        }


        /// Finance 



        var FINANCE_ = worksheet.FindAllString("[FINANCE]", true, true);
        foreach (var range in FINANCE_)
        {
            range.Text = hd.FINANCE_STATUS == "Approved" ? (Finance != null ? (Finance.ADEMPCODE.ToString() + " - " + Finance.Name.ToString()) : "") : "";
        }
        var Finance_ApproveDate = worksheet.FindAllString("[Finance_ApproveDate]", true, true);
        foreach (var range in Finance_ApproveDate)
        {
            range.Text = hd.FINANCE_STATUS == "Approved" ? (hd.FINANCE_DATETIME != null ? "Approve Date : " + ((DateTime)hd.FINANCE_DATETIME).ToString("dd-MMM-yyyy").ToLower().ToString() : "") : "";
        }


        var PLANT = worksheet.FindAllString("[PLANT]", true, true);
        foreach (var range in PLANT)
        {
            // START :: Added by Aumento :: SR107643 :: LC Enhancement :: Additional Change
            //range.Text = hd1.SYPLANTID != null ? hd1.SYPLANTID.ToString() + "F" : "";
            if (hd1.SYPLANTID == null)
            {
                range.Text = "";
            }
            else if (hd1.SYPLANTID.ToString() == "5")
            {
                range.Text = "HO";
            }
            else
            {
                range.Text = hd1.SYPLANTID + "F";
            }
            // END ::  Added by Aumento :: SR107643 :: LC Enhancement :: Additional Change
        }



        int startRow = 11;
        decimal Total_travelReimbursTotal = 0;
        decimal Total_Refresh_total = 0;
        decimal Total_Total = 0;


        foreach (var item in dtlist)
        {

            worksheet.InsertRow(startRow);

            worksheet.Range[$"c{startRow}"].NumberFormat = "dd-mmm-yyyy"; // Set date format
            worksheet.Range[$"c{startRow}"].Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thick;


            worksheet.Range[$"c{startRow}"].Text = item.DATEOFEXPENDITURE == null ? "" : ((DateTime)item.DATEOFEXPENDITURE).ToString("dd-MMM-yyyy").ToLower().ToString(); // Updated by Aumento :: SR81736 – CR-5140
            worksheet.Range[$"d{startRow}"].IsWrapText = true; // Added by Aumento As in 28-06-2024
            worksheet.Range[$"d{startRow}"].Text = item.PURPOSE == null ? "" : item.PURPOSE;   // Updated by Aumento :: SR81736 – CR-5140
            worksheet.Range[$"E{startRow}"].Text = item.FROMCITY == null ? "" : item.FROMCITY == "Other" ? item.FROMCITY_OTHER : item.FROMCITY; // Updated by Aumento :: SR81736 – CR-5140
            worksheet.Range[$"F{startRow}"].Text = item.TOCITY == null ? "" : item.TOCITY == "Other" ? item.TOCITY_OTHER : item.TOCITY; // Updated by Aumento :: SR81736 – CR-5140
            worksheet.Range[$"G{startRow}"].Text = item.TIMEIN == null ? "" : item.TIMEIN.ToString();  // Updated by Aumento :: SR81736 – CR-5140
            worksheet.Range[$"H{startRow}"].Text = item.TIMEOUT == null ? "" : item.TIMEOUT.ToString(); // Updated by Aumento :: SR81736 – CR-5140
            worksheet.Range[$"I{startRow}"].Text = item.NOOFHOURS == null ? "" : item.NOOFHOURS.ToString(); // Updated by Aumento :: SR81736 – CR-5140
            worksheet.Range[$"J{startRow}"].Text = item.MODEOFTRAVEL == null ? "" : item.MODEOFTRAVEL == "Other" ? (item.MODEOFTRAVEL_OTHER != null ? item.MODEOFTRAVEL_OTHER.ToString() : "") : item.MODEOFTRAVEL; // Updated by Aumento :: SR81736 – CR-5140 // SR107643 :: LC ENHANCEMENT ::  AUMENTO
            worksheet.Range[$"K{startRow}"].Text = item.KMCOVERED == null ? "" : item.KMCOVERED.ToString(); // Updated by Aumento :: SR81736 – CR-5140
            worksheet.Range[$"L{startRow}"].Text = item.RATEPERKM == null ? "" : item.RATEPERKM.ToString(); // Updated by Aumento :: SR81736 – CR-5140

            var TRAVELREIMBURSEMENT_ = (item.FIN_CONV_AMOUNT == null ?
                (decimal.Parse(item.TRAVELREIMBURSEMENT.ToString()) +
                decimal.Parse(item.TOLLTAXMISCAMOUNT.ToString())) :
                decimal.Parse(item.FIN_CONV_AMOUNT.ToString()));
            var REFRESHMENTMEALAmount_ = (item.FIN_MEAL_REFRE_AMOUNT == null ?
                (decimal.Parse(item.DINNER_ALLW_AMOUNT.ToString()) +
                decimal.Parse(item.LUNCH_ALLW_AMOUNT.ToString()) +
                decimal.Parse(item.REFRESHMENTMEALALLOWANCE.ToString())) :
                decimal.Parse(item.FIN_MEAL_REFRE_AMOUNT.ToString()));
            var Total_ = TRAVELREIMBURSEMENT_ + REFRESHMENTMEALAmount_;

            worksheet.Range[$"M{startRow}"].Text = TRAVELREIMBURSEMENT_.ToString();
            worksheet.Range[$"N{startRow}"].Text = REFRESHMENTMEALAmount_.ToString();
            worksheet.Range[$"O{startRow}"].Text = Total_.ToString();
            worksheet.Range[$"O{startRow}:P{startRow}"].Merge();

            Total_travelReimbursTotal = Total_travelReimbursTotal + TRAVELREIMBURSEMENT_;
            Total_Refresh_total = Total_Refresh_total + REFRESHMENTMEALAmount_;
            Total_Total = Total_Total + Total_;




            foreach (char col in new char[] { 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P' })
            {
                var cell = worksheet.Range[$"{col}{startRow}"];
                cell.Style.Font.Size = 11; // Set font size to 8
                cell.Style.HorizontalAlignment = HorizontalAlignType.Center;
                cell.Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;
                cell.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                cell.Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
                cell.Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;

            }


            startRow++;

        }






        var travelReimbursTotal = worksheet.FindAllString("[travelReimbursTotal]", true, true);
        foreach (var range in travelReimbursTotal)
        {
            range.Text = Total_travelReimbursTotal.ToString();
        }


        var Refresh_total = worksheet.FindAllString("[Refresh_total]", true, true);
        foreach (var range in Refresh_total)
        {
            range.Text = Total_Refresh_total.ToString();
        }


        var Total = worksheet.FindAllString("[Total]", true, true);
        foreach (var range in Total)
        {
            range.Text = Total_Total.ToString();
        }


        worksheet.PageSetup.FitToPagesWide = 1; // Fit to 1 page wide
        worksheet.PageSetup.FitToPagesTall = 1; // Fit to 1 page tall
        worksheet.PageSetup.Orientation = PageOrientationType.Landscape;

        // workbook.ActiveSheet.PageSetup.
        // workbook.SaveToFile(tempExcelPath, ExcelVersion.Version2016);   // Commented by Aumento :: SR81736 – CR-5140

        workbook.SaveToFile(pdfPath, FileFormat.PDF);

        // HeadData.PDFPATH = filename;
        // File.Delete(tempExcelPath);  // Commented by Aumento :: SR81736 – CR-5140
        workbook.Dispose();

        res.hasError = false;
        res.errorMessage = filename;



    }
    catch (Exception ex)
    {
        res.hasError = true;
        LogException(ex);
        res.errorMessage = ex.Message.ToString();

    }

    return res;
}


#endregion

#region  Comman Function

public object GetSysiteList() // Added by Aumento as on 01082024
{
    object data;

    data = _DB.SYSITE_LC.Where(x => x.ACTIVE == 1).Select(x =>
         new
         {
             text = x.DESCRIP,
             value = x.SYSITEID
         }

        ).ToList();


    return data;

}


public object GetPaymentPlants()
{
    object data;

    data = _DB.SYPLANT_LC.Where(x => x.ACTIVE == 1).Select(x =>
         new
         {
             text = x.PLANTNAME,
             value = x.SYPLANTID
         }

        ).ToList();


    return data;

}

public int GetAdminUserID(int adEmpCode, int PpPlantId)
{

    int ApproverID = adEmpCode;

    try
    {
        //var _SYPLANTID = (from k in _DB.VW_ASSOCIATELVLDETAILS_LC
        //                  where k.SYKI == _Syki
        //                  && k.ADEMPCODE == adEmpCode
        //                  && k.ACTIVE == 1
        //                  select k.SYPLANTID).FirstOrDefault();

        var ID = (from k in _DB.LC_ROLEMASTER
                  where k.PLANTID == PpPlantId
                  && k.ROLE == "ADMIN"
                  select k.EMPCODE).FirstOrDefault();

        ApproverID = int.Parse(ID.ToString());

    }
    catch (Exception ex)
    {

        throw ex;
    }
    return ApproverID;
}


public int GetFinanceUserID(int adEmpCode, int PpPlantId)
{

    int ApproverID = adEmpCode;

    try
    {
        //var _SYPLANTID = (from k in _DB.VW_ASSOCIATELVLDETAILS_LC
        //                  where k.SYKI == _Syki
        //                  && k.ADEMPCODE == adEmpCode
        //                  && k.ACTIVE == 1
        //                  select k.SYPLANTID).FirstOrDefault();

        var ID = (from k in _DB.LC_ROLEMASTER
                  where k.PLANTID == PpPlantId
                  && k.ROLE == "FINANCE"
                  select k.EMPCODE).FirstOrDefault();


        ApproverID = int.Parse(ID.ToString());

    }
    catch (Exception ex)
    {

        throw ex;
    }
    return ApproverID;
}


private void LogException(Exception ex)
{
    ////string logFilePath = "../../Uploads/errorLog.txt";
    //string logFilePath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/LCODD/errorLog.txt");
    //StreamWriter swMailLog = null;
    //try
    //{
    //    if (File.Exists(logFilePath))
    //    {
    //        swMailLog = new StreamWriter(logFilePath, true);
    //    }
    //    swMailLog.WriteLine("--============================================================");
    //    swMailLog.WriteLine($"[DateTime: {DateTime.Now}]");
    //    swMailLog.WriteLine($"[Exception Message]: {ex.Message}");
    //    swMailLog.WriteLine($"[Stack Trace]: {ex.StackTrace}");
    //    swMailLog.WriteLine(new string('-', 50));
    //    swMailLog.WriteLine("--============================================================");
    //    swMailLog.Close();
    //    swMailLog.Dispose();

    //    //using (StreamWriter writer = new StreamWriter(logFilePath, true))
    //    //{
    //    //    writer.WriteLine($"[DateTime: {DateTime.Now}]");
    //    //    writer.WriteLine($"[Exception Message]: {ex.Message}");
    //    //    writer.WriteLine($"[Stack Trace]: {ex.StackTrace}");
    //    //    writer.WriteLine(new string('-', 50));
    //    //    writer.WriteLine();
    //    //}
    //}
    //catch (Exception ioEx)
    //{
    //    // Handle any exceptions that occur during file I/O
    //    Console.WriteLine($"Error writing to log file: {ioEx.Message}");
    //    Console.WriteLine($"Original Exception: {ex.GetType().Name}: {ex.Message}");
    //    Console.WriteLine($"Original Stack Trace: {ex.StackTrace}");
    //}

}


public List<LC_HEAD> GetListDataForRecoApproval(int LoginCode)
{

    List<LC_HEAD> ilist = new List<LC_HEAD>();
    try
    {

        var Approval_ilist = _DB.LC_HEAD.Where(x => x.APPROVER_USER == LoginCode && x.APPROVER_STATUS == "Pending").ToList();
        var Reco_ilist = _DB.LC_HEAD.Where(x => x.RECOMM_USER == LoginCode && x.RECOMM_STATUS == "Pending").ToList();

        var AdminDH_ilist = _DB.LC_HEAD.Where(x => x.ADMINFUEL_DH_USER == LoginCode && x.ADMINFUEL_DH_STATUS == "Pending").ToList(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
        var AdminOH_ilist = _DB.LC_HEAD.Where(x => x.ADMINFUEL_OH_USER == LoginCode && x.ADMINFUEL_OH_STATUS == "Pending").ToList(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO

        ilist = Approval_ilist.Concat(Reco_ilist).Concat(AdminDH_ilist).Concat(AdminOH_ilist).ToList(); // SR107643 :: LC ENHANCEMENT ::  AUMENTO

    }
    catch (Exception ex)
    {
        throw ex;
    }
    return ilist;
}



public List<LC_HEAD_ViewModel> GetDataForExcelFromDB(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status, string Flag) // {Flag added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
{
    List<LC_HEAD_ViewModel> resultList = new List<LC_HEAD_ViewModel>();

    // Get data into DataTable
    DataTable dataTable = new DataTable();

    //ConnectionString objCnStr = new ConnectionString();
    string strCn = serverpath.getConnectingString();

    using (OracleConnection objCn = new OracleConnection(strCn))
    {
        try
        {
            objCn.Open();
            using (OracleCommand objCmd = objCn.CreateCommand())
            {
                objCmd.CommandText = "PKG_OFFOUTDUTYTRANS.SPROC_LC_USER_REQUESTREPORT";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                objCmd.Parameters.Add("CUR_OUTDUTY", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                objCmd.Parameters.Add("p_Ecode", OracleDbType.Int32).Value = Ecode;
                objCmd.Parameters.Add("p_LoginEcode", OracleDbType.Int32).Value = LoginCode;
                objCmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = FromDate;
                objCmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = ToDate;
                objCmd.Parameters.Add("p_Status", OracleDbType.Varchar2).Value = Status;
                objCmd.Parameters.Add("p_Flage", OracleDbType.Varchar2).Value = Flag; // SR107643 :: LC ENHANCEMENT ::  AUMENTO

                using (OracleDataAdapter adapter = new OracleDataAdapter(objCmd))
                {
                    adapter.Fill(dataTable);
                }

                foreach (DataRow row in dataTable.Rows)
                {

                    LC_HEAD_ViewModel model = new LC_HEAD_ViewModel();
                    //{

                    model.LCTRANNO = row["LCTRANNO"].ToString();
                    model.LCTRANDATE = row.IsNull("LCTRANDATE") ? "" : Convert.ToDateTime(row["LCTRANDATE"]).ToString("dd-MMM-yyyy");
                    model.EMPCODE = row.IsNull("EMPCODE") ? "" : row["EMPCODE"].ToString();


                    model.APPROVER_STATUS = row["APPROVER_STATUS"].ToString();
                    model.APPROVER_NAME = row["APPROVER_NAME"].ToString();
                    model.APPROVER_USER = row.IsNull("APPROVER_USER") ? "" : row["APPROVER_USER"].ToString();
                    model.APPROVER_DATETIME = row.IsNull("APPROVER_DATETIME") ? "" : Convert.ToDateTime(row["APPROVER_DATETIME"]).ToString("dd-MMM-yyyy");
                    // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    model.APPROVER_REMARKS = row["APPROVER_REMARKS"].ToString();
                    model.RECOMM_USER = row.IsNull("RECOMM_USER") ? "" : row["RECOMM_USER"].ToString();
                    model.RECOMM_STATUS = row["RECOMM_STATUS"].ToString();
                    model.RECOMM_NAME = row["RECOMM_NAME"].ToString();
                    model.RECOMM_DATETIME = row.IsNull("RECOMM_DATETIME") ? "" : Convert.ToDateTime(row["RECOMM_DATETIME"]).ToString("dd-MMM-yyyy");
                    model.RECOMM_REMARKS = row["RECOMM_REMARKS"].ToString();
                    // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO                      
                    model.FINANCE_STATUS = row["FINANCE_STATUS"].ToString();
                    model.FINANCE_NAME = row["FINANCE_NAME"].ToString();
                    model.FINANCE_USER = row.IsNull("FINANCE_USER") ? "" : row["FINANCE_USER"].ToString();
                    model.FINANCE_DATETIME = row.IsNull("FINANCE_DATETIME") ? "" : Convert.ToDateTime(row["FINANCE_DATETIME"]).ToString("dd-MMM-yyyy");
                    // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    model.FINANCE_REMARKS = row["FINANCE_REMARKS"].ToString();
                    model.ADMIN_STATUS = row["ADMIN_STATUS"].ToString();
                    model.ADMIN_NAME = row["ADMIN_NAME"].ToString();
                    model.ADMIN_USER = row.IsNull("ADMIN_USER") ? "" : row["ADMIN_USER"].ToString();
                    model.ADMIN_DATETIME = row.IsNull("ADMIN_DATETIME") ? "" : Convert.ToDateTime(row["ADMIN_DATETIME"]).ToString("dd-MMM-yyyy");
                    model.ADMIN_REMARKS = row["ADMIN_REMARKS"].ToString();
                    // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    model.REQUIRED_AMOUNT = row.IsNull("REQUIRED_AMOUNT") ? 0 : Convert.ToDecimal(row["REQUIRED_AMOUNT"]);
                    model.TOTAL_AMOUNT = row.IsNull("TOTAL_AMOUNT") ? 0 : Convert.ToDecimal(row["TOTAL_AMOUNT"].ToString()); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    model.TOTAL_NOOFHOURS = row.IsNull("TOTAL_NOOFHOURS") ? 0 : Convert.ToDecimal(row["TOTAL_NOOFHOURS"]);
                    model.TOTAL_KMCOVERED = row.IsNull("TOTAL_KMCOVERED") ? 0 : Convert.ToDecimal(row["TOTAL_KMCOVERED"]);
                    //model.APPROVED_FUEL = row.IsNull("TOTAL_FUEL") ? 0 : Convert.ToDecimal(row["TOTAL_FUEL"]); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
                    model.DESIGNATION = row["DESIGNATION"].ToString();
                    model.EMPNAME = row["EMPNAME"].ToString();

                    model.POSTINGDATE = row.IsNull("POSTINGDATE") ? "" : Convert.ToDateTime(row["POSTINGDATE"]).ToString("dd-MMM-yyyy");
                    model.TOTAL_MEAL_AMOUNT = row.IsNull("TOTAL_MEAL_AMOUNT") ? 0 : Convert.ToDecimal(row["TOTAL_MEAL_AMOUNT"]);
                    model.TOTAL_CONV_AMOUNT = row.IsNull("TOTAL_CONV_AMOUNT") ? 0 : Convert.ToDecimal(row["TOTAL_CONV_AMOUNT"]);
                    model.SAP_DOC_NO = row["SAP_DOC_NO"].ToString();
                    resultList.Add(model);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (objCn.State == System.Data.ConnectionState.Open)
            {
                objCn.Close();
            }
        }
    }
    return resultList;
}

private string FormatDate(DateTime? date)
{
    return date.HasValue ? date.Value.ToString("dd.MM.yyyy") : string.Empty;
}

private string FormatDateddmmyy(DateTime? date)
{
    return date.HasValue ? date.Value.ToString("dd.MM.yy") : string.Empty;
}


private string RemoveParts(string inputString)
{

    string pattern = @"Document no\. (\d+) 50AG";

    Match match = Regex.Match(inputString, pattern);
    if (match.Success)
    {
        string middlePart = match.Groups[1].Value;
        string outputString = inputString.Replace(match.Value, middlePart);
        return outputString;
    }
    else
    {
        return "ERR";
    }
}

// START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
public object GetRequestorDesg()
{
    object data;
    data = _DB.LC_OH_DIRAPPROVALFLOW_MASTER.Select(x =>
       new
       {
           text = x.REQUESTOR_DESG,
           value = x.REQUESTOR_DESG
       }
    ).ToList();
    return data;
}

public object GetApprovalDesg()
{
    object data;
    data = _DB.LC_OH_DIRAPPROVALFLOW_MASTER.Select(x =>
       new
       {
           text = x.APPROVAL_DESG,
           value = x.APPROVAL_DESG
       }
    ).ToList();
    return data;
}
// END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

#endregion

#region  mail

public void SendMailToNextAuthority(ADEMPLOYEE_LC REQUESTER, ADEMPLOYEE_LC NEXTAPPRVAL, int REQTYPE)
{

    try
    {
        string strReqType = REQTYPE == 1 ? "Recommandation" : REQTYPE == 2 ? "Approval" : REQTYPE == 3 ? "Admin" : REQTYPE == 4 ? "Finance" : REQTYPE == 5 ? "DH" : REQTYPE == 6 ? "OH" : "";

        #region Send mail next approval authority

        EmailCore NextsendMail = new EmailCore();
        NextsendMail.MailFrom = "portal.admin@honda.hmsi.in";
        NextsendMail.MailTo = NEXTAPPRVAL.EMAILID.ToString();
        // NextsendMail.MailTo = "deep.s@aumentotec.com";
        // NextsendMail.MailTo = "vishal.b@aumentotec.com";
        string strSubject_ = "Local Canveyance Request from - " + REQUESTER.FIRSTNAME.ToString() + ", Employee Code - " + REQUESTER.ADEMPCODE.ToString();
        string strBody_ = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Local Canveyance Request from " + REQUESTER.FIRSTNAME.ToString() + " - Emp Code (" + REQUESTER.ADEMPCODE.ToString() + ")</font></b></td></tr>" +
                          "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + NEXTAPPRVAL.FIRSTNAME.ToString() + " ,</br></br></td></tr><tr><td colspan=2 width=514 valign=top>" + REQUESTER.FIRSTNAME.ToString() + " San has raised a Local Canveyance Approval Request in Employee Portal. Below are the details :</td></tr>" +
                        "<tr><td width=125 height=22 valign=top>Type :</td><td width=389 valign=top>" + strReqType + "</td></tr>" +
                         "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "> Employee Portal</a> link to approve the request.</td></tr>" +
                         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

        NextsendMail.MailSubject = strSubject_;
        NextsendMail.MailBody = strBody_;
        try
        {
            bool status = NextsendMail.Send();
        }
        catch (Exception ex)
        {
            throw ex;
        }




        #endregion


    }
    catch (Exception ex)
    {
        throw ex;
    }

}
public void SendMailToRequester(ADEMPLOYEE_LC REQUESTER, ADEMPLOYEE_LC Curr_Approval, int approvalStatus, int REQTYPE, string Remarks, string Mailcc = null, ADEMPLOYEE_LC req = null)
{

    try
    {
        string RequestStatus = approvalStatus == 2 ? "Approved" : approvalStatus == 3 ? "Send back" : approvalStatus == 4 ? "Rejected" : "";
        //string strReqType = REQTYPE == 1 ? "Recommandation" : REQTYPE == 2 ? "Approval" : REQTYPE == 3 ? "Admin" : REQTYPE == 4 ? "Finance" : "";
        string strReqType = REQTYPE == 1 ? "Recommandation" : REQTYPE == 2 ? "Approval" : REQTYPE == 3 ? "Admin" : REQTYPE == 4 ? "Finance" : REQTYPE == 5 ? "DH" : REQTYPE == 6 ? "OH" : ""; // SR107643 :: LC ENHANCEMENT ::  AUMENTO

        #region Send mail for requestor

        commanEmail sendMail = new commanEmail();
        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        sendMail.MailTo = REQUESTER.EMAILID.ToString();
        //sendMail.MailTo = "deep.s@aumentotec.com";
        //sendMail.MailTo = "vishal.b@aumentotec.com";

        if (!string.IsNullOrEmpty(Mailcc))
            sendMail.MailCc = Mailcc;

        string strSubject = "Local Conveyance Approval Status - " + RequestStatus;
        string strBody = string.Empty;
        if ((REQTYPE == 5 || REQTYPE == 6) && Mailcc == null)
        {

            strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                       "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Local Conveyance Approval Status " + RequestStatus + " - Emp Code (" + REQUESTER.ADEMPCODE.ToString() + ")</font></b></td></tr>" +
                        "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                        "<tr><td valign=top colspan =2> Dear " + REQUESTER.FIRSTNAME.ToString() + " San ,</td></tr>" +
                       "<tr><td valign=top colspan =2> Local Conveyance request of " + REQUESTER.FIRSTNAME.ToString() + " San has been <b>" + RequestStatus + "</b> by " + Curr_Approval.FIRSTNAME + " San. The request details are as follows:</td></tr>" +
                      "<tr><td width=125 height=22 valign=top>Type :</td><td width=389 valign=top>" + strReqType + "</td></tr>" +
                       "<tr><td width=125 height=22 valign=top>Remarks :</td><td width=389 valign=top>" + Remarks + "</td></tr>" +
                       "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                       "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";


        }
        else
        {
            strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                        "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Local Conveyance Approval Status " + RequestStatus + " - Emp Code (" + REQUESTER.ADEMPCODE.ToString() + ")</font></b></td></tr>" +
                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                        "<tr><td valign=top colspan =2>Your Local Conveyance request has been <b>" + RequestStatus + "</b> by " + Curr_Approval.FIRSTNAME + " San. The request details are as follows:</td></tr>" +
                       "<tr><td width=125 height=22 valign=top>Type :</td><td width=389 valign=top>" + strReqType + "</td></tr>" +
                        "<tr><td width=125 height=22 valign=top>Remarks :</td><td width=389 valign=top>" + Remarks + "</td></tr>" +
                        "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                        "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";


        }
        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO



        sendMail.MailSubject = strSubject;
        sendMail.MailBody = strBody;
        try
        {
            bool status = sendMail.Send();
        }
        catch (Exception ex)
        {
            throw ex;
        }


        #endregion


    }
    catch (Exception ex)
    {
        throw ex;
    }

}


#endregion

// START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
#region Master Log View
public async Task<List<ExpandoObject>> GetLC_Master_Log_Data(int Srno, string TblName)
{

    var dynamicList = new List<ExpandoObject>();

    try
    {
        if (TblName == "LC_ZTABLE")
        {
            dynamicList = await GetLC_ZTABLE_Log(Srno);
        }
        else if (TblName == "LC_REIMBURSEMENT_B_TABLE")
        {
            dynamicList = await GetLC_REIMBURSEMENT_B_TABLE_Log(Srno);
        }
        else if (TblName == "LC_EXCEPTION_C_TABLE")
        {
            dynamicList = await GetLC_EXCEPTION_C_TABLE_Log(Srno);
        }
        else if (TblName == "LC_OH_DIRAPPROVALFLOW_MASTER")
        {
            dynamicList = await GetLC_OH_DIRAPPROVALFLOW_MASTER_LOG(Srno);
        }
        else if (TblName == "LC_HEAD")
        {
            dynamicList = await GetLC_HEAD_LOG(Srno);
        }
        else if (TblName == "LC_DETAIL")
        {
            dynamicList = await GetLC_DETAIL_LOG(Srno);
        }
        else
        {

        }
    }
    catch (Exception ex)
    {
        throw ex;
    }

    return dynamicList;
}

private async Task<List<ExpandoObject>> GetLC_ZTABLE_Log(int Srno)
{

    var dynamicList = new List<ExpandoObject>();

    try
    {
        //using (var db = new LCEntities())
        //{
            var data = await (from ztable in _DB.LC_ZTABLE_LOG
                              where ztable.MAINTBLSRNO == Srno
                              select new
                              {
                                  ztable.SRNO,
                                  ztable.STATE,
                                  ztable.FOURWHEELER_RATE,
                                  ztable.TWOWHEELER_RATE,
                                  ztable.FROMDATE,
                                  ztable.TODATE,
                                  ztable.ADDEDBY,
                                  ztable.ADDEDON,
                                  ztable.UPDATEDBY,
                                  ztable.UPDATEDON,
                                  ztable.STATEID,
                                  ztable.SYSITEID,
                                  ztable.SYSITENAME,
                                  ztable.LOGMODE,
                                  ztable.ACTIVE,
                                  ztable.MAINTBLSRNO
                              }).OrderByDescending(x => x.SRNO).ToListAsync();

            int srNo = 0;
            foreach (var record in data)
            {
                dynamic expando = new ExpandoObject();
                var expandoDict = expando as IDictionary<string, object>;
                srNo++;

                var addedByFullName = record.ADDEDBY != null ? GetEmpFullName(record.ADDEDBY) : "";
                var updatedByFullName = record.UPDATEDBY != null ? GetEmpFullName(record.UPDATEDBY) : "";

                expandoDict["SrNo"] = srNo;
                expandoDict["Site"] = record.SYSITENAME;
                expandoDict["Four Wheeler Rate"] = record.FOURWHEELER_RATE;
                expandoDict["Two Wheeler Rate"] = record.TWOWHEELER_RATE;
                expandoDict["From Date"] = DateTime.Parse(record.FROMDATE.ToString()).ToString("dd MMM yyyy");
                expandoDict["To Date"] = DateTime.Parse(record.TODATE.ToString()).ToString("dd MMM yyyy");
                expandoDict["Mode"] = record.LOGMODE;
                expandoDict["Status"] = record.ACTIVE == 1 ? "Active" : "DeActive";
                expandoDict["Added By"] = record.ADDEDBY != null ? $"{record.ADDEDBY}-{addedByFullName}" : "";
                expandoDict["Added On"] = record.ADDEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                expandoDict["Updated By"] = record.UPDATEDBY != null ? $"{record.UPDATEDBY}-{updatedByFullName}" : "";
                expandoDict["Updated On"] = record.UPDATEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";

                dynamicList.Add(expando);
            }
       // }
    }
    catch (Exception ex)
    {
        throw ex;
    }

    return dynamicList;
}

private async Task<List<ExpandoObject>> GetLC_REIMBURSEMENT_B_TABLE_Log(int Srno)
{

    var dynamicList = new List<ExpandoObject>();

    try
    {
        //using (var db = new LCEntities())
        //{
            var data = await (from lc_reimbursement_b_table in _DB.LC_REIMBURSEMENT_B_TABLE_LOG
                              where lc_reimbursement_b_table.MAINTBLSRNO == Srno
                              select new
                              {
                                  lc_reimbursement_b_table.SRNO,
                                  lc_reimbursement_b_table.DESIGNATION,
                                  lc_reimbursement_b_table.VEHICLE_ENTITLEMENT,
                                  lc_reimbursement_b_table.FUEL_REIMBUR_PER_LTR,
                                  lc_reimbursement_b_table.REFRESH_ALLOW_PER_DAY,
                                  lc_reimbursement_b_table.MEAL_ALLOWANCE,
                                  lc_reimbursement_b_table.FROMDATE,
                                  lc_reimbursement_b_table.TODATE,
                                  lc_reimbursement_b_table.ADDEDBY,
                                  lc_reimbursement_b_table.ADDEDON,
                                  lc_reimbursement_b_table.UPDATEDBY,
                                  lc_reimbursement_b_table.UPDATEDON,
                                  lc_reimbursement_b_table.DESIGNATIONID,
                                  lc_reimbursement_b_table.ACTIVE,
                                  lc_reimbursement_b_table.LOGMODE,
                                  lc_reimbursement_b_table.MAINTBLSRNO
                              }).OrderByDescending(x => x.SRNO).ToListAsync();

            int srNo = 0;
            foreach (var record in data)
            {
                dynamic expando = new ExpandoObject();
                var expandoDict = expando as IDictionary<string, object>;
                srNo++;

                var addedByFullName = record.ADDEDBY != null ? GetEmpFullName(record.ADDEDBY) : "";
                var updatedByFullName = record.UPDATEDBY != null ? GetEmpFullName(record.UPDATEDBY) : "";

                expandoDict["SrNo"] = srNo;
                expandoDict["Designation"] = record.DESIGNATION;
                expandoDict["Vehicle Entitlement"] = record.VEHICLE_ENTITLEMENT;
                expandoDict["Fuel Reimbursement Per Ltr"] = record.FUEL_REIMBUR_PER_LTR;
                expandoDict["Refreshment Allowance Per Day"] = record.REFRESH_ALLOW_PER_DAY;
                expandoDict["Meal Allowance"] = record.MEAL_ALLOWANCE;
                expandoDict["From Date"] = DateTime.Parse(record.FROMDATE.ToString()).ToString("dd MMM yyyy");
                expandoDict["To Date"] = DateTime.Parse(record.TODATE.ToString()).ToString("dd MMM yyyy");
                expandoDict["Active"] = record.ACTIVE == 1 ? "Active" : "DeActive";
                expandoDict["Added By"] = record.ADDEDBY != null ? $"{record.ADDEDBY}-{addedByFullName}" : "";
                expandoDict["Added On"] = record.ADDEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                expandoDict["Updated By"] = record.UPDATEDBY != null ? $"{record.UPDATEDBY}-{updatedByFullName}" : "";
                expandoDict["Updated On"] = record.UPDATEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";

                dynamicList.Add(expando);
            }
       // }
    }
    catch (Exception ex)
    {
        throw ex;
    }

    return dynamicList;
}

private async Task<List<ExpandoObject>> GetLC_EXCEPTION_C_TABLE_Log(int Srno)
{

    var dynamicList = new List<ExpandoObject>();

    try
    {
        //using (var db = new LCEntities())
        //{
            var data = await (from lc_exception_c_table in _DB.LC_EXCEPTION_C_TABLE_LOG
                              where lc_exception_c_table.MAINTBLSRNO == Srno
                              select new
                              {
                                  lc_exception_c_table.SRNO,
                                  lc_exception_c_table.EMPCODE,
                                  lc_exception_c_table.FUEL_REIMBUR_PER_LTR,
                                  lc_exception_c_table.FROMDATE,
                                  lc_exception_c_table.TODATE,
                                  lc_exception_c_table.ADDEDBY,
                                  lc_exception_c_table.ADDEDON,
                                  lc_exception_c_table.UPDATEDBY,
                                  lc_exception_c_table.UPDATEDON,
                                  lc_exception_c_table.EMPNAME,
                                  lc_exception_c_table.REMARKS,
                                  lc_exception_c_table.ACTIVE,
                                  lc_exception_c_table.LOGMODE,
                                  lc_exception_c_table.MAINTBLSRNO
                              }).OrderByDescending(x => x.SRNO).ToListAsync();

            int srNo = 0;
            foreach (var record in data)
            {
                dynamic expando = new ExpandoObject();
                var expandoDict = expando as IDictionary<string, object>;
                srNo++;

                var addedByFullName = record.ADDEDBY != null ? GetEmpFullName(record.ADDEDBY) : "";
                var updatedByFullName = record.UPDATEDBY != null ? GetEmpFullName(record.UPDATEDBY) : "";

                expandoDict["SrNo"] = srNo;
                expandoDict["Emp Code"] = record.EMPCODE;
                expandoDict["Emp Name"] = record.EMPNAME;
                expandoDict["FUEL REIMBUR PER LTR"] = record.FUEL_REIMBUR_PER_LTR;
                expandoDict["REMAKRS"] = record.REMARKS == null ? "" : record.REMARKS;
                expandoDict["From Date"] = DateTime.Parse(record.FROMDATE.ToString()).ToString("dd MMM yyyy");
                expandoDict["To Date"] = DateTime.Parse(record.TODATE.ToString()).ToString("dd MMM yyyy");
                expandoDict["Active"] = record.ACTIVE == 1 ? "Active" : "DeActive";
                expandoDict["Log Mode"] = record.LOGMODE;
                expandoDict["Added By"] = record.ADDEDBY != null ? $"{record.ADDEDBY}-{addedByFullName}" : "";
                expandoDict["Added On"] = record.ADDEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                expandoDict["Updated By"] = record.UPDATEDBY != null ? $"{record.UPDATEDBY}-{updatedByFullName}" : "";
                expandoDict["Updated On"] = record.UPDATEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";

                dynamicList.Add(expando);
            }
        //}
    }
    catch (Exception ex)
    {
        throw ex;
    }

    return dynamicList;
}

private async Task<List<ExpandoObject>> GetLC_OH_DIRAPPROVALFLOW_MASTER_LOG(int Srno)
{

    var dynamicList = new List<ExpandoObject>();

    try
    {
        //using (var db = new LCEntities())
        //{
            var data = await (from Dtable in _DB.LC_OH_DIRAPPROVALFLOW_MASTER_LOG
                              where Dtable.MAINTBLSRNO == Srno
                              select new
                              {
                                  Dtable.SRNO,
                                  Dtable.REQUESTOR_DESG,
                                  Dtable.APPROVAL_DESG,
                                  Dtable.UPDATEDBY,
                                  Dtable.UPDATEDDATE,
                                  Dtable.LOGMODE,
                                  Dtable.MAINTBLSRNO
                              }).OrderByDescending(x => x.SRNO).ToListAsync();

            int srNo = 0;
            foreach (var record in data)
            {
                dynamic expando = new ExpandoObject();
                var expandoDict = expando as IDictionary<string, object>;
                srNo++;

                expandoDict["SrNo"] = srNo;
                expandoDict["Requestor Desg"] = record.REQUESTOR_DESG;
                expandoDict["Approval Desg"] = record.APPROVAL_DESG;
                expandoDict["Mode"] = record.LOGMODE;
                expandoDict["Updated By"] = record.UPDATEDBY;
                expandoDict["Updated Date"] = record.UPDATEDDATE?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";

                dynamicList.Add(expando);
            }
        //}
    }
    catch (Exception ex)
    {
        throw ex;
    }

    return dynamicList;
}
//Added on 31-03-2025
private async Task<List<ExpandoObject>> GetLC_HEAD_LOG(int Srno)
{

    var dynamicList = new List<ExpandoObject>();

    try
    {
        //using (var db = new LCEntities())
        //{
            var data = await (from hd in _DB.LC_HEAD_LOG
                              where hd.LCTRANNO == Srno
                              select new
                              {
                                  hd.SRNO,
                                  hd.EMPCODE,
                                  hd.LCTRANNO,
                                  hd.LCTRANDATE,
                                  hd.PURPOSE,
                                  hd.TOTAL_AMOUNT,
                                  hd.TOTAL_AMT_IN_WORD,
                                  hd.APPROVER_USER,
                                  hd.APPROVER_STATUS,
                                  hd.APPROVER_DATETIME,
                                  hd.ADMIN_USER,
                                  hd.ADMIN_STATUS,
                                  hd.ADMIN_DATETIME,
                                  hd.FINANCE_USER,
                                  hd.FINANCE_STATUS,
                                  hd.FINANCE_DATETIME,
                                  hd.FUAL,
                                  hd.REQUIRED_AMOUNT,
                                  hd.ADDEDBY,
                                  hd.ADDEDON,
                                  hd.UPDATEDBY,
                                  hd.UPDATEDON,
                                  hd.TOTAL_NOOFHOURS,
                                  hd.TOTAL_KMCOVERED,
                                  hd.DESIGNATION,
                                  hd.EMPNAME,
                                  hd.APPROVER_REMARKS,
                                  hd.ADMIN_REMARKS,
                                  hd.FINANCE_REMARKS,
                                  hd.APPROVED_FUEL,
                                  hd.RECOMM_USER,
                                  hd.RECOMM_STATUS,
                                  hd.RECOMM_DATETIME,
                                  hd.POSTINGDATE,
                                  hd.FINAL_STATUS,
                                  hd.RECOMM_REMARKS,
                                  hd.TOTAL_MEAL_AMOUNT,
                                  hd.TOTAL_CONV_AMOUNT,
                                  hd.PDF_NAME,
                                  hd.SAP_DOC_NO,
                                  hd.PPPLANT,
                                  hd.ADMINFUEL_DH_USER,
                                  hd.ADMINFUEL_DH_STATUS,
                                  hd.ADMINFUEL_DH_REMARKS,
                                  hd.ADMINFUEL_DH_DATETIME,
                                  hd.ADMINFUEL_OH_USER,
                                  hd.ADMINFUEL_OH_STATUS,
                                  hd.ADMINFUEL_OH_REMARKS,
                                  hd.ADMINFUEL_OH_DATETIME,
                                  hd.LOGMODE,
                                  hd.ADMINFUEL_DOC
                              }).OrderByDescending(x => x.SRNO).ToListAsync();

            int srNo = 0;
            foreach (var record in data)
            {
                dynamic expando = new ExpandoObject();
                var expandoDict = expando as IDictionary<string, object>;
                srNo++;

                var addedByFullName = record.ADDEDBY != null ? GetEmpFullName(record.ADDEDBY) : "";
                var updatedByFullName = record.UPDATEDBY != null ? GetEmpFullName(record.UPDATEDBY) : "";

                expandoDict["SrNo"] = srNo;
                expandoDict["EMPCODE"] = record.EMPCODE;
                expandoDict["LCTRANNO"] = record.LCTRANNO;
                expandoDict["LCTRANDATE"] = DateTime.Parse(record.LCTRANDATE.ToString()).ToString("dd MMM yyyy");
                expandoDict["Purpose"] = record.PURPOSE;
                expandoDict["Total Amount"] = record.TOTAL_AMOUNT;
                expandoDict["Total Amount In Words"] = record.TOTAL_AMT_IN_WORD;
                expandoDict["Approver User"] = record.APPROVER_USER;
                expandoDict["Approver Status"] = record.APPROVER_STATUS;
                expandoDict["Approver Date"] = DateTime.Parse(record.APPROVER_DATETIME.ToString()).ToString("dd MMM yyyy");
                expandoDict["Admin User"] = record.ADMIN_USER;
                expandoDict["Admin Status"] = record.ADMIN_STATUS;
                expandoDict["Admin Date"] = DateTime.Parse(record.ADMIN_DATETIME.ToString()).ToString("dd MMM yyyy");
                expandoDict["Finance User"] = record.FINANCE_USER;
                expandoDict["Finance Status"] = record.FINANCE_STATUS;
                expandoDict["Finance Date"] = DateTime.Parse(record.FINANCE_DATETIME.ToString()).ToString("dd MMM yyyy");
                expandoDict["Fual"] = record.FUAL;
                expandoDict["Required Amount"] = record.REQUIRED_AMOUNT;
                expandoDict["Added By"] = record.ADDEDBY != null ? $"{record.ADDEDBY}-{addedByFullName}" : "";
                expandoDict["Added On"] = record.ADDEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                expandoDict["Updated By"] = record.UPDATEDBY != null ? $"{record.UPDATEDBY}-{updatedByFullName}" : "";
                expandoDict["Updated On"] = record.UPDATEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                expandoDict["Total No. Of Hours"] = record.TOTAL_NOOFHOURS;
                expandoDict["Total KM Covered"] = record.TOTAL_KMCOVERED;
                expandoDict["Designation"] = record.DESIGNATION;
                expandoDict["Employee Name"] = record.EMPNAME;
                expandoDict["Approver Remarks"] = record.APPROVER_REMARKS;
                expandoDict["Admin Remarks"] = record.ADMIN_REMARKS;
                expandoDict["Finance Remarks"] = record.FINANCE_REMARKS;
                expandoDict["Approved Fuel"] = record.APPROVED_FUEL;
                expandoDict["Recommandation User"] = record.RECOMM_USER;
                expandoDict["Recommandation Status"] = record.RECOMM_STATUS;
                expandoDict["Recommandation Date"] = DateTime.Parse(record.RECOMM_DATETIME.ToString()).ToString("dd MMM yyyy");
                expandoDict["Posting Date"] = DateTime.Parse(record.POSTINGDATE.ToString()).ToString("dd MMM yyyy");
                expandoDict["Final Status"] = record.FINAL_STATUS;
                expandoDict["Recommandation Remarks"] = record.RECOMM_REMARKS;
                expandoDict["Total Meal Amount"] = record.TOTAL_MEAL_AMOUNT;
                expandoDict["Total Conv Amount"] = record.TOTAL_CONV_AMOUNT;
                expandoDict["PDF Name"] = record.PDF_NAME;
                expandoDict["SAP DOC NO"] = record.SAP_DOC_NO;
                expandoDict["PPPLANT"] = record.PPPLANT;
                expandoDict["Admin DH User"] = record.ADMINFUEL_DH_USER;
                expandoDict["Admin DH Status"] = record.ADMINFUEL_DH_STATUS;
                expandoDict["Admin DH Remarks"] = record.ADMINFUEL_DH_REMARKS;
                expandoDict["Admin DH Date"] = DateTime.Parse(record.ADMINFUEL_DH_DATETIME.ToString()).ToString("dd MMM yyyy");
                expandoDict["Admin OH User"] = record.ADMINFUEL_OH_USER;
                expandoDict["Admin OH Status"] = record.ADMINFUEL_OH_STATUS;
                expandoDict["Admin OH Remarks"] = record.ADMINFUEL_OH_REMARKS;
                expandoDict["Admin OH Date"] = DateTime.Parse(record.ADMINFUEL_OH_DATETIME.ToString()).ToString("dd MMM yyyy");
                expandoDict["Log Mode"] = record.LOGMODE;
                expandoDict["Admin Fuel DOC"] = record.ADMINFUEL_DOC;
                dynamicList.Add(expando);
            }
        //}
    }
    catch (Exception ex)
    {
        throw ex;
    }

    return dynamicList;
}

private async Task<List<ExpandoObject>> GetLC_DETAIL_LOG(int Srno)
{

    var dynamicList = new List<ExpandoObject>();

    try
    {
        //using (var db = new LCEntities())
        //{
            var data = await (from dt in _DB.LC_DETAIL_LOG
                              where dt.LCTRANNO == Srno
                              select new
                              {
                                  dt.SRNO,
                                  dt.ASOFFODID,
                                  dt.EMPCODE,
                                  dt.EMPNAME,
                                  dt.DATEOFEXPENDITURE,
                                  dt.PURPOSE,
                                  dt.FROMCITY,
                                  dt.TOCITY,
                                  dt.FROMCITYID,
                                  dt.TOCITYID,
                                  dt.TIMEIN,
                                  dt.TIMEOUT,
                                  dt.NOOFHOURS,
                                  dt.MODEOFTRAVEL,
                                  dt.KMCOVERED,
                                  dt.RATEPERKM,
                                  dt.TRAVELREIMBURSEMENT,
                                  dt.REFRESHMENTMEALALLOWANCE,
                                  dt.TOLLTAXMISCAMOUNT,
                                  dt.TOTAL,
                                  dt.DT,
                                  dt.ADDEDBY,
                                  dt.ADDEDON,
                                  dt.UPDATEDBY,
                                  dt.UPDATEDON,
                                  dt.LCTRANDATE,
                                  dt.LCTRANNO,
                                  dt.FUEL_REIMBURSMENT,
                                  dt.MODEOFTRAVEL_OTHER,
                                  dt.FROMCITY_OTHER,
                                  dt.TOCITY_OTHER,
                                  dt.MEAL_REFRE_ALLW_CLAIM,
                                  dt.FUEL_ALLW_CLAIM,
                                  dt.STATUS,
                                  dt.DINNER_ALLW_CLAIM,
                                  dt.LUNCH_ALLW_CLAIM,
                                  dt.DINNER_ALLW_AMOUNT,
                                  dt.LUNCH_ALLW_AMOUNT,
                                  dt.TOURSTARTFROMHOMEKM,
                                  dt.FIN_CONV_AMOUNT,
                                  dt.FIN_MEAL_REFRE_AMOUNT,
                                  dt.LOGMODE
                              }).OrderByDescending(x => x.SRNO).ToListAsync();

            int srNo = 0;
            foreach (var record in data)
            {
                dynamic expando = new ExpandoObject();
                var expandoDict = expando as IDictionary<string, object>;
                srNo++;

                var addedByFullName = record.ADDEDBY != null ? GetEmpFullName(record.ADDEDBY) : "";
                var updatedByFullName = record.UPDATEDBY != null ? GetEmpFullName(record.UPDATEDBY) : "";

                expandoDict["SrNo"] = srNo;
                expandoDict["ASOFFODID"] = record.ASOFFODID;
                expandoDict["EMPCODE"] = record.EMPCODE;
                expandoDict["Employee Name"] = record.EMPNAME;
                expandoDict["Date Of Expenditure"] = record.DATEOFEXPENDITURE;
                expandoDict["Purpose"] = record.PURPOSE;
                expandoDict["From City"] = record.FROMCITY;
                expandoDict["To City"] = record.TOCITY;
                expandoDict["From City ID"] = record.FROMCITYID;
                expandoDict["To City ID"] = record.TOCITYID;
                expandoDict["Time In"] = record.TIMEIN;
                expandoDict["Time Out"] = record.TIMEOUT;
                expandoDict["No Of Hours"] = record.NOOFHOURS;
                expandoDict["Mode of Travel"] = record.MODEOFTRAVEL;
                expandoDict["KM Covered"] = record.KMCOVERED;
                expandoDict["Rate Per KM"] = record.RATEPERKM;
                expandoDict["Travel Reimbursement"] = record.TRAVELREIMBURSEMENT;
                expandoDict["Refreshment Meal Allowance"] = record.REFRESHMENTMEALALLOWANCE;
                expandoDict["Toll Tax Misc Amount"] = record.TOLLTAXMISCAMOUNT;
                expandoDict["Total"] = record.TOTAL;
                expandoDict["DT"] = record.DT;
                expandoDict["Added By"] = record.ADDEDBY != null ? $"{record.ADDEDBY}-{addedByFullName}" : "";
                expandoDict["Added On"] = record.ADDEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                expandoDict["Updated By"] = record.UPDATEDBY != null ? $"{record.UPDATEDBY}-{updatedByFullName}" : "";
                expandoDict["Updated On"] = record.UPDATEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                expandoDict["LCTRANDATE"] = DateTime.Parse(record.LCTRANDATE.ToString()).ToString("dd MMM yyyy");
                expandoDict["LCTRANNO"] = record.LCTRANNO;
                expandoDict["Fuel Reimbursment"] = record.FUEL_REIMBURSMENT;
                expandoDict["Mode Of travel (Other)"] = record.MODEOFTRAVEL_OTHER;
                expandoDict["From City (Other)"] = record.FROMCITY_OTHER;
                expandoDict["To City (Other)"] = record.TOCITY_OTHER;
                expandoDict["Meal Refre Allw Claim"] = record.MEAL_REFRE_ALLW_CLAIM;
                expandoDict["Fuel Allw Claim"] = record.FUEL_ALLW_CLAIM;
                expandoDict["Status"] = record.STATUS;
                expandoDict["Dinner Allw Claim"] = record.DINNER_ALLW_CLAIM;
                expandoDict["Lunch Allw Claim"] = record.LUNCH_ALLW_CLAIM;
                expandoDict["Dinner Allw Amount"] = record.DINNER_ALLW_AMOUNT;
                expandoDict["Lunch Allw Amount"] = record.LUNCH_ALLW_AMOUNT;
                expandoDict["Tour Start From Home KM"] = record.TOURSTARTFROMHOMEKM;
                expandoDict["FIN CONV Amount"] = record.FIN_CONV_AMOUNT;
                expandoDict["FIN Meal refre Amount"] = record.FIN_MEAL_REFRE_AMOUNT;
                expandoDict["Log Mode"] = record.LOGMODE;
                dynamicList.Add(expando);
            }
       // }
    }
    catch (Exception ex)
    {
        throw ex;
    }

    return dynamicList;
}

private string GetEmpFullName(long? Ecode)
{
    string Fullname = string.Empty;
    try
    {
        //using (var DB = new LCEntities())
        //{
            var Emp = _DB.ADEMPLOYEE_LC.Where(x => x.ACTIVE == 1 && x.ADEMPCODE == Ecode).FirstOrDefault();

            Fullname = Emp != null ? $"{Emp.FIRSTNAME} {Emp.LASTNAME}" : "";
        //}
    }
    catch (Exception ex)
    {
        throw ex;
    }

    return Fullname;
}

private bool InsertLC_ZTABLE_LOG(decimal? FourWheelerRate, decimal? TwoWheelerRate, DateTime? FromDate, DateTime? ToDate, int? AddedBy, DateTime? AddedOn, int? UpdatedBy, DateTime? UpdatedOn, long? SiteID, string SiteDescript, string Mode, int? Active = 1, long? MaintblSrNo = 0)
{
    try
    {
        //using (var db = new LCEntities())
        //{
            LC_ZTABLE_LOG ZtableLog = new LC_ZTABLE_LOG();
            var maxSrno = _DB.LC_ZTABLE_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);

            ZtableLog.SRNO = maxSrno + 1;
            ZtableLog.STATE = "";
            ZtableLog.FOURWHEELER_RATE = FourWheelerRate;
            ZtableLog.TWOWHEELER_RATE = TwoWheelerRate;
            ZtableLog.FROMDATE = FromDate;
            ZtableLog.TODATE = ToDate;
            ZtableLog.ADDEDBY = AddedBy;
            ZtableLog.ADDEDON = AddedOn;
            ZtableLog.UPDATEDBY = UpdatedBy;
            ZtableLog.UPDATEDON = UpdatedOn;
            ZtableLog.STATEID = 0;
            ZtableLog.SYSITEID = SiteID;
            ZtableLog.SYSITENAME = SiteDescript;
            ZtableLog.LOGMODE = Mode;
            ZtableLog.ACTIVE = (short)Active;
            ZtableLog.MAINTBLSRNO = long.Parse(MaintblSrNo.ToString());

            _DB.Entry(ZtableLog).State = EntityState.Added;

                _DB.LC_ZTABLE_LOG.Add(ZtableLog);
                _DB.SaveChanges();
        //}
    }
    catch (Exception ex)
    {
        return false;
    }

    return true;
}

private bool InsertLC_REIMBURSEMENT_B_TABLE_LOG(long? DesignationID, string DesignationDescript, string VehicleEntitlement, decimal? Fule, long? Refresh, long? Meal, DateTime? FromDate, DateTime? ToDate, long? AddedBy, DateTime? AddedOn, long? UpdatedBy, DateTime? UpdatedOn, int? Active = 1, string Mode = "ADD", long? MaintblSrNo = 0)
{
    try
    {
        //using (var db = new LCEntities())
        //{
            LC_REIMBURSEMENT_B_TABLE_LOG crmst = new LC_REIMBURSEMENT_B_TABLE_LOG();
            var maxSrno = _DB.LC_REIMBURSEMENT_B_TABLE_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);

            crmst.SRNO = maxSrno + 1;
            crmst.DESIGNATION = DesignationDescript;
            crmst.VEHICLE_ENTITLEMENT = VehicleEntitlement;
            crmst.FUEL_REIMBUR_PER_LTR = Fule;
            crmst.REFRESH_ALLOW_PER_DAY = Refresh;
            crmst.MEAL_ALLOWANCE = Meal;
            crmst.FROMDATE = FromDate;
            crmst.TODATE = ToDate;
            crmst.ADDEDBY = AddedBy;
            crmst.ADDEDON = AddedOn;
            crmst.UPDATEDBY = UpdatedBy;
            crmst.UPDATEDON = UpdatedOn;
            crmst.DESIGNATIONID = DesignationID;
            crmst.ACTIVE = (short)Active;
            crmst.LOGMODE = Mode;
            crmst.MAINTBLSRNO = MaintblSrNo;

                _DB.Entry(crmst).State = EntityState.Added;

                _DB.LC_REIMBURSEMENT_B_TABLE_LOG.Add(crmst);
                _DB.SaveChanges();
        //}
    }
    catch (Exception ex)
    {
        return false;
    }

    return true;
}

private bool InsertLC_EXCEPTION_C_TABLE_LOG(long? EmpCode, string EmpName, decimal? FuelReimbursementPerLtr, DateTime? FromDate, DateTime? ToDate, string Remarks, long? AddedBy, DateTime? AddedOn, long? UpdatedBy, DateTime? UpdatedOn, int? Active = 1, string Mode = "ADD", long? MaintblSrNo = 0)
{
    try
    {
        //using (var db = new LCEntities())
        //{
            LC_EXCEPTION_C_TABLE_LOG crmst = new LC_EXCEPTION_C_TABLE_LOG();
            var maxSrno = _DB.LC_EXCEPTION_C_TABLE_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);


            crmst.SRNO = maxSrno + 1;
            crmst.EMPCODE = EmpCode;
            crmst.EMPNAME = EmpName;
            crmst.FUEL_REIMBUR_PER_LTR = FuelReimbursementPerLtr;
            crmst.FROMDATE = FromDate;
            crmst.TODATE = ToDate;
            crmst.REMARKS = Remarks;
            crmst.ADDEDBY = AddedBy;
            crmst.ADDEDON = AddedOn;
            crmst.UPDATEDBY = UpdatedBy;
            crmst.UPDATEDON = UpdatedOn;
            crmst.ACTIVE = (short)Active;
            crmst.LOGMODE = Mode;
            crmst.MAINTBLSRNO = (int)MaintblSrNo;

                _DB.Entry(crmst).State = EntityState.Added;

                _DB.LC_EXCEPTION_C_TABLE_LOG.Add(crmst);
                _DB.SaveChanges();
        //}
    }
    catch (Exception ex)
    {
        return false;
    }

    return true;
}

//private bool InsertLC_OH_DirApprovalFlow_Master_LOG(string RequestorDesg, string ApprovalDesg, string UpdatedBy, DateTime? UpdatedDate, string Mode, long? MaintblSrNo = 0)
//{
//    try
//    {
//        //using (var db = new LCEntities())
//        //{
//            LC_OH_DIRAPPROVALFLOW_MASTER_LOG DMasterLog = new LC_OH_DIRAPPROVALFLOW_MASTER_LOG();
//            var maxSrno = _DB.LC_OH_DIRAPPROVALFLOW_MASTER_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);

//            DMasterLog.SRNO = maxSrno + 1;
//            DMasterLog.REQUESTOR_DESG = RequestorDesg;
//            DMasterLog.APPROVAL_DESG = ApprovalDesg;
//            DMasterLog.UPDATEDBY = UpdatedBy;
//            DMasterLog.UPDATEDDATE = UpdatedDate ?? DateTime.Now;
//            DMasterLog.LOGMODE = Mode;
//            DMasterLog.MAINTBLSRNO = long.Parse(MaintblSrNo.ToString());

//                _DB.Entry(DMasterLog).State = EntityState.Added;

//                _DB.LC_OH_DIRAPPROVALFLOW_MASTER_LOG.Add(DMasterLog);
//                _DB.SaveChanges();
//        //}
//    }
//    catch (Exception ex)
//    {
//        return false;
//    }

//    return true;
//}



        private bool InsertLC_OH_DirApprovalFlow_Master_LOG(
            string RequestorDesg,
            string ApprovalDesg,
            string UpdatedBy,
            DateTime? UpdatedDate,
            string Mode,
            long? MaintblSrNo = 0)
        {
            try
            {
                // Generate SRNO manually (since no PK exists)
                //var maxSrno = _DB.LC_OH_DIRAPPROVALFLOW_MASTER_LOG
                //           .FromSqlRaw("SELECT NVL(MAX(SRNO), 0) AS SRNO FROM LC_OH_DIRAPPROVALFLOW_MASTER_LOG")
                //           .AsEnumerable() // Convert to in-memory
                //           .FirstOrDefault()?.SRNO ?? 0;


                var maxSrno = _DB.LC_OH_DIRAPPROVALFLOW_MASTER_LOG
                    .FromSqlRaw("SELECT NVL(MAX(SRNO), 0) AS SRNO FROM LC_OH_DIRAPPROVALFLOW_MASTER_LOG")
                    .Select(x => x.SRNO)
                    .FirstOrDefault();



                //var maxSrno = _DB.Database.SqlQuery<long>("SELECT NVL(MAX(SRNO), 0) FROM LC_OH_DIRAPPROVALFLOW_MASTER_LOG")
                //    .First();



                var newSrno = maxSrno + 1;
                var updateDate = UpdatedDate ?? DateTime.Now;

                // Use raw SQL for insert
                string sql = @"
            INSERT INTO LC_OH_DIRAPPROVALFLOW_MASTER_LOG
            (SRNO, REQUESTOR_DESG, APPROVAL_DESG, UPDATEDBY, UPDATEDDATE, LOGMODE, MAINTBLSRNO)
            VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})";

                _DB.Database.ExecuteSqlRaw(sql, newSrno, RequestorDesg, ApprovalDesg, UpdatedBy, updateDate, Mode, MaintblSrNo);

                return true;
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return false;
            }
        }





        //Added on 31-03-2025
        private bool InsertLC_HEAD_LOG(LC_HEAD hd, string Mode)
{
    try
    {
        //using (var db = new LCEntities())
        //{
            LC_HEAD_LOG HeadLog = new LC_HEAD_LOG();
            var maxSrno = _DB.LC_HEAD_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);

            HeadLog.SRNO = maxSrno + 1;
            HeadLog.EMPCODE = hd.EMPCODE;
            HeadLog.LCTRANNO = hd.LCTRANNO;
            HeadLog.LCTRANDATE = hd.LCTRANDATE;
            HeadLog.PURPOSE = hd.PURPOSE;
            HeadLog.TOTAL_AMOUNT = hd.TOTAL_AMOUNT;
            HeadLog.TOTAL_AMT_IN_WORD = hd.TOTAL_AMT_IN_WORD;
            HeadLog.APPROVER_USER = hd.APPROVER_USER;
            HeadLog.APPROVER_STATUS = hd.APPROVER_STATUS;
            HeadLog.APPROVER_DATETIME = hd.APPROVER_DATETIME;
            HeadLog.ADMIN_USER = hd.ADMIN_USER;
            HeadLog.ADMIN_STATUS = hd.ADMIN_STATUS;
            HeadLog.ADMIN_DATETIME = hd.ADMIN_DATETIME;
            HeadLog.FINANCE_USER = hd.FINANCE_USER;
            HeadLog.FINANCE_STATUS = hd.FINANCE_STATUS;
            HeadLog.FINANCE_DATETIME = hd.FINANCE_DATETIME;
            HeadLog.FUAL = hd.FUAL;
            HeadLog.REQUIRED_AMOUNT = hd.REQUIRED_AMOUNT;
            HeadLog.ADDEDBY = hd.ADDEDBY;
            HeadLog.ADDEDON = hd.ADDEDON;
            HeadLog.UPDATEDBY = hd.UPDATEDBY;
            HeadLog.UPDATEDON = hd.UPDATEDON;
            HeadLog.TOTAL_NOOFHOURS = hd.TOTAL_NOOFHOURS;
            HeadLog.TOTAL_KMCOVERED = hd.TOTAL_KMCOVERED;
            HeadLog.DESIGNATION = hd.DESIGNATION;
            HeadLog.EMPNAME = hd.EMPNAME;
            HeadLog.APPROVER_REMARKS = hd.APPROVER_REMARKS;
            HeadLog.ADMIN_REMARKS = hd.ADMIN_REMARKS;
            HeadLog.FINANCE_REMARKS = hd.FINANCE_REMARKS;
            HeadLog.APPROVED_FUEL = hd.APPROVED_FUEL;
            HeadLog.RECOMM_USER = hd.RECOMM_USER;
            HeadLog.RECOMM_STATUS = hd.RECOMM_STATUS;
            HeadLog.RECOMM_DATETIME = hd.RECOMM_DATETIME;
            HeadLog.POSTINGDATE = hd.POSTINGDATE;
            HeadLog.FINAL_STATUS = hd.FINAL_STATUS;
            HeadLog.RECOMM_REMARKS = hd.RECOMM_REMARKS;
            HeadLog.TOTAL_MEAL_AMOUNT = hd.TOTAL_MEAL_AMOUNT;
            HeadLog.TOTAL_CONV_AMOUNT = hd.TOTAL_CONV_AMOUNT;
            HeadLog.PDF_NAME = hd.PDF_NAME;
            HeadLog.SAP_DOC_NO = hd.SAP_DOC_NO;
            HeadLog.PPPLANT = hd.PPPLANT;
            HeadLog.ADMINFUEL_DH_USER = hd.ADMINFUEL_DH_USER;
            HeadLog.ADMINFUEL_DH_STATUS = hd.ADMINFUEL_DH_STATUS;
            HeadLog.ADMINFUEL_DH_REMARKS = hd.ADMINFUEL_DH_REMARKS;
            HeadLog.ADMINFUEL_DH_DATETIME = hd.ADMINFUEL_DH_DATETIME;
            HeadLog.ADMINFUEL_OH_USER = hd.ADMINFUEL_OH_USER;
            HeadLog.ADMINFUEL_OH_STATUS = hd.ADMINFUEL_OH_STATUS;
            HeadLog.ADMINFUEL_OH_REMARKS = hd.ADMINFUEL_OH_REMARKS;
            HeadLog.ADMINFUEL_OH_DATETIME = hd.ADMINFUEL_OH_DATETIME;
            HeadLog.LOGMODE = Mode;
            HeadLog.ADMINFUEL_DOC = hd.ADMINFUEL_DOC;

                    _DB.Entry(HeadLog).State = EntityState.Added;

                    _DB.LC_HEAD_LOG.Add(HeadLog);
                    _DB.SaveChanges();
       // }
    }
    catch (Exception ex)
    {
        return false;
    }

    return true;
}


private bool InsertLC_Detail_LOG(List<LC_DETAIL> dtlist, string Mode)
{
    try
    {
        //using (var db = new LCEntities())
        //{
            foreach (var dt in dtlist)
            {
                LC_DETAIL_LOG DetailLog = new LC_DETAIL_LOG();
                var maxSrno = _DB.LC_DETAIL_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);

                DetailLog.SRNO = maxSrno + 1;
                DetailLog.ASOFFODID = dt.ASOFFODID;
                DetailLog.EMPCODE = dt.EMPCODE;
                DetailLog.EMPNAME = dt.EMPNAME;
                DetailLog.DATEOFEXPENDITURE = dt.DATEOFEXPENDITURE;
                DetailLog.PURPOSE = dt.PURPOSE;
                DetailLog.FROMCITY = dt.FROMCITY;
                DetailLog.TOCITY = dt.TOCITY;
                DetailLog.FROMCITYID = dt.FROMCITYID;
                DetailLog.TOCITYID = dt.TOCITYID;
                DetailLog.TIMEIN = dt.TIMEIN;
                DetailLog.TIMEOUT = dt.TIMEOUT;
                DetailLog.NOOFHOURS = dt.NOOFHOURS;
                DetailLog.MODEOFTRAVEL = dt.MODEOFTRAVEL;
                DetailLog.KMCOVERED = dt.KMCOVERED;
                DetailLog.RATEPERKM = dt.RATEPERKM;
                DetailLog.TRAVELREIMBURSEMENT = dt.TRAVELREIMBURSEMENT;
                DetailLog.REFRESHMENTMEALALLOWANCE = dt.REFRESHMENTMEALALLOWANCE;
                DetailLog.TOLLTAXMISCAMOUNT = dt.TOLLTAXMISCAMOUNT;
                DetailLog.TOTAL = dt.TOTAL;
                DetailLog.DT = dt.DT;
                DetailLog.ADDEDBY = dt.ADDEDBY;
                DetailLog.ADDEDON = dt.ADDEDON;
                DetailLog.UPDATEDBY = dt.UPDATEDBY;
                DetailLog.UPDATEDON = dt.UPDATEDON;
                DetailLog.LCTRANDATE = dt.LCTRANDATE;
                DetailLog.LCTRANNO = dt.LCTRANNO;
                DetailLog.FUEL_REIMBURSMENT = dt.FUEL_REIMBURSMENT;
                DetailLog.MODEOFTRAVEL_OTHER = dt.MODEOFTRAVEL_OTHER;
                DetailLog.FROMCITY_OTHER = dt.FROMCITY_OTHER;
                DetailLog.TOCITY_OTHER = dt.TOCITY_OTHER;
                DetailLog.MEAL_REFRE_ALLW_CLAIM = dt.MEAL_REFRE_ALLW_CLAIM;
                DetailLog.FUEL_ALLW_CLAIM = dt.FUEL_ALLW_CLAIM;
                DetailLog.STATUS = dt.STATUS;
                DetailLog.DINNER_ALLW_CLAIM = dt.DINNER_ALLW_CLAIM;
                DetailLog.LUNCH_ALLW_CLAIM = dt.LUNCH_ALLW_CLAIM;
                DetailLog.DINNER_ALLW_AMOUNT = dt.DINNER_ALLW_AMOUNT;
                DetailLog.LUNCH_ALLW_AMOUNT = dt.LUNCH_ALLW_AMOUNT;
                DetailLog.TOURSTARTFROMHOMEKM = dt.TOURSTARTFROMHOMEKM;
                DetailLog.FIN_CONV_AMOUNT = dt.FIN_CONV_AMOUNT;
                DetailLog.FIN_MEAL_REFRE_AMOUNT = dt.FIN_MEAL_REFRE_AMOUNT;
                DetailLog.LOGMODE = Mode;

                        _DB.Entry(DetailLog).State = EntityState.Added;

                        _DB.LC_DETAIL_LOG.Add(DetailLog);
            }
                    _DB.SaveChanges();
       // }
    }
    catch (Exception ex)
    {
        return false;
    }

    return true;
}


#endregion

#region OH Directore maping Master

public List<LC_OH_DIRAPPROVALFLOW_MASTERViewModel> GetLC_OH_DirApprovalFlowList()
{
    List<LC_OH_DIRAPPROVALFLOW_MASTERViewModel> Ilist = new List<LC_OH_DIRAPPROVALFLOW_MASTERViewModel>();

    try
    {
        Ilist = (from i in _DB.LC_OH_DIRAPPROVALFLOW_MASTER

                 select new LC_OH_DIRAPPROVALFLOW_MASTERViewModel
                 {
                     REQUESTOR_DESG = i.REQUESTOR_DESG,
                     APPROVAL_DESG = i.APPROVAL_DESG,
                     SRNO = i.SRNO
                 }

                                                   ).ToList();




    }
    catch (Exception ex)
    {

        throw ex;
    }

    return Ilist;
}

public LC_OH_DIRAPPROVALFLOW_MASTER GetEditDataforLC_OH_DIRAPPROVALFLOW_MASTERViewModel(int id)
{

            return _DB.LC_OH_DIRAPPROVALFLOW_MASTER
                .Where(x => x.SRNO == id)
                .FirstOrDefault();



            //return _DB.LC_OH_DIRAPPROVALFLOW_MASTER.Find(id);
}

public LibResult EditDataforLC_OH_DIRAPPROVALFLOW_MASTER(int SrNo, string RequestorDesg, string ApprovalDesg, int LoginCode)
{
    LibResult Res = new LibResult();
    try
    {

                //using (var db = new LCEntities())
                //{
                //LC_OH_DIRAPPROVALFLOW_MASTER crmst = _DB.LC_OH_DIRAPPROVALFLOW_MASTER.Find(SrNo);
            //    LC_OH_DIRAPPROVALFLOW_MASTER crmst = _DB.LC_OH_DIRAPPROVALFLOW_MASTER
            //        .Where(x => x.SRNO == SrNo)
            //        .FirstOrDefault();

            //crmst.REQUESTOR_DESG = RequestorDesg;
            //crmst.APPROVAL_DESG = ApprovalDesg;

            //crmst.UPDATEDBY = LoginCode.ToString();
            //crmst.UPDATEDDATE = DateTime.Now;

            //    _DB.Entry(crmst).State = EntityState.Modified;
            //    _DB.SaveChanges();

                _DB.Database.ExecuteSqlRawAsync(
                "UPDATE LC_OH_DIRAPPROVALFLOW_MASTER SET REQUESTOR_DESG = {0}, APPROVAL_DESG = {1}, UPDATEDBY = {2}, UPDATEDDATE = {3} WHERE SRNO = {4}", RequestorDesg, ApprovalDesg, LoginCode.ToString(), DateTime.Now, SrNo);

                var flag = InsertLC_OH_DirApprovalFlow_Master_LOG(RequestorDesg, ApprovalDesg, LoginCode.ToString(), DateTime.Parse(DateTime.Now.ToString()), "UPDATED", SrNo);


            Res.hasError = false;
            Res.errorMessage = "Record Updated successfully";
        //}
    }
    catch (Exception ex)
    {
        Res.hasError = true;
        Res.errorMessage = ex.ToString();

    }
    return Res;
}
#endregion

#region Transaction Log

public bool InsertTransactionLog(LC_HEAD hd, LC_DETAIL dt, string LogMode, int LogID)
{
    bool res = false;
    // LogID  :: Request Submit - 1 ,Re - Submit( in case of Request Sendback) - 2,Recommandation & Approval -  3 ,Finance - 4 ,Admin-  5 ,DH & OH -  6
    res = true;
    // Insert into Header Log
    if (new int[] { 3, 5, 6 }.Contains(LogID))
    {
        // Insert into Header Log
        res = true;
    }
    return res;
}

#endregion

#region Approval History

public System.Data.DataTable GetApprovalHistory(int lctranno)
{
    System.Data.DataTable Dt = new System.Data.DataTable();
    try
    {
        //CommonFunctions fn = new CommonFunctions();

        Dt = _iCmmnFunction.LC_GetApprovalHistory(lctranno);
    }
    catch (Exception ex)
    {

        throw ex;
    }

    return Dt;
}

#endregion

#region Convert Reject To Send Back
public List<LC_HEAD> GetListDataForRejectedRequest(int loginCode)
{
    try
    {

        var plantId = _DB.VW_ASSOCIATELVLDETAILS_LC
            .Where(x => x.ADEMPCODE == loginCode && x.SYKI == _Syki && x.ACTIVE == 1)
            .Select(x => x.SYPLANTID)
            .FirstOrDefault();

        //bool hasFinanceRole = _DB.LC_ROLEMASTER
        //    .Any(x => x.PLANTID == plantId && x.EMPCODE == loginCode && x.ROLE == "FINANCE");

                bool hasFinanceRole = _DB.LC_ROLEMASTER
            .Count(x => x.PLANTID == plantId && x.EMPCODE == loginCode && x.ROLE == "FINANCE")>0;
               

                if (!hasFinanceRole)
        {
            return new List<LC_HEAD>();
        }


        var result = (from h in _DB.LC_HEAD
                      join t in _DB.LC_DETAIL_TEMP on h.LCTRANNO equals t.TEMPID
                      where h.APPROVER_STATUS == "Rejected"
                            || h.RECOMM_STATUS == "Rejected"
                            || h.FINANCE_STATUS == "Rejected"
                            || t.ISSUBMITTED == 4
                      select h).ToList();

        return result;
    }
    catch (Exception)
    {
        return new List<LC_HEAD>();
    }
}


public LibResult OpenRejectedRequest(int lctranno, int empcode)
{
    var result = new LibResult();

    try
    {
        var dtTemp = _DB.LC_DETAIL_TEMP
            .Where(x => x.TEMPID == lctranno && x.EMPCODE == empcode)
            .ToList();

        foreach (var item in dtTemp)
        {
            item.ISSUBMITTED = 3;
            _DB.Entry(item).State = EntityState.Modified;
        }

        var lc_head = _DB.LC_HEAD
            .Where(x => x.LCTRANNO == lctranno && x.EMPCODE == empcode)
            .FirstOrDefault();

        if (lc_head.APPROVER_STATUS == "Rejected")
        {
            lc_head.APPROVER_STATUS = "SendBack";
        }
        else
        {
            lc_head.APPROVER_STATUS = "";

        }


        if (lc_head.RECOMM_STATUS == "Rejected")
        {
            lc_head.RECOMM_STATUS = "SendBack";
        }
        else
        {
            lc_head.RECOMM_STATUS = "";

        }


        if (lc_head.FINANCE_STATUS == "Rejected")
        {
            lc_head.FINANCE_STATUS = "SendBack";
        }
        else
        {
            lc_head.FINANCE_STATUS = "";

        }



        _DB.SaveChanges();
        var flag = InsertLC_HEAD_LOG(lc_head, "Update");
        result.hasError = false;
    }
    catch (Exception)
    {

        result.hasError = true;
        throw;
    }

    return result;
}

#endregion
       // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
    }
}
