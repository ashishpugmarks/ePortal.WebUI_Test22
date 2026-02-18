
function showreport() {
        var url = '@Url.Action("PrintableList", "TaxiRequest")' + '?sfromdate=' + $('#txtFrom').val() + '&stodate=' + $('#txtTo').val();

    window.open("PrintableList?sfromdate=" + $('#txtFrom').val() + "&stodate=" + $('#txtTo').val(), "Taxi Usage Report", "height=600,width=1000,status=no,resizable=no,scrollbars=yes");
    }
function GetDashboardRecord() {

    var data = {

        DATEOFTRAVELFROM: $('#txtFrom').val(),
        DATEOFTRAVELTO: $('#txtTo').val(),

    }
    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/TaxiRequest/TaxiUsesReport",
        data: data,
        // contentType: "application/json; charset=utf-8",
        // datatype: "json",
        success: function (data) {
            if (data == "error") {
                sweetAlert("", "An error occurred while processing your request.", "error");
            }
            else {
                $('#div_append').html(data);
            }
        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}


        $(document).ready(function () {
            $('.dpDate').datepicker({
                format: "dd-M-yyyy",
                todayHighlight: true,
                autoclose: true,

            });
            // sets today's date as default// disables all dates before today
            // toolbarplacement: 'top'
            $("[data-ctrl-id='btn-submit']").on('click', function () {
                GetDashboardRecord();
            });
            $("[data-ctrl-id='btn-print']").on('click', function () {
                showreport();

            });
            //--------------------------------------------------------------------------------
        }
        );
