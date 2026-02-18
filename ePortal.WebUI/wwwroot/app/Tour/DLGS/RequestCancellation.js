// ------------------------------ Globals -----------------------------------------------------------
$(".homeContent").removeClass("homeContent");

$(document).ready(function () {

    $('#btnCancel').on('click', function (e) {
        e.preventDefault();
        if (validentry()) {
            btnCancel_Click();
        }
    });

    $('#btnClose').on('click', function (e) {
        e.preventDefault();
        window.returnValue = 0;
        window.close();
    });

});


function validentry() {
    if (document.getElementById("txtCalcellationRemarks").value === "") {
        alert("Cancellation Remarks is required field.");
        document.getElementById("txtCalcellationRemarks").focus();
        return false;
    }
    return true;
}


function closePopup() {
    window.opener.location.reload(true);
    window.returnValue = 1;
    window.close();
}


function btnCancel_Click() {

    const remarks = ($('#txtCalcellationRemarks').val() || '').trim();

    var model = {
        txtCalcellationRemarks: remarks,
        lblTravelDate: $('#lblTravelDate').val(),// lblTravelDate.val(),
            lblCityFrom:$('#lblCityFrom').val(),// lblCityFrom.val(),
                lblCityTo: $('#lblCityTo').val(),// lblCityTo.val(),
                lblTicketNumber: $('#lblTicketNumber').val()// lblTicketNumber.val()
    } // Changes Tour

    return $.ajax({
        // url: '/TourDLGS/btnCancel_Click', // Changes Tour
        url: '/TourDLGS/btnCancel_Click_RC', // Changes Tour
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(model) // Changes Tour
    })
        .done(function (resp) {

            const err = (
                resp &&
                resp.result &&
                resp.result.err ||
                ''
            ).trim();

            if (err) {
                $('#lblErrMsg').text(err);
            } else {
                window.returnValue = 1;
                window.close();
            }

        })
        .fail(function (xhr) {

            const msg =
                (xhr.responseJSON &&
                    (xhr.responseJSON.error || xhr.responseJSON.message)) ||
                'Something went wrong. Please try again.';

            $('#lblErrMsg').text(msg);

        });
}
