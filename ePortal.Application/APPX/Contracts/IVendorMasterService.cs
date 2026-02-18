using ePortal.ViewModels.APPX.VendorMaster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.APPX.Contracts
{
    public interface IVendorMasterService
    {
        Task<Vendor> GetCsvValidation(ParseCsvRequest request, string fullPath);
        Task<VendorCsvParseResult> ParseFromFileAsync(string fullPath);
        Task<DataTable> GetApprovalAuthorityTableAsync(string bankAccNo, string panNumber);
        Task<VendorListMatchResponse> GetVendorListMatchDetailsAsync(VendorMatchRequest request);
        Task<VendorDto> GetDetailBehalfOfVendorCodeAsync(string vendorCode);
        VendorMasterViewModel GetWithholdingData();
        Task<VendorRequestResult> GetVendorRequests(VendorMasterFilter obj);
        Task<List<VendorApprovalRequest>> GetVendorApprovalRequests(VendorMasterFilter obj);
        Task<List<VendorApprovalRequest>> GetVendorApprovalRequestsFin(VendorMasterFilter obj);
        Task<VendorDetailsViewModel> GetVendorRequestDataById(string VMID, string basePath);
        Task<List<VendorBlockPendingRequest>> GetVendorBlockPendingRequests(VendorBlockFilter filter);
        Task<List<VendorBlockApprovalRequest>> GetVendorBlockApprovalRequests(VendorBlockFilter filter);
        //Task<VendorBlockRequestViewModel> GetVendorBlockRequestDataById(string VMID);
    }
}
