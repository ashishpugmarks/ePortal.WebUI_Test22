using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class OperatingHeadService : IOperatingHead
    {
        private readonly OperatingHeadRepository _objOperatingHeadRepositry;
        public OperatingHeadService(OperatingHeadRepository objOperatingHeadRepositry)
        {
            _objOperatingHeadRepositry = objOperatingHeadRepositry;
        }       
        public List<OperatingHeadSearchModel1> GetDivisionListForISSCMemberNomination(long Skyid, long Operationid, long empcode)
        {
            var List = _objOperatingHeadRepositry.GetDivisionListForISSCMemberNomination(Skyid, Operationid, empcode);
            return List;
        }

        public List<empName> GetEmpnameList()
        {
            var list = _objOperatingHeadRepositry.getEmpList();
            return list;
        }
        public OperatingHeadSYKIViewModel GetOperatingHeadSYKIList()
        {
            var SYKIList = _objOperatingHeadRepositry.GetAssetRegistrationSYKIList();
            return SYKIList;
        }

        public long? GetOperationID(long Empcode, long Sykid)
        {
            var OPERATIONID = _objOperatingHeadRepositry.GetOperationID(Empcode, Sykid);
            return OPERATIONID;
        }
        public OperatingHeadAddNominationVM InsertUpdateMemberNomination(OperatingHeadAddNominationVM AddNominationVM)
        {
            var NominationList = _objOperatingHeadRepositry.InsertUpdateMemberNomination(AddNominationVM);
            return NominationList;
        }

        public bool CheckDate_PeriodSetting(long Skyid)
        {
            bool objresult = _objOperatingHeadRepositry.CheckDate_PeriodSetting(Skyid);
            return objresult;
        }

        public int Operatinghead_recordexist(int userId, long Sykid)
        {
            int objresult = _objOperatingHeadRepositry.Operatinghead_recordexist(userId, Sykid);
            return objresult;
        }

        public ISSCMember_details GetIsscMemberDetails(long empcode)
        {
            var objresult = _objOperatingHeadRepositry.GetIsscMemberDetails(empcode);
            return objresult;
        }
    }
}
