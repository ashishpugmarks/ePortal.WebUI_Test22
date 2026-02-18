
$(document).ready(function () {
    $('#chSales').on('change', function () {       
        SetSalesView();
    });
    $('#ddlPlantMst').on('change', function () {
        ChangePlant($(this));
    });

    $('#btnAddMore').on('click', function () {
        AddMaterialData($(this));
    });
    $('#gvMMUpdateDraftDetail').on('click', '[data-ctr-edit="EditItem"]', function () {
        const id = $(this).attr('data-id');
        console.log('Edit:', id);
        bindMMDetails($(this));
    });
    $('#gvMMUpdateDraftDetail').on('click', '[data-ctr-delete="DeleteItem"]', function () {
        const id = $(this).attr('data-id');
        console.log('Edit:', id);
        DeleteMaterialData($(this));
    });
    $("#txtMaterialCode").on("change", function (e) {
        MaterialCodeChange($(this));
    });
    $('#btnCancel').on('click', function () {
        resetData()
    });
    $('#btnSubmit').on('click', function () {
        SubmitMaterialRequest();
    });
    //$('a').on('click', function () {
    //    let selectedval = $(this).attr('');
    //});
    $('#txtMaterialDescription').on('keydown', function (e) {
        const isCtrl = e.ctrlKey || e.metaKey;
        const k = e.key.toLowerCase();
        if (isCtrl && (k === 'c' || k === 'v' || k === 'x')) {
            e.preventDefault();
        }
    });

    $('#txtMaterialDescription').on('paste cut copy contextmenu', function (e) {
        e.preventDefault();
    });

    $('#txtMaterialSpecification').on('keydown', function (e) {
        const isCtrl = e.ctrlKey || e.metaKey;
        const k = e.key.toLowerCase();
        if (isCtrl && (k === 'c' || k === 'v' || k === 'x')) {
            e.preventDefault();
        }
    });

    $('#txtMaterialSpecification').on('paste cut copy contextmenu', function (e) {
        e.preventDefault();
    });
});
function FnValidate() {

    if (!FnValidateTextRequired($("#txtMaterialCode"), "Material code is mandatory")) {
        return false;
    }

    return true;
}
function FnValidateDDLRequired($ctrlDDL, msg) {
    var value = ($ctrlDDL.val() ?? "").toString();
    if (value === "0" || value === "") {
        return ShowError($ctrlDDL, msg);
    }
    return true;
}
function FnValidateTextRequired($ctrlText, msg) {
    var val = ($ctrlText.val() ?? "").trim();
    if (val === "") {
        return ShowError($ctrlText, msg);
    }
    return true;
}
function FnValidateCheckBoxRequired($ctrlChk, msg) {
    if (!$ctrlChk.prop("checked")) {
        return ShowError($ctrlChk, msg);
    }
    return true;
}
function isNumeric(val) {
    return /^[0-9]+$/.test(val);
}
function FnCheckInp($ctrlTxt, msg) {
    var numeric = ($ctrlTxt.val() ?? "");
    var regex = /^[0-9]+$/;
    if (!regex.test(numeric)) {
        return showError($ctrlTxt, msg);
    }
    return true;
}
function MutExChkList(chk) {
    var chkList = chk.parentNode.parentNode.parentNode;
    var chks = chkList.getElementsByTagName("input");
    for (var i = 0; i < chks.length; i++) {
        if (chks[i] != chk && chk.checked) {
            chks[i].checked = false;
        }
    }
}
function FnSubmitValidate() {
    return validateCheckboxRequired('#chkTerms', 'Please accept the terms & conditions.');
}

/* ================= HELPER FUNCTIONS ================= */

function validateDDLRequired(selector, msg) {
    if ($(selector).val() === '0' || $(selector).val() === '' || $(selector).val() == null) {
        return showError(selector, msg);
    }
    return true;
}
function validateTextRequired(selector, msg) {
    if ($.trim($(selector).val()) === '') {
        return ShowMessage("error", msg);
    }
    return true;
}
function validateCheckboxRequired(selector, msg) {
    if (!$(selector).is(':checked')) {
        return showError(selector, msg);
    }
    return true;
}
function countTextChar(descSelector, specSelector, msg) {
    var totalLength =
        $(descSelector).val().length +
        $(specSelector).val().length;

    if (totalLength < 40) {
        return showError(descSelector, msg);
    }
    return true;
}
function ShowError(selector, msg) {
    sweetAlert("Material Master", msg, "warning");
    $('#infoPanel').hide();
    $('#errorPanel').show();
    $('#errorMsg').html(msg);
    $('#infoMsg').html("");
    $(selector).focus();
    return false;
}
function ShowMessage(messagetype, message) {
    if (messagetype == "error") {
        sweetAlert("Material Master", message, "error");
        $("#errorPanel").show();
        $("#infoPanel").hide();
        $("#errorMsg").text(message);
        $("#infoMsg").text("");

    }
    else if (messagetype == "success") {
        sweetAlert("Material Master", message, "success");
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
/* ================= POPUP ================= */
window.openPopup = function (url, width, height) {
    window.open(
        url,
        'mywindow',
        'TOOLBAR=no,MENUBAR=no,RESIZABLE=no,SCROLLBARS=yes,LOCATION=no,DIRECTORIES=no,STATUS=no,' +
        'width=' + width + ',height=' + height
    );
};

function SetSalesView() {
    const isSalesChecked = $('#chSales').is(':checked');
    if (isSalesChecked) {       
        $('#Tr101, #Tr102, #Tr103, #Tr104, #Tr105').show();       
        const selectedText = $('#ddlMaterialType option:selected').text() || '';
        const isZpac = selectedText.substring(0, 4).toUpperCase() == 'ZPAC';
        if (isZpac) {
            $('#Tr106').show();
        } else {
            $('#Tr106').hide();          
            $('#txtMatGrpPack').val('');
            $('#txtPackagingMatType').val('');
        }
    } else {
        // Hide all rows when not checked
        $('#Tr101, #Tr102, #Tr103, #Tr104, #Tr105, #Tr106').hide();
    }
}
function ChangePlant(id) {
    try {
        let plantid = $(id).val();

        if (plantid == 0 || plantid == "") {
            $('#txtProfitCentre').val('');
            var dt = '<option value="0" selected>--Select--</option>';
            $("#ddlStorageLoc").html(dt);
            // ShowMessage('error', 'Please select plant!');            
            return;
        }

        var data = {
            Code: plantid
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/PlantMstChange",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == "1") {
                    $('#txtProfitCentre').val(data.ProfitCentre);
                    //ShowMessage("error", data.Message);
                    //sweetAlert("Customer Master", data.Message, "error");
                    var dt = '<option value="">-- Select --</option>';
                    $("#ddlStorageLoc").html("");
                    $.each(data.list, function (i, item) {
                        dt += '<option value="' + item.Code + '">' + item.Description + '</option>';
                    });
                    $("#ddlStorageLoc").html(dt);
                }
                else if (data.Rs == "0") {
                    $('#txtProfitCentre').val('');
                    var dt = '<option value="0" selected>--Select--</option>';
                    $("#ddlTransportationCode").html(dt);
                    $('#ddlStorageLoc').text("");
                    sweetAlert("Material Master", data.Message, "warning");
                }
            },
            error: function () {
                // $btn.prop('disabled', false).text('Verify Bank');
                sweetAlert("Material Master", "Something went wrong!", "error");
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
function MaterialCodeChange(id) {
    try {
        const code = ($('#txtMaterialCode').val() || '').trim();       
        if (!code) {
            $('#lblMaterialType, #ddlMaterialType').hide();
            $('#ddlMaterialType').prop('selectedIndex', 0);
            ShowMessage('error','Material code is mandatory');
            $('#txtMaterialCode').val('').focus();
            return;
        }

        var $materialInput = $("#txtMaterialCode");
        var materialCode = ($materialInput.val() || "").trim();
        if (($materialInput.val() || "").trim() !== "") {
            if (!isNumeric(materialCode)) {
                ShowMessage("error", "Enter numeric value is mandatory");
                return;
            }
        }
        var plantCode = parseInt($("#ddlPlantMst").val() || 0, 10);

        if (materialCode == undefined || materialCode == "") {
            return;
        }
        $('#txtMaterialCode').prop('disabled', true);
        var data = {
            MaterialCode: materialCode,
            PlantCode: plantCode
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/CheckMaterialCodeUpdate",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == "1") {
                    $('#IsExist').show();
                    $('#txtMaterialCode').prop('disabled', false);
                    $('#lbl_textDesc, #lbl_textSpec, #lblMaterialType, #ddlMaterialType, #lblDescL, #lblSpecificationL').show();
                    $('#lbl_Description').html(data.MATERIALDISCRIPTION);
                    $('#lbl_Specification').html(data.MATERIALSPECIFICATION);
                    $('#ddlMaterialType').val(data.MATERIALTYPE).trigger('change');
                    const selectedText = $('#ddlMaterialType option:selected').text() || '';
                    if (selectedText.substring(0, 4).toUpperCase() == 'ZMPN') {
                        ShowMessage('error','Enter valid material code or ZMPN material type code cannot update!');                       
                        $('#lbl_textDesc, #lbl_textSpec, #lblMaterialType, #ddlMaterialType, #lblDescL, #lblSpecificationL').hide();                        
                        return; 
                    }

                    if (data.ISSALES == "YES") {
                        $('#lblchSales').show();  
                        $('#chSales').show().prop('disabled', false).prop('checked', true);
                        var dt = '<option value="">-- Select --</option>';
                        $("#ddlStorageLoc").html("");
                        $.each(data.list, function (i, item) {
                            dt += '<option value="' + item.Code + '">' + item.Description + '</option>';
                        });
                        $("#ddlStorageLoc").html(dt);

                       
                    }
                    else {
                        $('#lblchSales').hide();
                        $('#chSales').prop('checked', false).show().prop('disabled', true);
                    }
                   var dt1 = '<option value="0">-- Select --</option>';
                    $("#ddlPlantMst").html("");
                    $.each(data.Plantlist, function (i, item) {
                        dt1 += '<option value="' + item.Code + '">' + item.Description + '</option>';
                    });
                    $("#ddlPlantMst").html(dt1);
                    SetSalesView();
                }
                else if (data.Rs == "0") {
                    $('#lblchSales').hide();
                    $('#IsExist').hide();
                    $('#lbl_textDesc, #lbl_textSpec, #lbl_Description, #lbl_Specification, #lblDescL, #lblSpecificationL').hide();
                    $('#lbl_Description').html('');
                    $('#lbl_Specification').html('');
                    ShowMessage('error', data.Message);
                    $('#chSales').prop('checked', false).hide().prop('disabled', true);
                    SetSalesView();
                    $('#txtProfitCentre').val('');                   
                    $('#txtMaterialCode').val('').focus();
                    $('#lblMaterialType, #ddlMaterialType').hide();

                }
            },
            error: function () {
                // $btn.prop('disabled', false).text('Verify Bank');
                $('#txtMaterialCode').prop('disabled', false);
                sweetAlert("Material Master", "Something went wrong!", "error");
            },
            complete: function () {
                // $(id).prop('disabled', false).text('Verify Bank');
                $('#txtMaterialCode').prop('disabled', false);
                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (ex) {
        $('#txtMaterialCode').prop('disabled', false);
        // $(id).prop('disabled', false).text('Verify Bank');
        console.log(ex);
        sweetAlert("Material Master", "Oops... Something went wrong!.", "warning");
    }
}
////////////////////////////////////////////////////////////////////////
///////////////Add Material Item
function resetData() {
    $('#lblMaterialType, #ddlMaterialType').hide();  
    $('#lbl_Description').html('');
    $('#lbl_Specification').html('');
    $('#ddlMaterialType').prop('selectedIndex', 0);
    $('#ddlUOM').prop('selectedIndex', 0);
    $('#ddlindicator').prop('selectedIndex', 0);
    $('#ddlMPNProfile').prop('selectedIndex', 0);
    $('#ddlPlantSpMaterialStatus').prop('selectedIndex', 0);
    $('#txtMaterialDescription').val('');
    $('#txtMaterialSpecification').val('');
    $('#txthsncode').val('');
    $('#txtMaterialCode').val('');
    // $('#txtPrice').val(''); // uncomment if present  
    $('#IsExist').hide();  
   
    $('#chkTerms').prop('checked', false);
   
    $('#chSales').prop('disabled', false);
   
    
    $('#txtMaterialCode').focus();

   
    // $('#errorPanel, #infoPanel').hide().find('#errorMsg').text('');


    SetSalesView();

}
function validateSale() {
    var err = "";
    var isValid = true;
    const _lstSales = ($('input[name="lstSales"]:checked').val() || "");

    if (_lstSales != "") {

        // Helper to append error text and mark invalid
        function addErr(msg) {
            if (err.length > 0 && !err.endsWith("/")) err += "/";
            err += msg + "/";
            isValid = false;
        }

        if ($("#ddlItemCategGroup")[0]?.selectedIndex <= 0) addErr("Item Category");
        if ($("#ddlGenItemCatgGrp")[0]?.selectedIndex <= 0) addErr("Gen Item Cat Grp");
        if ($("#ddlCheckAvailCheck")[0]?.selectedIndex <= 0) addErr("Availability check");
        if ($("#ddlTaxClassiFication")[0]?.selectedIndex <= 0) addErr("Tax classification");
        if ($("#ddlTrasnsportGrp")[0]?.selectedIndex <= 0) addErr("Transportation Group");
        if ($("#ddlLoadingGrp")[0]?.selectedIndex <= 0) addErr("Loading Group");
        if ($("#ddlStorageLoc")[0]?.selectedIndex <= 0) addErr("Storage Location");
        if ($("#ddlDisbChnn")[0]?.selectedIndex <= 0) addErr("Distribution Channel");
        if ($("#ddlBaseUnitOfMeasure")[0]?.selectedIndex <= 0) addErr("Base unit of measure");
        if ($.trim($("#txtProfitCentre").val()).length <= 0) addErr("Profit Centre");
        //var materialTypeVal = $("#ddlMaterialType").val();
        //if (materialTypeVal === "6") {
        //    if ($.trim($("#txtMatGrpPack").val()).length <= 0) addErr("Mat. Grp Pack. Matls");
        //    if ($.trim($("#txtPackagingMatType").val()).length <= 0) addErr("Packaging mat. type");
        //} else {
        //    // Clear when not type 6
        //    $("#txtMatGrpPack").val("");
        //    $("#txtPackagingMatType").val("");
        //}
    }

    // Trim trailing slash if present
    if (err.endsWith("/")) {
        err = err.slice(0, -1);
    }

    return { isValid: isValid, err: err };
}
const readInt = (selector) => {
    const v = $(selector).val();
    const n = parseInt(v, 10);
    return isNaN(n) ? 0 : n;
};
const readDecimal = (selector, defaultVal = 0) => {
    const v = $(selector).val();
    const n = parseFloat(v);
    return isNaN(n) ? defaultVal : n;
};
function readTextUpperTrim(selector) {
    return ($(selector).val() || "").trim().toUpperCase();
}
function readTextTrim(selector) {
    return ($(selector).val() || "").trim();
}
function AddMaterialData(id) {
    try {
        let rs = FnValidate();
        if (rs == false) {
            return false;
        }
        const plantCode = readInt('#ddlPlantMst');
        const materialCode = readInt('#txtMaterialCode');
        const ddlTrasnsportGrp = $('#ddlTrasnsportGrp').val();
        const ddlLoadingGrp = $('#ddlLoadingGrp').val();
        const ddlBaseUnitOfMeasure = $('#ddlBaseUnitOfMeasure').val();
        const txtSalesOrg = $('#txtSalesOrg').val();
        const ddlDisbChnn = $('#ddlDistriChan').val();
        const ddlItemCategGroup = $('#ddlItemCategGroup').val();
        const ddlCheckAvailCheck = $('#ddlCheckAvailCheck').val();
        const txtProfitCentre = $('#txtProfitCentre').val();
        const ddlStorageLoc = $('#ddlStorageLoc').val();
        const ddlTaxClassiFication = $('#ddlTaxClassiFication').val();
        const ddlGenItemCatgGrp = $('#ddlGenItemCatgGrp').val();
        const txtMatGrpPack = $('#txtMatGrpPack').val();
        const txtPackagingMatType = $('#txtPackagingMatType').val();
        const materialDesription = $('#txtMaterialDescription').val();
        const materialSpecification = $('#txtMaterialSpecification').val();
        const UOM = $('#ddlUOM').val();
        const txthsncode = $('#txthsncode').val();
        const ddlindicator = $('#ddlindicator').val();
        const mpn_profile = $('#ddlMPNProfile').val();
        const plant_sp_material_status = $('#ddlPlantSpMaterialStatus').val();
        const materialType = $('#ddlMaterialType').val();

        let isSale = "N";
        let btnText = $('#btnAddMore').text();
        let hdMMHeaderID = readInt('#hdMMHeaderID');
        let hdMMDetailID = readInt('#hdMMDetailID');
       
        if (btnText == "Update") {
            if (hdMMHeaderID == 0 || hdMMDetailID == 0) {
                ShowMessage("error", "You are not able to update!");
                return;
            }
        }
        else if (btnText == "Add More") {
            hdMMDetailID = 0;
        }

        if (plantCode == 0 || materialCode == 0) {
            ShowMessage("error", "Kindly check required fields.!");           
            return;
        }
       var chSales= $('#chSales').prop('checked');
        if (chSales) {
            isSale = "Y";
        }
       
        var data = {
            MMHEADERID: hdMMHeaderID,
            MMDETAILID: hdMMDetailID,
            MATERIALCODE: materialCode,
            PLANTCODE: plantCode,
            TRANSPORTATIONGROUP: ddlTrasnsportGrp,
            LOADINGGROUP: ddlLoadingGrp,
            MEASUREMENTUNIT: UOM,
            BASEUNITOFMEASURE: ddlBaseUnitOfMeasure,
            PROFITCENTER: txtProfitCentre,
            SALES_ORG: txtSalesOrg,
            DISTRI_CHN: ddlDisbChnn,
            ITEM_CATG_GRP: ddlItemCategGroup,
            AVAIL_CHK: ddlCheckAvailCheck,
            STORAGE_LOC: ddlStorageLoc,
            GEN_ITEM_CAT_GRP: ddlGenItemCatgGrp,
            TAX_CLASSIFICATION: ddlTaxClassiFication,
            MAT_GRP_PACK_MATLS: txtMatGrpPack,
            PACKAGING_MAT_TYPE: txtPackagingMatType,
            IsSales: isSale,
            MATERIALDISCRIPTION: materialDesription,
            MATERIALSPECIFICATION: materialSpecification,
            MPN_PROFILE: mpn_profile,
            PLANT_SP_MAT_STATUS: plant_sp_material_status,
            HSNCODE: txthsncode,
            MMINDICATER: ddlindicator,
            MATERIALTYPE: materialType         

        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/AddUpdateMaterialRequest",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == "1") {
                    const $tbody = $('#gvMMUpdateDraftDetailBody');
                    $tbody.empty();
                    $.each(data.dataList, function (i, item) {
                        const rowHtml = `
                 <tr class="grdrow" style="background-color:White; color:#5D838C;">
                 <td style="vertical-align:top; text-align:left;">${item.PLANTNAMEDESC ?? ''}</td>
                 <td style="vertical-align:top; text-align:left; width:5%;">${item.IsSales ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.MATERIALCODE ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.MATERIALDISCRIPTION ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.MATERIALSPECIFICATION ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.MPN_PROFILE ?? ''}</td>
                 <td style="vertical-align:top; text-align:left; width:5%;">${item.HSNCODE ?? ''}</td>
                 <td style="vertical-align:top; text-align:left; width:5%;">${item.INDICATOR ?? ''}</td>
                 <td style="vertical-align:top; text-align:left; width:5%;">${item.PROFITCENTER ?? ''}</td>
                 <td style="vertical-align:top; text-align:left; width:5%;">${item.UOM ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.PLANT_SP_MAT_STATUS ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.DISTRI_CHN ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.ITEM_CATG_GRP ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.TRANSPORTATIONGROUP ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.AVAIL_CHK ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.TAX_CLASSIFICATION ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.LOADINGGROUP ?? ''}</td>
                <td><img src="../images/icon_edit.gif" data-ctr-edit="EditItem" data-id="${item.MMDETAILID ?? ''}" alt="Edit" style="cursor:pointer" /></td>
                <td><img src="../Images/icon_delete.gif" data-ctr-delete="DeleteItem" data-id="${item.MMDETAILID ?? ''}" alt="Delete" style="cursor:pointer" /></td>
                 </tr>`;
                        $tbody.append(rowHtml);
                        $('hdMMHeaderID').val(item.MMHEADERID);
                    });

                    //$tbody.on('click', '[data-ctr-edit="EditItem"]', function () {
                    //    const id = $(this).attr('data-id');
                    //    console.log('Edit:', id);
                    //    bindMMDetails($(this));
                    //});

                    //$tbody.on('click', '[data-ctr-edit="EditItem"]', function () {
                    //    const id = $(this).attr('data-id');
                    //    console.log('Delete:', id);
                    //    DeleteMaterialData($(this));
                    //});
                    resetData();
                    //sweetAlert("Material Master", data.Message, "success");
                    ShowMessage("success", data.Message);
                }
                else if (data.Rs == "0") {
                    ShowMessage("error", data.Message);
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
////////////////////////////////////////////////////////////////////////////////
///////////////////////Edit Material Item Request//////////////////////////////////////////////////////////
function safeText(v) { return (v ?? '').toString().trim(); }
function toInt(v) { const n = parseInt(v, 10); return isNaN(n) ? 0 : n; }
function isNonEmpty(v) { return !!safeText(v); }
function bindMMDetails(Id) {
    try {
        let MMDetailId = $(Id).attr('data-id');
        if (MMDetailId == undefined || MMDetailId == "") {
            ShowMessage("error", "Error in proccess!");
            return;
        }
        var data = {
            Code: MMDetailId
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/GetMaterialUpdateItemRequest",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.RS == "1") {
                    
                    $('#hdMMDetailID').val(MMDetailId);
                    // Basic field population
                    $('#txtMaterialCode').val(data.MATERIALCODE || '');
                    $('#ddlPlantMst').val(data.PLANTCODE);
                    $('#ddlUOM').val(data.MEASUREMENTUNIT);
                    $('#txtMaterialDescription').val(data.MATERIALDISCRIPTION || '');
                    $('#txtMaterialSpecification').val(data.MATERIALSPECIFICATION || '');

                    // Show MaterialType controls and set MaterialType (value or text)
                    $('#lblMaterialType, #ddlMaterialType').show();
                    $('#ddlMaterialType').val(data.MATERIALTYPE);

                    // hsn code & other dropdowns
                    $('#txthsncode').val(data.HSNCODE || '');
                    $('#ddlPlantSpMaterialStatus').val(data.PLANT_SP_MAT_STATUS);
                    $('#ddlMPNProfile').val(data.MPN_PROFILE);

                    // indicator if present
                    if (data.MMINDICATER != null && String(data.MMINDICATER).trim() !== '') {
                        $('#ddlindicator').val(data.MMINDICATER);
                    }

                    // Mark existence (IsExist.Visible = true)
                    $('#IsExist').show();

                    // Sales View Binding
                    if (data.SALES_ORG != null && String(data.SALES_ORG).trim() != '') {
                        $('#chSales').prop('checked', true)
                            .prop('disabled', false);
                        SetSalesView();
                        if (data.ITEM_CAT_GRP != null || data.ITEM_CATG_GRP != '') {
                            $('#ddlItemCategGroup').val(data.ITEM_CAT_GRP);
                        }
                        if (data.GEN_ITEM_CAT_GRP != null) {
                            $('#ddlGenItemCatgGrp').val(data.GEN_ITEM_CAT_GRP);
                        }
                        if (data.AVAIL_CHK != null) {
                            $('#ddlCheckAvailCheck').val(data.AVAIL_CHK);
                        }
                        if (data.TAX_CLASSIFICATION != null) {
                            $('#ddlTaxClassiFication').val(data.TAX_CLASSIFICATION);
                        }
                        if (data.TRANSPORTATIONGROUP != null) {
                            $('#ddlTrasnsportGrp').val(data.TRANSPORTATIONGROUP);
                        }
                        if (data.LOADINGGROUP != null) {
                            $('#ddlLoadingGrp').val(data.LOADINGGROUP);
                        }
                        if (data.DISTRI_CHN != null) {
                            $('#ddlDistriChan').val(data.DISTRI_CHN);
                        }

                        // Profit centre
                        if (data.PROFITCENTER != null) {
                            $('#txtProfitCentre').val(data.PROFITCENTER);
                        }

                        // Packaging row (Tr106)
                        const hasPackaging = (data.PACKAGING_MAT_TYPE != null && String(data.PACKAGING_MAT_TYPE).trim() !== '') ||
                            (data.MAT_GRP_PACK_MATLS != null && String(data.MAT_GRP_PACK_MATLS).trim() !== '');
                        if (hasPackaging) {
                            // Show <tr id="Tr106">
                            $('#Tr106').show(); // If needed: .css('display','table-row')
                            if (data.PACKAGING_MAT_TYPE) $('#txtPackagingMatType').val(data.PACKAGING_MAT_TYPE);
                            if (data.MAT_GRP_PACK_MATLS) $('#txtMatGrpPack').val(data.MAT_GRP_PACK_MATLS);
                        } else {
                            $('#Tr106').hide();
                            $('#txtMatGrpPack').val('');
                            $('#txtPackagingMatType').val('');
                        }
                    }
                    else {
                        $('#Tr106').hide();
                        $('#txtMatGrpPack').val('');
                        $('#txtPackagingMatType').val('');
                        $('#chSales').prop('checked', false)
                            .prop('disabled', true);
                        SetSalesView();
                    }

                    $('#btnAddMore').text('Update');
                }
                else if (data.RS == 0) {
                    $('#hdMMDetailID').val("");
                    $('#Tr106').hide();
                    $('#txtMatGrpPack').val('');
                    $('#txtPackagingMatType').val('');
                    $('#IsExist').hide();
                    $('#chSales').prop('checked', false)
                        .prop('disabled', true);
                    SetSalesView(); 
                    $('#IsExist').hide();
                    ShowMessage("error", data.MESSAGE);
                }
            },
            error: function () {
                $("#ajaxLoader").removeClass('loader');
                // $btn.prop('disabled', false).text('Verify Bank');
                sweetAlert("Oops...", "Something went wrong!", "error");
            },
            complete: function () {
                // $(id).prop('disabled', false).text('Verify Bank');
                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (err) {
        $("#ajaxLoader").removeClass('loader');
        console.error(err);
        alert('Failed to bind material details. Please try again.');
    }
}
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////Deleted Material Item Request////////////////////////////////////////////////////////////////////////////////
function DeleteMaterialData(Id) {
    try {
        let MMDetailId = $(Id).attr('data-id');
        if (MMDetailId == undefined || MMDetailId == "") {
            ShowMessage("error", "Error in proccess!");
            return;
        }
        var data = {
            Code: MMDetailId
        }
        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/DeleteUpdateMaterialItem",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.RS == "1") {
                    const $tbody = $('#gvList');
                    $tbody.empty();
                    $.each(data.dataList, function (i, item) {
                        const rowHtml = `
            <tr>
                
                 <td style="vertical-align:top; text-align:left;">${item.PLANTNAME ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.MATERIALCODE ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.SALES_ORG ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.MATERIALDISCRIPTION ?? ''}@row.MATERIALDISCRIPTION</td>
                 <td style="vertical-align:top; text-align:left;"${item.DISTRI_CHN ?? ''}></td>
                 <td style="vertical-align:top; text-align:left;">${item.ITEM_CATG_GRP ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.GEN_ITEM_CAT_GRP ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.LOADINGGROUP ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.TAX_CLASSIFICATION ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.BASEUNITOFMEASURE ?? ''}</td>
                 <td style="vertical-align:top; text-align:left;">${item.AVAIL_CHK ?? ''}</td>

                <td><img src="../images/icon_edit.gif" data-ctr-edit="EditItem" data-id="${item.MMDETAILID ?? ''}" alt="Edit" style="cursor:pointer" /></td>
                <td><img src="../Images/icon_delete.gif" data-ctr-delete="DeleteItem" data-id="${item.MMDETAILID ?? ''}" alt="Delete" style="cursor:pointer" /></td>
                 </tr>`;
                        $tbody.append(rowHtml);
                        $('hdMMHeaderID').val(item.MMHEADERID);
                    });

                    //$tbody.on('click', '[data-ctr-edit="EditItem"]', function () {
                    //    const id = $(this).attr('data-id');
                    //    console.log('Edit:', id);
                    //    bindMMDetails($(this));
                    //});

                    //$tbody.on('click', '[data-ctr-edit="DeleteItem"]', function () {
                    //    const id = $(this).attr('data-id');
                    //    console.log('Edit:', id);
                    //    DeleteMaterialData($(this));
                    //});
                    resetData();
                    sweetAlert("Material Master", data.MESSAGE, "success");
                    ShowMessage("success", data.MESSAGE);
                }
                else if (data.RS == "0") {
                    /* $("#ddlValuationClass").html(dt);*/
                    //sweetAlert("Material Master", data.Message, "warning");
                    ShowMessage("error", data.MESSAGE);
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
//////////////////////////////////////////////////////////////////////////////
/////////////Submit Request//////////////////////////////////////////////////
function SubmitMaterialRequest() {
    try {
        let MMHeaderId = $('#hdMMHeaderID').val();
        if (MMHeaderId == undefined || MMHeaderId == "") {
            ShowMessage("error", "Error in proccess!");
            return;
        }
        var chkTerm = $('#chkTerms').prop('checked');
        if (!chkTerm) {
            ShowMessage("error", "Please check terms & condition.");
            return;
        }
        var _ApprovalAuth = $('#ddlApprovalAuthority').val();
        if (_ApprovalAuth == "0" || _ApprovalAuth == "") {
            ShowMessage("error", "Please check Approval authority!");
            return;
        }
        var data = {
            Id: MMHeaderId,
            Code: _ApprovalAuth
        }
        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/SubmitUpdateMaterialRequest",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == "1") {
                    $('#btnSubmit').prop('disabled', true);
                    $('#btnAddMore').prop('disabled', true);
                    ShowMessage("success", data.MESSAGE);

                }
                else if (data.Rs == "0") {
                    ShowMessage("error", data.MESSAGE);
                }

            },
            error: function () {

                sweetAlert("Oops...", "Something went wrong!", "error");
            },
            complete: function () {

                $("#ajaxLoader").removeClass('loader');
            }
        });

    } catch (ex) {
        console.log(ex);
        sweetAlert("Material Master", "Oops... Something went wrong!.", "warning");
    }
}