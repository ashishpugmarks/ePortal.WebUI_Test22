using System;
using System.Collections.Generic;

namespace ePortal.ViewModels.APPX.Leave
{
    public class LeaveCardViewModel
    {
        // Query params
        public string Year { get; set; } = "";
        public string ECode { get; set; } = "";            // decrypted ECode
        public string EncryptedECode { get; set; } = "";   // original eid if you need link-outs

        // Employee details (labels)
        public string Operation  { get; set; } = "";
        public string Division   { get; set; } = "";
        public string Department { get; set; } = "";
        public string Section    { get; set; } = "";
        public string Employee   { get; set; } = "";
        public string Gender     { get; set; } = "";

        // Leave balances (as on today)
        public string SL_Balance { get; set; } = "0";
        public string SL_Availed { get; set; } = "0";
        public string CL_Balance { get; set; } = "0";
        public string CL_Availed { get; set; } = "0";
        public string EL_Balance { get; set; } = "0";
        public string EL_Availed { get; set; } = "0";
        public string CO_Balance { get; set; } = "0";
        public string CO_Availed { get; set; } = "0";

        // Flags
        public bool ShowMaternity { get; set; } = false;   // Gender == "F" => true

        // (Optional) Initial records (we'll load via AJAX)
        public List<LeaveDetailRow> SickRows           { get; set; } = new();
        public List<LeaveDetailRow> CasualRows         { get; set; } = new();
        public List<LeaveDetailRow> EarnedRows         { get; set; } = new();
        public List<LeaveDetailRow> CompOffRows        { get; set; } = new();
        public List<LeaveDetailRow> LwpRows            { get; set; } = new();
        public List<LeaveDetailRow> MaternityRows      { get; set; } = new();
        public List<LeaveDetailRow> RelocationRows     { get; set; } = new();
        public List<LeaveDetailRow> AdvanceEarnedRows  { get; set; } = new();
        public List<LeaveDetailRow> SpecialLeaveRows   { get; set; } = new();
    }

  
public class LeaveDetailRow
{
    public string AppDate        { get; set; } = ""; // DATEADDED
    public string LeaveFrom      { get; set; } = ""; // DATEFROM
    public string LeaveTo        { get; set; } = ""; // DATETO
    public string Days           { get; set; } = ""; // NOOFDAYS
    public string Purpose        { get; set; } = ""; // PURPOSE
    public string RecommendedBy  { get; set; } = ""; // SUPERVISORNAME
    public string SanctionedBy   { get; set; } = ""; // APPROVALSUPERVISORNAME
    public string PlanPU         { get; set; } = ""; // LEAVEPLAN
}

}
