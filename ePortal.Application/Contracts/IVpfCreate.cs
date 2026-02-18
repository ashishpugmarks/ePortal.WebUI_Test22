using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface IVpfCreate
    {
        List<VPF_CreateDetailViewModel> GetVpfDetail(long id);
        List<VPF_CreateDetailViewModel> IRVpfDetail(SearchViewModel searchViewModel);
        String IRVpfDetailExcel(SearchViewModel searchViewModel);
        VPF_CreateDetailViewModel EditVPF_Detail(long id);
        VPF_CreateDetailViewModel Get_Data_By_UserId(long id);
        VPF_CreateDetailViewModel EditApproveVPF_Detail(long id);
        VPF_CreateDetailViewModel ApproveVPF_Detail(VPF_CreateDetailViewModel collection);
        VPF_CreateDetailViewModel SaveVPF_Create_Detail(VPF_CreateDetailViewModel collection);
        VPF_CreateDetailViewModel Update_VPF_Detail(VPF_CreateDetailViewModel collection);
        VPF_CreateDetailViewModel Cancel_VPF_Detail(VPF_CreateDetailViewModel collection);

        //VPF_CreateDetailViewModel CheckEnterRequest(VPF_CreateDetailViewModel Check);
    }
}
