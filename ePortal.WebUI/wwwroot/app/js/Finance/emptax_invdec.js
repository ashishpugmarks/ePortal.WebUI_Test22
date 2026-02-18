
function CalculateTotal() {

    let sum = 0;
    // Select all inputs with class "amount"
    document.querySelectorAll('.sub584').forEach(input => {
        let val = parseFloat(input.value) || 0; // convert to number, default 0
        sum += val;
    });
    document.getElementById('lbltotal584sub2').textContent = sum;
    sum = 0;
    // Select all inputs with class "amount"
    document.querySelectorAll('.C80').forEach(input => {
        let val = parseFloat(input.value) || 0; // convert to number, default 0
        sum += val;
    });
    document.getElementById('total80Cac').textContent = sum;

    sum = 0;
    // Select all inputs with class "amount"
    document.querySelectorAll('.D80').forEach(input => {
        let val = parseFloat(input.value) || 0; // convert to number, default 0
        sum += val;
    });
    document.getElementById('total80Dac').textContent = sum;
}

function openPopupwithScrol(strOpenUrl, width, height) {
    var winTop = (screen.height - height) / 2;
    var winLeft = (screen.width - width) / 2;
    var windowFeatures = "location=no,status=no,width=" + width + "px,height=" + height;
    windowFeatures = windowFeatures + "px,left = " + winLeft + "px,";
    windowFeatures = windowFeatures + "top=" + winTop + "px,resizable=yes" + ",SCROLLBARS";
    //window.open (strOpenUrl, "mywindow", "TOOLBAR=no,MENUBAR=no,SCROLLBARS=yes,RESIZABLE=no,LOCATION=no,DIRECTORIES=no,STATUS=yes,width="+width+",height="+height);
    window.open(strOpenUrl, "mywindow1", windowFeatures);
}
function openPopupwithScrol12bb(strOpenUrl, width, height) {
    var winTop = (screen.height - height) / 2;
    var winLeft = (screen.width - width) / 2;
    var windowFeatures = "location=no,status=no,width=" + width + "px,height=" + height;
    windowFeatures = windowFeatures + "px,left = " + winLeft + "px,";
    windowFeatures = windowFeatures + "top=" + winTop + "px,resizable=yes" + ",SCROLLBARS";
    //window.open (strOpenUrl, "mywindow", "TOOLBAR=no,MENUBAR=no,SCROLLBARS=yes,RESIZABLE=no,LOCATION=no,DIRECTORIES=no,STATUS=yes,width="+width+",height="+height);
    window.open(strOpenUrl, "mywindow12bb", windowFeatures);
}


$(document).ready(function () {
    $('.dpDate').datepicker({
        format: "dd-M-yyyy",
        todayHighlight: true,
        autoclose: true,

    });
    //setInterval("$('.blink').fadeOut(150).fadeIn(150);", 1000);
    showTaxDetails($("#TaxType").val());
    if ($("#TaxType").val() != "") {

    }
    if ($("#HRClaim").val() == "1") {
        $("#pnlrent1").show();
    }

    $("[data-ctrl-id='chkTaxType']").on('change', function (e) {
        debugger;
        var id = $(this)[0];

        showTaxDetails($(id).attr('data-type'));
    });
    $("[data-ctrl-id='lnkrentamount1']").on('click', function (e) {
        GetHRADetail();
    });
    $("[data-ctrl-id='ddlhraclaim']").on('change', function (e) {
        var id = $(this)[0];
        $("#pnlrent1").hide();

        if ($(id).val() == "1") {
            $("#pnlrent1").show();
        }
    });

    $("[data-ctrl-id='lnkDtl']").on('click', function (e) {
        debugger;
        GetReceiptsDetail($(this)[0]);
    });
    $("[data-cntrl-id='lnkForm12']").on('click', function (e) {

        GetB12Detail($(this)[0]);
    });

$("[data-ctrl-id='lnkLoan']").on('click', function (e) {

    GetLoanDetail($(this)[0]);
});
$("[data-ctrl-id='lnkLTAfrom']").on('click', function (e) {
    debugger;
    GetLTADetail($(this)[0]);
});
$("[data-cntrl-id='btnDraft']").on('click', function (e) {
    SaveAsDraft();
});
$("[data-cntrl-id='btnSubmit']").on('click', function (e) {
    SubmitDetails();
});
$("[data-cntrl-id='btnSubmitNew']").on('click', function (e) {
    SubmitNewDetails();
});

$("[data-cntrl-id='btnCancel']").on('click', function (e) {
    window.location.href = "/Finance/Dashboard";
});
$("[data-cntrl-id='btnPrint']").on('click', function (e) {
    openPopupwithScrol12bb('EmpTaxInv_12BBReport?', '950', '500');
    openPopupwithScrol('EmpTaxInv_DecReport', '1000', '500');
});
    $("[data-ctrl-id='txtProAmt']").on('change', function (e) {
        ChangeProjAmount($(this)[0]);
    });


});
function showTaxDetails(taxType) {
    $("#tbl_old").hide();
    $("#tbl_new").hide();
    $("#tbl_" + taxType).show();
}

function GetHRADetail(crnt) {
    if ($("#txtrentamount1").val() == "") {
        $("#txtrentamount1").focus();
        return;
    }
    var data = {

        KIID: $("#ddlki").val(),
        AnnualAmount: $("#txtrentamount1").val(),
    }

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/GetHRADetails",
        data: data,
        //  contentType: "application/json; charset=utf-8",
        //  datatype: "json",
        success: function (data) {
            if (data == "error") {
                sweetAlert("", "An error occurred while processing your request.", "error");
            }
            else {
                $("#modelHead").html("HRA");
                $('#divModel').html(data);



                $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
                $('#HRAModal').modal('show'); // then show it
                $(".pnlmultilandlord").hide();

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

function GetReceiptsDetail(crnt) {

    var data = {

        Head: $(crnt).attr('data-head'),
        TaxId: Number($(crnt).attr('data-tax-id')),

    }

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/ViewReceiptDetails",
        data: data,
        //  contentType: "application/json; charset=utf-8",
        //  datatype: "json",
        success: function (data) {

            $("#modelHead").html($(crnt).attr('data-head'));
            $('#divModel').html(data);



            $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
            $('#HRAModal').modal('show'); // then show it



        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}

function GetLoanDetail(crnt) {

    var data = {

        ki: $("#ddlki").val(),
        TaxId: $(crnt).attr('data-tax-id'),


    }

    $.ajax({
        type: "GET",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/GetLoanDetails",
        data: data,
        //  contentType: "application/json; charset=utf-8",
        //  datatype: "json",
        success: function (data) {
            debugger;
            $("#modelHead").html("Loan Declaration");
            $('#divModel').html(data);



            $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
            $('#HRAModal').modal('show'); // then show it

            if ($("#ConsDate").val() == "" && $("#ProjStatus").val() == "2") {
                alert("Please Fill the date for the completion of construction");
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

function GetLTADetail() {


    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/GetLTADetails",
        //data: data,
        //  contentType: "application/json; charset=utf-8",
        //  datatype: "json",
        success: function (data) {
            debugger;
            $("#modelHead").html("LTA");
            $('#divModel').html(data);



            $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
            $('#HRAModal').modal('show'); // then show it



        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function GetB12Detail(crnt) {

    var data = {

        Head: "",
        TaxId: "",

    }

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/ViewB12Details",
        data: data,
        //  contentType: "application/json; charset=utf-8",
        //  datatype: "json",
        success: function (data) {

            $("#modelHead").html("B12");
            $('#divModel').html(data);



            $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
            $('#HRAModal').modal('show'); // then show it



        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function SaveAsDraft() {
    
        var data = {
            HRClaim: $("#HRClaim").val()
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/Finance/TaxSaveAsDraft",
            data: data,
            //  contentType: "application/json; charset=utf-8",
            //  datatype: "json",
            success: function (data) {

                if (data.Item1 == "error") {
                    alert(data.Item2);
                }

                else {
                    alert(data.Item2);
                    window.location.href = "/Finance/Dashboard";

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
function SubmitDetails() {

    if (validate()) {
        var data = {
            HRClaim: $("#HRClaim").val(),
        }

        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/Finance/TaxSubmit",
            data: data,
            //  contentType: "application/json; charset=utf-8",
            //  datatype: "json",
            success: function (data) {

                if (data.Item1 == "error") {
                    alert(data.Item2);
                }

                else {
                    alert(data.Item2);
                    window.location.href = "/Finance/Dashboard";

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
}
function SubmitNewDetails() {

    var data = {
        HRClaim: $("#HRClaim").val()
    }

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/TaxSubmitNew",
        //data: data,
        //  contentType: "application/json; charset=utf-8",
        //  datatype: "json",
        success: function (data) {

            if (data.Item1 == "error") {
                alert(data.Item2);
            }

            else {
                alert(data.Item2);
                window.location.href = "/Finance/Dashboard";

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

function ChangeProjAmount(crnt) {
    debugger;
    var data = {

        Val: $(crnt).val(),
        TaxId: $(crnt).attr('data-tax-id'),


    }

    $.ajax({
        type: "GET",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/ChangeProjAmount",
        data: data,
        //  contentType: "application/json; charset=utf-8",
        //  datatype: "json",
        success: function (data) {
            CalculateLTATotal();

        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function isValidPAN(value, strictType = false) {
    if (!value) return false;
    const s = value.trim().toUpperCase();

    const basic = /^[A-Z]{5}[0-9]{4}[A-Z]$/;
    if (!basic.test(s)) return false;

    // Optional: enforce 4th-character holder type
    if (strictType) {
        const typeChar = s.charAt(3);
        if (!/[PCHABGJLFT]/.test(typeChar)) return false;
    }
    return true;
}
function validate() {
    var objmobileno = document.getElementById("txtMob");
    var objisexem = document.getElementById("chkTerms");
    var objrent = document.getElementById("HRClaim").value;
    var objhdloan = document.getElementById("txtprojamt");
     if (objisexem.checked == false) {
            alert("Please accept the undertaking.");
            return false;
        }
        if (objmobileno.value == "") {
            alert("Please enter mobile no.");
            return false;
        }
        if (objmobileno.value.length != 10) {
            alert("Mobile No. must be in 10 digit.");
            return false;
        }
        if (objrent == "") {
            alert("Please select HRA claim option.");
            return false;
        }
        if (objrent != "1") {
            if (!confirm("You want to submit without HRA amount."))
                return false;
        }
        else {
            if (objhdloan.value != "") {
                if (objhdloan.value != "0") {
                    if (!confirm("Please note you are claiming both exemption of both HRA as well as Interest on House Property. If place from where house property is located is commutable to your work premises, then please claim benefit of one of the two options.Are you sure you want to continue?."))
                        return false;
                }
            }
        }
    
    return true;
}




