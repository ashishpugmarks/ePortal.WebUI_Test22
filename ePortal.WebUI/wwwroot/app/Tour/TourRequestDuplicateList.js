$('.homeContent').removeClass('homeContent');  
$(document).ready(function () {
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

function showreport(groupID) {
    var url = "/TourDLGS/PrintableList?id=" + groupID + "&dup=Y";
    window.showModalDialog(url, "Tour Request", "dialogHeight:600px;dialogWidth:1000px;status:0;resizable:0;edge:raised;scroll:1");
}


async function cmdSearch_Click() {
    await Search();
}
function Search() {
    $.ajax({
        url: '/TourRequest/SearchTourRequestDuplicateList',
        type: 'GET',
        data: {
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
                    <td>${item.PRINTGROUPNUMBER}</td>
                    <td>${item.PRINTDATE}</td>
                    <td>${item.EMPNAME}</td>
                    <td>
                    <button type="button" class="print-item" data-group="${item.PRINTGROUPNUMBER}"><img src="~/images/icon_fprint.gif" alt="Print" /></button>
                    </td> 
                </tr>`;
            tbody.append(row);
        });
    } else {
        tbody.append('<tr><td colspan="5" style="text-align:center;">No record found.</td></tr>');
    }
}