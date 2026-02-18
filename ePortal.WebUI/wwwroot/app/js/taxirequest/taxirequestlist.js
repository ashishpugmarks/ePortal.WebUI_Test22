function openPopup(strOpenUrl) {
    window.open(strOpenUrl, "mywindow", "TOOLBAR=no,MENUBAR=no,SCROLLBARS=no,RESIZABLE=no,LOCATION=no,DIRECTORIES=no,STATUS=no,width=770,height=390");
}
$(document).ready(function () {
    $("#tblTaxiList").dataTable({
        "bFilter": false,
        "bLengthChange": false,
        "info": false
    });
    //if (status == 9) {
    //$(".row")[9].innerHTML += '<button style="float:right;" class="btn btn-success" onclick="uploadDetails()">Update Payment Advise</button>'
    //$(".row")[9].innerHTML += '<button style="float:right;margin-right:4px;" class="btn btn-success" onclick="UploadCSV()">Upload Payment Advise</button>'
    //}
});