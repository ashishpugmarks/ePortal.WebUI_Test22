
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
$(function () {
    $("._date").datepicker({
        format: "dd-M-yyyy",
        autoclose: true,
        toolbarplacement: 'top',
        todayHighlight: true
    });

    $('#clearDateFrom').on('click', function () {
        $('#txtdatefrom').val('');
    });

    $('#clearDateTo').on('click', function () {
        $('#txtdateto').val('');
    });
});

function loadVendorRequests() {
   
    var filter = {
        RequestType: $('#ddlrequesttype').val(),
        RequestCategory: $('#ddlcategory').val(),
        ReqDateFrom: $('#txtdatefrom').val(),
        ReqDateTo: $('#txtdateto').val(),
        VendorCode: $('#txtvendorcode').val(),
        Status: $('#ddlstatus').val()
    };

    $.ajax({
        url: '/VendorMaster/GetVendorBlockRequest',
        type: 'POST',
        data: filter,
        success: function (response) {
           
            bindTable(response);
            //initializeTables();
        },
        error: function () {
            alert('Error loading vendor requests');
        }
    });
}

function bindTable(items) {
    
    if ($.fn.DataTable.isDataTable($('#vendorBlockRequestList'))) {
        $('#vendorBlockRequestList').DataTable().destroy();
    }
    
    var $tbody = $('#vendorBlockRequestList tbody');
    $tbody.empty();

    if (!items || items.length === 0) {
        $tbody.append('<tr><td colspan="7" class="text-center text-muted">No Data Available</td></tr>');
        return;
    }
    $.each(items, function (i, item) {
        var row = `
        <tr>
            <td class="text-center">
                <a href="javascript:void(0);" 
                   class="viewVendor" 
                   data-id="${item.EncVendorHeaderId}" 
                   title="View Vendor Detail">
                    <i class="fa fa-eye text-primary"></i>
                </a>
            </td>
            <td>${item.REQUESTTYPEDESC}</td>
            <td>${item.REQUESTDATE}</td>
            <td>${item.REQUESTCATE}</td>
            <td>${item.STATUSDESCRIPTION}</td>
            <td class="text-center">
                ${item.PROCESSSTATUS === 0
            ?
            //`<a href="/VendorMaster/EditVendorMasterForm/${item.EncryptedVendorHeaderId}" title="Edit Vendor Master Request">
            //               <i class="fa fa-edit text-warning"></i>
            //           </a>`
            `<a href="/VendorMaster/EditVendorBlockForm?VMID=${item.EncVendorHeaderId}&returnUrl=${encodeURIComponent(window.location.pathname)}" 
                       title="Edit Vendor Block Request">
                       <i class="fa fa-edit text-warning"></i>
                   </a>`
                : ''}
            </td>
        </tr>`;
        $tbody.append(row);
    });
}

function initializeTables() {
    
    var $summary = $('#vendorRequestList');
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
    initializeTables();
    loadVendorRequests();

    // Search button click
    $('#btnSearch').on('click', function () {
        loadVendorRequests();
    });

    $(document).on("click", ".viewVendor", function () {
        
        var vendorId = $(this).data("id");

        var url = '/VendorMaster/ViewVendorBlock?VMID=' + vendorId;

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