using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Training
{
    public class RequestHistoryDetailsViewModel
    {

        // Employee Details
        public string empname { get; set; } = string.Empty;
        public string reqremarks { get; set; } = string.Empty;
        public string trainingperiod { get; set; } = string.Empty;
        public string trainingtime { get; set; } = string.Empty;
        public string dateadded { get; set; } = string.Empty;

        // Recommended Authority Details
        public string recEmpCode { get; set; } = string.Empty;
        public string recempname { get; set; } = string.Empty;
        public string recemail { get; set; } = string.Empty;
        public string recstatus { get; set; } = string.Empty;
        public string recremarks { get; set; } = string.Empty;
        public string recdate { get; set; } = string.Empty;
        public string RecmdRescheduledDate { get; set; } = string.Empty;

        // Approving Authority Details
        public string appempname { get; set; } = string.Empty;
        public string appemailid { get; set; } = string.Empty;
        public string appstatus { get; set; } = string.Empty;
        public string appremarks { get; set; } = string.Empty;
        public string appdate { get; set; } = string.Empty;

        // HR Approval Details
        public string hrempname { get; set; } = string.Empty;
        public string hrappemailid { get; set; } = string.Empty;
        public string hrstatus { get; set; } = string.Empty;
        public string hrappremarks { get; set; } = string.Empty;
        public string hrappdate { get; set; } = string.Empty;
        public string RescheduledDate { get; set; } = string.Empty;
    }

}

