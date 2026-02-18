using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class AssetRegistrationViewModel
    {

        //From SearchResultsView
        public A_SearchParameterList SearchParams;

        public List<OrgMappingVM> orgMappingVM; //Dinesh
    }
    public class AssetRegistrationSYKIViewModel
    {
        public List<AssetRegistrationSYKIList> _SYKIList { get; set; }
        public AssetRegistrationSYKIViewModel()
        {
            _SYKIList = new List<AssetRegistrationSYKIList>();
        }
    }

    public class AssetRegistrationSYKIViewModelOH
    {
        public List<AssetRegistrationSYKIListForOH> _SYKIList { get; set; }
        public AssetRegistrationSYKIViewModelOH()
        {
            _SYKIList = new List<AssetRegistrationSYKIListForOH>();
        }
    }

    public class AssetRegistrationSYKIList
    {
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public decimal ACTIVE { get; set; }
        public string DIVISIONNAMEN { get; set; }
        public decimal ADEMPCODE { get; set; }
        public string OperationName { get; set; }
        public string F_YEAR { get; set; } // Added By Aumento :: SR102194


    }

    public class AssetRegistrationSYKIListForOH
    {
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public decimal ACTIVE { get; set; }
        public string DIVISIONNAMEN { get; set; }
        public decimal ADEMPCODE { get; set; }
        public string OperationName { get; set; }
        public long? OPID { get; set; }

    }

    public class AssetRegistrationSearchModel
    {
        public long? Id { get; set; }
        public long? ISSCMemberNominationID { get; set; }
        public long ADEMPCODE { get; set; }

        public string SYKI { get; set; }
        public decimal? SYKIID { get; set; }//Dinesh
        public string OPERATIONID { get; set; }
        public string OPERATIONNAME { get; set; }
        public decimal DIVHDHDID { get; set; }
        public string DIVISIONNAMEN { get; set; }
        public long ADDEDBY { get; set; }
        public string EmpName { get; set; }

        public DateTime NomiationDate { get; set; }

        public string ISSC_Name { get; set; }
        public string Div_Head { get; set; }
        public string Opr_Head { get; set; }
        public decimal? ISSC_EMPCODE { get; set; }
        public decimal? Div_EMPCODE { get; set; }
        public decimal? Op_EMPCODE { get; set; }
    }
   

    public class A_SearchParameterList
{
    public string OPERATION { get; set; }
    public long? OPERATIONID { get; set; }
    public string DIVISION { get; set; }
    public long? DIVISIONID { get; set; }
    public string DEPARTMENT { get; set; }
    public long? DEPARTMENTID { get; set; }
    public string SECTION { get; set; }
    public long? SECTIONID { get; set; }
    public bool Selected { get; set; }
    public string ECode { get; set; }
    public string EmpName;
    public string SYKI { get; set; }
    [DisplayFormat(DataFormatString = "{0:n0}")]
    public decimal? SYKIID { get; set; }
    public short StatusId { get; set; }
    public string Status { get; set; }

    public string isITDivisionHead { get; set; }
    //Dinesh
    public string LastSYKI { get; set; }
    public Decimal LastSKID { get; set; }
    public string LastOPERATION { get; set; }
    public long? LastOPERATIONID { get; set; }
    public string LastDIVISION { get; set; }
    public long? LastDIVISIONID { get; set; }


}


    public class GetNominationDetails
{
    public long Id { get; set; }
    public string Skiid { get; set; }
    public string operationname { get; set; }
    public string divisonname { get; set; }
    public string empname { get; set; }

    public long? DIVISIONID { get; set; }
    public long? OPERATIONID { get; set; }
    public long? Ski { get; set; }
    public long? empcode { get; set; }
}

public class divison
{
    public string divisoid { get; set; }
    public string divisonname { get; set; }
}
public class PeriodForISSCMemberNominationVM
{
    [Required(ErrorMessage = "Please Select Current KI")]
    public decimal SYKIID { get; set; }
    [Required(ErrorMessage = "Please Select Start Date")]
    // public DateTime? StartDate { get; set; }
    public string StartDate { get; set; }
    [Required(ErrorMessage = "Please Select End Date")]
    // public DateTime? EndDate { get; set; }
    public string EndDate { get; set; }
    public long? PeriodForISSCMemberNominationID { get; set; }
    public long? ADDEDBY { get; set; }
    public int? Status { get; set; }
        
        public string? Msg { get; set; }
    /// <summary>
    /// ActionType:0=Add Entry,1=Edit Entry
    /// </summary>
    public int ActionType { get; set; }
    public long? DIVISIONID { get; set; }
    public long? OPERATIONID { get; set; }

}

public class PeriodForISSCMemberNominationNewVM
{
    [Required(ErrorMessage = "Please Select Current KI")]
    public decimal SYKIID { get; set; }
    [Required(ErrorMessage = "Please Select Start Date")]
    public string StartDate { get; set; }
    [Required(ErrorMessage = "Please Select End Date")]
    public string EndDate { get; set; }
    public long? PeriodForISSCMemberNominationID { get; set; }
    public long? ADDEDBY { get; set; }
    public int? Status { get; set; }
    public string Msg { get; set; }
    /// <summary>
    /// ActionType:0=Add Entry,1=Edit Entry
    /// </summary>
    public int ActionType { get; set; }
    public long? DIVISIONID { get; set; }
    public long? OPERATIONID { get; set; }

}

public class PeriodForISSCMemberNominationListVM
{
    public int SNo { get; set; }
    public string? SYKI { get; set; }
    public decimal SYKIID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long? PeriodForISSCMemberNominationID { get; set; }
    public long? ADDEDBY { get; set; }
    public DateTime AdditionDate { get; set; }
    public string EmpName { get; set; }


}
public class AssetRegisterISSCMNPeriodSetting
{

    public decimal ID { get; set; }
    public decimal KIID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    //  public string SYKI { get; set; }
    public long Status { get; set; }
    public DateTime CreationDate { get; set; }
    public long CreatedBy { get; set; }
    public DateTime UpdationDate { get; set; }
    public long UpdatedBy { get; set; }

}
public class AssetRegisterISSCMemberNomination
{
    public decimal ID { get; set; }
    public decimal PeriodSettingID { get; set; }
    public decimal KIID { get; set; }
    public long DivisionID { get; set; }
    public long ISSCEmpCode { get; set; }
    public long Status { get; set; }
    public DateTime CreationDate { get; set; }
    public long CreatedBy { get; set; }
    public DateTime UpdationDate { get; set; }
    public long UpdatedBy { get; set; }

}
public class AssestRegisterAddISSCMemberVM
{
    public long? ISSCMemberNominationID { get; set; }
    public decimal SYKIID { get; set; }
    public long? DIVISIONID { get; set; }
    public long? MEMBEREMPCODE { get; set; }
    public long? OPERATIONID { get; set; }
    public DateTime? ADDEDDate { get; set; }
    public long? ADDEDBY { get; set; }

}


public class PeriodForITGRCMemberNominationListVM
{
    public decimal ID { get; set; }
    public int SNo { get; set; }
    public decimal SYKIID { get; set; }
    public long OPERATIONID { get; set; }
    public long DIVISIONID { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreationDate { get; set; }
    public long UpdatedBy { get; set; }
    public DateTime UpdationDate { get; set; }
    public int Active { get; set; }

    public int? Status { get; set; }
    public string Msg { get; set; }
}

public class PeriodForITGRCMemberNominationListNewVM
{
    public decimal ID { get; set; }
    public int SNo { get; set; }
    public string SYKI { get; set; }
    public decimal SYKIID { get; set; }
    public long OPERATIONID { get; set; }
    public long DIVISIONID { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreationDate { get; set; }
    public long UpdatedBy { get; set; }
    public DateTime UpdationDate { get; set; }
    public int Active { get; set; }
    public string OperationName { get; set; }
    public string DivisionName { get; set; }
    public int? Status { get; set; }
    public string Msg { get; set; }
}

public class ISSCMember_details
{
    public string EmpName { get; set; }

    public string EmpEmail { get; set; }

    public long EmpCode { get; set; }
}
public class AssetRegister_Start_EndDate_ISSC_Member
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class PeriodForITGRCMMemberNominationListVM
{
    public decimal? ID { get; set; }
    public int? SNo { get; set; }

    public string SYKI { get; set; }
    public decimal? SYKIID { get; set; }
    public long? OPERATIONID { get; set; }
    public long? DIVISIONID { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreationDate { get; set; }
    public int? Active { get; set; }

    public string EmpName { get; set; }

    public string OperationName { get; set; }

    public string DivisionName { get; set; }

    public int? ActionType { get; set; }
    public string SelectedIds { get; set; }
    public int? Status { get; set; }
    public string Msg { get; set; }

    }

    public class SameDivisionAndOprationApprovalVM
    {
        public string SYKI { get; set; }
        public decimal SYKIID { get; set; }
        public long OPERATIONID { get; set; }
        public long DIVISIONID { get; set; }
        public long? ExistingOPHeadEmpCode { get; set; }
        public long? ExistingDivHeadEmpCode { get; set; }
        public long? ExistingISSCEmpCode { get; set; }
        public long? NewOPHeadEmpCode { get; set; }
        public long NewDivHeadEmpCode { get; set; }
        public long NewISSCEmpCode { get; set; }
        public long? CreatedBy { get; set; }
    }

    public class BulkUpdatePeriod
    {
        public string SYKI { get; set; }
        public string SelectedIds { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal? ID { get; set; }
        public decimal? CreatedBy { get; set; }
        public int? Status { get; set; }

    }
    public class BulkUpdatePeriodForITGRCMMemberNominationVM
    {
        public PeriodForITGRCMMemberNominationListVM PeriodForITGRC { get; set; }
        public List<PeriodForITGRCMMemberNominationListVM> lstPeriodForITGRC { get; set; }
    }

public class PeriodForITGRCMMemberNominationListNewVM
{
    public decimal ID { get; set; }
    public int SNo { get; set; }
    public string SYKI { get; set; }
    public decimal SYKIID { get; set; }
    public long OPERATIONID { get; set; }
    public long DIVISIONID { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreationDate { get; set; }
    public int Active { get; set; }

    public string EmpName { get; set; }

    public string OperationName { get; set; }

    public string DivisionName { get; set; }

    public int ActionType { get; set; }

}

public class AssetType
{
    public decimal AssetID { get; set; }
    public String ASSETNAME { get; set; }
    public short Status { get; set; }
}

public class AssetClassification
{
    public decimal CLASSIFICATIONID { get; set; }
    public String CLASSIFICATION { get; set; }
    public short Status { get; set; }
}

public class CommonAssetsRegListVM
{
    public decimal ID { get; set; }
    public int SNo { get; set; }
    public decimal SYKIID { get; set; }
    public string Primary { get; set; }
    public string Secondary { get; set; }
    public decimal AssetsID { get; set; }
    public decimal ClassificationID { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreationDate { get; set; }
    public long UpdatedBy { get; set; }
    public DateTime UpdationDate { get; set; }
    public int Active { get; set; }
    public int? Status { get; set; }
    public string Msg { get; set; }
}

    public class AssetApplicabilityUpdateVM
    {
        //public int ApplicabilityID { get; set; }
        public string Remarks { get; set; }
        public List<decimal> SelectedIds { get; set; }
        public long? CreatedBy { get; set; }
        public List<CommonAAssetsRegListVM> lstCommonAAssetsRegListVM { get; set; }
    }
public class CommonAAssetsRegListVM
{
    public decimal ID { get; set; }
    public int SNo { get; set; }
    public decimal SYKIID { get; set; }
    public string SYKIName { get; set; }
    public string Primarytxt { get; set; }
    public string PrimaryId { get; set; }
    public string Primary { get; set; }
    public string Secondary { get; set; }
    public string Assetlocation { get; set; }
    public string AssetOwner { get; set; }
    public string Custodian { get; set; }
    public string AssetUser { get; set; }
    public decimal? AssetsID { get; set; }
    public decimal ClassificationID { get; set; }
    public decimal Classification_Edit_ID { get; set; }
    public string Retention_Month { get; set; }
    public string Retention_Year { get; set; }
    public string Retention_Period { get; set; }

    public string Retention_Remarks { get; set; }
    public long ISSC_Member_EmpCode { get; set; }
    public string ISSC_Member_Reason { get; set; }
   public DateTime? FINALSUBMITDATE_ISSCMEMBER { get; set; }   //  Added by TTL ::  SR103777 > CR6964 

        public string DivisionHead_Remarks { get; set; }
    public string OperatingHead_Remarks { get; set; }
    public DateTime CreationDate { get; set; }
    public string AssetsType { get; set; }
    public string Classification { get; set; }
    public long CreatedBy { get; set; }
    public long UpdatedBy { get; set; }
    public long Division_Head_EmpCode { get; set; }
    public long Operating_Head_EmpCode { get; set; }

    public DateTime UpdationDate { get; set; }
    public decimal Active { get; set; }
    public string EmpName { get; set; }
    public decimal? ActionType { get; set; }
    public string Reason_NA { get; set; }
    //Dinesh
    public string DivisionHeadName { get; set; }
    public string OperatingHeadName { get; set; }
        //**
    public decimal? AMENDMENT_NO { get; set; } //Added By Aumento as on 19042024
        public long ITGRC_Head_EmpCode { get; set; }
        public string ITGRCHead_Remarks { get; set; }
    }
public class Get_Division_ISSC_Member
{
    public decimal? DivId { get; set; }
    public string DivisionName { get; set; }
    public decimal ISSCMember_EmpCode { get; set; }
    public string ISSCMember { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? ISSC_Member_Submit_Status { get; set; }
    public DateTime? ISSC_Member_Submit_Date { get; set; }
    public decimal? DivisionHead_Submit_Status { get; set; }
    public DateTime? DivisionHead_Submit_Date { get; set; }
    public decimal? OperatingHead_Submit_Status { get; set; }
    public DateTime? OperatingHead__Submit_Date { get; set; }
    public string ApplicationEnd_Date { get; set; }
    public string SYKI { get; set; }
    public string ProjectTitle { get; set; }
    public string Url { get; set; }

    public string DivisionHeadSubmitStatus { get; set; }
    public string OperatingHeadSubmitStatus { get; set; }

    public string DivisionHead_Name { get; set; }
    public string OperatingHead_Name { get; set; }
    public string OperationName { get; set; }
        public DateTime? ITGRCHead_Submit_Date { get; set; }
        public decimal? ITGRCHead_Submit_Status { get; set; }
        public string ITGRC_Head_Name { get; set; }
      
        //Added by Aumento for SR91196
        // START :: Added By Aumento :: SR102194
        public decimal? Div_Head_EmpCode { get; set; }
        public decimal? Op_Head_EmpCode { get; set; }
        public decimal ITGRC_Head_EmpCode { get; set; }
        public bool AutoApprove { get; set; }
        public bool IS_ITOPRATION { get; set; }
        // END :: Added By Aumento :: SR102194

    }

    public class Primary_AssetDeatils_VM
{
    public string Id { get; set; }
    public string PrimaryAsset { get; set; }
}

    public class Asset_Dvision_Deatils_VM
    {
        public long? SYKI { get; set; }
        public string SYKIName { get; set; }
        public long? DIVISIONID { get; set; }
        public string DIVISION { get; set; }
    }

    public class DivisionHead_Details
{
    public string Emailid { get; set; }
    public string Empname { get; set; }
    public string DivisionName { get; set; }
    public long DivisionEmpCode { get; set; }
    public string ISSC_Member_Name { get; set; }
    public string ISSC_MemberEmail { get; set; }
    public int? ISSC_Member_Submit_Status { get; set; }
    public DateTime? ISSC_Member_Submit_Date { get; set; }
    public int? DivisionHead_Submit_Status { get; set; }
    public DateTime? DivisionHead_Submit_Date { get; set; }
    public int? OperatingHead_Submit_Status { get; set; }
    public DateTime? OperatingHead__Submit_Date { get; set; }
        public long ITGRCEmpCode { get; set; }
}

public class Asset_Year
{
    public string Id { get; set; }
    public string Text { get; set; }
}

public class DifiencyReport
{
    public long operationId { get; set; }
    public long DivisionId { get; set; }
    public string PrimaryAssetId { get; set; }
    public long SecondaryAsset { get; set; }
    public string FeedBack { get; set; }
    public long createdby { get; set; }

}

public class PrimaryAsset
{
    public decimal PrimaryAssetId { get; set; }
    public string PrimaryAssetText { get; set; }
}

public class EmpDetails
{
    public long? OperationId { get; set; }
    public long? DivisionId { get; set; }
    public string Degination { get; set; }
}

public class DivisionWise_Asset_Count
{
    public string DivisionName { get; set; }
    public decimal TSCount { get; set; }
    public decimal SCount { get; set; }
    public decimal ISCount { get; set; }
    public decimal? ClassificationID { get; set; }
    public decimal Id { get; set; }
}

public class Asset_Approver_Remarks
{
    public string Divison_Remarks { get; set; }
    public DateTime? DivisionDate { get; set; }
    public string Operating_Remarks { get; set; }
    public DateTime? OperatingDate { get; set; }
        public string ITGRC_Remarks { get; set; }
              public DateTime? ITGRCDate {  get; set; }
        //Added by Aumento for SR91196
        public bool IS_ITOPRATION { get; set; } // START :: Added By Aumento :: SR102194

    }

    public class Asset_Approver_Remarks_ISSCM
    {
        public string Divison_Remarks { get; set; }
        public DateTime? DivisionDate { get; set; }
        public string Operating_Remarks { get; set; }
        public DateTime? OperatingDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ITGRC_Remarks { get; set; }
            public DateTime?    ITGRCDate { get; set; }
        public bool IS_ITOPRATION { get; set; } // START :: Added By Aumento :: SR102194

    }

    public class Asset_Deficency_Remarks
{
    public string PrimaryAsset { get; set; }
    public string SecondaryAsset { get; set; }
    public string Remarks { get; set; }
    public DateTime? cdate { get; set; }
}

//Dinesh
public class OrgMappingVM
{
    public decimal OrgMappingId { get; set; }
    public string Previouski { get; set; }
    public string PreviousOperation { get; set; }
    public string PreviousDivision { get; set; }
    public string CurrentKi { get; set; }
    public string CurrentOperation { get; set; }
    public string CurrentDivision { get; set; }
    public decimal? CurrentSYKIID { get; set; }
    public decimal? PreviousSYKIID { get; set; }
    public int? CurrentOperationID { get; set; }
    public int? PreviousOperationID { get; set; }
    public int? CurrentDevisionID { get; set; }
    public int? PreviousDevisionID { get; set; }
    public int? CreatedBY { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int? UpdatedBY { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class MailerReminderVM
{
    public int SNo { get; set; }
    public string SYKI { get; set; }
    public decimal SYKIID { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string ReminerDate { get; set; }
    public long? ReminderId { get; set; }
    public long? ADDEDBY { get; set; }

    public decimal ReminderFor { get; set; }

}

public class ISSC_Member_Nomination_deatilsVM
{
    public string SYKI { get; set; }
    public long Empcode { get; set; }
    public string EMpName { get; set; }
    public string ProjectTitle { get; set; }
    public string EndDate { get; set; }

}

    //Added By Aumento Start
    public class Get_AssetRegister_DigitalSign_Detail
    {
        //public decimal ISSCMember_EmpCode { get; set; }
        //public decimal DivisionHead_EmpCode { get; set; }
        //public decimal OperatingHead_EmpCode { get; set; }
        public string ISSCMember_Name { get; set; }
        public string DivisionHead_Name { get; set; }
        public string OperatingHead_Name { get; set; }
        //public decimal? ISSC_Member_Submit_Status { get; set; }
        public DateTime? ISSC_Member_Submit_Date { get; set; }
        //public decimal? DivisionHead_Submit_Status { get; set; }
        public DateTime? DivisionHead_Submit_Date { get; set; }
        //public decimal? OperatingHead_Submit_Status { get; set; }
        public DateTime? OperatingHead_Submit_Date { get; set; }
        //public string DivisionHeadSubmitStatus { get; set; }
        //public string OperatingHeadSubmitStatus { get; set; }

        public DateTime? ITGRCHead_Submit_Date { get; set; }
        public string ITGRC_Head_Name { get; set; }
               

    }
    public class Get_AssetRegister_Header_Detail
    {
        public string DOCUMENT_TITLE { get; set; }
        public DateTime? DATE_OF_RELEASE { get; set; }
        public string DOC_VERSION_NO { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public string AMENDMENT { get; set; }
        public string OPERATION { get; set; }
        public string DIVISION { get; set; }

    }
    //Added By Aumento End

}
