$(document).ready(function () {
    $(".joinCss").find("input, select, checkbox").prop("disabled", true);
    $('.dpDate').datepicker({
        format: "dd-M-yyyy",
        todayHighlight: true,
        autoclose: true,

    }) .on('changeDate', function (e) {
        // Call your function here
        if ($("#ProjStatus").val() == "2") {
            DateChangesDetails();
        }
    });
    $("[data-ctrl-id='PAN']").on('change', function (e) {
        debugger;
        $(".PAN").hide();
        if (!isValidPAN($(this).val())) {
            $(".PAN").show();
            $("[data-ctrl-id='PAN']").val("");
        }
    });
    if ($("#ProjStatus").val() == "1") {
        $("#projectedLoan").show();
    }
    else if ($("#ProjStatus").val() == "2") {
        $("#actualLoan").show();
    }
    
    addTotalInterest();
    $("[data-ctrl-id='txtInt']").on('change', function () {
        addTotalInterest();
    });
    $("[data-ctrl-id='cmdAddMore']").on('click', function () {
        addLoanDetails();
    });
    $("[data-ctrl-id='EditLoan']").on('click', function () {
        EditLoan($(this)[0]);
    });
    $("[data-ctrl-id='DeleteLoan']").on('click', function () {
        DeleteLoan($(this)[0]);
    });
    $("[data-ctrl-id='DeleteLoan']").on('click', function () {
        DeleteLoan($(this)[0]);
    });
    $("[data-ctrl-id='btnProjDelete']").on('click', function () {
        DeleteProjLoan();
    });
    
    $("[data-ctrl-id='submitLoan']").on('click', function () {
        SubmitLoan($(this)[0]);
    });
    $("[data-ctrl-id='btnCancel']").on('click', function () {
        $('#HRAModal').modal('hide');
    });
    //$("[data-ctrl-id='LoanTaken']").on('blur', function () {
    //    DateChangesDetails();
    //});
    $("[data-ctrl-id='IsUnderCons']").on('change', function () {

        IsUnderCons();
    });


    $("[data-ctrl-id='PropertyOwner']").on('change', function () {
        debugger;
        $(".joinCss").find("input, select, checkbox").prop("disabled", false);

        if ($(this).val() != "2") {
            $(".joinCss").find("input, select, checkbox").prop("disabled", true);
        }
    });

});
function IsUnderCons() {
    $(".joinCss").find("input, select, checkbox").prop("disabled", true);
    $("#pnlinterestdtl").find("input").prop("disabled", false);
    $("#LoanTaken").prop("disabled", false);
    $("#ConsDate").prop("disabled", false);
    $("#PropertyOwner").prop("disabled", false);
    $("#LoanTaken").val("");
    $("#ConsDate").val("");
    $("#PropertyOwner").val("-1");
    $("#CurrInterest").val("");
    $("#PreYear1").val("");
    $("#PreYear2").val("");
    $("#PreYear3").val("");
    $("#PreYear4").val("");
    $("#PreYear5").val("");
    $("#PreYear1").prop("disabled", true);
    $("#PreYear2").prop("disabled", true);
    $("#PreYear3").prop("disabled", true);
    $("#PreYear4").prop("disabled", true);
    $("#PreYear5").prop("disabled", true);
    if ($("input[data-ctrl-id='IsUnderCons']:checked").val() == "1") {
        $("#pnlinterestdtl").find("input").prop("disabled", true);
        $("#PropertyOwner").prop("disabled", true);
        $("#LoanTaken").prop("disabled", true);
        $("#ConsDate").prop("disabled", true);

    }
    addTotalInterest();
}
function addTotalInterest() {
    // Current interest
    const currInterest = $("#CurrInterest").val();
    $("#Tot_CurrInterest").html(currInterest || "");

    // Handle years 1–5 in a loop
    let total = parseFloat(currInterest) || 0;
    for (let i = 1; i <= 5; i++) {
        const yearVal = $(`#PreYear${i}`).val();
        if (yearVal) {
            const val = Math.floor(parseFloat(yearVal) / 5);
            $(`#Tot_Year${i}`).html(val);
            total += val;
        } else {
            $(`#Tot_Year${i}`).html("");
        }
    }

    // Show total
    $("#lbltotal").text(total);

    // Final total calculation
    const rdbjointVal = $("input[name='rdbjoint']:checked").val();
    const exemption = $("#txtexemption").val();

    if (rdbjointVal === "1" && exemption) {
        const finalTotal = Math.floor(total * (parseInt(exemption, 10) / 100));
        $("#lblfinaltotal").text(finalTotal);
    } else {
        $("#lblfinaltotal").text(total);
    }
}
function addLoanDetails() {

    var data = {
        IsUnderCons: $("input[name='IsUnderCons']:checked").val(),
        LoanTaken: $("#LoanTaken").val(),
        ConsDate: $("#ConsDate").val(),
        ValueOfHouse: $("#ValueOfHouse").val(),
        AnnualRent: $("#AnnualRent").val(),
        MunicipalTax: $("#MunicipalTax").val(),
        PropertyOwner: $("#PropertyOwner").val(),

        CurrentYear: $("#CurrentYear").val(),
        CurrInterest: $("#CurrInterest").val(),
        TotalCurrInterest: $("#Tot_CurrInterest").html(),
        Year1: $("#Year1").val(),
        PreYear1: $("#PreYear1").val(),
        TotalYear1: $("#Tot_Year1").html(),
        Year2: $("#Year2").val(),
        PreYear2: $("#PreYear2").val(),
        TotalYear2: $("#Tot_Year2").html(),
        Year3: $("#Year3").val(),
        PreYear3: $("#PreYear3").val(),
        TotalYear3: $("#Tot_Year3").html(),
        Year4: $("#Year4").val(),
        PreYear4: $("#PreYear4").val(),
        TotalYear4: $("#Tot_Year4").html(),
        Year5: $("#Year5").val(),
        PreYear5: $("#PreYear5").val(),
        TotalYear5: $("#Tot_Year5").html(),
        IsEmployeeOwner: $("input[name='IsEmployeeOwner']:checked").val(),
        IsJointLoan: $("input[name='IsJointLoan']:checked").val(),

        RelationshipId: $("#RelationshipId").val(),
        ShareInProperty: $("#ShareInProperty").val(),
        ShareInTaxExemption: $("#ShareInTaxExemption").val(),
        FinalTotalAmount: $("#lblfinaltotal").html(),
        SelfOccupancyFrom: $("#SelfOccupancyFrom").val(),
        SelfOccupancyTo: $("#SelfOccupancyTo").val(),
        HouseAddress: $("#HouseAddress").val(),
        TaxId: $("#TaxId").val(),
        LoanProvider: {
            SLNO: $("#LoanProvider_SLNO").val(),
            Id: $("#LoanProvider_Id").val(),
            LoanAmount: $("#LoanProvider_LoanAmount").val(),
            LoanProvider: $("#LoanProvider_LoanProvider").val(),
            AddressLoanProvider: $("#LoanProvider_AddressLoanProvider").val(),
            PanLoanProvider: $("#LoanProvider_PanLoanProvider").val(),

        }
       

    }


    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/AddLoanProvider",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data.Item1 == "error") {
                alert(data.Item2);
            }
            else {
                $("#modelHead").html("HRA");
                $('#HRAModal').modal('hide');
                $("#modelHead").html("Loan Declaration");
                $('#divModel').html(data);



                $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
                $('#HRAModal').modal('show'); // then show it

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
function EditLoan(current) {
    $('[data-ctrl-id="cmdAddMore"]').html('Update');
    var data = $(current).attr('data-value');
    data = JSON.parse(data.replace(')', ''));
    $("#LoanProvider_SLNO").val(data["SLNO"].toString());
    $("#LoanProvider_Id").val(data["Id"].toString());
    $("#LoanProvider_LoanAmount").val(data["LoanAmount"].toString());
    $("#LoanProvider_LoanProvider").val(data["LoanProvider"].toString());
    $("#LoanProvider_AddressLoanProvider").val(data["AddressLoanProvider"].toString());
    $("#LoanProvider_PanLoanProvider").val(data["PanLoanProvider"].toString());
    
}
function DeleteLoan(current) {
    var data = {
        Id: $(current).attr('data-value'),


    }


    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/DeleteLoanDetails",
        data: data,
        // contentType: "application/json; charset=utf-8",
        // datatype: "json",
        success: function (data) {
            if (data.Item1 == "error") {
                alert(data.Item2);
            }
            else {
                $("#modelHead").html($("#TaxHeadName").val());
                $('#HRAModal').modal('hide');
                $("#modelHead").html("Loan Declaration");
                $('#divModel').html(data);



                $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
                $('#HRAModal').modal('show'); // then show it
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
function DeleteProjLoan() {
   
    var data = {
        TaxId: $("#TaxId").val()
    }

    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/DeleteProjLoanDetails",
        data: data,
        // contentType: "application/json; charset=utf-8",
        // datatype: "json",
        success: function (data) {
            
                $("#modelHead").html($("#TaxHeadName").val());
                $('#HRAModal').modal('hide');
            alert('Loan detail deleted');

            $("[data-ctrl-id='txtLoanAmt']").val("0");

            

        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
function SubmitLoan(current) {
    var data = {
        ID: $("#ID").val(),
        ConsDate: $("#ConsDate").val(),
        IsUnderCons: $("input[name='IsUnderCons']:checked").val(),
        LoanTaken: $("#LoanTaken").val(),
        ConsDate: $("#ConsDate").val(),
        ValueOfHouse: $("#ValueOfHouse").val(),
        AnnualRent: $("#AnnualRent").val(),
        MunicipalTax: $("#MunicipalTax").val(),
        PropertyOwner: $("#PropertyOwner").val(),

        CurrentYear: $("#CurrentYear").val(),
        CurrInterest: $("#CurrInterest").val(),
        TotalCurrInterest: $("#Tot_CurrInterest").html(),
        Year1: $("#Year1").val(),
        PreYear1: $("#PreYear1").val(),
        TotalYear1: $("#Tot_Year1").html(),
        Year2: $("#Year2").val(),
        PreYear2: $("#PreYear2").val(),
        TotalYear2: $("#Tot_Year2").html(),
        Year3: $("#Year3").val(),
        PreYear3: $("#PreYear3").val(),
        TotalYear3: $("#Tot_Year3").html(),
        Year4: $("#Year4").val(),
        PreYear4: $("#PreYear4").val(),
        TotalYear4: $("#Tot_Year4").html(),
        Year5: $("#Year5").val(),
        PreYear5: $("#PreYear5").val(),
        TotalYear5: $("#Tot_Year5").html(),
        IsEmployeeOwner: $("input[name='IsEmployeeOwner']:checked").val(),
        IsJointLoan: $("input[name='IsJointLoan']:checked").val(),

        RelationshipId: $("#RelationshipId").val(),
        ShareInProperty: $("#ShareInProperty").val(),
        ShareInTaxExemption: $("#ShareInTaxExemption").val(),
        FinalTotalAmount: $("#lblfinaltotal").html(),
        SelfOccupancyFrom: $("#SelfOccupancyFrom").val(),
        SelfOccupancyTo: $("#SelfOccupancyTo").val(),
        HouseAddress: $("#HouseAddress").val(),
        TaxId: $("#TaxId").val(),
        Total: $("#lblfinaltotal").text(),
        ProjLoanTaken: $("#ProjLoanTaken").val(),
        ProjValueofHouse: $("#ProjValueofHouse").val(),
        ProjIntrest: $("#ProjIntrest").val(),
        LoanAmount: $("#LoanAmount").val(),
       LoanProviderName: $("#LoanProviderName").val(),
        AddressLoanProvider: $("#AddressLoanProvider").val(),
        PanLoanProvider: $("#PanLoanProvider").val(),


    }
    var url = "/Finance/SubmitLoanDetails";
    if ($("#ProjStatus").val() == "1") {
        var url = "/Finance/SubmitProjectedLoanDetails";
    }
    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: url,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data.Item1 == "error") {
                alert(data.Item2);
            }
            else {
                $("[data-ctrl-id='txtLoanAmt']").val(data);

                $('#HRAModal').modal('hide'); // then show it
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

function DateChangesDetails() {

    var data = {
        IsUnderCons: $("input[name='IsUnderCons']:checked").val(),
        LoanTaken: $("#LoanTaken").val(),
        ConsDate: $("#ConsDate").val(),
        ValueOfHouse: $("#ValueOfHouse").val(),
        AnnualRent: $("#AnnualRent").val(),
        MunicipalTax: $("#MunicipalTax").val(),
        PropertyOwner: $("#PropertyOwner").val(),

        CurrentYear: $("#CurrentYear").val(),
        CurrInterest: $("#CurrInterest").val(),
        TotalCurrInterest: $("#Tot_CurrInterest").html(),
        Year1: $("#Year1").val(),
        PreYear1: $("#PreYear1").val(),
        TotalYear1: $("#Tot_Year1").html(),
        Year2: $("#Year2").val(),
        PreYear2: $("#PreYear2").val(),
        TotalYear2: $("#Tot_Year2").html(),
        Year3: $("#Year3").val(),
        PreYear3: $("#PreYear3").val(),
        TotalYear3: $("#Tot_Year3").html(),
        Year4: $("#Year4").val(),
        PreYear4: $("#PreYear4").val(),
        TotalYear4: $("#Tot_Year4").html(),
        Year5: $("#Year5").val(),
        PreYear5: $("#PreYear5").val(),
        TotalYear5: $("#Tot_Year5").html(),
        IsEmployeeOwner: $("input[name='IsEmployeeOwner']:checked").val(),
        IsJointLoan: $("input[name='IsJointLoan']:checked").val(),

        RelationshipId: $("#RelationshipId").val(),
        ShareInProperty: $("#ShareInProperty").val(),
        ShareInTaxExemption: $("#ShareInTaxExemption").val(),
        FinalTotalAmount: $("#lblfinaltotal").html(),
        SelfOccupancyFrom: $("#SelfOccupancyFrom").val(),
        SelfOccupancyTo: $("#SelfOccupancyTo").val(),
        HouseAddress: $("#HouseAddress").val(),
        TaxId: $("#TaxId").val(),
        LoanProvider: {
            SLNO: $("#LoanProvider_SLNO").val(),
            Id: $("#LoanProvider_Id").val(),
            LoanAmount: $("#LoanProvider_LoanAmount").val(),
            LoanProvider: $("#LoanProvider_LoanProvider").val(),
            AddressLoanProvider: $("#LoanProvider_AddressLoanProvider").val(),
            PanLoanProvider: $("#LoanProvider_PanLoanProvider").val(),

        }

    }


    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/DateOfLoanChanged",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            if (data.Item1 == "error") {
                alert(data.Item2);
            }
            else {
                $("#modelHead").html("HRA");
                $('#HRAModal').modal('hide');
                $("#modelHead").html("Loan Declaration");
                $('#divModel').html(data);



                $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
                $('#HRAModal').modal('show'); // then show it

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
