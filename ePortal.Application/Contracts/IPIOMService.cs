using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IPIOMService
    {
        Tuple<short, long> SavePIOMRequest(IOMPHeaderViewModel model);
        Tuple<short, List<IOMPDetailViewModel>> SavePIOMAttachment(long addedDate, long iomHid, List<IOMPDetailViewModel> modelList);
        Tuple<short, List<IOMPDetailViewModel>> DeletePIOMAttachment(string fileName, string docType, long iomHid);
        IOMPHeaderViewModel GetPIOMRequestById(long id);
        short PIOMApproval(IOMPAppHistoryViewModel IHVM, Employee_Details emp_dtl, IOMPHeaderViewModel iomObj);
        short SendMailByApprovalAuthorityForHoldRequest(IOMPHeaderViewModel Model, Employee_Details employeeDetails);
        short PIOMCancel(IOMPHeaderViewModel PHVM);
        List<IOMPHeaderViewModel> GetPIOMPrevAuthority(long Adempcode);
        public short SendMailAnnotatedPdfOnFinalApproval(IOMPAppHistoryViewModel IHVM, short approvalStatus, Employee_Details employeeDetails, string attachment);
        public Tuple<short, long> SaveAdditionalPIOMRequest(IOMPHeaderViewModel model);

        long GetPIOMNextApprovalId(long IOMID, long ecode);
    }
}
