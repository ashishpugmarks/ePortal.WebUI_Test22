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

    changeterm();
   

    $('.cclickED').on('change', function (e) {
        cclickED();
    });

    $('.changeterm').on('change', function (e) {
        changeterm();
    });

    $('.cclick').on('change', function (e) {
        cclick();
    });

    $("#cmdReset").on("click", function () {
        location.reload(true);
    });

    $("#cmdSubmit").on("click", function () {
        if (upload_Click())
        {
            VerificationRequestForm();

        }
    });


});


function VerificationRequestForm() {
    var formData = new FormData();

    formData.append("hdauthlevel", $("#hdauthlevel").val());
    formData.append("ddl_appauth", $("#ddl_appauth").val());
    formData.append("hdauthcode", $("#hdauthcode").val());
    formData.append("hdauthname", $("#hdauthname").val());
    formData.append("hdauthemailid", $("#hdauthemailid").val());
    formData.append("ddlfrom", $("#ddlfrom").val());
    formData.append("ddlagreement", $("#ddlagreement").val());
    formData.append("ddlagreementText", $("#ddlagreement option:selected").text());
    formData.append("ddlcontract", $("#ddlcontract").val());
    formData.append("ddlcontractText", $("#ddlcontract option:selected").text());
    formData.append("optED", $("#optED").val());
    formData.append("optEM", $("#optEM option:selected").text());
    formData.append("optEY", $("#optEY").val());
    formData.append("ddlTermmonth", $("#ddlTermmonth").val());
    formData.append("ddlTermyear", $("#ddlTermyear").val());
    formData.append("hdnexpiryday", $("#hdnexpiryday").val());
    formData.append("hdnexpirymon", $("#hdnexpirymon").val());
    formData.append("hdnexpiryyear", $("#hdnexpiryyear").val());
    formData.append("ddlMannerOfPayment", $("#ddlMannerOfPayment").val());
    formData.append("txtPurpose", $("#txtPurpose").val());
    formData.append("txtRemarks", $("#txtRemarks").val());
    formData.append("txtvendorname", $("#txtvendorname").val());
    formData.append("optDay", $("#optDay").val());
    formData.append("optMonth", $("#optMonth option:selected").text());
    formData.append("optYear", $("#optYear").val());
    formData.append("lbl_appauth", $("#lbl_appauth").val());

    // Add files
    formData.append("fileUploadAttachmentAA", $("#fileUploadAttachmentAA")[0].files[0]);
    formData.append("fileUploadAttachmentRA", $("#fileUploadAttachmentRA")[0].files[0]);

    $.ajax({
        url: '/IOMContract/VerificationRequestForm',
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

function upload_Click() {
   
    var ct = document.getElementById("ddlcontract").value;
    if (ct == null || ct == "") {
        alert("Select Contract Type");
        document.getElementById("ddlcontract").focus;
        return false;
    }
    var ty = document.getElementById("ddlTermyear").value;
    var tm = document.getElementById("ddlTermmonth").value;
    if (ty == "0" && tm == "0") {
        alert("Select Term");
        document.getElementById("ddlTermyear").focus;
        return false;
    }
    var vn = document.getElementById("txtvendorname").value.trim();
    if (vn == null || vn == "") {
        alert("Enter Vendor Name");
        document.getElementById("txtvendorname").focus;
        return false;
    }
    var mp = document.getElementById("ddlMannerOfPayment").value;
    if (mp == null || mp == "") {
        alert("Selct Manner of Payment");
        document.getElementById("ddlMannerOfPayment").focus;
        return false;
    }
    //Start - Attachment Validation
    var aa = document.getElementById("fileUploadAttachmentAA").value;
    var an = document.getElementById("fileUploadAttachmentRA").value;
    if (aa == null || aa == "") {
        alert("Agreement Attachment is Mandatory");
        document.getElementById("fileUploadAttachmentAA").focus;
        return false;
    }
    else if (aa == an) {
        alert("Agreement and approval note attachment should not be same");
        document.getElementById("fileUploadAttachmentRA").focus;
        return false;
    }
    //End - Attachment Validation
    var pur = document.getElementById("txtPurpose").value.trim();
    if (pur == null || pur == "") {
        alert("Enter Purpose");
        document.getElementById("txtPurpose").focus;
        return false;
    }
    var rem = document.getElementById("txtRemarks").value.trim();
    if (rem == null || rem == "") {
        alert("Enter Remarks");
        document.getElementById("txtRemarks").focus;
        return false;
    }
    var isSubmitted = false;
    if (!isSubmitted) {
        isSubmitted = true;
        return true;
    }
    else {
        return false;
    }
}

function changeterm() {
    var termyear = document.getElementById("ddlTermyear").value;
    var termmonth = document.getElementById("ddlTermmonth").value;
    var effdate = document.getElementById("optED").value;
    var effmonth = document.getElementById("optEM").value;
    var effyear = document.getElementById("optEY").value;

    var effd = new Date(effyear, effmonth - 1, effdate);
    var calcdate = new Date(new Date(effd).setFullYear(effd.getFullYear() + parseInt(termyear)));
    var dateofexp = new Date(new Date(calcdate).setMonth(calcdate.getMonth() + parseInt(termmonth)));
    dateofexp.setDate(dateofexp.getDate() - 1);
    var year = dateofexp.getFullYear();
    var mon = dateofexp.getMonth();
    var month = parseInt(mon) + parseInt(1);
    var day = dateofexp.getDate();
    document.getElementById("optYear").value = year;
    document.getElementById("optMonth").value = month;
    document.getElementById("optDay").value = day;
    var hdnmon;
    if (month == 1)
        hdnmon = "Jan";
    if (month == 2)
        hdnmon = "Feb";
    if (month == 3)
        hdnmon = "Mar";
    if (month == 4)
        hdnmon = "Apr";
    if (month == 5)
        hdnmon = "May";
    if (month == 6)
        hdnmon = "Jun";
    if (month == 7)
        hdnmon = "Jul";
    if (month == 8)
        hdnmon = "Aug";
    if (month == 9)
        hdnmon = "Sep";
    if (month == 10)
        hdnmon = "Oct";
    if (month == 11)
        hdnmon = "Nov";
    if (month == 12)
        hdnmon = "Dec";

    document.getElementById("hdnexpiryday").value = day;
    document.getElementById("hdnexpirymon").value = hdnmon;
    document.getElementById("hdnexpiryyear").value = year;
}

//DATE VALIDATION
function cclick() {
    for (var i = 0; i < document.getElementById("optMonth").options.length; i++) {
        if (document.getElementById("optMonth").options[i].selected == true) {
            if (document.getElementById("optMonth").options[i].value == 2) {
                for (var j = 0; j < document.getElementById("optDay").options.length; j++) {
                    if (document.getElementById("optDay").options[j].selected == true) {
                        if (document.getElementById("optDay").options[j].value > 28) {
                            if (document.getElementById("optDay").options[j].value == 29 & (document.getElementById("optYear").options.value == 2008 || document.getElementById("optYear").options.value == 2012 || document.getElementById("optYear").options.value == 2016 || document.getElementById("optYear").options.value == 2020 || document.getElementById("optYear").options.value == 2024 || document.getElementById("optYear").options.value == 2028)) {
                            }
                            else {
                                alert("Invalid date! please select valid  date");
                            }
                        }
                    }
                }
            }
            if (document.getElementById("optMonth").options[i].value == 4 || document.getElementById("optMonth").options[i].value == 6 || document.getElementById("optMonth").options[i].value == 9 || document.getElementById("optMonth").options[i].value == 11) {
                for (var j = 0; j < document.getElementById("optDay").options.length; j++) {
                    if (document.getElementById("optDay").options[j].selected == true) {
                        if (document.getElementById("optDay").options[j].value > 30) {
                            alert("Invalid date! please select valid  date");
                        }
                    }
                }
            }
        }
    }
}

//DATE VALIDATION
function cclickED() {
    changeterm();
    for (var i = 0; i < document.getElementById("optEM").options.length; i++) {
        if (document.getElementById("optEM").options[i].selected == true) {
            if (document.getElementById("optEM").options[i].value == 2) {
                for (var j = 0; j < document.getElementById("optED").options.length; j++) {
                    if (document.getElementById("optED").options[j].selected == true) {
                        if (document.getElementById("optED").options[j].value > 28) {
                            if (document.getElementById("optED").options[j].value == 29 & (document.getElementById("optEY").options.value == 2008 || document.getElementById("optEY").options.value == 2012 || document.getElementById("optEY").options.value == 2016 || document.getElementById("optEY").options.value == 2020 || document.getElementById("optEY").options.value == 2024 || document.getElementById("optEY").options.value == 2028)) {
                            }
                            else {
                                alert("Invalid date! please select valid  date");
                            }
                        }
                    }
                }
            }
            if (document.getElementById("optEM").options[i].value == 4 || document.getElementById("optEM").options[i].value == 6 || document.getElementById("optEM").options[i].value == 9 || document.getElementById("optEM").options[i].value == 11) {
                for (var j = 0; j < document.getElementById("optED").options.length; j++) {
                    if (document.getElementById("optED").options[j].selected == true) {
                        if (document.getElementById("optED").options[j].value > 30) {
                            alert("Invalid date! please select valid  date");
                        }
                    }
                }
            }
        }
    }
}


