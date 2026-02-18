window.addEventListener('DOMContentLoaded', () => {
    for (let sheet of document.styleSheets) {
        try {
            for (let i = 0; i < sheet.cssRules.length; i++) {
                let rule = sheet.cssRules[i];
                if (rule.selectorText === '.homeContent div') {
                    rule.style.removeProperty('padding');
                }
                if (rule.selectorText === '.homeContent') {
                    rule.style.removeProperty('padding');
                }
            }
        } catch (e) {
            console.warn("Could not access stylesheet:", e);
        }
    }
});

function GENINFOCLICK() {
    $("#GENINFO").css("background-color", "#BFE0A4");
    $("#BANKDET, #TAXDET, #MATCHDATA, #WTHOLDING").css("background-color", "#DADADA");

    $("#GENINFODIV").show();
    $("#BANKDETDIV, #TAXDETDIV, #MATCHDATADIV, #HOLDINGDIV").hide();
}

function BANKDETCLICK() {
    $("#BANKDET").css("background-color", "#BFE0A4");
    $("#GENINFO, #TAXDET, #MATCHDATA, #WTHOLDING").css("background-color", "#DADADA");

    $("#BANKDETDIV").show();
    $("#GENINFODIV, #TAXDETDIV, #MATCHDATADIV, #HOLDINGDIV").hide();
}

function TAXDETCLICK() {
    $("#TAXDET").css("background-color", "#BFE0A4");
    $("#GENINFO, #BANKDET, #MATCHDATA, #WTHOLDING").css("background-color", "#DADADA");

    $("#TAXDETDIV").show();
    $("#GENINFODIV, #BANKDETDIV, #MATCHDATADIV, #HOLDINGDIV").hide();
}

function MATCHDATACLICK() {
    $("#MATCHDATA").css("background-color", "#BFE0A4");
    $("#GENINFO, #BANKDET, #TAXDET, #WTHOLDING").css("background-color", "#DADADA");

    $("#MATCHDATADIV").show();
    $("#GENINFODIV, #BANKDETDIV, #TAXDETDIV, #HOLDINGDIV").hide();
}

function HOLDINGTAXDATACLICK() {
    $("#WTHOLDING").css("background-color", "#BFE0A4");
    $("#GENINFO, #BANKDET, #TAXDET, #MATCHDATA").css("background-color", "#DADADA");

    $("#HOLDINGDIV").show();
    $("#GENINFODIV, #BANKDETDIV, #TAXDETDIV, #MATCHDATADIV").hide();
}

function OpenGuideline(strOpenUrl) {
    var url = strOpenUrl;
    var popup = window.open(url, "WindowPopup", 'width=615px,height=340px,resizable=no,toolbar=no,menubar=no');
    popup.focus();
}

function confirmation() {
    
    document.getElementById('light').style.display = 'block';
}

function closepopup() {
    document.getElementById('light').style.display = 'none';
}

function onFormChange() {
    const model = {
        CreateVendor: document.getElementById("rdocreatevendor").checked,
        GeneralPurchase: document.getElementById("rdogeneralpurchase").checked,
        GPImport: document.getElementById("rdogpimport").checked,
        GroupCompanies: document.getElementById("rdogroupcompanies").checked,
        BOPVendor: document.getElementById("rdobopvendor").checked,
        ContractualEmp: document.getElementById("rdocontractualemp").checked
    };

    fetch('/VendorMaster/VendorAccGroupChanged', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(model)
    })
        .then(res => res.json())
        .then(data => {
            
            document.getElementById("trAccgrp").style.display = data.ShowAccGroup ? "table-cell" : "none";
            document.getElementById("trAccgrpList").style.display = data.ShowAccGroupList ? "table-row" : "none";
            document.getElementById("trAccgrp").innerHTML = data.AccGroupLabel || "";
            document.getElementById("TRVENDORCODE").style.display = data.ShowVendorCode ? "block" : "none";
        });
}

//document.addEventListener("DOMContentLoaded", onFormChange);
document.addEventListener("DOMContentLoaded", function () {
    // Attach change event to all radios in RequestType group
    document.querySelectorAll("input[name='RequestType']").forEach(function (radio) {
        radio.addEventListener("change", onFormChange);
    });
});
document.addEventListener("DOMContentLoaded", function () {
    // Attach change event to all radios in VendorAccountGroup
    document.querySelectorAll("input[name='VendorAccountGroup']").forEach(function (radio) {
        radio.addEventListener("change", onFormChange);
    });
});

function onNextToContinue() {
    

    let VendorAccGrp = "";
    let RequestType = "";
    let VENDORCODE = $.trim($("#VendorCode").val());

    //// Withholding Tax UI logic
    //if ($("#chkWithHoldingTax").is(":checked")) {
    //    $("#WTHOLDING").show();
    //} else {
    //    $("#HOLDINGDIV, #WTHOLDING").hide();
    //}

    // Request Type
    RequestType = $("#rdocreatevendor").is(":checked") ? "1" : "2";

    // Vendor Account Group
    if ($("#rdogeneralpurchase").is(":checked")) VendorAccGrp = "5AG1";
    else if ($("#rdogpimport").is(":checked")) VendorAccGrp = "5AG2";
    else if ($("#rdogroupcompanies").is(":checked")) VendorAccGrp = "5AG3";
    else if ($("#rdobopvendor").is(":checked")) VendorAccGrp = "5AGB";
    else if ($("#rdocontractualemp").is(":checked")) VendorAccGrp = "5AGA";

    // Validation
    if (RequestType === "") {
        //alert("Select Request Type");
        sweetAlert("Mandatory", "Select Request Type", "warning");
        return;
    }
    if (RequestType === "1" && VendorAccGrp === "") {
        //alert("Select Vendor Account Group");
        sweetAlert("Mandatory", "Select Vendor Account Group", "warning");
        return;
    }
    if ((RequestType === "2" || (RequestType === "1" && VendorAccGrp === "5AGA") || (RequestType === "1" && VendorAccGrp === "5AGB")) && VENDORCODE === "") {
        sweetAlert("Mandatory", "Vendor Code is mandatory field", "warning");
        return;
    }

    // File Upload
    let fileInput = document.getElementById("fileuploader");
    let file = fileInput.files[0];

    if (!file) {
        //alert("Please Upload Vendor Master CSV file");
        sweetAlert("Mandatory", "Please Upload Vendor Master CSV file", "warning");
        return;
    }

    let formData = new FormData();
    formData.append("file", file);

    $.ajax({
        url: "/VendorMaster/UploadFile",
        type: "POST",
        //beforeSend: function () {
        //    $("#ajaxLoader").addClass('loader');
        //},
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.status === 1) {
                
                let csvfilename = response.filename;
                let fullpath = response.fullpath;

                clearTextFields();
                $("#HDCSVVENDORMSTTEMPLATE").val(csvfilename);

                // Validate CSV
                const request = {
                    FileName: csvfilename,
                    RequestType: RequestType,
                    VendorAccGrp: VendorAccGrp,
                    VendorCode: VENDORCODE
                };
                $.ajax({
                    url: "/VendorMaster/ValidateCSV",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(request),
                    success: function (result) {
                        if (result.Status === 0) {
                            if (result.Message && result.Message.includes('<li>')) {
                                var errmsg = "<ul>" + result.Message + "</ul>";
                                const $div = $('#ERRMSGDIV');
                                $div.html(errmsg).show().attr('tabindex', '-1').focus();
                            } else {
                                sweetAlert("Error", result.Message, "error");
                                $('#ERRMSGDIV').hide();
                            }
                            return "0";
                        } else {
                            let data = result.Data;

                            // alert("Reading CSV successfully done.");
                            $("#HDVENDORNAME").val(data.VENDORNAME1);
                            $("#HDVENDORADDRESS").val(data.STREET1 + " " + data.STREET2 + " " + data.STREET3);
                            $("#HDADDRESS1").val(data.STREET1);
                            $("#HDADDRESS2").val(data.STREET2);
                            $("#HDADDRESS3").val(data.STREET3);
                            $("#HDVENDORCITY").val(data.CITY);
                            $("#HDVENDORACNO").val(data.BANKACCNO);
                            $("#HDVENDORPANNO").val(data.PANNUMBER);
                            // File attachment TR are shown when values are entered
                            if (data.VENDORNAME1 === "" && data.VENDORNAME2 === "" && data.VENDORNAME3 === "" &&
                                data.STREET1 === "" && data.STREET2 === "" && data.STREET3 === "" && data.STREET4 === "" &&
                                data.CITY === "" && data.REGION === "" && data.POSTALCODE === "" && data.COUNTRY === "") {
                                $("#TRREGCERTIFICATENO").hide();
                            } else { $("#TRREGCERTIFICATENO").show(); }

                            if (data.MSMEINFOSTATUS.toUpperCase() === "YES") {
                                $("#TRMSMECERTIFICATE").show();
                            } else { $("#TRMSMECERTIFICATE").hide(); }

                            if (data.IFSCCODE !== "") {
                                $("#TRCANCELCHEQUE").show();
                                $("#TRMANDATEFORM").show();
                            } else {
                                $("#TRCANCELCHEQUE").hide();
                                $("#TRMANDATEFORM").hide();
                            }

                            if (data.PANNUMBER !== "") { $("#TRPANCARD").show(); }
                            else { $("#TRPANCARD").hide(); }

                            if (data.GSTIN !== "") { $("#TRGSTCERTFICATE").show(); }
                            else { $("#TRGSTCERTFICATE").hide(); }

                            if (data.REQUESTTYPE === "2" && (data.MSMEINFOSTATUS.toUpperCase() === "NO" || data.MSMEINFOSTATUS === "")) {
                                $("#TRMSMECERTIFICATE").hide();
                            }

                            if (data.IFSCCODE === "") { $("#tabbanksupdoc").hide(); }

                            if (data.PANNUMBER === "" && data.GSTIN === "" &&
                                data.E_INVOICEApplicable.toUpperCase() !== "NO" && data.E_INVOICEApplicable.toUpperCase() === "NO") {
                                $("#tabletaxsupdoc").hide();
                            }

                            if (data.VENDORACCGRP === "5AGA") {
                                $("#spn_mandatefrm_mandate").hide();
                                $("#spn_regncertificate_mandate").hide();
                            }

                            if (data.VENDORACCGRP !== "5AGB") {
                                $("#TRCONFCERTIFICATE").hide();
                            }

                            if (data.E_INVOICEApplicable.toUpperCase() === "NO") {
                                $("#spnEinvoice").show();
                            } else { $("#spnEinvoice").hide(); }

                            if (data.LEIAPPLICABLE.toUpperCase() === "YES") {
                                $("#spnlei").show();
                            } else { $("#spnlei").hide(); }

                            $("#ERRMSGDIV").hide();

                            $.ajax({
                                url: "/VendorMaster/GetVendorListMatchDetails",
                                type: "POST",
                                contentType: "application/json",
                                data: JSON.stringify({
                                    VendorName: $("#HDVENDORNAME").val(),
                                    VendorAddress: $("#HDVENDORADDRESS").val(),
                                    VendorCity: $("#HDVENDORCITY").val(),
                                    VendorAcNo: $("#HDVENDORACNO").val(),
                                    VendorPanNo: $("#HDVENDORPANNO").val(),
                                    Address1: $("#HDADDRESS1").val(),
                                    Address2: $("#HDADDRESS2").val(),
                                    Address3: $("#HDADDRESS3").val()
                                }),
                                success: function (matchResult) {

                                    // Vendor Name
                                    if (matchResult.ods1.Table0.length > 0) {
                                        $("#divvname").show();
                                        $("#divvnamenodata").hide();
                                        bindRepeater("#REPVENDORNAME", matchResult.ods1.Table0);
                                    } else {
                                        $("#divvname").hide();
                                        $("#divvnamenodata").show();
                                    }

                                    // Vendor Address
                                    if (matchResult.ods1.Table1.length > 0) {
                                        $("#vaddressdiv").show();
                                        $("#vaddressdivnodata").hide();
                                        bindRepeater("#REPVENDORADDRESS", matchResult.ods1.Table1);
                                    } else {
                                        $("#vaddressdiv").hide();
                                        $("#vaddressdivnodata").show();
                                    }

                                    // Vendor Account
                                    if (matchResult.ods1.Table2.length > 0) {
                                        $("#vacdiv").show();
                                        $("#vacdivnodata").hide();
                                        bindRepeater("#REPVENDORAC", matchResult.ods1.Table2);
                                        $("#HDSPLAPPID").val("1");
                                    } else {
                                        $("#vacdiv").hide();
                                        $("#vacdivnodata").show();
                                    }

                                    // Vendor PAN
                                    if (matchResult.ods1.Table3.length > 0) {
                                        $("#divvpan").show();
                                        $("#divvpannodata").hide();
                                        bindRepeater("#REPVENDORPAN", matchResult.ods1.Table3);
                                        $("#HDSPLAPPID").val("1");
                                    } else {
                                        $("#divvpan").hide();
                                        $("#divvpannodata").show();
                                    }

                                    // Employee Address
                                    if (matchResult.ods1.Table4.length > 0) {
                                        $("#empaddressdiv").show();
                                        $("#divempaddressnodata").hide();
                                        bindRepeater("#REPEMPADDRESS", matchResult.ods1.Table4);
                                    } else {
                                        $("#empaddressdiv").hide();
                                        $("#divempaddressnodata").show();
                                    }

                                    // Actual Match (ods2)
                                    if (matchResult.ods2.Table0.length > 0) {
                                        $("#divvnameA").show();
                                        $("#divvnamenodataA").hide();
                                        bindRepeater("#REPVENDORNAMEA", matchResult.ods2.Table0);
                                        $("#HDSPLAPPID").val("1");
                                    } else {
                                        $("#divvnameA").hide();
                                        $("#divvnamenodataA").show();
                                    }

                                    if (matchResult.ods2.Table1.length > 0) {
                                        $("#vaddressdivA").show();
                                        $("#vaddressdivnodataA").hide();
                                        bindRepeater("#REPVENDORADDRESSA", matchResult.ods2.Table1);
                                        $("#HDSPLAPPID").val("1");
                                    } else {
                                        $("#vaddressdivA").hide();
                                        $("#vaddressdivnodataA").show();
                                    }

                                    // Request Type Handling
                                    if (RequestType === "2") {
                                        GETDETAILBEHALFOFVENDORCODE(VENDORCODE);
                                        GetDetailsfromCSV();
                                        $("#TableVendorfrm1, #TableVendorfrm2, #TableVendorfrm3").hide();
                                        $("#TableVendorfrmdetails").show();
                                        $("#Guidelinediv, #list_Guideline, #vendorheading").hide();

                                        //$("#divvname").hide();
                                        //$("#divvnamenodata").show();
                                        //$("#vaddressdiv").hide();
                                        //$("#vaddressdivnodata").show();
                                        //$("#vacdiv").hide();
                                        //$("#vacdivnodata").show();
                                        //$("#divvpan").hide();
                                        //$("#divvpannodata").show();

                                        //$("#HDSPLAPPID").val("0");
                                    }

                                    if (RequestType === "1" &&
                                        (VendorAccGrp === "5AG1" || VendorAccGrp === "5AG2" || VendorAccGrp === "5AG3")) {

                                        if (matchResult.odsExists && matchResult.odsExists.Table0.length > 0) {
                                            bindRepeater("#REPVENDORLIST", matchResult.odsExists.Table0);
                                            confirmation(); // call JS confirmation
                                        } else {
                                            GetDetailsfromCSV();
                                            $("#TableVendorfrm1, #TableVendorfrm2, #TableVendorfrm3").hide();
                                            $("#TableVendorfrmdetails").show();
                                            //$("#Guidelinediv, #list_Guideline, #vendorheading").hide();
                                            $("#Guidelinediv, #vendorheading").hide();
                                        }
                                    } else {
                                        GetDetailsfromCSV();
                                        $("#TableVendorfrm1, #TableVendorfrm2, #TableVendorfrm3").hide();
                                        $("#TableVendorfrmdetails").show();
                                        //$("#Guidelinediv, #list_Guideline, #vendorheading").hide();
                                        $("#Guidelinediv, #vendorheading").hide();
                                    }
                                }
                            });
                        }
                    }
                });
            } else {
                //alert(response.message);
                sweetAlert("Error", response.message, "error");
            }
        },
        error: function () {
            //alert("File upload failed due to a network or server error.");
            sweetAlert("Failed", "File upload failed due to a network or server error.", "error");
        }
    });
}

function clearTextFields() {
    //--------------General Information----------------------------
    $("#lblvendorname").text("");
    $("#lblstreethouseno").text("");
    $("#lblcity").text("");
    $("#lblregion").text("");
    $("#lblpostalcode").text("");
    $("#lblcountry").text("");
    $("#lblmobilenumber").text("");
    $("#lbltelphoneno").text("");
    $("#lblemailid1").text("");
    $("#lblemailid2").text("");
    $("#lblemailid3").text("");
    $("#lblmsmeinfostatus").text("");
    $("#lblmsmecategory").text("");
    $("#lblmsmecertno").text("");
    $("#lblserviceagentgrp").text("");
    //$("#lblTypeOfIndustry").text("");
    //$("#lblClassificationOfYear").text("");
    //$("#lblDateOfClassification").text("");
    //$("#lblNTypeOfIndustry").text("");
    //$("#lblNClassificationOfYear").text("");
    //$("#lblNDateOfClassification").text("");

    //--------------Bank Details----------------------------
    $("#lblbankcountry").text("");
    $("#lblbankname").text("");
    $("#lblbranchname").text("");
    $("#lblbankaddress").text("");
    $("#lbltypeofaccount").text("");
    $("#lblbankaccountno").text("");
    $("#lblifsccode").text("");
    $("#lblbankcategory").text("");
    $("#lblschemagroup").text("");
    $("#lblordercurrency").text("");
    $("#lblcity").text("");   // note: duplicate ID in your C# code, ensure unique IDs in HTML
    //$("#lblswiftcode").text("");
    //$("#lblibanno").text("");

    //--------------TDS Details----------------------------
    $("#lblpannumber").text("");

    //--------------Sales Tax/Service Details----------------------------
    //$("#lblcstregno").text("");
    //$("#lbllstno").text("");
    //$("#lblserviceregno").text("");
    //$("#lbleccno").text("");
    //$("#lblexciseregnno").text("");
    //$("#lblexciserange").text("");
    //$("#lblexcisedivision").text("");
    //$("#lblcommistionerate").text("");

    $("#lblgstin").text("");
    $("#lblgstclassification").text("");
    //$("#LblVendorNameCode").text("");
    //$("#lblEinvoiceApplicable").text("");
}

//function bindRepeater(containerId, rows) {
    
//    let html = "";
//    rows.forEach(row => {
//        html += "<tr>";
//        row.forEach(cell => {
//            html += "<td>" + cell + "</td>";
//        });
//        html += "</tr>";
//    });
//    $(containerId).html(html);
//}

function bindRepeater(containerId, rows) {
    let html = "";
    rows.forEach(row => {
        html += "<tr>";
        Object.values(row).forEach(cell => {
            html += "<td>" + cell + "</td>";
        });
        html += "</tr>";
    });
    $(containerId).html(html);
}

function GetVendorDetails() {
    var payload = $('#vendorPayload').data('payload') || {};
    $('#HDVENDORREQUESTID').val(payload.VendorId || '');

    var reqType = payload.RequestType || '';
    if (reqType === "1") {
        $('#rdocreatevendor').prop('checked', true);
    } else {
        $('#rdoupdatevendor').prop('checked', true);
    }

    var vendorAccGp = payload.VendorAccountGpId || '';
    // set account group radios if you have them (ids assumed)
    if (vendorAccGp === "5AG1") $('#rdogeneralpurchase').prop('checked', true);
    if (vendorAccGp === "5AG3") $('#rdogroupcompanies').prop('checked', true);
    if (vendorAccGp === "5AG2") $('#rdogpimport').prop('checked', true);
    if (vendorAccGp === "5AGB") $('#rdobopvendor').prop('checked', true);
    if (vendorAccGp === "5AGA") $('#rdocontractualemp').prop('checked', true);

    var showVendorCode = (reqType === "2") || (reqType === "1" && (vendorAccGp === "5AGA" || vendorAccGp === "5AGB"));
    if (showVendorCode) $('#TRVENDORCODE').show();
    $('#VendorCode').val(payload.VendorCode || '');

    bindFile('#AnchRegnCertificateno', '#fileupload_regcertificateno', '#LinkRegnCertificateno', payload.RegistrationCert, payload.FilePath);
    bindFile('#AnchConfCertificate', '#fileupload_Confcertificateno', '#LinkConfCertificate', payload.ConflictCert, payload.FilePath);
    bindFile('#AnchMsmeCertificate', '#fileupload_msmecertificate', '#LinkMsmeCertificate', payload.MsmeCert, payload.FilePath);
    bindFile('#AnchmandateForm', '#fileupload_mandateform', '#LinkMandateForm', payload.MandateForm, payload.FilePath);
    bindFile('#AnchCancelCheque', '#fileupload_cancelcheque', '#LinkCancelCheque', payload.CancelCheque, payload.FilePath);
    bindFile('#AnchPanCard', '#fileupload_pancard', '#LinkPanCard', payload.PanCard, payload.FilePath);
    bindFile('#AnchGSTCertificate', '#fileupload_GSTCERTNO', '#LinkGstCertificate', payload.GstCert, payload.FilePath);
    $('#lblEinvoiceApplicable').text(payload.EInvoiceApplicable === "1" ? "Yes" : "No");
    bindFile('#AnchEINVOICE', '#fileupload_EINVOICE', '#LinkBtnEINVOICE', payload.EInvoiceFile, payload.FilePath);
    bindFile('#AnchLEIC', '#fileupload_LEI', '#LinkBtnLEIC', payload.LeiFile, payload.FilePath);

    if (payload.SendBackRemark) {
        $('#sendbackremark').html(payload.SendBackRemark);
        $('#sendback_div').show();
    } else {
        $('#sendback_div').hide();
    }

    if (payload.HasInterBank && payload.InterBank && payload.InterBank.length > 0) {
        $('#TblInterBank').show();
    } else {
        $('#TblInterBank').hide();
    }

    if (payload.WithHolding && payload.WithHolding.length > 0) {
        renderTable(payload.WithHolding);
        $('#WTHOLDING').show();
    } else {
        $('#WTHOLDING').hide();
    }

    populateDeptDropdown(payload.DepartmentHeads || []);

    function bindFile(anchorSel, fileSel, removeSel, fileName, filepath) {
        if (fileName && fileName.length > 0) {
            $(anchorSel).attr('href', payload.FilePath + fileName).text(fileName).show();
            $(fileSel).hide();
            $(fileSel).prop("disabled", true);
            $(removeSel).show();
        } else {
            $(anchorSel).hide();
            $(fileSel).show();
            $(fileSel).prop("disabled", false);
            $(removeSel).hide();
        }
    }

    //function renderTable(selector, rows) {
    //    var $tbl = $(selector);
    //    $tbl.empty();
    //    if (!rows || rows.length === 0) return;
    //    var cols = Object.keys(rows[0]);
    //    var $thead = $('<thead><tr></tr></thead>');
    //    cols.forEach(function (c) { $thead.find('tr').append('<th>' + c + '</th>'); });
    //    $tbl.append($thead);
    //    var $tbody = $('<tbody></tbody>');
    //    rows.forEach(function (r) {
    //        var $tr = $('<tr></tr>');
    //        cols.forEach(function (c) { $tr.append('<td>' + (r[c] == null ? '' : r[c]) + '</td>'); });
    //        $tbody.append($tr);
    //    });
    //    $tbl.append($tbody);
    //}

    function populateDeptDropdown(list) {
        var $ddl = $('#ddlforward');
        $ddl.empty();
        list.forEach(function (i) {
            $ddl.append($('<option>').val(i.Value).text(i.Text));
        });
    }
}

$(function () {
    // Navigation buttons
    onFormChange();
    GetVendorDetails();
    $('#btncontinuetonext').on('click', function (e) {
        e.preventDefault();
        onNextToContinue();
    });
    $('#btncancelfrm').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/VendorMasterForm";
    });
    $('#btncancel').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/ManageRequest";
    });
    $('#btnbackimg, #btnbackspan').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/ManageRequest";
    });

    // Section tabs
    $("#GENINFO").on("click", GENINFOCLICK);
    $("#BANKDET").on("click", BANKDETCLICK);
    $("#TAXDET").on("click", TAXDETCLICK);
    $("#MATCHDATA").on("click", MATCHDATACLICK);
    $("#WTHOLDING").on("click", HOLDINGTAXDATACLICK);

    // Datepickers
    $("._date").datepicker({
        format: "dd-M-yyyy",
        autoclose: true,
        toolbarplacement: 'top'
    });

    $('#txtDateOnWhichExemptionBegin').datepicker({
        dateFormat: 'dd-M-yy',
        changeMonth: true,
        changeYear: true,
        showAnim: 'fadeIn',
        onSelect: function () {
            const d = $(this).datepicker('getDate');
            if (!d) return;
            const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            const day = ('0' + d.getDate()).slice(-2);
            const month = monthNames[d.getMonth()];
            const year = d.getFullYear();
            $(this).val(`${day}-${month}-${year}`);
        }
    });

    //// Initial load
    //var getListUrl = window.location.origin + "/VendorMaster/GetListAjax";

    //$.get(getListUrl, function (res) {
    //    renderTable(res.items);
    //});

    //// Add Holding Tax
    //$("#btnAddHoldingTax").on("click", function () {
    //    const obj = {
    //        Withholding_Tax_Type: $("#Withholding_Tax_Type").val(),
    //        Withholding_Tax_Code: $("#Withholding_Tax_Code").val(),
    //        Liable: $("#chkLiable").is(":checked"),
    //        Recipient_Type: $("#ddlRecipientType").val(),
    //        Withholding_Tax_Id_No: $("#txtWithholdingTaxIdNo").val(),
    //        Exemption_Certi_No: $("#txtExeptionCertiNo").val(),
    //        Exemption_Rate: $("#txtExemptionRate").val(),
    //        Date_On_Which_Exemption_Begins: $("#txtDateOnWhichExemptionBegin").val() ? new Date($("#txtDateOnWhichExemptionBegin").val()) : null,
    //        Date_On_Which_Exemption_Ends: $("#txtDateOnWhichExemptionEnds").val() ? new Date($("#txtDateOnWhichExemptionEnds").val()) : null,
    //        Reason_For_Exemption: $("#ddlReasonForExemption").val()
    //    };

    //    const clientErrors = [];
    //    if (!obj.Withholding_Tax_Type) clientErrors.push("Select Holding Tax Type");
    //    if (!obj.Recipient_Type) clientErrors.push("Select Recipient Type");
    //    if (!obj.Liable) clientErrors.push("Check Liable");

    //    if (obj.Date_On_Which_Exemption_Begins && obj.Date_On_Which_Exemption_Ends &&
    //        obj.Date_On_Which_Exemption_Begins > obj.Date_On_Which_Exemption_Ends) {
    //        clientErrors.push("Exemption ends date should be after exemption begin date");
    //    }

    //    if (clientErrors.length) {
    //        swal({
    //            title: "Please fix the following:",
    //            text: clientErrors.join("\n"),
    //            icon: "warning",
    //            button: "OK"
    //        });
    //        return;
    //    }

    //    $.ajax({
    //        url: '/VendorMaster/AddAjax',
    //        type: "POST",
    //        contentType: "application/json",
    //        data: JSON.stringify(obj),
    //        success: function (res) {
    //            renderTable(res.items);
    //        },
    //        error: handleAjaxError
    //    });
    //});
    //// Clear Holding Tax
    //$("#btnAddHoldingTaxClear").on("click", function () {
    //    if (!confirm("Clear all withholding entries?")) return;
    //    $.ajax({
    //        url: '/VendorMaster/ClearAjax',
    //        method: 'POST',
    //        headers: { 'RequestVerificationToken': getAntiForgeryToken() },
    //        success: function (res) {
    //            renderTable(res.items);
    //            $("#holdingForm")[0].reset();
    //        },
    //        error: function () {
    //            swal("Failed", "Unable to clear items", "warning");
    //        }
    //    });
    //});
    //// Delegated delete
    //$("#withholdingTable").on("click", ".btn-delete", function () {
    //    const id = $(this).data("id");
    //    $.ajax({
    //        url: '/VendorMaster/DeleteAjax',
    //        type: "GET",
    //        data: { id },
    //        success: function (res) {
    //            renderTable(res.items);
    //        },
    //        error: handleAjaxError
    //    });
    //});
    //// User Manual Open
    //$('#userManualLink').on('click', function (e) {
    //    e.preventDefault();
    //    window.open(
    //        this.href,
    //        'ApprovalNote',
    //        'left=150,top=20,width=800,height=600,toolbar=0,resizable=0'
    //    );
    //});
    $('#btncontinuerequest').on('click', function () {

        GetDetailsfromCSV();

        // Hide certain sections
        document.getElementById("TableVendorfrm1").style.display = "none";
        document.getElementById("TableVendorfrm2").style.display = "none";
        document.getElementById("TableVendorfrm3").style.display = "none";

        // Show details section
        document.getElementById("TableVendorfrmdetails").style.display = "block";

        // Hide guideline div
        document.getElementById("Guidelinediv").style.display = "none";
        closepopup();
    });
    $('#btncancelrequest').on('click', function () {
        closepopup();
    });
    //$("#Withholding_Tax_Type").change(function () {
    //    onHoldingTypeChange(this);
    //});
    $('#btnsubmit').on('click', function () {
        const csvfilename = $("#HDCSVVENDORMSTTEMPLATE").val();
        if (!csvfilename) return;

        const REQUESTTYPE = $("#rdocreatevendor").is(":checked") ? "1" : "2";
        const VENDORCODE = $("#VendorCode").val() ?? "";
        const FORWARDTO = $("#ddlforward").val() ?? "";
        const REMARKS = $("#txtvendorremarks").val() ?? "";
        const chkdeclaration = $("#chkdeclaration").is(":checked");
        //const chkWithHoldingTax = $("#chkWithHoldingTax").is(":checked");

        let VENDORACCGRP = "";
        if ($("#rdogeneralpurchase").is(":checked")) VENDORACCGRP = "5AG1";
        else if ($("#rdogpimport").is(":checked")) VENDORACCGRP = "5AG2";
        else if ($("#rdogroupcompanies").is(":checked")) VENDORACCGRP = "5AG3";
        else if ($("#rdobopvendor").is(":checked")) VENDORACCGRP = "5AGB";
        else if ($("#rdocontractualemp").is(":checked")) VENDORACCGRP = "5AGA";

        const dto = new FormData();
        dto.append("REQUESTTYPE", REQUESTTYPE);
        dto.append("VENDORACCGRP", VENDORACCGRP);
        dto.append("VENDORCODE", VENDORCODE);
        dto.append("FORWARDTO", FORWARDTO);
        dto.append("REMARKS", REMARKS);
        dto.append("csvfilename", csvfilename);
        dto.append("chkdeclaration", chkdeclaration);
        dto.append("VendorHeaderId", $("#HDVENDORREQUESTID").val())
        //dto.append("chkWithHoldingTax", chkWithHoldingTax);

        dto.append("FILE_REGCERTIFICATENO", $("#AnchRegnCertificateno").text());
        dto.append("FILE_MSMECERTIFICATE", $("#AnchMsmeCertificate").text());
        dto.append("FILE_CONFLICTCERTIFICATE", $("#AnchConfCertificate").text());
        dto.append("FILE_MANDATEFORM", $("#AnchmandateForm").text());
        dto.append("FILE_CANCELCHEQUE", $("#AnchCancelCheque").text());
        dto.append("FILE_PANCARD", $("#AnchPanCard").text());
        dto.append("FILE_GSTCERTIFICATE", $("#AnchGSTCertificate").text());
        dto.append("FILE_EINVOICE", $("#AnchEINVOICE").text());
        dto.append("FILE_LEICERTIFICATE", $("#AnchLEIC").text());

        $('#ERRMSGDIV').hide();

        // helper to append file if present
        function appendFile(inputId, fieldName) {
            const input = document.getElementById(inputId);
            if (input?.files?.[0]) {
                dto.append(fieldName, input.files[0]);
            }
        }

        appendFile("fileupload_regcertificateno", "HDFILE_REGCERTIFICATENO");
        appendFile("fileupload_mandateform", "HDFILE_MANDATEFORM");
        appendFile("fileupload_cancelcheque", "HDFILE_CANCELCHEQUE");
        appendFile("fileupload_pancard", "HDFILE_PANCARD");
        appendFile("fileupload_msmecertificate", "HDFILE_MSMECERTIFICATE");
        appendFile("fileupload_GSTCERTNO", "HDNFILE_GSTCERTIFICATE");
        appendFile("fileupload_EINVOICE", "HDNFILE_EINVOICE");
        appendFile("fileupload_LEI", "HDNFILE_LEI");
        appendFile("fileupload_conflictcertificate", "HDFILE_conflictCERTIFICATE");

        $.ajax({
            url: '/VendorMaster/UpdateVendorData',
            method: 'POST',
            data: dto,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response) {
                    if (response.success) {
                        swal({
                            title: "Success!",
                            text: response.message,
                            type: "success"   // in v1 use 'type' instead of 'icon'
                        }, function () {
                            window.location.href = window.location.origin + "/VendorMaster/ManageRequest";
                        });
                    } else {
                        if (response.message?.includes('<ul>')) {
                            $('#ERRMSGDIV').html(response.message).show().attr('tabindex', '-1').focus();
                        } else {
                            swal("Error", response.message, "error");
                            $('#ERRMSGDIV').hide();
                        }
                    }
                } else {
                    swal("Failed", "Unable to submit!", "error");
                }
            },
            error: handleAjaxError
        });
    });

    // Bind events
    $("#LinkMandateForm").on("click", function () {
        removeAttachment("AnchmandateForm", "fileupload_mandateform", "LinkMandateForm");
    });

    $("#LinkRegnCertificateno").on("click", function () {
        removeAttachment("AnchRegnCertificateno", "fileupload_regcertificateno", "LinkRegnCertificateno");
    });

    $("#LinkMsmeCertificate").on("click", function () {
        removeAttachment("AnchMsmeCertificate", "fileupload_msmecertificate", "LinkMsmeCertificate");
    });

    $("#LinkConfCertificate").on("click", function () {
        removeAttachment("AnchConfCertificate", "fileupload_Confcertificateno", "LinkConfCertificate");
    });

    $("#LinkCancelCheque").on("click", function () {
        removeAttachment("AnchCancelCheque", "fileupload_cancelcheque", "LinkCancelCheque");
    });

    $("#LinkPanCard").on("click", function () {
        removeAttachment("AnchPanCard", "fileupload_pancard", "LinkPanCard");
    });

    $("#LinkGstCertificate").on("click", function () {
        removeAttachment("AnchGSTCertificate", "fileupload_GSTCERTNO", "LinkGstCertificate");
    });

    $("#LinkBtnEINVOICE").on("click", function () {
        removeAttachment("AnchEINVOICE", "fileupload_EINVOICE", "LinkBtnEINVOICE");
    });

    $("#LinkBtnLEIC").on("click", function () {
        removeAttachment("AnchLEIC", "fileupload_LEI", "LinkBtnLEIC");
    });
});
function removeAttachment(anchorId, fileId, linkId) {
    $("#" + anchorId).text("").hide();
    $("#" + fileId).prop("disabled", false);
    $("#" + fileId).show();
    $("#" + linkId).hide();
}

// Centralized error handler
function handleAjaxError(xhr) {
    const r = xhr.responseJSON;
    if (r && r.errors && r.errors.length) {
        swal({
            title: "Errors",
            text: r.errors.join("\n"),
            icon: "error",
            button: "OK"
        });
    } else {
        swal("Failed", "Operation could not be completed", "error");
    }
}

function GETDETAILBEHALFOFVENDORCODE(VENDORCODE) {
    // show tables
    $('#tablegeneralinfo').show();
    $('#tablebankdetail').show();
    $('#tabletaxdetail').show();

    $.ajax({
        url: '/VendorMaster/getdetailbehalfofvendorcode',
        method: 'GET',
        data: { vendorCode: VENDORCODE },
        dataType: 'json',
        success: function (response) {
            // response is expected to be a single object. If your API returns an array adjust accordingly.
            if (!response) return;
            // OLD VENDOR DETAIL
            $('#lbloldvendoraccgrp').text(response.VENDORACCOUNTGRP || '');
            $('#lblvendoraccgrp').text(response.VENDORACCOUNTGRP || '');
            var VENDORACCGRPID = response.VENDORACCGRPID || '';

            // Clear radios first
            $('#rdogeneralpurchase').prop('checked', false);
            $('#rdogpimport').prop('checked', false);
            $('#rdogroupcompanies').prop('checked', false);
            $('#rdocontractualemp').prop('checked', false);
            $('#rdobopvendor').prop('checked', false);

            // Reset oldInterbankdetail visibility
            $('#tableoldInterbankdetail').hide();

            if (VENDORACCGRPID === '5AG1') {
                $('#rdogeneralpurchase').prop('checked', true);
            } else if (VENDORACCGRPID === '5AG2') {
                $('#rdogpimport').prop('checked', true);
                //$('#tableoldInterbankdetail').show();
            } else if (VENDORACCGRPID === '5AG3') {
                $('#rdogroupcompanies').prop('checked', true);
            } else if (VENDORACCGRPID === '5AGA') {
                $('#rdocontractualemp').prop('checked', true);
            } else if (VENDORACCGRPID === '5AGB') {
                $('#rdobopvendor').prop('checked', true);
            }

            BindExistingVendorData(response);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching vendor details:', error);
        }
    });
}

function GetDetailsfromCSV() {
    
    let csvfilename = document.getElementById("HDCSVVENDORMSTTEMPLATE").value;
    if (typeof clearTextFields === 'function') {
        clearTextFields();
    }

    $('#lblvendorname').text('');
    $('#lblstreethouseno').text('');

    $.ajax({
        url: '/VendorMaster/GetDetailsfromCSV',
        method: 'GET',
        data: { csvfilename: csvfilename },
        dataType: 'json',
        success: function (response) {
            if (!response) return;
            if ($('#rdocreatevendor').length && $('#rdocreatevendor').prop('checked') === true) {
                $('#lbltypeofrequest').text('Create Vendor');
            } else {
                $('#lbltypeofrequest').text('Update Vendor');
            }

            if ($('#rdogeneralpurchase').length && $('#rdogeneralpurchase').prop('checked') === true) {
                $('#lblvendoraccgrp').text('General Purchase');
            } else if ($('#rdogpimport').length && $('#rdogpimport').prop('checked') === true) {
                $('#lblvendoraccgrp').text('GP Import');
            } else if ($('#rdogroupcompanies').length && $('#rdogroupcompanies').prop('checked') === true) {
                $('#lblvendoraccgrp').text('Group Companies');
            } else if ($('#rdobopvendor').length && $('#rdobopvendor').prop('checked') === true) {
                $('#lblvendoraccgrp').text('BOP Vendor');
            } else if ($('#rdocontractualemp').length && $('#rdocontractualemp').prop('checked') === true) {
                $('#lblvendoraccgrp').text('Contractual Employees');
            }

            // If txtvendorcode has text, show TRVENDORCODEDET and set lblvendorcode (same logic)
            var txtVendorCodeVal = ($('#VendorCode').length) ? $('#VendorCode').val() : '';
            if (txtVendorCodeVal && txtVendorCodeVal.toString() !== '') {
                $('#TRVENDORCODEDET').show();
                $('#lblvendorcode').text(txtVendorCodeVal.toString().trim());
            }
            // --- Populate fields (same as before) ---
            $('#lblvendorname').text(response.lblvendorname || '');
            $('#lblstreethouseno').text(response.lblstreethouseno || '');
            $('#lblbranchname').text(response.lblbranchname || '');
            $('#lblbankaddress').text(response.lblbankaddress || '');
            $('#LblVendorNameCode').text(response.LblVendorNameCode || '');

            $('#lblcity').text(response.lblcity || '');
            $('#lblregion').text(response.lblregion || '');
            $('#lblpostalcode').text(response.lblpostalcode || '');
            $('#lblcountry').text(response.lblcountry || '');
            $('#lblmobilenumber').text(response.lblmobilenumber || '');
            $('#lbltelphoneno').text(response.lbltelphoneno || '');
            $('#lblemailid1').text(response.lblemailid1 || '');
            $('#lblemailid2').text(response.lblemailid2 || '');
            $('#lblemailid3').text(response.lblemailid3 || '');
            $('#lblmsmeinfostatus').text(response.lblmsmeinfostatus || '');
            $('#lblmsmecategory').text(response.lblmsmecategory || '');
            $('#lblmsmecertno').text(response.lblmsmecertno || '');
            $('#lblmsmefrom').text(response.lblmsmefrom || '');
            $('#lblmsmeto').text(response.lblmsmeto || '');
            $('#lblmsmecity').text(response.lblmsmecity || '');
            $('#lblNTypeOfIndustry').text(response.lblNTypeOfIndustry || '');
            $('#lblNClassificationOfYear').text(response.lblNClassificationOfYear || '');
            $('#lblNDateOfClassification').text(response.lblNDateOfClassification || '');
            $('#lblserviceagentgrp').text(response.lblserviceagentgrp || '');

            // Bank details
            $('#lblbankaccountno').text(response.lblbankaccountno || '');
            $('#lblbankname').text(response.lblbankname || '');
            $('#lblbankaddress').text(response.lblbankaddress || '');
            $('#lblbankcity').text(response.lblbankcity || '');
            $('#lblbankstate').text(response.lblbankstate || '');
            $('#lblbankcountry').text(response.lblbankcountry || '');
            $('#lblbranchname').text(response.lblbranchname || '');
            $('#lbltypeofaccount').text(response.lbltypeofaccount || '');
            $('#lblifsccode').text(response.lblifsccode || '');
            $('#lblswiftcode').text(response.lblswiftcode || '');
            $('#lblibanno').text(response.lblibanno || '');
            $('#lblbankcategory').text(response.lblbankcategory || '');
            $('#lblschemagroup').text(response.lblschemagroup || '');
            $('#lblordercurrency').text(response.lblordercurrency || '');
            $('#lblreferencedetails').text(response.lblreferencedetails || '');

            // Intermediary bank
            $('#lblibankkey').text(response.lblibankkey || '');
            $('#lblibankaccountno').text(response.lblibankaccountno || '');
            $('#lblibankname').text(response.lblibankname || '');
            $('#lblibankaddress').text(response.lblibankaddress || '');
            $('#lblibankcity').text(response.lblibankcity || '');
            $('#lblibankstate').text(response.lblibankstate || '');
            $('#lblibankcountry').text(response.lblibankcountry || '');
            $('#lblibranchname').text(response.lblibranchname || '');
            $('#lblitypeofaccount').text(response.lblitypeofaccount || '');
            $('#lbliswiftcode').text(response.lbliswiftcode || '');
            $('#lbliibanno').text(response.lbliibanno || '');
            $('#lblibankcategory').text(response.lblibankcategory || '');
            $('#lblireferencedetails').text(response.lblireferenceddetails || '');

            // TDS / GST
            $('#lblpannumber').text(response.lblpannumber || '');
            $('#lblgstin').text(response.lblgstin || '');
            $('#lblgstclassification').text(response.lblgstclassification || '');
            $('#lblEinvoiceApplicable').text(response.lblEinvoiceApplicable || '');
            $('#lblleiapplicable').text(response.lblleiapplicable || '');
            $('#lblleino').text(response.lblleino || '');

            // Apply UI visibility for Interbank table based on response flag
            //if (response.rdogpimportChecked) {
            //    $('#tableInterbankdetail').show();
            //    $('#rdogpimport').prop('checked', true);
            //} else {
            //    $('#tableInterbankdetail').hide();
            //    $('#rdogpimport').prop('checked', false);
            //}

            //if ($("#rdogpimport").is(":checked")) {
            //    $('#tableInterbankdetail').show();
            //} else {
            //    $('#tableInterbankdetail').hide();
            //}

            // If MSME info status is NO clear cert and category as in original
            if ((response.lblmsmeinfostatus || '').toUpperCase() === 'NO') {
                $('#lblmsmecertno').text('');
                $('#lblmsmecategory').text('');
            }

            // Final cleanup same as original
            $('#lblvendorname').text($('#lblvendorname').text().replace(/"/g, ' ').trim());
            $('#lblstreethouseno').text($('#lblstreethouseno').text().replace(/"/g, ' ').trim());
            $('#lblbranchname').text($('#lblbranchname').text().replace(/"/g, ' ').trim());
            $('#lblbankaddress').text($('#lblbankaddress').text().replace(/"/g, ' ').trim());
            //$('#LblVendorNameCode').text($('#LblVendorNameCode').text().replace(/"/g, ' ').trim());

            // MSME Certificate
            if ($("#lblmsmeinfostatus").text().toUpperCase() === "NO") {
                $("#AnchMsmeCertificate").text("").attr("href", "");
                $("#fileupload_msmecertificate").prop("disabled", true);
            }

            // IFSC Code
            if ($("#lblifsccode").text().trim() === "") {
                $("#AnchmandateForm").text("").attr("href", "");
                $("#fileupload_mandateform").prop("disabled", true);

                $("#AnchCancelCheque").text("").attr("href", "");
                $("#fileupload_cancelcheque").prop("disabled", true);
            }

            // PAN Number
            if ($("#lblpannumber").text().trim() === "") {
                $("#AnchPanCard").text("").attr("href", "");
                $("#fileupload_pancard").prop("disabled", true);
            }

            // GSTIN
            if ($("#lblgstin").text().trim() === "") {
                $("#AnchGSTCertificate").text("").attr("href", "");
                $("#fileupload_GSTCERTNO").prop("disabled", true);
            }

            //GET_APPROVALAUTHORITY();

        }, // success
        error: function (xhr, status, err) {
            console.error('Error parsing CSV:', err);
        }
    });
}

function BindExistingVendorData(response) {
    $('#lbloldvendorname').text(response.VENDORNAME || '');
    $('#lbloldvendorcode').text((response.VENDORCODE || '').toUpperCase());
    $('#lbloldstreethouseno').text(response.STREET || '');
    $('#lbloldcity').text(response.CITY || '');
    $('#lbloldregion').text(response.REGION || '');
    $('#lbloldpostalcode').text(response.POSTALCODE || '');
    $('#lbloldcountry').text(response.COUNTRY || '');
    $('#lbloldmobilenumber').text(response.MOBILENO || '');
    $('#lbloldtelphoneno').text(response.TELEPHONENO || '');
    $('#lbloldemailid1').text(response.EMAIL1 || '');
    $('#lbloldemailid2').text(response.EMAIL2 || '');
    $('#lbloldemailid3').text(response.EMAIL3 || '');
    $('#lbloldmsmeinfostatus').text(response.MSMESTATUSINFO || '');
    $('#lbloldmsmecategory').text(response.MSMECATEGORY || '');
    $('#lbloldmsmecertno').text((response.MSMECERTIFICATENO || '').toUpperCase());
    $('#lbloldserviceagentgrp').text((response.SERVICEAGENTGROUP || '').toUpperCase());
    $('#lbloldmsmefrom').text(response.MSMEFROM || '');
    $('#lbloldmsmeto').text(response.MSMETO || '');
    $('#lbloldmsmecity').text(response.MSMECITY || '');

    //$('#lblTypeOfIndustry').text(response.TYPE_OF_INDUSTRY || '');
    //$('#lblClassificationOfYear').text(response.CLASSIFICATION_OF_YEAR || '');
    //$('#lblDateOfClassification').text(response.DATE_OF_CLASSIFICATION || '');

    $('#lbloldbankaccountno').text((response.BANKACCOUNTNO || '').toUpperCase());
    $('#lbloldbankname').text(response.BANKNAME || '');
    $('#lbloldbankaddress').text(response.BANKADDRESS || '');
    $('#lbloldbankcity').text(response.BANKCITY || '');
    $('#lbloldbankstate').text(response.BANKREGION || '');
    $('#lbloldbankcountry').text(response.BANKCOUNTRY || '');
    $('#lbloldbranchname').text(response.BRANCHNAME || '');
    $('#lbloldtypeofaccount').text(response.TYPEOFACCOUNT || '');
    $('#lbloldifsccode').text((response.IFSCCODE || '').toUpperCase());
    $('#lbloldbankcategory').text(response.BANKCATEGORY || '');
    $('#lbloldschemagroup').text(response.SCHEMAGROUP || '');
    $('#lbloldordercurrency').text(response.ORDERCURRENCY || '');

    $('#lbloldreferencedetails').text(response.REFERENCE_DETAIL || '');
    $('#lbloldswiftcode').text(response.SWIFT_CODE || '');
    $('#lbloldibanno').text(response.IBAN_NO || '');

    $('#lblibankkey').text((response.IBANK_KEY || '').toUpperCase());
    $('#lblibankaccountno').text((response.IBANK_ACC_NO || '').toUpperCase());
    $('#lblibankname').text((response.IBANK_NAME || '').toUpperCase());
    $('#lblibankaddress').text((response.IADDRESS || '').toUpperCase());
    $('#lblibankcity').text((response.ICITY || '').toUpperCase());
    $('#lblibankstate').text((response.IREGION_STATE || '').toUpperCase());
    $('#lblibankcountry').text((response.ICOUNTRY || '').toUpperCase());
    $('#lblibranchname').text((response.IBRANCH_NAME || '').toUpperCase());
    $('#lblitypeofaccount').text((response.ITYPE_OF_ACCOUNT || '').toUpperCase());
    $('#lbliswiftcode').text((response.ISWIFT_CODE || '').toUpperCase());
    $('#lbliibanno').text((response.IBAN_NO || '').toUpperCase());
    $('#lblibankcategory').text((response.IBANK_CATEGORY || '').toUpperCase());
    $('#lblireferencedetails').text((response.IREFERENCE_DETAIL || '').toUpperCase());

    $('#lbloldpannumber').text((response.PANNO || '').toUpperCase());

    $('#lbloldgstin').text((response.GSTIN || '').toUpperCase());
    $('#lbloldgstclassification').text((response.GSTCLASSIFICATION || '').toUpperCase());
    //$('#lbloldeinvoice').text((response.EINVOICE || '').toUpperCase());

    //var vendorName = $('#lblvendorname').text() || '';
    //var vendorCode = $('#lblvendorcode').text() || '';
    //$('#LblVendorNameCode').text(vendorName + '[' + vendorCode + ']');

    //$('#lbloldleiapplicable').text((response.LEI_APPLICABLE || '').toUpperCase());
    //$('#lbloldleino').text(response.LEINO || '');
}

function renderTable(items) {
    debugger;
    var tbody = $("#withholdingTable tbody");
    tbody.empty();
    for (var i = 0; i < items.length; i++) {
        var it = items[i];
        var id = it.Id || it.id || "";
        var liableText = (it.LIABLE == "Y" || it.LIABLE == "y" || it.LIABLE == "true") ? "Yes" : "No";
        var row = "<tr data-id='" + id + "'>"
            + "<td>" + (i + 1) + "</td>"
            + "<td>" + (it.WITHHOLDING_TAX_TYPE || "") + "</td>"
            + "<td>" + (it.WITHHOLDING_TAX_CODE || "") + "</td>"
            + "<td>" + liableText + "</td>"
            + "<td>" + (it.RECIPIENT_TYPE || "") + "</td>"
            + "<td>" + (it.WITHHOLDING_TAX_ID_NO || "") + "</td>"
            + "<td>" + (it.EXEMPTION_CERTI_NO || "") + "</td>"
            + "<td>" + (it.EXEMPTION_RATE || "") + "</td>"
            + "<td>" + (it.DATE_ON_WHICH_EXEMPTION_BEGINS || "") + "</td>"
            + "<td>" + (it.DATE_ON_WHICH_EXEMPTION_ENDS || "") + "</td>"
            + "<td>" + (it.REASON_FOR_EXEMPTION || "") + "</td>"
            //+ "<td><a href='javascript: void(0);' class='btn btn-sm btn-danger btn-delete' data-id=" + id + "><i class='fa fa-trash'></i></a >"
            + "</tr>";
        tbody.append(row);
    }
}

