using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using ePortal.Shared.Services;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Operations;


namespace ePortal.Infrastructure.Repositories
{
    public class AssetDisposalRepository
    {
        private readonly EPortalDBContext _AssetDBContext;
        private readonly SYKI _Syki;

        public AssetDisposalRepository(EPortalDBContext assetDBContext)
        {
            _AssetDBContext = assetDBContext;
            _Syki = _AssetDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        #region Get Data Method
        public SYKI GetActiveSyki()
        {
            return (from data in _AssetDBContext.SYKI.Where(x => x.ACTIVE == 1)
                    select new SYKI
                    {
                        SYKIID = data.SYKIID,
                        KICODE = data.KICODE,
                        FINANCIALYEAR = data.FINANCIALYEAR
                    }).FirstOrDefault();
        }

        public List<SIS_ASSETTYPE_MST> GetAssetTypeList()
        {
            var iList = from data in _AssetDBContext.SIS_ASSETTYPE_MST
                        where data.ACTIVE == 1
                        select data;
            return iList.ToList();
        }

        public List<SIS_ASSETCONDITION> GetConditionList()
        {
            return _AssetDBContext.SIS_ASSETCONDITION.Where(data => data.ACTIVE == 1).ToList<SIS_ASSETCONDITION>();
        }

        public List<SYSITE> GetSiteList()
        {
            var iList = from data in _AssetDBContext.SYSITE
                        where data.ACTIVE == 1
                        select data;
            return iList.ToList();
        }

        public List<ADORGLEVEL> GetOrgLevelList()
        {
            var iList = from data in _AssetDBContext.ADORGLEVEL
                        where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID && data.ADORGLEVELTYPEID == 1
                        select data;
            return iList.ToList();
        }

        public long? GetOperationMappId(List<long?> _orgList)
        {
            long? mappIds = new long?();
            if (_orgList.Count > 0)
                mappIds = _AssetDBContext.SIS_AST_OPERATIONMAPP.Where(data => data.ACTIVE == 1 && _orgList.Contains(data.ORGID)).Select(data => data.OPERATIONID).FirstOrDefault();
            return mappIds;
        }

        public string GetFnCodeByOperationId(long operationId)
        {
            var obj = (from data in _AssetDBContext.SIS_AST_OPMAPPING_MST
                       where data.ACTIVE == 1 && data.ADORGLEVELID == operationId
                       select data.FUNCTIONAREACODE).FirstOrDefault();
            return obj;
        }
        //Add by Alok
        public long? GetexecoordinatorByOperationId(long operationId)
        {
            var obj = (from data in _AssetDBContext.ADORGCOORDINATOR
                       where data.ISACTIVE == 1 && data.ADORGLEVELID == operationId && data.ISSKIP == 0
                       select data.EXECOORDINATOR).FirstOrDefault();
            return obj;
        }

        public SYSITE GetSiteDetailBySiteId(long siteId)
        {
            var obj = (from data in _AssetDBContext.SYSITE
                       where data.ACTIVE == 1 && data.SYSITEID == siteId
                       select data).FirstOrDefault();
            return obj;
        }

        public string GetValidationParmValue(string parmName)
        {
            var obj = (from data in _AssetDBContext.SIS_AST_VALIDATION_MST
                       join _prmMst in _AssetDBContext.SIS_AST_PARAM_MST on data.PARAM_MSTID equals _prmMst.SIS_AST_PARAM_MSTID
                       where data.ACTIVE == 1 && _prmMst.PARAMNAME == parmName
                       select data.PARAMVALUE).FirstOrDefault();
            return obj;
        }

        public string CheckValidationByFnCode(string fnCode, string parmName)
        {
            var obj = (from data in _AssetDBContext.SIS_AST_VALIDATION_MST
                       join _prmMst in _AssetDBContext.SIS_AST_PARAM_MST on data.PARAM_MSTID equals _prmMst.SIS_AST_PARAM_MSTID
                       where data.ACTIVE == 1 && data.PARAMVALUE.Contains(fnCode) && _prmMst.PARAMNAME == parmName
                       select data.PARAMVALUE).FirstOrDefault();
            return obj;
        }

        public List<SIS_AST_PROCESSSTATUS_MST> GetProcessStatusList()
        {
            var iList = from data in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST
                        where data.ACTIVE == 1
                        select data;
            return iList.ToList();
        }

        public List<SIS_AST_PARAM_MST> GetParamList()
        {
            var iList = from data in _AssetDBContext.SIS_AST_PARAM_MST
                        where data.ACTIVE == 1
                        select data;
            return iList.ToList();
        }
        #endregion

        #region Approval Authority Master
        public List<ApprovalAuthorityViewModel> GetApprovalAuthorityList()
        {
            var iList = (from data in _AssetDBContext.SIS_AST_APPAUTH_MST
                             //join _site in _AssetDBContext.SYSITE on data.SYSITEID equals _site.SYSITEID
                         join _orgLevel in _AssetDBContext.ADORGLEVEL on data.ADORGLEVELID equals _orgLevel.ADORGLEVELID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.APPROVALTYPE equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.EMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         where _process.CATEGORY == 1
                         select new ApprovalAuthorityViewModel
                         {
                             APPAUTHID = data.SIS_AST_APPAUTH_MSTID,
                             ADORGLEVELID = data.ADORGLEVELID,
                             ORGLEVEL = _orgLevel.LEVELDESCRIP,
                             //SYSITEID = data.SYSITEID,
                             //SITE = _site.DESCRIP,
                             EMPCODE = data.EMPCODE,
                             EMPNAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             APPROVAL_STATUSID = data.APPROVALTYPE,
                             APPROVALTYPE = _process.STATUSDESCRIPTION,
                             STATUS_LEVEL = _process.STATUS_LEVEL,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD
                         });
            return iList.ToList();
        }

        public List<ApprovalAuthorityViewModel> GetApprovalAuthorityListByFinance()
        {
            var iList = (from data in _AssetDBContext.SIS_AST_APPAUTH_MST
                         join _site in _AssetDBContext.SYSITE on data.SYSITEID equals _site.SYSITEID
                         join _orgLevel in _AssetDBContext.ADORGLEVEL on data.ADORGLEVELID equals _orgLevel.ADORGLEVELID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.APPROVALTYPE equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.EMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         where _process.CATEGORY == 2
                         select new ApprovalAuthorityViewModel
                         {
                             APPAUTHID = data.SIS_AST_APPAUTH_MSTID,
                             ADORGLEVELID = data.ADORGLEVELID,
                             ORGLEVEL = _orgLevel.LEVELDESCRIP,
                             SYSITEID = data.SYSITEID,
                             SITE = _site.DESCRIP,
                             EMPCODE = data.EMPCODE,
                             EMPNAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             APPROVAL_STATUSID = data.APPROVALTYPE,
                             APPROVALTYPE = _process.STATUSDESCRIPTION,
                             STATUS_LEVEL = _process.STATUS_LEVEL,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD
                         });
            return iList.ToList();
        }

        public List<ApprovalAuthorityViewModel> GetApprovalAuthorityListByPPC()
        {
            var iList = (from data in _AssetDBContext.SIS_AST_APPAUTH_MST
                         join _site in _AssetDBContext.SYSITE on data.SYSITEID equals _site.SYSITEID
                         join _orgLevel in _AssetDBContext.ADORGLEVEL on data.ADORGLEVELID equals _orgLevel.ADORGLEVELID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.APPROVALTYPE equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.EMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         where _process.CATEGORY == 3
                         select new ApprovalAuthorityViewModel
                         {
                             APPAUTHID = data.SIS_AST_APPAUTH_MSTID,
                             ADORGLEVELID = data.ADORGLEVELID,
                             ORGLEVEL = _orgLevel.LEVELDESCRIP,
                             SYSITEID = data.SYSITEID,
                             SITE = _site.DESCRIP,
                             EMPCODE = data.EMPCODE,
                             EMPNAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             APPROVAL_STATUSID = data.APPROVALTYPE,
                             APPROVALTYPE = _process.STATUSDESCRIPTION,
                             STATUS_LEVEL = _process.STATUS_LEVEL,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD
                         });
            return iList.ToList();
        }

        public ApprovalAuthorityViewModel GetApprovalAuthorityById(long id)
        {
            var _obj = (from data in _AssetDBContext.SIS_AST_APPAUTH_MST.Where(x => x.SIS_AST_APPAUTH_MSTID == id)
                            //join _site in _AssetDBContext.SYSITE on data.SYSITEID equals _site.SYSITEID
                        join _orgLevel in _AssetDBContext.ADORGLEVEL on data.ADORGLEVELID equals _orgLevel.ADORGLEVELID
                        join _Emp in _AssetDBContext.ADEMPLOYEE on data.EMPCODE equals _Emp.ADEMPCODE
                        join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        select new ApprovalAuthorityViewModel
                        {
                            APPAUTHID = data.SIS_AST_APPAUTH_MSTID,
                            ADORGLEVELID = data.ADORGLEVELID,
                            ORGLEVEL = _orgLevel.LEVELDESCRIP,
                            SYSITEID = data.SYSITEID,
                            //SITE = _site.DESCRIP,
                            EMPCODE = data.EMPCODE,
                            EMPNAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                            APPROVAL_STATUSID = data.APPROVALTYPE,
                            //ACTIVE = data.ACTIVE == 1 ? true : false,
                            ACTIVE = Convert.ToBoolean(data.ACTIVE),
                            ADDEDBY = data.ADDEDBY,
                            ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            MODIFIEDBY = data.MODIFIEDBY,
                            DATELSTMOD = data.DATELSTMOD
                        }).FirstOrDefault();
            return _obj;
        }


        //Added by TTL :: CR6754
        public ApprovalAuthorityViewModel GetApprovalAuthorityByOperationAndSiteId(long OperationId, long SiteId, short Status_level)
        {
            var _obj = (from data in _AssetDBContext.SIS_AST_APPAUTH_MST
                        join _ProcesStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.APPROVALTYPE equals _ProcesStatus.SIS_AST_PROCESSSTATUS_MSTID
                        join _Emp in _AssetDBContext.ADEMPLOYEE on data.EMPCODE equals _Emp.ADEMPCODE
                        where _ProcesStatus.STATUS_LEVEL == (int)Status_level && data.ACTIVE == 1 && data.ADORGLEVELID == (long?)OperationId && data.SYSITEID == (long?)SiteId
                        select new ApprovalAuthorityViewModel
                        {
                            APPAUTHID = data.SIS_AST_APPAUTH_MSTID,
                            SYSITEID = data.SYSITEID,
                            ADORGLEVELID = data.ADORGLEVELID,
                            EMPCODE = data.EMPCODE,
                            EMPNAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " [" + _Emp.ADEMPCODE + "]",
                            EMPFNAME = _Emp.FIRSTNAME,
                            EMPLNAME = _Emp.LASTNAME,
                            EMPEMAIL = _Emp.EMAILID,
                            APPROVAL_STATUSID = data.APPROVALTYPE,
                            STATUS_LEVEL = _ProcesStatus.STATUS_LEVEL
                        }).FirstOrDefault();
            return _obj;
        }

        public bool IsPlantPpcMapped(long OperationId, long SiteId, long EmpCode)
        {
            var count = _AssetDBContext.SIS_AST_APPAUTH_MST
                .FirstOrDefault(x => x.APPROVALTYPE == 29 &&
                                          x.ACTIVE == 1 &&
                                          //x.EMPCODE == empCode &&
                                          x.SYSITEID == SiteId &&
                                          x.ADORGLEVELID == OperationId) == null ? false : true;

            return count;
        }

        public Employee_Details GetEmpDetailById(long empCode)
        {
            short syki = Convert.ToInt16(_AssetDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(s => s.SYKIID).FirstOrDefault());
            var _obj = (from data in _AssetDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                        where _VW.SYKI == syki
                        select new Employee_Details
                        {
                            _ECode = data.ADEMPCODE,
                            _EFirstName = data.FIRSTNAME,
                            _ELastName = data.LASTNAME,
                            _EName = data.FIRSTNAME + " " + data.LASTNAME + " [" + data.ADEMPCODE + "]",
                            _EmailId = data.EMAILID,
                            _DOB = data.DOB,
                            _OpId = _VW.OPERATIONID,
                            _SiteId = _VW.SYSITEID
                        }).FirstOrDefault();
            return _obj;
        }

        //End by TTL :: CR6754

        public short SaveApprovalAuthority(ApprovalAuthorityViewModel AAVM, System.String transactionType)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    SIS_AST_APPAUTH_MST SAAM = new SIS_AST_APPAUTH_MST();

                    int FlagAdd = 0;
                    if (AAVM.APPAUTHID > 0)
                    {                       

                        var isApprovalValidAUTHID = _AssetDBContext.SIS_AST_APPAUTH_MST.FirstOrDefault(x =>
                                    (AAVM.SYSITEID == null || x.SYSITEID == AAVM.SYSITEID) &&
                                    x.APPROVALTYPE == AAVM.APPROVAL_STATUSID &&
                                    x.SIS_AST_APPAUTH_MSTID != AAVM.APPAUTHID);

                        //Commented by TTL :: CR6754
                        //if (isApprovalValidAUTHID != null)
                        ////if (_AssetDBContext.SIS_AST_APPAUTH_MST.Any(x => (AAVM.SYSITEID == null ? 1 == 1 : x.SYSITEID == AAVM.SYSITEID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID && x.SIS_AST_APPAUTH_MSTID != AAVM.APPAUTHID))
                        //{
                        //    return retVal = 2;
                        //}
                        //SAAM = _AssetDBContext.SIS_AST_APPAUTH_MST.Where(x => x.SIS_AST_APPAUTH_MSTID == AAVM.APPAUTHID).SingleOrDefault();
                        //Added by TTL :: CR6754 - Start
                        if (AAVM.APPROVAL_STATUSID == 29)
                        {

                            bool isRecord = _AssetDBContext.SIS_AST_APPAUTH_MST.FirstOrDefault(x => (AAVM.SYSITEID == null || x.SYSITEID == AAVM.SYSITEID) && (AAVM.ADORGLEVELID == 0 || x.ADORGLEVELID == AAVM.ADORGLEVELID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID && x.SIS_AST_APPAUTH_MSTID != AAVM.APPAUTHID) == null ? false : true;

                            //if (_AssetDBContext.SIS_AST_APPAUTH_MST.Any(x => (AAVM.SYSITEID == null ? 1 == 1 : x.SYSITEID == AAVM.SYSITEID) && (AAVM.ADORGLEVELID == 0 ? 1 == 1 : x.ADORGLEVELID == AAVM.ADORGLEVELID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID && x.SIS_AST_APPAUTH_MSTID != AAVM.APPAUTHID))
                            if (isRecord)
                            {
                                return retVal = 2;
                            }
                            SAAM = _AssetDBContext.SIS_AST_APPAUTH_MST.Where(x => x.SIS_AST_APPAUTH_MSTID == AAVM.APPAUTHID).SingleOrDefault();
                        }
                        else
                        { //Added by TTL :: CR6754 - End
                            
                            bool isRecords = _AssetDBContext.SIS_AST_APPAUTH_MST.FirstOrDefault(x => (AAVM.SYSITEID == null ? 1 == 1 : x.SYSITEID == AAVM.SYSITEID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID && x.SIS_AST_APPAUTH_MSTID != AAVM.APPAUTHID) == null ? false : true;

                            //if (_AssetDBContext.SIS_AST_APPAUTH_MST.Any(x => (AAVM.SYSITEID == null ? 1 == 1 : x.SYSITEID == AAVM.SYSITEID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID && x.SIS_AST_APPAUTH_MSTID != AAVM.APPAUTHID))
                            if(isRecords)
                            {
                                return retVal = 2;
                            }
                            SAAM = _AssetDBContext.SIS_AST_APPAUTH_MST.Where(x => x.SIS_AST_APPAUTH_MSTID == AAVM.APPAUTHID).SingleOrDefault();
                        }
                        //End by TTL :: CR6754
                    }
                    else
                    {
                        
                        var isApprovalValid = _AssetDBContext.SIS_AST_APPAUTH_MST.FirstOrDefault(x =>
                                                (AAVM.SYSITEID == null || x.SYSITEID == AAVM.SYSITEID) &&
                                                x.APPROVALTYPE == AAVM.APPROVAL_STATUSID);

                        //if (_AssetDBContext.SIS_AST_APPAUTH_MST.Any(x => (AAVM.SYSITEID == null ? 1 == 1 : x.SYSITEID == AAVM.SYSITEID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID))
                        // Commented by TTL :: CR6754
                        //if (isApprovalValid != null)
                        //{
                        //    return retVal = 2;
                        //}
                        //Added by TTL :: CR6754
                        if (AAVM.APPROVAL_STATUSID == 29)
                        {
                            bool isValid = _AssetDBContext.SIS_AST_APPAUTH_MST.FirstOrDefault(x => (AAVM.SYSITEID == null ? 1 == 1 : x.SYSITEID == AAVM.SYSITEID) && (AAVM.ADORGLEVELID == 0 ? 1 == 1 : x.ADORGLEVELID == AAVM.ADORGLEVELID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID) == null ? false : true;

                            //if (_AssetDBContext.SIS_AST_APPAUTH_MST.Any(x => (AAVM.SYSITEID == null ? 1 == 1 : x.SYSITEID == AAVM.SYSITEID) && (AAVM.ADORGLEVELID == 0 ? 1 == 1 : x.ADORGLEVELID == AAVM.ADORGLEVELID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID))
                            if (isValid)
                            {
                                return retVal = 2;
                            }
                        } //End by TTL :: CR6754
                        else
                        {
                            bool isValid = _AssetDBContext.SIS_AST_APPAUTH_MST.FirstOrDefault(x => (AAVM.SYSITEID == null ? 1 == 1 : x.SYSITEID == AAVM.SYSITEID) && (AAVM.ADORGLEVELID == 0 ? 1 == 1 : x.ADORGLEVELID == AAVM.ADORGLEVELID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID) == null ? false : true;

                            //if (_AssetDBContext.SIS_AST_APPAUTH_MST.Any(x => (AAVM.SYSITEID == null ? 1 == 1 : x.SYSITEID == AAVM.SYSITEID) && x.APPROVALTYPE == AAVM.APPROVAL_STATUSID))
                            if(isValid)
                            {
                                return retVal = 2;
                            }
                        }
                        //End by TTL :: CR6754
                        SAAM = new SIS_AST_APPAUTH_MST();
                        if (_AssetDBContext.SIS_AST_APPAUTH_MST.ToList().Count == 0)
                        {
                            SAAM.SIS_AST_APPAUTH_MSTID = 1;
                        }
                        else
                        {
                            SAAM.SIS_AST_APPAUTH_MSTID = _AssetDBContext.SIS_AST_APPAUTH_MST.Max(x => x.SIS_AST_APPAUTH_MSTID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    SAAM.EMPCODE = AAVM.EMPCODE;
                    SAAM.SYSITEID = AAVM.SYSITEID;
                    if (AAVM.ADORGLEVELID != 0)
                    {
                        SAAM.ADORGLEVELID = AAVM.ADORGLEVELID;
                    }
                    SAAM.APPROVALTYPE = Convert.ToInt16(AAVM.APPROVAL_STATUSID);
                    SAAM.ACTIVE = Convert.ToInt16(AAVM.ACTIVE == true ? 1 : 0);
                    if (FlagAdd == 1)
                    {
                        SAAM.ADDEDBY = AAVM.ADDEDBY;
                        SAAM.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        SAAM.MODIFIEDBY = AAVM.MODIFIEDBY;
                        SAAM.DATELSTMOD = DateTime.Now;
                    }
                    _AssetDBContext.Entry(SAAM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _AssetDBContext.SaveChanges();
                    transaction.Commit();
                    retVal = 1;
                    SaveAssetApprovalAuthorityLog(AAVM.ADORGLEVELID, AAVM.EMPCODE, SAAM.ACTIVE, AAVM.ADDEDBY, Convert.ToInt32(SAAM.SYSITEID), SAAM.APPROVALTYPE, transactionType, Convert.ToInt32(SAAM.SIS_AST_APPAUTH_MSTID));
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }

                return retVal;
            }
        }
        public void SaveAssetApprovalAuthorityLog(long orgLevelID, long empCode, long active, long updatedBy, long sySiteID, long approvalType, System.String transactionType, long appAuthMSTKey)
        {
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    ASSETAPP_LOG AST = new ASSETAPP_LOG();

                    AST.ASTAPPROVALLOGID = _AssetDBContext.ASSETAPP_LOG.Max(x => x.ASTAPPROVALLOGID) + 1;

                    AST.EMPCODE = Convert.ToString(empCode);
                    AST.SYSITEID = Convert.ToString(sySiteID);
                    AST.ADORGLEVELID = orgLevelID;
                    AST.ACTIVE = Convert.ToString(active);
                    AST.UPDATEDBY = Convert.ToString(updatedBy);
                    AST.APPROVALTYPE = Convert.ToString(approvalType);
                    AST.APPROVALMASTERKEY = appAuthMSTKey;
                    AST.TRANSACTIONTYPE = Convert.ToString(transactionType);
                    AST.UPDATEDON = System.DateTime.Now;
                    _AssetDBContext.ASSETAPP_LOG.Add(AST);
                    _AssetDBContext.Entry(AST).State = EntityState.Added;
                    _AssetDBContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }


            public List<ApprovalAuthorityViewModel> GetApprovalAuthorityByEmpCode(long EmpCode, long SiteId)
        {
            var _obj = (from data in _AssetDBContext.SIS_AST_APPAUTH_MST
                        join _ProcesStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.APPROVALTYPE equals _ProcesStatus.SIS_AST_PROCESSSTATUS_MSTID
                        where data.EMPCODE == EmpCode && (SiteId == -1L ? true : data.SYSITEID == SiteId) && data.ACTIVE == 1
                        select new ApprovalAuthorityViewModel
                        {
                            APPAUTHID = data.SIS_AST_APPAUTH_MSTID,
                            SYSITEID = data.SYSITEID,
                            EMPCODE = data.EMPCODE,
                            APPROVAL_STATUSID = data.APPROVALTYPE,
                            PROCESS_STATUSID = _ProcesStatus.SIS_AST_PROCESSSTATUS_MSTID,
                            STATUS_LEVEL = _ProcesStatus.STATUS_LEVEL
                        }).ToList();
            return _obj;
        }

        public ApprovalAuthorityViewModel GetApprovalAuthorityBySiteId(long SiteId, short Status_level)
        {

            var _obj = (from data in _AssetDBContext.SIS_AST_APPAUTH_MST
                        join _ProcesStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.APPROVALTYPE equals _ProcesStatus.SIS_AST_PROCESSSTATUS_MSTID
                        join _Emp in _AssetDBContext.ADEMPLOYEE on data.EMPCODE equals _Emp.ADEMPCODE
                        where _ProcesStatus.STATUS_LEVEL == (int)Status_level && data.ACTIVE == 1 &&
                        (_ProcesStatus.STATUS_LEVEL == 10 || _ProcesStatus.STATUS_LEVEL == 11 
                        || _ProcesStatus.STATUS_LEVEL == 12 || data.SYSITEID == (long?)SiteId)
                        //(_ProcesStatus.STATUS_LEVEL == 10 || _ProcesStatus.STATUS_LEVEL == 11
                        //|| _ProcesStatus.STATUS_LEVEL == 12 ? 1 == 1 : data.SYSITEID == (long?)SiteId) //// --- CPO, IC, IBM not check site condition
                        select new ApprovalAuthorityViewModel
                        {
                            APPAUTHID = data.SIS_AST_APPAUTH_MSTID,
                            SYSITEID = data.SYSITEID,
                            EMPCODE = data.EMPCODE,
                            EMPNAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " [" + _Emp.ADEMPCODE + "]",
                            EMPFNAME = _Emp.FIRSTNAME,
                            EMPLNAME = _Emp.LASTNAME,
                            EMPEMAIL = _Emp.EMAILID,
                            APPROVAL_STATUSID = data.APPROVALTYPE,
                            STATUS_LEVEL = _ProcesStatus.STATUS_LEVEL
                        }).FirstOrDefault();
            return _obj;
        }
        #endregion

        #region Operation Mapping Master
        public List<OPMappingViewModel> GetOperationMappingList()
        {
            var iList = (from data in _AssetDBContext.SIS_AST_OPMAPPING_MST
                         join _orgLevel in _AssetDBContext.ADORGLEVEL on data.ADORGLEVELID equals _orgLevel.ADORGLEVELID
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         select new OPMappingViewModel
                         {
                             OPMAPPINGID = data.SIS_AST_OPMAPPING_MSTID,
                             ADORGLEVELID = data.ADORGLEVELID,
                             ORGLEVEL = _orgLevel.LEVELDESCRIP,
                             FUNCTIONAREACODE = data.FUNCTIONAREACODE,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD
                         });
            return iList.ToList();
        }

        public OPMappingViewModel GetOperationMappingById(long id)
        {
            var _obj = (from data in _AssetDBContext.SIS_AST_OPMAPPING_MST.Where(x => x.SIS_AST_OPMAPPING_MSTID == id)
                        join _orgLevel in _AssetDBContext.ADORGLEVEL on data.ADORGLEVELID equals _orgLevel.ADORGLEVELID
                        join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        select new OPMappingViewModel
                        {
                            OPMAPPINGID = data.SIS_AST_OPMAPPING_MSTID,
                            ADORGLEVELID = data.ADORGLEVELID,
                            ORGLEVEL = _orgLevel.LEVELDESCRIP,
                            FUNCTIONAREACODE = data.FUNCTIONAREACODE,
                            //ACTIVE = data.ACTIVE == 1 ? true : false,
                            ACTIVE = Convert.ToBoolean(data.ACTIVE),
                            ADDEDBY = data.ADDEDBY,
                            ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            MODIFIEDBY = data.MODIFIEDBY,
                            DATELSTMOD = data.DATELSTMOD
                        }).FirstOrDefault();
            return _obj;
        }

        public short SaveOperationMapping(OPMappingViewModel OMVM)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    SIS_AST_OPMAPPING_MST SAOM = new SIS_AST_OPMAPPING_MST();
                    int FlagAdd = 0;
                    if (OMVM.OPMAPPINGID > 0)
                    {

                        var flagOpMappingId = _AssetDBContext.SIS_AST_OPMAPPING_MST.FirstOrDefault(x => x.ADORGLEVELID == OMVM.ADORGLEVELID && x.FUNCTIONAREACODE == OMVM.FUNCTIONAREACODE && x.SIS_AST_OPMAPPING_MSTID != OMVM.OPMAPPINGID);

                        //if (_AssetDBContext.SIS_AST_OPMAPPING_MST.Any(x => x.ADORGLEVELID == OMVM.ADORGLEVELID && x.FUNCTIONAREACODE == OMVM.FUNCTIONAREACODE && x.SIS_AST_OPMAPPING_MSTID != OMVM.OPMAPPINGID))
                        if (flagOpMappingId != null)
                        {
                            return retVal = 2;
                        }
                        SAOM = _AssetDBContext.SIS_AST_OPMAPPING_MST.Where(x => x.SIS_AST_OPMAPPING_MSTID == OMVM.OPMAPPINGID).SingleOrDefault();
                    }
                    else
                    {
                        var opMapping = _AssetDBContext.SIS_AST_OPMAPPING_MST.FirstOrDefault(x => x.ADORGLEVELID == OMVM.ADORGLEVELID && x.FUNCTIONAREACODE == OMVM.FUNCTIONAREACODE);
                        //if (_AssetDBContext.SIS_AST_OPMAPPING_MST.Any(x => x.ADORGLEVELID == OMVM.ADORGLEVELID && x.FUNCTIONAREACODE == OMVM.FUNCTIONAREACODE))
                        if(opMapping != null)
                        {
                            return retVal = 2;
                        }

                        SAOM = new SIS_AST_OPMAPPING_MST();
                        if (_AssetDBContext.SIS_AST_OPMAPPING_MST.ToList().Count == 0)
                        {
                            SAOM.SIS_AST_OPMAPPING_MSTID = 1;
                        }
                        else
                        {
                            SAOM.SIS_AST_OPMAPPING_MSTID = _AssetDBContext.SIS_AST_OPMAPPING_MST.Max(x => x.SIS_AST_OPMAPPING_MSTID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    SAOM.FUNCTIONAREACODE = OMVM.FUNCTIONAREACODE.Trim();
                    SAOM.ADORGLEVELID = OMVM.ADORGLEVELID;
                    SAOM.ACTIVE = Convert.ToInt16(OMVM.ACTIVE == true ? 1 : 0);
                    if (FlagAdd == 1)
                    {
                        SAOM.ADDEDBY = OMVM.ADDEDBY;
                        SAOM.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        SAOM.MODIFIEDBY = OMVM.MODIFIEDBY;
                        SAOM.DATELSTMOD = DateTime.Now;
                    }
                    _AssetDBContext.Entry(SAOM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _AssetDBContext.SaveChanges();
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
        #endregion

        #region Validation Master
        public List<ValidationViewModel> GetValidationList()
        {
            var iList = (from data in _AssetDBContext.SIS_AST_VALIDATION_MST
                         join _parm in _AssetDBContext.SIS_AST_PARAM_MST on data.PARAM_MSTID equals _parm.SIS_AST_PARAM_MSTID
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         select new ValidationViewModel
                         {
                             VALIDATIONID = data.SIS_AST_VALIDATION_MSTTID,
                             PARAM_MSTID = data.PARAM_MSTID,
                             Parm_mst = new ParamMstViewModel
                             {
                                 PARAMNAME = _parm.PARAMNAME,
                                 PARAMDESCRIPTION = _parm.PARAMDESCRIPTION,
                                 PARAMVALUETYPE = _parm.PARAMVALUETYPE,
                                 PARAMTYPE = _parm.PARAMTYPE,
                             },
                             PARAMVALUE = data.PARAMVALUE,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD
                         });
            return iList.ToList();
        }

        public ValidationViewModel GetValidationById(long id)
        {
            var _obj = (from data in _AssetDBContext.SIS_AST_VALIDATION_MST.Where(x => x.SIS_AST_VALIDATION_MSTTID == id)
                        join _parm in _AssetDBContext.SIS_AST_PARAM_MST on data.PARAM_MSTID equals _parm.SIS_AST_PARAM_MSTID
                        join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        select new ValidationViewModel
                        {
                            VALIDATIONID = data.SIS_AST_VALIDATION_MSTTID,
                            PARAM_MSTID = data.PARAM_MSTID,
                            Parm_mst = new ParamMstViewModel
                            {
                                PARAMNAME = _parm.PARAMNAME,
                                PARAMDESCRIPTION = _parm.PARAMDESCRIPTION,
                                PARAMVALUETYPE = _parm.PARAMVALUETYPE,
                                PARAMTYPE = _parm.PARAMTYPE,
                            },
                            PARAMVALUE = data.PARAMVALUE,
                            //ACTIVE = data.ACTIVE == 1 ? true : false,
                            ACTIVE = Convert.ToBoolean(data.ACTIVE),
                            ADDEDBY = data.ADDEDBY,
                            ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            MODIFIEDBY = data.MODIFIEDBY,
                            DATELSTMOD = data.DATELSTMOD
                        }).FirstOrDefault();
            return _obj;
        }

        public short SaveValidation(ValidationViewModel VVM)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    SIS_AST_VALIDATION_MST SVM = new SIS_AST_VALIDATION_MST();
                    int FlagAdd = 0;
                    if (VVM.VALIDATIONID > 0)
                    {
                        var validation = _AssetDBContext.SIS_AST_VALIDATION_MST.FirstOrDefault(x => x.PARAM_MSTID == VVM.PARAM_MSTID && x.SIS_AST_VALIDATION_MSTTID != VVM.VALIDATIONID);

                        if (validation != null)
                        //if (_AssetDBContext.SIS_AST_VALIDATION_MST.Any(x => x.PARAM_MSTID == VVM.PARAM_MSTID && x.SIS_AST_VALIDATION_MSTTID != VVM.VALIDATIONID))
                        {
                            return retVal = 2;
                        }
                        SVM = _AssetDBContext.SIS_AST_VALIDATION_MST.Where(x => x.SIS_AST_VALIDATION_MSTTID == VVM.VALIDATIONID).SingleOrDefault();
                    }
                    else
                    {
                        var validation = _AssetDBContext.SIS_AST_VALIDATION_MST.FirstOrDefault(x => x.PARAM_MSTID == VVM.PARAM_MSTID);

                        if (validation != null)
                        //if (_AssetDBContext.SIS_AST_VALIDATION_MST.Any(x => x.PARAM_MSTID == VVM.PARAM_MSTID))
                        {
                            return retVal = 2;
                        }
                        SVM = new SIS_AST_VALIDATION_MST();
                        if (_AssetDBContext.SIS_AST_VALIDATION_MST.ToList().Count == 0)
                        {
                            SVM.SIS_AST_VALIDATION_MSTTID = 1;
                        }
                        else
                        {
                            SVM.SIS_AST_VALIDATION_MSTTID = _AssetDBContext.SIS_AST_VALIDATION_MST.Max(x => x.SIS_AST_VALIDATION_MSTTID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    SVM.PARAM_MSTID = VVM.PARAM_MSTID;
                    SVM.PARAMVALUE = VVM.PARAMVALUE;
                    SVM.ACTIVE = Convert.ToInt16(VVM.ACTIVE == true ? 1 : 0);
                    if (FlagAdd == 1)
                    {
                        SVM.ADDEDBY = VVM.ADDEDBY;
                        SVM.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        SVM.MODIFIEDBY = VVM.MODIFIEDBY;
                        SVM.DATELSTMOD = DateTime.Now;
                    }
                    _AssetDBContext.Entry(SVM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _AssetDBContext.SaveChanges();
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
        #endregion

        #region Operation Master
        public List<AssetOperationViewModel> GetOperationList()
        {
            var iList = (from data in _AssetDBContext.SIS_AST_OPERATION
                         join _orgLevel in _AssetDBContext.ADORGLEVEL on data.OPERATIONID equals _orgLevel.ADORGLEVELID
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         select new AssetOperationViewModel
                         {
                             ASSETOPID = data.ASSETOPID,
                             OPERATIONID = data.OPERATIONID,
                             OPERATION = _orgLevel.LEVELDESCRIP,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD
                         });
            return iList.ToList();
        }

        public AssetOperationViewModel GetOperationDtlById(long id)
        {
            var _obj = (from data in _AssetDBContext.SIS_AST_OPERATION.Where(x => x.ASSETOPID == id)
                        join _orgLevel in _AssetDBContext.ADORGLEVEL on data.OPERATIONID equals _orgLevel.ADORGLEVELID
                        join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        select new AssetOperationViewModel
                        {
                            ASSETOPID = data.ASSETOPID,
                            OPERATIONID = data.OPERATIONID,
                            OPERATION = _orgLevel.LEVELDESCRIP,
                            //ACTIVE = data.ACTIVE == 1 ? true : false,
                            ACTIVE = Convert.ToBoolean(data.ACTIVE),
                            ADDEDBY = data.ADDEDBY,
                            ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            MODIFIEDBY = data.MODIFIEDBY,
                            DATELSTMOD = data.DATELSTMOD
                        }).FirstOrDefault();
            return _obj;
        }

        public short SaveOperation(AssetOperationViewModel AOVM)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    SIS_AST_OPERATION SAO = new SIS_AST_OPERATION();
                    int FlagAdd = 0;
                    if (AOVM.ASSETOPID > 0)
                    {

                        var assetOpId = _AssetDBContext.SIS_AST_OPERATION.FirstOrDefault(x => x.OPERATIONID == AOVM.OPERATIONID && x.ASSETOPID != AOVM.ASSETOPID);

                        if (assetOpId != null && Convert.ToInt64(assetOpId) != 0)
                        //if (_AssetDBContext.SIS_AST_OPERATION.Any(x => x.OPERATIONID == AOVM.OPERATIONID && x.ASSETOPID != AOVM.ASSETOPID))
                        {
                            return retVal = 2;
                        }
                        SAO = _AssetDBContext.SIS_AST_OPERATION.Where(x => x.ASSETOPID == AOVM.ASSETOPID).SingleOrDefault();
                    }
                    else
                    {
                        var operation = _AssetDBContext.SIS_AST_OPERATION.FirstOrDefault(x => x.OPERATIONID == AOVM.OPERATIONID);

                        if (operation != null && Convert.ToInt64(operation) != 0)                        
                        //if (_AssetDBContext.SIS_AST_OPERATION.Any(x => x.OPERATIONID == AOVM.OPERATIONID))
                        {
                            return retVal = 2;
                        }

                        SAO = new SIS_AST_OPERATION();
                        if (_AssetDBContext.SIS_AST_OPERATION.ToList().Count == 0)
                        {
                            SAO.ASSETOPID = 1;
                        }
                        else
                        {
                            SAO.ASSETOPID = _AssetDBContext.SIS_AST_OPERATION.Max(x => x.ASSETOPID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    SAO.OPERATIONID = AOVM.OPERATIONID;
                    SAO.ACTIVE = Convert.ToInt16(AOVM.ACTIVE == true ? 1 : 0);
                    if (FlagAdd == 1)
                    {
                        SAO.ADDEDBY = AOVM.ADDEDBY;
                        SAO.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        SAO.MODIFIEDBY = AOVM.MODIFIEDBY;
                        SAO.DATELSTMOD = DateTime.Now;
                    }
                    _AssetDBContext.Entry(SAO).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _AssetDBContext.SaveChanges();
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

        public int GetOperationCount(long _opId)
        {
            return _AssetDBContext.SIS_AST_OPERATION.Count(c => c.OPERATIONID == _opId && c.ACTIVE == 1);
        }
        #endregion

        #region Asset Disposal Request
        public List<AssetDisposalViewModel> GetAssetRequestList(long loginUser)
        {
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         where /*data.ACTIVE == 1 &&*/ data.ADEMPCODE == loginUser
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             STATUS_LEVEL = _process.STATUS_LEVEL,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                           
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public AssetDisposalViewModel GetDetailById(long id)
        {
            var _obj = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == id)
                        join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                        join _customerJoin in _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL on data.DISPOSALHEADERID equals _customerJoin.DISPOSALHEADERID into _customerJoin
                        from _customerDetail in _customerJoin.DefaultIfEmpty()
                        join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                        join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                        join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                        where _VWAssociate.SYKI == _Syki.SYKIID
                        select new AssetDisposalViewModel
                        {
                            DISPOSALHEADERID = data.DISPOSALHEADERID,
                            ADEMPCODE = data.ADEMPCODE,
                            EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                            ASSETTYPEID = data.ASSETTYPEID,
                            ASSETTYPE = _assetMst.DESCRIPTION,
                            PROCESSSTATUSID = data.PROCESSSTATUSID,
                            PROCESSSTATUS = _process.STATUSDESCRIPTION,
                            STATUS_LEVEL = _process.STATUS_LEVEL,
                            BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                            TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                            MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                            INVOICE_ATTACHMENT = data.INVOICE_ATTACHMENT,
                            REMARKS = data.REMARKS,
                            //ACTIVE = data.ACTIVE == 1 ? true : false,
                            ACTIVE = Convert.ToBoolean(data.ACTIVE),
                            ADDEDBY = data.ADDEDBY,
                            ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            MODIFIEDBY = data.MODIFIEDBY,
                            DATELSTMOD = data.DATELSTMOD,
                            IsPlantRequired = data.ISPLANTREQUIRED,
                            P1FAppStatus = data.P1F_APPSTATUS,
                            P2FAppStatus = data.P2F_APPSTATUS,
                            P3FAppStatus = data.P3F_APPSTATUS,
                            P4FAppStatus = data.P4F_APPSTATUS,
                            P1F_ATTACHMENT_NAME = data.P1F_ATTACHMENT,
                            P2F_ATTACHMENT_NAME = data.P2F_ATTACHMENT,
                            P3F_ATTACHMENT_NAME = data.P3F_ATTACHMENT,
                            P4F_ATTACHMENT_NAME = data.P4F_ATTACHMENT,
                            PLANTID = data.PLANTID,
                            //------------------START-[Code Added by Aumento on 14-Jan-2024]------------------------------
                            ICMONTH = data.ICMONTH,
                            IMPLEMENTMONTH = data.IMPLEMENTMONTH,
                            //------------------END-[Code Added by Aumento on 14-Jan-2024]--------------------------------
                            
                            Emp_Detail = new Employee_Details
                            {
                                _ECode = _Emp.ADEMPCODE,
                                _EName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
                                _DOB = DateTime.Now,
                                _SecDescrip = _VWAssociate.SECTION,
                                _DepDesc = _VWAssociate.DEPARTMENT,
                                _DivDesc = _VWAssociate.DIVISION,
                                _OpDesc = _VWAssociate.OPERATION,
                                _SiteId = _VWAssociate.SYSITEID,
                                _SecId = _VWAssociate.SECTIONID,
                                _DepId = _VWAssociate.DEPARTMENTID,
                                _DivId = _VWAssociate.DIVISIONID,
                                _OpId = _VWAssociate.OPERATIONID,
                            },                           
                        }).FirstOrDefault();

            // Fetch DisposalDetailList separately
            if (_obj != null)
            {
                _obj.CustomerDetail =
                       (from _customerJoin in _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL
                        where _customerJoin.DISPOSALHEADERID == _obj.DISPOSALHEADERID
                        select new DisposalCustomerViewModel
                        {
                            DISPOSALCUSTDETAILID = _customerJoin.DISPOSALCUSTDETAILID,
                            DISPOSALHEADERID = _customerJoin.DISPOSALHEADERID,
                            CUSTOMERCODE = _customerJoin.CUSTOMERCODE,
                            CUSTOMERNAME = _customerJoin.CUSTOMERNAME,
                            CUSTADDRESS = _customerJoin.CUSTADDRESS,
                            CUSTGSTIN = _customerJoin.CUSTGSTIN,
                            GSTRATE = _customerJoin.GSTRATE,
                            BASICVALUE = _customerJoin.BASICVALUE,
                            PAYMENTDETAILS = _customerJoin.PAYMENTDETAILS,
                            ACTIVE = Convert.ToBoolean(_customerJoin.ACTIVE)
                        }).FirstOrDefault();
                _obj.DisposalDetailList = (from _disposalDetail in _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == _obj.DISPOSALHEADERID && x.ACTIVE == 1 /*&& x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1*/)
                                           join _ConditionJoin in _AssetDBContext.SIS_ASSETCONDITION on _disposalDetail.CONDITIONID equals _ConditionJoin.CONDITIONID into _ConditionJoin
                                           from _ConditionMst in _ConditionJoin.DefaultIfEmpty()
                                           join _siteJoin in _AssetDBContext.SYSITE on _disposalDetail.PLANTID equals _siteJoin.SYSITEID into _site_Join
                                           from _siteApp in _site_Join.DefaultIfEmpty()
                                           select new DisposalDetailViewModel
                                           {
                                               DISPOSALDETAILID = _disposalDetail.DISPOSALDETAILID,
                                               DISPOSALHEADERID = _disposalDetail.DISPOSALHEADERID,
                                               ASSETCODE = _disposalDetail.ASSETCODE,
                                               ASSETDESCRIPTION = _disposalDetail.ASSETDESCRIPTION,
                                               VENDORCODE = _disposalDetail.VENDORCODE,
                                               VENDORNAME = _disposalDetail.VENDORNAME,
                                               INVOICENO = _disposalDetail.INVOICENO,
                                               INVOICEDATE = _disposalDetail.INVOICEDATE,
                                               //BILLENTRY = string.IsNullOrEmpty(_disposalDetail.BILLENTRY) ? "N/A" : _disposalDetail.BILLENTRY,
                                               //LICENSENO = string.IsNullOrEmpty(_disposalDetail.LICENSENO) ? "N/A" : _disposalDetail.LICENSENO,
                                               BILLENTRY = _disposalDetail.BILLENTRY ?? "",
                                               LICENSENO = _disposalDetail.LICENSENO ?? "",
                                               PURCHASEDDATE = _disposalDetail.PURCHASEDDATE,
                                               ORIGINALCOST = _disposalDetail.ORIGINALCOST,
                                               DEPRECIATIONCOST = _disposalDetail.DEPRECIATIONCOST,
                                               SALESPRICE = _disposalDetail.SALESPRICE,
                                               MGF_QLTYSTATUS = _disposalDetail.MGF_QLTYSTATUS,
                                               EVNSTATUS = _disposalDetail.EVNSTATUS,
                                               TAXTION_STATUS = _disposalDetail.TAXTION_STATUS,
                                               MAINTENANCEDEPT_STATUS = _disposalDetail.MAINTENANCEDEPT_STATUS,
                                               IC_STATUS = _disposalDetail.IC_STATUS,
                                               IBM_STATUS = _disposalDetail.IBM_STATUS,
                                               //ACTIVE = _disposalDetail.ACTIVE == 1 ? true : false,
                                               ACTIVE = Convert.ToBoolean(_disposalDetail.ACTIVE),
                                               WDV = (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
                                               PROFIT_LOSS = (_disposalDetail.SALESPRICE == null ? 0.00M : _disposalDetail.SALESPRICE) - (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
                                               VALUATION_GST = _disposalDetail.VALUATION_GST,
                                               TENTATIVE = _disposalDetail.TENTATIVE,
                                               PARTIAL_FULL = _disposalDetail.PARTIAL_FULL,
                                               VIBRATION_SENSOR = _disposalDetail.VIBRATION_SENSOR,
                                               ASSET_CATEGORY = _disposalDetail.ASSET_CATEGORY,
                                               CONDITIONID = _disposalDetail.CONDITIONID,
                                               ASSET_CONDITION = _ConditionMst.DESCRIPTION,
                                               SERIALNUMBER = _disposalDetail.SERIALNUMBER,
                                               PONUMBER = _disposalDetail.PONUMBER,
                                               PLANTID = _disposalDetail.PLANTID,
                                               //PLANTNAME = _siteApp == null ? "" : _siteApp.DESCRIP,
                                               PLANTNAME = _siteApp.DESCRIP ?? "",
                                               CAPTALIZED_DATE = _disposalDetail.CAPTALIZED_DATE,
                                           }).ToList();

                _obj.AppHistoryList = (from _disposalAppHis in _AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.Where(x => x.DISPOSALHEADERID == _obj.DISPOSALHEADERID)
                                       join _processStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on _disposalAppHis.PROCESSSTATUSID equals _processStatus.SIS_AST_PROCESSSTATUS_MSTID
                                       join _AppEmp in _AssetDBContext.ADEMPLOYEE on _disposalAppHis.APPCODE equals _AppEmp.ADEMPCODE
                                       select new ApprovalHisViewModel
                                       {
                                           DISPOSALHEADERID = _disposalAppHis.DISPOSALHEADERID,
                                           PROCESSSTATUSID = _disposalAppHis.PROCESSSTATUSID,
                                           APPROVALHISTORYID = _disposalAppHis.APPROVALHISTORYID,
                                           APPROVALSTATUS = _disposalAppHis.APPROVALSTATUS,
                                           STATUS_LEVEL = _processStatus.STATUS_LEVEL,
                                           ARS_MSG = ((_processStatus.STATUS_LEVEL == 15 || _processStatus.STATUS_LEVEL == 16 || _processStatus.STATUS_LEVEL == 17 || _processStatus.STATUS_LEVEL == 25) ? "Uploaded" : (_disposalAppHis.APPROVALSTATUS == 1 ? "Approved By" : _disposalAppHis.APPROVALSTATUS == 2 ? "Sendback By" : _disposalAppHis.APPROVALSTATUS == 3 ? "Rejected By" : "")),
                                           DISPLAY_MSG = _processStatus.DISPLAY_MSG,
                                           ATTACHMENT = _disposalAppHis.ATTACHMENT,
                                           APPCODE = _disposalAppHis.APPCODE,
                                           APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                                           REMARKS = _disposalAppHis.REMARKS,
                                           DATEADDED = _disposalAppHis.DATEADDED,
                                           ADDEDBY = _disposalAppHis.ADDEDBY,
                                           SEQ_NUMBER = _processStatus.SEQ_NUMBER
                                       }).OrderBy(o => o.DATEADDED).ToList();
            }

            //var _obj = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == id)
            //            join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
            //            join _customerJoin in _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL on data.DISPOSALHEADERID equals _customerJoin.DISPOSALHEADERID into _customerJoin
            //            from _customerDetail in _customerJoin.DefaultIfEmpty()
            //            join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
            //            join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
            //            join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //            join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
            //            where _VWAssociate.SYKI == _Syki.SYKIID
            //            select new AssetDisposalViewModel
            //            {
            //                DISPOSALHEADERID = data.DISPOSALHEADERID,
            //                ADEMPCODE = data.ADEMPCODE,
            //                EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
            //                ASSETTYPEID = data.ASSETTYPEID,
            //                ASSETTYPE = _assetMst.DESCRIPTION,
            //                PROCESSSTATUSID = data.PROCESSSTATUSID,
            //                PROCESSSTATUS = _process.STATUSDESCRIPTION,
            //                STATUS_LEVEL = _process.STATUS_LEVEL,
            //                BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
            //                TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
            //                MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
            //                INVOICE_ATTACHMENT = data.INVOICE_ATTACHMENT,
            //                REMARKS = data.REMARKS,
            //                //ACTIVE = data.ACTIVE == 1 ? true : false,
            //                ACTIVE = Convert.ToBoolean(data.ACTIVE),
            //                ADDEDBY = data.ADDEDBY,
            //                ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                DATEADDED = data.DATEADDED,
            //                MODIFIEDBY = data.MODIFIEDBY,
            //                DATELSTMOD = data.DATELSTMOD,
            //                IsPlantRequired = data.ISPLANTREQUIRED,
            //                P1FAppStatus = data.P1F_APPSTATUS,
            //                P2FAppStatus = data.P2F_APPSTATUS,
            //                P3FAppStatus = data.P3F_APPSTATUS,
            //                P4FAppStatus = data.P4F_APPSTATUS,
            //                P1F_ATTACHMENT_NAME = data.P1F_ATTACHMENT,
            //                P2F_ATTACHMENT_NAME = data.P2F_ATTACHMENT,
            //                P3F_ATTACHMENT_NAME = data.P3F_ATTACHMENT,
            //                P4F_ATTACHMENT_NAME = data.P4F_ATTACHMENT,
            //                PLANTID = data.PLANTID,
            //                Emp_Detail = new Employee_Details
            //                {
            //                    _ECode = _Emp.ADEMPCODE,
            //                    _EName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
            //                    _DOB = DateTime.Now,
            //                    _SecDescrip = _VWAssociate.SECTION,
            //                    _DepDesc = _VWAssociate.DEPARTMENT,
            //                    _DivDesc = _VWAssociate.DIVISION,
            //                    _OpDesc = _VWAssociate.OPERATION,
            //                    _SiteId = _VWAssociate.SYSITEID,
            //                    _SecId = _VWAssociate.SECTIONID,
            //                    _DepId = _VWAssociate.DEPARTMENTID,
            //                    _DivId = _VWAssociate.DIVISIONID,
            //                    _OpId = _VWAssociate.OPERATIONID,
            //                },
            //                CustomerDetail = _customerDetail == null ? null : new DisposalCustomerViewModel
            //                {
            //                    //DISPOSALCUSTDETAILID = _customerDetail.DISPOSALCUSTDETAILID,
            //                    //DISPOSALHEADERID = _customerDetail.DISPOSALHEADERID,
            //                    //CUSTOMERCODE = _customerDetail.CUSTOMERCODE,
            //                    //CUSTOMERNAME = _customerDetail.CUSTOMERNAME,
            //                    //CUSTADDRESS = _customerDetail.CUSTADDRESS,
            //                    //CUSTGSTIN = _customerDetail.CUSTGSTIN,
            //                    //GSTRATE = _customerDetail.GSTRATE,
            //                    //BASICVALUE = _customerDetail.BASICVALUE,
            //                    //PAYMENTDETAILS = _customerDetail.PAYMENTDETAILS,
            //                    ////ACTIVE = _customerDetail.ACTIVE == 1 ? true : false,
            //                    //ACTIVE = Convert.ToBoolean(_customerDetail.ACTIVE),
            //                },
            //                DisposalDetailList = (from _disposalDetail in _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 /*&& x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1*/)
            //                                      join _ConditionJoin in _AssetDBContext.SIS_ASSETCONDITION on _disposalDetail.CONDITIONID equals _ConditionJoin.CONDITIONID into _ConditionJoin
            //                                      from _ConditionMst in _ConditionJoin.DefaultIfEmpty()
            //                                      join _siteJoin in _AssetDBContext.SYSITE on _disposalDetail.PLANTID equals _siteJoin.SYSITEID into _site_Join
            //                                      from _siteApp in _site_Join.DefaultIfEmpty()
            //                                      select new DisposalDetailViewModel
            //                                      {
            //                                          DISPOSALDETAILID = _disposalDetail.DISPOSALDETAILID,
            //                                          DISPOSALHEADERID = _disposalDetail.DISPOSALHEADERID,
            //                                          ASSETCODE = _disposalDetail.ASSETCODE,
            //                                          ASSETDESCRIPTION = _disposalDetail.ASSETDESCRIPTION,
            //                                          VENDORCODE = _disposalDetail.VENDORCODE,
            //                                          VENDORNAME = _disposalDetail.VENDORNAME,
            //                                          INVOICENO = _disposalDetail.INVOICENO,
            //                                          INVOICEDATE = _disposalDetail.INVOICEDATE,
            //                                          //BILLENTRY = string.IsNullOrEmpty(_disposalDetail.BILLENTRY) ? "N/A" : _disposalDetail.BILLENTRY,
            //                                          //LICENSENO = string.IsNullOrEmpty(_disposalDetail.LICENSENO) ? "N/A" : _disposalDetail.LICENSENO,
            //                                          //BILLENTRY = _disposalDetail.BILLENTRY ?? "N/A",
            //                                          //LICENSENO = _disposalDetail.LICENSENO ?? "N/A",
            //                                          PURCHASEDDATE = _disposalDetail.PURCHASEDDATE,
            //                                          ORIGINALCOST = _disposalDetail.ORIGINALCOST,
            //                                          DEPRECIATIONCOST = _disposalDetail.DEPRECIATIONCOST,
            //                                          SALESPRICE = _disposalDetail.SALESPRICE,
            //                                          MGF_QLTYSTATUS = _disposalDetail.MGF_QLTYSTATUS,
            //                                          EVNSTATUS = _disposalDetail.EVNSTATUS,
            //                                          TAXTION_STATUS = _disposalDetail.TAXTION_STATUS,
            //                                          MAINTENANCEDEPT_STATUS = _disposalDetail.MAINTENANCEDEPT_STATUS,
            //                                          IC_STATUS = _disposalDetail.IC_STATUS,
            //                                          IBM_STATUS = _disposalDetail.IBM_STATUS,
            //                                          //ACTIVE = _disposalDetail.ACTIVE == 1 ? true : false,
            //                                          ACTIVE = Convert.ToBoolean(_disposalDetail.ACTIVE),
            //                                          //WDV = (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
            //                                          //PROFIT_LOSS = (_disposalDetail.SALESPRICE == null ? 0.00M : _disposalDetail.SALESPRICE) - (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
            //                                          VALUATION_GST = _disposalDetail.VALUATION_GST,
            //                                          TENTATIVE = _disposalDetail.TENTATIVE,
            //                                          PARTIAL_FULL = _disposalDetail.PARTIAL_FULL,
            //                                          VIBRATION_SENSOR = _disposalDetail.VIBRATION_SENSOR,
            //                                          ASSET_CATEGORY = _disposalDetail.ASSET_CATEGORY,
            //                                          CONDITIONID = _disposalDetail.CONDITIONID,
            //                                          ASSET_CONDITION = _ConditionMst.DESCRIPTION,
            //                                          SERIALNUMBER = _disposalDetail.SERIALNUMBER,
            //                                          PONUMBER = _disposalDetail.PONUMBER,
            //                                          PLANTID = _disposalDetail.PLANTID,
            //                                          //PLANTNAME = _siteApp == null ? "" : _siteApp.DESCRIP,
            //                                          //PLANTNAME = _siteApp.DESCRIP ?? "",
            //                                          CAPTALIZED_DATE = _disposalDetail.CAPTALIZED_DATE,
            //                                      }).ToList(),
            //                AppHistoryList = (from _disposalAppHis in _AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID)
            //                                  join _processStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on _disposalAppHis.PROCESSSTATUSID equals _processStatus.SIS_AST_PROCESSSTATUS_MSTID
            //                                  join _AppEmp in _AssetDBContext.ADEMPLOYEE on _disposalAppHis.APPCODE equals _AppEmp.ADEMPCODE
            //                                  select new ApprovalHisViewModel
            //                                  {
            //                                      DISPOSALHEADERID = _disposalAppHis.DISPOSALHEADERID,
            //                                      PROCESSSTATUSID = _disposalAppHis.PROCESSSTATUSID,
            //                                      APPROVALHISTORYID = _disposalAppHis.APPROVALHISTORYID,
            //                                      APPROVALSTATUS = _disposalAppHis.APPROVALSTATUS,
            //                                      STATUS_LEVEL = _processStatus.STATUS_LEVEL,
            //                                      ARS_MSG = ((_processStatus.STATUS_LEVEL == 15 || _processStatus.STATUS_LEVEL == 16 || _processStatus.STATUS_LEVEL == 17 || _processStatus.STATUS_LEVEL == 25) ? "Uploaded" : (_disposalAppHis.APPROVALSTATUS == 1 ? "Approved By" : _disposalAppHis.APPROVALSTATUS == 2 ? "Sendback By" : _disposalAppHis.APPROVALSTATUS == 3 ? "Rejected By" : "")),
            //                                      DISPLAY_MSG = _processStatus.DISPLAY_MSG,
            //                                      ATTACHMENT = _disposalAppHis.ATTACHMENT,
            //                                      APPCODE = _disposalAppHis.APPCODE,
            //                                      APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
            //                                      REMARKS = _disposalAppHis.REMARKS,
            //                                      DATEADDED = _disposalAppHis.DATEADDED,
            //                                      ADDEDBY = _disposalAppHis.ADDEDBY,
            //                                      SEQ_NUMBER = _processStatus.SEQ_NUMBER
            //                                  }).OrderBy(o => o.DATEADDED).ToList(),
            //            }).FirstOrDefault();
            //_obj.TOTAL_AMOUNT = (long)_obj.DisposalDetailList.Sum(s => s.ORIGINALCOST);
            _obj.TOTAL_AMOUNT = (long)_obj.DisposalDetailList.Where(w => w.IC_STATUS == 1).Max(s => s.ORIGINALCOST).Value;
            return _obj;
        }

        public AssetDisposalViewModel GetAssetRequestById(long id)
        {
            var _obj = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == id)
                        join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                        join _customerJoin in _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL on data.DISPOSALHEADERID equals _customerJoin.DISPOSALHEADERID into _customerJoin
                        from _customerDetail in _customerJoin.DefaultIfEmpty()
                        join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                        join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                        join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                        where _VWAssociate.SYKI == _Syki.SYKIID
                        select new AssetDisposalViewModel
                        {
                            DISPOSALHEADERID = data.DISPOSALHEADERID,
                            ADEMPCODE = data.ADEMPCODE,
                            EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                            ASSETTYPEID = data.ASSETTYPEID,
                            ASSETTYPE = _assetMst.DESCRIPTION,
                            PROCESSSTATUSID = data.PROCESSSTATUSID,
                            PROCESSSTATUS = _process.STATUSDESCRIPTION,
                            STATUS_LEVEL = _process.STATUS_LEVEL,
                            BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                            TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                            MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                            INVOICE_ATTACHMENT = data.INVOICE_ATTACHMENT,
                            REMARKS = data.REMARKS,
                            //ACTIVE = data.ACTIVE == 1 ? true : false,
                            ACTIVE = Convert.ToBoolean(data.ACTIVE),
                            ADDEDBY = data.ADDEDBY,
                            ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            MODIFIEDBY = data.MODIFIEDBY,
                            DATELSTMOD = data.DATELSTMOD,
                            IsPlantRequired = data.ISPLANTREQUIRED,
                            P1FAppStatus = data.P1F_APPSTATUS,
                            P2FAppStatus = data.P2F_APPSTATUS,
                            P3FAppStatus = data.P3F_APPSTATUS,
                            P4FAppStatus = data.P4F_APPSTATUS,
                            P1F_ATTACHMENT_NAME = data.P1F_ATTACHMENT,
                            P2F_ATTACHMENT_NAME = data.P2F_ATTACHMENT,
                            P3F_ATTACHMENT_NAME = data.P3F_ATTACHMENT,
                            P4F_ATTACHMENT_NAME = data.P4F_ATTACHMENT,
                            PLANTID = data.PLANTID,
                            //------------------START-[Code Added by Aumento on 14-Jan-2024]------------------------------
                            ICMONTH = data.ICMONTH,
                            IMPLEMENTMONTH = data.IMPLEMENTMONTH,
                            //------------------END-[Code Added by Aumento on 14-Jan-2024]--------------------------------
                            Emp_Detail = new Employee_Details
                            {
                                _ECode = _Emp.ADEMPCODE,
                                _EName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
                                _DOB = DateTime.Now,
                                _SecDescrip = _VWAssociate.SECTION,
                                _DepDesc = _VWAssociate.DEPARTMENT,
                                _DivDesc = _VWAssociate.DIVISION,
                                _OpDesc = _VWAssociate.OPERATION,
                                _SiteId = _VWAssociate.SYSITEID,
                                _SecId = _VWAssociate.SECTIONID,
                                _DepId = _VWAssociate.DEPARTMENTID,
                                _DivId = _VWAssociate.DIVISIONID,
                                _OpId = _VWAssociate.OPERATIONID,
                            },
                        }).FirstOrDefault();

            // Fetch DisposalDetailList separately
            if (_obj != null)
            {
                _obj.CustomerDetail =
                        (from _customerJoin in _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL
                         where _customerJoin.DISPOSALHEADERID == _obj.DISPOSALHEADERID
                         select new DisposalCustomerViewModel
                         {
                             DISPOSALCUSTDETAILID = _customerJoin.DISPOSALCUSTDETAILID,
                             DISPOSALHEADERID = _customerJoin.DISPOSALHEADERID,
                             CUSTOMERCODE = _customerJoin.CUSTOMERCODE,
                             CUSTOMERNAME = _customerJoin.CUSTOMERNAME,
                             CUSTADDRESS = _customerJoin.CUSTADDRESS,
                             CUSTGSTIN = _customerJoin.CUSTGSTIN,
                             GSTRATE = _customerJoin.GSTRATE,
                             BASICVALUE = _customerJoin.BASICVALUE,
                             PAYMENTDETAILS = _customerJoin.PAYMENTDETAILS,
                             ACTIVE = Convert.ToBoolean(_customerJoin.ACTIVE)
                         }).FirstOrDefault();
                _obj.DisposalDetailList = (from _disposalDetail in _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == _obj.DISPOSALHEADERID && x.ACTIVE == 1 /*&& x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1*/)
                                           join _ConditionJoin in _AssetDBContext.SIS_ASSETCONDITION on _disposalDetail.CONDITIONID equals _ConditionJoin.CONDITIONID into _ConditionJoin
                                           from _ConditionMst in _ConditionJoin.DefaultIfEmpty()
                                           join _siteJoin in _AssetDBContext.SYSITE on _disposalDetail.PLANTID equals _siteJoin.SYSITEID into _site_Join
                                           from _siteApp in _site_Join.DefaultIfEmpty()
                                           where (_obj.STATUS_LEVEL == 12 && _disposalDetail.ORIGINALCOST > 3000000) || _obj.STATUS_LEVEL != 12 //CR6452 Changes By TTL
                                           select new DisposalDetailViewModel
                                           {
                                               DISPOSALDETAILID = _disposalDetail.DISPOSALDETAILID,
                                               DISPOSALHEADERID = _disposalDetail.DISPOSALHEADERID,
                                               ASSETCODE = _disposalDetail.ASSETCODE,
                                               ASSETDESCRIPTION = _disposalDetail.ASSETDESCRIPTION,
                                               VENDORCODE = _disposalDetail.VENDORCODE,
                                               VENDORNAME = _disposalDetail.VENDORNAME,
                                               INVOICENO = _disposalDetail.INVOICENO,
                                               INVOICEDATE = _disposalDetail.INVOICEDATE,
                                               //BILLENTRY = string.IsNullOrEmpty(_disposalDetail.BILLENTRY) ? "N/A" : _disposalDetail.BILLENTRY,
                                               //LICENSENO = string.IsNullOrEmpty(_disposalDetail.LICENSENO) ? "N/A" : _disposalDetail.LICENSENO,
                                               BILLENTRY = _disposalDetail.BILLENTRY ?? "",
                                               LICENSENO = _disposalDetail.LICENSENO ?? "",
                                               PURCHASEDDATE = _disposalDetail.PURCHASEDDATE,
                                               ORIGINALCOST = _disposalDetail.ORIGINALCOST,
                                               DEPRECIATIONCOST = _disposalDetail.DEPRECIATIONCOST,
                                               SALESPRICE = _disposalDetail.SALESPRICE,
                                               MGF_QLTYSTATUS = _disposalDetail.MGF_QLTYSTATUS,
                                               EVNSTATUS = _disposalDetail.EVNSTATUS,
                                               TAXTION_STATUS = _disposalDetail.TAXTION_STATUS,
                                               MAINTENANCEDEPT_STATUS = _disposalDetail.MAINTENANCEDEPT_STATUS,
                                               IC_STATUS = _disposalDetail.IC_STATUS,
                                               IBM_STATUS = _disposalDetail.IBM_STATUS,
                                               //ACTIVE = _disposalDetail.ACTIVE == 1 ? true : false,
                                               ACTIVE = Convert.ToBoolean(_disposalDetail.ACTIVE),
                                               WDV = (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
                                               PROFIT_LOSS = (_disposalDetail.SALESPRICE == null ? 0.00M : _disposalDetail.SALESPRICE) - (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
                                               VALUATION_GST = _disposalDetail.VALUATION_GST,
                                               TENTATIVE = _disposalDetail.TENTATIVE,
                                               PARTIAL_FULL = _disposalDetail.PARTIAL_FULL,
                                               VIBRATION_SENSOR = _disposalDetail.VIBRATION_SENSOR,
                                               ASSET_CATEGORY = _disposalDetail.ASSET_CATEGORY,
                                               CONDITIONID = _disposalDetail.CONDITIONID,
                                               ASSET_CONDITION = _ConditionMst.DESCRIPTION,
                                               SERIALNUMBER = _disposalDetail.SERIALNUMBER,
                                               PONUMBER = _disposalDetail.PONUMBER,
                                               PLANTID = _disposalDetail.PLANTID,
                                               //PLANTNAME = _siteApp == null ? "" : _siteApp.DESCRIP,
                                               PLANTNAME = _siteApp.DESCRIP ?? "",
                                               CAPTALIZED_DATE = _disposalDetail.CAPTALIZED_DATE,
                                           }).ToList();

                _obj.AppHistoryList = (from _disposalAppHis in _AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.Where(x => x.DISPOSALHEADERID == _obj.DISPOSALHEADERID)
                                       join _processStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on _disposalAppHis.PROCESSSTATUSID equals _processStatus.SIS_AST_PROCESSSTATUS_MSTID
                                       join _AppEmp in _AssetDBContext.ADEMPLOYEE on _disposalAppHis.APPCODE equals _AppEmp.ADEMPCODE
                                       select new ApprovalHisViewModel
                                       {
                                           DISPOSALHEADERID = _disposalAppHis.DISPOSALHEADERID,
                                           PROCESSSTATUSID = _disposalAppHis.PROCESSSTATUSID,
                                           APPROVALHISTORYID = _disposalAppHis.APPROVALHISTORYID,
                                           APPROVALSTATUS = _disposalAppHis.APPROVALSTATUS,
                                           STATUS_LEVEL = _processStatus.STATUS_LEVEL,
                                           ARS_MSG = ((_processStatus.STATUS_LEVEL == 15 || _processStatus.STATUS_LEVEL == 16 || _processStatus.STATUS_LEVEL == 17 || _processStatus.STATUS_LEVEL == 25) ? "Uploaded" : (_disposalAppHis.APPROVALSTATUS == 1 ? "Approved By" : _disposalAppHis.APPROVALSTATUS == 2 ? "Sendback By" : _disposalAppHis.APPROVALSTATUS == 3 ? "Rejected By" : "")),
                                           DISPLAY_MSG = _processStatus.DISPLAY_MSG,
                                           ATTACHMENT = _disposalAppHis.ATTACHMENT,
                                           APPCODE = _disposalAppHis.APPCODE,
                                           APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                                           REMARKS = _disposalAppHis.REMARKS,
                                           DATEADDED = _disposalAppHis.DATEADDED,
                                           ADDEDBY = _disposalAppHis.ADDEDBY,
                                           SEQ_NUMBER = _processStatus.SEQ_NUMBER
                                       }).OrderBy(o => o.DATEADDED).ToList();
            }


            //var _obj = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == id)
            //            join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
            //            join _customerJoin in _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL on data.DISPOSALHEADERID equals _customerJoin.DISPOSALHEADERID into _customerJoin
            //            from _customerDetail in _customerJoin.DefaultIfEmpty()
            //            join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
            //            join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
            //            join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //            join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
            //            where _VWAssociate.SYKI == _Syki.SYKIID
            //            select new AssetDisposalViewModel
            //            {
            //                DISPOSALHEADERID = data.DISPOSALHEADERID,
            //                ADEMPCODE = data.ADEMPCODE,
            //                EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
            //                ASSETTYPEID = data.ASSETTYPEID,
            //                ASSETTYPE = _assetMst.DESCRIPTION,
            //                PROCESSSTATUSID = data.PROCESSSTATUSID,
            //                PROCESSSTATUS = _process.STATUSDESCRIPTION,
            //                STATUS_LEVEL = _process.STATUS_LEVEL,
            //                BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
            //                TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
            //                MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
            //                INVOICE_ATTACHMENT = data.INVOICE_ATTACHMENT,
            //                REMARKS = data.REMARKS,
            //                //ACTIVE = data.ACTIVE == 1 ? true : false,
            //                ACTIVE = Convert.ToBoolean(data.ACTIVE),
            //                ADDEDBY = data.ADDEDBY,
            //                ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                DATEADDED = data.DATEADDED,
            //                MODIFIEDBY = data.MODIFIEDBY,
            //                DATELSTMOD = data.DATELSTMOD,
            //                IsPlantRequired = data.ISPLANTREQUIRED,
            //                P1FAppStatus = data.P1F_APPSTATUS,
            //                P2FAppStatus = data.P2F_APPSTATUS,
            //                P3FAppStatus = data.P3F_APPSTATUS,
            //                P4FAppStatus = data.P4F_APPSTATUS,
            //                P1F_ATTACHMENT_NAME = data.P1F_ATTACHMENT,
            //                P2F_ATTACHMENT_NAME = data.P2F_ATTACHMENT,
            //                P3F_ATTACHMENT_NAME = data.P3F_ATTACHMENT,
            //                P4F_ATTACHMENT_NAME = data.P4F_ATTACHMENT,
            //                PLANTID = data.PLANTID,
            //                //------------------START-[Code Added by Aumento on 14-Jan-2024]------------------------------
            //                ICMONTH = data.ICMONTH,
            //                IMPLEMENTMONTH = data.IMPLEMENTMONTH,
            //                //------------------END-[Code Added by Aumento on 14-Jan-2024]--------------------------------
            //                Emp_Detail = new Employee_Details
            //                {
            //                    _ECode = _Emp.ADEMPCODE,
            //                    _EName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
            //                    _DOB = DateTime.Now,
            //                    _SecDescrip = _VWAssociate.SECTION,
            //                    _DepDesc = _VWAssociate.DEPARTMENT,
            //                    _DivDesc = _VWAssociate.DIVISION,
            //                    _OpDesc = _VWAssociate.OPERATION,
            //                    _SiteId = _VWAssociate.SYSITEID,
            //                    _SecId = _VWAssociate.SECTIONID,
            //                    _DepId = _VWAssociate.DEPARTMENTID,
            //                    _DivId = _VWAssociate.DIVISIONID,
            //                    _OpId = _VWAssociate.OPERATIONID,
            //                },
            //                CustomerDetail = _customerDetail == null ? null : new DisposalCustomerViewModel
            //                {
            //                    DISPOSALCUSTDETAILID = _customerDetail.DISPOSALCUSTDETAILID,
            //                    DISPOSALHEADERID = _customerDetail.DISPOSALHEADERID,
            //                    CUSTOMERCODE = _customerDetail.CUSTOMERCODE,
            //                    CUSTOMERNAME = _customerDetail.CUSTOMERNAME,
            //                    CUSTADDRESS = _customerDetail.CUSTADDRESS,
            //                    CUSTGSTIN = _customerDetail.CUSTGSTIN,
            //                    GSTRATE = _customerDetail.GSTRATE,
            //                    BASICVALUE = _customerDetail.BASICVALUE,
            //                    PAYMENTDETAILS = _customerDetail.PAYMENTDETAILS,
            //                    ACTIVE = _customerDetail.ACTIVE == 1 ? true : false,
            //                },
            //                DisposalDetailList = (from _disposalDetail in _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1)
            //                                      join _ConditionJoin in _AssetDBContext.SIS_ASSETCONDITION on _disposalDetail.CONDITIONID equals _ConditionJoin.CONDITIONID into _ConditionJoin
            //                                      from _ConditionMst in _ConditionJoin.DefaultIfEmpty()
            //                                      join _MgfJoin in _AssetDBContext.SIS_AST_MGFPLANTAPPROVAL on _disposalDetail.DISPOSALDETAILID equals _MgfJoin.DISPOSALDETAILID into _MgfJoin
            //                                      from _MgfPlanApp in _MgfJoin.DefaultIfEmpty()
            //                                      join _siteJoin in _AssetDBContext.SYSITE on _disposalDetail.PLANTID equals _siteJoin.SYSITEID into _site_Join
            //                                      from _siteApp in _site_Join.DefaultIfEmpty()
            //                                      select new DisposalDetailViewModel
            //                                      {
            //                                          DISPOSALDETAILID = _disposalDetail.DISPOSALDETAILID,
            //                                          DISPOSALHEADERID = _disposalDetail.DISPOSALHEADERID,
            //                                          ASSETCODE = _disposalDetail.ASSETCODE,
            //                                          ASSETDESCRIPTION = _disposalDetail.ASSETDESCRIPTION,
            //                                          VENDORCODE = _disposalDetail.VENDORCODE,
            //                                          VENDORNAME = _disposalDetail.VENDORNAME,
            //                                          INVOICENO = _disposalDetail.INVOICENO,
            //                                          INVOICEDATE = _disposalDetail.INVOICEDATE,
            //                                          BILLENTRY = string.IsNullOrEmpty(_disposalDetail.BILLENTRY) ? "N/A" : _disposalDetail.BILLENTRY,
            //                                          LICENSENO = string.IsNullOrEmpty(_disposalDetail.LICENSENO) ? "N/A" : _disposalDetail.LICENSENO,
            //                                          PURCHASEDDATE = _disposalDetail.PURCHASEDDATE,
            //                                          ORIGINALCOST = _disposalDetail.ORIGINALCOST,
            //                                          DEPRECIATIONCOST = _disposalDetail.DEPRECIATIONCOST,
            //                                          SALESPRICE = _disposalDetail.SALESPRICE,
            //                                          MGF_QLTYSTATUS = _disposalDetail.MGF_QLTYSTATUS,
            //                                          EVNSTATUS = _disposalDetail.EVNSTATUS,
            //                                          TAXTION_STATUS = _disposalDetail.TAXTION_STATUS,
            //                                          MAINTENANCEDEPT_STATUS = _disposalDetail.MAINTENANCEDEPT_STATUS,
            //                                          IC_STATUS = _disposalDetail.IC_STATUS,
            //                                          IBM_STATUS = _disposalDetail.IBM_STATUS,
            //                                          ACTIVE = _disposalDetail.ACTIVE == 1 ? true : false,
            //                                          WDV = (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
            //                                          PROFIT_LOSS = (_disposalDetail.SALESPRICE == null ? 0.00M : _disposalDetail.SALESPRICE) - (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
            //                                          VALUATION_GST = _disposalDetail.VALUATION_GST,
            //                                          TENTATIVE = _disposalDetail.TENTATIVE,
            //                                          PARTIAL_FULL = _disposalDetail.PARTIAL_FULL,
            //                                          VIBRATION_SENSOR = _disposalDetail.VIBRATION_SENSOR,
            //                                          ASSET_CATEGORY = _disposalDetail.ASSET_CATEGORY,
            //                                          CONDITIONID = _disposalDetail.CONDITIONID,
            //                                          ASSET_CONDITION = _ConditionMst.DESCRIPTION,
            //                                          C1F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C1F_APPROVEBY,
            //                                          C2F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C2F_APPROVEBY,
            //                                          C3F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C3F_APPROVEBY,
            //                                          C4F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C4F_APPROVEBY,
            //                                          CONDITION_1F = _MgfPlanApp == null ? "" : _MgfPlanApp.C1F_CONDITION,
            //                                          CONDITION_2F = _MgfPlanApp == null ? "" : _MgfPlanApp.C2F_CONDITION,
            //                                          CONDITION_3F = _MgfPlanApp == null ? "" : _MgfPlanApp.C3F_CONDITION,
            //                                          CONDITION_4F = _MgfPlanApp == null ? "" : _MgfPlanApp.C4F_CONDITION,
            //                                          SERIALNUMBER = _disposalDetail.SERIALNUMBER,
            //                                          PONUMBER = _disposalDetail.PONUMBER,
            //                                          PLANTID = _disposalDetail.PLANTID,
            //                                          PLANTNAME = _siteApp == null ? "" : _siteApp.DESCRIP,
            //                                          CAPTALIZED_DATE = _disposalDetail.CAPTALIZED_DATE,
            //                                      }).ToList(),
            //                AppHistoryList = (from _disposalAppHis in _AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID)
            //                                  join _processStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on _disposalAppHis.PROCESSSTATUSID equals _processStatus.SIS_AST_PROCESSSTATUS_MSTID
            //                                  join _AppEmp in _AssetDBContext.ADEMPLOYEE on _disposalAppHis.APPCODE equals _AppEmp.ADEMPCODE
            //                                  select new ApprovalHisViewModel
            //                                  {
            //                                      DISPOSALHEADERID = _disposalAppHis.DISPOSALHEADERID,
            //                                      PROCESSSTATUSID = _disposalAppHis.PROCESSSTATUSID,
            //                                      APPROVALHISTORYID = _disposalAppHis.APPROVALHISTORYID,
            //                                      APPROVALSTATUS = _disposalAppHis.APPROVALSTATUS,
            //                                      STATUS_LEVEL = _processStatus.STATUS_LEVEL,
            //                                      ARS_MSG = ((_processStatus.STATUS_LEVEL == 15 || _processStatus.STATUS_LEVEL == 16 || _processStatus.STATUS_LEVEL == 17 || _processStatus.STATUS_LEVEL == 25) ? "Uploaded" : (_disposalAppHis.APPROVALSTATUS == 1 ? "Approved By" : _disposalAppHis.APPROVALSTATUS == 2 ? "Sendback By" : _disposalAppHis.APPROVALSTATUS == 3 ? "Rejected By" : "")),
            //                                      DISPLAY_MSG = _processStatus.DISPLAY_MSG,
            //                                      ATTACHMENT = _disposalAppHis.ATTACHMENT,
            //                                      APPCODE = _disposalAppHis.APPCODE,
            //                                      APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
            //                                      REMARKS = _disposalAppHis.REMARKS,
            //                                      DATEADDED = _disposalAppHis.DATEADDED,
            //                                      ADDEDBY = _disposalAppHis.ADDEDBY,
            //                                      SEQ_NUMBER = _processStatus.SEQ_NUMBER
            //                                  }).OrderBy(o => o.DATEADDED).ToList(),
            //            }).FirstOrDefault();
            ////_obj.TOTAL_AMOUNT = (long)_obj.DisposalDetailList.Sum(s => s.ORIGINALCOST);
            _obj.TOTAL_AMOUNT = (long)_obj.DisposalDetailList.Max(s => s.ORIGINALCOST).Value;
            return _obj;
        }

        public Tuple<short, long> SaveAssetItem(AssetDisposalViewModel ADVM)
        {
            short retVal = 0; long _headerId = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                    int FlagAdd = 0;
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                    }
                    else
                    {
                        SADH = new SIS_AST_DISPOSALHEADER();

                        //if (_AssetDBContext.SIS_AST_DISPOSALHEADER.ToList().Count == 0)
                        if (_AssetDBContext.SIS_AST_DISPOSALHEADER.Count() == 0)
                        {
                            SADH.DISPOSALHEADERID = 1;
                        }
                        else
                        {
                            SADH.DISPOSALHEADERID = _AssetDBContext.SIS_AST_DISPOSALHEADER.Max(x => x.DISPOSALHEADERID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    ADVM.SYKI = Convert.ToInt64(_Syki.SYKIID);
                    ADVM.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(p => p.ACTIVE == 1 && p.STATUS_LEVEL == 0).Select(s => s.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                    ///// -----
                    SADH.ASSETTYPEID = ADVM.ASSETTYPEID;
                    SADH.ADEMPCODE = ADVM.ADEMPCODE;
                    SADH.SYKI = ADVM.SYKI;
                    SADH.PROCESSSTATUSID = ADVM.PROCESSSTATUSID;
                    SADH.PLANTID = ADVM.DisposalDetailList.FirstOrDefault().PLANTID;
                    SADH.ACTIVE = 1;
                    if (FlagAdd == 1)
                    {
                        SADH.ADDEDBY = ADVM.ADDEDBY;
                        SADH.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                        SADH.DATELSTMOD = DateTime.Now;
                    }
                    _AssetDBContext.Entry(SADH).State = FlagAdd == 1 ? EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _AssetDBContext.SaveChanges();

                    int SameLocationCount = ADVM.DisposalDetailList.Where(item => item.PLANTID == SADH.PLANTID).Count();
                    if (SameLocationCount == ADVM.DisposalDetailList.Count())
                    {
                        ///// -------- Insert Record Disposal Detail Table ----------/////
                        short _val = SaveDisposalDetails(SADH.ADDEDBY, SADH.DISPOSALHEADERID, SADH.ASSETTYPEID, ADVM.DisposalDetailList.ToList());
                        if (_val == 1)
                        {
                            transaction.Commit();
                            _headerId = SADH.DISPOSALHEADERID;
                            retVal = 1;
                        }
                        else
                        {
                            retVal = -2; ////// Asset item already process
                            transaction.Rollback();
                        }
                    }
                    else
                    {
                        retVal = -4; ////// Selected Asset items location not match.
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                Tuple<short, long> tuple_retVal = new Tuple<short, long>(retVal, _headerId);
                return tuple_retVal;
            }
        }

        public Tuple<short, long> SaveAssetRequest(AssetDisposalViewModel ADVM, short Status_Level)
        {
            long _headerId = 0;
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                    int FlagAdd = 0;
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                    }
                    else
                    {
                        SADH = new SIS_AST_DISPOSALHEADER();
                        if (_AssetDBContext.SIS_AST_DISPOSALHEADER.ToList().Count == 0)
                        {
                            SADH.DISPOSALHEADERID = 1;
                        }
                        else
                        {
                            SADH.DISPOSALHEADERID = _AssetDBContext.SIS_AST_DISPOSALHEADER.Max(x => x.DISPOSALHEADERID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    _headerId = SADH.DISPOSALHEADERID;
                    ///// ---- Get SYKI & ProcessStatus ///// ---- 22 for Maintenance Dept.
                    Status_Level = ADVM.IsFinalSubmit == 1 ? Status_Level : (short)0;
                    ADVM.SYKI = Convert.ToInt64(_Syki.SYKIID);
                    ADVM.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(p => p.ACTIVE == 1 && p.STATUS_LEVEL == Status_Level).Select(s => s.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                    ///// -----
                    SADH.ASSETTYPEID = ADVM.ASSETTYPEID;
                    SADH.ADEMPCODE = ADVM.ADEMPCODE;
                    SADH.SYKI = ADVM.SYKI;
                    SADH.REMARKS = ADVM.REMARKS;
                    if (ADVM.BACKUP_ATTACHMENT_NAME != null)
                    {
                        SADH.BACKUP_ATTACHMENT = ADVM.BACKUP_ATTACHMENT_NAME;
                    }
                    if (ADVM.TRANSFERSLIP_ATTACHMENT_NAME != null)
                    {
                        SADH.TRANSFERSLIP_ATTACHMENT = ADVM.TRANSFERSLIP_ATTACHMENT_NAME;
                    }
                    ////SADH.MUTILATION_ATTACHMENT = ADVM.MUTILATION_ATTACHMENT_NAME;
                    SADH.PROCESSSTATUSID = ADVM.PROCESSSTATUSID;
                    SADH.PLANTID = ADVM.DisposalDetailList.FirstOrDefault().PLANTID;
                    SADH.ACTIVE = 1;
                    //------------------START-[Code Added by Aumento on 08-Jan-2024]------------------------------
                    SADH.ICMONTH = ADVM.ICMONTH;
                    SADH.IMPLEMENTMONTH = ADVM.IMPLEMENTMONTH;
                    //------------------END-[Code Added by Aumento on 08-Jan-2024]--------------------------------
                    if (FlagAdd == 1)
                    {
                        SADH.ADDEDBY = ADVM.ADDEDBY;
                        SADH.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                        SADH.DATELSTMOD = DateTime.Now;
                    }
                    _AssetDBContext.Entry(SADH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _AssetDBContext.SaveChanges();

                    int SameLocationCount = ADVM.DisposalDetailList.Where(item => item.PLANTID == SADH.PLANTID).Count();
                    if (SameLocationCount == ADVM.DisposalDetailList.Count())
                    {
                        ///// -------- Insert Record Disposal Detail Table ----------/////
                        short _val = SaveDisposalDetails(SADH.ADDEDBY, SADH.DISPOSALHEADERID, SADH.ASSETTYPEID, ADVM.DisposalDetailList.ToList());
                        if (_val == 1)
                        {
                            if (SADH.ASSETTYPEID == 1 && ADVM.CustomerDetail.CUSTOMERCODE != null) ///// Sale
                            {
                                ///// -------- Insert Record Disposal Customer Table ----------/////
                                SaveDisposalCustomerDetails(SADH.ADDEDBY, SADH.DISPOSALHEADERID, ADVM.CustomerDetail);
                            }
                            transaction.Commit();
                            retVal = 1;
                        }
                        else
                        {
                            retVal = -2; ////// Asset item already process
                            transaction.Rollback();
                        }
                    }
                    else
                    {
                        retVal = -4; ////// Selected Asset items location not match.
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                Tuple<short, long> tuple_retVal = new Tuple<short, long>(retVal, _headerId);
                return tuple_retVal;
            }
        }

        public short SaveDisposalDetails(long AddedBy, long DisposalHeaderId, long AssetTypeId, List<DisposalDetailViewModel> DDVMList)
        {
            ///// ---- Delete unselected Asset Items ---- //////
            string[] AssetItems = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == DisposalHeaderId && x.ACTIVE == 1).Select(s => s.ASSETCODE).ToArray();
            if (AssetItems.Count() > 0)
            {
                foreach (string _assetCode in AssetItems)
                {
                    if (!DDVMList.Any(a => a.ASSETCODE == _assetCode))
                    {
                        SIS_AST_DISPOSALDETAIL _SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == DisposalHeaderId && x.ASSETCODE == _assetCode && x.ACTIVE == 1).FirstOrDefault();
                        if (_SADD != null)
                        {
                            _SADD.ACTIVE = 0;
                            _SADD.MGF_QLTYSTATUS = 0;
                            _SADD.EVNSTATUS = 0;
                            _SADD.TAXTION_STATUS = 0;
                            _SADD.MAINTENANCEDEPT_STATUS = 0;
                            _SADD.IC_STATUS = 0;
                            _SADD.IBM_STATUS = 0;
                            _SADD.MODIFIEDBY = AddedBy;
                            _SADD.DATELSTMOD = DateTime.Now;
                            _AssetDBContext.Entry(_SADD).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }
                    }
                }
            }
            ///// ------ End ---- /////

            foreach (DisposalDetailViewModel DDVM in DDVMList)
            {
                var _checkExist = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                                   join _headerDetail in _AssetDBContext.SIS_AST_DISPOSALDETAIL on data.DISPOSALHEADERID equals _headerDetail.DISPOSALHEADERID
                                   where data.ACTIVE == 1 && _headerDetail.ACTIVE == 1 && _headerDetail.ASSETCODE == DDVM.ASSETCODE && data.DISPOSALHEADERID != DisposalHeaderId
                                   && _headerDetail.MGF_QLTYSTATUS == 1 && _headerDetail.EVNSTATUS == 1 && _headerDetail.TAXTION_STATUS == 1 && _headerDetail.MAINTENANCEDEPT_STATUS == 1
                                   && _headerDetail.IC_STATUS == 1 && _headerDetail.IBM_STATUS == 1 && data.PROCESSSTATUSID != 21
                                   select data.DISPOSALHEADERID).FirstOrDefault();                
             
                if (_checkExist != null && _checkExist != 0)
                {
                    return 2;
                }
                /// adeded
                var record = _AssetDBContext.SIS_AST_DISPOSALDETAIL.FirstOrDefault(x => x.DISPOSALHEADERID == DisposalHeaderId && x.ASSETCODE == DDVM.ASSETCODE);
                
                ///////
                SIS_AST_DISPOSALDETAIL SADD = new SIS_AST_DISPOSALDETAIL();
                int FlagAdd = 0;
                if (DDVM.DISPOSALDETAILID > 0)
                {
                    SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALDETAILID == DDVM.DISPOSALDETAILID).SingleOrDefault();
                }
                //else if (_AssetDBContext.SIS_AST_DISPOSALDETAIL.Any(x => x.DISPOSALHEADERID == Convert.ToInt64 (DisposalHeaderId) && x.ASSETCODE == DDVM.ASSETCODE))
                else if (record != null)
                {
                    SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(y => y.DISPOSALHEADERID == DisposalHeaderId && y.ASSETCODE == DDVM.ASSETCODE).FirstOrDefault();
                }
                else
                {
                    SADD = new SIS_AST_DISPOSALDETAIL();
                    if (_AssetDBContext.SIS_AST_DISPOSALDETAIL.ToList().Count == 0)
                    {
                        SADD.DISPOSALDETAILID = 1;
                    }
                    else
                    {
                        SADD.DISPOSALDETAILID = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Max(x => x.DISPOSALDETAILID) + 1;
                    }
                    FlagAdd = 1;
                }
                SADD.DISPOSALHEADERID = DisposalHeaderId;
                SADD.ASSETCODE = DDVM.ASSETCODE.Trim();
                SADD.ASSETDESCRIPTION = DDVM.ASSETDESCRIPTION.Trim() + " - " + DDVM.ASSETDETAIL;
                //change done by aumento on 22042023=================
                //SADD.ASSET_CATEGORY = Convert.ToInt16(AssetTypeId);
                //===================================================
                SADD.VENDORCODE = DDVM.VENDORCODE == null ? "" : DDVM.VENDORCODE.Trim();
                SADD.VENDORNAME = DDVM.VENDORNAME == null ? "" : DDVM.VENDORNAME.Trim();
                SADD.INVOICENO = DDVM.INVOICENO == null ? "" : DDVM.INVOICENO.Trim();
                if (DDVM.INVOICEDATE != null)
                {
                    SADD.INVOICEDATE = DDVM.INVOICEDATE;
                }
                SADD.ORIGINALCOST = DDVM.ORIGINALCOST;
                SADD.DEPRECIATIONCOST = DDVM.DEPRECIATIONCOST;
                //change done by aumento on 22042023======================
                SADD.SALESPRICE = DDVM.SALESPRICE;
                //SADD.SALESPRICE = AssetTypeId == 1 ? DDVM.SALESPRICE : null;
                //===========================================================
                //SADD.BILLENTRY = "";
                //SADD.LICENSENO = "";
                //SADD.PURCHASEDDATE = DDVM.PURCHASEDDATE;
                SADD.MGF_QLTYSTATUS = 1;
                SADD.EVNSTATUS = 1;
                SADD.TAXTION_STATUS = 1;
                SADD.MAINTENANCEDEPT_STATUS = 1;
                SADD.IC_STATUS = 1;
                SADD.IBM_STATUS = 1;
                SADD.VIBRATION_SENSOR = DDVM.VIBRATION_SENSOR;
                SADD.TENTATIVE = DDVM.TENTATIVE;
                SADD.CONDITIONID = AssetTypeId == 2 ? DDVM.CONDITIONID : null;
                //change done by aumento on 22042023===============================
                SADD.PARTIAL_FULL =DDVM.PARTIAL_FULL;
                //SADD.PARTIAL_FULL = AssetTypeId == 2 ? DDVM.PARTIAL_FULL : null;
                //==================================================================

                SADD.ACTIVE = 1;
                SADD.SERIALNUMBER = DDVM.SERIALNUMBER;
                SADD.PONUMBER = DDVM.PONUMBER;
                SADD.PLANTID = DDVM.PLANTID;
                if (DDVM.CAPTALIZED_DATE != null)
                {
                    SADD.CAPTALIZED_DATE = DDVM.CAPTALIZED_DATE;
                }
                if (FlagAdd == 1)
                {
                    SADD.ADDEDBY = AddedBy;
                    SADD.DATEADDED = DateTime.Now;
                }
                else
                {
                    SADD.MODIFIEDBY = AddedBy;
                    SADD.DATELSTMOD = DateTime.Now;
                }
                _AssetDBContext.Entry(SADD).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                _AssetDBContext.SaveChanges();
            }
            return 1;
        }

        public void SaveDisposalCustomerDetails([FromBody] long AddedBy, long DisposalHeaderId, DisposalCustomerViewModel DCVM)
        {
            SIS_AST_DISPOSALCUSTDETAIL SADD = new SIS_AST_DISPOSALCUSTDETAIL();
            int FlagAdd = 0;
            if (DCVM.DISPOSALCUSTDETAILID > 0)
            {
                SADD = _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL.Where(x => x.DISPOSALCUSTDETAILID == DCVM.DISPOSALCUSTDETAILID).SingleOrDefault();
            }
            else
            {
                var disposalCustDetail = _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL
                                         .FirstOrDefault(x => x.DISPOSALCUSTDETAILID.ToString() == DCVM.DISPOSALCUSTDETAILID.ToString());

                //if (_AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL.Any(x => x.DISPOSALCUSTDETAILID == DCVM.DISPOSALCUSTDETAILID))
                if(disposalCustDetail != null)
                {
                    return;
                }

                SADD = new SIS_AST_DISPOSALCUSTDETAIL();
                if (_AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL.ToList().Count == 0)
                {
                    SADD.DISPOSALCUSTDETAILID = 1;
                }
                else
                {
                    SADD.DISPOSALCUSTDETAILID = _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL.Max(x => x.DISPOSALCUSTDETAILID) + 1;
                }
                FlagAdd = 1;
            }
            SADD.DISPOSALHEADERID = DisposalHeaderId;
            SADD.CUSTOMERCODE = DCVM.CUSTOMERCODE;
            SADD.CUSTOMERNAME = DCVM.CUSTOMERNAME;
            SADD.CUSTADDRESS = DCVM.CUSTADDRESS;
            SADD.CUSTGSTIN = DCVM.CUSTGSTIN;
            SADD.BASICVALUE = DCVM.BASICVALUE;
            SADD.GSTRATE = DCVM.GSTRATE;
            SADD.PAYMENTDETAILS = string.IsNullOrEmpty(DCVM.PAYMENTDETAILS) ? "N/A" : DCVM.PAYMENTDETAILS;
            SADD.ACTIVE = 1;
            if (FlagAdd == 1)
            {
                SADD.ADDEDBY = AddedBy;
                SADD.DATEADDED = DateTime.Now;
            }
            else
            {
                SADD.MODIFIEDBY = AddedBy;
                SADD.DATELSTMOD = DateTime.Now;
            }
            _AssetDBContext.Entry(SADD).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
            _AssetDBContext.SaveChanges();
        }

        public void SaveApprovalHistory(ApprovalHisViewModel AHVM)
        {
            SIS_AST_DISPOSALAPPHISTORY SDAH = new SIS_AST_DISPOSALAPPHISTORY();
            int FlagAdd = 0;
            if (AHVM.APPROVALHISTORYID > 0)
            {
                SDAH = _AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.Where(x => x.APPROVALHISTORYID == AHVM.APPROVALHISTORYID).SingleOrDefault();
            }
            else
            {
                SDAH = new SIS_AST_DISPOSALAPPHISTORY();
                if (_AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.ToList().Count == 0)
                {
                    SDAH.APPROVALHISTORYID = 1;
                }
                else
                {
                    SDAH.APPROVALHISTORYID = _AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.Max(x => x.APPROVALHISTORYID) + 1;
                }
                FlagAdd = 1;
            }
            SDAH.DISPOSALHEADERID = AHVM.DISPOSALHEADERID;
            SDAH.APPCODE = AHVM.APPCODE;
            SDAH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == AHVM.DISPOSALHEADERID).Select(x => x.PROCESSSTATUSID).SingleOrDefault();
            SDAH.REMARKS = AHVM.REMARKS;
            SDAH.APPROVALSTATUS = AHVM.APPROVALSTATUS;
            SDAH.ATTACHMENT = AHVM.ATTACHMENT;
            if (FlagAdd == 1)
            {
                SDAH.ADDEDBY = AHVM.ADDEDBY;
                SDAH.DATEADDED = DateTime.Now;
            }
            _AssetDBContext.Entry(SDAH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
            _AssetDBContext.SaveChanges();
        }

        public short UploadAttachmentByRequestor(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public short CancelRequest(AssetDisposalViewModel ADVM, int? cancelledByAdmin)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        //// -------- Request Cancellation ---- ////
                        SIS_AST_DISPOSALHEADER astDisposalheader = new SIS_AST_DISPOSALHEADER();
                        astDisposalheader = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(c => c.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).FirstOrDefault();
                        if (astDisposalheader != null)
                        {
                            short Status_Level = cancelledByAdmin == 1 ? Convert.ToInt16(27) : Convert.ToInt16(26); ///// 26 - Request Cancelled by User, 27 - Request cancelled by admin
                            astDisposalheader.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();

                            astDisposalheader.ACTIVE = 0;
                            astDisposalheader.MODIFIEDBY = ADVM.MODIFIEDBY;
                            astDisposalheader.DATELSTMOD = new DateTime?(DateTime.Now);
                            _AssetDBContext.Entry<SIS_AST_DISPOSALHEADER>(astDisposalheader).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }

                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retVal = (short)-1;
                }
            }
            return retVal;
        }
        #endregion

        #region Maintenance Dept. Approval
        public List<AssetDisposalViewModel> GetApprovalListByMaintenanceDept(long loginUser)
        {
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         //join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == 22 && data.ACTIVE == 1 ////--- 22 for Maintenance Dept.
                         && _AppAuth.EMPCODE == loginUser && _AppAuth.ACTIVE == 1
                         && _AppAuth.APPROVALTYPE == _process.SIS_AST_PROCESSSTATUS_MSTID
                         //&& _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByMaintenanceDept(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Detail Table ----------/////
                        foreach (DisposalDetailViewModel DDVM in ADVM.DisposalDetailList)
                        {
                            SIS_AST_DISPOSALDETAIL SADD = new SIS_AST_DISPOSALDETAIL();
                            if (DDVM.DISPOSALDETAILID > 0)
                            {
                                SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALDETAILID == DDVM.DISPOSALDETAILID).SingleOrDefault();
                                if (SADD != null)
                                {
                                    SADD.MAINTENANCEDEPT_STATUS = DDVM.APPROVAL_STATUS;
                                    SADD.MODIFIEDBY = ADVM.MODIFIEDBY;
                                    SADD.DATELSTMOD = DateTime.Now;
                                }
                            }
                            _AssetDBContext.Entry(SADD).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        #endregion

        #region Taxtion Approval
        public List<AssetDisposalViewModel> GetApprovalListByTaxtion(long loginUser)
        {
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         //join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == 1 && data.ACTIVE == 1
                         && _AppAuth.EMPCODE == loginUser && _AppAuth.ACTIVE == 1
                         && _AppAuth.APPROVALTYPE == _process.SIS_AST_PROCESSSTATUS_MSTID
                         //&& _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByTaxtion(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Detail Table ----------/////
                        foreach (DisposalDetailViewModel DDVM in ADVM.DisposalDetailList)
                        {
                            SIS_AST_DISPOSALDETAIL SADD = new SIS_AST_DISPOSALDETAIL();
                            if (DDVM.DISPOSALDETAILID > 0)
                            {
                                SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALDETAILID == DDVM.DISPOSALDETAILID).SingleOrDefault();
                                if (SADD != null)
                                {
                                    short? assetCategory = DDVM.ASSET_CATEGORY;
                                    int? nullable = assetCategory.HasValue ? new int?((int)assetCategory.GetValueOrDefault()) : new int?();
                                    int num2 = 1;
                                    if (nullable.GetValueOrDefault() == num2 && nullable.HasValue)
                                    {
                                        SADD.BILLENTRY = DDVM.BILLENTRY;
                                        SADD.LICENSENO = DDVM.LICENSENO;
                                        SADD.PURCHASEDDATE = DDVM.PURCHASEDDATE;
                                    }
                                    SADD.TAXTION_STATUS = DDVM.APPROVAL_STATUS;
                                    SADD.ASSET_CATEGORY = DDVM.ASSET_CATEGORY;
                                    SADD.VALUATION_GST = DDVM.VALUATION_GST;
                                    SADD.MODIFIEDBY = ADVM.MODIFIEDBY;
                                    SADD.DATELSTMOD = DateTime.Now;
                                }
                            }
                            _AssetDBContext.Entry(SADD).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public List<AssetDisposalViewModel> GetApprovalHistory(long loginUser)
        {
            var iList = (from _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST
                         join _disApp in _AssetDBContext.SIS_AST_DISPOSALAPPHISTORY on _process.SIS_AST_PROCESSSTATUS_MSTID equals _disApp.PROCESSSTATUSID
                         join _disHeader in _AssetDBContext.SIS_AST_DISPOSALHEADER on _disApp.DISPOSALHEADERID equals _disHeader.DISPOSALHEADERID
                         join _proStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on _disHeader.PROCESSSTATUSID equals _proStatus.SIS_AST_PROCESSSTATUS_MSTID
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on _disHeader.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on _disHeader.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on _disHeader.ADDEDBY equals _AddBy.ADEMPCODE
                         where _disApp.APPCODE == loginUser
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = _disHeader.DISPOSALHEADERID,
                             ADEMPCODE = _disHeader.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = _disHeader.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = _proStatus.SIS_AST_PROCESSSTATUS_MSTID,
                             PROCESSSTATUS = _proStatus.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = _disHeader.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = _disHeader.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = _disHeader.MUTILATION_ATTACHMENT,
                             REMARKS = _disHeader.REMARKS,
                             //ACTIVE = _disHeader.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(_disHeader.ACTIVE),
                             ADDEDBY = _disHeader.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = _disHeader.DATEADDED,
                             MODIFIEDBY = _disHeader.MODIFIEDBY,
                             DATELSTMOD = _disHeader.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == _disHeader.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d=>d.DATEADDED).ToList();
        }

        #endregion

        #region User Approval
        public List<AssetDisposalViewModel> GetApprovalListByUser(List<short> Status_Level_List, long _DepId, long _DivId, long _OpId, long _SiteId, long _EmpCode, List<UserApprovalAuthority> _AuthorityList)
        {
            List<AssetDisposalViewModel> FinalList = new List<AssetDisposalViewModel>();
            foreach (short Status_Level in Status_Level_List)
            {
                List<long?> _OrgLevel = new List<long?>();
                if (Status_Level == (short)2)
                {

                    List<long> ADORGIDS = _AssetDBContext.ADORGLEVELHEAD.Where(a => a.ADEMPCODE == _EmpCode && a.ISACTIVE == 1).Select(s => s.ADORGLEVELID).ToList();
                    if (ADORGIDS.Count > 0)
                    {
                        foreach (long orgId in ADORGIDS)
                        {
                            _OrgLevel.Add(new long?(orgId));
                        }
                    }
                    else
                    {
                        _OrgLevel.Add(new long?(_DepId));
                    }
                }
                else if (Status_Level == (short)3)
                    _OrgLevel = _AuthorityList.Where(y =>
                    {
                        long? coordinator = y.Coordinator;
                        long num = _EmpCode;
                        return coordinator.GetValueOrDefault() == num && coordinator.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                else if (Status_Level == (short)4)
                {
                    List<long> ADORGIDS = _AssetDBContext.ADORGLEVELHEAD.Where(a => a.ADEMPCODE == _EmpCode && a.ISACTIVE == 1).Select(s => s.ADORGLEVELID).ToList();
                    if (ADORGIDS.Count > 0)
                    {
                        foreach (long orgId in ADORGIDS)
                        {
                            _OrgLevel.Add(new long?(orgId));
                        }
                    }
                    else
                    {
                        _OrgLevel.Add(new long?(_DivId));
                    }
                }
                else if (Status_Level == (short)6)
                    _OrgLevel = _AuthorityList.Where(y =>
                    {
                        long? operationHead = y.OperationHead;
                        long num = _EmpCode;
                        return operationHead.GetValueOrDefault() == num && operationHead.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                else if (Status_Level == (short)5)
                    _OrgLevel = _AuthorityList.Where(y =>
                    {
                        long? exeCoordinator = y.EXECoordinator;
                        long num = _EmpCode;
                        return exeCoordinator.GetValueOrDefault() == num && exeCoordinator.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                else if (Status_Level == (short)9)
                {
                    _OrgLevel = _AuthorityList.Where(y =>
                    {
                        long? director = y.Director;
                        long num = _EmpCode;
                        return director.GetValueOrDefault() == num && director.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                }
                else if (Status_Level == (short)24)
                {
                    _OrgLevel = _AuthorityList.Where(y =>
                    {
                        long? director2 = y.Director2;
                        long num = _EmpCode;
                        return director2.GetValueOrDefault() == num && director2.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                }
                else
                {
                    if (Status_Level == (short)10)
                    {
                        #region Get CPO Data
                        var iListRes = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                                        join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                                        join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                                        join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                                        join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                                        join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on _EmpCode equals _AppAuth.EMPCODE
                                        where _process.STATUS_LEVEL == Status_Level && _process.SIS_AST_PROCESSSTATUS_MSTID == _AppAuth.APPROVALTYPE
                                        && data.ACTIVE == 1 && _AppAuth.EMPCODE == _EmpCode && _AppAuth.ACTIVE == 1
                                        select new AssetDisposalViewModel
                                        {
                                            DISPOSALHEADERID = data.DISPOSALHEADERID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                                            ASSETTYPEID = data.ASSETTYPEID,
                                            ASSETTYPE = _assetMst.DESCRIPTION,
                                            PROCESSSTATUSID = data.PROCESSSTATUSID,
                                            PROCESSSTATUS = _process.STATUSDESCRIPTION,
                                            BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                                            TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                                            MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                                            REMARKS = data.REMARKS,
                                            //ACTIVE = data.ACTIVE == 1 ? true : false,
                                            ACTIVE = Convert.ToBoolean(data.ACTIVE),
                                            ADDEDBY = data.ADDEDBY,
                                            ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                                            DATEADDED = data.DATEADDED,
                                            MODIFIEDBY = data.MODIFIEDBY,
                                            DATELSTMOD = data.DATELSTMOD,
                                            AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                                        });
                        return iListRes.OrderByDescending(d => d.DATEADDED).ToList();
                        #endregion 
                    }
                    _OrgLevel.Add(new long?(0L));
                }
                #region Get Dept, Div, Coordinator, Exe Coordination, Operation, Director, Director 2
                var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                             join _associateDetail in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _associateDetail.ADEMPCODE
                             join opMapp in _AssetDBContext.SIS_AST_OPERATIONMAPP on _associateDetail.OPERATIONID equals opMapp.ORGID into opMappJoin
                             from _opMapp in opMappJoin.DefaultIfEmpty()
                             join divMapp in _AssetDBContext.SIS_AST_OPERATIONMAPP on _associateDetail.DIVISIONID equals divMapp.ORGID into divMappJoin
                             from _divMapp in divMappJoin.DefaultIfEmpty()
                             join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                             join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                             join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                             join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                             where _process.STATUS_LEVEL == Status_Level && _associateDetail.SYKI == _Syki.SYKIID
                              && (
                                     (_process.STATUS_LEVEL == 2 && _OrgLevel.Contains(_associateDetail.DEPARTMENTID)) ||
                                     ((_process.STATUS_LEVEL == 3 || _process.STATUS_LEVEL == 4) && _OrgLevel.Contains(_associateDetail.DIVISIONID)) ||
                                     (_process.STATUS_LEVEL != 2 && _process.STATUS_LEVEL != 3 && _process.STATUS_LEVEL != 4 &&
                                     (
                                     (_opMapp == null && _divMapp == null && _OrgLevel.Contains(_associateDetail.OPERATIONID)) ||
                                     (_opMapp == null && _divMapp != null && _OrgLevel.Contains(_divMapp.OPERATIONID)) ||
                                     (_opMapp != null && _OrgLevel.Contains(_opMapp.OPERATIONID))
                                     )))
                             && data.ACTIVE == 1
                             select new AssetDisposalViewModel
                             {
                                 DISPOSALHEADERID = data.DISPOSALHEADERID,
                                 ADEMPCODE = data.ADEMPCODE,
                                 EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                                 ASSETTYPEID = data.ASSETTYPEID,
                                 ASSETTYPE = _assetMst.DESCRIPTION,
                                 PROCESSSTATUSID = data.PROCESSSTATUSID,
                                 PROCESSSTATUS = _process.STATUSDESCRIPTION,
                                 BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                                 TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                                 MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                                 REMARKS = data.REMARKS,
                                 //ACTIVE = data.ACTIVE == 1 ? true : false,
                                 ACTIVE = Convert.ToBoolean(data.ACTIVE),
                                 ADDEDBY = data.ADDEDBY,
                                 ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                                 DATEADDED = data.DATEADDED,
                                 MODIFIEDBY = data.MODIFIEDBY,
                                 DATELSTMOD = data.DATELSTMOD,
                                 AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                             });
                #endregion
                if (iList.Count() > 0)
                {
                    FinalList.AddRange(iList);
                }
            }
            return FinalList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByUserApproval(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
           //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                if (ADVM.Approval_History.APPROVALSTATUS == 2)///// Request Sendback
                                {
                                    SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == 0).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                }
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }
        #endregion

        #region IC Associate & IBM Approval
        public List<AssetDisposalViewModel> GetApprovalListByIC(long loginUser)
        {
            var disposalHeaders = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                                   join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                                   join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                                   join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                                   join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                                   join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                                   join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on loginUser equals _AppAuth.EMPCODE
                                   where _process.STATUS_LEVEL == 11 && data.ACTIVE == 1
                                    && _AppAuth.EMPCODE == loginUser && _process.SIS_AST_PROCESSSTATUS_MSTID == _AppAuth.APPROVALTYPE && _AppAuth.ACTIVE == 1
                                    && _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                                   select new
                                   {
                                       Data = data,
                                       AssetMst = _assetMst,
                                       Process = _process,
                                       Emp = _Emp,
                                       AddBy = _AddBy,
                                       VWAssociate = _VWAssociate
                                   }).ToList(); 

            var disposalDetails = _AssetDBContext.SIS_AST_DISPOSALDETAIL
                                  .Where(x => disposalHeaders.Select(d => d.Data.DISPOSALHEADERID).Contains(x.DISPOSALHEADERID) && x.ACTIVE == 1)
                                  .Select(x => new DisposalDetailViewModel
                                  {
                                      DISPOSALDETAILID = x.DISPOSALDETAILID,
                                      DISPOSALHEADERID = x.DISPOSALHEADERID,
                                      ORIGINALCOST = x.ORIGINALCOST,
                                      WDV = (x.ORIGINALCOST - x.DEPRECIATIONCOST)
                                  }).ToList();

                        var iList = disposalHeaders.Select(d => new AssetDisposalViewModel
                        {
                            DISPOSALHEADERID = d.Data.DISPOSALHEADERID,
                            ADEMPCODE = d.Data.ADEMPCODE,
                            EMP_NAME = d.Emp.FIRSTNAME + " " + d.Emp.LASTNAME + " - [" + d.Emp.ADEMPCODE + "]",
                            Emp_Detail = new Employee_Details
                            {
                                _ECode = d.Emp.ADEMPCODE,
                                _EName = d.Emp.FIRSTNAME + " " + d.Emp.LASTNAME,
                                _DOB = DateTime.Now,
                                _SecDescrip = d.VWAssociate.SECTION,
                                _DepDesc = d.VWAssociate.DEPARTMENT,
                                _DivDesc = d.VWAssociate.DIVISION,
                                _OpDesc = d.VWAssociate.OPERATION,
                                _SiteId = d.VWAssociate.SYSITEID
                            },
                            ASSETTYPEID = d.Data.ASSETTYPEID,
                            ASSETTYPE = d.AssetMst.DESCRIPTION,
                            PROCESSSTATUSID = d.Data.PROCESSSTATUSID,
                            PROCESSSTATUS = d.Process.STATUSDESCRIPTION,
                            BACKUP_ATTACHMENT_NAME = d.Data.BACKUP_ATTACHMENT,
                            TRANSFERSLIP_ATTACHMENT_NAME = d.Data.TRANSFERSLIP_ATTACHMENT,
                            MUTILATION_ATTACHMENT_NAME = d.Data.MUTILATION_ATTACHMENT,
                            REMARKS = d.Data.REMARKS,
                            ACTIVE = Convert.ToBoolean(d.Data.ACTIVE),
                            ADDEDBY = d.Data.ADDEDBY,
                            ADDEDBY_NAME = d.AddBy.FIRSTNAME + " " + d.AddBy.LASTNAME,
                            DATEADDED = d.Data.DATEADDED,
                            MODIFIEDBY = d.Data.MODIFIEDBY,
                            DATELSTMOD = d.Data.DATELSTMOD,
                            AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Count(x => x.DISPOSALHEADERID == d.Data.DISPOSALHEADERID && x.ACTIVE == 1),
                            ICMONTH = d.Data.ICMONTH,
                            IMPLEMENTMONTH = d.Data.IMPLEMENTMONTH,
                            DisposalDetailList = disposalDetails.Where(x => x.DISPOSALHEADERID == d.Data.DISPOSALHEADERID).ToList()
                        }).OrderByDescending(d => d.DATEADDED).ToList();


            //var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
            //             join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
            //             join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
            //             join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
            //             join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //             join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
            //             //join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on _VWAssociate.SYSITEID equals _AppAuth.SYSITEID
            //             join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on loginUser equals _AppAuth.EMPCODE
            //             where _process.STATUS_LEVEL == 11 && data.ACTIVE == 1
            //             && _AppAuth.EMPCODE == loginUser && _process.SIS_AST_PROCESSSTATUS_MSTID == _AppAuth.APPROVALTYPE && _AppAuth.ACTIVE == 1
            //             && _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1 
            //             select new AssetDisposalViewModel
            //             {
            //                 DISPOSALHEADERID = data.DISPOSALHEADERID,
            //                 ADEMPCODE = data.ADEMPCODE,
            //                 EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
            //                 Emp_Detail = new Employee_Details
            //                 {
            //                     _ECode = _Emp.ADEMPCODE,
            //                     _EName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
            //                     _DOB = DateTime.Now,
            //                     _SecDescrip = _VWAssociate.SECTION,
            //                     _DepDesc = _VWAssociate.DEPARTMENT,
            //                     _DivDesc = _VWAssociate.DIVISION,
            //                     _OpDesc = _VWAssociate.OPERATION,
            //                     _SiteId = _VWAssociate.SYSITEID
            //                 },
            //                 ASSETTYPEID = data.ASSETTYPEID,
            //                 ASSETTYPE = _assetMst.DESCRIPTION,
            //                 PROCESSSTATUSID = data.PROCESSSTATUSID,
            //                 PROCESSSTATUS = _process.STATUSDESCRIPTION,
            //                 BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
            //                 TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
            //                 MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
            //                 REMARKS = data.REMARKS,
            //                 //ACTIVE = data.ACTIVE == 1 ? true : false,
            //                 ACTIVE = Convert.ToBoolean(data.ACTIVE),
            //                 ADDEDBY = data.ADDEDBY,
            //                 ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                 DATEADDED = data.DATEADDED,
            //                 MODIFIEDBY = data.MODIFIEDBY,
            //                 DATELSTMOD = data.DATELSTMOD,
            //                 AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
            //                 //------------------START-[Code Added by Aumento on 21-Dec-2023]------------------------------
            //                 ICMONTH=data.ICMONTH, // [Added by Aumento on 09-Jan-2024]
            //                 IMPLEMENTMONTH =data.IMPLEMENTMONTH,// [Added by Aumento on 09-Jan-2024]
            //                 DisposalDetailList = (from _disposalDetail in _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID                    && x.ACTIVE == 1)
            //                                        select new DisposalDetailViewModel
            //                                        {
            //                                            DISPOSALDETAILID = _disposalDetail.DISPOSALDETAILID,
            //                                            DISPOSALHEADERID = _disposalDetail.DISPOSALHEADERID,
            //                                            ORIGINALCOST = _disposalDetail.ORIGINALCOST,

            //                                            WDV = (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),

            //                                        }).ToList(),
            //                 //------------------END-[Code Added by Aumento on 21-Dec-2023]--------------------------------
            //             });
            //return iList.OrderByDescending(d => d.DATEADDED).ToList();
            return iList;
        }

        public short RequestUpdateByIC(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Detail Table ----------/////
                        foreach (DisposalDetailViewModel DDVM in ADVM.DisposalDetailList)
                        {
                            SIS_AST_DISPOSALDETAIL SADD = new SIS_AST_DISPOSALDETAIL();
                            if (DDVM.DISPOSALDETAILID > 0)
                            {
                                SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALDETAILID == DDVM.DISPOSALDETAILID).SingleOrDefault();
                                if (SADD != null)
                                {
                                    SADD.IC_STATUS = DDVM.APPROVAL_STATUS;
                                    SADD.MODIFIEDBY = ADVM.MODIFIEDBY;
                                    SADD.DATELSTMOD = DateTime.Now;
                                }
                            }
                            _AssetDBContext.Entry(SADD).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                SADH.ISUPLOADINV_BYTAXATION = ADVM.ISUPLOADINV_BYTAXATION;
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public List<AssetDisposalViewModel> GetApprovalListByIBM(long loginUser)
        {
            string validationParmValue = GetValidationParmValue("IBM Approval");
            long validAssetAmt = string.IsNullOrEmpty(validationParmValue) ? 0 : Convert.ToInt64(validationParmValue);

            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         //join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on _VWAssociate.SYSITEID equals _AppAuth.SYSITEID
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on loginUser equals _AppAuth.EMPCODE
                         where _process.STATUS_LEVEL == 12 && data.ACTIVE == 1
                         && _AppAuth.EMPCODE == loginUser && _process.SIS_AST_PROCESSSTATUS_MSTID == _AppAuth.APPROVALTYPE && _AppAuth.ACTIVE == 1
                         && _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             Emp_Detail = new Employee_Details
                             {
                                 _ECode = _Emp.ADEMPCODE,
                                 _EName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
                                 _DOB = DateTime.Now,
                                 _SecDescrip = _VWAssociate.SECTION,
                                 _DepDesc = _VWAssociate.DEPARTMENT,
                                 _DivDesc = _VWAssociate.DIVISION,
                                 _OpDesc = _VWAssociate.OPERATION,
                                 _SiteId = _VWAssociate.SYSITEID
                             },
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1 && x.ORIGINALCOST <= validAssetAmt).Count(),
                             IBMAssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1 && x.ORIGINALCOST >= validAssetAmt).Count(),
                             IBM_APPDATE = data.IBM_APPDATE,
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByIBM(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Detail Table ----------/////
                        foreach (DisposalDetailViewModel DDVM in ADVM.DisposalDetailList)
                        {
                            SIS_AST_DISPOSALDETAIL SADD = new SIS_AST_DISPOSALDETAIL();
                            if (DDVM.DISPOSALDETAILID > 0)
                            {
                                SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALDETAILID == DDVM.DISPOSALDETAILID).SingleOrDefault();
                                if (SADD != null)
                                {
                                    SADD.IBM_STATUS = DDVM.APPROVAL_STATUS;
                                    SADD.MODIFIEDBY = ADVM.MODIFIEDBY;
                                    SADD.DATELSTMOD = DateTime.Now;
                                }
                            }
                            _AssetDBContext.Entry(SADD).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }
        #endregion

        //Added by TTL :: SR95154 | CR6236
        #region Plant PPC Approval
        public List<AssetDisposalViewModel> GetListForPlantPPCApproval(long loginUser, string plantId)
        {
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE // Added by TTL :: CR6754
                         //join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID // Changed by TTL :: CR6754
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on _VWAssociate.OPERATIONID equals _AppAuth.ADORGLEVELID // Changed by TTL :: CR6754
                         where _process.STATUS_LEVEL == 28 && data.ACTIVE == 1 ////--- 22 for Maintenance Dept.
                         && _AppAuth.EMPCODE == loginUser && _AppAuth.ACTIVE == 1
                         && _AppAuth.APPROVALTYPE == _process.SIS_AST_PROCESSSTATUS_MSTID
                         && _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1 && _VWAssociate.SYSITEID == _AppAuth.SYSITEID // Added by TTL :: CR6754
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }
        public short RequestUpdateByPlantPPC(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            using (var transaction = _AssetDBContext.Database.BeginTransaction())           
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                if (ADVM.Approval_History.APPROVALSTATUS == 2)///// Request Sendback
                                {
                                    SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == 0).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                }
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }
        #endregion
        //End by TTL :: SR95154 | CR6236

        #region Plant Mgf. & Quality Approval
        public AssetDisposalViewModel GetAssetRequestByMgf(long id)
        {
            var _obj = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == id)
                        join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                        join _customerJoin in _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL on data.DISPOSALHEADERID equals _customerJoin.DISPOSALHEADERID into _customerJoin
                        from _customerDetail in _customerJoin.DefaultIfEmpty()
                        join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                        join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                        join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                        where _VWAssociate.SYKI == _Syki.SYKIID
                        select new AssetDisposalViewModel
                        {
                            DISPOSALHEADERID = data.DISPOSALHEADERID,
                            ADEMPCODE = data.ADEMPCODE,
                            EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                            ASSETTYPEID = data.ASSETTYPEID,
                            ASSETTYPE = _assetMst.DESCRIPTION,
                            PROCESSSTATUSID = data.PROCESSSTATUSID,
                            PROCESSSTATUS = _process.STATUSDESCRIPTION,
                            STATUS_LEVEL = _process.STATUS_LEVEL,
                            BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                            TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                            MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                            INVOICE_ATTACHMENT = data.INVOICE_ATTACHMENT,
                            REMARKS = data.REMARKS,
                            //ACTIVE = data.ACTIVE == 1 ? true : false,
                            ACTIVE = Convert.ToBoolean(data.ACTIVE),
                            ADDEDBY = data.ADDEDBY,
                            ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            MODIFIEDBY = data.MODIFIEDBY,
                            DATELSTMOD = data.DATELSTMOD,
                            IsPlantRequired = data.ISPLANTREQUIRED,
                            P1FAppStatus = data.P1F_APPSTATUS,
                            P2FAppStatus = data.P2F_APPSTATUS,
                            P3FAppStatus = data.P3F_APPSTATUS,
                            P4FAppStatus = data.P4F_APPSTATUS,
                            P1F_ATTACHMENT_NAME = data.P1F_ATTACHMENT,
                            P2F_ATTACHMENT_NAME = data.P2F_ATTACHMENT,
                            P3F_ATTACHMENT_NAME = data.P3F_ATTACHMENT,
                            P4F_ATTACHMENT_NAME = data.P4F_ATTACHMENT,
                            PLANTID = data.PLANTID,
                            Emp_Detail = new Employee_Details
                            {
                                _ECode = _Emp.ADEMPCODE,
                                _EName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
                                _DOB = DateTime.Now,
                                _SecDescrip = _VWAssociate.SECTION,
                                _DepDesc = _VWAssociate.DEPARTMENT,
                                _DivDesc = _VWAssociate.DIVISION,
                                _OpDesc = _VWAssociate.OPERATION,
                                _SiteId = _VWAssociate.SYSITEID,
                                _SecId = _VWAssociate.SECTIONID,
                                _DepId = _VWAssociate.DEPARTMENTID,
                                _DivId = _VWAssociate.DIVISIONID,
                                _OpId = _VWAssociate.OPERATIONID,
                            },
                        }).FirstOrDefault();
            
            // Fetch DisposalDetailList separately
            if (_obj != null)
            {
                _obj.CustomerDetail =
                       (from _customerJoin in _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL
                        where _customerJoin.DISPOSALHEADERID == _obj.DISPOSALHEADERID
                        select new DisposalCustomerViewModel
                        {
                            DISPOSALCUSTDETAILID = _customerJoin.DISPOSALCUSTDETAILID,
                            DISPOSALHEADERID = _customerJoin.DISPOSALHEADERID,
                            CUSTOMERCODE = _customerJoin.CUSTOMERCODE,
                            CUSTOMERNAME = _customerJoin.CUSTOMERNAME,
                            CUSTADDRESS = _customerJoin.CUSTADDRESS,
                            CUSTGSTIN = _customerJoin.CUSTGSTIN,
                            GSTRATE = _customerJoin.GSTRATE,
                            BASICVALUE = _customerJoin.BASICVALUE,
                            PAYMENTDETAILS = _customerJoin.PAYMENTDETAILS,
                            ACTIVE = Convert.ToBoolean(_customerJoin.ACTIVE)
                        }).FirstOrDefault();

                _obj.DisposalDetailList = (from _disposalDetail in _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == _obj.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1)
                                           join _ConditionJoin in _AssetDBContext.SIS_ASSETCONDITION on _disposalDetail.CONDITIONID equals _ConditionJoin.CONDITIONID into _ConditionJoin
                                           from _ConditionMst in _ConditionJoin.DefaultIfEmpty()
                                           join _MgfJoin in _AssetDBContext.SIS_AST_MGFPLANTAPPROVAL on _disposalDetail.DISPOSALDETAILID equals _MgfJoin.DISPOSALDETAILID into _MgfJoin
                                           from _MgfPlanApp in _MgfJoin.DefaultIfEmpty()
                                           join _siteJoin in _AssetDBContext.SYSITE on _disposalDetail.PLANTID equals _siteJoin.SYSITEID into _site_Join
                                           from _siteApp in _site_Join.DefaultIfEmpty()
                                           select new DisposalDetailViewModel
                                           {
                                               DISPOSALDETAILID = _disposalDetail.DISPOSALDETAILID,
                                               DISPOSALHEADERID = _disposalDetail.DISPOSALHEADERID,
                                               ASSETCODE = _disposalDetail.ASSETCODE,
                                               ASSETDESCRIPTION = _disposalDetail.ASSETDESCRIPTION,
                                               VENDORCODE = _disposalDetail.VENDORCODE,
                                               VENDORNAME = _disposalDetail.VENDORNAME,
                                               INVOICENO = _disposalDetail.INVOICENO,
                                               INVOICEDATE = _disposalDetail.INVOICEDATE,
                                               //BILLENTRY = string.IsNullOrEmpty(_disposalDetail.BILLENTRY) ? "N/A" : _disposalDetail.BILLENTRY,
                                               //LICENSENO = string.IsNullOrEmpty(_disposalDetail.LICENSENO) ? "N/A" : _disposalDetail.LICENSENO,
                                               BILLENTRY = _disposalDetail.BILLENTRY ?? "",
                                               LICENSENO = _disposalDetail.LICENSENO ?? "",
                                               PURCHASEDDATE = _disposalDetail.PURCHASEDDATE,
                                               ORIGINALCOST = _disposalDetail.ORIGINALCOST,
                                               DEPRECIATIONCOST = _disposalDetail.DEPRECIATIONCOST,
                                               SALESPRICE = _disposalDetail.SALESPRICE,
                                               MGF_QLTYSTATUS = _disposalDetail.MGF_QLTYSTATUS,
                                               EVNSTATUS = _disposalDetail.EVNSTATUS,
                                               TAXTION_STATUS = _disposalDetail.TAXTION_STATUS,
                                               MAINTENANCEDEPT_STATUS = _disposalDetail.MAINTENANCEDEPT_STATUS,
                                               IC_STATUS = _disposalDetail.IC_STATUS,
                                               IBM_STATUS = _disposalDetail.IBM_STATUS,
                                               //ACTIVE = _disposalDetail.ACTIVE == 1 ? true : false,
                                               ACTIVE = Convert.ToBoolean(_disposalDetail.ACTIVE),
                                               WDV = (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
                                               PROFIT_LOSS = (_disposalDetail.SALESPRICE == null ? 0.00M : _disposalDetail.SALESPRICE) - (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
                                               VALUATION_GST = _disposalDetail.VALUATION_GST,
                                               TENTATIVE = _disposalDetail.TENTATIVE,
                                               PARTIAL_FULL = _disposalDetail.PARTIAL_FULL,
                                               VIBRATION_SENSOR = _disposalDetail.VIBRATION_SENSOR,
                                               ASSET_CATEGORY = _disposalDetail.ASSET_CATEGORY,
                                               CONDITIONID = _disposalDetail.CONDITIONID,
                                               ASSET_CONDITION = _ConditionMst.DESCRIPTION,
                                               //C1F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C1F_APPROVEBY,
                                               //C2F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C2F_APPROVEBY,
                                               //C3F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C3F_APPROVEBY,
                                               //C4F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C4F_APPROVEBY,
                                               //CONDITION_1F = _MgfPlanApp == null ? "" : _MgfPlanApp.C1F_CONDITION,
                                               //CONDITION_2F = _MgfPlanApp == null ? "" : _MgfPlanApp.C2F_CONDITION,
                                               //CONDITION_3F = _MgfPlanApp == null ? "" : _MgfPlanApp.C3F_CONDITION,
                                               //CONDITION_4F = _MgfPlanApp == null ? "" : _MgfPlanApp.C4F_CONDITION,

                                               C1F_APPROVEBY = _MgfPlanApp.C1F_APPROVEBY,
                                               C2F_APPROVEBY = _MgfPlanApp.C2F_APPROVEBY,
                                               C3F_APPROVEBY = _MgfPlanApp.C3F_APPROVEBY,
                                               C4F_APPROVEBY = _MgfPlanApp.C4F_APPROVEBY,

                                               CONDITION_1F = _MgfPlanApp.C1F_CONDITION ?? "",
                                               CONDITION_2F = _MgfPlanApp.C2F_CONDITION ?? "",
                                               CONDITION_3F = _MgfPlanApp.C3F_CONDITION ?? "",
                                               CONDITION_4F = _MgfPlanApp.C4F_CONDITION ?? "",

                                               SERIALNUMBER = _disposalDetail.SERIALNUMBER,
                                               PONUMBER = _disposalDetail.PONUMBER,
                                               PLANTID = _disposalDetail.PLANTID,
                                               //PLANTNAME = _siteApp == null ? "" : _siteApp.DESCRIP,
                                               PLANTNAME = _siteApp.DESCRIP ?? "",
                                               CAPTALIZED_DATE = _disposalDetail.CAPTALIZED_DATE,
                                           }).ToList();

                _obj.AppHistoryList = (from _disposalAppHis in _AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.Where(x => x.DISPOSALHEADERID == _obj.DISPOSALHEADERID)
                                       join _processStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on _disposalAppHis.PROCESSSTATUSID equals _processStatus.SIS_AST_PROCESSSTATUS_MSTID
                                       join _AppEmp in _AssetDBContext.ADEMPLOYEE on _disposalAppHis.APPCODE equals _AppEmp.ADEMPCODE
                                       select new ApprovalHisViewModel
                                       {
                                           DISPOSALHEADERID = _disposalAppHis.DISPOSALHEADERID,
                                           PROCESSSTATUSID = _disposalAppHis.PROCESSSTATUSID,
                                           APPROVALHISTORYID = _disposalAppHis.APPROVALHISTORYID,
                                           APPROVALSTATUS = _disposalAppHis.APPROVALSTATUS,
                                           STATUS_LEVEL = _processStatus.STATUS_LEVEL,
                                           ARS_MSG = ((_processStatus.STATUS_LEVEL == 15 || _processStatus.STATUS_LEVEL == 16 || _processStatus.STATUS_LEVEL == 17 || _processStatus.STATUS_LEVEL == 25) ? "Uploaded" : (_disposalAppHis.APPROVALSTATUS == 1 ? "Approved By" : _disposalAppHis.APPROVALSTATUS == 2 ? "Sendback By" : _disposalAppHis.APPROVALSTATUS == 3 ? "Rejected By" : "")),
                                           DISPLAY_MSG = _processStatus.DISPLAY_MSG,
                                           ATTACHMENT = _disposalAppHis.ATTACHMENT,
                                           APPCODE = _disposalAppHis.APPCODE,
                                           APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                                           REMARKS = _disposalAppHis.REMARKS,
                                           DATEADDED = _disposalAppHis.DATEADDED,
                                           ADDEDBY = _disposalAppHis.ADDEDBY,
                                           SEQ_NUMBER = _processStatus.SEQ_NUMBER
                                       }).OrderBy(o => o.DATEADDED).ToList();
            }


            //var _obj = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == id)
            //            join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
            //            join _customerJoin in _AssetDBContext.SIS_AST_DISPOSALCUSTDETAIL on data.DISPOSALHEADERID equals _customerJoin.DISPOSALHEADERID into _customerJoin
            //            from _customerDetail in _customerJoin.DefaultIfEmpty()
            //            join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
            //            join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
            //            join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //            join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
            //            where _VWAssociate.SYKI == _Syki.SYKIID
            //            select new AssetDisposalViewModel
            //            {
            //                DISPOSALHEADERID = data.DISPOSALHEADERID,
            //                ADEMPCODE = data.ADEMPCODE,
            //                EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
            //                ASSETTYPEID = data.ASSETTYPEID,
            //                ASSETTYPE = _assetMst.DESCRIPTION,
            //                PROCESSSTATUSID = data.PROCESSSTATUSID,
            //                PROCESSSTATUS = _process.STATUSDESCRIPTION,
            //                STATUS_LEVEL = _process.STATUS_LEVEL,
            //                BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
            //                TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
            //                MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
            //                INVOICE_ATTACHMENT = data.INVOICE_ATTACHMENT,
            //                REMARKS = data.REMARKS,
            //                //ACTIVE = data.ACTIVE == 1 ? true : false,
            //                ACTIVE = Convert.ToBoolean(data.ACTIVE),
            //                ADDEDBY = data.ADDEDBY,
            //                ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                DATEADDED = data.DATEADDED,
            //                MODIFIEDBY = data.MODIFIEDBY,
            //                DATELSTMOD = data.DATELSTMOD,
            //                IsPlantRequired = data.ISPLANTREQUIRED,
            //                P1FAppStatus = data.P1F_APPSTATUS,
            //                P2FAppStatus = data.P2F_APPSTATUS,
            //                P3FAppStatus = data.P3F_APPSTATUS,
            //                P4FAppStatus = data.P4F_APPSTATUS,
            //                P1F_ATTACHMENT_NAME = data.P1F_ATTACHMENT,
            //                P2F_ATTACHMENT_NAME = data.P2F_ATTACHMENT,
            //                P3F_ATTACHMENT_NAME = data.P3F_ATTACHMENT,
            //                P4F_ATTACHMENT_NAME = data.P4F_ATTACHMENT,
            //                PLANTID = data.PLANTID,
            //                Emp_Detail = new Employee_Details
            //                {
            //                    _ECode = _Emp.ADEMPCODE,
            //                    _EName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
            //                    _DOB = DateTime.Now,
            //                    _SecDescrip = _VWAssociate.SECTION,
            //                    _DepDesc = _VWAssociate.DEPARTMENT,
            //                    _DivDesc = _VWAssociate.DIVISION,
            //                    _OpDesc = _VWAssociate.OPERATION,
            //                    _SiteId = _VWAssociate.SYSITEID,
            //                    _SecId = _VWAssociate.SECTIONID,
            //                    _DepId = _VWAssociate.DEPARTMENTID,
            //                    _DivId = _VWAssociate.DIVISIONID,
            //                    _OpId = _VWAssociate.OPERATIONID,
            //                },
            //                CustomerDetail = _customerDetail == null ? null : new DisposalCustomerViewModel
            //                {
            //                    DISPOSALCUSTDETAILID = _customerDetail.DISPOSALCUSTDETAILID,
            //                    DISPOSALHEADERID = _customerDetail.DISPOSALHEADERID,
            //                    CUSTOMERCODE = _customerDetail.CUSTOMERCODE,
            //                    CUSTOMERNAME = _customerDetail.CUSTOMERNAME,
            //                    CUSTADDRESS = _customerDetail.CUSTADDRESS,
            //                    CUSTGSTIN = _customerDetail.CUSTGSTIN,
            //                    GSTRATE = _customerDetail.GSTRATE,
            //                    BASICVALUE = _customerDetail.BASICVALUE,
            //                    PAYMENTDETAILS = _customerDetail.PAYMENTDETAILS,
            //                    //ACTIVE = _customerDetail.ACTIVE == 1 ? true : false,
            //                    ACTIVE = Convert.ToBoolean(_customerDetail.ACTIVE),
            //                },
            //                DisposalDetailList = (from _disposalDetail in _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1)
            //                                      join _ConditionJoin in _AssetDBContext.SIS_ASSETCONDITION on _disposalDetail.CONDITIONID equals _ConditionJoin.CONDITIONID into _ConditionJoin
            //                                      from _ConditionMst in _ConditionJoin.DefaultIfEmpty()
            //                                      join _MgfJoin in _AssetDBContext.SIS_AST_MGFPLANTAPPROVAL on _disposalDetail.DISPOSALDETAILID equals _MgfJoin.DISPOSALDETAILID into _MgfJoin
            //                                      from _MgfPlanApp in _MgfJoin.DefaultIfEmpty()
            //                                      join _siteJoin in _AssetDBContext.SYSITE on _disposalDetail.PLANTID equals _siteJoin.SYSITEID into _site_Join
            //                                      from _siteApp in _site_Join.DefaultIfEmpty()
            //                                      select new DisposalDetailViewModel
            //                                      {
            //                                          DISPOSALDETAILID = _disposalDetail.DISPOSALDETAILID,
            //                                          DISPOSALHEADERID = _disposalDetail.DISPOSALHEADERID,
            //                                          ASSETCODE = _disposalDetail.ASSETCODE,
            //                                          ASSETDESCRIPTION = _disposalDetail.ASSETDESCRIPTION,
            //                                          VENDORCODE = _disposalDetail.VENDORCODE,
            //                                          VENDORNAME = _disposalDetail.VENDORNAME,
            //                                          INVOICENO = _disposalDetail.INVOICENO,
            //                                          INVOICEDATE = _disposalDetail.INVOICEDATE,
            //                                          BILLENTRY = string.IsNullOrEmpty(_disposalDetail.BILLENTRY) ? "N/A" : _disposalDetail.BILLENTRY,
            //                                          LICENSENO = string.IsNullOrEmpty(_disposalDetail.LICENSENO) ? "N/A" : _disposalDetail.LICENSENO,
            //                                          PURCHASEDDATE = _disposalDetail.PURCHASEDDATE,
            //                                          ORIGINALCOST = _disposalDetail.ORIGINALCOST,
            //                                          DEPRECIATIONCOST = _disposalDetail.DEPRECIATIONCOST,
            //                                          SALESPRICE = _disposalDetail.SALESPRICE,
            //                                          MGF_QLTYSTATUS = _disposalDetail.MGF_QLTYSTATUS,
            //                                          EVNSTATUS = _disposalDetail.EVNSTATUS,
            //                                          TAXTION_STATUS = _disposalDetail.TAXTION_STATUS,
            //                                          MAINTENANCEDEPT_STATUS = _disposalDetail.MAINTENANCEDEPT_STATUS,
            //                                          IC_STATUS = _disposalDetail.IC_STATUS,
            //                                          IBM_STATUS = _disposalDetail.IBM_STATUS,
            //                                          ACTIVE = _disposalDetail.ACTIVE == 1 ? true : false,
            //                                          WDV = (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
            //                                          PROFIT_LOSS = (_disposalDetail.SALESPRICE == null ? 0.00M : _disposalDetail.SALESPRICE) - (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),
            //                                          VALUATION_GST = _disposalDetail.VALUATION_GST,
            //                                          TENTATIVE = _disposalDetail.TENTATIVE,
            //                                          PARTIAL_FULL = _disposalDetail.PARTIAL_FULL,
            //                                          VIBRATION_SENSOR = _disposalDetail.VIBRATION_SENSOR,
            //                                          ASSET_CATEGORY = _disposalDetail.ASSET_CATEGORY,
            //                                          CONDITIONID = _disposalDetail.CONDITIONID,
            //                                          ASSET_CONDITION = _ConditionMst.DESCRIPTION,
            //                                          C1F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C1F_APPROVEBY,
            //                                          C2F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C2F_APPROVEBY,
            //                                          C3F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C3F_APPROVEBY,
            //                                          C4F_APPROVEBY = _MgfPlanApp == null ? null : _MgfPlanApp.C4F_APPROVEBY,
            //                                          CONDITION_1F = _MgfPlanApp == null ? "" : _MgfPlanApp.C1F_CONDITION,
            //                                          CONDITION_2F = _MgfPlanApp == null ? "" : _MgfPlanApp.C2F_CONDITION,
            //                                          CONDITION_3F = _MgfPlanApp == null ? "" : _MgfPlanApp.C3F_CONDITION,
            //                                          CONDITION_4F = _MgfPlanApp == null ? "" : _MgfPlanApp.C4F_CONDITION,
            //                                          SERIALNUMBER = _disposalDetail.SERIALNUMBER,
            //                                          PONUMBER = _disposalDetail.PONUMBER,
            //                                          PLANTID = _disposalDetail.PLANTID,
            //                                          PLANTNAME = _siteApp == null ? "" : _siteApp.DESCRIP,
            //                                          CAPTALIZED_DATE = _disposalDetail.CAPTALIZED_DATE,
            //                                      }).ToList(),
            //                AppHistoryList = (from _disposalAppHis in _AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID)
            //                                  join _processStatus in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on _disposalAppHis.PROCESSSTATUSID equals _processStatus.SIS_AST_PROCESSSTATUS_MSTID
            //                                  join _AppEmp in _AssetDBContext.ADEMPLOYEE on _disposalAppHis.APPCODE equals _AppEmp.ADEMPCODE
            //                                  select new ApprovalHisViewModel
            //                                  {
            //                                      DISPOSALHEADERID = _disposalAppHis.DISPOSALHEADERID,
            //                                      PROCESSSTATUSID = _disposalAppHis.PROCESSSTATUSID,
            //                                      APPROVALHISTORYID = _disposalAppHis.APPROVALHISTORYID,
            //                                      APPROVALSTATUS = _disposalAppHis.APPROVALSTATUS,
            //                                      STATUS_LEVEL = _processStatus.STATUS_LEVEL,
            //                                      ARS_MSG = ((_processStatus.STATUS_LEVEL == 15 || _processStatus.STATUS_LEVEL == 16 || _processStatus.STATUS_LEVEL == 17 || _processStatus.STATUS_LEVEL == 25) ? "Uploaded" : (_disposalAppHis.APPROVALSTATUS == 1 ? "Approved By" : _disposalAppHis.APPROVALSTATUS == 2 ? "Sendback By" : _disposalAppHis.APPROVALSTATUS == 3 ? "Rejected By" : "")),
            //                                      DISPLAY_MSG = _processStatus.DISPLAY_MSG,
            //                                      ATTACHMENT = _disposalAppHis.ATTACHMENT,
            //                                      APPCODE = _disposalAppHis.APPCODE,
            //                                      APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
            //                                      REMARKS = _disposalAppHis.REMARKS,
            //                                      DATEADDED = _disposalAppHis.DATEADDED,
            //                                      ADDEDBY = _disposalAppHis.ADDEDBY,
            //                                      SEQ_NUMBER = _processStatus.SEQ_NUMBER
            //                                  }).OrderBy(o => o.DATEADDED).ToList(),
            //            }).FirstOrDefault();
            ////_obj.TOTAL_AMOUNT = (long)_obj.DisposalDetailList.Sum(s => s.ORIGINALCOST);
            _obj.TOTAL_AMOUNT = (long)_obj.DisposalDetailList.Max(s => s.ORIGINALCOST).Value;
            return _obj;
        }

        public List<AssetDisposalViewModel> GetApprovalListByPlanQty(long loginUser)
        {
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         //join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == 8 && data.ACTIVE == 1
                         && _AppAuth.EMPCODE == loginUser && _AppAuth.ACTIVE == 1
                         && _AppAuth.APPROVALTYPE == _process.SIS_AST_PROCESSSTATUS_MSTID
                         //&& _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByQty(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Detail Table ----------/////
                        foreach (DisposalDetailViewModel DDVM in ADVM.DisposalDetailList)
                        {
                            SIS_AST_DISPOSALDETAIL SADD = new SIS_AST_DISPOSALDETAIL();
                            if (DDVM.DISPOSALDETAILID > 0)
                            {
                                SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALDETAILID == DDVM.DISPOSALDETAILID).SingleOrDefault();
                                if (SADD != null)
                                {
                                    SADD.MGF_QLTYSTATUS = DDVM.APPROVAL_STATUS;
                                    SADD.MODIFIEDBY = ADVM.MODIFIEDBY;
                                    SADD.DATELSTMOD = DateTime.Now;
                                }
                            }
                            _AssetDBContext.Entry(SADD).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public List<AssetDisposalViewModel> GetApprovalListByPlanMgf(long loginUser)
        {
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         //join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == 7 && data.ACTIVE == 1
                         && _AppAuth.EMPCODE == loginUser && _AppAuth.ACTIVE == 1
                         && _AppAuth.APPROVALTYPE == _process.SIS_AST_PROCESSSTATUS_MSTID
                         //&& _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByMgf(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Detail Table ----------/////
                        foreach (DisposalDetailViewModel DDVM in ADVM.DisposalDetailList)
                        {
                            SIS_AST_DISPOSALDETAIL SADD = new SIS_AST_DISPOSALDETAIL();
                            if (DDVM.DISPOSALDETAILID > 0)
                            {
                                SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALDETAILID == DDVM.DISPOSALDETAILID).SingleOrDefault();
                                if (SADD != null)
                                {
                                    SADD.MGF_QLTYSTATUS = DDVM.APPROVAL_STATUS;
                                    SADD.MODIFIEDBY = ADVM.MODIFIEDBY;
                                    SADD.DATELSTMOD = DateTime.Now;
                                }
                            }
                            _AssetDBContext.Entry(SADD).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public short UpdatePlantApprovalRequiredByMgf(AssetDisposalViewModel ADVM, string plantId)
        {
            short num1 = 0;
            //using (DbContextTransaction contextTransaction = this._AssetDBContext.Database.BeginTransaction())
            using (var contextTransaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        SIS_AST_DISPOSALHEADER astDisposalheader = new SIS_AST_DISPOSALHEADER();
                        astDisposalheader = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                        if (astDisposalheader != null)
                        {
                            short? isPlantRequired = ADVM.IsPlantRequired;
                            int? nullable = isPlantRequired.HasValue ? new int?((int)isPlantRequired.GetValueOrDefault()) : new int?();
                            int num2 = 1;
                            if (nullable.GetValueOrDefault() == num2 && nullable.HasValue)
                            {
                                astDisposalheader.P1F_APPSTATUS = 0;
                                astDisposalheader.P2F_APPSTATUS = 0;
                                astDisposalheader.P3F_APPSTATUS = 0;
                                astDisposalheader.P4F_APPSTATUS = 0;

                                ////---- Get Requestor plant id
                                plantId = Convert.ToString(_AssetDBContext.VW_ASSOCIATELVLDETAILS.Where(v => v.ADEMPCODE == astDisposalheader.ADEMPCODE && v.SYKI == _Syki.SYKIID).Select(s => s.SYPLANTID).FirstOrDefault());

                                if (plantId == "1")
                                    astDisposalheader.P1F_APPSTATUS = 1;
                                else if (plantId == "2")
                                    astDisposalheader.P2F_APPSTATUS = 1;
                                else if (plantId == "3")
                                    astDisposalheader.P3F_APPSTATUS = 1;
                                else if (plantId == "4")
                                    astDisposalheader.P4F_APPSTATUS = 1;
                            }
                            astDisposalheader.ISPLANTREQUIRED = ADVM.IsPlantRequired;
                            astDisposalheader.MODIFIEDBY = ADVM.MODIFIEDBY;
                            astDisposalheader.DATELSTMOD = DateTime.Now;
                            _AssetDBContext.Entry<SIS_AST_DISPOSALHEADER>(astDisposalheader).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }
                        contextTransaction.Commit();
                        num1 = (short)1;
                    }
                }
                catch (Exception ex)
                {
                    num1 = (short)-1;
                    contextTransaction.Rollback();
                }
                return num1;
            }
        }

        public List<AssetDisposalViewModel> GetListForPlantApproval(long loginUser, string plantId)
        {
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         //join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on _VWAssociate.SYSITEID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == 7 && data.ACTIVE == 1
                         && (plantId == "1" ? data.P1F_APPSTATUS == 0 : true) && (plantId == "2" ? data.P2F_APPSTATUS == 0 : true)
                         && (plantId == "3" ? data.P3F_APPSTATUS == 0 : true) && (plantId == "4" ? data.P4F_APPSTATUS == 0 : true)
                         && _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByPlantApproval(AssetDisposalViewModel ADVM, long loginUser, string plantId)
        {
            short num1 = 0;
            //using (DbContextTransaction contextTransaction = this._AssetDBContext.Database.BeginTransaction())
            using (var contextTransaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0L)
                    {
                        foreach (DisposalDetailViewModel disposalDetail in (IEnumerable<DisposalDetailViewModel>)ADVM.DisposalDetailList)
                        {
                            DisposalDetailViewModel DDVM = disposalDetail;
                            SIS_AST_MGFPLANTAPPROVAL entity = new SIS_AST_MGFPLANTAPPROVAL();
                            int num2 = 0;
                            if (DDVM.DISPOSALDETAILID > 0L)
                                entity = this._AssetDBContext.SIS_AST_MGFPLANTAPPROVAL.Where(x => x.DISPOSALDETAILID == (long?)DDVM.DISPOSALDETAILID).SingleOrDefault();
                            if (entity == null)
                            {
                                entity = new SIS_AST_MGFPLANTAPPROVAL();
                                if (this._AssetDBContext.SIS_AST_MGFPLANTAPPROVAL.ToList<SIS_AST_MGFPLANTAPPROVAL>().Count == 0)
                                    entity.MGFPLANTAPPID = 1L;
                                else
                                    entity.MGFPLANTAPPID = this._AssetDBContext.SIS_AST_MGFPLANTAPPROVAL.Max(x => x.MGFPLANTAPPID) + 1L;
                                num2 = 1;
                            }
                            entity.DISPOSALDETAILID = new long?(DDVM.DISPOSALDETAILID);
                            if (plantId == "1")
                            {
                                entity.C1F_CONDITION = DDVM.ASSET_REQUIRED;
                                entity.C1F_APPROVEBY = new long?(loginUser);
                            }
                            else if (plantId == "2")
                            {
                                entity.C2F_CONDITION = DDVM.ASSET_REQUIRED;
                                entity.C2F_APPROVEBY = new long?(loginUser);
                            }
                            else if (plantId == "3")
                            {
                                entity.C3F_CONDITION = DDVM.ASSET_REQUIRED;
                                entity.C3F_APPROVEBY = new long?(loginUser);
                            }
                            else if (plantId == "4")
                            {
                                entity.C4F_CONDITION = DDVM.ASSET_REQUIRED;
                                entity.C4F_APPROVEBY = new long?(loginUser);
                            }
                            entity.ACTIVE = 1;
                            if (num2 == 1)
                            {
                                entity.ADDEDBY = loginUser;
                                entity.DATEADDED = DateTime.Now;
                            }
                            else
                            {
                                entity.MODIFIEDBY = new long?(loginUser);
                                entity.DATELSTMOD = new DateTime?(DateTime.Now);
                            }
                            this._AssetDBContext.Entry<SIS_AST_MGFPLANTAPPROVAL>(entity).State = num2 == 1 ? EntityState.Added : EntityState.Modified;
                            this._AssetDBContext.SaveChanges();
                        }
                        SIS_AST_DISPOSALHEADER astDisposalheader = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0L)
                        {
                            SIS_AST_DISPOSALHEADER entity = this._AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (entity != null)
                            {
                                if (plantId == "1")
                                {
                                    entity.P1F_APPSTATUS = new short?((short)1);
                                    entity.P1F_ATTACHMENT = ADVM.INVOICE_ATTACHMENT_NAME;
                                }
                                else if (plantId == "2")
                                {
                                    entity.P2F_APPSTATUS = new short?((short)1);
                                    entity.P2F_ATTACHMENT = ADVM.INVOICE_ATTACHMENT_NAME;
                                }
                                else if (plantId == "3")
                                {
                                    entity.P3F_APPSTATUS = new short?((short)1);
                                    entity.P3F_ATTACHMENT = ADVM.INVOICE_ATTACHMENT_NAME;
                                }
                                else if (plantId == "4")
                                {
                                    entity.P4F_APPSTATUS = new short?((short)1);
                                    entity.P4F_ATTACHMENT = ADVM.INVOICE_ATTACHMENT_NAME;
                                }
                                entity.MODIFIEDBY = ADVM.MODIFIEDBY;
                                entity.DATELSTMOD = new DateTime?(DateTime.Now);
                                this._AssetDBContext.Entry<SIS_AST_DISPOSALHEADER>(entity).State = EntityState.Modified;
                                this._AssetDBContext.SaveChanges();
                            }
                        }
                        contextTransaction.Commit();
                        num1 = (short)1;
                    }
                }
                catch (Exception ex)
                {
                    num1 = (short)-1;
                    contextTransaction.Rollback();
                }
                return num1;
            }
        }
        #endregion

        #region Environment / Admin Approval
        public List<AssetDisposalViewModel> GetApprovalListByEnvironment(long loginUser)
        {
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         //join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == 13 && data.ACTIVE == 1 && _AppAuth.EMPCODE == loginUser
                         && _process.SIS_AST_PROCESSSTATUS_MSTID == _AppAuth.APPROVALTYPE
                         && _AppAuth.ACTIVE == 1 && data.ISUPLOADINV_BYTAXATION == 0
                         //&& _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByEnvironment(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Detail Table ----------/////
                        foreach (DisposalDetailViewModel DDVM in ADVM.DisposalDetailList)
                        {
                            SIS_AST_DISPOSALDETAIL SADD = new SIS_AST_DISPOSALDETAIL();
                            if (DDVM.DISPOSALDETAILID > 0)
                            {
                                SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALDETAILID == DDVM.DISPOSALDETAILID).SingleOrDefault();
                                if (SADD != null)
                                {
                                    SADD.EVNSTATUS = DDVM.APPROVAL_STATUS;
                                    SADD.MODIFIEDBY = ADVM.MODIFIEDBY;
                                    SADD.DATELSTMOD = DateTime.Now;
                                }
                            }
                            _AssetDBContext.Entry(SADD).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public List<AssetDisposalViewModel> GetApprovalListByAdmin(long loginUser)
        {
              var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         //join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == 14 && data.ACTIVE == 1 && _AppAuth.EMPCODE == loginUser
                         && _process.SIS_AST_PROCESSSTATUS_MSTID == _AppAuth.APPROVALTYPE
                         && _AppAuth.ACTIVE == 1
                         //&& _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByAdmin(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Detail Table ----------/////
                        foreach (DisposalDetailViewModel DDVM in ADVM.DisposalDetailList)
                        {
                            SIS_AST_DISPOSALDETAIL SADD = new SIS_AST_DISPOSALDETAIL();
                            if (DDVM.DISPOSALDETAILID > 0)
                            {
                                SADD = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALDETAILID == DDVM.DISPOSALDETAILID).SingleOrDefault();
                                if (SADD != null)
                                {
                                    SADD.EVNSTATUS = DDVM.APPROVAL_STATUS;
                                    SADD.MODIFIEDBY = ADVM.MODIFIEDBY;
                                    SADD.DATELSTMOD = DateTime.Now;
                                }
                            }
                            _AssetDBContext.Entry(SADD).State = EntityState.Modified;
                            _AssetDBContext.SaveChanges();
                        }

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }
        #endregion

        #region Security Approval
        public List<AssetDisposalViewModel> GetApprovalListBySecurity(long loginUser)
        {
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         //join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == 18 && data.ACTIVE == 1
                         && _AppAuth.EMPCODE == loginUser && _AppAuth.ACTIVE == 1
                         && _process.SIS_AST_PROCESSSTATUS_MSTID == _AppAuth.APPROVALTYPE
                         //&& _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateBySecurity(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        //// Get ProcessStatusId in Case Of Sendback 
                        long _processStatusId = (long)_AssetDBContext.SIS_AST_DISPOSALAPPHISTORY.Where(h => h.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).OrderByDescending(o => o.DATEADDED).Select(s => s.PROCESSSTATUSID).FirstOrDefault();

                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)///// Request Reject
                                {
                                    Status_Level = Convert.ToInt16(21);
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                if (ADVM.Approval_History.APPROVALSTATUS == 2)///// Request Sendback to last approval level
                                {
                                    SADH.PROCESSSTATUSID = _processStatusId;
                                }
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }

                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }
        #endregion

        #region Upload Invoice
        public List<AssetDisposalViewModel> GetApprovalListForUploadInvoice(int Status_Level, long loginUser)
        {           
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         //join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == Status_Level /*&& data.ADEMPCODE == (loginUser == 0 ? data.ADEMPCODE : loginUser) */
                          && data.ACTIVE == 1 && _process.SIS_AST_PROCESSSTATUS_MSTID == _AppAuth.APPROVALTYPE
                          && _AppAuth.EMPCODE == loginUser && _AppAuth.ACTIVE == 1
                          //&& _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                        && (Status_Level == 16 ? data.ISUPLOADINV_BYTAXATION == 0 : Status_Level == 25 ? data.ISUPLOADINV_BYTAXATION == 1 : 1 == 1)
                       select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short UploadAssetInvoice(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0; long _assetType = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                _assetType = SADH.ASSETTYPEID;
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.INVOICE_ATTACHMENT = ADVM.INVOICE_ATTACHMENT;
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }

                        ////// -------------- Add & Update CustomerDetail -------------///////
                        if (ADVM.CustomerDetail != null && ADVM.DISPOSALHEADERID > 0 && _assetType == 2)////// ----- 2 for Scrap
                        {
                            SaveDisposalCustomerDetails((long)ADVM.MODIFIEDBY, ADVM.DISPOSALHEADERID, ADVM.CustomerDetail);
                        }

                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }
        #endregion

        #region Asset Retirement
        public List<AssetDisposalViewModel> GetApprovalListByAssetRetirement(long loginUser)
        {  
            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         //join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE
                         join _AppAuth in _AssetDBContext.SIS_AST_APPAUTH_MST on data.PLANTID equals _AppAuth.SYSITEID
                         where _process.STATUS_LEVEL == 19 && data.ACTIVE == 1
                         && _AppAuth.EMPCODE == loginUser && _AppAuth.ACTIVE == 1
                         && _process.SIS_AST_PROCESSSTATUS_MSTID == _AppAuth.APPROVALTYPE
                         //&& _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public short RequestUpdateByAssetRetirement(AssetDisposalViewModel ADVM, short Status_Level)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = _AssetDBContext.Database.BeginTransaction())
            using (var transaction = _AssetDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (ADVM.DISPOSALHEADERID > 0)
                    {
                        ///// -------- Insert Record Approval History Table ----------/////
                        SaveApprovalHistory(ADVM.Approval_History);

                        ///// -------- Update Record Disposal Header Table ----------/////
                        SIS_AST_DISPOSALHEADER SADH = new SIS_AST_DISPOSALHEADER();
                        if (ADVM.DISPOSALHEADERID > 0)
                        {
                            SADH = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(x => x.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).SingleOrDefault();
                            if (SADH != null)
                            {
                                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                                {
                                    Status_Level = Convert.ToInt16(21); ///// Request Reject
                                }
                                SADH.PROCESSSTATUSID = _AssetDBContext.SIS_AST_PROCESSSTATUS_MST.Where(x => x.STATUS_LEVEL == Status_Level).Select(y => y.SIS_AST_PROCESSSTATUS_MSTID).FirstOrDefault();
                                SADH.MODIFIEDBY = ADVM.MODIFIEDBY;
                                SADH.DATELSTMOD = DateTime.Now;
                                SADH.ACTIVE = Convert.ToInt16(ADVM.Approval_History.APPROVALSTATUS == 3 ? 0 : 1);
                                _AssetDBContext.Entry(SADH).State = EntityState.Modified;
                                _AssetDBContext.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }
        #endregion

        public List<AssetDisposalViewModel> GetRequestReport(long AssetType, long EmpCode, string FromDate, string ToDate, long loginUser)
        {
            DateTime fromdate = DateTime.Now.Date; DateTime todate = DateTime.Now.Date;
            long fromdateflag = 0; long todateflag = 0; long assetType = 0; long empcode = 0;
            if (AssetType > 0)
            {
                assetType = 1;
            }
            if (EmpCode > 0)
            {
                empcode = 1;
            }
            if (!string.IsNullOrEmpty(FromDate))
            {
                fromdate = DateTime.ParseExact(FromDate, "dd-MMM-yyyy", (IFormatProvider)null);
                fromdateflag = 1;
            }
            if (!string.IsNullOrEmpty(ToDate))
            {
                todate = DateTime.ParseExact(ToDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", (IFormatProvider)null);
                todateflag = 1;
            }

            var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                         join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                         join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                         join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                         where data.ACTIVE == 1 && _process.STATUS_LEVEL == 20
                         && (fromdateflag == 0 ? 1 == 1 : data.DATEADDED >= fromdate)
                         && (todateflag == 0 ? 1 == 1 : data.DATEADDED <= todate)
                         && (empcode == 0 ? 1 == 1 : data.ADEMPCODE == EmpCode)
                         && (assetType == 0 ? true : data.ASSETTYPEID == AssetType)
                         select new AssetDisposalViewModel
                         {
                             DISPOSALHEADERID = data.DISPOSALHEADERID,
                             ADEMPCODE = data.ADEMPCODE,
                             EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
                             ASSETTYPEID = data.ASSETTYPEID,
                             ASSETTYPE = _assetMst.DESCRIPTION,
                             PROCESSSTATUSID = data.PROCESSSTATUSID,
                             PROCESSSTATUS = _process.STATUSDESCRIPTION,
                             BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
                             TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
                             MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
                             REMARKS = data.REMARKS,
                             //ACTIVE = data.ACTIVE == 1 ? true : false,
                             ACTIVE = Convert.ToBoolean(data.ACTIVE),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             DATEADDED = data.DATEADDED,
                             MODIFIEDBY = data.MODIFIEDBY,
                             DATELSTMOD = data.DATELSTMOD,
                             AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                         });
            return iList.OrderByDescending(d => d.DATEADDED).ToList();
        }

        public List<AssetDisposalViewModel> GetRequestCancellationListForAdmin()
        {
            //var iList = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
            //             join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
            //             join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
            //             join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
            //             join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE //---[Code Added by Aumento on 22-Dec-2023]
            //             join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //             where data.ACTIVE == 1 && _process.STATUS_LEVEL != 20 && _process.STATUS_LEVEL != 21 && _process.STATUS_LEVEL != 26
            //             && _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1 //---[Code Added by Aumento on 22-Dec-2023]

            //             select new AssetDisposalViewModel
            //             {
            //                 DISPOSALHEADERID = data.DISPOSALHEADERID,
            //                 ADEMPCODE = data.ADEMPCODE,
            //                 EMP_NAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - [" + _Emp.ADEMPCODE + "]",
            //                 ASSETTYPEID = data.ASSETTYPEID,
            //                 ASSETTYPE = _assetMst.DESCRIPTION,
            //                 PROCESSSTATUSID = data.PROCESSSTATUSID,
            //                 PROCESSSTATUS = _process.STATUSDESCRIPTION,
            //                 STATUS_LEVEL = _process.STATUS_LEVEL,
            //                 BACKUP_ATTACHMENT_NAME = data.BACKUP_ATTACHMENT,
            //                 TRANSFERSLIP_ATTACHMENT_NAME = data.TRANSFERSLIP_ATTACHMENT,
            //                 MUTILATION_ATTACHMENT_NAME = data.MUTILATION_ATTACHMENT,
            //                 REMARKS = data.REMARKS,
            //                 //ACTIVE = data.ACTIVE == 1 ? true : false,
            //                 ACTIVE = Convert.ToBoolean(data.ACTIVE),
            //                 ADDEDBY = data.ADDEDBY,
            //                 ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                 DATEADDED = data.DATEADDED,
            //                 MODIFIEDBY = data.MODIFIEDBY,
            //                 DATELSTMOD = data.DATELSTMOD,
            //                 AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
            //                 IBM_APPDATE = data.IBM_APPDATE,
            //                 //------------------START-[Code Added by Aumento on 22-Dec-2023]------------------------------
            //                 ICMONTH = data.ICMONTH,
            //                 IMPLEMENTMONTH = data.IMPLEMENTMONTH,
            //                 Emp_Detail = new Employee_Details
            //                 {
            //                     _ECode = _Emp.ADEMPCODE,
            //                     _EName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
            //                     _DOB = DateTime.Now,
            //                     _SecDescrip = _VWAssociate.SECTION,
            //                     _DepDesc = _VWAssociate.DEPARTMENT,
            //                     _DivDesc = _VWAssociate.DIVISION,
            //                     _OpDesc = _VWAssociate.OPERATION,
            //                     _SiteId = _VWAssociate.SYSITEID
            //                 },
            //                 DisposalDetailList = (from _disposalDetail in _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == data.DISPOSALHEADERID && x.ACTIVE == 1)
            //                                       select new DisposalDetailViewModel
            //                                       {
            //                                           DISPOSALDETAILID = _disposalDetail.DISPOSALDETAILID,
            //                                           DISPOSALHEADERID = _disposalDetail.DISPOSALHEADERID,
            //                                           ORIGINALCOST = _disposalDetail.ORIGINALCOST,

            //                                           WDV = (_disposalDetail.ORIGINALCOST - _disposalDetail.DEPRECIATIONCOST),

            //                                       }).ToList(),
            //                 //------------------END-[Code Added by Aumento on 22-Dec-2023]--------------------------------
            //             });
            //return iList.OrderByDescending(d => d.DATEADDED).ToList();          

            var _obj = (from data in _AssetDBContext.SIS_AST_DISPOSALHEADER
                        join _assetMst in _AssetDBContext.SIS_ASSETTYPE_MST on data.ASSETTYPEID equals _assetMst.SIS_ASSETTYPE_MSTID
                        join _process in _AssetDBContext.SIS_AST_PROCESSSTATUS_MST on data.PROCESSSTATUSID equals _process.SIS_AST_PROCESSSTATUS_MSTID
                        join _Emp in _AssetDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                        join _VWAssociate in _AssetDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VWAssociate.ADEMPCODE //---[Code Added by Aumento on 22-Dec-2023]
                        join _AddBy in _AssetDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        where data.ACTIVE == 1 && _process.STATUS_LEVEL != 20 && _process.STATUS_LEVEL != 21 && _process.STATUS_LEVEL != 26
                        && _VWAssociate.SYKI == _Syki.SYKIID && _VWAssociate.ACTIVE == 1 //---[Code Added by Aumento on 22-Dec-2023]
                        select new
                        {
                            HeaderData = data,
                            AssetMst = _assetMst,
                            Process = _process,
                            Emp = _Emp,
                            AddBy = _AddBy,
                            VWAssociate = _VWAssociate
                        }).ToList();

            var _disposalDetail = _AssetDBContext.SIS_AST_DISPOSALDETAIL
                                  .Where(x => _obj.Select(d => d.HeaderData.DISPOSALHEADERID).Contains(x.DISPOSALHEADERID) && x.ACTIVE == 1)
                                  .Select(x => new DisposalDetailViewModel
                                  {
                                      DISPOSALDETAILID = x.DISPOSALDETAILID,
                                      DISPOSALHEADERID = x.DISPOSALHEADERID,
                                      ORIGINALCOST = x.ORIGINALCOST,
                                      WDV = (x.ORIGINALCOST - x.DEPRECIATIONCOST)
                                  }).ToList();

            var iList = _obj.Select(d => new AssetDisposalViewModel
            {
                DISPOSALHEADERID = d.HeaderData.DISPOSALHEADERID,
                ADEMPCODE = d.HeaderData.ADEMPCODE,
                EMP_NAME = d.Emp.FIRSTNAME + " " + d.Emp.LASTNAME + " - [" + d.Emp.ADEMPCODE + "]",
                ASSETTYPEID = d.HeaderData.ASSETTYPEID,
                ASSETTYPE = d.AssetMst.DESCRIPTION,
                PROCESSSTATUSID = d.HeaderData.PROCESSSTATUSID,
                PROCESSSTATUS = d.Process.STATUSDESCRIPTION,
                STATUS_LEVEL = d.Process.STATUS_LEVEL,
                BACKUP_ATTACHMENT_NAME = d.HeaderData.BACKUP_ATTACHMENT,
                TRANSFERSLIP_ATTACHMENT_NAME = d.HeaderData.TRANSFERSLIP_ATTACHMENT,
                MUTILATION_ATTACHMENT_NAME = d.HeaderData.MUTILATION_ATTACHMENT,
                REMARKS = d.HeaderData.REMARKS,
                //ACTIVE = data.ACTIVE == 1 ? true : false,
                ACTIVE = Convert.ToBoolean(d.HeaderData.ACTIVE),
                ADDEDBY = d.HeaderData.ADDEDBY,
                ADDEDBY_NAME = d.AddBy.FIRSTNAME + " " + d.AddBy.LASTNAME,
                DATEADDED = d.HeaderData.DATEADDED,
                MODIFIEDBY = d.HeaderData.MODIFIEDBY,
                DATELSTMOD = d.HeaderData.DATELSTMOD,
                AssetCount = _AssetDBContext.SIS_AST_DISPOSALDETAIL.Where(x => x.DISPOSALHEADERID == d.HeaderData.DISPOSALHEADERID && x.ACTIVE == 1 && x.EVNSTATUS == 1 && x.MGF_QLTYSTATUS == 1 && x.TAXTION_STATUS == 1 && x.MAINTENANCEDEPT_STATUS == 1 && x.IC_STATUS == 1 && x.IBM_STATUS == 1).Count(),
                IBM_APPDATE = d.HeaderData.IBM_APPDATE,
                ICMONTH = d.HeaderData.ICMONTH,
                IMPLEMENTMONTH = d.HeaderData.IMPLEMENTMONTH,
                Emp_Detail = new Employee_Details
                {
                    _ECode = d.Emp.ADEMPCODE,
                    _EName = d.Emp.FIRSTNAME + " " + d.Emp.LASTNAME,
                    _DOB = DateTime.Now,
                    _SecDescrip = d.VWAssociate.SECTION,
                    _DepDesc = d.VWAssociate.DEPARTMENT,
                    _DivDesc = d.VWAssociate.DIVISION,
                    _OpDesc = d.VWAssociate.OPERATION,
                    _SiteId = d.VWAssociate.SYSITEID
                },
                DisposalDetailList = _disposalDetail.Where(x => x.DISPOSALHEADERID == d.HeaderData.DISPOSALHEADERID).ToList()
            }).OrderByDescending(d => d.DATEADDED).ToList();

            return iList;

        }

        public short UpdateIBMDate(AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (ADVM.DISPOSALHEADERID > 0)
                {
                    SIS_AST_DISPOSALHEADER astDisposalheader = new SIS_AST_DISPOSALHEADER();
                    astDisposalheader = _AssetDBContext.SIS_AST_DISPOSALHEADER.Where(c => c.DISPOSALHEADERID == ADVM.DISPOSALHEADERID).FirstOrDefault();
                    if (astDisposalheader != null)
                    {

                        astDisposalheader.IBM_APPDATE = ADVM.IBM_APPDATE;
                        astDisposalheader.MODIFIEDBY = ADVM.MODIFIEDBY;
                        astDisposalheader.DATELSTMOD = new DateTime?(DateTime.Now);
                        _AssetDBContext.Entry<SIS_AST_DISPOSALHEADER>(astDisposalheader).State = EntityState.Modified;
                        _AssetDBContext.SaveChanges();
                        retVal = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return retVal;
        }

    }
}
