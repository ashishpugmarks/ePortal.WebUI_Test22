using ePortal.ViewModels;
using System;
using System.Collections.Generic;

namespace ePortal.Application.Contracts
{

    public interface ISeatMgmtService
    {

        bool IsOffDay(string date, long siteId, long operationId);
        string GetParmValByParmName(string parmName);
        List<SeatSettingViewModel> GetParmListByName();
        //Tuple<int, int, int,int> GetSeatCountByDivID(long DivisionID, long SiteID);//Added by aumento : SR100656 //Commented for SR102091
        Tuple<int, int, int, int> GetSeatCountByDivID(long DivisionID, long SiteID, int NewJoineeCount, int InactiveCount);//SR102091 Added NewJoineeCount,InactiveCount
        //Tuple<int, int, int> GetSeatCountByOpID(long OpID, long SiteID); //Commented for SR102091
        Tuple<int, int, int> GetSeatCountByOpID(long OpID, long SiteID, int NewJoineeCount, int InactiveCount);//SR102091 Added NewJoineeCount,InactiveCount
        List<SeatEmpCalViewModel> GetEmpList(long userID, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl);
        Tuple<int, int> GetNewJoineeAndInactiveCount(List<SeatEmpCalViewModel> empList, List<SeatCalenderViewModel> HeaderList, int parmValue);//SR102091
        List<SeatEmpCalViewModel> GetEmpListByOpHead(long userID, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl);
        Tuple<short, string> SaveRoster(List<SeatRosterViewModel> ModelList, long ActionBy);
        List<SeatEmpCalViewModel> ViewRosterEmpList(long userID, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_dtl);
        string GetMapFilename(long floorID);

        //// Master
        List<Employee_Details> AutocompleteEmployee(string term);
        List<SeatAllocationViewModel> AutocompleteSeat(string term, long eCode);
        List<FixSeatViewModel> GetFixSeatList();
        Tuple<short, string> SaveFixMapping(long mapId, long eCode, string seatNo, long ActionBy, short status);
        short UpdateStatus(long mapId, long ActionBy, short status);
        List<SelectListViewModel> GetOrgLevelList(long typeId);
        List<SeatingReportViewModel> GetSeatingReport(SearchSeatViewModel SSVM);
        List<DivWiseSeatViewModel> GetDivisionWiseSeatList();
        DivWiseSeatViewModel GetDivisionWiseSeatById(long dwsId);
        Tuple<short, string> SaveDivisionWiseSeat(DivWiseSeatViewModel model, long ActionBy);
        List<SelectListViewModel> GetFloorOperationList();
        int GetFloorOperationSeatCount(long opId);
        List<SelectListViewModel> BindDivision(long opId);
        List<SeatEmpCalViewModel> GetDivisionWiseCountReport(List<SeatCalenderViewModel> CAL_LIST);
        List<SelectListViewModel> GetFloorList();

        List<FloorOperationMapViewModel> GetFloorOperationMapList();
        Tuple<short, string> SaveFloorOpMapping(FloorOperationMapViewModel model, long ActionBy);
        //short UpdateOpMappingStatus(long mapId, long ActionBy, short status); //Added by Aumento :: SR100223
        short UpdateOpMappingStatus(long mapId, long ActionBy, short status, string IsSeatAutoUpdated); //Added by Aumento :: SR100223

        List<FloorSeatMstViewModel> GetFloorSeatMstList();
        Tuple<short, string> SaveFloorSeat(FloorSeatMstViewModel model, long ActionBy);
        short UpdateSeatStatus(long SeatId, long ActionBy, short status);

        bool isPopUpEnableForDivisionHead(long eCode, long? funDesignation, long? divisionID); //Seat booking popup


        // Added by aumento for the SR56983 ===============
        List<SeatingAllocationDetailReportViewModel> GetSeatingAllocationDetail(SearchSeatingDetailViewModel SSVM);

        // Added by aumento for the SR56983 ===============
        List<SeatingAllocationSummaryReportViewModel> GetSeatingAllocationSummary(SearchSeatViewModel SSVM);

        // Added by aumento for the SR56983 ===============
        dynamic GetOpList(long typeId);

        // Added by aumento for the SR56980 ===============
        List<SeatEmpCalViewModel> GetViewRosterList(long userID, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_dtl, long? OpId, long? DivId);

        List<SelectListViewModel> BindBuilding();  // Added by aumento for the SR56983 ===============

        List<SelectListViewModel> GetOpeLevelList(long typeId);  // Added by aumento for the SR56983 ===============

        // Start :: Added by aumento : SR100656
        List<SelectListViewModel> GetDivisionList();

        List<ExtrSeatMstViewModel> GetExtrSeatMstList();

        short UpdateExtraSeatForDivStatus(long Srno, long ActionBy, short status);

        Tuple<short, string> SaveExtraSeatForDiv(ExtrSeatMstViewModel model, long ActionBy);
        // End :: Added by aumento : SR100656
        //SR102091 START
        List<SeatSettingViewModel> GetWFO_Mandatory_Days();

        bool Update_WFO_Days(string pName, string days, long ModifiedBy);

        List<EmpActiveInactiveforRosterViewModel> GetAssociatedata(SearchSeatViewModel SRVM);

        bool SaveEmployeeRoster(long empCode, int addedBy);

        int WFO_Mandatory_Days();
        List<EmpActiveInactiveforRosterLogViewModel> GetAssociateActiveInactiveRosterHistory(long empId);

        List<MandatoryWFODaysViewModel> GetWFOMandatoryDaysHistory(string Statement, string Description);

        //SR102091 END
    }
}
