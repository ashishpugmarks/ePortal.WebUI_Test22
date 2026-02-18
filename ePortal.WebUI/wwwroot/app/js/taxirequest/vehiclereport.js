  function showreport(userid, fromdate, todate) {
        var url = "PrintableList.aspx?id=" + userid + "&sfromdate=" + fromdate + "&stodate=" + todate;
        window.showModalDialog(url, "Taxi Usage Report", "dialogHeight:600px;dialogWidth:1000px;status:0;resizable:0;edge:raised;scroll:1");
    }
function GetDashboardRecord(crnt) {
    var data = {
        DATEOFTRAVELFROM: $('#txtFrom').val(),
        DATEOFTRAVELTO: $('#txtTo').val()
    }

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/TaxiRequest/VehicleReport",
        data: data,
        //  contentType: "application/json; charset=utf-8",
        //  datatype: "json",
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
            $("[data-ctrl-id='btn-cancel']").on('click', function () {
                document.getElementById("frmTaxi").reset();

            });
            //--------------------------------------------------------------------------------
        }
        );
