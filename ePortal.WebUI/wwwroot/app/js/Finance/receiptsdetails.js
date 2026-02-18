$(document).ready(function () {
    $('.dpDate').datepicker({
        format: "dd-M-yyyy",
        todayHighlight: true,
        autoclose: true,

    });

    $("[data-ctrl-id='addReceipt']").on('click', function () {
        addReceipt();
    });
    $("[data-ctrl-id='editReceipt']").on('click', function () {
        EditReceipt($(this)[0]);
    });
    $("[data-ctrl-id='deleteReceipt']").on('click', function () {
        deleteReceipt($(this)[0]);
    });
    $("[data-ctrl-id='btnsubmitReceipt']").on('click', function () {
        submitReceipt();
    });
    $("[data-ctrl-id='btncancelReceipt']").on('click', function () {
        $('#HRAModal').modal('hide');
    });

});

function addReceipt() {

    var data = {
        Id: parseInt($("#NewReceipt_SLNO").val()),
        TaxId: $("#NewReceipt__TaxHeadID").val(),
        ReceiptNo: $("#NewReceipt__ReceiptNo").val(),
        Date: $("#txtDtFrom").val(),
        Amount: $("#NewReceipt_Amount").val(),

    }


    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/AddReceiptDetails",
        data: data,
        // contentType: "application/json; charset=utf-8",
        // datatype: "json",
        success: function (data) {
            $("#modelHead").html("HRA");
            $('#HRAModal').modal('hide');
            $("#modelHead").html($("#TaxHeadName").val());
            $('#divModel').html(data);



            $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
            $('#HRAModal').modal('show'); // then show it


        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function deleteReceipt(current) {
    var data = {
        Id: $(current).attr('data-value'),
        TaxId: $("#NewReceipt__TaxHeadID").val(),

    }


    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/DeleteReceiptDetails",
        data: data,
        // contentType: "application/json; charset=utf-8",
        // datatype: "json",
        success: function (data) {
            if (data.Item1 == "error") {
                alert(data.Item2);
            }
            else {
                $("#modelHead").html($("#TaxHeadName").val());
                $('#HRAModal').modal('hide');
                $("#modelHead").html($("#TaxHeadName").val());
                $('#divModel').html(data);



                $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
                $('#HRAModal').modal('show'); // then show it
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
function EditReceipt(current) {
    var data = $(current).attr('data-value');
    data = JSON.parse(data.replace(')', ''));
    debugger;
    $("#NewReceipt_SLNO").val(data["SLNO"].toString());
    $("#NewReceipt__TaxHeadID").val(data["_TaxHeadID"]);
    $("#NewReceipt__ReceiptNo").val(data["_ReceiptNo"]);
    $("#txtDtFrom").val(data["_PrmDate"]);
    $("#NewReceipt_Amount").val(data["Amount"]);
}
function submitReceipt() {
    var data = {
        TaxHead: $("#TaxHeadName").val(),
        TaxId: $("#NewReceipt__TaxHeadID").val(),

    }


    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/SubmitReceiptDetails",
        data: data,
        // contentType: "application/json; charset=utf-8",
        // datatype: "json",
        success: function (data) {
            debugger;
            $("[data-ctrl-id='" + $("#NewReceipt__TaxHeadID").val() + "']").val(data);
            $('#HRAModal').modal('hide');

            calculateTotal();
        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
