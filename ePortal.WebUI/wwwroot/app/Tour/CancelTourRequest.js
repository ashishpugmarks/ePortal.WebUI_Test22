/* ================================
   Global variables
================================ */
$('.homeContent').removeClass('homeContent');

var ViewState_EmpDesig = "";
var ViewState_AppDate = "";
var oTourList = [];   // injected from Razor

/* ================================
   Document Ready
================================ */
$(document).ready(async function () {

    const hdnUser = document.getElementById("hdnUserId");
    const userId = hdnUser ? parseInt(hdnUser.value) : 0;

    const params = new URLSearchParams(window.location.search);
    window.reqid = params.get('id');

    // values injected from Razor
    ViewState_EmpDesig = typeof empDesig !== "undefined" ? empDesig : "";
    ViewState_AppDate = typeof appDate !== "undefined" ? appDate : "";

    //await FillRequestHeader();
    //await FillAdvanceDetails();
    //await FillRequestDetails();

    $('[data-ctrl="edit"]').on("click", function (e) {
        e.preventDefault();
        var id = $(this).data("index");
        showdetail(id);
    });


    $("#cmdSubmit").on("click", function (e) {
        e.preventDefault();
        if (fnValidateDetails()) {
            cmdSubmit_Click();
        }
    });

    $("#cmdCancel").on("click", function (e) {
        e.preventDefault();
        cmdCancel_Click();
    });

    $("#btnOK").on("click", function (e) {
        e.preventDefault();
        btnOK_Click();
    });

    $('input[name="rblCancelledTour"]').on('change', function () {
        const selectedValue = $('input[name="rblCancelledTour"]:checked').val();

        if (selectedValue === "1") {
            $('#pnlGridPart').hide();
            $('#btnOK').hide();
            $('#pnlLowerPart').show();
        } else {
            $('#pnlGridPart').show();
            $('#btnOK').show();
            $('#pnlLowerPart').hide();
        }
    });
});

/* ================================
   Popup cancel single request
================================ */
function CancelRequest(ID, detailID) {
    var refreshflag = openPopupwithScrol(
        "TourDLGS/RequestCancellation?id=" + ID + "&detailid=" + detailID,
        "Ticket Cancellation",
        "600",
        "380"
    );
    return refreshflag !== "0";
}

/* ================================
   Validation
================================ */
function fnValidateDetails() {
    var remarks = document.getElementById("txtCancellationRemarks");
    if (!remarks || remarks.value.trim() === "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML =
            "Remarks is required field for tour cancellation.";
        remarks.focus();
        return false;
    }
    return true;
}

/* ================================
   Popup helper
================================ */
function openPopupwithScrol(strOpenUrl, pagetype, width, height) {
    var winTop = (screen.height - height) / 2;
    var winLeft = (screen.width - width) / 2;

    var windowFeatures =
        "location=no,status=no,width=" + width +
        ",height=" + height +
        ",left=" + winLeft +
        ",top=" + winTop +
        ",resizable=yes,scrollbars=yes";

    window.open(strOpenUrl, pagetype, windowFeatures);
}

/* ================================
   AJAX – Header
================================ */
async function FillRequestHeader() {
    return $.ajax({
        url: '/TourRequest/GetRequestHeader',
        type: 'GET',
        data: { requestId: reqid }
    }).done(function (data) {

        $('#lblRequestID').text(reqid);
        $('#lblPeriod').text(data.period || "");
        $('#lblDays').text(data.days || "");
        $('#lblObjective').text(data.objective || "");
        $('#lblAdvRemarks').text(data.advRemarks || "");
        $('#lblMiscAmount').text(data.miscAmount || "0");
        $('#lblRequiredAmount').text(data.requiredAmount || "0");

        if (data.isAdvanceRequired) {
            $('#chkAdvance').prop('checked', true);
            $('#pnlAdvance').show();
        } else {
            $('#chkAdvance').prop('checked', false);
            $('#pnlAdvance').hide();
        }

    }).fail(function () {
        alert('Error loading request header details.');
    });
}
function toggleCancelPanels() {
    var selectedValue = $('input[name="rblCancelledTour"]:checked').val();

    if (selectedValue === "1") {
        // Complete Tour Cancellation
        $('#pnlGridPart').hide();
        $('#btnOK').hide();
        $('#pnlLowerPart').show();
    } else {
        // Single Day Cancellation
        $('#pnlGridPart').show();
        $('#btnOK').show();
        $('#pnlLowerPart').hide();
    }
}

// Initial load (important)
toggleCancelPanels();

// On radio change
$('input[name="rblCancelledTour"]').on('change', function () {
    toggleCancelPanels();
});
/* ================================
   AJAX – Advance details
================================ */
async function FillAdvanceDetails() {
    return $.ajax({
        url: '/TourRequest/GetAdvanceDetails',
        type: 'GET'
    }).done(function (response) {

        var rows = "";

        if (response.result && response.result.length > 0) {
            response.result.forEach(function (item) {
                rows += `<tr>
                            <td>${item.period}</td>
                            <td>${item.amount}</td>
                            <td>${item.day}</td>
                         </tr>`;
            });
        } else {
            rows = '<tr><td colspan="3">No advance details found</td></tr>';
        }

        $('#gvAdvance tbody').html(rows);

    }).fail(function () {
        alert('Error loading advance details.');
    });
}

/* ================================
   AJAX – Request details
================================ */
async function FillRequestDetails() {
    return $.ajax({
        url: '/TourRequest/GetRequestDetails',
        type: 'GET',
        data: { requestId: reqid }
    }).done(async function (response) {

        var rows = "";

        if (response.result && response.result.length > 0) {
            rows = response.result.map(item => `
                <tr>
                    <td>${item.fromDate}</td>
                    <td>${item.toDate}</td>
                    <td>${item.location}</td>
                </tr>
            `).join('');
        } else {
            rows = '<tr><td colspan="3">No request details found</td></tr>';
        }

        $('#gvList tbody').html(rows);
        await CalculateDayNights();

    }).fail(function () {
        alert('Error loading request details.');
    });
}

/* ================================
   Calculation logic
================================ */
async function CalculateDayNights() {

    let chargeAP = 0, chargeA = 0, chargeB = 0, chargeC = 0;
    let allowAP = 0, allowA = 0, allowB = 0, allowC = 0;

    let TotalDailyAllowance = 0;
    let TotalNightCharge = 0;

    if (!Array.isArray(oTourList) || oTourList.length === 0) {
        $('#pnlAdvance').hide();
        return;
    }

    let ProcessDates = [];
    let ProcessNights = [];

    for (const oTour of oTourList) {

        let days = 1;
        let nights = 1;

        let TravelDate = oTour.TourFromDate;
        let StayingCity = (oTour.StayingLoc || "").trim();
        let CityCode = oTour.StayingLocCode || oTour.ToLocCode;

        if (ProcessDates.includes(TravelDate)) days = 0;
        else ProcessDates.push(TravelDate);

        if (ProcessNights.includes(TravelDate) || StayingCity === "") nights = 0;
        else ProcessNights.push(TravelDate);

        const cityData = await GetCityCategory(CityCode);
        if (!cityData) continue;

        const allowanceData = await GetEmployeeAllowanceDetail(
            cityData.SYCITYCATEGORYID,
            ViewState_EmpDesig || 21,
            ViewState_AppDate,
            parseInt($('#hdnUserId').val()) > 70000000 ? "EXPAT" : "LOCAL"
        );

        if (!allowanceData) continue;

        TotalDailyAllowance += allowanceData.DAILYALLOWANCEAMT * days;
        TotalNightCharge += allowanceData.LODGINGAMT * nights;

        switch (cityData.DESCRIP) {
            case "A+": chargeAP += nights; allowAP += days; break;
            case "A": chargeA += nights; allowA += days; break;
            case "B": chargeB += nights; allowB += days; break;
            case "C": chargeC += nights; allowC += days; break;
        }
    }

    $('#lblStayChargeAPLUS').text(chargeAP);
    $('#lblStayChargeA').text(chargeA);
    $('#lblStayChargeB').text(chargeB);
    $('#lblStayChargeC').text(chargeC);

    $('#lblAllownceAPLUS').text(allowAP);
    $('#lblAllownceA').text(allowA);
    $('#lblAllownceB').text(allowB);
    $('#lblAllownceC').text(allowC);

    $('#lblTotalNights').text(chargeAP + chargeA + chargeB + chargeC);
    $('#lblTotalDays').text(allowAP + allowA + allowB + allowC);

    $('#lblTotalAllowances').text(TotalDailyAllowance.toFixed(2));
    $('#lblTotalCharges').text(TotalNightCharge.toFixed(2));

    let MiscAmount = parseFloat($('#txtMiscAmout').val()) || 0;
    $('#lblTotalAmount').text((TotalDailyAllowance + TotalNightCharge + MiscAmount).toFixed(2));
}

/* ================================
   AJAX helpers
================================ */
async function GetCityCategory(cityCode) {
    return $.ajax({
        url: '/TourRequest/GetCityCategory',
        type: 'GET',
        data: { CityCode: cityCode }
    }).then(r => r.result).catch(() => null);
}

async function GetEmployeeAllowanceDetail(cityCategoryCode, empDesignationID, currDate, userType) {
    return $.ajax({
        url: '/TourRequest/GetEmployeeAllowanceDetail',
        type: 'GET',
        data: {
            CityCategoryCode: cityCategoryCode,
            EmpDesignationID: empDesignationID,
            CurrDate: currDate,
            UserType: userType
        }
    }).then(r => r.result).catch(() => null);
}

/* ================================
   Submit / Cancel
================================ */
function cmdSubmit_Click() {

    var model = {
        txtCancellationRemarks: $('#txtCancellationRemarks').val(),
        lblObjective: $('#lblObjective').html().replace('&nbsp;', ''),
        lblPeriod: $('#lblPeriod').html().replace('&nbsp;', ''),
        lblDays: $('#lblDays').html().replace('&nbsp;', ''),
        lblRequestID: $('#lblRequestID').html().replace('&nbsp;', '')
    };

    $.ajax({
        url: "/TourRequest/CmdSubmit_Click_CR",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(model),
        success: function (res) {
            if (res.success) {
                    window.location.href ="/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx";
            } else {
                showError(res.message);
            }
        },
        error: function (err) {
            showError(err.responseText || "Error in cancelling");
        }
    });
    //const formData = new FormData();
    //formData.append('txtCancellationRemarks', $('#txtCancellationRemarks').val());

    //$.ajax({
    //    url: '/TourRequest/CmdSubmit_Click_CR?id=' + encodeURIComponent(reqid),
    //    method: 'POST',
    //    data: formData,
    //    processData: false,
    //    contentType: false
    //}).done(function (response) {
    //    window.location.href = response?.redirect ||
    //        "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx";
    //});
}

function cmdCancel_Click() {
    window.location.href =
        "/TourRequest/CancelTourRequest?id=" + reqid;
}

function showdetail(RequestID) {

    var url = "/TourDLGS/RequestCancellation?id=" + RequestID;
    window.open(url, "Tour Request", "height=550,width=800,status=0,resizable=0,scrollbars=1");
}
function btnOK_Click() {
    window.location.href =
        "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx";
}
