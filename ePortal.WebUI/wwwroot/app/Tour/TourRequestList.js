$('.homeContent').removeClass('homeContent'); 

$(document).ready(function () {
    $('#trGrid').hide();
    $('#trPrint').hide();
    $("#cmdSearch").on("click", function (e) {
        e.preventDefault(); 
            cmdSearch_Click(); 
    });
    $("#cmdPrint").on("click", function (e) {
        e.preventDefault(); 
            cmdPrint_Click(); 
    });
    $("#cmdExport").on("click", function (e) {
        e.preventDefault();
        var res = Search();
        cmdExport_Click(res);
    });
    $("#imgPickFromDate").on("click", function () {
        showfromdate();
    });
    $("#imgRemoveFromDate").on("click", function () {
        removefromdate();
    });

    $("#imgPickToDate").on("click", function () {
        showtodate();
    });
    $("#imgRemoveToDate").on("click", function () {
        removetodate();
    }); 
});



function showfromdate() {
    NewCal('txtFromDate', 'ddmmmyyyy');
}

function showtodate() {
    NewCal('txtToDate', 'ddmmmyyyy');
}

function removefromdate() {
    document.getElementById('txtFromDate').value = "";
}

function removetodate() {
    document.getElementById('txtToDate').value = "";
}

function showreport(groupID) {
    var url = "/TourDLGS/PrintableList?id=" + groupID + "&dup=N";
    window.open(url, "Tour Request", "height=600,width=1000,status=0,resizable=0,scrollbars=1");
}

function showdetail(RequestID) {
    var url = "/TourDLGS/RequestForm?id=" + RequestID;
    window.open(url, "Tour Request", "height=550,width=800,status=0,resizable=0,scrollbars=1");
}




async function cmdSearch_Click() {
    $('#trGrid').show();
    $('#trPrint').hide();
    await Search();
}
async function Search() { 
    $.ajax({
        type: 'GET', 
        data : {
            RequestID: $('#txtRequestID').val().trim(),
            EmpCode: $('#txtEmpCode').val().trim(),
            EmpName: $('#txtEmpName').val().trim(),
            RequestStatus: $('#cboStatus').val(),
            HardCopyStatus: $('#cboHardCopy').val(),
            FromDate: $('#txtFromDate').val(),
            TillDate: $('#txtToDate').val(),
            AdvanceRequired: $('#cboAdvance').val(),
            FilterType: $('#cboFilter').val()
        },
        success: function (response) {
            var res = JSON.parse(response.result);
            renderRequestListTable(res);
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}

function renderRequestListTable(requests) {
    $('#grdRequestList').show();
    var tbody = $('#grdRequestList tbody');
    tbody.empty();
    if (requests && requests.length > 0) { 
        $('#cmdPrint').show();
        $.each(requests, function (index, item) {
            // Determine checkbox state
            var chkDisabled = (item.REQUESTSTATUS.trim() === "Approved" && item.ISADVANCEREQ.trim() === "Yes") ? "" : "disabled";

            // Determine Change Approval image
            var changeApprovalImg = "";
            if (item.REQUESTSTATUS.trim() === "Approved") {
                changeApprovalImg = '<img id="btnCngApproval" src="~/images/icon_edit_bg.png" />';
            } else if (item.REQUESTSTATUS.trim() === "Pending") {
                changeApprovalImg = '<img id="btnCngApproval" src="~/images/icon_edit.gif" />';
            }

            var row = `
                <tr>
                    <td>
                        <input type="checkbox" class="chkPrint" value="${item.ADTOURREQUESTID}" ${chkDisabled} />
                    </td>
                    <td>${item.ADTOURREQUESTID}</td>
                    <td>${item.ADEMPCODE}</td>
                    <td>${item.EMPNAME}</td>
                    <td>${item.REQUESTDATE}</td>
                    <td>${item.STARTDATE}</td>
                    <td>${item.ENDDATE}</td>
                    <td>${item.ISADVANCEREQ}</td>
                    <td>${item.REQUESTSTATUS}</td>
                    <td>
                        <img ID="btnShow" src="~/images/icon_details.gif" ToolTip="Request Details" />
                    </td>
                    <td>${changeApprovalImg}</td>
                </tr>`;
            tbody.append(row);
        });
    } else {
        $('#cmdPrint').hide();
        tbody.append('<tr><td colspan="11" style="text-align:center;">No record found.</td></tr>');
    }
}

function cmdPrint_Click() {
    var selectedIds = []; 
    $('.chkPrint:checked').each(function () {
        selectedIds.push($(this).val());
    });

    if (selectedIds.length === 0) {
        alert('Select the records from the list using checkbox');
        return;
    } 
    $.ajax({
        url: '/TourRequest/UpdatePrintStatus',  
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ requestList: selectedIds }),
        success: function (response) { 
            var res = Search();
            renderRequestListTable(res);
            showreport(response.groupID);
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
} 


function cmdExport_Click(requests) {
    if (!requests || requests.length === 0) {
        alert('No data to export');
        return;
    }

    // Define headers
    var headers = [
        "Request ID", "Employee Code", "Employee Name", "Designation", "Objective",
        "From Date", "To Date", "Days", "Nights", "Daily Allowance", "Stay Charge",
        "Misc Amount", "Total Amount", "Recommended By", "Recommendation Date",
        "Approved By", "Approval Date", "Hard Copy Status"
    ];

    // Build CSV content
    var csvContent = headers.join(",") + "\n";

    requests.forEach(function (item) {
        var row = [
            item.ADTOURREQUESTID,
            item.ADEMPCODE,
            item.EMPNAME,
            item.DESIGNATION,
            item.OBJECTIVE,
            item.FROMDATE,
            item.TODATE,
            item.DAYS,
            item.NIGHTS,
            item.DAILYALLOWANCE,
            item.STAYCHARGE,
            item.MISCAMOUNT,
            item.TOTALAMOUNT,
            item.RECOMMENDEDBY,
            item.RECOMMENDATIONDATE,
            item.APPROVEDBY,
            item.APPROVALDATE,
            item.HARDCOPYSTATUS
        ];
        csvContent += row.join(",") + "\n";
    });

    // Trigger download
    var blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = "TourRequestList_" + new Date().toISOString().slice(0, 10) + ".csv";
    link.click();
}



