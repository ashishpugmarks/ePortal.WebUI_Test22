using ePortal.ViewModels;
using ePortal.DomainClasses;


namespace ePortal.Application.Contracts
{
    public interface IA00Service
    {
        //Added below GetCurrentActiveKi by Eshant on 27-06-22 to get current active syki
        Int64 GetCurrentActiveKi();
        //End
        A00DtlViewModel GetA00Dtl(A00SearchModel objSearchModel);

        A00DataViewModel GetA00DtlFullTable();

        A00SYKIViewModel GetA00SYKIList();
        //Changes By Ankit-15-01-2021, 
        /// <summary>
        /// Get the current ki and the future ki
        /// </summary>
        /// <returns></returns>
        A00SYKIViewModel GetLatestA00SYKIList();

        // OPERATIONlist GetOPERATIONlist();
        //SearchParameterList GetOPERATIONlist();
        //Added by kiran 18-12-2020
        List<SearchParameterList> BindDivision(long? op_Id, long? SYKIID);
        A00Deficiency InsertDeffience(A00Deficiency collection);
        List<SearchParameterList> BindDepartment(long? div_Id, long? op_Id, long SYKIID);
        List<SearchParameterList> BindSection(long? dep_Id, long? div_Id, long? op_Id, long SYKIID);

        //End by kiran 18-12-2020
        A00_VW_ASSOCIATELVLDETAILS1 GetA00_VW_ASSOCIATELVLDETAILS1(A00SearchModel objSearchModel);



        A00DataViewModel GetA00ADORGLEVELList(decimal? SYKIID);

        A00DataViewModel GetA00_IT_ADORGLEVELList(A00SearchModel objSearchModel);

        A00DataViewModel GetA00_INVFORCASTList();

        //A00PartialClass GetA00PartialClass();

        A00MaxA00DTLTBID GetA00MaxA00DTLTBID();

        A00DataViewModel GetA00INVFORCASTList();

        A00_APPROVAL GetA00APPROVAL(A00SearchModel objSearchModel);

        A00DataViewModel GetA00APPROVALFullTable();

        //07-Sept-2021 change start
        A00_ADORGCOORDINATOR getDataFromAdorgcoordinator(long? ADORGLEVELID);
        List<A00_ADORGCOORDINATORDataList> GetA00_ADORGCOORDINATORLIST(long? ADEMPCODE, string userType);
        //07-Sept-2021 change end

        A00DataViewModel GetA00ADEMPLOYEE();

        A00MaxA00APPROVAL GetA00MaxA00APPROVAL();

        A00_VW_ASSOCIATELVLDETAILS GetA00_VW_ASSOCIATELVLDETAILS(A00SearchModel objSearchModel);

        List<ADORGLEVEL> GetOrgLevelList(long typeId, long? SYKIID);
        //A00_VW_ASSOCIATELVLDETAILS GetA00_VW_ASSOCIATELVLDETAILS1(A00SearchModel objSearchModel);

        A00DataViewModel GetA00_VW_ASSOCIATELVLDETAILS_FullList();

        string Getoperationdetails(long empcode, int Syki);
        string Getdivisiondetails(long empcode, int Syki);//Added by Eshant on 17-Aug-22 to show Division on A00 View page  as per Review Meeting 01-aug-22

        A00DataViewModel GetA00_VW_ASSOCIATELVLDETAILS_ITApproval(A00SearchModel objSearchModel);

        A00DataViewModel GetA00_ADORGLEVELHEAD();

        Single_A00ADEMPLOYEE Get_Single_A00ADEMPLOYEE(A00SearchModel objSearchModel);
        Single_A00ADEMPLOYEE Get_Single_A00ADEMPLOYEEEmail(long? ADEMPCODE);

        UserRole getUserRole(long empCode, decimal? SYKI);

        UserRole getUserRoles(long empCode, decimal? SYKI);

        A00_ADORGCOORDINATOR getA00_ADORGCOORDINATOR(A00SearchModel objSearchModel);

        InsertA00DtlTb InsertUpdateA00Detail(InsertA00DtlTb collection);

        InsertA00APPROVAL InsertUpdateA00APPROVAL(InsertA00APPROVAL collection);
        //sa CR7306
        InsertA00DtlTb InsertUpdateA00DetailCordinator(InsertA00DtlTb collection);

        InsertA00APPROVAL InsertUpdateA00APPROVALCordinator(InsertA00APPROVAL collection);
        //ea CR7306
        InsertA00INVFORCAST InsertUpdateA00INVFORCAST(InsertA00INVFORCAST collection);

        int SaveA00Deficiency(string Remark, string fileName, long SYKI, string isDeficiencyFileExist, int userId, long A00DTLTBID);

        int SaveA00DeficiencyUpdate(string Remark, string fileName, long SYKI, string isDeficiencyFileExist, int userId, long A00DTLTBID);

        //int SaveA00Allocation(long SYKI, int userId, long A00DTLTBID, string ApplicationPIC, string InfrastructurePIC, string OtherMembers);

        int SaveA00DeficiencyClosure(int userId, long A00DTLTBID, long SYKI, string Remark, int deficiencyStatus);

        int GetDeficiencyStatus(long? A00DTLTBID, decimal? SYKIID);
        List<GetEmployeeA00AllocationVM> GetEmployeeA00AllocationList(long OPERATIONID, long SYKIID);

        List<GetDeficiencyRaisedVM> GetDeficiencyRaisedDetails(long? A00DTLTBID, decimal? SYKIID);

        bool GetUserRoleForDeficiency(long empCode, decimal? SYKI);
        int SaveA00Allocation(long SYKI, long ApplicationPIC, long InfrastructurePIC, string OtherMembers, int userId, long A00DTLTBID, int rdoMainPic);

        List<long> GetA00Allocation();

        List<GetAllocationVM> GetAllocationDetails(long? A00DTLTBID, decimal? SYKIID);

        bool IsA00DeficiencyRaised(long? A00DTLTBID, decimal? SYKIID);
        bool IsA00DeficiencyClosed(long? A00DTLTBID, decimal? SYKIID);

        //int IsUserItConfirmation(int UserId, long? A00DTLTBID, decimal? SYKIID);

        //int GetInfrastructure(int UserId, long? A00DTLTBID, decimal? SYKIID);


        A00DataViewModel GetA00AllocationList();
        A00DataViewModel GetA00ItConfirmationList();

        string GetDeficiencyUpdateEmialId(long? A00DTLTBID, long? SYKI);

        string GetA00ProjectTitle(long? A00DTLTBID, long? SYKI);
        string GetUserEmailForDeficiencyRaised(long? A00DTLTBID, long? SYKI);
        ProjectVm GetA00ProjectDetails(long? A00DTLTBID, decimal SYKI);
        string GetEmpDept(long? EmpCode, decimal? SYKIID);
        long GetA00MainPIC(long? A00DTLTBID, decimal? SYKI);
        int ITConfirmationSaveData(ITConfirmationVM ITConfirmation, int userId, string btnText, A00SearchModel _objA00SearchModel);
        ITConfirmationVM GetA00ItConfirmationDetails(long? A00DTLTBID, decimal SYKI);
        bool IsAllocationExist(long? A00DTLTBID, decimal? SYKI);
        A00RaisedDetails A00RaisedDetails(long? A00DTLTBID, decimal? SYKI);
        string GetEmpName(long? EmpCode);
        int GetA00AllocationSNo(long? A00DTLTBID, decimal? SYKI);
        long GetA00ApplicationPIC(long? A00DTLTBID, decimal SYKI);
        long GetA00InfraStructurePIC(long? A00DTLTBID, decimal SYKI);
        ITConfirmationApprovalHistoryVM GetA00ItConfirmationHistoryDetails(decimal A00ItConfirmationID);
        List<ITConfirmationApprovalHistoryListVM> GetA00ItConfirmationHistory(decimal A00ItConfirmationID);
        int SaveA00ProjectStatus(string ProjectStage, string ItConfrApprovedOn, int ProjectStatus, string Remark, string fileName, long SYKI, int userId, long A00DTLTBID, DateTime ProjectUpdateDate);
        int A00ActivitySaveData(A00AcitivtyVM a00AcitivtyVM);//Dinesh
        A00AcitivtyVM A00ActivityDetails(int Id);
        int A00ActivityUpdate(int A00ActivityID, int ActivitySchedule, string ActivityRemarks, int ActionBy);
        A00DataViewModel GetA00ActivityList();
        A00ProjectStateUpdate GetA00ProjectStateUpdate(long? A00DTLTBID, decimal SYKI);//Dinesh

        //Change start on 10-July-2021
        A00DataViewModel GetA00ProjectUpdateStatusList(long? A00DTLTBID, decimal? SYKI);
        //Change end on 10-July-2021

        List<A00AcitivtyHistoryVM> A00ActivityHistory(int Id);
        short? IsDeficiencyExist(long? A00DTLTBID, decimal? SYKIID);
        short? IsITConfirmationDone(long? A00DTLTBID, decimal? SYKIID);

        //Change start on 22-July-2021
        int UpdateA00ConvertToCR(long A00DTLTBID);
        int InsertA00ConvertToCR(long A00DTLTBID, int CHANGEDBY, string CONVERTTOCRREMARKS);
        A00ConvertToCRModel getConvertedToCRDtl(long? A00DTLTBID);
        //Change end on 22-July-2021

        //09-Sept-2021 change start
        long? getA00GetHOOH(long? OperationId, long? DivisionId);
        bool isPPCHOOH(long empID);
        //09-Sept-2021 change end
        short AddRemark(long PPVM, string employeeDetails, long loginUser);
        List<A00REMARKSVM> A00RemarksHistory(int Id);

    }
}
