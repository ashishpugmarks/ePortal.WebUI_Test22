using System.Data;
using System.Xml;
using ePortal.DomainClasses;
using ePortal.Web.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Infrastructure.DbContexts
{
    public class EPortalDBContext: DbContext
    {
        public EPortalDBContext(DbContextOptions<EPortalDBContext> options):base(options) { }

        //public virtual DbSet<ADEMPDIVDEPTSECT> ADEMPDIVDEPTSECT { get; set; }
        //public virtual DbSet<ADEMPLOYEE> ADEMPLOYEE { get; set; }
        public DbSet<JwtUserLog> JWTUSERLOGS { get; set; }
        public DbSet<RefreshToken> REFRESHTOKENS { get; set; }
        public virtual DbSet<CM_PRESIDENT_MST> CM_PRESIDENT_MST { get; set; }
        //public virtual DbSet<D_TMPL_HEADER> D_TMPL_HEADER { get; set; }
        //public virtual DbSet<D_TRAN> D_TRAN { get; set; }
        //public virtual DbSet<CM_PROCESS_MST> CM_PROCESS_MST { get; set; }
        //public virtual DbSet<ADEMPLOGINEXPIRYDETAILS> ADEMPLOGINEXPIRYDETAILS { get; set; }
        public virtual DbSet<ADFUNCTIONALDESIGNATION> ADFUNCTIONALDESIGNATION { get; set; }
        //public virtual DbSet<ADEMPLOGIN> ADEMPLOGIN { get; set; }
        //public virtual DbSet<ADLOGINUSER> ADLOGINUSER { get; set; }
        //public virtual DbSet<VW_ASSOCIATELVLDETAILS> VW_ASSOCIATELVLDETAILS { get; set; }
        //public virtual DbSet<ADLOGINPASSHISTORY> ADLOGINPASSHISTORY { get; set; }

        //public virtual DbSet<SYEMPLOYEETYPE> SYEMPLOYEETYPE { get; set; }
        //public virtual DbSet<SYLOCATION> SYLOCATION { get; set; }
        //public virtual DbSet<SYPARAMETERS> SYPARAMETERS { get; set; }
        //public virtual DbSet<SYUSERTYPE> SYUSERTYPE { get; set; }
        //public virtual DbSet<ADDESIGNATION> ADDESIGNATION { get; set; }
        public virtual DbSet<CM_PRESIDENTMSG_TRN> CM_PRESIDENTMSG_TRN { get; set; }
        public virtual DbSet<SYSITE> SYSITE { get; set; }
        public virtual DbSet<ADEXT_PORTAL> ADEXT_PORTAL { get; set; }
        public virtual DbSet<NEWSLETTERS> NEWSLETTERS { get; set; }
        public virtual DbSet<ADUAR_TRANS> ADUAR_TRANS { get; set; }
        public virtual DbSet<CM_PROCESSSCREENSAVER_MST> CM_PROCESSSCREENSAVER_MST { get; set; }
        public virtual DbSet<ADEMAIL_GROUPMST> ADEMAIL_GROUPMST { get; set; }
        public virtual DbSet<ADEMAIL_GROUPMAPPING> ADEMAIL_GROUPMAPPING { get; set; }
        public virtual DbSet<ADEMAIL_GROUPHISTORY> ADEMAIL_GROUPHISTORY { get; set; }
        public virtual DbSet<A00CONVERTTOCR> A00CONVERTTOCR { get; set; }
        public virtual DbSet<A00REMARKS> A00REMARKS { get; set; }
        public virtual DbSet<ADPOWERSCOPEMASTER> ADPOWERSCOPEMASTER { get; set; }
        public virtual DbSet<CM_PROCESSATTACHMENTAPPMAPPING_TRN> CM_PROCESSATTACHMENTAPPMAPPING_TRN { get; set; }
        public virtual DbSet<CALENDARMASTERHEADER> CALENDARMASTERHEADER { get; set; }
        public virtual DbSet<CALENDARMASTERAPPHIS> CALENDARMASTERAPPHIS { get; set; }
        public virtual DbSet<ISMSHDRMST> ISMSHDRMST { get; set; }
        public virtual DbSet<ISMSHDRDTL> ISMSHDRDTL { get; set; }
        public virtual DbSet<ADPOWERMASTER> ADPOWERMASTER { get; set; }
        public virtual DbSet<ASSETREGISTERDOCUMENTCONTROL> ASSETREGISTERDOCUMENTCONTROL { get; set; }
        public virtual DbSet<AD_EMERGENCYNO_TRN> AD_EMERGENCYNO_TRN { get; set; }
        //public virtual DbSet<SYPLANT> SYPLANT { get; set; }
        public virtual DbSet<ADMENUPARAM_MST> ADMENUPARAM_MST { get; set; }
        public virtual DbSet<ADMENUROLE_MAPPING_MST> ADMENUROLE_MAPPING_MST { get; set; }
        public virtual DbSet<ADMENUPARAMMAPPING_TRN> ADMENUPARAMMAPPING_TRN { get; set; }
        public virtual DbSet<ADORGLEVEL> ADORGLEVEL { get; set; }
        public virtual DbSet<ADORGLEVELHEAD> ADORGLEVELHEAD { get; set; }
        public virtual DbSet<ADORGLEVELTYPE> ADORGLEVELTYPE { get; set; }
        public virtual DbSet<ADPROCESS_MST> ADPROCESS_MST { get; set; }
        public virtual DbSet<ADROLE_MST> ADROLE_MST { get; set; }
        public virtual DbSet<ADROLEUSER_MAPPING_TRN> ADROLEUSER_MAPPING_TRN { get; set; }
        public virtual DbSet<AD_FAMOUSPERSONALITY_MST> AD_FAMOUSPERSONALITY_MST { get; set; }
        public virtual DbSet<AD_PERSONALITYQUOTES_TRN> AD_PERSONALITYQUOTES_TRN { get; set; }
        //public virtual DbSet<CM_PROCESSATTACHMENTAPP_TRN> CM_PROCESSATTACHMENTAPP_TRN { get; set; }
        public virtual DbSet<CM_PRESIDENTMSGAPP_TRN> CM_PRESIDENTMSGAPP_TRN { get; set; }
        //public virtual DbSet<CM_PROCESSATTACHMENT_TRN> CM_PROCESSATTACHMENT_TRN { get; set; }
        public virtual DbSet<ADMENU_MST> ADMENU_MST { get; set; }
        public virtual DbSet<VPF_ENTRYDETAILS> VPF_ENTRYDETAILS { get; set; }
        public virtual DbSet<AD_USERMENU_LOG> AD_USERMENU_LOG { get; set; }
        public virtual DbSet<MEDINS_EMPPOLICYLOC> MEDINS_EMPPOLICYLOC { get; set; }
        public virtual DbSet<TP_EMP_DETAIL> TP_EMP_DETAIL { get; set; }
        public virtual DbSet<MEDINS_DESG_POLICYTYPE_MAP> MEDINS_DESG_POLICYTYPE_MAP { get; set; }
        public virtual DbSet<MEDINS_POLICYTYPE_MST> MEDINS_POLICYTYPE_MST { get; set; }
        public virtual DbSet<MEDINS_TYPEPLANT_MAP> MEDINS_TYPEPLANT_MAP { get; set; }
        public virtual DbSet<MEDINS_APPHISTORY> MEDINS_APPHISTORY { get; set; }
        public virtual DbSet<MEDINS_EMPLOYEE_DTL> MEDINS_EMPLOYEE_DTL { get; set; }
        public virtual DbSet<MEDINS_DEPENDENTS_DTL> MEDINS_DEPENDENTS_DTL { get; set; }
        public virtual DbSet<MEDINS_DEPENDENTS_DTL_HIS> MEDINS_DEPENDENTS_DTL_HIS { get; set; }
        public virtual DbSet<MEDINS_EMPLOYEE_DTL_HIS> MEDINS_EMPLOYEE_DTL_HIS { get; set; }
        public virtual DbSet<MEDINS_RENEWAL_SETTING> MEDINS_RENEWAL_SETTING { get; set; }
        public virtual DbSet<ASRWFH_APPROVALHIS> ASRWFH_APPROVALHIS { get; set; }
        public virtual DbSet<ASRWFH_HEADER> ASRWFH_HEADER { get; set; }
        public virtual DbSet<ADORGCOORDINATOR> ADORGCOORDINATOR { get; set; }
        public virtual DbSet<SIS_ASSETCONDITION> SIS_ASSETCONDITION { get; set; }
        public virtual DbSet<ASSETAPP_LOG> ASSETAPP_LOG { get; set; }
        public virtual DbSet<SIS_ASSETTYPE_MST> SIS_ASSETTYPE_MST { get; set; }
        public virtual DbSet<SIS_AST_APPAUTH_MST> SIS_AST_APPAUTH_MST { get; set; }
        public virtual DbSet<SIS_AST_DISPOSALAPPHISTORY> SIS_AST_DISPOSALAPPHISTORY { get; set; }
        public virtual DbSet<SIS_AST_DISPOSALCUSTDETAIL> SIS_AST_DISPOSALCUSTDETAIL { get; set; }
        public virtual DbSet<SIS_AST_ERROR_MST> SIS_AST_ERROR_MST { get; set; }
        public virtual DbSet<SIS_AST_OPERATIONMAPP> SIS_AST_OPERATIONMAPP { get; set; }
        public virtual DbSet<SIS_AST_OPMAPPING_MST> SIS_AST_OPMAPPING_MST { get; set; }
        public virtual DbSet<SIS_AST_PROCESSSTATUS_MST> SIS_AST_PROCESSSTATUS_MST { get; set; }
        public virtual DbSet<SIS_AST_VALIDATION_MST> SIS_AST_VALIDATION_MST { get; set; }
        public virtual DbSet<VW_MEDINS_EMPDETAIL> VW_MEDINS_EMPDETAIL { get; set; }
        public virtual DbSet<VW_MEDINS_MSTDETAILS> VW_MEDINS_MSTDETAILS { get; set; }
        public virtual DbSet<HRSECURITYANSLIST> HRSECURITYANSLIST { get; set; }
        public virtual DbSet<HRSECURITYQUIZ> HRSECURITYQUIZ { get; set; }
        public virtual DbSet<HRSECURITYQUSLIST> HRSECURITYQUSLIST { get; set; }
        public virtual DbSet<HRSECURITYSURVEY> HRSECURITYSURVEY { get; set; }
        public virtual DbSet<HRSECURITYSURVEYCOMMENT> HRSECURITYSURVEYCOMMENT { get; set; }
        public virtual DbSet<HRSECURITYSURVEYSKIPCOUNT> HRSECURITYSURVEYSKIPCOUNT { get; set; }
        public virtual DbSet<HMSIHOLIDAYS> HMSIHOLIDAYS { get; set; }
        public virtual DbSet<ASRCALOPMAPPING> ASRCALOPMAPPING { get; set; }
        public virtual DbSet<SIS_AST_DISPOSALDETAIL> SIS_AST_DISPOSALDETAIL { get; set; }
        public virtual DbSet<SIS_AST_PARAM_MST> SIS_AST_PARAM_MST { get; set; }
        public virtual DbSet<SYSITESHIFT> SYSITESHIFT { get; set; }
        public virtual DbSet<SIS_AST_DISPOSALHEADER> SIS_AST_DISPOSALHEADER { get; set; }
        public virtual DbSet<SIS_AST_OPERATION> SIS_AST_OPERATION { get; set; }
        public virtual DbSet<SIS_AST_MGFPLANTAPPROVAL> SIS_AST_MGFPLANTAPPROVAL { get; set; }
        public virtual DbSet<FINVENDORMASTERMST> FINVENDORMASTERMST { get; set; }
        public virtual DbSet<DGIT_POAPPAUTHSEQ> DGIT_POAPPAUTHSEQ { get; set; }
        public virtual DbSet<DGIT_POAPPHISTORY> DGIT_POAPPHISTORY { get; set; }
        public virtual DbSet<DGIT_POAPPSKIP> DGIT_POAPPSKIP { get; set; }
        public virtual DbSet<DGIT_ICRPTRIGHTS> DGIT_ICRPTRIGHTS { get; set; }
        //public virtual DbSet<ICADORGLEVEL> ICADORGLEVELs { get; set; }
        public virtual DbSet<DGIT_PRAPPSKIP> DGIT_PRAPPSKIP { get; set; }
        public virtual DbSet<DGIT_PODETAIL> DGIT_PODETAIL { get; set; }
        public virtual DbSet<DGIT_POHEADER> DGIT_POHEADER { get; set; }
        public virtual DbSet<DGIT_POPRLINK_DTL> DGIT_POPRLINK_DTL { get; set; }
        public virtual DbSet<DGIT_SIGNAUTHMAP> DGIT_SIGNAUTHMAP { get; set; }
        //public virtual DbSet<DGIT_USERDETAIL> DGIT_USERDETAIL { get; set; }
        public virtual DbSet<DGIT_TR_OP_LIST> DGIT_TR_OP_LIST { get; set; }
        public virtual DbSet<DGIT_PRAPPAUTHSEQ> DGIT_PRAPPAUTHSEQ { get; set; }
        public virtual DbSet<DGIT_PRAPPHISTORY> DGIT_PRAPPHISTORY { get; set; }
        public virtual DbSet<DGIT_PRDETAIL> DGIT_PRDETAIL { get; set; }
        public virtual DbSet<DGIT_PRHEADER> DGIT_PRHEADER { get; set; }
        public virtual DbSet<DGIT_IOMAPPAUTHSEQ> DGIT_IOMAPPAUTHSEQ { get; set; }
        public virtual DbSet<DGIT_IOMAPPHEADER> DGIT_IOMAPPHEADER { get; set; }
        public virtual DbSet<DGIT_IOMAPPHISTORY> DGIT_IOMAPPHISTORY { get; set; }
        public virtual DbSet<DGIT_IOMDETAIL> DGIT_IOMDETAIL { get; set; }
        public virtual DbSet<DGIT_IOMHEADER> DGIT_IOMHEADER { get; set; }
        public virtual DbSet<DGIT_IOMCATMST> DGIT_IOMCATMST { get; set; }
        public virtual DbSet<DGIT_ACRAPPAUTHSEQ> DGIT_ACRAPPAUTHSEQ { get; set; }
        public virtual DbSet<DGIT_ACRAPPHISTORY> DGIT_ACRAPPHISTORY { get; set; }
        public virtual DbSet<DGIT_ACRAPPSKIP> DGIT_ACRAPPSKIP { get; set; }
        public virtual DbSet<DGIT_ACRDETAIL> DGIT_ACRDETAIL { get; set; }
        public virtual DbSet<DGIT_ACRHEADER> DGIT_ACRHEADER { get; set; }
        //public virtual DbSet<ASR_FORMA_DETAIL> ASR_FORMA_DETAIL { get; set; }
        //public virtual DbSet<ASR_FORMA_HEADER> ASR_FORMA_HEADER { get; set; }
        public virtual DbSet<ASSET_REGISTER_MEMNOMINATION> ASSET_REGISTER_MEMNOMINATION { get; set; }
        public virtual DbSet<ASSET_REGISTER_PERIODSETTING> ASSET_REGISTER_PERIODSETTING { get; set; }
        //public virtual DbSet<ASSEST_REGISTERUPDNOMINATION> ASSEST_REGISTERUPDNOMINATION { get; set; }
        public virtual DbSet<ASSET_REGISTE_D_ASSET> ASSET_REGISTE_D_ASSET { get; set; }
        public virtual DbSet<ASSET_REGISTER_ASSETDETAILS> ASSET_REGISTER_ASSETDETAILS { get; set; }
        public virtual DbSet<ASSET_REGISTER_CLASSIFICATION> ASSET_REGISTER_CLASSIFICATION { get; set; }
        public virtual DbSet<ASSET_REGISTER_COMMON_ASSET> ASSET_REGISTER_COMMON_ASSET { get; set; }
        public virtual DbSet<ASSET_REGISTER_COMMONEXIST> ASSET_REGISTER_COMMONEXIST { get; set; }
        public virtual DbSet<ASSET_REGISTER_ITGRC_PERIOD> ASSET_REGISTER_ITGRC_PERIOD { get; set; }
        public virtual DbSet<ASSET_REGISTER_TYPES> ASSET_REGISTER_TYPES { get; set; }
        //public virtual DbSet<ASSET_REGISTER_UPDATE> ASSET_REGISTER_UPDATE { get; set; }
        public virtual DbSet<A00_ALLOCATION> A00_ALLOCATION { get; set; }
        public virtual DbSet<DGIT_SES_PAYMENTADVISEMASTER> DGIT_SES_PAYMENTADVISEMASTER { get; set; }
        public virtual DbSet<DGIT_PAYMENTADVISE_DTL> DGIT_PAYMENTADVISE_DTL { get; set; }
        //public virtual DbSet<KAIZEN_FORM_STATUS> KAIZEN_FORM_STATUS { get; set; }
        //public virtual DbSet<KAIZEN_TOP10_DIV_COMMITTEE> KAIZEN_TOP10_DIV_COMMITTEE { get; set; }
        //public virtual DbSet<KAIZEN_DIV_COMMITTEE_RIGHTS> KAIZEN_DIV_COMMITTEE_RIGHTS { get; set; }
        //public virtual DbSet<KAIZEN_AREA_MASTER> KAIZEN_AREA_MASTER { get; set; }
        //public virtual DbSet<KAIZENAPPAUTH> KAIZENAPPAUTH { get; set; }
        //public virtual DbSet<KAIZEN_GRADE> KAIZEN_GRADE { get; set; }
        //public virtual DbSet<KAIZEN_GRADE_DETAILS> KAIZEN_GRADE_DETAILS { get; set; }
        //public virtual DbSet<APEX_LOGIN_VALIDATE> APEX_LOGIN_VALIDATE { get; set; }
        public virtual DbSet<KAIZEN_DEPT_COMMITTEE_USERS> KAIZEN_DEPT_COMMITTEE_USERS { get; set; }
        //public virtual DbSet<KAIZEN_DEPT_COMMITTEE_MASTER> KAIZEN_DEPT_COMMITTEE_MASTER { get; set; }
        //public virtual DbSet<KAIZEN_DIV_COMMITTEE_MASTER> KAIZEN_DIV_COMMITTEE_MASTER { get; set; }
        public virtual DbSet<KAIZEN_DIV_COMMITTEE_USERS> KAIZEN_DIV_COMMITTEE_USERS { get; set; }
        //public virtual DbSet<D_TRANSACTION_STATUS> D_TRANSACTION_STATUS { get; set; }
        public virtual DbSet<A00_DEFICIENCY> A00_DEFICIENCY { get; set; }
        public virtual DbSet<A00ACTIVITY> A00ACTIVITY { get; set; }
        public virtual DbSet<A00ACTIVITYHISTORY> A00ACTIVITYHISTORY { get; set; }
        //public virtual DbSet<A00ALLOCATION> A00ALLOCATION { get; set; }
        //public virtual DbSet<A00ALLOCATIONSUB> A00ALLOCATIONSUB { get; set; }
        public virtual DbSet<A00APPROVAL> A00APPROVAL { get; set; }
        //public virtual DbSet<A00DEFICIENCY_1> A00DEFICIENCY_1 { get; set; }
        //public virtual DbSet<A00DEFICIENCYCLOSURE_1> A00DEFICIENCYCLOSURE_1 { get; set; }
        public virtual DbSet<A00DTLTB> A00DTLTB { get; set; }
        public virtual DbSet<A00INVFORCAST> A00INVFORCAST { get; set; }
        public virtual DbSet<A00ITCONFAPPROVALHISTORY> A00ITCONFAPPROVALHISTORY { get; set; }
        public virtual DbSet<A00ITCONFIRMATION> A00ITCONFIRMATION { get; set; }
        public virtual DbSet<A00PROJECTSTATUSUPDATE> A00PROJECTSTATUSUPDATE { get; set; }
        public virtual DbSet<A00SYKI> A00SYKI { get; set; }
        //public virtual DbSet<A00ITAPPROVAL> A00ITAPPROVAL { get; set; }
        public virtual DbSet<INVFORCAST> INVFORCAST { get; set; }
        public virtual DbSet<ASSET_REGISTER_ORG_MAPPING> ASSET_REGISTER_ORG_MAPPING { get; set; }
        public virtual DbSet<SYPLANT> SYPLANT { get; set; }
        public virtual DbSet<DGIT_PRADDAUTH_MAP> DGIT_PRADDAUTH_MAP { get; set; }
        public virtual DbSet<ASSET_REGISTER_MAIL> ASSET_REGISTER_MAIL { get; set; }
        public virtual DbSet<DGIT_PROPERATIONMAP> DGIT_PROPERATIONMAP { get; set; }
        public virtual DbSet<DGIT_PRPURSTATUS> DGIT_PRPURSTATUS { get; set; }
        public virtual DbSet<DGIT_SESMRN_DTL> DGIT_SESMRN_DTL { get; set; }
        public virtual DbSet<DGIT_SESMRN_HEADER> DGIT_SESMRN_HEADER { get; set; }
        public virtual DbSet<DGIT_SMAPPAUTH_SEQ> DGIT_SMAPPAUTH_SEQ { get; set; }
        public virtual DbSet<DGIT_SMAPPHISTORY> DGIT_SMAPPHISTORY { get; set; }
        public virtual DbSet<DGIT_SMOPERATION_MAP> DGIT_SMOPERATION_MAP { get; set; }
        public virtual DbSet<DGIT_PRBUYER_MAP> DGIT_PRBUYER_MAP { get; set; }
        public virtual DbSet<DGIT_PRBUYERMST> DGIT_PRBUYERMST { get; set; }
        public virtual DbSet<DGIT_PRADDAPP_MST> DGIT_PRADDAPP_MST { get; set; }
        public virtual DbSet<DGIT_SMTAXAEMP_MST> DGIT_SMTAXAEMP_MST { get; set; }
        public virtual DbSet<DGIT_PRCAT_MST> DGIT_PRCAT_MST { get; set; }
        public virtual DbSet<DGIT_SMPLANTSITEMAP> DGIT_SMPLANTSITEMAP { get; set; }
        public virtual DbSet<ASSET_USER_MAPPING> ASSET_USER_MAPPING { get; set; }
        public virtual DbSet<VW_DGIT_SMHEADER> VW_DGIT_SMHEADER { get; set; }
        public virtual DbSet<VW_PRUSERDASHBOARD> VW_PRUSERDASHBOARD { get; set; }
        public virtual DbSet<VW_SMIOCGAPPROVER_LIST> VW_SMIOCGAPPROVER_LIST { get; set; }
        public virtual DbSet<DGIT_SMIOCGBLOCK> DGIT_SMIOCGBLOCK { get; set; }
        public virtual DbSet<VW_DGIT_PODASHBOARD> VW_DGIT_PODASHBOARD { get; set; }
        //public virtual DbSet<BC_ADMIN_MEALSBOOKING_TRN> BC_ADMIN_MEALSBOOKING_TRN { get; set; }
        //public virtual DbSet<BC_ALACARTEFOOD_HDR> BC_ALACARTEFOOD_HDR { get; set; }
        //public virtual DbSet<BC_ALACARTEFOOD_MST> BC_ALACARTEFOOD_MST { get; set; }
        //public virtual DbSet<BC_ALACARTEFOOD_TRN> BC_ALACARTEFOOD_TRN { get; set; }
        //public virtual DbSet<BC_GUEST_DTL> BC_GUEST_DTL { get; set; }
        //public virtual DbSet<BC_GUESTMEALBOOKING_DTL> BC_GUESTMEALBOOKING_DTL { get; set; }
        //public virtual DbSet<BC_GUESTMEALBOOKING_TRN> BC_GUESTMEALBOOKING_TRN { get; set; }
        //public virtual DbSet<BC_MEALS_AVAILABILITY> BC_MEALS_AVAILABILITY { get; set; }
        //public virtual DbSet<BC_MEALSBOOKING_TRN> BC_MEALSBOOKING_TRN { get; set; }
        //public virtual DbSet<BC_MEALSLOTMST> BC_MEALSLOTMST { get; set; }
        //public virtual DbSet<BC_MENUMST> BC_MENUMST { get; set; }
        //public virtual DbSet<BC_MST_MEAL_TYPE> BC_MST_MEAL_TYPE { get; set; }
        //public virtual DbSet<BC_MST_MEALS> BC_MST_MEALS { get; set; }
        //public virtual DbSet<BC_MST_VALIDATION> BC_MST_VALIDATION { get; set; }
        //public virtual DbSet<BC_PRICE_VALIDITY> BC_PRICE_VALIDITY { get; set; }
        //public virtual DbSet<BC_SUBSIDIZED_MEAL_TOKEN> BC_SUBSIDIZED_MEAL_TOKEN { get; set; }
        //public virtual DbSet<BC_SUSIDIZED_MEAL_TRN> BC_SUSIDIZED_MEAL_TRN { get; set; }
        //public virtual DbSet<BC_HRECODEMAP> BC_HRECODEMAP { get; set; }
        public virtual DbSet<ASR_BENEVOLENT_DT> ASR_BENEVOLENT_DT { get; set; }
        public virtual DbSet<ASR_BENEVOLENT_MST> ASR_BENEVOLENT_MST { get; set; }
        public virtual DbSet<IMPACTTYPE> IMPACTTYPE { get; set; }
        //public virtual DbSet<RISK_APPROVAL_DETAILS> RISK_APPROVAL_DETAILS { get; set; }
        public virtual DbSet<RISK_ASSESSMENT_DEFICIENCY> RISK_ASSESSMENT_DEFICIENCY { get; set; }
        public virtual DbSet<RISK_ASSESSMENT_ITGRC_PERIOD> RISK_ASSESSMENT_ITGRC_PERIOD { get; set; }
        public virtual DbSet<RISK_ASSESSMENT_MAIL> RISK_ASSESSMENT_MAIL { get; set; }
        //public virtual DbSet<RISK_ASSESSMENT_MEMNOMINATION> RISK_ASSESSMENT_MEMNOMINATION { get; set; }
        //public virtual DbSet<RISK_ASSESSMENT_PERIOSETTINGS> RISK_ASSESSMENT_PERIOSETTINGS { get; set; }
        public virtual DbSet<RISK_CATEGORY_MASTER> RISK_CATEGORY_MASTER { get; set; }
        public virtual DbSet<RISK_CURRENTMEASURE_LVL> RISK_CURRENTMEASURE_LVL { get; set; }
        public virtual DbSet<RISK_FREQUENCY_IMPACT_MAP_MAST> RISK_FREQUENCY_IMPACT_MAP_MAST { get; set; }
        public virtual DbSet<RISK_FREQUENCY_MASTER> RISK_FREQUENCY_MASTER { get; set; }
        public virtual DbSet<RISK_IMPACT_MASTER> RISK_IMPACT_MASTER { get; set; }
        public virtual DbSet<RISK_LEADTO_BREACH> RISK_LEADTO_BREACH { get; set; }
        public virtual DbSet<RISK_LEVEL_MASTER> RISK_LEVEL_MASTER { get; set; }
        //public virtual DbSet<RISK_POTENTIAL_MASTER> RISK_POTENTIAL_MASTER { get; set; }
        public virtual DbSet<RISK_REGISTER_ASSETDETAILS> RISK_REGISTER_ASSETDETAILS { get; set; }
        public virtual DbSet<RISK_REGISTER_COMMONEXIST> RISK_REGISTER_COMMONEXIST { get; set; }
        public virtual DbSet<RISK_STATEMENT_MASTER> RISK_STATEMENT_MASTER { get; set; }
        public virtual DbSet<RISK_TREATMENT_RISK_LVL> RISK_TREATMENT_RISK_LVL { get; set; }
        public virtual DbSet<RISK_USER_MAPPING> RISK_USER_MAPPING { get; set; }
        public virtual DbSet<RISKASSESSMENT> RISKASSESSMENT { get; set; }
        public virtual DbSet<DGIT_ACR_PAYMENTADVISE_DTL> DGIT_ACR_PAYMENTADVISE_DTL { get; set; }
        public virtual DbSet<DGIT_ACR_PAYMENTADVISEMASTER> DGIT_ACR_PAYMENTADVISEMASTER { get; set; }
        public virtual DbSet<VW_DGIT_ACRHEADER> VW_DGIT_ACRHEADER { get; set; }
        //public virtual DbSet<VW_ASSOCIATELVLDETAILS1> VW_ASSOCIATELVLDETAILS1 { get; set; }


        // WorkedBy Dalbir : Login Module

        public virtual DbSet<SYKI> SYKI { get; set; }
        public virtual DbSet<ADEMPLOGIN> ADEMPLOGIN { get; set; }
        public virtual DbSet<ADLOGINUSER> ADLOGINUSER { get; set; }
        public virtual DbSet<ADEMPLOYEE> ADEMPLOYEE { get; set; }
        public virtual DbSet<CM_PROCESSATTACHMENT_TRN> CM_PROCESSATTACHMENT_TRN { get; set; }
        public virtual DbSet<CM_PROCESS_MST> CM_PROCESS_MST { get; set; }
        public virtual DbSet<VW_ASSOCIATELVLDETAILS> VW_ASSOCIATELVLDETAILS { get; set; }   //for view do we need to do anything
        public virtual DbSet<ADEMPDIVDEPTSECT> ADEMPDIVDEPTSECT { get; set; }
        public virtual DbSet<ADDESIGNATION> ADDESIGNATION { get; set; }
        public virtual DbSet<SYPARAMETERS> SYPARAMETERS { get; set; }
        public virtual DbSet<CM_PROCESSATTACHMENTAPP_TRN> CM_PROCESSATTACHMENTAPP_TRN { get; set; }

        public virtual DbSet<SPROC_EMAILURL_GET> SPROC_EMAILURL_GET { get; set; }
        public virtual DbSet<T_HR_CHRONICLES> T_HR_CHRONICLES { get; set; }

        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - Start
        public virtual DbSet<DGIT_PR_IOM_LINKING> DGIT_PR_IOM_LINKING { get; set; }
        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - End
        public virtual DbSet<LostAndFound> LostAndFound { get; set; }
        public virtual DbSet<LostAndFoundHistory> LostAndFoundHistory { get; set; }
        public virtual DbSet<SECURITY_LOCATION_MAP> SECURITY_LOCATION_MAP { get; set; }

        // VEHICLE tables | CR7923 | Start
        public virtual DbSet<EMP_VEHICLE_MASTER> EMPVEHICLEMASTER { get; set; }
        public virtual DbSet<EMP_VEHICLE_DETAIL> EMP_VEHICLE_DETAIL { get; set; }
        public virtual DbSet<EMP_VEHICLE_HISTORY> EMP_VEHICLE_HISTORY { get; set; }
        public virtual DbSet<PARKING_ZONE> PARKING_ZONE { get; set; }
        public virtual DbSet<ADMIN_LOCATION_MAPPING> ADMIN_LOCATION_MAPPING { get; set; }
        public virtual DbSet<SECURITY_LOCATION_MAPPING> SECURITY_LOCATION_MAPPING { get; set; }
        // VEHICLE tables | CR7923 | End
        // Stage  + Parallel  + Sequentia
        public virtual DbSet<DGIT_PIOMAPPAUTHSEQ> DGIT_PIOMAPPAUTHSEQ { get; set; }
        public virtual DbSet<DGIT_PIOMAPPHEADER> DGIT_PIOMAPPHEADER { get; set; }
        public virtual DbSet<DGIT_PIOMAPPHISTORY> DGIT_PIOMAPPHISTORY { get; set; }
        public virtual DbSet<DGIT_PIOMDETAIL> DGIT_PIOMDETAIL { get; set; }
        public virtual DbSet<DGIT_PIOMHEADER> DGIT_PIOMHEADER { get; set; }
        public virtual DbSet<DGIT_PIOMCATMST> DGIT_PIOMCATMST { get; set; }
        public virtual DbSet<DGIT_PIOMREPORT> DGIT_PIOMREPORT { get; set; }
        public virtual DbSet<DGIT_PIOM_STAGE> DGIT_PIOM_STAGE { get; set; }
        public virtual DbSet<DGIT_PIOM_SEQ_APPROVAL> DGIT_PIOM_PARALLEL_GROUP { get; set; }
        public virtual DbSet<DGIT_PIOM_SEQ_APPROVAL> DGIT_PIOM_SEQ_APPROVAL { get; set; }
        public virtual DbSet<CONTINUOUSATTENDANCELOG> CONTINUOUSATTENDANCELOG { get; set; } // added by aumento :: SR113877
        public virtual DbSet<IRHEADMAILMAPPING> IRHEADMAILMAPPING { get; set; } // added by aumento :: SR113877

        //Locker Management System
        public virtual DbSet<FLOORMASTER> FLOORMASTER { get; set; }

        public virtual DbSet<LOCKERASSIGNMENTMASTER> LOCKERASSIGNMENTMASTER { get; set; }

        public virtual DbSet<LOCKERBOXMASTER> LOCKERBOXMASTER { get; set; }

        public virtual DbSet<LOCKERMASTER> LOCKERMASTER { get; set; }

        public virtual DbSet<LOCKER_ADMIN_LOCATION_MAPPING> LOCKER_ADMIN_LOCATION_MAPPING { get; set; }
        public virtual DbSet<EmployeeLockerAllocation> EmployeeLockerAllocation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<CM_PROCESSATTACHMENT_TRN>()
            //.ToTable("CM_PROCESSATTACHMENT_TRN")
            //.HasOne(k => k.CM_PROCESS_MST)
            //.HasMany(d => d.CM_PROCESSATTACHMENT_TRN);
            //modelBuilder.Entity<CM_PROCESS_MST>()
            // .Property(e => e.MSTPROCESSID)
            // .ValueGeneratedNever();

            modelBuilder.Entity<RISK_ASSESSMENT_DEFICIENCY>().HasNoKey().ToTable("RISK_ASSESSMENT_DEFICIENCY"); 

            modelBuilder.Entity<ADEMPLOGIN>().HasKey(k => new { k.ADEMPCODE, k.SYUSERTYPEID }); // for composite key
            modelBuilder.Entity<VW_ASSOCIATELVLDETAILS>().HasNoKey();
            modelBuilder.Entity<AD_USERMENU_LOG>().HasNoKey();
            modelBuilder.Entity<SPROC_EMAILURL_GET>().HasNoKey();

            // modelBuilder.Entity<DGIT_SMOPERATION_MAP>().HasNoKey();
            modelBuilder.Entity<VW_PRUSERDASHBOARD>().HasNoKey();
            modelBuilder.Entity<DGIT_ICRPTRIGHTS>().HasNoKey();
            //modelBuilder.Entity<VW_ASSOCIATELVLDETAILS1>().HasNoKey();
            //modelBuilder.Entity<ICADORGLEVEL>().HasNoKey();
            modelBuilder.Entity<VW_DGIT_SMHEADER>().HasNoKey();
            modelBuilder.Entity<VW_SMIOCGAPPROVER_LIST>().HasNoKey();
            modelBuilder.Entity<CONTINUOUSATTENDANCELOG>().HasNoKey(); // added by aumento :: SR113877
            modelBuilder.Entity<IRHEADMAILMAPPING>().HasNoKey(); // added by aumento :: SR113877

            //modelBuilder.Entity<CM_PROCESSATTACHMENT_TRN>().HasNoKey();

            modelBuilder.Entity<CM_PROCESSATTACHMENT_TRN>(entity =>
            {
                entity.Property(e => e.BANNER).HasColumnType("BLOB");
                entity.Property(e => e.ATTACHMENT1).HasColumnType("BLOB");
                entity.Property(e => e.ATTACHMENT2).HasColumnType("BLOB");
            });

            modelBuilder.Entity<ADMENU_MST>(entity =>
            {
                entity.Property(e => e.MENUICON).HasColumnType("BLOB");
            });

            modelBuilder.Entity<ADMENUPARAMMAPPING_TRN>(entity =>
            {
                entity.HasKey(e => e.MAPPINGID).HasName("ADMENUPARAMMAPPING_TRN_ID_PK");
                entity.ToTable("ADMENUPARAMMAPPING_TRN");
                entity.HasOne(d => d.ADMENU_MST).WithMany(p => p.ADMENUPARAMMAPPING_TRN)
                    .HasForeignKey(d => d.MENUID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MENUID_FK");

                entity.HasOne(d => d.ADMENUPARAM_MST).WithMany(p => p.ADMENUPARAMMAPPING_TRN)
                    .HasForeignKey(d => d.MENUPARAMID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MENUPARAMID_FK");
            });

            modelBuilder.Entity<DGIT_ACRDETAIL>()
             .HasOne(d => d.DGIT_ACRHEADER)
             .WithMany(h => h.DGIT_ACRDETAIL)
             .HasForeignKey(d => d.ACRHEADERID)
             .HasConstraintName("FK_ACRDTL_HEADERID");

            modelBuilder.Entity<ISMSHDRMST>()
    .Property(e => e.ISMS_BLOB)
    .HasColumnType("BLOB");

            modelBuilder.Entity<CM_PRESIDENTMSG_TRN>()
  .Property(e => e.ATTACHMENT)
  .HasColumnType("BLOB");

            // Specify table names explicitly in uppercase
            modelBuilder.Entity<JwtUserLog>()
                .ToTable("JWTUSERLOG"); // Use uppercase

            modelBuilder.Entity<RefreshToken>()
                .ToTable("REFRESHTOKEN"); // Use uppercase

            // Map columns if necessary, ensuring they match your schema
            modelBuilder.Entity<JwtUserLog>()
                    .Property(j => j.UserName)
                    .HasColumnName("USERNAME"); // Match casing

            modelBuilder.Entity<RefreshToken>()
                .Property(r => r.Token)
                .HasColumnName("TOKEN");

            modelBuilder.Entity<RefreshToken>()
                .Property(r => r.Expiration)
                .HasColumnName("EXPIRATION");

            modelBuilder.Entity<JwtUserLog>()
                 .HasOne(j => j.RefreshToken)
                 .WithMany() // Assuming RefreshToken doesn't have a collection of JwtUserLog
                 .HasForeignKey(j => j.RefreshTokenId);

            modelBuilder.Entity<VW_MEDINS_MSTDETAILS>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_MEDINS_MSTDETAILS"); // Optional but recommended
            });
            modelBuilder.Entity<VW_MEDINS_EMPDETAIL>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_MEDINS_EMPDETAIL"); // Optional but recommended
            });
            modelBuilder.Entity<VW_DGIT_PODASHBOARD>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_DGIT_PODASHBOARD"); // Optional but recommended
            });

            modelBuilder.Entity<LostAndFound>(entity =>
            {
                entity.ToTable("LOST_AND_FOUND");

                entity.HasKey(e => e.Id).HasName("PK_LOST_AND_FOUND");

                entity.Property(e => e.Id)
                      .HasColumnName("ID")
                      .ValueGeneratedOnAdd(); // DB trigger+sequence will handle

                entity.Property(e => e.ItemType).HasColumnName("ITEMTYPE").HasMaxLength(200);
                entity.Property(e => e.ItemName).HasColumnName("ITEMNAME").HasMaxLength(200);
                entity.Property(e => e.IdentificationMark).HasColumnName("IDENTIFICATIONMARK").HasMaxLength(500);
                entity.Property(e => e.MaterialType).HasColumnName("MATERIALTYPE").HasMaxLength(200);
                entity.Property(e => e.AdditionalDetails).HasColumnName("ADDITIONALDETAILS").HasMaxLength(1000);
                entity.Property(e => e.LocationId).HasColumnName("LOCATION_ID").IsRequired();
                entity.Property(e => e.Date).HasColumnName("DATE").IsRequired();
                entity.Property(e => e.ItemPhotoPath).HasColumnName("ITEMPHOTOPATH").HasMaxLength(500);
                entity.Property(e => e.Status).HasColumnName("STATUS");
                entity.Property(e => e.ClaimedBy).HasColumnName("CLAIMEDBY").HasMaxLength(200);
                entity.Property(e => e.ClaimedByEmpCode).HasColumnName("CLAIMEDBYEMPCODE").HasMaxLength(100);
                entity.Property(e => e.DateClaimed).HasColumnName("DATECLAIMED");
                entity.Property(e => e.VerificationDetails).HasColumnName("VERIFICATIONDETAILS").HasMaxLength(1000);
                entity.Property(e => e.IsActive)
         .HasColumnName("ISACTIVE")
         .HasConversion<short>()
         .HasDefaultValue(1);
                entity.Property(e => e.CreatedBy).HasColumnName("CREATEDBY").HasMaxLength(100).IsRequired();
                entity.Property(e => e.CreatedDate).HasColumnName("CREATEDDATE").HasDefaultValueSql("SYSDATE").IsRequired();
                entity.Property(e => e.ModifiedBy).HasColumnName("MODIFIEDBY").HasMaxLength(100);
                entity.Property(e => e.ModifiedDate).HasColumnName("MODIFIEDDATE");
                entity.Property(e => e.IsResubmit).HasColumnName("ISRESUBMIT")
         .HasConversion<short>()
         .HasDefaultValue(0);

                // ✅ New columns
                entity.Property(e => e.EmployeeType)
                      .HasColumnName("EMPLOYEETYPE");

                entity.Property(e => e.CompanyName)
                      .HasColumnName("COMPANYNAME")
                      .HasMaxLength(200);
                // Foreign key relationship
                entity.HasOne(d => d.Location)
                      .WithMany()
                      .HasForeignKey(d => d.LocationId)
                      .HasConstraintName("FK_LOST_AND_FOUND_SITE");
            });
            modelBuilder.Entity<LostAndFoundHistory>(entity =>
            {
                entity.ToTable("LOST_AND_FOUND_HISTORY");

                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID");

                entity.Property(e => e.LOST_AND_FOUND_ID)
                    .HasColumnName("LOST_AND_FOUND_ID");

                entity.Property(e => e.LAF_STATUS)
                    .HasColumnName("LAF_STATUS");

                entity.Property(e => e.REMARKS)
                    .HasColumnName("REMARKS")
                    .HasMaxLength(500);

                entity.Property(e => e.ADEMPCODE)
                    .HasColumnName("ADEMPCODE");
                   

                entity.Property(e => e.CHANGEDDATE)
                    .HasColumnName("CHANGEDDATE");

                entity.Property(e => e.ADDEDBY)
                    .HasColumnName("ADDEDBY");

                entity.Property(e => e.ADDEDDATE)
                    .HasColumnName("ADDEDDATE");

                entity.Property(e => e.UPDATEBY)
                    .HasColumnName("UPDATEBY");

                entity.Property(e => e.UPDATEDATE)
                    .HasColumnName("UPDATEDATE");

                entity.HasOne<LostAndFound>()
                        .WithMany()
                        .HasForeignKey(e => e.LOST_AND_FOUND_ID)
                        .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<SECURITY_LOCATION_MAP>(entity =>
            {
                entity.ToTable("SECURITY_LOCATION_MAP");

                entity.HasKey(e => e.Id).HasName("PK_SECURITY_LOCATION_MAP");

                entity.Property(e => e.Id)
                      .HasColumnName("ID")
                      .ValueGeneratedOnAdd(); 

                entity.Property(e => e.SiteId)
                      .HasColumnName("SITEID")
                      .IsRequired();

                entity.Property(e => e.EmpCode)
                      .HasColumnName("EMPCODE")
                      .IsRequired();

                entity.Property(e => e.IsActive)
                      .HasColumnName("ISACTIVE")
                      .HasDefaultValue((short)1);

                entity.Property(e => e.CreatedBy)
                      .HasColumnName("CREATEDBY")
                      .HasMaxLength(100);

                entity.Property(e => e.CreatedDate)
                      .HasColumnName("CREATEDDATE")
                      .HasDefaultValueSql("SYSDATE");

                entity.Property(e => e.ModifiedBy)
                      .HasColumnName("MODIFIEDBY")
                      .HasMaxLength(100);

                entity.Property(e => e.ModifiedDate)
                      .HasColumnName("MODIFIEDDATE");

                // Relationships (if SYSITE and ADEMPLOYEE are also mapped)
                entity.HasOne(e => e.Site)
               .WithMany()
              .HasForeignKey(e => e.SiteId)// <-- important
              .HasConstraintName("FK_SECURITY_LOCATION_SITE");

                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmpCode);
            });

            // VEHICLE tables | CR7923 | Start

            modelBuilder.Entity<EMP_VEHICLE_MASTER>(entity =>
            {
                entity.ToTable("EMP_VEHICLE_MASTER");

                entity.HasKey(e => e.VEHICLEID);

                entity.Property(e => e.VEHICLEID)
                      .HasColumnName("VEHICLEID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.EMPCODE)
                      .HasColumnName("EMPCODE")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.STATUS)
                      .HasColumnName("STATUS");

                entity.Property(x => x.IsExpired)
                        .HasColumnName("IS_EXPIRED")
                        .HasConversion(
                            v => v ? 1 : 0,  
                            v => v == 1       
                        );

                // Admin
                entity.Property(e => e.ADMIN_ACTION_BY)
                      .HasColumnName("ADMIN_ACTION_BY")
                      .HasMaxLength(50);

                entity.Property(e => e.ADMIN_ACTION_DATE)
                      .HasColumnName("ADMIN_ACTION_DATE");

                entity.Property(e => e.DRIVER_NAME)
                       .HasColumnName("DRIVER_NAME");

                // Parking Zone
                entity.Property(e => e.PARKING_ZONE_ID)
                      .HasColumnName("PARKING_ZONE_ID");

                entity.HasOne(e => e.PARKING_ZONE)
                      .WithMany()
                      .HasForeignKey(e => e.PARKING_ZONE_ID)
                      .HasConstraintName("FK_EMPVEHICLEMASTER_PARKINGZONE");

                // Security step
                entity.Property(e => e.SECURITY_ACTION_BY)
                      .HasColumnName("SECURITY_ACTION_BY")
                      .HasMaxLength(50);

                entity.Property(e => e.SECURITY_ACTION_DATE)
                      .HasColumnName("SECURITY_ACTION_DATE");



                // Vehicle Info (now SHORT)
                entity.Property(e => e.VEHICLE_TYPE)
                      .HasColumnName("VEHICLE_TYPE");

                entity.Property(e => e.VEHICLE_CATEGORY)
                      .HasColumnName("VEHICLE_CATEGORY");

                // DL Info
                entity.Property(e => e.DL_NUMBER)
                      .HasColumnName("DL_NUMBER")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.DL_VALID_TILL)
                      .HasColumnName("DL_VALID_TILL");

                entity.Property(e => e.DL_TYPE)
                      .HasColumnName("DL_TYPE");

                entity.Property(e => e.DL_PHOTOPATH)
                      .HasColumnName("DL_PHOTOPATH")
                      .HasMaxLength(200);

                //entity.Property(e => e.REMARK)
                //      .HasColumnName("REMARK")
                //      .HasMaxLength(500);

                // Audit
                entity.Property(e => e.CREATED_DATE)
                      .HasColumnName("CREATED_DATE")
                      .HasDefaultValueSql("SYSDATE");

                entity.Property(e => e.LOCATION_ID)
                      .HasColumnName("LOCATION_ID");

                entity.HasOne(e => e.LOCATION)
                      .WithMany()
                      .HasForeignKey(e => e.LOCATION_ID)
                      .HasConstraintName("FK_EMPVEHICLEMASTER_LOCATION");
            });

            modelBuilder.Entity<EMP_VEHICLE_DETAIL>(entity =>
            {
                entity.ToTable("EMP_VEHICLE_DETAIL");

                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                      .HasColumnName("ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.VEHICLENO)
                      .HasColumnName("VEHICLENO")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.FUELTYPE)
                      .HasColumnName("FUELTYPE");

                entity.Property(e => e.MODELYEAR)
                      .HasColumnName("MODELYEAR");

                entity.Property(e => e.OWNERTYPE)
                      .HasColumnName("OWNERTYPE");

                entity.Property(e => e.OWNERNAME)
                .HasColumnName("OWNERNAME")
                .HasMaxLength(100);

                entity.Property(e => e.VEHICLEID)
                      .HasColumnName("VEHICLEID")
                      .IsRequired();

                // RC
                entity.Property(e => e.RCNO)
                      .HasColumnName("RCNO")
                      .HasMaxLength(50);

                entity.Property(e => e.RCVALIDTILL)
                      .HasColumnName("RCVALIDTILL");

                entity.Property(e => e.RCPHOTOPATH)
                      .HasColumnName("RCPHOTOPATH")
                      .HasMaxLength(200);

                // Insurance
                entity.Property(e => e.INSURANCENO)
                      .HasColumnName("INSURANCENO")
                      .HasMaxLength(50);

                entity.Property(e => e.INSURANCEVALIDTILL)
                      .HasColumnName("INSURANCEVALIDTILL");

                entity.Property(e => e.INSURANCEPHOTOPATH)
                      .HasColumnName("INSURANCEPHOTOPATH")
                      .HasMaxLength(200);

                // PUC
                entity.Property(e => e.PUCNO)
                      .HasColumnName("PUCNO")
                      .HasMaxLength(50);

                entity.Property(e => e.PUCVALIDTILL)
                      .HasColumnName("PUCVALIDTILL");

                entity.Property(e => e.PUCPHOTOPATH)
                      .HasColumnName("PUCPHOTOPATH")
                      .HasMaxLength(200);

                // Audit
                entity.Property(e => e.UPLOADEDBY)
                      .HasColumnName("UPLOADEDBY")
                      .HasMaxLength(50);

                entity.Property(e => e.UPLOADEDON)
                      .HasColumnName("UPLOADEDON")
                      .HasDefaultValueSql("SYSDATE");

                entity.Property(e => e.VEHICLECUSTODIAN)
                      .HasColumnName("VEHICLECUSTODIAN")
                      .HasMaxLength(50);

                // Relation
                entity.HasOne(d => d.EMPVEHICLEMASTER)
                      .WithMany()
                      .HasForeignKey(d => d.VEHICLEID)
                      .HasConstraintName("FK_VEHDOC_VEH")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PARKING_ZONE>(entity =>
            {
                entity.ToTable("PARKING_ZONE");

                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                      .HasColumnName("ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.ZONE_NAME)
                      .HasColumnName("ZONE_NAME")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.ZONE_CODE)
                      .HasColumnName("ZONE_CODE")
                      .HasMaxLength(50);

                entity.Property(e => e.DESCRIPTION)
                     .HasColumnName("DESCRIPTION")
                     .HasMaxLength(500);

                entity.Property(e => e.TOTAL_CAPACITY)
                      .HasColumnName("TOTAL_CAPACITY");

                entity.Property(e => e.AVAILABLE_CAPACITY)
                    .HasColumnName("AVAILABLE_CAPACITY");

                entity.Property(e => e.LOCATION_ID)
                     .HasColumnName("LOCATION_ID");

                entity.Property(e => e.IS_ACTIVE)
                      .HasColumnName("IS_ACTIVE")
                      .HasConversion<int>()   
                      .HasDefaultValue(1);

                entity.Property(e => e.CREATED_AT)
                      .HasColumnName("CREATED_AT")
                      .HasDefaultValueSql("SYSDATE");

                entity.Property(e => e.UPDATED_AT)
                      .HasColumnName("UPDATED_AT");
            });

            modelBuilder.Entity<EMP_VEHICLE_HISTORY>(entity =>
            {
                entity.ToTable("EMP_VEHICLE_HISTORY");

                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                      .HasColumnName("ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.VEHICLEID)
                      .HasColumnName("VEHICLEID")
                      .IsRequired();

                // Foreign Key → EMPVEHICLEMASTER
                entity.HasOne(e => e.EMPVEHICLEMASTER)
                      .WithMany()
                      .HasForeignKey(e => e.VEHICLEID)
                      .HasConstraintName("FK_VEHICLE_HISTORY_MASTER")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.ACTIONBY)
                      .HasColumnName("ACTIONBY")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ACTIONBYROLE)
                      .HasColumnName("ACTIONBYROLE")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.ADMIN_STATUS)
                      .HasColumnName("ADMIN_STATUS");

                entity.Property(e => e.ADMINREMARK)
                       .HasColumnName("ADMINREMARK")
                       .HasMaxLength(500);

                entity.Property(e => e.SECURITY_STATUS)
                       .HasColumnName("SECURITY_STATUS");

                entity.Property(e => e.SPREMARK)
                       .HasColumnName("SPREMARK");

                entity.Property(e => e.SENDBACK_FOR)
                      .HasColumnName("SENDBACK_FOR");

                entity.Property(e => e.PARKINGZONEID)
                      .HasColumnName("PARKINGZONEID");

                // FK → ParkingZone
                entity.HasOne(e => e.PARKINGZONE)
                      .WithMany()
                      .HasForeignKey(e => e.PARKINGZONEID)
                      .HasConstraintName("FK_VEHICLE_HISTORY_PARKINGZONE");

                entity.Property(e => e.ACTIONDATE)
                      .HasColumnName("ACTIONDATE")
                      .HasDefaultValueSql("SYSDATE");
            });

            modelBuilder.Entity<ADMIN_LOCATION_MAPPING>(entity =>
            {
                entity.ToTable("ADMIN_LOCATION_MAPPING");

                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                      .HasColumnName("ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.SITE_ID)
                      .HasColumnName("SITE_ID")
                      .IsRequired();

                entity.Property(e => e.ADMIN_CODE)
                      .HasColumnName("ADMIN_CODE")
                      .IsRequired();

                entity.Property(e => e.IS_ACTIVE)
                      .HasColumnName("IS_ACTIVE")
                      .IsRequired();

                entity.Property(e => e.CREATED_DATE)
                      .HasColumnName("CREATED_DATE")
                      .HasDefaultValueSql("SYSDATE")
                      .IsRequired();

                entity.Property(e => e.CREATED_BY)
                      .HasColumnName("CREATED_BY");

                entity.Property(e => e.MODIFIED_DATE)
                      .HasColumnName("MODIFIED_DATE");
            });

            modelBuilder.Entity<SECURITY_LOCATION_MAPPING>(entity =>
            {
                entity.ToTable("SECURITY_LOCATION_MAPPING");

                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                      .HasColumnName("ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.SITE_ID).HasColumnName("SITE_ID");
                entity.Property(e => e.EMP_CODE).HasColumnName("EMP_CODE");
                entity.Property(e => e.IS_ACTIVE).HasColumnName("IS_ACTIVE");

                entity.Property(e => e.CREATED_DATE).HasColumnName("CREATED_DATE");
                entity.Property(e => e.MODIFIED_DATE).HasColumnName("MODIFIED_DATE");
            });

            // VEHICLE tables | CR7923 | End

            //Locker Management System

            modelBuilder.Entity<FLOORMASTER>(entity =>
            {
                entity.HasKey(e => e.FLOOR_ID).HasName("SYS_C00371080");

                entity.Property(e => e.FLOOR_ID)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.CREATED_DATE).HasColumnName("CREATED_DATE");
                entity.Property(e => e.FLOOR_NAME)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.STATUS).HasColumnType("NUMBER");
                entity.Property(e => e.SYSITEID);
            });

            modelBuilder.Entity<LOCKERASSIGNMENTMASTER>(entity =>
            {
                entity.HasKey(e => e.ASSIGN_ID).HasName("SYS_C00371086");

                entity.Property(e => e.ASSIGN_ID)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.APPROVED_BY);
                entity.Property(e => e.APPROVED_DATE).HasColumnType("DATE");
                entity.Property(e => e.ASSIGNED_BY);
                entity.Property(e => e.ASSIGNED_DATE).HasColumnType("DATE");
                entity.Property(e => e.BOX_ID)
                  ;
                entity.Property(e => e.CREATED_AT)
                    .HasDefaultValueSql("SYSDATE")
                    .HasColumnType("DATE");
                entity.Property(e => e.CREATED_BY);
                entity.Property(e => e.EMP_ID);
                entity.Property(e => e.FLOOR_ID);
                entity.Property(e => e.LOCKER_ID);
                entity.Property(e => e.MODIFIED_AT).HasColumnType("DATE");
                entity.Property(e => e.MODIFIED_BY);
                entity.Property(e => e.RELEASE_DATE).HasColumnType("DATE");
                entity.Property(e => e.REMARKS)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.REQUEST_DATE)
                    .HasDefaultValueSql("SYSDATE")
                    .HasColumnType("DATE");
                entity.Property(e => e.STATUS)
                    .HasColumnType("NUMBER");
                entity.Property(e => e.SYSITEID);

                entity.HasOne(d => d.BOX).WithMany(p => p.LOCKERASSIGNMENTMASTER)
                    .HasForeignKey(d => d.BOX_ID)
                    .HasConstraintName("FK_ASSIGN_BOX");
            });

            modelBuilder.Entity<LOCKERBOXMASTER>(entity =>
            {
                entity.HasKey(e => e.BOX_ID).HasName("SYS_C00371084");

                entity.Property(e => e.BOX_ID)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.BOX_NO)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.CREATED_DATE)
                    .HasDefaultValueSql("SYSDATE\n")
                    .HasColumnType("DATE");
                entity.Property(e => e.LOCKER_ID);
                entity.Property(e => e.STATUS).HasColumnType("NUMBER");

                entity.HasOne(d => d.LOCKER).WithMany(p => p.LOCKERBOXMASTER)
                    .HasForeignKey(d => d.LOCKER_ID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BOX_LOCKER");
            });

            modelBuilder.Entity<LOCKERMASTER>(entity =>
            {
                entity.HasKey(e => e.LOCKER_ID).HasName("SYS_C00371082");

                entity.Property(e => e.LOCKER_ID)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.CREATED_DATE)
                    .HasDefaultValueSql("SYSDATE\n")
                    .HasColumnType("DATE");
                entity.Property(e => e.FLOOR_ID);
                entity.Property(e => e.LOCKER_CODE)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.STATUS).HasColumnType("NUMBER");

                entity.HasOne(d => d.FLOOR).WithMany(p => p.LOCKERMASTER)
                    .HasForeignKey(d => d.FLOOR_ID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LOCKER_FLOOR");
            });

            modelBuilder.Entity<LOCKER_ADMIN_LOCATION_MAPPING>(entity =>
            {
                entity.HasKey(e => e.ID).HasName("SYS_C00371600");

                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                      .HasColumnName("ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.SITE_ID)
                      .HasColumnName("SITE_ID")
                      .IsRequired();

                entity.Property(e => e.ADMIN_CODE)
                      .HasColumnName("ADMIN_CODE")
                      .IsRequired();

                entity.Property(e => e.IS_ACTIVE)
                      .HasColumnName("IS_ACTIVE")
                      .IsRequired();

                entity.Property(e => e.CREATED_DATE)
                      .HasColumnName("CREATED_DATE")
                      .HasDefaultValueSql("SYSDATE")
                      .IsRequired();

                entity.Property(e => e.CREATED_BY)
                      .HasColumnName("CREATED_BY");

                entity.Property(e => e.MODIFIED_DATE)
                      .HasColumnName("MODIFIED_DATE");
            });

            modelBuilder.Entity<EmployeeLockerAllocation>(entity =>

            {

                entity.HasKey(e => e.AllocationId).HasName("SYS_C00347735");

                entity.ToTable("EMPLOYEE_LOCKER_ALLOCATION");

                entity.Property(e => e.AllocationId)

                    .ValueGeneratedOnAdd()

                    .HasColumnType("NUMBER")

                    .HasColumnName("ALLOCATION_ID");

                entity.Property(e => e.Active)

                    .HasColumnType("NUMBER")

                    .HasColumnName("ACTIVE");

                entity.Property(e => e.AssignedBy)

                    .HasColumnType("NUMBER")

                    .HasColumnName("ASSIGNED_BY");

                entity.Property(e => e.AssignedDate)

                    .ValueGeneratedOnAdd()

                    .HasColumnType("DATE")

                    .HasColumnName("ASSIGNED_DATE");

                entity.Property(e => e.CreatedAt)

                    .ValueGeneratedOnAdd()

                    .HasColumnType("DATE")

                    .HasColumnName("CREATED_AT");

                entity.Property(e => e.CreatedBy)

                    .HasColumnType("NUMBER")

                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.EmpId)

                    .HasColumnType("NUMBER")

                    .HasColumnName("EMP_ID");

                entity.Property(e => e.LockerBoxId)

                    .HasColumnType("NUMBER")

                    .HasColumnName("LOCKER_BOX_ID");

                entity.Property(e => e.ModifiedAt)

                    .HasColumnType("DATE")

                    .HasColumnName("MODIFIED_AT");

                entity.Property(e => e.ModifiedBy)

                    .HasColumnType("NUMBER")

                    .HasColumnName("MODIFIED_BY");

                entity.Property(e => e.Remarks)

                    .HasMaxLength(500)

                    .IsUnicode(false)

                    .HasColumnName("REMARKS");

                entity.Property(e => e.Status)

                    .HasMaxLength(20)

                    .IsUnicode(false)

                    .HasColumnName("STATUS");

            });


            base.OnModelCreating(modelBuilder);
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedure<T>(string procedureName, OracleParameter[] parameters) where T : class
        {
            // Create a cursor parameter for output
            var cursorParam = new OracleParameter("CUR_OUTPUT", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };

            // Combine parameters with the cursor parameter
            var allParameters = parameters.Append(cursorParam).ToArray();

            // Build the command string dynamically with uppercase procedure name
            var commandText = $"BEGIN {procedureName.ToUpper()}({string.Join(", ", parameters.Select(p => $":{p.ParameterName.ToUpper()}"))}, :CUR_OUTPUT); END;";

            // Execute the stored procedure
            var result = await Set<T>() // Using Set<T>() directly
                .FromSqlRaw(commandText, allParameters)
                .ToListAsync();

            return result; // The method now allows exceptions to propagate
        }

    }
}
