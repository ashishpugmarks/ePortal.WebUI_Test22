

using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class WFHService : IWFHService
    {
        WFHRepository _WFHRepo;
        CommonRepository _CommonRepo;

        public WFHService(WFHRepository WFHRepo, CommonRepository CommonRepo)
        {
            _WFHRepo = WFHRepo;
            _CommonRepo = CommonRepo;
        }

        public short EditWFHRequest(ASRWFH_HEADER_ViewModel AVM)
        {
            return _WFHRepo.EditWFHRequest(AVM);
        }

        public ASRWFH_HEADER_ViewModel GetWFHRequestDetail(long id)
        {
            ASRWFH_HEADER_ViewModel AHVM = _WFHRepo.GetWFHRequestDetail(id);
            if (AHVM != null)
            {
                AHVM.StartDateStr = ((DateTime)AHVM.STARTDATE).ToString("dd-MMM-yyyy");
                AHVM.EndDateStr = ((DateTime)AHVM.ENDDATE).ToString("dd-MMM-yyyy");
            }
            return AHVM;

        }

        public List<ASRWFH_HEADER_ViewModel> GetWFHRequestList(ASRWFH_HEADER_ViewModel AVM)
        {
            throw new NotImplementedException();
        }

        public short SaveWFHRequest(ASRWFH_HEADER_ViewModel AVM)
        {
            short retVal = 0; long _hearderId = 0;
            Tuple<short, long> _retVal_tuple = new Tuple<short, long>(0, 0);
            _retVal_tuple = _WFHRepo.SaveWFHRequest(AVM);
            retVal = _retVal_tuple.Item1;
            _hearderId = _retVal_tuple.Item2;
            if (retVal == 1 && _hearderId > 0)
            {
                try
                {
                    ASRWFH_HEADER_ViewModel _obj = GetWFHRequestDetail(_hearderId);
                    if (_obj != null)
                    {
                        short mailStatus = SendMailByRequestor(_obj, Convert.ToString(AVM.WorkingDays));
                    }
                }
                catch (Exception ex) { }
            }
            return retVal;
        }
        public short UpdateWFHApproval(ASRWFH_APPROVALHIS_ViewModel AAVM, Employee_Details employeeDetails)
        {
            short retVal = _WFHRepo.UpdateWFHApproval(AAVM);
            if (retVal == 1 && AAVM.ISAPPAPPROVED != 3) ///// -- 3 for Cancel by requestor
            {
                ASRWFH_HEADER_ViewModel _obj = GetWFHRequestDetail(Convert.ToInt64(AAVM.ASRWFHID));
                if (_obj != null)
                {
                    string requestStatus = string.Empty;
                    string _workingDays = Convert.ToString(AAVM.ASRWFH_HEADER.WorkingDays);
                    if (_obj.ASRWFH_APPROVALHIS.RECADEMPCODE == employeeDetails._ECode)
                    {
                        requestStatus = AAVM.ISRECAPPROVED == 1 ? "Approved" : AAVM.ISRECAPPROVED == 2 ? "Rejected" : "";
                    }
                    else if (_obj.ASRWFH_APPROVALHIS.APPADEMPCODE == employeeDetails._ECode)
                    {
                        requestStatus = AAVM.ISAPPAPPROVED == 1 ? "Approved" : AAVM.ISAPPAPPROVED == 2 ? "Rejected" : "";
                    }
                    if (!string.IsNullOrEmpty(requestStatus))
                    {
                        short mailStatus = SendMailByApprovalAuthority(_obj, employeeDetails, requestStatus, _workingDays);
                    }
                }
            }
            return retVal;
        }
        public short UpdateWFHApprovalList(List<ASRWFH_APPROVALHIS_ViewModel> AAVMList, Employee_Details employeeDetails)
        {
            short retVal = 0;
            foreach (ASRWFH_APPROVALHIS_ViewModel AAVM in AAVMList)
            {
                retVal = _WFHRepo.UpdateWFHApprovalList(AAVM);
                if (retVal == 1)
                {
                    ASRWFH_HEADER_ViewModel _obj = GetWFHRequestDetail(Convert.ToInt64(AAVM.ASRWFHID));
                    if (_obj != null)
                    {
                        string requestStatus = string.Empty; string _workingDays_Hours = "";
                        if (_obj.REQUESTTYPE == 1 && _obj.APPLYFOR == 1)
                        {
                            _workingDays_Hours = GetWorkingDays(_obj.StartDateStr, _obj.EndDateStr, _obj.ADEMPCODE);
                        }
                        else
                        {
                            _workingDays_Hours = GetHours(_obj.StartDateStr, _obj.EndDateStr, _obj.STARTTIME, _obj.ENDTIME);
                        }

                        if (_obj.ASRWFH_APPROVALHIS.RECADEMPCODE == employeeDetails._ECode)
                        {
                            requestStatus = AAVM.ISRECAPPROVED == 1 ? "Approved" : AAVM.ISRECAPPROVED == 2 ? "Rejected" : "";
                        }
                        else if (_obj.ASRWFH_APPROVALHIS.APPADEMPCODE == employeeDetails._ECode)
                        {
                            requestStatus = AAVM.ISAPPAPPROVED == 1 ? "Approved" : AAVM.ISAPPAPPROVED == 2 ? "Rejected" : "";
                        }
                        if (!string.IsNullOrEmpty(requestStatus))
                        {
                            short mailStatus = SendMailByApprovalAuthority(_obj, employeeDetails, requestStatus, _workingDays_Hours);
                        }
                    }
                }
            }
            return retVal;
        }

        public List<ShiftViewModel> GetShiftBySite(long SiteId, long? id)
        {
            return _WFHRepo.GetShiftBySite(SiteId, id);
        }

        public WFHAppAuthViewModel GetApprovalAuth(long loginUser)
        {
            return _WFHRepo.GetApprovalAuth(loginUser);
        }

        public short GetOffDays(string startDate, string endDate, long siteId, long operationId)
        {
            return _WFHRepo.GetOffDays(startDate, endDate, siteId, operationId);
        }

        public List<ASRWFH_HEADER_ViewModel> GetWFHReport(WFHReportViewModel WRVM)
        {
            return _WFHRepo.GetWFHReport(WRVM);
        }

        public List<ADORGLEVEL> GetOrgLevelList(long typeId)
        {
            return _WFHRepo.GetOrgLevelList(typeId);
        }

        public List<WFHDivViewModel> BindDivision(long op_Id)
        {
            return _WFHRepo.BindDivision(op_Id);
        }

        public List<WFHDepViewModel> BindDepartment(long div_Id, long op_Id)
        {
            return _WFHRepo.BindDepartment(div_Id, op_Id);
        }

        public List<WFHSecViewModel> BindSection(long dep_Id, long div_Id, long op_Id)
        {
            return _WFHRepo.BindSection(dep_Id, div_Id, op_Id);
        }

        public string GetWorkingDays(string startDate, string endDate, long requestorCode)
        {
            int offDays = 0; int totalDays = 0; string workingDays = "";
            try
            {
                
                long _siteId = _CommonRepo.GetSiteIdByEmpCode(requestorCode);
                if (_siteId > 0)
                {
                    long _opId = _CommonRepo.GetOperationIdByEmpCode(requestorCode);
                    offDays = _WFHRepo.GetOffDays(startDate, endDate, _siteId, _opId);
                    DateTime _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                    DateTime _edate = DateTime.ParseExact(endDate, "dd-MMM-yyyy", null);
                    totalDays = (int)((_edate.AddDays(1)) - _sdate).TotalDays;
                    workingDays = Convert.ToString(totalDays - offDays);
                }
            }
            catch (Exception ex) { }
            return workingDays;
        }

        public string GetHours(string startDate, string endDate, string startTime, string endTime)
        {
            string retval = ""; double totalHours = 0;
            try
            {
                DateTime _validSdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                DateTime _validEdate = DateTime.ParseExact(endDate, "dd-MMM-yyyy", null);

                string[] _start_HH_mm = startTime.Split(':');
                string[] _end_HH_mm = endTime.Split(':');
                startDate = startDate + " " + Convert.ToInt32(_start_HH_mm[0]).ToString("D2") + ":" + Convert.ToInt32(_start_HH_mm[1]).ToString("D2");
                endDate = endDate + " " + Convert.ToInt32(_end_HH_mm[0]).ToString("D2") + ":" + Convert.ToInt32(_end_HH_mm[1]).ToString("D2");
                DateTime _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy HH:mm", null);
                DateTime _edate = DateTime.ParseExact(endDate, "dd-MMM-yyyy HH:mm", null);
                totalHours = (_edate - _sdate).TotalHours;
                retval = String.Format("{0:0.##}", totalHours);
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public short SendMailByRequestor(ASRWFH_HEADER_ViewModel AVM, string strNoOfWorkingDays)
        {
            short retVal = 0;
            try
            {

                if (!string.IsNullOrEmpty(AVM.ASRWFH_APPROVALHIS.REC_EMAIL)) //// -- Approval Authority 1
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                        sendMail.MailTo = AVM.ASRWFH_APPROVALHIS.REC_EMAIL; //// -- Approval Authority 1

                    string strRequestType = AVM.REQUESTTYPE == 1 ? "Work from Home" : "Remote Support";
                    string strAppliedFor = AVM.APPLYFOR == 1 ? "Business Working Days" : AVM.APPLYFOR == 2 ? "Off Days" : "";
                    string strday_hour = (AVM.REQUESTTYPE == 1 && AVM.APPLYFOR == 1) ? "Working day(s)" : "Working hour(s)";
                    string strSubject = "WFH Request from - " + AVM.EmpName + ", Employee Code - " + AVM.ADEMPCODE;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>WFH Request from " + AVM.EmpName + " - Emp Code (" + AVM.ADEMPCODE + ")</font></b></td>" +
                                     "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + AVM.ADEMPCODE + "</td>" +
                                     "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + AVM.EmpName + "</td>" +
                                     " </tr><tr> " +
                                     "<td width=125 valign=top>From Date</td><td width=389 valign=top>" + AVM.StartDateStr + "</td></tr><tr><td width=125 valign=top>To Date</td>" +
                                    // "<td width=389 valign=top>" + AVM.EndDateStr + "</td></tr><tr><td width=125 valign=top>" + strday_hour + "</td><td width=389 valign=top>" + strNoOfWorkingDays + "</td></tr>" +
                                    "<td width=389 valign=top>" + AVM.EndDateStr + "</td></tr>" +
                                    "<tr><td width=125 valign=top>Request Type</td><td width=389 valign=top>" + strRequestType + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Applied For</td><td width=389 valign=top>" + strAppliedFor + "</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
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
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public short SendMailByApprovalAuthority(ASRWFH_HEADER_ViewModel AVM, Employee_Details employeeDetails, string RequestStatus, string strNoOfWorkingDays)
        {
            short retVal = 0;
            try
            {
                if (!string.IsNullOrEmpty(AVM.ASRWFH_APPROVALHIS.APP_EMAIL) && AVM.ASRWFH_APPROVALHIS.RECADEMPCODE == employeeDetails._ECode && AVM.ASRWFH_APPROVALHIS.RECADEMPCODE != AVM.ASRWFH_APPROVALHIS.APPADEMPCODE) ///// Approval Authority 2
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = AVM.ASRWFH_APPROVALHIS.APP_EMAIL;

                    string strRequestType = AVM.REQUESTTYPE == 1 ? "Work from Home" : "Remote Support";
                    string strAppliedFor = AVM.APPLYFOR == 1 ? "Business Working Days" : AVM.APPLYFOR == 2 ? "Off Days" : "";
                    string strday_hour = (AVM.REQUESTTYPE == 1 && AVM.APPLYFOR == 1) ? "Working day(s)" : "Working hour(s)";

                    string strSubject = "WFH Request from - " + AVM.EmpName + ", Employee Code - " + AVM.ADEMPCODE;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>WFH Request from " + AVM.EmpName + " - Emp Code (" + AVM.ADEMPCODE + ")</font></b></td>" +
                                     "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + AVM.ADEMPCODE + "</td>" +
                                     "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + AVM.EmpName + "</td>" +
                                     " </tr><tr> " +
                                     "<td width=125 valign=top>From Date</td><td width=389 valign=top>" + AVM.StartDateStr + "</td></tr><tr><td width=125 valign=top>To Date</td>" +
                                    // "<td width=389 valign=top>" + AVM.EndDateStr + "</td></tr><tr><td width=125 valign=top>" + strday_hour + "</td><td width=389 valign=top>" + strNoOfWorkingDays + "</td></tr>" +
                                    "<td width=389 valign=top>" + AVM.EndDateStr + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Request Type</td><td width=389 valign=top>" + strRequestType + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Applied For</td><td width=389 valign=top>" + strAppliedFor + "</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
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

                if (!string.IsNullOrEmpty(AVM.EmpEmail))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = AVM.EmpEmail;

                    string strRequestType = AVM.REQUESTTYPE == 1 ? "Work from Home" : "Remote Support";
                    string strAppliedFor = AVM.APPLYFOR == 1 ? "Business Working Days" : AVM.APPLYFOR == 2 ? "Off Days" : "";
                    string strday_hour = (AVM.REQUESTTYPE == 1 && AVM.APPLYFOR == 1) ? "Working day(s)" : "Working hour(s)";

                    string strSubject = "WFH Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>WFH Approval Status " + RequestStatus + " - Emp Code (" + AVM.ADEMPCODE + ")</font></b></td></tr>" +

                                     "<tr><td>" +
                                     "<table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your WFH request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + AVM.ADEMPCODE + "</td></tr>" +
                                     "<tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + AVM.EmpName + "</td> </tr>" +
                                     "<tr><td width=125 valign=top>From Date</td><td width=389 valign=top>" + AVM.StartDateStr + "</td></tr>" +
                                     "<tr><td width=125 valign=top>To Date</td><td width=389 valign=top>" + AVM.EndDateStr + "</td></tr>" +
                                     //"<tr><td width=125 valign=top>"+ strday_hour + "</td><td width=389 valign=top>" + strNoOfWorkingDays + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Request Type</td><td width=389 valign=top>" + strRequestType + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Applied For</td><td width=389 valign=top>" + strAppliedFor + "</td></tr>" +
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
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public ASRWFH_HEADER_ViewModel GetRejectedRequest(string startDate, string endDate, long EmpCode)
        {
            return _WFHRepo.GetRejectedRequest(startDate, endDate, EmpCode);
        }

        //--WFH issue correction
        public WFHAppAuthViewModel GetViewApprovalAuth(long _ReqID)
        {
            return _WFHRepo.GetViewApprovalAuth(_ReqID);
        }
        //--WFH issue correction
        //SR86752 Start
        public List<ASRWFH_HEADER_ViewModel> GetWFHAdminReport(WFHReportViewModel WRVM)
        {
            return _WFHRepo.GetWFHAdminReport(WRVM);
        }
        //SR86752 End
    }
}
