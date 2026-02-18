
$(document).ready(function () {
    $('.dpDate').datepicker({
        format: "dd-M-yyyy",
        todayHighlight: true,
        autoclose: true,

    }) // sets today's date as default// disables all dates before today
    // toolbarplacement: 'top'
    $("[data-ctrl-id='btn-submit']").on('click', function () {
        submitForm();
    });
    $("[data-ctrl-id='btn-cancel']").on('click', function () {
        document.getElementById("frmTaxi").reset();

    });
    //--------------------------------------------------------------------------------
}
);
function ValidateNumber(value) {
    var flag = 0;
    t1 = new String(value);
    for (x = 0; x < t1.length; ++x) {
        if (t1.charAt(x) < "0" || t1.charAt(x) > "9") {
            flag = 1;
            break;
        }
    }
    return flag;
}

function fnValidateVechileReqDetails() {
    var errorMsg = "";
    var returnStatus = true;
    if (new Date($("#txtDtFrom").val()) > new Date($("#txtDtTo").val())) {

        ShowMessage("To date should be greater than or equal to From date", "Error");
        document.getElementById("txtDtTo").focus();
        returnStatus = false;
        return false;
    }
    if (document.getElementById("cboshift").value == "0") {

        ShowMessage("Shift is a required field", "Error");
        document.getElementById("cboshift").focus();
        returnStatus = false;
        return false;
    }

    if (document.getElementById("txtExtNo").value == "") {

        ShowMessage("Extension No. is a required field", "Error")
        document.getElementById("txtExtNo").focus();
        returnStatus = false;
        return false;
    }
    else {
        if (ValidateNumber(document.getElementById("txtExtNo").value) == 1) {

            ShowMessage("Please enter a valid value for Extension No.", "Error")
            document.getElementById("txtExtNo").focus();
            returnStatus = false;
            return false;
        }
    }

    if (document.getElementById("txtPhoneNo").value == "") {

        ShowMessage("Phone No. is a required field", "Error");
        document.getElementById("txtPhoneNo").focus();
        returnStatus = false;
        return false;
    }
    else {
        if (ValidateNumber(document.getElementById("txtPhoneNo").value) == 1) {

            ShowMessage("Please enter a valid value for Phone No.", "Error");
            document.getElementById("txtPhoneNo").focus();
            returnStatus = false;
            return false;
        }
    }






    if (document.getElementById("drpLstHour").value == "") {

        ShowMessage("Reporting Hour is a mandatory field", "Error");
        document.getElementById("drpLstHour").focus();
        returnStatus = false;
        return false;
    }
    if (document.getElementById("drpLstMinute").value == "") {

        ShowMessage("Reporting Minute is a mandatory field", "Error");
        document.getElementById("drpLstMinute").focus();
        returnStatus = false;
        return false;
    }

    if (document.getElementById("txtReportingPlace").value == "") {

        ShowMessage("Reporting Place is a mandatory field", "Error");
        document.getElementById("txtReportingPlace").focus();
        returnStatus = false;
        return false;
    }

    if (document.getElementById("noOfPerson").value == "") {

        ShowMessage("Number of the person is a required field", "Error");
        document.getElementById("noOfPerson").focus();
        returnStatus = false;
        return false;
    }

    if (document.getElementById("txtPurposeVisit").value == "") {

        ShowMessage("Purpose of Visit is a required field", "Error");
        document.getElementById("txtPurposeVisit").focus();
        returnStatus = false;
        return false;
    }
    if (document.getElementById("txtPlaceVisit").value == "") {

        ShowMessage("Place of Visit is a required field", "Error");
        document.getElementById("txtPlaceVisit").focus();
        returnStatus = false;
        return false;
    }
    if (document.getElementById("drpLstApprovalAuthority").value == " - Select Approval Authority - ") {

        ShowMessage("Please select Approval Authority", "Error");
        document.getElementById("drpLstApprovalAuthority").focus();
        returnStatus = false;
        return false;
    }
    if (document.getElementById("chkTerms").checked == false) {

        ShowMessage("Please accept the terms of services", "Error");
        returnStatus = false;
        return false;
    }

    if (returnStatus == false) {
        return false;
    }

    return true;

}
//Equating From and to date
function IsDateEqual() {
    var optYear, optYearIndex;
    optYear = document.getElementById("optYear");
    optYearIndex = optYear.selectedIndex;

    document.getElementById("cbotoday.ClientID %>").options[document.getElementById("optDay").value - 1].selected = true;
    document.getElementById("cbotomonth.ClientID %>").options[document.getElementById("optMonth").value - 1].selected = true;
    document.getElementById("cbotoyear").options[optYearIndex].selected = true;
}
function submitForm() {
   
    var isFormVaildation = fnValidateVechileReqDetails();
    if (isFormVaildation) {

        $("#btnSubmit").attr("disabled", true);
        var data = {
            EXTENSIONO: $("#txtExtNo").val(),
            DATEOFTRAVELFROM: $("#txtDtFrom").val(),
            DATEOFTRAVELTO: $("#txtDtTo").val(),
            ADDRESS: $("#txtReportingPlace").val(),
            SITE: $("#ddSite").val(),
            PHONENO: parseFloat($("#txtPhoneNo").val()),
            REPORTINGHOUR: $("#drpLstHour").val(),
            REPORTINGMIN: $("#drpLstMinute").val(),
            PURPOSEOFVISIT: $("#txtPurposeVisit").val(),
            PLACEOFVISIT: $("#txtPlaceVisit").val(),
            SUPERVISORADEMPCODE: $("#drpLstApprovalAuthority").val(),
            SHIFT: $("#cboshift").val(),
            NOOFPERSON: parseFloat($("#noOfPerson").val()),
            REMARKS: $("#txtRemarks").val(),

        }
        $.ajax({
            type: "POST",
            url: "/TaxiRequest/TaxiRequest",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                debugger;
                if (data.Item1 == "1") {
                    swal({
                        title: "",
                        text: "Record saved successfully.",
                        type: "success",
                    }, function (isConfirm) {
                        if (isConfirm) {
                            window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx";
                        }
                    });
                }
                else if (data.Item1 == "2" || data.Item1 == "0") {
                    ShowMessage(data.Item2, "Error");
                    $("#btnSubmit").attr("disabled", false);
                }

            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {
                sweetAlert("Oops...", "Something went wrong!", "error");
                $("#btnSubmit").attr("disabled", false);
            }
        });
    }


}