using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using ePortal.Shared;
/// <summary>
/// New service
/// </summary>
namespace ePortal.Application.Services
{
    public class AssetDisposalService : IAssetDisposalService
    {
        //AssetDisposalRepository _AssetRepo;
        //CommonRepository _CommonRepo;
        //Tuple<short, long> _retVal_tuple;

        //public AssetDisposalService()
        //{
        //    _AssetRepo = new AssetDisposalRepository();
        //    _CommonRepo = new CommonRepository();
        //    _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        //}

        private readonly AssetDisposalRepository _AssetRepo;
        private readonly CommonRepository _CommonRepo;
        //private (short, long) _retVal_tuple;
        private Tuple<short, long> _retVal_tuple;
        public AssetDisposalService(AssetDisposalRepository assetRepo, CommonRepository commonRepo)
        {
            _AssetRepo = assetRepo ?? throw new ArgumentNullException(nameof(assetRepo));
            _CommonRepo = commonRepo ?? throw new ArgumentNullException(nameof(commonRepo));

            //_retVal_tuple = (0, 0L); // ValueTuple initialization
            _retVal_tuple = Tuple.Create((short)0, 0L);

        }

        #region Master
        public List<SIS_ASSETTYPE_MST> GetAssetTypeList()
        {
            return _AssetRepo.GetAssetTypeList();
        }

        public List<SIS_ASSETCONDITION> GetConditionList()
        {
            return _AssetRepo.GetConditionList();
        }

        public List<SYSITE> GetSiteList()
        {
            return _AssetRepo.GetSiteList();
        }

        public List<ADORGLEVEL> GetOrgLevelList()
        {
            return _AssetRepo.GetOrgLevelList();
        }

        public long? GetOperationMappId(Employee_Details _Employee_Details)
        {
            List<long?> _orgList = new List<long?>();
            if (_Employee_Details._SecId.HasValue)
                _orgList.Add(_Employee_Details._SecId);
            if (_Employee_Details._DepId.HasValue)
                _orgList.Add(_Employee_Details._DepId);
            if (_Employee_Details._DivId.HasValue)
                _orgList.Add(_Employee_Details._DivId);
            if (_Employee_Details._OpId.HasValue)
                _orgList.Add(_Employee_Details._OpId);
            return this._AssetRepo.GetOperationMappId(_orgList);
        }

        public string GetFnCodeByOperationId(long operationId)
        {
            return _AssetRepo.GetFnCodeByOperationId(operationId);
        }

        public SYSITE GetSiteDetailBySiteId(long siteId)
        {
            return _AssetRepo.GetSiteDetailBySiteId(siteId);
        }

        public List<SIS_AST_PARAM_MST> GetParamList()
        {
            return _AssetRepo.GetParamList();
        }

        public List<SIS_AST_PROCESSSTATUS_MST> GetProcessStatusList()
        {
            return _AssetRepo.GetProcessStatusList();
        }

        public List<ApprovalAuthorityViewModel> GetApprovalAuthorityList()
        {
            return _AssetRepo.GetApprovalAuthorityList();
        }

        public List<ApprovalAuthorityViewModel> ManageApprovalAuthorityByFinance()
        {
            return this._AssetRepo.GetApprovalAuthorityListByFinance();
        }

        public List<ApprovalAuthorityViewModel> ManageApprovalAuthorityByPPC()
        {
            return this._AssetRepo.GetApprovalAuthorityListByPPC();
        }

        public ApprovalAuthorityViewModel GetApprovalAuthorityById(long id)
        {
            return _AssetRepo.GetApprovalAuthorityById(id);
        }
        //Updated By Deloitte SR72269
        public short SaveApprovalAuthority(ApprovalAuthorityViewModel AAVM,String transationType)
        {
            return _AssetRepo.SaveApprovalAuthority(AAVM,transationType);
        }
        //--//Updated By Deloitte SR72269
        public List<OPMappingViewModel> GetOperationMappingList()
        {
            return _AssetRepo.GetOperationMappingList();
        }

        public OPMappingViewModel GetOperationMappingById(long id)
        {
            return _AssetRepo.GetOperationMappingById(id);
        }

        public short SaveOperationMapping(OPMappingViewModel OMVM)
        {
            return _AssetRepo.SaveOperationMapping(OMVM);
        }

        public List<ValidationViewModel> GetValidationList()
        {
            List<ValidationViewModel> validationList = this._AssetRepo.GetValidationList();
            foreach (ValidationViewModel validationViewModel1 in validationList)
            {
                ValidationViewModel VVM = validationViewModel1;
                if (!string.IsNullOrEmpty(VVM.PARAMVALUE) && VVM.Parm_mst.PARAMTYPE == "Location")
                {
                    string str1 = "";
                    List<SYSITE> siteList = this._AssetRepo.GetSiteList();
                    string paramvalue = VVM.PARAMVALUE;
                    char[] chArray = new char[1] { ',' };
                    foreach (string str2 in ((IEnumerable<string>)paramvalue.Split(chArray)).ToArray<string>())
                    {
                        if (!string.IsNullOrEmpty(str2))
                        {
                            long valId = Convert.ToInt64(str2);
                            str1 = str1 + siteList.Where(x => x.SYSITEID == valId).Select(s => s.DESCRIP).SingleOrDefault() + ", ";
                            ////str1 = str1 + siteList.Where<SYSITE>((Func<SYSITE, bool>)(x => x.SYSITEID == valId)).Select<SYSITE, string>((Func<SYSITE, string>)(x => x.DESCRIP)).SingleOrDefault<string>() + ", ";
                        }
                    }
                    ////ValidationViewModel validationViewModel2 = validationList.FirstOrDefault<ValidationViewModel>((Func<ValidationViewModel, bool>)(x => x.VALIDATIONID == VVM.VALIDATIONID));
                    ValidationViewModel validationViewModel2 = validationList.FirstOrDefault(x => x.VALIDATIONID == VVM.VALIDATIONID);
                    if (validationViewModel2 != null)
                        validationViewModel2.PARAMVALUE = str1;
                }
            }
            return validationList;
        }

        public ValidationViewModel GetValidationById(long id)
        {
            return _AssetRepo.GetValidationById(id);
        }

        public short SaveValidation(ValidationViewModel VVM)
        {
            string str = "";
            //if (VVM.SITE != null)
            if (VVM.SITE != null && VVM.SITE.Any(x => x.HasValue && x != 0))
            {
                foreach (long? obj in VVM.SITE)
                    str = str + obj.ToString() + ",";
                VVM.PARAMVALUE = str;
            }
            return _AssetRepo.SaveValidation(VVM);
        }

        public List<AssetOperationViewModel> GetOperationList()
        {
            return _AssetRepo.GetOperationList();
        }

        public AssetOperationViewModel GetOperationDtlById(long id)
        {
            return _AssetRepo.GetOperationDtlById(id);
        }

        public short SaveOperation(AssetOperationViewModel AOVM)
        {
            return _AssetRepo.SaveOperation(AOVM);
        }

        public int GetOperationCount(long _opId)
        {
            return _AssetRepo.GetOperationCount(_opId);
        }
        #endregion

        #region Asset Disposal Request
        public Tuple<short, long> SaveAssetItem(AssetDisposalViewModel model)
        {
            return _AssetRepo.SaveAssetItem(model);
        }

        public short SaveAssetRequest(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            long? operationMappId = GetOperationMappId(_Employee_Details);
            string codeByOperationId = GetFnCodeByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_Employee_Details._OpId);
            // string codeByOperationId = this._AssetRepo.GetFnCodeByOperationId(_Employee_Details._OpId.Value);
            short Status_Level = string.IsNullOrEmpty(codeByOperationId)
                    ? (short)1 : string.IsNullOrEmpty(this._AssetRepo.CheckValidationByFnCode(codeByOperationId, "Maintenance Dept. Approval"))
                    ? (short)1 : (short)22;

            _retVal_tuple = this._AssetRepo.SaveAssetRequest(ADVM, Status_Level);
            if (_retVal_tuple.Item1 == (short)1)
            {
                if (_retVal_tuple.Item2 > 0)
                {
                    ADVM.DISPOSALHEADERID = _retVal_tuple.Item2;
                }
                this.SendEmailForReqApproval(Status_Level, 0L, ADVM, _Employee_Details);
            }
            return _retVal_tuple.Item1;
        }

        public List<AssetDisposalViewModel> GetAssetRequestList(long loginUser)
        {
            return _AssetRepo.GetAssetRequestList(loginUser);
        }

        public AssetDisposalViewModel GetDetailById(long id, Employee_Details _Employee_Details)
        {
            AssetDisposalViewModel disposalViewModel = new AssetDisposalViewModel();
            try
            {
                disposalViewModel = this._AssetRepo.GetDetailById(id);
                if (disposalViewModel != null)
                {
                    this._retVal_tuple = this.SetStatusLevel(_Employee_Details, disposalViewModel);                    
                    short SetApprovalLevel = this._retVal_tuple.Item1;
                    if (SetApprovalLevel > (short)0)
                    {
                        string str = this._AssetRepo.GetProcessStatusList()
                                    .Where(p => (int)p.STATUS_LEVEL == (int)SetApprovalLevel)
                                    .Select(s => s.STATUSDESCRIPTION).FirstOrDefault().ToString();
                        Employee_Details approvalAuthorityDetail = this.GetNextApprovalAuthorityDetail(this._retVal_tuple.Item1, this._retVal_tuple.Item2, disposalViewModel);
                        if (approvalAuthorityDetail != null)
                            disposalViewModel.ForwardedTo = str + " - " + approvalAuthorityDetail._EName;
                    }
                    List<ApprovalHisViewModel> iList = (List<ApprovalHisViewModel>)disposalViewModel.AppHistoryList;
                    disposalViewModel.AppHistoryList = GetAppAuthorityList(disposalViewModel, iList, _Employee_Details);
                }
                else
                    disposalViewModel = new AssetDisposalViewModel();
            }
            catch (Exception ex)
            {
            }
            return disposalViewModel;
        }

        public AssetDisposalViewModel GetAssetRequestById(long id, Employee_Details _Employee_Details)
        {
            AssetDisposalViewModel disposalViewModel = new AssetDisposalViewModel();
            try
            {
                disposalViewModel = this._AssetRepo.GetAssetRequestById(id);
                short crnt_Status_Level = disposalViewModel.STATUS_LEVEL;
                if (disposalViewModel != null)
                {
                    ///// -- PPC Approval --/////
                    if (disposalViewModel.DisposalDetailList != null && disposalViewModel.DisposalDetailList.Any())
                    {
                        disposalViewModel.C1F_APPROVEBY_NAME = disposalViewModel.DisposalDetailList[0].C1F_APPROVEBY == null ? "" : _CommonRepo.GetEmpDetailById((long)disposalViewModel.DisposalDetailList[0].C1F_APPROVEBY)._EName;
                        disposalViewModel.C2F_APPROVEBY_NAME = disposalViewModel.DisposalDetailList[0].C2F_APPROVEBY == null ? "" : _CommonRepo.GetEmpDetailById((long)disposalViewModel.DisposalDetailList[0].C2F_APPROVEBY)._EName;
                        disposalViewModel.C3F_APPROVEBY_NAME = disposalViewModel.DisposalDetailList[0].C3F_APPROVEBY == null ? "" : _CommonRepo.GetEmpDetailById((long)disposalViewModel.DisposalDetailList[0].C3F_APPROVEBY)._EName;
                        disposalViewModel.C4F_APPROVEBY_NAME = disposalViewModel.DisposalDetailList[0].C4F_APPROVEBY == null ? "" : _CommonRepo.GetEmpDetailById((long)disposalViewModel.DisposalDetailList[0].C4F_APPROVEBY)._EName;
                    }
                    this._retVal_tuple = this.SetStatusLevel(_Employee_Details, disposalViewModel);
                    short SetApprovalLevel = this._retVal_tuple.Item1;
                    if (SetApprovalLevel > (short)0)
                    {
                        string str = this._AssetRepo.GetProcessStatusList().Where(p => (int)p.STATUS_LEVEL == (int)SetApprovalLevel).Select(s => s.STATUSDESCRIPTION).FirstOrDefault().ToString();
                        Employee_Details approvalAuthorityDetail = this.GetNextApprovalAuthorityDetail(this._retVal_tuple.Item1, this._retVal_tuple.Item2, disposalViewModel);
                        if (approvalAuthorityDetail != null)
                            disposalViewModel.ForwardedTo = str + " - " + approvalAuthorityDetail._EName;
                    }
                    List<ApprovalHisViewModel> iList = (List<ApprovalHisViewModel>)disposalViewModel.AppHistoryList;
                    disposalViewModel.AppHistoryList = GetAppAuthorityList(disposalViewModel, iList, _Employee_Details);
                    ApprovalHisViewModel appHis = disposalViewModel.AppHistoryList.Where(h => h.STATUS_LEVEL == crnt_Status_Level && h.APPROVALSTATUS == 0).FirstOrDefault();
                    if (appHis != null)
                    {
                        disposalViewModel.IsEnableForApp = (appHis.APPCODE == _Employee_Details._ECode) ? (short)1 : (short)0;
                    }
                }
                else
                    disposalViewModel = new AssetDisposalViewModel();
            }
            catch (Exception ex)
            {
            }
            return disposalViewModel;
        }

        public List<ApprovalHisViewModel> GetAppAuthorityList(AssetDisposalViewModel disposalViewModel, List<ApprovalHisViewModel> his_List, Employee_Details _Employee_Details)
        {
            List<SIS_AST_PROCESSSTATUS_MST> _proList = _AssetRepo.GetProcessStatusList();
            if (_proList.Count > 0 && disposalViewModel.STATUS_LEVEL != 20)
            {
                List<ApprovalHisViewModel> pending_his_List = new List<ApprovalHisViewModel>();
                //foreach (SIS_AST_PROCESSSTATUS_MST pro_obj in _proList.OrderBy(o => o.STATUS_LEVEL)) // Commented by TTL SR95154 :: CR6236
                foreach (SIS_AST_PROCESSSTATUS_MST pro_obj in _proList.OrderBy(t => t.SEQ_NUMBER == null ? 1 : 0).ThenBy(t => t.SEQ_NUMBER).ThenBy(t => t.STATUS_LEVEL)) // Added by TTL SR95154 :: CR6236
                {
                    if (pro_obj.STATUS_LEVEL > 0)
                    {
                        short[] LEVEL_ARRAY = { 2, 3, 4, 5, 6, 7, 8, 9, 24, 28 }; // Added 28 by TTL SR95154 :: CR6236
                        short[] LEVEL_ARRAY2 = { 1, 10, 11, 12 };
                        ApprovalHisViewModel sendbackObj = his_List.OrderBy(s => s.APPROVALHISTORYID).LastOrDefault(d => d.APPROVALSTATUS == 2 && d.STATUS_LEVEL != 18); //// -- 18 for Gate security

                        if (!his_List.Any(h => h.STATUS_LEVEL == pro_obj.STATUS_LEVEL) && sendbackObj == null)
                        {
                            #region Get Pending Authority
                            if (LEVEL_ARRAY.Contains(pro_obj.STATUS_LEVEL))
                            {
                                List<ApprovalHisViewModel> defaultAuthority = GetDefaultAuthority(disposalViewModel);
                                if (defaultAuthority.Count > 0)
                                {
                                    foreach (ApprovalHisViewModel AHVM_obj in defaultAuthority)
                                    {
                                        if (!his_List.Any(h => h.STATUS_LEVEL == AHVM_obj.STATUS_LEVEL /*&& h.APPCODE == AHVM_obj.APPCODE*/) && AHVM_obj.APPCODE != disposalViewModel.ADEMPCODE)
                                        {
                                            if (!his_List.Any(n => n.SEQ_NUMBER > AHVM_obj.SEQ_NUMBER))
                                            {
                                                his_List.Add(AHVM_obj);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (LEVEL_ARRAY2.Contains(pro_obj.STATUS_LEVEL))
                            {
                                bool isContinue = true;
                                long? operationMappId = GetOperationMappId(disposalViewModel.Emp_Detail);

                                if (pro_obj.STATUS_LEVEL == 1)
                                {
                                    string _codeByOperationId = GetFnCodeByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)disposalViewModel.Emp_Detail._OpId);
                                    short _STATUS_LEVEL = string.IsNullOrEmpty(_codeByOperationId)
                                            ? (short)1 : string.IsNullOrEmpty(_AssetRepo.CheckValidationByFnCode(_codeByOperationId, "Maintenance Dept. Approval"))
                                            ? (short)1 : (short)22;
                                    if (_STATUS_LEVEL == 22)
                                    {
                                        Employee_Details approvalAuthorityDetail = GetNextApprovalAuthorityDetail(_STATUS_LEVEL, 0, disposalViewModel);
                                        if (approvalAuthorityDetail != null)
                                        {
                                            if (!his_List.Any(h => h.STATUS_LEVEL == _STATUS_LEVEL) && approvalAuthorityDetail._ECode != disposalViewModel.ADEMPCODE)
                                            {
                                                his_List.Add(new ApprovalHisViewModel
                                                {
                                                    DISPOSALHEADERID = disposalViewModel.DISPOSALHEADERID,
                                                    PROCESSSTATUSID = 0,  /////// disposalViewModel.PROCESSSTATUSID, 
                                                    APPROVALHISTORYID = 0,
                                                    APPROVALSTATUS = 0,
                                                    STATUS_LEVEL = _STATUS_LEVEL,
                                                    DISPLAY_MSG = "Pending at Maintenance Dept.",
                                                    ATTACHMENT = "",
                                                    APPCODE = approvalAuthorityDetail._ECode,
                                                    APPEMP_NAME = approvalAuthorityDetail._EName,
                                                    REMARKS = "",
                                                });
                                            }
                                        }
                                    }
                                }

                                if (pro_obj.STATUS_LEVEL == 10)
                                {
                                    if (_AssetRepo.GetApprovalAuthorityBySiteId(Convert.ToInt64(disposalViewModel.Emp_Detail._SiteId == null ? 0 : disposalViewModel.Emp_Detail._SiteId), Convert.ToInt16(10)) != null) //// --- 10 for CPO approval
                                    {
                                        string codeByOperationId = _AssetRepo.GetFnCodeByOperationId((operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)disposalViewModel.Emp_Detail._OpId));
                                        if (!string.IsNullOrEmpty(codeByOperationId))
                                        {
                                            string validationParmValue = _AssetRepo.GetValidationParmValue("CPO Approval");
                                            isContinue = string.IsNullOrEmpty(validationParmValue) || !validationParmValue.Contains(Convert.ToString(codeByOperationId)) ? false : true;
                                        }
                                        else
                                            isContinue = false;
                                    }
                                    else
                                    {
                                        isContinue = false;
                                    }
                                }

                                if (pro_obj.STATUS_LEVEL == 12)
                                {
                                    string validationParmValue = _AssetRepo.GetValidationParmValue("IBM Approval");
                                    bool isNotValid = string.IsNullOrEmpty(validationParmValue) ? true : (disposalViewModel.TOTAL_AMOUNT <= Convert.ToInt64(validationParmValue));
                                    if (isNotValid)
                                    {
                                        isContinue = false;
                                    }
                                }

                                if (isContinue)
                                {
                                    Employee_Details approvalAuthorityDetail = GetNextApprovalAuthorityDetail(pro_obj.STATUS_LEVEL, 0, disposalViewModel);
                                    if (approvalAuthorityDetail != null)
                                    {
                                        his_List.Add(new ApprovalHisViewModel
                                        {
                                            DISPOSALHEADERID = disposalViewModel.DISPOSALHEADERID,
                                            PROCESSSTATUSID = 0, //////// disposalViewModel.PROCESSSTATUSID,
                                            APPROVALHISTORYID = 0,
                                            APPROVALSTATUS = 0,
                                            STATUS_LEVEL = pro_obj.STATUS_LEVEL,
                                            DISPLAY_MSG = pro_obj.STATUSDESCRIPTION,
                                            ATTACHMENT = "",
                                            APPCODE = approvalAuthorityDetail._ECode,
                                            APPEMP_NAME = approvalAuthorityDetail._EName,
                                            REMARKS = "",
                                        });
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (sendbackObj != null && !pending_his_List.Any(s => s.STATUS_LEVEL == pro_obj.STATUS_LEVEL))
                        {
                            #region Get Authority in Case of Sendback
                            List<ApprovalHisViewModel> afterSendbackRecordList = his_List.Where(w => w.APPROVALHISTORYID > sendbackObj.APPROVALHISTORYID).ToList();
                            if (afterSendbackRecordList.Count > 0)
                            {
                                foreach (ApprovalHisViewModel aObj in afterSendbackRecordList)
                                {
                                    if (!pending_his_List.Any(g => g.APPROVALHISTORYID == aObj.APPROVALHISTORYID && g.STATUS_LEVEL == aObj.STATUS_LEVEL))
                                    {
                                        pending_his_List.Add(aObj);
                                    }
                                }
                            }

                            if (LEVEL_ARRAY.Contains(pro_obj.STATUS_LEVEL))
                            {
                                List<ApprovalHisViewModel> defaultAuthority = GetDefaultAuthority(disposalViewModel);
                                if (defaultAuthority.Count > 0)
                                {
                                    foreach (ApprovalHisViewModel AHVM_obj in defaultAuthority)
                                    {
                                        if (!pending_his_List.Any(h => h.STATUS_LEVEL == AHVM_obj.STATUS_LEVEL /*&& h.APPCODE == AHVM_obj.APPCODE*/) && AHVM_obj.APPCODE != disposalViewModel.ADEMPCODE)
                                        {
                                            if (!pending_his_List.Any(n => n.SEQ_NUMBER > AHVM_obj.SEQ_NUMBER))
                                            {
                                                pending_his_List.Add(AHVM_obj);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (LEVEL_ARRAY2.Contains(pro_obj.STATUS_LEVEL))
                            {
                                bool isContinue = true;
                                long? operationMappId = GetOperationMappId(disposalViewModel.Emp_Detail);

                                if (pro_obj.STATUS_LEVEL == 1)
                                {
                                    string _codeByOperationId = GetFnCodeByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)disposalViewModel.Emp_Detail._OpId);
                                    short _STATUS_LEVEL = string.IsNullOrEmpty(_codeByOperationId)
                                            ? (short)1 : string.IsNullOrEmpty(_AssetRepo.CheckValidationByFnCode(_codeByOperationId, "Maintenance Dept. Approval"))
                                            ? (short)1 : (short)22;
                                    if (_STATUS_LEVEL == 22)
                                    {
                                        Employee_Details approvalAuthorityDetail = GetNextApprovalAuthorityDetail(_STATUS_LEVEL, 0, disposalViewModel);
                                        if (approvalAuthorityDetail != null)
                                        {
                                            if (!pending_his_List.Any(h => h.STATUS_LEVEL == _STATUS_LEVEL) && approvalAuthorityDetail._ECode != disposalViewModel.ADEMPCODE)
                                            {
                                                pending_his_List.Add(new ApprovalHisViewModel
                                                {
                                                    DISPOSALHEADERID = disposalViewModel.DISPOSALHEADERID,
                                                    PROCESSSTATUSID = 0,  /////// disposalViewModel.PROCESSSTATUSID, 
                                                    APPROVALHISTORYID = 0,
                                                    APPROVALSTATUS = 0,
                                                    STATUS_LEVEL = _STATUS_LEVEL,
                                                    DISPLAY_MSG = "Pending at Maintenance Dept.",
                                                    ATTACHMENT = "",
                                                    APPCODE = approvalAuthorityDetail._ECode,
                                                    APPEMP_NAME = approvalAuthorityDetail._EName,
                                                    REMARKS = "",
                                                });
                                            }
                                        }
                                    }
                                }

                                if (pro_obj.STATUS_LEVEL == 10)
                                {
                                    if (_AssetRepo.GetApprovalAuthorityBySiteId(Convert.ToInt64(disposalViewModel.Emp_Detail._SiteId == null ? 0 : disposalViewModel.Emp_Detail._SiteId), Convert.ToInt16(10)) != null) //// --- 10 for CPO approval
                                    {
                                        string codeByOperationId = _AssetRepo.GetFnCodeByOperationId((operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)disposalViewModel.Emp_Detail._OpId));
                                        if (!string.IsNullOrEmpty(codeByOperationId))
                                        {
                                            string validationParmValue = _AssetRepo.GetValidationParmValue("CPO Approval");
                                            isContinue = string.IsNullOrEmpty(validationParmValue) || !validationParmValue.Contains(Convert.ToString(codeByOperationId)) ? false : true;
                                        }
                                        else
                                            isContinue = false;
                                    }
                                    else
                                    {
                                        isContinue = false;
                                    }
                                }

                                if (pro_obj.STATUS_LEVEL == 12)
                                {
                                    string validationParmValue = _AssetRepo.GetValidationParmValue("IBM Approval");
                                    bool isNotValid = string.IsNullOrEmpty(validationParmValue) ? true : (disposalViewModel.TOTAL_AMOUNT <= Convert.ToInt64(validationParmValue));
                                    if (isNotValid)
                                    {
                                        isContinue = false;
                                    }
                                }

                                if (isContinue)
                                {
                                    Employee_Details approvalAuthorityDetail = GetNextApprovalAuthorityDetail(pro_obj.STATUS_LEVEL, 0, disposalViewModel);
                                    if (approvalAuthorityDetail != null && !pending_his_List.Any(s => s.STATUS_LEVEL == pro_obj.STATUS_LEVEL))
                                    {
                                        pending_his_List.Add(new ApprovalHisViewModel
                                        {
                                            DISPOSALHEADERID = disposalViewModel.DISPOSALHEADERID,
                                            PROCESSSTATUSID = 0, //////// disposalViewModel.PROCESSSTATUSID,
                                            APPROVALHISTORYID = 0,
                                            APPROVALSTATUS = 0,
                                            STATUS_LEVEL = pro_obj.STATUS_LEVEL,
                                            DISPLAY_MSG = pro_obj.STATUSDESCRIPTION,
                                            ATTACHMENT = "",
                                            APPCODE = approvalAuthorityDetail._ECode,
                                            APPEMP_NAME = approvalAuthorityDetail._EName,
                                            REMARKS = "",
                                        });
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                if (pending_his_List.Count > 0)
                {
                    foreach (ApprovalHisViewModel _obj in pending_his_List)
                    {
                        if (!his_List.Any(t => t.APPROVALHISTORYID == _obj.APPROVALHISTORYID && t.STATUS_LEVEL == _obj.STATUS_LEVEL))
                        {
                            his_List.Add(_obj);
                        }
                    }
                }
            }
            return his_List.Where(t => t.APPCODE > 0 && !string.IsNullOrEmpty(t.APPEMP_NAME)).ToList();
        }

        public List<ApprovalHisViewModel> GetDefaultAuthority(AssetDisposalViewModel disposalViewModel)
        {
            List<ApprovalHisViewModel> AuthList = new List<ApprovalHisViewModel>();
            try
            {
                long? operationMappId = GetOperationMappId(disposalViewModel.Emp_Detail);
                UserApprovalAuthority Obj = _CommonRepo.CheckApprovalAuthorityForAsset(disposalViewModel.ADEMPCODE, operationMappId);
                if (Obj != null)
                {
                    if (Obj.DepartmentManager != 0 && Obj.DepartmentManager != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DepartmentManager));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 3,
                                STATUS_LEVEL = 2,
                                DISPLAY_MSG = "Pending at Dept. Mgr.",
                                APPCODE = Emp_Dtl._ECode,
                                APPEMP_NAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName + "[" + Emp_Dtl._ECode + "]",
                            });
                        }
                    }
                    if (Obj.Coordinator != 0 && Obj.Coordinator != null && Obj.IsSkip == 0)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.Coordinator));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 4,
                                STATUS_LEVEL = 3,
                                DISPLAY_MSG = "Pending at Coordinator",
                                APPCODE = Emp_Dtl._ECode,
                                APPEMP_NAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName + "[" + Emp_Dtl._ECode + "]",
                            });
                        }
                    }
                    if (Obj.DivisionHead != 0 && Obj.DivisionHead != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DivisionHead));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 5,
                                STATUS_LEVEL = 4,
                                DISPLAY_MSG = "Pending at Div. Head.",
                                APPCODE = Emp_Dtl._ECode,
                                APPEMP_NAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName + "[" + Emp_Dtl._ECode + "]",
                            });
                        }
                    }
                    if (Obj.EXECoordinator != 0 && Obj.EXECoordinator != null && Obj.IsSkipExe == 0)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.EXECoordinator));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 6,
                                STATUS_LEVEL = 5,
                                DISPLAY_MSG = "Pending at Exe. Coordinator",
                                APPCODE = Emp_Dtl._ECode,
                                APPEMP_NAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName + "[" + Emp_Dtl._ECode + "]",
                            });
                        }
                    }
                    if (Obj.OperationHead != 0 && Obj.OperationHead != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.OperationHead));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 8, // Changed from 7 to 8 by TTL SR95154 :: CR6236
                                STATUS_LEVEL = 6,
                                DISPLAY_MSG = "Pending at OP Head",
                                APPCODE = Emp_Dtl._ECode,
                                APPEMP_NAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName + "[" + Emp_Dtl._ECode + "]",
                            });
                        }
                    }

                    //// Add PPC & Qty Autority ////
                    string codeByOperationId = _AssetRepo.GetFnCodeByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)disposalViewModel.Emp_Detail._OpId);
                    if (disposalViewModel.ASSETTYPEID == 2 && (!string.IsNullOrEmpty(_AssetRepo.CheckValidationByFnCode(codeByOperationId, "PPC Approval"))))
                    {
                        Employee_Details approvalAuthorityDetail = GetNextApprovalAuthorityDetail(7, 0, disposalViewModel);
                        if (approvalAuthorityDetail != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 9, // Changed from 8 to 9 by TTL SR95154 :: CR6236
                                STATUS_LEVEL = 7,
                                DISPLAY_MSG = "Pending at PPC HO",
                                ATTACHMENT = "",
                                APPCODE = approvalAuthorityDetail._ECode,
                                APPEMP_NAME = approvalAuthorityDetail._EName,
                                REMARKS = "",
                            });
                        }
                    }

                    if (disposalViewModel.ASSETTYPEID == 2 && (!string.IsNullOrEmpty(_AssetRepo.CheckValidationByFnCode(codeByOperationId, "Plant Quality Approval"))))
                    {
                        Employee_Details approvalAuthorityDetail = GetNextApprovalAuthorityDetail(8, 0, disposalViewModel);
                        if (approvalAuthorityDetail != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 10, // Changed from 9 to 10 by TTL SR95154 :: CR6236
                                STATUS_LEVEL = 8,
                                DISPLAY_MSG = "Pending at Plant Quality",
                                ATTACHMENT = "",
                                APPCODE = approvalAuthorityDetail._ECode,
                                APPEMP_NAME = approvalAuthorityDetail._EName,
                                REMARKS = "",
                            });
                        }
                    }

                    // Plant PPC Authority - Added by TTL SR95154 :: CR6236
                    if (disposalViewModel.ASSETTYPEID == 2 && (!string.IsNullOrEmpty(_AssetRepo.CheckValidationByFnCode(codeByOperationId, "Plant PPC Approval"))))
                    {
                        Employee_Details approvalAuthorityDetail = GetNextApprovalAuthorityDetail(28, 0, disposalViewModel);
                        if (approvalAuthorityDetail != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 7,
                                STATUS_LEVEL = 28,
                                DISPLAY_MSG = "Pending at Plant PPC",
                                ATTACHMENT = "",
                                APPCODE = approvalAuthorityDetail._ECode,
                                APPEMP_NAME = approvalAuthorityDetail._EName,
                                REMARKS = "",
                            });
                        }
                    }
                    // End by TTL SR95154 :: CR6236

                    //// End ////

                    if (Obj.Director != 0 && Obj.Director != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.Director));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 11, // Changed from 10 to 11 by TTL SR95154 :: CR6236
                                STATUS_LEVEL = 9,
                                DISPLAY_MSG = "Pending at Director",
                                APPCODE = Emp_Dtl._ECode,
                                APPEMP_NAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName + "[" + Emp_Dtl._ECode + "]",
                            });
                        }
                    }
                    if (Obj.Director2 != 0 && Obj.Director2 != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.Director2));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new ApprovalHisViewModel
                            {
                                APPROVALSTATUS = 0,
                                SEQ_NUMBER = 12, // Changed from 11 to 12 by TTL SR95154 :: CR6236
                                STATUS_LEVEL = 24,
                                DISPLAY_MSG = "Pending at Director 2",
                                APPCODE = Emp_Dtl._ECode,
                                APPEMP_NAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName + "[" + Emp_Dtl._ECode + "]",
                            });
                        }
                    }
                }
            }
            catch
            {
                AuthList = new List<ApprovalHisViewModel>();
            }
            //return AuthList.OrderBy(m => m.STATUS_LEVEL).ToList(); // Commented by TTL SR95154 :: CR6236
            return AuthList.OrderBy(m => m.SEQ_NUMBER).ToList(); // Added by TTL SR95154 :: CR6236
        }

        public short UploadAttachmentByRequestor(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            return Status_Level > (short)0 ? this._AssetRepo.UploadAssetInvoice(ADVM, Status_Level) : Status_Level;
        }

        public List<AssetDisposalViewModel> GetRequestReport(long AssetType, long EmpCode, string FromDate, string ToDate, long loginUser)
        {
            return this._AssetRepo.GetRequestReport(AssetType, EmpCode, FromDate, ToDate, loginUser);
        }

        public short CancelRequest(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details, int? cancelledByAdmin)
        {
            short num = 0;
            AssetDisposalViewModel model = new AssetDisposalViewModel();
            if (cancelledByAdmin == 1) // Admin end
            {
                num = this._AssetRepo.CancelRequest(ADVM, cancelledByAdmin);
            }
            else //// User end
            {
                model = _AssetRepo.GetDetailById(ADVM.DISPOSALHEADERID);
                if (model != null)
                {
                    num = this._AssetRepo.CancelRequest(ADVM, cancelledByAdmin);
                    if (num == (short)1)
                    {

                        List<ApprovalHisViewModel> iList = (List<ApprovalHisViewModel>)model.AppHistoryList;
                        model.AppHistoryList = GetAppAuthorityList(model, iList, _Employee_Details);

                        SendEmailForCancelation(model);
                    }
                }
            }

            return num;
        }
        #endregion

        #region Maintenance Dept. Approval
        public List<AssetDisposalViewModel> GetApprovalListByMaintenanceDept(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListByMaintenanceDept(_Employee_Details._ECode);
        }

        public short RequestUpdateByMaintenanceDept(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByMaintenanceDept(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }

        #endregion

        #region Taxtion Approval
        public List<AssetDisposalViewModel> GetApprovalListByTaxtion(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListByTaxtion(_Employee_Details._ECode);
        }

        public short RequestUpdateByTaxtion(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByTaxtion(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }

        public List<AssetDisposalViewModel> GetApprovalHistory(long loginUser)
        {
            return this._AssetRepo.GetApprovalHistory(loginUser);
        }
        #endregion

        #region User Approval
        public List<AssetDisposalViewModel> GetApprovalListByUser(Employee_Details _Employee_Details)
        {
            List<short> levelForUserApproval = this.GetStatusLevelForUserApproval(_Employee_Details);
            long int64_1 = Convert.ToInt64((object)(!_Employee_Details._DepId.HasValue ? new long?(0L) : _Employee_Details._DepId));
            long int64_2 = Convert.ToInt64((object)(!_Employee_Details._DivId.HasValue ? new long?(0L) : _Employee_Details._DivId));
            long int64_3 = Convert.ToInt64((object)(!_Employee_Details._OpId.HasValue ? new long?(0L) : _Employee_Details._OpId));
            long int64_4 = Convert.ToInt64((object)(!_Employee_Details._SiteId.HasValue ? new long?(0L) : _Employee_Details._SiteId));
            List<UserApprovalAuthority> coordinatorList = this._CommonRepo.GetCoordinatorList();
            return this._AssetRepo.GetApprovalListByUser(levelForUserApproval, int64_1, int64_2, int64_3, int64_4, _Employee_Details._ECode, coordinatorList);
        }

        public short RequestUpdateByUserApproval(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByUserApproval(ADVM, Status_Level);
            if (num == (short)1)
            {
                if (ADVM.Approval_History.APPROVALSTATUS == 2) /// Sendback
                {
                    this.SendEmailForSendback(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
                }
                if (ADVM.Approval_History.APPROVALSTATUS == 1)
                {
                    this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
                }
                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                {
                    this.SendEmailForReject(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
                }
            }
            return num;
        }
        #endregion

        #region IC & IBM Approval
        public List<AssetDisposalViewModel> GetApprovalListByIC(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListByIC(_Employee_Details._ECode);
        }

        public short RequestUpdateByIC(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByIC(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }

        public List<AssetDisposalViewModel> GetApprovalListByIBM(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListByIBM(_Employee_Details._ECode);
        }

        public short RequestUpdateByIBM(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByIBM(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }
        #endregion

        // Added by TTL  SR95154 :: CR6236
        #region Plant PPC Approval
        public List<AssetDisposalViewModel> GetListForPlantPPCApproval(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetListForPlantPPCApproval(_Employee_Details._ECode, _Employee_Details.Plant_Id);
        }
        public short RequestUpdateByPlantPPC(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByPlantPPC(ADVM, Status_Level);
            if (num == (short)1)
            {
                if (ADVM.Approval_History.APPROVALSTATUS == 2) /// Sendback
                {
                    this.SendEmailForSendback(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
                }
                if (ADVM.Approval_History.APPROVALSTATUS == 1)
                {
                    this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
                }
                if (ADVM.Approval_History.APPROVALSTATUS == 3)
                {
                    this.SendEmailForReject(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
                }
            }
            return num;
        }
        #endregion
        // End by TTL  SR95154 :: CR6236

        #region Plant Mgf. & Quality Approval
        public AssetDisposalViewModel GetAssetRequestByMgf(long id, Employee_Details _Employee_Details)
        {
            AssetDisposalViewModel disposalViewModel = this._AssetRepo.GetAssetRequestByMgf(id);
            if (disposalViewModel != null)
            {
                ///// -- PPC Approval --/////
                disposalViewModel.C1F_APPROVEBY_NAME = disposalViewModel.DisposalDetailList[0].C1F_APPROVEBY == null ? "" : _CommonRepo.GetEmpDetailById((long)disposalViewModel.DisposalDetailList[0].C1F_APPROVEBY)._EName;
                disposalViewModel.C2F_APPROVEBY_NAME = disposalViewModel.DisposalDetailList[0].C2F_APPROVEBY == null ? "" : _CommonRepo.GetEmpDetailById((long)disposalViewModel.DisposalDetailList[0].C2F_APPROVEBY)._EName;
                disposalViewModel.C3F_APPROVEBY_NAME = disposalViewModel.DisposalDetailList[0].C3F_APPROVEBY == null ? "" : _CommonRepo.GetEmpDetailById((long)disposalViewModel.DisposalDetailList[0].C3F_APPROVEBY)._EName;
                disposalViewModel.C4F_APPROVEBY_NAME = disposalViewModel.DisposalDetailList[0].C4F_APPROVEBY == null ? "" : _CommonRepo.GetEmpDetailById((long)disposalViewModel.DisposalDetailList[0].C4F_APPROVEBY)._EName;

                this._retVal_tuple = this.SetStatusLevel(_Employee_Details, disposalViewModel);
                short SetApprovalLevel = this._retVal_tuple.Item1;
                if (SetApprovalLevel > (short)0)
                {
                    string str = this._AssetRepo.GetProcessStatusList().Where(p => (int)p.STATUS_LEVEL == (int)SetApprovalLevel).Select(s => s.STATUSDESCRIPTION).FirstOrDefault().ToString();
                    Employee_Details approvalAuthorityDetail = this.GetNextApprovalAuthorityDetail(this._retVal_tuple.Item1, this._retVal_tuple.Item2, disposalViewModel);
                    if (approvalAuthorityDetail != null)
                        disposalViewModel.ForwardedTo = str + " - " + approvalAuthorityDetail._EName;
                }
            }
            else
                disposalViewModel = new AssetDisposalViewModel();
            return disposalViewModel;
        }

        public List<AssetDisposalViewModel> GetApprovalListByPlanQty(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListByPlanQty(_Employee_Details._ECode);
        }

        public short RequestUpdateByQty(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByQty(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }

        public List<AssetDisposalViewModel> GetApprovalListByPlanMgf(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListByPlanMgf(_Employee_Details._ECode);
        }

        public short RequestUpdateByMgf(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByMgf(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }

        public short UpdatePlantApprovalRequiredByMgf(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            return this._AssetRepo.UpdatePlantApprovalRequiredByMgf(ADVM, _Employee_Details.Plant_Id);
        }

        public List<AssetDisposalViewModel> GetListForPlantApproval(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetListForPlantApproval(_Employee_Details._ECode, _Employee_Details.Plant_Id);
        }

        public short RequestUpdateByPlantApproval(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            return this._AssetRepo.RequestUpdateByPlantApproval(ADVM, _Employee_Details._ECode, _Employee_Details.Plant_Id);
        }
        #endregion

        #region Environment / Admin Approval
        public List<AssetDisposalViewModel> GetApprovalListByEnvironment(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListByEnvironment(_Employee_Details._ECode);
        }

        public short RequestUpdateByEnvironment(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByEnvironment(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }

        public List<AssetDisposalViewModel> GetApprovalListByAdmin(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListByAdmin(_Employee_Details._ECode);
        }

        public short RequestUpdateByAdmin(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateByAdmin(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }
        #endregion

        #region Security Approval
        public List<AssetDisposalViewModel> GetApprovalListBySecurity(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListBySecurity(_Employee_Details._ECode);
        }

        public short RequestUpdateBySecurity(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.RequestUpdateBySecurity(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }
        #endregion

        #region Upload Invoice
        public List<AssetDisposalViewModel> GetApprovalListForUploadInvoice(int Status_Level, long loginUser)
        {
            return this._AssetRepo.GetApprovalListForUploadInvoice(Status_Level, loginUser);
        }

        public short UploadAssetInvoice(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            if (Status_Level <= (short)0)
                return Status_Level;
            short num = this._AssetRepo.UploadAssetInvoice(ADVM, Status_Level);
            if (num == (short)1)
                this.SendEmailForApproval(this._retVal_tuple.Item1, this._retVal_tuple.Item2, ADVM, _Employee_Details);
            return num;
        }
        #endregion

        #region Asset Retirement
        public List<AssetDisposalViewModel> GetApprovalListByAssetRetirement(Employee_Details _Employee_Details)
        {
            return this._AssetRepo.GetApprovalListByAssetRetirement(_Employee_Details._ECode);
        }

        public short RequestUpdateByAssetRetirement(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            this._retVal_tuple = this.SetStatusLevel(_Employee_Details, ADVM);
            short Status_Level = this._retVal_tuple.Item1;
            return Status_Level > (short)0 ? this._AssetRepo.RequestUpdateByAssetRetirement(ADVM, Status_Level) : Status_Level;
        }
        #endregion

        #region Comman Function
        public List<short> GetStatusLevelForUserApproval(Employee_Details Employee_Details)
        {
            List<short> StatusLevelList = new List<short>();
            short num1 = 0;
            long? fnDesigId = Employee_Details._FnDesigId;
            long num2 = 2;
            if (fnDesigId.GetValueOrDefault() == num2 && fnDesigId.HasValue)
            {
                num1 = (short)2;
                StatusLevelList.Add(num1);
            }
            else
            {
                fnDesigId = Employee_Details._FnDesigId;
                long num3 = 3;
                if (fnDesigId.GetValueOrDefault() == num3 && fnDesigId.HasValue)
                {
                    num1 = (short)4;
                    StatusLevelList.Add(num1);
                }
                else
                {
                    fnDesigId = Employee_Details._FnDesigId;
                    long num4 = 4;
                    if (fnDesigId.GetValueOrDefault() == num4 && fnDesigId.HasValue)
                    {
                        num1 = (short)6; // Changes from 6 to 7 by TTL SR95154 :: CR6236
                        StatusLevelList.Add(num1);
                    }
                    if (Employee_Details.Designation == "Coordinator" && string.IsNullOrEmpty(Employee_Details.Functional_Designation_Id))
                    {
                        num1 = (short)3;
                        StatusLevelList.Add(num1);
                    }
                    if (Employee_Details.Designation == "Executive Coordinator" && string.IsNullOrEmpty(Employee_Details.Functional_Designation_Id))
                    {
                        num1 = (short)5;
                        StatusLevelList.Add(num1);
                    }
                    //if ((Employee_Details.Designation == "Senior Director" || Employee_Details.Designation == "Director"
                    //        || Employee_Details.Designation == "Gr.VP & Director") && string.IsNullOrEmpty(Employee_Details.Functional_Designation_Id))
                    //{
                    //    num1 = (short)9;
                    //    StatusLevelList.Add(num1);
                    //}
                    if ((Employee_Details.Designation == "Senior Director" || Employee_Details.Designation == "Director"
                            || Employee_Details.Designation == "Gr.VP & Director"))
                    {
                        StatusLevelList.Add((short)9);
                        StatusLevelList.Add((short)24);
                    }
                    if (Employee_Details.Designation == "Chief Production Officer & Director")
                    {
                        num1 = (short)10;
                        StatusLevelList.Add(num1);
                    }
                    //else if (Employee_Details.Designation == "President & CEO" && string.IsNullOrEmpty(Employee_Details.Functional_Designation_Id))
                    //    num1 = (short)10;
                }
            }
            StatusLevelList.Add((short)28); // Added by TTL SR95154 :: CR6236
            return StatusLevelList;
        }

        public Tuple<short, long> SetStatusLevel(Employee_Details _Employee_Details, AssetDisposalViewModel ADVM)
        {
            short num1 = 0; long num2 = 0;
            Tuple<short, long> tuple = new Tuple<short, long>(num1, num2);
            switch (ADVM.STATUS_LEVEL)
            {
                case 1:
                    tuple = this.SetUserApprovalStatus_Level(ADVM, _Employee_Details);
                    break;
                case 2:
                    tuple = this.SetUserApprovalStatus_Level(ADVM, _Employee_Details);
                    break;
                case 3:
                    tuple = this.SetUserApprovalStatus_Level(ADVM, _Employee_Details);
                    break;
                case 4:
                    tuple = this.SetUserApprovalStatus_Level(ADVM, _Employee_Details);
                    break;
                case 5:
                    tuple = this.SetUserApprovalStatus_Level(ADVM, _Employee_Details);
                    break;
                // Added by TTL :: SR95154
                case 28:
                    tuple = this.SetUserApprovalStatus_Level(ADVM, _Employee_Details);
                    break;
                // End by TTL :: SR95154
                case 6:
                    tuple = this.SetMgf_QtyStatus_Level(Convert.ToInt16(ADVM.ASSETTYPEID), ADVM, _Employee_Details);
                    break;
                case 7:
                    tuple = this.SetUserApprovalStatus_Level(ADVM, _Employee_Details);
                    break;
                case 8:
                    tuple = this.SetUserApprovalStatus_Level(ADVM, _Employee_Details);
                    break;
                case 9:
                    tuple = this.SetUserApprovalStatus_Level(ADVM, _Employee_Details); ////new Tuple<short, long>(this.SetEVP_CPOApproval(ADVM, _Employee_Details), num2);
                    break;
                case 24: //// Director 2
                    tuple = new Tuple<short, long>(this.SetEVP_CPOApproval(ADVM, _Employee_Details), num2);
                    break;
                case 10:
                    tuple = new Tuple<short, long>(this.SetICAssociateApproval(ADVM.STATUS_LEVEL), num2);
                    break;
                case 11:
                    tuple = new Tuple<short, long>(this.CheckIBMApproval(ADVM, Convert.ToInt16(ADVM.ASSETTYPEID), ADVM.STATUS_LEVEL), num2);
                    break;
                case 12:
                    if (ADVM.ISUPLOADINV_BYTAXATION == 1)
                        tuple = new Tuple<short, long>(this.CheckTaxationInvoiceUpload(ADVM.PLANTID, Convert.ToInt16(ADVM.ASSETTYPEID), ADVM.STATUS_LEVEL), num2);
                    else
                        tuple = new Tuple<short, long>(this.CheckEnvironmentApproval(ADVM.PLANTID, Convert.ToInt16(ADVM.ASSETTYPEID), ADVM.STATUS_LEVEL), num2);
                    break;
                case 13:
                    tuple = new Tuple<short, long>(Convert.ToInt16(16), num2);
                    break;
                case 14:
                    tuple = new Tuple<short, long>(Convert.ToInt16(17), num2);
                    break;
                case 15:
                    tuple = new Tuple<short, long>(Convert.ToInt16(18), num2);
                    break;
                case 16:
                    tuple = new Tuple<short, long>(Convert.ToInt16(18), num2);
                    break;
                case 17:
                    tuple = new Tuple<short, long>(Convert.ToInt16(18), num2);
                    break;
                case 18:
                    tuple = new Tuple<short, long>(Convert.ToInt16(19), num2);
                    break;
                case 19:
                    tuple = new Tuple<short, long>(Convert.ToInt16(20), num2);
                    break;
                case 22: ////--- 22 for Maintenance Dept. 
                    tuple = new Tuple<short, long>(this.SetTaxationApproval(ADVM.STATUS_LEVEL), num2);
                    break;
                case 25: ////--- 25 for Taxation Dept for invoice upload. 
                    tuple = new Tuple<short, long>(Convert.ToInt16(18), num2);
                    break;
            }
            return tuple;
        }

        public short SetTaxationApproval(short crnt_level)
        {
            return Convert.ToInt16((int)1);
        }

        public Tuple<short, long> SetUserApprovalStatus_Level(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            short num1 = 0; long num2 = 0;
            long? operationMappId;
            if (ADVM.Emp_Detail != null)
            {
                operationMappId = GetOperationMappId(ADVM.Emp_Detail);
            }
            else
            {
                AssetDisposalViewModel disposalViewModel = this._AssetRepo.GetAssetRequestById(ADVM.DISPOSALHEADERID);
                operationMappId = GetOperationMappId(disposalViewModel.Emp_Detail);
            }
            UserApprovalAuthority _UserApprovalAuthority = this._CommonRepo.CheckApprovalAuthorityForAsset(ADVM.ADEMPCODE, operationMappId);
            List<UserApprovalAuthority> coordinatorList = this._CommonRepo.GetCoordinatorList();
            if (_UserApprovalAuthority != null && coordinatorList.Count<UserApprovalAuthority>() > 0)
            {
                List<long?> nullableList = new List<long?>();
                long? divisionHead;
                if (ADVM.STATUS_LEVEL == (short)1)
                {
                    long? departmentManager = _UserApprovalAuthority.DepartmentManager;
                    long num3 = 0;
                    int num4;
                    if ((departmentManager.GetValueOrDefault() > num3 ? (departmentManager.HasValue ? 1 : 0) : 0) != 0)
                    {
                        departmentManager = _UserApprovalAuthority.DepartmentManager;
                        long adempcode = ADVM.ADEMPCODE;
                        if ((departmentManager.GetValueOrDefault() == adempcode ? (!departmentManager.HasValue ? 1 : 0) : 1) != 0)
                        {
                            departmentManager = _UserApprovalAuthority.DepartmentManager;
                            divisionHead = _UserApprovalAuthority.DivisionHead;
                            num4 = departmentManager.GetValueOrDefault() == divisionHead.GetValueOrDefault() ? (departmentManager.HasValue != divisionHead.HasValue ? 1 : 0) : 1;
                            goto label_6;
                        }
                    }
                    num4 = 0;
                label_6:
                    if (num4 != 0)
                    {
                        num1 = (short)2;
                        num2 = Convert.ToInt64((object)_UserApprovalAuthority.DepartmentManager);
                    }
                    else
                        ++ADVM.STATUS_LEVEL;
                }
                if (ADVM.STATUS_LEVEL == (short)2)
                {
                    List<long?> list = coordinatorList.Where(y =>
                    {
                        long? coordinator = y.Coordinator;
                        long? nullable = !_UserApprovalAuthority.Coordinator.HasValue ? new long?(0L) : _UserApprovalAuthority.Coordinator;
                        return coordinator.GetValueOrDefault() == nullable.GetValueOrDefault() && coordinator.HasValue == nullable.HasValue && y.IsSkip == 0;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                    if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains((long)_UserApprovalAuthority.OperationId))
                    {
                        num1 = (short)3;
                        num2 = Convert.ToInt64((object)_UserApprovalAuthority.Coordinator);
                    }
                    else
                        ++ADVM.STATUS_LEVEL;
                }
                if (ADVM.STATUS_LEVEL == (short)3)
                {
                    divisionHead = _UserApprovalAuthority.DivisionHead;
                    long num3 = 0;
                    int num4;
                    if ((divisionHead.GetValueOrDefault() > num3 ? (divisionHead.HasValue ? 1 : 0) : 0) != 0)
                    {
                        divisionHead = _UserApprovalAuthority.DivisionHead;
                        long adempcode = ADVM.ADEMPCODE;
                        num4 = divisionHead.GetValueOrDefault() == adempcode ? (!divisionHead.HasValue ? 1 : 0) : 1;
                    }
                    else
                        num4 = 0;
                    if (num4 != 0)
                    {
                        num1 = (short)4;
                        num2 = Convert.ToInt64((object)_UserApprovalAuthority.DivisionHead);
                    }
                    else
                        ++ADVM.STATUS_LEVEL;
                }
                if (ADVM.STATUS_LEVEL == (short)4)
                {
                    //List<long?> list = coordinatorList.Where<UserApprovalAuthority>((Func<UserApprovalAuthority, bool>)(y =>
                    //{
                    //    long? exeCoordinator = y.EXECoordinator;
                    //    long? nullable = !_UserApprovalAuthority.EXECoordinator.HasValue ? new long?(0L) : _UserApprovalAuthority.EXECoordinator;
                    //    return exeCoordinator.GetValueOrDefault() == nullable.GetValueOrDefault() && exeCoordinator.HasValue == nullable.HasValue;
                    //})).Select<UserApprovalAuthority, long?>((Func<UserApprovalAuthority, long?>)(s => s.OrgLevel)).ToList<long?>();
                    //if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(_UserApprovalAuthority.OperationId))
                    ////   long? operationMappId = GetOperationMappId(_Employee_Details);
                    long? exeCoordinator = _AssetRepo.GetexecoordinatorByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_Employee_Details._OpId);
                    if (!string.IsNullOrEmpty(exeCoordinator.ToString()))
                    {
                        num1 = (short)5;
                        num2 = Convert.ToInt64(exeCoordinator);
                    }
                    else
                        ++ADVM.STATUS_LEVEL;
                }
                if (ADVM.STATUS_LEVEL == (short)5)
                {
                    List<long?> list = coordinatorList.Where(y =>
                    {
                        long? operationHead = y.OperationHead;
                        long? nullable = !_UserApprovalAuthority.OperationHead.HasValue ? new long?(0L) : _UserApprovalAuthority.OperationHead;
                        return operationHead.GetValueOrDefault() == nullable.GetValueOrDefault() && operationHead.HasValue == nullable.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                    if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
                    {
                        //num1 = (short)28; // Changes from 6 to 28 by TTL  SR95154 :: CR6236
                        //num2 = Convert.ToInt64((object)_UserApprovalAuthority.OperationHead);
                        //Get Op Id and Site - CR6754 - Start
                        Employee_Details empDetail = _AssetRepo.GetEmpDetailById(ADVM.ADEMPCODE);
                        //Changed by TTL on 27-Sep-2025 against SR109793 > CR7227
                        //if (IsPlantPpcMapped(empDetail._OpId ?? 0, empDetail._SiteId ?? 0, ADVM.ADEMPCODE))
                        //if (IsPlantPpcMapped((operationMappId ?? empDetail._OpId) ?? 0, ADVM.PLANTID ?? 0, ADVM.ADEMPCODE))
                        //Changed by TTL on 18-10-2025 against CR7306
                        if (IsPlantPpcMapped((operationMappId == null || operationMappId == 0 ? empDetail._OpId : operationMappId) ?? 0, (ADVM.PLANTID == null ? empDetail._SiteId : ADVM.PLANTID) ?? 0, ADVM.ADEMPCODE))
                        {
                            num1 = (short)28;
                            num2 = 0;
                        }
                        else
                        {
                            num1 = (short)6; // Changes from 6 to 28 by TTL  SR95154 :: CR6236
                            num2 = Convert.ToInt64((object)_UserApprovalAuthority.OperationHead);  // Changed to 0 by TTL :: CR6754
                        }
                        //CR6754 - End
                    }
                    else
                        ++ADVM.STATUS_LEVEL;
                }

                //Added by TTL SR95154 :: CR6236 for STATUS_LEVEL 28 - Plant PPC
                if (ADVM.STATUS_LEVEL == (short)28)
                {
                    List<long?> list = coordinatorList.Where(y =>
                    {
                        long? operationHead = y.OperationHead;
                        long? nullable = !_UserApprovalAuthority.OperationHead.HasValue ? new long?(0L) : _UserApprovalAuthority.OperationHead;
                        return operationHead.GetValueOrDefault() == nullable.GetValueOrDefault() && operationHead.HasValue == nullable.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                    if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
                    {
                        num1 = (short)6;
                        num2 = Convert.ToInt64((object)_UserApprovalAuthority.OperationHead);
                    }
                    else
                        ++ADVM.STATUS_LEVEL;
                }
                // End by TTL SR95154 :: CR6236

                if (ADVM.STATUS_LEVEL == (short)6)
                {
                    List<long?> list = coordinatorList.Where(y =>
                    {
                        long? director = y.Director;
                        long? nullable = !_UserApprovalAuthority.Director.HasValue ? new long?(0L) : _UserApprovalAuthority.Director;
                        return director.GetValueOrDefault() == nullable.GetValueOrDefault() && director.HasValue == nullable.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                    if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
                    {
                        num1 = (short)9;
                        num2 = Convert.ToInt64((object)_UserApprovalAuthority.Director);
                    }
                    else
                    {
                        num1 = this.SetEVP_CPOApproval(ADVM, _Employee_Details);
                        num2 = 0L;
                    }
                }
                if (ADVM.STATUS_LEVEL == (short)7)
                {
                    List<long?> list = coordinatorList.Where(y =>
                    {
                        long? director = y.Director;
                        long? nullable = !_UserApprovalAuthority.Director.HasValue ? new long?(0L) : _UserApprovalAuthority.Director;
                        return director.GetValueOrDefault() == nullable.GetValueOrDefault() && director.HasValue == nullable.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                    if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
                    {
                        num1 = (short)9;
                        num2 = Convert.ToInt64((object)_UserApprovalAuthority.Director);
                    }
                    else
                    {
                        num1 = this.SetEVP_CPOApproval(ADVM, _Employee_Details);
                        num2 = 0L;
                    }
                }
                if (ADVM.STATUS_LEVEL == (short)8)
                {
                    List<long?> list = coordinatorList.Where(y =>
                    {
                        long? director = y.Director;
                        long? nullable = !_UserApprovalAuthority.Director.HasValue ? new long?(0L) : _UserApprovalAuthority.Director;
                        return director.GetValueOrDefault() == nullable.GetValueOrDefault() && director.HasValue == nullable.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                    if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
                    {
                        num1 = (short)9;
                        num2 = Convert.ToInt64((object)_UserApprovalAuthority.Director);
                    }
                    else
                    {
                        num1 = this.SetEVP_CPOApproval(ADVM, _Employee_Details);
                        num2 = 0L;
                    }
                }

                if (ADVM.STATUS_LEVEL == (short)9) //// Director
                {
                    List<long?> list = coordinatorList.Where(y =>
                    {
                        long? director2 = y.Director2;
                        long? nullable = !_UserApprovalAuthority.Director2.HasValue ? new long?(0L) : _UserApprovalAuthority.Director2;
                        return director2.GetValueOrDefault() == nullable.GetValueOrDefault() && director2.HasValue == nullable.HasValue;
                    }).Select(s => s.OrgLevel).ToList<long?>();
                    if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
                    {
                        num1 = (short)24;
                        num2 = Convert.ToInt64((object)_UserApprovalAuthority.Director2);
                    }
                    else
                    {
                        num1 = this.SetEVP_CPOApproval(ADVM, _Employee_Details);
                        num2 = 0L;
                    }
                }
            }
            else
            {
                num1 = (short)-2;
                num2 = 0L;
            }
            return new Tuple<short, long>(num1, num2);
        }

        ////// ----  old pc code ------ //
        //public Tuple<short, long> SetUserApprovalStatus_Level(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        //{
        //    short num1 = 0;
        //    long num2 = 0;
        //    UserApprovalAuthority _UserApprovalAuthority = this._CommonRepo.CheckApprovalAuthority(ADVM.ADEMPCODE);
        //    List<UserApprovalAuthority> coordinatorList = this._CommonRepo.GetCoordinatorList();
        //    long? operationMappId;
        //    if (ADVM.Emp_Detail != null)
        //    {
        //        operationMappId = GetOperationMappId(ADVM.Emp_Detail);
        //    }
        //    else
        //    {
        //        AssetDisposalViewModel disposalViewModel = this._AssetRepo.GetAssetRequestById(ADVM.DISPOSALHEADERID);
        //        operationMappId = GetOperationMappId(disposalViewModel.Emp_Detail);
        //    }

        //    if (_UserApprovalAuthority != null && coordinatorList.Count<UserApprovalAuthority>() > 0)
        //    {
        //        List<long?> nullableList = new List<long?>();
        //        long? divisionHead;
        //        if (ADVM.STATUS_LEVEL == (short)1)
        //        {
        //            long? departmentManager = _UserApprovalAuthority.DepartmentManager;
        //            long num3 = 0;
        //            int num4;
        //            if ((departmentManager.GetValueOrDefault() > num3 ? (departmentManager.HasValue ? 1 : 0) : 0) != 0)
        //            {
        //                departmentManager = _UserApprovalAuthority.DepartmentManager;
        //                long adempcode = ADVM.ADEMPCODE;
        //                if ((departmentManager.GetValueOrDefault() == adempcode ? (!departmentManager.HasValue ? 1 : 0) : 1) != 0)
        //                {
        //                    departmentManager = _UserApprovalAuthority.DepartmentManager;
        //                    divisionHead = _UserApprovalAuthority.DivisionHead;
        //                    num4 = departmentManager.GetValueOrDefault() == divisionHead.GetValueOrDefault() ? (departmentManager.HasValue != divisionHead.HasValue ? 1 : 0) : 1;
        //                    goto label_6;
        //                }
        //            }
        //            num4 = 0;
        //        label_6:
        //            if (num4 != 0)
        //            {
        //                num1 = (short)2;
        //                num2 = Convert.ToInt64((object)_UserApprovalAuthority.DepartmentManager);
        //            }
        //            else
        //                ++ADVM.STATUS_LEVEL;
        //        }
        //        if (ADVM.STATUS_LEVEL == (short)2)
        //        {
        //            List<long?> list = coordinatorList.Where<UserApprovalAuthority>((Func<UserApprovalAuthority, bool>)(y =>
        //            {
        //                long? coordinator = y.Coordinator;
        //                long? nullable = !_UserApprovalAuthority.Coordinator.HasValue ? new long?(0L) : _UserApprovalAuthority.Coordinator;
        //                return coordinator.GetValueOrDefault() == nullable.GetValueOrDefault() && coordinator.HasValue == nullable.HasValue && y.IsSkip == 0;
        //            })).Select<UserApprovalAuthority, long?>((Func<UserApprovalAuthority, long?>)(s => s.OrgLevel)).ToList<long?>();
        //            if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(_UserApprovalAuthority.OperationId))
        //            {
        //                num1 = (short)3;
        //                num2 = Convert.ToInt64((object)_UserApprovalAuthority.Coordinator);
        //            }
        //            else
        //                ++ADVM.STATUS_LEVEL;
        //        }
        //        if (ADVM.STATUS_LEVEL == (short)3)
        //        {
        //            divisionHead = _UserApprovalAuthority.DivisionHead;
        //            long num3 = 0;
        //            int num4;
        //            if ((divisionHead.GetValueOrDefault() > num3 ? (divisionHead.HasValue ? 1 : 0) : 0) != 0)
        //            {
        //                divisionHead = _UserApprovalAuthority.DivisionHead;
        //                long adempcode = ADVM.ADEMPCODE;
        //                num4 = divisionHead.GetValueOrDefault() == adempcode ? (!divisionHead.HasValue ? 1 : 0) : 1;
        //            }
        //            else
        //                num4 = 0;
        //            if (num4 != 0)
        //            {
        //                num1 = (short)4;
        //                num2 = Convert.ToInt64((object)_UserApprovalAuthority.DivisionHead);
        //            }
        //            else
        //                ++ADVM.STATUS_LEVEL;
        //        }
        //        if (ADVM.STATUS_LEVEL == (short)4)
        //        {
        //            //List<long?> list = coordinatorList.Where<UserApprovalAuthority>((Func<UserApprovalAuthority, bool>)(y =>
        //            //{
        //            //    long? exeCoordinator = y.EXECoordinator;
        //            //    long? nullable = !_UserApprovalAuthority.EXECoordinator.HasValue ? new long?(0L) : _UserApprovalAuthority.EXECoordinator;
        //            //    return exeCoordinator.GetValueOrDefault() == nullable.GetValueOrDefault() && exeCoordinator.HasValue == nullable.HasValue;
        //            //})).Select<UserApprovalAuthority, long?>((Func<UserApprovalAuthority, long?>)(s => s.OrgLevel)).ToList<long?>();
        //            //if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(_UserApprovalAuthority.OperationId))

        //            //long? operationMappId = GetOperationMappId(_Employee_Details);
        //            long? exeCoordinator = _AssetRepo.GetexecoordinatorByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)ADVM.Emp_Detail._OpId);
        //            if (!string.IsNullOrEmpty(exeCoordinator.ToString()))
        //            {
        //                num1 = (short)5;
        //                num2 = Convert.ToInt64(exeCoordinator);
        //            }
        //            else
        //                ++ADVM.STATUS_LEVEL;
        //        }
        //        if (ADVM.STATUS_LEVEL == (short)5)
        //        {
        //            List<long?> list = coordinatorList.Where<UserApprovalAuthority>((Func<UserApprovalAuthority, bool>)(y =>
        //            {
        //                long? operationHead = y.OperationHead;
        //                long? nullable = !_UserApprovalAuthority.OperationHead.HasValue ? new long?(0L) : _UserApprovalAuthority.OperationHead;
        //                return operationHead.GetValueOrDefault() == nullable.GetValueOrDefault() && operationHead.HasValue == nullable.HasValue;
        //            })).Select<UserApprovalAuthority, long?>((Func<UserApprovalAuthority, long?>)(s => s.OrgLevel)).ToList<long?>();
        //            if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
        //            {
        //                num1 = (short)6;
        //                num2 = Convert.ToInt64((object)_UserApprovalAuthority.OperationHead);
        //                //num2 = Convert.ToInt64(opObj.OperationHead);
        //            }
        //            else
        //                ++ADVM.STATUS_LEVEL;
        //        }
        //        if (ADVM.STATUS_LEVEL == (short)6)
        //        {
        //            List<long?> list = coordinatorList.Where<UserApprovalAuthority>((Func<UserApprovalAuthority, bool>)(y =>
        //            {
        //                long? director = y.Director;
        //                long? nullable = !_UserApprovalAuthority.Director.HasValue ? new long?(0L) : _UserApprovalAuthority.Director;
        //                return director.GetValueOrDefault() == nullable.GetValueOrDefault() && director.HasValue == nullable.HasValue;
        //            })).Select<UserApprovalAuthority, long?>((Func<UserApprovalAuthority, long?>)(s => s.OrgLevel)).ToList<long?>();
        //            if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
        //            {
        //                num1 = (short)9;
        //                num2 = Convert.ToInt64((object)_UserApprovalAuthority.Director);
        //            }
        //            else
        //            {
        //                num1 = this.SetEVP_CPOApproval(ADVM, _Employee_Details);
        //                num2 = 0L;
        //            }
        //        }
        //        if (ADVM.STATUS_LEVEL == (short)7)
        //        {
        //            List<long?> list = coordinatorList.Where<UserApprovalAuthority>((Func<UserApprovalAuthority, bool>)(y =>
        //            {
        //                long? director = y.Director;
        //                long? nullable = !_UserApprovalAuthority.Director.HasValue ? new long?(0L) : _UserApprovalAuthority.Director;
        //                return director.GetValueOrDefault() == nullable.GetValueOrDefault() && director.HasValue == nullable.HasValue;
        //            })).Select<UserApprovalAuthority, long?>((Func<UserApprovalAuthority, long?>)(s => s.OrgLevel)).ToList<long?>();
        //            if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
        //            {
        //                num1 = (short)9;
        //                num2 = Convert.ToInt64((object)_UserApprovalAuthority.Director);
        //            }
        //            else
        //            {
        //                num1 = this.SetEVP_CPOApproval(ADVM, _Employee_Details);
        //                num2 = 0L;
        //            }
        //        }
        //        if (ADVM.STATUS_LEVEL == (short)8)
        //        {
        //            List<long?> list = coordinatorList.Where<UserApprovalAuthority>((Func<UserApprovalAuthority, bool>)(y =>
        //            {
        //                long? director = y.Director;
        //                long? nullable = !_UserApprovalAuthority.Director.HasValue ? new long?(0L) : _UserApprovalAuthority.Director;
        //                return director.GetValueOrDefault() == nullable.GetValueOrDefault() && director.HasValue == nullable.HasValue;
        //            })).Select<UserApprovalAuthority, long?>((Func<UserApprovalAuthority, long?>)(s => s.OrgLevel)).ToList<long?>();
        //            if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
        //            {
        //                num1 = (short)9;
        //                num2 = Convert.ToInt64((object)_UserApprovalAuthority.Director);
        //            }
        //            else
        //            {
        //                num1 = this.SetEVP_CPOApproval(ADVM, _Employee_Details);
        //                num2 = 0L;
        //            }
        //        }

        //        if (ADVM.STATUS_LEVEL == (short)9) //// Director
        //        {
        //            List<long?> list = coordinatorList.Where<UserApprovalAuthority>((Func<UserApprovalAuthority, bool>)(y =>
        //            {
        //                long? director2 = y.Director2;
        //                long? nullable = !_UserApprovalAuthority.Director2.HasValue ? new long?(0L) : _UserApprovalAuthority.Director2;
        //                return director2.GetValueOrDefault() == nullable.GetValueOrDefault() && director2.HasValue == nullable.HasValue;
        //            })).Select<UserApprovalAuthority, long?>((Func<UserApprovalAuthority, long?>)(s => s.OrgLevel)).ToList<long?>();
        //            if (list.Contains(_UserApprovalAuthority.DivisionId) || list.Contains(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_UserApprovalAuthority.OperationId))
        //            {
        //                num1 = (short)24;
        //                num2 = Convert.ToInt64((object)_UserApprovalAuthority.Director2);
        //            }
        //            else
        //            {
        //                num1 = this.SetEVP_CPOApproval(ADVM, _Employee_Details);
        //                num2 = 0L;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        num1 = (short)-2;
        //        num2 = 0L;
        //    }
        //    return new Tuple<short, long>(num1, num2);
        //}

        public Tuple<short, long> SetMgf_QtyStatus_Level(short assetType, AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            short num1 = 0; long num2 = 0;
            Tuple<short, long> tuple = new Tuple<short, long>(num1, num2);
            string codeByOperationId = this._AssetRepo.GetFnCodeByOperationId(_Employee_Details._OpId.Value);
            return string.IsNullOrEmpty(codeByOperationId)
                    ? new Tuple<short, long>((short)-2, num2) : (string.IsNullOrEmpty(this._AssetRepo.CheckValidationByFnCode(codeByOperationId, "PPC Approval"))
                    ? (string.IsNullOrEmpty(this._AssetRepo.CheckValidationByFnCode(codeByOperationId, "Plant Quality Approval"))
                    ? this.SetUserApprovalStatus_Level(ADVM, _Employee_Details) : (assetType != (short)2
                    ? this.SetUserApprovalStatus_Level(ADVM, _Employee_Details) : new Tuple<short, long>((short)8, num2))) : (assetType != (short)2
                    ? this.SetUserApprovalStatus_Level(ADVM, _Employee_Details) : new Tuple<short, long>((short)7, num2)));
        }

        public short SetEVP_CPOApproval(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details)
        {
            if (ADVM.Emp_Detail == null)
            {
                long siteIdByEmpCode = this._CommonRepo.GetSiteIdByEmpCode(ADVM.ADEMPCODE);
                ADVM.Emp_Detail = new Employee_Details()
                {
                    _SiteId = new long?(siteIdByEmpCode)
                };
            }
            short num;
            if (this._AssetRepo.GetApprovalAuthorityBySiteId(Convert.ToInt64(ADVM.Emp_Detail._SiteId == null ? 0 : ADVM.Emp_Detail._SiteId), Convert.ToInt16(10)) != null) //// --- 10 for CPO approval
            {
                //Change by Alok Singh
                long? operationMappId = GetOperationMappId(ADVM.Emp_Detail);
                string codeByOperationId = this._AssetRepo.GetFnCodeByOperationId((operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)ADVM.Emp_Detail._OpId));
                if (!string.IsNullOrEmpty(codeByOperationId))
                {
                    string validationParmValue = this._AssetRepo.GetValidationParmValue("CPO Approval");
                    num = string.IsNullOrEmpty(validationParmValue) || !validationParmValue.Contains(Convert.ToString(codeByOperationId)) ? (short)11 : Convert.ToInt16(10); //// --- 11 for IC Associate Approval
                }
                else
                    num = (short)-2;
            }
            else
                num = (short)-2;
            return num;
        }

        public short SetICAssociateApproval(short crnt_level)
        {
            return Convert.ToInt16((int)crnt_level + 1);
        }

        public short CheckIBMApproval(AssetDisposalViewModel ADVM, short assetType, short crnt_level)
        {
            if (ADVM.IsReVerify_TotalAmt == 1)//// Re-verify total amount validation
            {
                ADVM.TOTAL_AMOUNT = (long)ADVM.DisposalDetailList.Where(w => w.APPROVAL_STATUS == 1).Max(s => s.ORIGINALCOST).Value;
            }
            string validationParmValue = this._AssetRepo.GetValidationParmValue("IBM Approval");
            return string.IsNullOrEmpty(validationParmValue)
                    ? (short)-2 : (ADVM.TOTAL_AMOUNT <= Convert.ToInt64(validationParmValue)
                    ? ADVM.ISUPLOADINV_BYTAXATION == 1
                    ? this.CheckTaxationInvoiceUpload(ADVM.PLANTID, assetType, Convert.ToInt16(crnt_level + 1))
                    : this.CheckEnvironmentApproval(ADVM.PLANTID, assetType, Convert.ToInt16(crnt_level + 1)) : Convert.ToInt16(crnt_level + 1));
        }

        public short CheckEnvironmentApproval(long? PLANTID, short assetType, short crnt_level)
        {
            string validationParmValue = this._AssetRepo.GetValidationParmValue("Environment Approval");
            //long siteIdByEmpCode = this._CommonRepo.GetSiteIdByEmpCode(EmpCode);
            return string.IsNullOrEmpty(validationParmValue)
                    || !validationParmValue.Contains(Convert.ToString(PLANTID))
                    ? this.CheckAdminApproval(PLANTID, assetType, Convert.ToInt16(13)) : (assetType != (short)2
                    ? (short)15 : Convert.ToInt16(13));
        }

        public short CheckTaxationInvoiceUpload(long? PLANTID, short assetType, short crnt_level)
        {
            string validationParmValue = this._AssetRepo.GetValidationParmValue("Invoice Upload By Taxation");
            //long siteIdByEmpCode = this._CommonRepo.GetSiteIdByEmpCode(EmpCode);
            return string.IsNullOrEmpty(validationParmValue)
                    || !validationParmValue.Contains(Convert.ToString(PLANTID))
                    ? this.CheckAdminApproval(PLANTID, assetType, Convert.ToInt16(13)) : (assetType != (short)2
                    ? (short)15 : Convert.ToInt16(25));
        }

        public short CheckAdminApproval(long? PLANTID, short assetType, short crnt_level)
        {
            string validationParmValue = this._AssetRepo.GetValidationParmValue("Admin Approval");
            //long siteIdByEmpCode = this._CommonRepo.GetSiteIdByEmpCode(EmpCode);
            return string.IsNullOrEmpty(validationParmValue)
                    || !validationParmValue.Contains(Convert.ToString(PLANTID))
                    ? (short)-2 : (assetType != (short)2
                    ? (short)15 : Convert.ToInt16((int)crnt_level + 1));
        }

        public Employee_Details GetNextApprovalAuthorityDetail(short _StatusLevel, long _EmpCode, AssetDisposalViewModel _ADVM)
        {
            short[] LEVEL_ARRAY = { 1, 7, 8, 13, 14, 15, 16, 17, 18, 19, 22, 23, 25}; //Added 28 by TTL :: SR95154 | CR6236  // Changed by TTL :: CR6754 (Removed 28)
            Employee_Details employeeDetails = new Employee_Details();
            if (LEVEL_ARRAY.Contains(_StatusLevel))
            {
                ApprovalAuthorityViewModel authorityBySiteId = this._AssetRepo.GetApprovalAuthorityBySiteId(Convert.ToInt64(_ADVM.PLANTID), _StatusLevel);
                if (authorityBySiteId != null)
                {
                    employeeDetails._ECode = authorityBySiteId.EMPCODE;
                    employeeDetails._EFirstName = authorityBySiteId.EMPFNAME;
                    employeeDetails._ELastName = authorityBySiteId.EMPLNAME;
                    employeeDetails._EName = authorityBySiteId.EMPNAME;
                    employeeDetails._EmailId = authorityBySiteId.EMPEMAIL;
                }
            }
            else
            {
                if (_EmpCode > 0L)
                {
                    employeeDetails = this._CommonRepo.GetEmpDetailById(_EmpCode);
                }
                else
                {
                    if (_ADVM.Emp_Detail == null)
                    {
                        long siteIdByEmpCode = this._CommonRepo.GetSiteIdByEmpCode(_ADVM.ADEMPCODE);
                        _ADVM.Emp_Detail = new Employee_Details()
                        {
                            _SiteId = new long?(siteIdByEmpCode)
                        };
                    }

                    //Commented by TTL :: CR6754
                    //ApprovalAuthorityViewModel authorityBySiteId = this._AssetRepo.GetApprovalAuthorityBySiteId(Convert.ToInt64((object)(!_ADVM.Emp_Detail._SiteId.HasValue ? new long?(0L) : _ADVM.Emp_Detail._SiteId)), _StatusLevel);
                    //if (authorityBySiteId != null)
                    //{
                    //    employeeDetails._ECode = authorityBySiteId.EMPCODE;
                    //    employeeDetails._EFirstName = authorityBySiteId.EMPFNAME;
                    //    employeeDetails._ELastName = authorityBySiteId.EMPLNAME;
                    //    employeeDetails._EName = authorityBySiteId.EMPNAME;
                    //    employeeDetails._EmailId = authorityBySiteId.EMPEMAIL;
                    //}

                    //Added by TTL :: CR6754 - Start
                    if (_StatusLevel == 28)
                    {
                        long operationId = Convert.ToInt64((object)(!_ADVM.Emp_Detail._OpId.HasValue ? new long?(0L) : _ADVM.Emp_Detail._OpId));
                        long siteId = Convert.ToInt64((object)(!_ADVM.Emp_Detail._SiteId.HasValue ? new long?(0L) : _ADVM.Emp_Detail._SiteId));
                        ApprovalAuthorityViewModel authorityBySiteId = this._AssetRepo.GetApprovalAuthorityByOperationAndSiteId(operationId, siteId, _StatusLevel);
                        if (authorityBySiteId != null)
                        {
                            employeeDetails._ECode = authorityBySiteId.EMPCODE;
                            employeeDetails._EFirstName = authorityBySiteId.EMPFNAME;
                            employeeDetails._ELastName = authorityBySiteId.EMPLNAME;
                            employeeDetails._EName = authorityBySiteId.EMPNAME;
                            employeeDetails._EmailId = authorityBySiteId.EMPEMAIL;
                        }
                    }
                    else
                    { //Added by TTL :: CR6754 - End
                        ApprovalAuthorityViewModel authorityBySiteId = this._AssetRepo.GetApprovalAuthorityBySiteId(Convert.ToInt64((object)(!_ADVM.Emp_Detail._SiteId.HasValue ? new long?(0L) : _ADVM.Emp_Detail._SiteId)), _StatusLevel);
                        if (authorityBySiteId != null)
                        {
                            employeeDetails._ECode = authorityBySiteId.EMPCODE;
                            employeeDetails._EFirstName = authorityBySiteId.EMPFNAME;
                            employeeDetails._ELastName = authorityBySiteId.EMPLNAME;
                            employeeDetails._EName = authorityBySiteId.EMPNAME;
                            employeeDetails._EmailId = authorityBySiteId.EMPEMAIL;
                        }
                    }
                    //End by TTL :: CR6754
                }
            }
            return employeeDetails;
        }

        //Code Added by TTL :: CR6754
        public Employee_Details GetEmpDetailById(long _EmpCode)
        {
            Employee_Details employeeDetails = new Employee_Details();
            employeeDetails = _AssetRepo.GetEmpDetailById(_EmpCode);
            return employeeDetails;
        }

        public bool IsPlantPpcMapped(long OperationId, long SiteId, long EmpCode)
        {
            if(OperationId != 0 && SiteId != 0 && EmpCode != 0)
            {
                return _AssetRepo.IsPlantPpcMapped(OperationId, SiteId, EmpCode);
            }
            else
            {
                return false;
            }
        }
        //Code End by TTL :: CR6754

        public short SendEmailForApproval(short _StatusLevel, long _EmpCode, AssetDisposalViewModel _ADVM, Employee_Details _Employee_details)
        {
            short num = 0;
            try
            {
                if (_ADVM.DISPOSALHEADERID > 0)
                {
                    _ADVM = this._AssetRepo.GetAssetRequestById(_ADVM.DISPOSALHEADERID);
                    if (_ADVM.STATUS_LEVEL == 21) //// Request rejected
                    {
                        Employee_Details requestorDetail = _CommonRepo.GetEmpDetailById(_ADVM.ADEMPCODE);
                        if (!string.IsNullOrEmpty(requestorDetail._EmailId) && !string.IsNullOrEmpty(requestorDetail._EFirstName) && !string.IsNullOrEmpty(_ADVM.EMP_NAME))
                        {
                            commanEmail commanEmail = new commanEmail();
                            commanEmail.MailFrom = "portal.admin@honda.hmsi.in";
                            commanEmail.MailTo = requestorDetail._EmailId;
                            string str1 = "Asset Disposal Request Rejected by Approving Authority";
                            string str2 = string.Empty;

                            string rejectRemarks = _ADVM.AppHistoryList.OrderByDescending(o => o.APPROVALHISTORYID).Select(s => s.REMARKS).FirstOrDefault();

                            str2 = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Asset Disposal Request</b></div>"
                              + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                              + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + requestorDetail._EFirstName + " " + requestorDetail._ELastName + " San,</b><br/></td></tr>"
                              + "<tr><td colspan=2>&nbsp;</td></tr>"
                              + "<tr><td valign=top colspan=2>&nbsp &nbsp;<b>" + _Employee_details.Employee_First_Name + " " + _Employee_details.Employee_Last_Name + " San has rejected the Asset Disposal request in Employee Portal. Below are detail :-</b></td></tr>"
                              + "<tr><td colspan=2>&nbsp;</td></tr>"
                              + "<tr><td valign=top>&nbsp;&nbsp;Request ID:</td><td valign=top > " + _ADVM.DISPOSALHEADERID + "</td></tr>"
                              + "<tr><td valign=top>&nbsp;&nbsp;Request Type:</td><td valign=top > Asset " + _ADVM.ASSETTYPE + "</td></tr>"
                              + "<tr><td valign=top>&nbsp;&nbsp;Reject Remark:</td><td valign=top >" + rejectRemarks + "</td></tr>"
                               + "<tr><td colspan=2>&nbsp;</td></tr>"
                               + "<tr><td colspan=2>&nbsp;&nbsp;Please login in <a href=" + serverpath.getServerPath() + "/SSO > Employee Portal</a> to reprocess the request.</td></tr>"
                              + "<tr><td colspan=2><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                              + "<tr><td colspan=2>&nbsp;&nbsp;Eportal Team<br/><br/></td></tr>"
                              + "<tr><td colspan=2><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr><tr><td></td></tr></table> ";

                            commanEmail.MailSubject = str1;
                            commanEmail.MailBody = str2;
                            commanEmail.Send();
                            num = (short)1;
                        }
                    }
                    else
                    {
                        Employee_Details approvalAuthorityDetail = this.GetNextApprovalAuthorityDetail(_StatusLevel, _EmpCode, _ADVM);
                        //Below Code added by aumento for the SR70933-SaleScrapeEmail------------------------------
                        Employee_Details requestorDetail = _CommonRepo.GetEmpDetailById(_ADVM.ADEMPCODE);
                        if (approvalAuthorityDetail != null && !string.IsNullOrEmpty(requestorDetail._EmailId))
                        //if (approvalAuthorityDetail != null)
                        //-----------------------------------------------------------------------------------------
                        {
                            if (!string.IsNullOrEmpty(approvalAuthorityDetail.EMail_Id) && !string.IsNullOrEmpty(approvalAuthorityDetail.Employee_First_Name) && !string.IsNullOrEmpty(_ADVM.EMP_NAME))
                            {
                                commanEmail commanEmail = new commanEmail();
                                commanEmail.MailFrom = "portal.admin@honda.hmsi.in";
                                commanEmail.MailTo = approvalAuthorityDetail.EMail_Id;
                                //Below Code added by aumento for the SR70933-SaleScrapeEmail------------------------------
                                commanEmail.MailCc = requestorDetail.EMail_Id;
                                //-----------------------------------------------------------------------------------------

                                if (_ADVM.STATUS_LEVEL == 18) //// Gate Security
                                {
                                    Employee_Details nxtApprovalAuthority = this.GetNextApprovalAuthorityDetail(19, _EmpCode, _ADVM); //// 19 - Finance for Close
                                    if (nxtApprovalAuthority != null)
                                    {
                                        if (!string.IsNullOrEmpty(nxtApprovalAuthority.EMail_Id))
                                            commanEmail.MailCc = nxtApprovalAuthority.EMail_Id;
                                    }
                                }
                                string str1 = "Asset Disposal Request Approval";
                                string str2 = string.Empty;
                                short[] LEVEL_ARRAY = { 2, 3, 4, 5, 6, 9, 24, 10 }; //// Send mail only user authorities
                                if (LEVEL_ARRAY.Contains(_ADVM.STATUS_LEVEL))
                                {
                                    str2 = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Asset Disposal Request</b></div>"
                                      + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                                      + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + approvalAuthorityDetail.Employee_First_Name + " San,</b><br/></td></tr>"
                                      + "<tr><td colspan=2>&nbsp;</td></tr>"
                                      + "<tr><td valign=top colspan=2>&nbsp &nbsp;<b>" + _ADVM.EMP_NAME + " San has raised an Asset Disposal request in Employee Portal. Below are detail :-</b></td></tr>"
                                      + "<tr><td colspan=2>&nbsp;</td></tr>"
                                      + "<tr><td valign=top>&nbsp;&nbsp;Request ID:</td><td valign=top > " + _ADVM.DISPOSALHEADERID + "</td></tr>"
                                      + "<tr><td valign=top>&nbsp;&nbsp;Request Type:</td><td valign=top > Asset " + _ADVM.ASSETTYPE + "</td></tr>"
                                      + "<tr><td valign=top>&nbsp;&nbsp;Request Date:</td><td valign=top >" + _ADVM.DATEADDED + "</td></tr>"
                                      + "<tr><td valign=top>&nbsp;&nbsp;No. of items:</td><td valign=top >" + _ADVM.DisposalDetailList.Count() + "</td></tr>"
                                      + "<tr><td colspan=2>&nbsp;</td></tr>"
                                      + "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + approvalAuthorityDetail._ECode + "&C=AssetDisposal&A=updateUserApproval&Tid=" + _ADVM.DISPOSALHEADERID + " > Employee Portal</a> link to approve the request.</td></tr>"
                                      + "<tr><td colspan=2><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                                      + "<tr><td colspan=2>&nbsp;&nbsp;Eportal Team<br/><br/></td></tr>"
                                      + "<tr><td colspan=2><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr><tr><td></td></tr></table> ";
                                }
                                else
                                {
                                    str2 = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Asset Disposal Request</b></div>"
                                 + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                                 + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + approvalAuthorityDetail.Employee_First_Name + " San,</b><br/></td></tr>"
                                 + "<tr><td colspan=2>&nbsp;</td></tr>"
                                 + "<tr><td valign=top colspan=2>&nbsp &nbsp;<b>" + _ADVM.EMP_NAME + " San has raised an Asset Disposal request in Employee Portal. Below are detail :-</b></td></tr>"
                                 + "<tr><td colspan=2>&nbsp;</td></tr>"
                                 + "<tr><td valign=top>&nbsp;&nbsp;Request ID:</td><td valign=top > " + _ADVM.DISPOSALHEADERID + "</td></tr>"
                                 + "<tr><td valign=top>&nbsp;&nbsp;Request Type:</td><td valign=top > Asset " + _ADVM.ASSETTYPE + "</td></tr>"
                                 + "<tr><td valign=top>&nbsp;&nbsp;Request Date:</td><td valign=top >" + _ADVM.DATEADDED + "</td></tr>"
                                 + "<tr><td valign=top>&nbsp;&nbsp;No. of items:</td><td valign=top >" + _ADVM.DisposalDetailList.Count() + "</td></tr>"
                                 + "<tr><td colspan=2>&nbsp;</td></tr>"
                                 + "<tr><td colspan=2>&nbsp;&nbsp;Please login in <a href=" + serverpath.getServerPath() + "/SSO > Employee Portal</a> to approved the request.</td></tr>"
                                 + "<tr><td colspan=2><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                                 + "<tr><td colspan=2>&nbsp;&nbsp;Eportal Team<br/><br/></td></tr>"
                                 + "<tr><td colspan=2><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr><tr><td></td></tr></table> ";
                                }

                                commanEmail.MailSubject = str1;
                                commanEmail.MailBody = str2;
                                commanEmail.Send();
                                num = (short)1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                num = (short)-4;
            }
            return num;
        }
        public short SendEmailForReqApproval(short _StatusLevel, long _EmpCode, AssetDisposalViewModel _ADVM, Employee_Details _Employee_details)
        {
            short num = 0;
            try
            {
                //Below Code added by aumento for the SR70933-SaleScrapeEmail------------------------------
                _ADVM.PLANTID = long.Parse(_Employee_details.Plant_Id);
                //-----------------------------------------------------------------------------------------
                Employee_Details approvalAuthorityDetail = this.GetNextApprovalAuthorityDetail(_StatusLevel, _EmpCode, _ADVM);
                if (approvalAuthorityDetail != null)
                {
                    if (_ADVM.DISPOSALHEADERID > 0)
                    {
                        _ADVM = this._AssetRepo.GetAssetRequestById(_ADVM.DISPOSALHEADERID);
                        //Below Code added by aumento for the SR70933-SaleScrapeEmail------------------------------
                        Employee_Details requestorDetail = _CommonRepo.GetEmpDetailById(_ADVM.ADEMPCODE);
                        if (approvalAuthorityDetail != null && !string.IsNullOrEmpty(requestorDetail._EmailId))
                            //if (!string.IsNullOrEmpty(approvalAuthorityDetail.EMail_Id) && !string.IsNullOrEmpty(approvalAuthorityDetail.Employee_First_Name) && !string.IsNullOrEmpty(_ADVM.EMP_NAME))                               
                            if (!string.IsNullOrEmpty(approvalAuthorityDetail.EMail_Id) && !string.IsNullOrEmpty(approvalAuthorityDetail.Employee_First_Name) && !string.IsNullOrEmpty(_ADVM.EMP_NAME) && !string.IsNullOrEmpty(requestorDetail._EmailId))
                            //-----------------------------------------------------------------------------------------
                            {
                                commanEmail commanEmail = new commanEmail();
                                commanEmail.MailFrom = "portal.admin@honda.hmsi.in";
                                commanEmail.MailTo = approvalAuthorityDetail.EMail_Id;
                                //Below Code added by aumento for the SR70933-SaleScrapeEmail------------------------------
                                commanEmail.MailCc = requestorDetail.EMail_Id;
                                //-----------------------------------------------------------------------------------------
                                string str1 = "Asset Disposal Request Approval";
                                string str2 = string.Empty;

                                str2 = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Asset Disposal Request</b></div>"
                                   + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                                   + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + approvalAuthorityDetail.Employee_First_Name + " San,</b><br/></td></tr>"
                                   + "<tr><td colspan=2>&nbsp;</td></tr>"
                                   + "<tr><td valign=top colspan=2>&nbsp &nbsp;<b>Asset disposal request has been Submited by " + _ADVM.EMP_NAME + ". The detail are as follows:-</b></td></tr>"
                                   + "<tr><td colspan=2>&nbsp;</td></tr>"
                                   //+ "<tr><td valign=top width='20%'>&nbsp;&nbsp;Request By:</td><td valign=top >" + _ADVM.EMP_NAME + "</td></tr>"
                                   + "<tr><td valign=top>&nbsp;&nbsp;Request Type:</td><td valign=top Asset >" + _ADVM.ASSETTYPE + "</td></tr>"
                                   + "<tr><td valign=top>&nbsp;&nbsp;Request Date:</td><td valign=top >" + _ADVM.DATEADDED + "</td></tr>"
                                   + "<tr><td valign=top>&nbsp;&nbsp;No. of items:</td><td valign=top >" + _ADVM.DisposalDetailList.Count() + "</td></tr>"
                                   + "<tr><td valign=top>&nbsp;&nbsp;Approval Status:</td><td valign=top >" + _ADVM.PROCESSSTATUS + "</td></tr>"
                                   + "<tr><td colspan=2>&nbsp;</td></tr>"
                                   + "<tr><td colspan=2>&nbsp;&nbsp;Please login <a href=" + serverpath.getServerPath() + "/SSO > Employee Portal</a> for your approval.</td></tr>"
                                   + "<tr><td colspan=2><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                                   + "<tr><td colspan=2>&nbsp;&nbsp;Eportal Team<br/><br/></td></tr>"
                                   + "<tr><td colspan=2><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr><tr><td></td></tr></table> ";

                                commanEmail.MailSubject = str1;
                                commanEmail.MailBody = str2;
                                commanEmail.Send();
                                num = (short)1;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                num = (short)-4;
            }
            return num;
        }

        public short SendEmailForSendback(short _StatusLevel, long _EmpCode, AssetDisposalViewModel _ADVM, Employee_Details _Employee_details)
        {
            short num = 0;
            try
            {
                Employee_Details requestorDetail = _CommonRepo.GetEmpDetailById(_ADVM.ADEMPCODE); //this.GetNextApprovalAuthorityDetail(_StatusLevel, _EmpCode, _ADVM);
                if (requestorDetail != null)
                {
                    if (_ADVM.DISPOSALHEADERID > 0)
                    {
                        _ADVM = this._AssetRepo.GetAssetRequestById(_ADVM.DISPOSALHEADERID);
                        if (!string.IsNullOrEmpty(requestorDetail._EmailId) && !string.IsNullOrEmpty(requestorDetail._EFirstName) && !string.IsNullOrEmpty(_ADVM.EMP_NAME))
                        {
                            commanEmail commanEmail = new commanEmail();
                            commanEmail.MailFrom = "portal.admin@honda.hmsi.in";
                            commanEmail.MailTo = requestorDetail._EmailId;
                            string str1 = "Asset Disposal Request Send back by Approving Authority";
                            string str2 = string.Empty;

                            string senBackremarks = _ADVM.AppHistoryList.OrderByDescending(o => o.APPROVALHISTORYID).Select(s => s.REMARKS).FirstOrDefault();

                            str2 = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Asset Disposal Request</b></div>"
                              + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                              + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + requestorDetail._EFirstName + " " + requestorDetail._ELastName + " San,</b><br/></td></tr>"
                              + "<tr><td colspan=2>&nbsp;</td></tr>"
                              + "<tr><td valign=top colspan=2>&nbsp &nbsp;<b>" + _Employee_details.Employee_First_Name + " " + _Employee_details.Employee_Last_Name + " San has send back the Asset Disposal request in Employee Portal. Below are detail :-</b></td></tr>"
                              + "<tr><td colspan=2>&nbsp;</td></tr>"
                              + "<tr><td valign=top>&nbsp;&nbsp;Request ID:</td><td valign=top > " + _ADVM.DISPOSALHEADERID + "</td></tr>"
                              + "<tr><td valign=top>&nbsp;&nbsp;Request Type:</td><td valign=top > Asset " + _ADVM.ASSETTYPE + "</td></tr>"
                              + "<tr><td valign=top>&nbsp;&nbsp;Send back Remark:</td><td valign=top >" + senBackremarks + "</td></tr>"
                               + "<tr><td colspan=2>&nbsp;</td></tr>"
                              + "<tr><td colspan=2>&nbsp;&nbsp;Please login in <a href=" + serverpath.getServerPath() + "/SSO > Employee Portal</a> to reprocess the request.</td></tr>"
                              + "<tr><td colspan=2><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                              + "<tr><td colspan=2>&nbsp;&nbsp;Eportal Team<br/><br/></td></tr>"
                              + "<tr><td colspan=2><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr><tr><td></td></tr></table> ";


                            commanEmail.MailSubject = str1;
                            commanEmail.MailBody = str2;
                            commanEmail.Send();
                            num = (short)1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                num = (short)-4;
            }
            return num;
        }
        public short SendEmailForReject(short _StatusLevel, long _EmpCode, AssetDisposalViewModel _ADVM, Employee_Details _Employee_details)
        {
            short num = 0;
            try
            {
                Employee_Details requestorDetail = _CommonRepo.GetEmpDetailById(_ADVM.ADEMPCODE); //this.GetNextApprovalAuthorityDetail(_StatusLevel, _EmpCode, _ADVM);
                if (requestorDetail != null)
                {
                    if (_ADVM.DISPOSALHEADERID > 0)
                    {
                        _ADVM = this._AssetRepo.GetAssetRequestById(_ADVM.DISPOSALHEADERID);
                        if (!string.IsNullOrEmpty(requestorDetail._EmailId) && !string.IsNullOrEmpty(requestorDetail._EFirstName) && !string.IsNullOrEmpty(_ADVM.EMP_NAME))
                        {
                            commanEmail commanEmail = new commanEmail();
                            commanEmail.MailFrom = "portal.admin@honda.hmsi.in";
                            commanEmail.MailTo = requestorDetail._EmailId;
                            string str1 = "Asset Disposal Request Rejected by Approving Authority";
                            string str2 = string.Empty;

                            string rejectRemarks = _ADVM.AppHistoryList.OrderByDescending(o => o.APPROVALHISTORYID).Select(s => s.REMARKS).FirstOrDefault();

                            str2 = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Asset Disposal Request</b></div>"
                              + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                              + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + requestorDetail._EFirstName + " " + requestorDetail._ELastName + " San,</b><br/></td></tr>"
                              + "<tr><td colspan=2>&nbsp;</td></tr>"
                              + "<tr><td valign=top colspan=2>&nbsp &nbsp;<b>" + _Employee_details.Employee_First_Name + " " + _Employee_details.Employee_Last_Name + " San has rejected the Asset Disposal request in Employee Portal. Below are detail :-</b></td></tr>"
                              + "<tr><td colspan=2>&nbsp;</td></tr>"
                              + "<tr><td valign=top>&nbsp;&nbsp;Request ID:</td><td valign=top > " + _ADVM.DISPOSALHEADERID + "</td></tr>"
                              + "<tr><td valign=top>&nbsp;&nbsp;Request Type:</td><td valign=top > Asset " + _ADVM.ASSETTYPE + "</td></tr>"
                              + "<tr><td valign=top>&nbsp;&nbsp;Reject Remark:</td><td valign=top >" + rejectRemarks + "</td></tr>"
                               + "<tr><td colspan=2>&nbsp;</td></tr>"
                              + "<tr><td colspan=2>&nbsp;&nbsp;Please login in <a href=" + serverpath.getServerPath() + "/SSO > Employee Portal</a> to reprocess the request.</td></tr>"
                              + "<tr><td colspan=2><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                              + "<tr><td colspan=2>&nbsp;&nbsp;Eportal Team<br/><br/></td></tr>"
                              + "<tr><td colspan=2><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr><tr><td></td></tr></table> ";

                            commanEmail.MailSubject = str1;
                            commanEmail.MailBody = str2;
                            commanEmail.Send();
                            num = (short)1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                num = (short)-4;
            }
            return num;
        }

        public short SendEmailForCancelation(AssetDisposalViewModel _ADVM)
        {
            short num = 0;
            short[] LEVEL_ARRAY = { 2, 3, 4, 5, 6, 9, 24, 10, 11, 12 }; //// Send mail only user authorities
            Employee_Details approvalAuthorityDetail = new Employee_Details();
            if (LEVEL_ARRAY.Contains(_ADVM.STATUS_LEVEL))
            {
                long? appEcode = _ADVM.AppHistoryList.Where(a => a.STATUS_LEVEL == _ADVM.STATUS_LEVEL).Select(s => s.APPCODE).FirstOrDefault();
                if (appEcode != null)
                    approvalAuthorityDetail = _CommonRepo.GetEmpDetailById((long)appEcode);
                else
                    approvalAuthorityDetail = null;
            }
            else
            {
                approvalAuthorityDetail = this.GetNextApprovalAuthorityDetail(_ADVM.STATUS_LEVEL, 0, _ADVM);
            }
            if (approvalAuthorityDetail != null)
            {
                if (!string.IsNullOrEmpty(approvalAuthorityDetail.EMail_Id) && !string.IsNullOrEmpty(approvalAuthorityDetail.Employee_First_Name) && !string.IsNullOrEmpty(_ADVM.EMP_NAME))
                {
                    commanEmail commanEmail = new commanEmail();
                    commanEmail.MailFrom = "portal.admin@honda.hmsi.in";
                    commanEmail.MailTo = approvalAuthorityDetail.EMail_Id;

                    string str1 = "Asset Disposal Request Cancellation";
                    string str2 = string.Empty;

                    str2 = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Asset Disposal Request</b></div>"
                    + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                    + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + approvalAuthorityDetail.Employee_First_Name + " San,</b><br/></td></tr>"
                    + "<tr><td colspan=2>&nbsp;</td></tr>"
                    + "<tr><td valign=top colspan=2>&nbsp &nbsp;<b>" + _ADVM.EMP_NAME + " San has cancelled an Asset Disposal request in Employee Portal. Below are detail :-</b></td></tr>"
                    + "<tr><td colspan=2>&nbsp;</td></tr>"
                    + "<tr><td valign=top>&nbsp;&nbsp;Request ID:</td><td valign=top > " + _ADVM.DISPOSALHEADERID + "</td></tr>"
                    + "<tr><td valign=top>&nbsp;&nbsp;Request Type:</td><td valign=top > Asset " + _ADVM.ASSETTYPE + "</td></tr>"
                    + "<tr><td valign=top>&nbsp;&nbsp;Request Date:</td><td valign=top >" + _ADVM.DATEADDED + "</td></tr>"
                    + "<tr><td valign=top>&nbsp;&nbsp;No. of items:</td><td valign=top >" + _ADVM.DisposalDetailList.Count() + "</td></tr>"
                    + "<tr><td colspan=2>&nbsp;</td></tr>"
                    + "<tr><td colspan=2>&nbsp;&nbsp;Please login in <a href=" + serverpath.getServerPath() + "/SSO > Employee Portal</a></td></tr>"
                    + "<tr><td colspan=2><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                    + "<tr><td colspan=2>&nbsp;&nbsp;Eportal Team<br/><br/></td></tr>"
                    + "<tr><td colspan=2><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr><tr><td></td></tr></table> ";

                    commanEmail.MailSubject = str1;
                    commanEmail.MailBody = str2;
                    commanEmail.Send();
                    num = (short)1;
                }
            }
            return num;
        }

        public string CheckValidationByFnCode(string fnCode, string parmName)
        {
            return _AssetRepo.CheckValidationByFnCode(fnCode, parmName);
        }

        public ApprovalAuthorityViewModel GetApprovalAuthorityBySiteId(long SiteId, short Status_level)
        {
            return _AssetRepo.GetApprovalAuthorityBySiteId(SiteId, Status_level);
        }

        public string GetValidationParmValue(string parmName)
        {
            return _AssetRepo.GetValidationParmValue(parmName);
        }

        public List<AssetDisposalViewModel> GetRequestCancellationListForAdmin()
        {
            return _AssetRepo.GetRequestCancellationListForAdmin();
        }

        public short UpdateIBMDate(AssetDisposalViewModel ADVM)
        {
            return _AssetRepo.UpdateIBMDate(ADVM);
        }

        #endregion
    }
}
