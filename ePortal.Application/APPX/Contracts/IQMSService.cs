
using ePortal.ViewModels.APPX.QMS;

namespace ePortal.Application.APPX.Contracts
{
    public interface IQMSService
    {

        QMSOperationResult ISODOCDetail_Set(ISODocSaveModel model);
        QMSOperationResult ISODOCDetail_UPDATE(ISODocUpdateModel model);
        QMSOperationResult ISODocRequestAppReject(ISODocRequestAppRejectSaveModel model);

        IEnumerable<SubQcProcViewModel> SubQcProc(string ID);
        IEnumerable<DeptSectProcViewModel> DeptSectProcList(string DEPTID, string PLANTID);
        IEnumerable<GrdDeptProcViewModel> GrdDeptProcList(string? plant, string? DEPTID);

    }
}
