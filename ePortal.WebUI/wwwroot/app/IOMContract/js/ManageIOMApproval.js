$('.homeContent').removeClass('homeContent');
$(document).ready(function () {
    
    $("#history").on("click", function (e) {
        window.location.href = "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApprovalHistory.aspx?id=20"; //Fixed URL
    });

    $('#grdApprovalDetails').on('click', '.linkedit', function (e) {
        e.preventDefault();

        const $td = $(this).closest('td');

        const iomidEncoded = $td.find('.hdiomid').val();
        const empDesignEncoded = $td.find('.hdempdesign').val();

        window.location.href = "/IOMContract/IOMApprovalForm?IOMID=" + iomidEncoded + "&DESIGN=" + empDesignEncoded;
    });
    try {
        $.fn.dataTable.ext.errMode = 'none';
        $('#grdApprovalDetails').DataTable({
            //dom: 't',
            searching: false,
            lengthChange: false,
            paging: true,
            pageLength: 10,
            ordering: false,
            autoWidth: false,
            info: false,
            stripeClasses: [],
            columnDefs: [
                { targets: '_all', defaultContent: '' }
            ]
        });

        $('#grdApprovalDetails').on('click', '.view_iom_details', function (e) {
            e.preventDefault();
            var iomId = $(this).data('iomid');
            var url = '/iomcontract/ViewIOMDetail?IOMID=' + iomId;
            openPopup(url, 820, 590);
        });
    } catch { }

});


function openPopup(url, width, height) {
    var left = (screen.width / 2) - (width / 2);
    var top = (screen.height / 2) - (height / 2);
    window.open(url, '_blank', 'width=' + width + ',height=' + height + ',top=' + top + ',left=' + left + ',resizable=yes,scrollbars=yes');
}
