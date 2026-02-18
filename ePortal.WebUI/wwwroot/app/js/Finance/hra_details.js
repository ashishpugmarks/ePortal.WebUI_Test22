  $(document).ready(function () {
              $(".landlord1").hide();
              if(parseFloat($('#txtrentamount1').val())>100000)
              {
                  $(".landlord1").show();
      }

              if($("#chkmultilandlord").prop("checked")){
                  $(".pnlmultilandlord").show();
      }
      $("[data-ctrl-id='PAN']").on('change', function (e) {
          debugger;
          $(".PAN").hide();
          if (!isValidPAN($(this).val())) {
              $(".PAN").show();
              $("[data-ctrl-id='PAN']").val("");
          }
      });
            // Handle Monthly Rent change
            $("[data-ctrl-id='MonthlyRentAmount'], #MonthlyRentAmount").on('change input', function () {
              updateRentBands();
            });
          $("[data-ctrl-id='btnAdd']").on('click', function () {
        AddDetails();
          });

           $("[data-ctrl-id='btnEdit']").on('click', function () {
        EditDetails($(this)[0]);
          });
           $("[data-ctrl-id='btnDelete']").on('click', function () {
        DeleteDetails($(this)[0]);
          });
          $("[data-ctrl-id='btnsubmithra']").on('click', function () {
        SaveDetails();
          });
          $("[data-ctrl-id='btncancelhra']").on('click', function () {
         $('#HRAModal').modal('hide');
          });
          $("[data-ctrl-id='MonthlyRentAmount'], #MonthlyRentAmount").on('change input', function () {
            updateRentBands();
          });
            // Handle Multi-Landlord checkbox change
            $("[data-ctrl-id='chklandlord'], #chkmultilandlord").on('change', function () {
              const isChecked = $(this).is(':checked');
              // Hide or show the multi-landlord panel
              $(".pnlmultilandlord").toggle(isChecked);
            });

            // Initialize on page load
            updateRentBands();
            $("[data-ctrl-id='chklandlord'], #chkmultilandlord").trigger('change');

            // ---- Function: Update rent bands ----
            function updateRentBands() {
              // Support both data-ctrl-id and id-based lookup
              const $rent = $("[data-ctrl-id='MonthlyRentAmount'], #MonthlyRentAmount").first();

              // Get value safely (handles null), trim, and remove all non-digits (e.g., "₹12,000" -> "12000")
              const raw = (($rent.val() || '') + '').trim().replace(/[^\d]/g, '');
              const amount = parseInt(raw, 10);

              // Hide all bands first
              $('#tr_99999, #tr_100000, #tr_240001, #tr_600000').hide();

              // If empty or invalid, stop here
              if (!raw || isNaN(amount)) {
                return;
              }

              // Show based on ranges
              if (amount <= 8333) {
                $('#tr_99999').show();
              } else if (amount >= 8334 && amount <= 20000) {
                $('#tr_100000').show();
              } else if (amount >= 20001 && amount <= 50000) {
                $('#tr_240001').show();
              } else if (amount > 50000) {
                $('#tr_600000').show();
              }
            }
          });

       function AddDetails(){

         const objRentDetail = {
            _strFINEMPRENTDTLID:$('#hRADetails_ActionType').val()=="UPDATE"?parseFloat($('#hRADetails__strFINEMPRENTDTLID').val()):0 ,              // fixed as in your C# code
            strgridindex: 1,                     // fixed as in your C# code
              SLNO:parseFloat($("#hRADetails_SLNO").val()),
           // Top-level HRA details
           FinPeriodStartDate:"01-"+$('#hRADetails_PeriodFromMonth option:selected').text(),
           FinPeriodEndDate:  "01-"+$('#hRADetails_PeriodToMonth option:selected').text(),
           MonthlyRentAmount:  $('#hRADetails_MonthlyRentAmount').val(), // or getInt(...) if strictly numeric
              EmployeeSO:  $('#hRADetails_EmployeeSO').val(), // or getInt(...) if strictly numeric
           CityCategory:       $('#hRADetails_CityCategory').val(),
           PresentResidentialAddress:$('#hRADetails_PresentResidentialAddress').val(),
           CheckBox0:$("#chkbox0").prop("checked"),
           CheckBox1:$("#chkbox1").prop("checked"),
           CheckBox2:$("#chkbox2").prop("checked"),
           CheckBox3:$("#chkbox3").prop("checked"),
          MultipleLandlord:$("#chkmultilandlord").prop("checked"),
           TotalRentAmount:$('#txtrentamount1').val(),
           ActionType:$('#hRADetails_ActionType').val(),
           // Landlord 1
           LandLord1: {
             LandlordName:       $('#hRADetails_LandLord1_LandlordName').val(),
             LandlordPAN:        $('#hRADetails_LandLord1_LandlordPAN').val(),
             LandlordSO:         $('#hRADetails_LandLord1_LandlordSO').val(),
             LandlordResHNo:     $('#hRADetails_LandLord1_LandlordResHNo').val(),
             LandlordResGNo:     $('#hRADetails_LandLord1_LandlordResGNo').val(),
             LandlordResVillage: $('#hRADetails_LandLord1_LandlordResVillage').val(),
             LandlordResCity:    $('#hRADetails_LandLord1_LandlordResCity').val(),
             LandlordResPincode: $('#hRADetails_LandLord1_LandlordResPincode').val(),
             LandlordMobileNo:   $('#hRADetails_LandLord1_LandlordMobileNo').val()
           },

           // Landlord 2
           LandLord2: {
             LandlordName:       $('#hRADetails_LandLord2_LandlordName').val(),
             LandlordPAN:        $('#hRADetails_LandLord2_LandlordPAN').val(),
             LandlordSO:         $('#hRADetails_LandLord2_LandlordSO').val(),
             LandlordResHNo:     $('#hRADetails_LandLord2_LandlordResHNo').val(),
             LandlordResGNo:     $('#hRADetails_LandLord2_LandlordResGNo').val(),
             LandlordResVillage: $('#hRADetails_LandLord2_LandlordResVillage').val(),
             LandlordResCity:    $('#hRADetails_LandLord2_LandlordResCity').val(),
             LandlordResPincode: $('#hRADetails_LandLord2_LandlordResPincode').val(),
             LandlordMobileNo:   $('#hRADetails_LandLord2_LandlordMobileNo').val()
           },

           // Landlord 3
           LandLord3: {
             LandlordName:       $('#hRADetails_LandLord3_LandlordName').val(),
             LandlordPAN:        $('#hRADetails_LandLord3_LandlordPAN').val(),
             LandlordSO:         $('#hRADetails_LandLord3_LandlordSO').val(),
             LandlordResHNo:     $('#hRADetails_LandLord3_LandlordResHNo').val(),
             LandlordResGNo:     $('#hRADetails_LandLord3_LandlordResGNo').val(),
             LandlordResVillage: $('#hRADetails_LandLord3_LandlordResVillage').val(),
             LandlordResCity:    $('#hRADetails_LandLord3_LandlordResCity').val(),
             LandlordResPincode: $('#hRADetails_LandLord3_LandlordResPincode').val(),
             LandlordMobileNo:   $('#hRADetails_LandLord3_LandlordMobileNo').val()
           },

           // Landlord 4
           LandLord4: {
             LandlordName:       $('#hRADetails_LandLord4_LandlordName').val(),
             LandlordPAN:        $('#hRADetails_LandLord4_LandlordPAN').val(),
             LandlordSO:         $('#hRADetails_LandLord4_LandlordSO').val(),
             LandlordResHNo:     $('#hRADetails_LandLord4_LandlordResHNo').val(),
             LandlordResGNo:     $('#hRADetails_LandLord4_LandlordResGNo').val(),
             LandlordResVillage: $('#hRADetails_LandLord4_LandlordResVillage').val(),
             LandlordResCity:    $('#hRADetails_LandLord4_LandlordResCity').val(),
             LandlordResPincode: $('#hRADetails_LandLord4_LandlordResPincode').val(),
             LandlordMobileNo:   $('#hRADetails_LandLord4_LandlordMobileNo').val()
           }
         };


                                  $.ajax({
                     type: "POST",
                     beforeSend: function () {
                         $("#ajaxLoader").addClass('loader');
                     },
                     url: "/Finance/AddHRADetails",
                       data: JSON.stringify(objRentDetail),
                       contentType: "application/json; charset=utf-8",
                       datatype: "json",
                     success: function (data) {
                         if(data.Item1=="error"){
                             $("#tr_errorhra").show();
                          $("#lblerror_hra").html(data.Item2);}else{
                              alert('Rent Details Added Successfully');
                              $('#HRAModal').modal('hide');
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
        function DeleteDetails(currnt){
            debugger;
         var data={
              KIID: $("#ddlki").val(),
                Id:parseFloat($(currnt).attr('data-value')),
         }


                                   $.ajax({
                      type: "POST",
                      beforeSend: function () {
                          $("#ajaxLoader").addClass('loader');
                      },
                       url: "/Finance/DeleteHRADetails",
                         data: data,
                        // contentType: "application/json; charset=utf-8",
                        // datatype: "json",
                      success: function (data) {
                         $("#modelHead").html("HRA");
                               $('#HRAModal').modal('hide');
                               $("#modelHead").html("HRA");
                                   $('#divModel').html(data);



          $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
          $('#HRAModal').modal('show'); // then show it
          $(".pnlmultilandlord").hide();
                           
                      },
                      complete: function () {
                          $("#ajaxLoader").removeClass('loader');
                      },
                      error: function () {
                          sweetAlert("Oops...", "Something went wrong!", "error");
                      }
                  });
        }
         function EditDetails(currnt){
             debugger;
          var data={
               KIID: $("#ddlki").val(),
                 Id:parseFloat($(currnt).attr('data-value')),
          }


                                    $.ajax({
                       type: "POST",
                       beforeSend: function () {
                           $("#ajaxLoader").addClass('loader');
                       },
                        url: "/Finance/EditHRADetails",
                          data: data,
                         // contentType: "application/json; charset=utf-8",
                         // datatype: "json",
                       success: function (data) {
                          $("#modelHead").html("HRA");
                                $('#HRAModal').modal('hide');
                                $("#modelHead").html("HRA");
                                    $('#divModel').html(data);



           $('#HRAModal').modal({ backdrop: 'static', keyboard: false });
           $('#HRAModal').modal('show'); // then show it
           $(".pnlmultilandlord").hide();

                       },
                       complete: function () {
                           $("#ajaxLoader").removeClass('loader');
                       },
                       error: function () {
                           sweetAlert("Oops...", "Something went wrong!", "error");
                       }
                   });
         }
         function SaveDetails(){
             
          var data={
                 AnnualRent: $("#txtrentamount1").val(),
                  
          }


                                    $.ajax({
                       type: "POST",
                       beforeSend: function () {
                           $("#ajaxLoader").addClass('loader');
                       },
                         url: "/Finance/SubmitHRADetails",
                          data: data,
                         // contentType: "application/json; charset=utf-8",
                         // datatype: "json",
                       success: function (data) {
                            if(data.Item1=="error"){
                              $("#tr_errorhra").show();
                           $("#lblerror_hra").html(data.Item2);
                            }
                            else{
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

