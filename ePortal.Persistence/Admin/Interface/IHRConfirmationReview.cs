using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IHRConfirmationReview
    {
        DataTable GetTraitsList();
        DataTable GetTraitsAttributes(String strTraits, String ConfirmationFormID);
        DataSet GetEmpDetails(String strAdempcode);
        DataSet GetProcessDetails(String strProcessID);
        DataTable GetConfirmationFormRatings(String confirmationFormID);
        DataTable GetEmployeeListByConfirmationDateMonth(String Year, String Month, String Ecode, String Status, String Site);
        DataTable GetConfirmationList(String Month, String UserCode, String OperationID, String DivisionID, String DepratmentID);
        DataTable GetConfirmationHistory(String Month, String OperationID, String DivisionID, String DepratmentID);
        DataTable GetEmployeeMailDetails(String Adempcode, String ProcessID, String ReviewID);
        DataTable GetPendingConfirmationWithReviewAuth(String Year, String Month, String Ecode, String Status, String Site);
        int ValidateReviewAuthUpdate(String Adempcode, out int IsValidUpdate);
        string GetHRMailDetails();
        int CheckConfirmationMenuLinkRights(String Adempcode);
        DataTable GetConfirmationpending(String UserCode);
        DataTable GetConfirmationApproved(String UserCode);
        void InsertConfirmationReviewTransaction(FormDetails details);
        int UpdateHRStatus(String strAdempCode, String strProcessID, String strReviewFormID,
                                  String strProcessStatus, String strHRStatus, String strRemarks,
                                  String HRApprovedStatus, String strByUserCode);
        int UpdateReviewAuthorities(String HrConfirmationReviewID, String Adempcode, String Reviewer1,
                                            String Reviewer2, String Reviewer3, String strByUserCode);
        int InsertProcessRequest(String strAdempCode);
    }
}
