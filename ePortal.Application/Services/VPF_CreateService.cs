using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Services
{
    public class VPF_CreateService : IVpfCreate
    {
        private readonly VpfCreateRepositories _objVpfCreateRepositories;

        public VPF_CreateService(VpfCreateRepositories objVpfCreateRepositories)
        {
            _objVpfCreateRepositories = objVpfCreateRepositories;
        }
        public List<VPF_CreateDetailViewModel> GetVpfDetail(long id)
        {
            List<VPF_CreateDetailViewModel> collection = _objVpfCreateRepositories.GetVPFDetail(id);
            return collection;
        }

        public List<VPF_CreateDetailViewModel> IRVpfDetail(SearchViewModel searchViewModel)
        {
            List<VPF_CreateDetailViewModel> VPF_List = _objVpfCreateRepositories.IRVPFDetail();
            if (searchViewModel.EmployeeCode == null && searchViewModel.FromDate == null && searchViewModel.ToDate == null && searchViewModel.Status == null)
            {
                return VPF_List;
            }
            else if (searchViewModel.Status != null && searchViewModel.EmployeeCode != null && searchViewModel.FromDate != null && searchViewModel.ToDate != null)
            {
                //VPF_List.Where(x => x.EMPLOYEECODE == (string.IsNullOrEmpty(searchViewModel.EmployeeCode.ToString()) ? x.EMPLOYEECODE : searchViewModel.EmployeeCode));
                VPF_List = VPF_List.Where(x => (Convert.ToDateTime(x.EFFECTIVEDATE) >= (searchViewModel.FromDate == null ? Convert.ToDateTime(x.EFFECTIVEDATE) : searchViewModel.FromDate) && Convert.ToDateTime(x.EFFECTIVEDATE) <= (searchViewModel.ToDate == null ? Convert.ToDateTime(x.EFFECTIVEDATE) : searchViewModel.ToDate)) && x.EMPLOYEECODE == searchViewModel.EmployeeCode && x.APPROVESTATUS == searchViewModel.Status).ToList();
            }
            else if (searchViewModel.Status != null && searchViewModel.EmployeeCode != null && searchViewModel.FromDate == null && searchViewModel.ToDate == null)
            {
                VPF_List = VPF_List.Where(x => x.EMPLOYEECODE == searchViewModel.EmployeeCode && x.APPROVESTATUS == searchViewModel.Status).ToList();
            }
            else if (searchViewModel.Status != null && searchViewModel.EmployeeCode == null && searchViewModel.FromDate == null && searchViewModel.ToDate == null)
            {
                VPF_List = VPF_List.Where(x => x.APPROVESTATUS == searchViewModel.Status).ToList();
            }
            else if (searchViewModel.Status == null && searchViewModel.EmployeeCode != null && searchViewModel.FromDate == null && searchViewModel.ToDate == null)
            {
                VPF_List = VPF_List.Where(x => x.EMPLOYEECODE == searchViewModel.EmployeeCode).ToList();
                //VPF_List = VPF_List.Where(x => x.STATUS == 1).ToList();
            }
            else if (searchViewModel.Status == null && searchViewModel.EmployeeCode == null && searchViewModel.FromDate != null && searchViewModel.ToDate != null)
            {
                VPF_List = VPF_List.Where(x => (Convert.ToDateTime(x.EFFECTIVEDATE) >= searchViewModel.FromDate && Convert.ToDateTime(x.EFFECTIVEDATE) <= searchViewModel.ToDate)).ToList();
            }
            else if (searchViewModel.Status == null && searchViewModel.EmployeeCode != null && searchViewModel.FromDate != null && searchViewModel.ToDate != null)
            {
                VPF_List = VPF_List.Where(x => (Convert.ToDateTime(x.EFFECTIVEDATE) >= searchViewModel.FromDate && Convert.ToDateTime(x.EFFECTIVEDATE) <= searchViewModel.ToDate) && x.EMPLOYEECODE == searchViewModel.EmployeeCode).ToList();
            }
            else
            {

            }
            return VPF_List;
        }

        public String IRVpfDetailExcel(SearchViewModel searchViewModel)
        {
            string Htmltext = "";
            List<VPF_CreateDetailViewModel> VPF_List = _objVpfCreateRepositories.IRVPFDetail();
            if (searchViewModel.EmployeeCode == null && searchViewModel.FromDate == null && searchViewModel.ToDate == null && searchViewModel.Status == null)
            {

            }
            else if (searchViewModel.Status != null && searchViewModel.EmployeeCode != null && searchViewModel.FromDate != null && searchViewModel.ToDate != null)
            {
                VPF_List = VPF_List.Where(x => (Convert.ToDateTime(x.EFFECTIVEDATE) >= (searchViewModel.FromDate == null ? Convert.ToDateTime(x.EFFECTIVEDATE) : searchViewModel.FromDate) && Convert.ToDateTime(x.EFFECTIVEDATE) <= (searchViewModel.ToDate == null ? Convert.ToDateTime(x.EFFECTIVEDATE) : searchViewModel.ToDate)) && x.EMPLOYEECODE == searchViewModel.EmployeeCode && x.APPROVESTATUS == searchViewModel.Status).ToList();
            }
            else if (searchViewModel.Status != null && searchViewModel.EmployeeCode != null && searchViewModel.FromDate == null && searchViewModel.ToDate == null)
            {
                VPF_List = VPF_List.Where(x => x.EMPLOYEECODE == searchViewModel.EmployeeCode && x.APPROVESTATUS == searchViewModel.Status).ToList();
            }
            else if (searchViewModel.Status != null && searchViewModel.EmployeeCode == null && searchViewModel.FromDate == null && searchViewModel.ToDate == null)
            {
                VPF_List = VPF_List.Where(x => x.APPROVESTATUS == searchViewModel.Status).ToList();
            }
            else if (searchViewModel.Status == null && searchViewModel.EmployeeCode != null && searchViewModel.FromDate == null && searchViewModel.ToDate == null)
            {
                VPF_List = VPF_List.Where(x => x.EMPLOYEECODE == searchViewModel.EmployeeCode).ToList();
            }
            else if (searchViewModel.Status == null && searchViewModel.EmployeeCode == null && searchViewModel.FromDate != null && searchViewModel.ToDate != null)
            {
                VPF_List = VPF_List.Where(x => (Convert.ToDateTime(x.EFFECTIVEDATE) >= searchViewModel.FromDate && Convert.ToDateTime(x.EFFECTIVEDATE) <= searchViewModel.ToDate)).ToList();
            }
            else if (searchViewModel.Status == null && searchViewModel.EmployeeCode != null && searchViewModel.FromDate != null && searchViewModel.ToDate != null)
            {
                VPF_List = VPF_List.Where(x => (Convert.ToDateTime(x.EFFECTIVEDATE) >= searchViewModel.FromDate && Convert.ToDateTime(x.EFFECTIVEDATE) <= searchViewModel.ToDate) && x.EMPLOYEECODE == searchViewModel.EmployeeCode).ToList();
            }
            else
            {

            }
            if (VPF_List.Count > 0)
            {
                StringBuilder strHTMLBuilder = new StringBuilder();
                strHTMLBuilder.Append("<html >");
                strHTMLBuilder.Append("<head>");
                strHTMLBuilder.Append("</head>");
                strHTMLBuilder.Append("<body>");
                strHTMLBuilder.Append("<table border='1px'>");
                strHTMLBuilder.Append("<tr>");
                strHTMLBuilder.Append("<td style='width:10%;text-align:center;padding:7px;'><b>ECode</b></td>");
                strHTMLBuilder.Append("<td style='width:20%;padding:7px;'><b>Employee Name</b></td>");
                strHTMLBuilder.Append("<td style='width:10%;text-align:center;padding:7px;'><b>Request Type</b></td>");
                strHTMLBuilder.Append("<td style='width:10%;text-align:center;padding:7px;'><b>VPF Contribution(%)</ b></td>");
                strHTMLBuilder.Append("<td style='width:10%;text-align:center;padding:7px;'><b>Effective Date</b></td>");
                strHTMLBuilder.Append("<td style='width:10%;text-align:center;padding:7px;'><b>Approval Date</b></td>");
                strHTMLBuilder.Append("<td style='width:20%;text-align:center;padding:7px;'><b>Approval Remarks</b></td>");
                strHTMLBuilder.Append("<td style='width:10%;text-align:center;padding:7px;'><b>Status</b></td>");
                strHTMLBuilder.Append("</tr>");

                foreach (var Ritem in VPF_List)
                {
                    String status = "";
                    String type = "";
                    if (Ritem.APPROVESTATUS == 0)
                    {
                        status = "Rejected";
                    }
                    else if (Ritem.APPROVESTATUS == 1)
                    {
                        status = "Approved";
                    }
                    else if (Ritem.APPROVESTATUS == 2)
                    {
                        status = "Pending";
                    }
                    else
                    {
                        status = "";
                    }

                    if (Ritem.REQUESTTYPE == 1)
                    {
                        type = "Start";
                    }
                    else if (Ritem.REQUESTTYPE == 2)
                    {
                        type = "Change";
                    }
                    else if (Ritem.REQUESTTYPE == 3)
                    {
                        type = "Stop";
                    }
                    else
                    {
                        type = "";
                    }
                    strHTMLBuilder.Append("<tr>");
                    strHTMLBuilder.Append("<td style='padding:5px;text-align:center;'>" + Ritem.EMPLOYEECODE + "</td>");
                    strHTMLBuilder.Append("<td style='padding:5px;'>" + Ritem.FIRSTNAME + " " + Ritem.LASTNAME + "</td>");
                    strHTMLBuilder.Append("<td style='text-align:center;'>" + type + "</td>");
                    strHTMLBuilder.Append("<td style='text-align:center;'>" + Ritem.VPFCONTRIBUTION + "</td>");
                    strHTMLBuilder.Append("<td style='text-align:center;'>" + Ritem.EFFECTIVEDATE + "</td>");
                    strHTMLBuilder.Append("<td style='text-align:center;'>" + Ritem.APPROVALDATE + "</td>");
                    strHTMLBuilder.Append("<td style='text-align:left;'>" + Ritem.APPROVALREMARKS + "</td>");
                    strHTMLBuilder.Append("<td style='text-align:center;'>" + status + "</td>");
                    strHTMLBuilder.Append("</tr>");
                }
                strHTMLBuilder.Append("</table>");
                strHTMLBuilder.Append("</body>");
                strHTMLBuilder.Append("</html>");
                Htmltext = strHTMLBuilder.ToString();
            }
            return Htmltext;
        }

        public VPF_CreateDetailViewModel EditVPF_Detail(long id)
        {
            VPF_CreateDetailViewModel collection = _objVpfCreateRepositories.EditVPF_Detail(id);
            return collection;

        }

        public VPF_CreateDetailViewModel Get_Data_By_UserId(long id)
        {
            VPF_CreateDetailViewModel collection = _objVpfCreateRepositories.Get_Data_By_UserId(id);
            return collection;
        }

        //public VPF_CreateDetailViewModel CheckEnteredRequest(long id)
        //{
        //    VPF_CreateDetailViewModel collection = _objVpfCreateRepositories.CheckVPF_Detail(id);
        //    return collection;
        //}

        public VPF_CreateDetailViewModel EditApproveVPF_Detail(long id)
        {
            VPF_CreateDetailViewModel collection = _objVpfCreateRepositories.Edit_ApproveVPF_Detail(id);
            return collection;
        }

        public VPF_CreateDetailViewModel ApproveVPF_Detail(VPF_CreateDetailViewModel collection)
        {
            try
            {
                collection = _objVpfCreateRepositories.ApproveVPF_Detail(collection);
                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public VPF_CreateDetailViewModel Update_VPF_Detail(VPF_CreateDetailViewModel collection)
        {
            try
            {
                collection = _objVpfCreateRepositories.UpdateVPF_Detail(collection);
                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public VPF_CreateDetailViewModel Cancel_VPF_Detail(VPF_CreateDetailViewModel collection)
        {
            try
            {
                collection = _objVpfCreateRepositories.CancelVPF_Detail(collection);
                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public VPF_CreateDetailViewModel SaveVPF_Create_Detail(VPF_CreateDetailViewModel collection)
        {
            try
            {
                collection = _objVpfCreateRepositories.SaveVPF_CreateDetail(collection);
                return collection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
