$('.homeContent').removeClass('homeContent');
$(document).ready(function () {
    $('#txtdatefrom').datepicker({
        format: 'dd-M-yyyy',   // Date format
        todayHighlight: true,    // Highlight today's date
        autoclose: true,         // Close the datepicker after selection
        clearBtn: true,          // Adds a "Clear" button
        orientation: 'top auto'  // Position the calendar below the input
    });
    $('#txtdateto').datepicker({
        format: 'dd-M-yyyy',   // Date format
        todayHighlight: true,    // Highlight today's date
        autoclose: true,         // Close the datepicker after selection
        clearBtn: true,          // Adds a "Clear" button
        orientation: 'top auto'  // Position the calendar below the input
    });

    $('#calendarIcon').click(function () {
        $('#txtdatefrom').datepicker('show');
    });
    $('#ImgBtnToDate').click(function () {
        $('#txtdateto').datepicker('show');
    });
    ShowCustomerRequestData();
    $('#btnsubmit').on('click', function () {
        ShowCustomerRequestData();
    });
    $('#btncancelfrm').on('click', function () {
        ClearData();
        ShowCustomerRequestData();
    });
    $('#btnExport').on('click', function () {
        GetExcelData($(this));
    });
});

function ShowCustomerRequestData() {

    var data = {
        REQUESTTYPE: $('#ddlrequesttype').val(),
        CUSTACCGROUP: $('#ddlcustomeraccgrp').val(),
        REQDATEFROM: $('#txtdatefrom').val(),
        REQDATETO: $('#txtdateto').val(),
        CUSTCODE: $('#txtcustomercode').val(),
        STATUS: $('#ddlstatus').val(),
        CUSTOMERNAME: $('#txtName').val(),
        REQUEST_NO: $('#txtReq').val(),
        USERTYPE: 'U'
    }

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/CustomerMgmt/GetCustomerRequestList",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data == "error") {
                $("#RequestDataList").html("");
                ShowMessage("error", "Error in data binding")
                sweetAlert("Customer Master", "Error in Databinding", "error");
            }
            else {
                $("#RequestDataList").html(data);
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

function ClearData() {
    $('#ddlrequesttype').val("");
    $('#ddlcustomeraccgrp').val("");
    $('#txtdatefrom').val("");
    $('#txtdateto').val("");
    $('#txtcustomercode').val("");
    $('#ddlstatus').val("");
    $('#txtName').val("");
    $('#txtReq').val("");
}
function openDialog(url, w, h) { window.open(url, "_blank", `width=${w},height=${h},resizable=yes,scrollbars=yes`); }
function GetExcelData(crnt) {

    var data = {
        REQUESTTYPE: $('#ddlrequesttype').val(),
        CUSTACCGROUP: $('#ddlcustomeraccgrp').val(),
        REQDATEFROM: $('#txtdatefrom').val(),
        REQDATETO: $('#txtdateto').val(),
        CUSTCODE: $('#txtcustomercode').val(),
        STATUS: $('#ddlstatus').val(),
        CUSTOMERNAME: $('#txtName').val(),
        REQUEST_NO: $('#txtReq').val(),
        USERTYPE: 'U'
    }

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/CustomerMgmt/GetCustomerRequestExcel",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data == 1) {
                window.location.href = "/CustomerMgmt/DownloadReportExcel";
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
