$(document).ready(function () {
    $('.homeContent').removeClass('homeContent');
        $('.dpDate').datepicker({
            format: "dd-M-yyyy",
            todayHighlight: true,
            autoclose: true,

        }) // sets today's date as default// disables all dates before today
        // toolbarplacement: 'top'
        $("[data-ctrl-id='btn-submit']").on('click', function () {
            
            submitForm($(this)[0]);
        });

        //--------------------------------------------------------------------------------
    }
    );

        function fnValidate() {
            if (document.getElementById("txtRemarks").value == "") {
                swal("", "Remarks is a required field","error" );
                document.getElementById("txtRemarks").focus();
                return false;
            }
            return true;
        }
function submitForm(current) {
    
    var isFormVaildation = fnValidate();
    if (isFormVaildation) {

        $("#btnSubmit").attr("disabled", true);
        var data = {
            REQUESTID: $(current).attr('data-id'),
            SUPERVISORADEMPCODE: $("#drpLstApprovalAuthority").val(),
            APPRSTATUS: $(current).attr('data-status'),
            HDN_MFG_OPID: $(current).attr('mfg'),
            HDN_TAPUKARA_ID: $(current).attr('tapukara'),
            EMPCODE:$("#EMPCODE").val(),
            REMARKS: $("#txtRemarks").val(),

        }
        $.ajax({
            type: "POST",
            url: "/TaxiRequest/TaxiFinalApproval",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                debugger;
                if (data.Item1 == "Success") {
                    swal({
                        title: "",
                        text: "Record saved successfully.",
                        type: "success",
                    }, function (isConfirm) {
                        if (isConfirm) {
                            window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx";
                        }
                    });
                }
                else if (data.Item1 == "Error") {
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

