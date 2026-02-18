using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IISMSMaster
    {

        IEnumerable<Employee_Details> BindAppAuth1();

        Int16 SaveProcessAttachment_Trn(ISMSMasterViewModel AVM);

        List<ISMSMasterViewModel> GetISMSMasterList(ISMSMasterViewModel SVM);

        ISMSMasterViewModel GetISMSMasterDetails(int SrNo);

        FileViewModel GetFileForDownload(Int64 id, string type);

        Int16 UpdateProcessAttachment_Trn(ISMSMasterViewModel AVM);

        ISMSMasterViewModel GetISMSMSTRequestById(long id);

        short ISMSMasterAppr(ISMSHDRDTLViewModel PHVM, Employee_Details emp_dtl);

        short CancelReqById(long id);

        string GetActiveLink();
       
    }
}
