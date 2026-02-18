using ePortal.DomainClasses;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;



namespace ePortal.Application.Services
{
    public class A00Service : IA00Service
    {
        private readonly A00Repository _objA00Repositry;
        //09-Sept-2021 change start
        private readonly CommonRepository _CommonRepo;
        //09-Sept-2021 change end

        public A00Service(A00Repository objA00Repositry, CommonRepository CommonRepo)
        {
            _objA00Repositry = objA00Repositry;
            //09-Sept-2021 change start
            _CommonRepo = CommonRepo;
            //09-Sept-2021 change end
        }
        //Added below GetCurrentActiveKi by Eshant on 27-06-22 to get current active syki
        public Int64 GetCurrentActiveKi()
        {
            var activeSYKI = _objA00Repositry.GetCurrentActiveKi();
            return activeSYKI;
        }
        //End
        public A00DtlViewModel GetA00Dtl(A00SearchModel objSearchModel)
        {
            //var objResult = _objA00Repositry.GetA00Detail(objSearchModel);
            var objResult = _objA00Repositry.GetA00Dtl(objSearchModel);
            return objResult;
        }

        public A00DataViewModel GetA00DtlFullTable()
        {
            var objResult = _objA00Repositry.GetA00DtlFullTable();
            return objResult;
        }

        public A00SYKIViewModel GetA00SYKIList()
        {
            var SYKIList = _objA00Repositry.GetA00SYKIList();
            return SYKIList;
        }

        //Changes By Ankit-15-01-2021, 
        /// <summary>
        /// Get the current ki and the future ki
        /// </summary>
        /// <returns></returns>
        public A00SYKIViewModel GetLatestA00SYKIList()
        {
            var SYKIList = _objA00Repositry.GetLatestA00SYKIList();
            return SYKIList;
        }


        //public SearchParameterList GetOPERATIONlist()
        //{
        //    var op = _objA00Repositry.GetOPERATIONlist();
        //    return op;
        //}

        //public A00DIVISIONlist GetDIVISIONlist(long id)
        //{
        //    var op = _objA00Repositry.GetDIVISIONlist();
        //    return op;
        //}

        //public
        //Added by kiran 18-dec-2020
        public List<SearchParameterList> BindDivision(long? op_Id, long? SYKIID)
        {
            var op = _objA00Repositry.BindDivision(op_Id, SYKIID);
            return op;
        }


        public A00Deficiency InsertDeffience(A00Deficiency collection)
        {
            var diff = _objA00Repositry.InsertDeffience(collection);
            return diff;
        }

        //List<DEPARTMENTlist> BindDepartment(long op_Id);
        //List<SECTIONList> BindSection(long op_Id);

        public List<SearchParameterList> BindDepartment(long? div_Id, long? op_Id, long SYKIID)
        {
            var dep = _objA00Repositry.BindDepartment(div_Id, op_Id, SYKIID);
            return dep;
        }

        public List<SearchParameterList> BindSection(long? dep_Id, long? div_Id, long? op_Id, long SYKIID)
        {
            var sec = _objA00Repositry.BindSecion(dep_Id, div_Id, op_Id, SYKIID);
            return sec;
        }

        //End by Kiran 18-dec-2020






        public A00DataViewModel GetA00ADORGLEVELList(decimal? SYKIID)
        {
            var AdOrgLevelList = _objA00Repositry.GetA00ADORGLEVELList(SYKIID);
            return AdOrgLevelList;
        }

        public A00DataViewModel GetA00_IT_ADORGLEVELList(A00SearchModel objSearchModel)
        {
            var AdOrgLevelList = _objA00Repositry.GetA00_IT_ADORGLEVELList(objSearchModel);
            return AdOrgLevelList;
        }

        public A00DataViewModel GetA00_INVFORCASTList()
        {
            var Invforcast = _objA00Repositry.GetA00_INVFORCASTList();
            return Invforcast;
        }

        //public A00PartialClass GetA00PartialClass()
        //{

        //}

        public A00MaxA00DTLTBID GetA00MaxA00DTLTBID()
        {
            var maxA00DtlTbID = _objA00Repositry.GetA00MaxA00DTLTBID();
            return maxA00DtlTbID;
        }

        public A00MaxA00APPROVAL GetA00MaxA00APPROVAL()
        {
            var maxA00APPROVAL = _objA00Repositry.GetA00MaxA00APPROVAL();
            return maxA00APPROVAL;
        }

        public A00DataViewModel GetA00INVFORCASTList()
        {
            var A00Invforcast = _objA00Repositry.GetA00INVFORCASTList();
            return A00Invforcast;
        }

        public A00_APPROVAL GetA00APPROVAL(A00SearchModel objSearchModel)
        {
            var objResult = _objA00Repositry.GetA00APPROVAL(objSearchModel);
            return objResult;
        }

        public A00DataViewModel GetA00APPROVALFullTable()
        {
            var objResult = _objA00Repositry.GetA00APPROVALFullTable();
            return objResult;
        }

        //07-Sept-2021 change start
        public A00_ADORGCOORDINATOR getDataFromAdorgcoordinator(long? ADORGLEVELID)
        {
            var objResult = _objA00Repositry.getDataFromAdorgcoordinator(ADORGLEVELID);
            return objResult;
        }

        public List<A00_ADORGCOORDINATORDataList> GetA00_ADORGCOORDINATORLIST(long? ADEMPCODE, string userType)
        {
            var objResult = _objA00Repositry.GetA00_ADORGCOORDINATORLIST(ADEMPCODE, userType);
            return objResult;
        }
        //07-Sept-2021 change end


        public A00DataViewModel GetA00ADEMPLOYEE()
        {
            var A00Emp = _objA00Repositry.GetA00ADEMPLOYEE();
            return A00Emp;
        }

        public A00DataViewModel GetA00_VW_ASSOCIATELVLDETAILS_FullList()
        {
            var objResult = _objA00Repositry.GetA00_VW_ASSOCIATELVLDETAILS_FullList();
            return objResult;
        }
        public string Getoperationdetails(long empcode, int Syki)
        {
            var em = _objA00Repositry.Getoperationdetails(empcode, Syki);
            return em;

        }
        //Added below method by Eshant on 17-Aug-22 to show Division on A00 View page  as per Review Meeting 01-aug-22*@
        public string Getdivisiondetails(long empcode, int Syki)
        {
            var em = _objA00Repositry.Getdivisiondetails(empcode, Syki);
            return em;

        }

        public A00DataViewModel GetA00_VW_ASSOCIATELVLDETAILS_ITApproval(A00SearchModel objSearchModel)
        {
            var objResult = _objA00Repositry.GetA00_VW_ASSOCIATELVLDETAILS_ITApproval(objSearchModel);
            return objResult;
        }

        public A00_VW_ASSOCIATELVLDETAILS GetA00_VW_ASSOCIATELVLDETAILS(A00SearchModel objSearchModel)
        {
            var objResult = _objA00Repositry.GetA00_VW_ASSOCIATELVLDETAILS(objSearchModel);
            return objResult;
        }
        public List<ADORGLEVEL> GetOrgLevelList(long typeId, long? SYKIID)
        {
            return _objA00Repositry.GetOrgLevelList(typeId, SYKIID);
        }
        public A00_VW_ASSOCIATELVLDETAILS1 GetA00_VW_ASSOCIATELVLDETAILS1(A00SearchModel objSearchModel)
        {
            var objResult = _objA00Repositry.GetA00_VW_ASSOCIATELVLDETAILS1(objSearchModel);
            return objResult;
        }

        public A00DataViewModel GetA00_ADORGLEVELHEAD()
        {
            var objResult = _objA00Repositry.GetA00_ADORGLEVELHEAD();
            return objResult;
        }

        public Single_A00ADEMPLOYEE Get_Single_A00ADEMPLOYEE(A00SearchModel objSearchModel)
        {
            var objResult = _objA00Repositry.Get_Single_A00ADEMPLOYEE(objSearchModel);
            return objResult;
        }

        /// <summary>
        /// Code By Ankit-Swaransoft
        /// </summary>
        /// <param name="ADEMPCODE"></param>
        /// <returns></returns>
        public Single_A00ADEMPLOYEE Get_Single_A00ADEMPLOYEEEmail(long? ADEMPCODE)
        {
            var objResult = _objA00Repositry.Get_Single_A00ADEMPLOYEEEmail(ADEMPCODE);
            return objResult;
        }

        public UserRole getUserRole(long empCode, decimal? SYKI)
        {
            var uRole = _objA00Repositry.getUserRole(empCode, SYKI);
            return new UserRole { isDeptHead = uRole.isDeptHead, isCoOrdHead = uRole.isCoOrdHead, isDivHead = uRole.isDivHead, isExeCoOrdHead = uRole.isExeCoOrdHead, isOperatingHead = uRole.isOperatingHead };
        }

        public UserRole getUserRoles(long empCode, decimal? SYKI)
        {
            var uRole = _objA00Repositry.getUserRoles(empCode, SYKI);
            return new UserRole { isSecHead = uRole.isSecHead, isDeptHead = uRole.isDeptHead, isCoOrdHead = uRole.isCoOrdHead, isDivHead = uRole.isDivHead, isExeCoOrdHead = uRole.isExeCoOrdHead, isOperatingHead = uRole.isOperatingHead };
        }

        public A00_ADORGCOORDINATOR getA00_ADORGCOORDINATOR(A00SearchModel objSearchModel)
        {
            var objResult = _objA00Repositry.getA00_ADORGCOORDINATOR(objSearchModel);
            return objResult;
        }

        public InsertA00DtlTb InsertUpdateA00Detail(InsertA00DtlTb collection)
        {
            try
            {
                collection = _objA00Repositry.InsertUpdateA00Detail(collection);
                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public InsertA00APPROVAL InsertUpdateA00APPROVAL(InsertA00APPROVAL collection)
        {
            try
            {
                collection = _objA00Repositry.InsertUpdateA00APPROVAL(collection);
                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //sa CR7306
        public InsertA00DtlTb InsertUpdateA00DetailCordinator(InsertA00DtlTb collection)
        {
            try
            {
                collection = _objA00Repositry.InsertUpdateA00DetailCordinator(collection);
                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public InsertA00APPROVAL InsertUpdateA00APPROVALCordinator(InsertA00APPROVAL collection)
        {
            try
            {
                collection = _objA00Repositry.InsertUpdateA00APPROVALCordinator(collection);
                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //ea CR7306
        public InsertA00INVFORCAST InsertUpdateA00INVFORCAST(InsertA00INVFORCAST collection)
        {
            try
            {
                collection = _objA00Repositry.InsertUpdateA00INVFORCAST(collection);
                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SaveA00Deficiency(string Remark, string fileName, long SYKI, string isDeficiencyFileExist, int userId, long A00DTLTBID)
        {
            var diff = _objA00Repositry.SaveA00Deficiency(Remark, fileName, SYKI, isDeficiencyFileExist, userId, A00DTLTBID);
            return diff;
        }

        public int SaveA00DeficiencyUpdate(string Remark, string fileName, long SYKI, string isDeficiencyFileExist, int userId, long A00DTLTBID)
        {
            var diff = _objA00Repositry.SaveA00DeficiencyUpdate(Remark, fileName, SYKI, isDeficiencyFileExist, userId, A00DTLTBID);
            return diff;
        }
        //public int SaveA00Allocation(long SYKI, int userId, long A00DTLTBID, string ApplicationPIC, string InfrastructurePIC, string OtherMembers)
        //{
        //    var diff = _objA00Repositry.SaveA00Allocation(SYKI, userId, A00DTLTBID, ApplicationPIC, InfrastructurePIC, OtherMembers);
        //    return diff;
        //}

        public int SaveA00DeficiencyClosure(int userId, long A00DTLTBID, long SYKI, string Remark, int deficiencyStatus)
        {
            var diff = _objA00Repositry.SaveA00DeficiencyClosure(userId, A00DTLTBID, SYKI, Remark, deficiencyStatus);
            return diff;
        }

        public int GetDeficiencyStatus(long? A00DTLTBID, decimal? SYKIID)
        {
            var diff = _objA00Repositry.GetDeficiencyStatus(A00DTLTBID, SYKIID);
            return diff;
        }
        public List<GetEmployeeA00AllocationVM> GetEmployeeA00AllocationList(long OPERATIONID, long SYKIID)
        {
            return _objA00Repositry.GetEmployeeA00AllocationList(OPERATIONID, SYKIID);
        }

        public List<GetDeficiencyRaisedVM> GetDeficiencyRaisedDetails(long? A00DTLTBID, decimal? SYKIID)
        {
            return _objA00Repositry.GetDeficiencyRaisedDetails(A00DTLTBID, SYKIID);
        }

        public bool GetUserRoleForDeficiency(long empCode, decimal? SYKI)
        {
            var diff = _objA00Repositry.GetUserRoleForDeficiency(empCode, SYKI);
            return diff;
        }

        public int SaveA00Allocation(long SYKI, long ApplicationPIC, long InfrastructurePIC, string OtherMembers, int userId, long A00DTLTBID, int rdoMainPic)
        {
            var A00Allocated = _objA00Repositry.SaveA00Allocation(SYKI, ApplicationPIC, InfrastructurePIC, OtherMembers, userId, A00DTLTBID, rdoMainPic);
            return A00Allocated;
        }

        public List<long> GetA00Allocation()
        {
            var A00AllocationList = _objA00Repositry.GetA00Allocation();
            return A00AllocationList;
        }

        public List<GetAllocationVM> GetAllocationDetails(long? A00DTLTBID, decimal? SYKIID)
        {
            return _objA00Repositry.GetAllocationDetails(A00DTLTBID, SYKIID);
        }
        public bool IsA00DeficiencyRaised(long? A00DTLTBID, decimal? SYKIID)
        {
            var IsDeficiencyRaised = _objA00Repositry.IsA00DeficiencyRaised(A00DTLTBID, SYKIID);
            return IsDeficiencyRaised;
        }
        public bool IsA00DeficiencyClosed(long? A00DTLTBID, decimal? SYKIID)
        {
            var IsDeficiencyClosed = _objA00Repositry.IsA00DeficiencyClosed(A00DTLTBID, SYKIID);
            return IsDeficiencyClosed;
        }

        public A00DataViewModel GetA00AllocationList()
        {
            var A00Allocation = _objA00Repositry.GetA00AllocationList();
            return A00Allocation;
        }

        public A00DataViewModel GetA00ItConfirmationList()
        {
            var A00Allocation = _objA00Repositry.GetA00ItConfirmationList();
            return A00Allocation;
        }
        public string GetDeficiencyUpdateEmialId(long? A00DTLTBID, long? SYKI)
        {
            var emailId = _objA00Repositry.GetDeficiencyUpdateEmialId(A00DTLTBID, SYKI);
            return emailId;

        }
        public string GetA00ProjectTitle(long? A00DTLTBID, long? SYKI)
        {
            var ProjectTitle = _objA00Repositry.GetA00ProjectTitle(A00DTLTBID, SYKI);
            return ProjectTitle;
        }

        public string GetUserEmailForDeficiencyRaised(long? A00DTLTBID, long? SYKI)
        {
            var emailId = _objA00Repositry.GetUserEmailForDeficiencyRaised(A00DTLTBID, SYKI);
            return emailId;
        }

        public ProjectVm GetA00ProjectDetails(long? A00DTLTBID, decimal SYKI)
        {
            var ProjectDetails = _objA00Repositry.GetA00ProjectDetails(A00DTLTBID, SYKI);
            return ProjectDetails;
        }

        public string GetEmpDept(long? EmpCode, decimal? SYKIID)
        {
            var DeptName = _objA00Repositry.GetEmpDept(EmpCode, SYKIID);
            return DeptName;
        }

        public long GetA00MainPIC(long? A00DTLTBID, decimal? SYKI)
        {
            var MainPICEmpCode = _objA00Repositry.GetA00MainPIC(A00DTLTBID, SYKI);
            return MainPICEmpCode;
        }

        public int ITConfirmationSaveData(ITConfirmationVM ITConfirmation, int userId, string btnText, A00SearchModel _objA00SearchModel)
        {
            var ItConfirmationId = _objA00Repositry.ITConfirmationSaveData(ITConfirmation, userId, btnText, _objA00SearchModel);
            return ItConfirmationId;
        }

        public ITConfirmationVM GetA00ItConfirmationDetails(long? A00DTLTBID, decimal SYKI)
        {
            var ITConfirmationVM = _objA00Repositry.GetA00ItConfirmationDetails(A00DTLTBID, SYKI);
            return ITConfirmationVM;
        }

        public bool IsAllocationExist(long? A00DTLTBID, decimal? SYKI)
        {
            var IsAllocationExist = _objA00Repositry.IsAllocationExist(A00DTLTBID, SYKI);
            return IsAllocationExist;
        }

        public A00RaisedDetails A00RaisedDetails(long? A00DTLTBID, decimal? SYKI)
        {
            var result = _objA00Repositry.A00RaisedDetails(A00DTLTBID, SYKI);
            return result;
        }

        public string GetEmpName(long? EmpCode)
        {
            var EmpName = _objA00Repositry.GetEmpName(EmpCode);
            return EmpName;
        }

        public int GetA00AllocationSNo(long? A00DTLTBID, decimal? SYKI)
        {
            var Id = _objA00Repositry.GetA00AllocationSNo(A00DTLTBID, SYKI);
            return Id;
        }

        public long GetA00ApplicationPIC(long? A00DTLTBID, decimal SYKI)
        {
            var ApplicationPIC = _objA00Repositry.GetA00ApplicationPIC(A00DTLTBID, SYKI);
            return ApplicationPIC;
        }
        public long GetA00InfraStructurePIC(long? A00DTLTBID, decimal SYKI)
        {
            var InfraStructurePIC = _objA00Repositry.GetA00InfraStructurePIC(A00DTLTBID, SYKI);
            return InfraStructurePIC;
        }
        public ITConfirmationApprovalHistoryVM GetA00ItConfirmationHistoryDetails(decimal A00ItConfirmationID)
        {
            var ITConfirmationApprovalHistory = _objA00Repositry.GetA00ItConfirmationHistoryDetails(A00ItConfirmationID);
            return ITConfirmationApprovalHistory;
        }

        public List<ITConfirmationApprovalHistoryListVM> GetA00ItConfirmationHistory(decimal A00ItConfirmationID)
        {
            var ITConfirmationApprovalHistory = _objA00Repositry.GetA00ItConfirmationHistory(A00ItConfirmationID);
            return ITConfirmationApprovalHistory;
        }

        public int SaveA00ProjectStatus(string ProjectStage, string ItConfrApprovedOn, int ProjectStatus, string Remark, string fileName, long SYKI, int userId, long A00DTLTBID, DateTime ProjectUpdateDate)
        {
            var Status = _objA00Repositry.SaveA00ProjectStatus(ProjectStage, ItConfrApprovedOn, ProjectStatus, Remark, fileName, SYKI, userId, A00DTLTBID, ProjectUpdateDate);
            return Status;
        }

        public int A00ActivitySaveData(A00AcitivtyVM a00AcitivtyVM) //Dinesh
        {
            var Status1 = _objA00Repositry.A00ActivitySaveData(a00AcitivtyVM); //Dinesh
            return Status1;
        }
        public A00AcitivtyVM A00ActivityDetails(int Id)
        {
            var A00AcitivtyVM = _objA00Repositry.A00ActivityDetails(Id);
            return A00AcitivtyVM;
        }
        public int A00ActivityUpdate(int A00ActivityID, int ActivitySchedule, string ActivityRemarks, int ActionBy)
        {
            var Status = _objA00Repositry.A00ActivityUpdate(A00ActivityID, ActivitySchedule, ActivityRemarks, ActionBy);
            return Status;
        }
        public A00DataViewModel GetA00ActivityList()
        {
            var A00Activity = _objA00Repositry.GetA00ActivityList();
            return A00Activity;
        }
        //Dinesh
        public A00ProjectStateUpdate GetA00ProjectStateUpdate(long? A00DTLTBID, decimal SYKI)
        {
            var ProjectStateUpdateVM = _objA00Repositry.GetA00ProjectStateUpdate(A00DTLTBID, SYKI);
            return ProjectStateUpdateVM;
        }

        //Change start on 10-July-2021
        public A00DataViewModel GetA00ProjectUpdateStatusList(long? A00DTLTBID, decimal? SYKI)
        {
            var A00ProjectUpdateStatusList = _objA00Repositry.GetA00ProjectUpdateStatusList(A00DTLTBID, SYKI);
            return A00ProjectUpdateStatusList;
        }
        //Change end on 10-July-2021

        public List<A00AcitivtyHistoryVM> A00ActivityHistory(int Id)
        {
            var A00AcitivtyHistoryVM = _objA00Repositry.A00ActivityHistory(Id);
            return A00AcitivtyHistoryVM;
        }
        public short? IsDeficiencyExist(long? A00DTLTBID, decimal? SYKIID)
        {
            var diff = _objA00Repositry.IsDeficiencyExist(A00DTLTBID, SYKIID);
            return diff;
        }
        public short? IsITConfirmationDone(long? A00DTLTBID, decimal? SYKIID)
        {
            var diff = _objA00Repositry.IsITConfirmationDone(A00DTLTBID, SYKIID);
            return diff;
        }

        //Change start on 22-July-2021
        public int UpdateA00ConvertToCR(long A00DTLTBID)
        {
            var diff = _objA00Repositry.UpdateA00ConvertToCR(A00DTLTBID);
            return diff;
        }
        public int InsertA00ConvertToCR(long A00DTLTBID, int CHANGEDBY, string CONVERTTOCRREMARKS)
        {
            var InsertResult = _objA00Repositry.InsertA00ConvertToCR(A00DTLTBID, CHANGEDBY, CONVERTTOCRREMARKS);
            return InsertResult;
        }
        public A00ConvertToCRModel getConvertedToCRDtl(long? A00DTLTBID)
        {
            var objResult = _objA00Repositry.getConvertedToCRDtl(A00DTLTBID);
            return objResult;
        }
        //Change end on 22-July-2021

        //09-Sept-2021 change start
        public long? getA00GetHOOH(long? OperationId, long? DivisionId)
        {
            long? HOOH_ID = null;
            string strValue = _CommonRepo.GetParameterValue("A00PPCHOOH");//Changed By Eshant  PPCHOOH to A00PPCHOOH to avoid conflict with PPCHOOH(PR data), need to maintain manual enntry per year with PPC super head with plant ids 
            string strPPCOH = strValue.Split('~')[0];
            string[] strOHOperation = strValue.Split('~')[1].Split(',');
            if (!string.IsNullOrEmpty(strPPCOH) && (strOHOperation.Contains(OperationId.ToString()) || strOHOperation.Contains(DivisionId.ToString())))
            {
                HOOH_ID = Convert.ToInt64(strPPCOH);
            }
            return HOOH_ID;
        }

        public bool isPPCHOOH(long empID)
        {
            string strValue = _CommonRepo.GetParameterValue("A00PPCHOOH");//Changed By Eshant  PPCHOOH to A00PPCHOOH to avoid conflict with PPCHOOH(PR data), need to maintain manual enntry per year with PPC super head with plant ids
            string strPPCOH = strValue.Split('~')[0];
            if (Convert.ToString(empID) == strPPCOH)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public short AddRemark(long A00ID, string REMARK, long loginUser)
        {
            return _objA00Repositry.AddRemark(A00ID, REMARK, loginUser);
        }
        public List<A00REMARKSVM> A00RemarksHistory(int A00ID)
        {
            return _objA00Repositry.A00RemarksHistory(A00ID);
        }
        //09-Sept-2021 change end
    }
}