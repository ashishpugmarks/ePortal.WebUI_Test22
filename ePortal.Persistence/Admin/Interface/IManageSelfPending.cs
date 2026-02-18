using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IManageSelfPending
    {
        long GetTotalSelfPending(Int32 strEmpCode);
        DataTable GetPendingTraning(string strEmpCode);
        DataTable GetTourSettPendinglist(string strEmpcode);
        DataTable ResigClearancePending(string strecode);
        DataTable ResigExitProcessPending(string strecode);
        DataTable IncomeTaxDecPending(string strecode);
        DataTable SuperiorEvalPending(string strecode);
        DataTable CarPerkSystemPending(string strecode);
        DataTable RecruitmentJobDescPending(string strecode);
        DataTable GoalSettingSelfPending(string strecode);
        DataTable GetGoalSettEvalPending(string strecode);
        DataTable GetGoalSettReviewerPending(string strecode);
        DataTable FirstHalfSelfPending(string strecode);
        DataTable GetFirsstHalfEvalPending(string strecode);
        DataTable GetFirsstHalfReviewPending(string strecode);
        DataTable GetSecondHalfPending(string strecode);
        DataTable SecondHalfEvalPending(string strecode);
        DataTable ReviewerPending(string strecode);
        DataTable NormalizationFHDivPending(string strecode);
        DataTable NormalizationFHOPPending(string strecode);
        DataTable NormalizationSHDivPending(string strecode);
        DataTable NormalizationSHOPPending(string strecode);
        DataTable FetchWFHReqPending(string strecode);
    }
}
