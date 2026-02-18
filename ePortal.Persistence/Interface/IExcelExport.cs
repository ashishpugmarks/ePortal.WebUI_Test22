using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.Persistence.Interface
{
    public interface IExcelExport
    {
        FileContentResult ExportDetails(DataTable detailsTable, int[] columnList, string[] headers, ExportFormat formatType, string fileName);
    }
}
