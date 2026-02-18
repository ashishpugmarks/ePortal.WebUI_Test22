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
$(function () {
    // Navigation buttons
    $('#btncancelfrm').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/ManageApprovalRequestFin";
    });
    $('#btnbackimg, #btnbackspan').on('click', function () {
        window.location.href = window.location.origin + "/VendorMaster/ManageApprovalRequestFin";
    });

    // Section tabs
    $("#GENINFO").on("click", GENINFOCLICK);
    $("#BANKDET").on("click", BANKDETCLICK);
    $("#TAXDET").on("click", TAXDETCLICK);
    $("#MATCHDATA").on("click", MATCHDATACLICK);
    $("#WTHOLDING").on("click", HOLDINGTAXDATACLICK);

    verifications.forEach(v => bindVerification(v.anchor, v.label, v.hidden));

    $('#btnsubmit').on('click', function (e) {
        e.preventDefault();
        debugger;
        var model = {
            Vmid: $('#hdVmid').val(),
            Status: $('#ddlstatus').val(),
            Remarks: $('#txt_Remarks').val().trim(),
            RegCertVerified: $('#HasRegCertUrl').val() === "" ? true : $('#HDREGCERTNO_VERIFIED').val() === 'true',
            ConflictCertVerified: $('#HasConflictCertUrl').val() === "" ? true : $('#HDCONFLICTCERT_VERIFIED').val() === 'true',
            MsmeVerified: $('#HasMsmeCertUrl').val() === "" ? true : $('#HDMSMECERTIFICATE_VERIFIED').val() === 'true',
            CancelChequeVerified: $('#HasCancelChkUrl').val() === "" ? true : $('#HDCANCELCHEQUE_VERIFIED').val() === 'true',
            MandateFormVerified: $('#HasMandateFormUrl').val() === "" ? true : $('#HDMANDATEFORM_VERIFIED').val() === 'true',
            PanCardVerified: $('#HasPanCardUrl').val() === "" ? true : $('#HDPANCARD_VERIFIED').val() === 'true',
            GstVerified: $('#HasGstRegUrl').val() === "" ? true : $('#HDGSTREGN_VERIFIED').val() === 'true',
            EInvoiceVerified: $('#HasEinvoiceUrl').val() === "" ? true : $('#HDEINVOICE_VERIFIED').val() === 'true',
            LeiAttachmentVerified: $('#HasLeiAttachmentUrl').val() === "" ? true : $('#HDLEIATTACHMENT_VERIFIED').val() === 'true',
            SplAppId: $('#HDSPLAPPID').val() || '0',
            MatchChecked: $('#chk_match').is(':checked'),
            DeclarationChecked: $('#chkdeclaration').is(':checked'),
            AppAuthority: $('#cboSPAppAuthority').val() || ''
        };

        // client-side quick validations using SweetAlert (v1)
        if (!model.Status) {
            swal({ title: "Validation", text: "Status is mandatory field", type: "warning" });
            return;
        }
        if (!model.Remarks) {
            swal({ title: "Validation", text: "Remarks is mandatory field", type: "warning" });
            return;
        }
        if (model.SplAppId === '1' && !model.MatchChecked && $('#tr_chkmatch').is(':visible')) {
            swal({ title: "Validation", text: "Kindly check Match Detail tab and acknowledge the match detail verification checkbox.", type: "warning" });
            return;
        }
        if (!model.DeclarationChecked) {
            swal({ title: "Validation", text: "Declaration is mandatory", type: "warning" });
            return;
        }

        $.ajax({
            url: '/VendorMaster/ApproveVendorFin',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(model),
            success: function (res) {
                if (res.success) {
                    // show success and redirect on confirm
                    swal({
                        title: "Success",
                        text: res.message,
                        type: "success"
                    }, function () {
                        if (res.redirect) {
                            window.location.href = res.redirect;
                        }
                    });
                } else {
                    // show error
                    swal({ title: "Error", text: res.message, type: "error" });
                }
            },
            error: function () {
                swal({ title: "Error", text: "An error occurred while processing your request.", type: "error" });
            }
        });
    });
});

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
function OpenFile(href, target) {
    window.open(href, target, "left=150,top=20,width=800,height=600,toolbar=0,resizable=0");
}

// Generic verification + file open handler
function bindVerification(anchorId, labelId, hiddenId) {
    $(anchorId).on('click', function (e) {
        e.preventDefault(); // stop default navigation
        var $lbl = $(labelId);
        if ($lbl.length) {
            $lbl.text('Verified')
                .removeClass('notverifydoc')
                .addClass('verifydoc');
            $(hiddenId).val('true');
        }
        // open the file in a popup
        OpenFile($(this).attr('href'), '_blank');
    });
}

// Configuration array: all anchors, labels, hidden fields
const verifications = [
    { anchor: '#anchRegcertno', label: '#lblregcertno_ver', hidden: '#HDREGCERTNO_VERIFIED' },
    { anchor: '#anchmsmecertificate', label: '#lblmsmecertificate_ver', hidden: '#HDMSMECERTIFICATE_VERIFIED' },
    { anchor: '#anchConflictcertno', label: '#lblconfcertificate_ver', hidden: '#HDCONFLICTCERT_VERIFIED' },
    { anchor: '#anchcancelcheque', label: '#lblcancelcheque_ver', hidden: '#HDCANCELCHEQUE_VERIFIED' },
    { anchor: '#anchmandateform', label: '#lblmandateform_ver', hidden: '#HDMANDATEFORM_VERIFIED' },
    { anchor: '#anchpancard', label: '#lblpancard_ver', hidden: '#HDPANCARD_VERIFIED' },
    { anchor: '#anchgstregn', label: '#lblgstregn_ver', hidden: '#HDGSTREGN_VERIFIED' },
    { anchor: '#anchEinvoice', label: '#lblEInvoice_ver', hidden: '#HDEINVOICE_VERIFIED' },
    { anchor: '#anchLEIC', label: '#lbllei_ver', hidden: '#HDLEI_VERIFIED' }
];
