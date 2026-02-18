$('.homeContent').removeClass('homeContent');  
$(document).ready(function () {
    $("#cmdSearch").on("click", function (e) {
        e.preventDefault();
        if (ValidInput()) {
            cmdSearch_Click();
        }
    });
    $("#cmbMonthlyReport").on("click", function (e) {
        e.preventDefault();
        cmbMonthlyReport_Click();

    });
    Search();
});

function ValidInput() {
    if (document.getElementById("cboAssociate").value == "0") {
        alert("Please select any associate to get schedule.");
        return false;
    }
    return true;
}


async function cmdSearch_Click() {
    await Search();
}
function Search() {
    $.ajax({
        url: '/TourRequest/SearchTourSchedulle',
        type: 'GET',
        data: {
            cboAssociate: ($('#cboAssociate').val() || '0').trim(),
            optMonth: ($('#optMonth').val() || '0').trim(),
            optYear: ($('#optYear').val() || '0').trim() 
        },
        success: function (response) {
            var res = JSON.parse(response.result);
            //console.log(res);
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
        const today = new Date();
        $.each(requests, function (index, item) {
            // Determine highlight based on TRAVELDATE and DAYNAME
            const parsedDate = parseDateFlexible(item.TRAVELDATE);
            const dayNameText = String(item.DAYNAME || '').trim().toUpperCase();

            let rowStyle = '';
            // Server logic order: current day first, then Sunday overwrites
            if (parsedDate && isSameDay(parsedDate, today)) {
                rowStyle = 'background-color: PaleGreen;';
            }
            if (dayNameText === 'SUNDAY') {
                rowStyle = 'background-color: MistyRose;';
            }

            var row = `
                <tr style="${rowStyle}">  
                    <td>${sanitizeCell(item.TRAVELDATE)}</td>
                    <td>${sanitizeCell(item.DAYNAME)}</td>
                    <td>${sanitizeCell(item.TRAVELTIME)}</td>
                    <td>${sanitizeCell(item.DAYOBJECTIVE)}</td>
                    <td>${sanitizeCell(item.FRMCITY)}</td>
                    <td>${sanitizeCell(item.TOCITY)}</td>
                    <td>${sanitizeCell(item.STCITY)}</td>
                    <td>${sanitizeCell(item.TRAVELMODE)}</td>
                    <td>${sanitizeCell(item.TICKETCLASS)}</td>
                    <td>${sanitizeCell(item.FLIGHTTRAINNO)}</td>
                    <td>${sanitizeCell(item.TICKETINGBY)}</td>
                </tr>`;
            console.log(row);
            tbody.append(row);
        });
    } else {
        tbody.append('<tr><td colspan="5" style="text-align:center;">No record found.</td></tr>');
    }
}


function sanitizeCell(value) {
    if (value === null || value === undefined) return '&nbsp;';
    const str = String(value).trim();
    if (str === '' || str.toLowerCase() === 'null' || str.toLowerCase() === 'undefined') {
        return '&nbsp;';
    }
    return str;
}

function parseDateFlexible(text) {
    if (!text) return null;
    const s = String(text).trim();

    // If already a valid Date via native parser (ISO etc.), use it.
    const d1 = new Date(s);
    if (!isNaN(d1.getTime())) return d1;

    // Try Oracle style: DD-MON-YYYY (e.g., 30-DEC-2025)
    const m = /^(\d{1,2})-([A-Za-z]{3})-(\d{4})$/.exec(s);
    if (m) {
        const day = parseInt(m[1], 10);
        const monStr = m[2].toUpperCase();
        const year = parseInt(m[3], 10);
        const monMap = {
            JAN: 0, FEB: 1, MAR: 2, APR: 3, MAY: 4, JUN: 5,
            JUL: 6, AUG: 7, SEP: 8, OCT: 9, NOV: 10, DEC: 11
        };
        const month = monMap[monStr];
        if (month !== undefined) {
            const d = new Date(year, month, day);
            if (!isNaN(d.getTime())) return d;
        }
    }

    // Try DD/MM/YYYY
    const m2 = /^(\d{1,2})\/\-\/\-$/.exec(s);
    if (m2) {
        const day = parseInt(m2[1], 10);
        const month = parseInt(m2[2], 10) - 1;
        const year = parseInt(m2[3], 10);
        const d = new Date(year, month, day);
        if (!isNaN(d.getTime())) return d;
    }
    return null;
} 

function isSameDay(d1, d2) {
    return d1 && d2 &&
        d1.getFullYear() === d2.getFullYear() &&
        d1.getMonth() === d2.getMonth() &&
        d1.getDate() === d2.getDate();
}


function cmbMonthlyReport_Click() {   
    $.ajax({
        url: '/TourRequest/CmbMonthlyReport_Click',
        type: 'GET',
        data: {
            UserID: ($('#cboAssociate').val() || '0').trim(),
            MonthNum: ($('#optMonth').val() || '0').trim(),
            YearNum: ($('#optYear').val() || '0').trim()
        },
        success: function (response) {
            var res = JSON.parse(response.result);
            //console.log(res);
            renderRequestListTable(res);
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}

