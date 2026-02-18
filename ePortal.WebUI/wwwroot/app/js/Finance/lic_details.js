  $(document).ready(function () {
            $('.dpDate').datepicker({
                format: "dd-M-yyyy",
                todayHighlight: true,
                autoclose: true,

            });
           
            $("[data-ctrl-id='addReceipt']").on('click', function () {
           addReceipt();
            });
            $("[data-ctrl-id='editReceipt']").on('click', function () {
           EditReceipt($(this)[0]);
            });
            $("[data-ctrl-id='deleteReceipt']").on('click', function () {
           deleteReceipt($(this)[0]);
            });
            $("[data-ctrl-id='btnsubmitReceipt']").on('click', function () {
           submitReceipt();
            });
            $("[data-ctrl-id='btncancelReceipt']").on('click', function () {
           $('#HRAModal').modal('hide');
            });

        });

         function addReceipt(){

         var data={
             Id:parseInt($("#NewReceipt_SLNO").val()),
              TaxHead: $("#TaxHeadName").val(),
              TaxId: $("#NewReceipt__TaxHeadID").val(),
              ReceiptNo:$("#NewReceipt__ReceiptNo").val(),
              Date: $("#txtDtFrom").val(),
              Amount: $("#NewReceipt_Amount").val(),
              Policyno:$("#NewReceipt__PolicyNo").val(),
              PrmDate: $("#txtLDtFrom").val(),
              SumAssured: $("#NewReceipt__SumAssured").val(),

         }


                                   $.ajax({
                      type: "POST",
                      beforeSend: function () {
                          $("#ajaxLoader").addClass('loader');
                      },
                       url: "/Finance/AddLICDetails",
                         data: data,
                        // contentType: "application/json; charset=utf-8",
                        // datatype: "json",
                      success: function (data) {
                         $("#modelHead").html("HRA");
                               $('#HRAModal').modal('hide');
                               $("#modelHead").html($("#TaxHeadName").val());
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
         function deleteReceipt(current){
              var data={
             Id:$(current).attr('data-value'),
             TaxId: $("#NewReceipt__TaxHeadID").val(),

         }


                                   $.ajax({
                      type: "POST",
                      beforeSend: function () {
                          $("#ajaxLoader").addClass('loader');
                      },
                       url: "/Finance/DeleteLICDetails",
                         data: data,
                        // contentType: "application/json; charset=utf-8",
                        // datatype: "json",
                      success: function (data) {
                          if(data.Item1=="error"){
                          alert(data.Item2);
                          }
                          else{
                         $("#modelHead").html($("#TaxHeadName").val());
                               $('#HRAModal').modal('hide');
                               $("#modelHead").html($("#TaxHeadName").val());
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
        function EditReceipt(current){
            var data=$(current).attr('data-value');
            data=JSON.parse(data.replace(')',''));
            debugger;
        $("#NewReceipt_SLNO").val(data["SLNO"].toString());
        $("#NewReceipt__TaxHeadID").val(data["_TaxHeadID"]);
        $("#NewReceipt__ReceiptNo").val(data["_ReceiptNo"]);
        $("#txtDtFrom").val(data["_PrmDate"]);
        $("#NewReceipt_Amount").val(data["Amount"]);
        $("#NewReceipt__PolicyNo").val(data["_PolicyNo"]);
        $("#txtLDtFrom").val(data["_Date"]);
        $("#NewReceipt__SumAssured").val(data["_SumAssured"]);
        }
         function submitReceipt(){
           $("[data-ctrl-id='"+$("#NewReceipt__TaxHeadID").val()+"']").val($("#Total").html());
             submitLicReceipt();
}
function submitLicReceipt() {
    debugger;
    var data = {
        TaxHead: $("#TaxHeadName").val(),
        TaxId: $("#NewReceipt__TaxHeadID").val(),
        Amt: $("#Total").html()
    }


    $.ajax({
        type: "POST",
        beforeSend: function () {
            $("#ajaxLoader").addClass('loader');
        },
        url: "/Finance/SubmitLicDetails",
        data: data,
        // contentType: "application/json; charset=utf-8",
        // datatype: "json",
        success: function (data) {
            debugger;
            $("[data-ctrl-id='" + $("#NewReceipt__TaxHeadID").val() + "']").val(data);
            $('#HRAModal').modal('hide');

            calculateTotal();
        },
        complete: function () {
            $("#ajaxLoader").removeClass('loader');
        },
        error: function () {
            sweetAlert("Oops...", "Something went wrong!", "error");
        }
    });
}
