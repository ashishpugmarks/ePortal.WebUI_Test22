using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Services
{
    public class ISMSMasterService : IISMSMaster
    {
        ISMSMasterRepository _objISMSMasterRepositry;
        CommonRepository objcommRespository;
        public ISMSMasterService(ISMSMasterRepository objISMSMasterRepositry, CommonRepository _objcommRespository)
        {
            _objISMSMasterRepositry = objISMSMasterRepositry;
            objcommRespository = _objcommRespository;
        }

        public IEnumerable<Employee_Details> BindAppAuth1()
        {
            return _objISMSMasterRepositry.BindAppAuth1().ToList();
        }

        public Int16 SaveProcessAttachment_Trn(ISMSMasterViewModel AVM)
        {
            return _objISMSMasterRepositry.SaveProcessAttachment_Trn(AVM);
        }

        List<ISMSMasterViewModel> IISMSMaster.GetISMSMasterList(ISMSMasterViewModel SVM)
        {
            List<ISMSMasterViewModel> ISMSMSTList = new List<ISMSMasterViewModel>();

            if (SVM == null)
            {
                ISMSMSTList = _objISMSMasterRepositry.GetISMSMasterList();
            }            
           
            return ISMSMSTList.OrderByDescending(x => x.SRNO).ToList();
        }

        public ISMSMasterViewModel GetISMSMasterDetails(int SrNo)
        {
            return _objISMSMasterRepositry.GetISMSMasterDetails(SrNo);
        }

        public FileViewModel GetFileForDownload(Int64 id, string type)
        {
            return _objISMSMasterRepositry.GetFileForDownload(id, type);
        }

        public Int16 UpdateProcessAttachment_Trn(ISMSMasterViewModel AVM)
        {
            return _objISMSMasterRepositry.UpdateProcessAttachment_Trn(AVM);
        }

        public ISMSMasterViewModel GetISMSMSTRequestById(long id)
        {
            return _objISMSMasterRepositry.GetISMSMSTRequestById(id);
        }

        public short ISMSMasterAppr(ISMSHDRDTLViewModel PHVM, Employee_Details emp_dtl)
        {
            short retVal = _objISMSMasterRepositry.ISMSMasterAppr(PHVM, emp_dtl);
            if (retVal == 1)
            {
                //SendMailByApprovalAuthority(IHVM.IOMID, IHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }

        public short CancelReqById(long id)
        {
            return _objISMSMasterRepositry.CancelReqById(id);
        }

        //Added by aumento as on 08082024 for the SR72656 -----------------------------------------------------
        public string GetActiveLink()
        {
            return _objISMSMasterRepositry.GetActiveLink();
        }
        //------------------------------------------------------------------------------------------------------
    }
}
