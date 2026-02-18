using System.Globalization;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Persistence.Interface;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Infrastructure.Repositories
{
    public class RiskAssessmentRepository
    {
        private readonly IConnectionString _conn;
        private EPortalDBContext _arDBContext;

        public RiskAssessmentRepository(EPortalDBContext arDBContext, IConnectionString conn)
        {
            _arDBContext = arDBContext;
            _conn = conn;
        }

        public List<ADORGLEVEL> GetOrgLevelList(long typeId)
        {
            var iList = from data in _arDBContext.ADORGLEVEL
                        where data.ACTIVE == 1 && (data.SYKIID >= 21)
                        && data.ADORGLEVELTYPEID == typeId
                        orderby data.LEVELDESCRIP
                        select data;
            return iList.ToList();

        }
        public EmpDetails GetEmp_Details_Ki_Wise(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            EmpDetails emp = new EmpDetails();
            emp = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.ADEMPCODE == Id && x.SYKI == currentki).Select(x => new EmpDetails { OperationId = x.OPERATIONID, DivisionId = x.DIVISIONID, Degination = x.FUNCTIONALDESIGNATION }).FirstOrDefault();
            return emp;
        }

        public RiskSelfPending GetSelfRiskPending(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var divisionds = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => x.DIVISIONID).FirstOrDefault();
            var RiskList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Id && x.SYKI == currentki).Select(x => new RiskSelfPending
            {
                DivisionName = x.DIVISION,
                Process = "Risk Assessment",
                TargetDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(y => x.DIVISIONID == divisionds && y.ACTIVE == 1).Select(y => y.ENDDATE).FirstOrDefault(),
                Count = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Take(1).Count(),
                ISSCMSubmittedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_ISSCMEMBER).FirstOrDefault(),
                DHStatus = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.DIVISION_STATUS).FirstOrDefault(),
                DHApprovedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_DIVISIONHEAD).FirstOrDefault(),
                OPHeadStatus = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.OPERATING_STATUS).FirstOrDefault(),
                OPHApprovedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_OPERATINGHEAD).FirstOrDefault(),

            }).FirstOrDefault();
            //var riskAssessment = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Id && x.SKYID == currentki && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS==null)
            //    .Select(y => new RiskSelfPending
            //    {
            //        Process = "Risk Assessment",
            //        TargetDate= _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x=> x.DIVISIONID== divisionds && x.ACTIVE==1).Select(x=>x.ENDDATE).FirstOrDefault(),
            //        Count = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Id && x.SKYID == currentki && x.ACTIVE == 1).Take(1).Count(),
            //        DHStatus = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Id && x.SKYID == currentki && x.ACTIVE == 1).Select(x => x.DIVISION_STATUS).FirstOrDefault(),
            //        DHApprovedDate = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Id && x.SKYID == currentki && x.ACTIVE == 1).Select(x => x.SUBMIT_DATE_DIVISIONHEAD).FirstOrDefault(),
            //        OPHeadStatus = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Id && x.SKYID == currentki && x.ACTIVE == 1).Select(x => x.OPERATING_STATUS).FirstOrDefault(),
            //        OPHApprovedDate = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Id && x.SKYID == currentki && x.ACTIVE == 1).Select(x => x.SUBMIT_DATE_OPERATINGHEAD).FirstOrDefault(),

            //    }).FirstOrDefault();
            return RiskList;
        }

        public RiskSelfPending GetSelfRiskRequest(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var divisionds = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => x.DIVISIONID).FirstOrDefault();
            //var riskAssessment = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY != Id)
            //    .Select(y => new RiskSelfPending
            //    {
            //        Process = "Risk Assessment",
            //        TargetDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.DIVISIONID == divisionds && x.ACTIVE == 1).Select(x => x.ENDDATE).FirstOrDefault(),
            //        Count = 1
            //    }).FirstOrDefault();
            var riskAssessment = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Id && x.SYKI == currentki).Select(x => new RiskSelfPending
            {
                DivisionName = x.DIVISION,
                Process = "Risk Assessment",
                TargetDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(y => x.DIVISIONID == divisionds && y.ACTIVE == 1).Select(y => y.ENDDATE).FirstOrDefault(),
                Count = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Take(1).Count(),
                ISSCMSubmittedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_ISSCMEMBER).FirstOrDefault(),
                DHStatus = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.DIVISION_STATUS).FirstOrDefault(),
                DHApprovedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_DIVISIONHEAD).FirstOrDefault(),
                OPHeadStatus = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.OPERATING_STATUS).FirstOrDefault(),
                OPHApprovedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == Id && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_OPERATINGHEAD).FirstOrDefault(),
            }).FirstOrDefault();
            return riskAssessment;
        }

        public List<RiskApproval> GetRiskPendingForApprovalForDivHead(long Id)
        {
            List<RiskApproval> lstApp = new List<RiskApproval>();
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var divionids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => x.DIVISIONID).ToList();
            var ISSC_Members = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == currentki && divionids.Contains((long)x.DIVISIONID)).Select(x => x.ISSCEMPCODE).ToList();
            var riskAssessment = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => ISSC_Members.Contains(x.ADEMPCODE) && x.SYKI == currentki).Select(x => new RiskApproval
            {
                DivisionName = x.DIVISION,
                EmpCode = x.ADEMPCODE,
                Process = "Risk Assessment",
                TargetDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == currentki && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),
                ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                //Count = _arDBContext.RISKASSESSMENT.Where(y1 => y1.RISK_CREATEDBY == x.ADEMPCODE && y1.DIVISION_STATUS == null && y1.STATUS==1 && y1.SKYID == currentki && y1.ACTIVE == 1).Take(1).Count(),
            }).ToList();
            if (riskAssessment.Count > 0)
            {
                foreach (var item in riskAssessment)
                {
                    RiskApproval app = new RiskApproval();
                    app.DivisionName = item.DivisionName;
                    app.EmpCode = item.EmpCode;
                    app.Process = item.Process;
                    app.ISSCMember = item.ISSCMember;
                    app.TargetDate = item.TargetDate;
                    app.Count = _arDBContext.RISKASSESSMENT.Where(y1 => y1.RISK_CREATEDBY == item.EmpCode && y1.DIVISION_STATUS == null && y1.STATUS == 1 && y1.SKYID == currentki && y1.ACTIVE == 1).Take(1).Count();
                    app.ISSCMSubmittedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_ISSCMEMBER).FirstOrDefault();
                    //app.DHStatus = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.DIVISION_STATUS).FirstOrDefault();
                    app.DHApprovedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_DIVISIONHEAD).FirstOrDefault();
                    //app.OPHeadStatus = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.OPERATING_STATUS).FirstOrDefault();
                    app.OPHApprovedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_OPERATINGHEAD).FirstOrDefault();
                    lstApp.Add(app);
                }
                return lstApp;
            }
            else
            {
                return lstApp;
            }

        }


        public List<RiskApproval> Get_Risk_Details_For_OpHeadORDivHead(int? id)
        {
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();
            List<RiskApproval> RiskApprovalList = new List<RiskApproval>();
            var OperatingHead_Check = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.ADFUNCTIONALDESIGNATIONID).FirstOrDefault();
            if (OperatingHead_Check == 4)
            {
                // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
                var operationid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.OPERATIONID).FirstOrDefault(); ;
                var ISSC_Member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIList.SYKIID && x.OPERATIONID == operationid).Select(x => x.ISSCEMPCODE).ToList();

                foreach (var item in ISSC_Member)
                {
                    var statuscheck = _arDBContext.RISKASSESSMENT.Where(x => x.SKYID == SYKIList.SYKIID && x.RISK_CREATEDBY == item && x.STATUS == 2 && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1).Select(x => new { x.DIVISION_STATUS, x.OPERATING_STATUS, x.SUBMIT_DATE_ISSCMEMBER, x.SUBMIT_DATE_DIVISIONHEAD }).FirstOrDefault();
                    if (statuscheck != null)
                    {
                        if (statuscheck.DIVISION_STATUS != null && statuscheck.OPERATING_STATUS == null)
                        {
                            var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item && x.SYKI == SYKIList.SYKIID).Select(x => new RiskApproval
                            {
                                DivisionName = x.DIVISION,
                                ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                                TargetDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),
                                Process = "Risk Assessment Approval",
                                // Url = "/AssetRegistration/AssetDetailsApproveByOperatingHead?ISSC_Code=" + x.ADEMPCODE + "",
                                Url = "/Riskassessment/RiskAssessmentDashboardForOperatingHead",
                                ISSCMSubmittedDate = statuscheck.SUBMIT_DATE_ISSCMEMBER,
                                DHStatus = statuscheck.DIVISION_STATUS == null ? "Pending" : "Approved",
                                DHApprovedDate = statuscheck.SUBMIT_DATE_DIVISIONHEAD,
                                OPHeadStatus = statuscheck.OPERATING_STATUS == null ? "Pending" : "Approved",
                            }).FirstOrDefault();
                            RiskApprovalList.Add(PeriodSettingList);
                        }
                    }
                }
            }
            else if (OperatingHead_Check == 3)
            {
                // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
                var divi_id = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.DIVISIONID).FirstOrDefault(); ;
                var ISSC_Member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIList.SYKIID && x.DIVISIONID == divi_id).Select(x => x.ISSCEMPCODE).ToList();

                foreach (var item in ISSC_Member)
                {
                    var statuscheck = _arDBContext.RISKASSESSMENT.Where(x => x.SKYID == SYKIList.SYKIID && x.RISK_CREATEDBY == item && x.STATUS == 1 && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1).Select(x => new { x.DIVISION_STATUS, x.OPERATING_STATUS, x.SUBMIT_DATE_ISSCMEMBER, x.SUBMIT_DATE_DIVISIONHEAD }).FirstOrDefault();
                    if (statuscheck != null)
                    {
                        if (statuscheck.DIVISION_STATUS == null && statuscheck.OPERATING_STATUS == null)
                        {
                            var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item && x.SYKI == SYKIList.SYKIID).Select(x => new RiskApproval
                            {
                                DivisionName = x.DIVISION,
                                ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                                TargetDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),
                                Process = "Risk Assessment Approval",
                                Url = "/RiskAssessment/RiskDashboardForDivisionHead",
                                ISSCMSubmittedDate = statuscheck.SUBMIT_DATE_ISSCMEMBER,
                                DHStatus = statuscheck.DIVISION_STATUS == null ? "Pending" : "Approved",
                                DHApprovedDate = statuscheck.SUBMIT_DATE_DIVISIONHEAD,
                                OPHeadStatus = statuscheck.OPERATING_STATUS == null ? "Pending" : "Approved",
                            }).FirstOrDefault();
                            RiskApprovalList.Add(PeriodSettingList);
                        }
                    }
                }
            }
            return RiskApprovalList;
        }

        public List<long?> GetOperationIdsByLoginUserId(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var operationids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => x.OPERATIONID).ToList();
            return operationids;
        }

        public List<long?> GetDivisionsIdsByLoginUserId(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var divionids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => x.DIVISIONID).ToList();
            return divionids;
        }

        public List<RiskApprovalOP> RiskPendingForApprovalForOPHead(long Id)
        {
            List<RiskApprovalOP> lstApp = new List<RiskApprovalOP>();
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var operationids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => x.OPERATIONID).ToList();
            var ISSC_Member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == currentki && operationids.Contains((long)x.OPERATIONID)).Select(x => x.ISSCEMPCODE).ToList();
            var riskAssessment = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => ISSC_Member.Contains(x.ADEMPCODE) && x.SYKI == currentki).Select(x => new RiskApproval
            {
                DivisionName = x.DIVISION,
                EmpCode = x.ADEMPCODE,
                Process = "Risk Assessment",
                TargetDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == currentki && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),
                ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                //Count = _arDBContext.RISKASSESSMENT.Where(y1 => y1.RISK_CREATEDBY == x.ADEMPCODE && y1.DIVISION_STATUS == null && y1.STATUS==1 && y1.SKYID == currentki && y1.ACTIVE == 1).Take(1).Count(),
            }).ToList();

            //List<RiskApproval> lstApps = new List<RiskApproval>();
            //foreach (var item in ISSC_Member)
            //{
            //    var RiskList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item && x.SYKI == currentki).Select(x => new RiskApproval
            //    {
            //        DivisionName = x.DIVISION,
            //        EmpCode = x.ADEMPCODE,
            //        Process = "Risk Assessment",
            //        TargetDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == currentki && x1.OPERATIONID == x.OPERATIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),
            //        Operation = x.OPERATION,
            //        ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
            //    }).FirstOrDefault();
            //    lstApp.Add(RiskList);
            //}
            if (riskAssessment.Count > 0)
            {
                foreach (var item1 in riskAssessment)
                {
                    var result = _arDBContext.RISKASSESSMENT.Where(y1 => y1.RISK_CREATEDBY == item1.EmpCode && y1.OPERATING_STATUS == null && (y1.STATUS == 2 || y1.STATUS == 1) && y1.SKYID == currentki && y1.ACTIVE == 1).Count();
                    if (result > 0)
                    {
                        RiskApprovalOP app = new RiskApprovalOP();
                        app.DivisionName = item1.DivisionName;
                        app.EmpCode = item1.EmpCode;
                        app.Process = item1.Process;
                        app.ISSCMember = item1.ISSCMember;
                        app.Operation = item1.Operation;
                        app.TargetDate = item1.TargetDate;
                        app.Count = _arDBContext.RISKASSESSMENT.Where(y1 => y1.RISK_CREATEDBY == item1.EmpCode && y1.DIVISION_STATUS == null && y1.STATUS == 1 && y1.SKYID == currentki && y1.ACTIVE == 1).Take(1).Count();
                        app.ISSCMSubmittedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item1.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_ISSCMEMBER).FirstOrDefault();
                        app.DHStatus = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item1.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.DIVISION_STATUS).FirstOrDefault();
                        app.DHApprovedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item1.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_DIVISIONHEAD).FirstOrDefault();
                        app.OPHeadStatus = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item1.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.OPERATING_STATUS).FirstOrDefault();
                        app.OPHApprovedDate = _arDBContext.RISKASSESSMENT.Where(y => y.RISK_CREATEDBY == item1.EmpCode && y.SKYID == currentki && y.ACTIVE == 1).Select(y => y.SUBMIT_DATE_OPERATINGHEAD).FirstOrDefault();
                        lstApp.Add(app);
                    }

                }
                return lstApp;
            }
            else
            {
                return lstApp;
            }

        }

        public List<ADORGLEVEL> BindOperation(long? id)
        {
            List<ADORGLEVEL> op = new List<ADORGLEVEL>();
            var iList = from data in _arDBContext.ADORGLEVEL
                        where data.SYKIID == id && data.ACTIVE != 0
                        && data.ADORGLEVELTYPEID == 1
                        orderby data.LEVELDESCRIP
                        select data;
            foreach (var obj in iList)
            {
                op.Add(new ADORGLEVEL
                {
                    ADORGLEVELID = obj.ADORGLEVELID,
                    LEVELDESCRIP = obj.LEVELDESCRIP
                });
            }
            return op;
        }

        public RiskAssessmentSYKIViewModel GetRiskRegistrationSYKIListForViewTeam()
        {
            RiskAssessmentSYKIViewModel riskRegistrationSYKIViewModel = new RiskAssessmentSYKIViewModel();

            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.SYKIID > 21 && x.ACTIVE == 1)
                            select data).OrderByDescending(x => x.SYKIID).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    riskRegistrationSYKIViewModel._SYKIList.Add(new RiskAssessmentSYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return riskRegistrationSYKIViewModel;
        }
        public List<RiskCategoryVM> GetAllRiskCategory()
        {
            var iList = _arDBContext.RISK_CATEGORY_MASTER.Where(x => x.STATUS == 1).Select(x => new RiskCategoryVM
            {
                RiskId = (int)x.RISK_ID,
                Category = x.RISK_CATEGORIES
            }).ToList();
            return iList;
        }

        public List<RiskCategoryCreVM> GetAllRiskCategories()
        {
            var iList = _arDBContext.RISK_CATEGORY_MASTER.Where(x => x.STATUS == 1).Select(x => new RiskCategoryCreVM
            {
                RiskId = (int)x.RISK_ID,
                Category = x.RISK_CATEGORIES,
                Status = x.STATUS == 1 ? "Active" : "Inactive"
            }).OrderBy(x => x.RiskId).ToList();
            return iList;
        }

        public List<RiskStatementLstVM> GetAllPotentialRisk()
        {

            var iList = _arDBContext.RISK_STATEMENT_MASTER.Where(x => x.STATUS == 1).Select(x => new RiskStatementLstVM
            {
                RiskCategory = _arDBContext.RISK_CATEGORY_MASTER.Where(y => y.RISK_ID == x.RISKID).Select(y => y.RISK_CATEGORIES).FirstOrDefault(),
                StatementId = (int)x.STATEMENTID,
                Statement = x.RISK_STATEMENT,
                Status = x.STATUS == 1 ? "Active" : "Inactive"
            }).OrderBy(x => x.StatementId).ToList();
            return iList;
        }

        public RiskCategoryCreVM GetRiskCategoryById(long Id)
        {
            var rData = _arDBContext.RISK_CATEGORY_MASTER.Where(x => x.RISK_ID == Id && x.STATUS == 1).Select(x => new RiskCategoryCreVM
            {
                RiskId = (int)x.RISK_ID,
                Category = x.RISK_CATEGORIES,
                //IsActive = (int)x.STATUS
            }).FirstOrDefault();
            return rData;
        }

        public string DeleteRiskCategory(long Id)
        {
            var rData = _arDBContext.RISK_CATEGORY_MASTER.Where(x => x.RISK_ID == Id).FirstOrDefault();
            if (rData != null)
            {
                rData.STATUS = 0;
                _arDBContext.Entry(rData).State = EntityState.Modified;
                _arDBContext.SaveChanges();
                return "Risk category delete successfully.";
            }
            else
            {
                return "Risk category not found.";
            }
        }

        public RiskStatementCreVM EditPotentialRisk(long Id)
        {
            var rData = _arDBContext.RISK_STATEMENT_MASTER.Where(x => x.STATEMENTID == Id && x.STATUS == 1).Select(x => new RiskStatementCreVM
            {
                RiskId = (int)x.RISKID,
                Statement = x.RISK_STATEMENT,
                StatementId = (int)x.STATEMENTID,
                //IsActive = (int)x.STATUS
            }).FirstOrDefault();
            return rData;
        }

        public string DeletePotentialRisk(long Id)
        {
            var rData = _arDBContext.RISK_STATEMENT_MASTER.Where(x => x.STATEMENTID == Id).FirstOrDefault();
            if (rData != null)
            {
                rData.STATUS = 0;
                _arDBContext.Entry(rData).State = EntityState.Modified;
                _arDBContext.SaveChanges();
                return "Potential risk delete successfully.";
            }
            else
            {
                return "Potential risk not found.";
            }
        }

        public int InsertORUpdatePotentialRisk(RiskStatementCreVM riskStatementCreVM)
        {
            if (riskStatementCreVM.StatementId > 0)
            {
                var result = _arDBContext.RISK_STATEMENT_MASTER.Where(x => x.STATEMENTID == riskStatementCreVM.StatementId && x.STATUS == 1).FirstOrDefault();
                result.RISK_STATEMENT = riskStatementCreVM.Statement;
                result.RISKID = riskStatementCreVM.RiskId;
                _arDBContext.Entry(result).State = EntityState.Modified;
                _arDBContext.SaveChanges();
                return 0;
            }
            else
            {
                //int count = 0;
                RISK_STATEMENT_MASTER cMaster = new RISK_STATEMENT_MASTER();
                var result = _arDBContext.RISK_STATEMENT_MASTER.OrderByDescending(x => x.STATEMENTID).Select(x => x.STATEMENTID).FirstOrDefault();
                cMaster.STATEMENTID = result + 1;
                cMaster.RISKID = riskStatementCreVM.RiskId;
                cMaster.RISK_STATEMENT = riskStatementCreVM.Statement;
                cMaster.STATUS = 1;
                _arDBContext.Entry(cMaster).State = EntityState.Added;
                _arDBContext.SaveChanges();
                return 1;
            }
        }

        public int InsertORUpdateRiskCategory(RiskCategoryCreVM riskCategoryCreVM)
        {
            if (riskCategoryCreVM.RiskId > 0)
            {
                var result = _arDBContext.RISK_CATEGORY_MASTER.Where(x => x.RISK_ID == riskCategoryCreVM.RiskId && x.STATUS == 1).FirstOrDefault();
                result.RISK_CATEGORIES = riskCategoryCreVM.Category;
                _arDBContext.Entry(result).State = EntityState.Modified;
                _arDBContext.SaveChanges();
                return 0;
            }
            else
            {
                int count = 0;
                RISK_CATEGORY_MASTER cMaster = new RISK_CATEGORY_MASTER();
                count = _arDBContext.RISK_CATEGORY_MASTER.Count();
                cMaster.RISK_ID = count + 1;
                cMaster.RISK_CATEGORIES = riskCategoryCreVM.Category;
                cMaster.STATUS = 1;
                _arDBContext.Entry(cMaster).State = EntityState.Added;
                _arDBContext.SaveChanges();
                return 1;
            }
        }

        public List<PotentialRiskVM> GetPotentialRiskByRiskCategoryId(int categoryId)
        {
            var iList = _arDBContext.RISK_STATEMENT_MASTER.Where(x => x.RISKID == categoryId && x.STATUS == 1).Select(x => new PotentialRiskVM
            {
                PotentialId = (int)x.STATEMENTID,
                PotentialRisk = x.RISK_STATEMENT
            }).ToList();
            return iList;
        }

        public List<PrimaryAssestsVM> GetPrimaryAssetByUserId(int userid, int currenr_ki)
        {
            var iList = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == currenr_ki && x.CREATEDBY == userid
            && x.FINALSUBMITDATE_DIVISIONHEAD != null && x.REVIEW_REMARKS_DIVISIONHEAD != null
            && x.FINALSUBMITDATE_OPERATINGHEAD != null && x.REVIEW_REMARKS_OPERATINGHEAD != null && x.ACTIVE == 1 && x.PRIMARY != null
            && x.ACTIONTYPE == 0
            ).Select(x => new PrimaryAssestsVM
            {
                AssestId = (int)x.ID,
                Assets = x.PRIMARY
            }).ToList();

            iList = iList.GroupBy(x => x.Assets).Select(y => y.First()).ToList();
            return iList;
        }

        public List<RiskLeadVM> GetRiskLeadByPotential()
        {
            var iList = _arDBContext.RISK_LEADTO_BREACH.Select(x => new RiskLeadVM
            {
                RiskLeadId = (int)x.RISKLEADID,
                RiskLead = x.RISKLEADTYPE
            }).ToList();
            return iList;
        }

        public List<Risk_Current_Meaasure_LevelVM> GetAllCurrentMeasureLevel()
        {
            var iList = _arDBContext.RISK_CURRENTMEASURE_LVL.Select(x => new Risk_Current_Meaasure_LevelVM
            {
                CurrentMeasureLevelId = (int)x.CURRENTMEASURELEVELID,
                RiskLevel = x.CUR_LEVEL
            }).ToList();
            return iList;
        }

        public List<RiskFrequencyVM> GetAllFrequency()
        {
            var iList = _arDBContext.RISK_FREQUENCY_MASTER.Select(x => new RiskFrequencyVM
            {
                FrequencyLevelId = (int)x.FREQUENCY_LEVEL,
                DegreeOfFrequency = x.DEGREE_OF_FREQUENCY
            }).ToList();
            return iList;
        }

        public List<ImpactTypeVM> GetAllImpactType()
        {
            var iList = _arDBContext.IMPACTTYPE.Select(x => new ImpactTypeVM
            {
                ImpactTypeId = (int)x.IMAPECTTYPEID,
                ImpactType = x.IMPACTTYPE1
            }).ToList();
            return iList;
        }

        public List<RiskImpactVM> GetImpactByImpactTypeId(int impactTypeId)
        {
            var iList = _arDBContext.RISK_IMPACT_MASTER.Where(x => x.IMPACTTYPEID == impactTypeId).Select(x => new RiskImpactVM
            {
                RiskLevelid = (int)x.RISK_LEVEL,
                DegreeOfImpact = x.DEGREE_OF_IMPACT
            }).ToList();
            return iList;
        }

        public List<RiskImpactVM> GetImpactByImpactId(int impId)
        {
            var iList = _arDBContext.RISK_IMPACT_MASTER.Where(x => x.RISK_LEVEL == impId).Select(x => new RiskImpactVM
            {
                RiskLevelid = (int)x.RISK_LEVEL,
                DegreeOfImpact = x.DEGREE_OF_IMPACT
            }).ToList();
            return iList;
        }

        public List<RiskFrequencyVM> GetFrequencyByFrequencyId(int FreqId)
        {
            var iList = _arDBContext.RISK_FREQUENCY_MASTER.Where(x => x.FREQUENCY_LEVEL == FreqId).Select(x => new RiskFrequencyVM
            {
                FrequencyLevelId = (int)x.FREQUENCY_LEVEL,
                DegreeOfFrequency = x.DEGREE_OF_FREQUENCY
            }).ToList();
            return iList;
        }

        public List<RiskImpactVM> GetAllImpact()
        {
            var iList = _arDBContext.RISK_IMPACT_MASTER.Select(x => new RiskImpactVM
            {
                RiskLevelid = (int)x.RISK_LEVEL,
                DegreeOfImpact = x.DEGREE_OF_IMPACT
            }).ToList();
            return iList;
        }

        public List<Degree_LevelVM> GetAllDegreeLevelByFrequencyIdAndImpactId(int frequencyId, int impactId)
        {
            var iList = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x => x.FREQUENCY == frequencyId && x.IMPACT == impactId).Select(x => new Degree_LevelVM
            {
                DegreeId = (int)x.ID,
                Degree = x.RESULT,
                LevelId = (int)x.LEVELID
            }).ToList();
            return iList;
        }

        public List<RiskTreatmentLevelVM> GetRiskTreatmentBy(int levelId, int freqId, int impactId)
        {
            List<RiskTreatmentLevelVM> lstTreatmentLevel = new List<RiskTreatmentLevelVM>();
            var mplist = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x => x.FREQUENCY == freqId && x.IMPACT == impactId).FirstOrDefault();
            if (mplist != null)
            {
                var risk_TRT = _arDBContext.RISK_LEVEL_MASTER.Where(x => x.LEVELID == mplist.LEVELID).FirstOrDefault();
                if (risk_TRT != null)
                {
                    lstTreatmentLevel = _arDBContext.RISK_TREATMENT_RISK_LVL.Where(x => x.LEVELID == risk_TRT.LEVELID).Select(x => new RiskTreatmentLevelVM
                    {
                        TreatmentId = (int)x.TREATMENTID,
                        Treatment = x.TREATMENT
                    }).ToList();
                    return lstTreatmentLevel;
                }
                else
                {
                    return lstTreatmentLevel;
                }
            }
            else
            {
                return lstTreatmentLevel;
            }

        }

        public RiskAssessmentSYKIViewModel GetRiskAssessmentSYKIList()
        {
            RiskAssessmentSYKIViewModel riskAssessmentSYKIViewModel = new RiskAssessmentSYKIViewModel();

            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    riskAssessmentSYKIViewModel._SYKIList.Add(new RiskAssessmentSYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return riskAssessmentSYKIViewModel;
        }


        //mailer utility work by sujit

        public string RiskGetReminderDate(long id)
        {
            //var s = _arDBContext.RISK_ASSESSMENT_MAIL.AsEnumerable().Where(x => x.ID == id).Select(x => x.REMINDERDATE.ToString("dd/MM/yyyy")).FirstOrDefault();

            var s =_arDBContext.RISK_ASSESSMENT_MAIL
                        .AsEnumerable()
                        .Where(x => x.ID == id)
                        .Select(x => x.REMINDERDATE.ToString("dd'/'MM'/'yyyy"))
                        .FirstOrDefault();

            return s;
        }

        public List<RiskMailerReminderVM> GetReminderDetailsForISSCMember_Nomination()
        {
            var ViewToList = _arDBContext.RISK_ASSESSMENT_MAIL.Where(x => x.REMINDERFOR == 1).AsEnumerable().Select((x, index) => new RiskMailerReminderVM
            {
                SNo = index + 1,
                ReminerDate = Convert.ToString(x.REMINDERDATE),
                SYKI = _arDBContext.SYKI.Where(x1 => x1.SYKIID == x.SKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                ReminderId = (long)x.ID
            }).OrderBy(x => x.ReminerDate).ToList();
            return ViewToList;
        }



        public RiskMailerReminderVM GetStart_Dt_End_Dt_ISSC_Nomination(long SYKI)
        {

            RiskMailerReminderVM maildetails = new RiskMailerReminderVM();
            maildetails = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.AsEnumerable().Where(x => x.SKIID == SYKI && x.ACTIVE == 1)
                .Select(x => new RiskMailerReminderVM { StartDate = x.STARTDATE.ToString(), EndDate = x.ENDDATE.ToString() }).FirstOrDefault();
            maildetails.StartDate = Convert.ToDateTime(maildetails.StartDate).ToString("dd/MM/yyyy");
            maildetails.EndDate = Convert.ToDateTime(maildetails.EndDate).ToString("dd/MM/yyyy");
            return maildetails;
        }

        //ToString("dd/mm/yyy")

        public string InsertISSC_Reminder(RiskMailerReminderVM mailer)
        {
            string s = "Data Saved Sucessfully.";
            //CultureInfo provider = new CultureInfo("en-US");
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime sdt;

            //if (!string.IsNullOrEmpty(mailer.ReminerDate))
            //{               

            if (mailer.ReminerDate.Contains("-"))
            {
                sdt = DateTime.ParseExact(mailer.ReminerDate, "dd-MM-yyyy", provider);
            }
            else
            {
                sdt = DateTime.ParseExact(mailer.ReminerDate, "dd/MM/yyyy", provider);
            }
            //11}
            //DateTime sdt = DateTime.ParseExact(mailer.ReminerDate, "dd/MM/yyyy", provider);

            if (mailer.ReminderId > 0)
            {
                var PeriodSetting = _arDBContext.RISK_ASSESSMENT_MAIL.Where(x => x.ID == mailer.ReminderId).FirstOrDefault();
                if (PeriodSetting != null)
                {
                    PeriodSetting.REMINDERDATE = sdt;
                    _arDBContext.Entry(PeriodSetting).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                    s = "Data has been updated Sucessfully.";
                }
            }
            else
            {
                RISK_ASSESSMENT_MAIL PeriodSetting = new RISK_ASSESSMENT_MAIL();
                int count = _arDBContext.RISK_ASSESSMENT_MAIL.Count();
                count = count + 1;
                PeriodSetting.ID = count;
                PeriodSetting.SKIID = mailer.SYKIID;
                PeriodSetting.REMINDERDATE = sdt;
                PeriodSetting.REMINDERFOR = mailer.ReminderFor;
                _arDBContext.Entry(PeriodSetting).State = EntityState.Added;
                _arDBContext.SaveChanges();
            }
            return s;
        }

        //ends here

        public List<RiskAssessmentPeriodForITGRCMMemberNominationListVM> GetRiskAssessmentPeriodSettingITGRCList()
        {

            //var PeriodSettingListITGRC = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.AsEnumerable().Where(x => x.ACTIVE == 1).
            //    Select((x, index) => new RiskAssessmentPeriodForITGRCMMemberNominationListVM
            //{
            //    SNo = index + 1,
            //    ID = (long)x.ID,
            //    StartDate = Convert.ToString(x.STARTDATE),
            //    EndDate = Convert.ToString(x.ENDDATE),
            //    SYKIID = (Decimal)x.SKIID,
            //    SYKI = _arDBContext.SYKI.Where(y => y.SYKIID == x.SKIID).Select(y => y.KICODE).FirstOrDefault(),
            //    OPERATIONID = (long)x.OPERATIONID,
            //    OperationName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == x.OPERATIONID && y.SYKI == x.SKIID).Select(y => y.OPERATION).FirstOrDefault(),
            //    DIVISIONID = (long)x.DIVISIONID,
            //    DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKIID && y.DIVISIONID == x.DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),
            //    CreatedBy = (long)x.CREATEDBY,
            //    EmpName = _arDBContext.ADEMPLOYEE.AsEnumerable().Where(y => y.ADEMPCODE == x.CREATEDBY).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
            //    CreationDate = (DateTime)x.CREATIONDATE,
            //    ActionType = (int)_arDBContext.SYKI.Where(y => y.SYKIID == x.SKIID).Select(y => y.ACTIVE).FirstOrDefault(),

            //}).OrderBy(x => x.OperationName)
            //.ToList();

            var PeriodSettingListITGRC = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD
    .Where(x => x.ACTIVE == 1)
    .Select(x => new RiskAssessmentPeriodForITGRCMMemberNominationListVM
    {
        // SNo will be added later after ToList()
        ID = (long)x.ID,
        StartDate = x.STARTDATE.ToString(),
        EndDate = x.ENDDATE.ToString(),
        SYKIID = x.SKIID,
        SYKI = _arDBContext.SYKI
                    .Where(y => y.SYKIID == x.SKIID)
                    .Select(y => y.KICODE)
                    .FirstOrDefault(),
        OPERATIONID = (long)x.OPERATIONID,
        OperationName = _arDBContext.VW_ASSOCIATELVLDETAILS
                    .Where(y => y.OPERATIONID == x.OPERATIONID && y.SYKI == x.SKIID)
                    .Select(y => y.OPERATION)
                    .FirstOrDefault(),
        DIVISIONID = (long)x.DIVISIONID,
        DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS
                    .Where(y => y.SYKI == x.SKIID && y.DIVISIONID == x.DIVISIONID)
                    .OrderBy(y => y.ADEMPCODE)
                    .Select(y => y.DIVISION)
                    .FirstOrDefault(),
        CreatedBy = (long)x.CREATEDBY,
        EmpName = _arDBContext.ADEMPLOYEE
                    .Where(y => y.ADEMPCODE == x.CREATEDBY)
                    .Select(y => y.FIRSTNAME + " " + y.LASTNAME)
                    .FirstOrDefault(),
        CreationDate = x.CREATIONDATE,
        ActionType = (int)_arDBContext.SYKI
                    .Where(y => y.SYKIID == x.SKIID)
                    .Select(y => y.ACTIVE)
                    .FirstOrDefault()
    })
    .ToList()
    .Select((x, index) => { x.SNo = index + 1; return x; }) // Add SNo after materialization
    .OrderBy(x => x.OperationName)
    .ToList();
            return PeriodSettingListITGRC;
        }

        public RiskAssessmentForITGRCMemberNominationVM InsertUpdateRiskAssessmentPeriodITGRCSettingDetail(RiskAssessmentPeriodForITGRCMMemberNominationListVM periodSettingVM)
        {
            RiskAssessmentForITGRCMemberNominationVM riskAssessmentForITGRCMemberNominationVM = new RiskAssessmentForITGRCMemberNominationVM();
            var ExistingKIdata = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.SKIID == periodSettingVM.SYKIID).Select(x => new { x.SKIID, x.ID }).FirstOrDefault();
            if (ExistingKIdata != null)
            {
                riskAssessmentForITGRCMemberNominationVM.Status = 3;
                riskAssessmentForITGRCMemberNominationVM.Msg = "This ki details already exist. In case of any change, please update the entry.";
                return riskAssessmentForITGRCMemberNominationVM;
            }
            else
            {
                var operations = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == periodSettingVM.SYKIID && x.OPERATIONID != null).Select(x => x.OPERATIONID).Distinct().ToList();

                foreach (var item in operations)
                {
                    var divisions = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == periodSettingVM.SYKIID && x.OPERATIONID == item && x.DIVISIONID != null
                   && x.ADDESIGNATIONID != 1 && x.ADDESIGNATIONID != 3 && x.ADDESIGNATIONID != 7 && x.ADDESIGNATIONID != 8 && x.ADDESIGNATIONID != 9
                   && x.ADDESIGNATIONID != 13 && x.ADDESIGNATIONID != 16 && x.ADDESIGNATIONID != 18 && x.ADDESIGNATIONID != 19 && x.ADDESIGNATIONID != 20
                   && x.ADDESIGNATIONID != 21 && x.ADDESIGNATIONID != 22 && x.ADDESIGNATIONID != 23 && x.ADDESIGNATIONID != 24 && x.ADDESIGNATIONID != 25 && x.ADDESIGNATIONID != 33
                    ).Select(x => x.DIVISIONID).Distinct().ToList();

                    CultureInfo provider = CultureInfo.InvariantCulture;

                    DateTime dateTime14;
                    dateTime14 = DateTime.ParseExact(periodSettingVM.StartDate, "dd/MM/yyyy", provider);

                    DateTime dateTime15;
                    dateTime15 = DateTime.ParseExact(periodSettingVM.EndDate, "dd/MM/yyyy", provider);
                    foreach (var division_item in divisions)
                    {
                        RISK_ASSESSMENT_ITGRC_PERIOD up_nomination = new RISK_ASSESSMENT_ITGRC_PERIOD();
                        var count = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Count();
                        up_nomination.ID = count + 1;
                        up_nomination.SKIID = periodSettingVM.SYKIID;
                        up_nomination.OPERATIONID = item;
                        up_nomination.DIVISIONID = division_item;
                        up_nomination.STARTDATE = dateTime14;
                        up_nomination.ENDDATE = dateTime15;
                        up_nomination.ACTIVE = 1;
                        up_nomination.CREATEDBY = periodSettingVM.CreatedBy;
                        up_nomination.CREATIONDATE = DateTime.Now;
                        _arDBContext.Entry(up_nomination).State = EntityState.Added;
                        _arDBContext.SaveChanges();
                        _arDBContext.Entry(up_nomination).State = EntityState.Detached;
                        riskAssessmentForITGRCMemberNominationVM.Status = 1;
                        riskAssessmentForITGRCMemberNominationVM.Msg = "Data has been saved succesufully.";
                    }
                }
            }
            return riskAssessmentForITGRCMemberNominationVM;
        }

        public List<RiskAssessmentPeriodForITGRCMMemberNominationListVM> GetRisk_Assessment_period_ITGRC_List_WithID(int? id)
        {
            var PeriodSettingListITGRC = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.AsEnumerable().Where(x => x.ACTIVE == 1 && x.ID == id).Select((x, index) => new RiskAssessmentPeriodForITGRCMMemberNominationListVM
            {
                SNo = index + 1,
                ID = (long)x.ID,
                StartDate = Convert.ToString(x.STARTDATE),
                EndDate = Convert.ToString(x.ENDDATE),
                SYKIID = (Decimal)x.SKIID,
                SYKI = _arDBContext.SYKI.Where(y => y.SYKIID == x.SKIID).Select(y => y.KICODE).FirstOrDefault(),
                OPERATIONID = (long)x.OPERATIONID,
                OperationName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == x.OPERATIONID && y.SYKI == x.SKIID).Select(y => y.OPERATION).FirstOrDefault(),
                DIVISIONID = (long)x.DIVISIONID,
                DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKIID && y.DIVISIONID == x.DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),
                CreatedBy = (long)x.CREATEDBY,
                EmpName = _arDBContext.ADEMPLOYEE.AsEnumerable().Where(y => y.ADEMPCODE == x.CREATEDBY).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                CreationDate = (DateTime)x.CREATIONDATE,
                ActionType = (int)_arDBContext.SYKI.Where(y => y.SYKIID == x.SKIID).Select(y => y.ACTIVE).FirstOrDefault(),

            }).OrderBy(x => x.CreationDate)
            .ToList();
            return PeriodSettingListITGRC;
        }

        public BulkUpdatePeriod BulkUpdateITGRCNomination(BulkUpdatePeriod bulkUpdatePeriod)
        {
            RISK_ASSESSMENT_ITGRC_PERIOD up_nomination = new RISK_ASSESSMENT_ITGRC_PERIOD();
            up_nomination = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.ID == bulkUpdatePeriod.ID).FirstOrDefault();
            if (up_nomination != null)
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                //DateTime dateTime14;
                //dateTime14 = DateTime.ParseExact(bulkUpdatePeriod.StartDate, "dd/MM/yyyy", provider);

                //DateTime dateTime15 = Convert.ToDateTime(bulkUpdatePeriod.EndDate);
                //DateTime dateTime15 = DateTime.ParseExact(bulkUpdatePeriod.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                DateTime dateTime15;
                if (bulkUpdatePeriod.EndDate.Contains("-"))
                {
                    //dateTime15 = DateTime.ParseExact(bulkUpdatePeriod.EndDate, "dd-MM-yyyy", provider);
                    dateTime15 = DateTime.ParseExact(bulkUpdatePeriod.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    dateTime15 = DateTime.ParseExact(bulkUpdatePeriod.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                //dateTime15 = DateTime.ParseExact(bulkUpdatePeriod.EndDate, "dd/MM/yyyy", provider);
                //up_nomination.STARTDATE = dateTime14;
                up_nomination.ENDDATE = dateTime15;
                up_nomination.UPDATEDBY = (long)bulkUpdatePeriod.CreatedBy;
                up_nomination.UPDATIONDATE = DateTime.Now;
                _arDBContext.Entry(up_nomination).State = EntityState.Modified;
                _arDBContext.SaveChanges();
                bulkUpdatePeriod.Status = 2;
                return bulkUpdatePeriod;
            }
            else
            {
                return bulkUpdatePeriod;
            }
        }

        public long GetOldISSC_MemberByCurrentISSCMemCode(long SYKI, int Empcode)
        {
            var OldISSCMemCode = _arDBContext.RISK_USER_MAPPING.Where(x => x.NEWISSC_MEMBERCODE == Empcode && x.SYKIID == SYKI).Select(x => new { x.OLDISSC_MEMBERCODE }).FirstOrDefault();
            if (OldISSCMemCode != null)
            {
                return Convert.ToInt16(OldISSCMemCode.OLDISSC_MEMBERCODE);
            }
            else
            {
                return 0;
            }

        }

        public int? GetAssetUserDeailsForDivisionHeadByLoginUserID(long Userid, long Syki)
        {
            var UserDetails = _arDBContext.RISK_USER_MAPPING.Where(x => x.NEWDIVISIONEMPCODE == Userid && x.SYKIID == Syki).FirstOrDefault();
            if (UserDetails != null)
            {
                return Convert.ToInt16(UserDetails.OLDDIVISIONEMPCODE);
            }
            else
            {
                return null;
            }
        }

        public int? GetAssetUserDeailsForOPHeadByLoginUserID(long Userid, long Syki)
        {
            var UserDetails = _arDBContext.RISK_USER_MAPPING.Where(x => x.NEWOPERATIONHEADCODE == Userid && x.SYKIID == Syki).FirstOrDefault();
            if (UserDetails != null)
            {
                return Convert.ToInt16(UserDetails.OLDOPERATIONHEADCODE);
            }
            else
            {
                return null;
            }
        }

        public List<Get_Division_ISSC_Member> Get_ISSC_Members_Detail_For_DivisionHead(int? id)
        {
            var SYKIID = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var divionids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIID && x.ADEMPCODE == id).Select(x => x.DIVISIONID).ToList();
            var ISSC_Members = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIID && divionids.Contains((long)x.DIVISIONID)).Select(x => x.ISSCEMPCODE).ToList();
            var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => ISSC_Members.Contains(x.ADEMPCODE) && x.SYKI == SYKIID).Select(x => new Get_Division_ISSC_Member
            {
                DivId = (long)x.DIVISIONID,
                DivisionName = x.DIVISION,
                ISSCMember_EmpCode = x.ADEMPCODE,
                ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                EndDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),

                ISSC_Member_Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.SUBMIT_DATE_ISSCMEMBER).FirstOrDefault(),
                ISSC_Member_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.ISSC_MEMBER_STATUS).FirstOrDefault(),

                DivisionHead_Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.SUBMIT_DATE_DIVISIONHEAD).FirstOrDefault(),
                DivisionHead_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

                OperatingHead__Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.SUBMIT_DATE_OPERATINGHEAD).FirstOrDefault(),
                OperatingHead_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.OPERATING_STATUS).FirstOrDefault(),

            }).ToList();
            return PeriodSettingList;
        }

        public List<RiskAssessmentSearchModel> GetRiskAssessmentSunmitStatus(long? SYKIID, long? OPERATIONID, long? DIVISIONID)
        {
            var ViewToListold = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.STATUS == 1 && x.SYKIID == SYKIID
           && (OPERATIONID == null || x.OPERATIONID == OPERATIONID) && (DIVISIONID == null || x.DIVISIONID == DIVISIONID)).ToList();
            var ViewToList = ViewToListold.Select(x => new RiskAssessmentSearchModel
            {
                SYKIID = x.SYKIID,
                SYKI = x.SYKI.KICODE,
                OPERATIONID = x.OPERATIONID.ToString(),
                OPERATIONNAME = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == x.OPERATIONID && y.SYKI == x.SYKIID).Select(y => y.OPERATION).FirstOrDefault(),
                DIVHDHDID = x.DIVISIONID,
                DIVISIONNAMEN = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SYKIID && y.DIVISIONID == x.DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),
                EmpName = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ISSCEMPCODE && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3) && x1.ACTIVE == 1 && x1.ISSC_MEMBER_STATUS == 1).Count() > 0 ? _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ISSCEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() : "",
                ISSC_EMPCODE = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ISSCEMPCODE && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3) && x1.ACTIVE == 1 && x1.ISSC_MEMBER_STATUS == 1).Count() > 0 ? _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ISSCEMPCODE && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3) && x1.ACTIVE == 1 && x1.ISSC_MEMBER_STATUS == 1).Select(x1 => x1.RISK_CREATEDBY).FirstOrDefault() : null,
                Div_Head = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ISSCEMPCODE && (x1.STATUS == 2 || x1.STATUS == 3) && x1.ACTIVE == 1 && x1.ISSC_MEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1).Count() > 0 ? _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == (_arDBContext.ADORGLEVELHEAD.Where(x2 => x2.ISACTIVE == 1 && x2.ADORGLEVELID == x.DIVISIONID).Select(x2 => x2.ADEMPCODE).FirstOrDefault())).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() : "",
                Div_EMPCODE = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ISSCEMPCODE && x1.STATUS == 2 || x1.STATUS == 3 && x1.ACTIVE == 1 && x1.ISSC_MEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1).Count() > 0 ? _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ISSCEMPCODE && (x1.STATUS == 2 || x1.STATUS == 3) && x1.ACTIVE == 1 && x1.ISSC_MEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1).Select(x1 => x1.DIVISION_HEAD_ID).FirstOrDefault() : null,
                Op_EMPCODE = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ISSCEMPCODE && x1.STATUS == 3 && x1.ACTIVE == 1 && x1.ISSC_MEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1 && x1.OPERATING_STATUS == 1).Count() > 0 ? _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ISSCEMPCODE && x1.STATUS == 3 && x1.ACTIVE == 1 && x1.ISSC_MEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1 && x1.OPERATING_STATUS == 1).Select(x1 => x1.OPERATING_HEAD_ID).FirstOrDefault() : null,
                Opr_Head = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIID && x1.RISK_CREATEDBY == x.ISSCEMPCODE && x1.STATUS == 3 && x1.ACTIVE == 1 && x1.ISSC_MEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1 && x1.OPERATING_STATUS == 1).Count() > 0 ? _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADDEDBY).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() : "",

            }).OrderBy(x => x.OPERATIONNAME).ToList();
            return ViewToList;

        }

        public List<Asset_Dvision_Deatils_VM> Get_ISSC_Members_Division_Details_For_DivisionHead(int? id)
        {
            var SYKIID = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

            var divionDetails = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && (x.SYKI > 23 && x.SYKI < SYKIID) && x.ADEMPCODE == id).OrderByDescending(x => x.SYKI).Take(3)
                .Select(x => new
                {
                    x.SYKI,
                    SYKIName = _arDBContext.SYKI.Where(y => y.SYKIID == x.SYKI).Select(y => y.KICODE).FirstOrDefault(),
                    x.DIVISIONID,
                    x.DIVISION
                }).ToList();

            var divisionList = divionDetails.Select(x => new Asset_Dvision_Deatils_VM
            {
                SYKI = x.SYKI,
                DIVISIONID = x.DIVISIONID,
                DIVISION = x.DIVISION,
                SYKIName = x.SYKIName
            }).ToList();

            return divisionList;
        }

        public int Division_Head_AssetDetails_FinalSubmit_Check(int Userid, long? SYKI, long ISSC_Code)
        {
            var check = _arDBContext.RISKASSESSMENT.Where(x => x.SKYID == SYKI && x.DIVISION_HEAD_ID == Userid && x.RISK_CREATEDBY == ISSC_Code && x.STATUS == 2 && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1 && x.DIVISION_STATUS == 1).Count();
            return check;
        }

        public List<CommonRiskAssessmentVM> Get_ISSC_Member_Risk_For_DivisionHead(long? Userid, long? SYKI)
        {
            var assets = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Userid && x.SKYID == SYKI && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1).Select(x => new CommonRiskAssessmentVM
            {
                ID = x.ID,
                PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKI && x1.ID == x.PRIMARY_ASSETID).Select(x1 => x1.PRIMARY).FirstOrDefault(),

                //RiskCategory = x.RISK_CATEGORY_MASTER != null ? x.RISK_CATEGORY_MASTER.RISK_CATEGORIES : "",
                RiskCategory = _arDBContext.RISK_CATEGORY_MASTER
                .Where(a => a.RISK_ID == x.RISK_CATEGORYID)
                .Select(a => a.RISK_CATEGORIES)
                .FirstOrDefault(),

                //PotentialRisk = x.RISK_STATEMENT_MASTER != null ? x.RISK_STATEMENT_MASTER.RISK_STATEMENT : "",
                PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
                .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
                .Select(a => a.RISK_STATEMENT)
                .FirstOrDefault(),

                RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH.Where(x1 => x1.RISKLEADID == x.RISK_LEADID).Select(x1 => x1.RISKLEADTYPE).FirstOrDefault(),
                RiskOwner = x.RISK_OWNER,


                //Iden_Frequency = x.RISK_FREQUENCY_MASTER != null ? x.RISK_FREQUENCY_MASTER.DEGREE_OF_FREQUENCY : "",
                Iden_Frequency = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),


                Impact_Type = _arDBContext.IMPACTTYPE.Where(x1 => x1.IMAPECTTYPEID == x.IMPACTTYPEID).Select(x1 => x1.IMPACTTYPE1).FirstOrDefault(),


                //Iden_Impact = x.RISK_IMPACT_MASTER != null ? x.RISK_IMPACT_MASTER.DEGREE_OF_IMPACT : "",
                Iden_Impact = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),

                //Iden_Current_Risk_Level = x.RISK_FREQUENCY_IMPACT_MAP_MAST != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST.RESULT : "",
                Iden_Current_Risk_Level = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
                .Select(a => a.RESULT)
                .FirstOrDefault(),


                CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,
                Current_Measures_Level = _arDBContext.RISK_CURRENTMEASURE_LVL.Where(x1 => x1.CURRENTMEASURELEVELID == x.CURRENT_MEASURES_LEVELID).Select(x1 => x1.CUR_LEVEL).FirstOrDefault(),
                Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                Impact_RA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),
                Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL.Where(x1 => x1.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID).Select(x1 => x1.TREATMENT).FirstOrDefault(),
                Responsebiity = x.RESPONSBILITY,
                TargetDate = x.TARGET_DATE,
                ActionItem = x.ACTION_ITEM,
                RiskStatus = (int)x.RISK_STATUS,
                Status = (int)x.STATUS,
                ApproveStatus = (int)x.APRROVE_STATUS,
                Division_Head_EmpCode = (long)x.DIVISION_HEAD_ID,
                Operating_Head_EmpCode = (long)x.OPERATING_HEAD_ID,
                Div_Head_Remark = x.REVIEW_REMARK_DIVISIONHEAD,
                OP_Head_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                Div_Head_Status = (int)x.DIVISION_STATUS,
                ISSC_MEM_Status = (int)x.ISSC_MEMBER_STATUS,
                OP_Head_Status = (int)x.OPERATING_STATUS,
                Frequency_RRA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID_RRA).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                Impact_RRA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID_RRA).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                Current_Risk_Level_RRA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.IMPACT == x.IMAPCTID_RRA && x1.FREQUENCY == x.FREQUENCYID_RRA).Select(x1 => x1.RESULT).FirstOrDefault(),
                CreatedBy = (long)x.RISK_CREATEDBY,
                Other_Potential_Risk = x.OTHERPOTENTIAL
            }).ToList();
            return assets;
        }

        public int Risk_Approve_By_DivisionHead(CommonRiskAssessmentVM asset_details)
        {
            if (asset_details != null && asset_details.SKYID > 0 && asset_details.ISSC_MEM_EmpCode > 0 && asset_details.Division_Head_EmpCode > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                var result_List = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == asset_details.ISSC_MEM_EmpCode && x.SKYID == asset_details.SKYID && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1).ToList();
                result_List.ForEach(ass_det =>
                {
                    ass_det.DIVISION_HEAD_ID = asset_details.Division_Head_EmpCode;
                    ass_det.SUBMIT_DATE_DIVISIONHEAD = DateTime.Now;
                    ass_det.DIVISION_STATUS = 1;
                    ass_det.STATUS = 2;//Approve by division head
                    ass_det.REVIEW_REMARK_DIVISIONHEAD = asset_details.Div_Head_Remark;
                }
                );
                _arDBContext.SaveChanges();
                //}
                return 1;
            }
            return 2;
        }



        public int Risk_SendBack_By_DivisionHead(CommonRiskAssessmentVM asset_details)
        {
            if (asset_details != null && asset_details.SKYID > 0 && asset_details.ISSC_MEM_EmpCode > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                var result_List = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == asset_details.ISSC_MEM_EmpCode && x.SKYID == asset_details.SKYID && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1).ToList();
                result_List.ForEach(Risk_det =>
                {
                    Risk_det.ACTIVE = 1;
                    Risk_det.STATUS = 4;//Send back by division head
                    Risk_det.ISSC_MEMBER_STATUS = null;
                    Risk_det.SUBMIT_DATE_ISSCMEMBER = null;
                    Risk_det.RISK_UPDATEDDATE = DateTime.Now;
                    Risk_det.DIVISION_STATUS = null;
                    Risk_det.SUBMIT_DATE_DIVISIONHEAD = null;
                    Risk_det.REVIEW_REMARK_DIVISIONHEAD = asset_details.Div_Head_Remark;
                    Risk_det.REVIEW_REMARK_OPERATINGHEAD = null;
                }
                );
                _arDBContext.SaveChanges();
                //}
                return 3; //Send back 
            }
            return 2;//There is some problem, please try again later.
        }

        public int Check_Issc_Member_Nomination(long SYKI, int Empcode)
        {
            var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Empcode && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            var check = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == SYKI && x.OPERATIONID == curent_year_deatil.OPERATIONID && x.DIVISIONID == curent_year_deatil.DIVISIONID && x.ISSCEMPCODE == Empcode).Count();
            return check;
        }

        public List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead(CommonRiskAssessmentVM asset_details)
        {
            List<ISSCMember_details> Emp_Details = new List<ISSCMember_details>();
            List<decimal?> empcode = new List<decimal?>();
            var result = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.STATUS == 1 && x.SYKIID == asset_details.SKYID && x.ISSCEMPCODE == asset_details.ISSC_MEM_EmpCode).Select(x => x.ADDEDBY).FirstOrDefault();
            if (result != null)
                empcode.Add(result);
            foreach (var item in empcode)
            {
                var emp = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == item).Select(x => new ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                Emp_Details.Add(emp);
            }
            return Emp_Details;
        }

        public List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead_FroSendBack(CommonRiskAssessmentVM asset_details)
        {
            List<ISSCMember_details> Emp_Details = new List<ISSCMember_details>();
            List<long?> empcode = new List<long?>();
            empcode.Add(asset_details.ISSC_MEM_EmpCode);
            foreach (var item in empcode)
            {
                var emp = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == item).Select(x => new ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                Emp_Details.Add(emp);
            }
            return Emp_Details;
        }

        public List<CommonRiskAssessmentVM> Get_Last_Ki_Asset_DetailsNew(long? Userid, long? SYKI, long? DivId)
        {
            // var lastki = SYKI - 1;
            var get_prevyear_detail = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI && x.DIVISIONID == DivId).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == SYKI && x.OPERATIONID == get_prevyear_detail.OPERATIONID && x.DIVISIONID == get_prevyear_detail.DIVISIONID).Select(x => x.ISSCEMPCODE).FirstOrDefault();
            var assets = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == get_prevyear_userid && x.SKYID == SYKI && x.ACTIVE == 1 && x.OPERATING_STATUS == 1).Select(x => new CommonRiskAssessmentVM

            {
                ID = x.ID,
                PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKI && x1.ID == x.PRIMARY_ASSETID).Select(x1 => x1.PRIMARY).FirstOrDefault(),


                //RiskCategory = x.RISK_CATEGORY_MASTER != null ? x.RISK_CATEGORY_MASTER.RISK_CATEGORIES : "",
                RiskCategory = _arDBContext.RISK_CATEGORY_MASTER
                .Where(a => a.RISK_ID == x.RISK_CATEGORYID)
                .Select(a => a.RISK_CATEGORIES)
                .FirstOrDefault(),

                //PotentialRisk = x.RISK_STATEMENT_MASTER != null ? x.RISK_STATEMENT_MASTER.RISK_STATEMENT : "",
                PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
                .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
                .Select(a => a.RISK_STATEMENT)
                .FirstOrDefault(),

                RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH.Where(x1 => x1.RISKLEADID == x.RISK_LEADID).Select(x1 => x1.RISKLEADTYPE).FirstOrDefault(),
                RiskOwner = x.RISK_OWNER,


                //Iden_Frequency = x.RISK_FREQUENCY_MASTER != null ? x.RISK_FREQUENCY_MASTER.DEGREE_OF_FREQUENCY : "",
                Iden_Frequency = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),

                Impact_Type = _arDBContext.IMPACTTYPE.Where(x1 => x1.IMAPECTTYPEID == x.IMPACTTYPEID).Select(x1 => x1.IMPACTTYPE1).FirstOrDefault(),


                //Iden_Impact = x.RISK_IMPACT_MASTER != null ? x.RISK_IMPACT_MASTER.DEGREE_OF_IMPACT : "",
                Iden_Impact = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),

                //Iden_Current_Risk_Level = x.RISK_FREQUENCY_IMPACT_MAP_MAST != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST.RESULT : "",
                Iden_Current_Risk_Level = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
                .Select(a => a.RESULT)
                .FirstOrDefault(),


                CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,
                Current_Measures_Level = _arDBContext.RISK_CURRENTMEASURE_LVL.Where(x1 => x1.CURRENTMEASURELEVELID == x.CURRENT_MEASURES_LEVELID).Select(x1 => x1.CUR_LEVEL).FirstOrDefault(),
                Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                Impact_RA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),
                Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL.Where(x1 => x1.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID).Select(x1 => x1.TREATMENT).FirstOrDefault(),
                Responsebiity = x.RESPONSBILITY,
                TargetDate = x.TARGET_DATE,
                ActionItem = x.ACTION_ITEM,
                RiskStatus = (int)x.RISK_STATUS,
                Status = (int)x.STATUS,
                ApproveStatus = (int)x.APRROVE_STATUS,
                Division_Head_EmpCode = (long)x.DIVISION_HEAD_ID,
                Operating_Head_EmpCode = (long)x.OPERATING_HEAD_ID,
                Div_Head_Remark = x.REVIEW_REMARK_DIVISIONHEAD,
                OP_Head_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                Div_Head_Status = (int)x.DIVISION_STATUS,
                ISSC_MEM_Status = (int)x.ISSC_MEMBER_STATUS,
                OP_Head_Status = (int)x.OPERATING_STATUS,
                Frequency_RRA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID_RRA).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                Impact_RRA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID_RRA).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                Current_Risk_Level_RRA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.IMPACT == x.IMAPCTID_RRA && x1.FREQUENCY == x.FREQUENCYID_RRA).Select(x1 => x1.RESULT).FirstOrDefault(),
                CreatedBy = (long)x.RISK_CREATEDBY,
                Other_Potential_Risk = x.OTHERPOTENTIAL
            }).ToList();
            return assets;
        }

        public List<DownloadReportVM> Get_Risk_AssessmentFor_Download(long Userid, long? SYKI)
        {
            var assets = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Userid && x.SKYID == SYKI && x.ACTIVE == 1).Select(x => new DownloadReportVM
            {
                PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKI && x1.ID == x.PRIMARY_ASSETID).Select(x1 => x1.PRIMARY).FirstOrDefault(),

                //RiskCategory = x.RISK_CATEGORY_MASTER != null ? x.RISK_CATEGORY_MASTER.RISK_CATEGORIES : "",
                RiskCategory = _arDBContext.RISK_CATEGORY_MASTER
                .Where(a => a.RISK_ID == x.RISK_CATEGORYID)
                .Select(a => a.RISK_CATEGORIES)
                .FirstOrDefault(),

                //PotentialRisk = x.RISK_STATEMENT_MASTER != null ? x.RISK_STATEMENT_MASTER.RISK_STATEMENT : "",
                PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
                .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
                .Select(a => a.RISK_STATEMENT)
                .FirstOrDefault(),

                RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH.Where(x1 => x1.RISKLEADID == x.RISK_LEADID).Select(x1 => x1.RISKLEADTYPE).FirstOrDefault(),
                RiskOwner = x.RISK_OWNER,

                //Iden_Frequency = x.RISK_FREQUENCY_MASTER != null ? x.RISK_FREQUENCY_MASTER.DEGREE_OF_FREQUENCY : "",
                Iden_Frequency = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),

                Impact_Type = _arDBContext.IMPACTTYPE.Where(x1 => x1.IMAPECTTYPEID == x.IMPACTTYPEID).Select(x1 => x1.IMPACTTYPE1).FirstOrDefault(),


                //Iden_Impact = x.RISK_IMPACT_MASTER != null ? x.RISK_IMPACT_MASTER.DEGREE_OF_IMPACT : "",
                Iden_Impact = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),

                //Iden_Current_Risk_Level = x.RISK_FREQUENCY_IMPACT_MAP_MAST != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST.RESULT : "",
                Iden_Current_Risk_Level = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
                .Select(a => a.RESULT)
                .FirstOrDefault(),

                CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,
                Current_Measures_Level = _arDBContext.RISK_CURRENTMEASURE_LVL.Where(x1 => x1.CURRENTMEASURELEVELID == x.CURRENT_MEASURES_LEVELID).Select(x1 => x1.CUR_LEVEL).FirstOrDefault(),


                //Frequency_RA = x.RISK_FREQUENCY_MASTER1 != null ? x.RISK_FREQUENCY_MASTER1.DEGREE_OF_FREQUENCY : "",
                Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID_RISKIDENTIFICATION)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),


                //Impact_RA = x.RISK_IMPACT_MASTER1 != null ? x.RISK_IMPACT_MASTER1.DEGREE_OF_IMPACT : "",
                Impact_RA = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID_RISKIDENTIFICATION)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),


                //Current_Risk_Level_RA = x.RISK_FREQUENCY_IMPACT_MAP_MAST1 != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST1.RESULT : "",
                Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.DEG_LEVELID_RISKIDENTIFICATION)
                .Select(a => a.RESULT)
                .FirstOrDefault(),


                Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL.Where(x1 => x1.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID).Select(x1 => x1.TREATMENT).FirstOrDefault(),
                Responsebiity = x.RESPONSBILITY,
                TargetDate = x.TARGET_DATE != null ? Convert.ToDateTime(x.TARGET_DATE).ToString("dd/MM/yyyy") : "",
                ActionItem = x.ACTION_ITEM,
                Status = x.RISK_STATUS == 1 || x.RISK_STATUS == 0 ? "Open" : "Close",

                //Frequency_RRA = x.RISK_FREQUENCY_MASTER2 != null ? x.RISK_FREQUENCY_MASTER2.DEGREE_OF_FREQUENCY : "",
                Frequency_RRA = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID_RRA)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),


                //Impact_RRA = x.RISK_IMPACT_MASTER2 != null ? x.RISK_IMPACT_MASTER2.DEGREE_OF_IMPACT : "",
                Impact_RRA = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID_RRA)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),


                //Current_Risk_Level_RRA = x.RISK_FREQUENCY_IMPACT_MAP_MAST2 != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST2.RESULT : "",
                Current_Risk_Level_RRA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.RISK_DEGREE_LEVELID_RRA)
                .Select(a => a.RESULT)
                .FirstOrDefault(),


            }).ToList();
            return assets;
        }

        public List<CommonRiskAssessmentVM> Get_Risk_Assessment(long Userid, long? SYKI)
        {
            var assets = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Userid && x.SKYID == SYKI && x.ACTIVE == 1).Select(x => new CommonRiskAssessmentVM
            {
                ID = x.ID,
                PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKI && x1.ID == x.PRIMARY_ASSETID).Select(x1 => x1.PRIMARY).FirstOrDefault(),

                //RiskCategory = x.RISK_CATEGORY_MASTER != null ? x.RISK_CATEGORY_MASTER.RISK_CATEGORIES : "",
                RiskCategory = _arDBContext.RISK_CATEGORY_MASTER
                .Where(a => a.RISK_ID == x.RISK_CATEGORYID)
                .Select(a => a.RISK_CATEGORIES)
                .FirstOrDefault(),

                //PotentialRisk = x.RISK_STATEMENT_MASTER != null ? x.RISK_STATEMENT_MASTER.RISK_STATEMENT : "",
                PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
                .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
                .Select(a => a.RISK_STATEMENT)
                .FirstOrDefault(),

                RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH.Where(x1 => x1.RISKLEADID == x.RISK_LEADID).Select(x1 => x1.RISKLEADTYPE).FirstOrDefault(),
                RiskOwner = x.RISK_OWNER,


                //Iden_Frequency = x.RISK_FREQUENCY_MASTER != null ? x.RISK_FREQUENCY_MASTER.DEGREE_OF_FREQUENCY : "",
                Iden_Frequency = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),


                Impact_Type = _arDBContext.IMPACTTYPE.Where(x1 => x1.IMAPECTTYPEID == x.IMPACTTYPEID).Select(x1 => x1.IMPACTTYPE1).FirstOrDefault(),


                //Iden_Impact = x.RISK_IMPACT_MASTER != null ? x.RISK_IMPACT_MASTER.DEGREE_OF_IMPACT : "",
                Iden_Impact = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),

                //Iden_Current_Risk_Level = x.RISK_FREQUENCY_IMPACT_MAP_MAST != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST.RESULT : "",
                Iden_Current_Risk_Level = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
                .Select(a => a.RESULT)
                .FirstOrDefault(),

                CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,
                Current_Measures_Level = _arDBContext.RISK_CURRENTMEASURE_LVL.Where(x1 => x1.CURRENTMEASURELEVELID == x.CURRENT_MEASURES_LEVELID).Select(x1 => x1.CUR_LEVEL).FirstOrDefault(),
                Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                Impact_RA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),
                Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL.Where(x1 => x1.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID).Select(x1 => x1.TREATMENT).FirstOrDefault(),
                Responsebiity = x.RESPONSBILITY,
                TargetDate = x.TARGET_DATE,
                ActionItem = x.ACTION_ITEM,
                RiskStatus = (int)x.RISK_STATUS,
                Status = (int)x.STATUS,
                ApproveStatus = (int)x.APRROVE_STATUS,
                Division_Head_EmpCode = (long)x.DIVISION_HEAD_ID,
                Operating_Head_EmpCode = (long)x.OPERATING_HEAD_ID,
                Div_Head_Remark = x.REVIEW_REMARK_DIVISIONHEAD,
                OP_Head_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                Div_Head_Status = (int)x.DIVISION_STATUS,
                ISSC_MEM_Status = (int)x.ISSC_MEMBER_STATUS,
                OP_Head_Status = (int)x.OPERATING_STATUS,
                Frequency_RRA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID_RRA).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                Impact_RRA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID_RRA).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                Current_Risk_Level_RRA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.IMPACT == x.IMAPCTID_RRA && x1.FREQUENCY == x.FREQUENCYID_RRA).Select(x1 => x1.RESULT).FirstOrDefault(),
                CreatedBy = (long)x.RISK_CREATEDBY,
                Other_Potential_Risk = x.OTHERPOTENTIAL
            }).OrderBy(y => y.ID).ToList();
            return assets;
        }

        public List<CommonRiskAssessmentVM> Get_Risk_Details_Report_OperationHead_Division_Wise(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            List<CommonRiskAssessmentVM> risklist = new List<CommonRiskAssessmentVM>();
            var operations = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => (decimal)x.OPERATIONID).ToList();
            var emplist = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == currentki && x.ACTIVE == 1 && operations.Contains(x.OPERATIONID)).Select(x => x.ISSCEMPCODE).ToList();
            foreach (var item in emplist)
            {
                var risks = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == item && x.SKYID == currentki && x.ACTIVE == 1 && x.STATUS == 3 && x.ISSC_MEMBER_STATUS == 1 && x.OPERATING_STATUS == 1 && x.DIVISION_STATUS == 1).Select(x => new CommonRiskAssessmentVM
                {
                    ID = x.ID,
                    PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == currentki && x1.ID == x.PRIMARY_ASSETID).Select(x1 => x1.PRIMARY).FirstOrDefault(),

                    //RiskCategory = x.RISK_CATEGORY_MASTER != null ? x.RISK_CATEGORY_MASTER.RISK_CATEGORIES : "",
                    RiskCategory = _arDBContext.RISK_CATEGORY_MASTER
                .Where(a => a.RISK_ID == x.RISK_CATEGORYID)
                .Select(a => a.RISK_CATEGORIES)
                .FirstOrDefault(),

                    //PotentialRisk = x.RISK_STATEMENT_MASTER != null ? x.RISK_STATEMENT_MASTER.RISK_STATEMENT : "",
                    PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
                .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
                .Select(a => a.RISK_STATEMENT)
                .FirstOrDefault(),

                    RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH.Where(x1 => x1.RISKLEADID == x.RISK_LEADID).Select(x1 => x1.RISKLEADTYPE).FirstOrDefault(),
                    RiskOwner = x.RISK_OWNER,


                    //Iden_Frequency = x.RISK_FREQUENCY_MASTER != null ? x.RISK_FREQUENCY_MASTER.DEGREE_OF_FREQUENCY : "",
                    Iden_Frequency = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),

                    Impact_Type = _arDBContext.IMPACTTYPE.Where(x1 => x1.IMAPECTTYPEID == x.IMPACTTYPEID).Select(x1 => x1.IMPACTTYPE1).FirstOrDefault(),


                    //Iden_Impact = x.RISK_IMPACT_MASTER != null ? x.RISK_IMPACT_MASTER.DEGREE_OF_IMPACT : "",
                    Iden_Impact = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),

                    //Iden_Current_Risk_Level = x.RISK_FREQUENCY_IMPACT_MAP_MAST != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST.RESULT : "",
                    Iden_Current_Risk_Level = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
                .Select(a => a.RESULT)
                .FirstOrDefault(),

                    CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,
                    Current_Measures_Level = _arDBContext.RISK_CURRENTMEASURE_LVL.Where(x1 => x1.CURRENTMEASURELEVELID == x.CURRENT_MEASURES_LEVELID).Select(x1 => x1.CUR_LEVEL).FirstOrDefault(),
                    Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                    Impact_RA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                    Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),
                    Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL.Where(x1 => x1.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID).Select(x1 => x1.TREATMENT).FirstOrDefault(),
                    Responsebiity = x.RESPONSBILITY,
                    TargetDate = x.TARGET_DATE,
                    ActionItem = x.ACTION_ITEM,
                    RiskStatus = (int)x.RISK_STATUS,
                    Status = (int)x.STATUS,
                    ApproveStatus = (int)x.APRROVE_STATUS,
                    Division_Head_EmpCode = (long)x.DIVISION_HEAD_ID,
                    Operating_Head_EmpCode = (long)x.OPERATING_HEAD_ID,
                    Div_Head_Remark = x.REVIEW_REMARK_DIVISIONHEAD,
                    OP_Head_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                    Div_Head_Status = (int)x.DIVISION_STATUS,
                    ISSC_MEM_Status = (int)x.ISSC_MEMBER_STATUS,
                    OP_Head_Status = (int)x.OPERATING_STATUS,
                    Frequency_RRA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID_RRA).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                    Impact_RRA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID_RRA).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                    Current_Risk_Level_RRA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.IMPACT == x.IMAPCTID_RRA && x1.FREQUENCY == x.FREQUENCYID_RRA).Select(x1 => x1.RESULT).FirstOrDefault(),
                    CreatedBy = (long)x.RISK_CREATEDBY,
                    Other_Potential_Risk = x.OTHERPOTENTIAL,
                    OperatingHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ADEMPCODE == x.RISK_CREATEDBY && y.SYKI == x.SKYID).Select(y => y.OPERATION).FirstOrDefault() == null ? "" : _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ADEMPCODE == x.RISK_CREATEDBY && y.SYKI == x.SKYID).Select(y => y.OPERATION).FirstOrDefault(),
                    DivisionHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKYID && y.ADEMPCODE == x.RISK_CREATEDBY).Select(y => y.DIVISION).FirstOrDefault() == null ? "" : _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKYID && y.ADEMPCODE == x.RISK_CREATEDBY).Select(y => y.DIVISION).FirstOrDefault(),
                }).ToList();//Yogesh
                risklist.AddRange(risks);
            }
            return risklist;
        }

        public List<DivisionWise_Risk_Count> Get_Risk_Report_Count_OperationHead_DivisionWise(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            List<DivisionWise_Risk_Count> risklist = new List<DivisionWise_Risk_Count>();
            var operations = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => (decimal)x.OPERATIONID).ToList();
            var emplist = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == currentki && x.ACTIVE == 1 && operations.Contains(x.OPERATIONID)).Select(x => new { x.ISSCEMPCODE, x.DIVISIONID }).ToList();
            foreach (var item in emplist)
            {
                DivisionWise_Risk_Count qwerty = new DivisionWise_Risk_Count();
                var assets = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == item.ISSCEMPCODE && x.SKYID == currentki && x.ACTIVE == 1 & x.STATUS == 3 && x.ISSC_MEMBER_STATUS == 1 && x.OPERATING_STATUS == 1 && x.DIVISION_STATUS == 1).Select(x => new
                {
                    DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ACTIVE == 1 && y.ADEMPCODE == item.ISSCEMPCODE && y.SYKI == currentki && y.DIVISIONID == item.DIVISIONID).Select(y => y.DIVISION).FirstOrDefault(),
                    Id = x.ID,
                    Current_Degree_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),

                }).ToList();
                if (assets.Count() > 0)
                {
                    qwerty = assets.Select(x => new DivisionWise_Risk_Count { DivisionName = x.DivisionName }).FirstOrDefault();
                    qwerty.TSCount = assets.Where(x => x.Current_Degree_RA == "S").Count();
                    qwerty.TACount = assets.Where(x => x.Current_Degree_RA == "A").Count();
                    qwerty.TBCount = assets.Where(x => x.Current_Degree_RA == "B").Count();
                    qwerty.TCCount = assets.Where(x => x.Current_Degree_RA == "C").Count();
                    qwerty.TDCount = assets.Where(x => x.Current_Degree_RA == "D").Count();
                    risklist.Add(qwerty);
                }

            }
            return risklist;
        }

        public List<CommonRiskAssessmentVM> Get_Risk_Details_Report_Operation_Division_Wise(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            List<CommonRiskAssessmentVM> risklist = new List<CommonRiskAssessmentVM>();
            var divisionds = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => (decimal)x.DIVISIONID).ToList();
            var emplist = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == currentki && x.ACTIVE == 1 && divisionds.Contains(x.DIVISIONID)).Select(x => x.ISSCEMPCODE).ToList();
            foreach (var item in emplist)
            {
                var risks = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == item && x.SKYID == currentki && x.ACTIVE == 1 && x.STATUS == 3 && x.ISSC_MEMBER_STATUS == 1 && x.OPERATING_STATUS == 1 && x.DIVISION_STATUS == 1).Select(x => new CommonRiskAssessmentVM
                {
                    ID = x.ID,
                    PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == currentki && x1.ID == x.PRIMARY_ASSETID).Select(x1 => x1.PRIMARY).FirstOrDefault(),

                    //RiskCategory = x.RISK_CATEGORY_MASTER != null ? x.RISK_CATEGORY_MASTER.RISK_CATEGORIES : "",
                    RiskCategory = _arDBContext.RISK_CATEGORY_MASTER
                .Where(a => a.RISK_ID == x.RISK_CATEGORYID)
                .Select(a => a.RISK_CATEGORIES)
                .FirstOrDefault(),

                    //PotentialRisk = x.RISK_STATEMENT_MASTER != null ? x.RISK_STATEMENT_MASTER.RISK_STATEMENT : "",
                    PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
                .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
                .Select(a => a.RISK_STATEMENT)
                .FirstOrDefault(),


                    RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH.Where(x1 => x1.RISKLEADID == x.RISK_LEADID).Select(x1 => x1.RISKLEADTYPE).FirstOrDefault(),
                    RiskOwner = x.RISK_OWNER,


                    //Iden_Frequency = x.RISK_FREQUENCY_MASTER != null ? x.RISK_FREQUENCY_MASTER.DEGREE_OF_FREQUENCY : "",
                    Iden_Frequency = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),

                    Impact_Type = _arDBContext.IMPACTTYPE.Where(x1 => x1.IMAPECTTYPEID == x.IMPACTTYPEID).Select(x1 => x1.IMPACTTYPE1).FirstOrDefault(),


                    //Iden_Impact = x.RISK_IMPACT_MASTER != null ? x.RISK_IMPACT_MASTER.DEGREE_OF_IMPACT : "",
                    Iden_Impact = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),

                    //Iden_Current_Risk_Level = x.RISK_FREQUENCY_IMPACT_MAP_MAST != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST.RESULT : "",
                    Iden_Current_Risk_Level = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
                .Select(a => a.RESULT)
                .FirstOrDefault(),

                    CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,
                    Current_Measures_Level = _arDBContext.RISK_CURRENTMEASURE_LVL.Where(x1 => x1.CURRENTMEASURELEVELID == x.CURRENT_MEASURES_LEVELID).Select(x1 => x1.CUR_LEVEL).FirstOrDefault(),
                    Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                    Impact_RA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                    Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),
                    Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL.Where(x1 => x1.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID).Select(x1 => x1.TREATMENT).FirstOrDefault(),
                    Responsebiity = x.RESPONSBILITY,
                    TargetDate = x.TARGET_DATE,
                    ActionItem = x.ACTION_ITEM,
                    RiskStatus = (int)x.RISK_STATUS,
                    Status = (int)x.STATUS,
                    ApproveStatus = (int)x.APRROVE_STATUS,
                    Division_Head_EmpCode = (long)x.DIVISION_HEAD_ID,
                    Operating_Head_EmpCode = (long)x.OPERATING_HEAD_ID,
                    Div_Head_Remark = x.REVIEW_REMARK_DIVISIONHEAD,
                    OP_Head_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                    Div_Head_Status = (int)x.DIVISION_STATUS,
                    ISSC_MEM_Status = (int)x.ISSC_MEMBER_STATUS,
                    OP_Head_Status = (int)x.OPERATING_STATUS,
                    Frequency_RRA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID_RRA).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                    Impact_RRA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID_RRA).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                    Current_Risk_Level_RRA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.IMPACT == x.IMAPCTID_RRA && x1.FREQUENCY == x.FREQUENCYID_RRA).Select(x1 => x1.RESULT).FirstOrDefault(),
                    CreatedBy = (long)x.RISK_CREATEDBY,
                    Other_Potential_Risk = x.OTHERPOTENTIAL,
                    OperatingHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ADEMPCODE == x.RISK_CREATEDBY && y.SYKI == x.SKYID).Select(y => y.OPERATION).FirstOrDefault() == null ? "" : _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ADEMPCODE == x.RISK_CREATEDBY && y.SYKI == x.SKYID).Select(y => y.OPERATION).FirstOrDefault(),
                    DivisionHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKYID && y.ADEMPCODE == x.RISK_CREATEDBY).Select(y => y.DIVISION).FirstOrDefault() == null ? "" : _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKYID && y.ADEMPCODE == x.RISK_CREATEDBY).Select(y => y.DIVISION).FirstOrDefault(),
                }).ToList();//Yogesh
                risklist.AddRange(risks);
            }
            return risklist;
        }

        public List<DivisionWise_Risk_Count> Get_Risk_Report_Count_DivisionWise(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            List<DivisionWise_Risk_Count> risklist = new List<DivisionWise_Risk_Count>();
            var divisionds = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == currentki && x.ADEMPCODE == Id).Select(x => (decimal)x.DIVISIONID).ToList();
            var emplist = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == currentki && x.ACTIVE == 1 && divisionds.Contains(x.DIVISIONID)).Select(x => new { x.ISSCEMPCODE, x.DIVISIONID }).ToList();
            foreach (var item in emplist)
            {
                DivisionWise_Risk_Count qwerty = new DivisionWise_Risk_Count();
                var assets = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == item.ISSCEMPCODE && x.SKYID == currentki && x.ACTIVE == 1 & x.STATUS == 3 && x.ISSC_MEMBER_STATUS == 1 && x.OPERATING_STATUS == 1 && x.DIVISION_STATUS == 1).Select(x => new
                {
                    DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ACTIVE == 1 && y.ADEMPCODE == item.ISSCEMPCODE && y.SYKI == currentki && y.DIVISIONID == item.DIVISIONID).Select(y => y.DIVISION).FirstOrDefault(),
                    Id = x.ID,
                    Current_Degree_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),

                }).ToList();
                if (assets.Count() > 0)
                {
                    qwerty = assets.Select(x => new DivisionWise_Risk_Count { DivisionName = x.DivisionName }).FirstOrDefault();
                    qwerty.TSCount = assets.Where(x => x.Current_Degree_RA == "S").Count();
                    qwerty.TACount = assets.Where(x => x.Current_Degree_RA == "A").Count();
                    qwerty.TBCount = assets.Where(x => x.Current_Degree_RA == "B").Count();
                    qwerty.TCCount = assets.Where(x => x.Current_Degree_RA == "C").Count();
                    qwerty.TDCount = assets.Where(x => x.Current_Degree_RA == "D").Count();
                    risklist.Add(qwerty);
                }

            }
            return risklist;
        }


        public List<CommonRiskAssessmentVM> Get_Risk_Details_Operation_Division_Wise(long SYKI, decimal? operationid, decimal? divisionid)
        {
            List<CommonRiskAssessmentVM> risklist = new List<CommonRiskAssessmentVM>();
            var emplist = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKI && x.ACTIVE == 1 && x.OPERATIONID == (operationid == 0 ? x.OPERATIONID : operationid) && x.DIVISIONID == (divisionid == 0 ? x.DIVISIONID : divisionid)).Select(x => x.ISSCEMPCODE).ToList();
            foreach (var item in emplist)
            {
                var risks = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == item && x.SKYID == SYKI && x.ACTIVE == 1 && x.STATUS == 3 && x.ISSC_MEMBER_STATUS == 1 && x.OPERATING_STATUS == 1 && x.DIVISION_STATUS == 1).Select(x => new CommonRiskAssessmentVM
                {
                    ID = x.ID,
                    PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKI && x1.ID == x.PRIMARY_ASSETID).Select(x1 => x1.PRIMARY).FirstOrDefault(),

                    //RiskCategory = x.RISK_CATEGORY_MASTER != null ? x.RISK_CATEGORY_MASTER.RISK_CATEGORIES : "",
                    RiskCategory = _arDBContext.RISK_CATEGORY_MASTER
                                    .Where(a => a.RISK_ID == x.RISK_CATEGORYID)
                                    .Select(a => a.RISK_CATEGORIES)
                                    .FirstOrDefault(),



                    //PotentialRisk = x.RISK_STATEMENT_MASTER != null ? x.RISK_STATEMENT_MASTER.RISK_STATEMENT : "",
                    PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
                                    .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
                                    .Select(a => a.RISK_STATEMENT)
                                    .FirstOrDefault(),


                    RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH.Where(x1 => x1.RISKLEADID == x.RISK_LEADID).Select(x1 => x1.RISKLEADTYPE).FirstOrDefault(),
                    RiskOwner = x.RISK_OWNER,

                    //Iden_Frequency = x.RISK_FREQUENCY_MASTER != null ? x.RISK_FREQUENCY_MASTER.DEGREE_OF_FREQUENCY : "",
                    Iden_Frequency = _arDBContext.RISK_FREQUENCY_MASTER
                                    .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
                                    .Select(a => a.DEGREE_OF_FREQUENCY)
                                    .FirstOrDefault(),



                    Impact_Type = _arDBContext.IMPACTTYPE.Where(x1 => x1.IMAPECTTYPEID == x.IMPACTTYPEID).Select(x1 => x1.IMPACTTYPE1).FirstOrDefault(),


                    //Iden_Impact = x.RISK_IMPACT_MASTER != null ? x.RISK_IMPACT_MASTER.DEGREE_OF_IMPACT : "",
                    Iden_Impact = _arDBContext.RISK_IMPACT_MASTER
                                    .Where(a => a.RISK_LEVEL == x.IMAPCTID)
                                    .Select(a => a.DEGREE_OF_IMPACT)
                                    .FirstOrDefault(),



                    //Iden_Current_Risk_Level = x.RISK_FREQUENCY_IMPACT_MAP_MAST != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST.RESULT : "",
                    Iden_Current_Risk_Level = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                                    .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
                                    .Select(a => a.RESULT)
                                    .FirstOrDefault(),



                    CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,
                    Current_Measures_Level = _arDBContext.RISK_CURRENTMEASURE_LVL.Where(x1 => x1.CURRENTMEASURELEVELID == x.CURRENT_MEASURES_LEVELID).Select(x1 => x1.CUR_LEVEL).FirstOrDefault(),
                    Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                    Impact_RA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                    Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),
                    Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL.Where(x1 => x1.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID).Select(x1 => x1.TREATMENT).FirstOrDefault(),
                    Responsebiity = x.RESPONSBILITY,
                    TargetDate = x.TARGET_DATE,
                    ActionItem = x.ACTION_ITEM,
                    RiskStatus = (int)x.RISK_STATUS,
                    Status = (int)x.STATUS,
                    ApproveStatus = (int)x.APRROVE_STATUS,
                    Division_Head_EmpCode = (long)x.DIVISION_HEAD_ID,
                    Operating_Head_EmpCode = (long)x.OPERATING_HEAD_ID,
                    Div_Head_Remark = x.REVIEW_REMARK_DIVISIONHEAD,
                    OP_Head_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                    Div_Head_Status = (int)x.DIVISION_STATUS,
                    ISSC_MEM_Status = (int)x.ISSC_MEMBER_STATUS,
                    OP_Head_Status = (int)x.OPERATING_STATUS,
                    Frequency_RRA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID_RRA).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                    Impact_RRA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID_RRA).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                    Current_Risk_Level_RRA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.IMPACT == x.IMAPCTID_RRA && x1.FREQUENCY == x.FREQUENCYID_RRA).Select(x1 => x1.RESULT).FirstOrDefault(),
                    CreatedBy = (long)x.RISK_CREATEDBY,
                    Other_Potential_Risk = x.OTHERPOTENTIAL,
                    OperatingHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ADEMPCODE == x.RISK_CREATEDBY && y.SYKI == x.SKYID).Select(y => y.OPERATION).FirstOrDefault() == null ? "" : _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ADEMPCODE == x.RISK_CREATEDBY && y.SYKI == x.SKYID).Select(y => y.OPERATION).FirstOrDefault(),
                    DivisionHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKYID && y.ADEMPCODE == x.RISK_CREATEDBY).Select(y => y.DIVISION).FirstOrDefault() == null ? "" : _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKYID && y.ADEMPCODE == x.RISK_CREATEDBY).Select(y => y.DIVISION).FirstOrDefault(),
                    DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ADEMPCODE == x.DIVISION_HEAD_ID && y.SYKI == x.SKYID).Select(y => y.DIVISION).FirstOrDefault()
                }).ToList();//Yogesh
                risklist.AddRange(risks);
            }
            return risklist;
        }

        //public List<CommonRiskAssessmentVM> Get_Risk_Details_Operation_Division_Wise(long SYKI, decimal? operationid, decimal? divisionid)
        //{
        //    List<CommonRiskAssessmentVM> risklist = new List<CommonRiskAssessmentVM>();

        //    // Step 1: Get Employee list
        //    var emplist = _arDBContext.ASSET_REGISTER_MEMNOMINATION
        //        .Where(x =>
        //            x.SYKIID == SYKI &&
        //            x.ACTIVE == 1 &&
        //            (operationid == 0 || x.OPERATIONID == operationid) &&
        //            (divisionid == 0 || x.DIVISIONID == divisionid))
        //        .Select(x => x.ISSCEMPCODE)
        //        .ToList();

        //    // Step 2: Get Risks (with Includes for navigation properties)
        //    //var risks = _arDBContext.RISKASSESSMENT
        //    //    .Include(r => r.RISK_CATEGORY_MASTER)
        //    //    .Include(r => r.RISK_STATEMENT_MASTER)
        //    //    .Include(r => r.RISK_FREQUENCY_MASTER)
        //    //    .Include(r => r.RISK_IMPACT_MASTER)
        //    //    .Include(r => r.RISK_FREQUENCY_IMPACT_MAP_MAST)
        //    //    .Where(r =>
        //    //        r.SKYID == SYKI &&
        //    //        r.ACTIVE == 1 &&
        //    //        r.STATUS == 3 &&
        //    //        r.ISSC_MEMBER_STATUS == 1 &&
        //    //        r.OPERATING_STATUS == 1 &&
        //    //        r.DIVISION_STATUS == 1 &&
        //    //        emplist.Contains(Convert.ToDecimal(r.RISK_CREATEDBY)))
        //    //    .ToList(); // materialize first

        //    var risks = _arDBContext.RISKASSESSMENT                
        //        .Where(r =>
        //            r.SKYID == SYKI &&
        //            r.ACTIVE == 1 &&
        //            r.STATUS == 3 &&
        //            r.ISSC_MEMBER_STATUS == 1 &&
        //            r.OPERATING_STATUS == 1 &&
        //            r.DIVISION_STATUS == 1 &&
        //            emplist.Contains(Convert.ToDecimal(r.RISK_CREATEDBY)))
        //        .ToList(); // materialize first

        //    // Step 3: Map to VM
        //    foreach (var x in risks)
        //    {
        //        var vm = new CommonRiskAssessmentVM
        //        {
        //            ID = x.ID,
        //            PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS
        //                            .Where(a => a.SKIID == SYKI && a.ID == x.PRIMARY_ASSETID)
        //                            .Select(a => a.PRIMARY)
        //                            .FirstOrDefault(),

        //            //RiskCategory = x.RISK_CATEGORY_MASTER?.RISK_CATEGORIES ?? "",
        //            RiskCategory = _arDBContext.RISK_CATEGORY_MASTER
        //                            .Where(a => a.RISK_ID == x.RISK_CATEGORYID)
        //                            .Select(a => a.RISK_CATEGORIES)
        //                            .FirstOrDefault(),

        //            //PotentialRisk = x.RISK_STATEMENT_MASTER?.RISK_STATEMENT ?? "",
        //            PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
        //                            .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
        //                            .Select(a => a.RISK_STATEMENT)
        //                            .FirstOrDefault(),

        //            RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH
        //                                .Where(b => b.RISKLEADID == x.RISK_LEADID)
        //                                .Select(b => b.RISKLEADTYPE)
        //                                .FirstOrDefault(),

        //            RiskOwner = x.RISK_OWNER,

        //            //Iden_Frequency = x.RISK_FREQUENCY_MASTER?.DEGREE_OF_FREQUENCY ?? "",
        //            Iden_Frequency = _arDBContext.RISK_FREQUENCY_MASTER
        //                            .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
        //                            .Select(a => a.DEGREE_OF_FREQUENCY)
        //                            .FirstOrDefault(),


        //            Impact_Type = _arDBContext.IMPACTTYPE
        //                            .Where(i => i.IMAPECTTYPEID == x.IMPACTTYPEID)
        //                            .Select(i => i.IMPACTTYPE1)
        //                            .FirstOrDefault(),

        //            //Iden_Impact = x.RISK_IMPACT_MASTER?.DEGREE_OF_IMPACT ?? "",
        //            Iden_Impact = _arDBContext.RISK_IMPACT_MASTER
        //                            .Where(a => a.RISK_LEVEL == x.IMAPCTID)
        //                            .Select(a => a.DEGREE_OF_IMPACT)
        //                            .FirstOrDefault(),




        //            //Iden_Current_Risk_Level = x.RISK_FREQUENCY_IMPACT_MAP_MAST?.RESULT ?? "",
        //            Iden_Current_Risk_Level = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
        //                            .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
        //                            .Select(a => a.RESULT)
        //                            .FirstOrDefault(),

        //            CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,

        //            Current_Measures_Level = _arDBContext.RISK_CURRENTMEASURE_LVL
        //                                        .Where(c => c.CURRENTMEASURELEVELID == x.CURRENT_MEASURES_LEVELID)
        //                                        .Select(c => c.CUR_LEVEL)
        //                                        .FirstOrDefault(),

        //            Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER
        //                            .Where(f => f.FREQUENCY_LEVEL == x.FREQUENCYID)
        //                            .Select(f => f.DEGREE_OF_FREQUENCY)
        //                            .FirstOrDefault(),

        //            Impact_RA = _arDBContext.RISK_IMPACT_MASTER
        //                            .Where(i => i.RISK_LEVEL == x.IMAPCTID)
        //                            .Select(i => i.DEGREE_OF_IMPACT)
        //                            .FirstOrDefault(),

        //            Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
        //                            .Where(m => m.FREQUENCY == x.FREQUENCYID && m.IMPACT == x.IMAPCTID)
        //                            .Select(m => m.RESULT)
        //                            .FirstOrDefault(),

        //            Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL
        //                            .Where(t => t.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID)
        //                            .Select(t => t.TREATMENT)
        //                            .FirstOrDefault(),

        //            Responsebiity = x.RESPONSBILITY,
        //            TargetDate = x.TARGET_DATE,
        //            ActionItem = x.ACTION_ITEM,
        //            RiskStatus = (int)x.RISK_STATUS,
        //            Status = (int)x.STATUS,
        //            ApproveStatus = (int)x.APRROVE_STATUS,
        //            Division_Head_EmpCode = (long)x.DIVISION_HEAD_ID,
        //            Operating_Head_EmpCode = (long)x.OPERATING_HEAD_ID,
        //            Div_Head_Remark = x.REVIEW_REMARK_DIVISIONHEAD,
        //            OP_Head_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
        //            Div_Head_Status = (int)x.DIVISION_STATUS,
        //            ISSC_MEM_Status = (int)x.ISSC_MEMBER_STATUS,
        //            OP_Head_Status = (int)x.OPERATING_STATUS,

        //            Frequency_RRA = _arDBContext.RISK_FREQUENCY_MASTER
        //                                .Where(f => f.FREQUENCY_LEVEL == x.FREQUENCYID_RRA)
        //                                .Select(f => f.DEGREE_OF_FREQUENCY)
        //                                .FirstOrDefault(),

        //            Impact_RRA = _arDBContext.RISK_IMPACT_MASTER
        //                                .Where(i => i.RISK_LEVEL == x.IMAPCTID_RRA)
        //                                .Select(i => i.DEGREE_OF_IMPACT)
        //                                .FirstOrDefault(),

        //            Current_Risk_Level_RRA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
        //                                .Where(m => m.IMPACT == x.IMAPCTID_RRA && m.FREQUENCY == x.FREQUENCYID_RRA)
        //                                .Select(m => m.RESULT)
        //                                .FirstOrDefault(),

        //            CreatedBy = (long)x.RISK_CREATEDBY,
        //            Other_Potential_Risk = x.OTHERPOTENTIAL,

        //            OperatingHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS
        //                                .Where(y => y.ADEMPCODE == x.RISK_CREATEDBY && y.SYKI == x.SKYID)
        //                                .Select(y => y.OPERATION)
        //                                .FirstOrDefault() ?? "",

        //            DivisionHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS
        //                                .Where(y => y.SYKI == x.SKYID && y.ADEMPCODE == x.RISK_CREATEDBY)
        //                                .Select(y => y.DIVISION)
        //                                .FirstOrDefault() ?? "",

        //            DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS
        //                                .Where(y => y.ADEMPCODE == x.DIVISION_HEAD_ID && y.SYKI == x.SKYID)
        //                                .Select(y => y.DIVISION)
        //                                .FirstOrDefault()
        //        };

        //        risklist.Add(vm);
        //    }

        //    return risklist;
        //}


        public List<DivisionWise_Risk_Count> Get_Risk_Count_DivisionWise(long SYKI, decimal? operationid, decimal? divisionid)
        {
            List<DivisionWise_Risk_Count> risklist = new List<DivisionWise_Risk_Count>();
            var emplist = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKI && x.ACTIVE == 1 && x.OPERATIONID == (operationid == 0 ? x.OPERATIONID : operationid) && x.DIVISIONID == (divisionid == 0 ? x.DIVISIONID : divisionid)).Select(x => new { x.ISSCEMPCODE, x.DIVISIONID }).ToList();
            foreach (var item in emplist)
            {
                DivisionWise_Risk_Count qwerty = new DivisionWise_Risk_Count();
                var assets = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == item.ISSCEMPCODE && x.SKYID == SYKI && x.ACTIVE == 1 & x.STATUS == 3 && x.ISSC_MEMBER_STATUS == 1 && x.OPERATING_STATUS == 1 && x.DIVISION_STATUS == 1).Select(x => new
                {
                    DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ACTIVE == 1 && y.ADEMPCODE == item.ISSCEMPCODE && y.SYKI == SYKI && y.DIVISIONID == item.DIVISIONID).Select(y => y.DIVISION).FirstOrDefault(),
                    Id = x.ID,
                    Current_Degree_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),

                }).ToList();
                if (assets.Count() > 0)
                {
                    qwerty = assets.Select(x => new DivisionWise_Risk_Count { DivisionName = x.DivisionName }).FirstOrDefault();
                    qwerty.TSCount = assets.Where(x => x.Current_Degree_RA == "S").Count();
                    qwerty.TACount = assets.Where(x => x.Current_Degree_RA == "A").Count();
                    qwerty.TBCount = assets.Where(x => x.Current_Degree_RA == "B").Count();
                    qwerty.TCCount = assets.Where(x => x.Current_Degree_RA == "C").Count();
                    qwerty.TDCount = assets.Where(x => x.Current_Degree_RA == "D").Count();
                    risklist.Add(qwerty);
                }

            }
            return risklist;
        }


        public AssetRegister_Start_EndDate_ISSC_Member GetStartDate_EndDate_ForAsset_Register(CommonRiskAssessmentVM asset_details)
        {
            AssetRegister_Start_EndDate_ISSC_Member Emp_Details = new AssetRegister_Start_EndDate_ISSC_Member();
            var divisionid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == asset_details.SKYID && x.ADEMPCODE == asset_details.ISSC_MEM_EmpCode).Select(x => x.DIVISIONID).FirstOrDefault();
            Emp_Details = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.SKIID == asset_details.SKYID && x.DIVISIONID == divisionid).Select(x => new AssetRegister_Start_EndDate_ISSC_Member { StartDate = x.STARTDATE, EndDate = x.ENDDATE }).FirstOrDefault();

            return Emp_Details;
        }

        public List<Primary_RiskDeatils_VM> Get_Primary_Asset(long Userid, long? SYKI)
        {
            var s = _arDBContext.RISK_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == Userid && x.SKIID == SYKI && x.ACTIVE == 1).Distinct().Select(x => new Primary_RiskDeatils_VM { Id = x.PRIMARY, PrimaryAsset = x.PRIMARY }).Distinct().OrderBy(x => x.PrimaryAsset).ToList();
            return s;
        }



        public int ISSC_Member_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var check = _arDBContext.RISKASSESSMENT.Where(x => x.SKYID == SYKI && x.RISK_CREATEDBY == Empcode && (x.STATUS == 1 || x.STATUS == 4 || x.STATUS == 5 || x.STATUS == 6) && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1).Count();
            return check;
        }

        public int ISSC_Member_RiskDetails_EndDate_Check(int Empcode, long? SYKI)
        {
            int check = 0;
            var divisionid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADEMPCODE == Empcode).Select(x => x.DIVISIONID).FirstOrDefault();
            var enddate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.SKIID == SYKI && x.DIVISIONID == divisionid).Select(x => x.ENDDATE).FirstOrDefault();
            if (DateTime.Now.Date > Convert.ToDateTime(enddate))
            {
                check = 1;
            }
            return check;
        }

        public string InsertORUpdateRiskRegister(RiskAssessmentVM riskAssessmentVM)
        {
            if (riskAssessmentVM.Id == null || riskAssessmentVM.Id == 0)
            {
                //var riskDetails=_arDBContext.RISKASSESSMENT.Where(x=>x.SKYID== riskAssessmentVM.SkyId && x.PRIMARY_ASSETID== riskAssessmentVM.PrimaryAssetId && x.RISK_CREATEDBY== riskAssessmentVM.CreatedBy).FirstOrDefault();
                //if(riskDetails==null)
                //{
                RISKASSESSMENT riskReg = new RISKASSESSMENT();
                int count = _arDBContext.RISKASSESSMENT.AsEnumerable().OrderByDescending(x => x.ID).Count();
                riskReg.ID = count + 1;
                riskReg.SKYID = riskAssessmentVM.SkyId;
                riskReg.PRIMARY_ASSETID = riskAssessmentVM.PrimaryAssetId;
                riskReg.RISK_CATEGORYID = riskAssessmentVM.Risk_CategoryId;
                if (riskAssessmentVM.OtherPotential != null)
                {
                    riskReg.OTHERPOTENTIAL = riskAssessmentVM.OtherPotential;
                }
                else
                {
                    riskReg.POTENTIAL_RISKID = riskAssessmentVM.Potential_RiskId;
                }
                riskReg.RISK_LEADID = riskAssessmentVM.Risk_LeadId;
                riskReg.FREQUENCYID_RISKIDENTIFICATION = riskAssessmentVM.Ident_Frequency;
                riskReg.IMAPCTID_RISKIDENTIFICATION = riskAssessmentVM.Ident_Imapct;
                riskReg.DEG_LEVELID_RISKIDENTIFICATION = riskAssessmentVM.Ident_Current_Degree_Risk_Lebel;
                riskReg.IMPACTTYPEID = riskAssessmentVM.RiskImactTypeId;
                riskReg.CM_TOPRE_POTENTIAL_RISK = riskAssessmentVM.Current_Measures_To_Prevent_Potential_Risk;
                riskReg.RISK_OWNER = riskAssessmentVM.Risk_Owner;
                riskReg.CURRENT_MEASURES_LEVELID = riskAssessmentVM.Current_Measures_Level;
                riskReg.FREQUENCYID = riskAssessmentVM.Frequency;
                riskReg.IMAPCTID = riskAssessmentVM.Imapct;
                riskReg.CURRENT_DEGREE_RISK_LEBELID = riskAssessmentVM.Current_Degree_Risk_Lebel;
                if (riskAssessmentVM.Risk_Treatment_for_Risk_Level == 10 || riskAssessmentVM.Risk_Treatment_for_Risk_Level == 11)
                {
                    riskReg.TREATMENT_FOR_RISK_LEVELID = riskAssessmentVM.Risk_Treatment_for_Risk_Level;
                    riskReg.RISK_STATUS = 0;//Risk Accept
                }
                else
                {
                    riskReg.TREATMENT_FOR_RISK_LEVELID = riskAssessmentVM.Risk_Treatment_for_Risk_Level;
                    riskReg.RESPONSBILITY = riskAssessmentVM.Responsbility;
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime dateTime14;
                    dateTime14 = DateTime.ParseExact(riskAssessmentVM.Target_Date, "dd/MM/yyyy", provider);
                    riskReg.TARGET_DATE = dateTime14;
                    riskReg.RISK_STATUS = riskAssessmentVM.Risk_Status;
                    riskReg.ACTION_ITEM = riskAssessmentVM.Action_Item;
                    riskReg.FREQUENCYID_RRA = riskAssessmentVM.Frequency_RRA;
                    riskReg.IMAPCTID_RRA = riskAssessmentVM.Imapct_RRA;
                    riskReg.RISK_DEGREE_LEVELID_RRA = riskAssessmentVM.Risk_Degree_Level_RRA;
                }

                riskReg.ACTIVE = 1;
                riskReg.RISK_CREATEDBY = riskAssessmentVM.CreatedBy;
                riskReg.RISK_CREATEDDATE = DateTime.Now;
                _arDBContext.Entry(riskReg).State = EntityState.Added;
                _arDBContext.SaveChanges();
                return "Risk Assessment Save Successfully.";
                //}
                //else
                //{
                // return "You have already identified risk against this primary asset, if you want to add more risk please select again same primary Asset.";
                // }

            }
            else
            {
                var riskDet = _arDBContext.RISKASSESSMENT.Where(x => x.ID == riskAssessmentVM.Id).FirstOrDefault();
                riskDet.SKYID = riskAssessmentVM.SkyId;
                riskDet.PRIMARY_ASSETID = riskAssessmentVM.PrimaryAssetId;
                riskDet.RISK_CATEGORYID = riskAssessmentVM.Risk_CategoryId;
                if (riskAssessmentVM.OtherPotential != null)
                {
                    riskDet.OTHERPOTENTIAL = riskAssessmentVM.OtherPotential;
                }
                else
                {
                    riskDet.POTENTIAL_RISKID = riskAssessmentVM.Potential_RiskId;
                }
                riskDet.RISK_LEADID = riskAssessmentVM.Risk_LeadId;
                riskDet.FREQUENCYID_RISKIDENTIFICATION = riskAssessmentVM.Ident_Frequency;
                riskDet.IMAPCTID_RISKIDENTIFICATION = riskAssessmentVM.Ident_Imapct;
                riskDet.DEG_LEVELID_RISKIDENTIFICATION = riskAssessmentVM.Ident_Current_Degree_Risk_Lebel;
                riskDet.IMPACTTYPEID = riskAssessmentVM.RiskImactTypeId;
                riskDet.CM_TOPRE_POTENTIAL_RISK = riskAssessmentVM.Current_Measures_To_Prevent_Potential_Risk;
                riskDet.RISK_OWNER = riskAssessmentVM.Risk_Owner;
                riskDet.CURRENT_MEASURES_LEVELID = riskAssessmentVM.Current_Measures_Level;
                riskDet.FREQUENCYID = riskAssessmentVM.Frequency;
                riskDet.IMAPCTID = riskAssessmentVM.Imapct;
                riskDet.CURRENT_DEGREE_RISK_LEBELID = riskAssessmentVM.Current_Degree_Risk_Lebel;
                if (riskAssessmentVM.Risk_Treatment_for_Risk_Level == 10 || riskAssessmentVM.Risk_Treatment_for_Risk_Level == 11)
                {
                    riskDet.TREATMENT_FOR_RISK_LEVELID = riskAssessmentVM.Risk_Treatment_for_Risk_Level;
                    riskDet.RISK_STATUS = 0;//Risk Accept
                }
                else
                {
                    riskDet.TREATMENT_FOR_RISK_LEVELID = riskAssessmentVM.Risk_Treatment_for_Risk_Level;
                    riskDet.RESPONSBILITY = riskAssessmentVM.Responsbility;
                    //CultureInfo provider = CultureInfo.InvariantCulture;
                    //DateTime dateTime14;
                    //dateTime14 = DateTime.ParseExact(riskAssessmentVM.Target_Date, "dd/MM/yyyy", provider);
                    riskDet.TARGET_DATE = Convert.ToDateTime(riskAssessmentVM.Target_Date);
                    riskDet.RISK_STATUS = riskAssessmentVM.Risk_Status;
                    riskDet.ACTION_ITEM = riskAssessmentVM.Action_Item;
                    riskDet.FREQUENCYID_RRA = riskAssessmentVM.Frequency_RRA;
                    riskDet.IMAPCTID_RRA = riskAssessmentVM.Imapct_RRA;
                    riskDet.RISK_DEGREE_LEVELID_RRA = riskAssessmentVM.Risk_Degree_Level_RRA;
                }

                riskDet.ACTIVE = 1;
                riskDet.RISK_CREATEDBY = riskAssessmentVM.CreatedBy;
                riskDet.RISK_CREATEDDATE = DateTime.Now;
                _arDBContext.Entry(riskDet).State = EntityState.Modified;
                _arDBContext.SaveChanges();

                return "Risk Assessment Updated Successfully.";
            }

        }
        public List<CommonRiskAssessmentVM> Get_Last_Ki_Asset_Details(long? Userid, long? SYKI)
        {
            // var lastki = SYKI - 1;
            var get_prevyear_detail = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == SYKI && x.OPERATIONID == get_prevyear_detail.OPERATIONID && x.DIVISIONID == get_prevyear_detail.DIVISIONID).Select(x => x.ISSCEMPCODE).FirstOrDefault();
            var assets = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == get_prevyear_userid && x.SKYID == SYKI && x.ACTIVE == 1 && x.OPERATING_STATUS == 1).Select(x => new CommonRiskAssessmentVM
            {
                ID = x.ID,
                PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKI && x1.ID == x.PRIMARY_ASSETID).Select(x1 => x1.PRIMARY).FirstOrDefault(),

                //RiskCategory = x.RISK_CATEGORY_MASTER != null ? x.RISK_CATEGORY_MASTER.RISK_CATEGORIES : "",
                RiskCategory = _arDBContext.RISK_CATEGORY_MASTER
                .Where(a => a.RISK_ID == x.RISK_CATEGORYID)
                .Select(a => a.RISK_CATEGORIES)
                .FirstOrDefault(),

                //PotentialRisk = x.RISK_STATEMENT_MASTER != null ? x.RISK_STATEMENT_MASTER.RISK_STATEMENT : "",
                PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
                .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
                .Select(a => a.RISK_STATEMENT)
                .FirstOrDefault(),

                RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH.Where(x1 => x1.RISKLEADID == x.RISK_LEADID).Select(x1 => x1.RISKLEADTYPE).FirstOrDefault(),
                RiskOwner = x.RISK_OWNER,


                //Iden_Frequency = x.RISK_FREQUENCY_MASTER != null ? x.RISK_FREQUENCY_MASTER.DEGREE_OF_FREQUENCY : "",
                Iden_Frequency = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),

                Impact_Type = _arDBContext.IMPACTTYPE.Where(x1 => x1.IMAPECTTYPEID == x.IMPACTTYPEID).Select(x1 => x1.IMPACTTYPE1).FirstOrDefault(),


                //Iden_Impact = x.RISK_IMPACT_MASTER != null ? x.RISK_IMPACT_MASTER.DEGREE_OF_IMPACT : "",
                Iden_Impact = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),

                //Iden_Current_Risk_Level = x.RISK_FREQUENCY_IMPACT_MAP_MAST != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST.RESULT : "",
                Iden_Current_Risk_Level = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
                .Select(a => a.RESULT)
                .FirstOrDefault(),

                CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,
                Current_Measures_Level = _arDBContext.RISK_CURRENTMEASURE_LVL.Where(x1 => x1.CURRENTMEASURELEVELID == x.CURRENT_MEASURES_LEVELID).Select(x1 => x1.CUR_LEVEL).FirstOrDefault(),
                Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                Impact_RA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.FREQUENCY == x.FREQUENCYID && x1.IMPACT == x.IMAPCTID).Select(x1 => x1.RESULT).FirstOrDefault(),
                Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL.Where(x1 => x1.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID).Select(x1 => x1.TREATMENT).FirstOrDefault(),
                Responsebiity = x.RESPONSBILITY,
                TargetDate = x.TARGET_DATE,
                ActionItem = x.ACTION_ITEM,
                RiskStatus = (int)x.RISK_STATUS,
                Status = (int)x.STATUS,
                ApproveStatus = (int)x.APRROVE_STATUS,
                Division_Head_EmpCode = (long)x.DIVISION_HEAD_ID,
                Operating_Head_EmpCode = (long)x.OPERATING_HEAD_ID,
                Div_Head_Remark = x.REVIEW_REMARK_DIVISIONHEAD,
                OP_Head_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                Div_Head_Status = (int)x.DIVISION_STATUS,
                ISSC_MEM_Status = (int)x.ISSC_MEMBER_STATUS,
                OP_Head_Status = (int)x.OPERATING_STATUS,
                Frequency_RRA = _arDBContext.RISK_FREQUENCY_MASTER.Where(x1 => x1.FREQUENCY_LEVEL == x.FREQUENCYID_RRA).Select(x1 => x1.DEGREE_OF_FREQUENCY).FirstOrDefault(),
                Impact_RRA = _arDBContext.RISK_IMPACT_MASTER.Where(x1 => x1.RISK_LEVEL == x.IMAPCTID_RRA).Select(x1 => x1.DEGREE_OF_IMPACT).FirstOrDefault(),
                Current_Risk_Level_RRA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST.Where(x1 => x1.IMPACT == x.IMAPCTID_RRA && x1.FREQUENCY == x.FREQUENCYID_RRA).Select(x1 => x1.RESULT).FirstOrDefault(),
                CreatedBy = (long)x.RISK_CREATEDBY,
                Other_Potential_Risk = x.OTHERPOTENTIAL
            }).ToList();
            return assets;
        }

        public List<Risk_Approver_Remarks_ISSCM> GetRemrksForISSCMember(long SYKI, int Empcode)
        {
            var result = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Empcode && x.SKYID == SYKI && x.ACTIVE == 1).Select(x => new Risk_Approver_Remarks_ISSCM
            {
                Divison_Remarks = x.REVIEW_REMARK_DIVISIONHEAD,
                DivisionDate = x.SUBMIT_DATE_DIVISIONHEAD,
                Operating_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                OperatingDate = x.SUBMIT_DATE_OPERATINGHEAD,
                ModifiedDate = x.RISK_UPDATEDDATE
            }).ToList();
            return result;
        }


        public List<Risk_Deficency_Remarks> GetDeficencyRemrks(long SYKI, int Empcode)
        {
            var result1 = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ACTIVE == 1 && y.ADEMPCODE == Empcode && y.SYKI == SYKI).Select(y => new { y.OPERATIONID, y.DIVISIONID }).FirstOrDefault();
            var result = _arDBContext.RISK_ASSESSMENT_DEFICIENCY.AsEnumerable().Where(x => x.ISSCEMPCODE == Empcode && x.OPERATIONID == result1.OPERATIONID && x.DIVISIONID == result1.DIVISIONID)
                .Select(x => new Risk_Deficency_Remarks
                {
                    PrimaryAsset = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKI && x1.ID == x.PRIMARYASSETID).Select(x1 => x1.PRIMARY).FirstOrDefault(),
                    RiskCategory = _arDBContext.RISK_CATEGORY_MASTER.AsEnumerable().Where(p => p.RISK_ID == x.RISKCATEGORYID && p.STATUS == 1).Select(p => p.RISK_CATEGORIES).FirstOrDefault(),
                    Remarks = x.FEEDBACK,
                    cdate = x.CREATIONDATE
                }).ToList();
            return result;
        }

        public int Check_OrganizationMapping(long SYKI, int Empcode)
        {
            var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Empcode && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            int get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.CURRENT_DIVISIONID == curent_year_deatil.DIVISIONID && x.STATUS == 1).Count();
            return get_prevyear_detail;
        }


        public RiskAssessmentVM Get_Edit_Risk_Details(int? Id)
        {
            RiskAssessmentVM riskAssessmentVM = new RiskAssessmentVM();
            var rikDet = _arDBContext.RISKASSESSMENT.Where(x => x.ID == Id).FirstOrDefault();
            if (rikDet != null)
            {
                riskAssessmentVM.Id = rikDet.ID;
                riskAssessmentVM.SkyId = (decimal)rikDet.SKYID;
                riskAssessmentVM.PrimaryAssetId = (int)rikDet.PRIMARY_ASSETID;
                riskAssessmentVM.Risk_CategoryId = (int)rikDet.RISK_CATEGORYID;
                riskAssessmentVM.Risk_LeadId = (int)rikDet.RISK_LEADID;
                if (rikDet.POTENTIAL_RISKID != null)
                {
                    riskAssessmentVM.Potential_RiskId = (int)rikDet.POTENTIAL_RISKID;
                }
                else
                {
                    riskAssessmentVM.Potential_RiskId = null;
                }

                riskAssessmentVM.OtherPotential = rikDet.OTHERPOTENTIAL;
                riskAssessmentVM.Risk_Owner = rikDet.RISK_OWNER;
                riskAssessmentVM.Ident_Frequency = (int)rikDet.FREQUENCYID_RISKIDENTIFICATION;
                riskAssessmentVM.RiskImactTypeId = (int)rikDet.IMPACTTYPEID;
                riskAssessmentVM.Ident_Imapct = (int)rikDet.IMAPCTID_RISKIDENTIFICATION;
                riskAssessmentVM.Ident_Current_Degree_Risk_Lebel = (int)rikDet.DEG_LEVELID_RISKIDENTIFICATION;
                riskAssessmentVM.Current_Measures_To_Prevent_Potential_Risk = rikDet.CM_TOPRE_POTENTIAL_RISK;
                riskAssessmentVM.Current_Measures_Level = (int)rikDet.CURRENT_MEASURES_LEVELID;
                riskAssessmentVM.Frequency = (int)rikDet.FREQUENCYID;
                riskAssessmentVM.Imapct = (int)rikDet.IMAPCTID;
                riskAssessmentVM.Current_Degree_Risk_Lebel = (int)rikDet.CURRENT_DEGREE_RISK_LEBELID;
                riskAssessmentVM.Action_Item = rikDet.ACTION_ITEM;
                riskAssessmentVM.Risk_Treatment_for_Risk_Level = (int)rikDet.TREATMENT_FOR_RISK_LEVELID;
                riskAssessmentVM.Responsbility = rikDet.RESPONSBILITY;
                if (rikDet.TARGET_DATE != null)
                {
                    string newdate = Convert.ToString(rikDet.TARGET_DATE);
                    riskAssessmentVM.Target_Date = Convert.ToDateTime(newdate).ToString("dd/MM/yyyy");
                }
                else
                {
                    riskAssessmentVM.Target_Date = "";
                }
                riskAssessmentVM.Risk_Status = (int)rikDet.RISK_STATUS;
                if (rikDet.STATUS != null)
                {
                    riskAssessmentVM.Status = (int)rikDet.STATUS;
                }
                else
                {
                    riskAssessmentVM.Status = null;
                }

                riskAssessmentVM.Imapct_RRA = rikDet.IMAPCTID_RRA != null ? (int)rikDet.IMAPCTID_RRA : 0;
                riskAssessmentVM.Frequency_RRA = rikDet.FREQUENCYID_RRA != null ? (int)rikDet.FREQUENCYID_RRA : 0;
                riskAssessmentVM.Risk_Degree_Level_RRA = rikDet.RISK_DEGREE_LEVELID_RRA != null ? (int)rikDet.RISK_DEGREE_LEVELID_RRA : 0;
                return riskAssessmentVM;
            }
            else
            {
                return riskAssessmentVM;
            }
        }
        public string Delete_RiskDetails(int riskId, int userId)
        {
            if (riskId > 0 && userId > 0)
            {
                var riskDet = _arDBContext.RISKASSESSMENT.Where(x => x.ID == riskId).FirstOrDefault();
                if (riskDet != null)
                {
                    riskDet.RISK_UPDATEDBY = userId;
                    riskDet.RISK_UPDATEDDATE = DateTime.Now;
                    riskDet.ACTIVE = 0;
                    _arDBContext.Entry(riskDet).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                    return "Risk Deleted Successfully.";
                }
                return "There is problem something please try again.";
            }
            return "There is problem something please try again.";
        }

        public string SaveAsDraftkRegister(int skyid, int userId)
        {
            if (skyid > 0 && userId > 0)
            {
                var riskDet = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == userId && x.SKYID == skyid).ToList();
                if (riskDet.Count > 0)
                {
                    foreach (var item in riskDet)
                    {
                        item.RISK_UPDATEDBY = userId;
                        item.RISK_UPDATEDDATE = DateTime.Now;
                        item.STATUS = 0;
                        _arDBContext.Entry(item).State = EntityState.Modified;
                        _arDBContext.SaveChanges();
                    }
                    return "Risk Save as Draft Successfully.";
                }
                else
                {
                    return "Already Risk Save as Draft.";
                }
            }
            return "There is problem something please try again.";
        }

        public string CheckAllPrimaryRiskCreatedOrNotByUserId(List<int> pIds, int skyid, int userId)
        {
            if (skyid > 0 && userId > 0 && pIds.Count > 0)
            {
                foreach (var item in pIds)
                {
                    var riskAssess = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == userId && x.SKYID == skyid && x.PRIMARY_ASSETID == item && x.ACTIVE == 1).FirstOrDefault();
                    if (riskAssess == null)
                    {
                        return "Please fill risk Assessment against all primary assets.";
                    }
                }
            }
            else
            {
                return "There is problem something please try again.";
            }
            return "0";
        }

        public int CheckLast_year_RiskData(long Userid, long? SYKI)
        {
            if (Userid > 0 && SYKI > 0)
            {
                var common_asset_exist_for_ki = _arDBContext.RISK_REGISTER_COMMONEXIST.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKI && x.USERID == Userid && x.ACTIONTYPE == 2).Count();
                if (common_asset_exist_for_ki == 0)
                {
                    var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
                    if (curent_year_deatil != null)
                    {
                        var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.CURRENT_DIVISIONID == curent_year_deatil.DIVISIONID && x.STATUS == 1).Select(x => x).FirstOrDefault();
                        if (get_prevyear_detail != null)
                        {
                            var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == get_prevyear_detail.PREVIOUS_SYKIID && x.OPERATIONID == get_prevyear_detail.PREVIOUS_OPERATIONID && x.DIVISIONID == get_prevyear_detail.PREVIOUS_DIVISIONID).Select(x => x.ISSCEMPCODE).FirstOrDefault();
                            var risk_details = _arDBContext.RISKASSESSMENT.Where(x => x.ACTIVE == 1 && x.RISK_CREATEDBY == get_prevyear_userid && x.OPERATING_STATUS == 1 && x.SKYID == (SYKI - 1)).Select(x => x).ToList().Count();
                            if (risk_details > 0)
                            {
                                return 1;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
            return 0;
        }

        public int Last_year_Asset_Detail_Only_Non_CommonAsset(long Userid, long? SYKI)
        {
            if (Userid > 0 && SYKI > 0)
            {
                var common_asset_exist_for_ki = _arDBContext.RISK_REGISTER_COMMONEXIST.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKI && x.USERID == Userid && x.ACTIONTYPE == 2).Count();
                if (common_asset_exist_for_ki == 0)
                {
                    var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
                    if (curent_year_deatil != null)
                    {
                        var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.CURRENT_DIVISIONID == curent_year_deatil.DIVISIONID && x.STATUS == 1).Select(x => x).FirstOrDefault();
                        if (get_prevyear_detail != null)
                        {
                            var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == get_prevyear_detail.PREVIOUS_SYKIID && x.OPERATIONID == get_prevyear_detail.PREVIOUS_OPERATIONID && x.DIVISIONID == get_prevyear_detail.PREVIOUS_DIVISIONID).Select(x => x.ISSCEMPCODE).FirstOrDefault();
                            var risk_details = _arDBContext.RISKASSESSMENT.Where(x => x.ACTIVE == 1 && x.RISK_CREATEDBY == get_prevyear_userid && x.OPERATING_STATUS == 1 && x.SKYID == (SYKI - 1)).Select(x => x).ToList();

                            using (var transaction = _arDBContext.Database.BeginTransaction())
                            {
                                try
                                {
                                    List<RISKASSESSMENT> riskAssessmentList = new List<RISKASSESSMENT>();
                                    var lastgerneratedid = _arDBContext.RISKASSESSMENT.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
                                    foreach (var item in risk_details)
                                    {
                                        lastgerneratedid++;
                                        RISKASSESSMENT risk_det = new RISKASSESSMENT();
                                        risk_det.ID = lastgerneratedid;
                                        risk_det.SKYID = (decimal)SYKI;
                                        risk_det.PRIMARY_ASSETID = item.PRIMARY_ASSETID;
                                        risk_det.RISK_CATEGORYID = item.RISK_CATEGORYID;
                                        risk_det.POTENTIAL_RISKID = item.POTENTIAL_RISKID;
                                        risk_det.RISK_LEADID = item.RISK_LEADID;
                                        risk_det.FREQUENCYID_RISKIDENTIFICATION = item.FREQUENCYID_RISKIDENTIFICATION;
                                        risk_det.IMAPCTID_RISKIDENTIFICATION = item.IMAPCTID_RISKIDENTIFICATION;
                                        risk_det.DEG_LEVELID_RISKIDENTIFICATION = item.DEG_LEVELID_RISKIDENTIFICATION;
                                        risk_det.RISK_OWNER = item.RISK_OWNER;
                                        risk_det.CM_TOPRE_POTENTIAL_RISK = item.CM_TOPRE_POTENTIAL_RISK;
                                        risk_det.CURRENT_MEASURES_LEVELID = item.CURRENT_MEASURES_LEVELID; ;
                                        risk_det.FREQUENCYID = item.FREQUENCYID;
                                        risk_det.IMAPCTID = item.IMAPCTID;
                                        risk_det.CURRENT_DEGREE_RISK_LEBELID = item.CURRENT_DEGREE_RISK_LEBELID; ;
                                        risk_det.TREATMENT_FOR_RISK_LEVELID = item.TREATMENT_FOR_RISK_LEVELID;

                                        risk_det.ACTION_ITEM = item.ACTION_ITEM;
                                        risk_det.RESPONSBILITY = item.RESPONSBILITY;
                                        risk_det.TARGET_DATE = item.TARGET_DATE;
                                        risk_det.STATUS = 0;
                                        risk_det.RISK_STATUS = item.RISK_STATUS;
                                        risk_det.FREQUENCYID_RRA = item.FREQUENCYID_RRA;
                                        risk_det.IMAPCTID_RRA = item.IMAPCTID_RRA;
                                        risk_det.RISK_DEGREE_LEVELID_RRA = item.RISK_DEGREE_LEVELID_RRA;
                                        risk_det.ACTIVE = 1;

                                        risk_det.ISSC_MEMBER_STATUS = item.ISSC_MEMBER_STATUS;
                                        risk_det.DIVISION_HEAD_ID = item.DIVISION_HEAD_ID;
                                        risk_det.DIVISION_STATUS = item.DIVISION_STATUS;
                                        risk_det.OPERATING_HEAD_ID = item.OPERATING_HEAD_ID;
                                        risk_det.OPERATING_STATUS = item.FREQUENCYID_RISKIDENTIFICATION;
                                        risk_det.SUBMIT_DATE_ISSCMEMBER = item.SUBMIT_DATE_ISSCMEMBER;
                                        risk_det.SUBMIT_DATE_DIVISIONHEAD = item.SUBMIT_DATE_DIVISIONHEAD;
                                        risk_det.SUBMIT_DATE_OPERATINGHEAD = item.SUBMIT_DATE_OPERATINGHEAD;
                                        risk_det.REVIEW_REMARK_DIVISIONHEAD = item.REVIEW_REMARK_DIVISIONHEAD;

                                        risk_det.REVIEW_REMARK_OPERATINGHEAD = item.REVIEW_REMARK_OPERATINGHEAD;
                                        risk_det.RISK_CREATEDBY = item.RISK_CREATEDBY;
                                        risk_det.RISK_CREATEDDATE = item.RISK_CREATEDDATE; ;
                                        risk_det.IMPACTTYPEID = item.IMPACTTYPEID; ;
                                        risk_det.OTHERPOTENTIAL = item.OTHERPOTENTIAL; ;
                                        riskAssessmentList.Add(risk_det);

                                    }

                                    if (riskAssessmentList.Any())
                                    {
                                        _arDBContext.RISKASSESSMENT.AddRange(riskAssessmentList);

                                        ASSET_REGISTER_COMMONEXIST commonasset_entry = new ASSET_REGISTER_COMMONEXIST();
                                        commonasset_entry.SYKIID = (decimal)SYKI;
                                        commonasset_entry.USERID = Userid;
                                        commonasset_entry.ACTIVE = 1;
                                        commonasset_entry.CREATIONDATE = DateTime.Now;
                                        commonasset_entry.ACTIONTYPE = 2;
                                        _arDBContext.Entry(commonasset_entry).State = EntityState.Added;
                                        _arDBContext.SaveChanges();
                                        transaction.Commit();

                                        return 1;// Data imported successfully.
                                    }
                                    else
                                        return 4; // Data can not imported contact to ISMS Team.
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return 2;
                                }
                            }
                        }
                        return 2;
                    }
                    return 2;
                }
                return 3;//Data already imported
            }
            return 2;//There is some problem, please try again later.
        }


        public int BulkUpdateAssetApplicability(AssetApplicabilityUpdateVM bulkUpdateAssetApplicability)
        {
            if (bulkUpdateAssetApplicability.SelectedIds.Count() > 0)
            {
                _arDBContext.RISK_REGISTER_ASSETDETAILS
                        .Where(x => bulkUpdateAssetApplicability.SelectedIds.Contains(x.ID))
                        .ToList()
                        .ForEach(a =>
                        {
                            a.ISSC_MEMBER_REASON = bulkUpdateAssetApplicability.Remarks;
                            a.RETENTION_REMARK = string.Empty;
                            a.UPDATEDBY = bulkUpdateAssetApplicability.CreatedBy;
                            a.UPDATIONDATE = DateTime.Now;
                        }
                                );
                _arDBContext.SaveChanges();
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public DivisionHead_Details Get_DivisionHead_(long? SYKI, long Empcode)
        {
            var empNamelist_old = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADEMPCODE == Empcode).Select(x => new { x.OPERATION, x.DIVISIONID, x.DIVISION }).FirstOrDefault();

            var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 1 && x.ADORGLEVELID == empNamelist_old.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();

            var ISSC_Member = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Empcode).Select(x => new { ISSCName = x.FIRSTNAME + " " + x.LASTNAME, ISSCEmail = x.EMAILID }).FirstOrDefault();
            var DivisionHead_Details = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empNamelist).Select(x => new DivisionHead_Details
            {
                Empname = x.FIRSTNAME + " " + x.LASTNAME,
                Emailid = x.EMAILID,
                DivisionName = empNamelist_old.DIVISION,
                DivisionEmpCode = x.ADEMPCODE,
                ISSC_Member_Name = ISSC_Member.ISSCName,
                ISSC_MemberEmail = ISSC_Member.ISSCEmail
            }).FirstOrDefault();
            return DivisionHead_Details;
        }



        public string FinalSubmit_RiskDetails(CommonRiskAssessmentVM assetdetails)
        {
            if (assetdetails != null && assetdetails.CreatedBy > 0 && assetdetails.SKYID > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                var ass_det_List = _arDBContext.RISKASSESSMENT.Where(x => x.SKYID == assetdetails.SKYID && x.RISK_CREATEDBY == assetdetails.CreatedBy).ToList();
                ass_det_List.ForEach(common_data =>
                {
                    common_data.RISK_UPDATEDBY = assetdetails.CreatedBy;
                    common_data.RISK_UPDATEDDATE = DateTime.Now;
                    common_data.STATUS = 1; // final submit
                    common_data.ISSC_MEMBER_STATUS = 1;
                    common_data.SUBMIT_DATE_ISSCMEMBER = DateTime.Now;
                }
                );
                _arDBContext.SaveChanges();
                return "Submitted Sucessfully.";
                //}
            }
            return "Something went worng please try again.";
        }

        public string RiskOldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(SameDivisionAndOprationApprovalVM model)
        {
            string message = "";
            if (model.ExistingDivHeadEmpCode == 0)
            {
                model.ExistingDivHeadEmpCode = model.NewDivHeadEmpCode;
            }
            else if (model.ExistingOPHeadEmpCode == 0)
            {
                model.ExistingOPHeadEmpCode = model.NewOPHeadEmpCode;
            }
            if (model.NewDivHeadEmpCode > 0 && model.NewOPHeadEmpCode > 0)
            {

                var matchedRecords = _arDBContext.RISK_USER_MAPPING.Where(x => x.NEWDIVISIONEMPCODE == model.ExistingDivHeadEmpCode
                && x.NEWOPERATIONHEADCODE == model.ExistingOPHeadEmpCode && x.SYKIID == model.SYKIID).FirstOrDefault();
                if (matchedRecords != null)
                {
                    matchedRecords.NEWDIVISIONEMPCODE = model.NewDivHeadEmpCode;
                    matchedRecords.NEWOPERATIONHEADCODE = model.NewOPHeadEmpCode;
                    matchedRecords.MODIFYBY = model.CreatedBy;
                    matchedRecords.MODIFIEDDATE = DateTime.Now;
                    _arDBContext.Entry(matchedRecords).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                    message = "Operating Head and Division Head Change Successfully";
                    return message;
                }
                else
                {
                    RISK_USER_MAPPING asset = new RISK_USER_MAPPING();
                    var rowNum = _arDBContext.RISK_USER_MAPPING.Count();
                    asset.RISKUSERMAPPINGID = rowNum + 1;
                    asset.SYKIID = model.SYKIID;
                    asset.DIVISIONID = model.DIVISIONID;
                    asset.OLDDIVISIONEMPCODE = model.ExistingDivHeadEmpCode;
                    asset.NEWDIVISIONEMPCODE = model.NewDivHeadEmpCode;
                    asset.OPRATIONID = model.OPERATIONID;
                    asset.OLDOPERATIONHEADCODE = model.ExistingOPHeadEmpCode;
                    asset.NEWOPERATIONHEADCODE = model.NewOPHeadEmpCode;
                    asset.CREATEDBY = model.CreatedBy;
                    asset.CREATEDDATE = DateTime.Now;
                    _arDBContext.Entry(asset).State = EntityState.Added;
                    _arDBContext.SaveChanges();
                    message = "Operating Head and Division Head Change Successfully";
                    return message;
                }

            }
            else if (model.NewDivHeadEmpCode > 0)
            {
                var matchedRecords = _arDBContext.RISK_USER_MAPPING.Where(x => x.NEWDIVISIONEMPCODE == model.ExistingDivHeadEmpCode
               && x.SYKIID == model.SYKIID).FirstOrDefault();
                if (matchedRecords != null)
                {
                    matchedRecords.NEWDIVISIONEMPCODE = model.NewDivHeadEmpCode;
                    matchedRecords.MODIFYBY = model.CreatedBy;
                    matchedRecords.MODIFIEDDATE = DateTime.Now;
                    _arDBContext.Entry(matchedRecords).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                    message = "Division Head Change Successfully";
                    return message;
                }
                else
                {
                    RISK_USER_MAPPING asset = new RISK_USER_MAPPING();
                    var rowNum = _arDBContext.RISK_USER_MAPPING.Count();
                    asset.RISKUSERMAPPINGID = rowNum + 1;
                    asset.SYKIID = model.SYKIID;
                    asset.DIVISIONID = model.DIVISIONID;
                    asset.OLDDIVISIONEMPCODE = model.ExistingDivHeadEmpCode;
                    asset.NEWDIVISIONEMPCODE = model.NewDivHeadEmpCode;
                    asset.OPRATIONID = model.OPERATIONID;
                    asset.OLDOPERATIONHEADCODE = model.ExistingOPHeadEmpCode;
                    asset.CREATEDBY = model.CreatedBy;
                    asset.CREATEDDATE = DateTime.Now;
                    _arDBContext.Entry(asset).State = EntityState.Added;
                    _arDBContext.SaveChanges();
                    message = "Division Head Change Successfully";
                    return message;
                }

            }
            else if (model.NewOPHeadEmpCode > 0)
            {

                var matchedRecords = _arDBContext.RISK_USER_MAPPING.Where(x => x.NEWOPERATIONHEADCODE == model.ExistingOPHeadEmpCode && x.SYKIID == model.SYKIID).FirstOrDefault();
                if (matchedRecords != null)
                {
                    matchedRecords.NEWOPERATIONHEADCODE = model.NewOPHeadEmpCode;
                    matchedRecords.MODIFYBY = model.CreatedBy;
                    matchedRecords.MODIFIEDDATE = DateTime.Now;
                    _arDBContext.Entry(matchedRecords).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                    message = "Operating Head Change Successfully";
                    return message;
                }
                else
                {
                    RISK_USER_MAPPING asset = new RISK_USER_MAPPING();
                    var rowNum = _arDBContext.RISK_USER_MAPPING.Count();
                    asset.RISKUSERMAPPINGID = rowNum + 1;
                    asset.SYKIID = model.SYKIID;
                    asset.OPRATIONID = model.OPERATIONID;
                    asset.OLDOPERATIONHEADCODE = model.ExistingOPHeadEmpCode;
                    asset.NEWOPERATIONHEADCODE = model.NewOPHeadEmpCode;
                    asset.CREATEDBY = model.CreatedBy;
                    asset.CREATEDDATE = DateTime.Now;
                    _arDBContext.Entry(asset).State = EntityState.Added;
                    _arDBContext.SaveChanges();
                    message = "Operating Head Change Successfully";
                    return message;
                }

            }
            else
            {
                message = "No record update";
                return message;
            }
        }

        public List<SearchParameterList> BindDivision(long? op_Id)
        {
            List<SearchParameterList> iList = new List<SearchParameterList>();
            var iColl = (from data in _arDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                         && (e.SYKI >= 21)
                         && e.DIVISIONID != null && e.DIVISIONID != 0
                         && e.OPERATIONID == (op_Id == 0 ? e.OPERATIONID : op_Id)).OrderBy(e => e.DIVISION)
                         select new
                         {
                             data.DIVISIONID,
                             data.DIVISION
                         }).Distinct().OrderBy(x => x.DIVISION).ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new SearchParameterList
                {
                    DIVISIONID = Convert.ToInt64(obj.DIVISIONID == null ? 0 : obj.DIVISIONID),
                    DIVISION = obj.DIVISION
                });
            }
            return iList;
        }

        public long? BindDivisionWithOldOperationHeadEmpCodeByOperationId(long? op_Id, long? SYKIID)
        {
            var OperatingHeadId = (from s in _arDBContext.ASSET_REGISTER_ASSETDETAILS
                                   join r in _arDBContext.ASSET_REGISTER_MEMNOMINATION on s.CREATEDBY equals r.ISSCEMPCODE
                                   where s.SKIID == SYKIID && r.SYKIID == SYKIID
                                   && r.OPERATIONID == op_Id && s.ACTIVE == 1 && s.OPERATINGHEAD_ID != null
                                   select s.OPERATINGHEAD_ID).FirstOrDefault();
            if (OperatingHeadId != null && OperatingHeadId != 0)
            {
                return Convert.ToInt16(OperatingHeadId);
            }
            else
            {
                return 0;
            }
        }

        public long? BindOldDivisionHeadEmpCodeByDivisionId(long? divId, long? SYKIID)
        {
            var DivisionHeadId = (from s in _arDBContext.ASSET_REGISTER_ASSETDETAILS
                                  join r in _arDBContext.ASSET_REGISTER_MEMNOMINATION on s.CREATEDBY equals r.ISSCEMPCODE
                                  where s.SKIID == SYKIID && r.SYKIID == SYKIID
                                  && r.DIVISIONID == divId && s.ACTIVE == 1 && s.DIVISIONHEAD_ID != null
                                  select s.DIVISIONHEAD_ID).FirstOrDefault();
            if (DivisionHeadId != null && DivisionHeadId != 0)
            {
                return Convert.ToInt16(DivisionHeadId);
            }
            else
            {
                return 0;
            }
        }

        public Get_Division_ISSC_Member Get_Division_For_ISSC_Member(int? id)
        {
            string D_Head = string.Empty; string O_Head = string.Empty;
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();
            var empNamelist_old = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => new { x.OPERATION, x.DIVISIONID, x.DIVISION }).FirstOrDefault();


            if (empNamelist_old != null)
            {
                var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 1 && x.ADORGLEVELID == empNamelist_old.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                var D_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empNamelist).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                D_Head = D_HeadName == null?"":D_HeadName.Name;

                var ohead_id = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKIList.SYKIID && x.DIVISIONID == empNamelist_old.DIVISIONID && x.ACTIVE == 1).Select(x => x.ADDEDBY).FirstOrDefault();
                var O_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ohead_id).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                O_Head = O_HeadName==null?"": O_HeadName.Name;
            }

            var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == id && x.SYKI == SYKIList.SYKIID).AsEnumerable().Select(x => new Get_Division_ISSC_Member
            {
                DivId = (long)x.DIVISIONID,
                DivisionName = x.DIVISION,
                ISSCMember = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Count() > 0 ? _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(p1 => p1.SYKIID == SYKIList.SYKIID && p1.DIVISIONID == x.DIVISIONID).Select(p1 => p1.ISSCEMPCODE).FirstOrDefault()).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() : null,
                EndDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),

                ISSC_Member_Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == id && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3 || x1.STATUS == 4 || x1.STATUS == 5 || x1.STATUS == 6)).Select(x1 => x1.SUBMIT_DATE_ISSCMEMBER).FirstOrDefault(),
                ISSC_Member_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == id && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3 || x1.STATUS == 4 || x1.STATUS == 5 || x1.STATUS == 6)).Select(x1 => x1.ISSC_MEMBER_STATUS).FirstOrDefault(),

                DivisionHead_Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == id && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3 || x1.STATUS == 4 || x1.STATUS == 5 || x1.STATUS == 6)).Select(x1 => x1.SUBMIT_DATE_DIVISIONHEAD).FirstOrDefault(),
                DivisionHead_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == id && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3 || x1.STATUS == 4 || x1.STATUS == 5 || x1.STATUS == 6)).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

                OperatingHead__Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == id && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3 || x1.STATUS == 4 || x1.STATUS == 5 || x1.STATUS == 6)).Select(x1 => x1.SUBMIT_DATE_OPERATINGHEAD).FirstOrDefault(),
                OperatingHead_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == id && x1.ACTIVE == 1 && (x1.STATUS == 1 || x1.STATUS == 2 || x1.STATUS == 3 || x1.STATUS == 4 || x1.STATUS == 5 || x1.STATUS == 6)).Select(x1 => x1.OPERATING_STATUS).FirstOrDefault(),

                DivisionHead_Name = D_Head,
                OperatingHead_Name = O_Head
            }).FirstOrDefault();
            return PeriodSettingList;
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListForDashboard()
        {
            AssetRegistrationSYKIViewModel assetRegistrationSYKIViewModel = new AssetRegistrationSYKIViewModel();

            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.SYKIID > 23 && x.SYKIID < 23 && x.ACTIVE != 1)
                            select data).OrderByDescending(x => x.SYKIID).Take(3).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    assetRegistrationSYKIViewModel._SYKIList.Add(new AssetRegistrationSYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return assetRegistrationSYKIViewModel;
        }

        public int Operating_Head_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var check = _arDBContext.RISKASSESSMENT.Where(x => x.SKYID == SYKI && x.RISK_CREATEDBY == Empcode && x.STATUS == 3 && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1 && x.OPERATING_STATUS == 1).Count();
            return check;
        }


        public List<Asset_Approver_Remarks> GetRemrks(long SYKI, int Empcode)
        {
            var result = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == Empcode && x.SKYID == SYKI && x.ACTIVE == 1).Select(x => new Asset_Approver_Remarks
            {
                Divison_Remarks = x.REVIEW_REMARK_DIVISIONHEAD,
                DivisionDate = x.SUBMIT_DATE_DIVISIONHEAD,
                Operating_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                OperatingDate = x.SUBMIT_DATE_OPERATINGHEAD

            }).ToList();
            return result;
        }
        public List<OperatingHead> GetISSCMemberList(long? SYKI, long? OPERATIONID, long? Divisionid)
        {
            List<OperatingHead> emplist = new List<OperatingHead>();
            if (OPERATIONID == 0)
            {
                var EmpNameList = _arDBContext.ASSET_REGISTER_MEMNOMINATION.AsEnumerable().Where(x => x.SYKIID == SYKI).ToList();
                List<string> elist = new List<string>();
                foreach (var item in EmpNameList)
                {
                    elist.Add(item.ISSCEMPCODE.ToString());
                }

                var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
                foreach (var item in empNamelist)
                {
                    foreach (string eitem in elist)
                    {
                        if (eitem == item.ADEMPCODE.ToString())
                        {
                            var items = new OperatingHead();
                            items.Empname = item.FIRSTNAME + " " + item.LASTNAME;
                            items.Emailid = item.EMAILID;
                            emplist.Add(items);
                            break;
                        }
                    }
                }
            }
            else
            {
                var EmpNameList = _arDBContext.ASSET_REGISTER_MEMNOMINATION.AsEnumerable().Where(x => x.SYKIID == SYKI && x.OPERATIONID == (OPERATIONID == null ? x.OPERATIONID : (long)OPERATIONID) && x.DIVISIONID == (Divisionid == null ? x.DIVISIONID : (long)Divisionid)).ToList();
                List<string> elist = new List<string>();
                foreach (var item in EmpNameList)
                {
                    elist.Add(item.ISSCEMPCODE.ToString());
                }

                var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
                foreach (var item in empNamelist)
                {
                    foreach (string eitem in elist)
                    {
                        if (eitem == item.ADEMPCODE.ToString())
                        {
                            var items = new OperatingHead();
                            items.Empname = item.FIRSTNAME + " " + item.LASTNAME;
                            items.Emailid = item.EMAILID;
                            emplist.Add(items);
                            break;
                        }
                    }
                }
            }
            return emplist;
        }

        public List<BulkUpdateEmailVM> GetISSCMemberListForBulkUpdate(string ids, long currentki)
        {
            List<BulkUpdateEmailVM> emplist = new List<BulkUpdateEmailVM>();
            //List<decimal> list = ids.Cast<decimal>().ToList();
            //var dataList = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.AsEnumerable().Where(x => x.ACTIVE == 1 && list.Contains(x.ID)).ToList();
            string[] Newids = ids.Split(',');
            foreach (var item in Newids)
            {
                decimal id = Convert.ToDecimal(item);
                var dataList = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.ACTIVE == 1 && x.ID == id).FirstOrDefault();
                if (dataList != null)
                {
                    var EmpNameList = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == currentki && x.OPERATIONID == dataList.OPERATIONID && x.DIVISIONID == dataList.DIVISIONID).FirstOrDefault();
                    var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
                    foreach (var item1 in empNamelist)
                    {
                        if (EmpNameList.ISSCEMPCODE == item1.ADEMPCODE)
                        {
                            var items = new BulkUpdateEmailVM();
                            items.Empname = item1.FIRSTNAME + " " + item1.LASTNAME;
                            items.Emailid = item1.EMAILID;
                            items.StartDate = dataList.STARTDATE;
                            items.EndDate = dataList.ENDDATE;
                            emplist.Add(items);
                            break;
                        }
                    }
                    //return emplist;
                }
                else
                {
                    return emplist;
                }
            }
            return emplist;
        }
        public RiskAssessment_Start_EndDate_ISSC_Member GetStartDate_EndDate_ForRisk_Register(CommonRiskAssessmentVM risk_details)
        {
            RiskAssessment_Start_EndDate_ISSC_Member Emp_Details = new RiskAssessment_Start_EndDate_ISSC_Member();
            var divisionid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == risk_details.SKYID && x.ADEMPCODE == risk_details.CreatedBy).Select(x => x.DIVISIONID).FirstOrDefault();
            Emp_Details = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.SKIID == risk_details.SKYID && x.DIVISIONID == divisionid).Select(x => new RiskAssessment_Start_EndDate_ISSC_Member { StartDate = x.STARTDATE, EndDate = x.ENDDATE }).FirstOrDefault();

            return Emp_Details;
        }

        public int Division_Head_RiskDetails_FinalSubmit_Check(int Userid, long? SYKI, long ISSC_Code)
        {
            var check = _arDBContext.RISKASSESSMENT.Where(x => x.SKYID == SYKI && x.DIVISION_HEAD_ID == Userid && x.RISK_CREATEDBY == ISSC_Code && x.STATUS == 2 && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1 && x.DIVISION_STATUS == 1).Count();
            return check;
        }
        public List<CommonRiskAssessmentVM> OPeratingHeads_DeatiledReport()

        {
            //  List<CommonRiskAssessmentVM> opreport = new List<CommonRiskAssessmentVM>();
            //using (var DbContext = new ePortalEntities2())
            //{
            var OP_Report = _arDBContext.RISKASSESSMENT
            .Select(x => new CommonRiskAssessmentVM
            {
                //RiskCategory = x.RISK_CATEGORY_MASTER != null ? x.RISK_CATEGORY_MASTER.RISK_CATEGORIES : "",


                CM_To_Pre_Potential_Risk = x.CM_TOPRE_POTENTIAL_RISK,
                ActionItem = x.ACTION_ITEM,
                Div_Head_Remark = x.REVIEW_REMARK_DIVISIONHEAD,
                Div_Head_Status = (int)(x.DIVISION_STATUS),
                TargetDate = x.TARGET_DATE,
                RiskStatus = (int)(x.RISK_STATUS),
                Responsebiity = x.RESPONSBILITY,


                //PrimaryRisk = x.ASSET_REGISTER_ASSETDETAILS.PRIMARY,
                PrimaryRisk = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                .Where(a => a.ID == x.PRIMARY_ASSETID)
                .Select(a => a.PRIMARY)
                .FirstOrDefault(),


                //PotentialRisk = x.RISK_STATEMENT_MASTER.RISK_STATEMENT,
                PotentialRisk = _arDBContext.RISK_STATEMENT_MASTER
                .Where(a => a.STATEMENTID == x.POTENTIAL_RISKID)
                .Select(a => a.RISK_STATEMENT)
                .FirstOrDefault(),

                Other_Potential_Risk = x.OTHERPOTENTIAL,

                //Frequency_RA = x.RISK_FREQUENCY_MASTER != null ? x.RISK_FREQUENCY_MASTER.DEGREE_OF_FREQUENCY : "",
                Frequency_RA = _arDBContext.RISK_FREQUENCY_MASTER
                .Where(a => a.FREQUENCY_LEVEL == x.FREQUENCYID)
                .Select(a => a.DEGREE_OF_FREQUENCY)
                .FirstOrDefault(),

                //Impact_RA = x.RISK_IMPACT_MASTER != null ? x.RISK_IMPACT_MASTER.DEGREE_OF_IMPACT : "",
                Impact_RA = _arDBContext.RISK_IMPACT_MASTER
                .Where(a => a.RISK_LEVEL == x.IMAPCTID)
                .Select(a => a.DEGREE_OF_IMPACT)
                .FirstOrDefault(),

                //Current_Risk_Level_RA = x.RISK_FREQUENCY_IMPACT_MAP_MAST != null ? x.RISK_FREQUENCY_IMPACT_MAP_MAST.RESULT : "",
                Current_Risk_Level_RA = _arDBContext.RISK_FREQUENCY_IMPACT_MAP_MAST
                .Where(a => a.ID == x.CURRENT_DEGREE_RISK_LEBELID)
                .Select(a => a.RESULT)
                .FirstOrDefault(),

                //Treatement_For_Risk_Level = x.RISK_TREATMENT_RISK_LVL.TREATMENT,
                Treatement_For_Risk_Level = _arDBContext.RISK_TREATMENT_RISK_LVL
                .Where(a => a.TREATMENTID == x.TREATMENT_FOR_RISK_LEVELID)
                .Select(a => a.TREATMENT)
                .FirstOrDefault(),

                //RiskLeadToBreach = x.RISK_LEADTO_BREACH.RISKLEADTYPE,
                RiskLeadToBreach = _arDBContext.RISK_LEADTO_BREACH
                .Where(a => a.RISKLEADID == x.RISK_LEADID)
                .Select(a => a.RISKLEADTYPE)
                .FirstOrDefault(),

                ID = x.ID,
                SKYID = (long)(x.SKYID),
                RiskOwner = x.RISK_OWNER,
                Division_Head_EmpCode = (long)(x.DIVISION_HEAD_ID),
                Operating_Head_EmpCode = (long)(x.OPERATING_HEAD_ID),
                OP_Head_Remarks = x.REVIEW_REMARK_OPERATINGHEAD,
                OP_Head_Status = (int)(x.OPERATING_STATUS),

                ApproveStatus = (int)x.APRROVE_STATUS,
                CreatedBy = (long)x.RISK_CREATEDBY,


            }).ToList();
            return OP_Report;

            //}

        }


        public int? GetRiskAssessmentUserDeailsForOPHeadByLoginUserID(long Userid, long Syki)
        {
            var UserDetails = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.OPERATIONID == Userid && x.SKIID == Syki).FirstOrDefault();
            if (UserDetails != null)
            {
                return Convert.ToInt16(UserDetails.OPERATIONID);
            }
            else
            {
                return null;
            }
        }

        public int Operating_Head_RiskDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var check = _arDBContext.RISKASSESSMENT.Where(x => x.SKYID == SYKI && x.RISK_CREATEDBY == Empcode && x.STATUS == 2 && x.ACTIVE == 1 && x.ISSC_MEMBER_STATUS == 1 && x.OPERATING_STATUS == 1).Count();
            return check;
        }


        public int? GetRiskUserDeailsForOPHeadByLoginUserID(long Userid, long Syki)
        {
            //var UserDetails = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.OPERATIONID == Userid && x.SKIID == Syki).FirstOrDefault();
            var UserDetails = _arDBContext.RISK_USER_MAPPING.Where(x => x.NEWOPERATIONHEADCODE == Userid && x.SYKIID == Syki).FirstOrDefault();
            if (UserDetails != null)
            {
                return Convert.ToInt16(UserDetails.OLDOPERATIONHEADCODE);
            }
            else
            {
                return null;
            }
        }

        public List<Get_RiskDivision_ISSC_Member> Get_ISSC_Members_Risk_Details_For_OpearingHead(int? id)
        {
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();

            var operationids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.OPERATIONID).ToList(); ;
            var ISSC_Member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIList.SYKIID && operationids.Contains((long)x.OPERATIONID)).Select(x => x.ISSCEMPCODE).ToList();

            List<Get_RiskDivision_ISSC_Member> ISSCMemberList = new List<Get_RiskDivision_ISSC_Member>();
            foreach (var item in ISSC_Member)
            {
                var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item && x.SYKI == SYKIList.SYKIID).Select(x => new Get_RiskDivision_ISSC_Member
                {
                    DivId = (long)x.DIVISIONID,
                    DivisionName = x.DIVISION,
                    ISSCMember_EmpCode = x.ADEMPCODE,
                    OperationName = x.OPERATION,
                    ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                    EndDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),

                    ISSC_Member_Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && (x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.SUBMIT_DATE_ISSCMEMBER).FirstOrDefault(),
                    ISSC_Member_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && (x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.ISSC_MEMBER_STATUS).FirstOrDefault(),

                    DivisionHead_Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && (x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.SUBMIT_DATE_DIVISIONHEAD).FirstOrDefault(),
                    DivisionHead_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && (x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

                    OperatingHead__Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && (x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.SUBMIT_DATE_OPERATINGHEAD).FirstOrDefault(),
                    OperatingHead_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && (x1.STATUS == 2 || x1.STATUS == 3)).Select(x1 => x1.OPERATING_STATUS).FirstOrDefault(),

                }).FirstOrDefault();
                ISSCMemberList.Add(PeriodSettingList);
            }
            return ISSCMemberList;
        }

        public RiskAssessmentSYKIViewModel GetRiskRegistrationSYKIList()
        {
            RiskAssessmentSYKIViewModel riskRegistrationSYKIViewModel = new RiskAssessmentSYKIViewModel();

            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    riskRegistrationSYKIViewModel._SYKIList.Add(new RiskAssessmentSYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return riskRegistrationSYKIViewModel;
        }
        public List<Get_RiskDivision_ISSC_Member> Get_ISSC_Member_Details_For_OpearingHead(int? id)
        {
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();
            var operationid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.OPERATIONID).FirstOrDefault(); ;
            var ISSC_Member = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x => x.ACTIVE == 1 && x.SKIID == SYKIList.SYKIID && x.OPERATIONID == operationid).Select(x => x.DIVISIONID).ToList();
            List<Get_RiskDivision_ISSC_Member> ISSCMemberList = new List<Get_RiskDivision_ISSC_Member>();
            foreach (var item in ISSC_Member)
            {
                var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item && x.SYKI == SYKIList.SYKIID).Select(x => new Get_RiskDivision_ISSC_Member
                {
                    DivId = (long)x.DIVISIONID,
                    DivisionName = x.DIVISION,
                    ISSCMember_EmpCode = x.ADEMPCODE,
                    //ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                    EndDate = _arDBContext.RISK_ASSESSMENT_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),

                    ISSC_Member_Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && x1.STATUS == 2).Select(x1 => x1.SUBMIT_DATE_ISSCMEMBER).FirstOrDefault(),
                    ISSC_Member_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && x1.STATUS == 2).Select(x1 => x1.ISSC_MEMBER_STATUS).FirstOrDefault(),

                    DivisionHead_Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && x1.STATUS == 2).Select(x1 => x1.SUBMIT_DATE_DIVISIONHEAD).FirstOrDefault(),
                    DivisionHead_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && x1.STATUS == 2).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

                    OperatingHead__Submit_Date = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && x1.STATUS == 2).Select(x1 => x1.SUBMIT_DATE_OPERATINGHEAD).FirstOrDefault(),
                    OperatingHead_Submit_Status = _arDBContext.RISKASSESSMENT.Where(x1 => x1.SKYID == SYKIList.SYKIID && x1.RISK_CREATEDBY == item && x1.ACTIVE == 1 && x1.STATUS == 2).Select(x1 => x1.OPERATING_STATUS).FirstOrDefault(),

                }).FirstOrDefault();
                ISSCMemberList.Add(PeriodSettingList);
            }
            return ISSCMemberList;
        }

        public RiskAssessmentSYKIViewModelOH GetRiskAssessmentSYKIListFor_OperatingHeadDeails_Dashboard(long SYKI, int Empcode)
        {
            RiskAssessmentSYKIViewModelOH assetRegistrationSYKIViewModel = new RiskAssessmentSYKIViewModelOH();
            var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Empcode && x.SYKI == SYKI).Select(x => x.OPERATIONID).ToList();
            var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID > SYKI && curent_year_deatil.Contains(x.CURRENT_OPERATIONID) && x.STATUS == 1).Select(x => x).ToList();
            if (get_prevyear_detail != null)
            {
                var SYKIList = get_prevyear_detail.Select(y => y.PREVIOUS_SYKIID).ToList();
                var OperationIDList = get_prevyear_detail.Select(y => (decimal)y.PREVIOUS_OPERATIONID).ToList();
                var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && SYKIList.Contains(x.SYKIID) && OperationIDList.Contains(x.OPERATIONID)).Select(x => x).ToList();

                if (get_prevyear_userid.Count > 0)
                {
                    foreach (var item in get_prevyear_userid)
                    {
                        assetRegistrationSYKIViewModel._SYKIList.Add(new RiskAssessmentSYKIListForOH
                        {
                            DIVISIONNAMEN = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.DIVISION).FirstOrDefault(),
                            KICODE = _arDBContext.SYKI.Where(x => x.SYKIID == item.SYKIID).Select(x => x.KICODE).FirstOrDefault(),
                            SYKIID = item.SYKIID,
                            ADEMPCODE = item.ISSCEMPCODE,
                            OperationName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.OPERATION).FirstOrDefault(),
                            OPID = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.OPERATIONID).FirstOrDefault(),

                        });
                    }
                }
            }
            return assetRegistrationSYKIViewModel;
        }

        public int Risk_SendBack_By_OperatingHead(CommonRiskAssessmentVM risk_details)
        {
            if (risk_details != null && risk_details.SKYID > 0 && risk_details.CreatedBy > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                var result_List = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == risk_details.CreatedBy && x.SKYID == risk_details.SKYID && x.ACTIVE == 1 && x.DIVISION_STATUS == 1).ToList();
                result_List.ForEach(ass_det =>
                {
                    ass_det.ACTIVE = 1;
                    ass_det.STATUS = 5;

                    ass_det.ISSC_MEMBER_STATUS = null;
                    //ass_det.ISSC_MEMBER_REASON = null;
                    ass_det.SUBMIT_DATE_ISSCMEMBER = null;

                    ass_det.DIVISION_STATUS = null;
                    ass_det.SUBMIT_DATE_DIVISIONHEAD = null;
                    ass_det.REVIEW_REMARK_DIVISIONHEAD = null;
                    ass_det.RISK_UPDATEDDATE = DateTime.Now;
                    ass_det.OPERATING_STATUS = null;
                    ass_det.SUBMIT_DATE_OPERATINGHEAD = null;
                    ass_det.REVIEW_REMARK_OPERATINGHEAD = risk_details.OP_Head_Remarks;
                }
                );
                _arDBContext.SaveChanges();
                //}
                return 3; //Send back 
            }
            return 2;//There is some problem, please try again later.
        }

        public Risk_DivisionHead_Details Get_OperatingHead_(long? SYKI, long Empcode, long Userid)
        {
            var empNamelist = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADEMPCODE == Empcode).Select(x => new { x.SUPSUPERVISOREMPCODE, x.DIVISION }).FirstOrDefault();
            var ISSC_Member = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Empcode).Select(x => new { ISSCName = x.FIRSTNAME + " " + x.LASTNAME, ISSCEmail = x.EMAILID }).FirstOrDefault();
            var DivisionHead_Details = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Userid).Select(x => new Risk_DivisionHead_Details
            {
                Empname = x.FIRSTNAME + " " + x.LASTNAME,
                Emailid = x.EMAILID,
                DivisionName = empNamelist.DIVISION,
                DivisionEmpCode = x.ADEMPCODE,
                ISSC_Member_Name = ISSC_Member.ISSCName,
                ISSC_MemberEmail = ISSC_Member.ISSCEmail
            }).FirstOrDefault();
            return DivisionHead_Details;
        }
        public int Risk_Approve_By_OperatingHead(CommonRiskAssessmentVM risk_details)
        {
            if (risk_details != null && risk_details.SKYID > 0 && risk_details.CreatedBy > 0 && risk_details.Operating_Head_EmpCode > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                var result_List = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == risk_details.CreatedBy && x.SKYID == risk_details.SKYID && x.ACTIVE == 1 && x.DIVISION_STATUS == 1).ToList();
                result_List.ForEach(ass_det =>
                {
                    ass_det.OPERATING_HEAD_ID = risk_details.Operating_Head_EmpCode;
                    ass_det.SUBMIT_DATE_OPERATINGHEAD = DateTime.Now;
                    ass_det.OPERATING_STATUS = 1;
                    ass_det.STATUS = 3;
                    ass_det.RISK_UPDATEDBY = risk_details.Operating_Head_EmpCode;
                    ass_det.REVIEW_REMARK_OPERATINGHEAD = risk_details.OP_Head_Remarks;
                }
                );
                _arDBContext.SaveChanges();
                //}
                return 1;//Approved 
            }
            return 2;//There is some problem, please try again later.
        }
        public RiskAssessmentSYKIViewModel GetRiskAssessmentSYKIListFor_OperatingHead_Dashboard(long SYKI, int Empcode)
        {
            RiskAssessmentSYKIViewModel assetRegistrationSYKIViewModel = new RiskAssessmentSYKIViewModel();
            var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Empcode && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.STATUS == 1).Select(x => x).FirstOrDefault();
            if (get_prevyear_detail != null)
            {
                var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == get_prevyear_detail.PREVIOUS_SYKIID && x.OPERATIONID == get_prevyear_detail.PREVIOUS_OPERATIONID).Select(x => x).ToList();

                if (get_prevyear_userid.Count > 0)
                {
                    foreach (var item in get_prevyear_userid)
                    {
                        assetRegistrationSYKIViewModel._SYKIList.Add(new RiskAssessmentSYKIList
                        {
                            DIVISIONNAMEN = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.DIVISION).FirstOrDefault(),
                            KICODE = _arDBContext.SYKI.Where(x => x.SYKIID == item.SYKIID).Select(x => x.KICODE).FirstOrDefault(),
                            SYKIID = item.SYKIID,
                            ADEMPCODE = item.ISSCEMPCODE,
                            OperationName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.OPERATION).FirstOrDefault(),
                        });
                    }
                }
            }
            return assetRegistrationSYKIViewModel;
        }


        public int InsertRiskDifiency(Risk_DifiencyReport difiency)
        {
            if (difiency != null && difiency.DivisionId > 0 && difiency.operationId > 0)
            {
                var datacount = _arDBContext.RISK_ASSESSMENT_DEFICIENCY.Count();
                var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
                var empcode = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.DIVISIONID == difiency.DivisionId && x.OPERATIONID == difiency.operationId && x.STATUS == 1 && x.ACTIVE == 1 && x.SYKIID == currentki).Select(x => x.ISSCEMPCODE).FirstOrDefault();
                if (empcode > 0)
                {
                    using (var transaction = _arDBContext.Database.BeginTransaction())
                    {
                        try
                        {
                            //RISK_ASSESSMENT_DEFICIENCY difiy = new RISK_ASSESSMENT_DEFICIENCY();
                            //difiy.ID = datacount + 1;
                            //difiy.OPERATIONID = difiency.operationId;
                            //difiy.DIVISIONID = difiency.DivisionId;
                            //difiy.PRIMARYASSETID = difiency.PrimaryAssetId;
                            //difiy.RISKCATEGORYID = difiency.RiskCategoeyId;

                            //difiy.FEEDBACK = difiency.FeedBack;
                            //difiy.ISSCEMPCODE = empcode;
                            //difiy.CREATIONDATE = DateTime.Now;
                            //difiy.CREATEDBY = difiency.createdby;
                            //_arDBContext.Entry(difiy).State = EntityState.Added;
                            //_arDBContext.SaveChanges();


                            //Note: EF 6 in .NET Framework does allow working with tables without primary keys,
                            //and it can even track changes using internal mechanisms like shadow keys or by treating the entity as non - tracked in some cases.
                            //However, EF Core is stricter than EF 6.Here's a breakdown of the differences and how to work around it in EF Core:


                            string sql = @"
                            INSERT INTO RISK_ASSESSMENT_DEFICIENCY (ID, OPERATIONID, DIVISIONID, PRIMARYASSETID, RISKCATEGORYID, FEEDBACK, ISSCEMPCODE, CREATIONDATE, CREATEDBY)
                            VALUES (:param1, :param2, :param3, :param4, :param5, :param6, :param7,:param8, :param9)"
                            ;

                            _arDBContext.Database.ExecuteSqlRaw(sql,
                            new OracleParameter("param1", datacount + 1),
                                new OracleParameter("param2", difiency.operationId),
                                new OracleParameter("param3", difiency.DivisionId),
                                new OracleParameter("param4", difiency.PrimaryAssetId),
                                new OracleParameter("param5", difiency.RiskCategoeyId),
                                new OracleParameter("param6", difiency.FeedBack),
                                new OracleParameter("param7", empcode),
                                new OracleParameter("param8", DateTime.Now),
                                new OracleParameter("param9", difiency.createdby)
                            );


                            var result_List = _arDBContext.RISKASSESSMENT.Where(x => x.RISK_CREATEDBY == empcode && x.SKYID == currentki && x.ACTIVE == 1).ToList();
                            result_List.ForEach(ass_det =>
                            {
                                ass_det.STATUS = 6;

                                ass_det.ISSC_MEMBER_STATUS = null;
                                // ass_det.ISSC_MEMBER_REASON = null;
                                ass_det.SUBMIT_DATE_ISSCMEMBER = null;

                                ass_det.DIVISION_STATUS = null;
                                ass_det.SUBMIT_DATE_DIVISIONHEAD = null;
                                ass_det.REVIEW_REMARK_DIVISIONHEAD = null;

                                ass_det.OPERATING_STATUS = null;
                                ass_det.SUBMIT_DATE_OPERATINGHEAD = null;
                                ass_det.REVIEW_REMARK_OPERATINGHEAD = null;

                            });
                            _arDBContext.SaveChanges();
                            transaction.Commit();
                            return 1;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            return 2;
        }

        public List<PrimaryAssestsVM> BindPrimaryAsset(long? div_Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var empcode = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.DIVISIONID == div_Id && x.STATUS == 1 && x.ACTIVE == 1 && x.SYKIID == currentki).Select(x => x.ISSCEMPCODE).FirstOrDefault();
            var iList = (from reg in _arDBContext.ASSET_REGISTER_ASSETDETAILS
                         from riskasst in _arDBContext.RISKASSESSMENT

                         where reg.SKIID == currentki && reg.CREATEDBY == empcode && reg.ID == riskasst.PRIMARY_ASSETID
                         && reg.PRIMARY != null && reg.ACTIVE == 1 && riskasst.REVIEW_REMARK_DIVISIONHEAD != null && riskasst.REVIEW_REMARK_OPERATINGHEAD != null
                         && riskasst.SUBMIT_DATE_DIVISIONHEAD != null && riskasst.SUBMIT_DATE_OPERATINGHEAD != null
                         select new PrimaryAssestsVM
                         {
                             AssestId = (int)reg.ID,
                             Assets = reg.PRIMARY
                         }).ToList();

            iList = iList.GroupBy(x => x.Assets).Select(y => y.First()).ToList();
            return iList;


        }

        public Risk_ISSCMember_details Send_Email_To_ISSC_(CommonRiskAssessmentVM risk_details)
        {
            Risk_ISSCMember_details Emp_Details = new Risk_ISSCMember_details();
            Emp_Details = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == risk_details.CreatedBy).Select(x => new Risk_ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
            return Emp_Details;
        }
        public List<Risk_ISSCMember_details> Send_Email_To_ISSC_Member_DivisionHead(Risk_DifiencyReport difiency)
        {
            List<Risk_ISSCMember_details> Emp_Details = new List<Risk_ISSCMember_details>();
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var empcode = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.DIVISIONID == difiency.DivisionId && x.STATUS == 1 && x.ACTIVE == 1 && x.SYKIID == currentki).Select(x => x.ISSCEMPCODE).FirstOrDefault();
            var emp = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empcode).Select(x => new Risk_ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME, EmpCode = x.ADEMPCODE }).FirstOrDefault();
            Emp_Details.Add(emp);

            return Emp_Details;
        }
    }
}
