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
        window.location.href = "/IOMContract/ManageContract";
    });

    $("#cmdSubmit").on("click", function (e) {
        e.preventDefault();

        if (preventMultipleSubmissions()) {
            UserAcknowledgementSave();
        }
    });


});
function preventMultipleSubmissions() {
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


function UserAcknowledgementSave() {
    var formData = new FormData();

    formData.append('hdiomid', $('#hdiomid').val() || ''); 
    formData.append('txt_Remarks', $('#txt_Remarks').val() || '');
    formData.append('lblagreementtype', $('#lblagreementtype').text() || '');
    formData.append('lblcontracttype', $('#lblcontracttype').text() || '');
    formData.append('lbleffdate', $('#lbleffdate').text() || '');
    formData.append('lblexpdate', $('#lblexpdate').text() || '');
    formData.append('lblvendorname', $('#lblvendorname').text() || '');

    $.ajax({
        url: '/IOMContract/UserAcknowledgementSave',
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


