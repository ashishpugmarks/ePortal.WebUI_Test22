using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{

    public class AT_AssetTransferApprovalMasterViewModel
    {
        [DisplayName("SrNo")]
        public long SrNo { get; set; }

        [DisplayName("ApprovalType")]
        public string APPROVAL_TYPE { get; set; }

        [DisplayName("Designation_ID")]
        public string DESIGNATION_ID { get; set; }

        [DisplayName("DESIGNATION")]
        public string DESIGNATION { get; set; }

        [DisplayName("Sequence_No")]
        public int  SEQUENCE_NO { get; set; }
        
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDON { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEON { get; set; }

        [DisplayName("REMARKS")]
        public string REMARKS { get; set; }

        public List<AT_AssetTransferApprovalMasterViewModel> AT_AssetTransferApprovalMaster { get; set; }
    }
    public class AT_AssetTransferViewModel
    {
        public List<AT_AssetTransferHeaderViewModel> HeaderViewModel { get; set; }

        public List<AT_AssetTransferAuthorityViewModel> AuthorityViewModel { get; set; }

        public List<AT_AssetTransferDetailViewModel> DetailViewModel { get; set; }

     

    }
    public class AT_AssetTransferHeaderViewModel
    {

        [DisplayName("SrNo")]
        public long SRNO { get; set; }

        [DisplayName("TranType")]
        public string TRAN_TYPE { get; set; }

        [DisplayName("Tran_No")]
        public long TRAN_NO { get; set; }

        [DisplayName("Transferor")]
        public string TRANSFEROR { get; set; }

        [DisplayName("Approval Type")]
        public string APPROVAL_TYPE { get; set; }

        [DisplayName("Transfree")]
        public string TRANSFREE { get; set; }

        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDON { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEON { get; set; }

        [DisplayName("Tran Date")]
        public DateTime TRAN_DATE { get; set; }

        [DisplayName("Asset Type")]
        public string ASSET_TYPE { get; set; }

        [DisplayName("Total")]
        public decimal TOTAL { get; set; }

        [DisplayName("Status")]
        public string STATUS { get; set; }

        [DisplayName("APPAUTHSRNO")]
        public int APPAUTHSRNO { get; set; }

        [DisplayName("PDFPATH")]
        public string PDFPATH { get; set; }

        [DisplayName("GETOUT_DOC_PATH")]
        public string GETOUT_DOC_PATH { get; set; }

        [DisplayName("Fully/Partial")]
        public string FULLY_PARTIAL { get; set; }

        [DisplayName("Transfree Type")]
        public string TRANSFREE_TYPE { get; set; }

        [DisplayName("GETIN_DOC_PATH")]
        public string GETIN_DOC_PATH { get; set; }

        [DisplayName("DURATION")]
        public string DURATION { get; set; }


    }

    public class AT_AssetRequestorDetail
    {
        
        [DisplayName("AssetType")]
        public string ASSET_TYPE { get; set; }

        [DisplayName("Requested By")]
        public string Requested_By { get; set; }

        [DisplayName("Operation")]
        public string OPERATION { get; set; }

        [DisplayName("Division")]
        public string DIVISION { get; set; }

        [DisplayName("Department")]
        public string DEPARTMENT { get; set; }
        
        [DisplayName("Section")]
        public string SECTION { get; set; }
        
    }

    public class AT_AssetTransferAuthorityViewModel
    {
        [DisplayName("SrNo")]
        public long SrNo { get; set; }

        [DisplayName("TranNo")]
        public long TRAN_NO { get; set; }

        [DisplayName("EMP code")]
        public Nullable<long> ADEMPCODE { get; set; }

        [DisplayName("Employee Name")]
        public string ADEMPNAME { get; set; }

        [DisplayName("Designation")]
        public string ADDESIGNATION { get; set; }

        [DisplayName("Approval Status")]
        public string APPROVAL_STATUS { get; set; }

        [DisplayName("Department")]
        public string DEPARTMENT { get; set; }

        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDON { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEON { get; set; }

        [DisplayName("REMARKS")]
        public string REMARKS { get; set; }

        [DisplayName("Sequence No")]
        public Nullable<decimal> SEQUENCE_NO { get; set; }

       
    }

    public class AT_AssetTransferApprovalViewModel
    {

        [DisplayName("SRNO")]
        public long SRNO { get; set; }

        [DisplayName("Tran_No")]
        public long TRAN_NO { get; set; }

        [DisplayName("TranType")]
        public string TRAN_TYPE { get; set; }

        [DisplayName("Asset Type")]
        public string ASSET_TYPE { get; set; }

        [DisplayName("Tran Date")]
        public DateTime TRAN_DATE { get; set; }

        [DisplayName("Transferor")]
        public string TRANSFEROR { get; set; }

        [DisplayName("Transfree")]
        public string TRANSFREE { get; set; }

        [DisplayName("Total")]
        public decimal TOTAL { get; set; }

        [DisplayName("Approval Status")]
        public string APPROVAL_STATUS { get; set; }

       [DisplayName("APPAUTHSRNO")]
        public int APPAUTHSRNO { get; set; }

        [DisplayName("HeaderStatus")]
        public string HEADERSTATUS { get; set; }

        [DisplayName("PDFPATH")]
        public string PDFPATH { get; set; }

        [DisplayName("GETOUT_DOC_PATH")]
        public string GETOUT_DOC_PATH { get; set; }

        [DisplayName("Transfree Type")]
        public string TRANSFREE_TYPE { get; set; }

        [DisplayName("GETIN_DOC_PATH")]
        public string GETIN_DOC_PATH { get; set; }

    }

    public class AT_AssetTransferDetailViewModel
    {
        [DisplayName("SrNo")]
        public long SrNo { get; set; }

        [DisplayName("Tran No")]
        public long TRAN_NO { get; set; }

        [DisplayName("Asset Code")]
        public long ASSET_CODE { get; set; }

        [DisplayName("Po No")]
        public string PO_NO { get; set; }

        [DisplayName("Po Date")]
        public DateTime PO_DATE { get; set; }

        [DisplayName("Fully/Partial")]
        public string FULLY_PARTIAL { get; set; }

        [DisplayName("Qty")]
        public long QTY { get; set; }

        [DisplayName("Current Location")]
        public string CURRENT_LOCATION { get; set; }

        [DisplayName("New Location")]
        public string NEW_LOCATION { get; set; }

        [DisplayName("Original Cost")]
        public long ORIGINAL_COST { get; set; }

        [DisplayName("Depreciation")]
        public long DEPRECIATION { get; set; }

        [DisplayName("Net Block")]
        public long NET_BLOCK { get; set; }

        [DisplayName("Inv No")]
        public long INV_NO { get; set; }

        [DisplayName("Inv Date")]
        public DateTime INV_DATE { get; set; }

        [DisplayName("Asset Serial No")]
        public long ASSET_SERIAL_NO { get; set; }

        [DisplayName("Asset Class")]
        public string ASSET_CLASS { get; set; }

        [DisplayName("Capitalized Date")]
        public DateTime CAPITALIZED_DATE { get; set; }

        [DisplayName("Vendor Name")]
        public string VENDER_NAME { get; set; }

        [DisplayName("Vendor Code")]
        public long VENDOR_CODE { get; set; }

        [DisplayName("AssetMain No Text")]
        public string ASSETMAIN_NO_TEXT { get; set; }

        [DisplayName("License No")]
        public long LICENSE_NO { get; set; }

        [DisplayName("License Date")]
        public DateTime LICENSE_DATE { get; set; }

        [DisplayName("HSN Code")]
        public long HSN_CODE { get; set; }

        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDON { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEON { get; set; }

        [DisplayName("Description")]
        public string DESCRIPTION { get; set; }

        [DisplayName("NEW_ASSET_CODE")]
        public string NEW_ASSET_CODE { get; set; }
    }

    public class GetAssetFormHeader
    {
        public string Tran_Type { get; set; }
      
        public string Plant { get; set; }
        public string Designation { get; set; }
        public string Transferor { get; set; }
        public string Transferee { get; set; }

        public string Remarks { get; set; }

        public string Duration { get; set; }


    }

    public class GetAuthDetail
    {
        public string Preparedby { get; set; }
        public string DepartmentHead { get; set; }
        public string Coordinator { get; set; }
        public string DivisionHead { get; set; }
        public string ExecutiveCoordinator { get; set; }
        public string OperatingHead { get; set; }
        public string ExecutiveVicePresident { get; set; }
        public string Director { get; set; }
        public string Directortwo { get; set; }
        public string CPO { get; set; }
        public string TeamMember { get; set; }
        public string FSectionHead { get; set; }
        public string FDepartmentHead { get; set; }
        public string FDivisionHead { get; set; }

        public string Remarks { get; set; }

        public DateTime Preparedbydate { get; set; }
        public DateTime DepartmentHeaddate { get; set; }
        public DateTime Coordinatordate { get; set; }
        public DateTime DivisionHeaddate { get; set; }
        public DateTime ExecutiveCoordinatordate { get; set; }
        public DateTime OperatingHeaddate { get; set; }
        public DateTime ExecutiveVicePresidentdate { get; set; }
        public DateTime Directordate { get; set; }
        public DateTime Directortwodate { get; set; }
        public DateTime CPOdate { get; set; }
        public DateTime TeamMemberdate { get; set; }
        public DateTime FSectionHeaddate { get; set; }
        public DateTime FDepartmentHeaddate { get; set; }
        public DateTime FDivisionHeaddate { get; set; }
    }

    public class AT_AssetTransferMISReportViewModel
    {

        [DisplayName("SrNo")]
        public long SRNO { get; set; }

        [DisplayName("Plant/HO/RO/ZO")]
        public string Plant { get; set; }

        [DisplayName("Tran_No")]
        public long TRAN_NO { get; set; }

        [DisplayName("Tran Date")]
        public DateTime TRAN_DATE { get; set; }

        [DisplayName("TranType")]
        public string TRAN_TYPE { get; set; }

        [DisplayName("Asset Type")]
        public string ASSET_TYPE { get; set; }

        [DisplayName("Transfree Type")]
        public string TRANSFREE_TYPE { get; set; }


        [DisplayName("Transferor User")]
        public string TRANSFEROR { get; set; }

        [DisplayName("Operation")]
        public string OPERATION { get; set; }

        [DisplayName("Division")]
        public string DIVISION { get; set; }
        
        [DisplayName("Department")]
        public string DEPARTMENT { get; set; }
        
        [DisplayName("Transferee User")]
        public string TRANSFREE { get; set; }


        [DisplayName("Asset Code")]
        public string ASSET_CODE { get; set; }

        [DisplayName("Asset Class Description")]
        public string ASSET_CLS_DESC { get; set; }

        [DisplayName("Asset Description")]
        public string DESCRIPTION { get; set; }

        [DisplayName("Asset Main Text")]
        public string ASSETMAIN_NO_TEXT { get; set; }

        [DisplayName("Vendor Code")]
        public string VENDOR_CODE { get; set; }

        [DisplayName("Vendor Name")]
        public string VENDER_NAME { get; set; }
        
        [DisplayName("Po No")]
        public string PO_NO { get; set; }

        [DisplayName("Po Date")]
        public Nullable<DateTime> PO_DATE { get; set; }

        [DisplayName("Inv No")]
        public string INV_NO { get; set; }

        [DisplayName("Inv Date")]
        public Nullable<DateTime> INV_DATE { get; set; }

        [DisplayName("Fully/Partial")]
        public string FULLY_PARTIAL { get; set; }

        [DisplayName("Qty")]
        public Nullable<decimal> QTY { get; set; }

        [DisplayName("Current Location")]
        public string CURRENT_LOCATION { get; set; }

        [DisplayName("New Location")]
        public string NEW_LOCATION { get; set; }

        [DisplayName("Original Cost")]
        public decimal ORIGINAL_COST { get; set; }

        [DisplayName("Depreciation")]
        public decimal DEPRECIATION { get; set; }

        [DisplayName("Net Block")]
        public decimal NET_BLOCK { get; set; }
        
        [DisplayName("Basic Price")]
        public decimal BASIC_PRICE { get; set; }

        [DisplayName("Invoice Value")]
        public decimal INVOICE_VALUE { get; set; }

        [DisplayName("New Asset Code")]
        public string NEW_ASSET_CODE { get; set; }

        [DisplayName("EODC_CLEAREANSE")]
        public string EODC_CLEAREANSE { get; set; }

        [DisplayName("Status")]
        public string STATUS { get; set; }

      

    }
}
