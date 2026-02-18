
    $(document).ready(function () {
        $("[data-ctrl-id='btn-submit']").on('click', function (e) {

            submitVehicleRelease();
        });
    });

function submitVehicleRelease() {
   
    $("#btnSubmit").attr("disabled", true);
            const data = {
                ADVECHICLEDETAILID: $("#ADVECHICLEDETAILID").val(),
                RELEASEMETERREADING: $("#txtkmused").val(),
                RELEASEDAT: $("#ddlboardingpoint").val(),
                VISITE_PLACE: $("#txtvisitplace").val(),
                TAXICONDITIONSTATUS: $("input[name='TaxiCondition']:checked").val(),
                REMARKS: $("#txtremarks").val(),
                METERREADING: $("#METERREADING").val()
            };

            $.ajax({
                url: '/TaxiRequest/VehicleReleased',
                type: 'POST',
                beforeSend: function () {
                    $("#ajaxLoader").addClass('loader');
                },
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (response, status, xhr) {
                    
                    if (response.status == "error") {
                        ShowMessage(response.data, "Error");
                        $("#btnSubmit").attr("disabled", false);
                        return;
                    }
                    else if (response.status == "success") {
                        window.location.href = "/TaxiRequest/TaxiRequestList";
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
