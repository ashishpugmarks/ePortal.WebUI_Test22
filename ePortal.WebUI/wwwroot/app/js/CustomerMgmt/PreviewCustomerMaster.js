$(document).ready(function () {  
    $('a').on('click', function (event) {
        event.preventDefault();
        /* bindLink($(this), $(this).text()); */
        let urlpath = '../../Uploads/CustomerMaster/' + $(this).text();
        openDialog(urlpath, 980, 680);
    });

});
function setText(id, val) {
    $(id).text(val == null ? '' : String(val));
}
function openDialog(url, w, h) { window.open(url, "_blank", `width=${w},height=${h},resizable=yes,scrollbars=yes`); }
function bindLink(id, fileName) {
    const $a = $(id);
    const file = (fileName || '').trim();
    if (file.length > 0) {
        $a.attr('href', '../../Uploads/CustomerMaster/' + file)
            .attr('target', '_blank')
            //.text(file) // optional: show file name as link text
            //.show();
    } else {
       // $a.hide().attr('href', '').attr('target', '').text('');
    }
}

function bindCustomerMasterDetail(m) {
    if (!m) return;
    // Header
    setText('#lblrequesttype', m.REQUEST_TYPE);
    setText('#lblaccountgroup', m.CUST_ACC_TYPE);
    setText('#lbldistribution', m.DISTRIBTUION_CHH);
    setText('#lbldivisions', m.DIVISION_GRP);
    setText('#lblsalesorganization', m.SALES_ORG);
    setText('#lblcompanycode', m.COMPANY_CODE);
    // General
    setText('#lblcode', m.CUSTOMER_CODE);
    setText('#lblTitle', m.TITLE);
    setText('#lblName1', m.NAME1);
    setText('#lblName2', m.NAME2);
    setText('#lblName3', m.NAME3);
    setText('#lblName4', m.NAME4);
    setText('#lblSearchTerms', m.SEARCHTERM);
    setText('#lblStreetHouse', m.STREETHOUSENUMBER);
    setText('#lblStreet2', m.STREET2);
    setText('#lblStreet3', m.STREET3);
    setText('#lblStreet4', m.STREET4);
    setText('#lblStreet5', m.STREET5);
    setText('#lblPostalCode', m.POSTALCODE);
    setText('#lblCity', m.CITY);
    setText('#lblCountry', m.COUNTRY);
    setText('#lblRegion', m.CREGION);
    setText('#lblTimeZone', m.CTIMEZONE);
    setText('#lblTransportationCode', m.TRANSPORTATIONCODE);
    setText('#lblMobilePhone', m.MOBILEPHONE);
    setText('#lblFax', m.FAX);
    setText('#lblEmail', m.EMAIL);
    setText('#lblIndustry', m.INDUSTRY);
    setText('#lblTaxNumberGst', m.TAXNUMBER3);
    setText('#lblCityCode', m.CITYCODE);
    setText('#lblCustomerClass', m.CUSTOMERCLASS);
    setText('#lblVendorCode', m.VENDORNO);
    // Bank
    setText('#lblCtry', m.CTRY);
    setText('#lblBankKey', m.BANKKEY);
    setText('#lblBankAccount', m.BANKACCOUNT);
    setText('#lblAccountHolder', m.ACCOUNTHOLDER);
    setText('#lblBankControlKey', m.BANKCONTROLKEY);
    setText('#lblBankName', m.BANKNAME);
    setText('#lblBankRegion', m.BANKREGION);
    setText('#lblBankStreet', m.BANKSTREET);
    setText('#lblBankCity', m.BANKCITY);
    setText('#lblBankBranch', m.BANKBRANCH);
    setText('#lblBankCurrency', m.BANKCURRENCY);

    // Sales Area
    setText('#lblSalesCurrency', m.SALESCURRENCY);
    setText('#lblPaymentMethods', m.PAYMENTMETHODS);
    setText('#lblHouseBank', m.HOUSEBANK);
    setText('#lblPaymentMethodSupplier', m.PMTMETHSUPL);
    setText('#lblWithHoldingTaxType', m.WITHHOLDINGTAXTYPE);
    setText('#lblWithHoldingTaxCode', m.WITHHOLDINGTAXCODE);
    setText('#lblValidFrom', m.VALIDFROM);
    setText('#lblValidTo', m.VALIDTO);
    setText('#lblWithHoldingTaxNumber', m.WITHHOLDINGTAXNUMBER);
    setText('#lblIncoterms', m.INCOTERM1);
    setText('#lblCreditControlArea', m.CREDITCONTROLAREA);
    setText('#lblAccountAssignmentGroup', m.ACCASSIGMENTGROUP);
    setText('#lblTaxClassification', m.TAXCLASSIFICATION);
    setText('#lblExchangeRateType', m.EXCHANGERATETYPE);
    setText('#lblSalesDistrict', m.SALESDISTRICT);
    setText('#lblSalesOffice', m.SALESOFFICE);
    setText('#lblSalesGroup', m.SALESGROUP);
    setText('#lblCustomerGroup', m.CUSTOMERGROUP);
    setText('#lblPriceGroup', m.PRICEGROUP);
    setText('#lblCustomerPrice', m.CUSTPRICPROC1);
    setText('#lblDeliveryPriority', m.DELIVERYPRIORITYGROUP);
    setText('#lblShippingConditions', m.SHIPPINGCONDITION);
    // Industry details
    setText('#lblIndustry1', m.INDUSTRY1);
    setText('#lblIndustryCode1', m.INDUSTRYCODE1);
    setText('#lblIndustryName', m.INDUSTRYNAME);
    setText('#lblFirstName', m.FIRSTNAME);
    // CIN
    setText('#lblCstno', m.CSTNO);
    setText('#lblLstNo', m.LSTNO);
    setText('#lblInvoicingDates', m.INVOICINGDATES);
    setText('#lblInvoicingListDates', m.INVOICINGLISTDATES);
    setText('#lblServiceRegNo', m.SERREGNO);
    setText('#lblPanNo', m.PANNUMBER);
    setText('#lblPaymentGuarProc', m.PAYMENTGUARANTEEPROC);
    setText('#lblEInvoice', m.E_INVOICE_APPLICABLE);

    // Requester & remarks
    setText('#lblRequesterID', m.REQUESTEDID);
    setText('#lblRequestedDate', m.REQUESTED_DATE);
    setText('#lblremark', m.REMARKS);

    // Attachments (hyperlinks)
    bindLink('#hyperBankMandate', m.BANK_MANDATE_FILE);
    bindLink('#hyperCancelCheque', m.BANK_CANCELED_FILE);
    bindLink('#hyperPAN', m.CIN_PAN_NO_FILE);
    bindLink('#hyperGSTNo', m.CIN_GST_NO_FILE);
    bindLink('#hyperRTO', m.COMP_RTO_FILE);
    bindLink('#hyperLOI', m.COMP_LOI_FILE);
    bindLink('#hyperEInvoice', m.CIN_EINVOICE_DOC_FILE);
    bindLink('#hyperOtherDoc', m.OTHER_DOC_FILE);    
}

