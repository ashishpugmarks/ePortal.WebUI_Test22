 $(document).ready(function () {
            $('.dpDate').datepicker({
                format: "dd-M-yyyy",
                todayHighlight: true,
                autoclose: true,

            });
           
            $("[data-ctrl-id='addEEB']").on('click', function () {
           addEEB();
            });
            $("[data-ctrl-id='editEEB']").on('click', function () {
           EditEEB($(this)[0]);
            });
            $("[data-ctrl-id='deleteEEB']").on('click', function () {
           deleteEEB($(this)[0]);
            });
     
     $("[data-ctrl-id='btnsubmitEEB']").on('click', function () {
           submitEEB();
            });
            $("[data-ctrl-id='btncancelEEB']").on('click', function () {
           $('#HRAModal').modal('hide');
            });

        });

         function addEEB(){

         var data={
             Id:parseInt($("#NewReceipt_SLNO").val()),
              TaxId: $("#NewReceipt__TaxHeadID").val(),
              ReceiptNo:$("#NewReceipt__ReceiptNo").val(),
              Date: $("#txtDtFrom").val(),
              Amount: $("#NewReceipt_Amount").val(),

         }


                                   $.ajax({
                      type: "POST",
                      beforeSend: function () {
                          $("#ajaxLoader").addClass('loader');
                      },
                       url: "/Finance/AddEEBDetails",
                         data: data,
                        // contentType: "application/json; charset=utf-8",
                        // datatype: "json",
                      success: function (data) {
                          if(data.Item1=="error"){
                          alert(data.Item2);
                          }
                          else{
                         $("#modelHead").html("HRA");
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
         function deleteEEB(current){
              var data={
             Id:$(current).attr('data-value'),
             TaxId: $("#NewReceipt__TaxHeadID").val(),

         }


                                   $.ajax({
                      type: "POST",
                      beforeSend: function () {
                          $("#ajaxLoader").addClass('loader');
                      },
                       url: "/Finance/DeleteEEBDetails",
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
        function EditEEB(current){
            var data=$(current).attr('data-value');
            data=JSON.parse(data.replace(')',''));
            debugger;
            $("#NewReceipt_SLNO").val(data["SLNO"].toString());
        $("#NewReceipt__TaxHeadID").val(data["_TaxHeadID"]);
        $("#NewReceipt__ReceiptNo").val(data["_ReceiptNo"]);
        $("#txtDtFrom").val(data["_PrmDate"]);
        $("#NewReceipt_Amount").val(data["Amount"]);
        }
         function submitEEB(){
              var data={
             TaxHead:$("#TaxHeadName").val(),
             TaxId: $("#NewReceipt__TaxHeadID").val(),

         }


                                   $.ajax({
                      type: "POST",
                      beforeSend: function () {
                          $("#ajaxLoader").addClass('loader');
                      },
                       url: "/Finance/SubmitReceiptDetails",
                         data: data,
                        // contentType: "application/json; charset=utf-8",
                        // datatype: "json",
                      success: function (data) {
                          debugger;
                          $("[data-ctrl-id='"+$("#NewReceipt__TaxHeadID").val()+"']").val(data);
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
