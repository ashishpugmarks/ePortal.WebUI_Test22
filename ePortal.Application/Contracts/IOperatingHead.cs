using ePortal.ViewModels;


namespace ePortal.Application.Contracts
{
    public interface IOperatingHead
    {
        OperatingHeadSYKIViewModel GetOperatingHeadSYKIList();
        List<OperatingHeadSearchModel1> GetDivisionListForISSCMemberNomination(long Skyid, long Operationid, long empcode);
        long? GetOperationID(long Empcode, long Sykid);
        List<empName> GetEmpnameList();

        OperatingHeadAddNominationVM InsertUpdateMemberNomination(OperatingHeadAddNominationVM AddNominationVM);

        bool CheckDate_PeriodSetting(long Skyid);
        int Operatinghead_recordexist(int userId, long Sykid);
        ISSCMember_details GetIsscMemberDetails(long empcode);
    };
}
