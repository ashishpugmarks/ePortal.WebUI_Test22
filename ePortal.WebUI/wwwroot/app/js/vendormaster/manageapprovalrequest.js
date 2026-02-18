window.addEventListener('DOMContentLoaded', () => {
    for (let sheet of document.styleSheets) {
        try {
            for (let i = 0; i < sheet.cssRules.length; i++) {
                let rule = sheet.cssRules[i];
                if (rule.selectorText === '.homeContent div') {
                    rule.style.removeProperty('padding');
                }
                if (rule.selectorText === '.homeContent') {
                    rule.style.removeProperty('padding');
                }
            }
        } catch (e) {
            console.warn("Could not access stylesheet:", e);
        }
    }
});

function loadVendorRequests() {
    var filter = {
        requestType: $('#ddlrequesttype').val(),
        vendorAccGroup: $('#ddlvendoraccgrp').val(),
        vendorName: $('#txtvendorname').val(),
        status: $('#ddlstatus').val()
    };

    $.ajax({
        url: '/VendorMaster/GetApprovalRequest',
        type: 'POST',
        data: filter,
        success: function (response) {
            //debugger;
            bindTable(response);
            //initializeTables();
        },
        error: function () {
            alert('Error loading vendor requests');
        }
    });
}

function bindTable(items) {
    if ($.fn.DataTable.isDataTable($('#employeeRequestList'))) {
        $('#employeeRequestList').DataTable().destroy();
    }

    var $tbody = $('#employeeRequestList tbody');
    $tbody.empty();

    if (!items || items.length === 0) {
        $tbody.append('<tr><td colspan="7" class="text-center text-muted">No Data Available</td></tr>');
    } else {
        $.each(items, function (i, item) {
            var row = `
            <tr>
                <td class="text-center">
                    <a href="javascript:void(0);" 
                       class="viewVendor" 
                       data-id="${item.EncryptedVendorHeaderId}" 
                       title="View Vendor Detail">
                        <i class="fa fa-eye text-primary"></i>
                    </a>
                </td>
                <td>${item.EMPLOYEE }</td>
                <td>${item.REQUESTTYPEDESC ?? ''}</td>
                <td>${item.REQUESTDATE ?? ''}</td>
                <td>${item.VENDORACCOUNTGRP ?? ''}</td>
                <td>${item.VENDORNAME ?? ''}</td>
                <td>${item.HISTORYSTATUS ?? ''}</td>
                <td class="text-center">
                    ${item.PSTATUSID === 2 || item.PSTATUSID === 15
                ? `<a href="/VendorMaster/VendorApprovalForm?VMID=${item.EncryptedVendorHeaderId}" title="Approval Request">
                               <i class="fa fa-edit text-warning"></i>
                           </a>`
                    : ''}
                </td>
            </tr>`;
            $tbody.append(row);
        });
    }
}

function initializeTables() {
    //debugger;
    var $summary = $('#employeeRequestList');
    if ($.fn.DataTable.isDataTable($summary)) {
        $summary.DataTable().destroy();
    }
    $summary.DataTable({
        pageLength: 10,
        lengthChange: true,
        bFilter: true
    });
}

$(function () {
    initializeTables()
    loadVendorRequests();

    // Search button click
    $('#btnSearch').on('click', function () {
        loadVendorRequests();
    });

    $(document).on("click", ".viewVendor", function () {
        debugger;
        var vendorId = $(this).data("id");

        var url = '/VendorMaster/ViewVendorMaster?VMID=' + vendorId;

        window.open(
            url,
            'View',
            'scrollbars=0,center=yes,resizable=yes,STATUS=no,Width=840px,Height=600px,top=80px,left=140px'
        );
    });

    $("#btnCancel").on("click", function () {
        window.location.href = window.location.origin + '/TokenBridge/RedirectToOldApp?target=/Home/Home';
    });

    $('#btnbackimg, #btnbackspan').on('click', function () {
        window.location.href = window.location.origin + "/TokenBridge/RedirectToOldApp?target=/Home/Home";
    });

});