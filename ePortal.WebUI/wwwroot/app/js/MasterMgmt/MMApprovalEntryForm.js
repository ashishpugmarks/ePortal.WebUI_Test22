$(document).ready(function () {
    $('a').on('click', function () {
        let upath = $(this).attr('data-id');
        console.log(upath);
        openPopup(upath, 980, 780);
    });
    $('#gvMMCreationRequest tbody').on('click', '[data-ctrl-id="viewhistory"]', function (e) {
        e.preventDefault();
        let pathval = $(this).attr('data-id');
        openPopup(pathval, 980, 780);
    });
    $('#gvMMExtendRequest tbody').on('click', '[data-ctrl-id="viewhistory"]', function (e) {
        e.preventDefault();
        let pathval = $(this).attr('data-id');
        openPopup(pathval, 980, 780);
    });
    $('#gvUpdateRequest tbody').on('click', '[data-ctrl-id="viewhistory"]', function (e) {
        e.preventDefault();
        let pathval = $(this).attr('data-id');
        openPopup(pathval, 980, 780);
    });

    $('#ddlMaterialType').on('change', function () {
        ChangeMaterialType($(this));
    });
    $('#ddlMaterialGroup').on('change', function () {
        ChangeMaterialGroup($(this));
    });
    $('#btnSubmit').on('click', function () {
        SubmitResponse($(this));
    });
    $('#btnUpdate').on('click', function () {
        UpdatedData($(this));
    });
    $('#btnCancel').on('click', function () {
        resetCancel();
    });
    $('#btnReset').on('click', function () {
        resetForm();
    });

});
/***************Bind Material Group***************************/
function ChangeMaterialType(id) {
    try {
        let materialtype = $(id).val();

        if (materialtype == 0 || materialtype == "") {
            $('#chSales').prop('checked', false).prop('disabled', false);
            var dt = '<option value="0" selected>--Select--</option>';
            $("#ddlMaterialGroup").html(dt);
            $("#ddlValuationClass").html(dt);
            return;
        }


        // $(id).prop('disabled', true).text('Verify Bank...');
        var data = {
            Code: materialtype
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/MaterialTypeChange",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == "1") {
                    $("#ddlMaterialGroup").html("");
                    $("#ddlValuationClass").html("");
                    var dt = '<option value="0">-- Select --</option>';
                    $("#ddlMaterialGroup").html("");
                    $("#ddlValuationClass").html(dt);

                    $.each(data.list, function (i, item) {
                        dt += '<option value="' + item.Id + '">' + item.Description + '</option>';
                    });
                    $("#ddlMaterialGroup").html(dt);
                    if (data.ISSALES == "1") {
                        $('#chSales').prop('checked', true).prop('disabled', false);
                    }
                    else {
                        $('#chSales').prop('checked', false).prop('disabled', true);
                    }
                    SalesViewEnable($('#chSales'));
                }
                else if (data.Rs == "0") {
                    var dt = '<option value="0" selected>--Select--</option>';
                    $("#ddlMaterialGroup").html(dt);
                    $("#ddlValuationClass").html(dt);
                    sweetAlert("Material Master", data.Message, "warning");
                }


            },
            error: function () {
                // $btn.prop('disabled', false).text('Verify Bank');
                sweetAlert("Oops...", "Something went wrong!", "error");
            },
            complete: function () {
                // $(id).prop('disabled', false).text('Verify Bank');
                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (ex) {
        // $(id).prop('disabled', false).text('Verify Bank');
        console.log(ex);
        sweetAlert("Material Master", "Oops... Something went wrong!.", "warning");
    }
}
/***************Bind Valuation class***************************/
function ChangeMaterialGroup(id) {
    try {
        var materialgroup = $(id).val();
        let materialtype = parseInt($('#ddlMaterialType').val());

        if (materialtype == null || materialtype == 0) {
            var dt = '<option value="0" selected>--Select--</option>';
            $("#ddlValuationClass").html(dt);
            return;
        }
        if (materialgroup == null || materialgroup == "0") {
            var dt = '<option value="0" selected>--Select--</option>';
            $("#ddlValuationClass").html(dt);
            return;
        }


        // $(id).prop('disabled', true).text('Verify Bank...');
        var data = {
            Id: materialtype,
            Code: materialgroup
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/MaterialGroupChange",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == "1") {
                    $("#ddlValuationClass").html("");
                    var dt = '<option value="0">-- Select --</option>';

                    $.each(data.list, function (i, item) {
                        dt += '<option value="' + item.Id + '">' + item.Description + '</option>';
                    });
                    $("#ddlValuationClass").html(dt);
                }
                else if (data.Rs == "0") {
                    var dt = '<option value="0" selected>--Select--</option>';
                    $("#ddlValuationClass").html(dt);
                    sweetAlert("Material Master", data.Message, "warning");
                }

            },
            error: function () {
                // $btn.prop('disabled', false).text('Verify Bank');
                sweetAlert("Oops...", "Something went wrong!", "error");
            },
            complete: function () {
                // $(id).prop('disabled', false).text('Verify Bank');
                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (ex) {
        // $(id).prop('disabled', false).text('Verify Bank');
        console.log(ex);
        sweetAlert("Material Master", "Oops... Something went wrong!.", "warning");
    }
}
/**************Submit Approval Request****************************/
function SubmitResponse(id) {
    try {
        if (!FnValidate()) {
            return;
        }
        let requestid = $('#hdMMDetailId').val();
        let status = $('#ddlStatus').val();
        let remarks = $('#txtRemarks').val();
        let hdMultiple = $('#hdMultiple').val();  
        let hdListName = $('#hdListName').val();

        var MMDetailIDList = [];
        if (requestid.substring(0, 1) == "C") {
            MMDetailIDList = getAllMMDetailIds("C");
        }
        else if (requestid.substring(0, 1) == "E") {
            MMDetailIDList = getAllMMDetailIds("E");
        }
        else if (requestid.substring(0, 1) == "U") {
            MMDetailIDList = getAllMMDetailIds("U");
        }
        else {
            sweetAlert("Material Master", "Invalid process!", "warning");
            retur;
        }
        //if (hdMultiple == "YES" && hdListName !="") {
        //    MMDetailIDList = getCheckedIds(hdListName)
        //}
        var data = {
            MMDetailID: requestid,
            Status: status,
            Remarks: remarks,
            IsMultiple: hdMultiple,
            MMDetailHIDs: MMDetailIDList
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/SubmitApprovalRequest",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == "1") {
                    sweetAlert("Material Master", data.Message, "success");
                    window.location.href = "MaterialMasterRequestApproval";
                }
                else if (data.Rs == "0") {                  
                    //sweetAlert("Material Master", data.Message, "warning");
                    ShowError(this, data.Message);
                }

            },
            error: function () {
                // $btn.prop('disabled', false).text('Verify Bank');
                sweetAlert("Oops...", "Something went wrong!", "error");
            },
            complete: function () {
                // $(id).prop('disabled', false).text('Verify Bank');
                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (ex) {
        // $(id).prop('disabled', false).text('Verify Bank');
        console.log(ex);
        sweetAlert("Material Master", "Oops... Something went wrong!.", "warning");
    }
}
/****************************************************/
function getAllMMDetailIds(lstName) {
    if (lstName == "C") {
        var ids = $("#gvMMCreationRequest")
            .find('[data-ctrl-id="MMDetailIds"]')
            .map(function () {
                return $(this).attr("data-id"); // read the data-id attribute
            })
            .get(); // convert jQuery object to plain array
        // Select all elements with data-ctrl-id="MMDetailIds" inside the table
        return ids;
    }
    else if (lstName == "E") {
        var ids = $("#gvMMExtendRequest")
            .find('[data-ctrl-id="MMDetailIds"]')
            .map(function () {
                return $(this).attr("data-id"); // read the data-id attribute
            })
            .get(); // convert jQuery object to plain array
        // Select all elements with data-ctrl-id="MMDetailIds" inside the table
        return ids;
    }
    else if (lstName == "U") {
        var ids = $("#gvUpdateRequest")
            .find('[data-ctrl-id="MMDetailIds"]')
            .map(function () {
                return $(this).attr("data-id"); // read the data-id attribute
            })
            .get(); // convert jQuery object to plain array
        // Select all elements with data-ctrl-id="MMDetailIds" inside the table
        return ids;
    }
    else {
        var a = [];
        return a;
    }
   
}

/*****************Update Material Details******************************************/
function UpdatedData(id) {
    try {
        if (!FnUpdateValidate()) {
            return;
        }

        var _materialType = parseInt($("#ddlMaterialType").val() || 0, 10);
        var _materialGroup = parseInt($("#ddlMaterialGroup").val() || 0, 10);
        var _valuationClass = parseInt($("#ddlValuationClass").val() || 0, 10);
        var _materialDetailCode = parseInt($("#lbl_MaterialDetailCode").val().trim() || 0, 10);       
        if (_materialDetailCode === 0 || _materialType === 0 || _materialGroup === 0 || _valuationClass === 0) {
            e.preventDefault();            
            sweetAlert("Material Master", "Please select Material Type, Group, Valuation Class, and valid Material Detail Code.", "warning");
            return false;
        }
        
        // $(id).prop('disabled', true).text('Verify Bank...');
        var data = {
            materialDetailCode: _materialDetailCode,
            materialType: _materialType,
            materialGroup: _materialGroup,
            valuationClass: _valuationClass
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/UpdateApprovalRequest",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == "1") {
                    sweetAlert("Material Master", data.Message, "success");
                    window.location.href = "MasterMgmt/MaterialMasterRequestApproval";
                }
                else if (data.Rs == "0") {
                    //sweetAlert("Material Master", data.Message, "warning");
                    showError(this, data.Message);
                }

            },
            error: function () {
                // $btn.prop('disabled', false).text('Verify Bank');
                sweetAlert("Oops...", "Something went wrong!", "error");
            },
            complete: function () {
                // $(id).prop('disabled', false).text('Verify Bank');
                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (ex) {
        // $(id).prop('disabled', false).text('Verify Bank');
        console.log(ex);
        sweetAlert("Material Master", "Oops... Something went wrong!.", "warning");
    }
}
/****************Fetch Check data**************************/
function getCheckedIds(listtype) {
    if (listtype == "gvMMCreationRequest") {
        const ids = $("#gvMMCreationRequest tbody input[type='checkbox'][data-ctrl-id='chkApprove']:checked")
            .map(function () {
                return $(this).val();
            })
            .get(); // convert jQuery object to plain array
        return ids;
    }
    else if (listtype == "gvUpdateRequest") {
        const ids = $("#gvUpdateRequest tbody input[type='checkbox'][data-ctrl-id='chkApprove']:checked")
            .map(function () {
                return $(this).val();
            })
            .get(); // convert jQuery object to plain array
     return ids;
    }
    else if (listtype == "gvMMExtendRequest") {
        const ids = $("#gvMMExtendRequest tbody input[type='checkbox'][data-ctrl-id='chkApprove']:checked")
            .map(function () {
                return $(this).val();
            })
            .get(); // convert jQuery object to plain array
        return ids;
    }
    return;
    
}

/*************Reset Cancel*****************************/
function resetCancel() {
    // Clear label text
    $("#lbl_MaterialDetailCode").text("");

    // Clear dropdown options
    $("#ddlMaterialGroup").empty();
    $("#ddlMaterialType").empty();
    $("#ddlValuationClass").empty();

    // Hide sections
    $("#MaterialDetail1, #MaterialDetail2, #MaterialDetail3").hide();
}
/***************Reset Data****************************** */
function resetForm() {
    // Clear remarks textbox
    $("#txtRemarks").val("");
    $("#ddlStatus").val("");
    $("#chkTerms").prop('checked', false);
    try {
        $("#gvMMCreationRequest")
            .find("input[type='checkbox'][id='chkApprove'], input[type='checkbox'][data-ctrl-id='chkApprove']")
            .prop("checked", false);

    } catch (ex) { }
  
}

/******************************************/
function FnValidate() {
    var hiddenVal = document.getElementById('idchkval').value;
   
    if (hiddenVal == "All") {
       
        if (false == FnValidateTextRquired(document.getElementById("txtRemarks"), "Please enter remarks.")) {
            return false;
        }
       if (false == FnValidateDDLRequired(document.getElementById("ddlStatus"), "Status is mandatory")) {
            return false;
        }

        else if (false == FnValidateCheckBoxRequired(document.getElementById("chkTerms"), "Please accept the terms & conditions.")) {
            return false;
        }

        else {
            return true;
        }
    }
    else if (hiddenVal == "fin") {
       
        if (false == FnValidateTextRquired(document.getElementById("txtRemarks"), "Please enter remarks.")) {
            return false;
        }
  
        else if (false == FnValidateDDLRequired(document.getElementById("ddlStatus"), "Status is mandatory")) {
            return false;
        }
        else {
            return true;
        }
    }
}
function FnUpdateValidate() {
    var rtrn = false;

    // Material type is mandatory
    if (false == FnValidateDDLRequired(document.getElementById("ddlMaterialType"), "Material type is mandatory")) {
        return false;
    }
    else if (false == FnValidateDDLRequired(document.getElementById("ddlMaterialGroup"), "Material group is mandatory"))
   {
       return false;
   }
    else if (false == FnValidateDDLRequired(document.getElementById("ddlValuationClass"), "Valuation class is mandatory")) {
    return false;
    }
    else
    {
    return true;
    }
    }
function FnValidateCheckBoxRequired(ctrlDDL, msg) {
    if (ctrlDDL.checked == false) {
        return ShowError(ctrlDDL, msg);
    }
}
function FnValidateDDLRequired(ctrlDDL, msg) {
    if (ctrlDDL.options[ctrlDDL.selectedIndex].value == "0") {
        return ShowError(ctrlDDL, msg);
    }
}
function FnValidateTextRquired(ctrlText, msg) {
    if (ctrlText.value == "") {
        return ShowError(ctrlText, msg);
    }
}
function FnValidateDDLRequired(ctrlDDL, msg) {
    if (ctrlDDL.options[ctrlDDL.selectedIndex].value == "0") {
        return ShowError(ctrlDDL, msg);
    }
}
function ShowError(ctrl, msg) {
    document.getElementById("errorPanel").style.display = "inline";
    document.getElementById("errorMsg").innerHTML = msg;
    sweetAlert("Material Master", msg, "warning");
    ctrl.focus();

    return false;
}
function FnValidateCheckBoxRequired(ctrlDDL, msg) {
    if (ctrlDDL.checked == false) {
        return ShowError(ctrlDDL, msg);
    }
}
function openPopup(strOpenUrl, width, height) {
    window.open(strOpenUrl, "mywindow", "TOOLBAR=no,MENUBAR=no,RESIZABLE=no,SCROLLBARS=yes,LOCATION=no,DIRECTORIES=no,STATUS=no,width=" + width + ",height=" + height);
}