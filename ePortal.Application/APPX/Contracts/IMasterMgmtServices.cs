using ePortal.ViewModels.APPX.MasterMgmt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.APPX.Contracts
{
    public interface IMasterMgmtServices
    {
        public DataSet BindUserDetail();
        public DataSet PopulateValuationClassByMaterialType(int matrialType, ref string isSales);
        public DataSet BindMMCreationDetails(int userid);
        public MasterResponse GetProfitCentreAndStorageLoc(string plantid);
        public MasterResponse PopulateValuationClassByMaterialType(int matrialType);
        public MasterResponse PopulateValuationClassByMaterialTypeGroup(int matrialType, int MaterialGroup);
        public string ExcelExport(DataTable dt);
    }
}
