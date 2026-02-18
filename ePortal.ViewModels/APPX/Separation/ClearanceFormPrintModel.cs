using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ClearanceFormPrintModel
    {
      public ShowClearanceFormPrintViewModel ShowClearanceFormPrintViewModel { get; set; }
      public HRSPAcceptanceDetailViewModel HRSPAcceptanceDetailViewModel { get; set; }
        //-------SIS-------------
        public SISViewModel SISViewModel { get; set; }
        public SISClearanceViewModel SISClearanceViewModel { get; set; } 
        public SISAcBlockSAPViewModel SISAcBlockSAPViewModel { get; set; }
        public SISAppPcpackViewModel SISAppPcpackViewModel { get; set; } 
        public SISNetworkingViewModel SISNetworkingViewModel { get; set; } 
        public SISUserSupportViewModel SISUserSupportViewModel { get; set; } 
        //-------SecurityViewModel-------------
        public ANSRViewModel ANSRViewModel { get; set; }
        public ANSRProductLoanViewModel ANSRProductLoanViewModel { get; set; } 
        public PersonalLoanViewModel PersonalLoanViewModel { get; set; } 
        public NPSViewModel NPSViewModel { get; set; } 
        public GratuityViewModel GratuityViewModel { get; set; } 
        public HomeLoanIntSubsidyViewModel HomeLoanIntSubsidyViewModel { get; set; } 
        public LTAViewModel LTAViewModel { get; set; }
        public LTARecUABSViewModel LTARecUABSViewModel { get; set; } 
        public EarnLeaveViewModel EarnLeaveViewModel { get; set; } 
        public CasualLeaveViewModel CasualLeaveViewModel { get; set; }
        public SickLeaveViewModel SickLeaveViewModel { get; set; } 
        public COffViewModel COffViewModel { get; set; } 
        public AttndIdCardViewModel AttndIdCardViewModel { get; set; } 
        public AnyOtherViewModel AnyOtherViewModel { get; set; } 
        //-------SecurityViewModel-------------
        public LaptoAuthViewModel LaptoAuthViewModel { get; set; } 
        public CarParkStickerViewModel CarParkStickerViewModel { get; set; } 
        public SecurityViewModel SecurityViewModel { get; set; }
        //-------Administration-------------
        public AdministrationViewModel AdministrationViewModel { get; set; }
        public TeaSnackDedViewModel TeaSnackDedViewModel { get; set; } = new TeaSnackDedViewModel();
        public CanteenViewModel CanteenViewModel { get; set; } = new CanteenViewModel();
        public TelephoneViewModel TelephoneViewModel { get; set; } = new TelephoneViewModel();
        public TravelBookingViewModel TravelBookingViewModel { get; set; } = new TravelBookingViewModel();
        public UniformViewModel UniformViewModel { get; set; } = new UniformViewModel();
        public TransportDedViewModel TransportDedViewModel { get; set; } = new TransportDedViewModel();
        public CarPatrolViewModel CarPatrolViewModel { get; set; } = new CarPatrolViewModel();
       
        //-------Medical-------------
        public MedicalViewModel MedicalViewModel { get; set; }
        public MedInsuPremViewModel MedInsuPremViewModel { get; set; }
        public DelNmMedInsuPolcyViewModel DelNmMedInsuPolcyViewModel { get; set; } 
        //-------SectryLeglViewModel-------------
        public SectryLeglViewModel SectryLeglViewModel { get; set; }
        public BondFTraningViewModel BondFTraningViewModel { get; set; } 
        public BondFTrainingViewModel BondFTrainingViewModel { get; set; } 
        public GratuitySecLegViewModel GratuitySecLegViewModel { get; set; } 
        public SuperannuationViewModel SuperannuationViewModel { get; set; }
        //-------------HR----------------
        public HRViewModel HRViewModel { get; set; }
        public ResForSeprViewModel ResForSeprViewModel { get; set; }
        public ExitIntvViewModel ExitIntvViewModel { get; set; } 
        public AnyOthViewModel AnyOthViewModel { get; set; } 
        public RelocationDotViewModel RelocationDotViewModel { get; set; }
        public RelocationViewModel RelocationViewModel { get; set; }
        public NoticeViewModel NoticeViewModel { get; set; }
        public NoticeAppointLtrViewModel NoticeAppointLtrViewModel { get; set; } 
        public NoticePayDayViewModel NoticePayDayViewModel { get; set; } 

        //----------Account--------------
        public AccountViewModel AccountViewModel { get; set; }
      public ProductLoanViewModel ProductLoanViewModel { get; set; }
      public MedReiumbViewModel MedReiumbViewModel { get; set; } 
      public MedicalLoanViewModel MedicalLoanViewModel { get; set; }
      public LTAccountViewModel LTAccountViewModel { get; set; }
      public HouseRentViewModel HouseRentViewModel { get; set; }
      public HomeLoanIntViewModel HomeLoanIntViewModel { get; set; }
      public OthDed80ViewModel OthDed80ViewModel { get; set; }
      public SuspenseViewModel SuspenseViewModel { get; set; } 

    }
    public class ShowClearanceFormPrintViewModel
    {
        public bool lta { get; set; }
        public bool housrent { get; set; }
        public bool othrdedct { get; set; }
        public bool mdcllon { get; set; }
        public bool medreimb { get; set; }
        public bool Tr1 { get; set; }
        public bool Tr2 { get; set; }
        public bool Tr3 { get; set; }
        public bool Tr4 { get; set; }
        public bool Tr5 { get; set; }
        public bool Tr6 { get; set; }
        public bool Tr7 { get; set; }
        public bool Tr8 { get; set; }
        public bool Tr9 { get; set; }
        public bool Tr10 { get; set; }
        public bool Tr11 { get; set; }
        public bool Tr12 { get; set; }
        public bool Tr13 { get; set; }
        public bool Tr14 { get; set; }
        public bool Tr15 { get; set; }
        public bool Tr16 { get; set; }
        public bool Tr17 { get; set; }
        public bool Tr18 { get; set; }
        public bool Tr19 { get; set; }
        public bool Tr20 { get; set; }
        public bool Tr21 { get; set; }
        public bool Tr22 { get; set; }
        public bool Tr23 { get; set; }
        public bool Tr24 { get; set; }
        public bool Tr25 { get; set; }
        public bool Tr26 { get; set; }
        public bool Tr27 { get; set; }
        public bool Tr28 { get; set; }
        public bool Tr29 { get; set; }
        public bool Tr30 { get; set; }
        public bool Tr31 { get; set; }
        public bool Tr32 { get; set; }
        public bool Tr33 { get; set; }
        public bool Tr34 { get; set; }
        public bool Tr35 { get; set; }
        public bool Tr36 { get; set; }
        public bool Tr38 { get; set; }
        public bool Tr39 { get; set; }
        public bool Tr40 { get; set; }

        public bool TR50 { get; set; }
        public bool TR51 { get; set; }
        public bool TR52 { get; set; }
        public bool TR53 { get; set; }
        public bool TR54 { get; set; }
        public bool TR55 { get; set; }
        public bool Tr56 { get; set; }
        public bool TR57 { get; set; }
        public bool TR58 { get; set; }
        public bool TR59 { get; set; }
        public bool TR60 { get; set; }
        public bool TR61 { get; set; }
        public bool TR62 { get; set; }
        public bool TR70 { get; set; }
        public bool TR71 { get; set; }
        public bool TR72 { get; set; }
        public bool TR73 { get; set; }
        public bool TR74 { get; set; }
        public bool TR75 { get; set; }
        public bool TR76 { get; set; }
        public bool homlon { get; set; }
        public bool susptrvl { get; set; }
        public bool prdtlon { get; set; }
        public bool TRSecurity { get; set; }
        public bool TR77 { get; set; }
        public bool TR78 { get; set; }
        public bool TR79 { get; set; }
        public bool TR80 { get; set; }
        public bool TR81 { get; set; }
        public bool TR82 { get; set; }
        public bool TR91 { get; set; }
        public bool TR92 { get; set; }
        public bool TR93 { get; set; }
        public bool TR94 { get; set; }
       
        public bool TR83 { get; set; }
        public bool TR84 { get; set; }
        public bool TR85 { get; set; }
        public bool TR86 { get; set; }
        public bool TR87 { get; set; }
        public bool TR88 { get; set; }
        public bool TR89 { get; set; }     
        public bool TR67 { get; set; }
        public bool TR68 { get; set; }
        public bool TR69 { get; set; }     
        public bool TRHR { get; set; }
        public bool TRASR { get; set; }
        public bool TRLEGL { get; set; }
        public bool TRADMIN { get; set; }
        public bool TRMEDICAL { get; set; }
        public bool trsis { get; set; }
        public bool traccount { get; set; }
        public bool Amounttab { get; set; }

        public bool TR41 { get; set; }
        public bool TR42 { get; set; }
        public bool TR43 { get; set; }
        public bool TR44 { get; set; }
        public bool TR45 { get; set; }
        public bool TR46 { get; set; }
        public bool TR63 { get; set; }
        public bool TR64 { get; set; }
        public bool TR65 { get; set; }
        public bool TR66 { get; set; }


    }
    public class HRSPAcceptanceDetailViewModel
    {
        public string lblEmpName { get; set; }
        public string lblsignature { get; set; }
        public string lblEmpcode { get; set; }
        public string lblDesignation { get; set; }
        public string lblOperation { get; set; }
        public string lblresigndt { get; set; }
        public string lblrelevdt { get; set; }
        public string lbldate { get; set; }
        public string lbljoindt { get; set; }
        public string lblsalry { get; set; }
        public string lblremark { get; set; }
        public string lblsubmitby { get; set; }
        public string lblsubmitdt { get; set; }

        public string Label4 { get; set; }
        public string Label8 { get; set; }
        public string Label12 { get; set; }
        public string Label16 { get; set; }
        public string Label20 { get; set; }
        public string Label347 { get; set; }
    }

    public class SISViewModel
    {
        public string Label29 { get; set; }  
        public string Label30 { get; set; } 
        public string Label28 { get; set; }
       

    }
    public class SISClearanceViewModel
    {
        public string Label211 { get; set; }
        public string Label23 { get; set; }
        public string Label24 { get; set; }
        public string Label25 { get; set; }
        public string Label26 { get; set; }
        public string Label212 { get; set; }
    }
    public class SISAcBlockSAPViewModel
    {
        public string Label335 { get; set; }
        public string Label317 { get; set; }
        public string Label318 { get; set; }
        public string Label319 { get; set; }
        public string Label320 { get; set; }
        public string Label336 { get; set; }
    }
    public class SISAppPcpackViewModel
    {
        public string Label337 { get; set; }
        public string Label321 { get; set; }
        public string Label322 { get; set; }
        public string Label323 { get; set; }
        public string Label324 { get; set; }
        public string Label338 { get; set; }
    }
    public class SISNetworkingViewModel
    {
        public string Label333 { get; set; }
        public string Label325 { get; set; }
        public string Label326 { get; set; }
        public string Label327 { get; set; }
        public string Label328 { get; set; }
        public string Label334 { get; set; }
    }
    public class SISUserSupportViewModel
    {
        public string Label339 { get; set; }
        public string Label329 { get; set; }
        public string Label330 { get; set; }
        public string Label331 { get; set; }
        public string Label340 { get; set; }
    }

    public class ANSRViewModel
    {
        public string Label129 { get; set; }
        public string Label130 { get; set; }
        public string Label128 { get; set; }
       
    }
    public class ANSRProductLoanViewModel
    {
        public string Label237 { get; set; }
        public string Label99 { get; set; }
        public string Label100 { get; set; }
        public string Label101 { get; set; }
        public string Label102 { get; set; }
        public string Label238 { get; set; }

    }
    public class PersonalLoanViewModel
    {
        public string Label239 { get; set; }
        public string Label103 { get; set; }
        public string Label104 { get; set; }
        public string Label105 { get; set; }
        public string Label106 { get; set; }
        public string Label240 { get; set; }
    }
    public class NPSViewModel
    {
        public string Label241 { get; set; }
        public string Label107 { get; set; }
        public string Label108 { get; set; }
        public string Label109 { get; set; }
        public string Label110 { get; set; }
        public string Label242 { get; set; }
    }
    public class GratuityViewModel
    {
        public string Label111 { get; set; }
        public string Label243 { get; set; }
        public string Label112 { get; set; }
        public string Label113 { get; set; }
        public string Label114 { get; set; }
        public string Label216 { get; set; }
    }
    public class HomeLoanIntSubsidyViewModel
    {
        public string Label115 { get; set; }
        public string Label245 { get; set; }
        public string Label116 { get; set; }
        public string Label117 { get; set; }
        public string Label118 { get; set; }
        public string Label246 { get; set; }
    }
    public class LTAViewModel
    {
        public string Label119 { get; set; }
        public string Label259 { get; set; }
        public string Label120 { get; set; }
        public string Label121 { get; set; }
        public string Label122 { get; set; }
        public string Label260 { get; set; }
    }
    public class LTARecUABSViewModel
    {
        public string Label123 { get; set; }
        public string Label261 { get; set; }
        public string Label124 { get; set; }
        public string Label125 { get; set; }
        public string Label126 { get; set; }
        public string Label262 { get; set; }
    }
    public class EarnLeaveViewModel
    {
        public string Label131 { get; set; }
        public string Label247 { get; set; }
        public string Label132 { get; set; }
        public string Label133 { get; set; }
        public string Label134 { get; set; }
        public string Label248 { get; set; }
    }
    public class CasualLeaveViewModel
    {
        public string Label135 { get; set; }
        public string Label249 { get; set; }
        public string Label136 { get; set; }
        public string Label137 { get; set; }
        public string Label138 { get; set; }
        public string Label250 { get; set; }
    }
    public class SickLeaveViewModel
    {
        public string Label139 { get; set; }
        public string Label251 { get; set; }
        public string Label140 { get; set; }
        public string Label141 { get; set; }
        public string Label142 { get; set; }
        public string Label252 { get; set; }
    }
    public class COffViewModel
    {
        public string Label143 { get; set; }
        public string Label253 { get; set; }
        public string Label144 { get; set; }
        public string Label145 { get; set; }
        public string Label146 { get; set; }
        public string Label254 { get; set; }
    }
    public class AttndIdCardViewModel
    {
        public string Label147 { get; set; }
        public string Label255 { get; set; }
        public string Label148 { get; set; }
        public string Label149 { get; set; }
        public string Label150 { get; set; }
        public string Label256 { get; set; }
    }
    public class AnyOtherViewModel
    {
        public string Label151 { get; set; }
        public string Label257 { get; set; }
        public string Label152 { get; set; }
        public string Label153 { get; set; }
        public string Label154 { get; set; }
        public string Label258 { get; set; }
    }

    public class AdministrationViewModel
    {
        public string Label185 { get; set; }
        public string Label186 { get; set; }
        public string Label184 { get; set; }
        

    }
    public class TeaSnackDedViewModel
    {
        public string Label265 { get; set; }
        public string Label163 { get; set; }
        public string Label164 { get; set; }
        public string Label165 { get; set; }
        public string Label166 { get; set; }
        public string Label266 { get; set; }
    }
    public class CanteenViewModel
    {
        public string Label267 { get; set; }
        public string Label167 { get; set; }
        public string Label168 { get; set; }
        public string Label169 { get; set; }
        public string Label170 { get; set; }
        public string Label268 { get; set; }
    }
    public class TelephoneViewModel
    {
        public string Label155 { get; set; } 
        public string Label269 { get; set; }
        public string Label156 { get; set; }
        public string Label157 { get; set; }
        public string Label158 { get; set; }
        public string Label270 { get; set; }
    }
    public class TravelBookingViewModel
    {
        public string Label159 { get; set; }
        public string Label271 { get; set; }
        public string Label160 { get; set; }
        public string Label161 { get; set; }
        public string Label162 { get; set; }
        public string Label272 { get; set; }
    }
    public class UniformViewModel
    {
        public string Label171 { get; set; }
        public string Label273 { get; set; }
        public string Label172 { get; set; }
        public string Label173 { get; set; }
        public string Label174 { get; set; }
        public string Label274 { get; set; }
    }
    public class TransportDedViewModel
    {
        public string Label175 { get; set; }
        public string Label275 { get; set; }
        public string Label176 { get; set; }
        public string Label177 { get; set; }
        public string Label178 { get; set; }
        public string Label276 { get; set; }
    }
    public class CarPatrolViewModel
    {
        public string Label179 { get; set; }
        public string Label277 { get; set; }
        public string Label180 { get; set; }
        public string Label181 { get; set; }
        public string Label182 { get; set; }
        public string Label278 { get; set; }
    }

    public class SecurityViewModel
    {
        public string Label193 { get; set; }
        public string Label194 { get; set; }
        public string Label192 { get; set; }
       
    }
    public class LaptoAuthViewModel
    {
        public string Label188 { get; set; }
        public string Label189 { get; set; }
        public string Label190 { get; set; }
        public string Label187 { get; set; }
        public string Label279 { get; set; }
        public string Label280 { get; set; }
    }
    public class CarParkStickerViewModel
    {
        public string Label195 { get; set; }
        public string Label281 { get; set; }
        public string Label196 { get; set; }
        public string Label197 { get; set; }
        public string Label198 { get; set; }
        public string Label282 { get; set; }
    }


    public class MedicalViewModel
    {
        public string Label209 { get; set; }
        public string Label210 { get; set; }
        public string Label208 { get; set; }
        
    }
    public class MedInsuPremViewModel
    {
        public string Label199 { get; set; }
        public string Label283 { get; set; }
        public string Label200 { get; set; }
        public string Label201 { get; set; }
        public string Label202 { get; set; }
        public string Label280 { get; set; }
    }
    public class DelNmMedInsuPolcyViewModel
    {
        public string Label203 { get; set; }
        public string Label285 { get; set; }
        public string Label204 { get; set; }
        public string Label205 { get; set; }
        public string Label206 { get; set; }
        public string Label282 { get; set; }
    }


    public class SectryLeglViewModel
    {
        public string Label97 { get; set; }
        public string Label98 { get; set; }
        public string Label96 { get; set; }
       
    }
    public class BondFTraningViewModel
    {
        public string Label345 { get; set; }
        public string Label341 { get; set; }
        public string Label342 { get; set; }
        public string Label343 { get; set; }
        public string Label344 { get; set; }
        public string Label346 { get; set; }
    }
    public class BondFTrainingViewModel
    {
        public string Label91 { get; set; }
        public string Label263 { get; set; }
        public string Label92 { get; set; }
        public string Label93 { get; set; }
        public string Label94 { get; set; }
        public string Label264 { get; set; }
    }
    public class GratuitySecLegViewModel
    {
        public string Label313 { get; set; }
        public string Label305 { get; set; }
        public string Label306 { get; set; }
        public string Label307 { get; set; }
        public string Label308 { get; set; }
        public string Label314 { get; set; }
    }
    public class SuperannuationViewModel
    {
        public string Label315 { get; set; }
        public string Label309 { get; set; }
        public string Label310 { get; set; }
        public string Label311 { get; set; }
        public string Label312 { get; set; }
        public string Label316 { get; set; }
    }


    public class HRViewModel
    {
        public string Label57 { get; set; }
        public string Label58 { get; set; }
        public string Label56 { get; set; }
        

    }
    public class ResForSeprViewModel
    {
        public string Label219 { get; set; }
        public string Label35 { get; set; }
        public string Label36 { get; set; }
        public string Label37 { get; set; }
        public string Label38 { get; set; }
        public string Label220 { get; set; }

    }
    public class ExitIntvViewModel
    {
        public string Label223 { get; set; }
        public string Label39 { get; set; }
        public string Label40 { get; set; }
        public string Label41 { get; set; }
        public string Label42 { get; set; }
        public string Label224 { get; set; }
    }
    public class AnyOthViewModel
    {
        public string Label43 { get; set; }
        public string Label227 { get; set; }
        public string Label44 { get; set; }
        public string Label45 { get; set; }
        public string Label46 { get; set; }
        public string Label228 { get; set; }
    }
    public class RelocationDotViewModel
    {
        public string Label31 { get; set; }
        public string Label215 { get; set; }
        public string Label32 { get; set; }
        public string Label33 { get; set; }
        public string Label34 { get; set; }
        public string Label216 { get; set; }
    }
    public class RelocationViewModel
    {
        public string Label303 { get; set; }
        public string Label299 { get; set; }
        public string Label300 { get; set; }
        public string Label301 { get; set; }
        public string Label302 { get; set; }
        public string Label304 { get; set; }
    }
    public class NoticeViewModel
    {
        public string Label47 { get; set; }
        public string Label231 { get; set; }
        public string Label48 { get; set; }
        public string Label49 { get; set; }
        public string Label50 { get; set; }
        public string Label232 { get; set; }
    }
    public class NoticeAppointLtrViewModel
    {
        public string Label297 { get; set; }
        public string Label293 { get; set; }
        public string Label294 { get; set; }
        public string Label295 { get; set; }
        public string Label296 { get; set; }
        public string Label298 { get; set; }
    }
    public class NoticePayDayViewModel
    {
        public string Label51 { get; set; }
        public string Label235 { get; set; }
        public string Label52 { get; set; }
        public string Label53 { get; set; }
        public string Label54 { get; set; }
        public string Label236 { get; set; }
    }



    public class AccountViewModel
    {
        public string Label85 { get; set; }
        public string Label86 { get; set; }
        public string Label84 { get; set; }
        

    }
    public class MedReiumbViewModel
    {
        public string Label291 { get; set; }
        public string Label287 { get; set; }
        public string Label288 { get; set; }
        public string Label289 { get; set; }
        public string Label290 { get; set; }
        public string Label292 { get; set; }

    }
    public class ProductLoanViewModel
    {
        public string Label1 { get; set; }
        public string Label59 { get; set; }
        public string Label60 { get; set; }
        public string Label61 { get; set; }
        public string Label62 { get; set; }
        public string Label2 { get; set; }
    }
    public class MedicalLoanViewModel
    {
        public string Label63 { get; set; }
        public string Label213 { get; set; }
        public string Label64 { get; set; }
        public string Label65 { get; set; }
        public string Label66 { get; set; }
        public string Label214 { get; set; }
    }
    public class LTAccountViewModel
    {
        public string Label67 { get; set; }
        public string Label217 { get; set; }
        public string Label68 { get; set; }
        public string Label69 { get; set; }
        public string Label70 { get; set; }
        public string Label218 { get; set; }
    }
    public class HouseRentViewModel
    {
        public string Label71 { get; set; }
        public string Label221 { get; set; }
        public string Label72 { get; set; }
        public string Label73 { get; set; }
        public string Label74 { get; set; }
        public string Label222 { get; set; }
    }
    public class HomeLoanIntViewModel
    {
        public string Label75 { get; set; }
        public string Label225 { get; set; }
        public string Label76 { get; set; }
        public string Label77 { get; set; }
        public string Label78 { get; set; }
        public string Label226 { get; set; }
    }
    public class OthDed80ViewModel
    {
        public string Label79 { get; set; }
        public string Label229 { get; set; }
        public string Label80 { get; set; }
        public string Label81 { get; set; }
        public string Label82 { get; set; }
        public string Label230 { get; set; }
    }
    public class SuspenseViewModel
    {
        public string Label87 { get; set; }
        public string Label233 { get; set; }
        public string Label88 { get; set; }
        public string Label89 { get; set; }
        public string Label90 { get; set; }
        public string Label234 { get; set; }
    }

}
