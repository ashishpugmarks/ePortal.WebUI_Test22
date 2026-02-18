$('.homeContent').removeClass('homeContent');
$(document).ready(function () {
    $('#btnSubmit').on('click', function () {
        SubmitData($(this));
    });
    $('#btnCancel').on('click', function () {
        ResetFinance();
        ResetSwitchApprover();
    });
    $('#ddlMasterType').on('change', function () {
        //getMasterDataList();
    });
    $('#optFinance').on('change', function () {
        ActionRequest($(this));
        ShowFinanceApproverData();
    });
    $('#optSwitchApprover').on('change', function () {
        ActionRequest($(this));
    });
    ActionRequest($('#optFinance'));
    
    $('#btnCheck').on('click', function () {
        ShowApproverDetails();
    });
    $('#TxtEmpCode1').on('change', function () {
        ShowFinUserName($(this));
    });
    $('#TxtEmpCode1').on('change', function () {
        ShowFinUserName($(this));
    });
    $('#txtCurrentApprpver').on('change', function () {
        ShowUserName($(this), $('#lblCurrentApprover'));
    });
    $('#txtNewApprover').on('change', function () {
        ShowUserName($(this), $('#lblNewApprover'));
    });

});
function ActionRequest(id) {
    if ($(id).val() == "2") {
        $('#TblPanelFinance').show();
        $('#TblPanelSwitchApprover').hide();
        ShowFinanceApproverData();
    }
    else {
        $('#TblPanelFinance').hide();
        $('#TblPanelSwitchApprover').show();
        ShowSwitchApproverData();
    }
}
function ShowMessage(messagetype, message) {
    if (messagetype == "error") {
        $('#trMessagePanel').show();
        $("#errorPanel").show();
        $("#infoPanel").hide();
        $("#errorMsg").text(message);
        $("#infoMsg").text("");

    }
    else if (messagetype == "success") {
        $('#trMessagePanel').show();
        $("#errorPanel").hide();
        $("#infoPanel").show();
        $("#infoMsg").text(message);
        $("#errorMsg").text("");
    }
    else {
        $('#trMessagePanel').hide();
        $("#errorPanel, #infoPanel").hide();
        $("#errorMsg, #infoMsg").text("");
    }

}
function ShowApproverDetails() {

    if ($('#txtCustomerRequestno').val() == "") {
        ShowMessage("error", "Please enter customer request number.")
        return;
    }
    var data = {
        code: $('#txtCustomerRequestno').val()
    }

    $.ajax({
        type: "POST",
        url: "/CustomerMgmt/CustomerReqApproverDetail",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {

            if (data == "error") {
                $("#CustReqApprDetails").html("");
                ShowMessage("error", "")
                sweetAlert("Customer Master", "Error in Databinding", "error");
            }
            else {
                divs = '';

                divs += ' <table class="tableGridView" width="100%" cellpadding="0" cellspacing="0">';
                divs += '    <thead class="grdHeader" style="height:22px"><tr>';
                divs += '            <th>Cust. Req. No.</th>';
                divs += '            <th>Initiator Details</th>';
                divs += '            <th>Recommendation Authority</th>';
                divs += '            <th>Approval Authority</th>';
                divs += '            <th>Finance</th>';
                divs += '            <th>Finance2</th>';
                divs += '            <th>Status</th>';
                divs += '        </tr></thead><tbody>';
                var i = 0;
                $.each(data, function (i, item) {                    
                    i = i + 1;
                    var isAlt = (i % 2) == 1; // alternating row like GridView
                    divs += '   <tr class="@(isAlt ? " alt-row" : "grdrow")" >';
                    divs += '<td>' + item.GEN_REQUEST_NO + '</td>';
                    divs += '<td>' + item.INITIATOR + '</td>';
                    divs += '<td>' + item.Recommendation + '</td>';
                    divs += '<td>' + item.APPROVAL + '</td>';
                    divs += '<td>' + item.FINANCE + '</td>';
                    divs += '<td>' + item.FINANCE2 + '</td>';
                    divs += '<td>' + item.Status + '</td>';
                    divs += '</tr >';               

                });
                if (i == 0) {
                    divs += ' <tr><td colspan="7">Invalid Customer Request</td></tr>'; 
                }
                divs += '</tbody ></table >';
                $("#CustReqApprDetails").html(divs);
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function ShowFinanceApproverData() {
    $("#DataBindSIS").hide();
    $("#DataBindPPCFIN").show();
    $.ajax({
        type: "GET",
        url: "/CustomerMgmt/FinanceApproverData",
       // data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {

            if (data == "error") {
                $("#DataBindPPCFIN").html("");
                $("#DataBindSIS").html("");
                ShowMessage("error", "")
                sweetAlert("Customer Master", "Error in Databinding", "error");
            }
            else {               
                $("#DataBindPPCFIN").html(data);
                $("#DataBindSIS").html("");
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function ShowSwitchApproverData() {
    $("#DataBindSIS").show();
    $("#DataBindPPCFIN").hide();
    $.ajax({
        type: "GET",
        url: "/CustomerMgmt/GetSwitchApproverData",
        // data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data == "error") {
                $("#DataBindSIS").html("");
                $("#DataBindPPCFIN").html("");
                ShowMessage("error", "")
                sweetAlert("Customer Master", "Error in Databinding", "error");
            }
            else {
                
                $("#DataBindSIS").html(data);
                $("#DataBindPPCFIN").html("");
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function ShowFinUserName(id) {
    if ($(id).val() == "") {
        return;
    }
    var data = {
        code: $(id).val()
    }
    $.ajax({
        type: "POST",
        url: "/CustomerMgmt/GetApproverName",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data.Name == "error") { 
                $('#LblEmpName1').text("");
                ShowMessage("error", "")
                sweetAlert("Customer Master", "Invalid employee code!", "error");
            }
            else {               
                $('#LblEmpName1').text(data.Name);              
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function ShowUserName(id, lblid) {
    if ($(id).val() == "") {
         return;
    }
    var data = {
        code: $(id).val()
    }
    $.ajax({
        type: "POST",
        url: "/CustomerMgmt/GetApproverName",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data.Name == "error") {
                $(lblid).text("");
                ShowMessage("error", "")
                sweetAlert("Customer Master", "Invalid employee code!", "error");
            }
            else {
                $(lblid).text(data.Name);
            }
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function FnValidate() {
    var rtrn = false;
    try { 
     if ($('#optFinance').is(':checked')) 
        {
                var rtrn = false;
                // Plant Type is mandatory
             if ($('#ddlMastertype1').val() == "") {
                 ShowMessage("error", "Please select master type!");
                    return false;
                }
                // Master Type is mandatory
             else if($('#ddlRequestType').val() == "") {
                 ShowMessage("error", "Please select request type!");
                    return false;
                }
                // Authorization Type Emp code is mandatory
             else if($('#TxtEmpCode1').val() == "")
             {
                 ShowMessage("error", "Please enter employee code of finance approver!");
                    return false;
             }
             else {
                 ShowMessage("", "");
                    return true;
                }
            }
         elseif($('#optSwitchApprover').is(':checked'))
         {
             if ($('#ddlApprovalRole').val() == "") {
                 ShowMessage("error", "Please select Approver Role!");
                 return false;
             }
             else if($('#txtCurrentApprpver').val() == "") {
                 ShowMessage("error", "Please enter current Approver employee code!");
                 return false;
             }
             else if($('#txtNewApprover').val() == "") {
                 ShowMessage("error", "Please enter New Approver employee code!");
                 return false;
             }
             else if($('#txtRemarks').val() == "") {
                 ShowMessage("error", "Please enter remarks!");
                 return false;
             }
            else if($('#txtCustomerRequestno').val() == "")
            {
                 ShowMessage("error", "Please enter customer request number!");
                 return false;
             }
            else if($('#fileUpload').val() === '')
            {
                 ShowMessage("error", "Please upload reference document file!");
                 return false;
             }
            else
            {
                 ShowMessage("", "");
                 return true;
             }
        }
    } catch (ex) {
        console.log('');
    }
         return rtrn;
}
function SubmitData(id)
{    
    if (FnValidate() == false) {        
        return;
    }
    if ($('#optFinance').is(':checked'))
    {
        if ($(id).val() == "Submit" && (!$('#chkFINActiveUser').is(':checked'))) {
            ShowMessage("error", "Please check Authorization Active User Check Box!");
            return;
        }
        var data = {
            ActionType: $(id).val(),
            REF_ID: $('#hdRefID').val() == '0' ? 0 : parseInt($('#hdRefID').val()),
            MasterType: $('#ddlMastertype1').val(),
            RequestType: $('#ddlRequestType').val(),
            AuthorizationAuth: $('#TxtEmpCode1').val(),
            AuthActive: $('#chkFINActiveUser').is(':checked')?1:0            
        }

        $.ajax({
            type: "POST",
            url: "/CustomerMgmt/SaveFinanceData",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {

                if (data.code == "0") {
                    ShowMessage("error", data.message);
                    sweetAlert("Customer Master", "Error in Databinding", "error");
                }
                else if(data.code == "0") {
                    ShowMessage("success", data.message);
                    sweetAlert("Customer Master", "Error in Databinding", "success");
                    ResetFinance();
                    ShowFinanceApproverData();
                }
            },
            error: function () {
                sweetAlert("Oops...", "Something went wrong!", "error");
            }
        });
    }
    
}
function EditData(id) {
    var rf_id = $(id).attr('data-id');
    var emp = $(id).attr('data-empcode');
    var act = $(id).attr('data-empactive');
    var reqtype = $(id).attr('data-emp-type');
    var ecode = emp.substr(0, emp.indexOf("-"));
    var fintype = (reqtype == "Finance") ? "A" : "B";
    var activetype = (act == 1) ? true : false;
    $('#hdRefID').val(rf_id);
    $('#ddlMastertype1').val("1");
    $('#ddlRequestType').val(fintype);   
    $('#chkFINActiveUser').prop("checked", activetype);
    $('#btnSubmit').val("Update");
    $('#TxtEmpCode1').val(ecode);
}
function ResetFinance() {
    $('hdRefID').val("0");
    $('#ddlMastertype1').val("");
    $('#ddlRequestType').val("");
    $('#chkFINActiveUser').prop("checked", false);
    $('#btnSubmit').val("Submit");
    $('#TxtEmpCode1').val("");
}
function ResetSwitchApprover() {
    $('txtCustomerRequestno').val("");
    $('#ddlApprovalRole').val("");
    $('#txtCurrentApprpver').val("");
    $('#txtNewApprover').val("");
    $('#txtRemarks').val("");
    $('#btnSubmit').val("Submit");    
}