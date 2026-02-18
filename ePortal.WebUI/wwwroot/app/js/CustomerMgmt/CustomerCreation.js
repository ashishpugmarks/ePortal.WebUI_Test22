///////////////Global Variable declaration ///////////////////////
var withholdingList = [];
var groupFlags = {
    Gen_data: false,
    Bank_Data: false,
    Ind_data: false,
    Cin_data: false,
    Sales_Data: false,
    Comp_Data: false
};
$(document).ready(function () {    
    $('#btnTab1').click(function () {        
        btnTab_Click($(this).attr('id'));
    });
    $('#btnTab2').click(function () {
        btnTab_Click($(this).attr('id'));
    });
    $('#btnTab3').click(function () {
        btnTab_Click($(this).attr('id'));
    });
    $('#btnTab4').click(function () {
        btnTab_Click($(this).attr('id'));
    });
    $('#btnTab5').click(function () {
        btnTab_Click($(this).attr('id'));
    });
    $('#btnTab6').click(function () {
        btnTab_Click($(this).attr('id'));
    });
    btnTab_Click($('#btnTab1').attr('id'));    
    
    $('#rdCreateRequst').on('change', function () {
        showExtendUpdatePanel($(this));       
    });
    $('#rdUpdateRequst').on('change', function () {
        showExtendUpdatePanel($(this));
    });

    $(document).on('change', 'input[name="RequestType"]', function () {
        const selected = $('input[name="RequestType"]:checked').val();
        showExtendDomestic(selected);
    });
    $("#btnAddWithHolding").click(function () {
        btnAddWithHolding();
    });
   
    $('.btnDeleteRow').on("click", function () {
            var index = $(this).data("index");
            withholdingList.splice(index, 1);
            renderGrid();
            updateHiddenFields(); 
    });
        
    $('#ddlCountry').on("change", function () {
        ChangeCountry($(this));
    });
   
    $(document).on('change', 'input[name="CustomerAccountGroup"]', function () {
        const selected = $('input[name="CustomerAccountGroup"]:checked').val();
        onChk5AGChanged($(this));
    });
    initGroupOfData();
    getDetailDraft();
    $("#ddlSalesOffice").on("click", function () {
        BindSalesGroup();
    });
    $('#btnSaveGenData').on("click", function (e) {
        e.preventDefault();
        saveGeneralData();
    });
    $('#btnSaveBankData').on('click', function (e) {
        e.preventDefault();
        saveBankData();
    });
    $('#btnSaveIndustryData').on('click', function (e) {
        e.preventDefault();
        SaveIndustryData();
    });
    $('#btnSaveSaleData').on('click', function (e) {
        e.preventDefault();
        SaveSalesData();
    });
    $('#btnSaveCINData').on('click', function (e) {
        e.preventDefault();
        SaveCINData();
    });
    $('#btnSaveCompData').on('click', function (e) {
        e.preventDefault();
        SaveCompanyData();
    });
    $('#ddlEInvoiceApplicable').on('change', function () {
        if ($(this).val() === 'N') {
            $('#TRCINDoc').css('display', '');
        } else {
            $('#TRCINDoc').css('display', 'none');
        }
    });
    $('#ddlTransportationCode').on('change', function () {
        $('#ddlCityCode').val($(this).val());       
    });
    $('#btnSubmit').on('click', function () {
        finalsubmit($(this));
    });
    $('#btnPreview').on('click', function () {        
        openDialog('PreviewCustomerMaster', this);
    });
    $('#btnCancel').on('click', function () {
        finalsubmit($(this));
    });

    $('#txtEmail').on('change', function () {
        const val = $(this).val().trim();
        if (!val) {           
            return;
        }
        if (!isValidEmail(val)) {
            sweetAlert("Customer Master Request", "Invalid email id " + val, "warning");
            $(this).val("");
        }
    });
    $('#txtVendorCode').on('change', function () {
        checkVendorCode($(this));
    });
    $('#txtDealerCode').on('change', function () {
        checkDealerCode($(this));
    });
    $('#btnSearch').on('click', function () {
        SearchDealerCode($(this));
    });
    $('#btnVerifyBank').on('click', function () {
        verifyBankData($(this));
    });
    $('#btnReset,#btnReset2,#btnReset1,#btnReset7,#btnReset4,#btnReset6').on('click', function (e) {
        e.preventDefault();       
        ResetData($(this));
    });
    $(document).on('change', 'input[name="ResetData"]', function () {
        e.preventDefault();
        ResetData($(this));
    });
    
    $('#ddlRegion').on('change', function () {
        //ddlBankRegion.SelectedValue = ddlRegion.SelectedValue;
        $('#ddlBankRegion').val($(this).val());
    });

    $('#txtValidFromIn').datepicker({
        format: 'dd.mm.yyyy',   // Date format
        todayHighlight: true,    // Highlight today's date
        autoclose: true,         // Close the datepicker after selection
        clearBtn: true,          // Adds a "Clear" button
        orientation: 'top auto'  // Position the calendar below the input
    });
    $('#txtValidToIn').datepicker({
        format: 'dd.mm.yyyy',   // Date format
        todayHighlight: true,    // Highlight today's date
        autoclose: true,         // Close the datepicker after selection
        clearBtn: true,          // Adds a "Clear" button
        orientation: 'top auto'  // Position the calendar below the input
    });

    $('#ImgBtnFromDate').click(function () {
        $('#txtValidFromIn').datepicker('show');
    });
    $('#ImgBtnToDate').click(function () {
        $('#txtValidToIn').datepicker('show');
    });
    $("#grdvWithHoldingData tbody").on('click', "[data-ctrl-id='btnDeleteRow']", function () {
        let index = parseInt($(this).attr('data-index'));
        btnDeleteWithHolding(index);
    });
    $('a').on('click', function (event) {
        event.preventDefault();       
        let urlpath = '../../Uploads/CustomerMaster/' + $(this).text();
        openDialog(urlpath, 980, 680);
    });
});
function confirmMessage() {
    var confirmation = confirm("Are you sure you want to submit?");
    return confirmation;
}
function btnTab_Click(clickedButtonId) {
   
    $('.tab-body').hide();  
   // $('.tab-button').removeClass('active').addClass('tabs');
    $('#btnTab1').removeClass('active').addClass('tabs');
    $('#btnTab2').removeClass('active').addClass('tabs');
    $('#btnTab3').removeClass('active').addClass('tabs');
    $('#btnTab4').removeClass('active').addClass('tabs');
    $('#btnTab5').removeClass('active').addClass('tabs');
    $('#btnTab6').removeClass('active').addClass('tabs');
   
    if (clickedButtonId === 'btnTab1') {
        $('#tab_body_1').show();
        /*$('#btnTab1').removeClass('tabs').addClass('active');*/
        $('#btnTab1').addClass('tabs active');
    }
    if (clickedButtonId === 'btnTab2') {
        $('#tab_body_2').show();
        $('#btnTab2').addClass('tabs active');
    }
    if (clickedButtonId === 'btnTab3') {
        $('#tab_body_3').show();
        $('#btnTab3').addClass('tabs active');
    }
    if (clickedButtonId === 'btnTab4') {
        $('#tab_body_4').show();
        $('#btnTab4').addClass('tabs active');
    }
    if (clickedButtonId === 'btnTab5') {
        $('#tab_body_5').show();
        $('#btnTab5').addClass('tabs active');
    }
    if (clickedButtonId === 'btnTab6') {
        $('#tab_body_6').show();
        $('#btnTab6').addClass('tabs active');
    }
}
function showExtendUpdatePanel(id) {
    if ($(id).val() == "U") {
        $('#lblCreationCustID').show();
        $('#txtSearchCustomerID').show();
        $('#btnSearch').show();
    }
    else {
        $('#lblCreationCustID').hide();
        $('#txtSearchCustomerID').hide();
        $('#btnSearch').hide();
    }
}
function showExtendDomestic(selected) {

    if (selected == 'DM') {
        $('#dom_options1, #dom_options2').css('display', 'table-row');
        $('#exp_options').css('display', 'none');
        $('#ddlDistribution').val('DM');
    } else if (selected == 'EX') {
        $('#dom_options1, #dom_options2').css('display', 'none');
        $('#exp_options').css('display', 'table-row');
        $('#ddlDistribution').val('EX');
    }

}
function ShowMessage(messagetype, message) {
    if (messagetype == "error") {
        sweetAlert("Customer Master", message, "warning");
        $("#errorPanel").show();
        $("#infoPanel").hide();
        $("#errorMsg").text(message);
        $("#infoMsg").text("");

    }
    else if (messagetype == "success") {
        sweetAlert("Customer Master", message, "success");
        $("#errorPanel").hide();
        $("#infoPanel").show();
        $("#infoMsg").text(message);
        $("#errorMsg").text("");
    }
    else {
        $("#errorPanel, #infoPanel").hide();
        $("#errorMsg, #infoMsg").text("");
    }

}
function showError(msg) {
    sweetAlert("Customer Master", msg, "warning");
    $("#errorPanel").show();
    $("#infoPanel").hide();
    $("#errorMsg").text(msg);
    $("#infoMsg").text("");   
}
function btnAddWithHolding()
{
        // Validation
        if ($("#ddlWithHoldingTaxCodeIn").prop('selectedIndex') <= 0) {
            alert("Please select Holding Tax Code.");
            return;
        }
        if ($("#ddlWithHoldingTaxTypeIn").prop('selectedIndex') <= 0) {
            alert("Please select Holding Tax Type.");
            return;
        }
        if ($.trim($("#txtValidFromIn").val()).length <= 0) {
            alert("Please enter valid from date.");
            return;
        }
        if ($.trim($("#txtWithHoldingTaxNo1").val()).length <= 0) {
            alert("Please enter Holding Tax No.");
            return;
        }

        // Add new record to array
        withholdingList.push({
            withholdingtaxtype: $("#ddlWithHoldingTaxTypeIn").val(),
            withholdingtaxcode: $("#ddlWithHoldingTaxCodeIn").val(),
            validfrom: $("#txtValidFromIn").val(),
            validto: $("#txtValidToIn").val(),
            withholdingtaxno: $("#txtWithHoldingTaxNo1").val().toUpperCase()
        });

        // Update table/grid
        renderGrid();

        // Build concatenated strings (similar to C# loop)
        var allTypes = [];
        var allCodes = [];
        var allFromDates = [];
        var allToDates = [];
        var allTaxNos = [];

        $.each(withholdingList, function (i, item) {
            allTypes.push(item.withholdingtaxtype);
            allCodes.push(item.withholdingtaxcode);
            allFromDates.push(item.validfrom);
            allToDates.push(item.validto);
            allTaxNos.push(item.withholdingtaxno);
        });

        //Assign to hidden fields (equivalent of your C# textboxes)
        $("#txtWithHoldingTaxType").val(allTypes.join("|"));
        $("#txtWithHoldingTaxCode").val(allCodes.join("|"));
        $("#txtValidFrom").val(allFromDates.join("|"));
        $("#txtValidTo").val(allToDates.join("|"));
        $("#txtWithHoldingTaxNumber").val(allTaxNos.join("|"));

        // Reset fields
        $("#ddlWithHoldingTaxTypeIn").prop("selectedIndex", 0);
        $("#ddlWithHoldingTaxCodeIn").prop("selectedIndex", 0);
        $("#txtValidFromIn").val("");
        $("#txtWithHoldingTaxNo1").val(""); 

}
function renderGrid() {
    var tbody = $("#grdvWithHoldingData tbody");
    tbody.empty();

    $.each(withholdingList, function (i, item) {
        tbody.append(
            "<tr>" +
            "<td>" + (i+1) + "</td>" +
            "<td>" + item.withholdingtaxtype + "</td>" +
            "<td>" + item.withholdingtaxcode + "</td>" +
            "<td>" + item.validfrom + "</td>" +
            "<td>" + item.validto + "</td>" +
            "<td>" + item.withholdingtaxno + "</td>" +
            /*"<td><button class='btnDeleteRow' data-index='" + i + "'>Delete</button></td>" +*/
            "<td><button data-ctrl-id='btnDeleteRow' data-index='" + i + "' type='button' class='btnDeleteRow'>Delete</button></td>"+
            "</tr>"
        );
    });
}
function updateWithHolding() {

    var allTypes = [];
    var allCodes = [];
    var allFromDates = [];
    var allToDates = [];
    var allTaxNos = [];

    $.each(withholdingList, function (i, item) {
        allTypes.push(item.withholdingtaxtype);
        allCodes.push(item.withholdingtaxcode);
        allFromDates.push(item.validfrom);
        allToDates.push(item.validto);
        allTaxNos.push(item.withholdingtaxno);
    });

    $("#txtWithHoldingTaxType").val(allTypes.join("|"));
    $("#txtWithHoldingTaxCode").val(allCodes.join("|"));
    $("#txtValidFrom").val(allFromDates.join("|"));
    $("#txtValidTo").val(allToDates.join("|"));
    $("#txtWithHoldingTaxNumber").val(allTaxNos.join("|"));
}
function btnDeleteWithHolding(index) {    // Validation
   


    if (typeof index !== 'number' || index < 0 || index >= withholdingList.length) {
        alert("Invalid record index.");
        return;
    }
    // Remove the item
    withholdingList.splice(index, 1);  

    // Update table/grid
    renderGrid();

    // Build concatenated strings (similar to C# loop)
    var allTypes = [];
    var allCodes = [];
    var allFromDates = [];
    var allToDates = [];
    var allTaxNos = [];

    $.each(withholdingList, function (i, item) {
        allTypes.push(item.withholdingtaxtype);
        allCodes.push(item.withholdingtaxcode);
        allFromDates.push(item.validfrom);
        allToDates.push(item.validto);
        allTaxNos.push(item.withholdingtaxno);
    });

    //Assign to hidden fields (equivalent of your C# textboxes)
    $("#txtWithHoldingTaxType").val(allTypes.join("|"));
    $("#txtWithHoldingTaxCode").val(allCodes.join("|"));
    $("#txtValidFrom").val(allFromDates.join("|"));
    $("#txtValidTo").val(allToDates.join("|"));
    $("#txtWithHoldingTaxNumber").val(allTaxNos.join("|"));

    // Reset fields
    $("#ddlWithHoldingTaxTypeIn").prop("selectedIndex", 0);
    $("#ddlWithHoldingTaxCodeIn").prop("selectedIndex", 0);
    $("#txtValidFromIn").val("");
    $("#txtWithHoldingTaxNo1").val("");

}
function BinWithHoldingTax()
{
   
        var s = ['|'];;
    var withholdingtaxtype = [],
        withholdingtaxcode = [],
        withholdingfromdates = [],
        withholdingtodates = [],
        withholdingtaxno = [];
    
    withholdingtaxtype = $("#txtWithHoldingTaxType").val().split(s);
    withholdingtaxcode = $("#txtWithHoldingTaxCode").val().split(s);
    withholdingfromdates = $("#txtValidFrom").val().split(s);
    withholdingtodates = $("#txtValidTo").val().split(s);
    withholdingtaxno = $("#txtWithHoldingTaxNumber").val().split(s); 

    for (var i = 0; i < withholdingtaxtype.length; i++) {
        withholdingList.push({
            withholdingtaxtype: withholdingtaxtype[i],
            withholdingtaxcode: withholdingtaxcode[i],
            validfrom: withholdingfromdates[i],
            validto: withholdingtodates[i],
            withholdingtaxno: withholdingtaxno[i].toUpperCase()  
        });
    }
    
        renderGrid();
       

    }
function ChangeCountry(id) {
    if ($(id).val() == "") {
          return;
    }
    const selected = document.querySelector('input[name="RequestType"]:checked');
    if ($(selected).val() == "") {
        return;
    }
    var data = {
        CODE: $(selected).val(),
        CODE_DESC: $(id).val()
    }
    $('#ddlCtry').val($(id).val());
    $.ajax({
        type: "POST",
        url: "/CustomerMgmt/ChangeCountry",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data == "error") {
                $("#ddlTransportationCode").html("");
                ShowMessage("error", "Error in Databinding")
                sweetAlert("Customer Master", "Error in Databinding", "error");
            }
            else {
                var dt = '<option value="">-- Select --</option>';
                $("#ddlTransportationCode").html("");
                $.each(data, function (i, item) {
                    dt += '<option value="' + item.CODE + '">' + item.CODE_DESC + '</option>'; 
                });
                $("#ddlTransportationCode").html(dt);
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function checkVendorCode(id) {
    if ($(id).val() == "") {
        return;
    }
   
    var data = {
       CODE: $(id).val()
    }
    
    $.ajax({
        type: "POST",
        url: "/CustomerMgmt/ValidateVendorCode",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data.error == "error") {
                sweetAlert("Customer Master", data.message, "error");
            }
            else if (data.error == "E") {
                sweetAlert("Customer Master", data.message, "warning");
            }
            else {
                //sweetAlert("Customer Master", data.MESSAGE, "warning");
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function checkDealerCode(id) {
   
    const selected = document.querySelector('input[name="RequestType"]:checked');
    if ($(id).val() == "" || $(selected).val()=="U") {
        return;
    }
    var data = {
        CODE: $(id).val(),
        CODE_DESC: $(selected).val()
    }

    $.ajax({
        type: "POST",
        url: "/CustomerMgmt/ValidateDealerCode",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data.error == "error") {
                sweetAlert("Customer Master", data.message, "error");
            }
            else if (data.error == "N") {
                sweetAlert("Customer Master", "Dealer code" + data.message + " already exist!", "warning");
                $(id).val("");
            }
            else {
                //sweetAlert("Customer Master", data.MESSAGE, "warning");
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function setAccountData(accountType) {
    try {
        var map = {
            '5AG1': 'chk5AG1',
            '5AG2': 'chk5AG2',
            '5AG3': 'chk5AG3',
            '5AG5': 'chk5AG5',
            '5AG6': 'chk5AG6',
            '5AGG': 'chk5AGG',
            '5AGV': 'chk5AGV',
            '5AGW': 'chk5AGW'
        };

       
        for (var key in map) {
            if (!map.hasOwnProperty(key)) continue;
            var id = map[key];
            var $el = $('#' + id);
            if ($el.length) {
                $el.prop('checked', false);
            }
        }
        if (accountType == '') { return; }
        // Check the target input and trigger change
        var targetId = map[accountType];
        if (targetId) {
            var $target = $('#' + targetId);
            if ($target.length) {
                $target.prop('checked', true);
                if (accountType == '5AG1' || accountType =='chk5AG6') {
                    $('#chk5AGW').prop('checked', true);
                }
            } else {
                console.warn('setAccountData: target element not found:', targetId);
            }
        } else {
            console.warn('setAccountData: unknown accountType:', accountType);
        }

    } catch (ex) {
        console.error('Error in account binding:', ex);
        alert('Error in account binding');
    }
}
function setDivisionGrpByValue(divGrp) {
    /*var codes = (divGrp || "").split("|").map(function (s) { return s.trim(); }).filter(Boolean);*/
    var codes = (divGrp || "").split("|");
    if (!codes.length) return;

    //$("#chklDivision input[type=checkbox]").each(function () {
    //    var val = ($(this).val() || "").trim();
    //    $(this).prop("checked", codes.indexOf(val) !== -1);
    //});
    $.each(codes, function (index, item) {
        const chk = '#chk_' + item;
        $(chk).prop("checked", true).trigger("change");
    });
}
function isValidEmail(email) {    
    email = String(email || '').trim();
    // Basic pattern: local@domain.tld, allows subdomains and + tags
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/;
    return re.test(email);
}
function getAccountData() {
    let account_type = '';
    let ac_55AGW = '';

    const map = [
        { code: '5AG1', sel: '#chk5AG1' },
        { code: '5AG2', sel: '#chk5AG2' },
        { code: '5AG3', sel: '#chk5AG3' },
        { code: '5AG5', sel: '#chk5AG5' },
        { code: '5AG6', sel: '#chk5AG6' },
        { code: '5AGG', sel: '#chk5AGG' },
        { code: '5AGV', sel: '#chk5AGV' }
    ];


    map.forEach(item => {
        if ($(item.sel).prop('checked')) {
            account_type = item.code;
        }
    });

    const is5AG1 = $('#chk5AG1').prop('checked');
    const is5AG6 = $('#chk5AG6').prop('checked');
    const is5AGW = $('#chk5AGW').prop('checked');

    if (is5AGW && (is5AG1 || is5AG6)) {
        ac_55AGW = '5AGW';
    } else if (is5AGW) {
        account_type = '5AGW';
    }
    return { account_type, ac_55AGW };
}
//function fillAccountFields() {
    //    const { account_type, ac_55AGW } = getAccountData();
    //    $('#account_type').val(account_type);   // hidden input
    //    $('#ac_55AGW').val(ac_55AGW);           // hidden input
//}




//////////////////Call Account Change Event//////////////////////////
function onChk5AGChanged(id) {
   
    
    let ag5AG1 = false, ag5AG3 = false, ag5AGV = false, ag5AGG = false,
        ag5AG6 = false, ag5AGW = false, ag5AG2 = false, ag5AG5 = false;   
   
    if ($('#chk5AG1').val()==$(id).val()) {
        ag5AG1 = true;
        ag5AGW = true; // special cascade
    }
    if ($('#chk5AG2').val() == $(id).val()) { ag5AG2 = true; }
    if ($('#chk5AG3').val() == $(id).val()) { ag5AG3 = true; }
    if ($('#chk5AG5').val() == $(id).val()) { ag5AG5 = true; }
    if ($('#chk5AG6').val() == $(id).val()) {
        ag5AG6 = true;
        ag5AGW = true; // special cascade
    }
    if ($('#chk5AGG').val() == $(id).val()) { ag5AGG = true; }
    if ($('#chk5AGV').val() == $(id).val()) { ag5AGV = true; }
    if ($('#chk5AGW').val() == $(id).val()) { ag5AGW = true; }

    // Now apply the resulting flags back to the checkboxes (server sets Checked = flag)
    $('#chk5AG1').prop('checked', ag5AG1);
    $('#chk5AG2').prop('checked', ag5AG2);
    $('#chk5AG3').prop('checked', ag5AG3);
    $('#chk5AG5').prop('checked', ag5AG5);
    $('#chk5AG6').prop('checked', ag5AG6);
    $('#chk5AGG').prop('checked', ag5AGG);
    $('#chk5AGV').prop('checked', ag5AGV);
    $('#chk5AGW').prop('checked', ag5AGW);
        setMandatoryField();    
        checkAllGroupOfData();
    }

///////////////Set Mandatory Field/////////////////////////////////
function setMandatoryField() {
    try {
       
        var ag5AG1 = $('#chk5AG1').prop('checked') ? '1' : '';
        var ag5AG2 = $('#chk5AG2').prop('checked') ? '1' : '';
        var ag5AG3 = $('#chk5AG3').prop('checked') ? '1' : '';
        var ag5AG5 = $('#chk5AG5').prop('checked') ? '1' : '';
        var ag5AG6 = $('#chk5AG6').prop('checked') ? '1' : '';
        var ag5AGG = $('#chk5AGG').prop('checked') ? '1' : '';
        var ag5AGV = $('#chk5AGV').prop('checked') ? '1' : '';
        var ag5AGW = '';
        if (ag5AG1 == '1' || ag5AG6 == '1' || $('#chk5AGW').prop('checked')) {
            ag5AGW = '1';
        }
        else {
            ag5AGW = $('#chk5AGW').prop('checked') ? '1' : '';
        }
        var data = {
            Ag5AG1:ag5AG1,
            Ag5AG3:ag5AG3,
            Ag5AGV:ag5AGV,
            Ag5AGG:ag5AGG,
            Ag5AG6:ag5AG6,
            Ag5AGW:ag5AGW,
            Ag5AG2:ag5AG2,
            Ag5AG5:ag5AG5
        };
        $.ajax({
            type: "POST",
            url: "/CustomerMgmt/GetMandatoryField",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                //console.log(data);
                if (data !="") {                   
                    setTextMandatory(data); 
                    const rdlRequestVal = $('input[name="ApplicationRequestType"]:checked').val();

                    if (rdlRequestVal === 'C') {
                        //console.log("value is "+rdlRequestVal)
                        setLabelMandatory(data); 
                    } else {
                        $('#txtDealerCode').prop('disabled', true);
                    }
                    
                } else {
                    
                }
            },
            error: function () {
                alert('Error in mandatory field binding!');
            }
        });
    } catch (ex) {
        alert('Error in mandatory field binding!');
    }
}

///***********Set Mandatory Fields************************** */
function setTextMandatory(ds) {
    try { 
    const allSelectors = [
        // General
        '#txtDealerCode', '#ddlTitle', '#txtName1', '#txtName2', '#txtName3', '#txtName4',
        '#txtSearchTerms', '#txtStreetHouse', '#txtStreet2', '#txtStreet3', '#txtStreet4',
        '#txtStreet5', '#txtPostalCode', '#txtCity', '#ddlCountry', '#ddlRegion',
        '#ddlTimeZone', '#ddlTransportationCode', '#txtMobilePhone', '#txtFax', '#txtEmail',
        '#ddlIndustry', '#txtTaxNumberGst', '#ddlCityCode', '#ddlCtry', '#ddCustomerClass',
        '#txtVendorCode',

        // Bank Details
        '#txtBankKey', '#txtBankAccount', '#ddlAccountHolder', '#ddlBankControlKey',
        '#txtBankName', '#ddlBankRegion', '#txtBankStreet', '#txtBankCity', '#txtBankBranch',
        '#ddlBankCurrency',

        // Industry Details
        '#txtIndustry1', '#txtIndustryCode1', '#txtIndustryName', '#txtFirstName',

        // Sales Area Data
        '#ddlExchangeRateType', '#ddlPriceGroup', '#ddlSalesDistrict', '#ddlSalesOffice',
        '#ddlSalesGroup', '#ddlCustomerGroup', '#ddlSalesCurrency', '#ddlCustomerPrice',
        '#ddlDeliveryPriority', '#ddlShippingConditions',

        // CIN Details
        '#txtCstno', '#txtLstNo', '#txtInvoicingDates', '#txtInvoicingListDates',
        '#txtServiceRegNo', '#txtPanNumber', '#txtPaymentGuarProc',

        // Company Code Data
        '#ddlReconAccount', '#ddlIncoterms', '#ddlTermsOfPayment', '#ddlCreditControlArea',
        '#ddlAccountAssignmentGroup', '#ddlTaxClassification', '#txtWithHoldingTaxType',
        '#txtWithHoldingTaxCode', '#txtValidFrom', '#txtValidTo', '#txtWithHoldingTaxNumber',
        '#txtPaymentMethodSupplier', '#txtPaymentMethods', '#txtHouseBank'
    ];

    // Disable all and hide Withholding Tax group 
    $(allSelectors.join(',')).prop('disabled', true);
    $('#tbWithTax').hide();   

    $.each(ds, function (index,item) {              
       
        
        const $el = $('#' + item.CLIENTIDTEXTLABEL);
        if ($el.length) {
            $el.prop('disabled', false);

            if (item.CLIENTIDTEXTLABEL === 'txtWithHoldingTaxType') {
                $('#tbWithTax').show();
            }
        }
    });

    const rdlRequestVal = $('input[name="ApplicationRequestType"]:checked').val();
            const is5AG1 = $('#chk5AG1').prop('checked');
            const is5AG6 = $('#chk5AG6').prop('checked');

            if ((is5AG1 || is5AG6) && rdlRequestVal === 'C') {
                if (is5AG1) {
                    $('#ddlReconAccount').val('15002');
                    $('#ddlTermsOfPayment').val('ZADV');
                    $('#ddlIncoterms').val('CIF');
                    $('#ddlCreditControlArea').val('HMSI');
                    $('#ddlTaxClassification').val('1');
                    $('#ddlAccountAssignmentGroup').val('01');
                }
                if (is5AG6) {
                    $('#ddlReconAccount').val('15006');
                    $('#ddlTermsOfPayment').val('ZADV');
                    $('#ddlIncoterms').val('CIF');
                    $('#ddlCreditControlArea').val('HMSI');
                    $('#ddlTaxClassification').val('1');
                    $('#ddlAccountAssignmentGroup').val('01');
                }
            } else {

                const dropdownsToReset = [
                    '#ddlReconAccount', '#ddlTermsOfPayment', '#ddlIncoterms',
                    '#ddlCreditControlArea', '#ddlTaxClassification', '#ddlAccountAssignmentGroup'
                ];
                dropdownsToReset.forEach(function (sel) {
                    const $dd = $(sel);
                    if ($dd.prop('disabled')) {
                        // SelectedIndex = 0 equivalent
                        const $first = $dd.find('option:eq(0)');
                        if ($first.length) $dd.val($first.val());
                    }
                });
            }

    } catch (ex) {
        console.log(ex);
        alert('Error in mandatory field binding!');
    }

}

////***********************Set Mandatory Field Label******************************************************************//////
function setLabelMandatory(ds) {

    const allLabelSelectors = [
        // General labels
        '#lblcode', '#lblTitle', '#lblName1', '#lblName2', '#lblName3', '#lblName4',
        '#lblSearchTerms', '#lblStreetHouse', '#lblStreet2', '#lblStreet3', '#lblStreet4',
        '#lblStreet5', '#lblPostalCode', '#lblCity', '#lblCountry', '#lblRegion',
        '#lblTimeZone', '#lblTransportationCode', '#lblMobilePhone', '#lblFax', '#lblEmail',
        '#lblIndustry', '#lblTaxNumberGst', '#lblCityCode', '#lblCustomerClass', '#lblVendorcode',
        '#lblCtry',

        // Bank details
        '#lblBankKey', '#lblBankAccount', '#lblAccountHolder', '#lblBankControlKey', '#lblBankName',
        '#lblBankRegion', '#lblBankStreet', '#lblBankCity', '#lblBankBranch', '#lblBankCurrency',

        // Industry details
        '#lblIndustry1', '#lblIndustryCode1', '#lblIndustryName', '#lblFirstName',

        // Sales area
        '#lblExchangeRateType', '#lblPriceGroup', '#lblSalesDistrict', '#lblSalesOffice',
        '#lblSalesGroup', '#lblCustomerGroup', '#lblSalesCurrency', '#lblCustomerPrice',
        '#lblDeliveryPriority', '#lblShippingConditions',

        // CIN details
        '#lblCstno', '#lblLstNo', '#lblInvoicingDates', '#lblInvoicingListDates',
        '#lblServiceRegNo', '#lblPanNumber', '#lblPaymentGuarProc',

        // Company data
        '#lblReconAccount', '#lblIncoterms', '#lblTermsOfPayment', '#lblCreditControlArea',
        '#lblAccountAssignmentGroup', '#lblTaxClassification', '#lblWithHoldingTaxType',
        '#lblWithHoldingTaxCode', '#lblValidFrom', '#lblValidTo', '#lblWithHoldingTaxNumber',
        '#lblPaymentMethodSupplier', '#lblPaymentMethods', '#lblHouseBank',

        // Document mandatory section
        '#lblBankMandateDOC', '#lblCanceledChequeDOC', '#lblRTODOC', '#lblLOIDOC','#lblPANDOC', '#lblGSTNoDCO', '#lblCINDOC'
        ];

    // Clear all stars
    $(allLabelSelectors.join(',')).each(function () {
        removeStarFrom($(this));
    });
   
    $.each(ds, function (index,item) {
       
            const $lbl = $('#' + item.CLIENTIDLABEL);
            if ($lbl.length) {
                addRedStar($lbl);
            }
    });

    const rdlRequestVal = $('input[name="ApplicationRequestType"]:checked').val();      // rdlRequest.SelectedValue
    const rdlRequestTypeVal = $('input[name="RequestType"]:checked').val();  // rdlRequestType.SelectedValue
    const is5AG1 = $('#chk5AG1').prop('checked');
    const is5AGW = $('#chk5AGW').prop('checked');

    // If rdlRequest == "C" and 5AGW checked ➜ Bank Mandate & Canceled Cheque mandatory
    if (rdlRequestVal === 'C') {
        addRedStar($('#lblGSTNoDCO'));
    }
    if (rdlRequestVal === 'C' && is5AGW) {
        addRedStar($('#lblBankMandateDOC'));
        addRedStar($('#lblCanceledChequeDOC'));
    }
    // If rdlRequest == "C" and 5AG1 checked ➜ RTO & LOI mandatory
    if (rdlRequestVal === 'C' && is5AG1) {
        addRedStar($('#lblRTODOC'));
        addRedStar($('#lblLOIDOC'));
    }
    // If rdlRequest == "C" and rdlRequestType == "DM" ➜ PAN & GST mandatory
    if (rdlRequestVal === 'C' && rdlRequestTypeVal === 'DM') {
        addRedStar($('#lblPANDOC'));
        addRedStar($('#lblGSTNoDCO'));
    }

   
}
function removeStarFrom(label) {
      const original = $(label).text();
    const cleaned = original.replace("*","").trim();
        $(label).text(cleaned);
    }
    
function addRedStar(label) {
        removeStarFrom($(label));
        $(label).html($(label).html() + "<span style='color:red'>*</span>");
}

///////////////////////////
// enable/disable the final panel and adjust the preview button class
function initGroupOfData() {
    groupFlags.Gen_data = true;
    groupFlags.Bank_Data = true;
    groupFlags.Ind_data = true;
    groupFlags.Cin_data = true;
    groupFlags.Sales_Data = true;
    groupFlags.Comp_Data = true;

    // After initializing, evaluate UI
    //checkAllGroupOfData();
}
function setFinalState(enabled) {
        var $panFinal = $("#panFinal");
        var $btnPreview = $("#btnPreview");

        if (enabled) {           
            $panFinal.removeClass("disabled").attr("aria-disabled", "false");
            //$btnPreview.removeClass("disabled").addClass("btn btn-success");
            //$panFinal.removeClass("disabled-div").attr("aria-disabled", "false");
            //$btnPreview.removeClass("disabled").addClass("btn btn-success");
            $('#panFinal').find(':input').prop('disabled', false);
            $btnPreview.removeClass("disabled").addClass("btn btn-success");
        } else {         
            //$panFinal.addClass("disabled").attr("aria-disabled", "true");
            //$panFinal.addClass("disabled-div").attr("aria-disabled", "true");
            //$btnPreview.addClass("disabled").removeClass("btn-success").addClass("btn btn-success");
            $('#panFinal').find(':input').prop('disabled', true);
          
        }
    }
function checkAllGroupOfData() {
        // Get selected request value ('C' or 'U')
       
        const requestVal = $('input[name="ApplicationRequestType"]:checked').val();
        var Gen_data = !!groupFlags.Gen_data;
        var Bank_Data = !!groupFlags.Bank_Data;
        var Ind_data = !!groupFlags.Ind_data;
        var Cin_data = !!groupFlags.Cin_data;
        var Sales_Data = !!groupFlags.Sales_Data;
        var Comp_Data = !!groupFlags.Comp_Data;

        if (requestVal === "C") {
            // All flags must be true
            var allComplete = Gen_data && Bank_Data && Ind_data && Cin_data && Sales_Data && Comp_Data;
            setFinalState(allComplete);
        } else if (requestVal === "U") {            
            setFinalState(Gen_data);
        } else {           
            setFinalState(false);
        }

      
        return "";
    }

///////////////////////////Get Draf Datatype/////////////////////////////////////////////
// Utilities
function setText(selector, value) {
    $(selector).val(value ?? "");
}
function setSelect(selector, value) {
    if (value == null) return;
   // $(selector).val(value).trigger("change");
    $(selector).val(value);
}
function setVisible(selector, visible) {
    $(selector).toggle(!!visible);
}
function setHyperDialog(selector, fileName) {
    if (fileName && fileName.length > 0) {
        $(selector).text(fileName);
        setVisible(selector, true);
        // Mirrors: hyperX.OnClientClick = "javascript:openDialog('../../Uploads/CustomerMaster/' + file,980,680)"
        $(selector).off("click").on("click", function (e) {
            e.preventDefault();
            openDialog(`../../Uploads/CustomerMaster/${fileName}`, 980, 680);
        });
    } else {
        setVisible(selector, false);
    }
}
function openDialog(url, w, h) { window.open(url, "_blank", `width=${w},height=${h},resizable=yes,scrollbars=yes`); }
function BindSalesGroup() {
    try {
        if ($("#ddlSalesOffice").val() == '') { return; }

        $.getJSON("/CustomerMgmt/GetMasterDataWithPG", {
            GroupName: 'SALES GROUP',
            PGroup: $("#ddlSalesOffice").val()
        }).done(function (data) {
            if(data === 'error' || data === null || typeof data === 'undefined') { return; }                      
            var dt = '<option value="">-- Select --</option>';
            $("#ddlSalesGroup").html("");
            $.each(data.rows, function (i, item) {
                dt += '<option value="' + item.CODE + '">' + item.CODE_DESC + '</option>';
            });
            $("#ddlSalesGroup").html(dt);

            
        }).fail(function (xhr) {           
            console.error("Error in draft data fetching.", xhr.responseText || xhr.statusText);
            alert("Error in draft data fetching.");
        });
    } catch (ex) {
        console.log(ex);
        alert('Error in sales binding data!');
    }
}
function getDetailDraft() {
    try {
       
        // We emulate it via AJAX returning { rows: [ {...}, {...} ] } like objDs.Tables[0].Rows
        $.getJSON("/CustomerMgmt/GetDraftData", {            
            genDetailId: $("#hidGenDetailID").val()
        }).done(function (data) {
            if (data == 'error') { return; }
            const rows = data && data.rows ? data.rows : [];
            if (!rows.length) return;

            // Map server vars
            const row0 = rows[0] || {};
            let s = 0;
            let row1 = rows[1] || {};
            
            // Hidden IDs
            $("#hidHeaderID").val(row0["CMHEADERID"] || "");
            $("#hidDetaildID").val(row0["CMDETAILID"] || "");

            // s==1 if second row exists and has CUST_ACC_TYPE == '5AGW'
            if (rows.length > 1 && (row1["CUST_ACC_TYPE"] || "") === "5AGW") {
                $("#hidDetaildID1").val(row1["CMDETAILID"] || "");
                s = 1;
                // Equivalent to SetAccountData(row1["CUST_ACC_TYPE"])
                // $(document).trigger("account:type", row1["CUST_ACC_TYPE"]);
                setAccountData(row1["CUST_ACC_TYPE"]);
            }

            
            //$("#rdlRequest input[value='" + (row0["CM_REQ_TYPE"] || "") + "']").prop("checked", true).trigger("change"); // mirrors SelectedValue
            //$("#rdlRequestType input[value='" + (row0["REQUEST_TYPE"] || "") + "']").prop("checked", true).trigger("change");

            $("input[value='" + (row0["CM_REQ_TYPE"] || "") + "']").prop("checked", true).trigger("change"); 
            $("input[value='" + (row0["REQUEST_TYPE"] || "") + "']").prop("checked", true).trigger("change");

            //$(document).trigger("account:type", row0["CUST_ACC_TYPE"]);
            setAccountData(row0["CUST_ACC_TYPE"])

            // Distribution & Division Group
            setSelect("#ddlDistribution", row0["DISTRIBTUION_CHH"]);
           // $(document).trigger("division:group", row0["DIVISION_GRP"]);
            setDivisionGrpByValue(row0["DIVISION_GRP"]);            

            // Generic helper for "use row0 unless empty then row1 when s==1"
            function prefer(k0, k1) {
                const v0 = row0[k0] || "";
                //const v1 = row1[k1 || k0] || "";
                const v1 = row1[k0] || "";
                return (v0 === "" && s === 1) ? v1 : v0;
            }

            // ****** General Details ******
            setText("#txtDealerCode", prefer("CUSTOMER_CODE"));
            setSelect("#ddlTitle", prefer("TITLE"));
            setText("#txtName1", prefer("NAME1"));
            setText("#txtName2", prefer("NAME2"));
            setText("#txtName3", prefer("NAME3"));
            setText("#txtName4", prefer("NAME4"));
            setText("#txtSearchTerms", prefer("SEARCHTERM"));
            setText("#txtStreetHouse", prefer("STREETHOUSENUMBER"));
            setText("#txtStreet2", prefer("STREET2"));
            setText("#txtStreet3", prefer("STREET3"));
            setText("#txtStreet4", prefer("STREET4"));
            setText("#txtStreet5", prefer("STREET5"));
            setText("#txtPostalCode", prefer("POSTALCODE"));
            setText("#txtCity", prefer("CITY"));
            setSelect("#ddlCountry", prefer("COUNTRY"));          

            // Transport group bind (server: BindTransportGRP())
            //ChangeCountry($("#ddlCountry"));

            setSelect("#ddlRegion", prefer("CREGION"));
            setSelect("#ddlTimeZone", prefer("CTIMEZONE"));
            setSelect("#ddlTransportationCode", prefer("TRANSPORTATIONCODE"));
            setText("#txtMobilePhone", prefer("MOBILEPHONE"));
            setText("#txtFax", prefer("FAX"));
            setText("#txtEmail", prefer("EMAIL"));
            setSelect("#ddlIndustry", prefer("INDUSTRY"));
            setText("#txtTaxNumberGst", prefer("TAXNUMBER3"));
            setSelect("#ddlCityCode", prefer("CITYCODE"));
            setSelect("#ddCustomerClass", prefer("CUSTOMERCLASS"));
            setText("#txtVendorCode", prefer("VENDORNO"));

            // ********* Bank Details *********
            setText("#txtBankKey", prefer("BANKKEY"));
            setText("#txtBankAccount", prefer("BANKACCOUNT"));
            setSelect("#ddlAccountHolder", prefer("ACCOUNTHOLDER"));
            setSelect("#ddlBankControlKey", prefer("BANKCONTROLKEY"));
            setText("#txtBankName", prefer("BANKNAME"));
            setSelect("#ddlBankRegion", prefer("BANKREGION"));
            setText("#txtBankStreet", prefer("BANKSTREET"));
            setText("#txtBankCity", prefer("BANKCITY"));
            setText("#txtBankBranch", prefer("BANKBRANCH"));
            setSelect("#ddlBankCurrency", prefer("BANKCURRENCY"));
            setSelect("#ddlCtry", prefer("CTRY"));
          

            // ********* Industry Details *********
            setText("#txtIndustry1", prefer("INDUSTRY1"));
            setText("#txtIndustryCode1", prefer("INDUSTRYCODE1"));
            setText("#txtIndustryName", prefer("INDUSTRYNAME"));
            setText("#txtFirstName", prefer("FIRSTNAME"));

            // ********* Sales Area Data *********
            setSelect("#ddlExchangeRateType", prefer("EXCHANGERATETYPE"));
            setSelect("#ddlPriceGroup", prefer("PRICEGROUP"));
            setSelect("#ddlSalesDistrict", prefer("SALESDISTRICT"));
            setSelect("#ddlSalesOffice", prefer("SALESOFFICE"));

            // Server: BindSalesGroup(); then set sale group value
           // BindSalesGroup();

            setSelect("#ddlSalesGroup", prefer("SALESGROUP"));

            setSelect("#ddlCustomerGroup", prefer("CUSTOMERGROUP"));
            setSelect("#ddlSalesCurrency", prefer("SALESCURRENCY"));
            setSelect("#ddlCustomerPrice", prefer("CUSTPRICPROC1"));
            setSelect("#ddlDeliveryPriority", prefer("DELIVERYPRIORITYGROUP"));
            setSelect("#ddlShippingConditions", prefer("SHIPPINGCONDITION"));

            // ********* CIN Details *********
            setText("#txtCstno", prefer("CSTNO"));
            setText("#txtLstNo", prefer("LSTNO"));
            setText("#txtInvoicingDates", prefer("INVOICINGDATES"));
            setText("#txtInvoicingListDates", prefer("INVOICINGLISTDATES"));
            setText("#txtServiceRegNo", prefer("SERREGNO"));
            setText("#txtPanNumber", prefer("PANNUMBER"));
            setText("#txtPaymentGuarProc", prefer("PAYMENTGUARANTEEPROC"));

            // E-Invoice applicable dropdown + trigger SelectedIndexChanged
            setSelect("#ddlEInvoiceApplicable", prefer("E_INVOICE_APPLICABLE"));
            $("#ddlEInvoiceApplicable").trigger("change"); // mirrors ddlEInvoiceApplicable_SelectedIndexChanged
            // (Hook your real handler in a change event binding)

            // ********* Company Code Data *********
            setSelect("#ddlReconAccount", prefer("RECONSACCOUNT"));
            setSelect("#ddlIncoterms", prefer("INCOTERM1"));
            setSelect("#ddlTermsOfPayment", prefer("TERMOFPAYMENT"));
            setSelect("#ddlCreditControlArea", prefer("CREDITCONTROLAREA"));
            setSelect("#ddlAccountAssignmentGroup", prefer("ACCASSIGMENTGROUP"));
            setSelect("#ddlTaxClassification", prefer("TAXCLASSIFICATION"));
            setText("#txtWithHoldingTaxType", prefer("WITHHOLDINGTAXTYPE"));
            setText("#txtWithHoldingTaxCode", prefer("WITHHOLDINGTAXCODE"));
            setText("#txtValidFrom", prefer("VALIDFROM"));
            setText("#txtValidTo", prefer("VALIDTO"));
            setText("#txtWithHoldingTaxNumber", prefer("WITHHOLDINGTAXNUMBER"));
            setText("#txtPaymentMethodSupplier", prefer("PMTMETHSUPL"));
            setText("#txtPaymentMethods", prefer("PAYMENTMETHODS"));
            setText("#txtHouseBank", prefer("HOUSEBANK"));

            // ********* Hyper links (documents) *********
            setHyperDialog("#hyperBankMandate", prefer("BANK_MANDATE_FILE"));
            setHyperDialog("#hyperCancelCheque", prefer("BANK_CANCELED_FILE"));
            setHyperDialog("#hyperPAN", prefer("CIN_PAN_NO_FILE"));
            setHyperDialog("#hyperGSTNo", prefer("CIN_GST_NO_FILE"));
            setHyperDialog("#hyperRTO", prefer("COMP_RTO_FILE"));
            setHyperDialog("#hyperLOI", prefer("COMP_LOI_FILE"));
            setHyperDialog("#hyperCINDoc", prefer("CIN_EINVOICE_DOC_FILE")); // Added Logic for Einvoice Doc
            setHyperDialog("#hyperOtherDoc", prefer("OTHER_DOC_FILE"));

            // Remarks
            setText("#txt_Remarks", prefer("REMARKS"));

            // ********* Mandatory Field Highlight *********
            setMandatoryField();

            // ********* Recreate ViewState flag logic *********
            // if (txtDealerCode.Text.Trim().ToUpper() != "") { ViewState["Gen_data"] = true; }
            groupFlags.Gen_data = !!($("#txtDealerCode").val() || "").trim();

            // if (txtBankAccount.Text.Trim() != "") { ViewState["Bank_Data"] = true; }
            groupFlags.Bank_Data = !!($("#txtBankAccount").val() || "").trim();

            // if (txtIndustryCode1.Text.Trim() != "" || txtIndustry1.Text.Trim() != "") { ViewState["Ind_data"] = true; }
            const indCodeFilled = !!($("#txtIndustryCode1").val() || "").trim();
            const indTextFilled = !!($("#txtIndustry1").val() || "").trim();
            groupFlags.Ind_data = indCodeFilled || indTextFilled;

            // if (ddlSalesOffice.SelectedIndex > 0 || ddlPriceGroup.SelectedIndex > 0) { ViewState["Sales_Data"] = true; }
            groupFlags.Sales_Data = ($("#ddlSalesOffice").prop("selectedIndex") > 0) || ($("#ddlPriceGroup").prop("selectedIndex") > 0);

            // if (txtPanNumber.Text.Trim() != "" || txtCstno.Text.Trim() != "") { ViewState["Cin_data"] = true; }
            const panFilled = !!($("#txtPanNumber").val() || "").trim();
            const cstFilled = !!($("#txtCstno").val() || "").trim();
            groupFlags.Cin_data = panFilled || cstFilled;

            // if (ddlReconAccount.SelectedIndex > 0 || ddlIncoterms.SelectedIndex > 0) { ViewState["Comp_Data"] = true; }
            groupFlags.Comp_Data = ($("#ddlReconAccount").prop("selectedIndex") > 0) || ($("#ddlIncoterms").prop("selectedIndex") > 0);           

            // ********* Withholding tax visibility & binding *********
            if ($("#txtWithHoldingTaxType").val().trim().length > 0) {
                $("#tbWithTax").show();
                BinWithHoldingTax();
            } 

            // ********* DOM/EXP options *********
            // rdlRequestType values: "DM" or "EX" 
            //const reqType = $("#rdlRequestType input[type=radio]:checked").val();
            const reqType = $("input[name='RequestType']:checked").val();
            if (reqType === "DM") {
                $("#dom_options1").css("display", "table-row");
                $("#dom_options2").css("display", "table-row");
                $("#exp_options").css("display", "none");
            } else if (reqType === "EX") {
                $("#dom_options1").css("display", "none");
                $("#dom_options2").css("display", "none");
                $("#exp_options").css("display", "table-row");
            }

            // ********* Final check (mirrors CheckAllGroupOfData()) *********
            checkAllGroupOfData();            

        }).fail(function (xhr) {           
            console.error("Error in draft data fetching.", xhr.responseText || xhr.statusText);
            alert("Error in draft data fetching.");
        });
    } catch (ex) {
        console.error("Error in draft data fetching.", ex);
        alert("Error in draft data fetching.");
    }
}

////////////////////////////////General Data Setting////////////////////////////////////////////////////////////
function PrimaryValidateData() {
    var flag = false;
    var err = "";

    // --- Account type group: any of these must be checked ---
    var accountIds = [
        "#chk5AG1", "#chk5AG3", "#chk5AGV", "#chk5AGG",
        "#chk5AG6", "#chk5AGW", "#chk5AG2", "#chk5AG5"
    ];
    var anyAccountChecked = accountIds.some(function (sel) {
        var $el = $(sel);
        return $el.length && $el.is(":checked");
    });
    if (!anyAccountChecked) {
        flag = true;
        err += " check account type,";
    }

    // --- Distribution dropdown: selectedIndex must be > 0 ---
    var $ddlDistribution = $("#ddlDistribution");
    var distributionIndex = $ddlDistribution.length ? $ddlDistribution.prop("selectedIndex") : -1;
    if (distributionIndex <= 0) {
        flag = true;
        err += " select Distribution type,";
    }

    // --- Division CheckBoxList by NAME: at least one checked ---

    var divisionCheckedCount = $("input[type=checkbox][name='Divisions']:checked").length;
    if (divisionCheckedCount === 0) {
        flag = true;
        err += " select Division Group,";
    }

    // --- Sales Organization not blank ---
    var salesOrg = ($.trim($("#txtSalesOrganization").val() || ""));
    if (!salesOrg) {
        flag = true;
        err += " Sales Organization cannot be blank,";
    }

    var companyCode = ($.trim($("#txtCompanyCode").val() || ""));
    if (!companyCode) {
        flag = true;
        err += " Company code cannot be blank,";
    }


    if (err.length > 1) {
        err = "Please " + err.substring(0, err.length - 1);
    } else {
        err = "";
    }

    return { flag: flag, err: err };
}
function isBlankWhenEnabled($el) {
    if (!$el.length) return false;                     // element not found -> consider valid
    if ($el.prop("disabled")) return false;            // disabled -> skip
    var val = ($.trim($el.val() || ""));
    return val.length <= 0;
}
function isUnselectedWhenEnabled($sel) {
    if (!$sel.length) return false;
    if ($sel.prop("disabled")) return false;
    var idx = $sel.prop("selectedIndex");
    return (idx <= 0);
}
function getAccountData() {
    var account_type = "";
    var ac_55AGW = "";

    // Mirror the exact overwrite order in your C# code
    if ($("#chk5AG1").is(":checked")) { account_type = "5AG1"; }
    if ($("#chk5AG2").is(":checked")) { account_type = "5AG2"; }
    if ($("#chk5AG3").is(":checked")) { account_type = "5AG3"; }
    if ($("#chk5AG5").is(":checked")) { account_type = "5AG5"; }
    if ($("#chk5AG6").is(":checked")) { account_type = "5AG6"; }
    if ($("#chk5AGG").is(":checked")) { account_type = "5AGG"; }
    if ($("#chk5AGV").is(":checked")) { account_type = "5AGV"; }

    // Special 5AGW logic
    var is5AGW = $("#chk5AGW").is(":checked");
    var is5AG1 = $("#chk5AG1").is(":checked");
    var is5AG6 = $("#chk5AG6").is(":checked");

    if (is5AGW && (is5AG1 || is5AG6)) {
        ac_55AGW = "5AGW";              
    } else if (is5AGW) {
        account_type = "5AGW";         
    }

    return { account_type: account_type, ac_55AGW: ac_55AGW };
}
function getDivision() {
    var division = "";

    $("input[type=checkbox][name='Divisions']").each(function () {
        if ($(this).is(":checked")) {
            division += ($(this).val() || "").trim() + "|";
            
        }
    });
    
    if (division.endsWith("|")) {
        division = division.slice(0, -1);
    }

    return division; 
}
function validateGeneralData() {
    var flag = false;
    var err = "";     

    if (isBlankWhenEnabled($("#txtDealerCode"))) { flag = true; err += "Customer/Dealer code,"; }
    if (isUnselectedWhenEnabled($("#ddlTitle"))) { flag = true; err += "Title(Company), "; }
    if (isBlankWhenEnabled($("#txtName1"))) { flag = true; err += "Name 1,"; }
    if (isBlankWhenEnabled($("#txtName2"))) { flag = true; err += "Name 2,"; }
    if (isBlankWhenEnabled($("#txtName3"))) { flag = true; err += "Name 3,"; }
    if (isBlankWhenEnabled($("#txtName4"))) { flag = true; err += "Name 4,"; }
    if (isBlankWhenEnabled($("#txtSearchTerms"))) { flag = true; err += "Search term,"; }

    // --- Address fields ---
    if (isBlankWhenEnabled($("#txtStreet2"))) { flag = true; err += "Street2,"; }
    if (isBlankWhenEnabled($("#txtStreet3"))) { flag = true; err += "Street3,"; }
    if (isBlankWhenEnabled($("#txtStreet4"))) { flag = true; err += "Street4,"; }
    if (isBlankWhenEnabled($("#txtStreet5"))) { flag = true; err += "Street5,"; }
    if (isBlankWhenEnabled($("#txtPostalCode"))) { flag = true; err += "Postal Code,"; }
    if (isBlankWhenEnabled($("#txtCity"))) { flag = true; err += "City ,"; }
    if (isUnselectedWhenEnabled($("#ddlCountry"))) { flag = true; err += "Country,"; }
    if (isUnselectedWhenEnabled($("#ddlRegion"))) { flag = true; err += "Region,"; }
    if (isUnselectedWhenEnabled($("#ddlTimeZone"))) { flag = true; err += "Time Zone,"; }
    if (isUnselectedWhenEnabled($("#ddlTransportationCode"))) { flag = true; err += "Transportation Code,"; }

    // --- Contact fields ---
    if (isBlankWhenEnabled($("#txtMobilePhone"))) { flag = true; err += "Mobile phone,"; }
    // Preserve your server-side quirk: it overwrites err for Fax
    if (isBlankWhenEnabled($("#txtFax"))) { flag = true; err = "Fax,"; }
    if (isBlankWhenEnabled($("#txtEmail"))) { flag = true; err += "Email Id,"; }

    // --- Business fields ---
    if (isUnselectedWhenEnabled($("#ddlIndustry"))) { flag = true; err += "Industry,"; }
    if (isBlankWhenEnabled($("#txtTaxNumberGst"))) { flag = true; err += "GST No.,"; }
    if (isUnselectedWhenEnabled($("#ddlCityCode"))) { flag = true; err += "City Code,"; }

    // Finalize error text: "Please fill ..." and trim the trailing comma
    if (err.length > 0) {
        err = "Please fill " + err.substring(0, err.length - 1);
    } else {
        err = "";
    }

    return { flag: flag, err: err };
}
async function saveGeneralData() {
    try {
       
        const reqVal = $("input[name='ApplicationRequestType']:checked").val();     // "C" or "U"
        const reqTypeVal = $("input[name='RequestType']:checked").val(); // "DM" or "EX"
        if (reqVal == null || reqVal == undefined) {
            ShowMessage("error", "Please customer type!");
            return;
        }
        if (reqTypeVal == null || reqTypeVal == undefined) {
            ShowMessage("error", "Please request type!");
            return;
        }

        // Primary validation (always) ---
        var v1 = PrimaryValidateData();
        if (v1.flag) {
            ShowMessage("error", v1.err);  
            sweetAlert(" ", v1.err, "warning");
            return;
        }

        // General data validation (only for Create "C") ---
        if (reqVal == "C") {
            let v2 = validateGeneralData();
            if (v2.flag) {
                ShowMessage("error", v2.err);
                sweetAlert(" ", v2.err, "warning");
                return;
            }
        }
        var division = getDivision();
       
        //const gstUpload = $("#fileUploadGSTNo");
        const gstUpload = document.getElementById("fileUploadGSTNo");
        const hasGstFile = !!(gstUpload && gstUpload.files && gstUpload.files.length);
        const hyperGstText = ($("#hyperGSTNo").text() || "").trim();
        const hyperGSTNo = $('#hyperGSTNo');

        if (reqVal == "C" && reqTypeVal == "DM") {
            if ((!hasGstFile)&& hyperGstText == "") {               
                ShowMessage("error", "Please upload GST document file.");
                return;
            }
        }
        else if (reqVal === "U" && reqTypeVal === "DM") {
            const fieldsNonEmpty =
                ($("#txtTaxNumberGst").val() || "").trim() !== "" &&
                ($("#txtStreetHouse").val() || "").trim() !== "" &&
                ($("#txtStreet2").val() || "").trim() !== "" &&
                ($("#txtStreet4").val() || "").trim() !== "" &&
                ($("#txtStreet5").val() || "").trim() !== "" &&
                ($("#txtPostalCode").val() || "").trim() !== "";
            if (fieldsNonEmpty && !hasGstFile && hyperGstText == "") {
                ShowMessage("error", "Please upload GST document file.");
                return;
            }
        }
        var data = getAccountData();
        if (hasGstFile) {
            var extension = $(gstUpload).val().split('.').pop().toLowerCase();
            var validFileExtensions = ['pdf'];
            if ($.inArray(extension, validFileExtensions) == -1) {
                var _msg = "Sorry! Upload only 'pdf' file";
                sweetAlert(" ", _msg, "warning");
                return;
            }
            var MB1 = 1024 * 1024; // 1,048,576 bytes
            if (gstUpload.size > MB1) {
                sweetAlert(" ", "Sorry! Max allowed file size is 1 mb", "warning");
                return;
            }
        }
        
        var formData = new FormData();       
            
        formData.append("CMHEADERID", $('#hidHeaderID').val());
        formData.append("CMDETAILID", $("#hidDetaildID").val());
        formData.append("CMDETAILID1", $("#hidDetaildID1").val());
        formData.append("CM_REQ_TYPE", reqVal);
        formData.append("REQUEST_TYPE", reqTypeVal);
        formData.append("CUST_ACC_TYPE", data.account_type);
        formData.append("CUST_ACC_TYPE1", data.ac_55AGW);
        formData.append("SALES_ORG", $("#txtSalesOrganization").val());
        formData.append("COMPANY_CODE", $("#txtCompanyCode").val());
        formData.append("DIVISION_GRP", division);
        formData.append("DISTRIBTUION_CHH", $("#ddlDistribution").val());
        formData.append("CUSTOMER_CODE", $("#txtDealerCode").val());
        formData.append("TITLE", $("#ddlTitle").val());
        formData.append("NAME1", $("#txtName1").val());
        formData.append("NAME2", $("#txtName2").val());
        formData.append("NAME3", $("#txtName3").val());
        formData.append("NAME4", $("#txtName4").val());
        formData.append("SEARCHTERM", $("#txtSearchTerms").val());
        formData.append("STREETHOUSENUMBER", $("#txtStreetHouse").val());
        formData.append("STREET2", $("#txtStreet2").val());
        formData.append("STREET3", $("#txtStreet3").val());
        formData.append("STREET4", $("#txtStreet4").val());
        formData.append("STREET5", $("#txtStreet5").val());
        formData.append("POSTALCODE", $("#txtPostalCode").val());
        formData.append("CITY", $("#txtCity").val());
        formData.append("COUNTRY", $("#ddlCountry").val());
        formData.append("CREGION", $("#ddlRegion").val());
        formData.append("CTIMEZONE", $("#ddlTimeZone").val());
        formData.append("TRANSPORTATIONCODE", $("#ddlTransportationCode").val());
        formData.append("MOBILEPHONE", $("#txtMobilePhone").val());
        formData.append("FAX", $("#txtFax").val());
        formData.append("EMAIL", $("#txtEmail").val());
        formData.append("INDUSTRY", $("#ddlIndustry").val());
        formData.append("TAXNUMBER3", $("#txtTaxNumberGst").val());
        formData.append("VENDORNO", $("#txtVendorCode").val());
        formData.append("CITYCODE", $("#ddlCityCode").val());
        formData.append("CUSTOMERCLASS", $("#ddCustomerClass").val());
        formData.append("FILE1", document.getElementById("fileUploadGSTNo").files[0]);

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/CustomerMgmt/SaveGeneralData",
            data: formData,
            datatype: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.RS == 1 && data.CMHEADERID > 0) {
                    $("#hidHeaderID").val(data.CMHEADERID);
                    $("#hidDetaildID").val(data.CMDEAILID1);
                    $("#hidDetaildID1").val(data.CMDEAILID2);
                    groupFlags["Gen_data"] = true;
                    ShowMessage("success", data.MESSAGE);
                    sweetAlert("Customer Master data", data.MESSAGE, "success");
                    if (data.FILE1 != null) {                      
                        hyperGSTNo.text(data.FILE1).show();
                        hyperGSTNo.off('click').on('click', function (e) {
                            e.preventDefault();
                            openDialog('../../Uploads/CustomerMaster/' + data.FILE1, 980, 680);
                        });
                                              
                    }
                    checkAllGroupOfData();
                }
                else if (data.RS == 0 || data.res == -1) {
                    sweetAlert("", data.MESSAGE, "warning");
                    $("#fileUploadGSTNo").text("");  
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {
                sweetAlert("", "Oops... Something went wrong!.", "warning"); 
                $("#fileUploadGSTNo").val("");      
            }
        });

    } catch (ex) {
        $("#fileUploadGSTNo").val("");      
        sweetAlert("", "Oops... Something went wrong!.", "warning");
        console.log("Error message " + ex);
    }
}

/////////////////////////////Save Bank Data///////////////////////////////////////////////////////////////////////////////
function isEnabled($el) {
    return !$el.prop('disabled');
}
function isEmptyText($el) {
    return $.trim($el.val() || '').length === 0;
}
function isInvalidSelect($el) {
    const idx = $el.prop('selectedIndex');
    return idx === undefined || idx <= 0;
}
function validateBankData() {
    let errors = [];
    const $txtBankKey = $('#txtBankKey');
    const $txtBankAccount = $('#txtBankAccount');
    const $ddlAccountHolder = $('#ddlAccountHolder');
    const $ddlBankControlKey = $('#ddlBankControlKey');
    const $txtBankName = $('#txtBankName');
    const $ddlBankRegion = $('#ddlBankRegion');
    const $txtBankStreet = $('#txtBankStreet');
    const $txtBankCity = $('#txtBankCity');
    const $txtBankBranch = $('#txtBankBranch');
    const $ddlBankCurrency = $('#ddlBankCurrency');
    const $ddlCtry = $('#ddlCtry');

    if (isEnabled($txtBankKey) && isEmptyText($txtBankKey)) errors.push('Bank Key');
    if (isEnabled($txtBankAccount) && isEmptyText($txtBankAccount)) errors.push('Bank Account No.');
    if (isEnabled($ddlAccountHolder) && isInvalidSelect($ddlAccountHolder)) errors.push('Account Holder');
    if (isEnabled($ddlBankControlKey) && isInvalidSelect($ddlBankControlKey)) errors.push('Bank Control Key');
    if (isEnabled($txtBankName) && isEmptyText($txtBankName)) errors.push('Bank Name');
    if (isEnabled($ddlBankRegion) && isInvalidSelect($ddlBankRegion)) errors.push('Bank Region');
    if (isEnabled($txtBankStreet) && isEmptyText($txtBankStreet)) errors.push('Bank Street');
    if (isEnabled($txtBankCity) && isEmptyText($txtBankCity)) errors.push('Bank City');
    if (isEnabled($txtBankBranch) && isEmptyText($txtBankBranch)) errors.push('Bank Branch');
    if (isEnabled($ddlBankCurrency) && isInvalidSelect($ddlBankCurrency)) errors.push('Currency');
    if (isEnabled($ddlCtry) && isInvalidSelect($ddlCtry)) errors.push('CTry');

    const isInvalid = errors.length > 0;
    const err = isInvalid ? `Please fill ${errors.join(', ')}` : '';
    //return { isInvalid, err };
    return { flag: isInvalid, err: err };
}
async function saveBankData() {
    try {
        // Primary validation
        // Primary validation (always) ---
        var v1 = PrimaryValidateData();
        if (v1.flag) {
            ShowMessage("error", v1.err);
            sweetAlert(" ", v1.err, "warning");
            return;
        }
        
        const requestType = $("input[name='ApplicationRequestType']:checked").val();     // "C" or "U"
        const reqTypeVal = $("input[name='RequestType']:checked").val(); // "DM" or "EX"      

        // When request is C or U, validate bank data
        if (requestType == 'C' || requestType == 'U') {
            const bv = validateBankData(); // from your previous jQuery function
            if (bv.flag) {
                ShowMessage("error", bv.err);
                sweetAlert(" ", bv.err, "warning");
                return;
            } 
        }
        var data = getAccountData();
        const $chk5AGW = $('#chk5AGW');        

        
        //const $fpBankMandate = $('#fpBanckMandate');   // input[type=file]
        //const $fpCanceledCheque = $('#fpCanceledCheque'); // input[type=file]
        const fpBankMandate = document.getElementById("fpBankMandate");
        const hasBankMandateFile = !!(fpBankMandate && fpBankMandate.files && fpBankMandate.files.length);
        const fpCanceledCheque = document.getElementById("fpCanceledCheque");
        const hasCanceledChequeFile = !!(fpCanceledCheque && fpCanceledCheque.files && fpCanceledCheque.files.length);
        const hyperBankMandate = $('#hyperBankMandate'); // hyperlink text mirrors server usage
        const hyperCancelCheque = $('#hyperCancelCheque');

        // File validation based on server rules
        if (requestType == 'C' && $chk5AGW.prop('checked')) {
            if ((!hasBankMandateFile) && $.trim(hyperBankMandate.text()) == '') {
                showError('Please upload Bank Mandatory document.');
                return;
            }
            if ((!hasCanceledChequeFile) && $.trim(hyperCancelCheque.text()) == '') {
                showError('Please upload Canceled Cheque document.');
                return;
            }
        } else if (requestType == 'U') {
            if ((!hasBankMandateFile) && $.trim(hyperBankMandate.text()) == '') {
                showError('Please upload Bank Mandatory document.');
                return;
            }
            if ((!hasCanceledChequeFile) && $.trim(hyperCancelCheque.text()) == '') {
                showError('Please upload Canceled Cheque document.');
                return;
            }
        }

        
        var validFileExtensions = ['pdf'];
        var MB1 = 1024 * 1024; // 1,048,576 bytes
        if (hasBankMandateFile) {
            var extension = $(fpCanceledCheque).val().split('.').pop().toLowerCase();
            if ($.inArray(extension, validFileExtensions) == -1) {
                var _msg = "Sorry! Upload only 'pdf' file";
                sweetAlert(" ", _msg, "warning");
                return;
            }
           
            if (fpCanceledCheque.size > MB1) {
                sweetAlert(" ", "Sorry! Max allowed file size is 1 mb", "warning");
                return;
            }
        }
        if (hasCanceledChequeFile) {
            var extension1 = $(fpBankMandate).val().split('.').pop().toLowerCase();
            if ($.inArray(extension1, validFileExtensions) == -1) {
                var _msg = "Sorry! Upload only 'pdf' file";
                sweetAlert(" ", _msg, "warning");
                return;
            }

            if (fpBankMandate.size > MB1) {
                sweetAlert(" ", "Sorry! Max allowed file size is 1 mb", "warning");
                return;
            }
        }
       
        var division = getDivision();
        var formData = new FormData();
        formData.append("CMHEADERID", $('#hidHeaderID').val());
        formData.append("CMDETAILID", $("#hidDetaildID").val());
        formData.append("CMDETAILID1", $("#hidDetaildID1").val());
        formData.append("CM_REQ_TYPE", requestType);
        formData.append("REQUEST_TYPE", reqTypeVal);
        formData.append("CUST_ACC_TYPE", data.account_type);
        formData.append("CUST_ACC_TYPE1", data.ac_55AGW);
        formData.append("SALES_ORG", $("#txtSalesOrganization").val());
        formData.append("COMPANY_CODE", $("#txtCompanyCode").val());
        formData.append("DIVISION_GRP", division);
        formData.append("DISTRIBTUION_CHH", $("#ddlDistribution").val());
        formData.append("CUSTOMER_CODE", $("#txtDealerCode").val());
        formData.append("CTRY", $("#ddlCtry").val());
        formData.append("BANKKEY", $("#txtBankKey").val());
        formData.append("BANKACCOUNT", $("#txtBankAccount").val());
        formData.append("ACCOUNTHOLDER", $("#ddlAccountHolder").val());
        formData.append("BANKCONTROLKEY", $("#ddlBankControlKey").val());
        formData.append("BANKNAME", $("#txtBankName").val());
        formData.append("BANKREGION", $("#ddlBankRegion").val());
        formData.append("BANKSTREET", $("#txtBankStreet").val());
        formData.append("BANKCITY", $("#txtBankCity").val());
        formData.append("BANKBRANCH", $("#txtBankBranch").val());//
        formData.append("BANKCURRENCY", $("#ddlBankCurrency").val())      
        formData.append("FILE1", document.getElementById("fpBankMandate").files[0]);
        formData.append("FILE2", document.getElementById("fpCanceledCheque").files[0]);

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/CustomerMgmt/SaveBankData",
            data: formData,
            datatype: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.RS == 1 && data.CMHEADERID > 0) {
                    $("#hidHeaderID").val(data.CMHEADERID);
                    $("#hidDetaildID").val(data.CMDEAILID1);
                    $("#hidDetaildID1").val(data.CMDEAILID2);
                    groupFlags["Bank_Data"] = true;
                    ShowMessage("success", data.MESSAGE);
                    sweetAlert("Customer Master data", data.MESSAGE, "success");
                   
                    if (data.FILE1 != null) {                     

                        hyperBankMandate.text(data.FILE1).show();
                        hyperBankMandate.off('click').on('click', function (e) {
                            e.preventDefault();
                            openDialog('../../Uploads/CustomerMaster/' + data.FILE1, 980, 680);
                        });
                    }
                    if (data.FILE2 != null) {
                        hyperCancelCheque.text(data.FILE2).show();
                        hyperCancelCheque.off('click').on('click', function (e) {
                            e.preventDefault();
                            openDialog('../../Uploads/CustomerMaster/' + data.FILE2, 980, 680);
                        });
                    }

                    checkAllGroupOfData();
                }
                else if (data.RS == 0 || data.res == -1) {
                    sweetAlert("", data.MESSAGE, "warning");
                    $("#fpBankMandate").text("");
                    $("#fpCanceledCheque").text("");
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {
                $("#fpBankMandate").val("");
                $("#fpCanceledCheque").val("");
                sweetAlert("", "Oops... Something went wrong!.", "warning");
                ShowMessage("error", 'Error in AJAX calling!')
            }
        });
     

    } catch (ex) {
        $("#fpBankMandate").val("");
        $("#fpCanceledCheque").val("");
        sweetAlert("", "Oops... Something went wrong!.", "warning");       
        ShowMessage("error", ex.message || 'Unexpected error')
    }
}
/////////////////////////////////////Save Industry Data//////////////////////////////////////
function validateIndustry() {
    const errors = [];

    const $txtIndustry1 = $('#txtIndustry1');
    const $txtIndustryCode1 = $('#txtIndustryCode1');
    const $txtIndustryName = $('#txtIndustryName');
    const $txtFirstName = $('#txtFirstName');

    if (isEnabled($txtIndustry1) && isEmptyText($txtIndustry1)) errors.push('Industry');
    if (isEnabled($txtIndustryCode1) && isEmptyText($txtIndustryCode1)) errors.push('Industry Code 1 (RTO Code)');
    if (isEnabled($txtIndustryName) && isEmptyText($txtIndustryName)) errors.push('Name');
    if (isEnabled($txtFirstName) && isEmptyText($txtFirstName)) errors.push('First Name');

    const isInvalid = errors.length > 0;
    const err = isInvalid ? `Please fill ${errors.join(', ')}` : '';

    return { flag: isInvalid, err: err };
}
async function SaveIndustryData() {
    try {

        var v1 = PrimaryValidateData();
        if (v1.flag) {
            ShowMessage("error", v1.err);
            return;
        }

        const requestType = $("input[name='ApplicationRequestType']:checked").val();
        const reqTypeVal = $("input[name='RequestType']:checked").val(); // "DM" or "EX"      

        // When request is C or U, validate bank data
        if (requestType == 'C') {
            const bv = validateIndustry(); 
            if (bv.flag) { ShowMessage("error", bv.err); return; }
        }
        var data = getAccountData();
        var division = getDivision();
        var formData = new FormData();

        formData.append("CMHEADERID", $('#hidHeaderID').val());
        formData.append("CMDETAILID", $("#hidDetaildID").val());
        formData.append("CMDETAILID1", $("#hidDetaildID1").val());
        formData.append("CM_REQ_TYPE", requestType);
        formData.append("REQUEST_TYPE", reqTypeVal);
        formData.append("CUST_ACC_TYPE", data.account_type);
        formData.append("CUST_ACC_TYPE1", data.ac_55AGW);
        formData.append("SALES_ORG", $("#txtSalesOrganization").val());
        formData.append("COMPANY_CODE", $("#txtCompanyCode").val());
        formData.append("DIVISION_GRP", division);
        formData.append("DISTRIBTUION_CHH", $("#ddlDistribution").val());
        formData.append("CUSTOMER_CODE", $("#txtDealerCode").val());

        formData.append("INDUSTRY1", $("#txtIndustry1").val());
        formData.append("INDUSTRYCODE1", $("#txtIndustryCode1").val());
        formData.append("INDUSTRYNAME", $("#txtIndustryName").val());
        formData.append("FIRSTNAME", $("#txtFirstName").val());      

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/CustomerMgmt/SaveIndustryData",
            data: formData,
            datatype: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.RS == 1 && data.CMHEADERID > 0) {
                    $("#hidHeaderID").val(data.CMHEADERID);
                    $("#hidDetaildID").val(data.CMDEAILID1);
                    $("#hidDetaildID1").val(data.CMDEAILID2);
                    groupFlags["Ind_data"] = true;
                    ShowMessage("success", data.MESSAGE);
                    sweetAlert("Customer Master data", data.MESSAGE, "success"); 
                    checkAllGroupOfData();
                }
                else if (data.RS == 0 || data.res == -1) {
                    sweetAlert("", data.MESSAGE, "warning");
                    ShowMessage("error", data.MESSAGE);
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {
                
                sweetAlert("", "Oops... Something went wrong!.", "warning");
                ShowMessage("error", 'Error in AJAX calling!')
            }
        });


    } catch (ex) {       
        sweetAlert("", "Oops... Something went wrong!.", "warning");
        ShowMessage("error", ex.message || 'Unexpected error')
    }
}
///////////////////////Save Sale Data////////////////////////////////////////////////////////
function validateSalesData() {
    const errors = [];

    const $ddlExchangeRateType = $('#ddlExchangeRateType');
    const $ddlPriceGroup = $('#ddlPriceGroup');
    const $ddlSalesDistrict = $('#ddlSalesDistrict');
    const $ddlSalesOffice = $('#ddlSalesOffice');
    const $ddlSalesGroup = $('#ddlSalesGroup');
    const $ddlCustomerGroup = $('#ddlCustomerGroup');
    const $ddlSalesCurrency = $('#ddlSalesCurrency');
    const $ddlCustomerPrice = $('#ddlCustomerPrice');
    const $ddlDeliveryPriority = $('#ddlDeliveryPriority');
    const $ddlShippingConditions = $('#ddlShippingConditions');

    if (isEnabled($ddlExchangeRateType) && isInvalidSelect($ddlExchangeRateType)) errors.push('Exchange Rate Type');
    if (isEnabled($ddlPriceGroup) && isInvalidSelect($ddlPriceGroup)) errors.push('Price Group');
    if (isEnabled($ddlSalesDistrict) && isInvalidSelect($ddlSalesDistrict)) errors.push('Sales District');
    if (isEnabled($ddlSalesOffice) && isInvalidSelect($ddlSalesOffice)) errors.push('Sales Office');
    if (isEnabled($ddlSalesGroup) && isInvalidSelect($ddlSalesGroup)) errors.push('Sales Group');
    if (isEnabled($ddlCustomerGroup) && isInvalidSelect($ddlCustomerGroup)) errors.push('Customer Group');
    if (isEnabled($ddlSalesCurrency) && isInvalidSelect($ddlSalesCurrency)) errors.push('Sales Currency');
    if (isEnabled($ddlCustomerPrice) && isInvalidSelect($ddlCustomerPrice)) errors.push('Customer Price');
    if (isEnabled($ddlDeliveryPriority) && isInvalidSelect($ddlDeliveryPriority)) errors.push('Delivery Priority');
    if (isEnabled($ddlShippingConditions) && isInvalidSelect($ddlShippingConditions)) errors.push('Shipping Condition');

    const isInvalid = errors.length > 0;
    const err = isInvalid ? `Please fill ${errors.join(', ')}` : '';
   
    return { flag: isInvalid, err: err };
}
async function SaveSalesData() {
    try {

        var v1 = PrimaryValidateData();
        if (v1.flag) {
            ShowMessage("error", v1.err);
            return;
        }

        const requestType = $("input[name='ApplicationRequestType']:checked").val();
        const reqTypeVal = $("input[name='RequestType']:checked").val(); // "DM" or "EX"      

        // When request is C or U, validate bank data
        if (requestType == 'C') {
            const bv = validateSalesData();
            if (bv.flag) { ShowMessage("error", bv.err); return; }
        }
        var data = getAccountData();
        var division = getDivision();
        var formData = new FormData();

        formData.append("CMHEADERID", $('#hidHeaderID').val());
        formData.append("CMDETAILID", $("#hidDetaildID").val());
        formData.append("CMDETAILID1", $("#hidDetaildID1").val());
        formData.append("CM_REQ_TYPE", requestType);
        formData.append("REQUEST_TYPE", reqTypeVal);
        formData.append("CUST_ACC_TYPE", data.account_type);
        formData.append("CUST_ACC_TYPE1", data.ac_55AGW);
        formData.append("SALES_ORG", $("#txtSalesOrganization").val());
        formData.append("COMPANY_CODE", $("#txtCompanyCode").val());
        formData.append("DIVISION_GRP", division);
        formData.append("DISTRIBTUION_CHH", $("#ddlDistribution").val());
        formData.append("CUSTOMER_CODE", $("#txtDealerCode").val());
        
        formData.append("EXCHANGERATETYPE", $("#ddlExchangeRateType").val());
        formData.append("PRICEGROUP", $("#ddlPriceGroup").val());
        formData.append("SALESDISTRICT", $("#ddlSalesDistrict").val());
        formData.append("SALESOFFICE", $("#ddlSalesOffice").val());
        formData.append("SALESGROUP", $("#ddlSalesGroup").val());
        formData.append("CUSTOMERGROUP", $("#ddlCustomerGroup").val());
        formData.append("SALESCURRENCY", $("#ddlSalesCurrency").val());
        formData.append("CUSTPRICPROC1", $("#ddlCustomerPrice").val());
        formData.append("DELIVERYPRIORITYGROUP", $("#ddlDeliveryPriority").val());
        formData.append("SHIPPINGCONDITION", $("#ddlShippingConditions").val());       

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/CustomerMgmt/SaveSalesData",
            data: formData,
            datatype: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.RS == 1 && data.CMHEADERID > 0) {
                    $("#hidHeaderID").val(data.CMHEADERID);
                    $("#hidDetaildID").val(data.CMDEAILID1);
                    $("#hidDetaildID1").val(data.CMDEAILID2);
                    groupFlags["Sales_Data"] = true;
                    ShowMessage("success", data.MESSAGE);
                    sweetAlert("Customer Master data", data.MESSAGE, "success");
                    checkAllGroupOfData();
                }
                else if (data.RS == 0 || data.res == -1) {
                    sweetAlert("", data.MESSAGE, "warning");
                    ShowMessage("error", data.MESSAGE);
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {

                sweetAlert("", "Oops... Something went wrong!.", "warning");
                ShowMessage("error", 'Error in AJAX calling!')
            }
        });


    } catch (ex) {
        $("#fileuploadPAN").val("");
        $("#fileuploadCINDoc").val("");
        sweetAlert("", "Oops... Something went wrong!.", "warning");
        ShowMessage("error", ex.message || 'Unexpected error')
    }
}

/////////////////////CIN data save///////////////////////////////////////////////////////
function validateCINData() {
    const errors = [];

    const $txtCstno = $('#txtCstno');
    const $txtLstNo = $('#txtLstNo');
    const $txtInvoicingDates = $('#txtInvoicingDates');
    const $txtInvoicingListDates = $('#txtInvoicingListDates');
    const $txtServiceRegNo = $('#txtServiceRegNo');
    const $txtPanNumber = $('#txtPanNumber');
    const $txtPaymentGuarProc = $('#txtPaymentGuarProc');

    if (isEnabled($txtCstno) && isEmptyText($txtCstno)) errors.push('CST No.');
    if (isEnabled($txtLstNo) && isEmptyText($txtLstNo)) errors.push('LST No.');
    if (isEnabled($txtInvoicingDates) && isEmptyText($txtInvoicingDates)) errors.push('Invoicing Date');
    if (isEnabled($txtInvoicingListDates) && isEmptyText($txtInvoicingListDates)) errors.push('Invoicing List Date');
    if (isEnabled($txtServiceRegNo) && isEmptyText($txtServiceRegNo)) errors.push('Service Reg. No');
    if (isEnabled($txtPanNumber) && isEmptyText($txtPanNumber)) errors.push('PAN No.');
    if (isEnabled($txtPaymentGuarProc) && isEmptyText($txtPaymentGuarProc)) errors.push('Bank Street');

    const isInvalid = errors.length > 0;
    const err = isInvalid ? `Please fill ${errors.join(', ')}` : '';

    return { flag: isInvalid, err: err };
}
async function SaveCINData() {
    try {
      
        var v1 = PrimaryValidateData();
        if (v1.flag) {
            ShowMessage("error", v1.err);
            return;
        }

        const requestType = $("input[name='ApplicationRequestType']:checked").val();    
        const reqTypeVal = $("input[name='RequestType']:checked").val(); // "DM" or "EX"      

        // When request is C or U, validate bank data
        if (requestType == 'C') {
            const bv = validateCINData(); // from your previous jQuery function
            if (bv.flag) { ShowMessage("error", bv.err); return; }
        }
        var data = getAccountData();   

        const fileuploadPAN = document.getElementById("fileuploadPAN");
        const hasPANFile = !!(fileuploadPAN && fileuploadPAN.files && fileuploadPAN.files.length);
        const fileuploadCINDoc = document.getElementById("fileuploadCINDoc");
        const hasCINDocFile = !!(fileuploadCINDoc && fileuploadCINDoc.files && fileuploadCINDoc.files.length);
        const hyperPAN = $('#hyperPAN'); // hyperlink text mirrors server usage
        const hyperCINDoc = $('#hyperCINDoc');       

        if ((!hasCINDocFile) && $.trim(hyperCINDoc.text()) == '' && $('#ddlEInvoiceApplicable').val()=="N") {
            showError('Please upload CIN document.');
            return;
        }
        // File validation based on server rules
        if (requestType == 'C' && reqTypeVal=="DM" ) {
            if ((!hasPANFile) && $.trim(hyperPAN.text()) == '') {
                showError('Please upload PAN document.');
                return;
            }
           
        }
        else if (requestType == 'U' && ($('#txtPanNumber').val()).trim() != '') {
            if (hasPANFile && $.trim(hyperPAN.text()) == '') {
                showError('Please upload PAN document.');
                return;
            }          
        }
        var validFileExtensions = ['pdf'];
        var MB1 = 1024 * 1024; // 1,048,576 bytes
        if (hasPANFile) {
            var extension = $(fileuploadPAN).val().split('.').pop().toLowerCase();
            if ($.inArray(extension, validFileExtensions) == -1) {
                var _msg = "Sorry! Upload only 'pdf' file";
                sweetAlert(" ", _msg, "warning");
                return;
            }
           
            if (fileuploadPAN.size > MB1) {
                sweetAlert(" ", "Sorry! Max allowed file size is 1 mb", "warning");
                return;
            }
        }
        if (hasCINDocFile) {
            var extension1 = $(fileuploadCINDoc).val().split('.').pop().toLowerCase();
            if ($.inArray(extension1, validFileExtensions) == -1) {
                var _msg = "Sorry! Upload only 'pdf' file";
                sweetAlert(" ", _msg, "warning");
                return;
            }

            if (fileuploadCINDoc.size > MB1) {
                sweetAlert(" ", "Sorry! Max allowed file size is 1 mb", "warning");
                return;
            }
        }
        var division = getDivision();
        var formData = new FormData();

        formData.append("CMHEADERID", $('#hidHeaderID').val());
        formData.append("CMDETAILID", $("#hidDetaildID").val());
        formData.append("CMDETAILID1", $("#hidDetaildID1").val());
        formData.append("CM_REQ_TYPE", requestType);
        formData.append("REQUEST_TYPE", reqTypeVal);
        formData.append("CUST_ACC_TYPE", data.account_type);
        formData.append("CUST_ACC_TYPE1", data.ac_55AGW);
        formData.append("SALES_ORG", $("#txtSalesOrganization").val());
        formData.append("COMPANY_CODE", $("#txtCompanyCode").val());
        formData.append("DIVISION_GRP", division);
        formData.append("DISTRIBTUION_CHH", $("#ddlDistribution").val());
        formData.append("CUSTOMER_CODE", $("#txtDealerCode").val());

        formData.append("CSTNO", $("#txtCstno").val());
        formData.append("LSTNO", $("#txtLstNo").val());
        formData.append("INVOICINGDATES", $("#txtInvoicingDates").val());
        formData.append("INVOICINGLISTDATES", $("#txtInvoicingListDates").val());
        formData.append("SERREGNO", $("#txtServiceRegNo").val());
        formData.append("PANNUMBER", $("#txtPanNumber").val());
        formData.append("PAYMENTGUARANTEEPROC", $("#txtPaymentGuarProc").val());
        formData.append("E_INVOICE_APPLICABLE", $("#ddlEInvoiceApplicable").val());
        formData.append("CIN_EINVOICE_DOC_FILE", $("#hyperPAN").text());
        formData.append("CIN_PAN_NO_FILE", $("#hyperCINDoc").text());//        
        formData.append("FILE1", document.getElementById("fileuploadPAN").files[0]);
        formData.append("FILE2", document.getElementById("fileuploadCINDoc").files[0]);

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/CustomerMgmt/SaveCINData",
            data: formData,
            datatype: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.RS == 1 && data.CMHEADERID > 0) {
                    $("#hidHeaderID").val(data.CMHEADERID);
                    $("#hidDetaildID").val(data.CMDEAILID1);
                    $("#hidDetaildID1").val(data.CMDEAILID2);
                    groupFlags["Cin_data"] = true;
                    ShowMessage("success", data.MESSAGE);
                    sweetAlert("Customer Master data", data.MESSAGE, "success");

                    if (data.FILE1 != null) {
                        hyperPAN.text(data.FILE1).show();
                        hyperPAN.off('click').on('click', function (e) {
                            e.preventDefault();
                            openDialog('../../Uploads/CustomerMaster/' + data.FILE1, 980, 680);
                        });
                    }
                    if (data.FILE2 != null) {
                        hyperCINDoc.text(data.FILE2).show();
                        hyperCINDoc.off('click').on('click', function (e) {
                            e.preventDefault();
                            openDialog('../../Uploads/CustomerMaster/' + data.FILE2, 980, 680);
                        });
                    }

                    checkAllGroupOfData();
                }
                else if (data.RS == 0 || data.res == -1) {
                    sweetAlert("", data.MESSAGE, "warning");
                    $("#fileuploadPAN").val("");
                    $("#fileuploadCINDoc").val("");
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {
                $("#fileuploadPAN").val("");
                $("#fileuploadCINDoc").val("");
                sweetAlert("", "Oops... Something went wrong!.", "warning");
                ShowMessage("error", 'Error in AJAX calling!')
            }
        });


    } catch (ex) {
        $("#fileuploadPAN").val("");
        $("#fileuploadCINDoc").val("");
        sweetAlert("", "Oops... Something went wrong!.", "warning");
        ShowMessage("error", ex.message || 'Unexpected error')
    }
}
////////////////////////Save Company Data////////////////////////////////////////////////////////////////
function validateCompanyData() {
    const errors = [];
      
    const $ddlReconAccount = $('#ddlReconAccount');
    const $ddlIncoterms = $('#ddlIncoterms');
    const $ddlTermsOfPayment = $('#ddlTermsOfPayment');
    const $ddlCreditControlArea = $('#ddlCreditControlArea');
    const $ddlAccountAssignmentGroup = $('#ddlAccountAssignmentGroup');
    const $ddlTaxClassification = $('#ddlTaxClassification');   
    const $txtWithHoldingTaxType = $('#txtWithHoldingTaxType');
    const $txtWithHoldingTaxCode = $('#txtWithHoldingTaxCode');
    const $txtValidFrom = $('#txtValidFrom');
    const $txtValidTo = $('#txtValidTo');
    const $txtWithHoldingTaxNumber = $('#txtWithHoldingTaxNumber');
    const $txtPaymentMethods = $('#txtPaymentMethods');
    const $txtPaymentMethodSupplier = $('#txtPaymentMethodSupplier');
    const $txtHouseBank = $('#txtHouseBank');
    
    if (isEnabled($ddlReconAccount) && isInvalidSelect($ddlReconAccount)) errors.push('Recon Account');
    if (isEnabled($ddlIncoterms) && isInvalidSelect($ddlIncoterms)) errors.push('Incoterm');
    if (isEnabled($ddlTermsOfPayment) && isInvalidSelect($ddlTermsOfPayment)) errors.push('Term of Payment');
    if (isEnabled($ddlCreditControlArea) && isInvalidSelect($ddlCreditControlArea)) errors.push('Credit Control Area');
    if (isEnabled($ddlAccountAssignmentGroup) && isInvalidSelect($ddlAccountAssignmentGroup)) errors.push('Acc Assignment group');
    if (isEnabled($ddlTaxClassification) && isInvalidSelect($ddlTaxClassification)) errors.push('Tax Classification');

    if (isEnabled($txtWithHoldingTaxType) && isEmptyText($txtWithHoldingTaxType)) errors.push('With Holding Tax type');
    if (isEnabled($txtWithHoldingTaxCode) && isEmptyText($txtWithHoldingTaxCode)) errors.push('With Holding Tax code');
    if (isEnabled($txtValidFrom) && isEmptyText($txtValidFrom)) errors.push('Valid From');
    if (isEnabled($txtValidTo) && isEmptyText($txtValidTo)) errors.push('Valid To');
    if (isEnabled($txtWithHoldingTaxNumber) && isEmptyText($txtWithHoldingTaxNumber)) errors.push('With Holding Tax no.');
    if (isEnabled($txtPaymentMethods) && isEmptyText($txtPaymentMethods)) errors.push('Payment method');
    if (isEnabled($txtPaymentMethodSupplier) && isEmptyText($txtPaymentMethodSupplier)) errors.push('Payment Method Supplier');
    if (isEnabled($txtHouseBank) && isEmptyText($txtHouseBank)) errors.push('House Bank');

    const isInvalid = errors.length > 0;
    const err = isInvalid ? `Please fill ${errors.join(', ')}` : '';
   
    return { flag: isInvalid, err: err };
}
async function SaveCompanyData() {
    try {

        var v1 = PrimaryValidateData();
        if (v1.flag) {
            ShowMessage("error", v1.err);
            return;
        }

        const requestType = $("input[name='ApplicationRequestType']:checked").val();
        const reqTypeVal = $("input[name='RequestType']:checked").val(); // "DM" or "EX"      

        // When request is C or U, validate bank data
        if (requestType == 'C') {
            const bv = validateCompanyData(); // from your previous jQuery function
            if (bv.flag) { ShowMessage("error", bv.err); return; }
        }
        var data = getAccountData();

        const fileUploadRTO = document.getElementById("fileUploadRTO");
        const hasRTOFile = !!(fileUploadRTO && fileUploadRTO.files && fileUploadRTO.files.length);
        const fileUpLOILetter = document.getElementById("fileUpLOILetter");
        const hasLOIFile = !!(fileUpLOILetter && fileUpLOILetter.files && fileUpLOILetter.files.length);
        const fileUpOtherDocument = document.getElementById("fileUpOtherDocument");
        const hasOtherDocFile = !!(fileUpOtherDocument && fileUpOtherDocument.files && fileUpOtherDocument.files.length);
        const hyperRTO = $('#hyperRTO'); // hyperlink text mirrors server usage
        const hyperLOI = $('#hyperLOI');
        const hyperOtherDoc = $('#hyperOtherDoc');
       
        // File validation based on server rules
        if (requestType == 'C' && data.account_type == "5AG1") {
            if ((!hasRTOFile) && $.trim(hyperRTO.text()) == '') {
                showError('Please upload RTO document.');
                return;
            }
            if ((!hasLOIFile) && $.trim(hyperLOI.text()) == '') {
                showError('Please upload LOI document.');
                return;
            }
        }
        
        var validFileExtensions = ['pdf'];
        var MB1 = 1024 * 1024; // 1,048,576 bytes
        if (hasRTOFile) {
            var extension = $(fileUploadRTO).val().split('.').pop().toLowerCase();
            if ($.inArray(extension, validFileExtensions) == -1) {
                var _msg = "Sorry! Upload only 'pdf' file";
                sweetAlert(" ", _msg, "warning");
                return;
            }
           
            if (fileUploadRTO.size > MB1) {
                sweetAlert(" ", "Sorry! Max allowed file size is 1 mb", "warning");
                return;
            }
        }
        if (hasLOIFile) {
            var extension1 = $(fileUpLOILetter).val().split('.').pop().toLowerCase();
            if ($.inArray(extension1, validFileExtensions) == -1) {
                var _msg = "Sorry! Upload only 'pdf' file";
                sweetAlert(" ", _msg, "warning");
                return;
            }

            if (fileUpLOILetter.size > MB1) {
                sweetAlert(" ", "Sorry! Max allowed file size is 1 mb", "warning");
                return;
            }
        }
        if (hasOtherDocFile) {
            var extension1 = $(fileUpOtherDocument).val().split('.').pop().toLowerCase();
            if ($.inArray(extension1, validFileExtensions) == -1) {
                var _msg = "Sorry! Upload only 'pdf' file";
                sweetAlert(" ", _msg, "warning");
                return;
            }

            if (fileUpOtherDocument.size > MB1) {
                sweetAlert(" ", "Sorry! Max allowed file size is 1 mb", "warning");
                return;
            }
        }
        var division = getDivision();
        var formData = new FormData();

        formData.append("CMHEADERID", $('#hidHeaderID').val());
        formData.append("CMDETAILID", $("#hidDetaildID").val());
        formData.append("CMDETAILID1", $("#hidDetaildID1").val());
        formData.append("CM_REQ_TYPE", requestType);
        formData.append("REQUEST_TYPE", reqTypeVal);
        formData.append("CUST_ACC_TYPE", data.account_type);
        formData.append("CUST_ACC_TYPE1", data.ac_55AGW);
        formData.append("SALES_ORG", $("#txtSalesOrganization").val());
        formData.append("COMPANY_CODE", $("#txtCompanyCode").val());
        formData.append("DIVISION_GRP", division);
        formData.append("DISTRIBTUION_CHH", $("#ddlDistribution").val());
        formData.append("CUSTOMER_CODE", $("#txtDealerCode").val());
     
        formData.append("RECONSACCOUNT", $("#ddlReconAccount").val());
        formData.append("INCOTERM1", $("#ddlIncoterms").val());
        formData.append("INCOTERM2", $('#ddlIncoterms option:selected').text());
        formData.append("TERMOFPAYMENT", $("#ddlTermsOfPayment").val());
        formData.append("CREDITCONTROLAREA", $("#ddlCreditControlArea").val());
        formData.append("ACCASSIGMENTGROUP", $("#ddlAccountAssignmentGroup").val());
        formData.append("TAXCLASSIFICATION", $("#ddlTaxClassification").val());
        formData.append("WITHHOLDINGTAXTYPE", $("#txtWithHoldingTaxType").val());
        formData.append("WITHHOLDINGTAXCODE", $("#txtWithHoldingTaxCode").val());
        formData.append("VALIDFROM", $("#txtValidFrom").val());
        formData.append("VALIDTO", $("#txtValidTo").val());
        formData.append("WITHHOLDINGTAXNUMBER", $("#txtWithHoldingTaxNumber").val());
        formData.append("PAYMENTMETHODS", $("#txtPaymentMethods").val());
        formData.append("PMTMETHSUPL", $("#txtPaymentMethodSupplier").val());
        formData.append("HOUSEBANK", $("#txtHouseBank").val());
        
        formData.append("FILE1", document.getElementById("fileUploadRTO").files[0]);
        formData.append("FILE2", document.getElementById("fileUpLOILetter").files[0]);
        formData.append("FILE3", document.getElementById("fileUpOtherDocument").files[0]);

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/CustomerMgmt/SaveCompanyData",
            data: formData,
            datatype: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.RS == 1 && data.CMHEADERID > 0) {
                    $("#hidHeaderID").val(data.CMHEADERID);
                    $("#hidDetaildID").val(data.CMDEAILID1);
                    $("#hidDetaildID1").val(data.CMDEAILID2);
                    groupFlags["Cin_data"] = true;
                    ShowMessage("success", data.MESSAGE);
                    sweetAlert("Customer Master data", data.MESSAGE, "success");

                    if (data.FILE1 != null) {
                        hyperRTO.text(data.FILE1).show();
                        hyperRTO.off('click').on('click', function (e) {
                            e.preventDefault();
                            openDialog('../../Uploads/CustomerMaster/' + data.FILE1, 980, 680);
                        });
                    }
                    if (data.FILE2 != null) {
                        hyperLOI.text(data.FILE2).show();
                        hyperLOI.off('click').on('click', function (e) {
                            e.preventDefault();
                            openDialog('../../Uploads/CustomerMaster/' + data.FILE2, 980, 680);
                        });
                    }
                    if (data.FILE3 != null) {
                        hyperOtherDoc.text(data.FILE2).show();
                        hyperOtherDoc.off('click').on('click', function (e) {
                            e.preventDefault();
                            openDialog('../../Uploads/CustomerMaster/' + data.FILE3, 980, 680);
                        });
                    }
                    checkAllGroupOfData();
                }
                else if (data.RS == 0 || data.res == -1) {
                    sweetAlert("", data.MESSAGE, "warning");
                    $("#fileuploadPAN").text("");
                    $("#fileUpLOILetter").val("");
                    $("#fileUpOtherDocument").val("");
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {
                $("#fileuploadPAN").val("");
                $("#fileUpLOILetter").val("");
                $("#fileUpOtherDocument").val("");
                sweetAlert("", "Oops... Something went wrong!.", "warning");
                ShowMessage("error", 'Error in AJAX calling!')
            }
        });


    } catch (ex) {
        $("#fileuploadPAN").val("");
        $("#fileUpLOILetter").val("");
        $("#fileUpOtherDocument").val("");
        sweetAlert("", "Oops... Something went wrong!.", "warning");
        ShowMessage("error", ex.message || 'Unexpected error')
    }
}
///////////////////////////////Final Submit///////////////////////////////////////////////////////////////////////////
function validateSubmit() {
    var hidHeaderID = $('#hidHeaderID').val().trim();

    if (hidHeaderID === '' || hidHeaderID === '0') {
        ShowMessage('error','Invalid customer request');
        return false;
    }

    var secHeadId = parseInt($('#ddlApprovalAuthority').val(), 10) || 0;
    if (secHeadId <= 0) {
        ShowMessage('error', 'Please select approval Authority!');
        return false;
    }

    var deptDivId = parseInt($('#ddlDeptDivHead').val(), 10) || 0;
    if (deptDivId <= 0) {
        ShowMessage('error', 'Please select Department/Division Head!');
        return false;
    }

    if (!$('#chkTerms').is(':checked')) {
        ShowMessage('error', 'Please check terms & conditions!');
        return false;
    }

    var remarks = $('#txt_Remarks').val().trim();
    if (remarks.length <= 0) {
        ShowMessage('error', 'error','Please enter remarks!');
        return false;
    }

    //var Gen_data = readBoolFromHidden('#hidGen_data');
    //var Bank_Data = readBoolFromHidden('#hidBank_Data');
    //var Ind_data = readBoolFromHidden('#hidInd_data');
    //var Cin_data = readBoolFromHidden('#hidCin_data');
    //var Sales_Data = readBoolFromHidden('#hidSales_Data');
    //var Comp_Data = readBoolFromHidden('#hidComp_Data');

    var Gen_data = groupFlags["Gen_data"];
    var Bank_Data = groupFlags["Bank_Data"];
    var Ind_data = groupFlags["Ind_data"];
    var Cin_data = groupFlags["Cin_data"];
    var Sales_Data = groupFlags["Sales_Data"];
    var Comp_Data = groupFlags["Comp_Data"];

    var reqVal = $('input[name="ApplicationRequestType"]:checked').val(); // C or U

    if (reqVal == 'C') {
        if (!Gen_data) { ShowMessage('error','Please check General Data!'); return false; }
        if (!Bank_Data) { ShowMessage('error', 'Please check Bank Data!'); return false; }
        if (!Ind_data) { ShowMessage('error', 'Please check Industry Data!'); return false; }
        if (!Cin_data) { ShowMessage('error', 'Please check CIN Data!'); return false; }
        if (!Sales_Data) { ShowMessage('error', 'Please check Sales Data!'); return false; }
        if (!Comp_Data) { ShowMessage('error', 'Please check Company Data!'); return false; }
    } else {
        if (!Gen_data) { ShowMessage('error', 'Please check General Data!'); return false; }
    }

    return true;
}
function finalsubmit(id) {
    try {
        if (!validateSubmit()) return;
        var hidHeaderID = parseInt($('#hidHeaderID').val().trim(), 10);
        var dealerCode = $('#txtDealerCode').val() || '';
        var name1 = $('#txtName1').val() || '';
        var hidGenDetailID = $('#hidGenDetailID').val() || '';

        var $btn = $(id).prop('disabled', true).text('Submitting...');
        var payload = {
            SEC_HEAD_ID: parseInt($('#ddlApprovalAuthority').val(), 10),
            DEPT_DIV_ID: parseInt($('#ddlDeptDivHead').val(), 10),
            REMARKS: $('#txt_Remarks').val().trim(),
            cmhid: hidHeaderID,
            hidGenDetailID: hidGenDetailID,
            DealerCode: dealerCode,
            Name1: name1
        };

       
        $.ajax({
            url: '/CustomerMgmt/SubmitCutomerRequest',
            type: 'POST',
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(payload),
            success: function (res) {                
              
                if (res.RS == 1) {
                    $btn.prop('disabled', true).text('Submit');
                    $('#btnPreview').prop('disabled', true);
                    ShowMessage("success", res.MESSAGE); 
                    sweetAlert("Customer Master data", res.MESSAGE, "success");
                   
                } else if (res.RS == 0) {
                    $btn.prop('disabled', true).text('Submit');
                    ShowMessage("error", res.MESSAGE); 
                    sweetAlert("Customer Master data", res.MESSAGE, "warning");
                }
            },
            error: function (xhr) {
                $btn.prop('disabled', false).text('Submit');
                sweetAlert("", "Oops... Something went wrong!.", "warning");
            },
            complete: function () {
               
                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (ex) {
        $btn.prop('disabled', false).text('Submit');
        console.long(ex);
        sweetAlert("Customer Master data", "Oops... Something went wrong!.", "warning");
    }

        
   
}
/////////////////////////////////Search Update//////////////////////////////////////////////////////////////    
function padLeftNumeric(str, totalLen, ch) {
    str = String(str || '').trim();
    if (/^\d+$/.test(str) && str.length < totalLen) {
        return str.padStart(totalLen, ch || '0');
    }
    return str;
}
function SearchDealerCode(id) {
    try { 
        let customerCode = $('#txtSearchCustomerID').val().trim();
       
     if (customerCode.length == 0) {
         ShowMessage('error','Please enter Customer Code.');
            return;
        }      
        customerCode = padLeftNumeric(customerCode, 10, '0');
        $('#txtSearchCustomerID').val(customerCode);
      
     const accKey = '50AG';
     const selected = document.querySelector('input[name="RequestType"]:checked');
     if ($(selected).val() == "C") { return; }
     $(id).prop('disabled', true).text('Searching...');
     var data = {
         CODE: $(selected).val(),
         CODE_DESC: customerCode
     }

     $.ajax({
         type: "POST",
         beforeSend: function () {
             $("#ajaxLoader").addClass('loader');
         },
         url: "/CustomerMgmt/btnSearch",
         data: JSON.stringify(data),
         contentType: "application/json; charset=utf-8",
         datatype: "json",
         success: function (data) {
             if (data.error == "error") {
                 ShowMessage("error", data.message);
                 sweetAlert("Customer Master", data.message, "error");
             }
             else if (data.error == "Y") {
                 sweetAlert("Customer Master", data.message, "warning");                 
             }
             else if (data.error == "N") {
                 $('#txtDealerCode').val(data.CUSTOMER || '');
                 $('#txtDealerCode').prop('disabled', true);
                 $('#txtName1').val(data.NAME || '');
                 $('#txtName2').val(data.NAME_2 || '');
                 $('#txtName3').val(data.NAME_3 || '');
                 $('#txtName4').val(data.NAME_4 || '');
                 setAccountData(data.ACCNT_GRP);
                 setDivisionGrpByValue(data.DIVISION);
                 $('#ddlDistribution').val(data.DISTRCHN);
                 setMandatoryField();
                 initGroupOfData();
                 ShowMessage("success", "Please click on 'Save and Next' Button of Genral Data for Extend/Update Request.");
             }
         },
         error: function () {
             $btn.prop('disabled', false).text('Search');
             $("#ajaxLoader").removeClass('loader');
             sweetAlert("Oops...", "Something went wrong!", "error");
         },
         complete: function () {
             $(id).prop('disabled', false).text('Search');
             $("#ajaxLoader").removeClass('loader');
         }
     });

    } catch (ex) {
        $("#ajaxLoader").removeClass('loader');
        $(id).prop('disabled', false).text('Search');
        console.long(ex);
        sweetAlert("Customer Master data", "Oops... Something went wrong!.", "warning");
    }
    }
//////////////////////////////////verify bank data///////////////////////////////////////////////////////
function ResetAndEnableFields() {
    // Reset textboxes
    $('#txtBankName').val('');
    $('#txtBankStreet').val('');
    $('#txtBankCity').val('');
    $('#txtBankBranch').val('');  
    $('#ddlBankRegion').val('');   
    $('#txtBankName').prop('disabled', false);
    $('#txtBankStreet').prop('disabled', false);
    $('#txtBankCity').prop('disabled', false);
    $('#txtBankBranch').prop('disabled', false);
    $('#ddlBankRegion').prop('disabled', false);
}
function verifyBankData(id) {
    try {
        let bankey = $('#txtBankKey').val().trim();

        if (bankey.length == 0) {
            ShowMessage('error', 'Please enter Bank Key!');
            return;
        }
        let bankcountry = $('#ddlCtry').val().trim();
        if (bankcountry.length == 0) {
            ShowMessage('error', 'Please select Bank Country!');
            return;
        }
      
        $(id).prop('disabled', true).text('Verify Bank...');
        var data = {
            CODE: bankcountry,
            CODE_DESC: bankey 
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/CustomerMgmt/VerifyBankData",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.error == "error") {
                    ShowMessage("error", data.message);
                    sweetAlert("Customer Master", data.message, "error");
                }
                else if (data.error == "Y") {
                    ResetAndEnableFields();
                    $('#lblVerificationStatus').text(data.message); 
                    sweetAlert("Customer Master", data.message, "warning");
                }
                else if (data.error == "N") {
                    if (data.BANK_NAME != "") {
                        $('#txtBankName').val(data.BANK_NAME || '');
                        $('#txtBankName').prop('disabled', true);
                    }                   
                    if (data.REGION != "") {
                        $('#ddlBankRegion').val(data.REGION || '');
                        $('#ddlBankRegion').prop('disabled', true);
                    }
                    if (data.STREET != "") {
                        $('#txtBankStreet').val(data.STREET || '');
                        $('#txtBankStreet').prop('disabled', true);
                    }
                    if (data.CITY != "") {
                        $('#txtBankCity').val(data.CITY || '');
                        $('#txtBankCity').prop('disabled', true);
                    }
                    if (data.BANK_BRANCH != "") {
                        $('#txtBankBranch').val(data.BANK_BRANCH || '');
                        $('#txtBankBranch').prop('disabled', true);
                    }
                    $('#lblVerificationStatus').text(data.message);                 
                    $(id).prop('disabled', false).text('Verify Bank');
                }

            },
            error: function () {
                $(id).prop('disabled', false).text('Verify Bank');
                sweetAlert("Oops...", "Something went wrong!", "error");
                $("#ajaxLoader").removeClass('loader');
            },
            complete: function () {
                $(id).prop('disabled', false).text('Verify Bank');
                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (ex) {
        $(id).prop('disabled', false).text('Verify Bank');
        console.long(ex);
        sweetAlert("Customer Master data", "Oops... Something went wrong!.", "warning");
    }
}
////////////////////////////////////Reset Request/////////////////////////////////////////////////////////////////////
function setChecked(selector, val) { $(selector).prop('checked', !!val); }
async function ResetData(id) {
    try {
        const reqTypeVal = $("input[name='RequestType']:checked").val(); // "DM" or "EX"      
        var formData = new FormData();
        formData.append("REQUEST_TYPE", reqTypeVal);
        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/CustomerMgmt/ResetCustomerData",
            data: formData,
            datatype: "json",
            contentType: false,
            processData: false,
            success: function (data) {
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {

                sweetAlert("", "Oops... Something went wrong!.", "warning");
                ShowMessage("error", 'Error in AJAX calling!')
            }
        });
        $('#hidDetaildID, #hidDetaildID1, #hidHeaderID').val('');
        setDivisionGrpByValue("");
        setAccountData("");
        const $rList = $('input[name="RequestType"]');
        $rList.prop('checked', false);
        $('#rdlRequestTypeDM').prop('checked', true).trigger('change');
        setLabelMandatory("");
        ResetText();
        setTextMandatory("");
        
    } catch (ex) {
        console.log("error message" + ex);
        sweetAlert("", "Oops... Something went wrong!.", "warning");
        ShowMessage("error", 'Error in AJAX calling!')
    }
  
}
function ResetText() {
    
    // --- Textboxes / inputs ---
    $("#txtDealerCode, #ddlTitle, #txtName1, #txtName2, #txtName3, #txtName4, #txtSearchTerms, \
       #txtStreetHouse, #txtStreet2, #txtStreet3, #txtStreet4, #txtStreet5, #txtPostalCode, \
       #txtCity, #txtMobilePhone, #txtFax, #txtEmail, #txtTaxNumberGst, #txtVendorCode, \
       #txtBankKey, #txtBankAccount, #txtBankName, #txtBankStreet, #txtBankCity, #txtBankBranch, \
       #txtIndustry1, #txtIndustryCode1, #txtIndustryName, #txtFirstName, \
       #txtCstno, #txtLstNo, #txtInvoicingDates, #txtInvoicingListDates, #txtServiceRegNo, \
       #txtPanNumber, #txtPaymentGuarProc, \
       #txtWithHoldingTaxType, #txtWithHoldingTaxCode, #txtValidFrom, #txtValidTo, \
       #txtWithHoldingTaxNumber, #txtPaymentMethodSupplier, #txtPaymentMethods, #txtHouseBank"
    ).val("");

    // --- Dropdowns: set to first option ---
    $("#ddlCountry, #ddlRegion, #ddlTimeZone, #ddlTransportationCode, \
       #ddlIndustry, #ddlCityCode, #ddCustomerClass, \
       #ddlCtry, #ddlAccountHolder, #ddlBankControlKey, #ddlBankRegion, #ddlBankCurrency, \
       #ddlExchangeRateType, #ddlPriceGroup, #ddlSalesDistrict, #ddlSalesOffice, #ddlSalesGroup, \
       #ddlCustomerGroup, #ddlSalesCurrency, #ddlCustomerPrice, #ddlDeliveryPriority, #ddlShippingConditions, \
       #ddlReconAccount, #ddlIncoterms, #ddlTermsOfPayment, #ddlCreditControlArea, \
       #ddlAccountAssignmentGroup, #ddlTaxClassification"
    ).each(function () {           
            $(this).prop('selectedIndex', 0);
            $(this).trigger('change');
    });
   
     
    //$("#ddlCountry, #ddlRegion, #ddlTimeZone, #ddlTransportationCode, \
    //   #ddlIndustry, #ddlCityCode, #ddCustomerClass, \
    //   #ddlCtry, #ddlAccountHolder, #ddlBankControlKey, #ddlBankRegion, #ddlBankCurrency, \
    //   #ddlExchangeRateType, #ddlPriceGroup, #ddlSalesDistrict, #ddlSalesOffice, #ddlSalesGroup, \
    //   #ddlCustomerGroup, #ddlSalesCurrency, #ddlCustomerPrice, #ddlDeliveryPriority, #ddlShippingConditions, \
    //   #ddlReconAccount, #ddlIncoterms, #ddlTermsOfPayment, #ddlCreditControlArea, \
    //   #ddlAccountAssignmentGroup, #ddlTaxClassification"
    //).each(function () {
    //    // Reset native
    //    $(this).prop('selectedIndex', 0);      
    //    $(this).trigger('change');
    //});

    // --- Hide sections/files/hyperlinks ---
    $("#tbWithTax, #hyperRTO, #hyperLOI, #hyperPAN, #hyperGSTNo, \
       #hyperBankMandate, #hyperCancelCheque, #hyperOtherDoc, #hyperCINDoc"
    ).hide();

   
    $("#txtValidFrom, #txtValidTo").each(function () {
        try {
            // Bootstrap Datepicker
            $(this).datepicker && $(this).datepicker('clearDates');
        } catch (e) {
            // jQuery UI Datepicker fallback
            try { $(this).datepicker('setDate', null); } catch (_) { }
        }
    });

    // --- If you maintain any client-side arrays (e.g., withholding list), clear them too ---
    if (window.withholdingList && Array.isArray(window.withholdingList)) {
        window.withholdingList.length = 0; // clear array in place
        if (typeof renderGrid === 'function') renderGrid();
        // Also clear concatenated hidden fields (already done above), but ensure consistent state
        $("#txtWithHoldingTaxType, #txtWithHoldingTaxCode, #txtValidFrom, #txtValidTo, #txtWithHoldingTaxNumber").val("");
    }
}



