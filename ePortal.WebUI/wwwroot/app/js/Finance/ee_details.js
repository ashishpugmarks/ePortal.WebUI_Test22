     $(document).ready(function () {
              $('.dpDate').datepicker({
               format: "dd-M-yyyy",
               todayHighlight: true,
               autoclose: true,

              });
         ValidateLoansanction();
           $("[data-ctrl-id='btncancelEE']").on('click', function (e) {
           $('#HRAModal').modal('hide');
           });
            $("[data-ctrl-id='btnsubmitEE']").on('click', function (e) {
          AddDetails();
           });
                    $("[data-ctrl-id='Loansanctiondate']").off('change').on('change', function (e) {
                        debugger;
                        e.stopImmediatePropagation(); // prevents multiple triggers
              ChangeLoansanctiondate($(this));
            });
              $("[data-ctrl-id='validateLoan']").on('change', function (e) {
                        ValidateLoansanction();
              });
         $("#Declamount").on('change', function (e) {
             EEAmount();
         });
            }
);


        // Bind on both input and change to catch typing and blur
function EEAmount() {
    debugger;
    var percentStr = $('#Perofshareintax').val().trim();
    var declStr = $('#Declamount').val().trim();

    // If percent is empty: alert and focus
    if (percentStr === '') {
        $('#Perofshareintax').focus();
        alert('Please fill Percentage of Share in Tax exemption');
        return;
    }

    // If both have values, compute; else clear
    if (percentStr !== '' && declStr !== '') {
        var percent = parseFloat(percentStr);
        var decl = parseFloat(declStr);

        if (isNaN(percent) || isNaN(decl)) {
            // Invalid numbers → clear and show modal anyway (matching server behavior)
            $('#EEamount').val('');
        } else {
            var amt = (decl * percent) / 100;
            if (amt > 50000) amt = 50000;
            // Format as integer like your server code (Convert.ToInt64)
            $('#EEamount').val(Math.floor(amt));
        }
    } else {
        $('#EEamount').val('');
    }


}



function ValidateLoansanction() {
    debugger;
            if ($("#Loansanctionamount").val() !== "") {
                if (parseFloat($("#Loansanctionamount").val()) <= 3500000) {
                    enableFields();
                   
                } else {
                    disableFields();
                     $("#Loansanctionamount").prop("disabled", false);
                    alert("You are not eligible to get benefit U/S 80EE");
                    return;
                }
            }

            if ($("#Vofhouse").val() !== "") {
                if (parseFloat($("#Vofhouse").val()) <= 5000000) {
                    enableFields();
                   
                } else {
                    disableFields();
                    $("#Vofhouse").prop("disabled", false);
                    $("#Loansanctionamount").prop("disabled", false);
                    $("#Loansanctiondate").prop("disabled", false);
                    alert("You are not eligible to get benefit U/S 80EE");
                    return;
                }
            }

            if ($("#Ownerofoterhhouse").val() !== "") {
                debugger;
                if ($("#Ownerofoterhhouse").val() === "0") {
                    enableFields();
                   
                } else {
                    disableFields();
                    
                    $("#Ownerofoterhhouse").prop("disabled", false);
                    $("#Loansanctionamount").prop("disabled", false);
                    $("#Loansanctiondate").prop("disabled", false);
                    $("#Vofhouse").prop("disabled", false);
                    alert("You are not eligible to get benefit U/S 80EE");
                    return;
                }
            }

            if ($("#Amtclaimed24b").val() !== "") {
                if ($("#Amtclaimed24b").val() === "0") {
                    enableFields();
                   
                } else {
                    $("#Perofshareproperty").prop("disabled", true);
                    $("#Perofshareintax").prop("disabled", true);
                    $("#Declamount").prop("disabled", true);
                    alert("You are not eligible to get benefit U/S 80EE");
                    return;
                }
            }

            if ($("#Popunderconstr").val() !== "") {
                if ($("#Popunderconstr").val() === "1") {
                    $("#Compofconstdate").prop("disabled", true);
                    return;
                } else {
                    enableFields();
                    return;
                }
            }

            if ($("#Ownerofproperty").val() !== "") {
                debugger;
                if ($("#Ownerofproperty").val() === "1") {
                    $("#Perofshareproperty").val("100");
                    $("#Perofshareintax").val("100");
                     $("#Perofshareproperty").prop("disabled", true);
                    $("#Perofshareintax").prop("disabled", true);
                    if ($("#Perofshareintax").val() !== "" && $("#Declamount").val() !== "") {
                        var Amt = (parseFloat($("#Declamount").val()) * parseFloat($("#Perofshareintax").val())) / 100;
                        $("#EEamount").val(Amt > 50000 ? "50000" : Amt);
                    } else {
                        $("#EEamount").val("");
                    }
                    return;
                } else {
                    $("#Perofshareproperty").val("");$("#Perofshareproperty").prop("disabled", false);
                    $("#Perofshareintax").val("");$("#Perofshareintax").prop("disabled", false);
                }
            }
        }

         function ChangeLoansanctiondate (e) {
                var inputDateStr = e.val(); // e.g. "15-Dec-2016"
                var format = "DD-MMM-YYYY"; // moment.js format

                // Parse dates using moment.js (recommended for dd-MMM-yyyy format)
                var inputDate = moment(inputDateStr, format);
                var currentDate = moment().startOf("day");
                var startDate = moment("01-Apr-2016", format);
                var endDate = moment("31-Mar-2017", format);

                if (!inputDate.isValid()) {
                    alert("Invalid date format. Please use dd-MMM-yyyy.");
                    return;
                }

                // Check if greater than current date
                if (inputDate.isAfter(currentDate)) {
                    alert("Date of Loan Sanctioned is not greater than current date");
                   return;
                }

                // Check if outside eligibility window
                if (inputDate.isBefore(startDate) || inputDate.isAfter(endDate)) {
                    disableFields();
                    alert("You are not eligible to get benefit U/S 80EE");
                   return;
                }

                // Otherwise enable fields
                enableFields();

            };

            function disableFields() {
                $(".ctrl")
                    .prop("disabled", true);
            }

            function enableFields() {
                $(".ctrl").prop("disabled", false);}

                   function AddDetails(){
        
          var EEDetail = {
        TaxId: $("#TaxId").val(),
        Loansanctiondate: $("#Loansanctiondate").val(),
        Loansanctionamount: $("#Loansanctionamount").val(),
        Vofhouse: $("#Vofhouse").val(),
        Ownerofoterhhouse: $("#Ownerofoterhhouse").val(),
        Ownerofproperty: $("#Ownerofproperty").val(),
        Popunderconstr: $("#Popunderconstr").val(),
        Compofconstdate: $("#Compofconstdate").val(),
        Amtclaimed24b: $("#Amtclaimed24b").val(),
        Perofshareproperty: $("#Perofshareproperty").val(),
        Perofshareintax: $("#Perofshareintax").val(),
        Declamount: $("#Declamount").val(),
        EEamount: $("#EEamount").val()
        }


                                   $.ajax({
                      type: "POST",
                      beforeSend: function () {
                          $("#ajaxLoader").addClass('loader');
                      },
                      url: "/Finance/SubmitEE",
                        data: JSON.stringify(EEDetail),
                        contentType: "application/json; charset=utf-8",
                        datatype: "json",
                      success: function (data) {
                          
                          if(data.Item1=="error"){
                             alert(data.Item2)
                          } else {
                              $("[data-ctrl-id='" + $("#TaxId").val() + "']").val(data);
                               alert('Details Added Successfully');
                               $('#HRAModal').modal('hide');
                              CalculateTotal();
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

