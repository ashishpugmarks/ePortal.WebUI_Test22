using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web;
using System;
using System.ComponentModel.DataAnnotations;
using ePortal.DomainClasses;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;

namespace ePortal.ViewModels
{
    public partial class A00ViewModel
    {
        public List<SelectListItem> GetAllmonth;

        public List<SelectListItem> Getdate;

        public string OPERATION_PROPOSING { get; set; }
        public string PRJCTTXT { get; set; }

        public string BUKPITXT { get; set; }
        public string PURPSTXT { get; set; }
        public long A00DTLTBID { get; set; }


        public String ADDEDBY1 { get; set; }
        public string Remarks { get; set; }
        public class SearchResults
        {
            public int SNo { get; set; }
            public string ki { get; set; }

            public int DIVISION { get; set; }
            public string ECode { get; set; }
            public string EmpName { get; set; }
            public string ProjectTitle { get; set; }
            public string RequestDate { get; set; }
            public string Action { get; set; }
            public string View { get; set; }
            public string ApprovedBy { get; set; }
            public string ApprovedOn { get; set; }
        }

        public string Projecttitle { get; set; }
        public string Backgrounds { get; set; }
        public string BusinessKPI { get; set; }
        public string A001Purpose { get; set; }
        public short Budgeted { get; set; }
        public string TargetA00 { get; set; }

        public string Ifothers { get; set; }

        public string PurposeA00 { get; set; }
        public string RequirmentA00 { get; set; }
        public bool Selected { get; set; }

        public int Month { get; set; }

        public string years { get; set; }

        public string LEVELDESCRIP { get; set; }
        public long ADORGLEVELID { get; set; }

        public string StartDate { get; set; }

        public int StartMonth { get; set; }

        public string StartYears { get; set; }

        public string EndDate { get; set; }

        public int EndMonth { get; set; }

        public string EndYears { get; set; }

        public List<SelectListItem> CountryList { get; set; }

        public decimal SYKI { get; set; }

        public string KICODE { get; set; }

        [DisplayName("Upload File")]
        public string ImagePath { get; set; }

        //public IFormFile ImageFile { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<SelectListItem> getAllDaysList { get; set; }

        public List<SelectListItem> GetAllmonthList { get; }

        public List<SelectListItem> getAllWeekDaysList()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                 new SelectListItem{ Value="1",Text="2020"},
                 //new SelectListItem{ Value="2",Text="Tuesday"},
                 //new SelectListItem{ Value="3",Text="Wednesday"},
                 //new SelectListItem{ Value="4",Text="Thrusday"},
                 //new SelectListItem{ Value="5",Text="Friday"},
                 //new SelectListItem{ Value="6",Text="Saturday"},
                 //new SelectListItem{ Value="7",Text="Sunday"},
             };
            myList = data.ToList();
            return myList;
        }
        public List<SelectListItem> getMonth()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                 new SelectListItem{ Value="1",Text="Jan"},
                 new SelectListItem{ Value="2",Text="Feb"},
                 new SelectListItem{ Value="3",Text="Mar"},
                 new SelectListItem{ Value="4",Text="Apr"},
                 new SelectListItem{ Value="5",Text="May"},
                 new SelectListItem{ Value="6",Text="Jun"},
                 new SelectListItem{ Value="7",Text="Jul"},
                 new SelectListItem{ Value="8",Text="Aug"},
                 new SelectListItem{ Value="9",Text="Sep"},
                 new SelectListItem{ Value="10",Text="Oct"},
                 new SelectListItem{ Value="11",Text="Nov"},
                 new SelectListItem{ Value="12",Text="Dec"},

             };
            myList = data.ToList();
            return myList;
        }
        public List<SelectListItem> GetAllDate()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                 new SelectListItem{ Value="1",Text="1"},
                 new SelectListItem{ Value="2",Text="2"},
                 new SelectListItem{ Value="3",Text="3"},
                 new SelectListItem{ Value="4",Text="4"},
                 new SelectListItem{ Value="5",Text="5"},
                 new SelectListItem{ Value="6",Text="6"},
                 new SelectListItem{ Value="7",Text="7"},
                 new SelectListItem{ Value="8",Text="8"},
                 new SelectListItem{ Value="9",Text="9"},
                 new SelectListItem{ Value="10",Text="10"},
                 new SelectListItem{ Value="11",Text="11"},
                 new SelectListItem{ Value="12",Text="12"},
                  new SelectListItem{ Value="13",Text="13"},
                 new SelectListItem{ Value="14",Text="14"},
                 new SelectListItem{ Value="15",Text="15"},
                 new SelectListItem{ Value="16",Text="16"},
                 new SelectListItem{ Value="17",Text="17"},
                 new SelectListItem{ Value="18",Text="18"},
                  new SelectListItem{ Value="19",Text="19"},
                 new SelectListItem{ Value="20",Text="20"},
                 new SelectListItem{ Value="21",Text="21"},
                 new SelectListItem{ Value="22",Text="22"},
                 new SelectListItem{ Value="23",Text="23"},
                 new SelectListItem{ Value="24",Text="24"},
                  new SelectListItem{ Value="25",Text="25"},
                 new SelectListItem{ Value="26",Text="26"},
                 new SelectListItem{ Value="27",Text="27"},
                  new SelectListItem{ Value="28",Text="28"},
                 new SelectListItem{ Value="29",Text="29"},
                 new SelectListItem{ Value="30",Text="30"},
                 new SelectListItem{ Value="31",Text="31"},


             };
            myList = data.ToList();
            return myList;
        }

        public class SykiViewModel
        {
            public short SykiId { get; set; }
            public string KiCode { get; set; }
            public string FinancialYear { get; set; }
            public int Active { get; set; }
        }

        public string Start { get; set; }

        public string BudgetedSelected { get; set; }

        //From SearchResultsView
        public SearchParameterList SearchParams;

        public A00Deficiency Deff;

        public List<SearchResultList> ResultList;
        //public  A00OPERATIONlist oper;
        //public OPERATIONlist op;

        //public A00DEPARTMENTlist aoodep;
        //public DEPARTMENTlist dep;
        //public SECTIONList sec;
        //public A00SECTIONList A00sec;
        //public A00DIVISIONlist a00div;
        //public DIVISIONlist div;



    }
    public class AssetRegisterOperationHeadData
    {
        public long? OperatingHeadEmpCode { get; set; }
        public long? DivisionHeadEmpCode { get; set; }
        public long? ISSCMemCode { get; set; }
        public List<SearchParameterList> divList { get; set; }
    }
    public class SearchParameterList
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
        public List<SelectListItem> StatusList { get; set; }
        public List<SelectListItem> KyList { get; set; }
        public string isITDivisionHead { get; set; }



        public class A00OPERATIONlist
        {
            public List<OPERATIONlist> _OPERATIONlist { get; set; }
            public A00OPERATIONlist()
            {
                _OPERATIONlist = new List<OPERATIONlist>();
            }

        }
        public class OPERATIONlist
        {
            public string OPERATION { get; set; }
            public long? OPERATIONID { get; set; }
            public decimal ACTIVE { get; set; }

            public string LEVELDESCRIP { get; set; }
            public long ADORGLEVELID { get; set; }

            //LEVELDESCRIP = lvlList.LEVELDESCRIP, ADORGLEVELID 
        }



        public class A00DEPARTMENTlist
        {
            public List<DEPARTMENTlist> _DEPARTMENTlist { get; set; }
            public A00DEPARTMENTlist()
            {
                _DEPARTMENTlist = new List<DEPARTMENTlist>();
            }
        }
        public class DEPARTMENTlist
        {
            public string DEPARTMENT { get; set; }
            public long? DEPARTMENTID { get; set; }
            public decimal ACTIVE { get; set; }

            public string LEVELDESCRIP { get; set; }
            public long ADORGLEVELID { get; set; }
        }
        public class A00SECTIONList
        {
            public List<SECTIONList> _SECTIONlist { get; set; }
            public A00SECTIONList()
            {
                _SECTIONlist = new List<SECTIONList>();
            }


        }
        public class SECTIONList
        {
            public string SECTION { get; set; }
            public long? SECTIONID { get; set; }
            public decimal ACTIVE { get; set; }

            public string LEVELDESCRIP { get; set; }
            public long ADORGLEVELID { get; set; }
        }
        public class A00DIVISIONlist
        {
            public List<DIVISIONlist> _DIVISIONlist { get; set; }
            public A00DIVISIONlist()
            {
                _DIVISIONlist = new List<DIVISIONlist>();
            }

        }
        public class DIVISIONlist
        {
            public int? ID { get; set; }
            public decimal ACTIVE { get; set; }
            public string LEVELDESCRIP { get; set; }
            public long ADORGLEVELID { get; set; }
            public long? DIVISIONID { get; set; }
            public string DIVISION { get; set; }

        }
    }

    public class SearchResultList
    {
        public string ApprovedBy;
        public DateTime? ApprovedOn;
        // public string ApprovedRemark;
        public string EmpName;
        public string PRJCTTLE;
        public string ProjectTitle;
        public string ProjectCode;
        public DateTime? A00ApprovedOn;
        public DateTime? A00PPCApprovedOn;//Added by Eshant on 26-Aug-2022 for PPC date as approved date
        public DateTime? ITConfirmationDate;
        public DateTime? RequestDate;
        public string DaysDiffBetA00ApprovalAndITConfirmation;

        public string Action { get; set; }
        public string View { get; set; }
        [Key]
        public long A00DTLTBID { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SYKIID { get; set; }

        public string PRJCTTXT { get; set; }
        public string BUKPITXT { get; set; }
        public string PURPSTXT { get; set; }
        public string TRGTINDCD { get; set; }
        public string RQUMTTXT { get; set; }
        public short BUDGETFLG { get; set; }
        public decimal BUDGETSYKIID { get; set; }
        public long ADORGLEVELID { get; set; }
        public System.DateTime STARTDT { get; set; }
        public System.DateTime ENDDT { get; set; }
        public string FRCSTOTHER { get; set; }
        public string ATTACHMENT { get; set; }
        public short STATUSCD { get; set; }
        public short ACTIVE { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime? DATEADDEDID { get; set; }
        public Nullable<long> LSTMODBYID { get; set; }
        public Nullable<System.DateTime> LASTMODDATE { get; set; }
        public long? ECode { get; set; }
        public string ki { get; set; }
        public int SNo { get; set; }
        public short Budgeted { get; set; }
        public string LEVELDESCRIP { get; set; }

        public decimal SYKI { get; set; }

        public string KICODE { get; set; }
        public string OPERATION { get; set; }
        public string DIVISION { get; set; }//Added by Eshant on 17-Aug-22 to show Division on A00 View page  as per Review Meeting 01-aug-22
        public bool IsDeficiencyClosed { get; set; }
        public bool IsDeficiencyRaised { get; set; }
        public int DeficiencyStatus { get; set; }
        public bool IsAllocationExist { get; set; }
        public long UserId { get; set; }
        public long MainPIC { get; set; }
        public decimal? ApproverUserId { get; set; }
        public short? ITConfirmationStatus { get; set; }

        //Dinesh
        public string HSDMCR { get; set; }
        public string ActivityTitle { get; set; }
        public string ActivityStartDate { get; set; }
        public string ActivityEndDate { get; set; }

        public decimal? ActivityId { get; set; }
        public short? ActivitySchedule { get; set; }
        public int? ProjectUpdateStatus { get; set; }

        public string ProjectStage { get; set; }
        public string ActivityRemark { get; set; }
        public string Main_PIC_Name { get; set; }
        public string ActivityDetail { get; set; }
        public DateTime? ActivityRemarkDate { get; set; }
        public short? IsDeficiencyExist { get; set; }

        public DateTime? TargetActionOn { get; set; }//AddedBy Eshant to Display Current stage Target Date on 31-01-2023
        public string TargetActionOnString { get; set; }
    }


    public class A00SearchModel
    {
        public long? a00dtltbid { get; set; }
        public long ADEMPCODE { get; set; }
        public decimal SYKI { get; set; }
        public long? DEPTHDID { get; set; }
        public long? COORDDID { get; set; }
        public long? DIVHDHDID { get; set; }
        public long? EXECOHDID { get; set; }
        public long? OHID { get; set; }
        public long? OPERATIONID { get; set; }
        public long ADDEDBY { get; set; }
        public decimal SYKIID { get; set; }
    }

    public class A00DtlViewModel
    {
        public long A00DTLTBID { get; set; }
        public decimal? SYKIID { get; set; }
        public string PRJCTTLE { get; set; }
        public string PRJCTTXT { get; set; }
        public string BUKPITXT { get; set; }
        public string PURPSTXT { get; set; }
        public string TRGTINDCD { get; set; }
        public string RQUMTTXT { get; set; }
        public short BUDGETFLG { get; set; }
        public decimal? BUDGETSYKIID { get; set; }
        public short? ADORGLEVELID { get; set; }
        public DateTime? STARTDT { get; set; }
        public DateTime? ENDDT { get; set; }
        public string FRCSTOTHER { get; set; }
        public string ATTACHMENT { get; set; }
        public short STATUSCD { get; set; }
        public short ACTIVE { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime DATEADDEDID { get; set; }
        public short LSTMODBYID { get; set; }
        public DateTime LASTMODDATE { get; set; }
        public int? OPERATION { get; set; }
        public int? DIVISION { get; set; }
        public int? DEPARTMENT { get; set; }
        public int? SECTION { get; set; }
    }

    public class A00SYKIViewModel
    {
        public List<A00SYKIList> _SYKIList { get; set; }


        public A00SYKIViewModel()
        {
            _SYKIList = new List<A00SYKIList>();
        }
    }

    //Added by kiran 18-12-2020



    //End by kiran 18-12-2020




    public class A00SYKIList
    {
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public decimal ACTIVE { get; set; }
    }

    public class A00DataViewModel
    {

        public List<A00ADORGLEVELList> _ADOrgLevelList { get; set; }
        public List<A00_INVFORCASTList> _InvforcastList { get; set; }
        public List<A00INVFORCASTList> _A00InvforcastList { get; set; }

        //public List<DEPARTMENTlist> _DEPARTMENTlist { get; set; }
        //public List<SECTIONList> _SECTIONlist { get; set; }
        //public List<DIVISIONlist> _DIVISIONlist { get; set; }

        //public List<OPERATIONlist> _OPERATIONlist { get; set; }

        ///// <summary>
        /// 
        /// </summary>
        public List<A00ADEMPLOYEE> _A00ADEMPLOYEE { get; set; }
        public List<A00_ADORGLEVELHEAD> _A00_ADORGLEVELHEAD { get; set; }
        public List<A00DtlViewModelList> _A00DtlViewModelList { get; set; }
        public List<A00APPROVALList> _A00APPROVALList { get; set; }
        public List<A00_VW_ASSOCIATELVLDETAILS_FULL> _A00_VW_ASSOCIATELVLDETAILS_FULL { get; set; }

        //public List<VW_ASSOCIATELVLDETAIL> VW_ASSOCIATELVLDETAILS { get; set; }
        public List<A00Allocation> _A00AllocationList { get; set; }

        public List<A00AllocationListVM> _A00AllocationListVM { get; set; }
        public A00DataViewModel()
        {
            _ADOrgLevelList = new List<A00ADORGLEVELList>();
            _InvforcastList = new List<A00_INVFORCASTList>();
            _A00InvforcastList = new List<A00INVFORCASTList>();
            //_A00Approval = new A00_APPROVAL();
            _A00ADEMPLOYEE = new List<A00ADEMPLOYEE>();
            _A00_ADORGLEVELHEAD = new List<A00_ADORGLEVELHEAD>();
            _A00DtlViewModelList = new List<A00DtlViewModelList>();
            _A00APPROVALList = new List<A00APPROVALList>();
            _A00_VW_ASSOCIATELVLDETAILS_FULL = new List<A00_VW_ASSOCIATELVLDETAILS_FULL>();
            _A00AllocationListVM = new List<A00AllocationListVM>();
            _A00AllocationList = new List<A00Allocation>();
            //_DEPARTMENTlist = new List<DEPARTMENTlist>();
            //_SECTIONlist = new List<SECTIONList>();
            //_DIVISIONlist = new List<DIVISIONlist>();
            //_OPERATIONlist = new List<OPERATIONlist>();

            //     public List<DEPARTMENTlist> _DEPARTMENTlist { get; set; }
            //public List<SECTIONList> _SECTIONlist { get; set; }
            //public List<DIVISIONlist> _DIVISIONlist { get; set; }

            //public List<OPERATIONlist> _OPERATIONlist { get; set; }
        }
        public List<A00AcitivtyVM> _A00ActivityList { get; set; }

        public List<ITConfirmationVM> _A00ITConfirmationList { get; set; }

        public List<A00ProjectStateUpdate> _A00ProjectUpdateStatusList { get; set; }

        //07-Sept-2021 change start
        // public List<A00_ADORGCOORDINATORDataList> _A00_ADORGCOORDINATOR { get; set; }
        //07-Sept-2021 change end
    }

    //07-Sept-2021 change start
    public class A00_ADORGCOORDINATORDataList
    {
        public long? ADORGLEVELID { get; set; }
        public string myData { get; set; }
    }
    //07-Sept-2021 change end


    public class A00APPROVALList
    {
        public long A00APPROVALID { get; set; }
        public long A00DTLTBID { get; set; }
        public Nullable<long> DEPTHDID { get; set; }
        public Nullable<System.DateTime> DEPTHDAPPDATE { get; set; }
        public string DEPTHDAPPTXT { get; set; }
        public Nullable<long> COORDDID { get; set; }
        public Nullable<System.DateTime> COORDAPPDATE { get; set; }
        public string COORDAPPTXT { get; set; }
        public Nullable<long> DIVHDHDID { get; set; }
        public Nullable<System.DateTime> DIVHDAPPDATE { get; set; }
        public string DIVHDAPPTXT { get; set; }
        public Nullable<long> EXECOHDID { get; set; }
        public Nullable<System.DateTime> EXECOAPPDATE { get; set; }
        public string EXECOAPPTXT { get; set; }
        public Nullable<long> OHID { get; set; }
        public Nullable<System.DateTime> OHAPPDATE { get; set; }
        public string OHAPPTXT { get; set; }

        //09-Sept-2021 change start
        public Nullable<long> PPCHOOHID { get; set; }
        public Nullable<System.DateTime> PPCHOOHDATE { get; set; }
        public string PPCHOOHTXT { get; set; }
        //09-Sept-2021 change end
    }

    public class A00DtlViewModelList
    {
        public long A00DTLTBID { get; set; }
        public decimal? SYKIID { get; set; }
        public string PRJCTTLE { get; set; }
        public string PRJCTTXT { get; set; }
        public string BUKPITXT { get; set; }
        public string PURPSTXT { get; set; }
        public string TRGTINDCD { get; set; }
        public string RQUMTTXT { get; set; }
        public short BUDGETFLG { get; set; }
        public decimal? BUDGETSYKIID { get; set; }
        public short? ADORGLEVELID { get; set; }
        public DateTime? STARTDT { get; set; }
        public DateTime? ENDDT { get; set; }
        public string FRCSTOTHER { get; set; }
        public string ATTACHMENT { get; set; }
        public short STATUSCD { get; set; }
        public short ACTIVE { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime DATEADDEDID { get; set; }
        public short LSTMODBYID { get; set; }
        public DateTime LASTMODDATE { get; set; }
        public int? OPERATION { get; set; }
        public int? DIVISION { get; set; }
        public int? DEPARTMENT { get; set; }
        public int? SECTION { get; set; }
    }

    public class A00ADORGLEVELList
    {
        public long ADORGLEVELID { get; set; }
        public string LEVELDESCRIP { get; set; }
        public Nullable<long> PARENTLEVELID { get; set; }
        public long ADORGLEVELTYPEID { get; set; }
        public Nullable<byte> SYKIID { get; set; }
        public Nullable<short> ACTIVE { get; set; }
    }

    public class A00_INVFORCASTList
    {
        public long INVFORCASTID { get; set; }
        public string INCFORCASTDETAIL { get; set; }
        public short ACTIVE { get; set; }
        public long ADDEDBY { get; set; }
    }

    public class A00INVFORCASTList
    {
        public long A00INVFORCASTID { get; set; }
        public long A00DTLTBID { get; set; }
        public short ACTIVE { get; set; }
        public long ADDEDBY { get; set; }
        public long INVFORCASTID { get; set; }
    }

    public class A00_APPROVAL
    {
        public long A00APPROVALID { get; set; }
        public long A00DTLTBID { get; set; }
        public Nullable<long> DEPTHDID { get; set; }
        public Nullable<System.DateTime> DEPTHDAPPDATE { get; set; }
        public string DEPTHDAPPTXT { get; set; }
        public Nullable<long> COORDDID { get; set; }
        public Nullable<System.DateTime> COORDAPPDATE { get; set; }
        public string COORDAPPTXT { get; set; }
        public Nullable<long> DIVHDHDID { get; set; }
        public Nullable<System.DateTime> DIVHDAPPDATE { get; set; }
        public string DIVHDAPPTXT { get; set; }
        public Nullable<long> EXECOHDID { get; set; }
        public Nullable<System.DateTime> EXECOAPPDATE { get; set; }
        public string EXECOAPPTXT { get; set; }
        public Nullable<long> OHID { get; set; }
        public Nullable<System.DateTime> OHAPPDATE { get; set; }
        public string OHAPPTXT { get; set; }

        //09-Sept-2021 change start
        public Nullable<long> PPCHOOHID { get; set; }
        public Nullable<System.DateTime> PPCHOOHDATE { get; set; }
        public string PPCHOOHTXT { get; set; }
        //09-Sept-2021 change end
    }

    public class A00ADEMPLOYEE
    {
        public long ADEMPCODE { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string EMAILID { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string GENDER { get; set; }
        public short ACTIVE { get; set; }
        public Nullable<System.DateTime> CONFIRMATIONDATE { get; set; }
        public string PANCARDNO { get; set; }
        public Nullable<System.DateTime> MARITALDATE { get; set; }
    }

    public class A00MaxA00DTLTBID
    {
        public long A00DTLTBID { get; set; }
    }

    public class A00MaxA00APPROVAL
    {
        public long A00APPROVALID { get; set; }
    }


    public class A00Allocation
    {
        public decimal A00AllocationID { get; set; }
        public long A00ID { get; set; }
        public decimal? SKYID { get; set; }
        public decimal? ApplicationPICECode { get; set; }
        public decimal? InfraPICECode { get; set; }

        public string MainPIC { get; set; }
        public decimal? MainPICTypeID { get; set; }
        public int AddedBY { get; set; }
        public decimal? AllocatedBY { get; set; }
        public DateTime? AllocatedDate { get; set; }
        public string AddedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdateDate { get; set; }

    }

    public class A00AllocationSubPIC
    {

        public long A00AllocationSubPICID { get; set; }
        public long A00AllocationID { get; set; }
        public long SubPICID { get; set; }
        public string AddedBY { get; set; }
        public string AddedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdateDate { get; set; }
    }
    public class A00Deficiency
    {
        public long A00DeficiencyID { get; set; }
        public long A00ID { get; set; }
        public string DeficiencyText { get; set; }
        public string Attachment { get; set; }
        public int AddedBY { get; set; }
        public DateTime AddedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdateDate { get; set; }
        public string Status { get; set; }
    }

    public class A00DeficiencyClosure
    {

        public long A00DeficiencyClosureID { get; set; }
        public long A00DeficiencyID { get; set; }
        public string DeficiencyClosureText { get; set; }
        public string Attachment { get; set; }
        public string AddedBY { get; set; }
        public string AddedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdateDate { get; set; }
    }


    public class A00_VW_ASSOCIATELVLDETAILS
    {
        //public long ADEMPCODE { get; set; }
        //public Nullable<long> SYKI { get; set; }
        public Nullable<long> SECTIONID { get; set; }
        public Nullable<long> DEPARTMENTID { get; set; }
        public Nullable<long> DIVISIONID { get; set; }
        public Nullable<long> OPERATIONID { get; set; }
        public long ADEMPCODE { get; set; }
        public Nullable<long> SYKI { get; set; }
        public Nullable<long> SUPERVISOREMPCODE { get; set; }
        public Nullable<long> SUPSUPERVISOREMPCODE { get; set; }
        public Nullable<long> ADDESIGNATIONID { get; set; }
        public string FUNCTIONALDESIGNATION { get; set; }
        public Nullable<long> ADFUNCTIONALDESIGNATIONID { get; set; }
        public Nullable<long> SYSITEID { get; set; }
        public Nullable<long> SYPLANTID { get; set; }
        public string ZONE { get; set; }
        public Nullable<long> SYLOCATIONID { get; set; }
        public short ACTIVE { get; set; }
        public string SECTION { get; set; }
        public string SEC_COSTCENTRE { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPT_COSTCENTRE { get; set; }
        public string DIVISION { get; set; }
        public string DIV_COSTCENTRE { get; set; }
        public string OPERATION { get; set; }
        public string OP_COSTCENTRE { get; set; }
        public Nullable<decimal> OPERATIONID_1 { get; set; }
        public string OPERATION_1 { get; set; }
        public string OP1_COSTCENTRE { get; set; }

        //Dinesh
        public string LastSYKI { get; set; }
        public decimal LastSYKIID { get; set; }
        public Nullable<long> LastDIVISIONID { get; set; }
        public Nullable<long> LastOPERATIONID { get; set; }
        public string LastDIVISION { get; set; }
        public string LastOPERATION { get; set; }
        //****
    }


    public class A00_VW_ASSOCIATELVLDETAILS1
    {
        //public long ADEMPCODE { get; set; }
        //public Nullable<long> SYKI { get; set; }
        public Nullable<long> SECTIONID { get; set; }
        public Nullable<long> DEPARTMENTID { get; set; }
        public Nullable<long> DIVISIONID { get; set; }
        public Nullable<long> OPERATIONID { get; set; }
        public long ADEMPCODE { get; set; }
        public Nullable<long> SYKI { get; set; }
        public Nullable<long> SUPERVISOREMPCODE { get; set; }
        public Nullable<long> SUPSUPERVISOREMPCODE { get; set; }
        public Nullable<long> ADDESIGNATIONID { get; set; }
        public string FUNCTIONALDESIGNATION { get; set; }
        public Nullable<long> ADFUNCTIONALDESIGNATIONID { get; set; }
        public Nullable<long> SYSITEID { get; set; }
        public Nullable<long> SYPLANTID { get; set; }
        public string ZONE { get; set; }
        public Nullable<long> SYLOCATIONID { get; set; }
        public short ACTIVE { get; set; }
        public string SECTION { get; set; }
        public string SEC_COSTCENTRE { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPT_COSTCENTRE { get; set; }
        public string DIVISION { get; set; }
        public string DIV_COSTCENTRE { get; set; }
        public string OPERATION { get; set; }
        public string OP_COSTCENTRE { get; set; }
        public Nullable<decimal> OPERATIONID_1 { get; set; }
        public string OPERATION_1 { get; set; }
        public string OP1_COSTCENTRE { get; set; }
    }

    public class A00_VW_ASSOCIATELVLDETAILS_FULL
    {
        public Nullable<long> SECTIONID { get; set; }
        public Nullable<long> DEPARTMENTID { get; set; }
        public Nullable<long> DIVISIONID { get; set; }
        public Nullable<long> OPERATIONID { get; set; }
        public long ADEMPCODE { get; set; }
        public Nullable<long> SYKI { get; set; }
        public Nullable<long> SUPERVISOREMPCODE { get; set; }
        public Nullable<long> SUPSUPERVISOREMPCODE { get; set; }
        public Nullable<long> ADDESIGNATIONID { get; set; }
        public string FUNCTIONALDESIGNATION { get; set; }
        public Nullable<long> ADFUNCTIONALDESIGNATIONID { get; set; }
        public Nullable<long> SYSITEID { get; set; }
        public Nullable<long> SYPLANTID { get; set; }
        public string ZONE { get; set; }
        public Nullable<long> SYLOCATIONID { get; set; }
        public short ACTIVE { get; set; }
        public string SECTION { get; set; }
        public string SEC_COSTCENTRE { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPT_COSTCENTRE { get; set; }
        public string DIVISION { get; set; }
        public string DIV_COSTCENTRE { get; set; }
        public string OPERATION { get; set; }
        public string OP_COSTCENTRE { get; set; }
        public Nullable<decimal> OPERATIONID_1 { get; set; }
        public string OPERATION_1 { get; set; }
        public string OP1_COSTCENTRE { get; set; }
    }

    public class A00_ADORGLEVELHEAD
    {
        public long ADORGLEVELHEADID { get; set; }
        public long ADORGLEVELID { get; set; }
        public long ADEMPCODE { get; set; }
        public short ISACTIVE { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public Nullable<System.DateTime> DATELSTMOD { get; set; }
        public Nullable<long> LSTMODBY { get; set; }
    }

    public class Single_A00ADEMPLOYEE
    {
        public string EMAILID { get; set; }
        public string Name { get; set; }
        public string EmpCode { get; set; }
    }

    public class UserRole
    {
        public bool isSecHead { get; set; }
        public bool isDeptHead { get; set; }
        public bool isCoOrdHead { get; set; }
        public bool isDivHead { get; set; }
        public bool isExeCoOrdHead { get; set; }
        public bool isOperatingHead { get; set; }
    }

    public class A00_ADORGCOORDINATOR
    {
        public long ADORGCOORDINATORID { get; set; }
        public long ADORGLEVELID { get; set; }
        public Nullable<long> OPHEAD { get; set; }
        public Nullable<long> COORDINATOR { get; set; }
        public Nullable<long> EXECOORDINATOR { get; set; }
        public Nullable<long> DIRECTOR { get; set; }
        public short ISACTIVE { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
    }

    public class InsertA00DtlTb
    {
        public long A00DTLTBID { get; set; }
        public short ACTIVE { get; set; }
        public long ADDEDBY { get; set; }
        public short? ADORGLEVELID { get; set; }
        public string ATTACHMENT { get; set; }
        public short BUDGETFLG { get; set; }
        public decimal? BUDGETSYKIID { get; set; }
        public string BUKPITXT { get; set; }
        public DateTime DATEADDEDID { get; set; }
        public int? DEPARTMENT { get; set; }
        public int? DIVISION { get; set; }
        public DateTime? ENDDT { get; set; }
        public string FRCSTOTHER { get; set; }
        public DateTime? LASTMODDATE { get; set; }
        public long? LSTMODBYID { get; set; }
        public int? OPERATION { get; set; }

        //[Display(Name = "Project title")]
        //[Required(ErrorMessage = "Please enter Project title")]
        public string PRJCTTLE { get; set; }
        public string PRJCTTXT { get; set; }
        public string PURPSTXT { get; set; }
        public string RQUMTTXT { get; set; }
        public int? SECTION { get; set; }
        public DateTime? STARTDT { get; set; }
        public short STATUSCD { get; set; }
        public decimal? SYKIID { get; set; }
        public string TRGTINDCD { get; set; }
    }

    public class InsertA00APPROVAL
    {
        public long A00APPROVALID { get; set; }
        public long A00DTLTBID { get; set; }
        public DateTime? COORDAPPDATE { get; set; }
        public string COORDAPPTXT { get; set; }
        public long? COORDDID { get; set; }
        public DateTime? DEPTHDAPPDATE { get; set; }
        public string DEPTHDAPPTXT { get; set; }
        public long? DEPTHDID { get; set; }
        public DateTime? DIVHDAPPDATE { get; set; }
        public string DIVHDAPPTXT { get; set; }
        public long? DIVHDHDID { get; set; }
        public DateTime? EXECOAPPDATE { get; set; }
        public string EXECOAPPTXT { get; set; }
        public long? EXECOHDID { get; set; }
        public DateTime? OHAPPDATE { get; set; }
        public string OHAPPTXT { get; set; }
        public long? OHID { get; set; }

        //09-Sept-2021 change start
        public Nullable<long> PPCHOOHID { get; set; }
        public Nullable<System.DateTime> PPCHOOHDATE { get; set; }
        public string PPCHOOHTXT { get; set; }
        //09-Sept-2021 change end
    }

    public class InsertA00INVFORCAST
    {
        public long A00DTLTBID { get; set; }
        public long A00INVFORCASTID { get; set; }
        public short ACTIVE { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime DATEADDEDID { get; set; }
        public long INVFORCASTID { get; set; }
        public DateTime? LASTMODDATE { get; set; }
        public long? LSTMODBYID { get; set; }
    }
    public class ApprovalViewHistoryModel : A00APPROVAL
    {
        public string DeptHeadName { get; set; }
        public string CoOrdName { get; set; }
        public string DivHeadName { get; set; }
        public string ExeCoOrdName { get; set; }
        public string OpHeadName { get; set; }

        //09-Sept-2021 change start
        public string PPCHOOHName { get; set; }
        //09-Sept-2021 change end
    }

    public class SaveA00DeficiencyVM : A00APPROVAL
    {
        public string DeficiencyRemark { get; set; }
        public IFormFile File { get; set; }
    }

    public class GetEmployeeA00AllocationVM
    {
        public string Employee { get; set; }
        public long Id { get; set; }
    }

    public class GetDeficiencyRaisedVM
    {
        public string File { get; set; }
        public string Remarks { get; set; }
        public string ActionBy { get; set; }
        public string ActionOn { get; set; }
        public long ActionFor { get; set; }
        public int EmpActionBy { get; set; }
    }

    public class GetAllocationVM
    {
        public string ApplicationPic { get; set; }
        public string InfrastructurePic { get; set; }
        public string MainPic { get; set; }
        public string OtherMember { get; set; }
        public string AllocatedBy { get; set; }
        public string AllocationOn { get; set; }
    }

    public class A00AllocationListVM
    {
        public long A00DTLTBID { get; set; }
    }

    public class ITConfirmationVM
    {
        public long A00DTLTBID { get; set; }
        public decimal? ApplicationPIC { get; set; }
        public decimal? InfraStructurePIC { get; set; }
        public decimal SYKIID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectDate { get; set; }
        public string ProjectCode_Date { get; set; }
        public string ProjectName { get; set; }
        [Required]
        public decimal? ImpactedOperation { get; set; }
        public decimal? UserPL { get; set; }
        public string? UserDept { get; set; }
        public string UserPL_Name { get; set; }
        public decimal? ITPL { get; set; }
        public string? ITDept { get; set; }
        public string ITPL_Name { get; set; }
        [Required]
        public decimal? Development { get; set; }
        [Required]
        public string SAA { get; set; }
        [Required]
        public string LWEA { get; set; }
        [Required]
        public decimal? LicenseRequired { get; set; }
        [Required]
        public decimal? InfrastructureRequirement { get; set; }

        public string IR { get; set; }
        [Required]
        public string OTC { get; set; }
        [Required]
        public string RC { get; set; }

        public string OEM { get; set; }
        [Required]
        public string TTimeLine { get; set; }
        [Required]
        public decimal? Application { get; set; }
        [Required]
        public decimal? NOFS { get; set; }
        [Required]
        public string HSDM { get; set; }
        [Required]
        public string UOTarget { get; set; }
        [Required]
        public string Remarks { get; set; }
        public short? Status { get; set; }
        public int UserType { get; set; }
        public Nullable<short> ITCONFSUBMITTEDTOUSERTYPE { get; set; }
        public long? ITCONFIRMATIONSUBMITTEDTO { get; set; }
        public Nullable<short> ITCONFSUBMITTEDTOSTATUS { get; set; }

        public decimal ITCONFIRMATIONID { get; set; }
        public string ApprovalRemarks { get; set; }
        public decimal? A00ALLOCATIONID { get; set; }
        public short? MAINPIC_ID { get; set; }

        public decimal? ITConfirmationFilledBy { get; set; }

        public DateTime? ITCONFIRMATIONApprovalDate { get; set; }

        public bool IsOHLoggedIn { get; set; }

        public ITConfirmationApprovalHistoryVM ITConfirmationLatestHistoryVM { get; set; }

        //22-Sept-2021 change start
        public IFormFile? ImageFile { get; set; }
        public string ImagePath { get; set; }
        public string Attachment_Name { get; set; }
        //22-Sept-2021 change end
    }

    public class ProjectVm
    {
        public string ProjectCode { get; set; }
        public DateTime ProjectDate { get; set; }
        public string ProjectName { get; set; }
        public long AddedBy { get; set; }
    }

    public class A00RaisedDetails
    {
        public string EmpCode { get; set; }
        public string Name { get; set; }
    }

    public class ITConfirmationApprovalHistoryVM
    {
        public long? A00ItConfirmationID { get; set; }
        public long? A00ID { get; set; }
        public decimal? SYKIID { get; set; }
        public short? ActionType { get; set; }
        public short? UserType { get; set; }
        public decimal? ActionBy { get; set; }
        public string ActionByName { get; set; }
        public string Designation { get; set; }
        public DateTime? ActionOn { get; set; }
        public string Remarks { get; set; }
        public string PICType { get; set; }
        public decimal? ITConfirmationSubmittedTo { get; set; }
        public decimal? ITCONFSUBMITTEDTOUSERTYPE { get; set; }
        public short? Status { get; set; }
        public Nullable<short> ITCONFSUBMITTEDTOSTATUS { get; set; }
    }

    public partial class ITConfirmationApprovalHistoryListVM
    {
        public decimal HistoryID { get; set; }
        public Nullable<decimal> A00ITCONFIRMATIONID { get; set; }
        public Nullable<long> A00ID { get; set; }
        public Nullable<decimal> SYKIID { get; set; }
        public Nullable<short> ACTIONTYPE { get; set; }
        public Nullable<short> STATUS { get; set; }
        public string REMARKS { get; set; }
        public string EmpName { get; set; }
        public Nullable<decimal> ACTIONBY { get; set; }
        public Nullable<System.DateTime> ACTIONON { get; set; }
        public Nullable<short> ACTIVE { get; set; }
        public Nullable<short> USERTYPE { get; set; }
        public Nullable<decimal> ITCONFIRMATIONSUBMITTEDTO { get; set; }
        public Nullable<short> ITCONFSUBMITTEDTOUSERTYPE { get; set; }
        public Nullable<short> ITCONFSUBMITTEDTOSTATUS { get; set; }
        public string Designation { get; set; }
        public string PICType { get; set; }
    }
    public class A00AcitivtyVM
    {
        public decimal ActivityID { get; set; }
        public string ActivityName { get; set; }
        public string ActivityDetails { get; set; }
        public DateTime? StartDate { get; set; }
        public string ActivityStartDate { get; set; }
        public string ActivityEndDate { get; set; }
        //DInesh
        public decimal? ActionBy { get; set; }
        public Nullable<int> Status { get; set; }
        public short? ActivitySchedule { get; set; }

        public long? SYKID { get; set; }
        public int? A00DTLTBID { get; set; }
        public int? DEPARTMENTID { get; set; }

        public int? OPERATIONID { get; set; }
        public int? DIVISIONID { get; set; }
        public int? SECTIONID { get; set; }

        public string Remark { get; set; }
        //***
        public string EmpName { get; set; }
        public DateTime? ActivityRemarkDate { get; set; }

    }
    public class A00AcitivtyHistoryVM
    {
        public decimal? A00ActivityHistoryID { get; set; }
        public long A00ActivityID { get; set; }
        public string Remarks { get; set; }
        public int ActivitySchedule { get; set; }

        //Dinesh 26-04
        public DateTime? ActionOn { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }

    }
    public class A00ProjectStateUpdate
    {
        public decimal ProjectStatusUpdateID { get; set; }
        public long A00DTLTBID { get; set; }
        public decimal SYKIID { get; set; }
        public string PROJECTSTAGE { get; set; }
        public string ITCONFAPPROVEDON { get; set; }
        public string PROJECTSTATUSREMARKS { get; set; }
        public Nullable<short> PROJECTSTATUS { get; set; }
        public string ATTACHMENTNOTE { get; set; }
        public DateTime? ActionOn { get; set; }
        public string EmpName { get; set; }
    }
    public class A00ActivityDataforExcel
    {
        public string KI_Code { get; set; }
        public long? E_Code { get; set; }
        public string Name { get; set; }
        public string Activity_Title { get; set; }
        public string Start_Date { get; set; }
        public string End_Date { get; set; }
        public short? Status { get; set; }
        public string Remark { get; set; }
        public short STATUSCD { get; set; }
        public string ActivityDetail { get; set; }
        public DateTime? ActivityRemarkDate { get; set; }
    }
    public class ApprovedA00ListforExcel
    {
        public string KI_Code { get; set; }
        public string Operation { get; set; }
        public long? E_Code { get; set; }
        public string Name { get; set; }
        public string ProjectTitle { get; set; }
        public DateTime? Request_Date { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? Approved_Date { get; set; }
        public short? IsDeficiencyExist { get; set; }
        public bool Allocation { get; set; }
        public short? IsIT_ConfirmationExist { get; set; }
        public string Main_PIC_Name { get; set; }
        //Change start on 22-July-2021
        public short STATUSCD { get; set; }
        //Change end on 22-July-2021
    }

    //Change start on 22-July-2021
    public class A00ConvertToCRModel
    {
        public long A00DTLTBID { get; set; }
        public string CHANGEBY { get; set; }
        public DateTime? CHANGEON { get; set; }
        public string CONVERTTOCRREMARKS { get; set; }
    }
    //Change end on 22-July-2021
    public class A00REMARKSVM
    {

        public long A00REMARKSID { get; set; }
        public long A00DTLID { get; set; }
        public string REMARKS { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime? ADDEDDATE { get; set; }
        public Nullable<long> LSTMODBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string stringAddedDate { get; set; }


    }
}
