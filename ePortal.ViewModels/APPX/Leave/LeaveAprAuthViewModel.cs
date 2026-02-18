
// File: ViewModels/APPX/Leave/LeaveAprAuthViewModel.cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels.APPX.Leave
{
    public class LeaveAprAuthViewModel
    {
        // Query context
        public string? Id { get; set; }   // employee code from query
        public string? Ind { get; set; }  // page index from query

        // Header display
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? Designation  { get; set; }

        // Dropdowns: recommendation & approval
        public List<SelectListItem> RecommendationOptions { get; set; } = new();
        public List<SelectListItem> ApprovalOptions       { get; set; } = new();

        // Selected values
        public string? SelectedRecommendationCode { get; set; }
        public string? SelectedApprovalCode       { get; set; }

        // Enable/Disable (CheckAprAuth parity)
        public bool SubmitEnabled   { get; set; } = true;
        public bool RecomEnabled    { get; set; } = true;
        public bool ApproveEnabled  { get; set; } = true;

        // Messages
        public string? StatusMessage { get; set; }

        // Back navigation
        public string? BackUrl { get; set; }
    }
}
