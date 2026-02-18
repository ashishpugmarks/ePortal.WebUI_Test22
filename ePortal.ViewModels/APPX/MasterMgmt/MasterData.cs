using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.MasterMgmt
{
    public class MasterData
    {
        public long? Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
    }
    public class MasterResponse
    {
        public int? Rs { get; set; } = 0;
        public string? Message { get; set; }
        public List<MasterData>? list { get; set; } //storage location
        public string? ProfitCentre { get; set; } = string.Empty;
        public string? ISSALES { get; set; }
        public string? MATERIALDISCRIPTION { get; set; }
        public string? MATERIALSPECIFICATION { get; set; }
        public string? TRANSPORTATIONGROUP { get; set; }
        public string? LOADINGGROUP { get; set; }
        public string? BASEUNITOFMEASURE { get; set; }
        public string? DISTRI_CHN { get; set; }
        public string? ITEM_CATG_GRP { get; set; }
        public string? AVAIL_CHK { get; set; }
        public string? GEN_ITEM_CAT_GRP { get; set; }
        public string? STORAGE_LOC { get; set; }
        public string? TAX_CLASSIFICATION { get; set; }
        public string? MAT_GRP_PACK_MATLS { get; set; }
        public string? PACKAGING_MAT_TYPE { get; set; }
        public string? MATERIALTYPE { get; set; }
        public List<MasterData>? Plantlist { get; set; } //Plant List

    }
    public class CheckMaterialCodeData
    {
        public string? MaterialCode { get; set; }
        public int? PlantCode { get; set; }
    }
   
}