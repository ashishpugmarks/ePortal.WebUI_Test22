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

        //--------------------------------------------------------------------------------
    }
    );

        function fnValidate() {
            if (document.getElementById("txtCancelRequest").value == "") {
                ShowMessage("Cancel remarks is a required field", "Error");
                document.getElementById("txtCancelRequest").focus();
                return false;
            }
            return true;
        }
function submitForm() {
   
    var isFormVaildation = fnValidate();
    if (isFormVaildation) {

        $("#btnSubmit").attr("disabled", true);
        var data = {
            REQUESTID: $("#REQUESTID").val(),
            SUPERVISORADEMPCODE: $("#SUPERVISORADEMPCODE").val(),
            DATEOFTRAVELFROM: $("#DATEOFTRAVELFROM").val(),
            REPORTINGTIME: $("#REPORTINGTIME").val(),
            REMARKS: $("#txtCancelRequest").val(),

        }
        $.ajax({
            type: "POST",
            url: "/TaxiRequest/CancelRequest",
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
                            window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx";
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
