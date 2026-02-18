using System;
using System.Collections.Generic;
using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Services
{

    public class SeatMgmtService : ISeatMgmtService
    {
        private readonly SeatMgmtRepository _smRepo;
        Tuple<short, long> _retVal_tuple;

        public SeatMgmtService(SeatMgmtRepository smRepo)
        {
            _smRepo = smRepo;
            _retVal_tuple = new Tuple<short, long>(0, 0);
        }

        public List<SeatEmpCalViewModel> GetEmpList(long userID, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl)
        {
            return _smRepo.GetEmpDivisionHead(userID, Cal_List, Emp_Dtl);
        }
        //SR102091 START
        public Tuple<int, int> GetNewJoineeAndInactiveCount(List<SeatEmpCalViewModel> empList, List<SeatCalenderViewModel> HeaderList, int parmValue)
        {
            return _smRepo.GetNewJoineeAndInactiveCount(empList, HeaderList, parmValue);
        }
        //SR102091 END
        public List<SeatEmpCalViewModel> GetEmpListByOpHead(long userID, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl)
        {
            return _smRepo.GetEmpOpHead(userID, Cal_List, Emp_Dtl);
        }

        public bool IsOffDay(string date, long siteId, long operationId)
        {
            return _smRepo.IsOffDay(date, siteId, operationId);
        }

        public string GetParmValByParmName(string ParmName)
        {
            return _smRepo.GetParmValByParmName(ParmName);
        }

        public List<SeatSettingViewModel> GetParmListByName()
        {
            return GetParmListByName();
        }

        public Tuple<short, string> SaveRoster(List<SeatRosterViewModel> ModelList, long ActionBy)
        {
            return _smRepo.SaveRoster(ModelList, ActionBy);
        }

        public List<SeatEmpCalViewModel> ViewRosterEmpList(long userID, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_dtl)
        {
            return _smRepo.ViewRosterList(userID, Cal_List, Emp_dtl);
        }

        public string GetMapFilename(long floorID)
        {
            return _smRepo.GetMapFilename(floorID);
        }

        //public Tuple<int, int, int, int> GetSeatCountByDivID(long DivisionID, long SiteID)//Added by aumento : SR100656 //Commented for //SR102091
        public Tuple<int, int, int, int> GetSeatCountByDivID(long DivisionID, long SiteID, int NewJoineeCount, int InactiveCount)//SR102091 Added NewJoineeCount,InactiveCount
        {
            return _smRepo.GetSeatCountByDivID(DivisionID, SiteID, NewJoineeCount, InactiveCount);//SR102091 Added NewJoineeCount,InactiveCount
        }

        //public Tuple<int, int, int> GetSeatCountByOpID(long OpID, long SiteID) //Commented for //SR102091
        public Tuple<int, int, int> GetSeatCountByOpID(long OpID, long SiteID, int NewJoineeCount, int InactiveCount)//SR102091 Added NewJoineeCount,InactiveCount
        {
            return _smRepo.GetSeatCountByOpID(OpID, SiteID, NewJoineeCount, InactiveCount);//SR102091 Added NewJoineeCount,InactiveCount
        }

        //// Master
        public List<Employee_Details> AutocompleteEmployee(string term)
        {
            return _smRepo.AutocompleteEmployee(term);
        }
        public List<SeatAllocationViewModel> AutocompleteSeat(string term, long eCode)
        {
            return _smRepo.AutocompleteSeat(term, eCode);
        }

        public List<FixSeatViewModel> GetFixSeatList()
        {
            return _smRepo.GetFixSeatList();
        }

        public Tuple<short, string> SaveFixMapping(long mapId, long eCode, string seatNo, long ActionBy, short status)
        {
            return _smRepo.SaveFixMapping(mapId, eCode, seatNo, ActionBy, status);
        }

        public short UpdateStatus(long mapId, long ActionBy, short status)
        {
            return _smRepo.UpdateStatus(mapId, ActionBy, status);
        }

        public List<SelectListViewModel> GetOrgLevelList(long typeId)
        {
            return _smRepo.GetOrgLevelList(typeId);
        }

        public List<SeatingReportViewModel> GetSeatingReport(SearchSeatViewModel SSVM)
        {
            return _smRepo.GetSeatingReport(SSVM);
        }

        public List<DivWiseSeatViewModel> GetDivisionWiseSeatList()
        {
            return _smRepo.GetDivisionWiseSeatList();
        }

        public DivWiseSeatViewModel GetDivisionWiseSeatById(long dwsId)
        {
            return _smRepo.GetDivisionWiseSeatById(dwsId);
        }

        public Tuple<short, string> SaveDivisionWiseSeat(DivWiseSeatViewModel model, long ActionBy)
        {
            return _smRepo.SaveDivisionWiseSeat(model, ActionBy);
        }

        public List<SelectListViewModel> GetFloorOperationList()
        {
            return _smRepo.GetFloorOperationList();
        }

        public int GetFloorOperationSeatCount(long opId)
        {
            return _smRepo.GetFloorOperationSeatCount(opId);
        }
        public List<SelectListViewModel> BindDivision(long opId)
        {
            return _smRepo.BindDivision(opId);
        }

        public List<SeatEmpCalViewModel> GetDivisionWiseCountReport(List<SeatCalenderViewModel> CAL_LIST)
        {
            return _smRepo.GetDivisionWiseCountReport(CAL_LIST);
        }

        public List<SelectListViewModel> GetFloorList()
        {
            return _smRepo.GetFloorList();
        }

        public List<FloorOperationMapViewModel> GetFloorOperationMapList()
        {
            return _smRepo.GetFloorOperationMapList();
        }

        public Tuple<short, string> SaveFloorOpMapping(FloorOperationMapViewModel model, long ActionBy)
        {
            return _smRepo.SaveFloorOpMapping(model, ActionBy);
        }

        //public short UpdateOpMappingStatus(long mapId, long ActionBy, short status) //Added by Aumento :: SR100223
        public short UpdateOpMappingStatus(long mapId, long ActionBy, short status, string IsSeatAutoUpdated) //Added by Aumento :: SR100223
        {            
            //return _smRepo.UpdateOpMappingStatus(mapId, ActionBy, status); //Added by Aumento :: SR100223
            return _smRepo.UpdateOpMappingStatus(mapId, ActionBy, status, IsSeatAutoUpdated); //Added by Aumento :: SR100223
        }

        public List<FloorSeatMstViewModel> GetFloorSeatMstList()
        {
            return _smRepo.GetFloorSeatMstList();
        }

        public Tuple<short, string> SaveFloorSeat(FloorSeatMstViewModel model, long ActionBy)
        {
            return _smRepo.SaveFloorSeat(model, ActionBy);
        }

        public short UpdateSeatStatus(long SeatId, long ActionBy, short status)
        {
            return _smRepo.UpdateSeatStatus(SeatId, ActionBy, status);
        }

        //Seat booking popup
        public bool isPopUpEnableForDivisionHead(long eCode, long? funDesignation, long? divisionID)
        {
            return _smRepo.isPopUpEnableForDivisionHead(eCode, funDesignation, divisionID);
        }
        public List<SeatingAllocationDetailReportViewModel> GetSeatingAllocationDetail(SearchSeatingDetailViewModel SSVM) // Added by aumento for the SR56983 =============== 
        {
            return _smRepo.GetSeatingAllocationDetail(SSVM);
        }

        public List<SeatingAllocationSummaryReportViewModel> GetSeatingAllocationSummary(SearchSeatViewModel SSVM) // Added by aumento for the SR56983 =============== 
        {
            return _smRepo.GetSeatingAllocationSummary(SSVM);
        }

        public dynamic GetOpList(long typeId) // Added by aumento for the SR56983 =============== 
        {
            return _smRepo.GetOpList(typeId);
        }

        // Added by aumento for the SR56980 ===============
        public List<SeatEmpCalViewModel> GetViewRosterList(long userID, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_dtl, long? OpId, long? DivId)
        {
            return _smRepo.GetViewRosterList(userID, Cal_List, Emp_dtl, OpId, DivId);
        }

        // Added by aumento for the SR56983 ===============
        public List<SelectListViewModel> BindBuilding()
        {
            return _smRepo.BindBuilding();
        }

        // Added by aumento for the SR56983 ===============
        public List<SelectListViewModel> GetOpeLevelList(long typeId) // Added by aumento for the SR56983 ===============
        {
            return _smRepo.GetOpeLevelList(typeId);
        }

        // Start :: Added by aumento : SR100656
        public List<SelectListViewModel> GetDivisionList()
        {
            return _smRepo.GetDivisionList();
        }
        public List<ExtrSeatMstViewModel> GetExtrSeatMstList()
        {
            return _smRepo.GetExtrSeatMstList();
        }

        public short UpdateExtraSeatForDivStatus(long Srno, long ActionBy, short status)
        {
            return _smRepo.UpdateExtraSeatForDivStatus(Srno, ActionBy, status);
        }
        public Tuple<short, string> SaveExtraSeatForDiv(ExtrSeatMstViewModel model, long ActionBy)
        {
            return _smRepo.SaveExtraSeatForDiv(model, ActionBy);
        }
        // End :: Added by aumento : SR100656

        //SR102091 START
        public List<SeatSettingViewModel> GetWFO_Mandatory_Days()
        {
            return _smRepo.GetWFO_Mandatory_Days();
        }
        public bool Update_WFO_Days(string pName, string days, long ModifiedBy)
        {
            return _smRepo.Update_WFO_Days(pName, days, ModifiedBy);
        }
        public List<EmpActiveInactiveforRosterViewModel> GetAssociatedata(SearchSeatViewModel SRVM)
        {
            return _smRepo.GetAssociatedata(SRVM);
        }
        public bool SaveEmployeeRoster(long empCode, int addedBy)
        {
            return _smRepo.SaveEmployeeRoster(empCode, addedBy);
        }
        public int WFO_Mandatory_Days()
        {
            return _smRepo.WFO_Mandatory_Days();
        }
        public List<EmpActiveInactiveforRosterLogViewModel> GetAssociateActiveInactiveRosterHistory(long empId)
        {
            return _smRepo.GetAssociateActiveInactiveRosterHistory(empId);
        }
        public List<MandatoryWFODaysViewModel> GetWFOMandatoryDaysHistory(string Statement, string Description)
        {
            return _smRepo.GetWFOMandatoryDaysHistory(Statement, Description);
        }
        //SR102091 END
    }
}
