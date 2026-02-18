using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface ICalenderMaster
    {

        IEnumerable<SYSITE> Bind_SYSite();
        IEnumerable<Employee_Details> BindAppAuth1();

        List<CalenderMasterViewModel> GetCalenderMasterList(CalenderSearchViewModel SVM);
        List<CalenderMasterViewModel> GetCalenderMasterListForUser(CalenderSearchViewModel SVM);
        Int16 SaveProcessAttachment_Trn(CalenderMasterViewModel AVM);

        short SendMailByApprovalAuthority(CalenderMasterViewModel AVM, Employee_Details emp_dtl);

        CalenderMasterViewModel GetCalenderMasterDetails(int SrNo);

        Int16 UpdateProcessAttachment_Trn(CalenderMasterViewModel AVM);

        FileViewModel GetFileForDownload(Int64 id, string type);
        CalenderMasterViewModel GetCalenderMSTRequestById(long id);

        short CalenderMasterAppr(CALENDERMASTERMAPPINGTRNViewModel PHVM, Employee_Details emp_dtl);

        short DeactiveReqById(long id);





        //// Content Process Master
        //#region Content Process Master
        //IEnumerable<ProcessMstViewModel> GetProcess_Mst_List();
        //IEnumerable<ProcessMstViewModel> BindContentProcess();

        ////#endregion


        //#region Process Attachment Trn
        //List<CreativeMasterViewModel> GetCorporateNewsMasterList(SearchViewModel SVM);
        //Int16 SaveProcessAttachment_Trn(CreativeMasterViewModel AVM);
        //Int16 UpdateProcessAttachment_Trn(CreativeMasterViewModel AVM);

        //CreativeMasterViewModel GetCorporateNewsDetails(int id);

        //CreativeMasterViewModel GetCreativeMSTRequestById(long id);

        //short CreativeMasterAppr(CM_Processattachmentappmapping_TrnViewModel PHVM, Employee_Details emp_dtl);
        //CreativeMasterViewModel DeleteDocument(Int64 id, string type);

        //List<Employee_Details> PortalAutocompleteSuggestions(string term);


        //#endregion
    }
}
