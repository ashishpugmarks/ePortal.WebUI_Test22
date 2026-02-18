$('.homeContent').removeClass('homeContent');
$(document).ready(function () {

    $('.onpaste').on('paste', function (e) {
        e.preventDefault();
    });

    $('.numeric').on('keypress', function (e) {
        if (!/[0-9]/.test(e.key)) {
            e.preventDefault();
        }
    });

    $("#prevappnotelink").on("click", function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');
    });

    $("#prevfinaldoclink").on("click", function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');
    });


    $(".view_iom_details").on("click", function (e) {
        e.preventDefault();
        const iomid = $(this).closest('tr').data('iomid');
        var url = '/iomcontract/ViewIOMDetail?IOMID=' + iomid;
        openPopup(url, 820, 590);
    });

    $(".finaldoclink").on("click", function (e) {
        e.preventDefault();

        e.preventDefault();
        var finaldoc = $(this).data('finaldoc');
        var url = '../../Uploads/IOM/' + finaldoc;
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');

    });


    $("#agreementlink").on("click", function (e) {
        e.preventDefault();
        const url = $(this).data('url');
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');

    });

    $("#antibriberylink").on("click", function (e) {
        e.preventDefault();
        const url = $(this).data('url');
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');

    });

    $(".linkagreement").on("click", function (e) {
        e.preventDefault();
        var attachmentpath = $(this).data('attachmentpath');
        var url = '../../Uploads/IOM/' + attachmentpath;
        window.open(url, "_blank", 'width=800,height=550,top=90,left=200', 'resizable=yes,scrollbars=yes');
    });




    $("#cmdcancel").on("click", function () {
        window.location.href = "/IOMContract/ManageIOMApproval";
    });

    $("#cmdSubmit").on("click", function (e) {
        e.preventDefault();
        if (preventMultipleSubmissions()) {
            SaveIOMApprovalForm();
        }
    });


});


function preventMultipleSubmissions() {
    debugger

    var isSubmitted = false;
    var rem = document.getElementById("txt_Remarks").value.trim();
    if (rem == null || rem == "") {
        alert("Enter Remarks");
        document.getElementById("txt_Remarks").focus;
        return false;
    }
    if (!isSubmitted) {
        isSubmitted = true;
        return true;
    }
    else {
        return false;
    }
}

function SaveIOMApprovalForm() {
    var formData = new FormData();

    formData.append('hdiomid', $('#hdiomid').val() || '');
    formData.append('EMPDESIGNATION', $('#hdempdesign').val() || '');
    formData.append('hdbackdate', $('#hdbackdate').val() || '');
    formData.append('hddelaydate', $('#hddelaydate').val() || '')

    formData.append('txtbackdateremark', $('#txtbackdateremark').val() || '');
    formData.append('txt_Remarks', $('#txt_Remarks').val() || '');

    formData.append('chkTerms', $('#chkTerms').is(':checked'));

    const rdo = $('input[name="rdoStatus"]:checked').val() || '';
    formData.append('rdoStatus', rdo);

    formData.append('lbl_appauth', $('#lbl_appauth').text().trim() || '');
    formData.append('ddl_appauth', $('#ddl_appauth').val() || '');

    formData.append('hdauthlevel', $('#hdauthlevel').val() || '');
    formData.append('hdauthcode', $('#hdauthcode').val() || '');
    formData.append('hdauthname', $('#hdauthname').val() || '');
    formData.append('hdauthemailid', $('#hdauthemailid').val() || '');
    formData.append('hdreqemail', $('#hdreqemail').val() || '');
    formData.append('hdreqname', $('#hdreqname').val() || '');

    formData.append('lblagreementtype', $('#lblagreementtype').text().trim() || '');
    formData.append('lblcontracttype', $('#lblcontracttype').text().trim() || '');
    formData.append('lbleffdate', $('#lbleffdate').text().trim() || '');
    formData.append('lblexpdate', $('#lblexpdate').text().trim() || '');
    formData.append('lblvendorname', $('#lblvendorname').text().trim() || '');

    $.ajax({
        url: '/IOMContract/IOMApprovalFormSave',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            var result = response.result;
            if (result) {
                if (result.Status === true) {
                    if (result.obj && result.obj.hasOwnProperty("redirectionTo")) {
                        window.location.href = result.obj.redirectionTo;
                    }
                }
                else {
                    if (result.obj && result.obj.hasOwnProperty("IsWindowAlert") && result.obj.IsWindowAlert === true) {
                        alert(result.Message);
                    }
                    else if (result.obj && result.obj.hasOwnProperty("errorpanel_Visible") && result.obj.errorpanel_Visible === true) {
                        document.getElementById('errorpanel').style.display = '';
                        $("#status").text(result.Message);
                        $("#txtup").focus();
                    }
                    else {
                        alert(result.Message);
                    }
                }
            }
        },
        complete: function () {
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseJSON?.message || 'An error occurred .');
        }
    });

}

function openPopup(url, width, height) {
    var left = (screen.width / 2) - (width / 2);
    var top = (screen.height / 2) - (height / 2);
    window.open(url, '_blank', 'width=' + width + ',height=' + height + ',top=' + top + ',left=' + left + ',resizable=yes,scrollbars=yes');
}


