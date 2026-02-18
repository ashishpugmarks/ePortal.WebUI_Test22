using Microsoft.AspNetCore.Builder.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.MasterMgmt
{
    public class MMDetailViewModel
    {
        public int? MMDETAILID { get; set; }                // NUMBER (Not Null? -> "No" in your sheet means not nullable)
        public int? MMHEADERID { get; set; }                // NUMBER (nullable)

        //[StringLength(1)]
        public string? INDUSTRYSECTOR { get; set; } = "M";       // VARCHAR2(1) default 'M'

        public int? MATERIALTYPE { get; set; }              // NUMBER
        public int? MATERIALGROUP { get; set; }             // NUMBER
        public int? VALUATIONCLASS { get; set; }            // NUMBER

        // Descriptions
        //[StringLength(40)]
        public string? MATERIALDISCRIPTION { get; set; }    // VARCHAR2(40)

        //[StringLength(40)]
        public string? MATERIALSPECIFICATION { get; set; }  // VARCHAR2(40)

        // Units / Plant
        public int? MEASUREMENTUNIT { get; set; }           // NUMBER
        public int? PLANTCODE { get; set; }                 // NUMBER

        // Logistics / Availability / Loading
        //[StringLength(4)]
        public string? TRANSPORTATIONGROUP { get; set; }    // VARCHAR2(4)

        //[StringLength(2)]
        public string? CHECKINGGRPAVAILABILITYCHECK { get; set; } // VARCHAR2(2)

        //[StringLength(4)]
        public string? LOADINGGROUP { get; set; }           // VARCHAR2(4)

        //[StringLength(4)]
        public string? BASEUNITOFMEASURE { get; set; }      // VARCHAR2(4)

        // MRP / Pricing / Profit center
        //[StringLength(2)]
        public string? MRPTYPE { get; set; }                // VARCHAR2(2)

        //[StringLength(1)]
        public string? PRICECONTROLINDICATOR { get; set; }  // VARCHAR2(1)

        public string? PROFITCENTER { get; set; }

        // Section indicators (defaults noted as 'X' in schema)
        //[StringLength(1)]
        public string? BASICDATA { get; set; } = "X";             // VARCHAR2(1) default 'X'

        //[StringLength(1)]
        public string? PURCHASEDATAFOREIGN { get; set; }    // VARCHAR2(1) default 'X'

        //[StringLength(1)]
        public string? MRP { get; set; }                    // VARCHAR2(1) default 'X'

        //[StringLength(1)]
        public string? PLANTDATASTORAGE { get; set; }       // VARCHAR2(1) default 'X'

        //[StringLength(1)]
        public string? ACCOUNTING { get; set; }             // VARCHAR2(1) default 'X'

        //[StringLength(1)]
        public string? COSTING { get; set; }                // VARCHAR2(1) default 'X'

        // Material code
        public long? MATERIALCODE { get; set; }              // NUMBER

        // Approval flags (NUMBER(1,0) -> bool?)
        public short? ISSUPERVISORAPPROVED { get; set; }     // NUMBER(1,0)
        public short? ISSUPERSUPERVISORAPPROVED { get; set; }// NUMBER(1,0)
        public short? ISPPCAPPROVED { get; set; }            // NUMBER(1,0)
        public short? ISFINANCEAPPROVED { get; set; }        // NUMBER(1,0)
        public short? ISSISAPPROVED { get; set; }            // NUMBER(1,0)

        // Audit
        [DataType(DataType.Date)]
        public DateTime? CREATEDDATE { get; set; }          // DATE

        [DataType(DataType.Date)]
        public DateTime? UPDATEDDATE { get; set; }          // DATE

        // Status flags
        public short? ISSENTBACK { get; set; }               // NUMBER(1,0)
        public short? ACTIVE { get; set; }                   // NUMBER(1,0)

        // Price
        [DataType(DataType.Currency)]
        public decimal? PRICE { get; set; }                 // NUMBER (monetary: decimal?)

        // Request flags
        public short? ISREQUESTSENT { get; set; }            // NUMBER(1,0)
        public short? ISREQUESTCLOSED { get; set; }          // NUMBER(1,0)

        // Alternate IDs / HIDs
        //[StringLength(3)]
        public string? MMDETAILHID { get; set; }            // VARCHAR2(3)

        //[StringLength(18)]
        public string? MMDETAILHEADERID { get; set; }       // VARCHAR2(18)

        // Codes / indicators
        //[StringLength(16)]
        public string? HSNCODE { get; set; }                // VARCHAR2(16)

        //[StringLength(1)]
        public string? MMINDICATER { get; set; }            // VARCHAR2(1)

        // Sales & Distribution
        //[StringLength(100)]
        public string? SALES_ORG { get; set; }              // VARCHAR2(100)

        //[StringLength(100)]
        public string? DISTRI_CHN { get; set; }             // VARCHAR2(100)

        //[StringLength(50)]
        public string? ITEM_CATG_GRP { get; set; }          // VARCHAR2(50)

        //[StringLength(5)]
        public string? AVAIL_CHK { get; set; }              // VARCHAR2(5)

        //[StringLength(100)]
        public string? STORAGE_LOC { get; set; }            // VARCHAR2(100)

        //[StringLength(10)]
        public string? GEN_ITEM_CAT_GRP { get; set; }       // VARCHAR2(10)

        //[StringLength(2)]
        public string? TAX_CLASSIFICATION { get; set; }     // VARCHAR2(2)

        //[StringLength(10)]
        public string? MAT_GRP_PACK_MATLS { get; set; }     // VARCHAR2(10)

        //[StringLength(10)]
        public string? PACKAGING_MAT_TYPE { get; set; }     // VARCHAR2(10)

        //[StringLength(40)]
        public string? MPN_PROFILE { get; set; }            // VARCHAR2(40)

        //[StringLength(10)]
        public string? PLANT_SP_MAT_STATUS { get; set; }    // VARCHAR2(10)
        //[StringLength(20)]
        public string? INT_MATERIAL_NO { get; set; }        // VARCHAR2(20
        //[StringLength(250)]
        public string? SAP_REMARKS { get; set; }            // VARCHAR2(250)
        public short? EXTENDREQTYPE { get; set; }            // NUMBER(1,0) default (0)
        public string? EMAIL_ID { get; set; }
        public string? IsSales { get; set; } = "N";
        public string? MATERIALTYPETEXT { get; set; }
        public string? lstSales { get;set; }

    }
    public class MaterialDetail
    {
        //MMDETAILID	MATERIALDISCRIPTION	MATERIALSPECIFICATION	MMDESCSPEC	
        //MATERIALTYPE	MATERIALGROUP	VALUATIONCLASS	UOM	PRICE	PLANTNAMEDESC	PLANTNAME	PLANT	PLANTCODE	REQ_TYPE
        public int? MMDETAILID { get; set; }
        public int? MMHEADERID { get; set; }
        public string? MATERIALDISCRIPTION { get; set; }
        public string? MATERIALSPECIFICATION { get; set; }
        public string? MMDESCSPEC { get; set; }
        public string? MATERIALTYPE { get; set; }
        public int? MATERIALGROUP { get; set; }
        public int? VALUATIONCLASS { get; set; }
        public string? UOM { get; set; }
        public double? PRICE { get; set; }
        public string? PLANTNAMEDESC { get; set; }
        public string? PLANTNAME { get; set; }
        public string? PLANT { get; set; }
        public int? PLANTCODE { get; set; }
        public string? IsSales { get; set; }
        public string? MATERIALCODE { get; set; }
        public string? SALES_ORG { get; set; }
        public string? DISTRI_CHN { get; set; }
        public string? ITEM_CATG_GRP { get; set; }
        public string? GEN_ITEM_CAT_GRP { get; set; }
        public string? LOADINGGROUP { get; set; }
        public string? TAX_CLASSIFICATION { get; set; }
        public string? BASEUNITOFMEASURE { get; set; }
        public string? AVAIL_CHK { get; set; }
        public string? MPN_PROFILE { get; set; }
        public string? INDICATOR { get; set; }
        public string? PROFITCENTER { get; set; }
        public string? PLANT_SP_MAT_STATUS { get; set; }
        public string? TRANSPORTATIONGROUP { get; set; }
        public string? HSNCODE {  get; set; }
    }

    public class MaterilaRejectedList
    {
        //"MMHEADERID", "MMHEADERDETAILID", "REQUESTERID", "REQUESTERNAME", "REQUESTDATE"
        public int? MMHEADERID { get; set; }
        public string? MMHEADERDETAILID { get; set; }
        public int? REQUESTERID { get; set; }
        public string? REQUESTERNAME { get; set; }
        public string? REQUESTDATE { get; set; }
    }
    public class MMDetailEdit{
        public int? RS { get; set; } = 0;
        public string? MESSAGE { get; set; }
        public string? MATERIALTYPE { get; set; }
        public string? PLANTCODE { get; set; }
        public string? MATERIALCODE { get; set; }
        public string? MATERIALGROUP { get; set; }
        public string? VALUATIONCLASS { get; set; }
        public string? MATERIALDISCRIPTION { get; set; }
        public string? INT_MATERIAL_NO { get; set; }
        public string? MEASUREMENTUNIT { get; set; }
        public string? MATERIALSPECIFICATION { get; set; }
        public string? PRICE { get; set; }
        public string? HSNCODE { get; set; }
        public string? MMINDICATER { get; set; }
        public string? SALES_ORG { get; set; }
        public string? ITEM_CATG_GRP { get; set; }
        public string? GEN_ITEM_CAT_GRP { get; set; }
        public string? AVAIL_CHK { get; set; }
        public string? TAX_CLASSIFICATION { get; set; }
        public string? TRANSPORTATIONGROUP { get; set; }
        public string? LOADINGGROUP { get; set; }
        public string? DISTRI_CHN { get; set; }
        public string? BASEUNITOFMEASURE { get; set; }
        public string? PROFITCENTER { get; set; }
        public string? PACKAGING_MAT_TYPE { get; set; }
        public string? MAT_GRP_PACK_MATLS { get; set; }
        public string? STORAGE_LOC { get; set; }
        public string? ZMPNDESC {  get; set; }
        public string? ISSALES {  get; set; }
        public List<MasterData>? MaterialGroupList {  get; set; }       
        public List<MasterData>? StorageLocList { get; set; }
        public List<MasterData>? ValuationClassList { get; set; }
        public string? lstSales {  get; set; }
        public string? PLANT_SP_MAT_STATUS { get; set; }
        public string? MPN_PROFILE { get; set; }
    }
    public class SearchMaterilaData
    {
        public string? MMId {  get; set; }
        public string? Status { get; set; }
        public string? PlantMst { get; set; }
        public string? DayFrom { get; set; }
        public string? YearFrom { get; set; }
        public string? MonthFrom { get; set; }
        public string? DayTo { get; set; }
        public string? MonthTo { get; set; }
        public string? YearTo { get; set; }
        public string? RequestType { get; set; }
        public string? MaterialType { get; set; }
        public string? MaterialGroup { get; set; }
        public string? ValuationClass { get; set; }
        public string? RequestorId { get; set; }
        public string? MetrialCode { get; set; }
    }
    public class SearchMaterialOutput
    {
        //"MMHEADERID", "MMHEADERDETAILID", "REQUESTDATE", "REQUESTERID", "REQUESTORNAME",
        //"STATUS", "SAP_STATTUS", "SAP_SAPREMARKS" 
        public string? MMHEADERID {  get; set; }
        public string? MMHEADERDETAILID { get; set; }
        public string? REQUESTDATE { get; set; }
        public string? REQUESTERID { get; set; }
        public string? REQUESTORNAME { get; set; }
        public string? STATUS { get; set; }
        public string? SAP_STATTUS { get; set; }
        public string? SAP_SAPREMARKS { get; set; }
        public string? TotalCount {  get; set; }
        public string? IsPPCEditable { get; set; } = "N";

    }
    public class MaterialRequestDetails
    {
      
        public string? SRNO {  get; set; }
        public string? MMDETAILID { get; set; }
        public string? MMDETAILHID { get; set; }
        public string? MATERIALCODE { get; set; }
        public string? MATERIALDISCRIPTION { get; set; }
        public string? MATERIALSPECIFICATION { get; set; }
        public string? UOM { get; set; }
        public string? MATERIALTYPE { get; set; }
        public string? MATERIALGROUP { get; set; }
        public string? VALUATIONCLASS { get; set; }
        public string? PRICE { get; set; }
        public string? PLANTNAME { get; set; }
        public string? REQUESTDATE { get; set; }
        public string? CREATEDDATE { get; set; }
        public string? REQUESTERID { get; set; }
        public string? STATUS { get; set; }
        public string? HSNCODE { get; set; }
        public string? MMINDICATOR { get; set; }
        public string? SALES_ORG { get; set; }
        public string? TRANSPORTATIONGROUP { get; set; }
        public string? LOADINGGROUP { get; set; }
        public string? BASEUNITOFMEASURE { get; set; }
        public string? PROFITCENTER { get; set; }
        public string? DISTRI_CHN { get; set; }
        public string? ITEM_CATG_GRP { get; set; }
        public string? AVAIL_CHK { get; set; }
        public string? STORAGE_LOC { get; set; }
        public string? MATERIALCODE_1 { get; set; }
        public string? MPN_PROFILE { get; set; }
        public string? PLANT_SP_MAT_STATUS { get; set; }
        public string? TAX_CLASSIFICATION { get; set; }
        public string? GEN_ITEM_CAT_GRP { get; set; }
        public string? INT_MATERIAL_NO { get; set; }
        public string? MMHEADERID { get;set; }
        public string? MMHEADERDETAILID { get; set; }



    }
    public class MMApprovalViewModel
    {        
        public string? Requestor { get; set; }
        public string? RequestedDate { get; set; }
        public string? RecommendationBy { get; set; }
        public string? ApprovedBy { get; set; }
        public string? PPCApprovalAuthority { get; set; }
        public string? FinanceApprovalAuthority { get; set; }       
        public string? RecommendationStatus { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? PPCStatus { get; set; }
        public string? FinanceStatus { get; set; }       
        public string? RequestNo { get; set; }        
        public List<MMApprovalDetailItem>? ApprovalDetails { get; set; } 
        }
    public class MMApprovalDetailItem
    {
       public string? DATEADDED { get; set; }
       public string? REQUESTERID { get; set; }
       public string? EMPNAME { get; set; }
       public string? STATUS { get; set; }
       public string? REMARKS { get; set; }
       public string? LINKSTATUS { get; set; }
       public string? MMHEADERDETAILID { get; set; }
    }

    public class MMCreationViewModel
    {
        public string? RequestNo { get; set; }      
        public List<MMCreationDetailItem>? CreationDetails { get; set; }

    }
    public class MMCreationDetailItem
    {        
        public string? MMDETAILHID { get; set; }      
        public string? MATERIALDISCRIPTION { get; set; }     
        public string? MATERIALSPECIFICATION { get; set; }       
        public string? MATERIALTYPE { get; set; }     
        public string? MOD_MATERIALTYPE { get; set; }       
        public string? MATERIALGROUP { get; set; }
        public string? MOD_MATERIALGROUP { get; set; }       
        public string? VALUATIONCLASS { get; set; }      
        public string? MOD_VALUATIONCLASS { get; set; }
    }

    public class MaterialMasterApproval
    {
       public List<SearchMaterialOutput>? MMCreationRequests {  get; set; }
        public List<SearchMaterialOutput>? MMExtendRequests { get; set; }
        public List<SearchMaterialOutput>? MMUpdationRequests { get; set; }
    }
    public class ApprorvalParam
    {
        public string? CommandArgument {  get; set; }
        public string? EditMMDetailIdApprove { get; set; }
        public string? ApproveFormApprovalUpdate { get; set; }

    }

    public class MMCreationRequestRow
    {       
        public string? MMDETAILID { get; set; }
        public string? MMDETAILHID { get; set; }
        public string? PLANTNAME { get; set; }               
        public string? MATERIALDISCRIPTION { get; set; }  
        public string? MATERIALSPECIFICATION { get; set; } 
        public string? MATERIALTYPE { get; set; }
        public string? MATERIALGROUP { get; set; }
        public string? VALUATIONCLASS { get; set; } 
        public string? HSNCODE { get; set; }    
        public string? MMINDICATOR { get; set; }
        public string? UOM { get; set; } 
        public string? MMHEADERDETAILID { get; set; }
        public bool chkApprove { get; set; } = true;
    }
    public class MMApprovalRequest
    {
        public string? MMDetailID { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public string[]? MMDetailHIDs { get; set; }
        public string? IsMultiple { get; set; } = "NO";
    }

    public class MMUpdateApprovalReq
    {
        public int? materialDetailCode { get; set; }
        public int? materialType { get; set; }
        public int? materialGroup { get; set; }
        public int? valuationClass { get; set; }
    }
}
