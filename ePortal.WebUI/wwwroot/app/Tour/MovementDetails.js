$('.homeContent').removeClass('homeContent');  
$(document).ready(function () {
    const userId = parseInt(document.getElementById("hdnUserId").value);  
    $("#btnFromDate").on("click", function () { 
        showfromdate();
    });
    $("#btnReFromDate").on("click", function () {
        removefromdate();
    });
    $("#btnToDate").on("click", function () {
        showtodate();
    });
    $("#btnReToDate").on("click", function () {
        removetodate();
    });
    $("#cmdSearch").on("click", function (e) {
        e.preventDefault();
        cmdSearch_Click();
    });
});

//SET DATES
function showfromdate() {
    NewCal(document.getElementById('txtFromDate'), 'ddmmmyyyy');
}

function showtodate() {
    NewCal(document.getElementById('txtToDate'), 'ddmmmyyyy');
}

//REMOVE DATES
function removefromdate() {
    document.getElementById('txtFromDate').value = "";
}

function removetodate() {
    document.getElementById('txtToDate').value = "";
}



async function cmdSearch_Click() {
    await Search(); 
}
function Search() {
    $.ajax({
        url: '/TourRequest/SearchMovementDetails',
        type: 'GET',
        data: {
            RequestID: ($('#txtRequestID').val() || '').trim(),
            EmpCode: ($('#txtEmpCode').val() || '').trim(),
            EmpName: ($('#txtEmpName').val() || '').trim(),
            FromDate: $('#txtFromDate').val() || '',
            TillDate: $('#txtToDate').val() || ''
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
        $.each(requests, function (index, item) { 
            var row = `
                <tr> 
                    <td>${item.ADTOURREQUESTID}</td>
                    <td>${item.ADEMPCODE}</td>
                    <td>${item.EMPNAME}</td>
                    <td>${item.DESGINATION}</td>
                    <td>${item.DEPARTMENT}</td>
                </tr>`;
            tbody.append(row);
        });
    } else {
        //$('#cmdPrint').hide();
        tbody.append('<tr><td colspan="5" style="text-align:center;">No record found.</td></tr>');
    }
}
