using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using Oracle.ManagedDataAccess.Client;
using ePortal.Persistence.Admin.Services;
using ePortal.ViewModels.APPX.Finance;
namespace ePortal.Persistence.Admin.Interface
{
   public interface ITaxDeclaration
    {


        DataTable GetEmpUsrPrdDtl(string strEmpcode);
        DataSet GETEXPORT_EMPINVDETAIL(string strStartdate, string strEnddate, string strKi);
        String UpdateEmpTaxDecOpendate(string strEmpcode, string strStartdate, string strEnddate, string strAddedby, string strKi, string strIseditable, string strIseditableactual, string str_ltacount);
        String UpdateTaxhead(string strTaxheadid, string strTaxhead, string strTaxabbr, string strDesc, string strMaxlimit, string strIsmandatory, string strXmlopt, string strActive, string strAddedby, string strPropsapdesc, string strActsapdesc, string strSapinfotype, string strSeqno, string strSapSubinfotype);
        void UpdateTaxheadLog(string strTaxheadid, string strTaxhead, string strTaxabbr, string strDesc, string strMaxlimit, string strIsmandatory, string strXmlopt, string strActive, string strAddedby, string strPropsapdesc, string strActsapdesc, string strSapinfotype, string strSeqno, string strSapSubinfotype, string strTransactionType);
        DataTable GetTaxheaddetail(string strTaxheadid, string strTaxhead, string strActive, string strSapinfotype, string strKI);
        DataTable GetTaxhead_Optdetail(string strTaxheadoptid, string strTaxheadid, string strActive);
        DataTable GetEmpTax_Invdetail(string strTaxheadid, string strEmpcode, string strActive, string strKi, string strSapinfotype);
        DataTable GetEmpRent_Invdetail(string strEmpcode, string strKi);
        DataTable GetEmp584_Invdetail(string strTaxheadid, string strEmpcode, string strActive, string strKi, string strSapinfotype);

        DataTable GetEmpUserPeriodDetail(string strEmpcode, string strActive, string strKi);
        //void UpdateEmpInvDetail(string strEmpcode, string strKi, string strRentfrom, string strRentto, string strAcctype, string strCitycat, string strIsexemp, string strhraclaim, string strRentamt, string strTaddress, string strTmobile, List<Rent_Detail> objRent, string[,] strInv584detail, string[,] strInvdetail, string strActive, string strIsdraft, ArrayList arr_586, string grd586, ArrayList arr_585, string grd585, ReceiptDtl_LTAMain objlta, ReceiptDtl_12BMain obj12BMain, LoanDetail objloan, Emp80EE_Detail objEmp80EE, ArrayList HLLIST, PLoanDetail objploan, ArrayList objltcList, ArrayList objltcmList,string UserId);

        public void Update586Receiptdtl(List<ReceiptIndexViewModel> arr586, string strKIID, string strEmpcode, OracleCommand objCmd, string hdn_taxheadid, string UserId);


        //void Update586Receiptdtl(ArrayList arr586, string strKIID, string strEmpcode, OracleCommand objCmd, string grd586,string UserId);

        //Update LTA Exemption Detail
        void UpdateReceipt_LTADetail(ReceiptDtl_LTAMain obj, string strKIID, string strEmpcode, OracleCommand objCmd,string UserId);
        void Update_12BDetail(EmployerB12ViewModel obj, string strKIID, string strEmpcode, OracleCommand objCmd,string UserId);
        void Update_LoanDetail(LoanApplicationViewModel obj, string strKIID, string strEmpcode, OracleCommand objCmd);
        void Update_80EEDetail(EEDetailsModels obj, string strKIID, string strEmpcode, OracleCommand objCmd);
        void UpdateHLListDetail(LoanApplicationViewModel HLLIST, OracleCommand objCmd, string strEmpcode);
        //-----------Alok-----------------
        DataTable Gettaxstatus(string strEmpcode, string strstatus, string strActstatus, string strki);
        DataTable Get586_585ReceiptDetail(string strEmpcode, string strTaxheadid, string strki, string strStatus);
        DataTable Get586_585ReceiptDetail_V1(string strEmpcode, string strTaxheadid, string strki, string strStatus);
        DataSet GetLTADtl(string strEmpcode, string strki, string strStatus);
        DataTable Get12BDtl(string strEmpcode, string strki, string strStatus);
        DataTable GetFINANCIALYEAR();
        DataTable GetLOANDTL(string strEmpcode, string strki);

        DataTable Get80EEDTL(string strEmpcode, string strki);
        DataTable Get80EEADTL(string strEmpcode, string strki);
        DataTable GetPreviousYearDtl(string strEmpcode);
        #region  "Print Report"
        DataTable Get585_586RecDtl(string strEmpcode, string strki, string strStatus, string strSapinfotype, string strTaxheadid);
        // New Print Report
        DataTable Get585_586RecDtl_V1(string strEmpcode, string strki, string strStatus, string strSapinfotype, string strTaxheadid);
        #endregion

        String CopyTaxHead(string strAddedby);
        DataTable GetCurrKIStatus(string strEmpcode);
        DataTable GetTrainFareEligPerAge(string gender, string age, string normaltrainfare);
        DataTable GetEmpRent_Invdetail_N(string strEmpcode, string strKi);

        DataTable GetHOUSELOANDTL(string FIN_TAXLOANDTLID);
        DataSet GET_HOUSE_LOAN_DETAIL(string strStartdate, string strEnddate, string strKi);
        // Add below EportalDal/Admin/TaxDeclaration.cs 



        DataTable GETHRA_DETAIL(string strstartdate, string strenddate, string strEmpCodeFrom, string strEmpcodeTo, string strKi);
        DataTable GETHRA_DETAILTXT(string strstartdate, string strenddate, string strEmpCodeFrom, string strEmpcodeTo, string strKi);

        DataTable GetLOANDTL_proj(string strEmpcode, string strki);

        //Update Loan  Detail
        void Update_LoanDetail_PROJ(LoanApplicationViewModel obj, string strKIID, string strEmpcode, OracleCommand objCmd);
        DataSet GetLTCDtl(string strEmpcode, string strki, string strStatus);
        //Update LTA Exemption Detail
        void Update_LTCDetail(ArrayList objltcList, string strKIID, string strEmpcode, OracleCommand objCmd);
        void Update_LTCFMDetail(ArrayList objltcFmList, string strKIID, string strEmpcode, OracleCommand objCmd);
        public void UpdateTaxEmpInvDetail(string strEmpcode, string strKi, string strRentfrom, string strRentto, string strAcctype, string strCitycat, string strIsexemp, string strhraclaim, string strRentamt, string strTaddress, string strTmobile, List<HRADetails> objRent, string[,] strInv584detail, string[,] strInvdetail, string strActive, string strIsdraft, List<ReceiptIndexViewModel> arr_586, ReceiptDtl_LTAMain objlta, EmployerB12ViewModel obj12BMain, LoanApplicationViewModel objloan, EEDetailsModels objEmp80EE, LoanApplicationViewModel HLLIST, LoanApplicationViewModel objploan, string UserId,string Type);




        public void Update_TaxPeriod(string strtaxregime, string strEmpcode, string KII);
        public void Update_TaxFinalPeriod(string strtaxregime, string strEmpcode, string KII);
        public void Update_TaxPeriodProjected(string strtaxregime, string strEmpcode, string KII);
        public void Update_TaxFinalPeriodProjected(string strtaxregime, string strEmpcode, string KII);

    }
}


