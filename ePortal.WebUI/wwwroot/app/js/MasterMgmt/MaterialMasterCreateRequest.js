
$(document).ready(function () {
    $('#chSales').on('change', function () {
        SalesViewEnable($(this));
    });
    
    $('#ddlPlantMst').on('change', function () {
        ChangePlant($(this));
    });
    $('#ddlMaterialType').on('change', function () {
        ChangeMaterialType($(this));
    });
    $('#ddlMaterialGroup').on('change', function () {
        ChangeMaterialGroup($(this));
    });
    $('#ddlUOM').on('change', function () {
        ChangeUOM($(this));
    });

    $('#txtZMPNIntMaterialNo, #txtMaterialDescription').on('change', function () {
        txtZMPNIntMaterialNoChange();
    });
    $('#btnAddMore').on('click', function () {
        AddMaterialData($(this));
    });
    $('#draftTableBody2').on('click', '[data-ctr-edit="EditItem"]', function () {
        const id = $(this).attr('data-id');
        console.log('Edit:', id);
        bindMMDetails($(this));
    });
    $('#draftTableBody2').on('click', '[data-ctr-delete="DeleteItem"]', function () {
        const id = $(this).attr('data-id');
        console.log('Edit:', id);
        DeleteMaterialData($(this));
    });
    $('#btnSubmit').on('click', function () {
        SubmitMaterialRequest();
    });
    $('#btnCancel').on('click', function () {
        resetData()
    });
  

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

            // Plant
            if (!validateDDLRequired('#ddlPlantMst', 'Plant is mandatory')) return false;

            // Material Type
            if (!validateDDLRequired('#ddlMaterialType', 'Material type is mandatory')) return false;

            // Material Group
            if (!validateDDLRequired('#ddlMaterialGroup', 'Material group is mandatory')) return false;

            // Valuation Class
            if (!validateDDLRequired('#ddlValuationClass', 'Valuation class is mandatory')) return false;

            // UOM
            if (!validateDDLRequired('#ddlUOM', 'Unit of measurement is mandatory')) return false;

            // HSN Code
            if (!validateTextRequired('#txthsncode', 'HSN Code is mandatory')) return false;

            // Indicator
            if (!validateDDLRequired('#ddlindicator', 'Indicator is mandatory')) return false;

            // Material Description
            if (!validateTextRequired('#txtMaterialDescription', 'Material description is mandatory')) return false;

            // Description + Specification length validation
            if ($('#txtMaterialSpecification').val() !== '') {
                if (!countTextChar('#txtMaterialDescription', '#txtMaterialSpecification',
                    'Please enter only in Description')) {
                    return false;
                }
            }

            /* ================= SALES VIEW ================= */
            if ($('#chSales').is(':checked')) {

                if (!validateDDLRequired('#ddlItemCategGroup', 'Item Category is mandatory')) return false;
                if (!validateDDLRequired('#ddlGenItemCatgGrp', 'Gen Item Cat Grp is mandatory')) return false;
                if (!validateDDLRequired('#ddlCheckAvailCheck', 'Check Availability is mandatory')) return false;
                if (!validateDDLRequired('#ddlTaxClassiFication', 'Tax Classification is mandatory')) return false;
                if (!validateDDLRequired('#ddlTrasnsportGrp', 'Transport Group is mandatory')) return false;
                if (!validateDDLRequired('#ddlStorageLoc', 'Storage Location is mandatory')) return false;
                if (!validateDDLRequired('#ddlLoadingGrp', 'Loading Group is mandatory')) return false;
                if (!validateTextRequired('#txtProfitCentre', 'Profit Centre is mandatory')) return false;
                if (!validateDDLRequired('#ddlDisbChnn', 'Distribution Channel is mandatory')) return false;
                if (!validateDDLRequired('#ddlBaseUnitOfMeasure', 'Base unit of measure is mandatory')) return false;

                // Special Material Type
                if ($('#ddlMaterialType').val() === 'ZPAC') {
                    if (!validateTextRequired('#txtMatGrpPack', 'Mat. Grp Pack is mandatory')) return false;
                    if (!validateTextRequired('#txtPackagingMatType', 'Packaging Mat. Type is mandatory')) return false;
                }
            }

            return true;
        }

 /* ================= FINAL SUBMIT VALIDATION ================= */
function FnSubmitValidate () {
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

function showError(selector, msg) {
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
function SalesViewEnable(id) {

    var salesChecked = $('#chSales').prop('checked');

    if (salesChecked) {
       
        $('#Tr101, #Tr102, #Tr103, #Tr104, #Tr105, #Tr107').show();    
        //bindMasterData();      
        var matText = $('#ddlMaterialType option:selected').text() || '';

        // If the first 4 chars are "ZPAC"
        if (matText.substring(0, 4).toUpperCase() === 'ZPAC') {
            $('#Tr106').show();
            $('#txtMatGrpPack').val('BOX1');
            $('#txtPackagingMatType').val('0004');
        } else {
            $('#Tr106').hide();
            $('#txtMatGrpPack').val('');
            $('#txtPackagingMatType').val('');
        }
    } else {
       
        $('#Tr101, #Tr102, #Tr103, #Tr104, #Tr105, #Tr106, #Tr107').hide();
    }

    var text = $("#ddlMaterialType option:selected").text() || "";

    if (text.startsWith("ZMPN")) {
        
        $("#p001").hide();
        $("#p002").hide();
        $("#p003").show();
        $("#p004").hide();
        $("#p000").hide();        
        $("#ddlValuationClass").hide();        
        $("#ddlPlantMst").prop("disabled", true).prop("selectedIndex", 0);
    } else {
        // Show/Hide rows
        $("#p001").show();
        $("#p002").show();
        $("#p003").hide();
        $("#p004").show();
        $("#p000").show();       
        $("#ddlValuationClass").show();       
        $("#ddlPlantMst").prop("disabled", false);
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
       

       // $(id).prop('disabled', true).text('Verify Bank...');
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
                    else{
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
function ChangeUOM(id) {
    try {
        $('#ddlBaseUnitOfMeasure').val($(id).val());    

    } catch (ex) {
        // $(id).prop('disabled', false).text('Verify Bank');
        console.log(ex);
        sweetAlert("Material Master", "Oops... Something went wrong!.", "warning");
    }
}
function txtZMPNIntMaterialNoChange() {
   
    var matTypeText = ($('#ddlMaterialType option:selected').text() || '').trim();    
    var isZmpn = matTypeText.substring(0, 4).toUpperCase() === 'ZMPN';
    if (isZmpn) {
        var internalNo = ($('#txtZMPNIntMaterialNo').val() || '').trim();
        var materialDesc = ($('#txtMaterialDescription').val() || '').trim();        
        var combined = internalNo && materialDesc
            ? internalNo + '-' + materialDesc
            : internalNo || materialDesc; // 
        $('#lblZMPNDesc').text(combined);
    } else {
        $('#lblZMPNDesc').text('');
    }
}
////////////////////////////////////////////////////////////////////////
///////////////Add Material Item
function resetData() {
    // Reset dropdowns (SelectedIndex = 0)
    $('#ddlPlantMst').prop('selectedIndex', 0);
    $('#ddlMaterialType').prop('selectedIndex', 0);
    $('#ddlMaterialGroup').prop('selectedIndex', 0);
    $('#ddlValuationClass').prop('selectedIndex', 0);
    $('#ddlUOM').prop('selectedIndex', 0);
    $('#ddlindicator').prop('selectedIndex', 0);
    // Reset textboxes (string.Empty)
    $('#txtMaterialDescription').val('');
    $('#txtMaterialSpecification').val('');
    $('#txtPrice').val('');
    $('#txthsncode').val('');
    $('#txtZMPNIntMaterialNo').val('');   
    $('#hdEditMMDetailId').val(''); // ensure you have <input type="hidden" id="hdEditMMDetailId" />
    // Reset checkboxes
    $('#chkTerms').prop('checked', false);
    // Reset for Sales
    $('#chSales').prop('disabled', false).prop('checked', false); 
    SalesViewEnable($('#chSales'));    
    $('#p001').show();
    $('#p002').show();
    $('#p003').hide();
    $('#p004').show();
    $('#p000').show();

    // ddlValuationClass.Visible = true (show) and ddlPlantMst.Enabled = true (enable)
    $('#ddlValuationClass').show();
    $('#ddlPlantMst').prop('disabled', false);
    $('#btnAddMore').val('Add More');
}
function validateSale() {
    var err = "";
    var isValid = true;

    // Only validate when Sales checkbox is checked
    if ($("#chSales").prop("checked")) {

        // Helper to append error text and mark invalid
        function addErr(msg) {
            if (err.length > 0 && !err.endsWith("/")) err += "/";
            err += msg + "/";
            isValid = false;
        }

        // Dropdowns: check selectedIndex <= 0
        if ($("#ddlItemCategGroup")[0]?.selectedIndex <= 0) addErr("Item Category");
        if ($("#ddlGenItemCatgGrp")[0]?.selectedIndex <= 0) addErr("Gen Item Cat Grp");
        if ($("#ddlCheckAvailCheck")[0]?.selectedIndex <= 0) addErr("Availability check");
        if ($("#ddlTaxClassiFication")[0]?.selectedIndex <= 0) addErr("Tax classification");
        if ($("#ddlTrasnsportGrp")[0]?.selectedIndex <= 0) addErr("Transportation Group");
        if ($("#ddlLoadingGrp")[0]?.selectedIndex <= 0) addErr("Loading Group");
        if ($("#ddlStorageLoc")[0]?.selectedIndex <= 0) addErr("Storage Location");
        if ($("#ddlDisbChnn")[0]?.selectedIndex <= 0) addErr("Distribution Channel");
        if ($("#ddlBaseUnitOfMeasure")[0]?.selectedIndex <= 0) addErr("Base unit of measure");

        // Text inputs: check trimmed length
        if ($.trim($("#txtProfitCentre").val()).length <= 0) addErr("Profit Centre");

        // MaterialType-specific checks
        var materialTypeVal = $("#ddlMaterialType").val();
        if (materialTypeVal === "6") {
            if ($.trim($("#txtMatGrpPack").val()).length <= 0) addErr("Mat. Grp Pack. Matls");
            if ($.trim($("#txtPackagingMatType").val()).length <= 0) addErr("Packaging mat. type");
        } else {
            // Clear when not type 6
            $("#txtMatGrpPack").val("");
            $("#txtPackagingMatType").val("");
        }
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
        var MaterialText = $('#ddlMaterialType option:selected').text();
        let rs = FnValidate();
        if (rs == false && (!MaterialText.startsWith("ZMPN"))) {
            return false;
        }  
        const plantCode = readInt('#ddlPlantMst');
        const materialType = readInt('#ddlMaterialType');        
        const materialGroup = readInt('#ddlMaterialGroup');
        const valuationClass = readInt('#ddlValuationClass');
        const materialDesription = readTextUpperTrim('#txtMaterialDescription');
        const materialSpecification = readTextUpperTrim('#txtMaterialSpecification');       
        const price = readDecimal('#txtPrice', 1);
        const UOM = readInt('#ddlUOM');
        const hsn = readTextTrim('#txthsncode');
        const indicatorVal = readTextTrim('#ddlindicator');
        const txtZMPNIntMaterialNo = readTextTrim('#txtZMPNIntMaterialNo');
        const ddlTrasnsportGrp = $('#ddlTrasnsportGrp').val();
        const ddlLoadingGrp = $('#ddlLoadingGrp').val();
        const ddlBaseUnitOfMeasure = $('#ddlBaseUnitOfMeasure').val();
        const txtSalesOrg = $('#txtSalesOrg').val();
        const ddlDisbChnn = $('#ddlDisbChnn').val();
        const ddlItemCategGroup = $('#ddlItemCategGroup').val();
        const ddlCheckAvailCheck = $('#ddlCheckAvailCheck').val();
        const txtProfitCentre = $('#txtProfitCentre').val();
        const ddlStorageLoc = $('#ddlStorageLoc').val();
        const ddlTaxClassiFication = $('#ddlTaxClassiFication').val();
        const ddlGenItemCatgGrp = $('#ddlGenItemCatgGrp').val();
        const txtMatGrpPack = $('#txtMatGrpPack').val();
        const txtPackagingMatType = $('#txtPackagingMatType').val(); 
        let isSale = "N";
        let btnText = $('#btnAddMore').text();
        let hdMMHeaderID = readInt('#hdMMHeaderID');
        let hdMMDetailID = readInt('#hdMMDetailID');
        if (btnText == "Update") {
            if (hdMMDetailID == 0) {
                ShowMessage("error", "You are not able to update!");                
                return;
            }
        }
        else if (btnText == "Add More") {
            hdMMDetailID = 0;
        }
             
        if ((plantCode == 0 || materialType == 0 || materialGroup == 0 || valuationClass == 0 || materialDesription == "" || price == "0" || UOM == 0 || indicatorVal == "") && (!MaterialText.startsWith("ZMPN"))) {
            ShowMessage("error", "Kindly check required fields.!");
            //sweetAlert("Oops...", "Kindly check required fields.!", "warning");
            return;
        }
        else if (MaterialText.startsWith("ZMPN") && (materialType == 0 || materialGroup == 0 || materialDesription == "" || txtZMPNIntMaterialNo == "")) {
            ShowMessage("error", "Kindly check required fields.!");
            //sweetAlert("Oops...", "Kindly check required fields.!","warning");
            return;
        }
        if ($("#chSales").prop("checked")) {
            isSale = "Y";
            var res = validateSale();
            if (!res.isValid) {
                ShowMessage("error", res.err);
                sweetAlert("Oops...", "Please select/enter:-" + res.err, "warning");
                return;
            }
        }
        //$(id).prop('disabled', true).text('Adding...');
        var data = {
            MMHEADERID: hdMMHeaderID,
            MMDETAILID: hdMMDetailID,
            MATERIALTYPE: materialType,
            MATERIALGROUP: materialGroup,
            VALUATIONCLASS: valuationClass,
            MATERIALDISCRIPTION: materialDesription,
            MATERIALSPECIFICATION: materialSpecification,
            MEASUREMENTUNIT: UOM,
            PLANTCODE: plantCode,
            TRANSPORTATIONGROUP: ddlTrasnsportGrp,            
            LOADINGGROUP: ddlLoadingGrp,
            BASEUNITOFMEASURE: ddlBaseUnitOfMeasure,
            MMINDICATER: indicatorVal,
            PROFITCENTER: txtProfitCentre,
            PRICE: price,
            HSNCODE: hsn,
            SALES_ORG: txtSalesOrg,
            DISTRI_CHN: ddlDisbChnn,
            ITEM_CATG_GRP: ddlItemCategGroup,
            AVAIL_CHK: ddlCheckAvailCheck,
            STORAGE_LOC: ddlStorageLoc,
            GEN_ITEM_CAT_GRP: ddlGenItemCatgGrp,
            TAX_CLASSIFICATION: ddlTaxClassiFication,
            MAT_GRP_PACK_MATLS: txtMatGrpPack,
            PACKAGING_MAT_TYPE: txtPackagingMatType,
            INT_MATERIAL_NO: txtZMPNIntMaterialNo,
            MATERIALTYPETEXT: MaterialText,
            IsSales: isSale
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/AddMoreMaterial",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.Rs == "1") {
                    const $tbody = $('#draftTableBody2');
                    $tbody.empty();
                    $.each(data.dataList, function (i, item) {
                        const rowHtml = `
            <tr>
                <td>${item.PLANTNAME ?? ''}</td>
                <td>${item.IsSales ??''}</td>
                <td>${item.MATERIALDISCRIPTION ?? ''}</td>
                <td>${item.MATERIALSPECIFICATION ?? ''}</td>
                <td>${item.MATERIALTYPE ?? ''}</td>
                <td>${item.MATERIALGROUP ?? ''}</td>
                <td>${item.VALUATIONCLASS ?? ''}</td>
                <td>${item.UOM ?? ''}</td>
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
                    //    console.log('Delete:', id);
                    //    DeleteMaterialData($(this));
                    //});
                    resetData();
                    sweetAlert("Material Master", data.Message, "success");
                }
                else if (data.Rs == "0") {                   
                   /* $("#ddlValuationClass").html(dt);*/
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
            url: "/MasterMgmt/GetMaterialItemEdit",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.RS == "1") {
                    $('#hdMMDetailID').val(MMDetailId);
                    $('#ddlPlantMst').val(data.PLANTCODE);
                    $('#ddlMaterialType').val(data.MATERIALTYPE);
                    var dt = '<option value="0" selected>--Select--</option>';

                    $("#ddlMaterialGroup").html("");
                    $.each(data.MaterialGroupList, function (i, item) {
                        dt += '<option value="' + item.Id + '">' + item.Description + '</option>';
                    });
                    $('#ddlMaterialGroup').html(dt);
                    $('#ddlMaterialGroup').val(data.MATERIALGROUP);
                    $('#txtMaterialDescription').val(safeText(data.MATERIALDISCRIPTION));
                    if (data.MATERIALTYPE != "0" && data.MATERIALGROUP != "0") {
                        var dt = '<option value="0" selected>--Select--</option>';
                        $('#ddlValuationClass').html("");
                        $.each(data.ValuationClassList, function (i, item) {
                            dt += '<option value="' + item.Id + '">' + item.Description + '</option>';
                        });
                        $('#ddlValuationClass').html(dt);
                        $('#ddlValuationClass').val(data.VALUATIONCLASS);
                    }

                    const mtText = $('#ddlMaterialType option:selected').text();
                    const isZmpn = mtText.startsWith('ZMPN');
                    if (isZmpn) {
                        // Panels visibility & control states
                        $('#p001').hide();
                        $('#p002').hide();
                        $('#p003').show();
                        $('#p004').hide();
                        $('#p000').hide();
                        $('#ddlValuationClass').hide();
                        $('#ddlPlantMst').prop('disabled', true);
                        // ZMPN fields
                        const intMat = safeText(data.INT_MATERIAL_NO);
                        $('#txtZMPNIntMaterialNo').val(intMat);
                        $('#lblZMPNDesc').text(`${intMat}-${safeText(row.MATERIALDISCRIPTION)}`);
                    } else {
                        $('#p001').show();
                        $('#p002').show();
                        $('#p003').hide();
                        $('#p004').show();
                        $('#p000').show();
                        $('#ddlValuationClass').show();
                        $('#ddlPlantMst').prop('disabled', false);
                        $('#ddlUOM').val(data.MEASUREMENTUNIT);
                        $('#txtMaterialSpecification').val(safeText(data.MATERIALSPECIFICATION));
                        $('#txtPrice').val(safeText(data.PRICE));
                        $('#txthsncode').val(safeText(data.HSNCODE));
                        if (isNonEmpty(data.MMINDICATER)) {
                            $('#ddlindicator').val(data.MMINDICATER);
                        }
                        if (isNonEmpty(data.SALES_ORG)) {
                            $('#chSales').prop('checked', true).prop('disabled', true);
                            SalesViewEnable($('#chSales'));

                            if (isNonEmpty(data.ITEM_CATG_GRP)) $('#ddlItemCategGroup').val(data.ITEM_CATG_GRP);
                            if (isNonEmpty(data.GEN_ITEM_CAT_GRP)) $('#ddlGenItemCatgGrp').val(data.GEN_ITEM_CAT_GRP);
                            if (isNonEmpty(data.AVAIL_CHK)) $('#ddlCheckAvailCheck').val(data.AVAIL_CHK);
                            if (isNonEmpty(data.TAX_CLASSIFICATION)) $('#ddlTaxClassiFication').val(data.TAX_CLASSIFICATION);
                            if (isNonEmpty(data.TRANSPORTATIONGROUP)) $('#ddlTrasnsportGrp').val(data.TRANSPORTATIONGROUP);
                            if (isNonEmpty(data.LOADINGGROUP)) $('#ddlLoadingGrp').val(data.LOADINGGROUP);
                            if (isNonEmpty(data.DISTRI_CHN)) $('#ddlDisbChnn').val(data.DISTRI_CHN);
                            if (isNonEmpty(data.BASEUNITOFMEASURE)) $('#ddlBaseUnitOfMeasure').val(data.BASEUNITOFMEASURE);
                            if (isNonEmpty(data.PROFITCENTER)) $('#txtProfitCentre').val(safeText(data.PROFITCENTER));

                            // MATERIALTYPE == "6" branch
                            if (String(data.MATERIALTYPE) === '6') {
                                $('#Tr106').show();
                                if (isNonEmpty(data.PACKAGING_MAT_TYPE)) $('#txtPackagingMatType').val(safeText(data.PACKAGING_MAT_TYPE));
                                if (isNonEmpty(data.MAT_GRP_PACK_MATLS)) $('#txtMatGrpPack').val(safeText(data.MAT_GRP_PACK_MATLS));
                            } else {
                                $('#Tr106').hide();
                                $('#txtMatGrpPack').val('');
                                $('#txtPackagingMatType').val('');
                            }


                            if (isNonEmpty(data.STORAGE_LOC)) {
                                var dt = '<option value="0" selected>--Select--</option>';
                                $('#ddlStorageLoc').html("");
                                $.each(data.StorageLocList, function (i, item) {
                                    dt += '<option value="' + item.Code + '">' + item.Description + '</option>';
                                });
                                $('#ddlStorageLoc').html(dt);
                                $('#ddlStorageLoc').val(data.STORAGE_LOC);
                            }
                        } else {
                            // No sales org: reset sales view
                            $('#Tr106').hide();
                            $('#txtMatGrpPack').val('');
                            $('#txtPackagingMatType').val('');
                            $('#chSales').prop('checked', false).prop('disabled', true);
                            SalesViewEnable($('#chSales'));
                        }
                    }
                    $('#btnAddMore').text('Update');
                }
                else if (data.RS == "0") {                 
                   
                    sweetAlert("Material Master", data.MESSAGE, "warning");
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

    } catch (err) {
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
            url: "/MasterMgmt/DeleteMaterialItem",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.RS == "1") {
                    const $tbody = $('#draftTableBody2');
                    $tbody.empty();
                    $.each(data.dataList, function (i, item) {
                        const rowHtml = `
            <tr>
                <td>${item.PLANTNAME ?? ''}</td>
                <td>${item.IsSales ?? ''}</td>
                <td>${item.MATERIALDISCRIPTION ?? ''}</td>
                <td>${item.MATERIALSPECIFICATION ?? ''}</td>
                <td>${item.MATERIALTYPE ?? ''}</td>
                <td>${item.MATERIALGROUP ?? ''}</td>
                <td>${item.VALUATIONCLASS ?? ''}</td>
                <td>${item.UOM ?? ''}</td>
                <td><img src="../images/icon_edit.gif" data-ctr-edit="EditItem" data-id="${item.MMDETAILID ?? ''}" alt="Edit" style="cursor:pointer" /></td>
                <td><img src="../Images/icon_delete.gif" data-ctr-delete="DeleteItem" data-id="${item.MMDETAILID ?? ''}" alt="Delete" style="cursor:pointer" /></td>
                 </tr>`;
                        $tbody.append(rowHtml);
                        $('hdMMHeaderID').val(item.MMHEADERID);
                    });

                    $tbody.on('click', '[data-ctr-edit="EditItem"]', function () {
                        const id = $(this).attr('data-id');
                        console.log('Edit:', id);
                        bindMMDetails($(this));
                    });

                    $tbody.on('click', '[data-ctr-edit="DeleteItem"]', function () {
                        const id = $(this).attr('data-id');
                        console.log('Edit:', id);
                        DeleteMaterialData($(this));
                    });
                    resetData();
                    sweetAlert("Material Master", data.MESSAGE, "success");
                }
                else if (data.RS == "0") {
                    /* $("#ddlValuationClass").html(dt);*/
                    sweetAlert("Material Master", data.MESSAGE, "warning");
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
            url: "/MasterMgmt/SubmitMaterialRequest",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data.RS == "1") {
                    $('#btnSubmit').prop('disabled', true);
                    $('#btnAddMore').prop('disabled', true);
                    ShowMessage("success", data.MESSAGE);
                    
                }
                else if (data.RS == "0") {  
                    ShowMessage("error", "You are not able to update!"); 
                                
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