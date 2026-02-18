using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IWFHService
    {
        List<ShiftViewModel> GetShiftBySite(long SiteId, long? id);
        WFHAppAuthViewModel GetApprovalAuth(long loginUser);
        List<ASRWFH_HEADER_ViewModel> GetWFHRequestList(ASRWFH_HEADER_ViewModel AVM);
        ASRWFH_HEADER_ViewModel GetWFHRequestDetail(long id);
        short SaveWFHRequest(ASRWFH_HEADER_ViewModel AVM);
        short EditWFHRequest(ASRWFH_HEADER_ViewModel AVM);
        short UpdateWFHApproval(ASRWFH_APPROVALHIS_ViewModel AAVM, Employee_Details employeeDetails);
        short UpdateWFHApprovalList(List<ASRWFH_APPROVALHIS_ViewModel> AAVMList, Employee_Details _Employee_Details);
        short GetOffDays(string startDate, string endDate, long siteId, long operationId);
        List<ASRWFH_HEADER_ViewModel> GetWFHReport(WFHReportViewModel WRVM);
        List<ADORGLEVEL> GetOrgLevelList(long typeId);

        List<WFHDivViewModel> BindDivision( long op_Id);
        List<WFHDepViewModel> BindDepartment(long div_Id, long op_Id);
        List<WFHSecViewModel> BindSection(long dep_Id, long div_Id, long op_Id);
        ASRWFH_HEADER_ViewModel GetRejectedRequest(string startDate, string endDate, long EmpCode);
        WFHAppAuthViewModel GetViewApprovalAuth(long _ReqID); //--WFH issue correction

        //SR86752 Start
        List<ASRWFH_HEADER_ViewModel> GetWFHAdminReport(WFHReportViewModel WRVM);
        //SR86752 End

    }
}
