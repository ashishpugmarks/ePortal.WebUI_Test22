using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IBenevolent
    {
        List<BenevolentViewModel> GetBenevolentMSTList(SearchBenevolent sr);
        List<BENEVOLENT_MST> GetBenevolentMST();
        BENEVOLENT_MST GetEmpBenevolentMST(long id);

        short AddBenevolentMST(BENEVOLENT_MST mst);

        short EditBenevolentMST(BENEVOLENT_MST mst);
        List<Employee_Details> PortalAutocompleteSuggestions(string term, string designation);

        BenevolentViewModel GetEmpDetail(long id);

        short AddContribution(BENEVOLENT_DT dt);

        List<BenevolentViewModel> ContributionBenMSTList(SearchBenevolent sr);

        List<BenevolentViewModel> GetContributionReport(SearchBenevolent sr);

        short SentMailToVendor();

        int GetEmpCode(long id);

        BenevolentViewModel GetContributionReportDetail(long id);

        BenevolentViewModel GetDemiseEmployeeDetail(long id);

    }
}
