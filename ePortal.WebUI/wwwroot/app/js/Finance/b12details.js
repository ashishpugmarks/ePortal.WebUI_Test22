   $(document).ready(function () {
        $('.dpDate').datepicker({
            format: "dd-M-yyyy",
            todayHighlight: true,
            autoclose: true,

        });

    $("[data-ctrl-id='AddDetails']").on('click', function () {
        AddB12Details();
            });
    $("[data-ctrl-id='EditDetails']").on('click', function () {
        EditDetails($(this)[0]);
            });
    $("[data-ctrl-id='btnSubmitb12']").on('click', function () {
        SubmitDetails();
            });
    $("[data-ctrl-id='btnSubmitb12']").on('click', function () {
        SubmitDetails();
            });
    $("[data-ctrl-id='DeleteDetails']").on('click', function () {
        deleteB12($(this)[0]);
            });
    $("[data-ctrl-id='btnCancelb12']").on('click', function () {
        $('#HRAModal').modal('hide'); // then show it
            });
      });
    function deleteB12(current){
         var data={
        Id:$(current).attr('data-value'),
            

         }


    $.ajax({
        type: "POST",
    beforeSend: function () {
        $("#ajaxLoader").addClass('loader');
                      },
    url: "/Finance/DeleteB12Details",
    data: data,
    // contentType: "application/json; charset=utf-8",
    // datatype: "json",
    success: function (data) {
                          if(data.Item1=="error"){
        alert(data.Item2);
                          }
    else{
        $("#modelHead").html("B12");
    $('#HRAModal').modal('hide');
    $("#modelHead").html($("#TaxHeadName").val());
    $('#divModel').html(data);



    $('#HRAModal').modal({backdrop: 'static', keyboard: false });
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
    function EditDetails(current){
         
        var data=$(current).attr('data-value');
    data=JSON.parse(data.replace(')',''));
    $("#B12_ID").val(data["ID"]);
    $("#B12_EmployerName").val(data["EmployerName"]);
    $("#B12_EmployerAddress").val(data["EmployerAddress"]);
    $("#B12_EmployerTan").val(data["EmployerTan"]);
    $("#B12_EmployerPan").val(data["EmployerPan"]);
    $("#B12_FromDate").val(data["FromDate"]);
    $("#B12_ToDate").val(data["ToDate"]);
    $("#B12_TotalGross").val(data["TotalGross"]);
    $("#B12_Perk").val(data["Perk"]);
    $("#B12_TotalExempt").val(data["TotalExempt"]);
    $("#B12_Deductions").val(data["Deductions"]);
    $("#B12_TaxableIncome").val(data["TaxableIncome"]);
    $("#B12_TotalAmtTax").val(data["TotalAmtTax"]);
    $("#B12_SLNO").val(data["SLNO"]);
        }
    function AddB12Details(){
      var  B12 ={
        ID:$("#B12_ID").val(),
    EmployerName:$("#B12_EmployerName").val(),
    EmployerAddress:$("#B12_EmployerAddress").val(),
    EmployerTan:$("#B12_EmployerTan").val(),
    EmployerPan:$("#B12_EmployerPan").val(),
    FromDate:$("#B12_FromDate").val(),
    ToDate:$("#B12_ToDate").val(),
    TotalGross:$("#B12_TotalGross").val(),
    Perk:$("#B12_Perk").val(),
    TotalExempt:$("#B12_TotalExempt").val(),
    Deductions:$("#B12_Deductions").val(),
    TaxableIncome:$("#B12_TaxableIncome").val(),
    TotalAmtTax:$("#B12_TotalAmtTax").val(),
    SLNO:$("#B12_SLNO").val(),
    }
    var data={
        Address:$("#txtempaddress").val(),
    B12:B12
    }
    $.ajax({
        type: "POST",
    beforeSend: function () {
        $("#ajaxLoader").addClass('loader');
                     },
    url: "/Finance/AddB12Details",
    data: JSON.stringify(data),
    contentType: "application/json; charset=utf-8",
    datatype: "json",
    success: function (data) {
                         if(data.Item1=="error"){

        alert(data.Item2);
                          }else{
        alert('Details Added Successfully');
    $('#HRAModal').modal('hide');
    $("#modelHead").html("B12");
    $('#divModel').html(data);



    $('#HRAModal').modal({backdrop: 'static', keyboard: false });
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
    function SubmitDetails(){
        $.ajax({
            type: "POST",
            beforeSend: function () {
                $("#ajaxLoader").addClass('loader');
            },
            url: "/Finance/SubmitB12Details",
            //data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                debugger;
                if (data.Item1 == "error") {

                } else {
                    $("#1343").val(data.Item2["TotalGross"]);
                    $("#1413").val(data.Item2["TotalExempt"]);
                    $("#1374").val(data.Item2["Deductions"]);
                    $("#1395").val(data.Item2["TotalAmtTax"]);
                    $("#1353").val(data.Item2["Perk"]);
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
