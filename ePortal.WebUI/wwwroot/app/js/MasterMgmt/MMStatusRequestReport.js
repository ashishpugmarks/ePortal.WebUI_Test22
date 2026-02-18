const $dayFrom = $('#ddlDayFrom');
const $monthFrom = $('#ddlMonthFrom');
const $yearFrom = $('#ddlYearFrom');
const $dayTo = $('#ddlDayTo');
const $monthTo = $('#ddlMonthTo');
const $yearTo = $('#ddlYearTo');
const $infoPanel = $('#infoPanel');
const $errorPanel = $('#errorPanel');
const $errorMsg = $('#errorMsg');

$(document).ready(function () {
    $('#txtMetrialCode').on('keypress', function (event) {
        validateNumericInput(event);
    });
    $monthFrom.on('change', function () { cclick(); });
    $yearFrom.on('change', function () { cclick(); });
    $dayFrom.on('change', function () { cclick(); });    
    $monthTo.on('change', function () { cclickDT(); } );
    $yearTo.on('change', function () { cclickDT(); });
    $dayTo.on('change', function () { cclickDT(); });  
    $('#btnSubmit').on('click', function (e) {
        if (!validatePage()) {
            e.preventDefault(); // stop submit if invalid            
        }
        SearchMaterialRequest();
    });
   
    $('#txtPrice').on('keypress', function () { validateNumericInput(); } );
   
    
    $('#btnExport').on('click', function (e) {
        ExportMaterialRequest();
    });
});
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
function isSelected($ddl) {
    // Treat value "0" (or empty) as not selected
    const v = ($ddl.val() || '').trim();
    return v !== '' && v !== '0';
}

function isAllEmpty($day, $month, $year) {
    return !isSelected($day) && !isSelected($month) && !isSelected($year);
}

function isAllFilled($day, $month, $year) {
    return isSelected($day) && isSelected($month) && isSelected($year);
}

function parseIntSafe($ddl) {
    const n = parseInt(($ddl.val() || '0'), 10);
    return isNaN(n) ? 0 : n;
}

function buildDateFromDdls($day, $month, $year) {
    const d = parseIntSafe($day);
    const m = parseIntSafe($month); // 1-12
    const y = parseIntSafe($year);
    // Construct via (year, monthIndex, day) to avoid locale parsing issues
    return new Date(y, m - 1, d);
}

function isLeapYear(y) {
    return (y % 4 === 0 && y % 100 !== 0) || (y % 400 === 0);
}

/* ====== Error display (uery version of ShowError) ====== */
function ShowError($ctrl, msg) {
    $infoPanel.hide();
    $errorPanel.css('display', 'inline');
    $errorMsg.html(msg);
    if ($ctrl && $ctrl.length) {
        $ctrl.focus();
    }
    return false;
}
function validatePage() {
   
    if (!(isAllEmpty($dayFrom, $monthFrom, $yearFrom) || isAllFilled($dayFrom, $monthFrom, $yearFrom))) {
        return ShowError($dayFrom, 'From date is not in correct format');
    }

  
    if (!(isAllEmpty($dayTo, $monthTo, $yearTo) || isAllFilled($dayTo, $monthTo, $yearTo))) {
        return ShowError($dayTo, 'To date is not in correct format');
    }

  
    if (isAllFilled($dayFrom, $monthFrom, $yearFrom) && isAllFilled($dayTo, $monthTo, $yearTo)) {
        const fromDate = buildDateFromDdls($dayFrom, $monthFrom, $yearFrom);
        const toDate = buildDateFromDdls($dayTo, $monthTo, $yearTo);
        if (fromDate > toDate) {
            return ShowError($dayTo, 'To date cannot be less than From date');
        }
    }

    return true;
}
function fixDayForMonth($day, $month, $year) {
    const m = parseIntSafe($month);
    const y = parseIntSafe($year);
    let d = parseIntSafe($day);

    if (m === 2) {
        // February
        if (d > 28) {
            if (d === 29 && isLeapYear(y)) {
                // 29 allowed only in leap year
            } else {
                d = 28; // cap to 28 for non-leap (or >29)
            }
        }
    } else if ([4, 6, 9, 11].includes(m)) {
        // Apr, Jun, Sep, Nov
        if (d > 30) d = 30;
    } else {
        // 31-day months: OK
    }

    // If we changed d, set dropdown to that value if exists
    const exists = $day.find(`option[value="${d}"]`).length > 0;
    if (exists) {
        $day.val(String(d));
    } else {
        // Fallback by index (28th => index 27, 30th => index 29)
        if (d === 28) $day.prop('selectedIndex', 27);
        if (d === 30) $day.prop('selectedIndex', 29);
    }
}

function cclick() {
    fixDayForMonth($dayFrom, $monthFrom, $yearFrom);
}
function cclickDT() {
    fixDayForMonth($dayTo, $monthTo, $yearTo);
}

/* ====== Popup (same signature) ====== */
function openPopup(strOpenUrl, width, height) {
    window.open(
        strOpenUrl,
        'mywindow',
        'TOOLBAR=no,MENUBAR=no,RESIZABLE=no,SCROLLBARS=yes,LOCATION=no,DIRECTORIES=no,STATUS=no' +
        `,width=${width},height=${height}`
    );
}
function validateNumericInput(evt) {
    const theEvent = evt || window.event;
    let key = theEvent.key;
   
    if (!key) {
        const code = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(code);
    }
    const regex = /^[0-9.]$/;
    if (!regex.test(key)) {
        if (theEvent.preventDefault) theEvent.preventDefault();
        theEvent.returnValue = false;
        return false;
    }
    return true;
}

/*==============Search Data======================*/
function SearchMaterialRequest() {
    try {
        const $dayFrom = $('#ddlDayFrom');
        const $monthFrom = $('#ddlMonthFrom');
        const $yearFrom = $('#ddlYearFrom');
        const $dayTo = $('#ddlDayTo');
        const $monthTo = $('#ddlMonthTo');
        const $yearTo = $('#ddlYearTo');

        var data = {
            MMId: $('#txtMMId').val(),
            PlantMst: $('#ddlPlantMst').val(),
            DayFrom: $dayFrom.val(),
            MonthFrom: $monthFrom.val(),
            YearFrom: $yearFrom.val(),
            DayTo: $dayTo.val(),
            MonthTo: $monthTo.val(),
            YearTo: $yearTo.val(),
            RequestType: $('#ddlRequestType').val(),
            MaterialType: $('#ddlMaterialGroup').val(),
            MaterialGroup: $('#ddlMaterialType').val(),
            ValuationClass: $('#ddlValuationClass').val(),
            RequestorId: $('#txtRequestorId').val(),
            MetrialCode: $('#txtMetrialCode').val(),
            Status: $('#ddlStatus').val()
        }
        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/SearchMMHeaderStatus",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data == "error") {
                    $('#btnExport').prop('disabled', true);
                    $('#tableApprovalGrids').html("");
                    ShowMessage("error", "Error in data binding")
                    sweetAlert("Customer Master", "Error in Databinding", "error");
                }
                else {
                    $('#btnExport').prop('disabled', false);
                    $('#tableApprovalGrids').html(data);
                }
               
            },
            error: function () {
                $('#btnExport').prop('disabled', true);
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
/*==============Export Excel Data======================*/
function ExportMaterialRequest() {
    try {
        const $dayFrom = $('#ddlDayFrom');
        const $monthFrom = $('#ddlMonthFrom');
        const $yearFrom = $('#ddlYearFrom');
        const $dayTo = $('#ddlDayTo');
        const $monthTo = $('#ddlMonthTo');
        const $yearTo = $('#ddlYearTo');

        var data = {
            MMId: $('#txtMMId').val(),
            PlantMst: $('#ddlPlantMst').val(),
            DayFrom: $dayFrom.val(),
            MonthFrom: $monthFrom.val(),
            YearFrom: $yearFrom.val(),
            DayTo: $dayTo.val(),
            MonthTo: $monthTo.val(),
            YearTo: $yearTo.val(),
            RequestType: $('#ddlRequestType').val(),
            MaterialType: $('#ddlMaterialGroup').val(),
            MaterialGroup: $('#ddlMaterialType').val(),
            ValuationClass: $('#ddlValuationClass').val(),
            RequestorId: $('#txtRequestorId').val(),
            MetrialCode: $('#txtMetrialCode').val(),
            Status: $('#ddlStatus').val()
        }
        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/MasterMgmt/GetMMRequestExcel",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                if (data == 1) {
                    window.location.href = "/MasterMgmt/DownloadReportExcel";
                }
                else {
                    sweetAlert("Oops...", "Something went wrong!", "warning");
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