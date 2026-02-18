$(document).ready(function () {
    $('#btnSubmit').on('click', function () {
        SubmitMaterialRequest();
    });
    $('#btnCancel').on('click', function () {
        resetData();
    });
});
function ShowError(selector, msg) {
    sweetAlert("Material Master", msg, "warning");
    $('#infoPanel').hide();
    $('#errorPanel').show();
    $('#errorMsg').html(msg);
    $('#infoMsg').html("");
    $(selector).focus();
    return false;
}
function ShowMessage(messagetype, message) {
    if (messagetype == "error") {
        sweetAlert("Material Master", message, "error");
        $("#errorPanel").show();
        $("#infoPanel").hide();
        $("#errorMsg").text(message);
        $("#infoMsg").text("");

    }
    else if (messagetype == "success") {
        sweetAlert("Material Master", message, "success");
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
function FnValidateDDLContitionalRequired(ctrlDDL, conditionOposite, msg) {
    if (ctrlDDL.options[ctrlDDL.selectedIndex].value == "0" && conditionOposite == "True") {
        return ShowError(ctrlDDL, msg);
    }
}
// To check validation for Drop Down List for required
function FnValidateDDLRequired(ctrlDDL, msg) {
    if (ctrlDDL.options[ctrlDDL.selectedIndex].value == "0") {
        return ShowError(ctrlDDL, msg);
    }
}
// To check validation for textbox for required
function FnValidateTextRquired(ctrlText, msg) {
    if (ctrlText.value == "") {
        return ShowError(ctrlText, msg);
    }
}
function FnValidateCheckBoxRequired(ctrlDDL, msg) {
    if (ctrlDDL.checked == false) {
        return ShowError(ctrlDDL, msg);
    }
}
function openPopup(strOpenUrl, width, height) {
    window.open(strOpenUrl, "mywindow", "TOOLBAR=no,MENUBAR=no,RESIZABLE=no,SCROLLBARS=yes,LOCATION=no,DIRECTORIES=no,STATUS=no,width=" + width + ",height=" + height);
}

/////////////Submit Request//////////////////////////////////////////////////
function SubmitMaterialRequest() {
    try {
        let _MMHeaderIdApprove = $('#hdMMHeaderIdApprove').val();
        if (_MMHeaderIdApprove == undefined || _MMHeaderIdApprove == "") {
            ShowMessage("error", "Error in proccess!");
            return;
        }
       
        var _PPCApprovalAuth = $('#ddlPPC_User').val();
        if (_PPCApprovalAuth == "0" || _PPCApprovalAuth == "") {
            ShowMessage("error", "Please check PPC Approval authority!");
            return;
        }
        var _FinApprovalAuth = $('#ddlFIN_User').val();
        if (_FinApprovalAuth == "0" || _FinApprovalAuth == "") {
            ShowMessage("error", "Please check Finance Approval authority!");
            return;
        }
        var data = {
            MMHeaderIdApprove: _MMHeaderIdApprove,
            PPCApprovingAuthority: _PPCApprovalAuth,
            FinanceApprovingAuthority: _FinApprovalAuth,
        }
        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/MMApprovalDetailsUpdateSubmit",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == 1) {
                    $('#btnSubmit').prop('disabled', true);                   
                    ShowMessage("success", data.Message);
                    window.location.href = "/MasterMgmt/MMStatusRequestReport";

                }
                else if (data.Rs == 0) {
                    ShowMessage("error", data.Message);
                }

            },
            error: function () {
                $("#ajaxLoader").removeClass('loader');
                sweetAlert("Oops...", "Something went wrong!", "error");
            },
            complete: function () {

                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (ex) {
        $("#ajaxLoader").removeClass('loader');
        console.log(ex);
        sweetAlert("Material Master", "Oops... Something went wrong!.", "warning");
    }
}
//////////////////////////////////////////////////////////////
function resetData() {
   
    if ($("#ddlPPC_User").prop("disabled") === false) {
        $("#ddlPPC_User").prop("selectedIndex", 0);
    }
      $("#ddlFIN_User").prop("selectedIndex", 0);
}
