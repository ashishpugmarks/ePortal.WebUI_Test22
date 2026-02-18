using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace ePortal.Infrastructure.Repositories
{
    
    public class MedicalInsuranceRepository
    {
        private EPortalDBContext _MedicalDBContext;
        private ISessionService _sessionService;
        public MedicalInsuranceRepository(EPortalDBContext empLoginDBContext, ISessionService sessionService)
        {
            _MedicalDBContext = empLoginDBContext;
            _sessionService= sessionService;
           
        }

        #region Dashboard
        public int GetIsExistNewJoinee(long Emp_Code)
        {
            var obj = _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == Emp_Code).SingleOrDefault();
            return obj == null ? 0 : 1;
        }
        public int GetIsExistRenewal(long PlantId, long UserType)
        {
            var CurrentDate = DateTime.Now;
            var obj = _MedicalDBContext.MEDINS_RENEWAL_SETTING.Where(x => x.PLANTID == PlantId
            && CurrentDate.Date >= x.PERIOD_FROM && CurrentDate.Date <= x.PERIOD_TO && x.STATUS == 1 && x.USERTYPE == UserType).ToList();
            return obj.Count() == 0 ? 0 : 1;
        }
        public PolicyAndPlantMappingViewModel GetDashboardInfo(long DesId, long EmpCode)
        {
            PolicyAndPlantMappingViewModel PPVM = new PolicyAndPlantMappingViewModel();
            var plant = _MedicalDBContext.MEDINS_EMPPOLICYLOC.Where(x => x.STATUS == 1 && x.ADEMPCODE == EmpCode).FirstOrDefault();
            var type = _MedicalDBContext.MEDINS_POLICYTYPE_MST.Where(x => x.STATUS == 1 && x.ISDEPENDENTREQ == 1).FirstOrDefault();
            int Company_Count = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == EmpCode && x.DEP_PAIDTYPE == 2 && (x.STATUS == 1 || x.STATUS == 2) && (x.CHANGETYPE != 3)).Count();
            int Associated_Count = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == EmpCode && x.DEP_PAIDTYPE == 1 && (x.STATUS == 1 || x.STATUS == 2) && (x.CHANGETYPE != 3)).Count();
            int ApprovalStatus = _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.EMP_CODE == EmpCode && (x.APPROVAL_STATUS == 0 || x.APPROVAL_STATUS == 3)).Count();

            if (plant != null)
            {
                var PlantLocation = (from data in _MedicalDBContext.MEDINS_TYPEPLANT_MAP.Where(x => x.PLANTID == plant.PLANTID && x.POLICYTYPE_ID == type.POLICYTYPE_ID)
                                     join DESG_POLICYTYPE in _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP on data.MAPPINGID equals DESG_POLICYTYPE.MAPPINGID
                                     join PLANT in _MedicalDBContext.SYPLANT on data.PLANTID equals PLANT.SYPLANTID
                                     where (DESG_POLICYTYPE.DESG_ID == DesId)
                                     select new
                                     {
                                         data.COMPANYPAID_CNT,
                                         data.ASSOCIATEPAID_CNT,
                                         data.PLANTID,
                                         PLANT.PLANTNAME
                                     }).FirstOrDefault();
                if (PlantLocation != null)
                {
                    PPVM.AssociatedPaid_Cnt = Convert.ToInt16(Associated_Count);
                    PPVM.CompanyPaid_Cnt = Convert.ToInt16(Company_Count);
                    PPVM.TotalCompanyPaid = PlantLocation.COMPANYPAID_CNT;
                    PPVM.TotalAssociatedPaid = PlantLocation.ASSOCIATEPAID_CNT;
                    PPVM.Plant = PlantLocation.PLANTNAME;
                    PPVM.PlantId = PlantLocation.PLANTID;
                    PPVM.ApprovalStatus = Convert.ToInt16(ApprovalStatus);
                }
                var RenewalPeriod = _MedicalDBContext.MEDINS_RENEWAL_SETTING.Where(x => x.PLANTID == plant.PLANTID && x.STATUS == 1).FirstOrDefault();
                if (RenewalPeriod != null)
                {
                    PPVM.Renewal_StartDate = RenewalPeriod.PERIOD_FROM.ToString("dd-MMM-yyyy");
                    PPVM.Renewal_EndDate = RenewalPeriod.PERIOD_TO.ToString("dd-MMM-yyyy");
                }
            }

            return PPVM;
        }
        public MedicalInsuranceViewModel UserPendingRequest(long EmpCode)
        {
            MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
            var obj = (from data in _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.EMP_CODE == EmpCode && (x.APPROVAL_STATUS == 0 || x.APPROVAL_STATUS == 3))
                       select new
                       {
                           data.EMP_CODE,
                           data.CREATEDDATE,
                           data.REQUESTTYPE,
                           data.APPROVAL_STATUS,
                           data.APPROVAL_DATE,
                           data.APPROVAL_BY,
                       }).FirstOrDefault();
            if (obj != null)
            {
                MIVM.CreatedDate = obj.CREATEDDATE?.ToString("dd-MMM-yyyy");
                MIVM.EmpCode = Convert.ToInt64(obj.EMP_CODE);
                MIVM.RequestType = obj.REQUESTTYPE;
                MIVM.Approval_Status = obj.APPROVAL_STATUS;
                MIVM.Approval_By = obj.APPROVAL_BY.ToString();
                MIVM.Approval_Date = Convert.ToDateTime(obj.APPROVAL_DATE).ToString("dd-MMM-yyyy");
                MIVM.Approval_Status = obj.APPROVAL_STATUS;
            }
            return MIVM;
        }
        #endregion

        #region  Masters
        public IEnumerable<SYSITE> Bind_SYSite()
        {
            IEnumerable<SYSITE> iList;
            iList = (from data in _MedicalDBContext.SYSITE.Where(x => x.ACTIVE == 1).ToList()
                     select new SYSITE
                     {
                         SYSITEID = data.SYSITEID,
                         DESCRIP = data.DESCRIP,
                     });
            return iList;
        }

        public IEnumerable<SYPLANT> Bind_SYPlant()
        {
            IEnumerable<SYPLANT> iList;
            iList = (from data in _MedicalDBContext.SYPLANT.Where(x => x.ACTIVE == 1).ToList()
                     select new SYPLANT
                     {
                         SYPLANTID = data.SYPLANTID,
                         PLANTNAME = data.PLANTNAME,
                     });
            return iList;
        }

        public IEnumerable<ADDESIGNATION> Bind_ADDesignation()
        {
            IEnumerable<ADDESIGNATION> iList;
            iList = (from data in _MedicalDBContext.ADDESIGNATION.Where(x => x.ACTIVE == 1).ToList()
                     select new ADDESIGNATION
                     {
                         ADDESIGNATIONID = data.ADDESIGNATIONID,
                         DESCRIP = data.DESCRIP,
                     });
            return iList;
        }

        public List<PolicyMasterViewModel> GetPolicyTypeList()
        {
            List<PolicyMasterViewModel> iList = new List<PolicyMasterViewModel>();
            var iColl = (from data in _MedicalDBContext.MEDINS_POLICYTYPE_MST.Where(x => x.STATUS == 1)
                         select new
                         {
                             data.POLICYTYPE_ID,
                             data.TYPECODE,
                         }).ToList();
            if (iColl.Count() > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new PolicyMasterViewModel
                    {
                        PolicyTypeId = obj.POLICYTYPE_ID,
                        TypeCode = obj.TYPECODE,
                    });
                }
            }
            return iList;
        }

        public Int16 SaveRenewalPeriod(RenewalPeriodViewModel RPVM)
        {
            Int16 retVal = 0;
            try
            {
                MEDINS_RENEWAL_SETTING MIRS = new MEDINS_RENEWAL_SETTING();
                SYKI syki = _MedicalDBContext.SYKI.Where(s => s.ACTIVE == 1).FirstOrDefault();
                DateTime FromDate = DateTime.ParseExact(RPVM.FromDate, "dd-MMM-yyyy", null);
                DateTime ToDate = DateTime.ParseExact(RPVM.ToDate, "dd-MMM-yyyy", null);
                List<MEDINS_RENEWAL_SETTING> IsExistObj = _MedicalDBContext.MEDINS_RENEWAL_SETTING.Where(m => m.PLANTID == RPVM.PlantId && m.PERIOD_FROM >= FromDate && m.PERIOD_TO <= ToDate).ToList();
                if (IsExistObj.Count() == 0)
                {
                    if (syki != null)
                    {
                        if (RPVM.Plant_Ids != null)
                        {
                            //Call max value for increment
                            var id = _MedicalDBContext.MEDINS_RENEWAL_SETTING.Max(x => x.RENEWALID)+1;
                            foreach (var plant in RPVM.Plant_Ids)
                            {
                                MIRS = new MEDINS_RENEWAL_SETTING();
                                MIRS.RENEWALID = id;
                                MIRS.PLANTID = Convert.ToInt16(plant);
                                MIRS.SYKIID = syki.SYKIID;
                                MIRS.PERIOD_FROM = DateTime.ParseExact(RPVM.FromDate, "dd-MMM-yyyy", null);
                                MIRS.PERIOD_TO = DateTime.ParseExact(RPVM.ToDate, "dd-MMM-yyyy", null);
                                MIRS.STATUS = 1; // Active 
                                MIRS.USERTYPE = RPVM.UserType;
                                MIRS.CREATEDBY = Convert.ToInt64(RPVM.CreatedBy);
                                MIRS.CREATEDDATE = DateTime.Now;
                                _MedicalDBContext.MEDINS_RENEWAL_SETTING.Add(MIRS);
                                _MedicalDBContext.SaveChanges();
                                id++;
                            }
                            ;
                            retVal = 1;
                        }
                    }
                }
                else
                {
                    retVal = 2;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public List<EmployeePolicyLocationMapping> GetEmpForSendMail(Int16 PlanId)
        {
            List<EmployeePolicyLocationMapping> iList = new List<EmployeePolicyLocationMapping>();
            var iColl = (from data in _MedicalDBContext.MEDINS_EMPPOLICYLOC.Where(e => e.STATUS == 1 && e.PLANTID == PlanId)
                         join PLANT in _MedicalDBContext.SYPLANT on data.PLANTID equals PLANT.SYPLANTID
                         join ADEMP in _MedicalDBContext.ADEMPLOYEE on data.ADEMPCODE equals ADEMP.ADEMPCODE
                         select new
                         {
                             EMPPOLICYLOC_ID = data.EMPPOLICYLOC_ID,
                             ADEMPCODE = data.ADEMPCODE,
                             EmpFirstName = ADEMP.FIRSTNAME,
                             EmpLastName = ADEMP.LASTNAME,
                             EmpEmail = ADEMP.EMAILID,
                             PLANTID = data.PLANTID,
                             PLANTNAME = PLANT.PLANTNAME,
                             CREATEDDATE = data.CREATEDDATE,
                             STATUS = data.STATUS,
                         }).ToList();
            if (iColl.Count() > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new EmployeePolicyLocationMapping
                    {
                        EmpPolicyLocId = obj.EMPPOLICYLOC_ID,
                        EmpId = obj.ADEMPCODE,
                        Emp_Email = obj.EmpEmail,
                        Employee = obj.EmpFirstName + " " + obj.EmpLastName,
                        PlantId = Convert.ToInt64(obj.PLANTID),
                        Plant = obj.PLANTNAME,
                        CreatedDate = obj.CREATEDDATE,
                        Status = obj.STATUS,
                    });
                }
            }
            return iList;
        }

        public ADEmployeeViewModel GetADEmployeeDetail(long EmpCode)
        {
            ADEmployeeViewModel EVM = new ADEmployeeViewModel();
            var obj = (from data in _MedicalDBContext.ADEMPLOYEE.Where(e => e.ACTIVE == 1 && e.ADEMPCODE == EmpCode)
                       select new
                       {
                           data.ADEMPCODE,
                           data.FIRSTNAME,
                           data.LASTNAME,
                           data.EMAILID,
                       }).FirstOrDefault();
            if (obj != null)
            {
                EVM.EmpId = Convert.ToInt64(obj.ADEMPCODE);
                EVM.EmpName = obj.FIRSTNAME + " " + obj.LASTNAME;
                EVM.Email = obj.EMAILID;
            }
            return EVM;
        }

        public Int16 DeactivateRenewalPeriod(RenewalPeriodViewModel RPVM)
        {
            Int16 retVal = 0;
            try
            {
                MEDINS_RENEWAL_SETTING MIRS = new MEDINS_RENEWAL_SETTING();
                MIRS = _MedicalDBContext.MEDINS_RENEWAL_SETTING.Find(RPVM.RenewalId);
                if (MIRS != null)
                {
                    MIRS.STATUS = 0;
                    MIRS.MODIFIEDBY = Convert.ToInt64(RPVM.ModifiedBy);
                    MIRS.MODIFIEDDATE = DateTime.Now;
                    _MedicalDBContext.Entry(MIRS).State = EntityState.Modified;
                    _MedicalDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public List<RenewalPeriodViewModel> GetRenewalPeriodList()
        {
            List<RenewalPeriodViewModel> iList = new List<RenewalPeriodViewModel>();
            var iColl = (from data in _MedicalDBContext.MEDINS_RENEWAL_SETTING
                         join PLANT in _MedicalDBContext.SYPLANT on data.PLANTID equals PLANT.SYPLANTID
                         join KI in _MedicalDBContext.SYKI on data.SYKIID equals KI.SYKIID
                         select new
                         {
                             data.RENEWALID,
                             data.PLANTID,
                             PLANT.PLANTNAME,
                             data.SYKIID,
                             KI.KICODE,
                             data.PERIOD_FROM,
                             data.PERIOD_TO,
                             data.CREATEDDATE,
                             data.MODIFIEDDATE,
                             data.STATUS,
                             data.USERTYPE
                         }).OrderByDescending(o => o.RENEWALID).ToList();
            if (iColl.Count() > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new RenewalPeriodViewModel
                    {
                        RenewalId = obj.RENEWALID,
                        PlantId = obj.PLANTID,
                        Plant = obj.PLANTNAME,
                        KiId = Convert.ToInt64(obj.SYKIID),
                        Ki = obj.KICODE,
                        FromDate = obj.PERIOD_FROM.ToString("dd-MMM-yyyy"),
                        ToDate = obj.PERIOD_TO.ToString("dd-MMM-yyyy"),
                        CreatedDate = obj.CREATEDDATE,
                        ModifiedDate = obj.MODIFIEDDATE,
                        Status = obj.STATUS,
                        UserType = obj.USERTYPE
                    });
                }
            }
            return iList;
        }

        public Int16 SaveEmployeePolicyLocation(EmployeePolicyLocationMapping EPLM)
        {
            Int16 retVal = 0;
            try
            {
                var longArray = EPLM.Employee.Split(',').Select(long.Parse).ToArray();

                var olist = (from odata in _MedicalDBContext.ADEMPLOYEE
                             where longArray.Contains(odata.ADEMPCODE) && odata.ACTIVE == 1
                             select odata.ADEMPCODE
                           ).ToList();

                if (!(olist.Count() == longArray.Length))
                {
                    retVal = -2;
                    return retVal;
                }
                MEDINS_EMPPOLICYLOC MIEP = new MEDINS_EMPPOLICYLOC();
                if (!string.IsNullOrEmpty(EPLM.Employee))
                {
                    foreach (var EmpCode in EPLM.Employee.Split(',').ToArray())
                    {
                        long ecode = Convert.ToInt64(EmpCode);
                        var IsExistecode = _MedicalDBContext.MEDINS_EMPPOLICYLOC.Where(m => m.ADEMPCODE == ecode).ToList();
                        if (!string.IsNullOrEmpty(EmpCode) && IsExistecode.Count() == 0)
                        {
                            //Added code for Autoincrement value
                            MIEP.EMPPOLICYLOC_ID = _MedicalDBContext.MEDINS_EMPPOLICYLOC.Max(x => x.EMPPOLICYLOC_ID) + 1;
                            MIEP.PLANTID = Convert.ToInt16(EPLM.PlantId);
                            MIEP.ADEMPCODE = Convert.ToInt64(EmpCode);
                            MIEP.STATUS = 1; // Active 
                            MIEP.CREATEDBY = Convert.ToInt64(EPLM.CreatedBy);
                            MIEP.CREATEDDATE = DateTime.Now;
                            _MedicalDBContext.MEDINS_EMPPOLICYLOC.Add(MIEP);
                            _MedicalDBContext.SaveChanges();
                        }
                    }
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
        public List<EmployeePolicyLocationMapping> GetEmployeePolicyLocationList()
        {
            List<EmployeePolicyLocationMapping> iList = new List<EmployeePolicyLocationMapping>();
            var iColl = (from data in _MedicalDBContext.MEDINS_EMPPOLICYLOC.Where(e => e.STATUS == 1)
                         join PLANT in _MedicalDBContext.SYPLANT on data.PLANTID equals PLANT.SYPLANTID
                         join ADEMP in _MedicalDBContext.ADEMPLOYEE on data.ADEMPCODE equals ADEMP.ADEMPCODE
                         select new
                         {
                             EMPPOLICYLOC_ID = data.EMPPOLICYLOC_ID,
                             ADEMPCODE = data.ADEMPCODE,
                             EmpFirstName = ADEMP.FIRSTNAME,
                             EmpLastName = ADEMP.LASTNAME,
                             PLANTID = data.PLANTID,
                             PLANTNAME = PLANT.PLANTNAME,
                             CREATEDDATE = data.CREATEDDATE,
                             STATUS = data.STATUS,
                         }).ToList();
            if (iColl.Count() > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new EmployeePolicyLocationMapping
                    {
                        EmpPolicyLocId = obj.EMPPOLICYLOC_ID,
                        EmpId = obj.ADEMPCODE,
                        Employee = obj.EmpFirstName + " " + obj.EmpLastName,
                        PlantId = Convert.ToInt64(obj.PLANTID),
                        Plant = obj.PLANTNAME,
                        CreatedDate = obj.CREATEDDATE,
                        Status = obj.STATUS,
                    });
                }
            }
            return iList;
        }

        public Int16 SaveMappingDetail(List<PolicyAndPlantMappingViewModel> PPVM)
        {
            Int16 retVal = 0;
            try
            {
                foreach (var obj in PPVM)
                {
                    if (obj != null && obj.DesignationIds.Length > 0)
                    {
                        var fdata = _MedicalDBContext.MEDINS_TYPEPLANT_MAP.Where(m => m.PLANTID == obj.PlantId && m.POLICYTYPE_ID == obj.PolicyTypeId).FirstOrDefault();
                        if (fdata != null)
                        {
                            fdata.ASSOCIATEPAID_CNT = obj.AssociatedPaid_Cnt;
                            fdata.COMPANYPAID_CNT = obj.CompanyPaid_Cnt;
                            fdata.MODIFIEDBY = obj.CreatedBy;
                            fdata.MODIFIEDDATE = DateTime.Now;
                            _MedicalDBContext.Entry(fdata).State = EntityState.Modified;
                            _MedicalDBContext.SaveChanges();
                            #region "Update"
                            List<MEDINS_DESG_POLICYTYPE_MAP> oDesgmap = _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP.Where(m => m.MAPPINGID == fdata.MAPPINGID && m.STAUS == 1).ToList();
                            foreach (MEDINS_DESG_POLICYTYPE_MAP objDesg in oDesgmap.Where(m => !obj.DesignationIds.Contains(m.DESG_ID)))
                            {
                                objDesg.STAUS = 0;
                                objDesg.MODIFIEDBY = obj.CreatedBy;
                                objDesg.MODIFIEDDATE = DateTime.Now;
                                _MedicalDBContext.Entry(objDesg).State = EntityState.Modified;
                                _MedicalDBContext.SaveChanges();
                            }
                            var desg = oDesgmap.Select(m => m.DESG_ID).Intersect(obj.DesignationIds);
                            foreach (var o in desg)
                                obj.DesignationIds = obj.DesignationIds.Where(x => x != o).ToArray();
                            //ob  j.DesignationIds.Any(x => (oDesgmap.Select(m => m.DESG_ID).Contains(x)));
                            MEDINS_DESG_POLICYTYPE_MAP MDPM = new MEDINS_DESG_POLICYTYPE_MAP();
                            foreach (var newdesg in obj.DesignationIds)
                            {
                                //Add code for auto increment value
                                MDPM.DESGMAPPINGID = _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP.Max(x => x.DESGMAPPINGID) + 1;
                                MDPM.MAPPINGID = fdata.MAPPINGID;
                                MDPM.DESG_ID = newdesg;
                                MDPM.STAUS = 1;
                                MDPM.CREATEDBY = Convert.ToInt64(obj.CreatedBy);
                                MDPM.CREATEDDATE = DateTime.Now;
                                _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP.Add(MDPM);
                                _MedicalDBContext.SaveChanges();
                            }
                            #endregion
                        }
                        else
                        {
                            #region "Save"
                            MEDINS_TYPEPLANT_MAP MTM = new MEDINS_TYPEPLANT_MAP();
                            //Add code for auto increment value
                            MTM.MAPPINGID = _MedicalDBContext.MEDINS_TYPEPLANT_MAP.Max(x => x.MAPPINGID) + 1;
                            MTM.PLANTID = obj.PlantId;
                            MTM.POLICYTYPE_ID = obj.PolicyTypeId;
                            MTM.COMPANYPAID_CNT = obj.CompanyPaid_Cnt;
                            MTM.ASSOCIATEPAID_CNT = obj.AssociatedPaid_Cnt;
                            MTM.STATUS = 1;
                            MTM.CREATEDBY = Convert.ToInt64(obj.CreatedBy);
                            MTM.CREATEDDATE = DateTime.Now;
                            _MedicalDBContext.MEDINS_TYPEPLANT_MAP.Add(MTM);
                            _MedicalDBContext.SaveChanges();

                            MEDINS_DESG_POLICYTYPE_MAP MDPM = new MEDINS_DESG_POLICYTYPE_MAP();
                            foreach (var desId in obj.DesignationIds)
                            {
                                //Add code for auto increment value
                                MDPM.DESGMAPPINGID = _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP.Max(x => x.DESGMAPPINGID) + 1;
                                MDPM.MAPPINGID = MTM.MAPPINGID;
                                MDPM.DESG_ID = desId;
                                MDPM.STAUS = 1;
                                MDPM.CREATEDBY = Convert.ToInt64(obj.CreatedBy);
                                MDPM.CREATEDDATE = DateTime.Now;
                                _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP.Add(MDPM);
                                _MedicalDBContext.SaveChanges();
                            }
                            #endregion
                        }
                    }
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public List<PolicyAndDesignationMappingViewModel> GetMappingList()
        {
            List<PolicyAndDesignationMappingViewModel> iList = new List<PolicyAndDesignationMappingViewModel>();
            var iColl = (from data in _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP.Where(x => x.STAUS == 1)
                         join otype in _MedicalDBContext.MEDINS_TYPEPLANT_MAP on data.MAPPINGID equals otype.MAPPINGID
                         join oPM in _MedicalDBContext.MEDINS_POLICYTYPE_MST.Where(x => x.STATUS == 1) on otype.POLICYTYPE_ID equals oPM.POLICYTYPE_ID
                         join odesg in _MedicalDBContext.ADDESIGNATION on data.DESG_ID equals odesg.ADDESIGNATIONID
                         join op in _MedicalDBContext.SYPLANT on otype.PLANTID equals op.SYPLANTID
                         select new
                         {
                             data.DESGMAPPINGID,
                             data.DESG_ID,
                             data.MAPPINGID,
                             otype.ASSOCIATEPAID_CNT,
                             otype.COMPANYPAID_CNT,
                             otype.CREATEDBY,
                             otype.CREATEDDATE,
                             otype.PLANTID,
                             otype.POLICYTYPE_ID,
                             otype.STATUS,
                             oPM.TYPECODE,
                             oPM.TYPE_DESC,
                             odesg.DESCRIP,
                             op.PLANTNAME,
                         }).ToList();
            if (iColl.Count() > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new PolicyAndDesignationMappingViewModel
                    {
                        DesignationMappId = obj.DESGMAPPINGID,
                        MappingId = obj.MAPPINGID,
                        DesignationId = obj.DESG_ID,
                        Designation = obj.DESCRIP,
                        PlantMappID = obj.PLANTID,
                        PlantName = obj.PLANTNAME,
                        PolicyName = obj.TYPECODE,
                        CreatedBy = obj.CREATEDBY,
                        CreatedDate = obj.CREATEDDATE,
                        Status = obj.STATUS,
                        PolicyTypeId = obj.POLICYTYPE_ID
                    });
                }
            }
            return iList;
        }

        public List<MEDINS_TYPEPLANT_MAP> GetPolicyDesgMappingList(PolicyAndPlantMappingViewModel oData)
        {
            try
            {
                var iColl = (from data in _MedicalDBContext.MEDINS_TYPEPLANT_MAP.Where(x => x.STATUS == 1 && x.POLICYTYPE_ID == oData.PolicyTypeId && x.PLANTID == oData.PlantId)
                             select data).ToList();
                return iColl;

            }
            catch
            {
                List<MEDINS_TYPEPLANT_MAP> ret = new List<MEDINS_TYPEPLANT_MAP>();
                return ret;
            }

        }

        //public List<MEDINS_TYPEPLANT_MAP> GetPolicyDesgMappingList(PolicyAndPlantMappingViewModel oData)
        //{
        //    try
        //    {
        //        var iColl = (from data in _MedicalDBContext.MEDINS_TYPEPLANT_MAP.Where(x => x.STATUS == 1 && x.POLICYTYPE_ID == oData.PolicyTypeId && x.PLANTID == oData.PlantId)
        //                     select data).ToList();
        //        //to handle MEDINS_DESG_POLICYTYPE_MAP
        //        if (iColl != null)
        //        {
        //            foreach (var i in iColl)
        //            {
        //                var lst123 = _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP.Where(x => x.MAPPINGID == 10).ToList();

        //                var lst = _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP.Where(x => x.MAPPINGID == i.MAPPINGID).ToList();
        //                i.MEDINS_DESG_POLICYTYPE_MAP = lst;


        //            }
        //        }
        //        return iColl;

        //    }
        //    catch
        //    {
        //        List<MEDINS_TYPEPLANT_MAP> ret = new List<MEDINS_TYPEPLANT_MAP>();
        //        return ret;
        //    }

        //}
        #endregion

        #region Save/Update/Detail
        public Int16 SaveMedicalInsuranceDetail(MedicalInsuranceViewModel MIVM)
        {
            Int16 retVal = 0;
            MEDINS_APPHISTORY APPHISTORY = _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.EMP_CODE == MIVM.EmpCode && (x.APPROVAL_STATUS == 0 || x.APPROVAL_STATUS == 3)).FirstOrDefault();
            if (APPHISTORY != null && MIVM.RequestType != 1)
            {
                retVal = 2; // Request already in process
                return retVal;
            }
            using (IDbContextTransaction transaction = _MedicalDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (APPHISTORY == null)
                    {
                        APPHISTORY = SaveApprovalHistory(MIVM);
                    }
                    #region "Creating History for Employee self and Dependent detail before initiation of new changes"
                    var hisemp = _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode).FirstOrDefault();
                    if (MIVM.RequestType == 1 && hisemp != null)
                    {
                        APPHISTORY.APPROVAL_STATUS = 0;
                        APPHISTORY.MODIFIED_DATE = DateTime.Now;
                        APPHISTORY.MODIFIED_BY = MIVM.CreatedBy;

                        _MedicalDBContext.Entry(APPHISTORY).State = EntityState.Modified;
                        _MedicalDBContext.SaveChanges();

                        _MedicalDBContext.Entry(hisemp).State = EntityState.Deleted;
                        _MedicalDBContext.SaveChanges();
                        var hisdep = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode);
                        foreach (var obj in hisdep)
                        {
                            _MedicalDBContext.Entry(obj).State = EntityState.Deleted;
                            _MedicalDBContext.SaveChanges();
                        }
                        hisemp = null;

                    }

                    if (hisemp != null)
                    {
                        
                        var hisdep = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode);
                        var MI_EMPDETAILIDHIS = _MedicalDBContext.MEDINS_EMPLOYEE_DTL_HIS.Max(x => x.MI_EMPDETAILIDHIS) + 1;
                        MEDINS_EMPLOYEE_DTL_HIS objemphis = new MEDINS_EMPLOYEE_DTL_HIS();
                        objemphis.MI_EMPDETAILIDHIS = MI_EMPDETAILIDHIS;
                        objemphis.APPROVALID = APPHISTORY.APPROVALID;
                        objemphis.CREATED_BY = hisemp.CREATED_BY;
                        objemphis.CREATED_DATE = hisemp.CREATED_DATE;
                        objemphis.DOB = hisemp.DOB;
                        objemphis.DOM = hisemp.DOM;
                        objemphis.EMP_CODE = hisemp.EMP_CODE;
                        objemphis.EMP_MODIFIEDDATE = hisemp.EMP_MODIFIEDDATE;
                        objemphis.EMP_NAME = hisemp.EMP_NAME;
                        objemphis.EMP_STATUS = hisemp.EMP_STATUS;
                        objemphis.GENDER = hisemp.GENDER;
                        objemphis.MODIFIEDBY = hisemp.MODIFIEDBY;
                        objemphis.NOMINEE_DOB = hisemp.NOMINEE_DOB;
                        objemphis.NOMINEE_GENDER = hisemp.NOMINEE_GENDER;
                        objemphis.NOMINEE_MODIFIEDDATE = hisemp.NOMINEE_MODIFIEDDATE;
                        objemphis.NOMINEE_NAME = hisemp.NOMINEE_NAME;
                        objemphis.NOMINEE_RELATION = hisemp.NOMINEE_RELATION;
                        objemphis.NOMINEE_STATUS = hisemp.NOMINEE_STATUS;
                        objemphis.PREVIOUS_EMPDOB = hisemp.PREVIOUS_EMPDOB;
                        objemphis.PREVIOUS_EMP_NAME = hisemp.PREVIOUS_EMP_NAME;
                        objemphis.PREVIOUS_NOMINEE_DOB = hisemp.PREVIOUS_NOMINEE_DOB;
                        objemphis.PREVIOUS_NOMINEE_GENDER = hisemp.PREVIOUS_NOMINEE_GENDER;
                        objemphis.PREVIOUS_NOMINEE_NAME = hisemp.PREVIOUS_NOMINEE_NAME;
                        objemphis.PREVIOUS_NOMINEE_RELATION = hisemp.PREVIOUS_NOMINEE_RELATION;
                        _MedicalDBContext.Entry(objemphis).State = EntityState.Added;
                        _MedicalDBContext.SaveChanges();

                        var MI_DEPENDENTIDHIS = _MedicalDBContext.MEDINS_DEPENDENTS_DTL_HIS.Max(x => x.MI_DEPENDENTIDHIS) + 1;
                        foreach (var obj in hisdep)
                        {
                            
                            MEDINS_DEPENDENTS_DTL_HIS objMedDep = new MEDINS_DEPENDENTS_DTL_HIS();
                            objMedDep.MI_DEPENDENTIDHIS= MI_DEPENDENTIDHIS;
                            objMedDep.APPROVALID = APPHISTORY.APPROVALID;
                            objMedDep.CHANGETYPE = obj.CHANGETYPE;
                            objMedDep.CREATED_BY = obj.CREATED_BY;
                            objMedDep.CREATED_DATE = obj.CREATED_DATE;
                            objMedDep.DEP_DOB = obj.DEP_DOB;
                            objMedDep.DEP_GENDER = obj.DEP_GENDER;
                            objMedDep.DEP_NAME = obj.DEP_NAME;
                            objMedDep.DEP_PAIDTYPE = obj.DEP_PAIDTYPE;
                            objMedDep.DEP_PHOTO = obj.DEPDEL_PROOF;
                            objMedDep.DEP_PHOTO_CONTYPE = obj.DEPDEL_PROOF_CONTYPE;
                            objMedDep.DEP_RELATIONSHIP = obj.DEP_RELATIONSHIP;
                            objMedDep.EMP_CODE = obj.EMP_CODE;
                            objMedDep.MODIFIED_BY = obj.MODIFIED_BY;
                            objMedDep.MODIFIED_DATE = obj.MODIFIED_DATE;
                            objMedDep.POLICYTYPE = obj.POLICYTYPE;
                            objMedDep.PREVIOUS_DEP_DOB = obj.PREVIOUS_DEP_DOB;
                            objMedDep.PREVIOUS_DEP_GENDER = obj.PREVIOUS_DEP_GENDER;
                            objMedDep.PREVIOUS_DEP_NAME = obj.PREVIOUS_DEP_NAME;
                            objMedDep.PREVIOUS_DEP_RELATIONSHIP = obj.PREVIOUS_DEP_RELATIONSHIP;
                            objMedDep.REMARKS = obj.REMARKS;
                            objMedDep.STATUS = obj.STATUS;
                            _MedicalDBContext.Entry(objMedDep).State = EntityState.Added;
                            _MedicalDBContext.SaveChanges();
                            MI_DEPENDENTIDHIS += 1;
                        }
                    }
                    #endregion

                    MEDINS_EMPLOYEE_DTL MIED = new MEDINS_EMPLOYEE_DTL();
                    
                    if (MIVM.RequestType == 2 || MIVM.RequestType == 3)
                    {
                        MIED = _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode).FirstOrDefault();
                        if (MIED == null)
                        {
                            MIED = new MEDINS_EMPLOYEE_DTL();
                        }
                    }

                    if (MIVM.RequestType == 1)
                    {
                        // Emp Detail
                        MIED.EMP_CODE = MIVM.EmpCode;
                        MIED.EMP_NAME = MIVM.Name;
                        MIED.GENDER = Convert.ToInt16(MIVM.Gender == "Male" ? 1 : 2);
                        MIED.DOB = DateTime.ParseExact(MIVM.DOB, "dd-MMM-yyyy", null);
                        MIED.EMP_STATUS = 2; // WIP 

                        MIED.NOMINEE_NAME = MIVM.NomineeDetail.NomineeName;
                        MIED.NOMINEE_RELATION = MIVM.NomineeDetail.NomineeRelation;
                        MIED.NOMINEE_GENDER = Convert.ToInt16(MIVM.NomineeDetail.NomineeGender == "Male" ? 1 : 2);
                        MIED.NOMINEE_DOB = DateTime.ParseExact(MIVM.NomineeDetail.NomineeDOB, "dd-MMM-yyyy", null);
                        MIED.NOMINEE_STATUS = 2; // WIP 
                        MIED.CREATED_DATE = DateTime.Now;
                        MIED.CREATED_BY = MIVM.CreatedBy??0;
                    }

                    if (MIVM.RequestType == 3 && (MIVM.Name != MIED.EMP_NAME || MIVM.DOB != MIED.DOB.ToString("dd-MMM-yyyy")))
                    {
                        // Pre-Self detail
                        MIED.PREVIOUS_EMP_NAME = MIED.EMP_NAME;
                        MIED.PREVIOUS_EMPDOB = MIED.DOB;
                        // self detail
                        MIED.EMP_NAME = MIVM.Name;
                        MIED.DOB = DateTime.ParseExact(MIVM.DOB, "dd-MMM-yyyy", null);
                        MIED.EMP_STATUS = 2; // WIP 
                        MIED.EMP_MODIFIEDDATE = DateTime.Now;
                        MIED.MODIFIEDBY = MIVM.CreatedBy;
                    }

                    if (MIVM.RequestType == 3 && (MIED.NOMINEE_NAME != MIVM.NomineeDetail.NomineeName || MIED.NOMINEE_DOB.ToString("dd-MMM-yyyy") != MIVM.NomineeDetail.NomineeDOB || MIED.NOMINEE_GENDER != Convert.ToInt16(MIVM.NomineeDetail.NomineeGender == "Male" ? 1 : 2) || MIED.NOMINEE_RELATION != MIVM.NomineeDetail.NomineeRelation))
                    {
                        // Pre-nominee detail
                        MIED.PREVIOUS_NOMINEE_NAME = MIED.NOMINEE_NAME;
                        MIED.PREVIOUS_NOMINEE_DOB = MIED.NOMINEE_DOB;
                        MIED.PREVIOUS_NOMINEE_GENDER = MIED.NOMINEE_GENDER;
                        MIED.PREVIOUS_NOMINEE_RELATION = MIED.NOMINEE_RELATION;
                        // Nominee Detail
                        MIED.NOMINEE_NAME = MIVM.NomineeDetail.NomineeName;
                        MIED.NOMINEE_RELATION = MIVM.NomineeDetail.NomineeRelation;
                        MIED.NOMINEE_GENDER = Convert.ToInt16(MIVM.NomineeDetail.NomineeGender == "Male" ? 1 : 2);
                        MIED.NOMINEE_DOB = DateTime.ParseExact(MIVM.NomineeDetail.NomineeDOB, "dd-MMM-yyyy", null);
                        MIED.NOMINEE_STATUS = 2; // WIP 
                        MIED.NOMINEE_MODIFIEDDATE = DateTime.Now;
                        MIED.MODIFIEDBY = MIVM.CreatedBy;
                    }
                    //_MedicalDBContext.Entry(MIED).State = MIED.MI_EMPDETAILID == 0 ? EntityState.Added : EntityState.Modified;
                   
                    //Added for Auto increment
                    if (MIED.MI_EMPDETAILID == 0)
                    {
                        MIED.MI_EMPDETAILID = _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Max(x => x.MI_EMPDETAILID) + 1;
                        _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Add(MIED);
                    }
                    else
                    {
                        _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Update(MIED);
                    }
                    _MedicalDBContext.SaveChanges();

                    SaveDependentDetail(MIVM);
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

        public MEDINS_DEPENDENTS_DTL SaveDependentDetail(MedicalInsuranceViewModel MIVM)
        {
            MEDINS_DEPENDENTS_DTL MIDD = new MEDINS_DEPENDENTS_DTL();
            List<MEDINS_DEPENDENTS_DTL> MIDDList = new List<MEDINS_DEPENDENTS_DTL>();
            if (MIVM.RequestType == 2 || MIVM.RequestType == 3)
            {
                MIDDList = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode).ToList();
            }
            if (MIVM.DeleteDependentList != null && MIVM.DeleteDependentList.Count() > 0)
            {
                foreach (var oDel in MIVM.DeleteDependentList)
                {
                    var oItem = MIDDList.Where(m => m.MI_DEPENDENTID == oDel.DepItemId).FirstOrDefault();
                    if (oItem != null)
                    {
                        oItem.STATUS = 2;
                        oItem.CHANGETYPE = 3;
                        oItem.MODIFIED_BY = MIVM.CreatedBy;
                        oItem.REMARKS = oDel.DeleteDepRemarks;
                        if (MIVM.RequestType == 2)
                        {
                            var proofString = oDel.Doc_fileString;
                            proofString = proofString.Replace("data:image/jpeg;base64,", String.Empty);
                            proofString = proofString.Replace("data:image/jpg;base64,", String.Empty);
                            proofString = proofString.Replace("data:image/png;base64,", String.Empty);
                            proofString = proofString.Replace("data:application/pdf;base64,", String.Empty);
                            oItem.DEPDEL_PROOF = Convert.FromBase64String(proofString);
                            oItem.DEPDEL_PROOF_CONTYPE = oDel.Doc_ContentType;
                        }
                        ////oItem.POLICYTYPE = 3; // 1 Child Birth, 2 Marriage, 3 Death Case
                        oItem.MODIFIED_DATE = DateTime.Now;
                        //_MedicalDBContext.Entry(oItem).State = EntityState.Modified;

                        _MedicalDBContext.Update(oItem);

                        _MedicalDBContext.SaveChanges();
                    }
                }
            }
            if (MIVM.Dependents != null)
            {
                foreach (var dependent in MIVM.Dependents)
                {
                    if (MIDDList.Count() > 0)
                    {
                        MIDD = MIDDList.Where(m => m.MI_DEPENDENTID == dependent.DepItemId).FirstOrDefault();
                        if (MIDD == null)
                        {
                            MIDD = new MEDINS_DEPENDENTS_DTL();
                        }
                    }
                    else
                    {
                        MIDD = new MEDINS_DEPENDENTS_DTL();
                    }
                    if (MIDD.MI_DEPENDENTID == 0)
                    {
                        MIDD.EMP_CODE = MIVM.EmpCode;
                        MIDD.DEP_NAME = dependent.DepName;
                        MIDD.DEP_GENDER = Convert.ToInt16(dependent.DepGender == "Male" ? 1 : 2);
                        MIDD.DEP_DOB = DateTime.ParseExact(dependent.DepDOB, "dd-MMM-yyyy", null);
                        MIDD.DEP_RELATIONSHIP = dependent.DepRelation;
                        MIDD.STATUS = 2; // WIP 
                        MIDD.DEP_PAIDTYPE = Convert.ToInt16(dependent.DepPolicyType == "Associate Paid" ? 1 : 2);
                        MIDD.CHANGETYPE = dependent.DepChangeType; // 1-Addition, 2-Modification, 3-Delection
                        MIDD.CREATED_DATE = DateTime.Now;
                        MIDD.CREATED_BY = MIVM.CreatedBy;
                        if (MIVM.RequestType == 2)
                        {
                            int policyType = dependent.DepRequestType == "ChildBirth" ? 1 : dependent.DepRequestType == "Marriage" ? 2 : 3; // Child Birth, Marriage, Death Case
                            MIDD.POLICYTYPE = Convert.ToInt16(policyType); // 1 Child Birth, 2 Marriage, 3 Death Case
                        }
                    }
                    else
                    {
                        // Dependent Detail
                        if ((MIVM.RequestType == 3) && (MIDD.DEP_NAME.Trim() != dependent.DepName.Trim() || MIDD.DEP_GENDER != Convert.ToInt16(dependent.DepGender == "Male" ? 1 : 2) || MIDD.DEP_DOB.Value.Date != DateTime.ParseExact(dependent.DepDOB, "dd-MMM-yyyy", null) || MIDD.DEP_RELATIONSHIP != dependent.DepRelation || MIDD.CHANGETYPE != dependent.DepChangeType))
                        {
                            // preDep- details
                            MIDD.PREVIOUS_DEP_NAME = MIDD.DEP_NAME;
                            MIDD.PREVIOUS_DEP_GENDER = MIDD.DEP_GENDER;
                            MIDD.PREVIOUS_DEP_DOB = MIDD.DEP_DOB;
                            MIDD.PREVIOUS_DEP_RELATIONSHIP = MIDD.DEP_RELATIONSHIP;
                            // Dep- details
                            MIDD.EMP_CODE = MIVM.EmpCode;
                            MIDD.DEP_NAME = dependent.DepName;
                            MIDD.DEP_GENDER = Convert.ToInt16(dependent.DepGender == "Male" ? 1 : 2);
                            MIDD.DEP_DOB = DateTime.ParseExact(dependent.DepDOB, "dd-MMM-yyyy", null);
                            MIDD.DEP_RELATIONSHIP = dependent.DepRelation;
                            MIDD.STATUS = 2; // WIP 
                            MIDD.DEP_PAIDTYPE = Convert.ToInt16(dependent.DepPolicyType == "Associate Paid" ? 1 : 2);
                            MIDD.CHANGETYPE = dependent.DepChangeType; // 1-Addition, 2-Modification, 3-Delection
                            MIDD.MODIFIED_DATE = DateTime.Now;
                            MIDD.MODIFIED_BY = MIVM.CreatedBy;
                        }
                    }
                    //_MedicalDBContext.Entry(MIDD).State = MIDD.MI_DEPENDENTID == 0 ? EntityState.Added : EntityState.Modified;

                  var rs=  MIDD.MI_DEPENDENTID == 0 ? _MedicalDBContext.Add(MIDD) : _MedicalDBContext.Update(MIDD);

                    _MedicalDBContext.SaveChanges();
                }
            }

            return MIDD;
        }

        public MEDINS_APPHISTORY SaveApprovalHistory(MedicalInsuranceViewModel MIVM)
        {
            var approvalid = _MedicalDBContext.MEDINS_APPHISTORY.Max(x => x.APPROVALID) + 1;

            MEDINS_APPHISTORY MAP = new MEDINS_APPHISTORY();
            MAP.APPROVALID = approvalid;
            MAP.EMP_CODE = MIVM.EmpCode;
            MAP.APPROVAL_STATUS = 0; // Pending
            MAP.REQUESTTYPE = MIVM.RequestType;
            MAP.CREATEDBY = MIVM.CreatedBy ?? 0;
            MAP.CREATEDDATE = DateTime.Now;

            _MedicalDBContext.MEDINS_APPHISTORY.Add(MAP);
            _MedicalDBContext.SaveChanges();
            return MAP;


        }



        public MedicalInsuranceViewModel GetPolicyDetails(Int64 id)
        {
            MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
            List<DependentDetailViewModel> DepList = new List<DependentDetailViewModel>();
            var approvaldata = _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.EMP_CODE == id && x.APPROVAL_STATUS == 0).FirstOrDefault();
            var SelfDetail = (from data in _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == id)
                              select new
                              {
                                  data.MI_EMPDETAILID,
                                  data.EMP_CODE,
                                  data.EMP_NAME,
                                  data.GENDER,
                                  data.DOB,
                                  data.EMP_STATUS,
                                  data.NOMINEE_NAME,
                                  data.NOMINEE_RELATION,
                                  data.NOMINEE_GENDER,
                                  data.NOMINEE_DOB,
                                  data.NOMINEE_STATUS,
                                  data.DOM,
                                  data.PREVIOUS_EMPDOB,
                                  data.PREVIOUS_EMP_NAME,
                                  data.PREVIOUS_NOMINEE_DOB,
                                  data.PREVIOUS_NOMINEE_GENDER,
                                  data.PREVIOUS_NOMINEE_NAME,
                                  data.PREVIOUS_NOMINEE_RELATION
                              }
                    ).SingleOrDefault();
            var DepDetails = (from data in _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == id && x.STATUS == 2)
                              select new
                              {
                                  data.MI_DEPENDENTID,
                                  data.EMP_CODE,
                                  data.DEP_NAME,
                                  data.DEP_GENDER,
                                  data.DEP_DOB,
                                  data.DEP_PAIDTYPE,
                                  data.DEP_RELATIONSHIP,
                                  data.DEPDEL_PROOF,
                                  data.DEPDEL_PROOF_CONTYPE,
                                  data.STATUS,
                                  data.PREVIOUS_DEP_DOB,
                                  data.PREVIOUS_DEP_GENDER,
                                  data.PREVIOUS_DEP_NAME,
                                  data.PREVIOUS_DEP_RELATIONSHIP,
                                  data.CHANGETYPE
                              }
                     ).ToList();
            if (SelfDetail != null && DepDetails != null)
            {
                if (approvaldata != null)
                {
                    MIVM.Approval_Status = approvaldata.APPROVAL_STATUS;
                    MIVM.RequestType = approvaldata.REQUESTTYPE;
                }
                MIVM.MedicalInsuranceID = SelfDetail.MI_EMPDETAILID;
                MIVM.EmpCode = SelfDetail.EMP_CODE;
                MIVM.Name = SelfDetail.EMP_NAME;
                MIVM.Gender = SelfDetail.GENDER == 1 ? "Male" : "Female";
                MIVM.DOB = SelfDetail.DOB.ToString("dd-MMM-yyyy");
                MIVM.Emp_Status = SelfDetail.EMP_STATUS;
                MIVM.PREVIOUS_EMPDOB = (SelfDetail.PREVIOUS_EMPDOB != null ? SelfDetail.PREVIOUS_EMPDOB.Value.ToString("dd-MMM-yyyy") : "");
                MIVM.PREVIOUS_EMP_NAME = SelfDetail.PREVIOUS_EMP_NAME;
                MIVM.NomineeDetail = new NomineeDetailViewModel
                {
                    NomineeName = SelfDetail.NOMINEE_NAME == null ? "" : SelfDetail.NOMINEE_NAME.Trim(),
                    NomineeRelation = SelfDetail.NOMINEE_RELATION == null ? "" : SelfDetail.NOMINEE_RELATION.Trim(),
                    NomineeGender = SelfDetail.NOMINEE_GENDER == 1 ? "Male" : "Female",
                    NomineeDOB = SelfDetail.NOMINEE_DOB == null ? "" : SelfDetail.NOMINEE_DOB.ToString("dd-MMM-yyyy").Trim(),
                    Nominee_Status = SelfDetail.NOMINEE_STATUS,
                    PREVIOUS_NOMINEE_DOB = SelfDetail.PREVIOUS_NOMINEE_DOB == null ? "" : SelfDetail.PREVIOUS_NOMINEE_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                    PREVIOUS_NOMINEE_GENDER = SelfDetail.PREVIOUS_NOMINEE_GENDER != null ? (SelfDetail.PREVIOUS_NOMINEE_GENDER == 1 ? "Male" : "Female") : "",
                    PREVIOUS_NOMINEE_RELATION = SelfDetail.PREVIOUS_NOMINEE_RELATION == null ? "" : SelfDetail.PREVIOUS_NOMINEE_RELATION.Trim(),
                    PREVIOUS_NOMINEE_NAME = SelfDetail.PREVIOUS_NOMINEE_NAME == null ? "" : SelfDetail.PREVIOUS_NOMINEE_NAME.Trim(),
                };
                int Sno = 1;
                foreach (var item in DepDetails)
                {
                    DepList.Add(new DependentDetailViewModel
                    {
                        Sno = Sno++,
                        DepItemId = item.MI_DEPENDENTID,
                        DepName = item.DEP_NAME == null ? "" : item.DEP_NAME.Trim(),
                        DepDOB = item.DEP_DOB == null ? "" : item.DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                        DepGender = item.DEP_GENDER == 1 ? "Male" : "Female",
                        DepRelation = item.DEP_RELATIONSHIP == null ? "" : item.DEP_RELATIONSHIP.Trim(),
                        DepPolicyType = item.DEP_PAIDTYPE == 1 ? "Associate Paid" : "Company Paid",
                        Doc_fileString = item.DEPDEL_PROOF == null ? "" : Convert.ToBase64String(item.DEPDEL_PROOF),
                        Doc_ContentType = String.IsNullOrEmpty(item.DEPDEL_PROOF_CONTYPE) ? "" : item.DEPDEL_PROOF_CONTYPE,
                        PREVIOUS_DEP_DOB = item.PREVIOUS_DEP_DOB == null ? "" : item.PREVIOUS_DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                        PREVIOUS_DEP_GENDER = item.PREVIOUS_DEP_GENDER != null ? (item.PREVIOUS_DEP_GENDER == 1 ? "Male" : "Female") : "",
                        PREVIOUS_DEP_NAME = item.PREVIOUS_DEP_NAME == null ? "" : item.PREVIOUS_DEP_NAME.Trim(),
                        PREVIOUS_DEP_RELATION = item.PREVIOUS_DEP_RELATIONSHIP == null ? "" : item.PREVIOUS_DEP_RELATIONSHIP.Trim(),
                        DepChangeType = item.CHANGETYPE.Value,
                        Status = item.STATUS
                    });
                }
                MIVM.Dependents = DepList;
            }
            return MIVM;
        }

        public MedicalInsuranceViewModel GetActivePolicyDetails(Int64 id)
        {
            MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
            List<DependentDetailViewModel> DepList = new List<DependentDetailViewModel>();
            var SelfDetail = (from data in _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == id)
                              select new
                              {
                                  data.MI_EMPDETAILID,
                                  data.EMP_CODE,
                                  data.EMP_NAME,
                                  data.GENDER,
                                  data.DOB,
                                  data.EMP_STATUS,
                                  data.NOMINEE_NAME,
                                  data.NOMINEE_RELATION,
                                  data.NOMINEE_GENDER,
                                  data.NOMINEE_DOB,
                                  data.NOMINEE_STATUS,
                                  data.DOM,
                                  data.PREVIOUS_EMPDOB,
                                  data.PREVIOUS_EMP_NAME,
                                  data.PREVIOUS_NOMINEE_DOB,
                                  data.PREVIOUS_NOMINEE_GENDER,
                                  data.PREVIOUS_NOMINEE_NAME,
                                  data.PREVIOUS_NOMINEE_RELATION
                              }
                    ).SingleOrDefault();
            var DepDetails = (from data in _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == id && x.STATUS == 1 && x.CHANGETYPE != 3)
                              select new
                              {
                                  data.MI_DEPENDENTID,
                                  data.EMP_CODE,
                                  data.DEP_NAME,
                                  data.DEP_GENDER,
                                  data.DEP_DOB,
                                  data.DEP_PAIDTYPE,
                                  data.DEP_RELATIONSHIP,
                                  data.STATUS,
                                  data.PREVIOUS_DEP_DOB,
                                  data.PREVIOUS_DEP_GENDER,
                                  data.PREVIOUS_DEP_NAME,
                                  data.PREVIOUS_DEP_RELATIONSHIP,
                                  data.CHANGETYPE
                              }
                     ).ToList();
            if (SelfDetail != null && DepDetails != null)
            {
                MIVM.MedicalInsuranceID = SelfDetail.MI_EMPDETAILID;
                MIVM.EmpCode = SelfDetail.EMP_CODE;
                MIVM.Name = SelfDetail.EMP_NAME;
                MIVM.Gender = SelfDetail.GENDER == 1 ? "Male" : "Female";
                MIVM.DOB = SelfDetail.DOB.ToString("dd-MMM-yyyy");
                MIVM.Emp_Status = SelfDetail.EMP_STATUS;
                MIVM.PREVIOUS_EMPDOB = (SelfDetail.PREVIOUS_EMPDOB != null ? SelfDetail.PREVIOUS_EMPDOB.Value.ToString("dd-MMM-yyyy") : "");
                MIVM.PREVIOUS_EMP_NAME = SelfDetail.PREVIOUS_EMP_NAME;
                MIVM.NomineeDetail = new NomineeDetailViewModel
                {
                    NomineeName = SelfDetail.NOMINEE_NAME == null ? "" : SelfDetail.NOMINEE_NAME.Trim(),
                    NomineeRelation = SelfDetail.NOMINEE_RELATION == null ? "" : SelfDetail.NOMINEE_RELATION.Trim(),
                    NomineeGender = SelfDetail.NOMINEE_GENDER == 1 ? "Male" : "Female",
                    NomineeDOB = SelfDetail.NOMINEE_DOB == null ? "" : SelfDetail.NOMINEE_DOB.ToString("dd-MMM-yyyy").Trim(),
                    Nominee_Status = SelfDetail.NOMINEE_STATUS,
                    PREVIOUS_NOMINEE_DOB = SelfDetail.PREVIOUS_NOMINEE_DOB == null ? "" : SelfDetail.PREVIOUS_NOMINEE_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                    PREVIOUS_NOMINEE_GENDER = SelfDetail.PREVIOUS_NOMINEE_GENDER != null ? (SelfDetail.PREVIOUS_NOMINEE_GENDER == 1 ? "Male" : "Female") : "",
                    PREVIOUS_NOMINEE_RELATION = SelfDetail.PREVIOUS_NOMINEE_RELATION == null ? "" : SelfDetail.PREVIOUS_NOMINEE_RELATION.Trim(),
                    PREVIOUS_NOMINEE_NAME = SelfDetail.PREVIOUS_NOMINEE_NAME == null ? "" : SelfDetail.PREVIOUS_NOMINEE_NAME.Trim()
                };
                int Sno = 1;
                foreach (var item in DepDetails)
                {
                    DepList.Add(new DependentDetailViewModel
                    {
                        Sno = Sno++,
                        DepItemId = item.MI_DEPENDENTID,
                        DepName = item.DEP_NAME == null ? "" : item.DEP_NAME.Trim(),
                        DepDOB = item.DEP_DOB == null ? "" : item.DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                        DepGender = item.DEP_GENDER == 1 ? "Male" : "Female",
                        DepRelation = item.DEP_RELATIONSHIP == null ? "" : item.DEP_RELATIONSHIP.Trim(),
                        DepPolicyType = item.DEP_PAIDTYPE == 1 ? "Associate Paid" : "Company Paid",
                        PREVIOUS_DEP_DOB = item.PREVIOUS_DEP_DOB == null ? "" : item.PREVIOUS_DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                        PREVIOUS_DEP_GENDER = item.PREVIOUS_DEP_GENDER != null ? (item.PREVIOUS_DEP_GENDER == 1 ? "Male" : "Female") : "",
                        PREVIOUS_DEP_NAME = item.PREVIOUS_DEP_NAME == null ? "" : item.PREVIOUS_DEP_NAME.Trim(),
                        PREVIOUS_DEP_RELATION = item.PREVIOUS_DEP_RELATIONSHIP == null ? "" : item.PREVIOUS_DEP_RELATIONSHIP.Trim(),
                        DepChangeType = item.CHANGETYPE.Value,
                        Status = item.STATUS

                    });
                }
                MIVM.Dependents = DepList;
            }
            return MIVM;
        }

        #endregion

        #region Health Center
        public List<HealthCenterViewModel> HealthCenterApprovalList(HealthCenterApprovalViewModel PageMdl)
        {
            decimal syki = _MedicalDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault().SYKIID;
            DateTime Approvalfromdate = DateTime.Now.Date, Approvaltodate = DateTime.Now.Date, ReqDateFrom = DateTime.Now.Date, ReqDateTo = DateTime.Now.Date;
            long Approvalfromdateflag = 0, Approvaltodateflag = 0, ReqDateFromflag = 0, ReqDateToflag = 0;

            if (!string.IsNullOrEmpty(PageMdl.ApprovalDateFrom))
            {
                Approvalfromdate = DateTime.ParseExact(PageMdl.ApprovalDateFrom, "dd-MMM-yyyy", null);
                Approvalfromdateflag = 1;
            }
            if (!string.IsNullOrEmpty(PageMdl.ApprovalDateTo))
            {
                Approvaltodate = DateTime.ParseExact(PageMdl.ApprovalDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                Approvaltodateflag = 1;
            }
            if (!string.IsNullOrEmpty(PageMdl.ReqDateFrom))
            {
                ReqDateFrom = DateTime.ParseExact(PageMdl.ReqDateFrom, "dd-MMM-yyyy", null);
                ReqDateFromflag = 1;
            }
            if (!string.IsNullOrEmpty(PageMdl.ReqDateTo))
            {
                ReqDateTo = DateTime.ParseExact(PageMdl.ReqDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                ReqDateToflag = 1;
            }

            List<HealthCenterViewModel> iList = new List<HealthCenterViewModel>();
            var iColl = (from data in _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.APPROVAL_STATUS == PageMdl.ApprovalStatus)
                         join loc in _MedicalDBContext.MEDINS_EMPPOLICYLOC on data.EMP_CODE equals loc.ADEMPCODE
                         join ADEMP in _MedicalDBContext.ADEMPLOYEE on data.EMP_CODE equals ADEMP.ADEMPCODE
                         join dds in _MedicalDBContext.VW_ASSOCIATELVLDETAILS on data.EMP_CODE equals dds.ADEMPCODE
                         join desg in _MedicalDBContext.ADDESIGNATION on dds.ADDESIGNATIONID equals desg.ADDESIGNATIONID
                         where dds.SYKI == syki &&
                         (Approvalfromdateflag == 0 ? true : data.APPROVAL_DATE >= Approvalfromdate) &&
                         (Approvaltodateflag == 0 ? true : data.APPROVAL_DATE <= Approvaltodate) &&
                         (ReqDateFromflag == 0 ? true : data.CREATEDDATE >= ReqDateFrom) &&
                         (ReqDateToflag == 0 ? true : data.CREATEDDATE <= ReqDateTo) &&
                         (PageMdl.PlantID == 0 ? true : loc.PLANTID == PageMdl.PlantID) &&
                         loc.STATUS == 1
                         select new
                         {
                             data.APPROVALID,
                             data.EMP_CODE,
                             ADEMP.FIRSTNAME,
                             ADEMP.LASTNAME,
                             data.MODIFIED_DATE,
                             data.APPROVAL_STATUS,
                             dds.OPERATION,
                             dds.DIVISION,
                             dds.DEPARTMENT,
                             dds.SECTION,
                             desg.DESCRIP,
                             data.REQUESTTYPE,
                             data.CREATEDDATE,
                             data.APPROVAL_DATE
                         }).ToList();
            if (iColl.Count() > 0)
            {
                if (PageMdl.ApprovalStatus == 0)
                {
                    foreach (var obj in iColl)
                    {
                        iList.Add(new HealthCenterViewModel
                        {
                            ID = obj.APPROVALID,
                            EmpCode = obj.EMP_CODE,
                            EmpName = obj.FIRSTNAME + " " + obj.LASTNAME,
                            Operation = obj.OPERATION,
                            Department = obj.DEPARTMENT,
                            Division = obj.DIVISION,
                            Section = obj.SECTION,
                            Location = "",
                            Desg = obj.DESCRIP,
                            Status = obj.APPROVAL_STATUS,
                            RequestType = obj.REQUESTTYPE,
                            ApprovalDate = obj.APPROVAL_DATE != null ? obj.APPROVAL_DATE.Value.ToString("dd-MMM-yyyy") : "",
                            CreatedDate = obj.CREATEDDATE?.ToString("dd-MMM-yyyy")

                        });
                    }
                }
                else
                {
                    var approvalids = (from lst in iColl.Where(m => m.APPROVAL_STATUS == 1)
                                       group lst by new { lst.EMP_CODE } into g
                                       select new
                                       {
                                           EMP_CODE = g.Key.EMP_CODE,
                                           APPROVALID = g.Max(m => m.APPROVALID)
                                       });
                    iColl = (from oList in iColl
                             join app in approvalids on oList.APPROVALID equals app.APPROVALID
                             select oList
                            ).ToList();

                    foreach (var obj in iColl)
                    {
                        iList.Add(new HealthCenterViewModel
                        {
                            ID = obj.APPROVALID,
                            EmpCode = obj.EMP_CODE,
                            EmpName = obj.FIRSTNAME + " " + obj.LASTNAME,
                            Operation = obj.OPERATION,
                            Department = obj.DEPARTMENT,
                            Division = obj.DIVISION,
                            Section = obj.SECTION,
                            Location = "",
                            Desg = obj.DESCRIP,
                            Status = obj.APPROVAL_STATUS,
                            RequestType = obj.REQUESTTYPE,
                            ApprovalDate = obj.APPROVAL_DATE != null ? obj.APPROVAL_DATE.Value.ToString("dd-MMM-yyyy") : "",
                            CreatedDate = obj.CREATEDDATE?.ToString("dd-MMM-yyyy")

                        });
                    }
                }
            }
            return iList.OrderByDescending(m => m.CreatedDate).ToList();
        }
        public short RequestApproval(long APPROVALID, short AppStatus, string Remark, long UserID)
        {
            using (IDbContextTransaction transaction = _MedicalDBContext.Database.BeginTransaction())
            {
                try
                {
                    var Approvaldata = _MedicalDBContext.MEDINS_APPHISTORY.Where(m => m.EMP_CODE == APPROVALID && m.APPROVAL_STATUS == 0).FirstOrDefault();
                    var HeaderData = _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(m => m.EMP_CODE == APPROVALID).FirstOrDefault();
                    var DependentData = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(m => m.EMP_CODE == APPROVALID && m.STATUS == 2);
                    #region "Set Approval Status"
                    //int status = AppStatus;(AppStatus == 1 ? 1 : 2); // 1 for Approved, 2 for WIP
                    if (Approvaldata != null)
                    {
                        Approvaldata.APPROVAL_STATUS = AppStatus;
                        Approvaldata.APPROVAL_REMARK = Remark;
                        Approvaldata.APPROVAL_BY = UserID;
                        Approvaldata.APPROVAL_DATE = DateTime.Now;
                        Approvaldata.MODIFIED_BY = UserID;
                        Approvaldata.MODIFIED_DATE = DateTime.Now;
                        _MedicalDBContext.Entry(Approvaldata).State = EntityState.Modified;
                        _MedicalDBContext.SaveChanges();
                    }
                    #endregion
                    if (Approvaldata.REQUESTTYPE == 1 && (AppStatus == 3 || AppStatus == 1))
                    {
                        #region "Header Data Status Updation"

                        if (HeaderData != null)
                        {
                            if (HeaderData.NOMINEE_STATUS == 2)
                            {
                                HeaderData.NOMINEE_STATUS = 1;
                                HeaderData.MODIFIEDBY = UserID;
                                HeaderData.NOMINEE_MODIFIEDDATE = DateTime.Now;
                            }
                            if (HeaderData.EMP_STATUS == 2)
                            {
                                HeaderData.EMP_STATUS = 1;
                                HeaderData.MODIFIEDBY = UserID;
                                HeaderData.EMP_MODIFIEDDATE = DateTime.Now;
                            }
                            _MedicalDBContext.Entry(HeaderData).State = EntityState.Modified;
                            _MedicalDBContext.SaveChanges();
                        }
                        #endregion
                        #region "Dependent detail Status Updation"
                        foreach (var obj in DependentData)
                        {
                            obj.STATUS = 1;
                            obj.MODIFIED_BY = UserID;
                            obj.MODIFIED_DATE = DateTime.Now;
                            _MedicalDBContext.Entry(obj).State = EntityState.Modified;
                            _MedicalDBContext.SaveChanges();

                        }
                        #endregion
                    }


                    if (Approvaldata.REQUESTTYPE != 1)
                    {
                        if (AppStatus == 1)
                        {
                            #region "Header Data Status Updation"

                            if (HeaderData != null)
                            {
                                if (HeaderData.NOMINEE_STATUS == 2)
                                {
                                    HeaderData.NOMINEE_STATUS = 1;
                                    HeaderData.MODIFIEDBY = UserID;
                                    HeaderData.NOMINEE_MODIFIEDDATE = DateTime.Now;
                                }
                                if (HeaderData.EMP_STATUS == 2)
                                {
                                    HeaderData.EMP_STATUS = 1;
                                    HeaderData.MODIFIEDBY = UserID;
                                    HeaderData.EMP_MODIFIEDDATE = DateTime.Now;
                                }
                                _MedicalDBContext.Entry(HeaderData).State = EntityState.Modified;
                                _MedicalDBContext.SaveChanges();
                            }
                            #endregion
                            #region "Dependent detail Status Updation"
                            foreach (var obj in DependentData)
                            {
                                if (obj.CHANGETYPE == 3 && AppStatus == 1)
                                {
                                    obj.STATUS = 0;
                                }
                                else
                                {
                                    obj.STATUS = Convert.ToInt16(AppStatus);
                                }

                                obj.MODIFIED_BY = UserID;
                                obj.MODIFIED_DATE = DateTime.Now;
                                _MedicalDBContext.Entry(obj).State = EntityState.Modified;
                                _MedicalDBContext.SaveChanges();

                            }
                            #endregion
                        }
                        if (AppStatus == 4)
                        {
                            #region "Reject Case"
                            MEDINS_EMPLOYEE_DTL_HIS Mainemp = new MEDINS_EMPLOYEE_DTL_HIS();
                            List<MEDINS_DEPENDENTS_DTL_HIS> Maindep = new List<MEDINS_DEPENDENTS_DTL_HIS>();
                            MEDINS_EMPLOYEE_DTL hisemp = new MEDINS_EMPLOYEE_DTL();
                            List<MEDINS_DEPENDENTS_DTL> hisdep = new List<MEDINS_DEPENDENTS_DTL>();
                            hisemp = _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == Approvaldata.EMP_CODE).FirstOrDefault();
                            hisdep = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == Approvaldata.EMP_CODE).ToList();
                            Mainemp = _MedicalDBContext.MEDINS_EMPLOYEE_DTL_HIS.Where(x => x.APPROVALID == Approvaldata.APPROVALID).FirstOrDefault();
                            Maindep = _MedicalDBContext.MEDINS_DEPENDENTS_DTL_HIS.Where(x => x.APPROVALID == Approvaldata.APPROVALID).ToList();

                            #region "deletion of existing data"
                            _MedicalDBContext.Entry(hisemp).State = EntityState.Deleted;
                            _MedicalDBContext.SaveChanges();
                            foreach (var obj in hisdep)
                            {
                                _MedicalDBContext.Entry(obj).State = EntityState.Deleted;
                                _MedicalDBContext.SaveChanges();
                            }
                            _MedicalDBContext.Entry(Mainemp).State = EntityState.Deleted;
                            _MedicalDBContext.SaveChanges();
                            foreach (var obj in Maindep)
                            {
                                _MedicalDBContext.Entry(obj).State = EntityState.Deleted;
                                _MedicalDBContext.SaveChanges();
                            }

                            #endregion

                            //Move current data into history and copy history data to main table incase of rejection
                            MEDINS_EMPLOYEE_DTL_HIS objemphis = new MEDINS_EMPLOYEE_DTL_HIS();
                            objemphis.APPROVALID = Approvaldata.APPROVALID;
                            objemphis.CREATED_BY = hisemp.CREATED_BY;
                            objemphis.CREATED_DATE = hisemp.CREATED_DATE;
                            objemphis.DOB = hisemp.DOB;
                            objemphis.DOM = hisemp.DOM;
                            objemphis.EMP_CODE = hisemp.EMP_CODE;
                            objemphis.EMP_MODIFIEDDATE = hisemp.EMP_MODIFIEDDATE;
                            objemphis.EMP_NAME = hisemp.EMP_NAME;
                            objemphis.EMP_STATUS = hisemp.EMP_STATUS;
                            objemphis.GENDER = hisemp.GENDER;
                            objemphis.MODIFIEDBY = hisemp.MODIFIEDBY;
                            objemphis.NOMINEE_DOB = hisemp.NOMINEE_DOB;
                            objemphis.NOMINEE_GENDER = hisemp.NOMINEE_GENDER;
                            objemphis.NOMINEE_MODIFIEDDATE = hisemp.NOMINEE_MODIFIEDDATE;
                            objemphis.NOMINEE_NAME = hisemp.NOMINEE_NAME;
                            objemphis.NOMINEE_RELATION = hisemp.NOMINEE_RELATION;
                            objemphis.NOMINEE_STATUS = hisemp.NOMINEE_STATUS;
                            objemphis.PREVIOUS_EMPDOB = hisemp.PREVIOUS_EMPDOB;
                            objemphis.PREVIOUS_EMP_NAME = hisemp.PREVIOUS_EMP_NAME;
                            objemphis.PREVIOUS_NOMINEE_DOB = hisemp.PREVIOUS_NOMINEE_DOB;
                            objemphis.PREVIOUS_NOMINEE_GENDER = hisemp.PREVIOUS_NOMINEE_GENDER;
                            objemphis.PREVIOUS_NOMINEE_NAME = hisemp.PREVIOUS_NOMINEE_NAME;
                            objemphis.PREVIOUS_NOMINEE_RELATION = hisemp.PREVIOUS_NOMINEE_RELATION;
                            _MedicalDBContext.Entry(objemphis).State = EntityState.Added;
                            _MedicalDBContext.SaveChanges();

                            foreach (MEDINS_DEPENDENTS_DTL obj in hisdep)
                            {
                                MEDINS_DEPENDENTS_DTL_HIS objMedDep = new MEDINS_DEPENDENTS_DTL_HIS();
                                objMedDep.APPROVALID = Approvaldata.APPROVALID;
                                objMedDep.CHANGETYPE = obj.CHANGETYPE;
                                objMedDep.CREATED_BY = obj.CREATED_BY;
                                objMedDep.CREATED_DATE = obj.CREATED_DATE;
                                objMedDep.DEP_DOB = obj.DEP_DOB;
                                objMedDep.DEP_GENDER = obj.DEP_GENDER;
                                objMedDep.DEP_NAME = obj.DEP_NAME;
                                objMedDep.DEP_PAIDTYPE = obj.DEP_PAIDTYPE;
                                objMedDep.DEP_PHOTO = obj.DEPDEL_PROOF;
                                objMedDep.DEP_PHOTO_CONTYPE = obj.DEPDEL_PROOF_CONTYPE;
                                objMedDep.DEP_RELATIONSHIP = obj.DEP_RELATIONSHIP;
                                objMedDep.EMP_CODE = obj.EMP_CODE;
                                objMedDep.MODIFIED_BY = obj.MODIFIED_BY;
                                objMedDep.MODIFIED_DATE = obj.MODIFIED_DATE;
                                objMedDep.POLICYTYPE = obj.POLICYTYPE;
                                objMedDep.PREVIOUS_DEP_DOB = obj.PREVIOUS_DEP_DOB;
                                objMedDep.PREVIOUS_DEP_GENDER = obj.PREVIOUS_DEP_GENDER;
                                objMedDep.PREVIOUS_DEP_NAME = obj.PREVIOUS_DEP_NAME;
                                objMedDep.PREVIOUS_DEP_RELATIONSHIP = obj.PREVIOUS_DEP_RELATIONSHIP;
                                objMedDep.REMARKS = obj.REMARKS;
                                objMedDep.STATUS = obj.STATUS;
                                _MedicalDBContext.Entry(objMedDep).State = EntityState.Added;
                                _MedicalDBContext.SaveChanges();

                            }

                            //Roll Back history data to main table when request rejected
                            MEDINS_EMPLOYEE_DTL objemp = new MEDINS_EMPLOYEE_DTL();
                            objemp.CREATED_BY = Mainemp.CREATED_BY;
                            objemp.CREATED_DATE = Mainemp.CREATED_DATE;
                            objemp.DOB = Mainemp.DOB;
                            objemp.DOM = Mainemp.DOM;
                            objemp.EMP_CODE = Mainemp.EMP_CODE;
                            objemp.EMP_MODIFIEDDATE = Mainemp.EMP_MODIFIEDDATE;
                            objemp.EMP_NAME = Mainemp.EMP_NAME;
                            objemp.EMP_STATUS = Mainemp.EMP_STATUS;
                            objemp.GENDER = Mainemp.GENDER;
                            objemp.MODIFIEDBY = Mainemp.MODIFIEDBY;
                            objemp.NOMINEE_DOB = Mainemp.NOMINEE_DOB;
                            objemp.NOMINEE_GENDER = Mainemp.NOMINEE_GENDER;
                            objemp.NOMINEE_MODIFIEDDATE = Mainemp.NOMINEE_MODIFIEDDATE;
                            objemp.NOMINEE_NAME = Mainemp.NOMINEE_NAME;
                            objemp.NOMINEE_RELATION = Mainemp.NOMINEE_RELATION;
                            objemp.NOMINEE_STATUS = Mainemp.NOMINEE_STATUS;
                            objemp.PREVIOUS_EMPDOB = Mainemp.PREVIOUS_EMPDOB;
                            objemp.PREVIOUS_EMP_NAME = Mainemp.PREVIOUS_EMP_NAME;
                            objemp.PREVIOUS_NOMINEE_DOB = Mainemp.PREVIOUS_NOMINEE_DOB;
                            objemp.PREVIOUS_NOMINEE_GENDER = Mainemp.PREVIOUS_NOMINEE_GENDER;
                            objemp.PREVIOUS_NOMINEE_NAME = Mainemp.PREVIOUS_NOMINEE_NAME;
                            objemp.PREVIOUS_NOMINEE_RELATION = Mainemp.PREVIOUS_NOMINEE_RELATION;
                            _MedicalDBContext.Entry(objemp).State = EntityState.Added;
                            _MedicalDBContext.SaveChanges();
                            foreach (MEDINS_DEPENDENTS_DTL_HIS obj in Maindep)
                            {
                                MEDINS_DEPENDENTS_DTL objMedDep = new MEDINS_DEPENDENTS_DTL();
                                objMedDep.CHANGETYPE = obj.CHANGETYPE;
                                objMedDep.CREATED_BY = obj.CREATED_BY;
                                objMedDep.CREATED_DATE = obj.CREATED_DATE;
                                objMedDep.DEP_DOB = obj.DEP_DOB;
                                objMedDep.DEP_GENDER = obj.DEP_GENDER;
                                objMedDep.DEP_NAME = obj.DEP_NAME;
                                objMedDep.DEP_PAIDTYPE = obj.DEP_PAIDTYPE;
                                objMedDep.DEPDEL_PROOF = obj.DEP_PHOTO;
                                objMedDep.DEPDEL_PROOF_CONTYPE = obj.DEP_PHOTO_CONTYPE;
                                objMedDep.DEP_RELATIONSHIP = obj.DEP_RELATIONSHIP;
                                objMedDep.EMP_CODE = obj.EMP_CODE;
                                objMedDep.MODIFIED_BY = obj.MODIFIED_BY;
                                objMedDep.MODIFIED_DATE = obj.MODIFIED_DATE;
                                objMedDep.POLICYTYPE = obj.POLICYTYPE;
                                objMedDep.PREVIOUS_DEP_DOB = obj.PREVIOUS_DEP_DOB;
                                objMedDep.PREVIOUS_DEP_GENDER = obj.PREVIOUS_DEP_GENDER;
                                objMedDep.PREVIOUS_DEP_NAME = obj.PREVIOUS_DEP_NAME;
                                objMedDep.PREVIOUS_DEP_RELATIONSHIP = obj.PREVIOUS_DEP_RELATIONSHIP;
                                objMedDep.REMARKS = obj.REMARKS;
                                objMedDep.STATUS = obj.STATUS;
                                _MedicalDBContext.Entry(objMedDep).State = EntityState.Added;
                                _MedicalDBContext.SaveChanges();
                            }
                            #endregion
                        }
                    }
                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex) { return -1; transaction.Rollback(); }
            }
        }

        public MedicalInsuranceViewModel GetAllPolicyDetails(Int64 id)
        {
            MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
            var LastChange = (from data in _MedicalDBContext.MEDINS_APPHISTORY
                              where data.EMP_CODE == id
                              select data
                              ).OrderByDescending(m => m.APPROVALID).ToList().FirstOrDefault();
            if (LastChange != null && LastChange.APPROVAL_STATUS == 1)
            {
                MIVM.lastChangeDate = LastChange.APPROVAL_DATE == null ? "" : LastChange.APPROVAL_DATE.Value.ToString("dd-MMM-yyyy");
            }
            List<DependentDetailViewModel> DepList = new List<DependentDetailViewModel>();
            var SelfDetail = (from data in _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == id)
                              select new
                              {
                                  data.MI_EMPDETAILID,
                                  data.EMP_CODE,
                                  data.EMP_NAME,
                                  data.GENDER,
                                  data.DOB,
                                  data.EMP_STATUS,
                                  data.NOMINEE_NAME,
                                  data.NOMINEE_RELATION,
                                  data.NOMINEE_GENDER,
                                  data.NOMINEE_DOB,
                                  data.NOMINEE_STATUS,
                                  data.DOM,
                                  data.PREVIOUS_EMPDOB,
                                  data.PREVIOUS_EMP_NAME,
                                  data.PREVIOUS_NOMINEE_DOB,
                                  data.PREVIOUS_NOMINEE_GENDER,
                                  data.PREVIOUS_NOMINEE_NAME,
                                  data.PREVIOUS_NOMINEE_RELATION,
                                  data.EMP_MODIFIEDDATE,
                                  data.NOMINEE_MODIFIEDDATE
                              }
                    ).SingleOrDefault();
            var DepDetails = (from data in _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == id && (x.STATUS == 1 || x.STATUS == 2 || x.STATUS == 3)) // x.STATUS == 0 Added By Aumento on 06-08-2024 :: SR75145
                              select new
                              {
                                  data.MI_DEPENDENTID,
                                  data.EMP_CODE,
                                  data.DEP_NAME,
                                  data.DEP_GENDER,
                                  data.DEP_DOB,
                                  data.DEP_PAIDTYPE,
                                  data.DEP_RELATIONSHIP,
                                  data.STATUS,
                                  data.PREVIOUS_DEP_DOB,
                                  data.PREVIOUS_DEP_GENDER,
                                  data.PREVIOUS_DEP_NAME,
                                  data.PREVIOUS_DEP_RELATIONSHIP,
                                  data.CHANGETYPE,
                                  data.MODIFIED_DATE
                              }
                     ).ToList();
            if (SelfDetail != null && DepDetails != null)
            {
                MIVM.MedicalInsuranceID = SelfDetail.MI_EMPDETAILID;
                MIVM.EmpCode = SelfDetail.EMP_CODE;
                MIVM.Name = SelfDetail.EMP_NAME;
                MIVM.Gender = SelfDetail.GENDER == 1 ? "Male" : "Female";
                MIVM.DOB = SelfDetail.DOB.ToString("dd-MMM-yyyy");
                MIVM.PREVIOUS_EMPDOB = (SelfDetail.PREVIOUS_EMPDOB != null ? SelfDetail.PREVIOUS_EMPDOB.Value.ToString("dd-MMM-yyyy") : "");
                MIVM.PREVIOUS_EMP_NAME = SelfDetail.PREVIOUS_EMP_NAME;
                MIVM.ModifiedDate = SelfDetail.EMP_MODIFIEDDATE == null ? "" : SelfDetail.EMP_MODIFIEDDATE.Value.ToString("dd-MMM-yyyy");
                MIVM.Emp_Status = SelfDetail.EMP_STATUS;
                MIVM.NomineeDetail = new NomineeDetailViewModel
                {
                    NomineeName = SelfDetail.NOMINEE_NAME == null ? "" : SelfDetail.NOMINEE_NAME.Trim(),
                    NomineeRelation = SelfDetail.NOMINEE_RELATION == null ? "" : SelfDetail.NOMINEE_RELATION.Trim(),
                    NomineeGender = SelfDetail.NOMINEE_GENDER == 1 ? "Male" : "Female",
                    NomineeDOB = SelfDetail.NOMINEE_DOB == null ? "" : SelfDetail.NOMINEE_DOB.ToString("dd-MMM-yyyy").Trim(),
                    PREVIOUS_NOMINEE_DOB = SelfDetail.PREVIOUS_NOMINEE_DOB == null ? "" : SelfDetail.PREVIOUS_NOMINEE_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                    PREVIOUS_NOMINEE_GENDER = SelfDetail.PREVIOUS_NOMINEE_GENDER != null ? (SelfDetail.PREVIOUS_NOMINEE_GENDER == 1 ? "Male" : "Female") : "",
                    PREVIOUS_NOMINEE_RELATION = SelfDetail.PREVIOUS_NOMINEE_RELATION == null ? "" : SelfDetail.PREVIOUS_NOMINEE_RELATION.Trim(),
                    PREVIOUS_NOMINEE_NAME = SelfDetail.PREVIOUS_NOMINEE_NAME == null ? "" : SelfDetail.PREVIOUS_NOMINEE_NAME.Trim(),
                    ModifiedDate = SelfDetail.NOMINEE_MODIFIEDDATE == null ? "" : SelfDetail.NOMINEE_MODIFIEDDATE.Value.ToString("dd-MMM-yyyy"),
                    Nominee_Status = SelfDetail.NOMINEE_STATUS

                };
                int Sno = 1;
                foreach (var item in DepDetails)
                {
                    DepList.Add(new DependentDetailViewModel
                    {
                        Sno = Sno++,
                        DepItemId = item.MI_DEPENDENTID,
                        DepName = item.DEP_NAME == null ? "" : item.DEP_NAME.Trim(),
                        DepDOB = item.DEP_DOB == null ? "" : item.DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                        DepGender = item.DEP_GENDER == 1 ? "Male" : "Female",
                        DepRelation = item.DEP_RELATIONSHIP == null ? "" : item.DEP_RELATIONSHIP.Trim(),
                        DepPolicyType = item.DEP_PAIDTYPE == 1 ? "Associate Paid" : "Company Paid",
                        PREVIOUS_DEP_DOB = item.PREVIOUS_DEP_DOB == null ? "" : item.PREVIOUS_DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                        PREVIOUS_DEP_GENDER = item.PREVIOUS_DEP_GENDER != null ? (item.PREVIOUS_DEP_GENDER == 1 ? "Male" : "Female") : "",
                        PREVIOUS_DEP_NAME = item.PREVIOUS_DEP_NAME == null ? "" : item.PREVIOUS_DEP_NAME.Trim(),
                        PREVIOUS_DEP_RELATION = item.PREVIOUS_DEP_RELATIONSHIP == null ? "" : item.PREVIOUS_DEP_RELATIONSHIP.Trim(),
                        DepChangeType = item.CHANGETYPE.Value,
                        Status = item.STATUS,
                        ModifiedDate = item.MODIFIED_DATE == null ? "" : item.MODIFIED_DATE.Value.ToString("dd-MMM-yyyy")
                    });
                }
                MIVM.Dependents = DepList;
            }
            return MIVM;
        }

        public List<VW_MEDINS_EMPDETAIL> GetHealthCenterDataForExcel(HealthCenterApprovalViewModel PageMdl)
        {
            List<VW_MEDINS_EMPDETAIL> VWList = new List<VW_MEDINS_EMPDETAIL>();
            DateTime Approvalfromdate = DateTime.Now.Date, Approvaltodate = DateTime.Now.Date, ReqDateFrom = DateTime.Now.Date, ReqDateTo = DateTime.Now.Date;
            long Approvalfromdateflag = 0, Approvaltodateflag = 0, ReqDateFromflag = 0, ReqDateToflag = 0;

            if (!string.IsNullOrEmpty(PageMdl.ApprovalDateFrom))
            {
                Approvalfromdate = DateTime.ParseExact(PageMdl.ApprovalDateFrom, "dd-MMM-yyyy", null);
                Approvalfromdateflag = 1;
            }
            if (!string.IsNullOrEmpty(PageMdl.ApprovalDateTo))
            {
                Approvaltodate = DateTime.ParseExact(PageMdl.ApprovalDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                Approvaltodateflag = 1;
            }
            if (!string.IsNullOrEmpty(PageMdl.ReqDateFrom))
            {
                ReqDateFrom = DateTime.ParseExact(PageMdl.ReqDateFrom, "dd-MMM-yyyy", null);
                ReqDateFromflag = 1;
            }
            if (!string.IsNullOrEmpty(PageMdl.ReqDateTo))
            {
                ReqDateTo = DateTime.ParseExact(PageMdl.ReqDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                ReqDateToflag = 1;
            }

            var subquery = from c in _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.APPROVAL_STATUS == PageMdl.ApprovalStatus)
                           group c by c.EMP_CODE into g
                           select new
                           {
                               ECODE = g.Key,
                               APPID = g.Max(a => a.APPROVALID)
                           };
            var iColl = (from data in _MedicalDBContext.MEDINS_APPHISTORY
                         join app in subquery on data.APPROVALID equals app.APPID
                         join loc in _MedicalDBContext.MEDINS_EMPPOLICYLOC on data.EMP_CODE equals loc.ADEMPCODE
                         join VW_Dtl in _MedicalDBContext.VW_MEDINS_EMPDETAIL on data.EMP_CODE equals VW_Dtl.EMP_CODE
                         where (Approvalfromdateflag == 0 ? true : data.APPROVAL_DATE >= Approvalfromdate) &&
                         (Approvaltodateflag == 0 ? true : data.APPROVAL_DATE <= Approvaltodate) &&
                         (ReqDateFromflag == 0 ? true : data.CREATEDDATE >= ReqDateFrom) &&
                         (ReqDateToflag == 0 ? true : data.CREATEDDATE <= ReqDateTo) &&
                         (PageMdl.PlantID == 0 ? true : loc.PLANTID == PageMdl.PlantID) &&
                         loc.STATUS == 1
                         select new
                         {
                             VW_Dtl.SNO,
                             VW_Dtl.REQUESTTYPE,
                             VW_Dtl.EMP_CODE,
                             VW_Dtl.ENAME,
                             VW_Dtl.TMOBILE,
                             VW_Dtl.PERNAME,
                             VW_Dtl.GENDER,
                             VW_Dtl.DOB,
                             VW_Dtl.RELATIONSHIP,
                             VW_Dtl.RECTYPE,
                             VW_Dtl.DESIGNATION,
                             VW_Dtl.CHANGETYPE,
                             VW_Dtl.PAIDTYPE
                         }).ToList();
            if (iColl != null)
            {
                foreach (var obj in iColl)
                {
                    VWList.Add(new VW_MEDINS_EMPDETAIL
                    {
                        SNO = obj.SNO,
                        REQUESTTYPE = obj.REQUESTTYPE,
                        EMP_CODE = obj.EMP_CODE,
                        ENAME = obj.ENAME,
                        TMOBILE = obj.TMOBILE,
                        PERNAME = obj.PERNAME,
                        GENDER = obj.GENDER,
                        DOB = obj.DOB,
                        RELATIONSHIP = obj.RELATIONSHIP,
                        RECTYPE = obj.RECTYPE,
                        DESIGNATION = obj.DESIGNATION,
                        CHANGETYPE = obj.CHANGETYPE,
                        PAIDTYPE = obj.PAIDTYPE
                    });
                }
            }
            return VWList;
        }

        public List<VW_MEDINS_MSTDETAILS> GetMasterDataForExcel(int plantId, string _ApprovalDateFrom, string _ApprovalDateTo, string _ReqDateFrom, string _ReqDateTo)
        {
            DateTime Approvalfromdate = DateTime.Now.Date, Approvaltodate = DateTime.Now.Date, ReqDateFrom = DateTime.Now.Date, ReqDateTo = DateTime.Now.Date;
            long Approvalfromdateflag = 0, Approvaltodateflag = 0, ReqDateFromflag = 0, ReqDateToflag = 0;
            if (!string.IsNullOrEmpty(_ApprovalDateFrom))
            {
                Approvalfromdate = DateTime.ParseExact(_ApprovalDateFrom, "dd-MMM-yyyy", null);
                Approvalfromdateflag = 1;
            }
            if (!string.IsNullOrEmpty(_ApprovalDateTo))
            {
                Approvaltodate = DateTime.ParseExact(_ApprovalDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                Approvaltodateflag = 1;
            }
            if (!string.IsNullOrEmpty(_ReqDateFrom))
            {
                ReqDateFrom = DateTime.ParseExact(_ReqDateFrom, "dd-MMM-yyyy", null);
                ReqDateFromflag = 1;
            }
            if (!string.IsNullOrEmpty(_ReqDateTo))
            {
                ReqDateTo = DateTime.ParseExact(_ReqDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                ReqDateToflag = 1;
            }
            List<VW_MEDINS_MSTDETAILS> VWList = new List<VW_MEDINS_MSTDETAILS>();
            var iColl = (from data in _MedicalDBContext.VW_MEDINS_MSTDETAILS
                         join loc in _MedicalDBContext.MEDINS_EMPPOLICYLOC on data.EMP_CODE equals loc.ADEMPCODE
                         where (Approvalfromdateflag == 0 ? true : data.APPROVAL_DATE >= Approvalfromdate) &&
                                     (Approvaltodateflag == 0 ? true : data.APPROVAL_DATE <= Approvaltodate) &&
                                     (ReqDateFromflag == 0 ? true : data.CREATEDDATE >= ReqDateFrom) &&
                                     (ReqDateToflag == 0 ? true : data.CREATEDDATE <= ReqDateTo) &&
                                     (plantId == 0 ? true : loc.PLANTID == plantId) && loc.STATUS == 1
                         select new
                         {
                             data.SNO,
                             data.REQUESTTYPE,
                             data.EMP_CODE,
                             data.ENAME,
                             data.TMOBILE,
                             data.PERNAME,
                             data.GENDER,
                             data.DOB,
                             data.AGE,
                             data.RELATIONSHIP,
                             data.RECTYPE,
                             data.DESIGNATION,
                             data.CHANGETYPE,
                             data.PAIDTYPE,
                             data.DOJ
                         }).ToList();
            if (iColl != null)
            {
                foreach (var obj in iColl)
                {
                    VWList.Add(new VW_MEDINS_MSTDETAILS
                    {
                        SNO = obj.SNO,
                        REQUESTTYPE = obj.REQUESTTYPE,
                        EMP_CODE = obj.EMP_CODE,
                        ENAME = obj.ENAME,
                        TMOBILE = obj.TMOBILE,
                        PERNAME = obj.PERNAME,
                        GENDER = obj.GENDER,
                        DOB = obj.DOB,
                        AGE = obj.AGE,
                        RELATIONSHIP = obj.RELATIONSHIP,
                        RECTYPE = obj.RECTYPE,
                        DESIGNATION = obj.DESIGNATION,
                        CHANGETYPE = obj.CHANGETYPE,
                        PAIDTYPE = obj.PAIDTYPE,
                        DOJ = obj.DOJ
                    });
                }
            }
            return VWList.OrderBy(o => o.EMP_CODE).ToList();
        }

        public MasterReportViewModel GetMasterDataReport(int currentPage, int plantId, string _ApprovalDateFrom, string _ApprovalDateTo, string _ReqDateFrom, string _ReqDateTo)
        {
            DateTime Approvalfromdate = DateTime.Now.Date, Approvaltodate = DateTime.Now.Date, ReqDateFrom = DateTime.Now.Date, ReqDateTo = DateTime.Now.Date;
            long Approvalfromdateflag = 0, Approvaltodateflag = 0, ReqDateFromflag = 0, ReqDateToflag = 0;
            if (!string.IsNullOrEmpty(_ApprovalDateFrom))
            {
                Approvalfromdate = DateTime.ParseExact(_ApprovalDateFrom, "dd-MMM-yyyy", null);
                Approvalfromdateflag = 1;
            }
            if (!string.IsNullOrEmpty(_ApprovalDateTo))
            {
                Approvaltodate = DateTime.ParseExact(_ApprovalDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                Approvaltodateflag = 1;
            }
            if (!string.IsNullOrEmpty(_ReqDateFrom))
            {
                ReqDateFrom = DateTime.ParseExact(_ReqDateFrom, "dd-MMM-yyyy", null);
                ReqDateFromflag = 1;
            }
            if (!string.IsNullOrEmpty(_ReqDateTo))
            {
                ReqDateTo = DateTime.ParseExact(_ReqDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                ReqDateToflag = 1;
            }

            int maxRows = 1000;
            MasterReportViewModel MRVM = new MasterReportViewModel();
            List<MasterReportViewModel> VWList = new List<MasterReportViewModel>();
            var iReportColl = (from data in _MedicalDBContext.VW_MEDINS_MSTDETAILS
                               join loc in _MedicalDBContext.MEDINS_EMPPOLICYLOC on data.EMP_CODE equals loc.ADEMPCODE
                               where (Approvalfromdateflag == 0 ? true : data.APPROVAL_DATE >= Approvalfromdate) &&
                                     (Approvaltodateflag == 0 ? true : data.APPROVAL_DATE <= Approvaltodate) &&
                                     (ReqDateFromflag == 0 ? true : data.CREATEDDATE >= ReqDateFrom) &&
                                     (ReqDateToflag == 0 ? true : data.CREATEDDATE <= ReqDateTo) &&
                                     (plantId == 0 ? true : loc.PLANTID == plantId) && loc.STATUS == 1
                               select new
                               {
                                   data.SNO,
                                   data.REQUESTTYPE,
                                   data.EMP_CODE,
                                   data.ENAME,
                                   data.TMOBILE,
                                   data.PERNAME,
                                   data.GENDER,
                                   data.DOB,
                                   data.AGE,
                                   data.RELATIONSHIP,
                                   data.RECTYPE,
                                   data.DESIGNATION,
                                   data.CHANGETYPE,
                                   data.PAIDTYPE,
                                   data.DOJ
                               }).ToList();
            var iList = iReportColl.OrderBy(o => o.EMP_CODE).Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();

            double pageCount = (double)((decimal)iReportColl.Count() / Convert.ToDecimal(maxRows));
            MRVM.PageCount = (int)Math.Ceiling(pageCount);
            MRVM.CurrentPageIndex = currentPage;

            if (iList != null)
            {
                foreach (var obj in iList)
                {
                    VWList.Add(new MasterReportViewModel
                    {
                        SNO = obj.SNO,
                        REQUESTTYPE = obj.REQUESTTYPE,
                        EMP_CODE = obj.EMP_CODE,
                        ENAME = obj.ENAME,
                        TMOBILE = obj.TMOBILE,
                        PERNAME = obj.PERNAME,
                        GENDER = obj.GENDER,
                        DOB = obj.DOB,
                        AGE = obj.AGE,
                        RELATIONSHIP = obj.RELATIONSHIP,
                        RECTYPE = obj.RECTYPE,
                        DESIGNATION = obj.DESIGNATION,
                        CHANGETYPE = obj.CHANGETYPE,
                        PAIDTYPE = obj.PAIDTYPE,
                        DOJ = obj.DOJ
                    });
                }
                MRVM.MRVMList = VWList;
            }
            return MRVM;
        }

        public FileViewModel GetProofForDownload(Int64 Id, Int64 EmpCode)
        {
            FileViewModel file = new FileViewModel();
            var Item = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == EmpCode && x.MI_DEPENDENTID == Id && x.CHANGETYPE == 3).SingleOrDefault();
            if (Item != null)
            {
                file.FileName = "";
                file.FileContentType = Item.DEPDEL_PROOF_CONTYPE;
                file.File = Item.DEPDEL_PROOF;
            }
            return file;
        }
        #endregion

        #region Other Associate User Save/Update/Detail
        public List<MedicalInsuranceViewModel> AdminPendingRequest(long EmpCode)
        {
            List<MedicalInsuranceViewModel> MIVMList = new List<MedicalInsuranceViewModel>();
            var ListObj = (from data in _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.CREATEDBY == EmpCode && (x.APPROVAL_STATUS == 0 || x.APPROVAL_STATUS == 3))
                           select new
                           {
                               data.EMP_CODE,
                               data.CREATEDDATE,
                               data.REQUESTTYPE,
                               data.APPROVAL_STATUS,
                               data.APPROVAL_DATE,
                               data.APPROVAL_BY,
                           }).ToList();
            if (ListObj.Count > 0)
            {
                foreach (var obj in ListObj)
                {
                    MIVMList.Add(new MedicalInsuranceViewModel
                    {
                        CreatedDate = obj.CREATEDDATE?.ToString("dd-MMM-yyyy"),
                        EmpCode = Convert.ToInt64(obj.EMP_CODE),
                        RequestType = obj.REQUESTTYPE,
                        Approval_Status = obj.APPROVAL_STATUS,
                        Approval_By = obj.APPROVAL_BY.ToString(),
                        Approval_Date = Convert.ToDateTime(obj.APPROVAL_DATE).ToString("dd-MMM-yyyy"),
                    });
                }
            }
            return MIVMList;
        }

        public PolicyAndPlantMappingViewModel GetAdminDashboardInfo(long DesId, long EmpCode)
        {
            PolicyAndPlantMappingViewModel PPVM = new PolicyAndPlantMappingViewModel();
            Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
            var plantId = Convert.ToInt64(emp.Plant_Id);
            var type = _MedicalDBContext.MEDINS_POLICYTYPE_MST.Where(x => x.STATUS == 1 && x.ISDEPENDENTREQ == 1).FirstOrDefault();
            int Company_Count = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == EmpCode && x.DEP_PAIDTYPE == 2 && (x.STATUS == 1 || x.STATUS == 2)).Count();
            int Associated_Count = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == EmpCode && x.DEP_PAIDTYPE == 1 && (x.STATUS == 1 || x.STATUS == 2)).Count();
            int ApprovalStatus = _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.EMP_CODE == EmpCode && x.APPROVAL_STATUS == 0).Count();
            
            if (plantId > 0)
            {
                var PlantLocation = (from data in _MedicalDBContext.MEDINS_TYPEPLANT_MAP.Where(x => x.PLANTID == plantId && x.POLICYTYPE_ID == type.POLICYTYPE_ID)
                                     join DESG_POLICYTYPE in _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP on data.MAPPINGID equals DESG_POLICYTYPE.MAPPINGID
                                     join PLANT in _MedicalDBContext.SYPLANT on data.PLANTID equals PLANT.SYPLANTID
                                     where (DESG_POLICYTYPE.DESG_ID == DesId)
                                     select new
                                     {
                                         data.COMPANYPAID_CNT,
                                         data.ASSOCIATEPAID_CNT,
                                         data.PLANTID,
                                         PLANT.PLANTNAME
                                     }).FirstOrDefault();
                if (PlantLocation != null)
                {
                    PPVM.AssociatedPaid_Cnt = Convert.ToInt16(Associated_Count);
                    PPVM.CompanyPaid_Cnt = Convert.ToInt16(Company_Count);
                    PPVM.TotalCompanyPaid = PlantLocation.COMPANYPAID_CNT;
                    PPVM.TotalAssociatedPaid = PlantLocation.ASSOCIATEPAID_CNT;
                    PPVM.Plant = PlantLocation.PLANTNAME;
                    PPVM.PlantId = PlantLocation.PLANTID;
                    PPVM.ApprovalStatus = Convert.ToInt16(ApprovalStatus);
                }
            }
            var RenewalPeriod = _MedicalDBContext.MEDINS_RENEWAL_SETTING.Where(x => x.PLANTID == plantId && x.STATUS == 1).FirstOrDefault();
            if (RenewalPeriod != null)
            {
                PPVM.Renewal_StartDate = RenewalPeriod.PERIOD_FROM.ToString("dd-MMM-yyyy");
                PPVM.Renewal_EndDate = RenewalPeriod.PERIOD_TO.ToString("dd-MMM-yyyy");
            }
            return PPVM;
        }

        public PolicyAndPlantMappingViewModel GetOtherAssociateInfo(long EmpCode)
        {
            PolicyAndPlantMappingViewModel PPVM = new PolicyAndPlantMappingViewModel();
            var ADEmployee = (from data in _MedicalDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == EmpCode && x.ACTIVE == 1)
                              join ADEMP_DETAIL in _MedicalDBContext.ADEMPDIVDEPTSECT on data.ADEMPCODE equals ADEMP_DETAIL.ADEMPCODE
                              select new
                              {
                                  data.ADEMPCODE,
                                  data.FIRSTNAME,
                                  data.LASTNAME,
                                  data.DOB,
                                  data.GENDER,
                                  ADEMP_DETAIL.ADDESIGNATIONID,
                                  ADEMP_DETAIL.SYPLANTID
                              }).FirstOrDefault();
            if (ADEmployee == null)
            {
                return PPVM;
            }
            var plant = _MedicalDBContext.MEDINS_EMPPOLICYLOC.Where(x => x.STATUS == 1 && x.ADEMPCODE == EmpCode).FirstOrDefault();
            var type = _MedicalDBContext.MEDINS_POLICYTYPE_MST.Where(x => x.STATUS == 1 && x.ISDEPENDENTREQ == 1).FirstOrDefault();
            int ApprovalStatus = _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.EMP_CODE == EmpCode && x.APPROVAL_STATUS == 0).Count();
            if (plant != null)
            {
                var PlantLocation = (from data in _MedicalDBContext.MEDINS_TYPEPLANT_MAP.Where(x => x.PLANTID == plant.PLANTID && x.POLICYTYPE_ID == type.POLICYTYPE_ID)
                                     join DESG_POLICYTYPE in _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP on data.MAPPINGID equals DESG_POLICYTYPE.MAPPINGID
                                     join PLANT in _MedicalDBContext.SYPLANT on data.PLANTID equals PLANT.SYPLANTID
                                     where (DESG_POLICYTYPE.DESG_ID == ADEmployee.ADDESIGNATIONID)
                                     select new
                                     {
                                         data.COMPANYPAID_CNT,
                                         data.ASSOCIATEPAID_CNT,
                                         data.PLANTID,
                                         PLANT.PLANTNAME
                                     }).FirstOrDefault();
                if (PlantLocation != null)
                {
                    PPVM.TotalCompanyPaid = PlantLocation.COMPANYPAID_CNT;
                    PPVM.TotalAssociatedPaid = PlantLocation.ASSOCIATEPAID_CNT;
                    PPVM.Plant = PlantLocation == null ? "" : PlantLocation.PLANTNAME;
                }
            }
            PPVM.ApprovalStatus = Convert.ToInt16(ApprovalStatus);
            return PPVM;
        }

        public Int16 SaveOtherAssociateUserDetail(MedicalInsuranceViewModel MIVM)
        {
            Int16 retVal = 0;
            MEDINS_APPHISTORY APPHISTORY = _MedicalDBContext.MEDINS_APPHISTORY.Where(x => x.EMP_CODE == MIVM.EmpCode && (x.APPROVAL_STATUS == 0 || x.APPROVAL_STATUS == 3)).FirstOrDefault();
            if (APPHISTORY != null && MIVM.RequestType != 1)
            {
                retVal = 2; // Request already in process
                return retVal;
            }
            using (IDbContextTransaction transaction = _MedicalDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (APPHISTORY == null)
                    {
                        APPHISTORY = SaveOtherAssociateUserApprovalHistory(MIVM);
                    }
                    #region "Creating History for Employee self and Dependent detail before initiation of new changes"
                    var hisemp = _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode).FirstOrDefault();
                    if (MIVM.RequestType == 1 && hisemp != null)
                    {
                        APPHISTORY.APPROVAL_STATUS = 0;
                        APPHISTORY.MODIFIED_DATE = DateTime.Now;
                        APPHISTORY.MODIFIED_BY = MIVM.CreatedBy;

                        _MedicalDBContext.Entry(APPHISTORY).State = EntityState.Modified;
                        _MedicalDBContext.SaveChanges();

                        _MedicalDBContext.Entry(hisemp).State = EntityState.Deleted;
                        _MedicalDBContext.SaveChanges();
                        var hisdep = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode);
                        foreach (var obj in hisdep)
                        {
                            _MedicalDBContext.Entry(obj).State = EntityState.Deleted;
                            _MedicalDBContext.SaveChanges();
                        }
                        hisemp = null;

                    }

                    if (hisemp != null)
                    {
                        var hisdep = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode);
                        MEDINS_EMPLOYEE_DTL_HIS objemphis = new MEDINS_EMPLOYEE_DTL_HIS();
                        objemphis.MI_EMPDETAILIDHIS = _MedicalDBContext.MEDINS_EMPLOYEE_DTL_HIS.Max(x => x.MI_EMPDETAILIDHIS) + 1;
                        objemphis.APPROVALID = APPHISTORY.APPROVALID;
                        objemphis.CREATED_BY = hisemp.CREATED_BY;
                        objemphis.CREATED_DATE = hisemp.CREATED_DATE;
                        objemphis.DOB = hisemp.DOB;
                        objemphis.DOM = hisemp.DOM;
                        objemphis.EMP_CODE = hisemp.EMP_CODE;
                        objemphis.EMP_MODIFIEDDATE = hisemp.EMP_MODIFIEDDATE;
                        objemphis.EMP_NAME = hisemp.EMP_NAME;
                        objemphis.EMP_STATUS = hisemp.EMP_STATUS;
                        objemphis.GENDER = hisemp.GENDER;
                        objemphis.MODIFIEDBY = hisemp.MODIFIEDBY;
                        objemphis.NOMINEE_DOB = hisemp.NOMINEE_DOB;
                        objemphis.NOMINEE_GENDER = hisemp.NOMINEE_GENDER;
                        objemphis.NOMINEE_MODIFIEDDATE = hisemp.NOMINEE_MODIFIEDDATE;
                        objemphis.NOMINEE_NAME = hisemp.NOMINEE_NAME;
                        objemphis.NOMINEE_RELATION = hisemp.NOMINEE_RELATION;
                        objemphis.NOMINEE_STATUS = hisemp.NOMINEE_STATUS;
                        objemphis.PREVIOUS_EMPDOB = hisemp.PREVIOUS_EMPDOB;
                        objemphis.PREVIOUS_EMP_NAME = hisemp.PREVIOUS_EMP_NAME;
                        objemphis.PREVIOUS_NOMINEE_DOB = hisemp.PREVIOUS_NOMINEE_DOB;
                        objemphis.PREVIOUS_NOMINEE_GENDER = hisemp.PREVIOUS_NOMINEE_GENDER;
                        objemphis.PREVIOUS_NOMINEE_NAME = hisemp.PREVIOUS_NOMINEE_NAME;
                        objemphis.PREVIOUS_NOMINEE_RELATION = hisemp.PREVIOUS_NOMINEE_RELATION;
                        _MedicalDBContext.Entry(objemphis).State = EntityState.Added;
                        _MedicalDBContext.SaveChanges();
                        var MI_DEPENDENTIDHIS=_MedicalDBContext.MEDINS_DEPENDENTS_DTL_HIS.Max(x => x.MI_DEPENDENTIDHIS) + 1;
                        foreach (var obj in hisdep)
                        {
                            MEDINS_DEPENDENTS_DTL_HIS objMedDep = new MEDINS_DEPENDENTS_DTL_HIS();
                            objMedDep.MI_DEPENDENTIDHIS = MI_DEPENDENTIDHIS;
                            objMedDep.APPROVALID = APPHISTORY.APPROVALID;
                            objMedDep.CHANGETYPE = obj.CHANGETYPE;
                            objMedDep.CREATED_BY = obj.CREATED_BY;
                            objMedDep.CREATED_DATE = obj.CREATED_DATE;
                            objMedDep.DEP_DOB = obj.DEP_DOB;
                            objMedDep.DEP_GENDER = obj.DEP_GENDER;
                            objMedDep.DEP_NAME = obj.DEP_NAME;
                            objMedDep.DEP_PAIDTYPE = obj.DEP_PAIDTYPE;
                            objMedDep.DEP_PHOTO = obj.DEPDEL_PROOF;
                            objMedDep.DEP_PHOTO_CONTYPE = obj.DEPDEL_PROOF_CONTYPE;
                            objMedDep.DEP_RELATIONSHIP = obj.DEP_RELATIONSHIP;
                            objMedDep.EMP_CODE = obj.EMP_CODE;
                            objMedDep.MODIFIED_BY = obj.MODIFIED_BY;
                            objMedDep.MODIFIED_DATE = obj.MODIFIED_DATE;
                            objMedDep.POLICYTYPE = obj.POLICYTYPE;
                            objMedDep.PREVIOUS_DEP_DOB = obj.PREVIOUS_DEP_DOB;
                            objMedDep.PREVIOUS_DEP_GENDER = obj.PREVIOUS_DEP_GENDER;
                            objMedDep.PREVIOUS_DEP_NAME = obj.PREVIOUS_DEP_NAME;
                            objMedDep.PREVIOUS_DEP_RELATIONSHIP = obj.PREVIOUS_DEP_RELATIONSHIP;
                            objMedDep.REMARKS = obj.REMARKS;
                            objMedDep.STATUS = obj.STATUS;
                            _MedicalDBContext.Entry(objMedDep).State = EntityState.Added;
                            _MedicalDBContext.SaveChanges();
                            MI_DEPENDENTIDHIS++;
                        }
                    }
                    #endregion
                    MEDINS_EMPLOYEE_DTL MIED = new MEDINS_EMPLOYEE_DTL();
                    if (MIVM.RequestType == 2 || MIVM.RequestType == 3)
                    {
                        MIED = _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode).FirstOrDefault();
                        if (MIED == null)
                        {
                            MIED = new MEDINS_EMPLOYEE_DTL();
                        }
                    }

                    if (MIVM.RequestType == 1)
                    {
                        // Emp Detail
                        MIED.EMP_CODE = MIVM.EmpCode;
                        MIED.EMP_NAME = MIVM.Name;
                        MIED.GENDER = Convert.ToInt16(MIVM.Gender == "Male" ? 1 : 2);
                        MIED.DOB = DateTime.ParseExact(MIVM.DOB, "dd-MMM-yyyy", null);
                        MIED.EMP_STATUS = 2; // WIP 

                        MIED.NOMINEE_NAME = MIVM.NomineeDetail.NomineeName;
                        MIED.NOMINEE_RELATION = MIVM.NomineeDetail.NomineeRelation;
                        MIED.NOMINEE_GENDER = Convert.ToInt16(MIVM.NomineeDetail.NomineeGender == "Male" ? 1 : 2);
                        MIED.NOMINEE_DOB = DateTime.ParseExact(MIVM.NomineeDetail.NomineeDOB, "dd-MMM-yyyy", null);
                        MIED.NOMINEE_STATUS = 2; // WIP 
                        MIED.CREATED_DATE = DateTime.Now;
                        MIED.CREATED_BY = MIVM.CreatedBy ?? 0;
                    }

                    if (MIVM.RequestType == 3 && (MIVM.Name != MIED.EMP_NAME || MIVM.DOB != MIED.DOB.ToString("dd-MMM-yyyy")))
                    {
                        // Pre-Self detail
                        MIED.PREVIOUS_EMP_NAME = MIED.EMP_NAME;
                        MIED.PREVIOUS_EMPDOB = MIED.DOB;
                        // self detail
                        MIED.EMP_NAME = MIVM.Name;
                        MIED.DOB = DateTime.ParseExact(MIVM.DOB, "dd-MMM-yyyy", null);
                        MIED.EMP_STATUS = 2; // WIP 
                        MIED.EMP_MODIFIEDDATE = DateTime.Now;
                        MIED.MODIFIEDBY = MIVM.CreatedBy;
                    }

                    if (MIVM.RequestType == 3 && (MIED.NOMINEE_NAME != MIVM.NomineeDetail.NomineeName || MIED.NOMINEE_DOB.ToString("dd-MMM-yyyy") != MIVM.NomineeDetail.NomineeDOB || MIED.NOMINEE_GENDER != Convert.ToInt16(MIVM.NomineeDetail.NomineeGender == "Male" ? 1 : 2) || MIED.NOMINEE_RELATION != MIVM.NomineeDetail.NomineeRelation))
                    {
                        // Pre-nominee detail
                        MIED.PREVIOUS_NOMINEE_NAME = MIED.NOMINEE_NAME;
                        MIED.PREVIOUS_NOMINEE_DOB = MIED.NOMINEE_DOB;
                        MIED.PREVIOUS_NOMINEE_GENDER = MIED.NOMINEE_GENDER;
                        MIED.PREVIOUS_NOMINEE_RELATION = MIED.NOMINEE_RELATION;
                        // Nominee Detail
                        MIED.NOMINEE_NAME = MIVM.NomineeDetail.NomineeName;
                        MIED.NOMINEE_RELATION = MIVM.NomineeDetail.NomineeRelation;
                        MIED.NOMINEE_GENDER = Convert.ToInt16(MIVM.NomineeDetail.NomineeGender == "Male" ? 1 : 2);
                        MIED.NOMINEE_DOB = DateTime.ParseExact(MIVM.NomineeDetail.NomineeDOB, "dd-MMM-yyyy", null);
                        MIED.NOMINEE_STATUS = 2; // WIP 
                        MIED.NOMINEE_MODIFIEDDATE = DateTime.Now;
                        MIED.MODIFIEDBY = MIVM.CreatedBy;
                    }
                    if (MIED.MI_EMPDETAILID == 0)
                    {
                        MIED.MI_EMPDETAILID = _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Max(x => x.MI_EMPDETAILID) + 1;
                    }
                    _MedicalDBContext.Entry(MIED).State = MIED.MI_EMPDETAILID == 0 ? EntityState.Added : EntityState.Modified;
                    _MedicalDBContext.SaveChanges();
                    SaveOtherAssociateUserDependentDetail(MIVM);
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

        public MEDINS_DEPENDENTS_DTL SaveOtherAssociateUserDependentDetail(MedicalInsuranceViewModel MIVM)
        {
            MEDINS_DEPENDENTS_DTL MIDD = new MEDINS_DEPENDENTS_DTL();
            List<MEDINS_DEPENDENTS_DTL> MIDDList = new List<MEDINS_DEPENDENTS_DTL>();
            if (MIVM.RequestType == 2 || MIVM.RequestType == 3)
            {
                MIDDList = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == MIVM.EmpCode).ToList();
            }
            if (MIVM.DeleteDependentList != null && MIVM.DeleteDependentList.Count() > 0)
            {
                foreach (var oDel in MIVM.DeleteDependentList)
                {
                    var oItem = MIDDList.Where(m => m.MI_DEPENDENTID == oDel.DepItemId).FirstOrDefault();
                    if (oItem != null)
                    {
                        oItem.STATUS = 2;
                        oItem.CHANGETYPE = 3;
                        oItem.REMARKS = oDel.DeleteDepRemarks;
                        //var proofString = oDel.Doc_fileString;
                        //proofString = proofString.Replace("data:image/jpeg;base64,", String.Empty);
                        //proofString = proofString.Replace("data:image/jpg;base64,", String.Empty);
                        //proofString = proofString.Replace("data:image/png;base64,", String.Empty);
                        //proofString = proofString.Replace("data:image/pdf;base64,", String.Empty);
                        //oItem.DEPDEL_PROOF = Convert.FromBase64String(proofString);
                        //oItem.DEPDEL_PROOF_CONTYPE = oDel.Doc_ContentType;
                        oItem.MODIFIED_BY = MIVM.CreatedBy;
                        oItem.MODIFIED_DATE = DateTime.Now;
                        _MedicalDBContext.Entry(oItem).State = EntityState.Modified;
                        _MedicalDBContext.SaveChanges();
                    }
                }
            }
            if (MIVM.Dependents != null)
            {
                foreach (var dependent in MIVM.Dependents)
                {
                    if (MIDDList.Count() > 0)
                    {
                        MIDD = MIDDList.Where(m => m.MI_DEPENDENTID == dependent.DepItemId).FirstOrDefault();
                        if (MIDD == null)
                        {
                            MIDD = new MEDINS_DEPENDENTS_DTL();
                        }
                    }
                    else
                    {
                        MIDD = new MEDINS_DEPENDENTS_DTL();
                    }
                    if (MIDD.MI_DEPENDENTID == 0)
                    {
                        MIDD.EMP_CODE = MIVM.EmpCode;
                        MIDD.DEP_NAME = dependent.DepName;
                        MIDD.DEP_GENDER = Convert.ToInt16(dependent.DepGender == "Male" ? 1 : 2);
                        MIDD.DEP_DOB = DateTime.ParseExact(dependent.DepDOB, "dd-MMM-yyyy", null);
                        MIDD.DEP_RELATIONSHIP = dependent.DepRelation;
                        MIDD.STATUS = 2; // WIP 
                        MIDD.DEP_PAIDTYPE = Convert.ToInt16(dependent.DepPolicyType == "Associate Paid" ? 1 : 2);
                        MIDD.CHANGETYPE = dependent.DepChangeType; // 1-Addition, 2-Modification, 3-Delection
                        MIDD.CREATED_DATE = DateTime.Now;
                        MIDD.CREATED_BY = MIVM.CreatedBy;
                        if (MIVM.RequestType == 2)
                        {
                            int policyType = dependent.DepRequestType == "ChildBirth" ? 1 : dependent.DepRequestType == "Marriage" ? 2 : 3; // Child Birth, Marriage, Death Case
                            MIDD.POLICYTYPE = Convert.ToInt16(policyType); // Child Birth,Marriage,Death Case
                        }
                    }
                    else
                    {
                        // Dependent Detail
                        if ((MIVM.RequestType == 3) && (MIDD.DEP_NAME != dependent.DepName || MIDD.DEP_GENDER != Convert.ToInt16(dependent.DepGender == "Male" ? 1 : 2) || MIDD.DEP_DOB != DateTime.ParseExact(dependent.DepDOB, "dd-MMM-yyyy", null) || MIDD.DEP_RELATIONSHIP != dependent.DepRelation || MIDD.CHANGETYPE != dependent.DepChangeType))
                        {
                            // preDep- details
                            MIDD.PREVIOUS_DEP_NAME = MIDD.DEP_NAME;
                            MIDD.PREVIOUS_DEP_GENDER = MIDD.DEP_GENDER;
                            MIDD.PREVIOUS_DEP_DOB = MIDD.DEP_DOB;
                            MIDD.PREVIOUS_DEP_RELATIONSHIP = MIDD.DEP_RELATIONSHIP;
                            // Dep- details
                            MIDD.EMP_CODE = MIVM.EmpCode;
                            MIDD.DEP_NAME = dependent.DepName;
                            MIDD.DEP_GENDER = Convert.ToInt16(dependent.DepGender == "Male" ? 1 : 2);
                            MIDD.DEP_DOB = DateTime.ParseExact(dependent.DepDOB, "dd-MMM-yyyy", null);
                            MIDD.DEP_RELATIONSHIP = dependent.DepRelation;
                            MIDD.STATUS = 2; // WIP 
                            MIDD.DEP_PAIDTYPE = Convert.ToInt16(dependent.DepPolicyType == "Associate Paid" ? 1 : 2);
                            MIDD.CHANGETYPE = dependent.DepChangeType; // 1-Addition, 2-Modification, 3-Delection
                            MIDD.MODIFIED_DATE = DateTime.Now;
                            MIDD.MODIFIED_BY = MIVM.CreatedBy;
                        }
                    }
                    if(MIDD.MI_DEPENDENTID == 0)
                    {
                        MIDD.MI_DEPENDENTID = _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Max(x => x.MI_DEPENDENTID) + 1;
                    }
                    _MedicalDBContext.Entry(MIDD).State = MIDD.MI_DEPENDENTID == 0 ? EntityState.Added : EntityState.Modified;
                    _MedicalDBContext.SaveChanges();
                }
            }
            //MEDINS_APPHISTORY MAP = SaveOtherAssociateUserApprovalHistory(MIVM);
            return MIDD;
        }

        public MEDINS_APPHISTORY SaveOtherAssociateUserApprovalHistory(MedicalInsuranceViewModel MIVM)
        {
            var APPROVALID = _MedicalDBContext.MEDINS_APPHISTORY.Max(x => x.APPROVALID) + 1; 
            MEDINS_APPHISTORY MAP = new MEDINS_APPHISTORY();
            MAP.EMP_CODE = MIVM.EmpCode;
            MAP.APPROVALID = APPROVALID;
            MAP.APPROVAL_STATUS = 0; // Pending
            MAP.REQUESTTYPE = MIVM.RequestType;
            MAP.CREATEDBY = MIVM.CreatedBy ?? 0;
            MAP.CREATEDDATE = DateTime.Now;
            _MedicalDBContext.MEDINS_APPHISTORY.Add(MAP);
            _MedicalDBContext.SaveChanges();
            return MAP;
        }

        public MedicalInsuranceViewModel NewJoineeOtherAssociateByEmpCode(Int64 EmpCode, Int64 PlantId)
        {
            MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
            var sykiid = _MedicalDBContext.SYKI.Where(k => k.ACTIVE == 1).FirstOrDefault().SYKIID;
            String[] parameterValue = GetParameterValue("DESG_BELOWJE").Split(',');
            var ADEmployee = (from data in _MedicalDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == EmpCode && x.ACTIVE == 1)
                              join ADEMP_DETAIL in _MedicalDBContext.ADEMPDIVDEPTSECT on data.ADEMPCODE equals ADEMP_DETAIL.ADEMPCODE
                              where ADEMP_DETAIL.SYKI == sykiid
                              select new
                              {
                                  data.ADEMPCODE,
                                  data.FIRSTNAME,
                                  data.LASTNAME,
                                  data.DOB,
                                  data.GENDER,
                                  ADEMP_DETAIL.ADDESIGNATIONID,
                                  ADEMP_DETAIL.SYPLANTID
                              }).FirstOrDefault();
            if (ADEmployee == null)
            {
                MIVM.IsEmpExist = 2; // Emplyee does not exist
                return MIVM;
            }

            if (!parameterValue.Contains(Convert.ToString(ADEmployee.ADDESIGNATIONID)) && !(EmpCode > 70000000 && EmpCode < 79000000))
            {
                MIVM.IsEmpExist = 3; // You can add only Company Casual, Staff, Apprentice, Advisor.
                return MIVM;
            }

            var EmpPolicyLoc_Obj = (from data in _MedicalDBContext.MEDINS_EMPPOLICYLOC.Where(x => x.ADEMPCODE == ADEmployee.ADEMPCODE && x.STATUS == 1)
                                    select new
                                    {
                                        data.ADEMPCODE,
                                        data.PLANTID
                                    }).FirstOrDefault();
            if (EmpPolicyLoc_Obj == null)
            {
                MIVM.IsEmpExist = 4; // Policy Location is not mapped.
                return MIVM;
            }
            else if (Convert.ToInt64(EmpPolicyLoc_Obj.PLANTID) != PlantId)
            {
                MIVM.IsEmpExist = 5; // Plant not match with other assocate user.
                return MIVM;
            }
            var DesPolicyPlant_Obj = (from data in _MedicalDBContext.MEDINS_TYPEPLANT_MAP.Where(x => x.PLANTID == EmpPolicyLoc_Obj.PLANTID)
                                      join DESG_POLICYTYPE in _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP on data.MAPPINGID equals DESG_POLICYTYPE.MAPPINGID
                                      where (DESG_POLICYTYPE.DESG_ID == ADEmployee.ADDESIGNATIONID)
                                      select new
                                      {
                                          data.PLANTID,
                                          DESG_POLICYTYPE.DESG_ID
                                      }).FirstOrDefault();
            if (DesPolicyPlant_Obj == null)
            {
                MIVM.IsEmpExist = 6; // Designation Policy Plant is not mapped.
                return MIVM;
            }

            if (ADEmployee != null)
            {
                var MedicalUser = (from data in _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == EmpCode)
                                   select new
                                   {
                                       data.EMP_CODE,
                                       data.EMP_NAME,
                                       data.EMP_STATUS,
                                   }).SingleOrDefault();
                int isMedical = MedicalUser == null ? 0 : 1;
                var isSendBackUser = _MedicalDBContext.MEDINS_APPHISTORY.Where(m => m.APPROVAL_STATUS == 3 && m.EMP_CODE == EmpCode).FirstOrDefault();
                MIVM.IsMedicalInsurance = isSendBackUser == null ? Convert.ToInt16(isMedical) : Convert.ToInt16(0);
                if (isMedical == 0)
                {
                    MIVM.EmpCode = ADEmployee.ADEMPCODE;
                    MIVM.Name = ADEmployee.FIRSTNAME + " " + ADEmployee.LASTNAME;
                    MIVM.DOB = Convert.ToDateTime(ADEmployee.DOB).ToString("dd-MMM-yyyy");
                    MIVM.Gender = ADEmployee.GENDER == "M" ? "Male" : "Female";
                    MIVM.IsEmpExist = 1;
                }
                else if (isMedical == 1)
                {
                    List<DependentDetailViewModel> DepList = new List<DependentDetailViewModel>();
                    var SelfDetail = (from data in _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == EmpCode)
                                      select new
                                      {
                                          data.MI_EMPDETAILID,
                                          data.EMP_CODE,
                                          data.EMP_NAME,
                                          data.GENDER,
                                          data.DOB,
                                          data.EMP_STATUS,
                                          data.NOMINEE_NAME,
                                          data.NOMINEE_RELATION,
                                          data.NOMINEE_GENDER,
                                          data.NOMINEE_DOB,
                                          data.NOMINEE_STATUS,
                                          data.DOM,
                                          data.PREVIOUS_EMPDOB,
                                          data.PREVIOUS_EMP_NAME,
                                          data.PREVIOUS_NOMINEE_DOB,
                                          data.PREVIOUS_NOMINEE_GENDER,
                                          data.PREVIOUS_NOMINEE_NAME,
                                          data.PREVIOUS_NOMINEE_RELATION
                                      }
                            ).SingleOrDefault();
                    var DepDetails = (from data in _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == EmpCode && x.CHANGETYPE != 3)
                                      select new
                                      {
                                          data.MI_DEPENDENTID,
                                          data.EMP_CODE,
                                          data.DEP_NAME,
                                          data.DEP_GENDER,
                                          data.DEP_DOB,
                                          data.DEP_PAIDTYPE,
                                          data.DEP_RELATIONSHIP,
                                          data.STATUS,
                                          data.PREVIOUS_DEP_DOB,
                                          data.PREVIOUS_DEP_GENDER,
                                          data.PREVIOUS_DEP_NAME,
                                          data.PREVIOUS_DEP_RELATIONSHIP,
                                          data.CHANGETYPE
                                      }
                             ).ToList();
                    if (SelfDetail != null)
                    {
                        MIVM.MedicalInsuranceID = SelfDetail.MI_EMPDETAILID;
                        MIVM.EmpCode = SelfDetail.EMP_CODE;
                        MIVM.Name = SelfDetail.EMP_NAME;
                        MIVM.Gender = SelfDetail.GENDER == 1 ? "Male" : "Female";
                        MIVM.DOB = SelfDetail.DOB.ToString("dd-MMM-yyyy");
                        MIVM.Emp_Status = SelfDetail.EMP_STATUS;
                        MIVM.PREVIOUS_EMPDOB = (SelfDetail.PREVIOUS_EMPDOB != null ? SelfDetail.PREVIOUS_EMPDOB.Value.ToString("dd-MMM-yyyy") : "");
                        MIVM.PREVIOUS_EMP_NAME = SelfDetail.PREVIOUS_EMP_NAME;
                        MIVM.NomineeDetail = new NomineeDetailViewModel
                        {
                            NomineeName = SelfDetail.NOMINEE_NAME == null ? "" : SelfDetail.NOMINEE_NAME.Trim(),
                            NomineeRelation = SelfDetail.NOMINEE_RELATION == null ? "" : SelfDetail.NOMINEE_RELATION.Trim(),
                            NomineeGender = SelfDetail.NOMINEE_GENDER == 1 ? "Male" : "Female",
                            NomineeDOB = SelfDetail.NOMINEE_DOB == null ? "" : SelfDetail.NOMINEE_DOB.ToString("dd-MMM-yyyy").Trim(),
                            Nominee_Status = SelfDetail.NOMINEE_STATUS,
                            PREVIOUS_NOMINEE_DOB = SelfDetail.PREVIOUS_NOMINEE_DOB == null ? "" : SelfDetail.PREVIOUS_NOMINEE_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                            PREVIOUS_NOMINEE_GENDER = SelfDetail.PREVIOUS_NOMINEE_GENDER != null ? (SelfDetail.PREVIOUS_NOMINEE_GENDER == 1 ? "Male" : "Female") : "",
                            PREVIOUS_NOMINEE_RELATION = SelfDetail.PREVIOUS_NOMINEE_RELATION == null ? "" : SelfDetail.PREVIOUS_NOMINEE_RELATION.Trim(),
                            PREVIOUS_NOMINEE_NAME = SelfDetail.PREVIOUS_NOMINEE_NAME == null ? "" : SelfDetail.PREVIOUS_NOMINEE_NAME.Trim()
                        };
                        int Sno = 1;
                        foreach (var item in DepDetails)
                        {
                            DepList.Add(new DependentDetailViewModel
                            {
                                Sno = Sno++,
                                DepItemId = item.MI_DEPENDENTID,
                                DepName = item.DEP_NAME == null ? "" : item.DEP_NAME.Trim(),
                                DepDOB = item.DEP_DOB == null ? "" : item.DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                                DepGender = item.DEP_GENDER == 1 ? "Male" : "Female",
                                DepRelation = item.DEP_RELATIONSHIP == null ? "" : item.DEP_RELATIONSHIP.Trim(),
                                DepPolicyType = item.DEP_PAIDTYPE == 1 ? "Associate Paid" : "Company Paid",
                                PREVIOUS_DEP_DOB = item.PREVIOUS_DEP_DOB == null ? "" : item.PREVIOUS_DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                                PREVIOUS_DEP_GENDER = item.PREVIOUS_DEP_GENDER != null ? (item.PREVIOUS_DEP_GENDER == 1 ? "Male" : "Female") : "",
                                PREVIOUS_DEP_NAME = item.PREVIOUS_DEP_NAME == null ? "" : item.PREVIOUS_DEP_NAME.Trim(),
                                PREVIOUS_DEP_RELATION = item.PREVIOUS_DEP_RELATIONSHIP == null ? "" : item.PREVIOUS_DEP_RELATIONSHIP.Trim(),
                                DepChangeType = item.CHANGETYPE.Value
                            });
                        }
                        MIVM.IsEmpExist = 1;
                        MIVM.Dependents = DepList;
                    }
                }
            }
            return MIVM;
        }

        public MedicalInsuranceViewModel OtherAssociateByEmpCode(Int64 EmpCode)
        {
            MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
            String[] parameterValue = GetParameterValue("DESG_BELOWJE").Split(',');
            var sykiid = _MedicalDBContext.SYKI.Where(k => k.ACTIVE == 1).FirstOrDefault().SYKIID;
            var DesignationId = _MedicalDBContext.ADEMPDIVDEPTSECT.Where(x => x.ADEMPCODE == EmpCode && x.ACTIVE == 1 && x.SYKI == sykiid).Select(s => s.ADDESIGNATIONID).FirstOrDefault();
            if (DesignationId == null)
            {
                MIVM.IsEmpExist = 2; // Emplyee does not exist
                return MIVM;
            }
            if (parameterValue.Contains(Convert.ToString(DesignationId)) || (EmpCode > 70000000 && EmpCode < 79000000))
            {
                List<DependentDetailViewModel> DepList = new List<DependentDetailViewModel>();
                var SelfDetail = (from data in _MedicalDBContext.MEDINS_EMPLOYEE_DTL.Where(x => x.EMP_CODE == EmpCode)
                                  select new
                                  {
                                      data.MI_EMPDETAILID,
                                      data.EMP_CODE,
                                      data.EMP_NAME,
                                      data.GENDER,
                                      data.DOB,
                                      data.EMP_STATUS,
                                      data.NOMINEE_NAME,
                                      data.NOMINEE_RELATION,
                                      data.NOMINEE_GENDER,
                                      data.NOMINEE_DOB,
                                      data.NOMINEE_STATUS,
                                      data.DOM,
                                      data.PREVIOUS_EMPDOB,
                                      data.PREVIOUS_EMP_NAME,
                                      data.PREVIOUS_NOMINEE_DOB,
                                      data.PREVIOUS_NOMINEE_GENDER,
                                      data.PREVIOUS_NOMINEE_NAME,
                                      data.PREVIOUS_NOMINEE_RELATION
                                  }
                        ).SingleOrDefault();
                var DepDetails = (from data in _MedicalDBContext.MEDINS_DEPENDENTS_DTL.Where(x => x.EMP_CODE == EmpCode && x.CHANGETYPE != 3)
                                  select new
                                  {
                                      data.MI_DEPENDENTID,
                                      data.EMP_CODE,
                                      data.DEP_NAME,
                                      data.DEP_GENDER,
                                      data.DEP_DOB,
                                      data.DEP_PAIDTYPE,
                                      data.DEP_RELATIONSHIP,
                                      data.STATUS,
                                      data.PREVIOUS_DEP_DOB,
                                      data.PREVIOUS_DEP_GENDER,
                                      data.PREVIOUS_DEP_NAME,
                                      data.PREVIOUS_DEP_RELATIONSHIP,
                                      data.CHANGETYPE
                                  }
                         ).ToList();
                if (SelfDetail != null)
                {
                    MIVM.MedicalInsuranceID = SelfDetail.MI_EMPDETAILID;
                    MIVM.EmpCode = SelfDetail.EMP_CODE;
                    MIVM.Name = SelfDetail.EMP_NAME;
                    MIVM.Gender = SelfDetail.GENDER == 1 ? "Male" : "Female";
                    MIVM.DOB = SelfDetail.DOB.ToString("dd-MMM-yyyy");
                    MIVM.Emp_Status = SelfDetail.EMP_STATUS;
                    MIVM.PREVIOUS_EMPDOB = (SelfDetail.PREVIOUS_EMPDOB != null ? SelfDetail.PREVIOUS_EMPDOB.Value.ToString("dd-MMM-yyyy") : "");
                    MIVM.PREVIOUS_EMP_NAME = SelfDetail.PREVIOUS_EMP_NAME;
                    MIVM.NomineeDetail = new NomineeDetailViewModel
                    {
                        NomineeName = SelfDetail.NOMINEE_NAME == null ? "" : SelfDetail.NOMINEE_NAME.Trim(),
                        NomineeRelation = SelfDetail.NOMINEE_RELATION == null ? "" : SelfDetail.NOMINEE_RELATION.Trim(),
                        NomineeGender = SelfDetail.NOMINEE_GENDER == 1 ? "Male" : "Female",
                        NomineeDOB = SelfDetail.NOMINEE_DOB == null ? "" : SelfDetail.NOMINEE_DOB.ToString("dd-MMM-yyyy").Trim(),
                        Nominee_Status = SelfDetail.NOMINEE_STATUS,
                        PREVIOUS_NOMINEE_DOB = SelfDetail.PREVIOUS_NOMINEE_DOB == null ? "" : SelfDetail.PREVIOUS_NOMINEE_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                        PREVIOUS_NOMINEE_GENDER = SelfDetail.PREVIOUS_NOMINEE_GENDER != null ? (SelfDetail.PREVIOUS_NOMINEE_GENDER == 1 ? "Male" : "Female") : "",
                        PREVIOUS_NOMINEE_RELATION = SelfDetail.PREVIOUS_NOMINEE_RELATION == null ? "" : SelfDetail.PREVIOUS_NOMINEE_RELATION.Trim(),
                        PREVIOUS_NOMINEE_NAME = SelfDetail.PREVIOUS_NOMINEE_NAME == null ? "" : SelfDetail.PREVIOUS_NOMINEE_NAME.Trim()
                    };
                    int Sno = 1;
                    foreach (var item in DepDetails)
                    {
                        DepList.Add(new DependentDetailViewModel
                        {
                            Sno = Sno++,
                            DepItemId = item.MI_DEPENDENTID,
                            DepName = item.DEP_NAME == null ? "" : item.DEP_NAME.Trim(),
                            DepDOB = item.DEP_DOB == null ? "" : item.DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                            DepGender = item.DEP_GENDER == 1 ? "Male" : "Female",
                            DepRelation = item.DEP_RELATIONSHIP == null ? "" : item.DEP_RELATIONSHIP.Trim(),
                            DepPolicyType = item.DEP_PAIDTYPE == 1 ? "Associate Paid" : "Company Paid",
                            PREVIOUS_DEP_DOB = item.PREVIOUS_DEP_DOB == null ? "" : item.PREVIOUS_DEP_DOB.Value.ToString("dd-MMM-yyyy").Trim(),
                            PREVIOUS_DEP_GENDER = item.PREVIOUS_DEP_GENDER != null ? (item.PREVIOUS_DEP_GENDER == 1 ? "Male" : "Female") : "",
                            PREVIOUS_DEP_NAME = item.PREVIOUS_DEP_NAME == null ? "" : item.PREVIOUS_DEP_NAME.Trim(),
                            PREVIOUS_DEP_RELATION = item.PREVIOUS_DEP_RELATIONSHIP == null ? "" : item.PREVIOUS_DEP_RELATIONSHIP.Trim(),
                            DepChangeType = item.CHANGETYPE.Value
                        });
                    }
                    MIVM.IsEmpExist = 1;
                    MIVM.Dependents = DepList;
                }
                else
                {
                    MIVM.IsEmpExist = 2; // Emplyee does not exist
                }
            }
            else
            {
                MIVM.IsEmpExist = 3; // You can add only Company Casual, Staff, Apprentice, Advisor.
            }
            return MIVM;
        }

        public String GetParameterValue(string strParmaName)
        {
            var SelectedParameter = (from v in _MedicalDBContext.SYPARAMETERS
                                     where v.PARAMNAME == strParmaName
                                     select v
                                  ).ToList();
            return (SelectedParameter.FirstOrDefault().PARAMVALUE);
        }
        #endregion

        public long[] GetDesgMappingData(long MAPPINGID)
        {
            //MAPPINGID = 0;
            return _MedicalDBContext.MEDINS_DESG_POLICYTYPE_MAP.Where(x => x.STAUS == 1 && x.MAPPINGID== MAPPINGID).Select(x => x.DESG_ID).ToArray();
        }
    }
}
