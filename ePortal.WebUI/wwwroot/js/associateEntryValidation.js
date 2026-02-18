function checkEmail() {
    if (document.getElementById("EMAIL_IN").value == "")
        return true;
    if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(document.getElementById("EMAIL_IN").value))
        return true;
    else
        return false;
}



function TicketDetail() {
    var divTicketDetailsVal = $(".divTicketDetails");
    var divStayDetail = $(".divStayDetail");

    for (var i = 0; i < divTicketDetailsVal.length ; i++) {
   
        if ($(divTicketDetailsVal[i]).find("#strDate").val() != "") {

            if ($(divTicketDetailsVal[i]).find("#strDate").val() < $("#startDatePicker").val() ||
            $(divTicketDetailsVal[i]).find("#strDate").val() > $("#endDatePicker").val()) {
                ShowMessage("Travel Date should be in Tour Period.", 'Error');
                $(divTicketDetailsVal[i]).find("#TourStartDate").focus();
                return false
            }
        }
        if ($(divTicketDetailsVal[i]).find("#FromLocCode").val() != "") {

            if ($(divTicketDetailsVal[i]).find("#FromLocCode").val() == $(divTicketDetailsVal[i]).find("#ToLoc").val()) {
                ShowMessage("Travel Should not Be On Same City.", 'Error');
                $(divTicketDetailsVal[i]).find("#ToLoc").focus();
                return false
            }
        }
        debugger;
        if ($(divTicketDetailsVal[i]).find("#ModeID").val() != "") {

            if ($(divTicketDetailsVal[i]).find("#ModeID").val() != "Bus") {
                if ($(divTicketDetailsVal[i]).find("#TicketClasssID").val()== "") {
                    ShowMessage("Please select Class.", 'Error');
                    $(divTicketDetailsVal[i]).find("#TicketClasssID").focus();
                    return false
                }
            }
        }

    }



    
    for (var i = 0; i < divStayDetail.length ; i++) {

        if ($(divStayDetail[i]).find("#TourStartDate").val() != "") {

            if ($(divStayDetail[i]).find("#TourStartDate").val() < $("#startDatePicker").val() ||
            $(divStayDetail[i]).find("#TourEndDate").val() > $("#endDatePicker").val()) {
                ShowMessage("Stay Date should be in Tour Period.", 'Error');
                $(divStayDetail[i]).find("#TourStartDate").focus();
                return false
            }
        }

        debugger;

            if ($(divStayDetail[i]).find("#StayingLoc").val() != "") {

                if ($(divStayDetail[i]).find("#FromLoc").val() != "") {

                    ShowMessage("Stay City and other city only select one at same time.", 'Error');
                    $(divStayDetail[i]).find("#FromLoc").focus();

                    return false
                }


            }
        



    }

    return true;
}










function submitval() {
    //var fromdate = new Date($("#startDatePicker").val());

    var fromdate = $("#startDatePicker").val();
    var todate = $("#endDatePicker").val();
    //var todate = new Date($("#endDatePicker").val());
    var traveldate = new Date($("#TourStartDate").val());


    var fromStaydate = new Date($("#TourStartDate").val());
    var toStaydate = new Date($("#TourEndDate").val());

    var currdate = new Date();

    var today = new Date(currdate.getFullYear(), currdate.getMonth(), currdate.getDate());

  
    if (fromdate == "") {
        ShowMessage("From Date is required field", 'Error');
        document.getElementById("FromLocCode").focus();
        return false
    }

    if (todate == "") {
        ShowMessage("To Date is required field", 'Error');
        document.getElementById("FromLocCode").focus();
        return false
    }


    if (fromStaydate > toStaydate) {
        //document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
        //  document.getElementById("<%=status.ClientID%>").innerHTML = "Tour Start Date should be less than equal to Tour End Date.";
        ShowMessage("Invalid date! please select valid travel date.", 'Error');
        document.getElementById("TourStartDate").focus();
        return false
    }


    if (fromdate > todate) {
        //document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
        //  document.getElementById("<%=status.ClientID%>").innerHTML = "Tour Start Date should be less than equal to Tour End Date.";
        ShowMessage("Tour Start Date should be less than equal to Tour End Date.", 'Error');
        document.getElementById("STARTDATE_IN").focus();
        return false
    }
    

    //TOUR DATE SHOULD IN TOUR PERIOD
    if (traveldate < fromdate || traveldate > todate) {
        // document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
        //  document.getElementById("<%=status.ClientID%>").innerHTML = "Travel Date should be in Tour Period.";
        ShowMessage("Travel Date should be in Tour Period.", 'Error');
        document.getElementById("TourStartDate").focus();
        return false
    }

    //Tour request open for back date
    //        //TOUR DATE SHOUD BE GREATER THAN EQUAL TO CURRENT DATE
    if (fromdate < today) {
        ShowMessage("Tour Date should be greater than or equal to current date.", 'Error');
        document.getElementById("STARTDATE_IN").focus();
        return false
    }






    var fromdate = $("#startDatePicker").val()
    //var fromdate = new Date(document.getElementById("<%=STARTDATE_IN%>").value);
    var today = new Date();
    var currentdate = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    //MS in 1 day
    var msPerDay = 24 * 60 * 60 * 1000;
    var dbd = Math.floor((fromdate - currentdate) / msPerDay);
    //alert(dbd);
    //debugger;
    if (document.getElementById("MOBILENO_IN").value == "") {
        //document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
        //document.getElementById("<%=status.ClientID%>").innerHTML = "Mobile No. is a required field.";
        ShowMessage("Mobile No. is a required field.", 'Error');
        document.getElementById("MOBILENO_IN").focus();
        return false
    }

    if (checkEmail() == false) {
      //  document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
        //document.getElementById("<%=status.ClientID%>").innerHTML = "Please enter the valid Email ID.";
        ShowMessage("Please enter the valid Email ID.", 'Error');
        document.getElementById("EMAIL_IN").focus();
        return false
    }

    if (document.getElementById("OBJECTIVE_IN").value == "") {
      //  document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
        //   document.getElementById("<%=status.ClientID%>").innerHTML = "Ojective of Journey is a required field.";
        ShowMessage("Ojective of Journey is a required field.", 'Error');
        document.getElementById("OBJECTIVE_IN").focus();
        return false
    }

    //if (document.getElementById("<%=hfAdvRemakrs.ClientID %>").value == "YES" && trim(document.getElementById("<%=txtAdvRemarks.ClientID %>").value) == "") {
    //    document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
    //    document.getElementById("<%=status.ClientID%>").innerHTML = "Previous Advance Remarks is required field in case of pending settlement.";
    //    document.getElementById("<%=txtAdvRemarks.ClientID %>").focus();
    //    return false
    //}

    if (document.getElementById("AdvRequired").value == "") {
       // document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
        //  document.getElementById("<%=status.ClientID%>").innerHTML = "Required Tour Advance is required field.";
        ShowMessage("Required Tour Advance is required field.", 'Error');
        document.getElementById("AdvRequired").focus();
        return false
    }
   
   
    if ($("#EMPCODE_IN").val() == "") {
      //  document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
        //  document.getElementById("<%=status.ClientID%>").innerHTML = "Approval authority is required field.";
        ShowMessage("Approval authority is required field.", 'Error');
        $("#EMPCODE_IN").val().focus();
        return false
    }

    var divTicketDetails = $(".divTicketDetails");
    var divStayDetail = $(".divStayDetail");

    if (divTicketDetails.length == 1 && divStayDetail.length == 1) {
        ShowMessage("Ticket and stay Deatil is required field", 'Error');
        return false

    }

    for (var i = 0; i < divTicketDetails.length - 1; i++) {

        if (($(divTicketDetails[i]).find("#FromLocCode").val() == "") || ($(divTicketDetails[i]).find("#FromLocCode").val() == "" && $(divTicketDetails[i]).find("#FromLocCode").val() == "")) {
            ShowMessage("From City is required field", 'Error');
            $(divTicketDetails[i]).find("#FromLocCode").val().focus();
            return false
        }

        if (($(divTicketDetails[i]).find("#ToLoc").val() == "")) {
            ShowMessage("To City is required field", 'Error');
            $(divTicketDetails[i]).find("#ToLoc").val().focus();
            return false
        }


        //TRAVEL MODE REQUIRE WHEN FROM TO CITY ARE NOT SAME
        if ($(divTicketDetails[i]).find("#FromLocCode").val() != $(divTicketDetails[i]).find("#FromLocCode").val()) {
            if ($(divTicketDetails[i]).find("#ModeID").val() == "") {
                ShowMessage("Travel Mode is required field.", 'Error');
                $(divTicketDetails[i]).find("#ModeID").val().focus();
                return false
            }
        }


        //CHECK FOR TICKET CLASS IN CASE OF TRAIN AND FLIGHT(1=AIR,3=TRAIN
        if (($(divTicketDetails[i]).find("#ModeID").val() == "1" || $(divTicketDetails[i]).find("#ModeID").val() == "3") && ($(divTicketDetails[i]).find("#TicketClasssID").val() == "")) {
            ShowMessage("Travel class is required field for selected travel mode.", 'Error');
            $(divTicketDetails[i]).find("#TicketClasssID").val().focus();
            return false
        }


        if (($(divTicketDetails[i]).find("#ModeID").val() == "1" || $(divTicketDetails[i]).find("#ModeID").val() == "3") && ($(divTicketDetails[i]).find("#ModeDetail").val() == "")) {

            ShowMessage("FLT/Train Name is a required field for selected travel mode.", 'Error');
            $(divTicketDetails[i]).find("#ModeDetail").val().focus();
            return false
        }

        if ($(divTicketDetails[i]).find("#TickingBy").val() == "") {

            ShowMessage("Accommodation is required field.", 'Error');
            $(divTicketDetails[i]).find("#TickingBy").val().focus();
            return false
        }
    }





    //Honda query
    debugger;
    if (document.getElementById("AdvRequired").value == true) {
        if (document.getElementById("MiscAmount").value != "" && document.getElementById("MiscRemarks").value == "") {
            //   document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
            //   document.getElementById("<%=status.ClientID%>").innerHTML = "Remarks for Miscellaneous Amount is required if amount filled by associate.";
            ShowMessage("Remarks for Miscellaneous Amount is required if amount filled by associate.", 'Error');
            document.getElementById("MiscAmount").focus();
            return false
        }
    }
    //Honda query
    //if (dbd <= 3)
    //    alert("Request generated before 3 days of actual travel. so special approval is added with this request.");
    return true;

   // isValidEntry();
}

function isValidEntry() {
   
   
    //TRIM THE VALUE OF DAY OBJECTIVE
    //if (trim(document.getElementById("<%=txtDailyObjective.ClientID %>").value) == "") {
    //    document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
    //    document.getElementById("<%=status.ClientID%>").innerHTML = "Day objective is required field";

    //    document.getElementById("<%=txtDailyObjective.ClientID %>").focus();
    //    return false
    //}
  
  

    var divTicketDetails = $(".divTicketDetails");
    var divStayDetail = $(".divStayDetail");

    if (divTicketDetails.length == 1 && divStayDetail.length==1)
    {
        ShowMessage("Ticket and stay Deatil is required field", 'Error');
    }
   

        for (var i = 0; i < divTicketDetails.length - 1; i++) {

            if (($(divTicketDetails[i]).find("#FromLocCode").val() == "") || ($(divTicketDetails[i]).find("#FromLocCode").val() == "" && $(divTicketDetails[i]).find("#FromLocCode").val() == "")) {
                ShowMessage("From City is required field", 'Error');
                $(divTicketDetails[i]).find("#FromLocCode").val().focus();
                return false
            }

            if (($(divTicketDetails[i]).find("#ToLoc").val() == "")) {
                ShowMessage("To City is required field", 'Error');
                $(divTicketDetails[i]).find("#ToLoc").val().focus();
                return false
            }


            //TRAVEL MODE REQUIRE WHEN FROM TO CITY ARE NOT SAME
            if ($(divTicketDetails[i]).find("#FromLocCode").val() != $(divTicketDetails[i]).find("#FromLocCode").val()) {
                if ($(divTicketDetails[i]).find("#ModeID").val() == "") {
                    ShowMessage("Travel Mode is required field.", 'Error');
                    $(divTicketDetails[i]).find("#ModeID").val().focus();
                    return false
                }
            }


            //CHECK FOR TICKET CLASS IN CASE OF TRAIN AND FLIGHT(1=AIR,3=TRAIN
            if (($(divTicketDetails[i]).find("#ModeID").val() == "1" || $(divTicketDetails[i]).find("#ModeID").val() == "3") && ($(divTicketDetails[i]).find("#TicketClasssID").val() == "")) {
                ShowMessage("Travel class is required field for selected travel mode.", 'Error');
                $(divTicketDetails[i]).find("#TicketClasssID").val().focus();
                return false
            }


            if (($(divTicketDetails[i]).find("#ModeID").val() == "1" || $(divTicketDetails[i]).find("#ModeID").val() == "3") && ($(divTicketDetails[i]).find("#ModeDetail").val() == "")) {

                ShowMessage("FLT/Train Name is a required field for selected travel mode.", 'Error');
                $(divTicketDetails[i]).find("#ModeDetail").val().focus();
                return false
            }

            if ($(divTicketDetails[i]).find("#TickingBy").val() == "") {

                ShowMessage("Accommodation is required field.", 'Error');
                $(divTicketDetails[i]).find("#TickingBy").val().focus();
                return false
            }
        }

  



      


    
    //by honda
    //if (document.getElementById("HotelReserv").value == "1" && document.getElementById("<%=cmbPickDrop.ClientID %>").value == "") {
    //    //alert("Please Enter a Title")
    //    document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
    //    document.getElementById("<%=status.ClientID%>").innerHTML = "Pick-up Drop is a required field in selected Reservation case";
    //    document.getElementById("<%=cmbPickDrop.ClientID %>").focus();
    //    return false
    //}

    

    //if (document.getElementById("<%=chkSpecialApp.ClientID %>").checked == true && trim(document.getElementById("<%=txtRemarks.ClientID %>").value) == "") {
    //    //alert("Please Enter a Title")
    //    document.getElementById("<%=errorpanel.ClientID%>").style.display = "inline";
    //    document.getElementById("<%=status.ClientID%>").innerHTML = "Remarks is Required in Case of Special Approval.";
    //    document.getElementById("<%=txtRemarks.ClientID %>").focus();
    //    return false
    //}

    return true;
}



//Honda MAKE TEXT BOX ENABLE INCASE OF OTHER CITY(FROM CITY)
//function SetFromCity() {
//    if (document.getElementById("FromLocCode").value == "") {
//        document.getElementById("<%=txtFromCity.ClientID %>").disabled = false;
//        document.getElementById("<%=txtFromCity.ClientID %>").focus();
//    }
//    else {
//        document.getElementById("<%=txtFromCity.ClientID %>").disabled = true;
//        document.getElementById("<%=txtFromCity.ClientID %>").value = "";
//        document.getElementById("FromLocCode").focus();

//    }
//}



//MAKE TEXT BOX ENABLE INCASE OF OTHER CITY(STAYING CITY)
function SetStayCity() {
    if (document.getElementById("StayingLoc").value == "") {
        document.getElementById("FromLoc").disabled = false;
        document.getElementById("FromLoc").focus();
    }
    else {
        document.getElementById("FromLoc").disabled = true;
        document.getElementById("FromLoc").value = "";
        document.getElementById("StayingLoc").focus();

    }
}

function CalculateAmount() {
 
    var MiscAmount = 0;
    var StayCharge = 0;
    var Allowances = 0;
    var TotalAmount = 0;
    if (document.getElementById("MiscAmount").value != "")
        MiscAmount = document.getElementById("MiscAmount").value;

    StayCharge = $('#TotalCharges').html();
    Allowances = $('#TotalAllowances').html();
    //alert(StayCharge)
    TotalAmount = parseFloat(StayCharge) + parseFloat(Allowances) + parseFloat(MiscAmount);
    document.getElementById("TotalAmount").innerHTML = TotalAmount.toFixed(2);
    document.getElementById("RequiredAmount").value = TotalAmount.toFixed(2);
}

//CHECK REQUIRED AMOUNT IS LESS THAN EQUAL TO SYSTEM GENERATED AMOUNT
function ReqAmountCHK() {
   
    var TotalAmount = document.getElementById("TotalAmount").innerHTML;
    var ReqAmount = document.getElementById("RequiredAmount").value;

    if (parseFloat(ReqAmount) > parseFloat(TotalAmount)) {
        document.getElementById("RequiredAmount").value = document.getElementById("TotalAmount").innerHTML;
        alert('Required amount should be less than equal to System generated amount');
        document.getElementById("RequiredAmount").focus();
    }
}