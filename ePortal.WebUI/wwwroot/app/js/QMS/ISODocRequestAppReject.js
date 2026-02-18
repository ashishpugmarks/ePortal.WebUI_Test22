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


    pageload_controls();


    $('#FileList').on('click', function (e) {
        e.preventDefault();

        const docid = $(this).attr('data-docid');
        const url = "/TokenBridge/RedirectToOldApp?target=/aspxview/Admin/Qms/OpenForm.aspx?docid=" + encodeURIComponent(docid); //update url
        window.open(url, '_blank', 'noopener,noreferrer');

    });

    $("#btnCancel").on("click", function (e) {
        e.preventDefault();
        debugger
        const RequestIDC = $("#RequestIDC").val();
        let redirectionUrl = "";
        if (RequestIDC != null && RequestIDC)
        {
            redirectionUrl = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx"; //update url
        }
        else
        {
            redirectionUrl = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx"; //update url
        }

        window.location.href = redirectionUrl;

    });

    $("#btnSubmit").on("click", function (e) {
        e.preventDefault();
        if (fnRequired()) {
            SaveISODocRequestAppReject();
        }
    });
    
  

    //--------------------------------------- Methods -----------------------------------------
    function pageload_controls() {
        var checked_id = $('input[name="changeType_rdomaster"]:checked').attr('id');
        $("#" + checked_id).prop('disabled', false);

    }

    

    function fnRequired() {
        var errorMsg = "";
        var returnStatus = true;
        var a = document.getElementById("pnlApproval");
        var b = document.getElementById("pnlCancel");

        if (a) {
            if (document.getElementById("txtAPPRemarks").value == "") {
                document.getElementById("errorpanel").style.display = "inline";
                document.getElementById("status").innerHTML = "Remarks is required field.";
                document.getElementById("txtAPPRemarks").style.border = "solid 2px red";
                document.getElementById("txtAPPRemarks").focus();
                returnStatus = false;
                return false;
            }
        }
        else {
            if (document.getElementById("txtCancallation").value == "") {
                document.getElementById("errorpanel").style.display = "inline";
                document.getElementById("status").innerHTML = "Remarks is required field.";
                document.getElementById("txtCancallation").style.border = "solid 2px red";
                document.getElementById("txtCancallation").focus();
                returnStatus = false;
                return false;
            }
        }

        if (returnStatus == false) {
            return false;
        }
        else {

            return true;
        }
    }



    function SaveISODocRequestAppReject() {
        var formData = new FormData();

        formData.append('Cancelid', $('#RequestIDC').val() || '');
        formData.append('Appid', $('#RequestIDA').val() || '');


        formData.append('txtCancallation', $('#txtCancallation').val() || '');
        formData.append('txtAPPRemarks', $('#txtAPPRemarks').val() || '')
        const rdo = $('input[name="rblApprovalStatus"]:checked').val() || '';
        formData.append('rblApprovalStatus', rdo);

        formData.append('hdnplant', $('#hdnplant').val() || '');
        formData.append('lblEmpCode', $('#lblEmpCode').text().trim() || '');
        formData.append('lblEmpName', $('#lblEmpName').text().trim() || '');
        formData.append('lblDocNo', $('#lblDocNo').text().trim() || '');
        formData.append('lblDocTitle', $('#lblDocTitle').text().trim() || '');
        formData.append('EmpEmail', $('#EMPEMAIL').val() || '');
        formData.append('chg', $('#chg').val() || '');
        

        $.ajax({
            url: '/QMS/ISODocRequestAppReject',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                var result = response.result;
                if (result) {
                    if (result.Status === true) {
                        if (result.obj && result.obj.hasOwnProperty("IsWindowAlert") && result.obj.IsWindowAlert === true) {
                            alert(result.Message);
                        }
                        if (result.obj && result.obj.hasOwnProperty("redirectionTo")) {
                            const url = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx"; //update url
                            window.location.href = url;
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
    


});

