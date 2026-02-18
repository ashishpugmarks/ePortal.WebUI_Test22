$('.homeContent').removeClass('homeContent');
$(document).ready(function () {
    $('#btnSubmit').on('click', function () {
        SubmitData();
    });
    $('#btnCancel').on('click', function () {
        ResetData();
    });
    $('#ddlMasterType').on('change', function () {
        getMasterDataList();
    });
});
function SubmitData() {
    
    if (ValidateData()) { return; }
    var data = {
        Code: $('#txtCode').val(),
        CodeDesc: $('#txtDesc').val(),
        GroupName: $('#ddlMasterType').val(),
        PgroupName: $('#txtPGroup').val(),
        IsActive: $('#chkStatus').val()!=""?"1":"0",
        CmdId: $('#txtCmd').val() == "" ? 0 : $('#txtCmd').val()
    }
    
    var urlval = "";
    if ($("#btnSubmit").val() == "Update") {
        urlval = "/CustomerMgmt/CustomerMasterDataUpdate";
    }
    else if($("#btnSubmit").val() == "Submit")
    {
        urlval = "/CustomerMgmt/CustomerMasterData";
    }

    $.ajax({
        type: "POST",
        url: urlval,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data.code == "1") {
                getMasterDataList();
                ShowMessage("success", data.message)
                sweetAlert("Customer Master", data.message, "success");
                ResetData();
            }
            else {
                ShowMessage("error", data.message)
                sweetAlert("Customer Master", data.message, "error");
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function ValidateData() {
    var flag = false;
    var msg = "";
    if ($('#ddlMasterType').val() == "") {
        flag = true;
        msg += "Select Master Type!";
    }
    if ($('#txtCode').val == "") {
        flag = true;
        msg += "Enter the code!";
    }
    if ($('#txtDesc').val() == "") {
        flag = true;
        msg += "Enter the Description!";
}
    if (flag) {
        ShowMessage("error", msg);
    }
    else {
        ShowMessage("", "");
    }
    return flag;

}
function ShowMessage(messagetype, message) {
    if (messagetype == "error") {
       
        $("#errorPanel").show();
        $("#infoPanel").hide();
        $("#errorMsg").text(message);
        $("#infoMsg").text("");

    }
    else if (messagetype == "success") {
        $("#errorPanel").hide();
        $("#infoPanel").show();
        $("#infoMsg").text(message);
        $("#errorMsg").text("");
    }
    else {
        $("#errorPanel, #infoPanel").hide();
        $("#errorMsg, #infoMsg").text("");
    }
   
}
function ResetData() {
    $("#txtCode").val("");         
    $("#txtDesc").val("");
    $("#txtPGroup").val("");   
    $('#chkStatus').prop("checked", false);
    $("#btnSubmit").val("Submit");
   
}

function EditMasterData(row_id) {
    var rwno1 = $(row_id).attr('data-id');
    var data = {
        code: $('#ddlMasterType').val(),
        message: rwno1
    }
   
    $.ajax({
        type: "POST",
        url: "/CustomerMgmt/CustomerMasterList",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {

            if (data == "error") {
                $("#MasterDataList").html("");
                ShowMessage("error", "")
                sweetAlert("Customer Master", "Error in Databinding", "error");
            }
            else {
                
                $("#txtCode").val(data.Code);
                $("#txtDesc").val(data.CodeDesc);
                $("#txtPGroup").val(data.PgroupName);
                if (data.IsActive == 1) {
                    $('#chkStatus').prop("checked", true);
                }
                else {
                    $('#chkStatus').prop("checked", false);
                }
                $("#txtCmd").val(data.CmdId);
                $("#btnSubmit").val("Update");
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
    }
function getMasterDataList() {
        if ($('#ddlMasterType').val() == "") {
            ShowMessage("error", "Error in process")
            return;
        }
        var data = {
            code: $('#ddlMasterType').val()
        }

        $.ajax({
            type: "POST",
            url: "/CustomerMgmt/CustomerMasterDataList",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {

                if (data == "error") {
                    $("#MasterDataList").html("");
                    ShowMessage("error", "")
                    sweetAlert("Customer Master", "Error in Databinding", "error");
                }
                else {
                    $("#MasterDataList").html(data);
                }
            },
            error: function () {
                sweetAlert("Oops...", "Something went wrong!", "error");
            }
        });
    }