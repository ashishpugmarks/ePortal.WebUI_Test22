$(document).ready(function () {
    CalculateLTATotal();
    applyMotChanges('load');
    $('.dpDate').datepicker({
        format: "dd-M-yyyy",
        todayHighlight: true,
        autoclose: true,

    });
});
$("[data-ctrl-id='GetDetails']").on('click', function () {
    GetLTADetail();
});
$("[data-ctrl-id='submit']").on('click', function () {
    AddDetails();
});
$("[data-ctrl-id='Cancel']").on('click', function () {
    $('#HRAModal').modal('hide');

});
$("[data-ctrl-id='Calculate']").on('change', function () {
    
    $(".tblChild tbody tr").each(function () {

        GetFare(this);
    });

});
$("[data-ctrl-id='MOTHeader']").on('change', function () {
    applyMotChanges('add');

    $(".tblChild tbody tr").each(function () {
        GetFare(this);
    });
});
$("[data-ctrl-id='IsTravel']").on('change', function () {

    const $tr = $(this).closest('tr');
    if ($(this).val() == "1") {
        if ($("#MOTHeader").val() != "1") {
        $tr.find("input[name$='.TktBillNo']").prop("disabled", false);
        $tr.find("input[name$='.Amount']").prop("disabled", false);
       
            $tr.find("input[name$='.TrainFare']").prop("disabled", false);
        }
    } else {
        $tr.find("input[name$='.TktBillNo']").prop("disabled", true);
        $tr.find("input[name$='.Amount']").prop("disabled", true);
        $tr.find("input[name$='.TrainFare']").prop("disabled", true);
    }
    $(".tblChild tbody tr").each(function () {
        GetFare(this);
    });
});
$("[data-ctrl-id='IsTravel']").on('change', function () {
    debugger;
    $(".tblChild tbody tr").each(function () {
        GetFare(this);

    });
});
function CalculateLTATotal() {
    // Sum Amount column
    var totalAmount = 0;
    var tainAmount = 0;
    var eleAmount = 0;
    $(".tblChild tbody tr").each(function () {
        var amt = parseFloat($(this).find("input[name='item.Amount']").val()) || 0;
        if ($("#MOTHeader").val() == "1") {
            $(this).find("input[name='item.TrainFare']").prop("disabled", true);
        }

        var trainFare = parseFloat($(this).find("input[name='item.TrainFare']").val()) || 0;
        var ElilableAmt = parseFloat($(this).find("span[id='ElilableAmt']").html()) || 0;
        totalAmount += amt;
        tainAmount += trainFare;
        eleAmount += ElilableAmt;

        //$("#totalEligbleAmount").text(totalEligible.toFixed(2));



        // Put results into <tfoot>
        $("#totalAmount").html(totalAmount);   // Amount column
        $("#totalTainAmount").html(tainAmount);     // TrainFare column
        $("#totalEligbleAmount").html(eleAmount); // Eligible Amount column
    })
};


function AddDetails() {
    var objLTAdtlcolls = [];

    $(".tblChild tbody tr").each(function () {

        var obj = {
            ID: $(this).find("input[name='item.ID']").val(),
            Age: $(this).find("span[id='Age']").html(),
            Amount: $(this).find("input[name='item.Amount']").val(),
            MOT: $(this).find("input[name='item.MOT']").val(),

            Name: $(this).find("span[id='Age']").html(),
            Relation: $(this).find("span[id='Relation']").html(),
            TktBillNo: $(this).find("input[name='item.TktBillNo']").val(),
            TrainFare: $(this).find("input[name='item.TrainFare']").val(),
            IsTravel: $(this).find("select[name='item.IsTravel']").val(),
            Gender: $(this).find("input[name='item.Gender']").val(),
            ElilableAmt: $(this).find("span[id='ElilableAmt']").html(),
        }
        objLTAdtlcolls.push(obj);
    });
    console.log(objLTAdtlcolls);
    const objLTADetail = {

        JFrom: $("#JFrom").val(),
        JTo: $("#JTo").val(),
        LTAAmount: $("#LTAAmount").val(),
        Placeofvisit: $("#Placeofvisit").val(),
        TripNo: $("#TripNoCnt").html(),
        MOTHeader: $("#MOTHeader").val(),
        NormalTrainFare: $("#NormalTrainFare").val(),
        objLTAdtlcolls: objLTAdtlcolls
    };


    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/SubmitLTA",
        data: JSON.stringify(objLTADetail),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            debugger;
            if (data.Item1 == "error") {
                alert(data.Item2)
            } else {
                alert('LTA Details Added Successfully');
                $("#1386").val(data.Item2["TotEligableAmount"]);
                $("#1340").val(data.Item2["StartDate"]);
                $("#1370").val(data.Item2["EndDate"]);
                $("#1394").val(data.Item2["Trip"]);
                $('#HRAModal').modal('hide');
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
function applyMotChanges(action) {
    // Get selected MOT value from the <select data-ctrl-id="MOTHeader">
    var motVal = $("[data-ctrl-id='MOTHeader']").val();
    debugger;
    // Adjust this mapping if needed:
    var isTrain = (motVal !== "1"); // Train in your Razor SelectList

    // Toggle the "Normal Train Fare" row
    $("#spnormaltrainfare").toggle(isTrain);

    // When hiding, clear the NormalTrainFare input
    
    if ($("#MOTHeader").val() == "1") {
        $("#spnormaltrainfare").show();
        
    } else {
        $("#spnormaltrainfare").hide();
       
    }
    // Handle rows in the child table (grid)
    var $rows = $(".tblChild tbody tr");

    if ($rows.length > 0) {
        // First row: set IsTravel="1" and disable
        var $firstRow = $rows.eq(0);
        if ($("#MOTHeader").val() == "1") {
            $firstRow.find("select[name$='.IsTravel']").val("1").prop("disabled", false);
        }

        // Walk through each row to apply resets and enable/disable logic
        $rows.each(function (i, tr) {
            var $tr = $(tr);

            // Relation text; note: IDs are duplicated per row, so scope with find on the row
            var relation = $tr.find("span#Relation").text().trim().toUpperCase();
            if (action == 'add') {
                // Clear row fields
                $tr.find("input[name$='.TktBillNo']").val("");
                $tr.find("input[name$='.Amount']").val("");
                $tr.find("input[name$='.TrainFare']").val("");
                $tr.find("span#ElilableAmt").text("");
            }
            if (relation !== "SELF") {
              
                    // Non-SELF rows: disable entry and blank IsTravel
                    $tr.find("input[name$='.TktBillNo']").prop("disabled", true);
                    $tr.find("input[name$='.Amount']").prop("disabled", true);
                    $tr.find("input[name$='.TrainFare']").prop("disabled", true);
                
                $tr.find("select[name$='.IsTravel']").prop("disabled", false);
                 if (action == 'add') {
                    $tr.find("select[name$='.IsTravel']").val("");
                }
            } else {
                // SELF row: enable billno + amount
                $tr.find("input[name$='.TktBillNo']").prop("disabled", false);
                $tr.find("input[name$='.Amount']").prop("disabled", false);

                // Mirror original C#: toggle ONLY the first row's fare based on MOT
                if (i === 0) {
                    // Original WebForms toggled by mot == "1"; here we use isTrain (mot == "4").
                    // If you want the original behavior exactly, change to: var disableFare = (motVal === "1");
                    var disableFare = !isTrain; // disable fare when NOT train; enable when train
                    $tr.find("input[name$='.TrainFare']").prop("disabled", disableFare);
                }
            }
        });
        if (action == 'add') {
            $("#NormalTrainFare").val("");
            // Clear footer totals
            $("#totalAmount").text("");
            $("#totalTainAmount").text("");
            $("#totalEligbleAmount").text("");
        }
    }

    // Equivalent of mpeEditltaP.Show():
    // If you have a Bootstrap/Custom modal, call it here, e.g.:
    // $("#EditLtaModal").modal("show");
}

function GetFare(e) {


    if ($("#MOTHeader").val() == "1") {
        const $tr = $(e);
        const rowIndex = $tr.index();


        var obj = {
            TravalType: $tr.find("select[name$='.IsTravel']").val(),
            lblname: ($tr.find("span#Name").text() || "").trim(),
            NormalTrainFare: $("#NormalTrainFare").val(),
            TravelMode: $("#MOTHeader").val(),
            Amount: $tr.find("input[name='item.Amount']").val(),
            Index: rowIndex,

        }



        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/Finance/GetFareEligibity",
            data: obj,
            //contentType: "application/json; charset=utf-8",
            //datatype: "json",
            success: function (data) {
                debugger;
                if (data.Item1 == "Error") {
                    alert(data.Item2)
                } else {
                    debugger;
                    var Amount = $tr.find("input[name='item.Amount']").val();
                    if (Amount == "") {
                        $tr.find("input[name='item.Amount']").val("0");
                    }
                    $tr.find("input[name='item.TrainFare']").val(data.Item1);
                    $tr.find("#ElilableAmt").html(data.Item2);
                    CalculateLTATotal();
                }
            },
            complete: function () {
                $("#ajaxLoader").removeClass('loader');
            },
            error: function () {
                sweetAlert("Oops...", "Something went wrong!", "error");
            }
        });
    } else {
        const $tr = $(e);
        const rowIndex = $tr.index();
        $tr.find("input[name='item.TrainFare']").val();
        $tr.find("#ElilableAmt").html($tr.find("input[name='item.TrainFare']").val());
        CalculateLTATotal();
    }
}
function handleIsTravelChange(e) {
    const $ddl = $(e.target);
    const $tr = $ddl.closest("tr");

    // Inputs in the current row
    const $billno = $tr.find("input[name$='.TktBillNo']");
    const $amount = $tr.find("input[name$='.Amount']");
    const $fare = $tr.find("input[name$='.TrainFare']");
    const $eligAmt = $tr.find("span#ElilableAmt");
    const relation = ($tr.find("span#Relation").text() || "").trim().toUpperCase();

    const travalType = $ddl.val();        // "1" = travelled, "" or "0" = not travelled
    const travelMode = $mot.val();        // "1"/"2"/"3"/"4" per your Razor dropdown
    const normalFare = ($normalTrainFare.val() || "").trim();
    const isTrain = isTrainSelected();

    if (travalType === "1") {
        // === Travelled ===
        if (isTrain) {
            // Train-specific branch (old server code checked TravelMode == "1"; here we treat Train as "4")
            if (normalFare !== "") {
                // If relation is NOT SELF → clear bill no & set amount to 0 (per server logic comments)
                if (relation !== "SELF") {
                    $billno.val("");
                    $amount.val("0");
                }
            }

            // Disable inputs when Train (per original behavior)
            $billno.prop("disabled", true);
            $amount.prop("disabled", true);
            $fare.prop("disabled", true);
        } else {
            // Non-Train: Enable inputs and focus bill no
            $billno.prop("disabled", false).trigger("focus");
            $amount.prop("disabled", false);
            $fare.prop("disabled", false);
        }

    } else {
        // === Not travelled ===
        $billno.prop("disabled", true).val("");
        $amount.prop("disabled", true).val("");
        $fare.prop("disabled", true).val("");
        $eligAmt.text("0");

        if (isTrain) {
            if (normalFare !== "") {
                calculateTotal();
                $fare.val("0");
            }
        } else {
            if (travelMode !== "") {
                const $firstRow = $rows.eq(0);
                const firstFare = ($firstRow.find("input[name$='.TrainFare']").val() || "").trim();
                const firstAmt = ($firstRow.find("input[name$='.Amount']").val() || "").trim();
                if (firstAmt !== "" && firstFare !== "") {
                    calculateTotal();
                }
            }
        }
    }
}



