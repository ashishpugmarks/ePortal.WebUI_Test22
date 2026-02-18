using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface ICreativeMaster
    {
        //// Content Process Master
        //#region Content Process Master
        //IEnumerable<ProcessMstViewModel> GetProcess_Mst_List();
        IEnumerable<ProcessMstViewModel> BindContentProcess();
        
        //#endregion

       
        #region Process Attachment Trn
        List<CreativeMasterViewModel> GetCorporateNewsMasterList(SearchViewModel SVM);
        Int16 SaveProcessAttachment_Trn(CreativeMasterViewModel AVM);
        Int16 UpdateProcessAttachment_Trn(CreativeMasterViewModel AVM);
       
        CreativeMasterViewModel GetCorporateNewsDetails(int id);

        CreativeMasterViewModel GetCreativeMSTRequestById(long id);

        short CreativeMasterAppr(CM_Processattachmentappmapping_TrnViewModel PHVM, Employee_Details emp_dtl);
        CreativeMasterViewModel DeleteDocument(Int64 id, string type);
       
        List<Employee_Details> PortalAutocompleteSuggestions(string term);

        IEnumerable<Employee_Details> BindAppAuth1();
        #endregion
    }
}
