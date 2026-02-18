using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class CalenderMasterService : ICalenderMaster
    {
        private readonly CalenderMasterRepository _objCalenderMasterRepositry;
        private readonly CommonRepository objcommRespository;
        public CalenderMasterService(CalenderMasterRepository objCalenderMasterRepositry, CommonRepository _objcommRespository)
        {
            _objCalenderMasterRepositry = objCalenderMasterRepositry;
            objcommRespository = _objcommRespository;
        }

        public IEnumerable<SYSITE> Bind_SYSite()
        {
            return _objCalenderMasterRepositry.Bind_SYSite();
        }
        public IEnumerable<Employee_Details> BindAppAuth1()
        {
            return _objCalenderMasterRepositry.BindAppAuth1().ToList();
        }
        List<CalenderMasterViewModel> ICalenderMaster.GetCalenderMasterList(CalenderSearchViewModel SVM)
        {
            List<CalenderMasterViewModel> CalenderMSTList = new List<CalenderMasterViewModel>();

            if (SVM == null)
            {
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList();
            }
            //if (SVM.SelectedFinancialYear == null && SVM.CALENDER == null && SVM.LOCATION == null)
            //{
            //    return CalenderMSTList;
            //}
            else if (SVM.SelectedFinancialYear != null && SVM.CALENDER != null && SVM.LOCATION != null)
            {
                var SelectedFinancialYear = SVM.SelectedFinancialYear;
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList();
                CalenderMSTList = CalenderMSTList.Where(x => x.FINANCIALYEAR == SelectedFinancialYear && x.CALENDER == SVM.CALENDER && x.LOCATION.Contains(SVM.LOCATION)).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear != null && SVM.CALENDER != null && SVM.LOCATION == null)
            {
                var SelectedFinancialYear = SVM.SelectedFinancialYear;
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList();
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList().Where(x => x.FINANCIALYEAR == SelectedFinancialYear && x.CALENDER == SVM.CALENDER).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear == null && SVM.CALENDER != null && SVM.LOCATION != null)
            {
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList();
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList().Where(x => x.CALENDER == SVM.CALENDER && x.LOCATION.Contains(SVM.LOCATION)).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear != null && SVM.CALENDER == null && SVM.LOCATION != null)
            {
                var SelectedFinancialYear = SVM.SelectedFinancialYear;
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList();
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList().Where(x => x.FINANCIALYEAR == SelectedFinancialYear && x.LOCATION.Contains(SVM.LOCATION)).OrderByDescending(x => x.SRNO).ToList();

            }
            else if (SVM.SelectedFinancialYear != null && SVM.CALENDER == null && SVM.LOCATION == null)
            {
                var SelectedFinancialYear = SVM.SelectedFinancialYear;
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList();
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList().Where(x => x.FINANCIALYEAR == SelectedFinancialYear).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear == null && SVM.CALENDER != null && SVM.LOCATION == null)
            {
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList().Where(x => x.CALENDER == SVM.CALENDER).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear == null && SVM.CALENDER == null && SVM.LOCATION != null)
            {
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList();
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList().Where(x => x.LOCATION.Contains(SVM.LOCATION)).OrderByDescending(x => x.SRNO).ToList();
            }
            else
            {
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterList();
                return CalenderMSTList.OrderByDescending(x => x.SRNO).ToList();
            }
            return CalenderMSTList.OrderByDescending(x => x.SRNO).ToList();
        }



        List<CalenderMasterViewModel> ICalenderMaster.GetCalenderMasterListForUser(CalenderSearchViewModel SVM)
        {
            List<CalenderMasterViewModel> CalenderMSTList = new List<CalenderMasterViewModel>();

            if (SVM == null)
            {

                DateTime currentDate = DateTime.Now;
                var SelectedFinancialYear = "";
                int startYear = currentDate.Month >= 4 ? currentDate.Year : currentDate.Year - 1;
                SelectedFinancialYear = $"{startYear}-{startYear + 1}";

                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser().Where(x => x.FINANCIALYEAR == SelectedFinancialYear).OrderByDescending(x => x.SRNO).ToList();
            }

            else if (SVM.SelectedFinancialYear != null && SVM.CALENDER != null && SVM.LOCATION != null)
            {
                var SelectedFinancialYear = SVM.SelectedFinancialYear;
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser().Where(x => x.FINANCIALYEAR == SelectedFinancialYear && x.CALENDER == SVM.CALENDER && x.LOCATION.Contains(SVM.LOCATION)).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear != null && SVM.CALENDER != null && SVM.LOCATION == null)
            {
                var SelectedFinancialYear = SVM.SelectedFinancialYear;
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser().Where(x => x.FINANCIALYEAR == SelectedFinancialYear && x.CALENDER == SVM.CALENDER).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear == null && SVM.CALENDER != null && SVM.LOCATION != null)
            {
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser().Where(x => x.CALENDER == SVM.CALENDER && x.LOCATION.Contains(SVM.LOCATION)).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear != null && SVM.CALENDER == null && SVM.LOCATION != null)
            {
                var SelectedFinancialYear = SVM.SelectedFinancialYear;
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser().Where(x => x.FINANCIALYEAR == SelectedFinancialYear && x.LOCATION.Contains(SVM.LOCATION)).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear != null && SVM.CALENDER == null && SVM.LOCATION == null)
            {
                var SelectedFinancialYear = SVM.SelectedFinancialYear;
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser().Where(x => x.FINANCIALYEAR == SelectedFinancialYear).OrderByDescending(x => x.SRNO).ToList();
            }

            else if (SVM.SelectedFinancialYear == null && SVM.CALENDER != null && SVM.LOCATION == null)
            {
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser().Where(x => x.CALENDER == SVM.CALENDER).OrderByDescending(x => x.SRNO).ToList();
            }
            else if (SVM.SelectedFinancialYear == null && SVM.CALENDER == null && SVM.LOCATION != null)
            {
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser().Where(x => x.LOCATION.Contains(SVM.LOCATION)).OrderByDescending(x => x.SRNO).ToList();
            }
            else
            {
                //DateTime currentDate = DateTime.Now;
                //var SelectedFinancialYear = "";
                //int startYear = currentDate.Month >= 4 ? currentDate.Year : currentDate.Year - 1;
                //SelectedFinancialYear = $"{startYear}-{startYear + 1}";

                //CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser().Where(x => x.FINANCIALYEAR == SelectedFinancialYear).OrderByDescending(x => x.SRNO).ToList();
                CalenderMSTList = _objCalenderMasterRepositry.GetCalenderMasterListForUser();
                return CalenderMSTList.OrderByDescending(x => x.SRNO).ToList();
            }
            return CalenderMSTList.OrderByDescending(x => x.SRNO).ToList(); ;
        }


        public Int16 SaveProcessAttachment_Trn(CalenderMasterViewModel AVM)
        {
            return _objCalenderMasterRepositry.SaveProcessAttachment_Trn(AVM);
        }

        public short SendMailByApprovalAuthority(CalenderMasterViewModel AVM, Employee_Details emp_dtl)
        {

            short retVal = 0;
            try
            {
                #region Send mail for requestor
                var Requester = AVM.CREATED_BY;
                ADEMPLOYEE ade = new ADEMPLOYEE();
                ade = _objCalenderMasterRepositry.GetEmpDtl(AVM.CREATED_BY);

                if (!string.IsNullOrEmpty(ade.EMAILID))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = ade.EMAILID;

                    string strSubject = "Calendar master request created successfully by Emp Code - " + AVM.CREATED_BY;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Calendar Master Request By Emp Code (" + AVM.CREATED_BY + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Calendar master request has been submitted by " + ade.FIRSTNAME + " " + ade.LASTNAME + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=200 height=22 valign=top>Calendar Description:</td><td width=389 valign=top> For Calendar " + AVM.CALENDER + " Financial Year " + AVM.SelectedFinancialYear + "</td></tr>" +
                                     //"<tr><td width=389 valign=top> For Calendar " + AVM.CALENDER + " Financial Year " + AVM.SelectedFinancialYear + "</td></tr>"+
                                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    finally
                    {
                        retVal = 1;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public CalenderMasterViewModel GetCalenderMasterDetails(int SrNo)
        {
            return _objCalenderMasterRepositry.GetCalenderMasterDetails(SrNo);
        }
        public Int16 UpdateProcessAttachment_Trn(CalenderMasterViewModel AVM)
        {
            return _objCalenderMasterRepositry.UpdateProcessAttachment_Trn(AVM);
        }

        public FileViewModel GetFileForDownload(Int64 id, string type)
        {
            return _objCalenderMasterRepositry.GetFileForDownload(id, type);
        }

        public CalenderMasterViewModel GetCalenderMSTRequestById(long id)
        {
            return _objCalenderMasterRepositry.GetCalenderMSTRequestById(id);
        }
        public short CalenderMasterAppr(CALENDERMASTERMAPPINGTRNViewModel PHVM, Employee_Details emp_dtl)
        {
            short retVal = _objCalenderMasterRepositry.CalenderMasterAppr(PHVM, emp_dtl);
            if (retVal == 1)
            {

                ADEMPLOYEE Req1 = new ADEMPLOYEE();
                ADEMPLOYEE App2 = new ADEMPLOYEE();
                ADEMPLOYEE App1_DTL = new ADEMPLOYEE();
                CALENDARMASTERHEADER CMH = new CALENDARMASTERHEADER();
                CALENDARMASTERAPPHIS APP_LEVEL1 = new CALENDARMASTERAPPHIS();

                APP_LEVEL1 = _objCalenderMasterRepositry.GetApp_Lvl(Int32.Parse(PHVM.CAL_MAS_SRNO.ToString()), Int32.Parse(PHVM.MODIFIEDBY.ToString()));
                int AppLvl = Int32.Parse(APP_LEVEL1.APP_LEVEL.ToString());
                var Requester = APP_LEVEL1.CREATEDBY;
                if (AppLvl == 1 && PHVM.STATUS == 1)
                {
                    short retVal1 = 0;
                    try
                    {
                        #region Send mail for requestor

                        Req1 = _objCalenderMasterRepositry.GetEmpDtl(APP_LEVEL1.CREATEDBY.ToString());
                        App1_DTL = _objCalenderMasterRepositry.GetEmpDtl(APP_LEVEL1.APPAUTH_ECODE.ToString());
                        App2 = _objCalenderMasterRepositry.GetNextAppECodeEmail(Int32.Parse(PHVM.CAL_MAS_SRNO.ToString()));
                        CMH = _objCalenderMasterRepositry.GetHeaderDetails(Int32.Parse(PHVM.CAL_MAS_SRNO.ToString()));
                        var Status = "";
                        if (PHVM.STATUS == 1)
                        {
                            Status = "Approved";
                        }
                        else
                        {
                            Status = "Rejected";
                        }
                        if (!string.IsNullOrEmpty(Req1.EMAILID) && !string.IsNullOrEmpty(App2.EMAILID))
                        {
                            commanEmail sendMail = new commanEmail();
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            sendMail.MailTo = Req1.EMAILID + "," + App2.EMAILID;

                            string strSubject = "Calendar master request created by Emp Code - " + APP_LEVEL1.CREATEDBY;
                            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Calendar Master Request By Emp Code (" + APP_LEVEL1.CREATEDBY + ")</font></b></td></tr>" +

                                             "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                             "<tr><td valign=top colspan =2>Calendar master request has been " + Status + " by " + App1_DTL.FIRSTNAME + " " + App1_DTL.LASTNAME + " San. The request details are as follows:</td></tr>" +
                                             "<tr><td width=200 height=22 valign=top>Calendar Description:</td><td width=389 valign=top> For Calendar " + CMH.CALENDER + " Financial Year " + CMH.FINANCIALYEAR + "</td></tr>" +
                                             //"<tr><td width=389 valign=top> For Calendar " + CMH.CALENDER + " Financial Year " + CMH.FINANCIALYEAR + "</td></tr>" +
                                             "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                                retVal1 = -1;
                            }
                            finally
                            {
                                retVal1 = 1;
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    return retVal;
                }
                if (AppLvl == 1 && PHVM.STATUS == 2)
                {
                    {
                        short retVal1 = 0;
                        try
                        {
                            #region Send mail for requestor

                            Req1 = _objCalenderMasterRepositry.GetEmpDtl(APP_LEVEL1.CREATEDBY.ToString());
                            App1_DTL = _objCalenderMasterRepositry.GetEmpDtl(APP_LEVEL1.APPAUTH_ECODE.ToString());
                            //App2 = _objCalenderMasterRepositry.GetNextAppECodeEmail(Int32.Parse(PHVM.CAL_MAS_SRNO.ToString()));
                            CMH = _objCalenderMasterRepositry.GetHeaderDetails(Int32.Parse(PHVM.CAL_MAS_SRNO.ToString()));
                            var Status = "";
                            if (PHVM.STATUS == 1)
                            {
                                Status = "Approved";
                            }
                            else
                            {
                                Status = "Rejected";
                            }
                            if (!string.IsNullOrEmpty(Req1.EMAILID))
                            {
                                commanEmail sendMail = new commanEmail();
                                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                                sendMail.MailTo = Req1.EMAILID;

                                string strSubject = "Calendar master request created by Emp Code - " + APP_LEVEL1.CREATEDBY;
                                string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                                 "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Calendar Master Request By Emp Code (" + APP_LEVEL1.CREATEDBY + ")</font></b></td></tr>" +

                                                 "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                                 "<tr><td valign=top colspan =2>Your Calendar master request has been " + Status + " by " + App1_DTL.FIRSTNAME + " " + App1_DTL.LASTNAME + " San. The request details are as follows:</td></tr>" +
                                                 "<tr><td width=200 height=22 valign=top>Calendar Description:</td><td width=389 valign=top> For Calendar " + CMH.CALENDER + " Financial Year " + CMH.FINANCIALYEAR + "</td></tr>" +
                                                 //"<tr><td width=389 valign=top> For Calendar " + CMH.CALENDER + " Financial Year " + CMH.FINANCIALYEAR + "</td></tr>" +
                                                 "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                                sendMail.MailSubject = strSubject;
                                sendMail.MailBody = strBody;
                                try
                                {
                                    bool status = sendMail.Send();
                                }
                                catch (Exception ex)
                                {
                                    retVal1 = -1;
                                }
                                finally
                                {
                                    retVal1 = 1;
                                }
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            retVal = -1;
                        }
                        return retVal;
                    }

                }
            }
            return retVal;
        }
        public short DeactiveReqById(long id)
        {
            return _objCalenderMasterRepositry.DeactiveReqById(id);
        }
    }
}
