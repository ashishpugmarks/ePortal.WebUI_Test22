$('.homeContent').removeClass('homeContent');  
var ViewState_MODE = "";
var ViewState_EmpDesig = "";
var ViewState_AppDate = "";

var oTourListprv = [];
var oTourList = [];
var otourperiod = [];
var oTourListOrg = [];
var oday = [];

$(document).ready(function () {
    var cal1 = new CalendarPopup();
    InitializePage();
    //$('#BtnEdit').on("click", async function (e) {
    //    e.preventDefault();
    //    if (confirm('Are you sure to edit this record?')) {
    //        var id = $(this).data('id');
    //        const index = $(this).data("index");
    //        await gvList_RowEditing(index);
    //        //BtnEdit_Click();
    //    }
    //});


    $(document).on("click", "#btnEdit", async function (e) {
        e.preventDefault();
        const index = $(this).data("index");
        await gvList_RowEditing(index);
    });

    $('#cmdAddMore').on('click', function (e) {
        e.preventDefault();
        if (isValidEntry()) {
            cmdAddMore_Click();
        }
    });
    async function gvList_RowEditing(index) {
        debugger;
        const tour = oTourList[index];
        ViewState_EditRow = index;

        document.getElementById("txtDailyObjective").value = tour.TourObjective;
        var travelDate = tour.TourFromDate.split("-");
        var travelTime = tour.TourTime.split(":");
        var travelTimeTo = tour.TourTimeTo.split(":");

        document.getElementById("optDay").value = parseInt(travelDate[0]).toString();
        document.getElementById("optMonth").value = getMonthValue(travelDate[1]);
        document.getElementById("optYear").value = travelDate[2];

        document.getElementById("optHour").value = travelTime[0];
        document.getElementById("optMinute").value = travelTime[1];

        document.getElementById("optHourTo").value = travelTimeTo[0];
        document.getElementById("optMinuteTo").value = travelTimeTo[1];

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

        document.getElementById("cmbClass").value = tour.TicketClassID;
        document.getElementById("txtModeDetail").value = tour.ModeDetail;

        document.getElementById("cmbTicketingBy").value = tour.TicketingBy;
        document.getElementById("cmbHotelReserv").value = tour.HotelReserv;
        document.getElementById("preferredLocation").value = tour.PreferredLocation;

        document.getElementById("chkSpecialApp").checked = tour.SpecialApp === "1";
        document.getElementById("txtRemarks").value = tour.Remarks;

        document.getElementById("cmdAddMore").innerText = "Update";
        ViewState_MODE = "UPDATE";

        CalculateDayNights();
    }

    $('#btnSaveAsDraft').on('click', async function (e) {
        e.preventDefault();
        if (submitval()) {
            await btnSaveAsDraft_Click();
        }
    }); 

    $('#cmdSubmit').on('click', async function (e) {
        e.preventDefault();
        if (submitval()) {
            await cmdSubmit_Click();
        }
    });

    $('#cmdReset').on('click', function (e) {
        e.preventDefault();
        cmdReset_Click();
    });
    $('#txtexp').on('change', function (e) {
        e.preventDefault();
        txtexp_TextChanged();
    });

    $("#Img4").on("click", function (e) {
        e.preventDefault();
        cal1.select(document.getElementById("txtsubdate"), 'Img4', 'dd-NNN-yyyy');
        return false;
    });
    $("#Img3").on("click", function (e) {
        e.preventDefault();  
        cal1.select(document.getElementById("txtchqdate"), 'Img3', 'dd-NNN-yyyy');
        return false;
    });

    $('#txtexp').on('change', function (e) {
        e.preventDefault();
        txtexp_TextChanged();
    });

    $('#txtMiscAmout')
        .on('input', function () {
            // allow only digits and one dot (FilteredTextBoxExtender behavior)
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

    $('#txtMiscAmout').on('change', function (e) {
        ReqAmountCHK();
    });

    $("#optDayStart, #optMonthStart, #optYearStart").on("change", function () {
        dateChanged();
    });

    $("#optDayEnd, #optMonthEnd, #optYearEnd").on("change", function () {
        endcclick();
    });

    $("#optDay, #optMonth, #optYear").on("change", function () {
        cclick();
        TravelDateChanged();
    });

    $('#chkAdvance').on('change', function (e) {
        e.preventDefault();
        chkAdvance_CheckedChanged();
    });
    $('#cmbHotelReserv').on('change', function () {
        document.getElementById("cmbHotelReserv").focus();
    });
    $("#cmbFrom").on("change", function () {
        cmbFrom_SelectedIndexChanged();
    });
    $("#cmbTo").on("change", function () {
        cmbTo_SelectedIndexChanged();
    });
    $("#cmbStaying").on("change", function () {
        cmbStaying_SelectedIndexChanged();
    });
    $("#cmbMode").on("change", async function () {
       await cmbMode_SelectedIndexChanged();
    });
    $("#cmbHotelReserv").on("change", function () {
        SetPickDrop();
        cmbHotelReserv_SelectedIndexChanged();
    });


    $('#txtRemarks')
        .on('input', function () {
            restrictSpecialChars(this);
        })
        .on('paste', function (e) {
            restrictSpecialChars(this, e);
        });

    $("#BtnAdd").on("click", function (e) {
        e.preventDefault();
        BtnAdd_Click();
    });
});

function InitializePage() {
    ViewState_AppDate = AppDate;
    ViewState_MODE = MODE;
    ViewState_EmpDesig = empDesig;

    oTourList = oTourList;
    oTourListprv = oTourListprv;
    otourperiod = otourperiod;
    oTourListOrg = oTourListOrg;
    oday = oday;


    $('#lblMSG').hide().attr('aria-hidden', 'true');
    $('#txtFromCity, #txtToCity, #txtStayingCity')
        .prop('disabled', true)
        .addClass('is-disabled')
        .attr('aria-disabled', 'true');
    $('#errorpanel').css('display', 'none');


    if ($('#cmbStaying').val() === "0") {
        $('#txt_GST').hide();
        $('#txtNo').hide();
    } else {
        $('#txt_GST').show();
        $('#txtNo').show();
    }
} 

function checkEmail() {
    if (document.getElementById("txtEmail").value == "")
        return true;

    if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(document.getElementById("txtEmail").value))
        return true;
    else
        return false;
} 

function submitval() {
    //@*<% --Added By Aumento For SR73072 Start--%>*@
    var fromdate = new Date(document.getElementById("optYearStart").value, (document.getElementById("optMonthStart").value - 1), document.getElementById("optDayStart").value);
    var todate = new Date(document.getElementById("optYearEnd").value, (document.getElementById("optMonthEnd").value - 1), document.getElementById("optDayEnd").value);

    if (fromdate > todate) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Tour Start Date should be less than equal to Tour End Date.";
        document.getElementById("optDayStart").focus();
        return false
    }
    //@*<%--Added By Aumento For SR73072 End--%>*@
    if (document.getElementById("txtMobile").value == "") {
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

    if (document.getElementById("txtObjective").value == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Ojective of Journey is a required field.";
        document.getElementById("txtObjective").focus();
        return false
    }

    if ((document.getElementById("txtMiscAmout").value != "" && document.getElementById("txtMiscAmout").value != "0") && document.getElementById("txtMiscRemarks").value == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Remarks for Miscellaneous Amount is required if amount filled by associate.";
        document.getElementById("txtMiscRemarks").focus();
        return false
    }
    return true;
}

function isValidEntry() {
    //@*<% --Added By Aumento For SR73072 Start--%>*@
    var fromdate = new Date(document.getElementById("optYearStart").value, (document.getElementById("optMonthStart").value - 1), document.getElementById("optDayStart").value);
    var todate = new Date(document.getElementById("optYearEnd").value, (document.getElementById("optMonthEnd").value - 1), document.getElementById("optDayEnd").value);
    var traveldate = new Date(document.getElementById("optYear").value, (document.getElementById("optMonth").value - 1), document.getElementById("optDay").value);
    var currdate = new Date();
    var today = new Date(currdate.getFullYear(), currdate.getMonth(), currdate.getDate());

    if (fromdate > todate) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Tour Start Date should be less than equal to Tour End Date.";
        document.getElementById("optDayStart").focus();
        return false
    }
    //TOUR DATE SHOULD IN TOUR PERIOD
    if (traveldate < fromdate || traveldate > todate) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Travel Date should be in Tour Period.";
        document.getElementById("optDay").focus();
        return false
    }
    //@*<%--Added By Aumento For SR73072 End--%>*@

    if (document.getElementById("txtDailyObjective").value == "") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Day objective is required field";

        document.getElementById("txtDailyObjective").focus();
        return false
    }

    if ((document.getElementById("cmbFrom").value == "0") || (document.getElementById("cmbFrom").value == "" && document.getElementById("txtFromCity").value == "")) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "From City is required field";

        document.getElementById("cmbFrom").focus();
        return false
    }

    if ((document.getElementById("cmbTo").value == "0") || (document.getElementById("cmbTo").value == "" && document.getElementById("txtToCity").value == "")) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "To City is required field";
        document.getElementById("cmbTo").focus();
        return false
    }


    //TRAVEL MODE REQUIRE WHEN FROM TO CITY ARE NOT SAME
    if (document.getElementById("cmbFrom").value != document.getElementById("cmbTo").value) {
        if (document.getElementById("cmbMode").value == "") {
            document.getElementById("errorpanel").style.display = "inline";
            document.getElementById("status").innerHTML = "Travel Mode is required field.";
            document.getElementById("cmbMode").focus();
            return false
        }

        //CHECK FOR TICKET CLASS IN CASE OF TRAIN AND FLIGHT(1=AIR,3=TRAIN
        if ((document.getElementById("cmbMode").value == "1" || document.getElementById("cmbMode").value == "3") && document.getElementById("cmbClass").value == "") {
            //alert("Please Enter a Title")
            document.getElementById("errorpanel").style.display = "inline";
            document.getElementById("status").innerHTML = "Travel class is required field for selected travel mode.";
            document.getElementById("cmbClass").focus();
            return false
        }

        if ((document.getElementById("cmbMode").value == "1" || document.getElementById("cmbMode").value == "3") && document.getElementById("txtModeDetail").value == "") {
            //alert("Please Enter a Title")
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

    if (document.getElementById("chkSpecialApp").checked == true && document.getElementById("txtRemarks").value == "") {
        //alert("Please Enter a Title")
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerHTML = "Remarks is Required in Case of Special Approval.";
        document.getElementById("txtRemarks").focus();
        return false
    }

    return true;
}

function isValidDate(d) {
    return d instanceof Date && !isNaN(d);
}
function updateDropdown(selector, items) {
    let dropdown = $(selector);
    dropdown.empty();
    items.forEach(item => {
        dropdown.append($("<option>", { value: item.value, text: item.text }));
    });
}

//FROM DATE VALIDATION
function cclick() {
    for (var i = 0; i < document.getElementById("optMonth").options.length; i++) {
        if (document.getElementById("optMonth").options[i].selected == true) {
            if (document.getElementById("optMonth").options[i].value == 2) {
                for (var j = 0; j < document.getElementById("optDay").options.length; j++) {
                    if (document.getElementById("optDay").options[j].selected == true) {
                        if (document.getElementById("optDay").options[j].value > 28) {
                            if (document.getElementById("optDay").options[j].value == 29 & (document.getElementById("optYear").options.value == 2008 || document.getElementById("optYear").options.value == 2012 || document.getElementById("optYear").options.value == 2016 || document.getElementById("optYear").options.value == 2020 || document.getElementById("optYear").options.value == 2024 || document.getElementById("optYear").options.value == 2028)) {
                            }
                            else {
                                //alert("Invalid date!")
                                document.getElementById("errorpanel").style.display = "inline";
                                document.getElementById("status").innerHTML = "Invalid date! please select valid date.";
                                document.getElementById("optDay").options[27].selected = true
                            }
                        }
                    }
                }
            }
            if (document.getElementById("optMonth").options[i].value == 4 || document.getElementById("optMonth").options[i].value == 6 || document.getElementById("optMonth").options[i].value == 9 || document.getElementById("optMonth").options[i].value == 11) {
                for (var j = 0; j < document.getElementById("optDay").options.length; j++) {
                    if (document.getElementById("optDay").options[j].selected == true) {
                        if (document.getElementById("optDay").options[j].value > 30) {
                            document.getElementById("errorpanel").style.display = "inline";
                            document.getElementById("status").innerHTML = "Invalid date! please select valid date.";
                            document.getElementById("optDay").options[29].selected = true
                        }
                    }
                }
            }
        }
    }
}

//@*<%--Added By Aumento For SR73072 Start--%>*@
//TOUR START DATE VALIDATION
function startcclick() {
    for (var i = 0; i < document.getElementById("optMonthStart").options.length; i++) {
        if (document.getElementById("optMonthStart").options[i].selected == true) {
            if (document.getElementById("optMonthStart").options[i].value == 2) {
                for (var j = 0; j < document.getElementById("optDayStart").options.length; j++) {
                    if (document.getElementById("optDayStart").options[j].selected == true) {
                        if (document.getElementById("optDayStart").options[j].value > 28) {
                            if (document.getElementById("optDayStart").options[j].value == 29 & (document.getElementById("optYearStart").options.value == 2008 || document.getElementById("optYearStart").options.value == 2012 || document.getElementById("optYearStart").options.value == 2016 || document.getElementById("optYearStart").options.value == 2020 || document.getElementById("optYearStart").options.value == 2024 || document.getElementById("optYearStart").options.value == 2028)) {
                            }
                            else {
                                //alert("Invalid date!")
                                document.getElementById("optDayStart").options[27].selected = true
                            }
                        }
                    }
                }
            }
            if (document.getElementById("optMonthStart").options[i].value == 4 || document.getElementById("optMonthStart").options[i].value == 6 || document.getElementById("optMonthStart").options[i].value == 9 || document.getElementById("optMonthStart").options[i].value == 11) {
                for (var j = 0; j < document.getElementById("optDayStart").options.length; j++) {
                    if (document.getElementById("optDayStart").options[j].selected == true) {
                        if (document.getElementById("optDayStart").options[j].value > 30) {
                            document.getElementById("optDayStart").options[29].selected = true
                        }
                    }
                }
            }
        }
    }
}

//TOURN END DATE VALIDATION
function endcclick() {
    for (var i = 0; i < document.getElementById("optMonthEnd").options.length; i++) {
        if (document.getElementById("optMonthEnd").options[i].selected == true) {
            if (document.getElementById("optMonthEnd").options[i].value == 2) {
                for (var j = 0; j < document.getElementById("optDayEnd").options.length; j++) {
                    if (document.getElementById("optDayEnd").options[j].selected == true) {
                        if (document.getElementById("optDayEnd").options[j].value > 28) {
                            if (document.getElementById("optDayEnd").options[j].value == 29 & (document.getElementById("optYearEnd").options.value == 2008 || document.getElementById("optYearEnd").options.value == 2012 || document.getElementById("optYearEnd").options.value == 2016 || document.getElementById("optYearEnd").options.value == 2020 || document.getElementById("optYearEnd").options.value == 2024 || document.getElementById("optYearEnd").options.value == 2028)) {
                            }
                            else {
                                //alert("Invalid date!")
                                document.getElementById("optDayEnd").options[27].selected = true
                            }
                        }
                    }
                }
            }
            if (document.getElementById("optMonthEnd").options[i].value == 4 || document.getElementById("optMonthEnd").options[i].value == 6 || document.getElementById("optMonthEnd").options[i].value == 9 || document.getElementById("optMonthEnd").options[i].value == 11) {
                for (var j = 0; j < document.getElementById("optDayEnd").options.length; j++) {
                    if (document.getElementById("optDayEnd").options[j].selected == true) {
                        if (document.getElementById("optDayEnd").options[j].value > 30) {
                            document.getElementById("optDayEnd").options[29].selected = true
                        }
                    }
                }
            }
        }
    }
}

//CALCULATE TOTAL AMOUNT INCLUDING MISC AMOUNT
function CalculateAmount() {
    var MiscAmount = 0;
    var StayCharge = 0;
    var Allowances = 0;
    var TotalAmount = 0;
    if (document.getElementById("txtMiscAmout").value != "")
        MiscAmount = document.getElementById("txtMiscAmout").value;

    StayCharge = document.getElementById("lblTotalCharges").innerHTML;
    Allowances = document.getElementById("lblTotalAllowances").innerHTML;
    //alert(StayCharge)
    TotalAmount = parseFloat(StayCharge) + parseFloat(Allowances) + parseFloat(MiscAmount);
    document.getElementById("lblTotalAmount").innerHTML = TotalAmount.toFixed(2);
    document.getElementById("txtRequiredAmount").value = TotalAmount.toFixed(2);
}

//CHECK REQUIRED AMOUNT IS LESS THAN EQUAL TO SYSTEM GENERATED AMOUNT
function ReqAmountCHK() {
    var TotalAmount = parseFloat(document.getElementById("lblTotalAmount").innerHTML || "0");
    var RequiredAmount = parseFloat(document.getElementById("txtRequiredAmount").value || "0");
    if (RequiredAmount > TotalAmount) {
        document.getElementById("txtRequiredAmount").value = TotalAmount.toFixed(2);
        document.getElementById("txtRequiredAmount").focus();
        alert('Required amount should be less than equal to System generated amount');
        return;
    }
}
//@*<%--Added By Aumento For SR73072 End--%>*@


//MAKE TEXT BOX ENABLE INCASE OF OTHER CITY(STAYING CITY)
function SetStayCity() {
    // alert($('#cmbStaying').val());
    if (document.getElementById("cmbStaying").value == "") {
        document.getElementById("txtStayingCity").disabled = false;
        document.getElementById("txtStayingCity").focus();
    }
    else {
        document.getElementById("txtStayingCity").disabled = true;
        document.getElementById("txtStayingCity").value = "";
        document.getElementById("cmbStaying").focus();

    }
}


//@*<%--  --- Added by Aumento :: SR78268 --%>*@
function restrictSpecialChars(textBox) {
    var pattern = /[<,>`]/g; // Matches <, ,, >, and `
    textBox.value = textBox.value.replace(pattern, '');
}
//@*<%--  --- Added by Aumento :: SR78268--%>*@



async function FillTravelClass(modeID) {
    $.ajax({
        url: '/TourRequest/FillTravelClass',
        type: 'GET',
        data: { ModeID: modeID },
        dataType: 'json',
        success: function (response) {

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

            // Refresh SwiftSelect if needed
            ddl.trigger('change'); // or ddl.swiftselect('refresh');
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseJSON?.message || 'An error occurred.');
        }
    });
}


function dateChanged() {
    // Prepare date parts
    let strDay = ("00" + document.getElementById("optDayStart").value).slice(-2);
    let strMonth = document.getElementById("optMonthStart").value;
    let strYear = document.getElementById("optYearStart").value;

    let tourStartDateStr = strDay + "-" + strMonth + "-" + strYear;

    // Validate date
    let startDate = new Date(tourStartDateStr);
    if (!isValidDate(startDate)) {
        // Show error
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerText = "Please select valid Date!";
        return;
    }

    // Compare with today
    let today = new Date();
    if (today > startDate) {
        document.getElementById("chkAdvance").value = "NO";
        document.getElementById("chkAdvance").disabled = true;
        document.getElementById("cmbTicketingBy").value = "0";
        document.getElementById("cmbTicketingBy").disabled = true;
    } else {
        document.getElementById("chkAdvance").disabled = false;
        document.getElementById("cmbTicketingBy").disabled = false;
    }
}


function BtnAdd_Click() {
    // Get footer controls
    var ddltourperiod = document.getElementById("ddltourperiod");
    var txtadvance = document.getElementById("txtadvance");
    var txtexp = document.getElementById("txtexp");
    var txtbalance = document.getElementById("txtbalance");
    var txtsubdate = document.getElementById("txtsubdate");
    var ddlrefundby = document.getElementById("ddlrefundby");
    var txtchqno = document.getElementById("txtchqno");
    var txtchqdate = document.getElementById("txtchqdate");
    var txtrefamount = document.getElementById("txtrefamount");

    // Validation
    if (ddltourperiod.value === "0") {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerText = "Select Tour Period";
        ddltourperiod.focus();
        return;
    }

    // Collect values
    var Tourperiod = ddltourperiod.options[ddltourperiod.selectedIndex].text;
    var Advance = ddltourperiod.value;
    var Exp = txtexp.value;
    var Balance = txtbalance.value;
    var Subdate = txtsubdate.value;
    var Refundby = ddlrefundby.value;
    var Chqno = txtchqno.value;
    var Chqdate = txtchqdate.value;
    var Refamount = txtrefamount.value;

    var refundbydes = "";
    if (Refundby === "1") refundbydes = "Cheque";
    else if (Refundby === "2") refundbydes = "Cash";

    // Create object
    var oTour = {
        Tourperiod: Tourperiod,
        Advance: Advance,
        Exp: Exp,
        Balance: Balance,
        Subdate: Subdate,
        Refundby: Refundby,
        Chqno: Chqno,
        Chqdate: Chqdate,
        Refamount: Refamount,
        Refundbydes: refundbydes
    };

    // Check mode (simulate ViewState)
    var mode = ViewState_MODE;
    if (mode === "ADD") {
        oTourListprv.push(oTour);
    } else {
        var editIndex = ViewBag_PreEditRow;
        oTourListprv[editIndex] = oTour;
        ViewState_MODE = "ADD";
        document.getElementById("cmdAddMore").innerText = "Add More";
        ViewState_EditRow = "";
    }

    // Update grid
    renderGridAdd(oTourListprv);
   // TourPeriodDropdown();
}
function renderGridAdd(list) {
    var grid = document.getElementById("grdprvadv");
    list.forEach(function (item, index) {
        var row = `<tr class="row${index+1}" style="color:#333333;background-color:#F7F6F3;">
                <td align="left" valign="top" style="width:10%;">${item.Tourperiod}</td>
                <td align="left" valign="top" style="width:10%;">${item.Advance}</td>
                <td align="left" valign="top" style="width:10%;">${item.Exp}</td>
                <td align="left" valign="top" style="width:10%;">${item.Balance}</td>
                <td align="left" valign="top" style="width:10%;">${item.Subdate}</td>
                <td align="left" valign="top" style="width:10%;">${item.Refundbydes}</td>
                <td align="left" valign="top" style="width:10%;">${item.Chqno}</td>
                <td align="left" valign="top" style="width:10%;">${item.Chqdate}</td>
                <td align="left" valign="top" style="width:10%;">${item.Refamount}</td>
            </tr>`;
        grid.innerHTML += row;
    });
}

function TourPeriodDropdown() {
    fetch('/TourRequest/TourPerioddropdown')
        .then(response => response.json())
        .then(data => {
            const ddl = data.result.result;

            const dropdown = document.querySelector("#grdprvadv tfoot #ddltourperiod");
            if (dropdown) {
                dropdown.innerHTML = "";

                const defaultOption = document.createElement("option");
                defaultOption.value = "0";
                defaultOption.textContent = "-Select Period-";
                dropdown.appendChild(defaultOption);
                ddl.forEach(item => {
                    const option = document.createElement("option");
                    option.value = item.amount;
                    option.textContent = item.tourperiod;
                    dropdown.appendChild(option);
                });
            }

        })
        .catch(error => console.log('Error fetching TourPeriod:', error));
}

function TravelDateChanged() {
    // Get selected values
    let mode = $("#cmbMode").val(); // 2 = Bus
    let day = ("00" + $("#optDay").val()).slice(-2);
    let month = $("#optMonth option:selected").text();
    let year = $("#optYear").val();

    let dateStr = `${day}-${month}-${year}`;

    // Parse date in dd-MMM-yyyy format
    let startDate = parseDate(dateStr);
    if (!startDate) {
        alert("Invalid date!");
        return;
    }

    let today = new Date();

    // Clear and update dropdowns based on date comparison
    if (startDate < today) {
        // Self
        updateDropdown("#cmbHotelReserv", [{ text: "Self", value: "0" }]);
        updateDropdown("#cmbTicketingBy", [{ text: "Self", value: "0" }]);
    } else {
        // Admin
        updateDropdown("#cmbHotelReserv", [{ text: "Admin", value: "1" }]);
        updateDropdown("#cmbTicketingBy", [{ text: "Admin", value: "1" }]);
    }

    // If mode = Bus (2), add both options
    if (mode === "2") {
        updateDropdown("#cmbTicketingBy", [
            { text: "Self", value: "0" },
            { text: "Admin", value: "1" }
        ]);
    }

    // Focus on Add More button
    //$("#cmdAddMore").focus();
    document.getElementById("cmdAddMore").focus();
}
function cmbFrom_SelectedIndexChanged() {
    let selectedValue = $("#cmbFrom").val();

    if (selectedValue === "") {
        //$("#txtFromCity").prop("disabled", false).focus();
        document.getElementById("txtFromCity").disabled = false;
        document.getElementById("txtFromCity").focus();
    } else {
        $("#txtFromCity").val("").prop("disabled", true);
        //$("#cmbFrom").focus();
        document.getElementById("cmbFrom").focus();
    }
    //$("#cmdAddMore").focus();
    document.getElementById("cmdAddMore").focus();
}
function cmbTo_SelectedIndexChanged() {
    let selectedValue = $("#cmbTo").val();

    if (selectedValue === "") {
        //$("#txtToCity").prop("disabled", false).focus();
        document.getElementById("txtToCity").disabled = false;
        document.getElementById("txtToCity").focus();
    } else {
        $("#txtToCity").val("").prop("disabled", true);
        //$("#cmbTo").focus();
        document.getElementById("cmbTo").focus();
    }
    //$("#cmdAddMore").focus();
    document.getElementById("cmdAddMore").focus();
}
function cmbStaying_SelectedIndexChanged() {
    let cityId = $("#cmbStaying").val();
    fillGSTINNOClass(cityId);
    //$("#cmbStaying").focus();
    document.getElementById("cmbStaying").focus();

    if (cityId === "" || cityId === "0") { // Updated by Aumento as on 28022024
        $("#txt_GST").val("-");
    }

    // IN CASE OF OTHER (empty value)
    if (cityId === "") {
        //$("#txtStayingCity").prop("disabled", false).focus();
        document.getElementById("txtStayingCity").disabled = false;
        document.getElementById("txtStayingCity").focus();
    } else {
        $("#txtStayingCity").val("").prop("disabled", true);
        //$("#cmbStaying").focus();
        document.getElementById("cmbStaying").focus();
    }
    //$("#cmdAddMore").focus();
    document.getElementById("cmdAddMore").focus();
}
function fillGSTINNOClass(cityId) {
    $.ajax({
        url: '/TourRequest/FillGSTINNOClass',
        type: 'GET',
        data: { CityId: cityId },
        success: function (response) {
            var result = response.result;
            if (result) {
                $("#txt_GST").text(result.txt_GST_Text);
            }
        },
        complete: function () {
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseJSON?.message || 'An error occurred .');
        }
    });
}
async function cmbMode_SelectedIndexChanged() {
    debugger
    let cmbMode = document.getElementById('cmbMode');
    let ModeID = cmbMode.value;

    await FillTravelClass(ModeID);

    cmbMode.focus();

    let tr_train = document.getElementById('tr_train');
    if (ModeID === "3") {
        tr_train.style.display = 'block';
    } else {
        tr_train.style.display = 'none';
    }

    if (ModeID === "2") {
        let cmbTicketingBy = document.getElementById('cmbTicketingBy');
        cmbTicketingBy.innerHTML = '';
        let liSlf = new Option("Self", "0");
        let liAdm = new Option("Admin", "1");
        cmbTicketingBy.add(liSlf);
        cmbTicketingBy.add(liAdm);
        return;
    }
    document.getElementById('cmdAddMore').focus();
}
async function FillTravelClass(modeID) {
    $.ajax({
        url: '/TourRequest/FillTravelClass',
        type: 'GET',
        data: { ModeID: modeID },
        dataType: 'json',
        success: function (response) {
            //console.log(response.result);

            var ddl = $('#cmbClass');  // Matches rendered HTML
            ddl.empty();

            if (response.result && response.result.length > 0) {
                $.each(response.result, function (i, item) {
                    ddl.append($('<option>', {
                        value: item.Value,      // Correct property name
                        text: item.Text,        // Correct property name
                        selected: item.Selected, // Optional
                        disabled: item.Disabled  // Optional
                    }));
                });
            }

            // Refresh SwiftSelect if needed
            ddl.trigger('change'); // or ddl.swiftselect('refresh');
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseJSON?.message || 'An error occurred.');
        }
    });
}
function cmbHotelReserv_SelectedIndexChanged(selectedValue, gvList_length) {
    document.getElementById('cmbHotelReserv').focus();
}
function onExpenseChange() {
    var advance = parseFloat(document.getElementById("txtadvance").value) || 0;
    var expense = parseFloat(document.getElementById("txtexp").value) || 0;

    var balance = advance - expense;

    document.getElementById("txtbalance").value = balance.toFixed(2);

    // Elements to toggle
    var refundFields = [
        "ddlrefundby",
        "txtchqno",
        "txtchqdate",
        "txtrefamount"
    ];

    // Show/Hide based on balance
    refundFields.forEach(function (id) {
        document.getElementById(id).style.display = (balance < 0) ? "none" : "block";
    });
}


//---------------cmdAddMore_Click : Start--------------
function cmdAddMore_Click() {
    const showError = (msg) => {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerText = msg;
        return false;
    };

    // Collect values
    const tourObjective = document.getElementById("txtDailyObjective").value.trim();
    const tourStartDate = new Date(document.getElementById("optYearStart").value, document.getElementById("optMonthStart").value - 1, document.getElementById("optDayStart").value);
    const tourEndDate = new Date(document.getElementById("optYearEnd").value, document.getElementById("optMonthEnd").value - 1, document.getElementById("optDayEnd").value);
    const tourFromDate = new Date(document.getElementById("optYear").value, document.getElementById("optMonth").value - 1, document.getElementById("optDay").value);

    const fromLocCode = document.getElementById("cmbFrom").value;
    const toLocCode = document.getElementById("cmbTo").value;
    const stayingLocCode = document.getElementById("cmbStaying").value;
    const modeID = document.getElementById("cmbMode").value;
    const strIdType = document.getElementById("ddlidtype").value;
    const strIdNo = document.getElementById("txtidcardno").value.trim();
    const fromOtherCity = document.getElementById("txtFromCity").value.trim();
    const toOtherCity = document.getElementById("txtToCity").value.trim();

    // Validation
    if (!tourObjective) return showError("Please Enter Day Objective.");
    if (!(tourFromDate >= tourStartDate && tourFromDate <= tourEndDate)) return showError("Please select a travel date between the tour start date and the tour end date.");
    if (fromLocCode === "0") return showError("Please select City From.");
    if (toLocCode === "0") return showError("Please select City To.");
    if (fromLocCode === "" && fromOtherCity === "") return showError("Please Enter Other cities from");
    if (toLocCode === "" && toOtherCity === "") return showError("Please Enter Other cities to");
    if (modeID === "3" && strIdType === "0" && strIdNo === "") return showError("Please Fill ID Card Type and No");

    // Calculate ValidDays for special approval
    const today = new Date();
    const validDays = Math.floor((tourFromDate - today) / (1000 * 60 * 60 * 24));
    let specialApp = document.getElementById("chkSpecialApp").checked ? "1" : "0";
    let specialAppDesc = document.getElementById("chkSpecialApp").checked ? "Yes" : "No";

    if (validDays <= 3) {
        const userId = parseInt(document.getElementById("hdnUserId").value);
        if (userId > 70000000) {
            document.getElementById("chkSpecialApp").disabled = true;
            specialApp = "0";
            specialAppDesc = "No";
        } else {
            specialApp = "1";
            specialAppDesc = "Yes";
        }
    }

    // Staying charge validation
    if ((fromLocCode !== "0" && fromLocCode === toLocCode && stayingLocCode !== "0") ||
        (fromLocCode === "" && toLocCode === "" && stayingLocCode !== "0" && fromOtherCity === toOtherCity)) {
        alert("Staying charges not applicable at the time of return from the tour");
        document.getElementById("cmbStaying").focus();
        return false;
    }

    // Ticket Class handling
    const ticketClassValue = document.getElementById("cmbClass").value;
    const ticketClassText = (ticketClassValue === "" || ticketClassValue === "0") ? "" : document.getElementById("cmbClass").selectedOptions[0].text;
    const monthText = document.querySelector("#optMonth option:checked").textContent;
    // Prepare tourData object
    const tourData = {
        TourObjective: tourObjective,
        TourStartDate: `${document.getElementById("optDayStart").value}-${document.getElementById("optMonthStart").value}-${document.getElementById("optYearStart").value}`,
        TourEndDate: `${document.getElementById("optDayEnd").value}-${document.getElementById("optMonthEnd").value}-${document.getElementById("optYearEnd").value}`,
        TourFromDate: `${document.getElementById("optDay").value}-${monthText}-${document.getElementById("optYear").value}`, 
        TourTime: `${document.getElementById("optHour").value}:${document.getElementById("optMinute").value}`,
        TourTimeTo: `${document.getElementById("optHourTo").value}:${document.getElementById("optMinuteTo").value}`,
        FromLocCode: fromLocCode,
        ToLocCode: toLocCode,
        StayingLocCode: stayingLocCode,
        FromLoc: fromLocCode === "" ? fromOtherCity : document.getElementById("cmbFrom").selectedOptions[0].text,
        ToLoc: toLocCode === "" ? toOtherCity : document.getElementById("cmbTo").selectedOptions[0].text,
        StayingLoc: (stayingLocCode === "" || stayingLocCode === "0") ? document.getElementById("txtStayingCity").value.trim() : document.getElementById("cmbStaying").selectedOptions[0].text,
        ModeID: modeID,
        Mode: (document.getElementById("cmbMode").value === "0" || document.getElementById("cmbMode").value === "") ? "" : document.getElementById("cmbMode").selectedOptions[0].text,
        ModeDetail: document.getElementById("txtModeDetail").value.trim(),
        TicketClassID: ticketClassValue,
        TicketClass: ticketClassText,
        TicketingBy: document.getElementById("cmbTicketingBy").value,
        TicketingByDesc: document.getElementById("cmbTicketingBy").selectedOptions[0].text,
        HotelReserv: document.getElementById("cmbHotelReserv").value,
        PickDrop: "0",
        SpecialApp: specialApp,
        SpecialAppDesc: specialAppDesc,
        Remarks: document.getElementById("txtRemarks").value.trim(),
        IDType: strIdType,
        IDNo: strIdNo,
        PreferredLocation: document.getElementById("preferredLocation").value.trim()
    };
    updateTourList(tourData);
}
function updateTourList(oTour) {
    if (ViewState_MODE === "ADD") {
        oTourList.push(oTour);
    } else {
        if (ViewState_EditRow !== null && ViewState_EditRow >= 0 && ViewState_EditRow < oTourList.length) {
            oTourList[ViewState_EditRow] = oTour;
        }
        ViewState_MODE = "ADD";
        document.getElementById("cmdAddMore").innerText = "Add More";
        editIndex = null;
    }
    bind_gvListGrid(oTourList);
    clearControls();
    CalculateDayNights();
    document.getElementById("tr_train").style.display = "none";
    document.getElementById("ddlidtype").value = "0";
    document.getElementById("txtidcardno").value = "";
    // console.log("oTourList unsder updateTourList() : " + JSON.stringify(oTourList));
}

async function CalculateDayNights() {
    let chargeAP = 0, chargeA = 0, chargeB = 0, chargeC = 0;
    let allowAP = 0, allowA = 0, allowB = 0, allowC = 0;
    let TotalDailyAllowance = 0, TotalNightCharge = 0;

    let chkAdvance = document.getElementById('chkAdvance').value;
    let gvListRows = document.querySelectorAll('#gvList tbody tr');

    if (chkAdvance === "YES" && gvListRows.length > 0 && gvListRows[0].cells[0].innerText !== "No Data Found") {
        document.getElementById('pnlAdvance').style.display = 'block';

        let ProcessDates = [];
        let ProcessNights = [];

        for (const oTour of oTourList) {
            let days = 1, nights = 1;
            let TravelDate = oTour.TourFromDate;
            let StayingCity = oTour.StayingLoc.trim();
            let CityCode = oTour.StayingLocCode;

            // Check if date already processed
            isDayCalculated = DailyAllowanceCaluculation(ProcessDates, TravelDate);
            if (ProcessDates.includes(TravelDate)) {
                days = 0;
            } else {
                ProcessDates.push(TravelDate);
            }

            // Check if night already processed
            isNightCalculated = StayChargeCaluculation(ProcessNights, TravelDate, StayingCity)
            if (ProcessNights.includes(TravelDate)) {
                nights = 0;
            } else if (StayingCity !== "") {
                ProcessNights.push(TravelDate);
            }

            if (StayingCity === "") {
                nights = 0;
                CityCode = oTour.ToLocCode;
            }
            //console.log("CityCode -> oTour.ToLocCode : " + CityCode);

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
    for (let str of processDates) {
        // PROCESSED
        if (str === travelDate) {
            return true;
        }
    }
    // NOT PROCESSED
    return false;
}
function StayChargeCaluculation(processNights, travelDate, stayingCity) {
    for (let str of processNights) {
        // PROCESSED AND STAYING CITY EXIST
        if (str === travelDate && stayingCity.trim() !== "") {
            return true;
        }
    }
    // NOT PROCESSED
    return false;
}

async function GetCityCategory(cityCode) {
    return $.ajax({
        url: '/TourRequest/GetCityCategory',
        type: 'GET',
        data: { CityCode: cityCode }
    }).then(function (response) {
        return response.result;
    }).catch(function (error) {
        console.log(error.responseJSON?.message || 'An error occurred.');
        return null;
    });
}
async function GetEmployeeAllowanceDetail(cityCategoryCode, empDesignationID, currDate, userType) {
    return $.ajax({
        url: '/TourRequest/GetEmployeeAllowanceDetail',
        type: 'GET',
        data: { CityCategoryCode: cityCategoryCode, EmpDesignationID: empDesignationID, CurrDate: currDate, UserType: userType }
    }).then(function (response) {
        return response.result;
    }).catch(function (error) {
        console.log(error.responseJSON?.message || 'An error occurred.');
        return null;
    });
}

function bind_gvListGrid(list) {
  //console.log("oTourList under bind_gvListGrid() : " + JSON.stringify(list));
    const gridBody = document.querySelector("#gvList tbody");
    if (!gridBody) {
        console.log("Grid element not found!");
        return;
    }
    gridBody.innerHTML = ""; // Clear existing rows
    list.forEach((tour, index) => {
        const row = document.createElement("tr");
        row.className = "row" + index;
        row.style.color = "#333333";
        row.style.backgroundColor = "#F7F6F3";
        row.innerHTML = `
            <td align="left" valign="top">${tour.TourObjective}</td>
            <td align="left" valign="top">${tour.TourFromDate}</td>
            <td align="left" valign="top">${tour.TourTime}</td>
            <td align="left" valign="top">${tour.FromLoc}</td>
            <td align="left" valign="top">${tour.ToLoc}</td>
            <td align="left" valign="top">${tour.StayingLoc}</td>
            <td align="left" valign="top">${tour.Mode}</td>
            <td align="left" valign="top">${tour.TicketClass}</td>
            <td align="left" valign="top">${tour.ModeDetail}</td>
            <td align="left" valign="top">${tour.TicketingByDesc}</td>
            <td align="left" valign="top">${tour.SpecialAppDesc}</td>
            <td align="left" valign="top"><button id="btnEdit" data-index="${index}"><img src="/images/icon_edit.gif" /></button></td>
            <td align="left" valign="top"> <button id="btnDelete"><img src="/images/icon_delete.gif" /></button></td>
        `;
        gridBody.appendChild(row);
    });
}
function clearControls() {
    // Clear text fields
    document.getElementById("txtModeDetail").value = "";
    document.getElementById("txtDailyObjective").value = "";
    document.getElementById("txtRemarks").value = "";
    document.getElementById("txtFromCity").value = "";
    document.getElementById("txtToCity").value = "";
    document.getElementById("txtStayingCity").value = "";
    document.getElementById("preferredLocation").value = ""; // Added By Kishan Dodiya

    // Reset dropdowns
    document.getElementById("cmbFrom").value = "0";
    document.getElementById("cmbTo").value = "0";
    document.getElementById("cmbStaying").value = "0";
    document.getElementById("cmbMode").value = "";
    document.getElementById("cmbClass").value = "";
    // document.getElementById("cmbPickDrop").value = ""; // Commented By Kishan Dodiya

    // Reset checkbox
    document.getElementById("chkSpecialApp").checked = false;

    // Disable text fields
    document.getElementById("txtFromCity").disabled = true;
    document.getElementById("txtToCity").disabled = true;
    document.getElementById("txtStayingCity").disabled = true;

    // Reset time dropdowns
    document.getElementById("optHour").value = "0";      // Added By Aumento For SR73072
    document.getElementById("optMinute").value = "0";    // Added By Aumento For SR73072
    document.getElementById("optHourTo").value = "0";    // Added By Aumento For SR73072
    document.getElementById("optMinuteTo").value = "0";  // Added By Aumento For SR73072
}
//-------------------cmdAddMore_Click : End---------------------------




//-------------------btnSaveAsDraft_Click : Start---------------------------
async function btnSaveAsDraft_Click() { 
    oTourList = oTourList || [];
    oTourListprv = oTourListprv || [];
    oTourListOrg = oTourListOrg || [];
    oday = otourperiod || [];
    const _deleted = [];
     
    // IF NO TRAVEL DETAIL IS FILLED
    if (document.querySelectorAll("#gvList tr").length === 0) {
        document.getElementById("errorpanel").style.display = "inline";
        document.getElementById("status").innerText = "Please Fill at least one tour details.";
        return;
    }

    ReqAmountCHK();
    const userDetails = await getEmployeeOfficialDetails();
    const userSiteID = userDetails?.SYSITEID ?? "";

    const sitesDay = String(await getParameterValue() ?? "");
    console.log(sitesDay);
    const siteDayArray = sitesDay.split("|");  

    if (document.getElementById("chkAdvance").checked) {
        for (let str of siteDayArray) {
            const parts = str.split("~");
            const PDay = parts[0];
            const psite = parts[1];
            if (userSiteID === psite) {
                for (let vrownum = 0; vrownum < oday.length; vrownum++) {
                    const oTour = oday[vrownum];
                    const ODDAY = oTour.Day ? parseInt(oTour.Day) : 0;
                    if (ODDAY > parseInt(PDay)) {
                        document.getElementById("errorpanel").style.display = "inline";
                        document.getElementById("status").innerText =
                            "Your Previous advance is overdue for settlement by " + oTour.Day + " days. Kindly submit your bill first.";
                        highlightRow("gvAdvance", vrownum);
                        document.getElementById("chkAdvance").focus();
                        return;
                    }
                }
            }
        }
    } 
    const RequestID = new URLSearchParams(window.location.search).get("id");
    const MobileNo = document.getElementById("txtMobile").value;
    const ExtNo = document.getElementById("txtExtension").value;
    const Email = document.getElementById("txtEmail").value;
    const Objective = document.getElementById("txtObjective").value;
    const AdvRemarks = document.getElementById("txtAdvRemarks").value;
    const AdvRequired = (document.getElementById("chkAdvance").value === "YES" ? 1 : 0);

    let APlusNights = "";
    let ANights = "";
    let BNights = "";
    let CNights = "";
    let APlusDays = "";
    let ADays = "";
    let BDays = "";
    let CDays = "";
    let StayCharge = 0;
    let DailyAllow = 0;
    let MiscAllow = 0;
    let MiscRemarks = "";

    if (AdvRequired === 1) {
        APlusNights = document.getElementById("lblStayChargeAPLUS").innerText;
        ANights = document.getElementById("lblStayChargeA").innerText;
        BNights = document.getElementById("lblStayChargeB").innerText;
        CNights = document.getElementById("lblStayChargeC").innerText;
        StayCharge = parseFloat(document.getElementById("lblTotalCharges").innerText) || 0;

        APlusDays = document.getElementById("lblAllownceAPLUS").innerText;
        ADays = document.getElementById("lblAllownceA").innerText;
        BDays = document.getElementById("lblAllownceB").innerText;
        CDays = document.getElementById("lblAllownceC").innerText;
        DailyAllow = parseFloat(document.getElementById("lblTotalAllowances").innerText) || 0;

        if (document.getElementById("txtMiscAmout").value !== "")
            MiscAllow = parseFloat(document.getElementById("txtMiscAmout").value) || 0;

        if (document.getElementById("txtRequiredAmount").value !== "")
            var RequiredAmount = parseFloat(document.getElementById("txtRequiredAmount").value) || 0;

        MiscRemarks = document.getElementById("txtMiscRemarks").value;
    }
    // Save As Draft
    let Initiator_Status = "0";

    let resStatus;
    //console.log('oTourList ', oTourList);
    //console.log('oTourListprv ', oTourListprv);
    resStatus = await updateTourDetail(RequestID,MobileNo, ExtNo, Email, Objective, AdvRemarks,
        AdvRequired, APlusNights, ANights, BNights, CNights, StayCharge,
        APlusDays, ADays, BDays, CDays, DailyAllow, MiscAllow, oTourList,
        MiscRemarks, RequiredAmount, oTourListprv, Initiator_Status, _deleted
    );
    //console.log('Hi resStatus :', resStatus);
    // Show message label (same as server)
    let lblMSG = document.getElementById("lblMSG");
    lblMSG.style.display = "block";
    if (resStatus > 0) {
        lblMSG.innerText = "Request Save as Draft Successfully";
        window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx";
    } else {
        lblMSG.innerText = "Error in Request Save As Draft...";
    }
}

async function getEmployeeOfficialDetails() {
    await $.ajax({
        url: '/TourRequest/GetEmployeeOfficialDetails',
        type: 'GET',
        success: function (response) {
            return response?.result ?? null;
        },
        complete: function () {
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseJSON?.message || 'An error occurred .');
        }
    });
}
async function getParameterValue() {
    var _SitesDay = "";
    await $.ajax({
        url: '/TourRequest/GetParameterValue',
        type: 'GET',
        success: function (response) {
            var result = response.result;
            if (result) {
                _SitesDay = result._SitesDay;
            }
            return _SitesDay;
        },
        complete: function () {
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseJSON?.message || 'An error occurred .');
        }
    });
} 

async function updateTourDetail(
    RequestID, MobileNo, ExtNo, Email, Objective, AdvRemarks,
    AdvRequired, APlusNights, ANights, BNights, CNights, StayCharge,
    APlusDays, ADays, BDays, CDays, DailyAllow, MiscAllow, oTourList,
    MiscRemarks, RequiredAmount, oTourListprv, Initiator_Status, _deleted
) {
    try {
        const payload = {RequestID, MobileNo, ExtNo, Email, Objective, AdvRemarks,
                AdvRequired, APlusNights, ANights, BNights, CNights, StayCharge,
                APlusDays, ADays, BDays, CDays, DailyAllow, MiscAllow,oTourList,              
                MiscRemarks, RequiredAmount, oTourListprv,Initiator_Status, _deleted
        };

        const response = await $.ajax({
            url: '/TourRequest/UpdateTourDetail',
            type: 'POST',
            data: JSON.stringify(payload),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json'
        });

        const resStatus =
            response?.result?.resStatus ??
            response?.resStatus ??
            response?.status ??
            0;

        return resStatus;
    } catch (xhr) {
        console.log(xhr.responseJSON?.message || 'An error occurred .');
        return 0;
    }
}

//-------------------btnSaveAsDraft_Click : End---------------------------

//-----------------------------cmdSubmit_Click : Start -------------------
async function cmdSubmit_Click() { 
    oTourList = oTourList || [];
    oTourListprv = oTourListprv || [];
    oTourListOrg = oTourListOrg || [];
    oday = otourperiod || [];

    //IF NO TRAVEL DETAIL IS FILLED
    if (document.querySelectorAll("#gvList tr").length === 0) {
        showError("Please fill at least one tour detail.");
        return;
    }
    ReqAmountCHK();

    const qOrgList = oTourListOrg;
    const qNewList = oTourList;

    const _deleted = qOrgList.filter(x =>
        !qNewList.some(x2 => String(x2.RequestID) === String(x.RequestID))
    );
    console.log('_deleted', _deleted);


    const RequestID = new URLSearchParams(window.location.search).get("id");
    const MobileNo = document.getElementById("txtMobile").value;
    const ExtNo = document.getElementById("txtExtension").value;
    const Email = document.getElementById("txtEmail").value;
    const Objective = document.getElementById("txtObjective").value;
    const AdvRemarks = document.getElementById("txtAdvRemarks").value;
    const AdvRequired = (document.getElementById("chkAdvance").value === "YES" ? 1 : 0);

    let APlusNights = "";
    let ANights = "";
    let BNights = "";
    let CNights = "";
    let APlusDays = "";
    let ADays = "";
    let BDays = "";
    let CDays = "";
    let StayCharge = 0;
    let DailyAllow = 0;
    let MiscAllow = 0;
    let MiscRemarks = "";

    if (AdvRequired === 1) {
        APlusNights = document.getElementById("lblStayChargeAPLUS").innerText;
        ANights = document.getElementById("lblStayChargeA").innerText;
        BNights = document.getElementById("lblStayChargeB").innerText;
        CNights = document.getElementById("lblStayChargeC").innerText;
        StayCharge = parseFloat(document.getElementById("lblTotalCharges").innerText) || 0;

        APlusDays = document.getElementById("lblAllownceAPLUS").innerText;
        ADays = document.getElementById("lblAllownceA").innerText;
        BDays = document.getElementById("lblAllownceB").innerText;
        CDays = document.getElementById("lblAllownceC").innerText;
        DailyAllow = parseFloat(document.getElementById("lblTotalAllowances").innerText) || 0;

        if (document.getElementById("txtMiscAmout").value !== "")
            MiscAllow = parseFloat(document.getElementById("txtMiscAmout").value) || 0;

        if (document.getElementById("txtRequiredAmount").value !== "")
            var RequiredAmount = parseFloat(document.getElementById("txtRequiredAmount").value) || 0;

        MiscRemarks = document.getElementById("txtMiscRemarks").value;
    }

    const userDetails = await getEmployeeOfficialDetails();
    const userSiteID = userDetails?.SYSITEID ?? "";

    const sitesDay = String(await getParameterValue() ?? "");
    const siteDayArray = sitesDay.split("|");

    if (document.getElementById("chkAdvance").checked) {
        for (let str of siteDayArray) {
            const parts = str.split("~");
            const PDay = parts[0];
            const psite = parts[1];
            if (userSiteID === psite) {
                for (let vrownum = 0; vrownum < oday.length; vrownum++) {
                    const oTour = oday[vrownum];
                    const ODDAY = oTour.Day ? parseInt(oTour.Day) : 0;
                    if (ODDAY > parseInt(PDay)) {
                        document.getElementById("errorpanel").style.display = "inline";
                        document.getElementById("status").innerText =
                            "Your Previous advance is overdue for settlement by " + oTour.Day + " days. Kindly submit your bill first.";
                        highlightRow("gvAdvance", vrownum);
                        document.getElementById("chkAdvance").focus();
                        return;
                    }
                }
            }
        }
    } 

    let Initiator_Status = "1";
    let resStatus;
    resStatus = await updateTourDetail(RequestID, MobileNo, ExtNo, Email, Objective, AdvRemarks,
        AdvRequired, APlusNights, ANights, BNights, CNights, StayCharge,
        APlusDays, ADays, BDays, CDays, DailyAllow, MiscAllow, oTourList,
        MiscRemarks, RequiredAmount, oTourListprv, Initiator_Status, _deleted
    );
    let lblMSG = document.getElementById("lblMSG");
    lblMSG.style.display = "block";
    if (resStatus > 0) {
        lblMSG.innerText = "Request Successfully Updated";
        window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageRequest.aspx";
    } else {
        lblMSG.innerText = "Error in Request Submission...";
    }
} 
//-----------------------------cmdSubmit_Click : ENd ---------------------

// Helper function start
function getMonthValue(monthText) { //to map month text to value(if needed)
    const months = {
        "Jan": "1", "Feb": "2", "Mar": "3", "Apr": "4", "May": "5", "Jun": "6",
        "Jul": "7", "Aug": "8", "Sep": "9", "Oct": "10", "Nov": "11", "Dec": "12"
    };
    return months[monthText] || monthText;
}
function validateNumber(input) {
    // Remove invalid characters (anything except digits and dot)
    input.value = input.value.replace(/[^0-9.]/g, '');

    // Ensure only one decimal point
    if ((input.value.match(/\./g) || []).length > 1) {
        input.value = input.value.replace(/\.+$/, '');
    }
}
 
function updateDropdown(selector, items) {
    let dropdown = $(selector);
    dropdown.empty();
    items.forEach(item => {
        dropdown.append($("<option>", { value: item.value, text: item.text }));
    });
}
function parseDate(dateStr) {
    let parts = dateStr.split("-");
    if (parts.length !== 3) return null;

    let day = parseInt(parts[0], 10);
    let monthName = parts[1];
    let year = parseInt(parts[2], 10);

    let monthIndex = [
        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    ].indexOf(monthName);

    if (monthIndex === -1) return null;

    return new Date(year, monthIndex, day);
}
function showError(message) {
    document.getElementById("errorpanel").style.display = "inline";
    document.getElementById("status").innerText = message;
    return false;
}
function highlightRow(tableId, rowIndex) {
    document.querySelector(`#${tableId} tr:nth-child(${rowIndex + 1})`).style.backgroundColor = "red";
}
function getSelectedValue(id) {
    return document.getElementById(id).value;
}
function getSelectedText(id) {
    const el = document.getElementById(id);
    return el.options[el.selectedIndex].text;
}
// Helper function end
