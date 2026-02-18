using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.ViewModels;
using ePortal.Infrastructure.Repositories;
using ePortal.Persistence.Admin.Interface;

namespace ePortal.Application.Services
{
    public class UARService : IUARService
    {
        private readonly UARRepository _UARRepo;
        //NavigationMaster nav = new NavigationMaster();
        private readonly INavigationMaster_DAL nav;
        public UARService(UARRepository UARRepo, INavigationMaster_DAL _nav)
        {
            _UARRepo = UARRepo;
            nav = _nav;
        }

        public bool GetEmailandName(long Ecode, out string Email, out string Ename)
        {
            try
            {
                _UARRepo.GetEmailandName(Ecode, out Email, out Ename);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<UAR_AccessReviewViewModel> GetUARList(int logingCode, string Status, string Ename, string Ecode)
        {
            try
            {
                List<UAR_AccessReviewViewModel> data = new List<UAR_AccessReviewViewModel>();
                string strName;
                DataTable dt = new DataTable();
                dt = nav.GetUARList(logingCode, Status, Ename, Ecode);
                data = ConvertDataTableUAR_AccessReviewList(dt);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public List<UAR_ViewModel> UARRequestList(string strEmpcode)
        //{
        //    try
        //    {
        //        List<UAR_ViewModel> data = new List<UAR_ViewModel>();
        //        string strName;
        //        DataTable dt = new DataTable();
        //        dt = nav.UARRequestList_Get(strEmpcode.ToString());
        //        data = ConvertDataTableToList(dt);
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public List<UAR_ViewModel> UARRequestList(string strEmpcode, string LoginCode)
        {
            try
            {
                List<UAR_ViewModel> data = new List<UAR_ViewModel>();
                string strName;
                DataTable dt = new DataTable();
                dt = nav.UARRequestList_Get(strEmpcode.ToString(), LoginCode.ToString());
                data = ConvertDataTableToList(dt);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UAR_ViewModel> ConvertDataTableToList(DataTable dataTable)
        {
            List<UAR_ViewModel> resultList = new List<UAR_ViewModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                UAR_ViewModel item = new UAR_ViewModel
                {
                    ACCESSRIGHTS = row["accessrights"].ToString(),
                    CREATIONDATE = row["CreationDate"].ToString(),
                    SYUSERRIGHTSID = row["syuserrightsid"].ToString(),
                    PARENTPATH = row["ParentPath"].ToString(),
                    Checked = row["Checked"].ToString(),
                    Isvisible = row["Isvisible"].ToString(),
                };

                resultList.Add(item);
            }

            return resultList;
        }

        public List<UAR_AccessReviewViewModel> ConvertDataTableUAR_AccessReviewList(DataTable dataTable)
        {
            List<UAR_AccessReviewViewModel> resultList = new List<UAR_AccessReviewViewModel>();

            foreach (DataRow row in dataTable.Rows)
            {
                UAR_AccessReviewViewModel item = new UAR_AccessReviewViewModel
                {
                    ECODE = row["ECODE"] != DBNull.Value ? Convert.ToInt64(row["ECODE"]) : (long?)null,
                    EMPLOYEE = row["EMPLOYEE"].ToString(),
                    OPERATIONID = Convert.ToInt64(row["OPERATIONID"]),
                    LEVELDESC = row["LEVELDESC"].ToString(),
                    DIVISIONID = row["DIVISIONID"].ToString(),
                    DIVISION = row["DIVISION"].ToString(),
                    DEPARTMENTID = row["DEPARTMENTID"].ToString(),
                    DEPARTMENT = row["DEPARTMENT"].ToString(),
                    USERTYPE = row["USERTYPE"].ToString(),
                    REVIEWER = Convert.ToInt64(row["REVIEWER"]),
                    REVIEWERName = row["REPORTINGMANAGER1"].ToString(),
                    STATUS = row["STATUS"].ToString(),
                    UARID = Convert.ToInt32(row["UARID"]), //SR92683
                    ADDEDON = row["ADDEDON"].ToString(), //SR92683
                    SELF_REVIEWED_ON = row["SELF_REVIEWED_ON"].ToString(), //SR92683
                    REVIEWED_ON = row["REVIEWED_ON"].ToString() //SR92683
                };

                resultList.Add(item);
            }

            return resultList;
        }

        public List<UAR_AccessReviewViewModel> GetEmpNameList(long strEmpcode)
        {
            List<UAR_AccessReviewViewModel> resultList = new List<UAR_AccessReviewViewModel>();
            resultList = _UARRepo.GetEmpNameList(strEmpcode);
            return resultList;
        }

        public string ReviewListData(string EmpCode, string strActivityID, string strBy, string ACCESSTYPE)// ACCESSTYPE added for SR78412)
        {
            try
            {
                string res = Convert.ToString(nav.DeleteNavRights(EmpCode, strActivityID, strBy, ACCESSTYPE)); // ACCESSTYPE added for SR78412));
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string DeActiveUser(string EmpCode, string modifiedBy)
        {
            try
            {
                string res = nav.DeActiveUser(EmpCode, modifiedBy);

                return res;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        public string UpdateADUARRequest(long AdEmpCode)
        {
            try
            {
                string res = _UARRepo.UpdateADUARRequest(AdEmpCode);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string GetSYKIID()
        {
            try
            {
                return _UARRepo.GetSYKIID();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //SR78412 Changes Start
        public List<UAR_AccessReviewViewModel> GetUARRegularList(int logingCode, string Status, string Ename, string Ecode)
        {
            try
            {
                List<UAR_AccessReviewViewModel> data = new List<UAR_AccessReviewViewModel>();
                string strName;
                DataTable dt = new DataTable();
                dt = nav.GetUARRegularList(logingCode, Status, Ename, Ecode);
                data = ConvertDataTableUAR_AccessReviewList(dt);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UAR_Regular_ViewModel> UARRegularRequestList(string strEmpcode, string REVIEWER)
        {
            try
            {
                List<UAR_Regular_ViewModel> data = new List<UAR_Regular_ViewModel>();
                string strName;
                DataTable dt = new DataTable();
                dt = nav.UARRegularRequestList_Get(strEmpcode.ToString(), REVIEWER.ToString());
                data = ConvertRegularDataTableToList(dt);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ReviewRegularListData(string EmpCode, string strActivityID, string strBy, string ACCESSTYPE, int UserChecked) //SR102715
        {
            try
            {
                string res = Convert.ToString(nav.DeleteRegularNavRights(EmpCode, strActivityID, strBy, ACCESSTYPE, UserChecked)); //SR102715
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public List<UAR_Regular_ViewModel> ConvertRegularDataTableToList(DataTable dataTable)
        {
            List<UAR_Regular_ViewModel> resultList = new List<UAR_Regular_ViewModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                UAR_Regular_ViewModel item = new UAR_Regular_ViewModel
                {
                    ACCESSRIGHTS = row["accessrights"].ToString(),
                    CREATIONDATE = row["CreationDate"].ToString(),
                    SYUSERRIGHTSID = row["syuserrightsid"].ToString(),
                    PARENTPATH = row["ParentPath"].ToString(),
                    Checked = row["Checked"].ToString(),
                    Isvisible = row["Isvisible"].ToString(),
                    USERCHECKED = row["USERCHECKED"].ToString(),
                    ACCESSTYPE = row["ACCESSTYPE"].ToString(),
                };

                resultList.Add(item);
            }

            return resultList;
        }
        public string UpdateADUARRegularRequest(long AdEmpCode)
        {
            try
            {
                string res = _UARRepo.UpdateADUARRegularRequest(AdEmpCode);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public List<UAR_Regular_ViewModel> UARRegularReviewerRequestList(string strEmpcode, string REVIEWER)
        {
            try
            {
                List<UAR_Regular_ViewModel> data = new List<UAR_Regular_ViewModel>();
                string strName;
                DataTable dt = new DataTable();
                dt = nav.UARRegularReviewerRequestList_Get(strEmpcode.ToString(), REVIEWER.ToString());
                data = ConvertRegularDataTableToList(dt);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ReviewRegularReviewerListData(string EmpCode, string strActivityID, string strBy, string ACCESSTYPE) // ACCESSTYPE added for SR78412
        {
            try
            {
                string res = Convert.ToString(nav.DeleteNavRights(EmpCode, strActivityID, strBy, ACCESSTYPE)); // ACCESSTYPE added for SR78412
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string SendBackADUARRequest(long AdEmpCode, long REVIEWER)
        {
            try
            {
                string res = _UARRepo.SendBackADUARRequest(AdEmpCode, REVIEWER);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public List<UAR_AccessReviewViewModel> GetUARStatusList(int logingCode, string Status, string Ename, string Ecode, string SYKIID)
        {
            try
            {
                List<UAR_AccessReviewViewModel> data = new List<UAR_AccessReviewViewModel>();
                string strName;
                DataTable dt = new DataTable();
                dt = nav.GetUARStatusList(logingCode, Status, Ename, Ecode, SYKIID);
                data = ConvertDataTableUAR_AccessReviewList(dt);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SYKI> GetKICodeList(long userId)
        {
            return _UARRepo.GetKICodeList(userId);
        }
        //SR78412 Changes End
        //SR92683 Changes Start
        public string GetSelfReviewRemarks(long AdEmpCode, long REVIEWER)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = nav.GetSelfReviewRemarks(AdEmpCode, REVIEWER);
                string res = "";
                if (dt.Rows.Count > 0)
                {
                    res = dt.Rows[0][0].ToString();
                }
                return res;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string UpdateSelfReviewRemarks(long AdEmpCode, String SelfRemarks)
        {
            try
            {
                string res = "";
                res = nav.UpdateSelfReviewRemarks(AdEmpCode, SelfRemarks);
                return res;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string GetReviewerRemarks(long AdEmpCode, long REVIEWER)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = nav.GetReviewerRemarks(AdEmpCode, REVIEWER);
                string res = "";
                if (dt.Rows.Count > 0)
                {
                    res = dt.Rows[0][0].ToString();
                }
                return res;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string UpdateReviewerRemarks(long AdEmpCode, String Remarks)
        {
            try
            {
                string res = "";
                res = nav.UpdateReviewerRemarks(AdEmpCode, Remarks);
                return res;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public List<UAR_Regular_ViewModel> UARRegularRequestHistoryList(String UARID)
        {
            try
            {
                List<UAR_Regular_ViewModel> data = new List<UAR_Regular_ViewModel>();
                DataTable dt = new DataTable();
                dt = nav.UARRegularRequestHistoryList_Get(UARID);
                data = ConvertRegularDataTableToList(dt);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //SR92683 Changes End
    }
}
