using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ePortal.ViewModels.APPX.QMS
{
    public class QcProc1ViewModel
    {
        public QcProc1ViewModel()
        {
            ddlPlantList = new();
            ddlDeptList = new();
        }
        public List<SelectListItem> ddlPlantList { get; set; }
        public List<SelectListItem> ddlDeptList { get; set; }

        public StringBuilder? strhtml { get; set; }    
        public string? ddlPlant { get; set;}
        public string? ddlDept { get; set; }
        public string? QCPROCSESSION { get; set; }
    }

    public class GrdDeptProcViewModel
    {
        public string? Descrip { get; set; }
        public string? Plant { get; set; }
        public string? ADDEPARTMENTID { get; set; }
        public string? PLANTID { get; set; }

    }

    public class GetgrdDeptProcList_DTO
    {
        public string? plant { get; set; }
        public string? DEPTID { get; set; }
        public bool isChangedPlant { get; set; }


    }


    public class QCPROCSESSION
    {
        public string DEPARTMENTID { get; set; } = string.Empty;
        public string PLANTID { get; set; } = string.Empty;
        public string FILTERPLANT { get; set; } = string.Empty;
        public string FILTERDEPTID { get; set; } = string.Empty;
        public string DEPTNAME { get; set; } = string.Empty;
        public int GRIDPAGENO { get; set; } = 0;
    }
}
