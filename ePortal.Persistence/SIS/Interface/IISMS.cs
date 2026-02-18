using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ePortal.ViewModels;

namespace ePortal.Persistence.SIS.Interface
{
    public interface IISMS
    {
        DataTable GetISMSDetail(string strismsId, string strismsdes, string strismsstatus);
        DataTable BindRepeater();
        DataTable GetISMSViewDetails(Int32 ID);
        DataTable GetISMSDTL(string strismsdtlId, string strismsId, string strismsdtldes, string strismsdtlstatus);
        DataTable GetCategory(String Strflag);
        DataTable GetMILLERDetail(string strmaillerid, string strcat, string stryear, string strmonth, string strdes, string strstatus);
        DataTable GetPolicyCategory();
        DataTable GetITProcedureCategory();
        DataTable GetUserPolicyCategory();
        DataTable GetUserProcedureCategory();
        String UpdateISMS(string strTransid, string strDescription, string strStatus, string strDisplay, string StrAttachment, string strAddedby, string StrCat, string StrYear, string StrMonth);
        String ISMSDetail_Set(string strDetailid, string strId, string strDescription, string strStatus, string StrAttachment, string strAddedby, string Strcate, string Stryear, string Strmonth);
        DataTable GetDescription_Get(string description);
        DataTable GetISMSAnnonacement();
        DataTable GetDescriptionIDValue(int proc_id_IN);
        DataTable DocumentType_Get();
        DataTable PolicyProcedure_Get(string strId, string strType, string strStatus);
        String PolicyProcedure_Set(string strId, string strDescription, string strCode, string strType, string strStatus, string StrAttachment, string strAddedby, string Strcategory, string Stryear, string Strmonth, string Strday);
        DataTable SearchCatYearMon_Get(string strId, string strddcategory, string stryear, string strmonth);
        String ISMSANNOUCEMENT_Set(string strAnnouceid, string strDescription, string strDetails, string Strstatus, string strAddedby, string StrISMSId, string StrYear, string StrMonth);
        DataTable GetISMSANNOUCEMENTDetail(string strannouceId);
        DataTable GetISMSANNOUCEMENTDetail_GET(string strannouceId);
        String ISMSFAQ_Set(string strDetailid, string strismsId, string strDescription, string strcat, string stryear, string strmonth, string strStatus, string strAddedby, string stranswers);
        DataTable GetISMSFAQ_GET(string strfaqid);
        DataTable GetISMSFAQ_GETEDIT(string strfaqid);
        string GetISMSMAILLER_SET(string strmaillerId, string strdescription, string strismsstatus, string StrAttachment, string strAddedby, string Strcate, string Stryear, string Strmonth, string Strday);
        DataTable GetISMSMailler_GET(string strmaillerId, string strcat, string stryear, string strmonth, string strismsdes, string strismsstatus);
        DataTable GetISMSMailler_GETSearch(string strmaillerId, string strcat, string stryear, string strmonth, string strismsstatus, string strday);
        DataTable GetISMSMailler_GETID(string strmaillerId);
        DataTable GetISMSANNOUCEMENT_GETSearch(string strmaillerId, string stryear, string strmonth, string strismsstatus);
        DataTable GetISMSFAQ_GETSearch(string strmaillerId, string strcat, string stryear, string strmonth, string strismsstatus);
        String CheckList_Set(string strcheckId, string strDescription, string strStatus, string StrAttachment, string strAddedby, string Strcategory, string Stryear, string Strmonth, string Strday);
        DataTable CheckList_GETID(string strcheckId);
        DataTable CheckList_GETDETAILS();
        DataTable CheckList_SEARCH(string strchkId, string strcat, string stryear, string strmonth, string strStatus, string Strday);
        DataTable PolicyProcedure_GetDETAILS();
        DataTable PolicyProcedure__SEARCH(string strdoc, string strcat, string stryear, string strmonth, string strStatus, string Strday);
        DataTable PolicyProcedure_GetITPOLICYDETAILS();
        DataTable ITPolicyProcedure__SEARCH(string strcat, string stryear, string strmonth, string strStatus);
        DataTable PolicyProcedure_GetITPROCEDEL();
        DataTable ITProcedure__SEARCH(string strcat, string stryear, string strmonth, string strStatus);
        DataTable PolicyProcedure_GetUserPolciyDEL();
        DataTable UserPolicy__SEARCH(string strcat, string stryear, string strmonth, string strStatus);
        DataTable PolicyProcedure_GetUserProcedureDEL();
        DataTable UserProcedure__SEARCH(string strcat, string stryear, string strmonth, string strStatus);
        DataTable GetMonthCategory();
        DataTable GetMaileryearSearh();
        DataTable GetISMSMailler_GETSearchUser(string strmaillerId, string strcat, string stryear, string strmonth, string strismsstatus);
        DataTable MailerofMonth();
        DataTable GetISMSFAQUSER();
        DataTable CheckList_SEARCHUSER(string strchkId, string strcat, string stryear, string strmonth, string strStatus);
        DataTable SearchMonthChecklistCategory();
        DataTable UserPolicyMonthCategory();
        DataTable UserProcedureMonthCategory();
        DataTable ITPOLICYMonthCategory();
        DataTable ITPROCEDUREMonthCategory();
        List<ISMSMENUVM> GetMenu(string strismsId, string strismsdes, string strismsstatus);
        List<MenuISMSViewModel> GetISMSMenu();
        List<SIS_ISMS> GetSisIsmsList();
        List<ISMS_Info_Security> GetInfoSecurityList();
       
    }
}
