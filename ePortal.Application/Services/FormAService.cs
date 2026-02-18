using ePortal.Application.Contracts;
using ePortal.Repositories;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Services
{
    public class FormAService : IFormAService
    {
        FormARepository _FormARepo;
        CommonRepository _CommonRepo;
        Tuple<short, long> _retVal_tuple;

        public FormAService()
        {
            _FormARepo = new FormARepository();
            _CommonRepo = new CommonRepository();
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        }

        public FormAHeaderViewModel GetFormARequestById(long id)
        {
            FormAHeaderViewModel model = _FormARepo.GetFormARequestById(id);
            if (model != null)
            {
                model.strFROMDATE = ((DateTime)model.FROMDATE).ToString("dd-MMM-yyyy");
                model.strTODATE = ((DateTime)model.TODATE).ToString("dd-MMM-yyyy");
                if (model.FormADetail != null)
                {
                    model.FormADetail.strDATE_OF_NOTICE = model.FormADetail.DATE_OF_NOTICE == null ? "" : ((DateTime)model.FormADetail.DATE_OF_NOTICE).ToString("dd-MMM-yyyy");
                    model.FormADetail.strDATE_OF_DISCHARGE = model.FormADetail.DATE_OF_DISCHARGE == null ? "" : ((DateTime)model.FormADetail.DATE_OF_DISCHARGE).ToString("dd-MMM-yyyy");
                    model.FormADetail.strDATE_OF_PREGNANCY = model.FormADetail.DATE_OF_PREGNANCY == null ? "" : ((DateTime)model.FormADetail.DATE_OF_PREGNANCY).ToString("dd-MMM-yyyy");
                    model.FormADetail.strDATE_OF_CHILD_BIRTH = model.FormADetail.DATE_OF_CHILD_BIRTH == null ? "" : ((DateTime)model.FormADetail.DATE_OF_CHILD_BIRTH).ToString("dd-MMM-yyyy");
                    model.FormADetail.strDATE_OF_DMD = model.FormADetail.DATE_OF_DMD == null ? "" : ((DateTime)model.FormADetail.DATE_OF_DMD).ToString("dd-MMM-yyyy");
                    model.FormADetail.strDATE_OF_ILLNESS = model.FormADetail.DATE_OF_ILLNESS == null ? "" : ((DateTime)model.FormADetail.DATE_OF_ILLNESS).ToString("dd-MMM-yyyy");
                }
                model.Cal_Day_Detail = GetCalDayCalculation(model.FROMDATE, model.TODATE);
            }
            return model;
        }

        public Tuple<short, long> SaveFormARequest(FormAHeaderViewModel model)
        {
            Tuple<short, long> _tuple = _FormARepo.SaveFormARequest(model);
            //if (_tuple.Item1 == 1 && _tuple.Item2 > 0)
            //{
            //    SendMailByRequestor(_tuple.Item2);
            //}
            return _tuple;
        }

        public List<FormAHeaderViewModel> GetFormAList()
        {
            return _FormARepo.GetFormAList();
        }

        public short SaveFormADetails(long AddedBy, long FORMAHEADERID, FormADetailViewModel _formADetail)
        {
            return _FormARepo.SaveFormADetails(AddedBy, FORMAHEADERID, _formADetail);
        }

        public List<FormACaldayViewModel> GetCalDayCalculation(DateTime FromDate, DateTime ToDate)
        {
            string[] monthArray = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            List<FormACaldayViewModel> iList = new List<FormACaldayViewModel>();
            try
            {
                DateTime _fD = FromDate; DateTime _tD = FromDate.AddMonths(12);
                while (_fD < _tD)
                {
                    iList.Add(new FormACaldayViewModel
                    {
                        Month = _fD.ToString("MMM-yyyy"),
                        ND_Employed = "-",
                        NDN_Employed = "-",
                        ND_LaidOff = "-"
                    });
                    _fD = _fD.AddMonths(1);
                }

                var monthList = new List<string>();
                DateTime _fdate = FromDate; DateTime _tDate = ToDate.AddMonths((FromDate.ToString("yyyy") == ToDate.ToString("yyyy") ? 1 : 0));
                while (_fdate < _tDate)
                {
                    monthList.Add(_fdate.ToString("MM/yyyy"));
                    _fdate = _fdate.AddMonths(1);
                }

                foreach (string month_year in monthList)
                {
                    string[] strArry = month_year.Split('/');
                    int _month = Convert.ToInt32(strArry[0]); int _year = Convert.ToInt32(strArry[1]);
                    DateTime month_lastDate = new DateTime(_year, _month, 1).AddMonths(1).AddDays(-1);
                    string fullMonthName = monthArray[_month - 1] + "-" + _year;

                    FormACaldayViewModel obj = iList.Where(x => x.Month == fullMonthName).FirstOrDefault();
                    if (obj != null)
                    {
                        if (month_year == FromDate.ToString("MM/yyyy"))
                        {
                            obj.ND_Employed = FromDate.ToString("dd");
                            obj.NDN_Employed = Convert.ToString((month_lastDate - FromDate).TotalDays);
                            obj.ND_LaidOff = "0";
                        }
                        else if (month_year == ToDate.ToString("MM/yyyy"))
                        {
                            obj.ND_Employed = Convert.ToString((month_lastDate - ToDate).TotalDays);
                            obj.NDN_Employed = ToDate.ToString("dd");
                            obj.ND_LaidOff = "0";
                        }
                        else
                        {
                            obj.ND_Employed = "0";
                            obj.NDN_Employed = Convert.ToString(DateTime.DaysInMonth(_year, _month));
                            obj.ND_LaidOff = "0";
                        }
                    }
                    //obj.Month = monthArray[_month - 1] + "-" + _year;
                    //iList.Add(obj);
                }
            }
            catch (Exception ex)
            {
                iList = new List<FormACaldayViewModel>();
            }
            return iList;
        }

        public short UpdateByFormId(long FormAId, long updatedBy)
        {
            return _FormARepo.UpdateByFormId(FormAId, updatedBy);
        }

        public List<Employee_Details> PortalAutocompleteSuggestions(string Key)
        {
            return _FormARepo.PortalAutocompleteSuggestions(Key);
        }

        //public short SendMailByRequestor(long poHeaderId)
        //{
        //    short retVal = 0;
        //    try
        //    {
        //        FormAHeaderViewModel PHVM = _FormARepo.GetFormARequestById(poHeaderId);
        //        if (PHVM.FormAAppHis.Count > 0)
        //        {
        //            FormAAppHistoryViewModel Auth_obj = PHVM.FormAAppHis.Where(a => a.FormAAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
        //            if (Auth_obj != null)
        //            {
        //                commanEmail sendMail = new commanEmail();
        //                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
        //                if (serverpath.isTestServer())
        //                    sendMail.MailTo = serverpath.getTestEMail();
        //                else
        //                    sendMail.MailTo = Auth_obj.APP_EMAIL;
        //                string struid = (Auth_obj.APPEMP_CODE).ToString();
        //                string strid = poHeaderId.ToString();

        //                string strSubject = "FormA Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
        //                string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
        //                                 "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PO Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

        //                                 "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a PO Approval Request in Employee Portal. Below are the details :</td></tr>" +
        //                                 "<tr><td width=125 height=22 valign=top>FormA Number</td><td width=389 valign=top>" + PHVM.FormANO + "</td></tr>" +
        //                                 "<tr><td width=125 height=23 valign=top>Amount</td><td width=389 valign=top>" + PHVM.AMOUNT + "</td></tr>" +
        //                                 "<tr><td width=125 height=23 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
        //                                 "<tr><td width=125 height=23 valign=top>Supplier</td><td width=389 valign=top>" + PHVM.SUPPLIER_NAME + "</td></tr>" +
        //                                 "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=PO&A=POApproval&Tid=" + strid + " > Employee Portal</a> for approval process.</td></tr>" +
        //                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

        //                sendMail.MailSubject = strSubject;
        //                sendMail.MailBody = strBody;
        //                try
        //                {
        //                    bool status = sendMail.Send();
        //                }
        //                catch (Exception ex)
        //                {
        //                    retVal = -1;
        //                }
        //                finally
        //                {
        //                    retVal = 1;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return retVal;
        //}

        //public short SendMailByApprovalAuthority(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        //{
        //    short retVal = 0;
        //    try
        //    {
        //        string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";

        //        #region Send mail next approval authority
        //        FormAHeaderViewModel PHVM = _FormARepo.GetFormARequestById(poHeaderId);
        //        if (PHVM.FormAAppHis.Count > 0 && approvalStatus == 1)
        //        {
        //            FormAAppHistoryViewModel Auth_obj = PHVM.FormAAppHis.Where(a => a.FormAAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
        //            if (Auth_obj != null)
        //            {
        //                commanEmail sendMail = new commanEmail();
        //                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
        //                if (serverpath.isTestServer())
        //                    sendMail.MailTo = serverpath.getTestEMail();
        //                else
        //                    sendMail.MailTo = Auth_obj.APP_EMAIL;
        //                string struid = (Auth_obj.APPEMP_CODE).ToString();
        //                string strid = poHeaderId.ToString();
        //                string strSubject = "PO Approval Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
        //                string strBody = string.Empty;
        //                // if (struid == "70000167")
        //                // { 
        //                //      strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
        //                //                      "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PO Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

        //                //                      "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear" + Auth_obj.APPEMP_NAME + " San,</br>" + PHVM.Emp_Detail._EName + " San has raised a PO Approval Request in Employee Portal. Below are the details :</td></tr>" +
        //                //                      "<tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
        //                //                      "<tr><td width=125 height=23 valign=top>Vendor Code</td><td width=389 valign=top>" + PHVM.VENDORID + "</td></tr>" +
        //                //                      "<tr><td width=125 height=23 valign=top>Vendor Name</td><td width=389 valign=top>" + PHVM.VENDORNAME + "</td></tr>" +
        //                //                      "<tr><td width=125 height=23 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
        //                //                      //"<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
        //                //                      "<tr><td valign=top colspan=2>To digital sign the PO, please insert  digital token in your laptop and click on <a href=" + @"file://E:\PO\DigitalSignatureWindowApp.exe"+ "> PO Approval</a> link.</td></tr>" +
        //                //     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
        //                //}
        //                // else
        //                // {
        //                strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
        //                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PO Approval Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

        //                         // "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
        //                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a PO Approval Request in Employee Portal. Below are the details :</td></tr>" +
        //                          "<tr><td width=125 height=22 valign=top>FormA Number</td><td width=389 valign=top>" + PHVM.FormANO + "</td></tr>" +
        //                         "<tr><td width=125 height=23 valign=top>Amount</td><td width=389 valign=top>" + PHVM.AMOUNT + "</td></tr>" +
        //                         "<tr><td width=125 height=23 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
        //                         "<tr><td width=125 height=23 valign=top>Supplier</td><td width=389 valign=top>" + PHVM.SUPPLIER_NAME + "</td></tr>" +
        //                         //"<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
        //                         "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=PO&A=POApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
        //                         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

        //                //}

        //                sendMail.MailSubject = strSubject;
        //                sendMail.MailBody = strBody;
        //                try
        //                {
        //                    bool status = sendMail.Send();
        //                }
        //                catch (Exception ex)
        //                {
        //                    retVal = -1;
        //                }
        //                finally
        //                {
        //                    retVal = 1;
        //                }
        //            }
        //        }
        //        #endregion

        //        #region Send mail for requestor
        //        if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
        //        {
        //            commanEmail sendMail = new commanEmail();
        //            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
        //            sendMail.MailTo = PHVM.Emp_Detail._EmailId;

        //            string strSubject = "PO Approval Status - " + RequestStatus;
        //            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
        //                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PO Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

        //                             "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
        //                             "<tr><td valign=top colspan =2>Your PO request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
        //                             "<tr><td width=125 height=22 valign=top>FormA Number</td><td width=389 valign=top>" + PHVM.FormANO + "</td></tr>" +
        //                             "<tr><td width=125 height=23 valign=top>Amount</td><td width=389 valign=top>" + PHVM.AMOUNT + "</td> </tr>" +
        //                             "<tr><td width=125 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
        //                             "<tr><td width=125 valign=top>Supplier</td><td width=389 valign=top>" + PHVM.SUPPLIER_NAME + "</td></tr>" +
        //                             "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
        //                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

        //            sendMail.MailSubject = strSubject;
        //            sendMail.MailBody = strBody;
        //            try
        //            {
        //                bool status = sendMail.Send();
        //            }
        //            catch (Exception ex)
        //            {
        //                retVal = -1;
        //            }
        //            finally
        //            {
        //                retVal = 1;
        //            }
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return retVal;
        //}

    }
}

