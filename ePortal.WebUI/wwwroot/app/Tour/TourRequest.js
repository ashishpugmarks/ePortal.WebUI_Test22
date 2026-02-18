// ------------------------------ Globals -----------------------------------------------------------
$(".homeContent").removeClass("homeContent");
var ViewState_SAVESTATUS = "";
var ViewState_EditRow = null;
var ViewState_MODE = "ADD";
var ViewState_EmpDesig = "";
var oTourList = [];
var oTourListprv = [];
var otourperiod = [];
var oday = [];

$(document).ready(function () {
    var cal1 = new CalendarPopup();
    InitializePage();

    $('.alertfont[data-url]').on('click', function (e) {
        e.preventDefault();
        const $a = $(this);
        const url = $a.data('url');
        const name = $a.data('name') || 'popup';
        const width = parseInt($a.data('width')) || 550;
        const height = parseInt($a.data('height')) || 300;
        window.WindowSettings_WithoutMenu(url, name, width, height);
    });

    $("#OptDayStart, #OptMonthStart, #OptYearStart").on("change", function () {
        dateChanged();
    });

    $("#optDayEnd, #optMonthEnd, #optYearEnd").on("change", function () {
        endcClick();
    });

    $("#cboAdvance").on("change", async function () {
        await cboAdvance_SelectedIndexChanged($(this).val(), $("#gvList tr").length);
        document.getElementById("txtDailyObjective").focus();
    });

    $("#optDay, #OptMonth, #OptYear").on("change", function () {
        cclick();
        TravelDateChanged();
    });

    $('#cmbHotelReserv').on('change', function () {
        document.getElementById("cmbHotelReserv").focus();
    });

    $("#cmbFrom").on("change", function () {
        SetFromCity();
        cmbFrom_SelectedIndexChanged();
    });

    $("#cmbTo").on("change", function () {
        SetToCity();
        cmbTo_SelectedIndexChanged();
    });

    $("#cmbStaying").on("change", async function () {
        SetStayCity();
        await cmbStaying_SelectedIndexChanged();
    });

    $("#cmbMode").on("change", async function () {
        await cmbMode_SelectedIndexChanged();
    });

    $("#cmbHotelReserv").on("change", function () {
        SetPickDrop();
        cmbHotelReserv_SelectedIndexChanged();
    });

    $("#Img4").on("click", function (e) {
        e.preventDefault();
        cal1.select(document.getElementById("txtsubdate"), 'Img4', 'dd-NNN-yyyy');
        return false;
    });

    $("#Img4").on("mouseout", function () { HideTooltip(); });
    $("#Img4").on("mouseover", function () { ShowTooltip('Only Finance submit date'); });

    $("#Img3").on("click", function (e) {
        e.preventDefault();
        cal1.select(document.getElementById("txtchqdate"), 'Img3', 'dd-NNN-yyyy');
        return false;
    });

    $("#txtexp").on("change", function () { onExpenseChange(); });

    $("#BtnAdd").on("click", function (e) { e.preventDefault(); BtnAdd_Click(); });

    $(document).on('click', '.btnPrevDelete', function (e) {
        e.preventDefault();
        const index = $(this).data('index');
        if (confirm("Are you sure to delete this record?")) {
            oTourListprv.splice(index, 1);
            renderPrevAdvanceGrid(oTourListprv);
            TourPeriodDropdown(otourperiod);
        }
    });

    $('#cmdAddMore').on('click', function (e) {
        e.preventDefault();
        if (isValidEntry()) {
            cmdAddMore_Click();
        }
    });

    $(document).on("click", ".btnEdit", async function (e) { e.preventDefault(); const index = $(this).data("index"); await gvList_RowEditing(index); });
    $(document).on("click", ".btnDelete", function (e) { e.preventDefault(); const index = $(this).data("index"); if (confirm("Are you sure to delete this record?")) { gvList_RowDeleting(index); } });

    $("#btnSaveAsDraft").on("click", async function (e) {
        e.preventDefault();
        if (submitval()) {
            await btnSaveAsDraft_Click();
        }
    });
    $("#cmdSubmit").on("click", async function (e) {
        e.preventDefault();
        if (submitval()) {
            await cmdSubmit_Click();
        }
    });
    $("#cmdReset").on("click", function (e) { e.preventDefault(); cmdReset_Click(); });

    $("#txtexp").on("input", function () {
        const advance = parseFloat($("#txtadvance").val()) || 0;
        const exp = parseFloat($(this).val()) || 0;
        const balance = advance - exp;
        $("#txtbalance").val(balance.toFixed(2));
        if (balance < 0) {
            $("#ddlrefundby, #txtchqno, #txtchqdate, #txtrefamount").hide();
        } else {
            $("#ddlrefundby, #txtchqno, #txtchqdate, #txtrefamount").show();
        }
    });

    $("#ddltourperiod").on("change", function () { $("#txtadvance").val($(this).val()); });

    $("#txtMiscAmout").on("input", function () {
        let miscAmount = parseFloat($(this).val()) || 0;
        let stayCharge = parseFloat($("#lblTotalCharges").text()) || 0;
        let allowances = parseFloat($("#lblTotalAllowances").text()) || 0;
        let total = stayCharge + allowances + miscAmount;
        $("#lblTotalAmount").text(total.toFixed(2));
        $("#txtRequiredAmount").val(total.toFixed(2));
    });

    $('#txtMiscAmout')
        .on('input', function () {
            this.value = this.value.replace(/[^0-9.]/g, '');
            var i = this.value.indexOf('.');
            if (i !== -1) {
                this.value = this.value.slice(0, i + 1) + this.value.slice(i + 1).replace(/\./g, '');
            }
        })
        .on('change', function (e) {
            e.preventDefault();
            txtMiscAmout_TextChanged();
            CalculateAmount();
        });

    $("#txtRequiredAmount").on("input", function () {
        let totalAmount = parseFloat($("#lblTotalAmount").text()) || 0;
        let requiredAmount = parseFloat($(this).val()) || 0;
        if (requiredAmount > totalAmount) {
            $(this).val(totalAmount.toFixed(2));
            document.getElementById("txtRequiredAmount").focus();
            alert('Required amount should be less than or equal to System generated amount');
        }
    });

    $('#txtRemarks')
        .on('input', function () {
            restrictSpecialChars(this);
        })
        .on('paste', function (e) {
            restrictSpecialChars(this, e);
        });
});


function InitializePage() {
    var style = document.createElement('style');
    style.innerHTML = getCalendarStyles();
    document.head.appendChild(style);

    const cmbStaying = document.getElementById("cmbStaying");
    const txtGST = document.getElementById("txt_GST");
    const txtNo = document.getElementById("txtNo");

    if (cmbStaying && cmbStaying.value === "0") {
        if (txtGST) txtGST.style.display = "none";
        if (txtNo) txtNo.style.display = "none";
    } else {
        if (txtGST) txtGST.style.display = "block";
        if (txtNo) txtNo.style.display = "block";
    }

    blankrow_ADD();
    ViewState_SAVESTATUS = typeof SAVESTATUS !== 'undefined' ? SAVESTATUS : "";
    ViewState_MODE = typeof MODE !== 'undefined' ? MODE : "ADD";
    ViewState_EmpDesig = typeof empDesig !== 'undefined' ? empDesig : "";

    oTourList = oTourList;
    oTourListprv = oTourListprv;
    otourperiod = JSON.parse(otourperiod);
    oday = oday;

    $("#gvListThead").hide();
}

function ShowTooltip(msg) {
    document.getElementById("lblmsgtooltip").innerHTML = msg;
    let x = event.clientX + document.body.scrollLeft;
    let y = event.clientY + document.body.scrollTop + 10;
    Popup.style.display = "block";
    Popup.style.left = x + "px";
    Popup.style.top = y + "px";
}
function HideTooltip() { Popup.style.display = "none"; }

function openPopup(strOpenUrl, width, height) {
    window.open(strOpenUrl, "mywindow", "TOOLBAR=no,MENUBAR=no,SCROLLBARS=no,RESIZABLE=no,LOCATION=no,DIRECTORIES=no,STATUS=no,width=" + width + ",height=" + height);
}
function WindowSettings_WithoutMenu(str1, str2, winWidth, winHeight) {
    var winTop = (screen.height / 2) - 325;
    var winLeft = (screen.width / 2) - 425;
    var windowFeatures = "location=no,status=no,width=" + winWidth + ",height=" + winHeight + ",left=" + winLeft + ",top=" + winTop + ",resizable,scrollbars";
    window.open(str1, str2, windowFeatures);
}

// ------------------------------ Validation -------------------------------------------------------
function checkEmail() {
    if (document.getElementById("txtEmail").value == "")
        return true;

    if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(document.getElementById("txtEmail").value))
        return true;
    else
        return false;
}

function submitval() {
    var fromdate = new Date(document.getElementById("OptYearStart").value, (document.getElementById("OptMonthStart").value - 1), document.getElementById("OptDayStart").value);
    var today = new Date();
    var currentdate = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    var msPerDay = 24 * 60 * 60 * 1000;
    var dbd = Math.floor((fromdate - currentdate) / msPerDay);

    if ((document.getElementById("txtMobile").value).trim() == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Mobile No. is a required field.";
        document.getElementById("txtMobile").focus();
        return false
    }
    if (checkEmail() == false) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Please enter the valid Email ID.";
        document.getElementById("txtEmail").focus();
        return false
    }
    if ((document.getElementById("txtObjective").value).trim() == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Ojective of Journey is a required field.";
        document.getElementById("txtObjective").focus();
        return false
    }
    if (document.getElementById("hfAdvRemakrs").value == "YES" && (document.getElementById("txtAdvRemarks").value).trim() == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Previous Advance Remarks is required field in case of pending settlement.";
        document.getElementById("txtAdvRemarks").focus();
        return false
    }
    if ((document.getElementById("cboAdvance").value).trim() == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Required Tour Advance is required field.";
        document.getElementById("cboAdvance").focus();
        return false
    }
    if (document.getElementById("cboAppAuthority").value == "0") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Approval authority is required field.";
        document.getElementById("cboAppAuthority").focus();
        return false
    }
    if (document.getElementById("txtMiscAmout")) {
        if ((document.getElementById("txtMiscAmout").value).trim() != "" && (document.getElementById("txtMiscRemarks").value).trim() == "") {
            document.getElementById("errorpanel").style.display = "inline";
            document.getElementById("status").innerHTML = "Remarks for Miscellaneous Amount is required if amount filled by associate.";
            document.getElementById("txtMiscRemarks").focus();
            return false
        }
    }
    var uID = parseInt(document.getElementById("hdnUserId").value);
    if (uID > 70000000) return true; // EXPAT

    if (dbd <= 3) alert("Request generated before 3 days of actual travel. so special approval is added with this request.");
    return true;
}

function isValidEntry() {
    var fromdate = new Date(document.getElementById("OptYearStart").value, (document.getElementById("OptMonthStart").value - 1), document.getElementById("OptDayStart").value);
    var todate = new Date(document.getElementById("OptYearEnd").value, (document.getElementById("OptMonthEnd").value - 1), document.getElementById("OptDayEnd").value);
    var traveldate = new Date(document.getElementById("OptYear").value, (document.getElementById("OptMonth").value - 1), document.getElementById("OptDay").value);
    var currdate = new Date();
    var today = new Date(currdate.getFullYear(), currdate.getMonth(), currdate.getDate());

    if (fromdate > todate) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Tour Start Date should be less than equal to Tour End Date.";
        document.getElementById("OptDayStart").focus();
        return false
    }
    if (traveldate < fromdate || traveldate > todate) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Travel Date should be in Tour Period.";
        document.getElementById("OptDay").focus();
        return false
    }
    if ((document.getElementById("txtDailyObjective").value).trim() == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Day objective is required field";
        document.getElementById("txtDailyObjective").focus();
        return false
    }
    if ((document.getElementById("cmbFrom").value == "0") ||
        (document.getElementById("cmbFrom").value == "" && (document.getElementById("txtFromCity").value).trim() == "")) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "From City is required field";
        document.getElementById("cmbFrom").focus();
        return false
    }
    if ((document.getElementById("cmbTo").value == "0") ||
        (document.getElementById("cmbTo").value == "" && (document.getElementById("txtToCity").value).trim() == "")) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "To City is required field";
        document.getElementById("cmbTo").focus();
        return false
    }
    if (document.getElementById("cmbFrom").value != document.getElementById("cmbTo").value) {
        if (document.getElementById("cmbMode").value == "" ||
            document.getElementById("cmbMode").value == "0") {
            document.getElementById("errorpanel").style.display = "inline";
            document.getElementById("status").innerHTML = "Travel Mode is required field.";
            document.getElementById("cmbMode").focus();
            return false
        }
        if ((document.getElementById("cmbMode").value == "1" ||
            document.getElementById("cmbMode").value == "3") && document.getElementById("cmbClass").value == "") {
            document.getElementById("errorpanel").style.display = "inline";
            document.getElementById("status").innerHTML = "Travel class is required field for selected travel mode.";
            document.getElementById("cmbClass").focus();
            return false
        }
        if ((document.getElementById("cmbMode").value == "1" ||
            document.getElementById("cmbMode").value == "3") && document.getElementById("txtModeDetail").value == "") {
            document.getElementById("errorpanel").style.display = "inline";
            document.getElementById("status").innerHTML = "FLT/Train Name is a required field for selected travel mode.";
            document.getElementById("txtModeDetail").focus();
            return false
        }
    }
    if (document.getElementById("cmbHotelReserv").value == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Accommodation is required field.";
        document.getElementById("cmbHotelReserv").focus();
        return false
    }
    if (document.getElementById("chkSpecialApp").checked == true && (document.getElementById("txtRemarks").value).trim() == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Remarks is Required in Case of Special Approval.";
        document.getElementById("txtRemarks").focus();
        return false
    }
    return true;
}

// ------------------------------ Date change logic ------------------------------------------------
//TRAVEL DATE VALIDATION
function cclick() {
    for (var i = 0; i < document.getElementById("OptMonth").options.length; i++) {
        if (document.getElementById("OptMonth").options[i].selected == true) {
            if (document.getElementById("OptMonth").options[i].value == 2) {
                for (var j = 0; j < document.getElementById("OptDay").options.length; j++) {
                    if (document.getElementById("OptDay").options[j].selected == true) {
                        if (document.getElementById("OptDay").options[j].value > 28) {
                            if (document.getElementById("OptDay").options[j].value == 29 & (document.getElementById("OptYear").options.value == 2008 ||
                                document.getElementById("OptYear").options.value == 2012 ||
                                document.getElementById("OptYear").options.value == 2016 ||
                                document.getElementById("OptYear").options.value == 2020 ||
                                document.getElementById("OptYear").options.value == 2024 ||
                                document.getElementById("OptYear").options.value == 2028)) { } else {
                                //alert("Invalid date!")
                                document.getElementById("errorpanel").style.display = "inline";
                                document.getElementById("status").innerHTML = "Invalid date! please select valid travel date.";
                                document.getElementById("OptDay").options[27].selected = true
                            }
                        }
                    }
                }
            }
            if (document.getElementById("OptMonth").options[i].value == 4 ||
                document.getElementById("OptMonth").options[i].value == 6 ||
                document.getElementById("OptMonth").options[i].value == 9 ||
                document.getElementById("OptMonth").options[i].value == 11) {
                for (var j = 0; j < document.getElementById("OptDay").options.length; j++) {
                    if (document.getElementById("OptDay").options[j].selected == true) {
                        if (document.getElementById("OptDay").options[j].value > 30) {
                            document.getElementById("errorpanel").style.display = "inline";
                            document.getElementById("status").innerHTML = "Invalid date! please select valid travel date.";
                            document.getElementById("OptDay").options[29].selected = true
                        }
                    }
                }
            }
        }
    }
}

//TOUR START DATE VALIDATION
function startcclick() {
    for (var i = 0; i < document.getElementById("OptMonthStart").options.length; i++) {
        if (document.getElementById("OptMonthStart").options[i].selected == true) {
            if (document.getElementById("OptMonthStart").options[i].value == 2) {
                for (var j = 0; j < document.getElementById("OptDayStart").options.length; j++) {
                    if (document.getElementById("OptDayStart").options[j].selected == true) {
                        if (document.getElementById("OptDayStart").options[j].value > 28) {
                            if (document.getElementById("OptDayStart").options[j].value == 29 & (document.getElementById("OptYearStart").options.value == 2008 ||
                                document.getElementById("OptYearStart").options.value == 2012 ||
                                document.getElementById("OptYearStart").options.value == 2016 ||
                                document.getElementById("OptYearStart").options.value == 2020 ||
                                document.getElementById("OptYearStart").options.value == 2024 ||
                                document.getElementById("OptYearStart").options.value == 2028)) { } else {
                                //alert("Invalid date!")
                                //document.getElementById("errorpanel").style.display = "inline";
                                //document.getElementById("status").innerHTML = "Invalid date! please select valid date.";
                                document.getElementById("OptDayStart").options[27].selected = true
                            }
                        }
                    }
                }
            }
            if (document.getElementById("OptMonthStart").options[i].value == 4 ||
                document.getElementById("OptMonthStart").options[i].value == 6 ||
                document.getElementById("OptMonthStart").options[i].value == 9 ||
                document.getElementById("OptMonthStart").options[i].value == 11) {
                for (var j = 0; j < document.getElementById("OptDayStart").options.length; j++) {
                    if (document.getElementById("OptDayStart").options[j].selected == true) {
                        if (document.getElementById("OptDayStart").options[j].value > 30) {
                            //document.getElementById("errorpanel").style.display = "inline";
                            //document.getElementById("status").innerHTML = "Invalid date! please select valid date.";
                            document.getElementById("OptDayStart").options[29].selected = true
                        }
                    }
                }
            }
        }
    }
}

//TOURN END DATE VALIDATION
function endcclick() {
    for (var i = 0; i < document.getElementById("OptMonthEnd").options.length; i++) {
        if (document.getElementById("OptMonthEnd").options[i].selected == true) {
            if (document.getElementById("OptMonthEnd").options[i].value == 2) {
                for (var j = 0; j < document.getElementById("OptDayEnd").options.length; j++) {
                    if (document.getElementById("OptDayEnd").options[j].selected == true) {
                        if (document.getElementById("OptDayEnd").options[j].value > 28) {
                            if (document.getElementById("OptDayEnd").options[j].value == 29 & (document.getElementById("OptYearEnd").options.value == 2008 ||
                                document.getElementById("OptYearEnd").options.value == 2012 ||
                                document.getElementById("OptYearEnd").options.value == 2016 ||
                                document.getElementById("OptYearEnd").options.value == 2020 ||
                                document.getElementById("OptYearEnd").options.value == 2024 ||
                                document.getElementById("OptYearEnd").options.value == 2028)) { } else {
                                //alert("Invalid date!")
                                //document.getElementById("errorpanel").style.display = "inline";
                                //document.getElementById("status").innerHTML = "Invalid date! please select valid date.";
                                document.getElementById("OptDayEnd").options[27].selected = true
                            }
                        }
                    }
                }
            }
            if (document.getElementById("OptMonthEnd").options[i].value == 4 ||
                document.getElementById("OptMonthEnd").options[i].value == 6 ||
                document.getElementById("OptMonthEnd").options[i].value == 9 ||
                document.getElementById("OptMonthEnd").options[i].value == 11) {
                for (var j = 0; j < document.getElementById("OptDayEnd").options.length; j++) {
                    if (document.getElementById("OptDayEnd").options[j].selected == true) {
                        if (document.getElementById("OptDayEnd").options[j].value > 30) {
                            //document.getElementById("errorpanel").style.display = "inline";
                            //document.getElementById("status").innerHTML = "Invalid date! please select valid date.";
                            document.getElementById("OptDayEnd").options[29].selected = true
                        }
                    }
                }
            }
        }
    }
}

// ------------------------------ City Input Enablers ----------------------------------------------
function SetFromCity() {
    if (document.getElementById("cmbFrom").value == "") {
        document.getElementById("txtFromCity").disabled = false;
        document.getElementById("txtFromCity").focus();
    } else {
        document.getElementById("txtFromCity").disabled = true;
        document.getElementById("txtFromCity").value = "";
        document.getElementById("cmbFrom").focus();
    }
}
function SetToCity() {
    if (document.getElementById("cmbTo").value == "") {
        document.getElementById("txtToCity").disabled = false;
        document.getElementById("txtToCity").focus();
    } else {
        document.getElementById("txtToCity").disabled = true;
        document.getElementById("txtToCity").value = "";
        document.getElementById("cmbTo").focus();
    }
}
function SetStayCity() {
    if (document.getElementById("cmbStaying").value == "") {
        document.getElementById("txtStayingCity").disabled = false;
        document.getElementById("txtStayingCity").focus();
    } else {
        document.getElementById("txtStayingCity").disabled = true;
        document.getElementById("txtStayingCity").value = "";
        document.getElementById("cmbStaying").focus();
    }
}

//MAKE ENABLE PICK AND DROP COMBO INCASE OF HOTEL RESV. BY ADMIN
// <% --Adddded By Kishan Dodiya-- % >
function SetPickDrop() {
    /*<% --document.getElementById("cmbPickDrop").value = "";
    // IN CASE OF ADMIN
    if (document.getElementById("cmbHotelReserv").value == "1")
     document.getElementById("cmbPickDrop").disabled = false;
    else
     document.getElementById("cmbPickDrop").disabled = true;
    -- %>*/
} /*<% --End Added-- % >*/

// ------------------------------ Amount Calculations ----------------------------------------------
function CalculateAmount() {
    var MiscAmount = 0;
    var StayCharge = 0;
    var Allowances = 0;
    var TotalAmount = 0;

    if (document.getElementById("txtMiscAmout").value != "")
        MiscAmount = document.getElementById("txtMiscAmout").value;

    StayCharge = document.getElementById("lblTotalCharges").innerHTML;
    Allowances = document.getElementById("lblTotalAllowances").innerHTML;
    TotalAmount = parseFloat(StayCharge) + parseFloat(Allowances) + parseFloat(MiscAmount);

    document.getElementById("lblTotalAmount").innerHTML = TotalAmount.toFixed(2);
    document.getElementById("txtRequiredAmount").value = TotalAmount.toFixed(2);
}

function ReqAmountCHK() {
    var TotalAmount = document.getElementById("lblTotalAmount").innerHTML;
    var ReqAmount = document.getElementById("txtRequiredAmount").value;

    if (parseFloat(ReqAmount) > parseFloat(TotalAmount)) {
        document.getElementById("txtRequiredAmount").value = document.getElementById("lblTotalAmount").innerHTML;
        alert('Required amount should be less than equal to System generated amount');
        document.getElementById("txtRequiredAmount").focus();
    }
}

// SR78268
function restrictSpecialChars(textBox) {
    var pattern = /[\<,\>`]/g;
    textBox.value = textBox.value.replace(pattern, '');
}

// ------------------------------ Start Date Changed -----------------------------------------------
function dateChanged() {
    let strDay = ("00" + document.getElementById("OptDayStart").value).slice(-2);
    let strMonth = document.getElementById("OptMonthStart").value;
    let strYear = document.getElementById("OptYearStart").value;

    let tourStartDateStr = strDay + "-" + strMonth + "-" + strYear;
    let startDate = new Date(tourStartDateStr);

    if (!isValidDate(startDate)) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerText = "Please select valid Date!";
        return;
    }

    let today = new Date();

    if (today > startDate) {
        document.getElementById("cboAdvance").value = "NO";
        document.getElementById("cboAdvance").disabled = true;

        document.getElementById("cmbTicketingBy").value = "0";
        document.getElementById("cmbTicketingBy").disabled = true;
    } else {
        document.getElementById("cboAdvance").disabled = false;
        document.getElementById("cmbTicketingBy").disabled = false;
    }
}

async function cboAdvance_SelectedIndexChanged() {
    await CalculateDayNights();
    document.getElementById('txtDailyObjective').focus();
}

// ------------------------------ Day/Night Calculation --------------------------------------------
async function CalculateDayNights() {
    let chargeAP = 0, chargeA = 0, chargeB = 0, chargeC = 0;
    let allowAP = 0, allowA = 0, allowB = 0, allowC = 0;
    let TotalDailyAllowance = 0, TotalNightCharge = 0;

    let cboAdvance = document.getElementById('cboAdvance').value;
    let gvListRows = document.querySelectorAll('#gvList tbody tr');

    if (cboAdvance === "YES" && gvListRows.length > 0 && gvListRows[0].cells[0].innerText !== "No Data Found") {
        document.getElementById('pnlAdvance').style.display = 'block';

        let ProcessDates = [];
        let ProcessNights = [];

        // Use for...of because we need await inside the loop
        for (const oTour of oTourList) {
            let days = 1, nights = 1;

            let TravelDate = oTour.TourFromDate;
            let StayingCity = (oTour.StayingLoc || "").trim();
            let CityCode = oTour.StayingLocCode;

            // Day processed?
            if (DailyAllowanceCaluculation(ProcessDates, TravelDate)) { days = 0; }
            else { ProcessDates.push(TravelDate); }

            // Night processed?
            if (StayChargeCaluculation(ProcessNights, TravelDate, StayingCity)) { nights = 0; }
            else if (StayingCity !== "") { ProcessNights.push(TravelDate); }

            if (StayingCity === "") { nights = 0; CityCode = oTour.ToLocCode; }

            const cityData = await GetCityCategory(CityCode);
            if (!cityData) continue;

            let CityCategory = cityData.DESCRIP;
            let CityCategoryCode = cityData.SYCITYCATEGORYID;
            let EmpDesignationID = ViewState_EmpDesig || 21;

            let CurrDate = new Date().toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
            let UserType = parseInt(document.getElementById("hdnUserId").value) > 70000000 ? "EXPAT" : "LOCAL";

            const allowanceData = await GetEmployeeAllowanceDetail(CityCategoryCode, EmpDesignationID, CurrDate, UserType);
            if (!allowanceData) continue;

            let NightCharge = allowanceData.LODGINGAMT;
            let DailyAllowance = allowanceData.DAILYALLOWANCEAMT;

            TotalDailyAllowance += (DailyAllowance * days);
            TotalNightCharge += (NightCharge * nights);

            switch (CityCategory) {
                case "A+": chargeAP += nights; allowAP += days; break;
                case "A": chargeA += nights; allowA += days; break;
                case "B": chargeB += nights; allowB += days; break;
                case "C": chargeC += nights; allowC += days; break;
            }
        }

        let TotalNights = chargeAP + chargeA + chargeB + chargeC;
        let TotalDays = allowAP + allowA + allowB + allowC;

        document.getElementById('lblStayChargeAPLUS').innerText = chargeAP;
        document.getElementById('lblAllownceAPLUS').innerText = allowAP;

        document.getElementById('lblStayChargeA').innerText = chargeA;
        document.getElementById('lblAllownceA').innerText = allowA;

        document.getElementById('lblStayChargeB').innerText = chargeB;
        document.getElementById('lblAllownceB').innerText = allowB;

        document.getElementById('lblStayChargeC').innerText = chargeC;
        document.getElementById('lblAllownceC').innerText = allowC;

        document.getElementById('lblTotalNights').innerText = TotalNights;
        document.getElementById('lblTotalDays').innerText = TotalDays;

        document.getElementById('lblTotalAllowances').innerText = TotalDailyAllowance.toFixed(2);
        document.getElementById('lblTotalCharges').innerText = TotalNightCharge.toFixed(2);
    } else {
        document.getElementById('pnlAdvance').style.display = 'none';
    }

    let MiscAmount = parseFloat(document.getElementById('txtMiscAmout').value) || 0;
    let totalAmount = TotalDailyAllowance + TotalNightCharge + MiscAmount;

    document.getElementById('lblTotalAmount').innerText = totalAmount.toFixed(2);
    document.getElementById('txtRequiredAmount').value = totalAmount.toFixed(2);
}

function DailyAllowanceCaluculation(processDates, travelDate) {
    for (let str of processDates) { if (str === travelDate) return true; }
    return false;
}
function StayChargeCaluculation(processNights, travelDate, stayingCity) {
    for (let str of processNights) { if (str === travelDate && stayingCity.trim() !== "") return true; }
    return false;
}

// ------------------------------ AJAX Helpers -----------------------------------------------------
async function GetCityCategory(cityCode) {
    return $.ajax({
        url: '/TourRequest/GetCityCategory',
        type: 'GET',
        data: { CityCode: cityCode }
    }).then(function (response) { return response.result; })
        .catch(function (error) { console.log(error.responseJSON?.message || 'An error occurred.'); return null; });
}

async function GetEmployeeAllowanceDetail(cityCategoryCode, empDesignationID, currDate, userType) {
    return $.ajax({
        url: '/TourRequest/GetEmployeeAllowanceDetail',
        type: 'GET',
        data: { CityCategoryCode: cityCategoryCode, EmpDesignationID: empDesignationID, CurrDate: currDate, UserType: userType }
    }).then(function (response) { return response.result; })
        .catch(function (error) { console.log(error.responseJSON?.message || 'An error occurred.'); return null; });
}

// ------------------------------ Travel Date Changed ----------------------------------------------
function TravelDateChanged() {
    let mode = $("#cmbMode").val();
    let day = ("00" + $("#OptDay").val()).slice(-2);
    let month = $("#OptMonth option:selected").text();// $("#OptMonth").val();
    let year = $("#OptYear").val();

    let dateStr = `${day}-${month}-${year}`;
    let startDate = parseDate(dateStr);
    if (!startDate) { alert("Invalid date!"); return; }

    let today = new Date();
    if (startDate < today) {
        updateDropdown("#cmbHotelReserv", [{ text: "Self", value: "0" }]);
        updateDropdown("#cmbTicketingBy", [{ text: "Self", value: "0" }]);
    } else {
        updateDropdown("#cmbHotelReserv", [{ text: "Admin", value: "1" }]);
        updateDropdown("#cmbTicketingBy", [{ text: "Admin", value: "1" }]);
    }

    if (mode === "2") {
        updateDropdown("#cmbTicketingBy", [{ text: "Self", value: "0" }, { text: "Admin", value: "1" }]);
    }
    document.getElementById("cmdAddMore").focus();
}

function cmbFrom_SelectedIndexChanged() {
    let selectedValue = $("#cmbFrom").val();
    if (selectedValue === "") {
        document.getElementById("txtFromCity").disabled = false;
        document.getElementById("txtFromCity").focus();
    } else {
        $("#txtFromCity").val("").prop("disabled", true);
        document.getElementById("cmbFrom").focus();
    }
    document.getElementById("cmdAddMore").focus();
}

function cmbTo_SelectedIndexChanged() {
    let selectedValue = $("#cmbTo").val();
    if (selectedValue === "") {
        document.getElementById("txtToCity").disabled = false;
        document.getElementById("txtToCity").focus();
    } else {
        $("#txtToCity").val("").prop("disabled", true);
        document.getElementById("cmbTo").focus();
    }
    document.getElementById("cmdAddMore").focus();
}

async function cmbStaying_SelectedIndexChanged() {
    let cityId = $("#cmbStaying").val();
    await fillGSTINNOClass(cityId);
    document.getElementById("cmbStaying").focus();

    if (cityId === "" || cityId === "0") { $("#txt_GST").val("-"); }

    if (cityId === "") {
        document.getElementById("txtStayingCity").disabled = false;
        document.getElementById("txtStayingCity").focus();
    } else {
        $("#txtStayingCity").val("").prop("disabled", true);
        document.getElementById("cmbStaying").focus();
    }
    document.getElementById("cmdAddMore").focus();
}

async function fillGSTINNOClass(cityId) {
    return $.ajax({
        url: '/TourRequest/FillGSTINNOClass',
        type: 'GET',
        data: { CityId: cityId },
        success: function (response) {
            var result = response.result;
            if (result) { $("#txt_GST").text(result.txt_GST_Text); }
        }
    }).catch(function (xhr) { console.log(xhr.responseJSON?.message || 'An error occurred.'); });
}


async function cmbMode_SelectedIndexChanged() {
    const cmbMode = document.getElementById('cmbMode');
    const ModeID = ["", "0"].includes(cmbMode.value) ? "0" : cmbMode.value;


    await FillTravelClass(ModeID);
    cmbMode.focus();

    let tr_train = document.getElementById('tr_train');
    if (tr_train) tr_train.style.display = (ModeID === "3") ? 'block' : 'none';

    if (ModeID === "2") {
        let cmbTicketingByEl = document.getElementById('cmbTicketingBy');
        cmbTicketingByEl.innerHTML = '';
        let liSlf = new Option("Self", "0");
        let liAdm = new Option("Admin", "1");
        cmbTicketingByEl.add(liSlf);
        cmbTicketingByEl.add(liAdm);
        return;
    }
    document.getElementById('cmdAddMore').focus();
}

async function FillTravelClass(modeID) {
    return $.ajax({
        url: '/TourRequest/FillTravelClass',
        type: 'GET',
        data: { ModeID: modeID },
        dataType: 'json'
    }).then(function (response) {
        var ddl = $('#cmbClass');
        ddl.empty();

        if (response.result && response.result.length > 0) {
            $.each(response.result, function (i, item) {
                ddl.append($('<option>', {
                    value: item.Value,
                    text: item.Text,
                    selected: item.Selected,
                    disabled: item.Disabled
                }));
            });
        }

        // Ensure something is selected to avoid selectedIndex = -1
        const el = document.getElementById('cmbClass');
        if (el && el.options.length > 0) {
            if (el.selectedIndex < 0) el.selectedIndex = 0;
        }

        ddl.trigger('change');
    }).catch(function (xhr) {
        console.log(xhr.responseJSON?.message || 'An error occurred.');
    });
}

function cmbHotelReserv_SelectedIndexChanged() {
    document.getElementById('cmbHotelReserv').focus();
}

function onExpenseChange() {
    var advance = parseFloat(document.getElementById("txtadvance").value) || 0;
    var expense = parseFloat(document.getElementById("txtexp").value) || 0;
    var balance = advance - expense;

    document.getElementById("txtbalance").value = balance.toFixed(2);
    var refundFields = ["ddlrefundby", "txtchqno", "txtchqdate", "txtrefamount"];
    refundFields.forEach(function (id) {
        document.getElementById(id).style.display = (balance < 0) ? "none" : "block";
    });
}

// ------------------------------ Prev Advance Grid -----------------------------------------------
function BtnAdd_Click() {
    const ddltourperiod = document.getElementById("ddltourperiod");
    const txtadvance = document.getElementById("txtadvance");
    const txtexp = document.getElementById("txtexp");
    const txtbalance = document.getElementById("txtbalance");
    const txtsubdate = document.getElementById("txtsubdate");
    const ddlrefundby = document.getElementById("ddlrefundby");
    const txtchqno = document.getElementById("txtchqno");
    const txtchqdate = document.getElementById("txtchqdate");
    const txtrefamount = document.getElementById("txtrefamount");

    if (ddltourperiod.value === "0") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerText = "Select Tour Period";
        ddltourperiod.focus();
        return;
    }

    const Tourperiod = ddltourperiod.options[ddltourperiod.selectedIndex].text;
    const Advance = ddltourperiod.value;
    const Exp = txtexp.value;
    const Balance = txtbalance.value;
    const Subdate = txtsubdate.value;
    const Refundby = ddlrefundby.value;
    const Refundbydes = ddlrefundby.value === "0" ? "" : ddlrefundby.options[ddlrefundby.selectedIndex].text;
    const Chqno = txtchqno.value;
    const Chqdate = txtchqdate.value;
    const Refamount = txtrefamount.value;

    const oTour = { Tourperiod, Advance, Exp, Balance, Subdate, Refundby, Chqno, Chqdate, Refamount, Refundbydes };

    const modeInput = document.getElementById("MODE");
    const mode = modeInput ? modeInput.value : "ADD";

    if (mode === "ADD") {
        oTourListprv.push(oTour);
    } else {
        const editIndex = parseInt(document.getElementById("EditRow").value, 10);
        oTourListprv[editIndex] = oTour;
        document.getElementById("MODE").value = "ADD";
        document.getElementById("cmdAddMore").innerText = "Add More";
        document.getElementById("EditRow").value = "";
    }

    renderPrevAdvanceGrid(oTourListprv);
    TourPeriodDropdown(otourperiod);

    txtexp.value = "";
    txtbalance.value = "";
    txtsubdate.value = "";
    ddlrefundby.selectedIndex = 0;
    txtchqno.value = "";
    txtchqdate.value = "";
    txtrefamount.value = "";
    ddltourperiod.selectedIndex = 0;
    txtadvance.value = "";
}

function renderPrevAdvanceGrid(list) {
    const tbody = document.querySelector('#grdprvadv tbody');
    if (!tbody) { console.log('tbody for #grdprvadv not found'); return; }

    tbody.innerHTML = '';

    list.forEach((item, i) => {
        const tr = document.createElement('tr');
        tr.className = 'grdrow';
        tr.innerHTML = `
      <td align="left" valign="top" style="width:10%;">
        <span id="lblTourPeriod" style="display:inline-block;width:113px;">${item.Tourperiod ?? ''}</span>
      </td>
      <td align="left" valign="top" style="width:10%;">
        <span id="lbladvance" style="display:inline-block;width:70px;">${item.Advance ?? ''}</span>
      </td>
      <td align="left" valign="top" style="width:10%;">
        <span id="lblexp" style="display:inline-block;width:70px;">${item.Exp ?? ''}</span>
      </td>
      <td align="left" valign="top" style="width:10%;">
        <span id="lblbalance" style="display:inline-block;width:70px;">${item.Balance ?? ''}</span>
      </td>
      <td align="left" valign="top" style="width:10%;">
        <span id="lblsubdate" style="display:inline-block;width:70px;">${item.Subdate ?? ''}</span>
      </td>
      <td align="left" valign="top" style="width:10%;">
        <span id="lblrefundbydes" style="display:inline-block;width:70px;">${item.Refundbydes ?? ''}</span>
      </td>
      <td align="left" valign="top" style="width:10%;">
        <span id="lblchqno" style="display:inline-block;width:70px;">${item.Chqno ?? ''}</span>
      </td>
      <td align="left" valign="top" style="width:10%;">
        <span id="lblchqdate" style="display:inline-block;width:70px;">${item.Chqdate ?? ''}</span>
      </td>
      <td align="left" valign="top" style="width:10%;">
        <span id="lblrefamount" style="display:inline-block;width:70px;">${item.Refamount ?? ''}</span>
      </td>
      <td align="left" valign="top">
        <img class="btnPrevDelete" src="/images/deleteactive.png" title="Click to delete the record" data-index="${i}" style="cursor:pointer;" />
      </td>`;
        tbody.appendChild(tr);
    });

    // If nothing to show, keep hidden placeholder row
    if (list.length === 0) {
        const tr = document.createElement('tr');
        tr.style.display = 'none';
        tr.innerHTML = `<td colspan="10"></td>`;
        tbody.appendChild(tr);
    }
}

function BtnDelete_Click(index) {
    oTourListprv.splice(index, 1);
    renderPrevAdvanceGrid();
    if (oTourListprv.length === 0) { blankrow_ADD(); }
    TourPeriodDropdown(otourperiod);
}

function blankrow_ADD() {
    const emptyRow = { Tourperiod: "", Advance: "", Exp: "", Balance: "", Subdate: "", Refundbydes: "", Chqno: "", Chqdate: "", Refamount: "" };
    const grid = document.getElementById("grdprvadv");
    if (grid) {
        const row = document.createElement("tr");
        row.style.display = "none";
        for (let key in emptyRow) { const cell = document.createElement("td"); cell.textContent = emptyRow[key]; row.appendChild(cell); }
        grid.querySelector("tbody")?.appendChild(row);
    }
}

function TourPeriodDropdown(otourperiod) {
    if (!Array.isArray(otourperiod)) {
        otourperiod = otourperiod ? [otourperiod] : [];
    }
    const $dropdown = $('#grdprvadv tfoot #ddltourperiod');
    if ($dropdown.length === 0) {
        console.log('Dropdown #ddltourperiod not found under #grdprvadv tfoot.');
        return;
    }
    $dropdown.empty();
    $dropdown.append($('<option>', { value: '0', text: '-Select Period-' }));
    $.each(otourperiod, function (_, item) {
        const value = item.Amount ?? item.amount ?? item.Id ?? item.value ?? '';
        const text = item.TourPeriod ?? item.tourperiod ?? item.text ?? item.Name ?? String(value);
        $dropdown.append($('<option>', { value: value, text: text }));
    });
}

// ------------------------------ Add More Click ----------------------------------------

function cmdAddMore_Click() {
    $("#gvListThead").show();

    const showError = (msg) => {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerText = msg;
        return false;
    };

    const tourObjective = document.getElementById("txtDailyObjective").value.trim();
    const tourStartDate = new Date(
        document.getElementById("OptYearStart").value,
        document.getElementById("OptMonthStart").value - 1,
        document.getElementById("OptDayStart").value
    );
    const tourEndDate = new Date(
        document.getElementById("OptYearEnd").value,
        document.getElementById("OptMonthEnd").value - 1,
        document.getElementById("OptDayEnd").value
    );
    const tourFromDate = new Date(
        document.getElementById("OptYear").value,
        document.getElementById("OptMonth").value - 1,
        document.getElementById("OptDay").value
    );

    const fromLocCode = document.getElementById("cmbFrom").value;
    const toLocCode = document.getElementById("cmbTo").value;
    const stayingLocCode = document.getElementById("cmbStaying").value;

    const cmbMode = document.getElementById('cmbMode');
    const modeID = ["", "0"].includes(cmbMode.value) ? "0" : cmbMode.value;

    const strIdType = document.getElementById("ddlidtype").value;
    const strIdNo = document.getElementById("txtidcardno").value.trim();
    const fromOtherCity = document.getElementById("txtFromCity").value.trim();
    const toOtherCity = document.getElementById("txtToCity").value.trim();

    if (!tourObjective) return showError("Please Enter Day Objective.");
    if (!(tourFromDate >= tourStartDate && tourFromDate <= tourEndDate))
        return showError("Please select a travel date between the tour start date and the tour end date.");
    if (fromLocCode === "0") return showError("Please select City From.");
    if (toLocCode === "0") return showError("Please select City To.");
    if (fromLocCode === "" && fromOtherCity === "") return showError("Please Enter Other cities from");
    if (toLocCode === "" && toOtherCity === "") return showError("Please Enter Other cities to");
    if (modeID === "3" && strIdType === "0" && strIdNo === "") return showError("Please Fill ID Card Type and No");
    const hffromcity = document.getElementById("hffromcity");
    const hffromOthercity = document.getElementById("hffromOthercity");
    const today = new Date();
    const validDays = Math.floor((tourFromDate - today) / (1000 * 60 * 60 * 24));
    let specialApp = document.getElementById("chkSpecialApp").checked ? "1" : "0";
    let specialAppDesc = document.getElementById("chkSpecialApp").checked ? "Yes" : "No";

    if (validDays <= 3) {
        const uId = parseInt(document.getElementById("hdnUserId").value);
        if (uId > 70000000) {
            document.getElementById("chkSpecialApp").disabled = true;
            specialApp = "0";
            specialAppDesc = "No";
        } else {
            specialApp = "1";
            specialAppDesc = "Yes";
        }
    }

    //if (
    //    ((fromLocCode !== "0" && fromLocCode === toLocCode && stayingLocCode !== "0") ||
    //        (fromLocCode === "" && toLocCode === "" && stayingLocCode !== "0" && fromOtherCity === toOtherCity))
    //) {
    //    alert("Staying charges not applicable at the time of return from the tour");
    //    document.getElementById("cmbStaying").focus();
    //    return false;
    //}
    if (hffromcity.value === "0") {
        hffromcity.value = fromLocCode;
        hffromOthercity.value = fromOtherCity; // Added By Aumento as on 08032024
    }

    if (hffromcity.value !== "0") {

        if (hffromcity.value === toLocCode && stayingLocCode !== "0") {

            // Below If Condition Added By Aumento as on 28022024
            if (fromLocCode === "" && toLocCode === "" && stayingLocCode !== "0") {

                // If Condition Added By Aumento as on 08032024
                if (hffromOthercity.value !== "0") {

                    // If Condition Modify By Aumento as on 08032024
                    if (hffromOthercity.value === toOtherCity) {

                        alert("Staying charges not applicable at the time of return from the tour");
                        document.getElementById("cmbStaying").focus();

                        // Reset values
                        hffromcity.value = "0";
                        hffromOthercity.value = "0";

                        document.getElementById("cmbFrom").value = "0";
                        document.getElementById("cmbTo").value = "0";
                        document.getElementById("txtFromCity").value = "";
                        document.getElementById("txtToCity").value = "";

                        return false;
                    }
                }

            } else {

                alert("Staying charges not applicable at the time of return from the tour");
                document.getElementById("cmbStaying").focus();

                // Reset values
                hffromcity.value = "0";
                hffromOthercity.value = "0";

                document.getElementById("cmbFrom").value = "0";
                document.getElementById("cmbTo").value = "0";

                return false;
            }
        }
    }


    const ticketClassValue = document.getElementById("cmbClass").value;
    const ticketClassText =
        (ticketClassValue === "" || ticketClassValue === "0") ? "" : safeSelectedText("cmbClass");

    // Use safeSelectedText for month name to avoid nulls
    const monthText = safeSelectedText("OptMonth");

    const tourData = {
        TourObjective: tourObjective,
        TourStartDate: `${document.getElementById("OptDayStart").value}-${document.getElementById("OptMonthStart").value}-${document.getElementById("OptYearStart").value}`,
        TourEndDate: `${document.getElementById("OptDayEnd").value}-${document.getElementById("OptMonthEnd").value}-${document.getElementById("OptYearEnd").value}`,
        TourFromDate: `${document.getElementById("OptDay").value}-${monthText}-${document.getElementById("OptYear").value}`,

        TourTime: `${document.getElementById("OptHour").value}:${document.getElementById("OptMinute").value}`,
        TourTimeTo: `${document.getElementById("OptHourTo").value}:${document.getElementById("OptMinuteTo").value}`,

        FromLocCode: fromLocCode,
        ToLocCode: toLocCode,
        StayingLocCode: (stayingLocCode == "" || stayingLocCode == "0") ? "": stayingLocCode,

        FromLoc: fromLocCode === "" ? fromOtherCity : safeSelectedText("cmbFrom"),
        ToLoc: toLocCode === "" ? toOtherCity : safeSelectedText("cmbTo"),
        StayingLoc: (stayingLocCode === "" || stayingLocCode === "0")
            ? document.getElementById("txtStayingCity").value.trim()
            : safeSelectedText("cmbStaying"),

        ModeID: modeID,
        Mode:  ["", "0"].includes(cmbMode.value) ? "" : safeSelectedText("cmbMode"),

        ModeDetail: document.getElementById("txtModeDetail").value.trim(),

        TicketClassID: ticketClassValue,
        TicketClass: ticketClassText,

        //TickingBy
        TicketingBy: document.getElementById("cmbTicketingBy").value,
        HotelReserv: document.getElementById("cmbHotelReserv").value,
        PickDrop: "0",
        SpecialApp: specialApp,
        SpecialAppDesc: specialAppDesc,
        Remarks: document.getElementById("txtRemarks").value.trim(),
        IDType: strIdType,
        IDNo: strIdNo,
        PreferredLocation: document.getElementById("preferredLocation").value.trim(),
        TicketClasssID : document.getElementById("cmbClass").value
    };

    updateTourList(tourData);
}

// ------------------------------ List Handling ----------------------------------------------------
function updateTourList(oTour) {
    if (ViewState_MODE === "ADD") {
        oTourList.push(oTour);
    }
    else {
        if (ViewState_EditRow !== null && ViewState_EditRow >= 0 && ViewState_EditRow < oTourList.length) {
            oTourList[ViewState_EditRow] = oTour;
        }
        ViewState_MODE = "ADD";
        document.getElementById("cmdAddMore").innerText = "Add More";
        ViewState_EditRow = null;
    }

    bind_gvListGrid(oTourList);
    clearControls();
    CalculateDayNights();

    document.getElementById("tr_train").style.display = "none";
    document.getElementById("ddlidtype").value = "0";
    document.getElementById("txtidcardno").value = "";
}

function bind_gvListGrid(list) {
    const gridBody = document.querySelector("#gvList tbody");
    if (!gridBody) { console.log("Grid element not found!"); return; }

    gridBody.innerHTML = "";

    list.forEach((tour, index) => {
        const rowNumber = index + 1;
        const row = document.createElement("tr");
        row.className = `row${rowNumber}`;
        row.style.color = "#333333";
        row.style.backgroundColor = "#F7F6F3";

        row.innerHTML = `
      <td align="left" valign="top"><span id="lblTourObjective_${rowNumber}">${tour.TourObjective ?? ""}</span></td>
      <td align="left" valign="top"><span id="lblTourFromDate_${rowNumber}">${tour.TourFromDate ?? ""}</span></td>
      <td align="left" valign="top"><span id="lblTourTime_${rowNumber}">${tour.TourTime ?? ""}</span></td>
      <td align="left" valign="top"><span id="lblFromLoc_${rowNumber}">${tour.FromLoc ?? ""}</span></td>
      <td align="left" valign="top"><span id="lblToLoc_${rowNumber}">${tour.ToLoc ?? ""}</span></td>
      <td align="left" valign="top"><span id="lblStayingLoc_${rowNumber}">${tour.StayingLoc ?? ""}</span></td>
      <td align="left" valign="top"><span id="lblMode_${rowNumber}">${tour.Mode ?? ""}</span></td>
      <td align="left" valign="top"><span id="lblTicketClass_${rowNumber}">${tour.TicketClass ?? ""}</span></td>
      <td align="left" valign="top"><span id="lblModeDetail_${rowNumber}">${tour.ModeDetail ?? ""}</span></td>
      <td align="left" valign="top"><span id="lblSpecialAppDesc_${rowNumber}">${tour.SpecialAppDesc ?? ""}</span></td>
      <td align="left" valign="top">
        <img class="btnEdit" src="/images/icon_edit.gif" style="vertical-align: middle;" title="Edit" data-index="${index}" />
      </td>
      <td align="left" valign="top">
        <img class="btnDelete" src="/images/icon_delete.gif" style="vertical-align: middle;" title="Delete" data-index="${index}" />
      </td>
    `;
        gridBody.appendChild(row);
    });
}

function clearControls() {
    document.getElementById("txtModeDetail").value = "";
    document.getElementById("txtDailyObjective").value = "";
    document.getElementById("txtRemarks").value = "";
    document.getElementById("txtFromCity").value = "";
    document.getElementById("txtToCity").value = "";
    document.getElementById("txtStayingCity").value = "";
    document.getElementById("preferredLocation").value = "";

    document.getElementById("cmbFrom").value = "0";
    document.getElementById("cmbTo").value = "0";
    document.getElementById("cmbStaying").value = "0";
    document.getElementById("cmbMode").value = "0";
    document.getElementById("cmbClass").value = "0";

    document.getElementById("chkSpecialApp").checked = false;

    document.getElementById("txtFromCity").disabled = true;
    document.getElementById("txtToCity").disabled = true;
    document.getElementById("txtStayingCity").disabled = true;

    document.getElementById("OptHour").value = "0";
    document.getElementById("OptMinute").value = "0";
    document.getElementById("OptHourTo").value = "0";
    document.getElementById("OptMinuteTo").value = "0";
}

async function gvList_RowEditing(index) {
    const tour = oTourList[index];
    ViewState_EditRow = index;

    document.getElementById("txtDailyObjective").value = tour.TourObjective;

    const travelDate = tour.TourFromDate.split("-");
    const travelTime = tour.TourTime.split(":");
    const travelTimeTo = tour.TourTimeTo.split(":");

    document.getElementById("OptDay").value = travelDate[0];
    document.getElementById("OptMonth").value = getMonthValue(travelDate[1]);
    document.getElementById("OptYear").value = travelDate[2];

    document.getElementById("OptHour").value = travelTime[0];
    document.getElementById("OptMinute").value = travelTime[1];

    document.getElementById("OptHourTo").value = travelTimeTo[0];
    document.getElementById("OptMinuteTo").value = travelTimeTo[1];

    document.getElementById("cmbFrom").value = tour.FromLocCode;
    document.getElementById("cmbTo").value = tour.ToLocCode;
    document.getElementById("cmbStaying").value = tour.StayingLocCode === "" ? "0" : tour.StayingLocCode;

    document.getElementById("txtFromCity").disabled = tour.FromLocCode !== "";
    document.getElementById("txtFromCity").value = tour.FromLocCode === "" ? tour.FromLoc : "";

    document.getElementById("txtToCity").disabled = tour.ToLocCode !== "";
    document.getElementById("txtToCity").value = tour.ToLocCode === "" ? tour.ToLoc : "";

    document.getElementById("txtStayingCity").disabled = tour.StayingLocCode !== "";
    document.getElementById("txtStayingCity").value = tour.StayingLocCode === "" ? tour.StayingLoc : "";

    document.getElementById("cmbMode").value = tour.ModeID;
    await FillTravelClass(tour.ModeID);

    document.getElementById("cmbClass").value = tour.TicketClasssID;
    document.getElementById("txtModeDetail").value = tour.ModeDetail;

    document.getElementById("cmbTicketingBy").value = tour.TickingBy;
    document.getElementById("cmbHotelReserv").value = tour.HotelReserv;
    document.getElementById("preferredLocation").value = tour.PreferredLocation;

    document.getElementById("chkSpecialApp").checked = tour.SpecialApp === "1";
    document.getElementById("txtRemarks").value = tour.Remarks;

    document.getElementById("cmdAddMore").innerText = "Update";
    ViewState_MODE = "UPDATE";

    CalculateDayNights();
}

function gvList_RowDeleting(index) {
    oTourList.splice(index, 1);
    bind_gvListGrid(oTourList);
    CalculateDayNights();
}

// ------------------------------ Save as Draft ----------------------------------------------------
async function btnSaveAsDraft_Click() {
    oTourList = oTourList || [];
    oTourListprv = oTourListprv || [];
    oday = oday || [];

    const saveStatus = (ViewState_SAVESTATUS || "").trim();
    if (saveStatus !== "NO") return;

    const userDetails = await getEmployeeOfficialDetails();
    const userSiteID = userDetails?.SYSITEID;

    const sitesDay = await getParameterValue();
    const siteDayArray = (sitesDay || "").split("\n");

    if (document.getElementById("cboAdvance").value === "YES") {
        for (let str of siteDayArray) {
            const [PDay, psite] = str.split("~");
            if (userSiteID === psite) {
                for (let vrownum = 0; vrownum < oday.length; vrownum++) {
                    const oTour = oday[vrownum];
                    const ODDAY = oTour.Day ? parseInt(oTour.Day) : 0;
                    if (ODDAY > parseInt(PDay)) {
                        showError(`Your Previous advance is overdue by ${oTour.Day} days. Submit your bill first.`);
                        highlightRow("gvAdvance", vrownum);
                        return;
                    }
                }
            }
        }
    }

    if (document.querySelectorAll("#gvList tr").length === 0) { showError("Please fill at least one tour details."); return; }
    if (oday.length >= 3 && oTourListprv.length === 0) { showError("Please fill previous advance details."); return; }

    const MobileNo = document.getElementById("txtMobile").value;
    const ExtNo = document.getElementById("txtExtension").value;
    const Email = document.getElementById("txtEmail").value;
    const Objective = document.getElementById("txtObjective").value;

    const AdvRequired = document.getElementById("cboAdvance").value === "YES" ? 1 : 0;
    const AdvRemarks = document.getElementById("txtAdvRemarks").value;

    const TourStartDate = `${getSelectedValue("OptDayStart")}-${getSelectedText("OptMonthStart")}-${getSelectedValue("OptYearStart")}`;
    const TourEndDate = `${getSelectedValue("OptDayEnd")}-${getSelectedText("OptMonthEnd")}-${getSelectedValue("OptYearEnd")}`;

    let APlusNights = document.getElementById("lblStayChargeAPLUS").innerText;
    let ANights = document.getElementById("lblStayChargeA").innerText;
    let BNights = document.getElementById("lblStayChargeB").innerText;
    let CNights = document.getElementById("lblStayChargeC").innerText;
    let StayCharge = parseFloat(document.getElementById("lblTotalCharges").innerText) || 0;

    let APlusDays = document.getElementById("lblAllownceAPLUS").innerText;
    let ADays = document.getElementById("lblAllownceA").innerText;
    let BDays = document.getElementById("lblAllownceB").innerText;
    let CDays = document.getElementById("lblAllownceC").innerText;
    let DailyAllow = parseFloat(document.getElementById("lblTotalAllowances").innerText) || 0;
    let MiscAllow = parseFloat(document.getElementById("txtMiscAmout").value) || 0;
    let RequiredAmount = parseFloat(document.getElementById("txtRequiredAmount").value) || 0;
    let MiscRemarks = document.getElementById("txtMiscRemarks").value;

    let EmpPOS = "", EmpOperationID = "", AppAuthCode = "", AppAuthPOS = "", AppAuthEmailID = "", authType = "";
    var EmpCode = (document.getElementById("hdnUserId").value);//Fixes
    //let dt = await getAppAuthorities(0);//Fixes
    let dt = await getAppAuthorities(EmpCode);//Fixes
    AppAuthCode = document.getElementById("cboAppAuthority").value;

    if (dt && dt.length > 0) {
        EmpPOS = dt[0].POS; EmpOperationID = dt[0].ADVPID;

        let dtAuth = await getAppAuthorities(AppAuthCode);
        AppAuthPOS = dtAuth[0]?.POS; AppAuthEmailID = dtAuth[0]?.EMAILID;

        if (EmpPOS === "OTH" && AppAuthPOS === "SEC") authType = "REC";
        else if (EmpPOS === "OTH") authType = "APP";
        else if (EmpPOS === "SEC" && AppAuthPOS === "DPT") authType = "REC";
        else if (EmpPOS === "SEC") authType = "APP";
        else if (EmpPOS === "DPT" && AppAuthPOS === "DIV" && EmpOperationID === "3") authType = "REC";
        else authType = "APP";
        if (EmpPOS === "DIV" && AppAuthPOS === "DIV") authType = "REC";
        if (EmpPOS === "OTH" && parseInt(EmpCode) > 70000000) authType = "JPNDIR";
    }

    let Initiator_Status = "0"; // draft

    let TxnNumber = await insertTourDetail(
        MobileNo, ExtNo, Email, Objective, AdvRemarks,
        AdvRequired, APlusNights, ANights, BNights, CNights, StayCharge,
        APlusDays, ADays, BDays, CDays, DailyAllow, MiscAllow,
        authType, AppAuthCode, oTourList, MiscRemarks, RequiredAmount,
        TourStartDate, TourEndDate, oTourListprv, Initiator_Status, AppAuthEmailID
    );

    document.getElementById("lbltxnNumber").innerText = (TxnNumber || 0).toString();
    let lblMSG = document.getElementById("lblMSG");
    lblMSG.style.display = "block";

    if (TxnNumber > 0) {
        SAVESTATUS = "YES";
        lblMSG.innerText = "Request Successfully added with Request ID " + TxnNumber.toString();
        window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx";
    } else {
        lblMSG.innerText = "Error in Request Save As Draft...";
    }
}

// ------------------------------ Submit -----------------------------------------------------------
async function cmdSubmit_Click() {
    oTourList = oTourList || [];
    oTourListprv = oTourListprv || [];
    oday = oday || [];

    const saveStatus = (ViewState_SAVESTATUS || "").trim();
    if (saveStatus !== "NO") return;

    const userDetails = await getEmployeeOfficialDetails();
    const userSiteID = userDetails?.SYSITEID;

    const sitesDay = await getParameterValue();
    const siteDayArray = (sitesDay || "").split("\n");

    if (document.getElementById("cboAdvance").value === "YES") {
        for (let str of siteDayArray) {
            const [PDay, psite] = str.split("~");
            if (userSiteID === psite) {
                for (let vrownum = 0; vrownum < oday.length; vrownum++) {
                    const oTour = oday[vrownum];
                    const ODDAY = oTour.Day ? parseInt(oTour.Day) : 0;
                    if (ODDAY > parseInt(PDay)) {
                        showError(`Your Previous advance is overdue by ${oTour.Day} days. Submit your bill first.`);
                        highlightRow("gvAdvance", vrownum);
                        return;
                    }
                }
            }
        }
    }

    if (document.querySelectorAll("#gvList tr").length === 0) { showError("Please fill at least one tour detail."); return; }
    if (oday.length >= 3 && oTourListprv.length === 0) { showError("Please fill previous advance details."); return; }

    //const EmpCode = userId;
    const MobileNo = document.getElementById("txtMobile").value;
    const ExtNo = document.getElementById("txtExtension").value;
    const Email = document.getElementById("txtEmail").value;
    const Objective = document.getElementById("txtObjective").value;

    const AdvRequired = document.getElementById("cboAdvance").value === "YES" ? 1 : 0;
    const AdvRemarks = document.getElementById("txtAdvRemarks").value;

    const TourStartDate = `${getSelectedValue("OptDayStart")}-${getSelectedText("OptMonthStart")}-${getSelectedValue("OptYearStart")}`;
    const TourEndDate = `${getSelectedValue("OptDayEnd")}-${getSelectedText("OptMonthEnd")}-${getSelectedValue("OptYearEnd")}`;

    let APlusNights = document.getElementById("lblStayChargeAPLUS").innerText;
    let ANights = document.getElementById("lblStayChargeA").innerText;
    let BNights = document.getElementById("lblStayChargeB").innerText;
    let CNights = document.getElementById("lblStayChargeC").innerText;
    let StayCharge = parseFloat(document.getElementById("lblTotalCharges").innerText) || 0;

    let APlusDays = document.getElementById("lblAllownceAPLUS").innerText;
    let ADays = document.getElementById("lblAllownceA").innerText;
    let BDays = document.getElementById("lblAllownceB").innerText;
    let CDays = document.getElementById("lblAllownceC").innerText;
    let DailyAllow = parseFloat(document.getElementById("lblTotalAllowances").innerText) || 0;
    let MiscAllow = parseFloat(document.getElementById("txtMiscAmout").value) || 0;
    let RequiredAmount = parseFloat(document.getElementById("txtRequiredAmount").value) || 0;
    let MiscRemarks = document.getElementById("txtMiscRemarks").value;

    let EmpPOS = "", EmpOperationID = "", AppAuthCode = "", AppAuthPOS = "", AppAuthEmailID = "", authType = "";
    var EmpCode = (document.getElementById("hdnUserId").value);//Fix
    //let dt = await getAppAuthorities(0);
    let dt = await getAppAuthorities(EmpCode);//Fix
    AppAuthCode = document.getElementById("cboAppAuthority").value;

    if (dt && dt.length > 0) {
        EmpPOS = dt[0].POS; EmpOperationID = dt[0].ADVPID;

        let dtAuth = await getAppAuthorities(AppAuthCode);
        AppAuthPOS = dtAuth[0]?.POS; AppAuthEmailID = dtAuth[0]?.EMAILID;

        if (EmpPOS === "OTH" && AppAuthPOS === "SEC") authType = "REC";
        else if (EmpPOS === "OTH") authType = "APP";
        else if (EmpPOS === "SEC" && AppAuthPOS === "DPT") authType = "REC";
        else if (EmpPOS === "SEC") authType = "APP";
        else if (EmpPOS === "DPT" && AppAuthPOS === "DIV" && EmpOperationID === "3") authType = "REC";
        else authType = "APP";
        if (EmpPOS === "DIV" && AppAuthPOS === "DIV") authType = "REC";
        if (EmpPOS === "OTH" && parseInt(EmpCode) > 70000000) authType = "JPNDIR";
    }

    let Initiator_Status = "1"; // submit

    let TxnNumber = await insertTourDetail(
        MobileNo, ExtNo, Email, Objective, AdvRemarks,
        AdvRequired, APlusNights, ANights, BNights, CNights, StayCharge,
        APlusDays, ADays, BDays, CDays, DailyAllow, MiscAllow,
        authType, AppAuthCode, oTourList, MiscRemarks, RequiredAmount,
        TourStartDate, TourEndDate, oTourListprv, Initiator_Status, AppAuthEmailID
    );

    document.getElementById("lbltxnNumber").innerText = (TxnNumber || 0).toString();
    let lblMSG = document.getElementById("lblMSG");
    lblMSG.style.display = "block";

    if (TxnNumber > 0) {
        SAVESTATUS = "YES";
        lblMSG.innerText = "Request Successfully added with Request ID " + TxnNumber.toString();
        window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx";
    } else {
        lblMSG.innerText = "Error in Request Save As Draft...";
    }
}

// ------------------------------ Reset ------------------------------------------------------------
function cmdReset_Click() { window.location.href = '/TourRequest/TourRequest'; }

// ------------------------------ AJAX helpers -----------------------------------------------------
async function getEmployeeOfficialDetails() {
    return $.ajax({ url: '/TourRequest/GetEmployeeOfficialDetails', type: 'GET' })
        .then(response => response.result)
        .catch(error => { console.log(error.responseJSON?.message || 'An error occurred .'); return null; });
}
async function getParameterValue() {
    return $.ajax({ url: '/TourRequest/GetParameterValue', type: 'GET' })
        .then(response => (response.result && response.result._SitesDay) ? response.result._SitesDay : "")
        .catch(error => { console.log(error.responseJSON?.message || 'An error occurred .'); return ""; });
}
async function tourDayDetailsForAdvance(tourFromDate) {
    return $.ajax({ url: '/TourRequest/TourDayDetailsForAdvance', data: { TourFromDate: tourFromDate }, type: 'GET' })
        .then(response => response.result)
        .catch(error => { console.log(error.responseJSON?.message || 'An error occurred .'); return null; });
}
//async function getAppAuthorities(empCode) {
//    console.log(empCode);
//    return
//    $.ajax({
//        url: '/TourRequest/GetAppAuthorities',
//        data: { EmpCode: empCode },
//        type: 'GET'
//    })
//    .then(response => {
//        console.log('Response:', response);
//        return response.result;
//    })
//    .catch(error => { console.error(error.responseJSON?.message || 'An error occurred .'); return []; });
//} 
async function getAppAuthorities(empCode) {
    console.log(empCode);

    try {
        const response = await $.ajax({
            url: '/TourRequest/GetAppAuthorities',
            data: { EmpCode: empCode },
            type: 'GET'
        });

        console.log('Response:', response);
        return response.result;
    }
    catch (error) {
        console.log(error.responseJSON?.message || 'An error occurred.');
        return [];
    }
}

async function insertTourDetail(
    MobileNo, ExtNo, Email, Objective, AdvRemarks,
    AdvRequired, APlusNights, ANights, BNights, CNights, StayCharge,
    APlusDays, ADays, BDays, CDays, DailyAllow, MiscAllow,
    authType, AppAuthCode, oTourList, MiscRemarks, RequiredAmount,
    TourStartDate, TourEndDate, oTourListprv, Initiator_Status, AppAuthEmailID
) {
    try {
        const payload = {
            MobileNo, ExtNo, Email, Objective, AdvRemarks,
            AdvRequired, APlusNights, ANights, BNights, CNights, StayCharge,
            APlusDays, ADays, BDays, CDays, DailyAllow, MiscAllow,
            authType, AppAuthCode, oTourList, MiscRemarks, RequiredAmount,
            TourStartDate, TourEndDate, oTourListprv, Initiator_Status, AppAuthEmailID
        };

        const response = await $.ajax({
            url: '/TourRequest/InsertTourDetail',
            type: 'POST',
            data: JSON.stringify(payload),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json'
        });

        const resStatus =
            response?.result?.resStatus ??
            response?.resStatus ??
            response?.status ??
            response.result.TxnNumber ??
            0;

        return resStatus;
    } catch (xhr) {
        console.log(xhr.responseJSON?.message || 'An error occurred .');
        return 0;
    }
}

// ------------------------------ Misc helpers -----------------------------------------------------
function getMonthValue(monthText) {
    const months = { "Jan": "1", "Feb": "2", "Mar": "3", "Apr": "4", "May": "5", "Jun": "6", "Jul": "7", "Aug": "8", "Sep": "9", "Oct": "10", "Nov": "11", "Dec": "12" };
    return months[monthText] || monthText;
}
function validateNumber(input) {
    input.value = input.value.replace(/[^0-9.]/g, '');
    if (((input.value.match(/\./g) || []).length) > 1) { input.value = input.value.replace(/\.+$/, ''); }
}
function isValidDate(d) { return d instanceof Date && !isNaN(d); }

function updateDropdown(selector, items) {
    let dropdown = $(selector);
    dropdown.empty();

    items.forEach(item => {
        dropdown.append($("<option>", { value: item.value, text: item.text }));
    });

    const el = dropdown.get(0);
    if (el && el.options && el.options.length > 0) {
        if (el.selectedIndex < 0) el.selectedIndex = 0;
    }
}

function parseDate(dateStr) {
    let parts = dateStr.split("-");
    if (parts.length !== 3) return null;
    let day = parseInt(parts[0], 10), monthName = parts[1], year = parseInt(parts[2], 10);
    let monthIndex = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"].indexOf(monthName);
    if (monthIndex === -1) return null;
    return new Date(year, monthIndex, day);
}
function showError(message) { document.getElementById("errorpanel").style.display = "inline"; document.getElementById("status").innerText = message; return false; }
function highlightRow(tableId, rowIndex) { const el = document.querySelector(`#${tableId} tr:nth-child(${rowIndex + 1})`); if (el) el.style.backgroundColor = "red"; }
function getSelectedValue(id) {
    console.log(id);
    return document.getElementById(id).value;
}

function getSelectedText(id) {
    const el = document.getElementById(id);
    if (!el) return '';
    const idx = el.selectedIndex;
    if (idx == null || idx < 0) return '';
    const opt = el.options && el.options[idx];
    return opt ? (opt.text || '') : '';
}

function safeSelectedText(id) {
    const el = document.getElementById(id);
    if (!el || !el.options || el.options.length === 0) return '';
    const idx = el.selectedIndex >= 0 ? el.selectedIndex : 0;
    const opt = el.options[idx];
    return opt ? (opt.text || '') : '';
}